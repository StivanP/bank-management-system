using API.DTOs.RequestDTOs.Accounts;
using API.DTOs.ResponseDTOs.Accounts;
using Common.Entities;
using Common.Services;
using Common.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Employee,Manager")]
    public class AccountsController
           : BaseCrudController<Account, AccountRequest, AccountGetRequest, AccountGetResponse, AccountService>
    {
        private bool TryGetLoggedUserId(out int userId)
        {
            userId = 0;
            var raw = User.FindFirst("loggedUserId")?.Value;
            return int.TryParse(raw, out userId);
        }

        private bool HasAccountPermission(int accountId, bool requireWrite)
        {
            if (!User.IsInRole("Employee"))
                return true; 

            if (!TryGetLoggedUserId(out var employeeId))
                return false;

            using var db = new BankDbContext();

            return db.EmployeeAccountPermissions.Any(p =>
                p.EmployeeId == employeeId &&
                p.AccountId == accountId &&
                (requireWrite
                    ? (p.Permission != null && p.Permission.ToUpper() == "WRITE")
                    : (p.Permission != null && (p.Permission.ToUpper() == "READ" || p.Permission.ToUpper() == "WRITE"))));
        }

        [HttpGet("{id:int}")]
        public override IActionResult GetById(int id)
        {
            if (!HasAccountPermission(id, requireWrite: false))
                return Forbid();

            return base.GetById(id);
        }

        [HttpPut("{id:int}")]
        public override IActionResult Update(int id, [FromBody] AccountRequest model)
        {
            if (!HasAccountPermission(id, requireWrite: true))
                return Forbid();

            return base.Update(id, model);
        }

        [HttpDelete("{id:int}")]
        public override IActionResult Delete(int id)
        {
            if (!HasAccountPermission(id, requireWrite: true))
                return Forbid();

            return base.Delete(id);
        }


        protected override void MapToEntity(AccountRequest model, Account entity)
        {
            entity.BranchId = model.BranchId;
            entity.Iban = (model.Iban ?? string.Empty).Trim();
            entity.AccountType = (model.AccountType ?? string.Empty).Trim();
            entity.Balance = model.Balance;
            entity.Status = (model.Status ?? string.Empty).Trim();

            if (entity.CreatedAt == default)
                entity.CreatedAt = DateTime.UtcNow;
        }

        protected override Expression<Func<Account, bool>>? BuildFilter(AccountGetRequest request)
        {
            var isEmployee = User.IsInRole("Employee");
            var hasEmployeeId = TryGetLoggedUserId(out var employeeId);

            Expression<Func<Account, bool>> perm = x =>
                !isEmployee ||
                (hasEmployeeId && x.EmployeeAccountPermissions.Any(p =>
                    p.EmployeeId == employeeId &&
                    p.Permission != null &&
                    (p.Permission.ToUpper() == "READ" || p.Permission.ToUpper() == "WRITE")));

            var f = request.Filter;
            if (f == null)
            {

                return perm;
            }

            return x =>
                (!f.BranchId.HasValue || x.BranchId == f.BranchId.Value) &&
                (string.IsNullOrWhiteSpace(f.Iban) || x.Iban.Contains(f.Iban)) &&
                (string.IsNullOrWhiteSpace(f.AccountType) || x.AccountType.Contains(f.AccountType)) &&
                (string.IsNullOrWhiteSpace(f.Status) || x.Status.Contains(f.Status)) &&
                (!f.MinBalance.HasValue || x.Balance >= f.MinBalance.Value) &&
                (!f.MaxBalance.HasValue || x.Balance <= f.MaxBalance.Value) &&
                (!f.CreatedFrom.HasValue || x.CreatedAt >= f.CreatedFrom.Value) &&
                (!f.CreatedTo.HasValue || x.CreatedAt <= f.CreatedTo.Value)
                && (!isEmployee || (hasEmployeeId && x.EmployeeAccountPermissions.Any(p =>
                       p.EmployeeId == employeeId &&
                       p.Permission != null &&
                       (p.Permission.ToUpper() == "READ" || p.Permission.ToUpper() == "WRITE")))); ; 
        }
    }
}

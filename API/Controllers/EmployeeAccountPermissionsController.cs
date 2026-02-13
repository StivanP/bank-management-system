using API.DTOs.RequestDTOs.EmployeeAccountPermissions;
using API.DTOs.ResponseDTOs.EmployeeAccountPermissions;
using API.Services;
using Common;
using Common.Entities;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager")]
    public class EmployeeAccountPermissionsController : ControllerBase
    {
        private static string Norm(string? p) => (p ?? string.Empty).Trim().ToUpperInvariant();
        private static bool Valid(string p) => p is "READ" or "WRITE";

        [HttpPost("get")]
        public IActionResult Get([FromBody] EmployeeAccountPermissionGetRequest? model)
        {
            model ??= new EmployeeAccountPermissionGetRequest();
            model.Filter ??= new EmployeeAccountPermissionGetFilterRequest();
            var f = model.Filter;

            string? perm = null;
            if (!string.IsNullOrWhiteSpace(f.Permission))
            {
                perm = Norm(f.Permission);
                if (!Valid(perm))
                    return Error(400, "Permission", "Permission must be READ or WRITE.");
            }

            int? employeeId = f.EmployeeId;
            int? accountId = f.AccountId;
            DateTime? from = f.GrantedFrom;
            DateTime? to = f.GrantedTo;

            Expression<Func<EmployeeAccountPermission, bool>> filter = x =>
                (!employeeId.HasValue || x.EmployeeId == employeeId.Value) &&
                (!accountId.HasValue || x.AccountId == accountId.Value) &&
                (perm == null || x.Permission == perm) &&
                (!from.HasValue || x.GrantedAt >= from.Value) &&
                (!to.HasValue || x.GrantedAt <= to.Value);

            var page = model.Pager?.Page > 0 ? model.Pager.Page : 1;
            var pageSize = model.Pager?.PageSize > 0 ? model.Pager.PageSize : int.MaxValue;

            var service = new EmployeeAccountPermissionService();

            var count = service.Count(filter);
            var items = service.GetAll(
                filter: filter,
                orderBy: string.IsNullOrWhiteSpace(model.OrderBy) ? null : model.OrderBy,
                sortAsc: model.SortAsc,
                page: page,
                pageSize: pageSize
            );

            return Ok(new EmployeeAccountPermissionGetResponse
            {
                Items = items,
                Filter = model.Filter,
                OrderBy = model.OrderBy,
                SortAsc = model.SortAsc,
                Pager = new() { Page = page, PageSize = pageSize, Count = count }
            });
        }

        [HttpPost]
        public IActionResult Create([FromBody] EmployeeAccountPermissionRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));

            var perm = Norm(model.Permission);
            if (!Valid(perm))
                return Error(400, "Permission", "Permission must be READ or WRITE.");

            var service = new EmployeeAccountPermissionService();

            if (service.Exists(model.EmployeeId, model.AccountId))
                return Error(409, "Global", "Permission record already exists for this employee and account.");

            var entity = new EmployeeAccountPermission
            {
                EmployeeId = model.EmployeeId,
                AccountId = model.AccountId,
                Permission = perm,
                GrantedAt = model.GrantedAt ?? DateTime.Now
            };

            service.Create(entity);
            return Ok(entity);
        }

        [HttpPut]
        public IActionResult Update([FromBody] EmployeeAccountPermissionRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));

            var perm = Norm(model.Permission);
            if (!Valid(perm))
                return Error(400, "Permission", "Permission must be READ or WRITE.");

            var service = new EmployeeAccountPermissionService();

            var entity = service.GetByIds(model.EmployeeId, model.AccountId);
            if (entity == null) return Error(404, "Global", "Permission record not found.");

            entity.Permission = perm;
            if (model.GrantedAt.HasValue) entity.GrantedAt = model.GrantedAt.Value;

            service.Update(entity);
            return Ok(entity);
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] int employeeId, [FromQuery] int accountId)
        {
            var service = new EmployeeAccountPermissionService();

            if (!service.Exists(employeeId, accountId))
                return Error(404, "Global", "Permission record not found.");

            service.DeleteByIds(employeeId, accountId);
            return Ok();
        }

        private IActionResult Error(int status, string key, string msg)
        {
            ModelState.AddModelError(key, msg);
            return StatusCode(status, ServiceResultExtentions<List<Error>>.Failure(null, ModelState));
        }
    }
}

using API.DTOs.RequestDTOs.CustomerAccounts;
using API.DTOs.ResponseDTOs.CustomerAccounts;
using API.Services;
using Common;
using Common.Entities;
using Common.Persistence;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Employee,Manager,Customer")]
    public class CustomerAccountsController : ControllerBase
    {
        [HttpPost("get")]
        public IActionResult Get([FromBody] CustomerAccountGetRequest? model)
        {
            model ??= new CustomerAccountGetRequest();
            model.Filter ??= new CustomerAccountGetFilterRequest();

            if (User.IsInRole("Customer"))
            {
                var id = GetLoggedUserId();
                if (id == null) return Forbid();

                if (model.Filter.CustomerId.HasValue && model.Filter.CustomerId.Value != id.Value)
                    return Forbid();

                model.Filter.CustomerId = id.Value;
            }

            var f = model.Filter;

            int? customerId = f.CustomerId;
            int? accountId = f.AccountId;
            string role = f.Role;
            DateTime? sinceFrom = f.SinceFrom;
            DateTime? sinceTo = f.SinceTo;

            Expression<Func<CustomerAccount, bool>> filter = x =>
                (!customerId.HasValue || x.CustomerId == customerId.Value) &&
                (!accountId.HasValue || x.AccountId == accountId.Value) &&
                (string.IsNullOrWhiteSpace(role) || x.Role.Contains(role)) &&
                (!sinceFrom.HasValue || x.SinceDate >= sinceFrom.Value) &&
                (!sinceTo.HasValue || x.SinceDate <= sinceTo.Value);

            var page = model.Pager?.Page > 0 ? model.Pager.Page : 1;
            var pageSize = model.Pager?.PageSize > 0 ? model.Pager.PageSize : int.MaxValue;

            var service = new CustomerAccountService();

            var count = service.Count(filter);

            var items = service.GetAll(
                filter: filter,
                orderBy: string.IsNullOrWhiteSpace(model.OrderBy) ? null : model.OrderBy,
                sortAsc: model.SortAsc,
                page: page,
                pageSize: pageSize
            );

            return Ok(new CustomerAccountGetResponse
            {
                Items = items,
                Filter = model.Filter,
                OrderBy = model.OrderBy,
                SortAsc = model.SortAsc,
                Pager = new() { Page = page, PageSize = pageSize, Count = count }
            });
        }

        [HttpPost]
        [Authorize(Roles = "Employee,Manager")]
        public IActionResult Create([FromBody] CustomerAccountRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));

            var service = new CustomerAccountService();

            if (service.Exists(model.CustomerId, model.AccountId))
                return Error(409, "Global", "Customer already has access to this account.");

            var entity = new CustomerAccount
            {
                CustomerId = model.CustomerId,
                AccountId = model.AccountId,
                Role = (model.Role ?? string.Empty).Trim(),
                SinceDate = model.SinceDate ?? DateTime.Now
            };

            service.Create(entity);
            return Ok(entity);
        }

        [HttpPut]
        [Authorize(Roles = "Employee,Manager")]
        public IActionResult Update([FromBody] CustomerAccountRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));

            var service = new CustomerAccountService();

            var entity = service.GetByIds(model.CustomerId, model.AccountId);
            if (entity == null) return Error(404, "Global", "Access record not found.");

            entity.Role = (model.Role ?? string.Empty).Trim();
            if (model.SinceDate.HasValue) entity.SinceDate = model.SinceDate.Value;

            service.Update(entity);
            return Ok(entity);
        }

        [HttpDelete]
        [Authorize(Roles = "Employee,Manager")]
        public IActionResult Delete([FromQuery] int customerId, [FromQuery] int accountId)
        {
            var service = new CustomerAccountService();

            if (!service.Exists(customerId, accountId))
                return Error(404, "Global", "Access record not found.");

            service.DeleteByIds(customerId, accountId);
            return Ok();
        }

        private int? GetLoggedUserId()
        {
            var raw = User.FindFirstValue("loggedUserId");
            return int.TryParse(raw, out var id) ? id : null;
        }

        private IActionResult Error(int status, string key, string msg)
        {
            ModelState.AddModelError(key, msg);
            return StatusCode(status, ServiceResultExtentions<List<Error>>.Failure(null, ModelState));
        }
    }
}

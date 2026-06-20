using API.DTOs.RequestDTOs.EmployeeBranches;
using API.DTOs.ResponseDTOs.EmployeeBranches;
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
    public class EmployeeBranchesController : ControllerBase
    {
        private readonly EmployeeBranchService _service;

        public EmployeeBranchesController(EmployeeBranchService service)
        {
            _service = service;
        }

        [HttpPost("get")]
        public IActionResult Get([FromBody] EmployeeBranchGetRequest? model)
        {
            model ??= new EmployeeBranchGetRequest();
            var f = model.Filter;

            int? employeeId = f?.EmployeeId;
            int? branchId = f?.BranchId;
            string? position = f?.Position;
            DateTime? startFrom = f?.StartFrom;
            DateTime? startTo = f?.StartTo;

            Expression<Func<EmployeeBranch, bool>> filter = x =>
                (!employeeId.HasValue || x.EmployeeId == employeeId.Value) &&
                (!branchId.HasValue || x.BranchId == branchId.Value) &&
                (string.IsNullOrWhiteSpace(position) || x.Position.Contains(position)) &&
                (!startFrom.HasValue || x.StartDate >= startFrom.Value) &&
                (!startTo.HasValue || x.StartDate <= startTo.Value);

            var page = model.Pager?.Page > 0 ? model.Pager.Page : 1;
            var pageSize = model.Pager?.PageSize > 0 ? model.Pager.PageSize : int.MaxValue;

            return Ok(new EmployeeBranchGetResponse
            {
                Items = _service.GetAll(filter, string.IsNullOrWhiteSpace(model.OrderBy) ? null : model.OrderBy, model.SortAsc, page, pageSize),
                Filter = model.Filter,
                OrderBy = model.OrderBy,
                SortAsc = model.SortAsc,
                Pager = new() { Page = page, PageSize = pageSize, Count = _service.Count(filter) }
            });
        }

        [HttpPost]
        public IActionResult Create([FromBody] EmployeeBranchRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));

            if (_service.Exists(model.EmployeeId, model.BranchId))
                return Error(409, "Global", "Employee is already assigned to this branch.");

            var entity = new EmployeeBranch
            {
                EmployeeId = model.EmployeeId,
                BranchId = model.BranchId,
                Position = (model.Position ?? string.Empty).Trim(),
                StartDate = model.StartDate ?? DateTime.Now
            };

            _service.Create(entity);
            return Ok(entity);
        }

        [HttpPut]
        public IActionResult Update([FromBody] EmployeeBranchRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));

            var entity = _service.GetByIds(model.EmployeeId, model.BranchId);
            if (entity == null) return Error(404, "Global", "Assignment not found.");

            entity.Position = (model.Position ?? string.Empty).Trim();
            if (model.StartDate.HasValue) entity.StartDate = model.StartDate.Value;

            _service.Update(entity);
            return Ok(entity);
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] int employeeId, [FromQuery] int branchId)
        {
            if (!_service.Exists(employeeId, branchId))
                return Error(404, "Global", "Assignment not found.");

            _service.DeleteByIds(employeeId, branchId);
            return Ok();
        }

        private IActionResult Error(int status, string key, string msg)
        {
            ModelState.AddModelError(key, msg);
            var payload = ServiceResultExtentions<List<Error>>.Failure(null, ModelState);
            return status switch
            {
                404 => NotFound(payload),
                409 => Conflict(payload),
                _ => StatusCode(status, payload)
            };
        }
    }
}
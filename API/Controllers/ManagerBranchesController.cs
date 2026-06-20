using API.DTOs.RequestDTOs.ManagerBranches;
using API.DTOs.ResponseDTOs.ManagerBranches;
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
    public class ManagerBranchesController : ControllerBase
    {
        private readonly ManagerBranchService _service;

        public ManagerBranchesController(ManagerBranchService service)
        {
            _service = service;
        }

        [HttpPost("get")]
        public IActionResult Get([FromBody] ManagerBranchGetRequest? model)
        {
            model ??= new ManagerBranchGetRequest();
            var f = model.Filter;

            int? managerId = f?.ManagerId;
            int? branchId = f?.BranchId;
            DateTime? startFrom = f?.StartFrom;
            DateTime? startTo = f?.StartTo;

            Expression<Func<ManagerBranch, bool>> filter = x =>
                (!managerId.HasValue || x.ManagerId == managerId.Value) &&
                (!branchId.HasValue || x.BranchId == branchId.Value) &&
                (!startFrom.HasValue || x.StartDate >= startFrom.Value) &&
                (!startTo.HasValue || x.StartDate <= startTo.Value);

            var page = model.Pager?.Page > 0 ? model.Pager.Page : 1;
            var pageSize = model.Pager?.PageSize > 0 ? model.Pager.PageSize : int.MaxValue;

            return Ok(new ManagerBranchGetResponse
            {
                Items = _service.GetAll(filter, string.IsNullOrWhiteSpace(model.OrderBy) ? null : model.OrderBy, model.SortAsc, page, pageSize),
                Filter = model.Filter,
                OrderBy = model.OrderBy,
                SortAsc = model.SortAsc,
                Pager = new() { Page = page, PageSize = pageSize, Count = _service.Count(filter) }
            });
        }

        [HttpPost]
        public IActionResult Create([FromBody] ManagerBranchRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));

            if (_service.Exists(model.ManagerId, model.BranchId))
                return Error(409, "Global", "Manager is already assigned to this branch.");

            var entity = new ManagerBranch
            {
                ManagerId = model.ManagerId,
                BranchId = model.BranchId,
                StartDate = model.StartDate ?? DateTime.Now
            };

            _service.Create(entity);
            return Ok(entity);
        }

        [HttpPut]
        public IActionResult Update([FromBody] ManagerBranchRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));

            var entity = _service.GetByIds(model.ManagerId, model.BranchId);
            if (entity == null) return Error(404, "Global", "Assignment not found.");

            if (model.StartDate.HasValue) entity.StartDate = model.StartDate.Value;

            _service.Update(entity);
            return Ok(entity);
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] int managerId, [FromQuery] int branchId)
        {
            if (!_service.Exists(managerId, branchId))
                return Error(404, "Global", "Assignment not found.");

            _service.DeleteByIds(managerId, branchId);
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
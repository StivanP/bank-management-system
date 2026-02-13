using API.DTOs.RequestDTOs.Employees;
using API.DTOs.ResponseDTOs.Employees;
using Common.Entities;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager,Employee")]
    public class EmployeesController
        : BaseCrudController<Employee, EmployeeRequest, EmployeeGetRequest, EmployeeGetResponse, EmployeeService>
    {
        private bool TryGetLoggedUserId(out int userId)
        {
            userId = 0;
            var raw = User.FindFirst("loggedUserId")?.Value;
            return int.TryParse(raw, out userId);
        }

        private static Employee Sanitize(Employee e) => new Employee
        {
            EmployeeId = e.EmployeeId,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Address = e.Address,
            Password = string.Empty, 
            EmployeeBranches = new List<EmployeeBranch>(),
            EmployeeAccountPermissions = new List<EmployeeAccountPermission>()
        };

        [HttpGet("{id:int}")]
        public override IActionResult GetById(int id)
        {
            if (User.IsInRole("Employee"))
            {
                if (!TryGetLoggedUserId(out var myId) || myId != id)
                    return Forbid();
            }

            var result = base.GetById(id);

            if (result is OkObjectResult ok && ok.Value is Employee entity)
                return Ok(Sanitize(entity));

            return result;
        }

        [HttpPost("get")]
        public override IActionResult Get([FromBody] EmployeeGetRequest? model)
        {
            model ??= new EmployeeGetRequest();
            model.Filter ??= new EmployeeGetFilterRequest();

            if (User.IsInRole("Employee"))
            {
                if (!TryGetLoggedUserId(out var myId))
                    return Forbid();

                model.Filter.EmployeeId = myId; 
            }

            var result = base.Get(model);

            if (result is OkObjectResult ok && ok.Value is EmployeeGetResponse response && response.Items != null)
            {
                response.Items = response.Items.Select(Sanitize).ToList();
                return Ok(response);
            }

            return result;
        }

        protected override void MapToEntity(EmployeeRequest model, Employee entity)
        {
            entity.FirstName = (model.FirstName ?? string.Empty).Trim();
            entity.LastName = (model.LastName ?? string.Empty).Trim();
            entity.Email = (model.Email ?? string.Empty).Trim().ToLowerInvariant();
            entity.Password = model.Password ?? string.Empty;
            entity.Address = (model.Address ?? string.Empty).Trim();
        }

        protected override Expression<Func<Employee, bool>>? BuildFilter(EmployeeGetRequest request)
        {
            var f = request.Filter;
            if (f == null) return null;

            return x =>
                (!f.EmployeeId.HasValue || x.EmployeeId == f.EmployeeId.Value) &&
                (string.IsNullOrWhiteSpace(f.FirstName) || x.FirstName.Contains(f.FirstName)) &&
                (string.IsNullOrWhiteSpace(f.LastName) || x.LastName.Contains(f.LastName)) &&
                (string.IsNullOrWhiteSpace(f.Email) || x.Email.Contains(f.Email));
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public override IActionResult Create([FromBody] EmployeeRequest model) => base.Create(model);

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Manager")]
        public override IActionResult Update(int id, [FromBody] EmployeeRequest model) => base.Update(id, model);

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Manager")]
        public override IActionResult Delete(int id) => base.Delete(id);
    }
}

using API.DTOs.RequestDTOs.Customers;
using API.DTOs.ResponseDTOs.Customers;
using Common.Entities;
using Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Employee,Manager,Customer")]
    public class CustomersController
        : BaseCrudController<Customer, CustomerRequest, CustomerGetRequest, CustomerGetResponse, CustomerService>
    {
        private bool TryGetLoggedUserId(out int userId)
        {
            userId = 0;
            var raw = User.FindFirst("loggedUserId")?.Value;
            return int.TryParse(raw, out userId);
        }

        private static Customer Sanitize(Customer c) => new Customer
        {
            CustomerId = c.CustomerId,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Email = c.Email,
            Address = c.Address,
            Password = string.Empty,              
            CustomerAccounts = new List<CustomerAccount>()
        };

        [HttpGet("{id:int}")]
        public override IActionResult GetById(int id)
        {
            if (User.IsInRole("Customer"))
            {
                if (!TryGetLoggedUserId(out var myId) || myId != id)
                    return Forbid();
            }

            var result = base.GetById(id);

            if (result is OkObjectResult ok && ok.Value is Customer entity)
                return Ok(Sanitize(entity));

            return result;
        }

        [HttpPost("get")]
        public override IActionResult Get([FromBody] CustomerGetRequest? model)
        {
            model ??= new CustomerGetRequest();
            model.Filter ??= new CustomerGetFilterRequest();

            if (User.IsInRole("Customer"))
            {
                if (!TryGetLoggedUserId(out var myId))
                    return Forbid();

                model.Filter.CustomerId = myId; 
            }

            var result = base.Get(model);

            if (result is OkObjectResult ok && ok.Value is CustomerGetResponse response && response.Items != null)
            {
                response.Items = response.Items.Select(Sanitize).ToList();
                return Ok(response);
            }

            return result;
        }

        protected override void MapToEntity(CustomerRequest model, Customer entity)
        {
            entity.FirstName = (model.FirstName ?? string.Empty).Trim();
            entity.LastName = (model.LastName ?? string.Empty).Trim();
            entity.Email = (model.Email ?? string.Empty).Trim().ToLowerInvariant();
            entity.Password = model.Password ?? string.Empty;
            entity.Address = (model.Address ?? string.Empty).Trim();
        }

        protected override Expression<Func<Customer, bool>>? BuildFilter(CustomerGetRequest request)
        {
            var f = request.Filter;
            if (f == null) return null;

            return x =>
                (!f.CustomerId.HasValue || x.CustomerId == f.CustomerId.Value) &&
                (string.IsNullOrWhiteSpace(f.FirstName) || x.FirstName.Contains(f.FirstName)) &&
                (string.IsNullOrWhiteSpace(f.LastName) || x.LastName.Contains(f.LastName)) &&
                (string.IsNullOrWhiteSpace(f.Email) || x.Email.Contains(f.Email));
        }

        [HttpPost]
        [Authorize(Roles = "Employee,Manager")]
        public override IActionResult Create([FromBody] CustomerRequest model) => base.Create(model);

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Employee,Manager")]
        public override IActionResult Update(int id, [FromBody] CustomerRequest model) => base.Update(id, model);

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Employee,Manager")]
        public override IActionResult Delete(int id) => base.Delete(id);
    }
}

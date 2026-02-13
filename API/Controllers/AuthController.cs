using API.DTOs.RequestDTOs.Auth;
using API.DTOs.RequestDTOs.Customers;
using API.Services;
using Common;
using Common.Entities;
using Common.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthTokenRequest model) => CreateToken(model);

        [HttpPost("token")]
        public IActionResult CreateToken([FromBody] AuthTokenRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));

            var email = NormalizeEmail(model.Email);
            var password = model.Password ?? string.Empty;

            using var db = new BankDbContext();

            var manager = db.Managers.FirstOrDefault(x => x.Email.ToLower() == email && x.Password == password);
            if (manager != null) return Ok(new { token = new TokenService().CreateToken(manager) });

            var employee = db.Employees.FirstOrDefault(x => x.Email.ToLower() == email && x.Password == password);
            if (employee != null) return Ok(new { token = new TokenService().CreateToken(employee) });

            var customer = db.Customers.FirstOrDefault(x => x.Email.ToLower() == email && x.Password == password);
            if (customer != null) return Ok(new { token = new TokenService().CreateToken(customer) });

            ModelState.AddModelError("Global", "Invalid email or password.");
            return Unauthorized(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] CustomerRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));

            using var db = new BankDbContext();

            var email = NormalizeEmail(model.Email);

            if (EmailExists(db, email))
            {
                ModelState.AddModelError("Email", "Email is already used.");
                return Conflict(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));
            }

            var entity = new Customer
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = email,
                Password = model.Password,
                Address = model.Address
            };

            db.Customers.Add(entity);
            db.SaveChanges();

            return Ok(new { id = entity.CustomerId, role = "Customer" });
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("set-role")]
        public IActionResult SetRole([FromBody] SetRoleRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));

            var email = NormalizeEmail(model.Email);
            var role = (model.Role ?? string.Empty).Trim();

            using var db = new BankDbContext();

            if (email == "stivanp3@gmail.com" &&
                !role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
                return Forbid();

            var customer = db.Customers.FirstOrDefault(x => x.Email.ToLower() == email);
            var employee = db.Employees.FirstOrDefault(x => x.Email.ToLower() == email);
            var manager = db.Managers.FirstOrDefault(x => x.Email.ToLower() == email);

            PersonBase? src =
                     (PersonBase?)manager ??
                     (PersonBase?)employee ??
                     (PersonBase?)customer;
            if (src == null)
                return NotFound(new { message = "User not found." });

            static void Copy(PersonBase s, PersonBase d)
            {
                d.FirstName = s.FirstName;
                d.LastName = s.LastName;
                d.Email = s.Email;
                d.Password = s.Password;
                d.Address = s.Address;
            }

            if (role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
            {
                if (manager == null)
                {
                    var m = new Manager();
                    Copy(src, m);
                    db.Managers.Add(m);
                }

                if (employee != null) db.Employees.Remove(employee);

                try { db.SaveChanges(); }
                catch (DbUpdateException) { return Conflict(new { message = "Cannot change role due to related data." }); }

                return Ok(new { email, role = "Manager" });
            }

            if (role.Equals("Employee", StringComparison.OrdinalIgnoreCase))
            {
                if (employee == null)
                {
                    var e = new Employee();
                    Copy(src, e);
                    db.Employees.Add(e);
                }

                if (manager != null) db.Managers.Remove(manager);

                try { db.SaveChanges(); }
                catch (DbUpdateException) { return Conflict(new { message = "Cannot change role due to related data." }); }

                return Ok(new { email, role = "Employee" });
            }

            if (role.Equals("Customer", StringComparison.OrdinalIgnoreCase))
            {
                if (customer == null)
                {
                    var c = new Customer();
                    Copy(src, c);
                    db.Customers.Add(c);
                }

                if (manager != null) db.Managers.Remove(manager);
                if (employee != null) db.Employees.Remove(employee);

                try { db.SaveChanges(); }
                catch (DbUpdateException) { return Conflict(new { message = "Cannot demote due to related data." }); }

                return Ok(new { email, role = "Customer" });
            }

            ModelState.AddModelError("Role", "Role must be Customer, Employee or Manager.");
            return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));
        }

        private static string NormalizeEmail(string email)
            => (email ?? string.Empty).Trim().ToLowerInvariant();

        private static bool EmailExists(BankDbContext db, string email)
        {
            return db.Customers.Any(x => x.Email.ToLower() == email)
                || db.Employees.Any(x => x.Email.ToLower() == email)
                || db.Managers.Any(x => x.Email.ToLower() == email);
        }
    }
}

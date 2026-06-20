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
        private readonly BankDbContext _db;
        private readonly TokenService _tokenService;

        public AuthController(BankDbContext db, TokenService tokenService)
        {
            _db = db;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthTokenRequest model) => CreateToken(model);

        [HttpPost("token")]
        public IActionResult CreateToken([FromBody] AuthTokenRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));

            var email = NormalizeEmail(model.Email);
            var password = model.Password ?? string.Empty;

            var manager = _db.Managers.FirstOrDefault(x => x.Email == email);
            if (manager != null && BCrypt.Net.BCrypt.Verify(password, manager.Password))
                return Ok(new { token = _tokenService.CreateToken(manager) });

            var employee = _db.Employees.FirstOrDefault(x => x.Email == email);
            if (employee != null && BCrypt.Net.BCrypt.Verify(password, employee.Password))
                return Ok(new { token = _tokenService.CreateToken(employee) });

            var customer = _db.Customers.FirstOrDefault(x => x.Email == email);
            if (customer != null && BCrypt.Net.BCrypt.Verify(password, customer.Password))
                return Ok(new { token = _tokenService.CreateToken(customer) });

            ModelState.AddModelError("Global", "Invalid email or password.");
            return Unauthorized(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] CustomerRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));

            var email = NormalizeEmail(model.Email);

            if (EmailExists(email))
            {
                ModelState.AddModelError("Email", "Email is already used.");
                return Conflict(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));
            }

            var entity = new Customer
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Address = model.Address
            };

            _db.Customers.Add(entity);
            _db.SaveChanges();

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

            if (email == "stivanp3@gmail.com" &&
                !role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
                return Forbid();

            var customer = _db.Customers.FirstOrDefault(x => x.Email == email);
            var employee = _db.Employees.FirstOrDefault(x => x.Email == email);
            var manager = _db.Managers.FirstOrDefault(x => x.Email == email);

            PersonBase? src = (PersonBase?)manager ?? (PersonBase?)employee ?? (PersonBase?)customer;
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
                if (manager == null) { var m = new Manager(); Copy(src, m); _db.Managers.Add(m); }
                if (employee != null) _db.Employees.Remove(employee);

                try { _db.SaveChanges(); }
                catch (DbUpdateException) { return Conflict(new { message = "Cannot change role due to related data." }); }

                return Ok(new { email, role = "Manager" });
            }

            if (role.Equals("Employee", StringComparison.OrdinalIgnoreCase))
            {
                if (employee == null) { var e = new Employee(); Copy(src, e); _db.Employees.Add(e); }
                if (manager != null) _db.Managers.Remove(manager);

                try { _db.SaveChanges(); }
                catch (DbUpdateException) { return Conflict(new { message = "Cannot change role due to related data." }); }

                return Ok(new { email, role = "Employee" });
            }

            if (role.Equals("Customer", StringComparison.OrdinalIgnoreCase))
            {
                if (customer == null) { var c = new Customer(); Copy(src, c); _db.Customers.Add(c); }
                if (manager != null) _db.Managers.Remove(manager);
                if (employee != null) _db.Employees.Remove(employee);

                try { _db.SaveChanges(); }
                catch (DbUpdateException) { return Conflict(new { message = "Cannot demote due to related data." }); }

                return Ok(new { email, role = "Customer" });
            }

            ModelState.AddModelError("Role", "Role must be Customer, Employee or Manager.");
            return BadRequest(ServiceResultExtentions<List<Error>>.Failure(null, ModelState));
        }

        private static string NormalizeEmail(string email)
            => (email ?? string.Empty).Trim().ToLowerInvariant();

        private bool EmailExists(string email)
            => _db.Customers.Any(x => x.Email == email)
            || _db.Employees.Any(x => x.Email == email)
            || _db.Managers.Any(x => x.Email == email);
    }
}
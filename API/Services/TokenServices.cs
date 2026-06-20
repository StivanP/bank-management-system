using Common.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
    public class TokenService
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiresMinutes;

        public TokenService(IConfiguration configuration)
        {
            _key = configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key is not configured.");
            _issuer = configuration["Jwt:Issuer"] ?? "BankAPI";
            _audience = configuration["Jwt:Audience"] ?? "BankClients";
            _expiresMinutes = int.TryParse(configuration["Jwt:ExpiresMinutes"], out var m) ? m : 60;
        }

        public string CreateToken(Customer customer)
            => CreateTokenInternal(customer.CustomerId, "Customer", customer.Email);

        public string CreateToken(Employee employee)
            => CreateTokenInternal(employee.EmployeeId, "Employee", employee.Email);

        public string CreateToken(Manager manager)
            => CreateTokenInternal(manager.ManagerId, "Manager", manager.Email);

        private string CreateTokenInternal(int userId, string role, string email)
        {
            var claims = new[]
            {
                new Claim("loggedUserId", userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim("email", email)
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiresMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
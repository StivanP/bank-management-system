using Common.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
    public class TokenService
    {
        private const string Issuer = "BankAPI";
        private const string Audience = "postman";
        private const string KeyText = "!BushidoClub123!BushidoClub123!BushidoClub123";

        public string CreateToken(Customer customer)
        {
            return CreateTokenInternal(
                userId: customer.CustomerId,
                role: "Customer",
                email: customer.Email
            );
        }

        public string CreateToken(Employee employee)
        {
            return CreateTokenInternal(
                userId: employee.EmployeeId,
                role: "Employee",
                email: employee.Email
            );
        }

        public string CreateToken(Manager manager)
        {
            return CreateTokenInternal(
                userId: manager.ManagerId,
                role: "Manager",
                email: manager.Email
            );
        }

        private string CreateTokenInternal(int userId, string role, string email)
        {
            Claim[] claims = new Claim[]
            {
                new Claim("loggedUserId", userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim("email", email)

            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KeyText));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

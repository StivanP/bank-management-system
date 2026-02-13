namespace API.DTOs.RequestDTOs.Employees
{
    public class EmployeeRequest
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
    }
}

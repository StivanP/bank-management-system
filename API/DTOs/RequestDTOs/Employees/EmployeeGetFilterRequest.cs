namespace API.DTOs.RequestDTOs.Employees
{
    public class EmployeeGetFilterRequest
    {
        public int? EmployeeId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }
    }
}

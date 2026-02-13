namespace API.DTOs.RequestDTOs.Managers
{
    public class ManagerGetFilterRequest
    {
        public int? ManagerId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }
    }
}

namespace API.DTOs.RequestDTOs.Customers
{
    public class CustomerGetFilterRequest
    {
        public int? CustomerId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }
    }
}

namespace API.DTOs.RequestDTOs.CustomerAccounts
{
    public class CustomerAccountGetFilterRequest
    {
        public int? CustomerId { get; set; }
        public int? AccountId { get; set; }
        public string? Role { get; set; }

        public DateTime? SinceFrom { get; set; }
        public DateTime? SinceTo { get; set; }
    }
}

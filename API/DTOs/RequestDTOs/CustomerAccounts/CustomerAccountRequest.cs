namespace API.DTOs.RequestDTOs.CustomerAccounts
{
    public class CustomerAccountRequest
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public string Role { get; set; } = string.Empty; 
        public DateTime? SinceDate { get; set; }
    }
}

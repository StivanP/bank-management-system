namespace API.DTOs.RequestDTOs.Accounts
{
    public class AccountRequest
    {
        public int BranchId { get; set; }

        public string Iban { get; set; } = string.Empty;

        public string AccountType { get; set; } = string.Empty; 

        public decimal Balance { get; set; } 

        public string Status { get; set; } = "ACTIVE";
    }
}

namespace API.DTOs.RequestDTOs.Accounts
{
    public class AccountGetFilterRequest
    {
        public int? BranchId { get; set; }
        public string? Iban { get; set; }
        public string? AccountType { get; set; }  
        public string? Status { get; set; }      

        public decimal? MinBalance { get; set; }
        public decimal? MaxBalance { get; set; }

        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
    }
}

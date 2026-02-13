namespace API.DTOs.RequestDTOs.Branches
{
    public class BranchGetFilterRequest
    {
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? Location { get; set; }
    }
}

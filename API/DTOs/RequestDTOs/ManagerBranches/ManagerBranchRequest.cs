namespace API.DTOs.RequestDTOs.ManagerBranches
{
    public class ManagerBranchRequest
    {
        public int ManagerId { get; set; }
        public int BranchId { get; set; }
        public DateTime? StartDate { get; set; }
    }
}

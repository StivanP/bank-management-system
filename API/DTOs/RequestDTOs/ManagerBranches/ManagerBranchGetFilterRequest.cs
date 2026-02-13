namespace API.DTOs.RequestDTOs.ManagerBranches
{
    public class ManagerBranchGetFilterRequest
    {
        public int? ManagerId { get; set; }
        public int? BranchId { get; set; }

        public DateTime? StartFrom { get; set; }
        public DateTime? StartTo { get; set; }
    }
}

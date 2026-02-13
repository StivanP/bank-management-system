namespace API.DTOs.RequestDTOs.EmployeeBranches
{
    public class EmployeeBranchGetFilterRequest
    {
        public int? EmployeeId { get; set; }
        public int? BranchId { get; set; }

        public string? Position { get; set; }

        public DateTime? StartFrom { get; set; }
        public DateTime? StartTo { get; set; }
    }
}

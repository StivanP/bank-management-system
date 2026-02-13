namespace API.DTOs.RequestDTOs.EmployeeBranches
{
    public class EmployeeBranchRequest
    {
        public int EmployeeId { get; set; }
        public int BranchId { get; set; }

        public string Position { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }
    }
}

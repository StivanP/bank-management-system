namespace API.DTOs.RequestDTOs.EmployeeAccountPermissions
{
    public class EmployeeAccountPermissionGetFilterRequest
    {
        public int? EmployeeId { get; set; }
        public int? AccountId { get; set; }
        public string? Permission { get; set; }

        public DateTime? GrantedFrom { get; set; }
        public DateTime? GrantedTo { get; set; }
    }
}

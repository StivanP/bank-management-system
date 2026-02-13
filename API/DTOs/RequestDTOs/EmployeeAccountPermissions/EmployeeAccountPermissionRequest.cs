namespace API.DTOs.RequestDTOs.EmployeeAccountPermissions
{
    public class EmployeeAccountPermissionRequest
    {
        public int EmployeeId { get; set; }
        public int AccountId { get; set; }

        public string Permission { get; set; }
        public DateTime? GrantedAt { get; set; }
    }
}

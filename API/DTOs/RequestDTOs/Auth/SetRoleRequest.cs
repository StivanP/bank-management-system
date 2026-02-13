namespace API.DTOs.RequestDTOs.Auth
{
    public class SetRoleRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; 
    }
}

using API.DTOs.RequestDTOs.Accounts;
using API.DTOs.RequestDTOs.Shared;

namespace API.DTOs.RequestDTOs.EmployeeAccountPermissions
{
    public class EmployeeAccountPermissionGetRequest : BaseGetRequest
    {
        public EmployeeAccountPermissionGetFilterRequest Filter { get; set; }
}
}

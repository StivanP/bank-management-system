using API.DTOs.RequestDTOs.EmployeeAccountPermissions;
using API.DTOs.ResponseDTOs.Shared;
using Common.Entities;

namespace API.DTOs.ResponseDTOs.EmployeeAccountPermissions
{
    public class EmployeeAccountPermissionGetResponse : BaseGetResponse<EmployeeAccountPermission>
    {
        public EmployeeAccountPermissionGetFilterRequest Filter { get; set; }
    }
}

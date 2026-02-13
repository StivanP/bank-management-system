using API.DTOs.RequestDTOs.Shared;
using API.DTOs.ResponseDTOs.Shared;

namespace API.DTOs.RequestDTOs.EmployeeBranches
{
    public class EmployeeBranchGetRequest : BaseGetRequest
    {
        public EmployeeBranchGetFilterRequest Filter { get; set; }
    }
}

using API.DTOs.RequestDTOs.EmployeeBranches;
using API.DTOs.ResponseDTOs.Shared;
using Common.Entities;

namespace API.DTOs.ResponseDTOs.EmployeeBranches
{
    public class EmployeeBranchGetResponse : BaseGetResponse<EmployeeBranch>
    {
        public EmployeeBranchGetFilterRequest Filter { get; set; }
    }
}

using API.DTOs.RequestDTOs.ManagerBranches;
using API.DTOs.ResponseDTOs.Shared;
using Common.Entities;

namespace API.DTOs.ResponseDTOs.ManagerBranches
{
    public class ManagerBranchGetResponse : BaseGetResponse<ManagerBranch>
    {
        public ManagerBranchGetFilterRequest Filter { get; set; }
    }
}

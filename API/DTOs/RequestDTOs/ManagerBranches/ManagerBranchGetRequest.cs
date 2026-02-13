using API.DTOs.RequestDTOs.Shared;

namespace API.DTOs.RequestDTOs.ManagerBranches
{
    public class ManagerBranchGetRequest : BaseGetRequest
    {
        public ManagerBranchGetFilterRequest Filter { get; set; }
    }
}

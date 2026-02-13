using API.DTOs.RequestDTOs.Shared;

namespace API.DTOs.RequestDTOs.Branches
{
    public class BranchGetRequest : BaseGetRequest
    {
        public BranchGetFilterRequest Filter { get; set; }
    }
}

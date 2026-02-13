using API.DTOs.RequestDTOs.Branches;
using API.DTOs.ResponseDTOs.Shared;
using Common.Entities;

namespace API.DTOs.ResponseDTOs.Branches
{
    public class BranchGetResponse : BaseGetResponse<Branch>
    {
        public BranchGetFilterRequest Filter { get; set; }
    }
}

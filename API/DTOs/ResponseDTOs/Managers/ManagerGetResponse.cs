using API.DTOs.RequestDTOs.Managers;
using API.DTOs.ResponseDTOs.Shared;
using Common.Entities;

namespace API.DTOs.ResponseDTOs.Managers
{
    public class ManagerGetResponse : BaseGetResponse<Manager>
    {
        public ManagerGetFilterRequest Filter { get; set; }
    }
}

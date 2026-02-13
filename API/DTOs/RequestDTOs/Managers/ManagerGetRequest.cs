using API.DTOs.RequestDTOs.Shared;

namespace API.DTOs.RequestDTOs.Managers
{
    public class ManagerGetRequest : BaseGetRequest
    {
        public ManagerGetFilterRequest Filter { get; set; }
    }
}

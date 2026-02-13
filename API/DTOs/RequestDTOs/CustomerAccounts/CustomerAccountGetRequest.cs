using API.DTOs.RequestDTOs.Shared;
using API.DTOs.ResponseDTOs.Shared;

namespace API.DTOs.RequestDTOs.CustomerAccounts
{
    public class CustomerAccountGetRequest : BaseGetRequest
    {
        public CustomerAccountGetFilterRequest Filter { get; set; }
    }
}

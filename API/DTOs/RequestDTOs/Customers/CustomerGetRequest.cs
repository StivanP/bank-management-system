using API.DTOs.RequestDTOs.Accounts;
using API.DTOs.RequestDTOs.Shared;
using API.DTOs.ResponseDTOs.Shared;

namespace API.DTOs.RequestDTOs.Customers
{
    public class CustomerGetRequest : BaseGetRequest
    {
        public CustomerGetFilterRequest Filter { get; set; }

    }
}

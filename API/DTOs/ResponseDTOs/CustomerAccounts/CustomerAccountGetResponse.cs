using API.DTOs.RequestDTOs.CustomerAccounts;
using API.DTOs.ResponseDTOs.Shared;
using Common.Entities;

namespace API.DTOs.ResponseDTOs.CustomerAccounts
{
    public class CustomerAccountGetResponse : BaseGetResponse<CustomerAccount>
    {
        public CustomerAccountGetFilterRequest Filter { get; set; }
    }
}

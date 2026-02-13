using API.DTOs.RequestDTOs.Shared;

namespace API.DTOs.RequestDTOs.Accounts
{
    public class AccountGetRequest : BaseGetRequest
    {
        public AccountGetFilterRequest Filter {  get; set; }
    }
}

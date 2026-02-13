using API.DTOs.RequestDTOs.Accounts;
using API.DTOs.ResponseDTOs.Shared;
using Common.Entities;

namespace API.DTOs.ResponseDTOs.Accounts
{
    public class AccountGetResponse : BaseGetResponse<Account>
    {
        public AccountGetFilterRequest Filter { get; set; }
    }
}

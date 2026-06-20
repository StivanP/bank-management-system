using Common.Entities;
using Common.Persistence;

namespace Common.Services
{
    public class AccountService : BaseService<Account>
    {
        public AccountService(BankDbContext context) : base(context) { }
    }
}
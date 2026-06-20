using Common.Entities;
using Common.Persistence;

namespace Common.Services
{
    public class CustomerAccountService : BaseJunctionService<CustomerAccount>
    {
        public CustomerAccountService(BankDbContext context) : base(context) { }
    }
}
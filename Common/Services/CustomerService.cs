using Common.Entities;
using Common.Persistence;

namespace Common.Services
{
    public class CustomerService : BaseService<Customer>
    {
        public CustomerService(BankDbContext context) : base(context) { }
    }
}
using Common.Entities;
using Common.Persistence;

namespace Common.Services
{
    public class ManagerService : BaseService<Manager>
    {
        public ManagerService(BankDbContext context) : base(context) { }
    }
}
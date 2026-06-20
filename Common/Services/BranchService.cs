using Common.Entities;
using Common.Persistence;

namespace Common.Services
{
    public class BranchService : BaseService<Branch>
    {
        public BranchService(BankDbContext context) : base(context) { }
    }
}
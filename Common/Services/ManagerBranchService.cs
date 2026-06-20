using Common.Entities;
using Common.Persistence;

namespace Common.Services
{
    public class ManagerBranchService : BaseJunctionService<ManagerBranch>
    {
        public ManagerBranchService(BankDbContext context) : base(context) { }
    }
}
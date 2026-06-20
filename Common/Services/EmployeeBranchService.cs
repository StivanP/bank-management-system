using Common.Entities;
using Common.Persistence;

namespace Common.Services
{
    public class EmployeeBranchService : BaseJunctionService<EmployeeBranch>
    {
        public EmployeeBranchService(BankDbContext context) : base(context) { }
    }
}
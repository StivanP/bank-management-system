using Common.Entities;
using Common.Persistence;

namespace Common.Services
{
    public class EmployeeAccountPermissionService : BaseJunctionService<EmployeeAccountPermission>
    {
        public EmployeeAccountPermissionService(BankDbContext context) : base(context) { }
    }
}
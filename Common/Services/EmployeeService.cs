using Common.Entities;
using Common.Persistence;

namespace Common.Services
{
    public class EmployeeService : BaseService<Employee>
    {
        public EmployeeService(BankDbContext context) : base(context) { }
    }
}
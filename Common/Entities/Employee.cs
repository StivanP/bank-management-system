using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Entities
{
    public class Employee : PersonBase
    {
        [Key]
        public int EmployeeId { get; set; }

        public ICollection<EmployeeBranch> EmployeeBranches { get; set; } = new List<EmployeeBranch>();
        public ICollection<EmployeeAccountPermission> EmployeeAccountPermissions { get; set; } = new List<EmployeeAccountPermission>();
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Entities
{
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }

        [Required, MaxLength(60)]
        public string BranchName { get; set; } = null!;

        [Required, MaxLength(120)]
        public string Location { get; set; } = null!;

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public ICollection<EmployeeBranch> EmployeeBranches { get; set; } = new List<EmployeeBranch>();
        public ICollection<ManagerBranch> ManagerBranches { get; set; } = new List<ManagerBranch>();
    }
}

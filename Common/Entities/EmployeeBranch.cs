using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Entities
{
    public class EmployeeBranch
    {
        public int EmployeeId { get; set; }
        public int BranchId { get; set; }

        [Required, MaxLength(40)]
        public string Position { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public Employee Employee { get; set; } = null!;
        public Branch Branch { get; set; } = null!;
    }

}

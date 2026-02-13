using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Entities
{
    public class EmployeeAccountPermission
    {
        public int EmployeeId { get; set; }
        public int AccountId { get; set; }

        [Required, MaxLength(10)]
        public string Permission { get; set; } = null!;

        public DateTime GrantedAt { get; set; }

        public Employee Employee { get; set; } = null!;
        public Account Account { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Common.Entities
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required, MaxLength(34)]
        public string Iban { get; set; } = null!;

        [Required, MaxLength(20)]
        public string AccountType { get; set; } = null!;

        [Column(TypeName = "decimal(15,2)")]
        public decimal Balance { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required, MaxLength(12)]
        public string Status { get; set; } = null!; 

        public Branch Branch { get; set; } = null!;
        public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();
        public ICollection<EmployeeAccountPermission> EmployeeAccountPermissions { get; set; } = new List<EmployeeAccountPermission>();
    }
}

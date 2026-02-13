using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Entities
{
    public class CustomerAccount
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }

        [Required, MaxLength(10)]
        public string Role { get; set; } = null!; 

        public DateTime SinceDate { get; set; }

        public Customer Customer { get; set; } = null!;
        public Account Account { get; set; } = null!;
    }
}

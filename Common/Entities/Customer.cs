using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Entities
{
    public class Customer : PersonBase
    {

        [Key]
        public int CustomerId { get; set; }
        public ICollection<CustomerAccount> CustomerAccounts { get; set; } = new List<CustomerAccount>();
    }
}

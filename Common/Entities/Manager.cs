using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.Entities
{
    public class Manager : PersonBase
    {
        [Key]
        public int ManagerId { get; set; }
        public ICollection<ManagerBranch> ManagerBranches { get; set; } = new List<ManagerBranch>();
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Entities
{
    public class ManagerBranch
    {
        public int ManagerId { get; set; }
        public int BranchId { get; set; }

        public DateTime StartDate { get; set; }

        public Manager Manager { get; set; } = null!;
        public Branch Branch { get; set; } = null!;
    }

}

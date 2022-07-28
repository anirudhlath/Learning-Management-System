using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class AssignmentCategory
    {
        public AssignmentCategory()
        {
            Assignments = new HashSet<Assignment>();
        }

        public string CatalogId { get; set; } = null!;
        public string Semester { get; set; } = null!;
        public uint Section { get; set; }
        public uint GradingWeight { get; set; }
        public string Name { get; set; } = null!;
        public uint CategoryId { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}

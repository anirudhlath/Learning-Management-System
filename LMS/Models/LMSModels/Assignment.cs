using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignment
    {
        public string Name { get; set; } = null!;
        public uint MaxPoints { get; set; }
        public string? Content { get; set; }
        public DateTime? DueDateTime { get; set; }
        public uint CategoryId { get; set; }
        public uint AssignmentId { get; set; }

        public virtual AssignmentCategory Category { get; set; } = null!;
    }
}

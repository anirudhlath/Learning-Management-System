using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AssignmentCategories = new HashSet<AssignmentCategory>();
            Submissions = new HashSet<Submission>();
            UIds = new HashSet<Professor>();
        }

        public string CatalogId { get; set; } = null!;
        public uint Section { get; set; }
        public string Semester { get; set; } = null!;
        public string Location { get; set; } = null!;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public uint ClassId { get; set; }

        public virtual Course Catalog { get; set; } = null!;
        public virtual ICollection<AssignmentCategory> AssignmentCategories { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }

        public virtual ICollection<Professor> UIds { get; set; }
    }
}

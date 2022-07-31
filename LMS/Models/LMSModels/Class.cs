using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AssignmentCategories = new HashSet<AssignmentCategory>();
            Enrollments = new HashSet<Enrollment>();
            Submissions = new HashSet<Submission>();
        }

        public uint ClassId { get; set; }
        public string Season { get; set; } = null!;
        public uint Year { get; set; }
        public uint CatalogId { get; set; }
        public string Location { get; set; } = null!;
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string ProfessorUId { get; set; } = null!;

        public virtual Course Catalog { get; set; } = null!;
        public virtual Professor ProfessorU { get; set; } = null!;
        public virtual ICollection<AssignmentCategory> AssignmentCategories { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; }
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}

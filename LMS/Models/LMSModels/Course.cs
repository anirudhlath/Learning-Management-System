using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Course
    {
        public Course()
        {
            Classes = new HashSet<Class>();
        }

        public string CourseName { get; set; } = null!;
        public string CourseNumber { get; set; } = null!;
        public uint CatalogId { get; set; }
        public string SubjectAbb { get; set; } = null!;

        public virtual Department SubjectAbbNavigation { get; set; } = null!;
        public virtual ICollection<Class> Classes { get; set; }
    }
}

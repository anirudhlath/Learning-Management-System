using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Department
    {
        public Department()
        {
            Courses = new HashSet<Course>();
            StudentMajorsIns = new HashSet<StudentMajorsIn>();
            UIds = new HashSet<Professor>();
        }

        public string Name { get; set; } = null!;
        public string SubjectAbb { get; set; } = null!;

        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<StudentMajorsIn> StudentMajorsIns { get; set; }

        public virtual ICollection<Professor> UIds { get; set; }
    }
}

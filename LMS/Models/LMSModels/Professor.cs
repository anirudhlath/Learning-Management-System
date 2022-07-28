using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professor
    {
        public Professor()
        {
            Classes = new HashSet<Class>();
            SubjectAbbs = new HashSet<Department>();
        }

        public string UId { get; set; } = null!;

        public virtual User UIdNavigation { get; set; } = null!;

        public virtual ICollection<Class> Classes { get; set; }
        public virtual ICollection<Department> SubjectAbbs { get; set; }
    }
}

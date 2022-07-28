using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class StudentMajorsIn
    {
        public string UId { get; set; } = null!;
        public string? SubjectAbb { get; set; }

        public virtual Department? SubjectAbbNavigation { get; set; }
        public virtual Student UIdNavigation { get; set; } = null!;
    }
}

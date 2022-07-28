using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Enrollment
    {
        public string UId { get; set; } = null!;
        public string CatalogId { get; set; } = null!;
        public string Grade { get; set; } = null!;
        public string Semester { get; set; } = null!;

        public virtual Student UIdNavigation { get; set; } = null!;
    }
}

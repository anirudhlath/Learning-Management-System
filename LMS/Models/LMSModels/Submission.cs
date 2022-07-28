using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public DateTime SubmissionTime { get; set; }
        public uint? Score { get; set; }
        public string SubmissionContents { get; set; } = null!;
        public string UId { get; set; } = null!;
        public string CatalogId { get; set; } = null!;
        public string Semester { get; set; } = null!;
        public uint Section { get; set; }
        public uint SubmissionId { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual Student UIdNavigation { get; set; } = null!;
    }
}

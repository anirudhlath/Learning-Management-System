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
        public uint SubmissionId { get; set; }
        public uint ClassId { get; set; }
        public uint AssignmentId { get; set; }

        public virtual Assignment Assignment { get; set; } = null!;
        public virtual Class Class { get; set; } = null!;
        public virtual Student UIdNavigation { get; set; } = null!;
    }
}

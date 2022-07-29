using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class User
    {
        public string UId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime Dob { get; set; }

        public virtual Administrator Administrator { get; set; } = null!;
        public virtual Professor Professor { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
    }
}

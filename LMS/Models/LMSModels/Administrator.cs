using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Administrator
    {
        public string UId { get; set; } = null!;

        public virtual User UIdNavigation { get; set; } = null!;
    }
}

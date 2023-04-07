using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ActiveProjectLabour
    {
        public long ActiveProjectId { get; set; }
        public long ProjectLabourRequirementsId { get; set; }
        public double Progress { get; set; }

        public virtual ActiveProject ActiveProject { get; set; }
        public virtual ProjectLabourRequirement ProjectLabourRequirements { get; set; }
    }
}

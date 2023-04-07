using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ProjectLabourImpact
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Definition { get; set; }
        public long ProjectLabourRequirementId { get; set; }
        public double MinimumHoursForImpactToKickIn { get; set; }

        public virtual ProjectLabourRequirement ProjectLabourRequirement { get; set; }
    }
}

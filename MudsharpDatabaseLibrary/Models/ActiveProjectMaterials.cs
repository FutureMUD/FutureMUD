using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ActiveProjectMaterial
    {
        public long ActiveProjectId { get; set; }
        public long ProjectMaterialRequirementsId { get; set; }
        public double Progress { get; set; }

        public virtual ActiveProject ActiveProject { get; set; }
        public virtual ProjectMaterialRequirement ProjectMaterialRequirements { get; set; }
    }
}

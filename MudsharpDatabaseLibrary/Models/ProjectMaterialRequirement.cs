using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ProjectMaterialRequirement
    {
        public ProjectMaterialRequirement()
        {
            ActiveProjectMaterials = new HashSet<ActiveProjectMaterial>();
        }

        public long Id { get; set; }
        public string Type { get; set; }
        public long ProjectPhaseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Definition { get; set; }
        public bool IsMandatoryForProjectCompletion { get; set; }

        public virtual ProjectPhase ProjectPhase { get; set; }
        public virtual ICollection<ActiveProjectMaterial> ActiveProjectMaterials { get; set; }
    }
}

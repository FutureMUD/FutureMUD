using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ProjectPhase
    {
        public ProjectPhase()
        {
            ActiveProjects = new HashSet<ActiveProject>();
            ProjectActions = new HashSet<ProjectAction>();
            ProjectLabourRequirements = new HashSet<ProjectLabourRequirement>();
            ProjectMaterialRequirements = new HashSet<ProjectMaterialRequirement>();
        }

        public long Id { get; set; }
        public long ProjectId { get; set; }
        public int ProjectRevisionNumber { get; set; }
        public int PhaseNumber { get; set; }
        public string Description { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<ActiveProject> ActiveProjects { get; set; }
        public virtual ICollection<ProjectAction> ProjectActions { get; set; }
        public virtual ICollection<ProjectLabourRequirement> ProjectLabourRequirements { get; set; }
        public virtual ICollection<ProjectMaterialRequirement> ProjectMaterialRequirements { get; set; }
    }
}

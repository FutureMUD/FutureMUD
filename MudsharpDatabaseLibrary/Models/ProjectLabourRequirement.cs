using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ProjectLabourRequirement
    {
        public ProjectLabourRequirement()
        {
            ActiveProjectLabours = new HashSet<ActiveProjectLabour>();
            Characters = new HashSet<Character>();
            ProjectLabourImpacts = new HashSet<ProjectLabourImpact>();
        }

        public long Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public long ProjectPhaseId { get; set; }
        public string Description { get; set; }
        public double TotalProgressRequired { get; set; }
        public int MaximumSimultaneousWorkers { get; set; }
        public string Definition { get; set; }

        public virtual ProjectPhase ProjectPhase { get; set; }
        public virtual ICollection<ActiveProjectLabour> ActiveProjectLabours { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
        public virtual ICollection<ProjectLabourImpact> ProjectLabourImpacts { get; set; }
    }
}

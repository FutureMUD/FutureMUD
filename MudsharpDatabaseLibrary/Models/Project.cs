using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Project
    {
        public Project()
        {
            ActiveProjects = new HashSet<ActiveProject>();
            ProjectPhases = new HashSet<ProjectPhase>();
        }

        public long Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int RevisionNumber { get; set; }
        public long EditableItemId { get; set; }
        public string Definition { get; set; }
        public bool AppearInJobsList { get; set; }

        public virtual EditableItem EditableItem { get; set; }
        public virtual ICollection<ActiveProject> ActiveProjects { get; set; }
        public virtual ICollection<ProjectPhase> ProjectPhases { get; set; }
    }
}

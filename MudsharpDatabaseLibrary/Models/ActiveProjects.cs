using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ActiveProject
    {
        public ActiveProject()
        {
            ActiveProjectLabours = new HashSet<ActiveProjectLabour>();
            ActiveProjectMaterials = new HashSet<ActiveProjectMaterial>();
            Characters = new HashSet<Character>();
        }

        public long Id { get; set; }
        public long ProjectId { get; set; }
        public int ProjectRevisionNumber { get; set; }
        public long CurrentPhaseId { get; set; }
        public long? CharacterId { get; set; }
        public long? CellId { get; set; }

        public virtual Cell Cell { get; set; }
        public virtual Character Character { get; set; }
        public virtual ProjectPhase CurrentPhase { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<ActiveProjectLabour> ActiveProjectLabours { get; set; }
        public virtual ICollection<ActiveProjectMaterial> ActiveProjectMaterials { get; set; }
        public virtual ICollection<Character> Characters { get; set; }
    }
}

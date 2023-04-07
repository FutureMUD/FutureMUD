using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ProjectAction
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int SortOrder { get; set; }
        public string Definition { get; set; }
        public long ProjectPhaseId { get; set; }

        public virtual ProjectPhase ProjectPhase { get; set; }
    }
}

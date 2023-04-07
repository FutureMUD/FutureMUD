using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ArtificialIntelligence
    {
        public ArtificialIntelligence()
        {
            NpcsArtificialIntelligences = new HashSet<NpcsArtificialIntelligences>();
            NpctemplatesArtificalIntelligences = new HashSet<NpcTemplatesArtificalIntelligences>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Definition { get; set; }

        public virtual ICollection<NpcsArtificialIntelligences> NpcsArtificialIntelligences { get; set; }
        public virtual ICollection<NpcTemplatesArtificalIntelligences> NpctemplatesArtificalIntelligences { get; set; }
    }
}

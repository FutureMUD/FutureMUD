using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class NpcsArtificialIntelligences
    {
        public long Npcid { get; set; }
        public long ArtificialIntelligenceId { get; set; }

        public virtual ArtificialIntelligence ArtificialIntelligence { get; set; }
        public virtual Npc Npc { get; set; }
    }
}

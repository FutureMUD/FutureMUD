using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class NpcTemplatesArtificalIntelligences
    {
        public long NpcTemplateId { get; set; }
        public long AiId { get; set; }
        public int NpcTemplateRevisionNumber { get; set; }

        public virtual ArtificialIntelligence Ai { get; set; }
        public virtual NpcTemplate Npctemplate { get; set; }
    }
}

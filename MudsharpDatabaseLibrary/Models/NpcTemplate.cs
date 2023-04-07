using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class NpcTemplate
    {
        public NpcTemplate()
        {
            Npcs = new HashSet<Npc>();
            NpctemplatesArtificalIntelligences = new HashSet<NpcTemplatesArtificalIntelligences>();
        }

        public long Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public long EditableItemId { get; set; }
        public int RevisionNumber { get; set; }

        public virtual EditableItem EditableItem { get; set; }
        public virtual ICollection<Npc> Npcs { get; set; }
        public virtual ICollection<NpcTemplatesArtificalIntelligences> NpctemplatesArtificalIntelligences { get; set; }
    }
}

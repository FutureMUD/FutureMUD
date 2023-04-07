using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Npc
    {
        public Npc()
        {
            NpcsArtificialIntelligences = new HashSet<NpcsArtificialIntelligences>();
        }

        public long Id { get; set; }
        public long CharacterId { get; set; }
        public long TemplateId { get; set; }
        public int TemplateRevnum { get; set; }
        public long? BodyguardCharacterId { get; set; }

        public virtual Character BodyguardCharacter { get; set; }
        public virtual Character Character { get; set; }
        public virtual NpcTemplate Template { get; set; }
        public virtual ICollection<NpcsArtificialIntelligences> NpcsArtificialIntelligences { get; set; }
    }
}

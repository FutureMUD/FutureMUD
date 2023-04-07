using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CharacterKnowledge
    {
        public long Id { get; set; }
        public long CharacterId { get; set; }
        public long KnowledgeId { get; set; }
        public DateTime WhenAcquired { get; set; }
        public string HowAcquired { get; set; }
        public int TimesTaught { get; set; }

        public virtual Character Character { get; set; }
        public virtual Knowledge Knowledge { get; set; }
    }
}

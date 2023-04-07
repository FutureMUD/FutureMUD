using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CharacterAccent
    {
        public long CharacterId { get; set; }
        public long AccentId { get; set; }
        public int Familiarity { get; set; }
        public bool IsPreferred { get; set; }

        public virtual Accent Accent { get; set; }
        public virtual Character Character { get; set; }
    }
}

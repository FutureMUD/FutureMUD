using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CharactersScripts
    {
        public long CharacterId { get; set; }
        public long ScriptId { get; set; }

        public virtual Character Character { get; set; }
        public virtual Script Script { get; set; }
    }
}

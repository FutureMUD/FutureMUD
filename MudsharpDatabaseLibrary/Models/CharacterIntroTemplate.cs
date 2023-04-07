using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CharacterIntroTemplate
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int ResolutionPriority { get; set; }
        public long AppliesToCharacterProgId { get; set; }
        public string Definition { get; set; }
        public int Order { get; set; }

        public virtual FutureProg AppliesToCharacterProg { get; set; }
    }
}

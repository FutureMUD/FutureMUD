using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Ally
    {
        public long CharacterId { get; set; }
        public long AllyId { get; set; }
        public bool Trusted { get; set; }

        public virtual Character AllyCharacter { get; set; }
        public virtual Character OwnerCharacter { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class CharactersMagicResources
    {
        public long CharacterId { get; set; }
        public long MagicResourceId { get; set; }
        public double Amount { get; set; }

        public virtual Character Character { get; set; }
        public virtual MagicResource MagicResource { get; set; }
    }
}

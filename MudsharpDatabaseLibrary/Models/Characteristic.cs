using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Characteristic
    {
        public long BodyId { get; set; }
        public long CharacteristicId { get; set; }
        public int Type { get; set; }

        public virtual Body Body { get; set; }
        public virtual CharacteristicValue CharacteristicValue { get; set; }
    }
}

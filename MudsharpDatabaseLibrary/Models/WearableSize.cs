using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class WearableSize
    {
        public long Id { get; set; }
        public bool OneSizeFitsAll { get; set; }
        public double? Height { get; set; }
        public double? Weight { get; set; }
        public double? TraitValue { get; set; }
        public long BodyPrototypeId { get; set; }
    }
}

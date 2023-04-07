using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodypartProtoOrientationHits
    {
        public long BodypartProtoId { get; set; }
        public int Orientation { get; set; }
        public int? HitChance { get; set; }
        public long Id { get; set; }

        public virtual BodypartProto BodypartProto { get; set; }
    }
}

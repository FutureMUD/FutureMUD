using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodypartInternalInfos
    {
        public long BodypartProtoId { get; set; }
        public long InternalPartId { get; set; }
        public bool IsPrimaryOrganLocation { get; set; }
        public double HitChance { get; set; }
        public string ProximityGroup { get; set; }

        public virtual BodypartProto BodypartProto { get; set; }
        public virtual BodypartProto InternalPart { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class LimbBodypartProto
    {
        public long BodypartProtoId { get; set; }
        public long LimbId { get; set; }

        public virtual BodypartProto BodypartProto { get; set; }
        public virtual Limb Limb { get; set; }
    }
}

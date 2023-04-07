using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodiesSeveredParts
    {
        public long BodiesId { get; set; }
        public long BodypartProtoId { get; set; }

        public virtual Body Bodies { get; set; }
        public virtual BodypartProto BodypartProto { get; set; }
    }
}

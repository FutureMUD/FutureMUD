using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodypartGroupDescribersBodyProtos
    {
        public long BodypartGroupDescriberId { get; set; }
        public long BodyProtoId { get; set; }

        public virtual BodyProto BodyProto { get; set; }
        public virtual BodypartGroupDescriber BodypartGroupDescriber { get; set; }
    }
}

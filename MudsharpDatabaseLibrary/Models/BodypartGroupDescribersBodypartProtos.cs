using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodypartGroupDescribersBodypartProtos
    {
        public long BodypartGroupDescriberId { get; set; }
        public long BodypartProtoId { get; set; }
        public bool Mandatory { get; set; }

        public virtual BodypartGroupDescriber BodypartGroupDescriber { get; set; }
        public virtual BodypartProto BodypartProto { get; set; }
    }
}

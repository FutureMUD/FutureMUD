using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ButcheryProductsBodypartProtos
    {
        public long ButcheryProductId { get; set; }
        public long BodypartProtoId { get; set; }

        public virtual BodypartProto BodypartProto { get; set; }
        public virtual ButcheryProducts ButcheryProduct { get; set; }
    }
}

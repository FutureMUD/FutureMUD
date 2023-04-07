using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodyProtosAdditionalBodyparts
    {
        public long BodyProtoId { get; set; }
        public long BodypartId { get; set; }
        public string Usage { get; set; }

        public virtual BodyProto BodyProto { get; set; }
        public virtual BodypartProto Bodypart { get; set; }
    }
}

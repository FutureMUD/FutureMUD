using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodypartShape
    {
        public BodypartShape()
        {
            BodypartGroupDescribersShapeCount = new HashSet<BodypartGroupDescribersShapeCount>();
            BodypartProto = new HashSet<BodypartProto>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BodypartGroupDescribersShapeCount> BodypartGroupDescribersShapeCount { get; set; }
        public virtual ICollection<BodypartProto> BodypartProto { get; set; }
    }
}

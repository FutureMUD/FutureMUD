using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodypartGroupDescriber
    {
        public BodypartGroupDescriber()
        {
            BodypartGroupDescribersBodyProtos = new HashSet<BodypartGroupDescribersBodyProtos>();
            BodypartGroupDescribersBodypartProtos = new HashSet<BodypartGroupDescribersBodypartProtos>();
            BodypartGroupDescribersShapeCount = new HashSet<BodypartGroupDescribersShapeCount>();
        }

        public long Id { get; set; }
        public string DescribedAs { get; set; }
        public string Comment { get; set; }
        public string Type { get; set; }

        public virtual ICollection<BodypartGroupDescribersBodyProtos> BodypartGroupDescribersBodyProtos { get; set; }
        public virtual ICollection<BodypartGroupDescribersBodypartProtos> BodypartGroupDescribersBodypartProtos { get; set; }
        public virtual ICollection<BodypartGroupDescribersShapeCount> BodypartGroupDescribersShapeCount { get; set; }
    }
}

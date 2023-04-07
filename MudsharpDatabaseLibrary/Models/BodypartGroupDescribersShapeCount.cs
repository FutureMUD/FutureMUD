using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class BodypartGroupDescribersShapeCount
    {
        public int MinCount { get; set; }
        public long BodypartGroupDescriptionRuleId { get; set; }
        public long TargetId { get; set; }
        public int MaxCount { get; set; }

        public virtual BodypartGroupDescriber BodypartGroupDescriptionRule { get; set; }
        public virtual BodypartShape Target { get; set; }
    }
}

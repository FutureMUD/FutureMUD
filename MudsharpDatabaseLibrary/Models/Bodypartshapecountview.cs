using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Bodypartshapecountview
    {
        public sbyte BodypartGroupDescriptionRuleId { get; set; }
        public sbyte DescribedAs { get; set; }
        public sbyte MinCount { get; set; }
        public sbyte MaxCount { get; set; }
        public sbyte TargetId { get; set; }
        public sbyte Name { get; set; }
    }
}

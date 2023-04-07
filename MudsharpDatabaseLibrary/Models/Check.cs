using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Check
    {
        public int Type { get; set; }
        public long TraitExpressionId { get; set; }
        public long CheckTemplateId { get; set; }
        public int MaximumDifficultyForImprovement { get; set; }

        public virtual CheckTemplate CheckTemplate { get; set; }
        public virtual TraitExpression TraitExpression { get; set; }
    }
}

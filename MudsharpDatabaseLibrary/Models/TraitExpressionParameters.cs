using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class TraitExpressionParameters
    {
        public long TraitExpressionId { get; set; }
        public long TraitDefinitionId { get; set; }
        public string Parameter { get; set; }
        public bool CanImprove { get; set; }
        public bool CanBranch { get; set; }

        public virtual TraitDefinition TraitDefinition { get; set; }
        public virtual TraitExpression TraitExpression { get; set; }
    }
}

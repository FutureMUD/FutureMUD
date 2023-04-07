using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class Trait
    {
        public long BodyId { get; set; }
        public double Value { get; set; }
        public long TraitDefinitionId { get; set; }
        public double AdditionalValue { get; set; }

        public virtual Body Body { get; set; }
        public virtual TraitDefinition TraitDefinition { get; set; }
    }
}

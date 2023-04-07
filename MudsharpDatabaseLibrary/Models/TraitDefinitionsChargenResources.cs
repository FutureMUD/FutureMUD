using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class TraitDefinitionsChargenResources
    {
        public long TraitDefinitionId { get; set; }
        public long ChargenResourceId { get; set; }
        public int Amount { get; set; }
        public bool RequirementOnly { get; set; }

        public virtual ChargenResource ChargenResource { get; set; }
        public virtual TraitDefinition TraitDefinition { get; set; }
    }
}

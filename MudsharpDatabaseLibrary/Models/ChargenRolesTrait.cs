using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class ChargenRolesTrait
    {
        public long ChargenRoleId { get; set; }
        public long TraitId { get; set; }
        public double Amount { get; set; }
        public bool GiveIfDoesntHave { get; set; }

        public virtual ChargenRole ChargenRole { get; set; }
        public virtual TraitDefinition Trait { get; set; }
    }
}

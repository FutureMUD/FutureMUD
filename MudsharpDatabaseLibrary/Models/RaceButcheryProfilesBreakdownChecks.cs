using System;
using System.Collections.Generic;

namespace MudSharp.Models
{
    public partial class RaceButcheryProfilesBreakdownChecks
    {
        public long RaceButcheryProfileId { get; set; }
        public string Subcageory { get; set; }
        public long TraitDefinitionId { get; set; }
        public int Difficulty { get; set; }

        public virtual RaceButcheryProfile RaceButcheryProfile { get; set; }
        public virtual TraitDefinition TraitDefinition { get; set; }
    }
}

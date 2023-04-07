using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MudSharp.Models
{
    public class MagicSpell
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Blurb { get; set; }
        public string Description { get; set; }
        public long SpellKnownProgId { get; set; }
        public long MagicSchoolId { get; set; }
        public double ExclusiveDelay { get; set; }
        public double NonExclusiveDelay { get; set; }
        public string Definition { get; set; }
        public int CastingDifficulty { get; set; }
        public int? ResistingDifficulty { get; set; }
        public long? CastingTraitDefinitionId { get; set; }
        public long? ResistingTraitDefinitionId { get; set; }
        public long? EffectDurationExpressionId {get; set; }
        public int MinimumSuccessThreshold { get; set; }
        public bool AppliedEffectsAreExclusive { get; set; }

        public string CastingEmote { get; set; }
        public string FailCastingEmote { get; set; }
        public string TargetEmote { get; set; }
        public string TargetResistedEmote { get; set; }
        public int CastingEmoteFlags { get; set; }
        public int TargetEmoteFlags { get; set; }

        public virtual MagicSchool MagicSchool { get; set; }
        public virtual FutureProg SpellKnownProg { get; set; }
        [CanBeNull] public virtual TraitDefinition CastingTraitDefinition { get; set; }
        [CanBeNull] public virtual TraitExpression EffectDurationExpression { get;set; }
        [CanBeNull] public virtual TraitDefinition ResistingTraitDefinition { get;set; }
    }
}

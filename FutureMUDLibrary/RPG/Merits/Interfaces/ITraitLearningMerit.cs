using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;

namespace MudSharp.RPG.Merits.Interfaces
{
    public interface ITraitLearningMerit : ICharacterMerit
    {
        double BranchingChanceModifier(IPerceivableHaveTraits ch, ITraitDefinition trait);
        double SkillLearningChanceModifier(IPerceivableHaveTraits ch, ITraitDefinition trait, Outcome outcome, Difficulty difficulty, TraitUseType useType);
    }
}

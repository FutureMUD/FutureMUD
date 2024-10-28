using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic
{
    public interface IMagicCapability : IFrameworkItem, IProgVariable, IEditableItem
    {
        IMagicSchool School { get; }
        int PowerLevel { get; }
        IEnumerable<IMagicPower> InherentPowers(ICharacter actor);
        double ConcentrationAbility(ICharacter actor);
        Difficulty GetConcentrationDifficulty(ICharacter actor, double concentrationPercentageOfCapability, double individualPowerConcentrationPercentage);
        IEnumerable<IMagicResourceRegenerator> Regenerators { get; }
        bool ShowMagicResourcesInPrompt { get; }
        IEnumerable<IMagicPower> AllPowers { get; }

        IMagicCapability Clone(string newName);
    }
}

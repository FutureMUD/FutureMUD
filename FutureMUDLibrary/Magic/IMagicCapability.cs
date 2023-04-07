using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic
{
    public interface IMagicCapability : IFrameworkItem, IFutureProgVariable
    {
        IMagicSchool School { get; }
        int PowerLevel { get; }
        IEnumerable<IMagicPower> InherentPowers(ICharacter actor);
        double ConcentrationAbility(ICharacter actor);
        Difficulty GetConcentrationDifficulty(double concentrationPercentageOfCapability, double individualPowerConcentrationPercentage);
        IEnumerable<IMagicResourceRegenerator> Regenerators { get; }
        bool ShowMagicResourcesInPrompt { get; }
    }
}

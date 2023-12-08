using MudSharp.Framework;
using MudSharp.RPG.Merits;

namespace MudSharp.Body.Traits
{
	public interface IPerceivableHaveTraits : IPerceivable, IHaveTraits, IHaveMerits {
        double GetCurrentBonusLevel();
    }
}
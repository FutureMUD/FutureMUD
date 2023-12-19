using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Combat;

public interface IArenaMatchBet : IFrameworkItem, ISaveable
{
	decimal BetAmount { get; }
	decimal Odds { get; }
	bool Won { get; }
	bool CashedOut { get; }
	IArenaMatch Match { get; }
	int Team { get; }
}

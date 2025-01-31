using MudSharp.RPG.Checks;

namespace MudSharp.RPG.Merits.Interfaces;

public interface IDarksightMerit : ICharacterMerit
{
	Difficulty MinimumEffectiveDifficulty { get; }
}
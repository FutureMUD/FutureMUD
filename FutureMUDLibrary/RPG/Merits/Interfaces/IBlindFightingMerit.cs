#nullable enable
namespace MudSharp.RPG.Merits.Interfaces;

public interface IBlindFightingMerit : ICharacterMerit
{
	/// <summary>Positive stages make blind fighting harder; negative stages make it easier.</summary>
	int DifficultyStages { get; }
}

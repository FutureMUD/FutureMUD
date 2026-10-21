#nullable enable

namespace MudSharp.RPG.Merits.Interfaces;

/// <summary>Changes maximum stamina without changing recovery or restoring spent stamina.</summary>
public interface IStaminaCapacityMerit : ICharacterMerit
{
	double StaminaBonus { get; }
	double StaminaMultiplier { get; }
}

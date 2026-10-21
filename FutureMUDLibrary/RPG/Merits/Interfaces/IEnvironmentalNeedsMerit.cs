#nullable enable

using MudSharp.Character;

namespace MudSharp.RPG.Merits.Interfaces;

/// <summary>
/// Optional environmental contributions to passive needs. Rates use the same game-hour clock
/// as passive needs; water is expressed separately in litres per game hour. Signed credits
/// reduce loss and must never be applied as positive food or water fulfilment.
/// </summary>
public sealed record EnvironmentalNeedsAdjustment(
	double HungerRateDelta = 0,
	double ThirstRateDelta = 0,
	double WaterLossLitresPerHour = 0,
	double? MaximumHungerRate = null,
	double? MaximumThirstRate = null);

public interface IEnvironmentalNeedsMerit : ICharacterMerit
{
	EnvironmentalNeedsAdjustment GetNeedsAdjustment(ICharacter character);
}

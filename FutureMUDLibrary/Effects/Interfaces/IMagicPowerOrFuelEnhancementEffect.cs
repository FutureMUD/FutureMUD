#nullable enable

using MudSharp.GameItems;

namespace MudSharp.Effects.Interfaces;

public interface IMagicPowerOrFuelEnhancementEffect : IEffectSubtype
{
	double PowerProductionMultiplier { get; }
	double PowerConsumptionMultiplier { get; }
	double FuelUseMultiplier { get; }
	bool AppliesToPoweredItem(IGameItem item);
}

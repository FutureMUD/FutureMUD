using MudSharp.Effects;

namespace MudSharp.Planes;

public interface IPlanarOverlayEffect : IEffect
{
	PlanarPresenceDefinition PlanarPresenceDefinition { get; }
	int PlanarPriority { get; }
	bool OverridesBasePlanarPresence { get; }
}

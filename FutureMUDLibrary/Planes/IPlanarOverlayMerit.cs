using MudSharp.RPG.Merits;

namespace MudSharp.Planes;

public interface IPlanarOverlayMerit : IMerit
{
	PlanarPresenceDefinition PlanarPresenceDefinition { get; }
	int PlanarPriority { get; }
	bool OverridesBasePlanarPresence { get; }
}

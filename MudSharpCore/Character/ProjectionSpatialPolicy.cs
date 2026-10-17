#nullable enable

using MudSharp.Construction;
using MudSharp.Magic;

namespace MudSharp.Character;

public static class ProjectionSpatialPolicy
{
	public static string? Error(IPerceivable mover, SpatialLocation destination, bool throughExit) =>
		mover.EffectsOfType<ISpellProjectionBoundary>().Select(x => x.TravelError(destination, throughExit)).FirstOrDefault(x => x is not null);
}

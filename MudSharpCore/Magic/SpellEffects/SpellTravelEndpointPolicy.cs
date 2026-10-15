#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Planes;

namespace MudSharp.Magic.SpellEffects;

/// <summary>Shared physical endpoint admission for the S4 travel routes. Destination selection remains spell-specific.</summary>
public static class SpellTravelEndpointPolicy
{
	public static string? ConnectionError(ICharacter caster, IRoom source, IRoom destination,
		IMagicSpell spell, RoomLayer layer)
	{
		if (!ReferenceEquals(caster.Gameworld, spell.Gameworld) ||
			!ReferenceEquals(source.Gameworld, caster.Gameworld) ||
			!ReferenceEquals(destination.Gameworld, caster.Gameworld) ||
			!ReferenceEquals(caster.Gameworld.Rooms.Get(source.Id), source) ||
			!ReferenceEquals(caster.Gameworld.Rooms.Get(destination.Id), destination) ||
			!StableRoom(source, layer) || !StableRoom(destination, layer))
			return "Travel requires two current, permanent rooms with the selected layer.";
		if (!caster.CanInteractPlanar(source, PlanarInteractionKind.MovementBarrier) ||
			!caster.CanInteractPlanar(destination, PlanarInteractionKind.MovementBarrier) ||
			MagicInterdictionHelper.GetInterdiction(caster, source, spell.School, false) is not null ||
			MagicInterdictionHelper.GetInterdiction(caster, destination, spell.School, false) is not null)
			return "A planar or magical ward prevents travel.";
		return null;
	}

	public static string? RelocationError(ICharacter caster, ICharacter mover, IRoom source,
		IRoom destination, IMagicSpell spell, RoomLayer layer)
	{
		if (!ReferenceEquals(mover.Gameworld, caster.Gameworld) ||
			!ReferenceEquals(mover.Location, source) || mover.Body is null ||
			mover.RoomLayer != layer ||
			!caster.CanInteractPlanar(mover, PlanarInteractionKind.Magic))
			return "The exact physical traveller is no longer eligible.";
		var error = ConnectionError(caster, source, destination, spell, layer);
		if (error is not null) return error;
		if (!mover.CanInteractPlanar(destination, PlanarInteractionKind.MovementBarrier) ||
			MagicInterdictionHelper.GetInterdiction(mover, destination, spell.School, false) is not null)
			return "A planar or magical ward prevents the traveller's relocation.";
		if (ProjectionSpatialPolicy.Error(mover,
			new SpatialLocation(destination, layer), false) is { } projectionError)
			return projectionError;
		return null;
	}

	private static bool StableRoom(IRoom room, RoomLayer layer) => room.Id > 0 && !room.Temporary &&
		room is not Room { HostedVehicleId: not null } && room.RouteDefinition is null &&
		room.CurrentOverlay?.Package?.Status == RevisionStatus.Current &&
		room.Terrain(null).TerrainLayers.Contains(layer);
}

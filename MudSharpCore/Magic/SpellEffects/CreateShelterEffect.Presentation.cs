#nullable enable

using MudSharp.Construction.Autobuilder;

namespace MudSharp.Magic.SpellEffects;

public sealed partial class CreateShelterEffect
{
	private static Func<bool> CaptureAutobuilderFrame(IAutobuilderRoomPresentation source)
	{
		var snapshot = source.CaptureDefaultPresentation();
		var world = source.Gameworld;
		var terrain = snapshot.Presentation.Terrain;
		var terrainName = terrain.Name;
		var layers = terrain.TerrainLayers.ToArray();
		var atmosphere = terrain.Atmosphere;
		var profile = snapshot.Presentation.ForagableProfile;
		var revision = profile?.RevisionNumber; var status = profile?.Status;
		return () => ReferenceEquals(source.Gameworld, world) && snapshot.IsCurrent &&
			ReferenceEquals(world.Terrains.Get(terrain.Id), terrain) && terrain.Name == terrainName &&
			terrain.TerrainLayers.SequenceEqual(layers) && ReferenceEquals(terrain.Atmosphere, atmosphere) &&
			(atmosphere is null || ReferenceEquals(world.Gases.Get(atmosphere.Id), atmosphere)) &&
			(profile is null || ReferenceEquals(world.ForagableProfiles.Get(profile.Id), profile) &&
				profile.RevisionNumber == revision && profile.Status == status);
	}
}

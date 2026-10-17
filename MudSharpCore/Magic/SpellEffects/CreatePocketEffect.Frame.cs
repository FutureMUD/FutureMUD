#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.Magic.SpellEffects;

public sealed partial class CreatePocketEffect
{
	private Func<bool> CaptureNativeFrame(ICharacter caster, IGameItem source, SpellPocketConfiguration configuration)
	{
		var world = Gameworld;
		var body = caster.Body; var bodyId = body.Id; var instance = caster.InstanceId;
		var identity = CharacterInstanceIdentityComparer.IdentityId(caster);
		var location = caster.Location; var layer = caster.RoomLayer; var position = caster.RoutePositionMetres;
		var sourceId = source.Id; var quantity = source.Quantity; var sourcePrototype = source.Prototype;
		var sourceRevision = sourcePrototype.RevisionNumber;
		var sourceComponents = source.Components.ToArray();
		var inventory = source.InInventoryOf; var container = source.ContainedIn; var floor = (source as GameItem)?.DirectLocation;
		var prototype = world.ItemProtos.Get(configuration.PrototypeId);
		var prototypeCurrent = CapturePrototypeFrame(prototype);
		var fallback = world.Rooms.Get(configuration.FallbackRoomId);
		var overlay = fallback.CurrentOverlay; var terrain = fallback.Terrain(null);
		var package = overlay?.Package; var packageStatus = package?.Status; var packageRevision = package?.RevisionNumber;
		var route = fallback.RouteDefinition; var routeVersion = route?.TopologyVersion;
		var temporary = fallback.Temporary;
		var vehicle = (fallback as Room)?.HostedVehicleId;
		var compartment = (fallback as Room)?.HostedVehicleCompartmentId;
		return () => ReferenceEquals(caster.Gameworld, world) && ReferenceEquals(caster.Body, body) && body.Id == bodyId &&
			caster.InstanceId == instance && CharacterInstanceIdentityComparer.IdentityId(caster) == identity &&
			ReferenceEquals(caster.Location, location) && caster.RoomLayer == layer && caster.RoutePositionMetres == position &&
			source.Id == sourceId && source.Quantity == quantity && quantity > 0 && !source.Deleted && !source.Destroyed &&
			ReferenceEquals(source.Gameworld, world) && ReferenceEquals(world.Items.Get(sourceId), source) &&
			caster.Body.HeldItems.Any(x => ReferenceEquals(x, source)) &&
			ReferenceEquals(source.InInventoryOf, inventory) && ReferenceEquals(source.ContainedIn, container) && ReferenceEquals((source as GameItem)?.DirectLocation, floor) &&
			ReferenceEquals(source.Prototype, sourcePrototype) && sourcePrototype.RevisionNumber == sourceRevision &&
			source.Components.SequenceEqual(sourceComponents, ReferenceEqualityComparer.Instance) &&
			ReferenceEquals(world.ItemProtos.Get(configuration.PrototypeId), prototype) && prototypeCurrent() &&
			ReferenceEquals(world.Rooms.Get(configuration.FallbackRoomId), fallback) && fallback.Temporary == temporary &&
			(fallback as Room)?.HostedVehicleId == vehicle && (fallback as Room)?.HostedVehicleCompartmentId == compartment &&
			ReferenceEquals(fallback.CurrentOverlay, overlay) && ReferenceEquals(fallback.Terrain(null), terrain) &&
			ReferenceEquals(fallback.CurrentOverlay?.Package, package) && package?.Status == packageStatus && package?.RevisionNumber == packageRevision &&
			ReferenceEquals(fallback.RouteDefinition, route) && route?.TopologyVersion == routeVersion;
	}

	private static Func<bool> CapturePrototypeFrame(IGameItemProto prototype)
	{
		var revision = prototype.RevisionNumber; var status = prototype.Status;
		var components = prototype.Components.ToArray();
		var states = components.Select(x => (Component: x, Revision: x.RevisionNumber, Status: x.Status)).ToArray();
		var pocket = prototype.GetItemType<FoldedPocketGameItemComponentProto>();
		var weight = pocket?.WeightLimit; var size = pocket?.MaximumContentsSize;
		var closable = pocket?.Closable; var transparent = pocket?.Transparent; var once = pocket?.OnceOnly;
		var preposition = pocket?.ContentsPreposition;
		var allowed = pocket?.AllowedTags.ToArray(); var blocked = pocket?.BlockedTags.ToArray();
		return () => prototype.RevisionNumber == revision && prototype.Status == status &&
			prototype.Components.SequenceEqual(components, ReferenceEqualityComparer.Instance) &&
			states.All(x => x.Component.RevisionNumber == x.Revision && x.Component.Status == x.Status) &&
			ReferenceEquals(prototype.GetItemType<FoldedPocketGameItemComponentProto>(), pocket) &&
			(pocket is null || pocket.WeightLimit == weight && pocket.MaximumContentsSize == size &&
				pocket.Closable == closable && pocket.Transparent == transparent && pocket.OnceOnly == once &&
				pocket.ContentsPreposition == preposition && pocket.AllowedTags.SequenceEqual(allowed!, ReferenceEqualityComparer.Instance) &&
				pocket.BlockedTags.SequenceEqual(blocked!, ReferenceEqualityComparer.Instance));
	}
}

#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.NPC.AI;

namespace MudSharp.GameItems;

/// <summary>Prepares completion for one exact component participant before its detach.</summary>
internal static class ComponentUnloadCompletion
{
	internal static bool OwnedBy(IGameItem item, IGameItem componentOwner) =>
		item is { Deleted: false, Destroyed: false } && ReferenceEquals(item.ContainedIn, componentOwner) &&
		!ComponentItemTransfer.HasDirectBodyCustody(item) && ComponentItemTransfer.DirectLocationOf(item) is null &&
		item.GetItemType<IBeltable>()?.ConnectedTo is null;

	internal static Action? PrepareReceive(ICharacter actor, IGameItem item)
	{
		if (!CommandExecutionScope.TryContinue(actor)) return null;
		if (actor.Body is MudSharp.Body.Implementations.Body body) return body.PrepareComponentUnload(item);
		var floor = PrepareFloor(actor, item, actor);
		var receiver = actor.Body;
		var canReceive = receiver.CanGet(item, 0);
		if (!CommandExecutionScope.TryContinue(actor) || !ReferenceEquals(actor.Body, receiver)) return null;
		return () =>
		{
			if (!ComponentItemTransfer.IsDetached(item)) return;
			if (canReceive) receiver.Get(item, silent: true);
			if (ComponentItemTransfer.IsDetached(item)) floor?.Invoke();
		};
	}

	internal static Action? PrepareFloor(ICharacter actor, IGameItem item, ILocateable source)
	{
		var deliver = PrepareFloorDestination(actor, source);
		return deliver is null ? null : () => deliver(item);
	}

	// Capture the safe point before a load splits its exact participant.
	internal static Action<IGameItem>? PrepareFloorDestination(ICharacter actor, ILocateable source)
	{
		if (!CommandExecutionScope.TryContinue(actor) || source.Location is null) return null;
		var destination = new SpatialLocation(source.Location, source.RoomLayer,
			source.Location.RouteDefinition is null ? null : source.RoutePositionMetres ?? source.Location.RouteDefinition.DefaultPositionMetres);
		if (!RouteSpatialService.Instance.TryValidateLocation(destination, out _) || !CommandExecutionScope.TryContinue(actor)) return null;
		return item =>
		{
			if (!ComponentItemTransfer.IsDetached(item)) return;
			if (item is GameItem native)
			{
				if (!native.TryDropPrepared(destination)) return;
			}
			else
			{
			item.RoomLayer = destination.Layer;
			if (!ComponentItemTransfer.IsDetached(item)) return;
			item.Drop(destination.Cell);
			if (!CanCompleteAt(item, destination)) return;
			if (destination.Cell.RouteDefinition is not null) item.MoveTo(destination);
			}
			if (!CanCompleteAt(item, destination)) return;
			if (!destination.Cell.GameItems.Any(x => ReferenceEquals(x, item))) destination.Cell.Insert(item);
		};
	}

	private static bool CanCompleteAt(IGameItem item, SpatialLocation point) => item is { Deleted: false, Destroyed: false } &&
		item.InInventoryOf is null && item.ContainedIn is null && item.GetItemType<IBeltable>()?.ConnectedTo is null &&
		ReferenceEquals(ComponentItemTransfer.DirectLocationOf(item), point.Cell) && item.RoomLayer == point.Layer &&
		(point.Cell.RouteDefinition is null || item.RoutePositionMetres == point.RoutePositionMetres);

	internal static bool Detach(ICharacter actor, IGameItem item, IGameItem owner, Action clearExactReference, Func<bool> exactParticipant)
	{
		if (!CommandExecutionScope.TryContinue(actor) || !OwnedBy(item, owner) || !exactParticipant()) return false;
		if (item is GameItem native)
			return native.TrySetContainedIn(null, () => CommandExecutionScope.TryContinue(actor) && OwnedBy(item, owner) && exactParticipant(), () =>
			{
				clearExactReference();
				CommandExecutionScope.MarkCommitted(actor);
			});
		// Non-native adapters retain their ordinary setter; native acceptance uses the exact API above.
		item.ContainedIn = null;
		if (ReferenceEquals(item.ContainedIn, owner)) return false;
		// Clearing only the captured reference preserves a replacement installed by a callback.
		clearExactReference();
		CommandExecutionScope.MarkCommitted(actor);
		return true;
	}
}

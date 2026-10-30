#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.NPC.AI;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	private sealed record ExchangeItemState(GameItem Item, Body Holder, IGameItemProto Prototype,
		HoldableGameItemComponent Holdable, int Quantity, ItemOwnershipReference? Owner,
		(IGameItemComponent Component, IGameItemComponentProto Prototype)[] Components)
	{
		internal static ExchangeItemState? Capture(IGameItem item, Body holder) =>
			item is GameItem native && item.GetItemType<HoldableGameItemComponent>() is { } holdable
				? new(native, holder, item.Prototype, holdable, item.Quantity, item.OwnershipReference,
					item.Components.Select(x => (x, x.Prototype)).ToArray()) : null;
		internal bool Current() => !Item.Deleted && !Item.Destroyed && ReferenceEquals(Item.Prototype, Prototype) &&
			ReferenceEquals(Item.GetItemType<HoldableGameItemComponent>(), Holdable) && ReferenceEquals(Holdable.HeldBy, Holder) &&
			Holder._heldItems.Any(x => ReferenceEquals(x.Item1, Item)) && Item.ContainedIn is null && Item.DirectLocation is null &&
			Item.Quantity == Quantity && Item.OwnershipReference == Owner && Item.Components.Count() == Components.Length &&
			Components.All(x => Item.Components.Any(y => ReferenceEquals(x.Component, y)) && ReferenceEquals(x.Component.Prototype, x.Prototype));
	}

	public bool TryExchangeItems(IGameItem offered, IBody partner, IGameItem requested)
	{
		if (partner is not Body other || ReferenceEquals(this, other) || ReferenceEquals(offered, requested)) return false;
		var first = ExchangeItemState.Capture(offered, this);
		var second = ExchangeItemState.Capture(requested, other);
		if (first is null || second is null || !first.Current() || !second.Current()) return false;
		var executor = Actor;
		var receiver = other.Actor;
		var firstInventory = _heldItems.ToArray();
		var secondInventory = other._heldItems.ToArray();
		var origin = RouteSpatialService.Instance.GetEffectiveLocation(executor);
		var destination = RouteSpatialService.Instance.GetEffectiveLocation(receiver);
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		bool Current() => ReferenceEquals(Actor, executor) && ReferenceEquals(executor.Body, this) &&
			ReferenceEquals(other.Actor, receiver) && ReferenceEquals(receiver.Body, other) &&
			CommandExecutionScope.TryContinue(executor) && CommandExecutionAuthority.IsCurrent(receiver, false) &&
			RouteSpatialService.Instance.GetEffectiveLocation(executor) == origin &&
			RouteSpatialService.Instance.GetEffectiveLocation(receiver) == destination && first.Current() && second.Current() &&
			_heldItems.Count == firstInventory.Length && firstInventory.All(x => _heldItems.Any(y => ReferenceEquals(x, y))) &&
			other._heldItems.Count == secondInventory.Length && secondInventory.All(x => other._heldItems.Any(y => ReferenceEquals(x, y)));
		if (!Current() || !CanGive(offered, other) || !other.CanGive(requested, this) || !Current()) return false;
		var receiveFirst = other.PrepareGetPlacement(offered, false);
		var receiveSecond = PrepareGetPlacement(requested, false);
		if (receiveFirst?.Hand is null || receiveSecond?.Hand is null || !Current()) return false;
		var firstDescription = $"{"<held in " + other.DescribeBodypartGroup([receiveFirst.Hand]) + ">",-35}";
		var secondDescription = $"{"<held in " + DescribeBodypartGroup([receiveSecond.Hand]) + ">",-35}";
		var witnesses = origin.Room.EventHandlersFor(executor).Except(new IHandleEvents[] { executor, receiver }).ToArray();
		bool HandCurrent(Body body, IGrab hand) => body.HoldLocs.Any(x => ReferenceEquals(x, hand)) && (!hand.Unary ||
			!body._heldItems.Any(x => ReferenceEquals(x.Item2, hand)) && !body._wieldedItems.Any(x => ReferenceEquals(x.Item2, hand)));
		bool PreparedCurrent() => Current() && HandCurrent(other, receiveFirst.Hand) && HandCurrent(this, receiveSecond.Hand);
		using var proximity = Gameworld.ProximityEventService?.BeginChange(ProximityChangeCause.Containment, offered, requested);
		ForeignCustodyTransferContext.EnsureBody(this, offered);
		ForeignCustodyTransferContext.EnsureBody(other, requested);
		if (!PreparedCurrent()) return false;
		// No gameplay callbacks between this final check and both coherent custody writes.
		CommandExecutionScope.MarkCommitted(executor);
		_heldItems.RemoveAll(x => ReferenceEquals(x.Item1, offered));
		other._heldItems.RemoveAll(x => ReferenceEquals(x.Item1, requested));
		first.Item.SetPreparedHeldCustody(other);
		second.Item.SetPreparedHeldCustody(this);
		other._heldItems.Add(Tuple.Create(offered, receiveFirst.Hand));
		_heldItems.Add(Tuple.Create(requested, receiveSecond.Hand));
		first.Holdable.CurrentInventoryDescription = firstDescription;
		second.Holdable.CurrentInventoryDescription = secondDescription;
		InventoryChanged = true;
		other.InventoryChanged = true;
		first.Item.Changed = true;
		second.Item.Changed = true;
		// Observers see both legs already committed. Their subsequent actions are not rolled back.
		PublishExchangeLeg(first.Item, this, other, executor, receiver, witnesses);
		PublishExchangeLeg(second.Item, other, this, receiver, executor, witnesses);
		proximity?.Complete();
		return true;
	}

	private static void PublishExchangeLeg(GameItem item, Body source, Body destination, ICharacter giver,
		ICharacter receiver, IHandleEvents[] witnesses)
	{
		if (item.Deleted || item.Destroyed) return;
		// Taken/component callbacks run after both legs commit. A callback's later custody claim wins.
		foreach (var component in item.Components.ToArray())
		{
			if (item.Deleted || item.Destroyed || !ReferenceEquals(item.InInventoryOf, destination)) break;
			component.Taken();
		}
		if (!item.Deleted && !item.Destroyed && ReferenceEquals(item.InInventoryOf, destination)) item.Get(destination);
		if (item.Deleted || item.Destroyed) return;
		source.OnInventoryChange?.Invoke(InventoryState.Held, InventoryState.Dropped, item);
		item.InvokeInventoryChange(InventoryState.Held, InventoryState.Dropped);
		destination.NotifyGotItem(item, receiver);
		giver.HandleEvent(EventType.CharacterGiveItemGiver, giver, receiver, item);
		receiver.HandleEvent(EventType.CharacterGiveItemReceiver, giver, receiver, item);
		item.HandleEvent(EventType.ItemGiven, giver, receiver, item);
		foreach (var witness in witnesses) witness.HandleEvent(EventType.CharacterGiveItemWitness, giver, receiver, item, witness);
		foreach (var witness in source.ExternalItems.ToArray()) witness.HandleEvent(EventType.CharacterGiveItemWitness, giver, receiver, item, witness);
	}
}

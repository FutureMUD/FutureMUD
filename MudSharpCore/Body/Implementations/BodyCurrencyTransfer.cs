#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.NPC.AI;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	private enum CurrencyTransferKind { GetRoom, GetContainer, Put, Drop, Give }

	private sealed record CurrencyState(GameItem Item, IGameItemProto Prototype, CurrencyGameItemComponent Pile,
		IGameItemComponentProto PilePrototype, HoldableGameItemComponent Holdable, ICurrency Currency,
		Dictionary<ICoin, int> Coins, IBody? Holder, IGameItem? Container, ICell? Cell,
		RoomLayer Layer, double? Coordinate, ItemOwnershipReference? Owner)
	{
		internal static CurrencyState? Capture(IGameItem item) => item is GameItem native &&
			item.GetItemType<CurrencyGameItemComponent>() is { } pile && item.GetItemType<HoldableGameItemComponent>() is not null
			? new(native, native.Prototype, pile, pile.Prototype, native.GetItemType<HoldableGameItemComponent>(), pile.Currency, pile.Coins.ToDictionary(x => x.Item1, x => x.Item2),
				item.GetItemType<HoldableGameItemComponent>().HeldBy, item.ContainedIn, native.DirectLocation,
				item.RoomLayer, item.RoutePositionMetres, item.OwnershipReference) : null;
		internal bool CustodyCurrent() => !Item.Deleted && !Item.Destroyed && ReferenceEquals(Pile.Currency, Currency) &&
			ReferenceEquals(Item.Prototype, Prototype) && ReferenceEquals(Item.GetItemType<CurrencyGameItemComponent>(), Pile) &&
			ReferenceEquals(Pile.Prototype, PilePrototype) && ReferenceEquals(Item.GetItemType<HoldableGameItemComponent>(), Holdable) &&
			ReferenceEquals(Holdable.HeldBy, Holder) &&
			ReferenceEquals(Item.ContainedIn, Container) && ReferenceEquals(Item.DirectLocation, Cell) &&
			Item.RoomLayer == Layer && Item.RoutePositionMetres == Coordinate && Item.OwnershipReference == Owner;
		internal bool MembershipCurrent() => Holder is not null
			? Holder.HeldOrWieldedItems.Any(x => ReferenceEquals(x, Item))
			: Container is not null ? Container.GetItemType<IContainer>()?.Contents.Any(x => ReferenceEquals(x, Item)) == true
			: Cell?.GameItems.Any(x => ReferenceEquals(x, Item)) == true;
		internal bool CountsCurrent() => Pile.Coins.Count() == Coins.Count &&
			Pile.Coins.All(x => Coins.TryGetValue(x.Item1, out var count) && count == x.Item2);
	}

	// Native list-backed containers expose this exact storage through Contents. Capture it
	// before callbacks, validate the component/storage again, and mutate only the prepared list.
	private sealed record CurrencyContainer(GameItemComponent Component, IContainer Container,
		GameItemComponent StorageOwner, IGameItemProto Prototype, IGameItemComponentProto ComponentPrototype, IGameItemComponentProto StoragePrototype,
		List<IGameItem> Storage, IGameItem[] Contents, ICell? Cell, RoomLayer Layer, double? Coordinate,
		IGameItem? ParentContainer, IBody? Holder, ICharacter? HolderActor, IOpenable? Openable, bool? IsOpen)
	{
		internal static CurrencyContainer? Capture(IGameItem? item)
		{
			if (item?.GetItemType<IContainer>() is not GameItemComponent component ||
				component is not IContainer container || container.Contents is not List<IGameItem> storage) return null;
			var storageOwner = component is VehicleCargoSpaceGameItemComponent
				? item.Components.OfType<IContainer>().FirstOrDefault(x => !ReferenceEquals(x, component)) as GameItemComponent : component;
			if (storageOwner is not IContainer backing || !ReferenceEquals(backing.Contents, storage)) return null;
			return new(component, container, storageOwner, item.Prototype, component.Prototype, storageOwner.Prototype, storage, storage.ToArray(), item.Location, item.RoomLayer,
				item.RoutePositionMetres, item.ContainedIn, item.InInventoryOf, item.InInventoryOf?.Actor,
				item.GetItemType<IOpenable>(), item.GetItemType<IOpenable>()?.IsOpen);
		}
		internal bool Current() => Component.Parent is { Deleted: false, Destroyed: false } item &&
			ReferenceEquals(item.Prototype, Prototype) && ReferenceEquals(Component.Prototype, ComponentPrototype) &&
			ReferenceEquals(StorageOwner.Prototype, StoragePrototype) &&
			ReferenceEquals(item.GetItemType<IContainer>(), Container) && ReferenceEquals(Container.Contents, Storage) &&
			ReferenceEquals(item.GetItemType<IOpenable>(), Openable) && Openable?.IsOpen == IsOpen &&
			item.Components.Any(x => ReferenceEquals(x, StorageOwner)) && StorageOwner is IContainer backing && ReferenceEquals(backing.Contents, Storage) &&
			Storage.Count == Contents.Length && Storage.Zip(Contents).All(x => ReferenceEquals(x.First, x.Second)) &&
			ReferenceEquals(item.Location, Cell) && item.RoomLayer == Layer && item.RoutePositionMetres == Coordinate &&
			ReferenceEquals(item.ContainedIn, ParentContainer) && ReferenceEquals(item.InInventoryOf, Holder) &&
			(Holder is null || ReferenceEquals(Holder.Actor, HolderActor) && ReferenceEquals(HolderActor?.Body, Holder));
	}

	private GameItem? TransferPreparedCurrency(ICurrency currency, decimal amount, bool exact,
		CurrencyTransferKind kind, IGameItem? containerItem = null, ICharacter? containerOwner = null,
		IBody? recipient = null, ICorpse? corpse = null, bool newStack = false,
		IEmote? playerEmote = null, bool silent = false, IEnumerable<IHandleEvents>? witnessHandlers = null)
	{
		var executor = Actor;
		using var execution = CommandExecutionScope.EnterBodyOperation(executor);
		var origin = RouteSpatialService.Instance.GetEffectiveLocation(executor);
		var recipientBody = kind == CurrencyTransferKind.Give ? recipient as Body : null;
		var recipientActor = recipientBody?.Actor;
		var recipientPoint = recipientActor is null ? (SpatialLocation?)null : RouteSpatialService.Instance.GetEffectiveLocation(recipientActor);
		var recipientWasCanonical = recipientActor is not null && ReferenceEquals(recipientActor.Body, recipientBody);
		var capturedCorpseBody = corpse?.Body;
		var capturedCorpseParent = corpse?.Parent;
		var corpsePoint = capturedCorpseParent is null ? (SpatialLocation?)null : RouteSpatialService.Instance.GetEffectiveLocation(capturedCorpseParent);
		var container = CurrencyContainer.Capture(containerItem);
		bool Active() => CommandExecutionScope.TryContinue(executor) &&
			ReferenceEquals(Actor, executor) && ReferenceEquals(executor.Body, this) &&
			ReferenceEquals(Location, origin.Cell) && RoomLayer == origin.Layer && executor.RoutePositionMetres == origin.RoutePositionMetres &&
			(containerItem is null || container?.Current() == true) &&
			(recipientBody is null || ReferenceEquals(recipientBody.Actor, recipientActor) &&
				ReferenceEquals(recipientActor!.Location, recipientPoint!.Value.Cell) && recipientActor.RoomLayer == recipientPoint.Value.Layer &&
				recipientActor.RoutePositionMetres == recipientPoint.Value.RoutePositionMetres &&
				(corpse is not null ? !capturedCorpseParent!.Deleted && !capturedCorpseParent.Destroyed && ReferenceEquals(corpse.Body, capturedCorpseBody) &&
					ReferenceEquals(capturedCorpseParent.Location, corpsePoint!.Value.Cell) && capturedCorpseParent.RoomLayer == corpsePoint.Value.Layer &&
					capturedCorpseParent.RoutePositionMetres == corpsePoint.Value.RoutePositionMetres
					: recipientWasCanonical && ReferenceEquals(recipientActor.Body, recipientBody) && CommandExecutionAuthority.IsCurrent(recipientActor, false)));
		if (!Active() || amount <= 0 || kind == CurrencyTransferKind.Give && recipientBody is null) return null;
		var parents = kind switch
		{
			CurrencyTransferKind.GetRoom => Location.LayerGameItems(RoomLayer).ToArray(),
			CurrencyTransferKind.GetContainer => container!.Contents,
			_ => HeldItems.ToArray()
		};
		var states = new List<CurrencyState>();
		foreach (var parent in parents)
		{
			if (!Active()) return null;
			if (parent.GetItemType<ICurrencyPile>() is not { } pile || !ReferenceEquals(pile.Currency, currency)) continue;
			var state = CurrencyState.Capture(parent);
			if (state is null) return null;
			var eligible = kind switch
			{
				CurrencyTransferKind.GetRoom => CanGet(parent, 0, ItemCanGetIgnore.IgnoreWeight),
				CurrencyTransferKind.GetContainer => CanGet(parent, containerItem!, 0, ItemCanGetIgnore.IgnoreWeight),
				_ => ReferenceEquals(state.Holder, this) && _heldItems.Any(x => ReferenceEquals(x.Item1, parent))
			};
			if (!Active() || !state.CustodyCurrent() || !state.CountsCurrent()) return null;
			if (eligible) states.Add(state);
		}
		var selected = FindCurrencyPreservingOwnership(currency, states.Select(x => (ICurrencyPile)x.Pile), amount);
		if (!Active() || selected.Count == 0 || states.Any(x => !x.CustodyCurrent() || !x.CountsCurrent())) return null;
		var debits = new List<(CurrencyState State, Dictionary<ICoin, int> Coins)>();
		foreach (var selection in selected)
		{
			var state = states.SingleOrDefault(x => ReferenceEquals(x.Pile, selection.Key));
			if (state is null || selection.Value.Count == 0 || selection.Value.Any(x => x.Value <= 0 || !state.Coins.TryGetValue(x.Key, out var count) || x.Value > count)) return null;
			debits.Add((state, new(selection.Value)));
		}
		var owner = debits[0].State.Owner;
		if (debits.Any(x => x.State.Owner != owner) || debits.Select(x => x.State.Pile).Distinct(ReferenceEqualityComparer.Instance).Count() != debits.Count) return null;
		var denominationTotals = debits.SelectMany(x => x.Coins).GroupBy(x => x.Key)
			.ToDictionary(x => x.Key, x => x.Sum(y => (long)y.Value));
		if (denominationTotals.Any(x => x.Value > int.MaxValue)) return null;
		var total = denominationTotals.Sum(x => x.Key.Value * x.Value);
		if (total <= 0 || exact && total != amount) return null;
		var draft = (GameItem)CreateCurrencyPileFromSelection(currency, selected, true);
		var allowed = kind switch
		{
			CurrencyTransferKind.GetRoom or CurrencyTransferKind.GetContainer => CanGet(draft, 0),
			CurrencyTransferKind.Put => CanPut(draft, containerItem!, containerOwner, 0, false),
			CurrencyTransferKind.Drop => CanDrop(draft, 0),
			_ => corpse is null ? CanGive(draft, recipientBody!) : CanGive(draft, corpse)
		};
		if (!Active() || debits.Any(x => !x.State.CustodyCurrent() || !x.State.CountsCurrent())) return null;
		if (!allowed)
		{
			var reason = kind switch
			{
				CurrencyTransferKind.GetRoom or CurrencyTransferKind.GetContainer => WhyCannotGet(draft, 0),
				CurrencyTransferKind.Put => WhyCannotPut(draft, containerItem!, containerOwner, 0, false),
				CurrencyTransferKind.Drop => WhyCannotDrop(draft, 0),
				_ => corpse is null ? WhyCannotGive(draft, recipientBody!) : WhyCannotGive(draft, corpse)
			};
			if (!silent && Active()) OutputHandler.Send(reason);
			return null;
		}
		var destinationBody = kind is CurrencyTransferKind.GetRoom or CurrencyTransferKind.GetContainer ? this : recipientBody;
		var bodyOwner = destinationBody?.Actor;
		var get = destinationBody?.PrepareGetPlacement(draft, true);
		if (destinationBody is not null && get is null) return null;
		CurrencyState? merge = get?.Merge is { } bodyMerge ? CurrencyState.Capture(bodyMerge) : null;
		var floor = kind == CurrencyTransferKind.Drop ? origin.Cell as Cell : null;
		var floorPoint = origin;
		if (kind == CurrencyTransferKind.Drop)
		{
			if (floor is null) return null;
			// Prepare layer/proximity/merge queries before any money or custody changes.
			draft.SetPreparedCurrencyCustody(null, null, origin);
			floorPoint = new(origin.Cell, floor.PrepareCurrencyInsertionLayer(draft), origin.RoutePositionMetres);
			if (!Active()) return null;
			if (!newStack)
				foreach (var candidate in floor.LayerGameItems(floorPoint.Layer).ToArray())
				{
					if (!Active()) return null;
					var nearby = floor.RouteDefinition is null || RouteSpatialService.Instance.GetProximity(draft, candidate) <= Proximity.Immediate;
					var canMerge = nearby && draft.CanMerge(candidate);
					if (!Active()) return null;
					if (canMerge) { merge = CurrencyState.Capture(candidate); if (merge is null) return null; break; }
				}
			draft.SetPreparedCurrencyCustody(null, null, null);
		}
		if (kind == CurrencyTransferKind.Put)
			foreach (var candidate in container!.Contents)
			{
				var canMerge = candidate.CanMerge(draft);
				if (!Active()) return null;
				if (canMerge) { merge = CurrencyState.Capture(candidate); if (merge is null) return null; break; }
			}
		if (get?.Merge is not null && merge is null) return null;
		if (merge is not null && (!merge.CustodyCurrent() || !merge.CountsCurrent() || merge.Owner != owner || !ReferenceEquals(merge.Currency, currency))) return null;
		var description = get?.Hand is null ? null : $"{"<held in " + destinationBody!.DescribeBodypartGroup(new[] { get.Hand }) + ">",-35}";
		var witnesses = (witnessHandlers ?? Location.EventHandlersFor(executor)).ToArray();
		var externalWitnesses = witnessHandlers is null ? ExternalItems.ToArray() : witnesses.OfType<IGameItem>().ToArray();
		if (!Active() || !RouteSpatialService.Instance.TryValidateLocation(floorPoint, out _)) return null;
		var survivor = merge?.Item ?? draft;
		var survivorPile = survivor.GetItemType<CurrencyGameItemComponent>();
		var maps = new Dictionary<CurrencyGameItemComponent, Dictionary<ICoin, int>>(ReferenceEqualityComparer.Instance);
		foreach (var debit in debits) maps.Add(debit.State.Pile, new(debit.State.Coins));
		foreach (var debit in debits)
			foreach (var coin in debit.Coins)
			{
				var remaining = maps[debit.State.Pile][coin.Key] - coin.Value;
				if (remaining == 0) maps[debit.State.Pile].Remove(coin.Key); else maps[debit.State.Pile][coin.Key] = remaining;
			}
		if (!maps.TryGetValue(survivorPile, out var survivorCoins))
			maps[survivorPile] = survivorCoins = merge is null ? [] : new(merge.Coins);
		foreach (var coin in denominationTotals)
		{
			survivorCoins.TryGetValue(coin.Key, out var previous);
			if (previous > int.MaxValue - coin.Value) return null;
			survivorCoins[coin.Key] = checked(previous + (int)coin.Value);
		}
		var draftPile = draft.GetItemType<CurrencyGameItemComponent>();
		if (merge is not null) maps[draftPile] = [];
		bool PreparedDestinationCurrent() => destinationBody is null || ReferenceEquals(destinationBody.Actor, bodyOwner) &&
			(get!.Merge is not null ? destinationBody.HeldOrWieldedItems.Any(x => ReferenceEquals(x, survivor)) :
				get.Hand is not null && !destinationBody._heldItems.Any(x => ReferenceEquals(x.Item2, get.Hand)) &&
				!destinationBody._wieldedItems.Any(x => ReferenceEquals(x.Item2, get.Hand)));
		using var proximityChange = Gameworld.ProximityEventService?.BeginChange(ProximityChangeCause.Containment,
			debits.Select(x => (IPerceivable)x.State.Item).Append(draft).Append(survivor).Distinct().ToArray());
		// This is the final gate. No gameplay callback runs among the following denomination/custody writes.
		bool PreparedCurrent() => Active() && PreparedDestinationCurrent() &&
			debits.All(x => x.State.CustodyCurrent() && x.State.CountsCurrent() && x.State.MembershipCurrent()) &&
			(merge is null || merge.CustodyCurrent() && merge.CountsCurrent() && merge.MembershipCurrent());
		if (!PreparedCurrent()) return null;
		foreach (var debit in debits) ForeignCustodyTransferContext.EnsureItem(debit.State.Item, destructive: true);
		ForeignCustodyTransferContext.EnsureItem(survivor, destructive: true);
		if (!PreparedCurrent()) return null;
		CommandExecutionScope.MarkCommitted(executor);
		foreach (var map in maps) map.Key.ReplacePreparedCoins(map.Value);
		if (merge is null)
		{
			if (destinationBody is not null)
			{
				draft.SetPreparedCurrencyCustody(destinationBody, null, null);
				destinationBody._heldItems.Add(Tuple.Create((IGameItem)draft, get!.Hand!));
				draft.GetItemType<HoldableGameItemComponent>().CurrentInventoryDescription = description!;
			}
			else if (kind == CurrencyTransferKind.Put)
			{
				draft.SetPreparedCurrencyCustody(null, containerItem, null);
				container!.Storage.Add(draft);
			}
			else
			{
				draft.SetPreparedCurrencyCustody(null, null, floorPoint);
				floor!.SetPreparedCurrencyCellMembership(draft, true);
			}
		}
		// All value and destination membership is coherent before save queues and observers run.
		foreach (var map in maps) map.Key.MarkPreparedCoinsChanged();
		InventoryChanged = true;
		if (destinationBody is not null) destinationBody.InventoryChanged = true;
		if (kind == CurrencyTransferKind.Put) container!.StorageOwner.Changed = true;
		if (floor is not null) floor.ContentsChanged = true;
		draft.ActivateCurrencySplit();
		Gameworld.Add(draft);
		RouteSpatialService.Instance.TrackPerceivable(draft);
		// Merged candidates retain their normal lifecycle too. Their coins have already
		// been consumed, so an observer can never see both piles funded by this transfer.
		draft.FinishCurrencySplitLoading(() => !draft.Deleted && !draft.Destroyed);
		if (!draft.Deleted && !draft.Destroyed) draft.NotifyCommittedCurrencyOwner(debits[0].State.Item);
		if (merge is null && destinationBody is not null)
		{
			if (!draft.Deleted && !draft.Destroyed && ReferenceEquals(draft.InInventoryOf, destinationBody) &&
				draft.ContainedIn is null && draft.DirectLocation is null) draft.Get(destinationBody);
			if (draft.Deleted || draft.Destroyed || !ReferenceEquals(draft.InInventoryOf, destinationBody) ||
				draft.ContainedIn is not null || draft.DirectLocation is not null)
			{
				destinationBody._heldItems.RemoveAll(x => ReferenceEquals(x.Item1, draft));
				destinationBody._wieldedItems.RemoveAll(x => ReferenceEquals(x.Item1, draft));
				destinationBody.InventoryChanged = true;
			}
		}
		// Cleanup belongs to the committed transfer. Delete observers may refill/relocate an empty pile.
		foreach (var debit in debits)
		{
			var state = debit.State;
			if (ReferenceEquals(state.Item, survivor) || state.Pile.Coins.Any()) continue;
			state.Item.DeleteCommittedEmptyCurrency(() => state.CustodyCurrent() && !state.Pile.Coins.Any(), () =>
			{
				if (state.Holder is Body sourceBody)
				{
					sourceBody._heldItems.RemoveAll(x => ReferenceEquals(x.Item1, state.Item));
					sourceBody._wieldedItems.RemoveAll(x => ReferenceEquals(x.Item1, state.Item));
					sourceBody.InventoryChanged = true;
				}
				if (state.Container is not null && container is not null)
				{
					container.Storage.RemoveAll(x => ReferenceEquals(x, state.Item));
					container.StorageOwner.Changed = true;
				}
				if (state.Cell is Cell sourceCell) { sourceCell.SetPreparedCurrencyCellMembership(state.Item, false); sourceCell.ContentsChanged = true; }
				state.Item.SetPreparedCurrencyCustody(null, null, null);
			});
		}
		if (merge is not null)
		{
			survivor.NotifyCommittedCurrencyMerge(draft);
			draft.DeleteCommittedEmptyCurrency(() => ComponentItemTransfer.IsDetached(draft) && !draftPile.Coins.Any(), () => { });
		}
		if (merge is not null && !draft.Deleted && !draft.Destroyed && ComponentItemTransfer.IsDetached(draft) && draftPile.Coins.Any())
		{
			// A loading/delete observer may refill the absorbed candidate. Preserve that
			// new value at the captured floor; a callback's existing custody claim wins.
			draft.SetPreparedCurrencyCustody(null, null, origin);
			if (origin.Cell is Cell completionCell) { completionCell.SetPreparedCurrencyCellMembership(draft, true); completionCell.ContentsChanged = true; }
			draft.Changed = true;
			RouteSpatialService.Instance.TrackPerceivable(draft);
		}
		foreach (var debit in debits) if (!debit.State.Item.Deleted) debit.State.Pile.NotifyCurrencyTransferDebit();
		if (!survivor.Deleted && !survivor.Destroyed) survivorPile.NotifyCurrencyTransferDebit();
		if (floor is not null && merge is null) floor.FinishPreparedCurrencyInsertion(survivor, floorPoint);
		proximityChange?.Complete();
		PublishCurrencyTransfer(kind, survivor, executor, containerItem, containerOwner, recipientActor, corpse, playerEmote, silent, witnesses, externalWitnesses);
		return survivor.Deleted || survivor.Destroyed ? null : survivor;
	}

	private void PublishCurrencyTransfer(CurrencyTransferKind kind, GameItem item, ICharacter executor,
		IGameItem? container, ICharacter? containerOwner, ICharacter? recipient, ICorpse? corpse, IEmote? playerEmote, bool silent,
		IHandleEvents[] witnesses, IGameItem[] externalWitnesses)
	{
		if (item.Deleted || item.Destroyed) return;
		var output = new MixedEmoteOutput(kind switch
		{
			CurrencyTransferKind.GetRoom => new Emote("@ get|gets $0", executor, item),
			CurrencyTransferKind.GetContainer => new Emote("@ get|gets $0 from $1", executor, item, container!),
			CurrencyTransferKind.Put => containerOwner is null ? new Emote("@ put|puts $0 in $1", executor, item, container!) :
				new Emote("@ put|puts $0 in $2's !1", executor, item, container!, containerOwner),
			CurrencyTransferKind.Drop => new Emote("@ drop|drops $0", executor, item),
			_ => new Emote("@ give|gives $0 to $1", executor, item, corpse?.Parent ?? (IPerceivable)recipient!)
		}, flags: OutputFlags.SuppressObscured);
		output.Append(playerEmote);
		if (!silent) executor.OutputHandler.Handle(output);
		var oldState = kind == CurrencyTransferKind.GetContainer ? InventoryState.InContainer :
			kind == CurrencyTransferKind.GetRoom ? InventoryState.Dropped : InventoryState.Held;
		var newState = kind is CurrencyTransferKind.GetRoom or CurrencyTransferKind.GetContainer ? InventoryState.Held :
			kind == CurrencyTransferKind.Put ? InventoryState.InContainer : InventoryState.Dropped;
		OnInventoryChange?.Invoke(oldState, newState, item);
		item.InvokeInventoryChange(oldState, newState);
		if (kind == CurrencyTransferKind.Give)
		{
			var receiver = corpse is null ? recipient : corpse.GetOriginalCharacterWithMatchingBody();
			if (receiver is not null)
			{
				executor.HandleEvent(EventType.CharacterGiveItemGiver, executor, receiver, item);
				receiver.HandleEvent(EventType.CharacterGiveItemReceiver, executor, receiver, item);
				item.HandleEvent(EventType.ItemGiven, executor, receiver, item);
				foreach (var witness in witnesses.Concat(externalWitnesses).Distinct().Where(x => !ReferenceEquals(x, executor) && !ReferenceEquals(x, receiver)))
					witness.HandleEvent(EventType.CharacterGiveItemWitness, executor, receiver, item, witness);
			}
		}
		else
		{
			var characterEvent = kind switch { CurrencyTransferKind.GetRoom => EventType.CharacterGotItem, CurrencyTransferKind.GetContainer => EventType.CharacterGotItemContainer, CurrencyTransferKind.Put => EventType.CharacterPutItemContainer, _ => EventType.CharacterDroppedItem };
			var itemEvent = kind switch { CurrencyTransferKind.GetRoom => EventType.ItemGotten, CurrencyTransferKind.GetContainer => EventType.ItemGottenContainer, CurrencyTransferKind.Put => EventType.ItemPutContainer, _ => EventType.ItemDropped };
			var witnessEvent = kind switch { CurrencyTransferKind.GetRoom => EventType.CharacterGotItemWitness, CurrencyTransferKind.GetContainer => EventType.CharacterGotItemContainerWitness, CurrencyTransferKind.Put => EventType.CharacterPutItemContainerWitness, _ => EventType.CharacterDroppedItemWitness };
			if (container is null) { executor.HandleEvent(characterEvent, executor, item); item.HandleEvent(itemEvent, executor, item); }
			else { executor.HandleEvent(characterEvent, executor, item, container); item.HandleEvent(itemEvent, executor, item, container); }
			foreach (var witness in witnesses.Concat(externalWitnesses).Distinct().Where(x => !ReferenceEquals(x, executor) && !ReferenceEquals(x, item) && !ReferenceEquals(x, container)))
				if (container is null) witness.HandleEvent(witnessEvent, executor, item, witness);
				else witness.HandleEvent(witnessEvent, executor, item, container, witness);
		}
		if (ReferenceEquals(Actor, executor) && ReferenceEquals(executor.Body, this)) CheckConsequences();
	}
}

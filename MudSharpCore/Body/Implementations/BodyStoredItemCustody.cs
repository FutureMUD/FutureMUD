#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.NPC.AI;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	internal IGrab? PrepareStoredItemReceipt(GameItem item) =>
		CanGet(item, 0, ItemCanGetIgnore.None, false) ? PrepareGetPlacement(item, false)?.Hand : null;

	internal bool StoredItemReceiptCurrent(IGrab hand) => HoldLocs.Any(x => ReferenceEquals(x, hand)) &&
		(!hand.Unary || !_heldItems.Any(x => ReferenceEquals(x.Item2, hand)) &&
			!_wieldedItems.Any(x => ReferenceEquals(x.Item2, hand)));

	internal void PublishStoredItemReceipt(GameItem item, IGrab hand, string description)
	{
		_heldItems.Add(Tuple.Create((IGameItem)item, hand));
		item.SetPreparedHeldCustody(this);
		item.GetItemType<HoldableGameItemComponent>().CurrentInventoryDescription = description;
		InventoryChanged = true;
	}

	internal void NotifyStoredItemReceipt(GameItem item)
	{
		if (item.Deleted || item.Destroyed || !ReferenceEquals(item.InInventoryOf, this)) return;
		item.Get(this);
		if (!item.Deleted && !item.Destroyed && ReferenceEquals(item.InInventoryOf, this)) NotifyGotItem(item, Actor);
	}

	/// <summary>Prepares a held parcel's removal without running inventory callbacks during payment publication.</summary>
	internal ICurrencyTransferParticipant? PrepareStoredItemCustody(ICharacter seller, GameItem item, Room destination)
	{
		var participant = new HeldItemCustody(this, seller, item, destination);
		return participant.Prepare() ? participant : null;
	}

	private sealed class HeldItemCustody : ICurrencyTransferParticipant
	{
		private readonly Body _body;
		private readonly ICharacter _actor;
		private readonly ICharacter _seller;
		private readonly IBody _sellerBody;
		private readonly GameItem _item;
		private readonly IGameItemProto _prototype;
		private readonly HoldableGameItemComponent? _holdable;
		private readonly Room _destination;
		private readonly SpatialLocation _origin;
		private readonly int _quantity;
		private RoomLayer _destinationLayer;
		private bool _prepared;
		private bool _committed;

		internal HeldItemCustody(Body body, ICharacter seller, GameItem item, Room destination)
		{
			_body = body;
			_actor = body.Actor;
			_seller = seller;
			_sellerBody = seller.Body;
			_item = item;
			_prototype = item.Prototype;
			_holdable = item.GetItemType<HoldableGameItemComponent>();
			_destination = destination;
			_origin = RouteSpatialService.Instance.GetEffectiveLocation(_actor);
			_quantity = item.Quantity;
		}

		private bool Current => !_committed && ReferenceEquals(_body.Actor, _actor) && ReferenceEquals(_actor.Body, _body) &&
			CommandExecutionScope.TryContinue(_actor) && ReferenceEquals(_seller.Body, _sellerBody) &&
			ReferenceEquals(_sellerBody.Actor, _seller) && !ReferenceEquals(_body, _sellerBody) &&
			_actor.State.IsAble() && _seller.State.IsAwake() && _seller.Combat is null && _actor.ColocatedWith(_seller) &&
			RouteSpatialService.Instance.GetEffectiveLocation(_actor) == _origin && _origin.Room.RouteDefinition is null &&
			_destination.RouteDefinition is null && ReferenceEquals(_destination.Gameworld, _body.Gameworld) &&
			ReferenceEquals(_seller.Gameworld, _body.Gameworld) && ReferenceEquals(_item.Gameworld, _body.Gameworld) &&
			!_item.Deleted && !_item.Destroyed && ReferenceEquals(_item.Prototype, _prototype) && _item.Quantity == _quantity &&
			_holdable is not null && ReferenceEquals(_item.GetItemType<HoldableGameItemComponent>(), _holdable) &&
			ReferenceEquals(_holdable.HeldBy, _body) && _item.DirectLocation is null && _item.ContainedIn is null &&
			_body._heldItems.Any(x => ReferenceEquals(x.Item1, _item)) &&
			!_body._wieldedItems.Any(x => ReferenceEquals(x.Item1, _item)) &&
			!_body._wornItems.Any(x => ReferenceEquals(x.Item, _item)) &&
			_item.GetItemType<ICurrencyPile>() is null && PortableTree(_item);

		private static bool PortableTree(IGameItem item)
		{
			var pending = new Stack<IGameItem>();
			var seen = new HashSet<IGameItem>(ReferenceEqualityComparer.Instance);
			pending.Push(item);
			while (pending.Count > 0)
			{
				var candidate = pending.Pop();
				if (!seen.Add(candidate) || candidate.Deleted || candidate.Destroyed ||
					candidate.AttachedAndConnectedItems.Any() || candidate.GetItemType<IVehicleExterior>() is not null ||
					candidate.GetItemType<DwellingGameItemComponent>() is not null) return false;
				foreach (var child in candidate.GetItemType<IContainer>()?.Contents ?? []) pending.Push(child);
			}
			return true;
		}

		public bool IsCurrent => _prepared && Current &&
			_destination.CurrentOverlay.Terrain.TerrainLayers.Contains(_destinationLayer);

		public bool Prepare()
		{
			if (_prepared) return IsCurrent;
			if (!Current || !_body.CanDrop(_item, 0)) return false;
			_destinationLayer = _destination.PrepareCurrencyInsertionLayer(_item);
			if (!Current || !_destination.CurrentOverlay.Terrain.TerrainLayers.Contains(_destinationLayer)) return false;
			ForeignCustodyTransferContext.EnsureBody(_body, _item);
			_prepared = true;
			return IsCurrent;
		}

		public void Commit()
		{
			_body._heldItems.RemoveAll(x => ReferenceEquals(x.Item1, _item));
			_item.SetPreparedCurrencyCustody(null, null, new SpatialLocation(_destination, _destinationLayer, null));
			_destination.SetPreparedCurrencyRoomMembership(_item, true);
			_body.InventoryChanged = true;
			_destination.ContentsChanged = true;
			_item.Changed = true;
			_committed = true;
		}

		public void Notify()
		{
			if (!_committed) return;
			foreach (var component in _item.Components.ToArray())
			{
				if (_item.Deleted || _item.Destroyed || !ReferenceEquals(_item.DirectLocation, _destination)) break;
				component.Taken();
			}
			_body.OnInventoryChange?.Invoke(InventoryState.Held, InventoryState.Dropped, _item);
			_item.InvokeInventoryChange(InventoryState.Held, InventoryState.Dropped);
			if (_item.Deleted || _item.Destroyed || !ReferenceEquals(_item.DirectLocation, _destination)) return;
			RouteSpatialService.Instance.TrackPerceivable(_item);
			_destination.FinishPreparedCurrencyInsertion(_item, new SpatialLocation(_destination, _destinationLayer, null));
		}

		public void Dispose() { }
	}
}

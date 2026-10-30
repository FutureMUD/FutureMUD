#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Body;
using NativeBody = MudSharp.Body.Implementations.Body;
using MudSharp.Economy.Currency;
using MudSharp.Form.Material;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Size;
using MudSharp.GameItems.Prototypes;
using MudSharp.NPC.AI;
using NativeGameItem = MudSharp.GameItems.GameItem;

namespace MudSharp.FutureProg.Functions.GameItem;

/// <summary>
/// Atomically sells one native, unconnected room-stock wearable and delivers it to the buyer's room.
/// Depot access and ticket authorization remain the responsibility of the calling prog.
/// </summary>
internal sealed class PurchaseStoredItemFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new BooleanVariable(false);
		WearableFit? requestedFit = null;
		if (ParameterFunctions[0].Result?.GetObject is not ICharacter buyer || buyer.Body is not { } buyerBody ||
			ParameterFunctions[1].Result?.GetObject is not ICharacter seller || seller.Body is not { } sellerBody ||
			ParameterFunctions[2].Result?.GetObject is not NativeGameItem item ||
			ParameterFunctions[3].Result?.GetObject is not Room sourceRoom ||
			ParameterFunctions[4].Result?.GetObject is not ICurrency currency ||
			ParameterFunctions[5].Result?.GetObject is not decimal price || price < 0 ||
			item.GetItemType<HoldableGameItemComponent>() is not { } holdable ||
			buyer.Body is null || seller.Body is null || ReferenceEquals(buyerBody, sellerBody) ||
			!buyer.State.IsAble() || !seller.State.IsAwake() || seller.Combat is not null ||
			!buyer.ColocatedWith(seller) || buyer.Location is not Room destinationRoom ||
			buyer.Location.RouteDefinition is not null || sourceRoom.RouteDefinition is not null ||
			!ReferenceEquals(buyer.Gameworld, seller.Gameworld) || !ReferenceEquals(buyer.Gameworld, item.Gameworld) ||
			!ReferenceEquals(buyer.Gameworld, sourceRoom.Gameworld))
		{
			return StatementResult.Normal;
		}
		var wearable = item.GetItemType<WearableGameItemComponent>();
		var requireInventory = ParameterFunctions.Count == 7 && ParameterFunctions[6].Result?.GetObject is true;
		if (ParameterFunctions.Count == 7 && ParameterFunctions[6].Result?.GetObject is not bool)
		{
			if (wearable?.UsesSizing != true || ParameterFunctions[6].Result?.GetObject is not string fitText ||
				!WearableSizing.TryParse(fitText, null, item.Gameworld, out var fit, out _) ||
				fit.Kind is WearableFitKind.Unfitted or WearableFitKind.Universal) return StatementResult.Normal;
			requestedFit = fit;
		}

		using var delivery = new PreparedStoredItemDelivery(buyer, seller, item, wearable, sourceRoom, destinationRoom,
			buyerBody, sellerBody, holdable, price, requestedFit, requireInventory);
		using var exposure = EnvironmentalExposureService.Changing(item);
		if (price == 0)
		{
			if (!delivery.Prepare() || !delivery.IsCurrent) return StatementResult.Normal;
			delivery.Commit();
			delivery.Notify();
			Result = new BooleanVariable(true);
			return StatementResult.Normal;
		}

		Result = new BooleanVariable(buyerBody.TryGiveCurrency(currency, sellerBody, price, true, delivery, true));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new(
			"purchaseitemdelivery",
			[ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Location,
				ProgVariableTypes.Currency, ProgVariableTypes.Number, ProgVariableTypes.Boolean],
			(p, _) => new PurchaseStoredItemFunction(p),
			["buyer", "seller", "item", "sourceRoom", "currency", "price", "requireInventory"],
			["The buyer.", "The colocated seller.", "A native portable stored item.", "Its ordinary storage room.",
				"The currency.", "A nonnegative exact price.", "True requires native inventory capacity and delivers directly into inventory, never onto the floor."],
			"Pays and delivers stored items atomically; optional direct inventory receipt preserves native capacity and leaves refused items in private storage.",
			"Items", ProgVariableTypes.Boolean));
		FutureProg.RegisterBuiltInFunctionCompiler(new(
			"purchaseitemdelivery",
			[ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Location,
				ProgVariableTypes.Currency, ProgVariableTypes.Number],
			(p, _) => new PurchaseStoredItemFunction(p),
			["buyer", "seller", "item", "sourceRoom", "currency", "price"],
			["The buyer.", "The colocated seller.", "A native portable room-stock item.", "Its ordinary storage room.",
				"The payment currency.", "A nonnegative exact price."],
			"Atomically pays for a stored item and delivers it at the buyer's room and layer without changing its fit. Authorization belongs in the calling prog.",
			"Items", ProgVariableTypes.Boolean));
		FutureProg.RegisterBuiltInFunctionCompiler(new(
		"purchaseitemdelivery",
		[ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Location,
			ProgVariableTypes.Currency, ProgVariableTypes.Number, ProgVariableTypes.Text],
		(p, _) => new PurchaseStoredItemFunction(p),
		["buyer", "seller", "item", "sourceRoom", "currency", "price", "fit"],
		["The purchasing character.", "The awake noncombat seller receiving exact held cash.",
			"One native wearable directly present in sourceRoom.", "The ordinary room containing the shop's stock item.",
			"The payment currency.", "A nonnegative exact base-unit price.", "A concrete wearable fit snapshot accepted by WearableSizing.TryParse."],
		"Atomically pays for a native wearable in an ordinary room and delivers it beside the buyer, setting its concrete fit before item observers run. Price zero still prepares and publishes delivery. Requires buyer and seller to be colocated; ticket authorization and stock policy belong in the calling prog.",
		"Items", ProgVariableTypes.Boolean));
	}
}

/// <summary>Prepared movement of a native room-stock item through an exact cash Give.</summary>
internal sealed class PreparedStoredItemDelivery : ICurrencyTransferParticipant, IDisposable
{
	private readonly ICharacter _buyer;
	private readonly ICharacter _seller;
	private readonly NativeGameItem _item;
	private readonly WearableGameItemComponent? _wearable;
	private readonly HoldableGameItemComponent _holdable;
	private readonly IGameItemProto _itemPrototype;
	private readonly WearableGameItemComponentProto? _wearablePrototype;
	private readonly WearableFit? _initialFit;
	private readonly Room _source;
	private readonly Room _destination;
	private readonly IBody _buyerBody;
	private readonly IBody _sellerBody;
	private readonly long _buyerInstanceId;
	private readonly long _sellerInstanceId;
	private readonly long _buyerIdentityId;
	private readonly long _sellerIdentityId;
	private readonly decimal _price;
	private readonly WearableFit? _fit;
	private readonly RoomLayer _sourceLayer;
	private RoomLayer _destinationLayer;
	private readonly bool _requireInventory;
	private IGrab? _receiptHand;
	private string _receiptDescription = string.Empty;
	private double _receiptItemWeight;
	private double _receiptCarriedWeight;
	private bool _prepared;
	private bool _committed;

	internal PreparedStoredItemDelivery(ICharacter buyer, ICharacter seller, NativeGameItem item,
		WearableGameItemComponent? wearable, Room source, Room destination, IBody buyerBody, IBody sellerBody,
		HoldableGameItemComponent holdable, decimal price, WearableFit? fit, bool requireInventory = false)
	{
		_buyer = buyer;
		_seller = seller;
		_item = item;
		_wearable = wearable;
		_holdable = holdable;
		_itemPrototype = item.Prototype;
		_wearablePrototype = wearable?.Prototype as WearableGameItemComponentProto;
		_initialFit = wearable?.Fit;
		_source = source;
		_destination = destination;
		_buyerBody = buyerBody;
		_sellerBody = sellerBody;
		_buyerInstanceId = buyer.InstanceId;
		_sellerInstanceId = seller.InstanceId;
		_buyerIdentityId = CharacterInstanceIdentityComparer.IdentityId(buyer);
		_sellerIdentityId = CharacterInstanceIdentityComparer.IdentityId(seller);
		_price = price;
		_fit = fit;
		_sourceLayer = item.RoomLayer;
		_requireInventory = requireInventory;
	}

	private bool CharactersCurrent => !ReferenceEquals(_buyer, _seller) &&
		ReferenceEquals(_buyer.Body, _buyerBody) && ReferenceEquals(_seller.Body, _sellerBody) &&
		ReferenceEquals(_buyerBody.Actor, _buyer) && ReferenceEquals(_sellerBody.Actor, _seller) &&
		CharacterInstanceIdentityComparer.SamePhysicalInstance(_buyer, _buyerBody) &&
		CharacterInstanceIdentityComparer.SamePhysicalInstance(_seller, _sellerBody) &&
		_buyer.InstanceId == _buyerInstanceId && _seller.InstanceId == _sellerInstanceId &&
		CharacterInstanceIdentityComparer.IdentityId(_buyer) == _buyerIdentityId &&
		CharacterInstanceIdentityComparer.IdentityId(_seller) == _sellerIdentityId &&
		CommandExecutionScope.TryContinue(_buyer) &&
		_buyer.State.IsAble() && _seller.State.IsAwake() && _seller.Combat is null && _buyer.ColocatedWith(_seller) &&
		ReferenceEquals(_buyer.Location, _destination) && _buyer.Location?.RouteDefinition is null &&
		ReferenceEquals(_buyer.Gameworld, _item.Gameworld) && ReferenceEquals(_seller.Gameworld, _item.Gameworld) &&
		ReferenceEquals(_source.Gameworld, _item.Gameworld) && ReferenceEquals(_destination.Gameworld, _item.Gameworld);

	private bool StockCurrent => !_item.Deleted && !_item.Destroyed &&
		ReferenceEquals(_item.DirectLocation, _source) && _item.InInventoryOf is null && _item.ContainedIn is null &&
		ReferenceEquals(_item.Gameworld, _destination.Gameworld) && ReferenceEquals(_item.Prototype, _itemPrototype) &&
		_item.RoomLayer == _sourceLayer && ReferenceEquals(_item.GetItemType<WearableGameItemComponent>(), _wearable) &&
		ReferenceEquals(_item.GetItemType<HoldableGameItemComponent>(), _holdable) &&
		ReferenceEquals(_wearable?.Prototype, _wearablePrototype) && _wearable?.Fit == _initialFit &&
		_source.LayerGameItems(_sourceLayer).Any(x => ReferenceEquals(x, _item)) &&
		!_item.AttachedAndConnectedItems.Any() && (_fit is null || _wearable?.UsesSizing == true) &&
		_wearable?.WornBy is null && _item.GetItemType<IVehicleExterior>() is null && _item.GetItemType<DwellingGameItemComponent>() is null;

	public bool IsCurrent => _prepared && !_committed && CharactersCurrent && StockCurrent &&
		_buyer.RoomLayer == _destinationLayer &&
		(!_requireInventory || _buyerBody is NativeBody body && _receiptHand is not null && body.StoredItemReceiptCurrent(_receiptHand) &&
			_item.Weight == _receiptItemWeight && body.CarriedItems.Sum(x => x.Weight) == _receiptCarriedWeight) &&
		_source.RouteDefinition is null && _destination.RouteDefinition is null && _price >= 0;

	public bool Prepare()
	{
		if (_prepared) return IsCurrent;
		if (!CharactersCurrent || !StockCurrent || _source.RouteDefinition is not null || _destination.RouteDefinition is not null)
			return false;
		_destinationLayer = _buyer.RoomLayer;
		if (_requireInventory)
		{
			if (_buyerBody is not NativeBody body || (_receiptHand = body.PrepareStoredItemReceipt(_item)) is null) return false;
			_receiptDescription = $"{"<held in " + body.DescribeBodypartGroup([_receiptHand]) + ">",-35}";
			_receiptItemWeight = _item.Weight;
			_receiptCarriedWeight = body.CarriedItems.Sum(x => x.Weight);
		}
		if (!CharactersCurrent || !StockCurrent) return false;
		if (!RouteSpatialService.Instance.TryValidateLocation(new SpatialLocation(_destination, _destinationLayer, null), out _))
			return false;
		_prepared = true;
		return IsCurrent;
	}

	public void Commit()
	{
		_source.SetPreparedCurrencyRoomMembership(_item, false);
		if (_requireInventory) ((NativeBody)_buyerBody).PublishStoredItemReceipt(_item, _receiptHand!, _receiptDescription);
		else
		{
			_item.SetPreparedCurrencyCustody(null, null, new SpatialLocation(_destination, _destinationLayer, null));
			_destination.SetPreparedCurrencyRoomMembership(_item, true);
		}
		if (_fit is not null) _wearable!.SetFit(_fit);
		_item.Changed = true;
		_source.ContentsChanged = true;
		_destination.ContentsChanged = true;
		_committed = true;
	}

	public void Notify()
	{
		if (!_committed) return;
		RouteSpatialService.Instance.TrackPerceivable(_item);
		if (_requireInventory) ((NativeBody)_buyerBody).NotifyStoredItemReceipt(_item);
		else _destination.FinishPreparedCurrencyInsertion(_item, new SpatialLocation(_destination, _destinationLayer, null));
	}

	public void Dispose() { }
}

#nullable enable

using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Form.Material;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Functions.GameItem;

internal sealed class LiquidRefillCostFunction(IList<IFunction> parameters, IFuturemud world) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Number; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var item = ParameterFunctions[0].Result?.GetObject as IGameItem;
		var liquid = LiquidRefillPurchase.ResolveLiquid(world, (decimal)(ParameterFunctions[1].Result?.GetObject ?? 0m));
		var unitVolume = (double)(decimal)(ParameterFunctions[2].Result?.GetObject ?? 0m);
		var unitPrice = (decimal)(ParameterFunctions[3].Result?.GetObject ?? 0m);
		Result = new NumberVariable(LiquidRefillPurchase.Quote(item?.GetItemType<ILiquidContainer>(), liquid, unitVolume, unitPrice));
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new("liquidrefillcost",
		[ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Number],
		(p, w) => new LiquidRefillCostFunction(p, w), ["container", "liquidId", "unitVolume", "unitPrice"],
		["An open native liquid container.", "The native liquid to add.", "Volume of one priced unit in native units.", "Price per unit in base currency units."],
		"Pure full-refill quote, prorated and rounded up to a whole base currency unit. Returns zero when full and -1 for invalid, closed, projection-only or incompatible contents. Existing matching liquid is preserved.", "Items", ProgVariableTypes.Number));
}

internal sealed class PurchaseLiquidRefillFunction(IList<IFunction> parameters, IFuturemud world) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new BooleanVariable(false);
		if (ParameterFunctions[0].Result?.GetObject is not ICharacter buyer ||
			ParameterFunctions[1].Result?.GetObject is not ICharacter seller ||
			ParameterFunctions[2].Result?.GetObject is not IGameItem item ||
			ParameterFunctions[3].Result?.GetObject is not ICurrency currency) return StatementResult.Normal;
		var liquid = LiquidRefillPurchase.ResolveLiquid(world, (decimal)(ParameterFunctions[4].Result?.GetObject ?? 0m));
		var unitVolume = (double)(decimal)(ParameterFunctions[5].Result?.GetObject ?? 0m);
		var unitPrice = (decimal)(ParameterFunctions[6].Result?.GetObject ?? 0m);
		if (liquid is null || buyer.Body is null || seller.Body is null || item.GetItemType<ILiquidContainer>() is not { } container ||
			!buyer.CanSee(item) || !buyer.CanSee(seller) || !ItemManipulationGuard.CanManipulate(buyer, out _, item)) return StatementResult.Normal;
		// Settle exposure before taking the snapshot. Commit uses a native raw write;
		// this scope retracks the replacement only after cash and liquid are coherent.
		using var exposure = EnvironmentalExposureService.Changing(item);
		using var purchase = new LiquidRefillPurchase(buyer, seller, item, container, liquid, unitVolume, unitPrice);
		if (purchase.Price <= 0 || buyer.Body is null || seller.Body is null) return StatementResult.Normal;
		Result = new BooleanVariable(buyer.Body.TryGiveCurrency(currency, seller.Body, purchase.Price, true, purchase, true));
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new("purchaseliquidrefill",
		[ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Currency,
			ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Number],
		(p, w) => new PurchaseLiquidRefillFunction(p, w), ["buyer", "seller", "container", "currency", "liquidId", "unitVolume", "unitPrice"],
		["The paying customer.", "The awake noncombat seller.", "A directly held or wielded open liquid container.", "Native payment currency.",
			"Native liquid to add.", "Volume of a priced unit in native units.", "Price per unit in base currency units."],
		"Buys a full refill through prepared native held-cash Give. Refuses invalid custody, closed/full containers and other liquids before payment; preserves existing matching liquid. Cash and refill commit before observers. Price is prorated and rounded up to a whole base currency unit.", "Items", ProgVariableTypes.Boolean));
}

internal sealed class LiquidRefillPurchase : ICurrencyTransferParticipant
{
	private readonly ICharacter _buyer;
	private readonly ICharacter _seller;
	private readonly IGameItem _item;
	private readonly ILiquidContainer _container;
	private readonly IBody _body;
	private readonly IBody _sellerBody;
	private readonly LiquidMixture? _original;
	private readonly string? _originalXml;
	private readonly double _capacity;
	private readonly LiquidMixture? _replacement;
	public decimal Price { get; }
	internal LiquidRefillPurchase(ICharacter buyer, ICharacter seller, IGameItem item, ILiquidContainer container,
		ILiquid liquid, double unitVolume, decimal unitPrice)
	{
		_buyer = buyer; _seller = seller; _item = item; _container = container;
		_body = buyer.Body; _sellerBody = seller.Body;
		_original = container.LiquidMixture;
		_originalXml = _original?.SaveToXml().ToString(SaveOptions.DisableFormatting);
		_capacity = container.LiquidCapacity;
		Price = Quote(container, liquid, unitVolume, unitPrice);
		if (Price <= 0) return;
		var addition = new LiquidMixture(liquid, _capacity - container.LiquidVolume, item.Gameworld);
		_replacement = _original?.Clone() ?? addition;
		if (_original is not null) _replacement.AddLiquid(addition);
	}
	public bool IsCurrent => Price > 0 && !ReferenceEquals(_buyer, _seller) &&
		ReferenceEquals(_buyer.Body, _body) && ReferenceEquals(_seller.Body, _sellerBody) &&
		_buyer.State.IsAble() && _seller.State.IsAwake() && _seller.Combat is null && _buyer.ColocatedWith(_seller) &&
		!_item.Deleted && !_item.Destroyed && ReferenceEquals(_item.InInventoryOf, _body) && _item.ContainedIn is null &&
		_body.HeldOrWieldedItems.Any(x => ReferenceEquals(x, _item)) && _container.IsOpen &&
		ReferenceEquals(_item.GetItemType<ILiquidContainer>(), _container) && _container.OwnsLiquidMixture &&
		_container.LiquidCapacity == _capacity && ReferenceEquals(_container.LiquidMixture, _original) &&
		_container.LiquidMixture?.SaveToXml().ToString(SaveOptions.DisableFormatting) == _originalXml;
	public bool Prepare() => IsCurrent;
	public void Commit() => ((IPreparedLiquidContainer)_container).CommitPreparedLiquidMixture(_replacement!);
	public void Notify() => ((IPreparedLiquidContainer)_container).FinishPreparedLiquidMixture();
	public void Dispose() { }
	internal static ILiquid? ResolveLiquid(IFuturemud world, decimal id) =>
		id > 0 && id <= long.MaxValue && decimal.Truncate(id) == id ? world.Liquids.Get((long)id) : null;

	internal static decimal Quote(ILiquidContainer? container, ILiquid? liquid, double unitVolume, decimal unitPrice)
	{
		if (container is not IPreparedLiquidContainer || liquid is null || !container.OwnsLiquidMixture || !container.IsOpen ||
			!double.IsFinite(unitVolume) || unitVolume <= 0 || unitPrice <= 0 ||
			!double.IsFinite(container.LiquidCapacity) || !double.IsFinite(container.LiquidVolume) ||
			container.LiquidCapacity <= 0 || container.LiquidVolume < 0 || container.LiquidVolume > container.LiquidCapacity ||
			container.LiquidMixture?.Instances.Any(x => x.OriginLiquid.Id != liquid.Id) == true) return -1m;
		var missing = container.LiquidCapacity - container.LiquidVolume;
		if (missing == 0) return 0m;
		try { return Math.Max(1m, decimal.Ceiling((decimal)missing / (decimal)unitVolume * unitPrice)); }
		catch (Exception error) when (error is OverflowException or DivideByZeroException) { return -1m; }
	}
}

#nullable enable

using NativeBody = MudSharp.Body.Implementations.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Form.Material;
using MudSharp.FutureProg.Variables;
using NativeGameItem = MudSharp.GameItems.GameItem;

namespace MudSharp.FutureProg.Functions.GameItem;

internal sealed class PurchaseHeldItemCustodyFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new BooleanVariable(false);
		if (ParameterFunctions[0].Result?.GetObject is not ICharacter { Body: NativeBody body } buyer ||
			ParameterFunctions[1].Result?.GetObject is not ICharacter { Body: NativeBody } seller ||
			ParameterFunctions[2].Result?.GetObject is not NativeGameItem item ||
			ParameterFunctions[3].Result?.GetObject is not Room destination ||
			ParameterFunctions[4].Result?.GetObject is not ICurrency currency ||
			ParameterFunctions[5].Result?.GetObject is not decimal price || price < 0) return StatementResult.Normal;
		using var exposure = EnvironmentalExposureService.Changing(item);
		using var delivery = body.PrepareStoredItemCustody(seller, item, destination);
		if (delivery is null) return StatementResult.Normal;
		if (price == 0)
		{
			if (!delivery.IsCurrent) return StatementResult.Normal;
			delivery.Commit();
			delivery.Notify();
			Result = new BooleanVariable(true);
		}
		else
		{
			Result = new BooleanVariable(body.TryGiveCurrency(currency, seller.Body, price, true, delivery, true));
		}
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"purchasehelditemcustody",
		[ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Location,
			ProgVariableTypes.Currency, ProgVariableTypes.Number],
		(p, _) => new PurchaseHeldItemCustodyFunction(p),
		["buyer", "seller", "parcel", "storage", "currency", "price"],
		["The paying character holding the parcel.", "The colocated service provider.", "A directly held portable item, with its contents.",
			"An ordinary custody room.", "The payment currency.", "Nonnegative exact base-unit price."],
		"Atomically transfers held cash and places the held parcel in storage before inventory callbacks. Refuses loose currency parcels, wielded/worn items, connected items and vehicle/dwelling interiors. Authorization and parcel metadata belong in the calling prog.",
		"Items", ProgVariableTypes.Boolean));
}

#nullable enable

using MudSharp.Construction;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Form.Material;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Vehicles;

namespace MudSharp.FutureProg.Functions.Vehicles;

internal sealed class VehicleOccupiedFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var vehicle = VehicleFutureProgHelpers.VehicleFrom(ParameterFunctions[0]);
		Result = new BooleanVariable(vehicle is null || EmptyVehicleRelocation.HasLoadedPassengers(vehicle) ||
			EmptyVehicleRelocation.HasPersistedPassengers(vehicle));
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new("vehicleoccupied",
		[ProgVariableTypes.Item], (p, _) => new VehicleOccupiedFunction(p), ["vehicle"], ["The vehicle exterior."],
		"Whether a vehicle has assigned occupants, characters in its compartments or offline residents. Invalid vehicles return true. Cargo alone does not count.", "Vehicles", ProgVariableTypes.Boolean));
}

internal sealed class RelocateEmptyVehicleFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new BooleanVariable(false);
		if (ParameterFunctions[0].Result?.GetObject is not IGameItem item ||
			ParameterFunctions[1].Result?.GetObject is not IRoom destination) return StatementResult.Normal;
		using var exposure = EnvironmentalExposureService.Changing(item);
		using var relocation = EmptyVehicleRelocation.Create(item, destination, (bool?)ParameterFunctions[2].Result?.GetObject == true);
		if (relocation?.Prepare() != true || !relocation.IsCurrent) return StatementResult.Normal;
		relocation.Commit();
		relocation.Notify();
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new("relocateemptyvehicle",
		[ProgVariableTypes.Item, ProgVariableTypes.Location, ProgVariableTypes.Boolean], (p, _) => new RelocateEmptyVehicleFunction(p),
		["vehicle", "destination", "unhitchCharacters"], ["The native exterior.", "An ordinary destination room.", "Release draft characters in the origin room first."],
		"Moves one empty stationary vehicle between ordinary rooms, retaining its exterior, compartments and cargo. Refuses offline passengers, active journeys, route rooms and coupled vehicles. Optionally unhitches draft characters without moving them.", "Vehicles", ProgVariableTypes.Boolean));
}

internal sealed class PurchaseVehicleTransferFunction(IList<IFunction> parameters) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = new BooleanVariable(false);
		if (ParameterFunctions[0].Result?.GetObject is not ICharacter buyer || buyer.Body is null ||
			ParameterFunctions[1].Result?.GetObject is not ICharacter seller || seller.Body is null ||
			ParameterFunctions[2].Result?.GetObject is not IGameItem item ||
			ParameterFunctions[3].Result?.GetObject is not IRoom destination ||
			ParameterFunctions[4].Result?.GetObject is not ICurrency currency ||
			ParameterFunctions[5].Result?.GetObject is not decimal amount || amount <= 0 ||
			!buyer.ColocatedWith(seller) || !ReferenceEquals(buyer.Location, destination) ||
			!seller.State.IsAwake() || seller.Combat is not null) return StatementResult.Normal;
		using var exposure = EnvironmentalExposureService.Changing(item);
		using var relocation = EmptyVehicleRelocation.Create(item, destination, false,
			() => !ReferenceEquals(buyer, seller) && buyer.State.IsAble() && seller.State.IsAwake() && seller.Combat is null &&
				buyer.ColocatedWith(seller) && ReferenceEquals(buyer.Location, destination));
		if (relocation is null) return StatementResult.Normal;
		Result = new BooleanVariable(buyer.Body.TryGiveCurrency(currency, seller.Body, amount, true, relocation, true));
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new("purchasevehicletransfer",
		[ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Location, ProgVariableTypes.Currency, ProgVariableTypes.Number],
		(p, _) => new PurchaseVehicleTransferFunction(p), ["buyer", "seller", "vehicle", "destination", "currency", "amount"],
		["The paying collector.", "The awake noncombat recipient.", "The empty vehicle exterior.", "The collector's ordinary room.", "Native currency.", "Positive exact base-unit price."],
		"Pays held cash for relocation of one empty, unhitched native vehicle to the collector. Prepared cash and vehicle location publish before notifications; a refusal moves neither cash nor vehicle. Depot and ticket permissions belong in the calling prog.", "Vehicles", ProgVariableTypes.Boolean));
}

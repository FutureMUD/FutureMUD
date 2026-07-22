#nullable enable

using MudSharp.FutureProg.Variables;
using MudSharp.Vehicles;

namespace MudSharp.FutureProg.Functions.Vehicles;

internal sealed class VehicleOperationalLookupFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;
	private readonly LookupType _lookupType;
	private readonly ProgVariableTypes _returnType;

	private VehicleOperationalLookupFunction(IList<IFunction> parameters, IFuturemud gameworld,
		LookupType lookupType, ProgVariableTypes returnType) : base(parameters)
	{
		_gameworld = gameworld;
		_lookupType = lookupType;
		_returnType = returnType;
	}

	public override ProgVariableTypes ReturnType
	{
		get => _returnType;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}
		var value = ParameterFunctions[0].Result?.GetObject;
		Result = _lookupType switch
		{
			LookupType.RouteById => (IProgVariable?)_gameworld.VehicleRoutes.Get(Convert.ToInt64(value)) ??
				new NullVariable(ProgVariableTypes.VehicleRoute),
			LookupType.RouteByName => (IProgVariable?)_gameworld.VehicleRoutes.GetByIdOrName(Convert.ToString(value) ?? string.Empty) ??
				new NullVariable(ProgVariableTypes.VehicleRoute),
			LookupType.ServiceById => (IProgVariable?)_gameworld.VehicleServices.Get(Convert.ToInt64(value)) ??
				new NullVariable(ProgVariableTypes.VehicleService),
			LookupType.ServiceByName => (IProgVariable?)_gameworld.VehicleServices.GetByIdOrName(Convert.ToString(value) ?? string.Empty) ??
				new NullVariable(ProgVariableTypes.VehicleService),
			LookupType.JourneyById => (IProgVariable?)_gameworld.VehicleJourneys.Get(Convert.ToInt64(value)) ??
				new NullVariable(ProgVariableTypes.VehicleJourney),
			LookupType.JourneyByOperation => Guid.TryParse(Convert.ToString(value), out var operationId)
				? (IProgVariable?)_gameworld.VehicleJourneys.FirstOrDefault(x => x.OperationId == operationId) ??
				  new NullVariable(ProgVariableTypes.VehicleJourney)
				: new NullVariable(ProgVariableTypes.VehicleJourney),
			_ => new NullVariable(_returnType)
		};
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		Register("vehicleroute", ProgVariableTypes.Number, ProgVariableTypes.VehicleRoute,
			LookupType.RouteById, "route id", "Returns the current vehicle-route revision with this identity, or null.");
		Register("vehicleroute", ProgVariableTypes.Text, ProgVariableTypes.VehicleRoute,
			LookupType.RouteByName, "route name", "Returns a vehicle route by ID or name, or null.");
		Register("vehicleservice", ProgVariableTypes.Number, ProgVariableTypes.VehicleService,
			LookupType.ServiceById, "service id", "Returns a vehicle service by ID, or null.");
		Register("vehicleservice", ProgVariableTypes.Text, ProgVariableTypes.VehicleService,
			LookupType.ServiceByName, "service name", "Returns a vehicle service by ID or name, or null.");
		Register("vehiclejourney", ProgVariableTypes.Number, ProgVariableTypes.VehicleJourney,
			LookupType.JourneyById, "journey id", "Returns a durable vehicle journey by ID, or null.");
		Register("vehiclejourney", ProgVariableTypes.Text, ProgVariableTypes.VehicleJourney,
			LookupType.JourneyByOperation, "operation id", "Returns a durable vehicle journey by operation GUID, or null.");
	}

	private static void Register(string name, ProgVariableTypes parameterType, ProgVariableTypes returnType,
		LookupType lookupType, string parameterHelp, string functionHelp)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			[parameterType],
			(parameters, gameworld) => new VehicleOperationalLookupFunction(parameters, gameworld, lookupType, returnType),
			["identifier"],
			[parameterHelp],
			functionHelp,
			"Vehicles",
			returnType));
	}

	private enum LookupType
	{
		RouteById,
		RouteByName,
		ServiceById,
		ServiceByName,
		JourneyById,
		JourneyByOperation
	}
}

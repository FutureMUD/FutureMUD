#nullable enable

using MudSharp.Construction;
using MudSharp.FutureProg.Variables;
using MudSharp.Vehicles;

namespace MudSharp.FutureProg.Functions.Vehicles;

internal sealed class LoadVehicleFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;
	public LoadVehicleFunction(IList<IFunction> parameters, IFuturemud gameworld) : base(parameters) => _gameworld = gameworld;
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Item; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var prototype = _gameworld.VehiclePrototypes.Get(Convert.ToInt64(ParameterFunctions[0].Result?.GetObject));
		var location = ParameterFunctions[1].Result?.GetObject as IRoom;
		if (prototype is null || location is null || !prototype.CanCreateVehicle(out _))
		{
			Result = new NullVariable(ProgVariableTypes.Item);
			return StatementResult.Normal;
		}
		Result = VehicleFactory.CreateVehicle(prototype, location, RoomLayer.GroundLevel).ExteriorItem;
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
		"loadvehicle", [ProgVariableTypes.Number, ProgVariableTypes.Location], (pars, world) => new LoadVehicleFunction(pars, world),
		["prototype", "location"], ["The vehicle prototype ID.", "The exterior room."],
		"Creates a complete native vehicle and returns its exterior item; invalid prototypes or locations return null.",
		"Vehicles", ProgVariableTypes.Item));
}

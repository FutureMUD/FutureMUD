#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Vehicles;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Vehicles;

internal static class VehicleFutureProgHelpers
{
	private static readonly IVehicleHitchGraphService GraphService = new VehicleHitchGraphService();
	private static readonly IVehicleOperationalReadinessService ReadinessService = new VehicleOperationalReadinessService(GraphService);

	public static IVehicleHitchGraphService Graph => GraphService;
	public static IVehicleOperationalReadinessService Readiness => ReadinessService;

	public static IGameItem? ItemFrom(IFunction function)
	{
		return function.Result as IGameItem ?? function.Result?.GetObject as IGameItem;
	}

	public static ICharacter? CharacterFrom(IFunction function)
	{
		return function.Result as ICharacter ?? function.Result?.GetObject as ICharacter;
	}

	public static IVehicle? VehicleFrom(IFunction function)
	{
		return ItemFrom(function)?.GetItemType<IVehicleExterior>()?.Vehicle;
	}

	public static IVehicleMovementProfilePrototype? MovementProfile(IVehicle vehicle)
	{
		return vehicle.Prototype.MovementProfiles
		              .Where(x => x.MovementType == VehicleMovementProfileType.CellExit)
		              .OrderByDescending(x => x.IsDefault)
		              .FirstOrDefault();
	}
}

internal class IsVehicleFunction : BuiltInFunction
{
	private IsVehicleFunction(IList<IFunction> parameters) : base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = new BooleanVariable(VehicleFutureProgHelpers.VehicleFrom(ParameterFunctions[0]) is not null);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"isvehicle",
			new[] { ProgVariableTypes.Item },
			(pars, _) => new IsVehicleFunction(pars),
			new[] { "item" },
			new[] { "The item to inspect." },
			"Returns true if the supplied item is a linked vehicle exterior.",
			"Vehicles",
			ProgVariableTypes.Boolean));
	}
}

internal class VehicleCanActionFunction : BuiltInFunction
{
	private readonly VehicleOperationalAction _action;

	private VehicleCanActionFunction(IList<IFunction> parameters, VehicleOperationalAction action) : base(parameters)
	{
		_action = action;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var actor = VehicleFutureProgHelpers.CharacterFrom(ParameterFunctions[0]);
		var vehicle = VehicleFutureProgHelpers.VehicleFrom(ParameterFunctions[1]);
		Result = new BooleanVariable(vehicle is not null && actor is not null &&
		                              VehicleFutureProgHelpers.Readiness.CanPerformAction(vehicle, actor, _action, out var result) &&
		                              result.Allowed);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		Register("vehiclecanboard", VehicleOperationalAction.Board, "Returns true if the character can board the vehicle.");
		Register("vehiclecancontrol", VehicleOperationalAction.Control, "Returns true if the character can control the vehicle.");
		Register("vehiclecanservice", VehicleOperationalAction.Service, "Returns true if the character can service the vehicle.");
		Register("vehiclecanrepair", VehicleOperationalAction.Repair, "Returns true if the character can repair the vehicle.");
		Register("vehiclecanhitch", VehicleOperationalAction.Hitch, "Returns true if the character can hitch or unhitch the vehicle.");
	}

	private static void Register(string name, VehicleOperationalAction action, string description)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Item },
			(pars, _) => new VehicleCanActionFunction(pars, action),
			new[] { "character", "vehicle" },
			new[] { "The character attempting the action.", "The vehicle exterior item." },
			description,
			"Vehicles",
			ProgVariableTypes.Boolean));
	}
}

internal class VehicleCanStartFunction : BuiltInFunction
{
	private VehicleCanStartFunction(IList<IFunction> parameters) : base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var actor = VehicleFutureProgHelpers.CharacterFrom(ParameterFunctions[0]);
		var vehicle = VehicleFutureProgHelpers.VehicleFrom(ParameterFunctions[1]);
		if (actor is null || vehicle is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var result = VehicleFutureProgHelpers.Readiness.BuildMovementReadiness(new VehicleMovementReadinessRequest(
			vehicle,
			actor,
			null,
			VehicleFutureProgHelpers.MovementProfile(vehicle)));
		Result = new BooleanVariable(result.CanMove);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"vehiclecanstart",
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Item },
			(pars, _) => new VehicleCanStartFunction(pars),
			new[] { "character", "vehicle" },
			new[] { "The character attempting to start or route the vehicle.", "The vehicle exterior item." },
			"Returns true if the character and vehicle pass route-ready movement preflight without checking a specific exit.",
			"Vehicles",
			ProgVariableTypes.Boolean));
	}
}

internal class VehicleReadinessReasonFunction : BuiltInFunction
{
	private VehicleReadinessReasonFunction(IList<IFunction> parameters) : base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Text;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var actor = VehicleFutureProgHelpers.CharacterFrom(ParameterFunctions[0]);
		var vehicle = VehicleFutureProgHelpers.VehicleFrom(ParameterFunctions[1]);
		var actionText = (ParameterFunctions[2].Result?.GetObject as string ?? string.Empty).ToLowerInvariant();
		if (actor is null)
		{
			Result = new TextVariable("There is no such character.");
			return StatementResult.Normal;
		}

		if (vehicle is null)
		{
			Result = new TextVariable("That item is not a linked vehicle exterior.");
			return StatementResult.Normal;
		}

		if (actionText.EqualToAny("start", "move", "route"))
		{
			var movement = VehicleFutureProgHelpers.Readiness.BuildMovementReadiness(new VehicleMovementReadinessRequest(
				vehicle,
				actor,
				null,
				VehicleFutureProgHelpers.MovementProfile(vehicle)));
			Result = new TextVariable(movement.CanMove ? string.Empty : movement.Reason);
			return StatementResult.Normal;
		}

		if (!TryParseAction(actionText, out var action))
		{
			Result = new TextVariable("Unknown vehicle readiness action.");
			return StatementResult.Normal;
		}

		VehicleFutureProgHelpers.Readiness.CanPerformAction(vehicle, actor, action, out var access);
		Result = new TextVariable(access.Allowed ? string.Empty : access.Reason);
		return StatementResult.Normal;
	}

	private static bool TryParseAction(string text, out VehicleOperationalAction action)
	{
		return text switch
		{
			"board" => Set(out action, VehicleOperationalAction.Board),
			"control" => Set(out action, VehicleOperationalAction.Control),
			"service" => Set(out action, VehicleOperationalAction.Service),
			"repair" => Set(out action, VehicleOperationalAction.Repair),
			"hitch" => Set(out action, VehicleOperationalAction.Hitch),
			_ => Set(out action, VehicleOperationalAction.Board, false)
		};
	}

	private static bool Set(out VehicleOperationalAction action, VehicleOperationalAction value, bool result = true)
	{
		action = value;
		return result;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"vehiclereadinessreason",
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Text },
			(pars, _) => new VehicleReadinessReasonFunction(pars),
			new[] { "character", "vehicle", "action" },
			new[] { "The character attempting the action.", "The vehicle exterior item.", "board, control, service, repair, hitch, start, move, or route." },
			"Returns the blocking reason for a vehicle action, or blank text if the action is ready.",
			"Vehicles",
			ProgVariableTypes.Text));
	}
}

internal class VehicleTrainWeightFunction : BuiltInFunction
{
	private VehicleTrainWeightFunction(IList<IFunction> parameters) : base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var vehicle = VehicleFutureProgHelpers.VehicleFrom(ParameterFunctions[0]);
		Result = new NumberVariable(vehicle is null
			? 0.0
			: VehicleFutureProgHelpers.Graph.VehicleTrainWeight(vehicle.Gameworld, vehicle));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"vehicletrainweight",
			new[] { ProgVariableTypes.Item },
			(pars, _) => new VehicleTrainWeightFunction(pars),
			new[] { "vehicle" },
			new[] { "The vehicle exterior item." },
			"Returns the effective weight of the vehicle's unified hitch/tow train.",
			"Vehicles",
			ProgVariableTypes.Number));
	}
}

internal class VehicleTowStressFunction : BuiltInFunction
{
	private VehicleTowStressFunction(IList<IFunction> parameters) : base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var vehicle = VehicleFutureProgHelpers.VehicleFrom(ParameterFunctions[0]);
		if (vehicle is null || !VehicleFutureProgHelpers.Graph.TryBuildVehicleTrain(vehicle.Gameworld, vehicle, out var plan, out _))
		{
			Result = new NumberVariable(0.0);
			return StatementResult.Normal;
		}

		var policy = VehicleFutureProgHelpers.Readiness.TowStressPolicy(vehicle.Gameworld);
		var stress = VehicleFutureProgHelpers.Graph.EvaluateTowStress(plan, policy);
		Result = new NumberVariable(stress.Any() ? stress.Max(x => x.StressRatio) : 0.0);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"vehicletowstress",
			new[] { ProgVariableTypes.Item },
			(pars, _) => new VehicleTowStressFunction(pars),
			new[] { "vehicle" },
			new[] { "The vehicle exterior item." },
			"Returns the highest hitch/tow stress ratio in the vehicle's unified train, or 0 if none applies.",
			"Vehicles",
			ProgVariableTypes.Number));
	}
}
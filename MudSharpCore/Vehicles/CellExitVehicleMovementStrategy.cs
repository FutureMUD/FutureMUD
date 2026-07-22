using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp.Vehicles;

public class CellExitVehicleMovementStrategy : IVehicleMovementStrategy
{
	private readonly IVehicleHitchGraphService _graphService;
	private readonly IVehicleOperationalReadinessService _readinessService;

	public CellExitVehicleMovementStrategy() : this(new VehicleTowService(), new VehicleHitchGraphService())
	{
	}

	public CellExitVehicleMovementStrategy(IVehicleTowService towService) : this(towService, new VehicleHitchGraphService())
	{
	}

	public CellExitVehicleMovementStrategy(IVehicleTowService towService, IVehicleHitchGraphService graphService)
		: this(towService, graphService, new VehicleOperationalReadinessService(graphService))
	{
	}

	public CellExitVehicleMovementStrategy(IVehicleTowService towService, IVehicleHitchGraphService graphService,
		IVehicleOperationalReadinessService readinessService)
	{
		_graphService = graphService;
		_readinessService = readinessService;
	}

	public VehicleMovementProfileType MovementType => VehicleMovementProfileType.CellExit;

	public bool CanMove(IVehicle vehicle, ICharacter actor, ICellExit exit, out string reason)
	{
		if (exit is null)
		{
			reason = "There is no such exit.";
			return false;
		}

		if (exit.Exit.Door?.IsOpen == false)
		{
			reason = "The door through that exit is closed.";
			return false;
		}

		var profile = vehicle is null ? null : MovementProfile(vehicle);
		var result = _readinessService.BuildMovementReadiness(new VehicleMovementReadinessRequest(vehicle, actor, exit, profile));
		reason = result.Reason;
		return result.CanMove;
	}

	public bool Move(IVehicle vehicle, ICharacter actor, ICellExit exit)
	{
		if (!TryPrepareMove(vehicle, actor, exit, true, out var towTrain, out var transition, out var readiness,
			    out _))
		{
			return false;
		}

		if (readiness.PropulsionReadiness is not null &&
		    !_readinessService.TryCommitPropulsion(readiness.PropulsionReadiness, out _, out var propulsionReason))
		{
			actor.OutputHandler.Send(propulsionReason);
			return false;
		}

		EchoDeparture(vehicle, actor, exit, towTrain);
		BeginMove(vehicle, exit, towTrain, transition);
		CompleteMove(vehicle, exit, transition, readiness);
		EchoArrival(vehicle, actor, exit, towTrain, transition.TargetLayer);
		return true;
	}

	public bool TryPrepareMove(IVehicle vehicle, ICharacter actor, ICellExit exit, out IReadOnlyList<IVehicle> towTrain,
		out (CellMovementTransition TransitionType, RoomLayer TargetLayer) transition, out string reason)
	{
		return TryPrepareMove(vehicle, actor, exit, true, out towTrain, out transition, out _, out reason);
	}

	public bool TryPrepareMove(IVehicle vehicle, ICharacter actor, ICellExit exit, bool rollTowCatastrophe,
		out IReadOnlyList<IVehicle> towTrain,
		out (CellMovementTransition TransitionType, RoomLayer TargetLayer) transition,
		out VehicleMovementReadinessResult readiness, out string reason,
		VehiclePropulsionMovePlan committedPropulsionPlan = null,
		IReadOnlyCollection<ICharacter> externalPullers = null,
		IVehicleMovementProfilePrototype movementProfile = null,
		bool automaticOperation = false)
	{
		towTrain = [];
		transition = (CellMovementTransition.NoViableTransition, RoomLayer.GroundLevel);
		readiness = new VehicleMovementReadinessResult(false, "There is no such exit.", null, null, []);
		if (exit is null)
		{
			reason = "There is no such exit.";
			return false;
		}

		if (exit.Exit.Door?.IsOpen == false)
		{
			reason = "The door through that exit is closed.";
			return false;
		}

		var profile = movementProfile ?? (vehicle is null ? null : MovementProfile(vehicle));
		readiness = _readinessService.BuildMovementReadiness(new VehicleMovementReadinessRequest(vehicle, actor, exit,
			profile, committedPropulsionPlan, externalPullers, automaticOperation));
		if (!readiness.CanMove || readiness.MovePlan is null)
		{
			reason = readiness.Reason;
			return false;
		}

		IPerceiver transitionPerceiver = externalPullers?.FirstOrDefault();
		transitionPerceiver ??= vehicle.Prototype.Scale == VehicleScale.RoomScale
			? vehicle.ExteriorItem
			: actor ?? (IPerceiver)vehicle.ExteriorItem;
		transitionPerceiver ??= actor;
		if (transitionPerceiver is null)
		{
			reason = "That vehicle has no exterior projection with which to resolve the exit transition.";
			return false;
		}

		transition = exit.MovementTransition(transitionPerceiver);
		if (rollTowCatastrophe)
		{
			var catastropheActor = actor ?? externalPullers?.FirstOrDefault() ?? vehicle.Controller;
			var catastrophe = _readinessService.RollTowCatastrophe(readiness.MovePlan, catastropheActor);
			if (catastrophe.Catastrophe)
			{
				reason = catastrophe.Reason;
				if (vehicle.ExteriorItem?.OutputHandler is { } outputHandler)
				{
					outputHandler.Handle(catastrophe.Reason, OutputRange.Local);
				}
				return false;
			}
		}

		towTrain = readiness.MovePlan.Vehicles.ToList();
		reason = string.Empty;
		return true;
	}

	public bool TryCommitPropulsion(VehicleMovementReadinessResult readiness, out VehiclePropulsionMovePlan plan,
		out string reason)
	{
		if (readiness.PropulsionReadiness is null)
		{
			plan = null;
			reason = string.Empty;
			return true;
		}

		return _readinessService.TryCommitPropulsion(readiness.PropulsionReadiness, out plan, out reason);
	}

	public void EchoDeparture(IVehicle vehicle, ICharacter actor, ICellExit exit, IReadOnlyList<IVehicle> towTrain)
	{
		var exterior = vehicle.ExteriorItem;
		if (exterior?.OutputHandler is { } outputHandler)
		{
			outputHandler.Handle(
				new EmoteOutput(new Emote(
					$"@ move|moves {exit.OutboundMovementSuffix}{TowEchoSuffix(towTrain.Skip(1), exterior)}.",
					exterior)),
				OutputRange.Local);
		}
		else
		{
			exit.Origin.HandleRoomEcho($"{vehicle.Name} moves {exit.OutboundMovementSuffix}.", vehicle.RoomLayer);
		}

		EchoHostedInteriors(vehicle, "The vehicle shudders as it begins moving.");
	}

	public void EchoArrival(IVehicle vehicle, ICharacter actor, ICellExit exit, IReadOnlyList<IVehicle> towTrain,
		RoomLayer targetLayer)
	{
		var exterior = vehicle.ExteriorItem;
		if (exterior?.OutputHandler is { } outputHandler)
		{
			outputHandler.Handle(
				new EmoteOutput(new Emote(
					$"@ arrive|arrives {exit.InboundMovementSuffix}{TowEchoSuffix(towTrain.Skip(1), exterior)}.",
					exterior)),
				OutputRange.Local);
		}
		else
		{
			exit.Destination.HandleRoomEcho($"{vehicle.Name} arrives {exit.InboundMovementSuffix}.", targetLayer);
		}

		EchoHostedInteriors(vehicle, "The vehicle settles as it arrives at its next location.");
	}

	public void BeginMove(IVehicle vehicle, ICellExit exit, IReadOnlyList<IVehicle> towTrain,
		(CellMovementTransition TransitionType, RoomLayer TargetLayer) transition)
	{
		foreach (var linkedVehicle in towTrain.DefaultIfEmpty(vehicle))
		{
			linkedVehicle.BeginMoveToCell(exit.Destination, transition.TargetLayer, exit);
		}
	}

	public void CompleteMove(IVehicle vehicle, ICellExit exit,
		(CellMovementTransition TransitionType, RoomLayer TargetLayer) transition,
		VehicleMovementReadinessResult readiness, IMovement movement = null)
	{
		if (!readiness.CanMove || readiness.MovePlan is null)
		{
			return;
		}

		if (readiness.ResourcePlan is not null)
		{
			_readinessService.ConsumeMovementResources(readiness.ResourcePlan);
		}

		_graphService.CompleteVehicleTrainMove(readiness.MovePlan, exit.Destination, transition.TargetLayer, exit,
			movement);
	}

	private static IVehicleMovementProfilePrototype MovementProfile(IVehicle vehicle)
	{
		return vehicle.MovementProfile;
	}

	private static string TowEchoSuffix(IEnumerable<IVehicle> towedVehicles, IPerceiver voyeur)
	{
		var vehicles = towedVehicles.ToList();
		return vehicles.Any()
			? $" towing {vehicles.Select(x => x.ExteriorItem?.HowSeen(voyeur) ?? x.Name).ListToString()}"
			: string.Empty;
	}

	private static void EchoHostedInteriors(IVehicle vehicle, string message)
	{
		if (vehicle.Prototype.Scale != VehicleScale.RoomScale)
		{
			return;
		}

		foreach (var cell in vehicle.Compartments
			         .Select(x => x.InteriorCell)
			         .Where(x => x is not null)
			         .Distinct())
		{
			cell.Handle(message);
		}
	}
}

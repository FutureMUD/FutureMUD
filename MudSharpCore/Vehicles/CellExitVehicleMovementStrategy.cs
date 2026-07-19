using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Movement;

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
		out VehicleMovementReadinessResult readiness, out string reason)
	{
		towTrain = [];
		transition = (CellMovementTransition.NoViableTransition, RoomLayer.GroundLevel);
		readiness = new VehicleMovementReadinessResult(false, "There is no such exit.", null, null, []);
		if (exit is null)
		{
			reason = "There is no such exit.";
			return false;
		}

		var profile = vehicle is null ? null : MovementProfile(vehicle);
		readiness = _readinessService.BuildMovementReadiness(new VehicleMovementReadinessRequest(vehicle, actor, exit, profile));
		if (!readiness.CanMove || readiness.MovePlan is null)
		{
			reason = readiness.Reason;
			return false;
		}

		transition = exit.MovementTransition(actor);
		if (rollTowCatastrophe)
		{
			var catastrophe = _readinessService.RollTowCatastrophe(readiness.MovePlan, actor);
			if (catastrophe.Catastrophe)
			{
				reason = catastrophe.Reason;
				exit.Origin.HandleRoomEcho(catastrophe.Reason, vehicle.RoomLayer);
				return false;
			}
		}

		towTrain = readiness.MovePlan.Vehicles.ToList();
		reason = string.Empty;
		return true;
	}

	public void EchoDeparture(IVehicle vehicle, ICharacter actor, ICellExit exit, IReadOnlyList<IVehicle> towTrain)
	{
		exit.Origin.HandleRoomEcho($"{vehicle.ExteriorItem?.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf) ?? vehicle.Name} moves {exit.OutboundMovementSuffix}{TowEchoSuffix(towTrain.Skip(1), actor)}.", vehicle.RoomLayer);
	}

	public void EchoArrival(IVehicle vehicle, ICharacter actor, ICellExit exit, IReadOnlyList<IVehicle> towTrain,
		RoomLayer targetLayer)
	{
		exit.Destination.HandleRoomEcho($"{vehicle.ExteriorItem?.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf) ?? vehicle.Name} arrives {exit.InboundMovementSuffix}{TowEchoSuffix(towTrain.Skip(1), actor)}.", targetLayer);
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
		return vehicle.Prototype.MovementProfiles
		              .Where(x => x.MovementType == VehicleMovementProfileType.CellExit)
		              .OrderByDescending(x => x.IsDefault)
		              .FirstOrDefault();
	}

	private static string TowEchoSuffix(IEnumerable<IVehicle> towedVehicles, ICharacter actor)
	{
		var vehicles = towedVehicles.ToList();
		return vehicles.Any()
			? $" towing {vehicles.Select(x => x.ExteriorItem?.HowSeen(actor) ?? x.Name).ListToString()}"
			: string.Empty;
	}
}

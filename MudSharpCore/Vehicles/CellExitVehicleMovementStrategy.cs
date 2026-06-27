using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Vehicles;

public class CellExitVehicleMovementStrategy : IVehicleMovementStrategy
{
	private readonly IVehicleTowService _towService;
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
		_towService = towService;
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
		if (!TryPrepareMove(vehicle, actor, exit, out var towTrain, out var transition, out _))
		{
			return false;
		}

		EchoDeparture(vehicle, actor, exit, towTrain);
		BeginMove(vehicle, exit, towTrain, transition);
		CompleteMove(vehicle, exit, towTrain, transition);
		EchoArrival(vehicle, actor, exit, towTrain, transition.TargetLayer);
		return true;
	}

	public bool TryPrepareMove(IVehicle vehicle, ICharacter actor, ICellExit exit, out IReadOnlyList<IVehicle> towTrain,
		out (CellMovementTransition TransitionType, RoomLayer TargetLayer) transition, out string reason)
	{
		towTrain = [];
		transition = (CellMovementTransition.NoViableTransition, RoomLayer.GroundLevel);
		if (exit is null)
		{
			reason = "There is no such exit.";
			return false;
		}

		var profile = vehicle is null ? null : MovementProfile(vehicle);
		var readiness = _readinessService.BuildMovementReadiness(new VehicleMovementReadinessRequest(vehicle, actor, exit, profile));
		if (!readiness.CanMove || readiness.MovePlan is null)
		{
			reason = readiness.Reason;
			return false;
		}

		transition = exit.MovementTransition(actor);
		var catastrophe = _readinessService.RollTowCatastrophe(readiness.MovePlan, actor);
		if (catastrophe.Catastrophe)
		{
			reason = catastrophe.Reason;
			exit.Origin.HandleRoomEcho(catastrophe.Reason, vehicle.RoomLayer);
			return false;
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

	public void CompleteMove(IVehicle vehicle, ICellExit exit, IReadOnlyList<IVehicle> towTrain,
		(CellMovementTransition TransitionType, RoomLayer TargetLayer) transition, IMovement movement = null)
	{
		var vehicles = towTrain.Any() ? towTrain : [vehicle];
		var profile = vehicle is null ? null : MovementProfile(vehicle);
		if (profile is not null)
		{
			_readinessService.ConsumeMovementResources(vehicle, profile);
		}

		if (_graphService.CanMoveVehicleTrain(vehicle.Gameworld, vehicle, exit, out var movePlan, out _))
		{
			_graphService.CompleteVehicleTrainMove(movePlan, exit.Destination, transition.TargetLayer, exit, movement);
			return;
		}

		foreach (var linkedVehicle in vehicles)
		{
			linkedVehicle.MoveToCell(exit.Destination, transition.TargetLayer, exit, movement);
		}
		MoveHitchItems(_towService.TowLinksFrom(vehicle), exit.Destination, transition.TargetLayer);
	}

	private static IVehicleMovementProfilePrototype MovementProfile(IVehicle vehicle)
	{
		return vehicle.Prototype.MovementProfiles
		              .Where(x => x.MovementType == VehicleMovementProfileType.CellExit)
		              .OrderByDescending(x => x.IsDefault)
		              .FirstOrDefault();
	}

	private static string DescribeResourceFailure(ICharacter actor, string reason, IReadOnlyList<VehicleResourceCandidate> candidates)
	{
		var failed = candidates.Where(x => !x.Available && !string.IsNullOrWhiteSpace(x.Reason)).ToList();
		return failed.Any()
			? $"{reason} {failed.Select(x => $"{(x.Item?.HowSeen(actor) ?? x.Installation.Prototype.Name)}: {x.Reason}").ListToString()}"
			: reason;
	}

	private static string TowEchoSuffix(IEnumerable<IVehicle> towedVehicles, ICharacter actor)
	{
		var vehicles = towedVehicles.ToList();
		return vehicles.Any()
			? $" towing {vehicles.Select(x => x.ExteriorItem?.HowSeen(actor) ?? x.Name).ListToString()}"
			: string.Empty;
	}

	private static void MoveHitchItems(IEnumerable<IVehicleTowLink> towLinks, ICell destination, RoomLayer layer)
	{
		foreach (var item in towLinks.Select(x => x.HitchItem).Where(x => x is not null).Distinct().ToList())
		{
			if (item!.ContainedIn is not null || item.InInventoryOf is not null)
			{
				continue;
			}

			item.Location?.Extract(item);
			item.RoomLayer = layer;
			destination.Insert(item, true);
		}
	}
}

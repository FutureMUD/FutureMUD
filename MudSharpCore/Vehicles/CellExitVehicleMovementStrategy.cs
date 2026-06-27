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
		if (vehicle is null)
		{
			reason = "There is no such vehicle.";
			return false;
		}

		if (actor is null)
		{
			reason = "There is no such driver.";
			return false;
		}

		if (exit is null)
		{
			reason = "There is no such exit.";
			return false;
		}

		if (vehicle.Destroyed)
		{
			reason = "That vehicle is destroyed and cannot move.";
			return false;
		}

		if (vehicle.Disabled)
		{
			var damageReason = vehicle.DamageDisabledReason(VehicleDamageEffectTargetType.WholeVehicleMovement, null);
			reason = string.IsNullOrWhiteSpace(damageReason)
				? "That vehicle is disabled and cannot move."
				: $"That vehicle cannot move because {damageReason}.";
			return false;
		}

		if (vehicle.Controller != actor)
		{
			reason = "You must be in control of the vehicle to move it.";
			return false;
		}

		if (!_readinessService.CanPerformAction(vehicle, actor, VehicleOperationalAction.Control, out var accessResult))
		{
			reason = accessResult.Reason;
			return false;
		}

		if (vehicle.Location != exit.Origin)
		{
			reason = "The vehicle is not at the origin of that exit.";
			return false;
		}

		if (actor.Location != vehicle.Location)
		{
			reason = "You must be in the same location as the vehicle to move it.";
			return false;
		}

		if (actor.RoomLayer != vehicle.RoomLayer)
		{
			reason = "You must be on the same room layer as the vehicle to move it.";
			return false;
		}

		var profile = MovementProfile(vehicle);
		if (profile is null)
		{
			reason = "That vehicle cannot move through normal cell exits.";
			return false;
		}

		if (vehicle.IsDisabledByDamage(VehicleDamageEffectTargetType.MovementProfile, profile.Id))
		{
			reason = $"That movement profile is disabled because {vehicle.DamageDisabledReason(VehicleDamageEffectTargetType.MovementProfile, profile.Id)}.";
			return false;
		}

		if (vehicle.ExteriorItem is not null && exit.Exit.MaximumSizeToEnter < vehicle.ExteriorItem.Size)
		{
			reason = "That exit is too small for the vehicle.";
			return false;
		}

		if (!_graphService.CanMoveVehicleTrain(vehicle.Gameworld, vehicle, exit, out var movePlan, out reason))
		{
			return false;
		}

		foreach (var linkedVehicle in movePlan.Vehicles.DefaultIfEmpty(vehicle))
		{
			if (linkedVehicle.ExteriorItem?.PreventsMovement() != true)
			{
				continue;
			}

			reason = linkedVehicle.ExteriorItem.WhyPreventsMovement(actor);
			return false;
		}

		if (profile.RequiresAccessPointsClosed || vehicle.AccessPoints.Any(x => x.Prototype.MustBeClosedForMovement))
		{
			var openAccess = vehicle.AccessPoints.FirstOrDefault(x =>
				x.IsOpen && (profile.RequiresAccessPointsClosed || x.Prototype.MustBeClosedForMovement));
			if (openAccess is not null)
			{
				reason = $"{openAccess.Name} must be closed before the vehicle can move.";
				return false;
			}
		}

		var missingRequiredInstallation = vehicle.Installations
		                                        .FirstOrDefault(x => x.Prototype.RequiredForMovement &&
		                                                             !_readinessService.IsInstallationFunctionalForMovement(x, out _));
		if (missingRequiredInstallation is not null)
		{
			_readinessService.IsInstallationFunctionalForMovement(missingRequiredInstallation, out var moduleReason);
			reason = $"{missingRequiredInstallation.Prototype.Name} must have a functional module installed before the vehicle can move: {moduleReason}.";
			return false;
		}

		if (!_readinessService.HasFunctionalRole(vehicle, profile.RequiredInstalledRole, out reason))
		{
			return false;
		}

		if (!_readinessService.HasPower(vehicle, profile, out var powerCandidates, out reason))
		{
			reason = DescribeResourceFailure(actor, reason, powerCandidates);
			return false;
		}

		if (!_readinessService.HasFuel(vehicle, profile, out var fuelCandidates, out reason))
		{
			reason = DescribeResourceFailure(actor, reason, fuelCandidates);
			return false;
		}

		if (profile.RequiresTowLinksClosed)
		{
			var invalidTowLink = movePlan.Links.FirstOrDefault(x => x.IsDisabled);
			if (invalidTowLink is not null)
			{
				reason = "One of that vehicle's tow links is disabled.";
				return false;
			}
		}

		var transition = exit.MovementTransition(actor);
		if (transition.TransitionType == CellMovementTransition.NoViableTransition)
		{
			reason = "That exit is not a viable transition from your current position.";
			return false;
		}

		reason = string.Empty;
		return true;
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
		if (!CanMove(vehicle, actor, exit, out reason))
		{
			return false;
		}

		transition = exit.MovementTransition(actor);
		if (!_graphService.CanMoveVehicleTrain(vehicle.Gameworld, vehicle, exit, out var movePlan, out reason))
		{
			return false;
		}

		var catastrophe = _readinessService.RollTowCatastrophe(movePlan, actor);
		if (catastrophe.Catastrophe)
		{
			reason = catastrophe.Reason;
			exit.Origin.HandleRoomEcho(catastrophe.Reason, vehicle.RoomLayer);
			return false;
		}

		towTrain = movePlan.Vehicles.ToList();
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
		var profile = MovementProfile(vehicle);
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

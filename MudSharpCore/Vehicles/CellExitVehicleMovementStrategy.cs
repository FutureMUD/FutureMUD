using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Vehicles;

public class CellExitVehicleMovementStrategy : IVehicleMovementStrategy
{
	private readonly IVehicleTowService _towService;

	public CellExitVehicleMovementStrategy() : this(new VehicleTowService())
	{
	}

	public CellExitVehicleMovementStrategy(IVehicleTowService towService)
	{
		_towService = towService;
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

		if (vehicle.Location != exit.Origin)
		{
			reason = "The vehicle is not at the origin of that exit.";
			return false;
		}

		var profile = vehicle.Prototype.MovementProfiles
		                     .Where(x => x.MovementType == VehicleMovementProfileType.CellExit)
		                     .OrderByDescending(x => x.IsDefault)
		                     .FirstOrDefault();
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

		if (!_towService.CanMoveTowTrain(vehicle, exit, out _, out reason))
		{
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
		                                                             (x.IsDisabled || x.InstalledItem is null));
		if (missingRequiredInstallation is not null)
		{
			reason = $"{missingRequiredInstallation.Prototype.Name} must have a functional module installed before the vehicle can move.";
			return false;
		}

		if (!string.IsNullOrWhiteSpace(profile.RequiredInstalledRole) &&
		    !vehicle.Installations.Any(x => !x.IsDisabled &&
		                                   x.InstalledItem?.GetItemType<IVehicleInstallable>()?.Role.EqualTo(profile.RequiredInstalledRole) == true))
		{
			reason = $"That vehicle requires a functional {profile.RequiredInstalledRole.ColourCommand()} module to move.";
			return false;
		}

		if (profile.RequiredPowerSpikeInWatts > 0.0 &&
		    !vehicle.Installations
		            .Select(x => x.InstalledItem?.GetItemType<IProducePower>())
		            .Where(x => x is not null)
		            .Any(x => x.CanDrawdownSpike(profile.RequiredPowerSpikeInWatts)))
		{
			reason = "That vehicle does not have enough available power to move.";
			return false;
		}

		if (profile.FuelLiquidId is not null && profile.FuelVolumePerMove > 0.0 &&
		    !FuelContainers(vehicle, profile).Any())
		{
			reason = "That vehicle does not have enough configured fuel to move.";
			return false;
		}

		if (profile.RequiresTowLinksClosed)
		{
			var invalidTowLink = _towService.TowLinksFrom(vehicle).FirstOrDefault(x => x.IsDisabled);
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
		if (!CanMove(vehicle, actor, exit, out _))
		{
			return false;
		}

		var transition = exit.MovementTransition(actor);
		if (!_towService.CanMoveTowTrain(vehicle, exit, out var towTrain, out _))
		{
			return false;
		}

		var towedVehicles = towTrain.Skip(1).ToList();
		var towLinks = _towService.TowLinksFrom(vehicle).ToList();
		exit.Origin.HandleRoomEcho($"{vehicle.ExteriorItem?.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf) ?? vehicle.Name} moves {exit.OutboundMovementSuffix}{TowEchoSuffix(towedVehicles, actor)}.", vehicle.RoomLayer);
		ConsumeMovementRequirements(vehicle);
		vehicle.MoveToCell(exit.Destination, transition.TargetLayer, exit);
		foreach (var linkedVehicle in towedVehicles)
		{
			linkedVehicle.MoveToCell(exit.Destination, transition.TargetLayer, exit);
		}
		MoveHitchItems(towLinks, exit.Destination, transition.TargetLayer);
		exit.Destination.HandleRoomEcho($"{vehicle.ExteriorItem?.HowSeen(actor, flags: PerceiveIgnoreFlags.IgnoreSelf) ?? vehicle.Name} arrives {exit.InboundMovementSuffix}{TowEchoSuffix(towedVehicles, actor)}.", transition.TargetLayer);
		return true;
	}

	private static IVehicleMovementProfilePrototype MovementProfile(IVehicle vehicle)
	{
		return vehicle.Prototype.MovementProfiles
		              .Where(x => x.MovementType == VehicleMovementProfileType.CellExit)
		              .OrderByDescending(x => x.IsDefault)
		              .FirstOrDefault();
	}

	private static IEnumerable<ILiquidContainer> FuelContainers(IVehicle vehicle, IVehicleMovementProfilePrototype profile)
	{
		if (profile.FuelLiquidId is null || profile.FuelVolumePerMove <= 0.0)
		{
			return Enumerable.Empty<ILiquidContainer>();
		}

		return vehicle.Installations
		              .Where(x => !x.IsDisabled)
		              .Select(x => x.InstalledItem)
		              .Where(x => x is not null)
		              .SelectMany(x => x.GetItemTypes<ILiquidContainer>())
		              .Where(x => x.LiquidVolume >= profile.FuelVolumePerMove &&
		                          x.LiquidMixture?.Instances.Any(y => y.Liquid.Id == profile.FuelLiquidId.Value) == true);
	}

	private static void ConsumeMovementRequirements(IVehicle vehicle)
	{
		var profile = MovementProfile(vehicle);
		if (profile is null)
		{
			return;
		}

		if (profile.RequiredPowerSpikeInWatts > 0.0)
		{
			vehicle.Installations
			       .Select(x => x.InstalledItem?.GetItemType<IProducePower>())
			       .FirstOrDefault(x => x?.CanDrawdownSpike(profile.RequiredPowerSpikeInWatts) == true)
			       ?.DrawdownSpike(profile.RequiredPowerSpikeInWatts);
		}

		var fuel = FuelContainers(vehicle, profile).FirstOrDefault();
		fuel?.ReduceLiquidQuantity(profile.FuelVolumePerMove, null, "movement fuel consumption");
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

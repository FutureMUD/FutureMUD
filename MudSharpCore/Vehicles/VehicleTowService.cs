#nullable enable

using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Vehicles;

public class VehicleTowService : IVehicleTowService
{
	public IReadOnlyList<IVehicle> TowTrainFrom(IVehicle root)
	{
		var vehicles = new List<IVehicle>();
		AddTowTrain(root, vehicles);
		return vehicles;
	}

	public IReadOnlyList<IVehicleTowLink> TowLinksFrom(IVehicle root)
	{
		var links = new List<IVehicleTowLink>();
		AddTowLinks(root, links, new List<IVehicle>());
		return links;
	}

	public double TowTrainWeight(IVehicle root)
	{
		return TowTrainFrom(root).Sum(x => x.ExteriorItem?.Weight ?? 0.0);
	}

	public bool CanHitch(ICharacter actor, IVehicle sourceVehicle, IVehicleTowPointPrototype sourceTowPoint,
		IVehicle targetVehicle, IVehicleTowPointPrototype targetTowPoint, IGameItem? hitchItem, out string reason)
	{
		if (actor is null)
		{
			reason = "There is no such character.";
			return false;
		}

		if (sourceVehicle is null || targetVehicle is null || sourceTowPoint is null || targetTowPoint is null)
		{
			reason = "You must specify valid vehicles and tow points.";
			return false;
		}

		if (SameVehicle(sourceVehicle, targetVehicle))
		{
			reason = "You cannot hitch a vehicle to itself.";
			return false;
		}

		if (sourceVehicle.Destroyed || targetVehicle.Destroyed)
		{
			reason = "Destroyed vehicles cannot be hitched.";
			return false;
		}

		if (!sourceTowPoint.CanTow)
		{
			reason = "The source tow point cannot tow other vehicles.";
			return false;
		}

		if (!targetTowPoint.CanBeTowed)
		{
			reason = "The target tow point cannot be towed.";
			return false;
		}

		if (!TowTypesCompatible(sourceTowPoint, targetTowPoint))
		{
			reason = "Those tow points use incompatible tow types.";
			return false;
		}

		if (sourceVehicle.Location != targetVehicle.Location || sourceVehicle.RoomLayer != targetVehicle.RoomLayer)
		{
			reason = "Both vehicles must be in the same location and layer.";
			return false;
		}

		if (!RequiredAccessAvailable(actor, sourceVehicle, sourceTowPoint, out reason) ||
		    !RequiredAccessAvailable(actor, targetVehicle, targetTowPoint, out reason))
		{
			return false;
		}

		if (sourceVehicle.IsDisabledByDamage(VehicleDamageEffectTargetType.TowPoint, sourceTowPoint.Id))
		{
			reason = $"{sourceTowPoint.Name} is disabled because {sourceVehicle.DamageDisabledReason(VehicleDamageEffectTargetType.TowPoint, sourceTowPoint.Id)}.";
			return false;
		}

		if (targetVehicle.IsDisabledByDamage(VehicleDamageEffectTargetType.TowPoint, targetTowPoint.Id))
		{
			reason = $"{targetTowPoint.Name} is disabled because {targetVehicle.DamageDisabledReason(VehicleDamageEffectTargetType.TowPoint, targetTowPoint.Id)}.";
			return false;
		}

		if (IsTowPointInUse(sourceVehicle, sourceTowPoint))
		{
			reason = "That source tow point is already in use.";
			return false;
		}

		if (IsTowPointInUse(targetVehicle, targetTowPoint))
		{
			reason = "That target tow point is already in use.";
			return false;
		}

		if (IncomingLinks(targetVehicle).Any())
		{
			reason = "That target vehicle is already being towed.";
			return false;
		}

		if (TowTrainFrom(targetVehicle).Any(x => SameVehicle(x, sourceVehicle)))
		{
			reason = "That hitch would create a tow cycle.";
			return false;
		}

		var targetTrainWeight = TowTrainWeight(targetVehicle);
		if (targetTrainWeight > sourceTowPoint.MaximumTowedWeight)
		{
			reason = $"That tow point can only handle {sourceTowPoint.MaximumTowedWeight:N2} weight, but the target train weighs {targetTrainWeight:N2}.";
			return false;
		}

		if (hitchItem is not null)
		{
			if (hitchItem.Deleted || hitchItem.Destroyed)
			{
				reason = "That hitch item is not usable.";
				return false;
			}

			if (hitchItem.InInventoryOf != actor.Body)
			{
				reason = "You must be holding the hitch item.";
				return false;
			}

			if (!actor.Body.CanDrop(hitchItem, 0))
			{
				reason = actor.Body.WhyCannotDrop(hitchItem, 0);
				return false;
			}
		}

		reason = string.Empty;
		return true;
	}

	public bool ValidateLink(IVehicleTowLink link, out string reason)
	{
		if (link is null)
		{
			reason = "tow link is missing";
			return false;
		}

		if (link.IsManuallyDisabled)
		{
			reason = "the link is manually disabled";
			return false;
		}

		var sourceVehicle = link.SourceVehicle;
		if (sourceVehicle is null)
		{
			reason = "the source vehicle is missing";
			return false;
		}

		var targetVehicle = link.TargetVehicle;
		if (targetVehicle is null)
		{
			reason = "the target vehicle is missing";
			return false;
		}

		var sourceTowPoint = link.SourceTowPoint;
		if (sourceTowPoint is null)
		{
			reason = "the source tow point is missing";
			return false;
		}

		var targetTowPoint = link.TargetTowPoint;
		if (targetTowPoint is null)
		{
			reason = "the target tow point is missing";
			return false;
		}

		if (!sourceTowPoint.CanTow)
		{
			reason = "the source tow point cannot tow";
			return false;
		}

		if (!targetTowPoint.CanBeTowed)
		{
			reason = "the target tow point cannot be towed";
			return false;
		}

		if (!TowTypesCompatible(sourceTowPoint, targetTowPoint))
		{
			reason = "the tow point types are incompatible";
			return false;
		}

		if (sourceVehicle.Location != targetVehicle.Location || sourceVehicle.RoomLayer != targetVehicle.RoomLayer)
		{
			reason = "the linked vehicles are not in the same location and layer";
			return false;
		}

		if (sourceVehicle.IsDisabledByDamage(VehicleDamageEffectTargetType.TowPoint, sourceTowPoint.Id))
		{
			reason = $"{sourceTowPoint.Name} is disabled because {sourceVehicle.DamageDisabledReason(VehicleDamageEffectTargetType.TowPoint, sourceTowPoint.Id)}";
			return false;
		}

		if (targetVehicle.IsDisabledByDamage(VehicleDamageEffectTargetType.TowPoint, targetTowPoint.Id))
		{
			reason = $"{targetTowPoint.Name} is disabled because {targetVehicle.DamageDisabledReason(VehicleDamageEffectTargetType.TowPoint, targetTowPoint.Id)}";
			return false;
		}

		if (link.HitchItemId is not null)
		{
			var item = link.HitchItem;
			if (item is null)
			{
				reason = "the hitch item is missing";
				return false;
			}

			if (item.Deleted || item.Destroyed)
			{
				reason = "the hitch item is destroyed";
				return false;
			}

			if (item.ContainedIn is not null || item.InInventoryOf is not null ||
			    item.Location != sourceVehicle.Location || item.RoomLayer != sourceVehicle.RoomLayer)
			{
				reason = "the hitch item is not with the tow train";
				return false;
			}
		}

		if (!string.IsNullOrWhiteSpace(link.WhyInvalid))
		{
			reason = link.WhyInvalid;
			return false;
		}

		var targetTrainWeight = TowTrainWeight(targetVehicle);
		if (targetTrainWeight > sourceTowPoint.MaximumTowedWeight)
		{
			reason = $"target train weighs {targetTrainWeight:N2}, exceeding {sourceTowPoint.Name}'s {sourceTowPoint.MaximumTowedWeight:N2} limit";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public bool CanMoveTowTrain(IVehicle root, ICellExit exit, out IReadOnlyList<IVehicle> towTrain, out string reason)
	{
		var vehicles = new List<IVehicle>();
		towTrain = vehicles;
		if (root is null)
		{
			reason = "There is no such vehicle.";
			return false;
		}

		if (exit is null)
		{
			reason = "There is no such exit.";
			return false;
		}

		if (IncomingLinks(root).Any())
		{
			reason = "That vehicle is currently being towed.";
			return false;
		}

		if (!AddValidatedTowTrain(root, exit, vehicles, new List<IVehicle>(), out reason))
		{
			return false;
		}

		foreach (var group in TowLinksFrom(root)
		                      .Where(x => x.SourceVehicle is not null && x.SourceTowPoint is not null)
		                      .GroupBy(x => (VehicleId: x.SourceVehicle.Id, TowPointId: x.SourceTowPoint.Id))
		                      .Where(x => x.Count() > 1))
		{
			reason = "A tow point in the train is used by more than one link.";
			return false;
		}

		foreach (var group in TowLinksFrom(root)
		                      .Where(x => x.TargetVehicle is not null && x.TargetTowPoint is not null)
		                      .GroupBy(x => (VehicleId: x.TargetVehicle.Id, TowPointId: x.TargetTowPoint.Id))
		                      .Where(x => x.Count() > 1))
		{
			reason = "A towed tow point in the train is used by more than one link.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public bool IsTowPointInUse(IVehicle vehicle, IVehicleTowPointPrototype towPoint, IVehicleTowLink? exceptLink = null)
	{
		if (vehicle is null || towPoint is null)
		{
			return false;
		}

		return Links(vehicle).Any(link =>
			(exceptLink is null || link.Id != exceptLink.Id) &&
			((SameVehicle(link.SourceVehicle, vehicle) && SameTowPoint(link.SourceTowPoint, towPoint)) ||
			 (SameVehicle(link.TargetVehicle, vehicle) && SameTowPoint(link.TargetTowPoint, towPoint))));
	}

	private bool AddValidatedTowTrain(IVehicle vehicle, ICellExit exit, List<IVehicle> vehicles,
		List<IVehicle> path, out string reason)
	{
		if (path.Any(x => SameVehicle(x, vehicle)))
		{
			reason = "The tow train contains a cycle.";
			return false;
		}

		if (vehicles.Any(x => SameVehicle(x, vehicle)))
		{
			reason = "The tow train reaches the same vehicle more than once.";
			return false;
		}

		if (vehicle.Destroyed)
		{
			reason = $"{vehicle.Name} is destroyed and cannot be part of a moving tow train.";
			return false;
		}

		if (vehicle.ExteriorItem is not null && exit.Exit.MaximumSizeToEnter < vehicle.ExteriorItem.Size)
		{
			reason = path.Count == 0
				? "That exit is too small for the vehicle."
				: "That exit is too small for one of the towed vehicles.";
			return false;
		}

		vehicles.Add(vehicle);
		path.Add(vehicle);
		foreach (var link in OutgoingLinks(vehicle))
		{
			if (!ValidateLink(link, out var linkReason))
			{
				reason = $"Tow link #{link.Id:N0} is invalid: {linkReason}.";
				return false;
			}

			var target = link.TargetVehicle;
			if (IncomingLinks(target).Any(x => x.Id != link.Id))
			{
				reason = "A linked towed vehicle is also attached to another towing vehicle.";
				return false;
			}

			if (target.Location != vehicle.Location || target.RoomLayer != vehicle.RoomLayer)
			{
				reason = "A linked towed vehicle is not in the same location and layer.";
				return false;
			}

			if (!AddValidatedTowTrain(target, exit, vehicles, path, out reason))
			{
				return false;
			}
		}

		path.RemoveAt(path.Count - 1);
		reason = string.Empty;
		return true;
	}

	private void AddTowTrain(IVehicle? vehicle, List<IVehicle> vehicles)
	{
		if (vehicle is null || vehicles.Any(x => SameVehicle(x, vehicle)))
		{
			return;
		}

		vehicles.Add(vehicle);
		foreach (var link in OutgoingLinks(vehicle))
		{
			AddTowTrain(link.TargetVehicle, vehicles);
		}
	}

	private void AddTowLinks(IVehicle? vehicle, List<IVehicleTowLink> links, List<IVehicle> visited)
	{
		if (vehicle is null || visited.Any(x => SameVehicle(x, vehicle)))
		{
			return;
		}

		visited.Add(vehicle);
		foreach (var link in OutgoingLinks(vehicle))
		{
			if (links.All(x => x.Id != link.Id))
			{
				links.Add(link);
			}

			AddTowLinks(link.TargetVehicle, links, visited);
		}
	}

	private static IEnumerable<IVehicleTowLink> Links(IVehicle vehicle)
	{
		return vehicle.TowLinks ?? Enumerable.Empty<IVehicleTowLink>();
	}

	private static IEnumerable<IVehicleTowLink> OutgoingLinks(IVehicle vehicle)
	{
		return Links(vehicle).Where(x => SameVehicle(x.SourceVehicle, vehicle));
	}

	private static IEnumerable<IVehicleTowLink> IncomingLinks(IVehicle vehicle)
	{
		return Links(vehicle).Where(x => SameVehicle(x.TargetVehicle, vehicle));
	}

	private static bool RequiredAccessAvailable(ICharacter actor, IVehicle vehicle, IVehicleTowPointPrototype towPoint,
		out string reason)
	{
		reason = string.Empty;
		var required = towPoint.RequiredAccessPoint;
		if (required is null)
		{
			return true;
		}

		var access = (vehicle.AccessPoints ?? Enumerable.Empty<IVehicleAccessPoint>())
			.FirstOrDefault(x => x.Prototype?.Id == required.Id);
		if (access is null)
		{
			reason = $"{towPoint.Name} requires an access point that is missing on {vehicle.Name}.";
			return false;
		}

		if (access.CanUse(actor, out reason))
		{
			return true;
		}

		reason = $"{towPoint.Name} is unavailable: {reason}";
		return false;
	}

	private static bool TowTypesCompatible(IVehicleTowPointPrototype sourceTowPoint,
		IVehicleTowPointPrototype targetTowPoint)
	{
		return sourceTowPoint.TowType.Equals(targetTowPoint.TowType, StringComparison.InvariantCultureIgnoreCase);
	}

	private static bool SameVehicle(IVehicle? lhs, IVehicle? rhs)
	{
		return lhs is not null && rhs is not null &&
		       (ReferenceEquals(lhs, rhs) || lhs.Id != 0 && lhs.Id == rhs.Id);
	}

	private static bool SameTowPoint(IVehicleTowPointPrototype? lhs, IVehicleTowPointPrototype? rhs)
	{
		return lhs is not null && rhs is not null &&
		       (ReferenceEquals(lhs, rhs) || lhs.Id != 0 && lhs.Id == rhs.Id);
	}
}

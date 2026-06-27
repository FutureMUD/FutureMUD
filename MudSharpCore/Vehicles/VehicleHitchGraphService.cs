#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Vehicles;

public class VehicleHitchGraphService : IVehicleHitchGraphService
{
	public IReadOnlyList<VehicleHitchGraphLink> LinksFrom(IFuturemud? gameworld, IVehicle source)
	{
		return AllLinks(gameworld, source is null ? [] : [source])
		       .Where(x => EndpointMatches(x.Source, source))
		       .ToList();
	}

	public IReadOnlyList<VehicleHitchGraphLink> LinksInvolving(IFuturemud? gameworld, IPerceivable perceivable)
	{
		return AllLinks(gameworld, VehicleSeed(perceivable))
		       .Where(x => EndpointMatches(x.Source, perceivable) ||
		                   EndpointMatches(x.Target, perceivable))
		       .ToList();
	}

	public IReadOnlyList<IVehicle> VehicleTrainFrom(IFuturemud? gameworld, IVehicle root)
	{
		return TryBuildVehicleTrain(gameworld, root, out var members, out _, out _)
			? members.Select(x => x.Vehicle).ToList()
			: [];
	}

	public IReadOnlyList<VehicleHitchGraphLink> VehicleTrainLinksFrom(IFuturemud? gameworld, IVehicle root)
	{
		return TryBuildVehicleTrain(gameworld, root, out _, out var links, out _)
			? links
			: [];
	}

	public double VehicleTrainWeight(IFuturemud? gameworld, IVehicle root)
	{
		return VehicleTrainFrom(gameworld, root).Sum(x => x.ExteriorItem?.Weight ?? 0.0);
	}

	public bool TryBuildVehicleTrain(IFuturemud? gameworld, IVehicle root, out VehicleHitchGraphMovePlan movePlan,
		out string reason, bool allowRootIncoming = false)
	{
		if (root is null)
		{
			movePlan = EmptyMovePlan(null);
			reason = "There is no such vehicle.";
			return false;
		}

		if (!TryBuildVehicleTrain(gameworld ?? root.Gameworld, root, out var members, out var links, out reason))
		{
			movePlan = EmptyMovePlan(root);
			return false;
		}

		if (!ValidateTrainGraph(root, members, links, out reason, allowRootIncoming))
		{
			movePlan = EmptyMovePlan(root);
			return false;
		}

		movePlan = BuildMovePlan(root, members, links);
		reason = string.Empty;
		return true;
	}

	public IReadOnlyList<VehicleHitchGraphTowStress> EvaluateTowStress(VehicleHitchGraphMovePlan movePlan,
		double warningRatio = 0.90, double failureStartRatio = 0.95, double maximumFailureChance = 0.25)
	{
		if (movePlan is null)
		{
			return [];
		}

		var results = new List<VehicleHitchGraphTowStress>();
		foreach (var link in movePlan.Links)
		{
			var targetVehicle = link.Target.Vehicle;
			if (targetVehicle is null)
			{
				continue;
			}

			var capacity = LinkCapacity(link);
			if (capacity <= 0.0)
			{
				continue;
			}

			var effectiveWeight = DownstreamWeight(movePlan, targetVehicle);
			var ratio = effectiveWeight / capacity;
			var isWarning = ratio >= warningRatio;
			var canFail = ratio >= failureStartRatio;
			var failureChance = canFail
				? Math.Clamp((ratio - failureStartRatio) / Math.Max(0.0001, 1.0 - failureStartRatio), 0.0, 1.0) * maximumFailureChance
				: 0.0;
			var reason = isWarning
				? $"{targetVehicle.Name} is pulling {effectiveWeight:N2} weight against a {capacity:N2} hitch capacity ({ratio:P0})."
				: string.Empty;
			results.Add(new VehicleHitchGraphTowStress(link, targetVehicle, effectiveWeight, capacity, ratio,
				isWarning, canFail, failureChance, reason));
		}

		return results;
	}

	public bool ValidateLink(VehicleHitchGraphLink link, out string reason)
	{
		if (link is null)
		{
			reason = "the link is missing";
			return false;
		}

		if (link.IsManuallyDisabled)
		{
			reason = "the link is manually disabled";
			return false;
		}

		if (!ValidateEndpoint(link, true, out reason) ||
		    !ValidateEndpoint(link, false, out reason))
		{
			return false;
		}

		var sourceVehicle = link.Source.Vehicle;
		var targetVehicle = link.Target.Vehicle;
		var sourceTowPoint = link.Source.TowPoint;
		var targetTowPoint = link.Target.TowPoint;
		if (sourceTowPoint is not null && targetTowPoint is not null &&
		    !TowTypesCompatible(sourceTowPoint, targetTowPoint))
		{
			reason = "the tow point types are incompatible";
			return false;
		}

		var sourceLocation = EndpointLocation(link.Source);
		var targetLocation = EndpointLocation(link.Target);
		if (sourceLocation is null || targetLocation is null)
		{
			reason = "one of the hitch endpoints is not in the world";
			return false;
		}

		if (sourceLocation != targetLocation || EndpointRoomLayer(link.Source) != EndpointRoomLayer(link.Target))
		{
			reason = link.Kind == VehicleHitchGraphLinkKind.LegacyVehicleTow
				? "the linked vehicles are not in the same location and layer"
				: "the hitch endpoints are not in the same location and layer";
			return false;
		}

		var targetTrainWeight = targetVehicle is null
			? EndpointWeight(link.Target)
			: VehicleTrainWeight(targetVehicle.Gameworld, targetVehicle);
		if (RequiresHitchItem(link) && link.HitchItemId is null && link.HitchItem is null)
		{
			reason = link.Kind == VehicleHitchGraphLinkKind.LegacyVehicleTow
				? "the tow point requires a hitch item"
				: "the vehicle tow point requires a hitch item";
			return false;
		}

		if (link.HitchItemId is not null || link.HitchItem is not null)
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

			if (!HitchItemIsWithChain(link, item, sourceLocation, EndpointRoomLayer(link.Source)))
			{
				reason = link.Kind == VehicleHitchGraphLinkKind.LegacyVehicleTow
					? "the hitch item is not with the tow train"
					: "the hitch item is not with the hitch chain";
				return false;
			}

			if (!HitchGearRules.GearCompatible(item, targetTrainWeight, out reason, false,
				    sourceTowPoint, targetTowPoint))
			{
				return false;
			}
		}

		if (sourceVehicle is not null && sourceTowPoint is not null && targetTrainWeight > sourceTowPoint.MaximumTowedWeight)
		{
			reason = $"target train weighs {targetTrainWeight:N2}, exceeding {sourceTowPoint.Name}'s {sourceTowPoint.MaximumTowedWeight:N2} limit";
			return false;
		}

		if (link.Source.NodeType == VehicleHitchGraphNodeType.Character &&
		    targetVehicle is not null &&
		    targetTowPoint is not null &&
		    targetTrainWeight > targetTowPoint.MaximumTowedWeight)
		{
			reason = $"target train weighs {targetTrainWeight:N2}, exceeding {targetTowPoint.Name}'s {targetTowPoint.MaximumTowedWeight:N2} limit";
			return false;
		}

		if (!string.IsNullOrWhiteSpace(link.InvalidReason))
		{
			reason = link.InvalidReason;
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public bool CanAddVehicleTowLink(ICharacter actor, IVehicle sourceVehicle, IVehicleTowPointPrototype sourceTowPoint,
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

		if (IsTowPointInUse(sourceVehicle.Gameworld, sourceVehicle, sourceTowPoint))
		{
			reason = "That source tow point is already in use.";
			return false;
		}

		if (IsTowPointInUse(targetVehicle.Gameworld, targetVehicle, targetTowPoint))
		{
			reason = "That target tow point is already in use.";
			return false;
		}

		if (IncomingVehicleLinks(targetVehicle.Gameworld, targetVehicle).Any())
		{
			reason = "That target vehicle is already being towed.";
			return false;
		}

		if (VehicleTrainFrom(targetVehicle.Gameworld, targetVehicle).Any(x => SameVehicle(x, sourceVehicle)))
		{
			reason = "That hitch would create a tow cycle.";
			return false;
		}

		var targetTrainWeight = VehicleTrainWeight(targetVehicle.Gameworld, targetVehicle);
		if (targetTrainWeight > sourceTowPoint.MaximumTowedWeight)
		{
			reason = $"That tow point can only handle {sourceTowPoint.MaximumTowedWeight:N2} weight, but the target train weighs {targetTrainWeight:N2}.";
			return false;
		}

		if (HitchGearRules.TowPointsRequireHitchItem(sourceTowPoint, targetTowPoint) && hitchItem is null)
		{
			reason = "Those tow points require a hitch item. Use #3with <item>#0.".SubstituteANSIColour();
			return false;
		}

		if (!ValidateNewHitchItem(actor, hitchItem, targetTrainWeight, out reason, sourceTowPoint, targetTowPoint))
		{
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public bool CanAddCharacterVehicleHitch(ICharacter actor, ICharacter source, IVehicle targetVehicle,
		IVehicleTowPointPrototype targetTowPoint, IGameItem? hitchItem, IDragAid? dragAid, out string reason)
	{
		if (targetVehicle is null || targetTowPoint is null)
		{
			reason = "That is not a valid hitch target.";
			return false;
		}

		if (targetVehicle.Destroyed)
		{
			reason = "Destroyed vehicles cannot be hitched.";
			return false;
		}

		if (!targetTowPoint.CanBeTowed)
		{
			reason = "That tow point cannot be towed.";
			return false;
		}

		if (targetVehicle.Location != source.Location || targetVehicle.RoomLayer != source.RoomLayer)
		{
			reason = "The vehicle must be in the same location and layer.";
			return false;
		}

		if (!RequiredAccessAvailable(actor, targetVehicle, targetTowPoint, out reason))
		{
			return false;
		}

		if (targetVehicle.IsDisabledByDamage(VehicleDamageEffectTargetType.TowPoint, targetTowPoint.Id))
		{
			reason = $"{targetTowPoint.Name} is disabled because {targetVehicle.DamageDisabledReason(VehicleDamageEffectTargetType.TowPoint, targetTowPoint.Id)}.";
			return false;
		}

		if (targetVehicle.ExteriorItem?.AffectedBy<Dragging.DragTarget>() == true)
		{
			reason = $"{targetVehicle.ExteriorItem.HowSeen(actor, true)} is already being pulled or dragged.";
			return false;
		}

		if (targetVehicle.ExteriorItem is null)
		{
			reason = "That vehicle does not have a linked exterior item.";
			return false;
		}

		if (IncomingVehicleLinks(targetVehicle.Gameworld, targetVehicle).Any())
		{
			reason = $"{targetVehicle.ExteriorItem.HowSeen(actor, true)} already has a hitch link.";
			return false;
		}

		var targetTrainWeight = VehicleTrainWeight(targetVehicle.Gameworld, targetVehicle);
		if (targetTrainWeight > targetTowPoint.MaximumTowedWeight)
		{
			reason = $"That tow point can only handle {targetTowPoint.MaximumTowedWeight.ToString("N2", actor)} weight, but the target train weighs {targetTrainWeight.ToString("N2", actor)}.";
			return false;
		}

		if (HitchGearRules.TowPointRequiresHitchItem(targetTowPoint) && hitchItem is null)
		{
			reason = $"That {targetTowPoint.TowType.ColourCommand()} tow point requires a hitch item. Use #3with <item>#0.".SubstituteANSIColour();
			return false;
		}

		if (hitchItem is not null &&
		    !HitchGearRules.GearCompatible(hitchItem, targetTrainWeight, out reason, targetTowPoint))
		{
			return false;
		}

		var pullMultiplier = Math.Max(1.0, targetTowPoint.CharacterPullMultiplier);
		var capacity = (source.MaximumDragWeight - source.Body.ExternalItems.Sum(x => x.Weight)) *
		               (dragAid?.EffortMultiplier ?? 1.0);
		var effectiveWeight = targetTrainWeight / pullMultiplier;
		if (capacity < effectiveWeight)
		{
			reason = $"{source.HowSeen(actor, true)} can only pull {capacity.ToString("N2", actor)} effective weight, but {targetVehicle.ExteriorItem.HowSeen(actor)} and its tow train need {effectiveWeight.ToString("N2", actor)}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public bool CanMoveVehicleTrain(IFuturemud? gameworld, IVehicle root, ICellExit exit,
		out VehicleHitchGraphMovePlan movePlan, out string reason)
	{
		movePlan = EmptyMovePlan(root);
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

		if (IncomingVehicleLinks(gameworld ?? root.Gameworld, root).Any())
		{
			reason = "That vehicle is currently being towed.";
			return false;
		}

		if (!TryBuildVehicleTrain(gameworld ?? root.Gameworld, root, out var members, out var links, out reason))
		{
			return false;
		}

		if (!ValidateTrain(root, exit, members, links, out reason))
		{
			return false;
		}

		movePlan = BuildMovePlan(root, members, links);
		reason = string.Empty;
		return true;
	}

	public bool CanDragVehicleTrain(IFuturemud? gameworld, IVehicle root, ICellExit exit,
		IEnumerable<ICharacter> allowedPullers, out VehicleHitchGraphMovePlan movePlan, out string reason)
	{
		movePlan = EmptyMovePlan(root);
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

		var pullers = allowedPullers?.Where(x => x is not null).ToList() ?? [];
		var incomingLinks = IncomingVehicleLinks(gameworld ?? root.Gameworld, root).ToList();
		var allowedIncomingLinks = incomingLinks
		                           .Where(x => x.Source.NodeType == VehicleHitchGraphNodeType.Character &&
		                                       pullers.Any(y => SameCharacter(y, x.Source.Character)))
		                           .ToList();
		var unexpectedIncoming = incomingLinks.Except(allowedIncomingLinks).FirstOrDefault();
		if (unexpectedIncoming is not null)
		{
			reason = "That vehicle is currently being towed.";
			return false;
		}

		if (!TryBuildVehicleTrain(gameworld ?? root.Gameworld, root, out var members, out var links, out reason))
		{
			return false;
		}

		foreach (var link in allowedIncomingLinks)
		{
			if (links.All(x => x.Key != link.Key))
			{
				links.Insert(0, link);
			}
		}

		if (!ValidateTrain(root, exit, members, links, out reason, allowRootIncoming: true))
		{
			return false;
		}

		movePlan = BuildMovePlan(root, members, links);
		reason = string.Empty;
		return true;
	}

	public bool IsTowPointInUse(IFuturemud? gameworld, IVehicle vehicle, IVehicleTowPointPrototype towPoint,
		IVehicleTowLink? exceptLegacyLink = null)
	{
		if (vehicle is null || towPoint is null)
		{
			return false;
		}

		return AllLinks(gameworld ?? vehicle.Gameworld, [vehicle]).Any(link =>
			(exceptLegacyLink is null ||
			 link.Kind != VehicleHitchGraphLinkKind.LegacyVehicleTow ||
			 link.WrappedLink?.Id != exceptLegacyLink.Id) &&
			((EndpointTowPointMatches(link.Source, vehicle, towPoint)) ||
			 (EndpointTowPointMatches(link.Target, vehicle, towPoint))));
	}

	public void CompleteVehicleTrainMove(VehicleHitchGraphMovePlan movePlan, ICell destination, RoomLayer layer,
		ICellExit exit, IMovement? movement = null, IVehicle? alreadyMovedVehicle = null)
	{
		if (movePlan is null)
		{
			return;
		}

		foreach (var vehicle in movePlan.Vehicles.Where(x => !SameVehicle(x, alreadyMovedVehicle)))
		{
			vehicle.MoveToCell(destination, layer, exit, movement);
		}

		MoveHitchItems(movePlan.HitchItems, destination, layer);
	}

	private bool ValidateTrain(IVehicle root, ICellExit exit, IReadOnlyList<VehicleHitchGraphTrainMember> members,
		IReadOnlyList<VehicleHitchGraphLink> links, out string reason, bool allowRootIncoming = false)
	{
		foreach (var member in members)
		{
			var vehicle = member.Vehicle;
			if (vehicle.Destroyed)
			{
				reason = $"{vehicle.Name} is destroyed and cannot be part of a moving tow train.";
				return false;
			}

			if (vehicle.ExteriorItem is not null && exit.Exit.MaximumSizeToEnter < vehicle.ExteriorItem.Size)
			{
				reason = SameVehicle(vehicle, root)
					? "That exit is too small for the vehicle."
					: "That exit is too small for one of the towed vehicles.";
				return false;
			}
		}

		if (!ValidateTrainGraph(root, members, links, out reason, allowRootIncoming))
		{
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private bool ValidateTrainGraph(IVehicle root, IReadOnlyList<VehicleHitchGraphTrainMember> members,
		IReadOnlyList<VehicleHitchGraphLink> links, out string reason, bool allowRootIncoming = false)
	{
		foreach (var link in links)
		{
			if (!ValidateLink(link, out var linkReason))
			{
				reason = link.Kind == VehicleHitchGraphLinkKind.LegacyVehicleTow
					? $"Tow link #{link.WrappedLink?.Id.ToString("N0") ?? "0"} is invalid: {linkReason}."
					: $"Hitch link #{link.WrappedLink?.Id.ToString("N0") ?? "0"} is invalid: {linkReason}.";
				return false;
			}
		}

		foreach (var group in links
		                      .Where(x => x.Source.Vehicle is not null && x.Source.TowPoint is not null)
		                      .GroupBy(x => (VehicleId: x.Source.Vehicle!.Id, TowPointId: x.Source.TowPoint!.Id))
		                      .Where(x => x.Count() > 1))
		{
			reason = "A tow point in the train is used by more than one link.";
			return false;
		}

		foreach (var group in links
		                      .Where(x => x.Target.Vehicle is not null && x.Target.TowPoint is not null)
		                      .GroupBy(x => (VehicleId: x.Target.Vehicle!.Id, TowPointId: x.Target.TowPoint!.Id))
		                      .Where(x => x.Count() > 1))
		{
			reason = "A towed tow point in the train is used by more than one link.";
			return false;
		}

		foreach (var member in members.Where(x => !SameVehicle(x.Vehicle, root)))
		{
			var incoming = IncomingVehicleLinks(member.Vehicle.Gameworld, member.Vehicle)
				.Where(x => x.Target.Vehicle is not null &&
				            SameVehicle(x.Target.Vehicle, member.Vehicle))
				.Where(x => links.All(y => y.Key != x.Key))
				.ToList();
			if (incoming.Any())
			{
				reason = "A linked towed vehicle is also attached to another towing vehicle.";
				return false;
			}
		}

		if (!allowRootIncoming && IncomingVehicleLinks(root.Gameworld, root).Any())
		{
			reason = "That vehicle is currently being towed.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private bool TryBuildVehicleTrain(IFuturemud? gameworld, IVehicle root,
		out List<VehicleHitchGraphTrainMember> members, out List<VehicleHitchGraphLink> links, out string reason)
	{
		members = [];
		links = [];
		if (root is null)
		{
			reason = "There is no such vehicle.";
			return false;
		}

		return AddVehicleToTrain(gameworld ?? root.Gameworld, root, null, 0, [], members, links, out reason);
	}

	private bool AddVehicleToTrain(IFuturemud? gameworld, IVehicle vehicle, VehicleHitchGraphLink? incomingLink,
		int depth, List<IVehicle> path, List<VehicleHitchGraphTrainMember> members, List<VehicleHitchGraphLink> links,
		out string reason)
	{
		if (path.Any(x => SameVehicle(x, vehicle)))
		{
			reason = "The tow train contains a cycle.";
			return false;
		}

		if (members.Any(x => SameVehicle(x.Vehicle, vehicle)))
		{
			reason = "The tow train reaches the same vehicle more than once.";
			return false;
		}

		members.Add(new VehicleHitchGraphTrainMember(vehicle, depth, incomingLink));
		path.Add(vehicle);
		foreach (var link in OutgoingVehicleLinks(gameworld, vehicle))
		{
			if (links.All(x => x.Key != link.Key))
			{
				links.Add(link);
			}

			var target = link.Target.Vehicle;
			if (target is null)
			{
				continue;
			}

			if (!AddVehicleToTrain(gameworld, target, link, depth + 1, path, members, links, out reason))
			{
				return false;
			}
		}

		path.RemoveAt(path.Count - 1);
		reason = string.Empty;
		return true;
	}

	private IReadOnlyList<VehicleHitchGraphLink> IncomingVehicleLinks(IFuturemud? gameworld, IVehicle vehicle)
	{
		return AllLinks(gameworld ?? vehicle.Gameworld, [vehicle])
		       .Where(x => x.Target.Vehicle is not null && SameVehicle(x.Target.Vehicle, vehicle))
		       .ToList();
	}

	private IReadOnlyList<VehicleHitchGraphLink> OutgoingVehicleLinks(IFuturemud? gameworld, IVehicle vehicle)
	{
		return AllLinks(gameworld ?? vehicle.Gameworld, [vehicle])
		       .Where(x => x.Source.Vehicle is not null && SameVehicle(x.Source.Vehicle, vehicle))
		       .ToList();
	}

	private IReadOnlyList<VehicleHitchGraphLink> AllLinks(IFuturemud? gameworld, IEnumerable<IVehicle> seedVehicles)
	{
		var links = new List<VehicleHitchGraphLink>();
		foreach (var link in LegacyTowLinks(gameworld, seedVehicles))
		{
			AddDistinct(links, link);
		}

		if (gameworld is null)
		{
			return links;
		}

		foreach (var link in (gameworld.VehicleHitchLinks ?? Enumerable.Empty<IVehicleHitchLink>())
		         .Select(PersistentHitchLink))
		{
			AddDistinct(links, link);
		}

		foreach (var link in TransientHitchLinks(gameworld))
		{
			AddDistinct(links, link);
		}

		return links;
	}

	private IEnumerable<VehicleHitchGraphLink> LegacyTowLinks(IFuturemud? gameworld, IEnumerable<IVehicle> seedVehicles)
	{
		var seen = new HashSet<string>();
		var vehicles = gameworld?.Vehicles?.ToList() ?? ReachableVehicles(seedVehicles).ToList();
		foreach (var link in vehicles
		                     .SelectMany(x => x.TowLinks ?? Enumerable.Empty<IVehicleTowLink>()))
		{
			var graphLink = LegacyTowLink(link);
			if (seen.Add(graphLink.Key))
			{
				yield return graphLink;
			}
		}
	}

	private IEnumerable<IVehicle> ReachableVehicles(IEnumerable<IVehicle> seedVehicles)
	{
		var queue = new Queue<IVehicle>(seedVehicles.Where(x => x is not null));
		var seen = new List<IVehicle>();
		while (queue.Any())
		{
			var vehicle = queue.Dequeue();
			if (seen.Any(x => SameVehicle(x, vehicle)))
			{
				continue;
			}

			seen.Add(vehicle);
			yield return vehicle;
			foreach (var link in vehicle.TowLinks ?? Enumerable.Empty<IVehicleTowLink>())
			{
				if (link.SourceVehicle is not null)
				{
					queue.Enqueue(link.SourceVehicle);
				}

				if (link.TargetVehicle is not null)
				{
					queue.Enqueue(link.TargetVehicle);
				}
			}
		}
	}

	private IEnumerable<VehicleHitchGraphLink> TransientHitchLinks(IFuturemud gameworld)
	{
		foreach (var character in (gameworld.Actors ?? Enumerable.Empty<ICharacter>()).Where(x => x is not null))
		{
			foreach (var hitch in character.EffectsOfType<CharacterHitch>()
			                         .Where(x => x.VehicleHitchLinkId is null))
			{
				yield return TransientCharacterHitchLink(character, hitch);
			}

			foreach (var drag in character.EffectsOfType<Dragging>())
			{
				if (character.EffectsOfType<CharacterHitch>().Any(x => x.Target == drag.Target))
				{
					continue;
				}

				if (VehicleForPerceivable(drag.Target) is null && drag.Target is not ICharacter)
				{
					continue;
				}

				yield return TransientDragLink(character, drag);
			}
		}
	}

	private static VehicleHitchGraphLink LegacyTowLink(IVehicleTowLink link)
	{
		return new VehicleHitchGraphLink(
			$"LegacyVehicleTow:{link.Id}:{link.SourceVehicleId}:{link.TargetVehicleId}:{link.SourceTowPointPrototypeId}:{link.TargetTowPointPrototypeId}",
			VehicleHitchGraphLinkKind.LegacyVehicleTow,
			VehicleEndpoint(link.SourceVehicle, link.SourceTowPoint),
			VehicleEndpoint(link.TargetVehicle, link.TargetTowPoint),
			link.HitchItem,
			link.HitchItemId,
			link.IsManuallyDisabled,
			link.IsDisabled,
			link.WhyInvalid,
			link);
	}

	private static VehicleHitchGraphLink PersistentHitchLink(IVehicleHitchLink link)
	{
		return new VehicleHitchGraphLink(
			$"PersistentHitch:{link.Id}:{link.SourceType}:{link.SourceVehicleId}:{link.SourceCharacterId}:{link.TargetType}:{link.TargetVehicleId}:{link.TargetCharacterId}",
			VehicleHitchGraphLinkKind.PersistentHitch,
			Endpoint(link.SourceType, link.SourceVehicle, link.SourceCharacter, link.SourceTowPoint),
			Endpoint(link.TargetType, link.TargetVehicle, link.TargetCharacter, link.TargetTowPoint),
			link.HitchItem,
			link.HitchItemId,
			link.IsManuallyDisabled,
			link.IsDisabled,
			link.WhyInvalid,
			link);
	}

	private static VehicleHitchGraphLink TransientCharacterHitchLink(ICharacter source, CharacterHitch hitch)
	{
		var targetVehicle = VehicleForPerceivable(hitch.Target);
		return new VehicleHitchGraphLink(
			$"TransientCharacterHitch:{CharacterInstanceIdentityComparer.IdentityId(source)}:{CharacterInstanceIdentityComparer.InstanceId(source)}:{CharacterInstanceIdentityComparer.FrameworkItemId(hitch.Target)}:{hitch.TargetTowPointId}:{hitch.HitchItemId}",
			VehicleHitchGraphLinkKind.TransientCharacterHitch,
			CharacterEndpoint(source),
			targetVehicle is null
				? CharacterEndpoint(hitch.Target as ICharacter)
				: VehicleEndpoint(targetVehicle,
					targetVehicle.Prototype.TowPoints.FirstOrDefault(x => x.Id == hitch.TargetTowPointId)),
			HitchItem(source.Gameworld, hitch.HitchItemId),
			hitch.HitchItemId,
			false,
			false,
			string.Empty,
			null);
	}

	private static VehicleHitchGraphLink TransientDragLink(ICharacter source, Dragging drag)
	{
		var targetVehicle = VehicleForPerceivable(drag.Target);
		return new VehicleHitchGraphLink(
			$"TransientDrag:{CharacterInstanceIdentityComparer.IdentityId(source)}:{CharacterInstanceIdentityComparer.InstanceId(source)}:{CharacterInstanceIdentityComparer.FrameworkItemId(drag.Target)}",
			VehicleHitchGraphLinkKind.TransientDrag,
			CharacterEndpoint(source),
			targetVehicle is null ? CharacterEndpoint(drag.Target as ICharacter) : VehicleEndpoint(targetVehicle, null),
			null,
			null,
			false,
			false,
			string.Empty,
			null);
	}

	private static VehicleHitchGraphEndpoint Endpoint(VehicleHitchEndpointType type, IVehicle? vehicle,
		ICharacter? character, IVehicleTowPointPrototype? towPoint)
	{
		return type == VehicleHitchEndpointType.Vehicle
			? VehicleEndpoint(vehicle, towPoint)
			: CharacterEndpoint(character);
	}

	private static VehicleHitchGraphEndpoint VehicleEndpoint(IVehicle? vehicle, IVehicleTowPointPrototype? towPoint)
	{
		return new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Vehicle, vehicle, null, towPoint);
	}

	private static VehicleHitchGraphEndpoint CharacterEndpoint(ICharacter? character)
	{
		return new VehicleHitchGraphEndpoint(VehicleHitchGraphNodeType.Character, null, character, null);
	}

	private static IGameItem? HitchItem(IFuturemud? gameworld, long? hitchItemId)
	{
		return hitchItemId is null ? null : gameworld?.TryGetItem(hitchItemId.Value, true);
	}

	private static IVehicle? VehicleForPerceivable(IPerceivable? perceivable)
	{
		return perceivable is IGameItem item
			? item.GetItemType<IVehicleExterior>()?.Vehicle
			: null;
	}

	private static IEnumerable<IVehicle> VehicleSeed(IPerceivable perceivable)
	{
		var vehicle = VehicleForPerceivable(perceivable);
		return vehicle is null ? [] : [vehicle];
	}

	private static void AddDistinct(List<VehicleHitchGraphLink> links, VehicleHitchGraphLink link)
	{
		if (links.All(x => x.Key != link.Key))
		{
			links.Add(link);
		}
	}

	private static double LinkCapacity(VehicleHitchGraphLink link)
	{
		var capacities = new List<double>();
		if (link.Source.Vehicle is not null && link.Source.TowPoint is not null)
		{
			capacities.Add(link.Source.TowPoint.MaximumTowedWeight);
		}

		if (link.Source.NodeType == VehicleHitchGraphNodeType.Character && link.Target.TowPoint is not null)
		{
			capacities.Add(link.Target.TowPoint.MaximumTowedWeight);
		}

		return capacities.Where(x => x > 0.0).DefaultIfEmpty(0.0).Min();
	}

	private static double DownstreamWeight(VehicleHitchGraphMovePlan movePlan, IVehicle targetVehicle)
	{
		return movePlan.Members
		               .Where(x => IsDownstreamOf(movePlan, x.Vehicle, targetVehicle))
		               .Sum(x => x.Vehicle.ExteriorItem?.Weight ?? 0.0);
	}

	private static bool IsDownstreamOf(VehicleHitchGraphMovePlan movePlan, IVehicle candidate, IVehicle ancestor)
	{
		if (SameVehicle(candidate, ancestor))
		{
			return true;
		}

		var member = movePlan.Members.FirstOrDefault(x => SameVehicle(x.Vehicle, candidate));
		while (member?.IncomingLink?.Source.Vehicle is not null)
		{
			var source = member.IncomingLink.Source.Vehicle;
			if (SameVehicle(source, ancestor))
			{
				return true;
			}

			member = movePlan.Members.FirstOrDefault(x => SameVehicle(x.Vehicle, source));
		}

		return false;
	}
	private VehicleHitchGraphMovePlan BuildMovePlan(IVehicle root, IReadOnlyList<VehicleHitchGraphTrainMember> members,
		IReadOnlyList<VehicleHitchGraphLink> links)
	{
		var hitchItems = links
		                 .Select(x => x.HitchItem)
		                 .Where(x => x is not null)
		                 .Cast<IGameItem>()
		                 .DistinctBy(x => x.Id)
		                 .ToList();
		return new VehicleHitchGraphMovePlan(root, members, links, hitchItems,
			members.Sum(x => x.Vehicle.ExteriorItem?.Weight ?? 0.0));
	}

	private static VehicleHitchGraphMovePlan EmptyMovePlan(IVehicle? root)
	{
		return root is null
			? new VehicleHitchGraphMovePlan(null!, [], [], [], 0.0)
			: new VehicleHitchGraphMovePlan(root, [new VehicleHitchGraphTrainMember(root, 0, null)], [], [], root.ExteriorItem?.Weight ?? 0.0);
	}

	private bool ValidateEndpoint(VehicleHitchGraphLink link, bool source, out string reason)
	{
		var endpoint = source ? link.Source : link.Target;
		var label = source ? "source" : "target";
		switch (endpoint.NodeType)
		{
			case VehicleHitchGraphNodeType.Vehicle:
				if (endpoint.Vehicle is null)
				{
					reason = $"the {label} vehicle is missing";
					return false;
				}

				if (link.Kind != VehicleHitchGraphLinkKind.TransientDrag && endpoint.TowPoint is null)
				{
					reason = $"the {label} tow point is missing";
					return false;
				}

				if (source && endpoint.TowPoint?.CanTow == false)
				{
					reason = "the source tow point cannot tow";
					return false;
				}

				if (!source && endpoint.TowPoint?.CanBeTowed == false)
				{
					reason = "the target tow point cannot be towed";
					return false;
				}

				if (endpoint.Vehicle.Destroyed)
				{
					reason = $"the {label} vehicle is destroyed";
					return false;
				}

				if (endpoint.TowPoint is not null &&
				    endpoint.Vehicle.IsDisabledByDamage(VehicleDamageEffectTargetType.TowPoint, endpoint.TowPoint.Id))
				{
					reason = $"{endpoint.TowPoint.Name} is disabled because {endpoint.Vehicle.DamageDisabledReason(VehicleDamageEffectTargetType.TowPoint, endpoint.TowPoint.Id)}";
					return false;
				}

				reason = string.Empty;
				return true;
			case VehicleHitchGraphNodeType.Character:
				if (endpoint.Character is null)
				{
					reason = $"the {label} character is missing";
					return false;
				}

				reason = string.Empty;
				return true;
			default:
				reason = $"the {label} endpoint type is invalid";
				return false;
		}
	}

	private static ICell? EndpointLocation(VehicleHitchGraphEndpoint endpoint)
	{
		return endpoint.NodeType switch
		{
			VehicleHitchGraphNodeType.Vehicle => endpoint.Vehicle?.Location,
			VehicleHitchGraphNodeType.Character => endpoint.Character?.Location,
			_ => null
		};
	}

	private static RoomLayer EndpointRoomLayer(VehicleHitchGraphEndpoint endpoint)
	{
		return endpoint.NodeType switch
		{
			VehicleHitchGraphNodeType.Vehicle => endpoint.Vehicle?.RoomLayer ?? RoomLayer.GroundLevel,
			VehicleHitchGraphNodeType.Character => endpoint.Character?.RoomLayer ?? RoomLayer.GroundLevel,
			_ => RoomLayer.GroundLevel
		};
	}

	private static double EndpointWeight(VehicleHitchGraphEndpoint endpoint)
	{
		return endpoint.NodeType switch
		{
			VehicleHitchGraphNodeType.Vehicle => endpoint.Vehicle?.ExteriorItem?.Weight ?? 0.0,
			VehicleHitchGraphNodeType.Character => endpoint.Character?.Weight ?? 0.0,
			_ => 0.0
		};
	}

	private static bool EndpointMatches(VehicleHitchGraphEndpoint endpoint, IVehicle? vehicle)
	{
		return endpoint.Vehicle is not null && SameVehicle(endpoint.Vehicle, vehicle);
	}

	private static bool EndpointMatches(VehicleHitchGraphEndpoint endpoint, IPerceivable? perceivable)
	{
		return endpoint.NodeType switch
		{
			VehicleHitchGraphNodeType.Vehicle => VehicleForPerceivable(perceivable) is { } vehicle &&
			                                    SameVehicle(endpoint.Vehicle, vehicle),
			VehicleHitchGraphNodeType.Character => perceivable is ICharacter character &&
			                                      SameCharacter(endpoint.Character, character),
			_ => false
		};
	}

	private static bool EndpointTowPointMatches(VehicleHitchGraphEndpoint endpoint, IVehicle vehicle,
		IVehicleTowPointPrototype towPoint)
	{
		return endpoint.Vehicle is not null &&
		       endpoint.TowPoint is not null &&
		       SameVehicle(endpoint.Vehicle, vehicle) &&
		       SameTowPoint(endpoint.TowPoint, towPoint);
	}

	private static bool RequiresHitchItem(VehicleHitchGraphLink link)
	{
		return link.Kind switch
		{
			VehicleHitchGraphLinkKind.LegacyVehicleTow =>
				HitchGearRules.TowPointsRequireHitchItem(link.Source.TowPoint, link.Target.TowPoint),
			VehicleHitchGraphLinkKind.TransientDrag => false,
			_ => HitchGearRules.TowPointRequiresHitchItem(link.Source.TowPoint) ||
			     HitchGearRules.TowPointRequiresHitchItem(link.Target.TowPoint)
		};
	}

	private static bool HitchItemIsWithChain(VehicleHitchGraphLink link, IGameItem item, ICell sourceLocation,
		RoomLayer sourceLayer)
	{
		if (item.Location == sourceLocation && item.RoomLayer == sourceLayer && item.ContainedIn is null &&
		    item.InInventoryOf is null)
		{
			return true;
		}

		return link.Source.Character?.Body.ExternalItems.Any(x => x.Id == item.Id) == true ||
		       link.Target.Character?.Body.ExternalItems.Any(x => x.Id == item.Id) == true;
	}

	private static bool ValidateNewHitchItem(ICharacter actor, IGameItem? hitchItem, double targetTrainWeight,
		out string reason, params IVehicleTowPointPrototype?[] towPoints)
	{
		if (hitchItem is null)
		{
			reason = string.Empty;
			return true;
		}

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

		return HitchGearRules.GearCompatible(hitchItem, targetTrainWeight, out reason, towPoints);
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

	private static void MoveHitchItems(IEnumerable<IGameItem> hitchItems, ICell destination, RoomLayer layer)
	{
		foreach (var item in hitchItems.Where(x => x is not null).DistinctBy(x => x.Id).ToList())
		{
			if (item.ContainedIn is not null || item.InInventoryOf is not null)
			{
				continue;
			}

			item.Location?.Extract(item);
			item.RoomLayer = layer;
			destination.Insert(item, true);
		}
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

	private static bool SameCharacter(ICharacter? lhs, ICharacter? rhs)
	{
		return lhs is not null && rhs is not null &&
		       CharacterInstanceIdentityComparer.SamePhysicalInstance(lhs, rhs);
	}
}

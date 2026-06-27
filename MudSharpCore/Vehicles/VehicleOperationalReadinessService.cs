#nullable enable

using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Vehicles;

public class VehicleOperationalReadinessService : IVehicleOperationalReadinessService
{
	private readonly IVehicleHitchGraphService _graphService;

	public VehicleOperationalReadinessService() : this(new VehicleHitchGraphService())
	{
	}

	public VehicleOperationalReadinessService(IVehicleHitchGraphService graphService)
	{
		_graphService = graphService;
	}

	public bool CanPerformAction(IVehicle vehicle, ICharacter actor, VehicleOperationalAction action,
		out VehicleAccessPermissionResult result)
	{
		var requiredLevel = RequiredAccessLevel(action);
		if (vehicle is null)
		{
			result = new VehicleAccessPermissionResult(false, action, requiredLevel, null, "There is no such vehicle.");
			return false;
		}

		if (actor is null)
		{
			result = new VehicleAccessPermissionResult(false, action, requiredLevel, null, "There is no such character.");
			return false;
		}

		if (actor.IsAdministrator())
		{
			result = new VehicleAccessPermissionResult(true, action, requiredLevel, null, string.Empty);
			return true;
		}

		var states = vehicle.AccessStates.ToList();
		if (!states.Any())
		{
			result = new VehicleAccessPermissionResult(true, action, requiredLevel, null, string.Empty);
			return true;
		}

		var matching = states.FirstOrDefault(x => x.Character?.SameIdentity(actor) == true && AccessMatches(x, action, requiredLevel));
		if (matching is not null)
		{
			result = new VehicleAccessPermissionResult(true, action, requiredLevel, matching, string.Empty);
			return true;
		}

		result = new VehicleAccessPermissionResult(false, action, requiredLevel, null,
			$"You do not have {ActionTag(action).ColourCommand()} access to {vehicle.Name.ColourName()}.");
		return false;
	}

	public IReadOnlyList<VehicleModuleReadiness> ModuleReadiness(IVehicle vehicle)
	{
		return vehicle?.Installations.Select(x =>
		{
			var item = x.InstalledItem;
			var installable = item?.GetItemType<IVehicleInstallable>();
			var functional = IsInstallationFunctional(x, out var reason);
			var movement = IsInstallationFunctionalForMovement(x, out var movementReason);
			return new VehicleModuleReadiness(x, item, installable, functional, movement,
				movement ? string.Empty : movementReason.IfNullOrWhiteSpace(reason));
		}).ToList() ?? [];
	}

	public bool IsInstallationFunctional(IVehicleInstallation installation, out string reason)
	{
		if (installation is null)
		{
			reason = "there is no such installation point";
			return false;
		}

		if (installation.IsDisabled)
		{
			reason = installation.Vehicle.DamageDisabledReason(VehicleDamageEffectTargetType.InstallationPoint, installation.Prototype.Id);
			reason = string.IsNullOrWhiteSpace(reason)
				? "the installation point is disabled"
				: $"the installation point is disabled because {reason}";
			return false;
		}

		var item = installation.InstalledItem;
		if (item is null)
		{
			reason = "no module is installed";
			return false;
		}

		if (item.Deleted || item.Destroyed)
		{
			reason = "the installed module is destroyed";
			return false;
		}

		var installable = item.GetItemType<IVehicleInstallable>();
		if (installable is null)
		{
			reason = "the installed item is not a vehicle module";
			return false;
		}

		if (!installable.IsFunctional(out reason))
		{
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public bool IsInstallationFunctionalForMovement(IVehicleInstallation installation, out string reason)
	{
		if (!IsInstallationFunctional(installation, out reason))
		{
			return false;
		}

		var installable = installation.InstalledItem?.GetItemType<IVehicleInstallable>();
		if (installable is not null && !installable.IsFunctionalForMovement(out reason))
		{
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public bool HasFunctionalRole(IVehicle vehicle, string role, out string reason)
	{
		if (string.IsNullOrWhiteSpace(role))
		{
			reason = string.Empty;
			return true;
		}

		foreach (var installation in vehicle.Installations)
		{
			var installable = installation.InstalledItem?.GetItemType<IVehicleInstallable>();
			if (installable?.Role.EqualTo(role) != true)
			{
				continue;
			}

			if (IsInstallationFunctionalForMovement(installation, out reason))
			{
				reason = string.Empty;
				return true;
			}
		}

		reason = $"That vehicle requires a functional {role.ColourCommand()} module to move.";
		return false;
	}

	public bool HasFuel(IVehicle vehicle, IVehicleMovementProfilePrototype profile,
		out IReadOnlyList<VehicleResourceCandidate> candidates, out string reason)
	{
		if (profile.FuelLiquidId is null || profile.FuelVolumePerMove <= 0.0)
		{
			candidates = [];
			reason = string.Empty;
			return true;
		}

		var fuelLiquidId = profile.FuelLiquidId.Value;
		var results = new List<VehicleResourceCandidate>();
		foreach (var installation in vehicle.Installations)
		{
			var item = installation.InstalledItem;
			if (!IsInstallationFunctionalForMovement(installation, out var moduleReason))
			{
				results.Add(new VehicleResourceCandidate(installation, item, "fuel", false, moduleReason));
				continue;
			}

			var containers = item!.GetItemTypes<ILiquidContainer>().ToList();
			if (!containers.Any())
			{
				results.Add(new VehicleResourceCandidate(installation, item, "fuel", false, "the module is not a fuel container"));
				continue;
			}

			var usable = HasUsableFuel(containers, fuelLiquidId, profile.FuelVolumePerMove, out var fuelReason);
			results.Add(new VehicleResourceCandidate(installation, item, "fuel", usable, fuelReason));
		}

		candidates = results;
		if (results.Any(x => x.Available))
		{
			reason = string.Empty;
			return true;
		}

		reason = "That vehicle does not have enough configured fuel to move.";
		return false;
	}

	private static bool HasUsableFuel(IEnumerable<ILiquidContainer> containers, long fuelLiquidId, double requiredVolume,
		out string reason)
	{
		var containerList = containers.ToList();
		if (containerList.All(x => FuelVolume(x, fuelLiquidId) <= 0.0))
		{
			reason = "the module contains the wrong fuel";
			return false;
		}

		if (containerList.All(x => FuelVolume(x, fuelLiquidId) < requiredVolume))
		{
			reason = "the module does not contain enough configured fuel";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	private static double FuelVolume(ILiquidContainer container, long fuelLiquidId)
	{
		return container.LiquidMixture?.Instances
			.Where(x => x.Liquid.Id == fuelLiquidId)
			.Sum(x => x.Amount) ?? 0.0;
	}

	private static void ConsumeFuel(ILiquidContainer container, long fuelLiquidId, double amount)
	{
		var mixture = container.LiquidMixture;
		if (mixture is null || amount <= 0.0)
		{
			return;
		}

		var remaining = amount;
		foreach (var instance in mixture.Instances
		                                .Where(x => x.Liquid.Id == fuelLiquidId)
		                                .ToList())
		{
			var removed = Math.Min(instance.Amount, remaining);
			mixture.RemoveLiquidVolume(instance, removed);
			remaining -= removed;
			if (remaining <= 0.0)
			{
				break;
			}
		}

		container.LiquidMixture = mixture;
	}

	public bool HasPower(IVehicle vehicle, IVehicleMovementProfilePrototype profile,
		out IReadOnlyList<VehicleResourceCandidate> candidates, out string reason)
	{
		if (profile.RequiredPowerSpikeInWatts <= 0.0)
		{
			candidates = [];
			reason = string.Empty;
			return true;
		}

		var results = new List<VehicleResourceCandidate>();
		foreach (var installation in vehicle.Installations)
		{
			var usable = TryGetUsablePowerProducer(installation, profile.RequiredPowerSpikeInWatts, out var item,
				out _, out var candidateReason);
			results.Add(new VehicleResourceCandidate(installation, item, "power", usable, candidateReason));
		}

		candidates = results;
		if (results.Any(x => x.Available))
		{
			reason = string.Empty;
			return true;
		}

		reason = "That vehicle does not have enough available power to move.";
		return false;
	}

	private bool TryGetUsablePowerProducer(IVehicleInstallation installation, double requiredPowerSpikeInWatts,
		out IGameItem? item, out IProducePower? producer, out string reason)
	{
		item = installation.InstalledItem;
		producer = null;
		if (!IsInstallationFunctionalForMovement(installation, out reason))
		{
			return false;
		}

		producer = item!.GetItemType<IProducePower>();
		if (producer is null)
		{
			reason = "the module does not produce power";
			return false;
		}

		if (item.GetItemType<IOnOff>()?.SwitchedOn == false)
		{
			reason = "the module is switched off";
			return false;
		}

		if (!producer.ProducingPower)
		{
			reason = "the module is not producing power";
			return false;
		}

		if (!producer.CanDrawdownSpike(requiredPowerSpikeInWatts))
		{
			reason = "the module cannot supply the required power spike";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public void ConsumeMovementResources(IVehicle vehicle, IVehicleMovementProfilePrototype profile)
	{
		if (profile.RequiredPowerSpikeInWatts > 0.0)
		{
			vehicle.Installations
			       .Select(x => TryGetUsablePowerProducer(x, profile.RequiredPowerSpikeInWatts, out _, out var producer, out _)
				       ? producer
				       : null)
			       .FirstOrDefault(x => x is not null)
			       ?.DrawdownSpike(profile.RequiredPowerSpikeInWatts);
		}

		if (profile.FuelLiquidId is null || profile.FuelVolumePerMove <= 0.0)
		{
			return;
		}

		var fuelLiquidId = profile.FuelLiquidId.Value;
		var fuelContainer = vehicle.Installations
		                           .Where(x => IsInstallationFunctionalForMovement(x, out _))
		                           .Select(x => x.InstalledItem)
		                           .Where(x => x is not null)
		                           .SelectMany(x => x!.GetItemTypes<ILiquidContainer>())
		                           .FirstOrDefault(x => FuelVolume(x, fuelLiquidId) >= profile.FuelVolumePerMove);
		if (fuelContainer is not null)
		{
			ConsumeFuel(fuelContainer, fuelLiquidId, profile.FuelVolumePerMove);
		}
	}

	public VehicleOperationalReadinessReport BuildReport(IVehicle vehicle, ICharacter voyeur,
		VehicleHitchGraphMovePlan? movePlan = null, IVehicleMovementProfilePrototype? movementProfile = null)
	{
		var modules = ModuleReadiness(vehicle);
		IReadOnlyList<VehicleResourceCandidate> fuel = [];
		IReadOnlyList<VehicleResourceCandidate> power = [];
		if (movementProfile is not null)
		{
			HasFuel(vehicle, movementProfile, out fuel, out _);
			HasPower(vehicle, movementProfile, out power, out _);
		}

		movePlan ??= BuildCurrentMovePlan(vehicle);
		IReadOnlyList<VehicleHitchGraphTowStress> stress = movePlan is null ? [] : _graphService.EvaluateTowStress(movePlan);
		var issues = new List<VehicleOperationalIssue>();

		foreach (var zone in vehicle.DamageZones.Where(x => x.Status != VehicleSystemStatus.Functional || x.CurrentDamage > 0.0))
		{
			issues.Add(new VehicleOperationalIssue(VehicleOperationalSubsystem.Damage,
				zone.Status == VehicleSystemStatus.Destroyed ? VehicleOperationalSeverity.Blocking : VehicleOperationalSeverity.Warning,
				zone.Id, zone.Name, $"{zone.Name} is {zone.Status.DescribeEnum()} with {zone.CurrentDamage:N2} damage.",
				$"repair {vehicle.ExteriorItem?.HowSeen(voyeur) ?? vehicle.Name} with <kit>"));
		}

		foreach (var module in modules.Where(x => !x.IsFunctionalForMovement))
		{
			issues.Add(new VehicleOperationalIssue(VehicleOperationalSubsystem.Module, VehicleOperationalSeverity.Warning,
				module.Installation.Id, module.Installation.Prototype.Name, module.Reason,
				module.Item is null ? "install a compatible module" : "repair or replace the installed module"));
		}

		if (vehicle.ExteriorItem is not null)
		{
			foreach (var link in _graphService.LinksInvolving(vehicle.Gameworld, vehicle.ExteriorItem).Where(x => !string.IsNullOrWhiteSpace(x.InvalidReason)))
			{
				issues.Add(new VehicleOperationalIssue(VehicleOperationalSubsystem.HitchTow, VehicleOperationalSeverity.Blocking,
					link.WrappedLink?.Id, link.WrappedLink?.Name ?? link.Key, link.InvalidReason, "unhitch and re-hitch after repairing the cause"));
			}
		}

		foreach (var towStress in stress.Where(x => x.IsWarning))
		{
			issues.Add(new VehicleOperationalIssue(VehicleOperationalSubsystem.HitchTow,
				towStress.CanFail ? VehicleOperationalSeverity.Warning : VehicleOperationalSeverity.Information,
				towStress.Link.WrappedLink?.Id, towStress.Link.WrappedLink?.Name ?? towStress.Link.Key,
				towStress.Reason, "reduce load or use stronger hitch gear/tow points"));
		}

		return new VehicleOperationalReadinessReport(vehicle, issues, modules, fuel, power, stress);
	}

	public VehicleTowCatastropheResult RollTowCatastrophe(VehicleHitchGraphMovePlan movePlan, ICharacter actor)
	{
		var stress = _graphService.EvaluateTowStress(movePlan)
		                          .Where(x => x.CanFail && x.FailureChance > 0.0)
		                          .OrderByDescending(x => x.StressRatio)
		                          .ToList();
		foreach (var candidate in stress)
		{
			if (!RandomUtilities.Roll(1.0, candidate.FailureChance))
			{
				continue;
			}

			var damagedItems = new List<IGameItem>();
			var damagedVehicles = new List<IVehicle>();
			var damageAmount = Math.Max(1.0, candidate.EffectiveWeight * 0.02);
			var damage = new Damage
			{
				DamageType = DamageType.Shearing,
				DamageAmount = damageAmount,
				ActorOrigin = actor
			};

			if (candidate.Link.HitchItem is not null)
			{
				candidate.Link.HitchItem.SufferDamage(damage);
				damagedItems.Add(candidate.Link.HitchItem);
			}
			else
			{
				foreach (var vehicle in new[] { candidate.Link.Source.Vehicle, candidate.Link.Target.Vehicle }
					         .Where(x => x is not null)
					         .Cast<IVehicle>()
					         .DistinctBy(x => x.Id))
				{
					vehicle.SufferDamage(damage);
					damagedVehicles.Add(vehicle);
				}
			}

			DisableLinkForCatastrophe(candidate.Link, out _);
			return new VehicleTowCatastropheResult(true, candidate,
				$"The hitch on {candidate.TargetVehicle?.Name ?? "the tow train"} catastrophically fails under strain.",
				damagedItems, damagedVehicles);
		}

		return new VehicleTowCatastropheResult(false, null, string.Empty, [], []);
	}

	public bool DisableLinkForCatastrophe(VehicleHitchGraphLink link, out string reason)
	{
		switch (link.Kind)
		{
			case VehicleHitchGraphLinkKind.LegacyVehicleTow when link.WrappedLink is IVehicleTowLink towLink:
				HitchGearRules.Release(link.HitchItem, vehicleTowLinkId: towLink.Id);
				towLink.SetDisabled(true);
				reason = string.Empty;
				return true;
			case VehicleHitchGraphLinkKind.PersistentHitch when link.WrappedLink is IVehicleHitchLink hitchLink:
				HitchGearRules.Release(link.HitchItem, vehicleHitchLinkId: hitchLink.Id);
				hitchLink.SetDisabled(true);
				ClearTransientEffects(link);
				reason = string.Empty;
				return true;
			case VehicleHitchGraphLinkKind.TransientCharacterHitch:
			case VehicleHitchGraphLinkKind.TransientDrag:
				ClearTransientEffects(link);
				reason = string.Empty;
				return true;
			default:
				reason = "That hitch link cannot be disabled.";
				return false;
		}
	}

	public bool RepairHitchLink(VehicleHitchGraphLink link, out string reason)
	{
		switch (link.WrappedLink)
		{
			case IVehicleTowLink towLink:
				towLink.SetDisabled(false);
				if (!RevalidateRepairedLink(towLink, out reason))
				{
					towLink.SetDisabled(true);
					return false;
				}

				reason = string.Empty;
				return true;
			case IVehicleHitchLink hitchLink:
				hitchLink.SetDisabled(false);
				if (!RevalidateRepairedLink(hitchLink, out reason))
				{
					hitchLink.SetDisabled(true);
					return false;
				}

				reason = string.Empty;
				return true;
			default:
				reason = "Only persistent hitch or tow links can be repaired.";
				return false;
		}
	}

	private VehicleHitchGraphMovePlan? BuildCurrentMovePlan(IVehicle vehicle)
	{
		return _graphService.TryBuildVehicleTrain(vehicle.Gameworld, vehicle, out var movePlan, out _)
			? movePlan
			: null;
	}

	private bool RevalidateRepairedLink(IFrameworkItem wrappedLink, out string reason)
	{
		var graphLink = RefreshedLinkFor(wrappedLink);
		if (graphLink is null)
		{
			reason = "The repaired link could not be found in the hitch graph.";
			return false;
		}

		if (!_graphService.ValidateLink(graphLink, out reason))
		{
			return false;
		}

		var endpointVehicles = new[] { graphLink.Source.Vehicle, graphLink.Target.Vehicle }
		                      .Where(x => x is not null)
		                      .Cast<IVehicle>()
		                      .DistinctBy(x => x.Id)
		                      .ToList();
		foreach (var vehicle in endpointVehicles)
		{
			if (vehicle.ExteriorItem is null)
			{
				continue;
			}

			var incoming = _graphService.LinksInvolving(vehicle.Gameworld, vehicle.ExteriorItem)
			                            .Where(x => SameVehicle(x.Target.Vehicle, vehicle))
			                            .Where(x => !x.IsManuallyDisabled)
			                            .ToList();
			if (incoming.Count > 1)
			{
				reason = $"{vehicle.Name} has more than one incoming hitch or tow link.";
				return false;
			}

			if (!_graphService.TryBuildVehicleTrain(vehicle.Gameworld, vehicle, out _, out reason,
				    allowRootIncoming: true))
			{
				return false;
			}
		}

		reason = string.Empty;
		return true;
	}

	private VehicleHitchGraphLink? RefreshedLinkFor(IFrameworkItem wrappedLink)
	{
		VehicleHitchGraphLinkKind kind;
		IPerceivable? perceivable;
		switch (wrappedLink)
		{
			case IVehicleTowLink towLink:
				kind = VehicleHitchGraphLinkKind.LegacyVehicleTow;
				perceivable = towLink.SourceVehicle?.ExteriorItem ?? towLink.TargetVehicle?.ExteriorItem;
				break;
			case IVehicleHitchLink hitchLink:
				kind = VehicleHitchGraphLinkKind.PersistentHitch;
				perceivable = hitchLink.SourceVehicle?.ExteriorItem ?? hitchLink.TargetVehicle?.ExteriorItem;
				perceivable ??= hitchLink.SourceCharacter;
				perceivable ??= hitchLink.TargetCharacter;
				break;
			default:
				return null;
		}

		return perceivable is null
			? null
			: _graphService.LinksInvolving(perceivable.Gameworld, perceivable)
			               .FirstOrDefault(x => x.Kind == kind && x.WrappedLink?.Id == wrappedLink.Id);
	}

	private static bool SameVehicle(IVehicle? lhs, IVehicle? rhs)
	{
		return lhs is not null && rhs is not null && lhs.Id == rhs.Id;
	}

	private static bool AccessMatches(IVehicleAccessState access, VehicleOperationalAction action, int requiredLevel)
	{
		if (access.AccessLevel >= 3)
		{
			return true;
		}

		if (access.AccessLevel < requiredLevel)
		{
			return false;
		}

		var tag = access.AccessTag?.Trim().ToLowerInvariant() ?? string.Empty;
		return string.IsNullOrWhiteSpace(tag) || tag.EqualTo("all") || tag.EqualTo(ActionTag(action));
	}

	private static int RequiredAccessLevel(VehicleOperationalAction action)
	{
		return action == VehicleOperationalAction.Board ? 1 : 2;
	}

	private static string ActionTag(VehicleOperationalAction action)
	{
		return action switch
		{
			VehicleOperationalAction.Board => "board",
			VehicleOperationalAction.Control => "control",
			VehicleOperationalAction.Service => "service",
			VehicleOperationalAction.Repair => "repair",
			VehicleOperationalAction.Hitch => "hitch",
			_ => "all"
		};
	}

	private static void ClearTransientEffects(VehicleHitchGraphLink link)
	{
		var source = link.Source.Character;
		var target = link.Target.Perceivable;
		if (source is null || target is null)
		{
			return;
		}

		source.RemoveAllEffects<Dragging>(x => x.Target == target, true);
		source.RemoveAllEffects<CharacterHitch>(x => x.Target == target, true);
	}
}
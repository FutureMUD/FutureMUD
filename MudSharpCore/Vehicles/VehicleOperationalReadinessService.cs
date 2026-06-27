#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
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

	public VehicleTowStressPolicy TowStressPolicy(IFuturemud? gameworld)
	{
		if (gameworld is null)
		{
			return VehicleTowStressPolicy.Default;
		}

		var warning = Math.Clamp(gameworld.GetStaticDouble("VehicleTowStressWarningRatio"), 0.0, 1.0);
		var failureStart = Math.Clamp(gameworld.GetStaticDouble("VehicleTowStressFailureStartRatio"), 0.0, 1.0);
		return new VehicleTowStressPolicy(
			warning,
			Math.Max(warning, failureStart),
			Math.Clamp(gameworld.GetStaticDouble("VehicleTowStressMaximumFailureChance"), 0.0, 1.0),
			Math.Max(0.0, gameworld.GetStaticDouble("VehicleTowStressDamageMultiplier")),
			"static configuration");
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
		var plan = BuildResourcePlan(vehicle, profile);
		candidates = plan.FuelCandidates;
		if (plan.HasFuel)
		{
			reason = string.Empty;
			return true;
		}

		reason = "That vehicle does not have enough configured fuel to move.";
		return false;
	}

	public bool HasPower(IVehicle vehicle, IVehicleMovementProfilePrototype profile,
		out IReadOnlyList<VehicleResourceCandidate> candidates, out string reason)
	{
		var plan = BuildResourcePlan(vehicle, profile);
		candidates = plan.PowerCandidates;
		if (plan.HasPower)
		{
			reason = string.Empty;
			return true;
		}

		reason = "That vehicle does not have enough available power to move.";
		return false;
	}

	public VehicleResourceReadinessPlan BuildResourcePlan(IVehicle vehicle, IVehicleMovementProfilePrototype profile)
	{
		var fuelCandidates = new List<VehicleResourceCandidate>();
		var powerCandidates = new List<VehicleResourceCandidate>();
		var uses = new List<VehicleResourceUse>();
		var reasons = new List<string>();

		var hasFuel = profile.FuelLiquidId is null || profile.FuelVolumePerMove <= 0.0;
		if (!hasFuel)
		{
			var fuelLiquidId = profile.FuelLiquidId!.Value;
			foreach (var installation in vehicle.Installations)
			{
				var item = installation.InstalledItem;
				if (!IsInstallationFunctionalForMovement(installation, out var moduleReason))
				{
					fuelCandidates.Add(new VehicleResourceCandidate(installation, item, "fuel", false, moduleReason));
					continue;
				}

				var containers = item!.GetItemTypes<ILiquidContainer>().ToList();
				if (!containers.Any())
				{
					fuelCandidates.Add(new VehicleResourceCandidate(installation, item, "fuel", false, "the module is not a fuel container"));
					continue;
				}

				var usableContainer = containers.FirstOrDefault(x => FuelVolume(x, fuelLiquidId) >= profile.FuelVolumePerMove);
				if (usableContainer is not null)
				{
					fuelCandidates.Add(new VehicleResourceCandidate(installation, item, "fuel", true, string.Empty));
					if (uses.All(x => x.ResourceType != VehicleResourceUseType.Fuel))
					{
						uses.Add(new VehicleResourceUse(VehicleResourceUseType.Fuel, installation, item, usableContainer,
							fuelLiquidId, profile.FuelVolumePerMove, null, 0.0, string.Empty));
					}
					continue;
				}

				var fuelReason = containers.All(x => FuelVolume(x, fuelLiquidId) <= 0.0)
					? "the module contains the wrong fuel"
					: "the module does not contain enough configured fuel";
				fuelCandidates.Add(new VehicleResourceCandidate(installation, item, "fuel", false, fuelReason));
			}

			hasFuel = uses.Any(x => x.ResourceType == VehicleResourceUseType.Fuel);
			if (!hasFuel)
			{
				reasons.Add("That vehicle does not have enough configured fuel to move.");
			}
		}

		var hasPower = profile.RequiredPowerSpikeInWatts <= 0.0;
		if (!hasPower)
		{
			foreach (var installation in vehicle.Installations)
			{
				var usable = TryGetUsablePowerProducer(installation, profile.RequiredPowerSpikeInWatts, out var item,
					out var producer, out var candidateReason);
				powerCandidates.Add(new VehicleResourceCandidate(installation, item, "power", usable, candidateReason));
				if (usable && uses.All(x => x.ResourceType != VehicleResourceUseType.Power))
				{
					uses.Add(new VehicleResourceUse(VehicleResourceUseType.Power, installation, item, null, null, 0.0,
						producer, profile.RequiredPowerSpikeInWatts, string.Empty));
				}
			}

			hasPower = uses.Any(x => x.ResourceType == VehicleResourceUseType.Power);
			if (!hasPower)
			{
				reasons.Add("That vehicle does not have enough available power to move.");
			}
		}

		return new VehicleResourceReadinessPlan(vehicle, profile, uses, fuelCandidates, powerCandidates, hasFuel, hasPower,
			reasons.ListToString(separator: " ", conjunction: "", twoItemJoiner: " "));
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
		var plan = BuildResourcePlan(vehicle, profile);
		if (plan.IsSatisfied)
		{
			ConsumeMovementResources(plan);
		}
	}

	public void ConsumeMovementResources(VehicleResourceReadinessPlan plan)
	{
		foreach (var use in plan.Uses)
		{
			switch (use.ResourceType)
			{
				case VehicleResourceUseType.Power:
					use.PowerProducer?.DrawdownSpike(use.PowerSpikeInWatts);
					break;
				case VehicleResourceUseType.Fuel when use.FuelContainer is not null && use.FuelLiquidId is not null:
					ConsumeFuel(use.FuelContainer, use.FuelLiquidId.Value, use.FuelVolume);
					break;
			}
		}
	}

	public VehicleMovementReadinessResult BuildMovementReadiness(VehicleMovementReadinessRequest request)
	{
		var issues = new List<VehicleOperationalIssue>();
		VehicleMovementReadinessResult Fail(string reason, VehicleHitchGraphMovePlan? plan = null,
			VehicleResourceReadinessPlan? resourcePlan = null)
		{
			issues.Add(new VehicleOperationalIssue(VehicleOperationalSubsystem.Movement,
				VehicleOperationalSeverity.Blocking, request.Vehicle?.Id, request.Vehicle?.Name ?? "vehicle", reason, string.Empty));
			return new VehicleMovementReadinessResult(false, reason, plan, resourcePlan, issues);
		}

		var vehicle = request.Vehicle;
		var actor = request.Actor;
		var exit = request.Exit;
		if (vehicle is null)
		{
			return Fail("There is no such vehicle.");
		}

		if (actor is null)
		{
			return Fail("There is no such driver.");
		}

		if (vehicle.Destroyed)
		{
			return Fail("That vehicle is destroyed and cannot move.");
		}

		if (vehicle.Disabled)
		{
			var damageReason = vehicle.DamageDisabledReason(VehicleDamageEffectTargetType.WholeVehicleMovement, null);
			return Fail(string.IsNullOrWhiteSpace(damageReason)
				? "That vehicle is disabled and cannot move."
				: $"That vehicle cannot move because {damageReason}.");
		}

		if (vehicle.Controller != actor)
		{
			return Fail("You must be in control of the vehicle to move it.");
		}

		if (!CanPerformAction(vehicle, actor, VehicleOperationalAction.Control, out var accessResult))
		{
			return Fail(accessResult.Reason);
		}

		if (actor.Location != vehicle.Location)
		{
			return Fail("You must be in the same location as the vehicle to move it.");
		}

		if (actor.RoomLayer != vehicle.RoomLayer)
		{
			return Fail("You must be on the same room layer as the vehicle to move it.");
		}

		var profile = request.MovementProfile ?? MovementProfile(vehicle);
		if (profile is null)
		{
			return Fail("That vehicle cannot move through normal cell exits.");
		}

		if (vehicle.IsDisabledByDamage(VehicleDamageEffectTargetType.MovementProfile, profile.Id))
		{
			return Fail($"That movement profile is disabled because {vehicle.DamageDisabledReason(VehicleDamageEffectTargetType.MovementProfile, profile.Id)}.");
		}

		VehicleHitchGraphMovePlan movePlan;
		string reason;
		if (exit is null)
		{
			if (!_graphService.TryBuildVehicleTrain(vehicle.Gameworld, vehicle, out movePlan, out reason))
			{
				return Fail(reason);
			}
		}
		else
		{
			if (vehicle.Location != exit.Origin)
			{
				return Fail("The vehicle is not at the origin of that exit.");
			}

			if (vehicle.ExteriorItem is not null && exit.Exit.MaximumSizeToEnter < vehicle.ExteriorItem.Size)
			{
				return Fail("That exit is too small for the vehicle.");
			}

			if (!_graphService.CanMoveVehicleTrain(vehicle.Gameworld, vehicle, exit, out movePlan, out reason))
			{
				return Fail(reason);
			}
		}

		foreach (var linkedVehicle in movePlan.Vehicles.DefaultIfEmpty(vehicle))
		{
			if (linkedVehicle.ExteriorItem?.PreventsMovement() != true)
			{
				continue;
			}

			return Fail(linkedVehicle.ExteriorItem.WhyPreventsMovement(actor), movePlan);
		}

		if (profile.RequiresAccessPointsClosed || vehicle.AccessPoints.Any(x => x.Prototype.MustBeClosedForMovement))
		{
			var openAccess = vehicle.AccessPoints.FirstOrDefault(x =>
				x.IsOpen && (profile.RequiresAccessPointsClosed || x.Prototype.MustBeClosedForMovement));
			if (openAccess is not null)
			{
				return Fail($"{openAccess.Name} must be closed before the vehicle can move.", movePlan);
			}
		}

		var missingRequiredInstallation = vehicle.Installations
		                                        .FirstOrDefault(x => x.Prototype.RequiredForMovement &&
		                                                             !IsInstallationFunctionalForMovement(x, out _));
		if (missingRequiredInstallation is not null)
		{
			IsInstallationFunctionalForMovement(missingRequiredInstallation, out var moduleReason);
			return Fail($"{missingRequiredInstallation.Prototype.Name} must have a functional module installed before the vehicle can move: {moduleReason}.", movePlan);
		}

		if (!HasFunctionalRole(vehicle, profile.RequiredInstalledRole, out reason))
		{
			return Fail(reason, movePlan);
		}

		var resourcePlan = BuildResourcePlan(vehicle, profile);
		if (!resourcePlan.HasPower)
		{
			return Fail(DescribeResourceFailure(actor, "That vehicle does not have enough available power to move.", resourcePlan.PowerCandidates), movePlan, resourcePlan);
		}

		if (!resourcePlan.HasFuel)
		{
			return Fail(DescribeResourceFailure(actor, "That vehicle does not have enough configured fuel to move.", resourcePlan.FuelCandidates), movePlan, resourcePlan);
		}

		if (profile.RequiresTowLinksClosed)
		{
			var invalidTowLink = movePlan.Links.FirstOrDefault(x => x.IsDisabled);
			if (invalidTowLink is not null)
			{
				return Fail("One of that vehicle's tow links is disabled.", movePlan, resourcePlan);
			}
		}

		if (exit is not null)
		{
			var transition = exit.MovementTransition(actor);
			if (transition.TransitionType == CellMovementTransition.NoViableTransition)
			{
				return Fail("That exit is not a viable transition from your current position.", movePlan, resourcePlan);
			}
		}

		return new VehicleMovementReadinessResult(true, string.Empty, movePlan, resourcePlan, issues);
	}

	public VehicleOperationalReadinessReport BuildReport(IVehicle vehicle, ICharacter voyeur,
		VehicleHitchGraphMovePlan? movePlan = null, IVehicleMovementProfilePrototype? movementProfile = null)
	{
		var modules = ModuleReadiness(vehicle);
		IReadOnlyList<VehicleResourceCandidate> fuel = [];
		IReadOnlyList<VehicleResourceCandidate> power = [];
		if (movementProfile is not null)
		{
			var resourcePlan = BuildResourcePlan(vehicle, movementProfile);
			fuel = resourcePlan.FuelCandidates;
			power = resourcePlan.PowerCandidates;
		}

		movePlan ??= BuildCurrentMovePlan(vehicle);
		IReadOnlyList<VehicleHitchGraphTowStress> stress = movePlan is null
			? []
			: _graphService.EvaluateTowStress(movePlan, TowStressPolicy(vehicle.Gameworld));
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
		var stress = _graphService.EvaluateTowStress(movePlan, TowStressPolicy(movePlan.RootVehicle.Gameworld))
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
			var damageAmount = Math.Max(1.0, candidate.EffectiveWeight * candidate.Policy.DamageMultiplier);
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

	private static IVehicleMovementProfilePrototype? MovementProfile(IVehicle vehicle)
	{
		return vehicle.Prototype.MovementProfiles
		              .Where(x => x.MovementType == VehicleMovementProfileType.CellExit)
		              .OrderByDescending(x => x.IsDefault)
		              .FirstOrDefault();
	}

	private static string DescribeResourceFailure(ICharacter actor, string reason,
		IReadOnlyList<VehicleResourceCandidate> candidates)
	{
		var failed = candidates.Where(x => !x.Available && !string.IsNullOrWhiteSpace(x.Reason)).ToList();
		return failed.Any()
			? $"{reason} {failed.Select(x => $"{(x.Item?.HowSeen(actor) ?? x.Installation.Prototype.Name)}: {x.Reason}").ListToString()}"
			: reason;
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
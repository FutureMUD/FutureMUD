#nullable enable

using ExpressionEngine;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction.Boundary;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Vehicles;

public class VehiclePropulsionService : IVehiclePropulsionService
{
	public VehiclePropulsionReadinessResult BuildReadiness(IVehicle vehicle, ICharacter actor, ICellExit? exit)
	{
		VehiclePropulsionReadinessResult Fail(string reason,
			IVehiclePropulsionProfilePrototype? profile = null,
			IReadOnlyList<VehiclePropulsionContributor>? contributors = null,
			IReadOnlyList<VehiclePropulsionMotorCandidate>? motors = null,
			WindLevel wind = WindLevel.None)
		{
			return new VehiclePropulsionReadinessResult(false, reason, vehicle, actor, exit, profile,
				contributors ?? [], motors ?? [], wind, false);
		}

		if (vehicle.MovementProfile?.MovementEnvironment != VehicleMovementEnvironment.SurfaceWater)
		{
			return new VehiclePropulsionReadinessResult(true, string.Empty, vehicle, actor, exit, null, [], [],
				WindLevel.None, true);
		}

		var supported = vehicle.MovementProfile.PropulsionProfiles?.ToList() ?? [];
		if (!supported.Any())
		{
			return new VehiclePropulsionReadinessResult(true, string.Empty, vehicle, actor, exit, null, [], [],
				WindLevel.None, true);
		}

		var profile = vehicle.ActivePropulsionProfile;
		if (profile is null)
		{
			return Fail("That vehicle has no selected propulsion mode.");
		}

		if (profile.PropulsionType == VehiclePropulsionType.None)
		{
			return Fail("That vehicle's selected propulsion mode does not permit it to initiate movement.", profile);
		}

		return profile.PropulsionType switch
		{
			VehiclePropulsionType.SelfPowered => SelfPoweredReadiness(vehicle, actor, exit, profile),
			VehiclePropulsionType.Rowed => RowedReadiness(vehicle, actor, exit, profile),
			VehiclePropulsionType.Sail => SailReadiness(vehicle, actor, exit, profile),
			VehiclePropulsionType.OutboardMotor => MotorReadiness(vehicle, actor, exit, profile),
			_ => Fail("That vehicle's selected propulsion mode is invalid.", profile)
		};
	}

	private static VehiclePropulsionReadinessResult SelfPoweredReadiness(IVehicle vehicle, ICharacter actor,
		ICellExit? exit, IVehiclePropulsionProfilePrototype profile)
	{
		if (profile.PropulsionTrait is null)
		{
			return Failed(vehicle, actor, exit, profile, "The selected self-powered mode has no configured propulsion trait.");
		}

		if (!actor.State.IsAble())
		{
			return Failed(vehicle, actor, exit, profile, "You are not currently able to propel that vehicle.");
		}

		var maximumCost = MaximumStaminaCost(profile, actor);
		if (!double.IsFinite(maximumCost) || maximumCost < 0.0)
		{
			return Failed(vehicle, actor, exit, profile, "The selected self-powered mode has an invalid stamina-cost expression.");
		}
		if (!actor.CanSpendStamina(maximumCost))
		{
			return Failed(vehicle, actor, exit, profile,
				$"You are too exhausted to propel that vehicle ({maximumCost.ToString("N2", actor).ColourValue()} stamina required)." );
		}

		var contributor = new VehiclePropulsionContributor(actor, null, null, maximumCost);
		return Ready(vehicle, actor, exit, profile, [contributor], [], WindLevel.None);
	}

	private static VehiclePropulsionReadinessResult RowedReadiness(IVehicle vehicle, ICharacter actor,
		ICellExit? exit, IVehiclePropulsionProfilePrototype profile)
	{
		if (profile.PropulsionTrait is null)
		{
			return Failed(vehicle, actor, exit, profile, "The selected rowed mode has no configured propulsion trait.");
		}

		var contributors = new List<VehiclePropulsionContributor>();
		foreach (var occupancy in vehicle.Occupancies.Where(x => x.Slot.ContributesToPropulsion))
		{
			var rower = occupancy.Occupant;
			if (rower is null || !rower.State.IsAble() || rower.Combat is not null)
			{
				continue;
			}

			var bestOar = rower.Body.HeldOrWieldedItems
				.Where(x => !x.Deleted && !x.Destroyed && x.Condition > 0.0)
				.Select(x => (Item: x, Oar: x.GetItemType<IVehicleOar>()))
				.Where(x => x.Oar is not null && x.Oar.EfficiencyMultiplier > 0.0)
				.OrderByDescending(x => x.Oar!.EfficiencyMultiplier * Math.Clamp(x.Item.Condition, 0.0, 1.0))
				.FirstOrDefault();
			if (bestOar.Oar is null)
			{
				continue;
			}

			var maximumCost = MaximumStaminaCost(profile, rower);
			if (!double.IsFinite(maximumCost) || maximumCost < 0.0 || !rower.CanSpendStamina(maximumCost))
			{
				continue;
			}

			contributors.Add(new VehiclePropulsionContributor(rower, bestOar.Item, bestOar.Oar, maximumCost));
		}

		if (!contributors.Any())
		{
			return Failed(vehicle, actor, exit, profile,
				"At least one able occupant in a propulsion slot must be holding or wielding a usable oar and have enough stamina to row.");
		}

		return Ready(vehicle, actor, exit, profile, contributors, [], WindLevel.None);
	}

	private static VehiclePropulsionReadinessResult SailReadiness(IVehicle vehicle, ICharacter actor,
		ICellExit? exit, IVehiclePropulsionProfilePrototype profile)
	{
		var wind = (exit?.Origin ?? vehicle.Location)?.CurrentWeather(null)?.Wind ?? WindLevel.None;
		if (wind <= WindLevel.Still)
		{
			return Failed(vehicle, actor, exit, profile, "There is not enough wind to move under sail.", wind: wind);
		}

		return Ready(vehicle, actor, exit, profile, [], [], wind);
	}

	private static VehiclePropulsionReadinessResult MotorReadiness(IVehicle vehicle, ICharacter actor,
		ICellExit? exit, IVehiclePropulsionProfilePrototype profile)
	{
		var motors = vehicle.Installations
			.Select(BuildMotorCandidate)
			.Where(x => x.Motor is not null)
			.ToList();
		if (motors.All(x => !x.Available))
		{
			var exclusions = motors
				.Where(x => !string.IsNullOrWhiteSpace(x.Reason))
				.Select(x => $"{x.Item?.HowSeen(actor) ?? x.Installation.Prototype.Name}: {x.Reason}")
				.ToList();
			var reason = "That vehicle has no installed, functional, energy-ready outboard motor.";
			if (exclusions.Any())
			{
				reason = $"{reason} {exclusions.ListToString()}";
			}

			return Failed(vehicle, actor, exit, profile, reason, motors: motors);
		}

		return Ready(vehicle, actor, exit, profile, [], motors, WindLevel.None);
	}

	private static VehiclePropulsionMotorCandidate BuildMotorCandidate(IVehicleInstallation installation)
	{
		var item = installation.InstalledItem;
		var motor = item?.GetItemType<IOutboardMotor>();
		VehiclePropulsionMotorCandidate Candidate(bool available, string reason,
			ILiquidContainer? container = null, IProducePower? producer = null)
		{
			return new VehiclePropulsionMotorCandidate(installation, item, motor, container, producer, available, reason);
		}

		if (motor is null)
		{
			return Candidate(false, "the installed item is not an outboard motor");
		}

		if (installation.IsDisabled)
		{
			return Candidate(false, "the installation point is disabled");
		}

		if (item is null || item.Deleted || item.Destroyed)
		{
			return Candidate(false, "the motor item is destroyed");
		}

		var installable = item.GetItemType<IVehicleInstallable>();
		if (installable is null)
		{
			return Candidate(false, "the motor is missing its vehicle-installable component");
		}

		if (!installable.IsFunctionalForMovement(out var functionalReason))
		{
			return Candidate(false, functionalReason);
		}

		if (item.GetItemType<IOnOff>()?.SwitchedOn == false)
		{
			return Candidate(false, "the motor is switched off");
		}

		if (!double.IsFinite(motor.OutputMultiplier) || motor.OutputMultiplier <= 0.0)
		{
			return Candidate(false, "the motor has invalid output");
		}

		if (motor.EnergySource == OutboardMotorEnergySource.Fuelled)
		{
			if (motor.FuelLiquidId is null || motor.FuelVolumePerMove <= 0.0)
			{
				return Candidate(false, "the motor's fuel requirement is not configured");
			}

			var container = item.GetItemTypes<ILiquidContainer>()
				.FirstOrDefault(x => FuelVolume(x, motor.FuelLiquidId.Value) >= motor.FuelVolumePerMove);
			return container is null
				? Candidate(false, "the motor does not contain enough of its configured fuel")
				: Candidate(true, string.Empty, container);
		}

		if (motor.RequiredPowerSpikeInWatts <= 0.0)
		{
			return Candidate(false, "the motor's electrical power spike is not configured");
		}

		var producer = item.GetItemType<IProducePower>();
		if (producer is null)
		{
			return Candidate(false, "the motor item does not produce power");
		}

		if (!producer.ProducingPower)
		{
			return Candidate(false, "the motor item is not producing power", producer: producer);
		}

		return producer.CanDrawdownSpike(motor.RequiredPowerSpikeInWatts)
			? Candidate(true, string.Empty, producer: producer)
			: Candidate(false, "the motor item cannot supply its configured power spike", producer: producer);
	}

	public bool TryCommitDeparture(VehiclePropulsionReadinessResult readiness, out VehiclePropulsionMovePlan? plan,
		out string reason)
	{
		plan = null;
		if (readiness.UsesLegacyMovement)
		{
			reason = string.Empty;
			return true;
		}

		var refreshed = BuildReadiness(readiness.Vehicle, readiness.Actor, readiness.Exit);
		if (!refreshed.CanMove || refreshed.Profile is null || refreshed.Exit is null)
		{
			reason = refreshed.Reason.IfNullOrWhiteSpace("The propulsion departure plan is no longer ready.");
			return false;
		}

		var contributorResults = new List<VehiclePropulsionContributorResult>();
		double multiplier;
		switch (refreshed.Profile.PropulsionType)
		{
			case VehiclePropulsionType.SelfPowered:
				var self = RollContributor(refreshed.Profile, refreshed.Contributors.Single(), CheckType.PaddleVehicleCheck);
				contributorResults.Add(self);
				multiplier = self.SpeedContribution;
				break;
			case VehiclePropulsionType.Rowed:
				foreach (var contributor in refreshed.Contributors)
				{
					contributorResults.Add(RollContributor(refreshed.Profile, contributor, CheckType.RowVehicleCheck));
				}
				multiplier = Math.Sqrt(contributorResults.Sum(x => x.SpeedContribution));
				break;
			case VehiclePropulsionType.Sail:
				multiplier = Evaluate(refreshed.Profile.SpeedMultiplierExpression,
					wind: Math.Clamp((int)refreshed.Wind - 1, 1, 7));
				break;
			case VehiclePropulsionType.OutboardMotor:
				var output = refreshed.Motors
					.Where(x => x.Available)
					.Sum(x => x.Motor!.OutputMultiplier);
				multiplier = Evaluate(refreshed.Profile.SpeedMultiplierExpression, output: output);
				break;
			default:
				reason = "The selected propulsion mode cannot initiate movement.";
				return false;
		}

		if (!double.IsFinite(multiplier) || multiplier <= 0.0)
		{
			reason = "The selected propulsion mode produced a non-positive effective speed.";
			return false;
		}

		if (contributorResults.Any(x => !x.Contributor.Character.CanSpendStamina(x.StaminaCost)))
		{
			reason = "A propulsion contributor no longer has enough stamina to depart.";
			return false;
		}

		var readyMotors = refreshed.Motors.Where(x => x.Available).ToList();
		foreach (var result in contributorResults)
		{
			result.Contributor.Character.SpendStamina(result.StaminaCost);
		}

		if (refreshed.Profile.PropulsionType == VehiclePropulsionType.OutboardMotor)
		{
			var committedMotors = new List<VehiclePropulsionMotorCandidate>();
			var committedOutput = 0.0;
			foreach (var motor in readyMotors)
			{
				var prospectiveOutput = committedOutput + motor.Motor!.OutputMultiplier;
				var prospectiveMultiplier = Evaluate(refreshed.Profile.SpeedMultiplierExpression,
					output: prospectiveOutput);
				if (!double.IsFinite(prospectiveMultiplier) || prospectiveMultiplier <= 0.0 ||
				    !ConsumeMotorEnergy(motor))
				{
					continue;
				}

				committedMotors.Add(motor);
				committedOutput = prospectiveOutput;
			}

			if (!committedMotors.Any())
			{
				reason = "No outboard motor could supply its departure energy.";
				return false;
			}

			readyMotors = committedMotors;
			multiplier = Evaluate(refreshed.Profile.SpeedMultiplierExpression, output: committedOutput);
		}

		var milliseconds = refreshed.Profile.BaseMoveTimeMilliseconds *
		                   Math.Max(refreshed.Exit.Exit.TimeMultiplier, 0.01) / multiplier;
		var maximum = refreshed.Actor.Gameworld.GetStaticDouble("MaximumMoveTimeMilliseconds");
		if (maximum > 0.0)
		{
			milliseconds = Math.Min(milliseconds, maximum);
		}

		plan = new VehiclePropulsionMovePlan(refreshed.Vehicle, refreshed.Actor, refreshed.Exit, refreshed.Profile,
			contributorResults, readyMotors, refreshed.Wind, multiplier,
			TimeSpan.FromMilliseconds(Math.Max(0.0, milliseconds)));
		reason = string.Empty;
		return true;
	}

	public bool ValidateCommittedPlan(VehiclePropulsionMovePlan plan, out string reason)
	{
		if (plan.Vehicle.ActivePropulsionProfile?.Id != plan.Profile.Id)
		{
			reason = "The vehicle's active propulsion mode changed during movement.";
			return false;
		}

		if (plan.Vehicle.Controller?.SamePhysicalInstance(plan.Actor) != true)
		{
			reason = "The vehicle's controller changed during movement.";
			return false;
		}

		foreach (var result in plan.Contributors)
		{
			var contributor = result.Contributor;
			if (!plan.Vehicle.IsOccupant(contributor.Character) || !contributor.Character.State.IsAble() ||
			    contributor.Character.Combat is not null)
			{
				reason = $"{contributor.Character.HowSeen(plan.Actor)} can no longer contribute to propulsion.";
				return false;
			}

			if (contributor.OarItem is not null &&
			    (!contributor.Character.Body.HeldOrWieldedItems.Contains(contributor.OarItem) ||
			     contributor.OarItem.Deleted || contributor.OarItem.Destroyed || contributor.OarItem.Condition <= 0.0))
			{
				reason = $"{contributor.Character.HowSeen(plan.Actor)} no longer has the selected oar ready.";
				return false;
			}
		}

		foreach (var motor in plan.Motors)
		{
			var item = motor.Installation.InstalledItem;
			var installable = item?.GetItemType<IVehicleInstallable>();
			if (motor.Installation.IsDisabled || item is null || item.Id != motor.Item?.Id || item.Deleted ||
			    item.Destroyed || installable is null || !installable.IsFunctionalForMovement(out _) ||
			    item.GetItemType<IOnOff>()?.SwitchedOn == false)
			{
				reason = $"{motor.Item?.HowSeen(plan.Actor) ?? "An outboard motor"} is no longer operational.";
				return false;
			}
		}

		reason = string.Empty;
		return true;
	}

	private static VehiclePropulsionContributorResult RollContributor(
		IVehiclePropulsionProfilePrototype profile, VehiclePropulsionContributor contributor, CheckType checkType)
	{
		var check = contributor.Character.Gameworld.GetCheck(checkType);
		if (!UsesSelectedTrait(check, checkType))
		{
			check = contributor.Character.Gameworld.GetCheck(CheckType.GenericSkillCheck);
		}

		var outcome = check.Check(contributor.Character, profile.CheckDifficulty, profile.PropulsionTrait);
		var degrees = Math.Clamp(outcome.CheckDegrees(), -3, 3);
		var outcomeMultiplier = Evaluate(profile.SpeedMultiplierExpression, outcome: degrees);
		var effectiveness = contributor.Oar is null || contributor.OarItem is null
			? 1.0
			: contributor.Oar.EfficiencyMultiplier * Math.Clamp(contributor.OarItem.Condition, 0.0, 1.0);
		var speedContribution = effectiveness * outcomeMultiplier;
		var stamina = Evaluate(profile.StaminaCostExpression, outcome: degrees,
			swimcost: contributor.Character.SwimStaminaCost());
		return new VehiclePropulsionContributorResult(contributor, outcome, speedContribution, stamina);
	}

	private static bool UsesSelectedTrait(ICheck check, CheckType expectedType)
	{
		return check.Type == expectedType &&
		       check.TargetNumberExpression?.NonTraitParameters.Any(x => x.EqualTo("variable")) == true;
	}

	private static double MaximumStaminaCost(IVehiclePropulsionProfilePrototype profile, ICharacter character)
	{
		return Enumerable.Range(-3, 7)
			.Select(x => Evaluate(profile.StaminaCostExpression, outcome: x, swimcost: character.SwimStaminaCost()))
			.Max();
	}

	private static double Evaluate(string text, double outcome = 0.0, double wind = 1.0,
		double output = 1.0, double swimcost = 1.0)
	{
		return new Expression(text).EvaluateDoubleWith(
			("outcome", outcome),
			("wind", wind),
			("output", output),
			("swimcost", swimcost));
	}

	private static VehiclePropulsionReadinessResult Ready(IVehicle vehicle, ICharacter actor, ICellExit? exit,
		IVehiclePropulsionProfilePrototype profile, IReadOnlyList<VehiclePropulsionContributor> contributors,
		IReadOnlyList<VehiclePropulsionMotorCandidate> motors, WindLevel wind)
	{
		return new VehiclePropulsionReadinessResult(true, string.Empty, vehicle, actor, exit, profile, contributors,
			motors, wind, false);
	}

	private static VehiclePropulsionReadinessResult Failed(IVehicle vehicle, ICharacter actor, ICellExit? exit,
		IVehiclePropulsionProfilePrototype profile, string reason,
		IReadOnlyList<VehiclePropulsionContributor>? contributors = null,
		IReadOnlyList<VehiclePropulsionMotorCandidate>? motors = null,
		WindLevel wind = WindLevel.None)
	{
		return new VehiclePropulsionReadinessResult(false, reason, vehicle, actor, exit, profile,
			contributors ?? [], motors ?? [], wind, false);
	}

	private static double FuelVolume(ILiquidContainer container, long liquidId)
	{
		return container.LiquidMixture?.Instances
			.Where(x => x.Liquid.Id == liquidId)
			.Sum(x => x.Amount) ?? 0.0;
	}

	private static bool ConsumeMotorEnergy(VehiclePropulsionMotorCandidate candidate)
	{
		var motor = candidate.Motor!;
		if (motor.EnergySource == OutboardMotorEnergySource.Electric)
		{
			return candidate.PowerProducer?.DrawdownSpike(motor.RequiredPowerSpikeInWatts) == true;
		}

		if (candidate.FuelContainer?.LiquidMixture is not { } mixture || motor.FuelLiquidId is null)
		{
			return false;
		}

		var remaining = motor.FuelVolumePerMove;
		foreach (var instance in mixture.Instances
		                                .Where(x => x.Liquid.Id == motor.FuelLiquidId.Value)
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

		candidate.FuelContainer.LiquidMixture = mixture;
		return remaining <= 0.0;
	}
}

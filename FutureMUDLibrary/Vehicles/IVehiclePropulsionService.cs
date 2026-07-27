#nullable enable

using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction.Boundary;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using System.Collections.Generic;

namespace MudSharp.Vehicles;

public sealed record VehiclePropulsionContributor(
	ICharacter Character,
	IGameItem? OarItem,
	IVehicleOar? Oar,
	double MaximumStaminaCost);

public sealed record VehiclePropulsionMotorCandidate(
	IVehicleInstallation Installation,
	IGameItem? Item,
	IOutboardMotor? Motor,
	ILiquidContainer? FuelContainer,
	IProducePower? PowerProducer,
	bool Available,
	string Reason);

public sealed record VehicleEngineCandidate(
	IVehicleInstallation Installation,
	IGameItem? Item,
	IVehicleEngine? Engine,
	bool Available,
	string Reason);

public sealed record VehicleEngineReadinessResult(
	bool CanMove,
	string Reason,
	IReadOnlyList<VehicleEngineCandidate> Engines,
	double AvailablePowerInWatts,
	double RequiredPowerInWatts);

public sealed record VehiclePropulsionReadinessResult(
	bool CanMove,
	string Reason,
	IVehicle Vehicle,
	ICharacter Actor,
	ICellExit? Exit,
	IVehiclePropulsionProfilePrototype? Profile,
	IReadOnlyList<VehiclePropulsionContributor> Contributors,
	IReadOnlyList<VehiclePropulsionMotorCandidate> Motors,
	WindLevel Wind,
	bool UsesLegacyMovement,
	IReadOnlyList<VehicleEngineCandidate>? Engines = null,
	double RiderStaminaCost = 0.0,
	double RiderStaminaMultiplier = 1.0);

public sealed record VehiclePropulsionContributorResult(
	VehiclePropulsionContributor Contributor,
	CheckOutcome Outcome,
	double SpeedContribution,
	double StaminaCost);

public sealed record VehiclePropulsionMovePlan(
	IVehicle Vehicle,
	ICharacter Actor,
	ICellExit Exit,
	IVehiclePropulsionProfilePrototype Profile,
	IReadOnlyList<VehiclePropulsionContributorResult> Contributors,
	IReadOnlyList<VehiclePropulsionMotorCandidate> Motors,
	WindLevel Wind,
	double EffectiveMultiplier,
	System.TimeSpan Duration,
	IReadOnlyList<VehicleEngineCandidate>? Engines = null,
	double RiderStaminaCost = 0.0,
	double RiderStaminaMultiplier = 1.0);

public interface IVehiclePropulsionService
{
	VehicleEngineReadinessResult BuildEngineReadiness(IVehicle vehicle, double requiredPowerInWatts);
	VehiclePropulsionReadinessResult BuildReadiness(IVehicle vehicle, ICharacter actor, ICellExit? exit);
	bool TryCommitDeparture(VehiclePropulsionReadinessResult readiness, out VehiclePropulsionMovePlan? plan,
		out string reason);
	bool ValidateCommittedPlan(VehiclePropulsionMovePlan plan, out string reason);
}

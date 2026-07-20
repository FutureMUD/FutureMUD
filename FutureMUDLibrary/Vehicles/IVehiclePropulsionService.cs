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
	bool UsesLegacyMovement);

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
	System.TimeSpan Duration);

public interface IVehiclePropulsionService
{
	VehiclePropulsionReadinessResult BuildReadiness(IVehicle vehicle, ICharacter actor, ICellExit? exit);
	bool TryCommitDeparture(VehiclePropulsionReadinessResult readiness, out VehiclePropulsionMovePlan? plan,
		out string reason);
	bool ValidateCommittedPlan(VehiclePropulsionMovePlan plan, out string reason);
}

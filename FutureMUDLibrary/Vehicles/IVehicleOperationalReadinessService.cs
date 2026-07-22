#nullable enable

using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Movement;
using System;
using System.Collections.Generic;

namespace MudSharp.Vehicles;

public enum VehicleOperationalSubsystem
{
	Access,
	Control,
	Module,
	Fuel,
	Power,
	Damage,
	Repair,
	HitchTow,
	Movement,
	Propulsion,
	Interior,
	Docking,
	Route,
	Journey
}

public enum VehicleOperationalSeverity
{
	Information,
	Warning,
	Blocking,
	Catastrophic
}

public enum VehicleOperationalAction
{
	Board,
	Control,
	Service,
	Repair,
	Hitch
}

public sealed record VehicleOperationalIssue(
	VehicleOperationalSubsystem Subsystem,
	VehicleOperationalSeverity Severity,
	long? TargetId,
	string TargetName,
	string Reason,
	string RepairHint);

public sealed record VehicleAccessPermissionResult(
	bool Allowed,
	VehicleOperationalAction Action,
	int RequiredLevel,
	IVehicleAccessState? MatchingAccess,
	string Reason);

public sealed record VehicleModuleReadiness(
	IVehicleInstallation Installation,
	IGameItem? Item,
	IVehicleInstallable? Installable,
	bool IsFunctional,
	bool IsFunctionalForMovement,
	string Reason);

public sealed record VehicleResourceCandidate(
	IVehicleInstallation Installation,
	IGameItem? Item,
	string ResourceType,
	bool Available,
	string Reason);

public enum VehicleResourceUseType
{
	Fuel,
	Power
}

public sealed record VehicleResourceUse(
	VehicleResourceUseType ResourceType,
	IVehicleInstallation Installation,
	IGameItem? Item,
	ILiquidContainer? FuelContainer,
	long? FuelLiquidId,
	double FuelVolume,
	IProducePower? PowerProducer,
	double PowerSpikeInWatts,
	string Reason);

public sealed record VehicleResourceReadinessPlan(
	IVehicle Vehicle,
	IVehicleMovementProfilePrototype Profile,
	IReadOnlyList<VehicleResourceUse> Uses,
	IReadOnlyList<VehicleResourceCandidate> FuelCandidates,
	IReadOnlyList<VehicleResourceCandidate> PowerCandidates,
	bool HasFuel,
	bool HasPower,
	string Reason)
{
	public bool IsSatisfied => HasFuel && HasPower;
}

public sealed record VehicleMovementReadinessRequest(
	IVehicle Vehicle,
	ICharacter? Actor,
	ICellExit? Exit,
	IVehicleMovementProfilePrototype? MovementProfile = null,
	VehiclePropulsionMovePlan? CommittedPropulsionPlan = null,
	IReadOnlyCollection<ICharacter>? ExternalPullers = null,
	bool AutomaticOperation = false);

public sealed record VehicleLongitudinalReadinessRequest(
	IVehicle Vehicle,
	ICharacter? Actor,
	IVehicleMovementProfilePrototype MovementProfile,
	double DistanceMetres,
	TimeSpan Duration,
	bool AutomaticOperation = false,
	IMovement? ContinuingMovement = null);

public sealed record VehicleMovementReadinessResult(
	bool CanMove,
	string Reason,
	VehicleHitchGraphMovePlan? MovePlan,
	VehicleResourceReadinessPlan? ResourcePlan,
	IReadOnlyList<VehicleOperationalIssue> Issues,
	VehiclePropulsionReadinessResult? PropulsionReadiness = null);

public sealed record VehicleTowCatastropheResult(
	bool Catastrophe,
	VehicleHitchGraphTowStress? Stress,
	string Reason,
	IReadOnlyList<IGameItem> DamagedItems,
	IReadOnlyList<IVehicle> DamagedVehicles);

public sealed record VehicleOperationalReadinessReport(
	IVehicle Vehicle,
	IReadOnlyList<VehicleOperationalIssue> Issues,
	IReadOnlyList<VehicleModuleReadiness> Modules,
	IReadOnlyList<VehicleResourceCandidate> FuelCandidates,
	IReadOnlyList<VehicleResourceCandidate> PowerCandidates,
	IReadOnlyList<VehicleHitchGraphTowStress> TowStress);

public interface IVehicleOperationalReadinessService
{
	bool CanPerformAction(IVehicle vehicle, ICharacter actor, VehicleOperationalAction action,
		out VehicleAccessPermissionResult result);
	IReadOnlyList<VehicleModuleReadiness> ModuleReadiness(IVehicle vehicle);
	bool IsInstallationFunctional(IVehicleInstallation installation, out string reason);
	bool IsInstallationFunctionalForMovement(IVehicleInstallation installation, out string reason);
	bool HasFunctionalRole(IVehicle vehicle, string role, out string reason);
	bool HasFuel(IVehicle vehicle, IVehicleMovementProfilePrototype profile,
		out IReadOnlyList<VehicleResourceCandidate> candidates, out string reason);
	bool HasPower(IVehicle vehicle, IVehicleMovementProfilePrototype profile,
		out IReadOnlyList<VehicleResourceCandidate> candidates, out string reason);
	VehicleResourceReadinessPlan BuildResourcePlan(IVehicle vehicle, IVehicleMovementProfilePrototype profile);
	VehicleResourceReadinessPlan BuildLongitudinalResourcePlan(IVehicle vehicle,
		IVehicleMovementProfilePrototype profile, double distanceMetres, TimeSpan duration);
	void ConsumeMovementResources(IVehicle vehicle, IVehicleMovementProfilePrototype profile);
	void ConsumeMovementResources(VehicleResourceReadinessPlan plan);
	bool TryCommitPropulsion(VehiclePropulsionReadinessResult readiness, out VehiclePropulsionMovePlan? plan,
		out string reason);
	VehicleMovementReadinessResult BuildMovementReadiness(VehicleMovementReadinessRequest request);
	VehicleMovementReadinessResult BuildLongitudinalMovementReadiness(
		VehicleLongitudinalReadinessRequest request);
	VehicleTowStressPolicy TowStressPolicy(IFuturemud? gameworld);
	VehicleOperationalReadinessReport BuildReport(IVehicle vehicle, ICharacter voyeur,
		VehicleHitchGraphMovePlan? movePlan = null, IVehicleMovementProfilePrototype? movementProfile = null);
	VehicleTowCatastropheResult RollTowCatastrophe(VehicleHitchGraphMovePlan movePlan, ICharacter? actor);
	bool DisableLinkForCatastrophe(VehicleHitchGraphLink link, out string reason);
	bool RepairHitchLink(VehicleHitchGraphLink link, out string reason);
}

#nullable enable

using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
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
	Movement
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
	void ConsumeMovementResources(IVehicle vehicle, IVehicleMovementProfilePrototype profile);
	VehicleOperationalReadinessReport BuildReport(IVehicle vehicle, ICharacter voyeur,
		VehicleHitchGraphMovePlan? movePlan = null, IVehicleMovementProfilePrototype? movementProfile = null);
	VehicleTowCatastropheResult RollTowCatastrophe(VehicleHitchGraphMovePlan movePlan, ICharacter actor);
	bool DisableLinkForCatastrophe(VehicleHitchGraphLink link, out string reason);
	bool RepairHitchLink(VehicleHitchGraphLink link, out string reason);
}
#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Vehicles;

public enum VehicleFleetAuditMode
{
	All,
	Readiness,
	Access,
	Resources,
	Hitch,
	Damage,
	Recovery,
	Interior,
	Docking,
	Route,
	Journey
}

public enum VehicleRecoveryMode
{
	All,
	Projection,
	Access,
	Cargo,
	Install,
	Tow,
	Hitch,
	Damage,
	Interior,
	Docking
}

public enum VehicleRecoveryAction
{
	None,
	Relinked,
	Disabled,
	Repaired,
	Warning
}

public sealed record VehicleAccessPresetGrant(
	string AccessTag,
	int AccessLevel);

public sealed record VehicleAccessPreset(
	string Name,
	IReadOnlyList<VehicleAccessPresetGrant> Grants);

public sealed record VehicleAccessPresetApplicationResult(
	IVehicle Vehicle,
	ICharacter Character,
	VehicleAccessPreset Preset,
	IReadOnlyList<IVehicleAccessState> Grants,
	string Reason);

public sealed record VehicleFleetAuditFinding(
	IVehicle Vehicle,
	VehicleOperationalSubsystem Subsystem,
	VehicleOperationalSeverity Severity,
	string Reason,
	string Hint);

public sealed record VehicleFleetAuditResult(
	IReadOnlyList<IVehicle> Vehicles,
	IReadOnlyList<VehicleFleetAuditFinding> Findings)
{
	public bool HasBlockingFindings => Findings.Any(x => x.Severity >= VehicleOperationalSeverity.Blocking);
}

public sealed record VehicleRecoveryFinding(
	IVehicle Vehicle,
	VehicleOperationalSubsystem Subsystem,
	VehicleOperationalSeverity Severity,
	string Reason,
	string Hint,
	VehicleRecoveryAction Action);

public sealed record VehicleRecoveryResult(
	IVehicle Vehicle,
	IReadOnlyList<VehicleRecoveryFinding> Findings,
	bool Applied);

public interface IVehicleFleetOperationsService
{
	IReadOnlyList<VehicleAccessPreset> AccessPresets(IFuturemud gameworld);
	bool TryGetAccessPreset(IFuturemud gameworld, string name, out VehicleAccessPreset? preset, out string reason);
	VehicleAccessPreset SaveAccessPreset(IFuturemud gameworld, string name, IEnumerable<VehicleAccessPresetGrant> grants);
	bool DeleteAccessPreset(IFuturemud gameworld, string name, out string reason);
	VehicleAccessPreset ResetAccessPreset(IFuturemud gameworld, string name);
	IReadOnlyList<VehicleAccessPreset> ResetAllAccessPresets(IFuturemud gameworld);
	VehicleAccessPresetApplicationResult ApplyAccessPreset(IVehicle vehicle, ICharacter character, VehicleAccessPreset preset);
	int CloneAccess(IVehicle targetVehicle, IVehicle sourceVehicle);
	VehicleFleetAuditResult Audit(IEnumerable<IVehicle> vehicles, ICharacter voyeur, VehicleFleetAuditMode mode);
	VehicleRecoveryResult Recover(IVehicle vehicle, VehicleRecoveryMode mode, bool apply);
}

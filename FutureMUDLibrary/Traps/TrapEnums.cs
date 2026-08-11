#nullable enable

namespace MudSharp.Traps;

/// <summary>
/// The fiction and deployment discipline for a trap template. A trap has exactly one source kind in v1;
/// anchoring a magical or natural trap to an item does not make it a mechanical trap.
/// </summary>
public enum TrapSourceKind
{
	Mechanical,
	Magical,
	Natural
}

/// <summary>
/// The durable lifecycle state of a deployed trap.
/// </summary>
public enum TrapState
{
	Unarmed,
	Armed,
	Resolving,
	CoolingDown,
	Spent,
	Disarmed,
	Expired
}

/// <summary>
/// The supported first-party trigger families. Templates use OR semantics when more than one trigger is present.
/// </summary>
public enum TrapTriggerType
{
	ExitTraversal,
	Openable,
	Proximity,
	CellEntry,
	Signal,
	Manual
}

/// <summary>
/// The supported first-party payload families.
/// </summary>
public enum TrapPayloadType
{
	DetonateItem,
	CastSpell,
	EmitSignal,
	ExecuteProg,
	DirectDamage,
	LiquidDischarge,
	GasCloud,
	Restraint
}

public enum TrapDisarmPolicy
{
	Impossible,
	Safe,
	Risky,
	Dispellable
}

public enum TrapLifecyclePolicy
{
	Indefinite,
	FixedExpiry,
	Unstable
}

public enum TrapTargetSelector
{
	Triggerer,
	AnchorOccupants,
	SnapshotTarget
}

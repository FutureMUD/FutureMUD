#nullable enable

using System;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Framework;

namespace MudSharp.Magic;

public enum SpellProjectionKind { SandEffigy, WalkingShadow }

/// <summary>Authored native policy; seconds and room-edge distance are not legacy affect units.</summary>
public sealed record SpellProjectionConfiguration(SpellProjectionKind Kind, long PlaneId, long EffigyPrototypeId,
	double SecondsPerGrade, int MaximumRoomDistance, double BacklashDamage, bool CrossClosedDoors = false)
{
	public string? Error(int grade) => !Enum.IsDefined(Kind) || grade is < 1 or > 7 || PlaneId <= 0 ||
		!double.IsFinite(SecondsPerGrade) || SecondsPerGrade <= 0 || SecondsPerGrade * grade > 86400 ||
		MaximumRoomDistance is < 0 or > 32 || !double.IsFinite(BacklashDamage) || BacklashDamage is < 0 or > 1000 ||
		(Kind == SpellProjectionKind.SandEffigy ? EffigyPrototypeId <= 0 || MaximumRoomDistance != 0 || CrossClosedDoors : EffigyPrototypeId != 0)
		? "Projection configuration needs an explicit plane, bounded real-time lifetime, range and damage policy. Sand Effigy also needs an item prototype and zero movement range."
		: null;
}

/// <summary>Instance-local restriction, shared by commands and native physical entry points.</summary>
public interface ISpellProjectionBoundary : IEffect
{
	SpellProjectionKind Kind { get; }
	string? TravelError(SpatialLocation destination, bool throughExit);
}

public interface ISpellOwnedProjectionService
{
	string? AdmissionError(ICharacter caster, SpellProjectionConfiguration configuration, int grade);
	ICharacterInstance Create(ICharacter caster, SpellProjectionConfiguration configuration, SpellLifecycleOrigin origin);
	bool OwnsInstance(long instanceId);
	void RequestRetirement(ICharacter projection, SpellRetirementReason reason);
	bool TryRetire(long instanceId, SpellRetirementReason reason, out string diagnostic);
	bool RequestAnchorRemoval(long itemId);
	int ReconcileRetirements(DateTime nowUtc, int limit = 100);
}

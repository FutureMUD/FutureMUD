using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic;

public sealed record AcquiredSpell(long CharacterId, long SpellId, int ControlledGrade, int ProfileVersion,
	DateTime AcquiredUtc, string Provenance, DateTime NextMasteryUtc, long Version);

public sealed record MagicCastingRoute(long CapabilityId, Guid AdmissionId, long SpellId, long TraitId,
	long ReserveId, bool Available, string Reason);

public enum MagicCastingMode { Manifest, Practice }
public sealed record MagicCastingIntent(ICharacter Actor, long CapabilityId, long SpellId, int Grade,
	bool Overreach, string Targets, MagicCastingMode Mode = MagicCastingMode.Manifest, string Method = "Say",
	Guid? OriginId = null, MagicCastingSpeech? Speech = null);

public sealed record MagicCastingCost(long HolderId, long ResourceId, double Amount);

/// <summary>An advisory immutable description, never a spendable token or shared spell state.</summary>
public sealed record ResolvedMagicCastingInvocation(Guid Id, long ActorId, long BodyId, long CharacterId,
	long CapabilityId, Guid CapabilityIdentity, Guid AdmissionId, long SpellId, long NativeSchoolId,
	long TraitId, long ReserveHolderId, long ReserveId, int Grade, SpellPower Power, bool Overreach,
	Difficulty Difficulty, string TargetSpecification, IReadOnlyList<SpellAdditionalParameter> TargetParameters,
	IReadOnlyList<MagicCastingCost> Costs, int ConfigurationVersion, int ProfileVersion, int ControlledGrade = 1,
	MagicCastingMode Mode = MagicCastingMode.Manifest, MagicCastingDelivery? Delivery = null);

public sealed record MagicCastingQuote(ResolvedMagicCastingInvocation? Invocation, string Reason)
{
	public bool Allowed => Invocation is not null;
}

public enum MagicCastingStatus { Refused, Failed, Succeeded, NeedsReview, Started }
public sealed record MagicCastingResult(MagicCastingStatus Status, string Message, Guid? OperationId = null);
public sealed record MagicCastingGrant(bool Changed, bool Allowed, string Message);

/// <summary>World-local authority for acquired knowledge, explicit routes, paid casting and progression.</summary>
public interface IMagicCastingService
{
	AcquiredSpell? Acquisition(ICharacter character, long spellId);
	IReadOnlyList<MagicCastingRoute> Routes(ICharacter actor, long? spellId = null);
	string? Preflight(ICharacter actor, long capabilityId, long spellId, int grade, bool overreach);
	MagicCastingQuote Quote(MagicCastingIntent intent);
	MagicCastingResult Cast(MagicCastingIntent intent);
	/// <summary>Resolves a complete authored formula into the same guarded paid casting pipeline.</summary>
	MagicCastingResult? CastFormula(ICharacter actor, string formula, string method = "Say", long? schoolId = null,
		MagicCastingSpeech? speech = null) => new(MagicCastingStatus.Refused, "Formula casting is unavailable.");
	MagicCastingGrant Grant(ICharacter authority, ICharacter target, long capabilityId, long spellId, string reason);
	MagicCastingGrant Enrol(ICharacter authority, ICharacter target, long capabilityId, string reason);
	void NotifyProgress(ICharacter character, long? traitId = null, long? spellId = null);
	void Reconcile(ICharacter actor);
	/// <summary>Interrupts owned prepaid practice without refund or progress; does not resume work.</summary>
	void InterruptPractice(ICharacter actor, string reason) { }
	/// <summary>Rechecks owned prepaid practice after native physical eligibility changes; lost inputs latch interruption.</summary>
	void NotifyPracticeInputsChanged(ICharacter actor) { }
	/// <summary>Re-evaluates existing canonical reserve maxima without granting energy, knowledge or progression.</summary>
	void NotifyCapacityChange(ICharacter actor) { }
	/// <summary>Pure native improvement ceiling; null retains native policy. Never trims stored proficiency.</summary>
	double? RawSkillImprovementCap(ICharacter actor, long traitId) => null;
	string? QuarantineReason(ICharacter actor, long? spellId = null, long? traitId = null, long? reserveId = null,
		IEnumerable<long>? itemIds = null);
}

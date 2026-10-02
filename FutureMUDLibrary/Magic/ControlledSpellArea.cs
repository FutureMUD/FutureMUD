using System.Collections.Generic;

#nullable enable
namespace MudSharp.Magic;

public enum SpellAreaScope { RoomCharacters, ImmediateCharacters }
public enum SpellAreaSelection { Ordered, RandomWithReplacement, RandomDistinct }
public enum SpellAreaIdentity { PhysicalBody, CanonicalCharacter }
public enum SpellAreaPlane { MagicReach, PhysicalAndMagicReach }

/// <summary>Explicit delivery variant; no implicit ally immunity or caster exclusion.</summary>
public sealed record ControlledSpellArea(SpellAreaScope Scope, SpellAreaSelection Selection,
	SpellAreaIdentity Identity, SpellAreaPlane Plane, int MaximumTargets, int MaximumApplications,
	bool IncludeCaster, bool IncludeAllies, bool IncludeOthers, bool SameLayer, bool GroundedOnly,
	bool ExcludeStaff, double CasterDamageMultiplier, double OtherDamageMultiplier,
	long FilterProgId, IReadOnlyList<ControlledSpellDelivery> Deliveries, string Provenance);

public sealed record SpellAreaTarget(long CharacterId, long InstanceId, long BodyId, double DamageMultiplier);
public sealed record ResolvedMagicCastingArea(SpellAreaScope Scope, SpellAreaSelection Selection,
	SpellAreaIdentity Identity, int MaximumApplications, IReadOnlyList<SpellAreaTarget> Candidates);

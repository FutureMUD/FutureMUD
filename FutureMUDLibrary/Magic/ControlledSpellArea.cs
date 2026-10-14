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
	long FilterProgId, IReadOnlyList<ControlledSpellDelivery> Deliveries, string Provenance,
	int ApplicationBase = 0, int ApplicationsPerGrade = 0, SpellAreaChainLightning? ChainLightning = null)
{
	public int ApplicationsAtGrade(int grade, bool amplified = false) => ChainLightning is not null && amplified
		? checked(ApplicationBase + 2 * ApplicationsPerGrade * grade)
		: ApplicationsPerGrade == 0
		? MaximumApplications
		: checked(ApplicationBase + ApplicationsPerGrade * grade);
}

/// <summary>Typed source-shaped chain binding. The native damage roll is shared by every selected strike.</summary>
public sealed record SpellAreaChainLightning(long SiltTagId, long ShallowsTagId, long WaterPlaneId,
	long LightningPlaneId);

public sealed record SpellAreaTarget(long CharacterId, long InstanceId, long BodyId, double DamageMultiplier);
public sealed record ResolvedMagicCastingArea(SpellAreaScope Scope, SpellAreaSelection Selection,
	SpellAreaIdentity Identity, int MaximumApplications, IReadOnlyList<SpellAreaTarget> Candidates);

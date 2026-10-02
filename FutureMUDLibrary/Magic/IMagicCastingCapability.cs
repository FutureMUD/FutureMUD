using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.RPG.Checks;
using MudSharp.Body.Traits;

#nullable enable
namespace MudSharp.Magic;

/// <summary>Optional explicit admission policy. Presence, including invalid or disabled policy, excludes a legacy grant.</summary>
public interface IMagicCastingCapability : IMagicCapability
{
	bool HasCastingPolicy { get; }
	MagicCastingPolicy? CastingPolicy { get; }
	IReadOnlyList<string> CastingConfigurationErrors();
}

public enum MagicCastingPrerequisiteKind { Spell, SupportTrait }
public sealed record MagicCastingPrerequisite(Guid Key, long SpellId, int MinimumGrade, double MinimumProficiency,
	MagicCastingPrerequisiteKind Kind = MagicCastingPrerequisiteKind.Spell, long? TraitId = null)
{
	public long SourceId => Kind == MagicCastingPrerequisiteKind.SupportTrait ? TraitId ?? 0 : SpellId;
}

public sealed record MagicCastingSupportGrant(Guid Key, long TraitId, double OpeningSkill, double? RawSkillCap,
	bool Starting, IReadOnlyList<MagicCastingPrerequisite> Prerequisites);

public sealed record MagicCastingAdmission(Guid Key, long SpellId, long? TraitId, bool Starting,
	int MinimumGrade, int MaximumGrade, IReadOnlyList<MagicCastingPrerequisite> Prerequisites,
	double? OpeningSkill = null, double? RawSkillCap = null, bool CapRelativeProficiency = false)
{
	public double RequiredProficiency(ControlledSpellGrade grade) => CapRelativeProficiency
		? grade.MinimumProficiency * RawSkillCap!.Value / 100.0
		: grade.MinimumProficiency;
}

public sealed record MagicCastingPolicy(int Version, Guid Identity, bool Enabled, long DefaultTraitId,
	long SourceResourceId, long ReserveResourceId, bool PassiveEntitlement, int StartingGrantVersion,
	IReadOnlyList<MagicCastingAdmission> Admissions, IReadOnlyList<MagicCastingSupportGrant>? SupportGrants = null)
{
	public IReadOnlyList<MagicCastingSupportGrant> Supports => SupportGrants ?? Array.Empty<MagicCastingSupportGrant>();
}

public sealed record ControlledSpellGrade(int Grade, SpellPower Power, double MinimumProficiency, int DifficultySteps);

public sealed record SpellScalarBinding(string List, int Index, string Effect, string Field, string Expression);

/// <summary>Explicit practice policy; a null maximum uses the ordinary admission/profile range.</summary>
public sealed record ControlledSpellPractice(bool Enabled, Difficulty Difficulty, TimeSpan Duration,
	double EnergyMultiplier, int? MaximumGrade, bool RequiresSpeech, bool RequiresFreeHand, bool AllowMovement);

/// <summary>One shared grade meaning per spell; capability admissions may only narrow its range.</summary>
public sealed record ControlledSpellProfile(int Version, IReadOnlyList<ControlledSpellGrade> Grades,
	double OverreachMultiplier, int OverreachDifficultySteps, double MasteryChance,
	TimeSpan MasteryInterval, TimeSpan SkillInterval, double OpeningSkill,
	IReadOnlyList<SpellScalarBinding> ScalarBindings, ControlledSpellEfficiency? Efficiency = null,
	ControlledSpellPractice? Practice = null, ControlledSpellIncantation? Incantation = null,
	ControlledSpellArea? Area = null);

public interface IControlledMagicSpell : IMagicSpell
{
	IReadOnlyDictionary<IMagicResource, ITraitExpression> CastingCosts { get; }
	ControlledSpellProfile? GradeProfile { get; }
	IReadOnlyList<string> GradeConfigurationErrors();
}

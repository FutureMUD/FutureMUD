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

public sealed record MagicCastingPrerequisite(Guid Key, long SpellId, int MinimumGrade, double MinimumProficiency);

public sealed record MagicCastingAdmission(Guid Key, long SpellId, long? TraitId, bool Starting,
	int MinimumGrade, int MaximumGrade, IReadOnlyList<MagicCastingPrerequisite> Prerequisites);

public sealed record MagicCastingPolicy(int Version, Guid Identity, bool Enabled, long DefaultTraitId,
	long SourceResourceId, long ReserveResourceId, bool PassiveEntitlement, int StartingGrantVersion,
	IReadOnlyList<MagicCastingAdmission> Admissions);

public sealed record ControlledSpellGrade(int Grade, SpellPower Power, double MinimumProficiency, int DifficultySteps);

public sealed record SpellScalarBinding(string List, int Index, string Effect, string Field, string Expression);

/// <summary>One shared grade meaning per spell; capability admissions may only narrow its range.</summary>
public sealed record ControlledSpellProfile(int Version, IReadOnlyList<ControlledSpellGrade> Grades,
	double OverreachMultiplier, int OverreachDifficultySteps, double MasteryChance,
	TimeSpan MasteryInterval, TimeSpan SkillInterval, double OpeningSkill,
	IReadOnlyList<SpellScalarBinding> ScalarBindings);

public interface IControlledMagicSpell : IMagicSpell
{
	IReadOnlyDictionary<IMagicResource, ITraitExpression> CastingCosts { get; }
	ControlledSpellProfile? GradeProfile { get; }
	IReadOnlyList<string> GradeConfigurationErrors();
}

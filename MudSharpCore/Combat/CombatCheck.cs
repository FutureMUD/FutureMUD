using MudSharp.Body.Traits;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Interfaces;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

#nullable enable
namespace MudSharp.Combat;

/// <summary>Opt-in accuracy/active defence checks against a known melee opponent.</summary>
internal sealed class CombatCheck(ICheck inner, IFuturemud gameworld) : ICheck
{
	public static ICheck For(IFuturemud gameworld, CheckType type)
	{
		var check = gameworld.GetCheck(type);
		return IsEnabled(gameworld) ? new CombatCheck(check, gameworld) : check;
	}

	private static bool IsEnabled(IFuturemud gameworld) =>
		bool.TryParse(gameworld.GetStaticConfiguration("BlindFightingEnabled"), out var enabled) && enabled;

	internal static ICheck ForDefense(IFuturemud gameworld, CheckType type, ICombatMove attack) =>
		CoversPhysicalDefense(attack) ? For(gameworld, type) : gameworld.GetCheck(type);

	internal static bool CoversPhysicalDefense(ICombatMove attack) =>
		attack is not (IRangedAttackMove or IRangedWeaponAttackMove or BreakClinchMove or ExtendGrappleMove or
			ForcedMovementMove or TakedownMove or WrenchingAttack or StrangleAttack or ScreechAttackMove) &&
		attack is MeleeWeaponAttack or NaturalAttackMove or ClinchAttackMove or ClinchNaturalAttackMove or
			StartClinchMove or InitiateGrappleMove or AuxiliaryMove or MagicPowerAttackMove;

	internal static Difficulty Setting(IFuturemud world, string name, Difficulty fallback) =>
		int.TryParse(world.GetStaticConfiguration(name), out var value) && Enum.IsDefined((Difficulty)value)
			? (Difficulty)value : fallback;

	internal static Difficulty? SightDifficulty(ICharacter actor, ICharacter opponent)
	{
		if (actor.IsAdministrator()) return null;
		// IgnoreSpotting prevents an assessment or a strategy preview from rolling SpotStealth,
		// learning a skill, or revealing a hidden opponent.
		if (!actor.CanSee(opponent, PerceiveIgnoreFlags.IgnoreSpotting) || actor.VisionPercentage <= 0.0)
			return Setting(actor.Gameworld, "BlindFightingUnseenDifficulty", Difficulty.ExtremelyHard);

		var sight = actor.IlluminationSightDifficulty(opponent.Location);
		if (sight < Setting(actor.Gameworld, "BlindFightingMinimumSightDifficulty", Difficulty.Hard)) return null;
		return sight switch
		{
			>= Difficulty.Impossible => Setting(actor.Gameworld, "BlindFightingUnseenDifficulty", Difficulty.ExtremelyHard),
			Difficulty.Insane => Setting(actor.Gameworld, "BlindFightingSightInsaneDifficulty", Difficulty.VeryHard),
			Difficulty.ExtremelyHard => Setting(actor.Gameworld, "BlindFightingSightExtremelyHardDifficulty", Difficulty.Hard),
			Difficulty.VeryHard => Setting(actor.Gameworld, "BlindFightingSightVeryHardDifficulty", Difficulty.Normal),
			_ => Setting(actor.Gameworld, "BlindFightingSightHardDifficulty", Difficulty.Easy)
		};
	}

	internal static Difficulty MeritDifficulty(ICharacter actor, ICharacter opponent, Difficulty difficulty)
	{
		var stages = actor.Merits.OfType<IBlindFightingMerit>().DistinctBy(x => x.Id)
			.Where(x => x.Applies(actor, opponent)).Sum(x => (long)x.DifficultyStages);
		return (Difficulty)Math.Clamp((long)difficulty + stages, (long)Difficulty.Automatic, (long)Difficulty.Impossible);
	}

	private (CheckEvaluationContext? Context, ICheck? Blind, Difficulty Difficulty) Prepare(
		IPerceivableHaveTraits actor, IPerceivable? opponent, (string Parameter, object value)[] parameters,
		bool stageSharedBonus = true)
	{
		// Recheck here so a retained wrapper also honours a live configuration change.
		if (!IsEnabled(Gameworld)) return default;
		if (actor is not ICharacter character || opponent is not ICharacter target ||
			!character.ColocatedWith(target)) return default;
		var blind = Gameworld.GetCheck(CheckType.BlindFightingCheck);
		if (blind?.TargetNumberExpression is not { } expression || SightDifficulty(character, target) is not { } sight) return default;
		var difficulty = MeritDifficulty(character, target, sight);
		// OGL applies the ordinary innate bonus directly to its DC, so do not also stage
		// the BF floor with that same bonus. BonusAbsent deliberately accepts no bonuses.
		var bonus = inner is BonusAbsentCheck ? 0.0 : (stageSharedBonus ? actor.GetCurrentBonusLevel() : 0.0) +
			actor.EffectsOfType<ICheckBonusEffect>().Where(x => x.Applies(actor) && x.AppliesToCheck(CheckType.BlindFightingCheck) &&
				(stageSharedBonus || !x.AppliesToCheck(Type))).Sum(x => x.CheckBonus) +
			actor.Merits.OfType<ICheckBonusMerit>().Where(x => x.Applies(actor, target))
				.Where(x => stageSharedBonus || x.CheckBonus(character, target, Type) == 0.0)
				.Sum(x => x.CheckBonus(character, target, CheckType.BlindFightingCheck));
		// No weapon/tool trait is supplied: the ordinary trait expression owns all BF bindings.
		var maximum = expression.EvaluateWith(actor, values: parameters);
		return (new CheckEvaluationContext(maximum, difficulty.ApplyBonus(bonus), true), blind, difficulty);
	}

	private static (string Parameter, object value)[] WithContext((string Parameter, object value)[] parameters,
		CheckEvaluationContext? context) => context is null ? parameters :
		parameters.Where(x => x.Parameter != CheckEvaluationContext.ParameterName)
			.Append((CheckEvaluationContext.ParameterName, (object)context)).ToArray();

	private static void Learn(IPerceivableHaveTraits actor, IPerceivable? opponent,
		(CheckEvaluationContext? Context, ICheck? Blind, Difficulty Difficulty) prepared,
		TraitUseType use, (string Parameter, object value)[] parameters)
	{
		if (prepared.Context?.WasEvaluated != true || prepared.Blind is not { } blind ||
			!CheckLearningScope.CanContinue(actor)) return;
		if (actor is ICharacter character && !MudSharp.NPC.AI.CommandExecutionScope.TryContinue(character)) return;
		// A constant expression has nothing to learn or branch and needs no auxiliary roll.
		if (!blind.TargetNumberExpression.Parameters.Any(x =>
			blind.TargetNumberExpression.Formula.ParameterNames.Any(name => name.EqualTo(x.Key)) &&
			(blind.ImproveTraits && x.Value.CanImprove || blind.CanTraitBranchIfMissing && x.Value.CanBranch))) return;
		blind.Check(actor, prepared.Difficulty, opponent, traitUseType: use, customParameters: parameters);
	}

	public double TargetNumber(IPerceivableHaveTraits checkee, Difficulty difficulty, ITraitDefinition trait,
		IPerceivable? target = null, double externalBonus = 0.0, params (string Parameter, object value)[] customParameters)
	{
		var prepared = Prepare(checkee, target, customParameters, inner is not OGLCheck);
		return inner.TargetNumber(checkee, difficulty, trait, target, externalBonus, WithContext(customParameters, prepared.Context));
	}

	internal double ImpairedTargetNumberOrDefault(ICharacter actor, Difficulty difficulty, IPerceivable target,
		double fallback)
	{
		var prepared = Prepare(actor, target, []);
		return prepared.Context is null ? fallback :
			inner.TargetNumber(actor, difficulty, null, target, customParameters: WithContext([], prepared.Context));
	}

	public CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty, IPerceivable? target = null,
		IUseTrait? tool = null, double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		var prepared = Prepare(checkee, target, customParameters, inner is not OGLCheck);
		var result = inner.Check(checkee, difficulty, target, tool, externalBonus, traitUseType,
			WithContext(customParameters, prepared.Context));
		Learn(checkee, target, prepared, traitUseType, customParameters);
		return result;
	}

	public CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty, ITraitDefinition trait,
		IPerceivable? target = null, double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		var prepared = Prepare(checkee, target, customParameters, inner is not OGLCheck);
		var result = inner.Check(checkee, difficulty, trait, target, externalBonus, traitUseType,
			WithContext(customParameters, prepared.Context));
		Learn(checkee, target, prepared, traitUseType, customParameters);
		return result;
	}

	public Dictionary<Difficulty, CheckOutcome> CheckAgainstAllDifficulties(IPerceivableHaveTraits checkee,
		Difficulty referenceDifficulty, ITraitDefinition trait, IPerceivable? target = null, double externalBonus = 0.0,
		TraitUseType traitUseType = TraitUseType.Practical, params (string Parameter, object value)[] customParameters)
	{
		var prepared = Prepare(checkee, target, customParameters);
		var result = inner.CheckAgainstAllDifficulties(checkee, referenceDifficulty, trait, target, externalBonus,
			traitUseType, WithContext(customParameters, prepared.Context));
		Learn(checkee, target, prepared, traitUseType, customParameters);
		return result;
	}

	public Tuple<CheckOutcome, CheckOutcome> MultiDifficultyCheck(IPerceivableHaveTraits checkee, Difficulty difficulty1,
		Difficulty difficulty2, IPerceivable? target = null, ITraitDefinition? tool = null, double externalBonus = 0.0,
		TraitUseType traitUseType = TraitUseType.Practical, params (string Parameter, object value)[] customParameters)
	{
		var prepared = Prepare(checkee, target, customParameters, inner is not OGLCheck);
		var result = inner.MultiDifficultyCheck(checkee, difficulty1, difficulty2, target, tool, externalBonus,
			traitUseType, WithContext(customParameters, prepared.Context));
		Learn(checkee, target, prepared, traitUseType, customParameters);
		return result;
	}

	public string Name => inner.Name;
	public long Id => inner.Id;
	public string FrameworkItemType => inner.FrameworkItemType;
	public IFuturemud Gameworld => gameworld;
	public CheckType Type => inner.Type;
	public bool ImproveTraits => inner.ImproveTraits;
	public bool CanTraitBranchIfMissing => inner.CanTraitBranchIfMissing;
	public FailIfTraitMissingType FailIfTraitMissing => inner.FailIfTraitMissing;
	public ITraitExpression TargetNumberExpression => inner.TargetNumberExpression;
	public string CheckTemplateName => inner.CheckTemplateName;
	public Difficulty MaximumDifficultyForImprovement => inner.MaximumDifficultyForImprovement;
	public bool WouldBeAbjectFailure(IPerceivableHaveTraits checkee, ITraitDefinition? trait = null) => inner.WouldBeAbjectFailure(checkee, trait);
}

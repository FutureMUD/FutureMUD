using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Traits;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Magic;

#nullable enable
namespace MudSharp.RPG.Checks;

/// <summary>
/// Optional per-invocation constraints carried in ICheck's custom parameters. They constrain
/// resolution, not the original check's learning difficulty or choice of traits.
/// </summary>
public sealed class CheckEvaluationContext(double maximumBaseTargetNumber, Difficulty minimumDifficulty,
	bool suppressVisionPenalty = false)
{
	public const string ParameterName = "__checkEvaluationContext";
	/// <summary>Optional double target-number bonus; does not bypass retained check outcomes.</summary>
	public const string FlatBonusParameterName = "__checkFlatTargetBonus";
	public double MaximumBaseTargetNumber { get; } = maximumBaseTargetNumber;
	public Difficulty MinimumDifficulty { get; } = minimumDifficulty;
	public bool SuppressVisionPenalty { get; } = suppressVisionPenalty;
	public bool WasEvaluated { get; private set; }

	public static CheckEvaluationContext? Find(IEnumerable<(string Parameter, object value)> parameters) =>
		parameters.FirstOrDefault(x => x.Parameter == ParameterName).value as CheckEvaluationContext;

	/// <summary>Evaluate a classified native check, applying intrinsic equipment bonuses before target caps.</summary>
	public static double EvaluateForCheck(ITraitExpression expression, IHaveTraits owner, ITraitDefinition? trait,
		CheckType checkType, params (string Parameter, object value)[] parameters) =>
		EvaluateCore(expression, owner, trait, checkType, parameters);

	public static double Evaluate(ITraitExpression expression, IHaveTraits owner, ITraitDefinition? trait,
		params (string Parameter, object value)[] parameters) => EvaluateCore(expression, owner, trait, null, parameters);

	private static double EvaluateCore(ITraitExpression expression, IHaveTraits owner, ITraitDefinition? trait,
		CheckType? checkType, (string Parameter, object value)[] parameters)
	{
		var value = expression.EvaluateWith(owner, trait, values: parameters);
		var character = owner as ICharacter ?? (owner as IBody)?.Actor;
		if (character is not null && (trait?.TraitType is TraitType.Skill or TraitType.TheoreticalSkill or TraitType.DerivedSkill ||
			expression.Parameters?.Values.Any(x => x?.Trait?.TraitType is TraitType.Skill or TraitType.TheoreticalSkill or TraitType.DerivedSkill) == true))
			value += character.CombinedEffectsOfType<IMindImpairmentEffect>().Where(x => x.Applies()).Sum(x => x.SkillUseModifier);
		if (parameters.FirstOrDefault(x => x.Parameter == FlatBonusParameterName).value is double bonus && double.IsFinite(bonus))
			value += bonus;
		if (checkType is { } type)
		{
			var kind = type == CheckType.ResistMagicSpellCheck ? EquippedItemBonusKind.SpellResistance :
				type.IsOffensiveCombatAction() ? EquippedItemBonusKind.CombatAttack :
				type.IsDefensiveCombatAction() ? EquippedItemBonusKind.CombatDefence : (EquippedItemBonusKind?)null;
			if (kind is { } bonusKind) value += EquippedItemBonuses.BonusFor(character, bonusKind);
		}
		var context = Find(parameters);
		if (context is null) return value;
		context.WasEvaluated = true;
		return Math.Min(value, context.MaximumBaseTargetNumber);
	}

	public static Difficulty Constrain(Difficulty difficulty, params (string Parameter, object value)[] parameters) =>
		Find(parameters) is { } context && context.MinimumDifficulty > difficulty
			? context.MinimumDifficulty : difficulty;
}

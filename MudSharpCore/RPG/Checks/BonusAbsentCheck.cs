using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;
using MudSharp.Framework;

namespace MudSharp.RPG.Checks;

/// <summary>
/// The BonusAbsentCheck is similar to a StandardCheck but that no intrinsic bonus is used
/// </summary>
public class BonusAbsentCheck : StandardCheck
{
	public BonusAbsentCheck(Models.Check check, IFuturemud game) : base(check, game)
	{
	}

	public override CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty,
		IPerceivable target = null, IUseTrait tool = null, double externalBonus = 0.0,
		TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		return Check(checkee, difficulty, tool?.Trait, target, externalBonus, traitUseType, customParameters);
	}

	public override CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty,
		ITraitDefinition trait, IPerceivable target = null, double externalBonus = 0.0,
		TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		var targetNumber = TargetNumberExpression.EvaluateWith(checkee, trait, values: customParameters);
		var difficultyModifier = Modifiers[difficulty];
		var outcome = RollAgainst(targetNumber + difficultyModifier, out var rolls);

		var result = HandleStandardCheck(checkee, target, outcome, difficulty, trait, true, traitUseType);

		result.CheckTemplateName = CheckTemplateName;
		result.CheckType = Type;
		result.FinalBonus = 0;
		result.ActiveBonuses = Enumerable.Empty<Tuple<string, double>>();
		result.OriginalDifficulty = difficulty;
		result.OriginalDifficultyModifier = 0;
		result.FinalDifficulty = difficulty;
		result.FinalDifficultyModifier = difficultyModifier;
		result.Burden = 0;
		result.TargetNumber = targetNumber + difficultyModifier;
		result.Rolls = rolls;

		return result;
	}

	public override Dictionary<Difficulty, CheckOutcome> CheckAgainstAllDifficulties(IPerceivableHaveTraits checkee,
		Difficulty referenceDifficulty, ITraitDefinition trait, IPerceivable target = null,
		double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		var results = new Dictionary<Difficulty, CheckOutcome>();
		var targetNumber = TargetNumberExpression.EvaluateWith(checkee, trait, values: customParameters);
		var rolls = new[]
		{
			Constants.Random.NextDouble() * 100.0,
			Constants.Random.NextDouble() * 100.0,
			Constants.Random.NextDouble() * 100.0
		};
		foreach (var evaluatedDifficulty in AllDifficulties)
		{
			var originalDifficulty = evaluatedDifficulty;
			var originalModifier = Modifiers[originalDifficulty];

			var difficultyModifier = Modifiers[originalDifficulty];
			var successes = RandomUtilities.EvaluateConsecutiveSuccesses(rolls, targetNumber + difficultyModifier);
			var outcome = GetOutcome(successes);
			var result = HandleStandardCheck(checkee, target, outcome, originalDifficulty, trait,
				evaluatedDifficulty == referenceDifficulty, traitUseType);

			result.CheckTemplateName = CheckTemplateName;
			result.CheckType = Type;
			result.FinalBonus = 0;
			result.ActiveBonuses = Enumerable.Empty<Tuple<string, double>>();
			result.OriginalDifficulty = referenceDifficulty;
			result.OriginalDifficultyModifier = 0;
			result.FinalDifficulty = originalDifficulty;
			result.FinalDifficultyModifier = 0;
			result.Burden = 0;
			result.TargetNumber = targetNumber + difficultyModifier;
			result.Rolls = rolls;

			results[evaluatedDifficulty] = result;
		}

		return results;
	}

	public override Tuple<CheckOutcome, CheckOutcome> MultiDifficultyCheck(IPerceivableHaveTraits checkee,
		Difficulty difficulty1, Difficulty difficulty2, IPerceivable target = null, ITraitDefinition trait = null,
		double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		var originalDifficulty1 = difficulty1;
		var originalDifficultyModifier1 = Modifiers[originalDifficulty1];
		var originalDifficulty2 = difficulty2;
		var originalDifficultyModifier2 = Modifiers[originalDifficulty2];

		var rolls = new[]
		{
			Constants.Random.NextDouble() * 100.0,
			Constants.Random.NextDouble() * 100.0,
			Constants.Random.NextDouble() * 100.0
		};

		var basetarget = TargetNumberExpression.EvaluateWith(checkee, trait, values: customParameters);
		var difficultyModifier1 = Modifiers[difficulty1];
		var difficultyModifier2 = Modifiers[difficulty2];

		var successes1 = RandomUtilities.EvaluateConsecutiveSuccesses(rolls, basetarget + difficultyModifier1);

		var successes2 = RandomUtilities.EvaluateConsecutiveSuccesses(rolls, basetarget + difficultyModifier2);
		var outcome1 = GetOutcome(successes1);
		var outcome2 = GetOutcome(successes2);

		var result1 = HandleStandardCheck(checkee, target, outcome1, difficulty1, trait, true, traitUseType);
		var result2 = HandleStandardCheck(checkee, target, outcome2, difficulty2, trait, true, traitUseType);

		result1.CheckTemplateName = CheckTemplateName;
		result1.CheckType = Type;
		result1.FinalBonus = 0;
		result1.ActiveBonuses = Enumerable.Empty<Tuple<string, double>>();
		result1.OriginalDifficulty = originalDifficulty1;
		result1.OriginalDifficultyModifier = originalDifficultyModifier1;
		result1.FinalDifficulty = difficulty1;
		result1.FinalDifficultyModifier = difficultyModifier1;
		result1.Burden = 0;
		result1.TargetNumber = basetarget + difficultyModifier1;
		result1.Rolls = rolls.Select(x => (double)x).ToList();

		result2.CheckTemplateName = CheckTemplateName;
		result2.CheckType = Type;
		result2.FinalBonus = 0;
		result2.ActiveBonuses = Enumerable.Empty<Tuple<string, double>>();
		result2.OriginalDifficulty = originalDifficulty2;
		result2.OriginalDifficultyModifier = originalDifficultyModifier2;
		result2.FinalDifficulty = difficulty2;
		result2.FinalDifficultyModifier = difficultyModifier2;
		result2.Burden = 0;
		result2.TargetNumber = basetarget + difficultyModifier2;
		result2.Rolls = rolls.Select(x => (double)x).ToList();

		return Tuple.Create(result1, result2);
	}
}
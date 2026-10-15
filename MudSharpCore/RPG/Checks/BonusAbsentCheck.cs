using MudSharp.Body.Traits;

namespace MudSharp.RPG.Checks;

/// <summary>
/// The BonusAbsentCheck is similar to a StandardCheck but that no intrinsic bonus is used
/// </summary>
public class BonusAbsentCheck : StandardCheck
{
    public BonusAbsentCheck(Models.Check check, IFuturemud game) : base(check, game)
    {
    }

	public override double TargetNumber(IPerceivableHaveTraits checkee, Difficulty difficulty, ITraitDefinition trait,
		IPerceivable target = null, double externalBonus = 0.0, params (string Parameter, object value)[] customParameters)
	{
		if (CheckEvaluationContext.Find(customParameters) is null)
			return base.TargetNumber(checkee, difficulty, trait, target, externalBonus, customParameters);
		return CheckEvaluationContext.Evaluate(TargetNumberExpression, checkee, trait, customParameters) +
			Modifiers[CheckEvaluationContext.Constrain(difficulty, customParameters)];
	}

    public override CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty,
        IPerceivable target = null, IUseTrait tool = null, double externalBonus = 0.0,
        TraitUseType traitUseType = TraitUseType.Practical,
        params (string Parameter, object value)[] customParameters)
    {
        using var learning = CheckLearningScope.Enter(checkee);
        return Check(checkee, difficulty, tool?.Trait, target, externalBonus, traitUseType, customParameters);
    }

    public override CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty,
        ITraitDefinition trait, IPerceivable target = null, double externalBonus = 0.0,
        TraitUseType traitUseType = TraitUseType.Practical,
        params (string Parameter, object value)[] customParameters)
    {
        using var learning = CheckLearningScope.Enter(checkee);
        double targetNumber = CheckEvaluationContext.Evaluate(TargetNumberExpression, checkee, trait, customParameters);
        double difficultyModifier = Modifiers[CheckEvaluationContext.Constrain(difficulty, customParameters)];
        Outcome outcome = RollAgainst(targetNumber + difficultyModifier, out List<double> rolls);

        CheckOutcome result = HandleStandardCheck(checkee, target, outcome, difficulty, trait, true, traitUseType);

        result.CheckTemplateName = CheckTemplateName;
        result.CheckType = Type;
        result.FinalBonus = 0;
        result.ActiveBonuses = Enumerable.Empty<Tuple<string, double>>();
        result.OriginalDifficulty = difficulty;
        result.OriginalDifficultyModifier = 0;
        result.FinalDifficulty = CheckEvaluationContext.Constrain(difficulty, customParameters);
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
        using var learning = CheckLearningScope.Enter(checkee);
        Dictionary<Difficulty, CheckOutcome> results = new();
        double targetNumber = CheckEvaluationContext.Evaluate(TargetNumberExpression, checkee, trait, customParameters);
        double[] rolls = new[]
        {
            Constants.Random.NextDouble() * 100.0,
            Constants.Random.NextDouble() * 100.0,
            Constants.Random.NextDouble() * 100.0
        };
        foreach (Difficulty evaluatedDifficulty in AllDifficulties)
        {
            Difficulty originalDifficulty = evaluatedDifficulty;
            double originalModifier = Modifiers[originalDifficulty];

            double difficultyModifier = Modifiers[CheckEvaluationContext.Constrain(originalDifficulty, customParameters)];
            int successes = RandomUtilities.EvaluateConsecutiveSuccesses(rolls, targetNumber + difficultyModifier);
            Outcome outcome = GetOutcome(successes);
            CheckOutcome result = HandleStandardCheck(checkee, target, outcome, originalDifficulty, trait,
                evaluatedDifficulty == referenceDifficulty, traitUseType);

            result.CheckTemplateName = CheckTemplateName;
            result.CheckType = Type;
            result.FinalBonus = 0;
            result.ActiveBonuses = Enumerable.Empty<Tuple<string, double>>();
            result.OriginalDifficulty = referenceDifficulty;
            result.OriginalDifficultyModifier = 0;
            result.FinalDifficulty = CheckEvaluationContext.Constrain(originalDifficulty, customParameters);
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
        using var learning = CheckLearningScope.Enter(checkee);
        Difficulty originalDifficulty1 = difficulty1;
        double originalDifficultyModifier1 = Modifiers[originalDifficulty1];
        Difficulty originalDifficulty2 = difficulty2;
        double originalDifficultyModifier2 = Modifiers[originalDifficulty2];

        double[] rolls = new[]
        {
            Constants.Random.NextDouble() * 100.0,
            Constants.Random.NextDouble() * 100.0,
            Constants.Random.NextDouble() * 100.0
        };

        double basetarget = CheckEvaluationContext.Evaluate(TargetNumberExpression, checkee, trait, customParameters);
        double difficultyModifier1 = Modifiers[CheckEvaluationContext.Constrain(difficulty1, customParameters)];
        double difficultyModifier2 = Modifiers[CheckEvaluationContext.Constrain(difficulty2, customParameters)];

        int successes1 = RandomUtilities.EvaluateConsecutiveSuccesses(rolls, basetarget + difficultyModifier1);

        int successes2 = RandomUtilities.EvaluateConsecutiveSuccesses(rolls, basetarget + difficultyModifier2);
        Outcome outcome1 = GetOutcome(successes1);
        Outcome outcome2 = GetOutcome(successes2);

        CheckOutcome result1 = HandleStandardCheck(checkee, target, outcome1, difficulty1, trait, true, traitUseType);
        CheckOutcome result2 = HandleStandardCheck(checkee, target, outcome2, difficulty2, trait, true, traitUseType);

        result1.CheckTemplateName = CheckTemplateName;
        result1.CheckType = Type;
        result1.FinalBonus = 0;
        result1.ActiveBonuses = Enumerable.Empty<Tuple<string, double>>();
        result1.OriginalDifficulty = originalDifficulty1;
        result1.OriginalDifficultyModifier = originalDifficultyModifier1;
        result1.FinalDifficulty = CheckEvaluationContext.Constrain(difficulty1, customParameters);
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
        result2.FinalDifficulty = CheckEvaluationContext.Constrain(difficulty2, customParameters);
        result2.FinalDifficultyModifier = difficultyModifier2;
        result2.Burden = 0;
        result2.TargetNumber = basetarget + difficultyModifier2;
        result2.Rolls = rolls.Select(x => (double)x).ToList();

        return Tuple.Create(result1, result2);
    }
}

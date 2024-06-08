using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Logging;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Checks;

public class BranchCheck : FrameworkItem, ICheck
{
	public BranchCheck(Models.Check check, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = check.Type + 1;
		TargetNumberExpression = gameworld.TraitExpressions.Get(check.TraitExpressionId);
		Type = (CheckType)check.Type;
		CheckTemplateName = check.CheckTemplate.Name;
	}

	public CheckType Type { get; protected set; }

	public bool ImproveTraits => false;

	public bool CanTraitBranchIfMissing => false;

	public FailIfTraitMissingType FailIfTraitMissing => FailIfTraitMissingType.DoNotFail;

	public double TargetNumber(IPerceivableHaveTraits checkee, Difficulty difficulty, ITraitDefinition trait,
		IPerceivable target = null, double externalBonus = 0,
		params (string Parameter, object value)[] customParameters)
	{
		// TODO: effects
		var merits = checkee.Merits
		                    .OfType<ITraitLearningMerit>()
		                    .Where(x => x.Applies(checkee))
		                    .Select(x => x.BranchingChanceModifier(checkee, trait))
		                    .Aggregate(1.0, (x, y) => x * y);

		if (!(trait is ISkillDefinition sd))
		{
			return TargetNumberExpression.Evaluate(checkee, trait) * merits * trait.BranchMultiplier;
		}

		var effect = checkee.EffectsOfType<IncreasedBranchChance>().FirstOrDefault();
		if (effect == null)
		{
			effect = new IncreasedBranchChance(checkee);
			checkee.AddEffect(effect);
		}

		var expr = IncreasedBranchChance.IncreasedCapExpression;
		expr.Formula.Parameters["attempts"] = effect.GetAttemptsForSkill(sd);
		expr.Formula.Parameters["base"] = TargetNumberExpression.Evaluate(checkee, trait) * merits;

		effect.UseSkill(sd);
		return expr.EvaluateWith(checkee, values: customParameters);
	}

	public CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty, IPerceivable target = null,
		IUseTrait tool = null, double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		return Check(checkee, difficulty, tool?.Trait, target, customParameters: customParameters);
	}

	public CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty, ITraitDefinition trait,
		IPerceivable target = null, double externalBonus = 0.0,
		TraitUseType traitUseType = TraitUseType.Theoretical,
		params (string Parameter, object value)[] customParameters)
	{
		var targetNumber = TargetNumber(checkee, difficulty, trait, target, customParameters: customParameters);
		var outcome = RollAgainst(targetNumber, out var rolls);
		return new CheckOutcome
		{
			Outcome = outcome,
			Rolls = rolls,
			CheckTemplateName = CheckTemplateName,
			CheckType = Type,
			ActiveBonuses = Enumerable.Empty<Tuple<string, double>>(),
			ImprovedTraits = Enumerable.Empty<ITraitDefinition>(),
			AcquiredTraits = Enumerable.Empty<ITraitDefinition>(),
			TargetNumber = targetNumber
		};
	}

	public Tuple<CheckOutcome, CheckOutcome> MultiDifficultyCheck(IPerceivableHaveTraits checkee,
		Difficulty difficulty1, Difficulty difficulty2, IPerceivable target = null, ITraitDefinition tool = null,
		double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		throw new NotSupportedException("BranchChecks should never be called in a MultiDifficultyCheck.");
	}

	public Dictionary<Difficulty, CheckOutcome> CheckAgainstAllDifficulties(IPerceivableHaveTraits checkee,
		Difficulty referenceDifficulty, ITraitDefinition trait,
		IPerceivable target = null, double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		throw new NotSupportedException(
			"BranchChecks should never be called in a CheckAgainstAllDifficulties check.");
	}

	protected virtual Outcome RollAgainst(double target, out List<double> rolls)
	{
		return GetOutcome(RandomUtilities.ConsecutiveRoll(100.0, target, 3, out rolls));
	}

	protected virtual Outcome RollAgainst(double target)
	{
		return GetOutcome(RandomUtilities.ConsecutiveRoll(100.0, target, 3));
	}

	protected virtual Outcome GetOutcome(int successes)
	{
		switch (successes)
		{
			case -3:
				return Outcome.MajorFail;
			case -2:
				return Outcome.Fail;
			case -1:
				return Outcome.MinorFail;
			case 1:
				return Outcome.MinorPass;
			case 2:
				return Outcome.Pass;
			case 3:
				return Outcome.MajorPass;
			default:
				return Outcome.NotTested;
		}
	}

	/// <summary>
	///     A TraitExpression representing the Target Number of the check
	/// </summary>
	public ITraitExpression TargetNumberExpression { get; protected set; }

	/// <summary>
	///     Name of the Check Template that this check uses
	/// </summary>
	public string CheckTemplateName { get; set; }

	public Difficulty MaximumDifficultyForImprovement => Difficulty.Impossible;
	public IFuturemud Gameworld { get; set; }
	public override string FrameworkItemType => "BranchCheck";

	public bool WouldBeAbjectFailure(IPerceivableHaveTraits checkee, ITraitDefinition trait = null)
	{
		return false;
	}
}
using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Character;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Checks;

/// <summary>
///     A D20/OGL style check, where a 1d20 + bonuses is rolled and returns a success if greater than or equal to a target number, a fail if not, a major success on a natural 20 and a major fail on a natural 1.
/// </summary>
public class OGLCheck : StandardCheck
{
	public OGLCheck(Check check, IFuturemud game) : base(check, game)
	{
		;
	}

	#region Check overrides

	public override string FrameworkItemType => "OGLCheck";

	public override Tuple<CheckOutcome, CheckOutcome> MultiDifficultyCheck(IPerceivableHaveTraits checkee,
		Difficulty difficulty1, Difficulty difficulty2, IPerceivable target = null, ITraitDefinition trait = null,
		double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		var checkeeAsCharacter = checkee as ICharacter ?? (checkee as IBody)?.Actor;

		// Final difficulty
		var bonus =
			checkee.GetCurrentBonusLevel() +
			checkee.EffectsOfType<ICheckBonusEffect>()
			       .Where(x => x.AppliesToCheck(Type))
			       .ToList()
			       .Sum(x => x.CheckBonus) +
			checkee.Merits
			       .OfType<ICheckBonusMerit>()
			       .Where(x => x.Applies(checkee, target))
			       .Sum(x => x.CheckBonus(checkeeAsCharacter, Type)) +
			externalBonus;

		var roll = Dice.Roll(1, 20);

		var outcome1 = GetOutcome(roll,
			Modifiers[difficulty1] - TargetNumberExpression.EvaluateWith(checkee, trait, values: customParameters) -
			bonus);
		var outcome2 = GetOutcome(roll,
			Modifiers[difficulty1] - TargetNumberExpression.EvaluateWith(checkee, trait, values: customParameters) -
			bonus);

		return Tuple.Create(HandleStandardCheck(checkee, target, outcome1, difficulty1, trait, true, traitUseType),
			HandleStandardCheck(checkee, target, outcome2, difficulty2, trait, true, traitUseType));
	}

	public override CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty,
		IPerceivable target = null, IUseTrait tool = null, double externalBonus = 0.0,
		TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		return Check(checkee, difficulty, tool?.Trait, target, externalBonus, traitUseType, customParameters);
	}

	public override double TargetNumber(IPerceivableHaveTraits checkee, Difficulty difficulty, ITraitDefinition trait,
		IPerceivable target = null, double externalBonus = 0.0,
		params (string Parameter, object value)[] customParameters)
	{
		var checkeeAsCharacter = checkee as ICharacter ?? (checkee as IBody)?.Actor;
		// Final difficulty
		var bonus =
			checkee.GetCurrentBonusLevel() +
			checkee.EffectsOfType<ICheckBonusEffect>()
			       .Where(x => x.AppliesToCheck(Type))
			       .ToList()
			       .Sum(x => x.CheckBonus) +
			checkee.Merits
			       .OfType<ICheckBonusMerit>()
			       .Where(x => x.Applies(checkee, target))
			       .Sum(x => x.CheckBonus(checkeeAsCharacter, Type)) +
			externalBonus;

		return Modifiers[difficulty] - TargetNumberExpression.EvaluateWith(checkee, trait, values: customParameters) -
		       bonus;
	}

	// For the OGL roll, the TN is set by the Difficulty. The "Target Number" of the Check is actually what formula to add to your 1d20 roll, which is then subtracted from the difficulty.
	public override CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty,
		ITraitDefinition trait,
		IPerceivable target = null, double externalBonus = 0.0,
		TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		var checkeeAsCharacter = checkee as ICharacter ?? (checkee as IBody)?.Actor;

		// Final difficulty
		var bonus =
			checkee.GetCurrentBonusLevel() +
			checkee.EffectsOfType<ICheckBonusEffect>()
			       .Where(x => x.AppliesToCheck(Type))
			       .ToList()
			       .Sum(x => x.CheckBonus) +
			checkee.Merits
			       .OfType<ICheckBonusMerit>()
			       .Where(x => x.Applies(checkee, target))
			       .Sum(x => x.CheckBonus(checkeeAsCharacter, Type)) +
			externalBonus;

		// TODO: allowing for arbitrary DCs i.e. attacking someone.
		var outcome = RollAgainst(Modifiers[difficulty] -
		                          TargetNumberExpression.EvaluateWith(checkee, trait, values: customParameters) -
		                          bonus);

		return HandleStandardCheck(checkee, target, outcome, difficulty, trait, true, traitUseType);
	}

	protected override Outcome RollAgainst(double target)
	{
		return GetOutcome(Dice.Roll(1, 20), target);
	}

	protected Outcome GetOutcome(int roll, double target)
	{
		switch (roll)
		{
			case 1:
				return Outcome.MajorFail;
			case 20:
				return Outcome.MajorPass;
			default:
				if (roll >= target)
				{
					return Outcome.Pass;
				}
				else
				{
					return Outcome.Fail;
				}
		}
	}

	#endregion
}
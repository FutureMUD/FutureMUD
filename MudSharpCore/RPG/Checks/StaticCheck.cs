using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Checks;

public class StaticCheck : FrameworkItem, ICheck
{
	protected Outcome GetOutcome(int successes)
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

	public StaticCheck(Models.Check check, IFuturemud game)
	{
		Gameworld = game;
		_id = check.Type + 1;
		TargetNumberExpression = game.TraitExpressions.Get(check.TraitExpressionId);
		Type = (CheckType)check.Type;
		FailIfTraitMissing = (FailIfTraitMissingType)check.CheckTemplate.FailIfTraitMissingMode;
		foreach (var difficulty in check.CheckTemplate.CheckTemplateDifficulties)
		{
			Modifiers.Add((Difficulty)difficulty.Difficulty, difficulty.Modifier);
		}

		CheckTemplateName = check.CheckTemplate.Name;
	}

	protected readonly Dictionary<Difficulty, double> Modifiers = new();

	#region Overrides of Item

	public override string FrameworkItemType => "Check";

	#endregion

	#region Implementation of IHaveFuturemud

	public IFuturemud Gameworld { get; set; }

	#endregion

	#region Implementation of ICheck

	public string CheckTemplateName { get; set; }
	public CheckType Type { get; set; }
	public bool ImproveTraits => false;

	public bool CanTraitBranchIfMissing => false;

	public CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty, IPerceivable target = null,
		IUseTrait tool = null, double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		return Check(checkee, difficulty, tool?.Trait, target, externalBonus, traitUseType, customParameters);
	}

	public double TargetNumber(IPerceivableHaveTraits checkee, Difficulty difficulty, ITraitDefinition trait,
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

		difficulty = bonus > 0
			? difficulty.StageDown((int)(bonus / StandardCheck.BonusesPerDifficultyLevel))
			: difficulty.StageUp((int)(-1 * bonus / StandardCheck.BonusesPerDifficultyLevel));

		// Roll versus target number w/difficulty mods, store outcome
		return TargetNumberExpression.EvaluateWith(checkee, trait, values: customParameters) + Modifiers[difficulty];
	}

	public CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty, ITraitDefinition trait,
		IPerceivable target = null, double externalBonus = 0.0,
		TraitUseType traitUseType = TraitUseType.Theoretical,
		params (string Parameter, object value)[] customParameters)
	{
		if (TargetNumberExpression.Parameters.Any() &&
		    ((FailIfTraitMissing == FailIfTraitMissingType.FailIfAnyMissing &&
		      TargetNumberExpression.Parameters.Values.Any(x => !checkee.HasTrait(x.Trait))) ||
		     (FailIfTraitMissing == FailIfTraitMissingType.FailIfAllMissing &&
		      TargetNumberExpression.Parameters.Values.All(x => !checkee.HasTrait(x.Trait)))))
		{
			return new CheckOutcome { Outcome = Outcome.MajorFail };
		}

		if (trait != null && FailIfTraitMissing != FailIfTraitMissingType.DoNotFail && !checkee.HasTrait(trait))
		{
			return new CheckOutcome { Outcome = Outcome.MajorFail };
		}

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

		difficulty = bonus > 0
			? difficulty.StageDown((int)(bonus / StandardCheck.BonusesPerDifficultyLevel))
			: difficulty.StageUp((int)(-1 * bonus / StandardCheck.BonusesPerDifficultyLevel));

		// Roll versus target number w/difficulty mods, store outcome
		var tnexp = TargetNumberExpression.EvaluateWith(checkee, trait, values: customParameters);
		if (tnexp + Modifiers[difficulty] >= 100.0)
		{
			if (tnexp + Modifiers[difficulty.StageUp(1)] >= 100.0)
			{
				if (tnexp + Modifiers[difficulty.StageUp(2)] >= 100.0)
				{
					return new CheckOutcome { Outcome = Outcome.MajorPass };
				}

				return new CheckOutcome { Outcome = Outcome.Pass };
			}

			return new CheckOutcome { Outcome = Outcome.MinorPass };
		}

		if (tnexp + Modifiers[difficulty.StageDown(1)] >= 100.0)
		{
			if (tnexp + Modifiers[difficulty.StageDown(2)] >= 100.0)
			{
				return new CheckOutcome { Outcome = Outcome.MinorFail };
			}

			return new CheckOutcome { Outcome = Outcome.Fail };
		}

		return new CheckOutcome { Outcome = Outcome.MajorFail };
	}

	private static Difficulty[] _allDifficulties =
	{
		Difficulty.Automatic,
		Difficulty.Trivial,
		Difficulty.ExtremelyEasy,
		Difficulty.VeryEasy,
		Difficulty.Easy,
		Difficulty.Normal,
		Difficulty.Hard,
		Difficulty.VeryHard,
		Difficulty.ExtremelyHard,
		Difficulty.Insane,
		Difficulty.Impossible
	};

	public virtual Dictionary<Difficulty, CheckOutcome> CheckAgainstAllDifficulties(IPerceivableHaveTraits checkee,
		Difficulty referenceDifficulty, ITraitDefinition trait,
		IPerceivable target = null, double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		var results = new Dictionary<Difficulty, CheckOutcome>();

		var checkeeAsCharacter = checkee as ICharacter ?? (checkee as IBody)?.Actor;

		// Final difficulty
		var bonuses = new List<Tuple<string, double>>();

		var innateBonus = checkee.GetCurrentBonusLevel();
		if (innateBonus != 0)
		{
			bonuses.Add(Tuple.Create("innate", innateBonus));
		}

		if (externalBonus != 0)
		{
			bonuses.Add(Tuple.Create("external", externalBonus));
		}

		bonuses.AddRange(checkee.EffectsOfType<ICheckBonusEffect>()
		                        .Where(x => x.AppliesToCheck(Type))
		                        .ToList()
		                        .Select(x => Tuple.Create(x.ToString(), x.CheckBonus)));

		bonuses.AddRange(checkee.Merits.OfType<ICheckBonusMerit>()
		                        .Where(x => x.Applies(checkee, target))
		                        .Select(x => Tuple.Create(x.ToString(), x.CheckBonus(checkeeAsCharacter, Type))));

		var bonus = bonuses.Sum(x => x.Item2);
		// Roll versus target number w/difficulty mods, store outcome
		var targetNumber = TargetNumberExpression.EvaluateWith(checkee, trait, values: customParameters);
		foreach (var evaluatedDifficulty in _allDifficulties)
		{
			var originalDifficulty = evaluatedDifficulty;
			var originalModifier = Modifiers[originalDifficulty];
			var difficulty = bonus > 0
				? originalDifficulty.StageDown((int)(bonus / StandardCheck.BonusesPerDifficultyLevel))
				: originalDifficulty.StageUp((int)(-1 * bonus / StandardCheck.BonusesPerDifficultyLevel));

			var difficultyplusburden = difficulty;

			var burden = 0;
			if (checkeeAsCharacter != null && Type.IsOffensiveCombatAction())
			{
				burden = checkeeAsCharacter.CombatBurdenOffense;
				difficultyplusburden = difficultyplusburden.StageUp(burden);
			}
			else if (checkeeAsCharacter != null && Type.IsDefensiveCombatAction())
			{
				burden = checkeeAsCharacter.CombatBurdenDefense;
				difficultyplusburden = difficultyplusburden.StageUp(burden);
			}


			var difficultyModifier = Modifiers[difficultyplusburden];
			var result = targetNumber + difficultyModifier >= 100.0
				? new CheckOutcome() { Outcome = Outcome.MinorPass }
				: new
					CheckOutcome { Outcome = Outcome.MinorFail };

			result.CheckTemplateName = CheckTemplateName;
			result.CheckType = Type;
			result.FinalBonus = bonus;
			result.ActiveBonuses = bonuses;
			result.OriginalDifficulty = originalDifficulty;
			result.OriginalDifficultyModifier = originalModifier;
			result.FinalDifficulty = difficultyplusburden;
			result.FinalDifficultyModifier = difficultyModifier;
			result.Burden = burden;
			result.TargetNumber = targetNumber + difficultyModifier;

			results[evaluatedDifficulty] = result;
		}

		return results;
	}

	public Tuple<CheckOutcome, CheckOutcome> MultiDifficultyCheck(IPerceivableHaveTraits checkee,
		Difficulty difficulty1, Difficulty difficulty2,
		IPerceivable target = null, ITraitDefinition tool = null, double externalBonus = 0.0,
		TraitUseType traitUseType = TraitUseType.Practical, params (string Parameter, object value)[] customParameters)
	{
		return Tuple.Create(Check(checkee, difficulty1, tool, target, externalBonus, traitUseType, customParameters),
			Check(checkee, difficulty2, tool, target, externalBonus, traitUseType, customParameters));
	}

	#endregion

	public FailIfTraitMissingType FailIfTraitMissing { get; protected set; }

	/// <summary>
	///     A TraitExpression representing the Target Number of the check
	/// </summary>
	public ITraitExpression TargetNumberExpression { get; protected set; }

	public Difficulty MaximumDifficultyForImprovement => Difficulty.Impossible;

	public bool WouldBeAbjectFailure(IPerceivableHaveTraits checkee, ITraitDefinition trait = null)
	{
		return false;
	}
}
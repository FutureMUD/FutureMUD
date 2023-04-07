using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Logging;
using MudSharp.RPG.Merits.Interfaces;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp.RPG.Checks;

public class StandardCheck : FrameworkItem, ICheck
{
	private static int _bonusesPerDifficultyLevel;

	/// <summary>
	///     A Dictionary of Difficulty Modifiers to the roll
	/// </summary>
	protected readonly Dictionary<Difficulty, double> Modifiers = new();

	public StandardCheck(Check check, IFuturemud game)
	{
		Gameworld = game;
		_id = check.Type + 1;
		TargetNumberExpression = game.TraitExpressions.Get(check.TraitExpressionId);
		Type = (CheckType)check.Type;
		ImproveTraits = check.CheckTemplate.ImproveTraits;
		FailIfTraitMissing = (FailIfTraitMissingType)check.CheckTemplate.FailIfTraitMissingMode;
		CanTraitBranchIfMissing = check.CheckTemplate.CanBranchIfTraitMissing;
		CheckTemplateName = check.CheckTemplate.Name;
		foreach (var difficulty in check.CheckTemplate.CheckTemplateDifficulties)
		{
			Modifiers.Add((Difficulty)difficulty.Difficulty, difficulty.Modifier);
		}

		foreach (var difficulty in Enum.GetValues(typeof(Difficulty)).OfType<Difficulty>())
		{
			if (!Modifiers.ContainsKey(difficulty))
			{
				Console.WriteLine(
					$"Critical Warning: Check \"{Type}\" Template \"{check.CheckTemplate.Name}\" is missing Difficulty for {difficulty.Describe()}.");
				Modifiers[difficulty] = 0;
			}
		}

		MaximumDifficultyForImprovement = (Difficulty)check.MaximumDifficultyForImprovement;
	}

	public static int BonusesPerDifficultyLevel
	{
		get
		{
			if (_bonusesPerDifficultyLevel == 0)
			{
				_bonusesPerDifficultyLevel =
					Futuremud.Games.First().GetStaticInt("CheckBonusPerDifficultyLevel");
			}

			return _bonusesPerDifficultyLevel;
		}
	}

	public Difficulty MaximumDifficultyForImprovement { get; protected set; }

	/// <summary>
	///     Whether or not this check should call TraitUsed on any targeted traits
	/// </summary>
	public bool ImproveTraits { get; protected set; }

	public bool CanTraitBranchIfMissing { get; protected set; }

	public FailIfTraitMissingType FailIfTraitMissing { get; protected set; }

	/// <summary>
	///     A TraitExpression representing the Target Number of the check
	/// </summary>
	protected ITraitExpression TargetNumberExpression { get; set; }

	/// <summary>
	///     Name of the Check Template that this check uses
	/// </summary>
	public string CheckTemplateName { get; set; }

	public override string FrameworkItemType => "StandardCheck";

	/// <summary>
	///     The CheckType which is covered by this Check
	/// </summary>
	public CheckType Type { get; protected set; }

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; }

	#endregion

	protected CheckOutcome HandleStandardCheck(IPerceivableHaveTraits checkee, IPerceivable target, Outcome outcome,
		Difficulty difficulty, ITraitDefinition trait, bool permitBranchingAndImprovement = true,
		TraitUseType traitUseType = TraitUseType.Practical)
	{
		var abject = false;
		// Handle checks that specify traits must be possessed by the owner
		if (TargetNumberExpression.Parameters.Any() &&
		    ((FailIfTraitMissing == FailIfTraitMissingType.FailIfAnyMissing &&
		      TargetNumberExpression.Parameters.Values.Any(x => !checkee.HasTrait(x.Trait))) ||
		     (FailIfTraitMissing == FailIfTraitMissingType.FailIfAllMissing &&
		      TargetNumberExpression.Parameters.Values.All(x => !checkee.HasTrait(x.Trait)))))
		{
			outcome = Outcome.MajorFail;
			abject = true;
		}

		if (trait != null && FailIfTraitMissing != FailIfTraitMissingType.DoNotFail && !checkee.HasTrait(trait))
		{
			outcome = Outcome.MajorFail;
			abject = true;
		}

		if (trait != null && !TargetNumberExpression.Formula.Parameters.Any(x => x.Key.EqualTo("variable")))
		{
			trait = null;
		}

		var chTrait = trait != null ? checkee.GetTrait(trait) : null;
		// Handle Trait Improvement if applicable
		var improvedTraits = new List<ITraitDefinition>();
		if (ImproveTraits && permitBranchingAndImprovement)
		{
			foreach (
				var tr in TargetNumberExpression.Parameters.Values.Where(x => x.CanImprove)
				                                .Select(x => checkee.GetTrait(x.Trait)).Where(tr => tr != null))
			{
				if (tr.TraitUsed(checkee, outcome, difficulty.Lowest(MaximumDifficultyForImprovement), traitUseType))
				{
					improvedTraits.Add(tr.Definition);
				}
			}

			if (chTrait?.TraitUsed(checkee, outcome, difficulty.Lowest(MaximumDifficultyForImprovement),
				    traitUseType) ?? false)
			{
				improvedTraits.Add(chTrait.Definition);
			}
		}

		if (!CanTraitBranchIfMissing ||
		    (TargetNumberExpression.Parameters.Values.All(x => checkee.HasTrait(x.Trait)) &&
		     (trait == null || chTrait != null)))
		{
			return new CheckOutcome
			{
				IsAbjectFailure = abject,
				AcquiredTraits = Enumerable.Empty<ITraitDefinition>(),
				ImprovedTraits = improvedTraits,
				Outcome = outcome
			};
		}

		var checkeeAsCharacter = checkee as ICharacter ?? (checkee as IBody)?.Actor;
		var newTraits = new List<ITraitDefinition>();
		if (permitBranchingAndImprovement)
		{
			foreach (var item in TargetNumberExpression.Parameters.Values.Where(
				         x => x.CanBranch && !checkee.HasTrait(x.Trait)))
			{
				var itemAsSkill = item.Trait as ISkillDefinition;
				if (itemAsSkill?.CanLearn(checkeeAsCharacter) != true)
				{
					continue;
				}

				var branchCheckOutcome = Gameworld.GetCheck(CheckType.TraitBranchCheck)
				                                  .Check(checkee, difficulty, item.Trait, target);
				var branchOutcome = branchCheckOutcome.Outcome;
				Gameworld.LogManager.CustomLogEntry(LogEntryType.SkillBranch, checkee, itemAsSkill,
					branchCheckOutcome);
				if (branchOutcome.IsPass())
				{
					checkee.AddTrait(
						item.Trait,
						branchOutcome.SuccessDegrees() *
						Gameworld.GetStaticDouble("SkillBranchBaseValue")); // TODO - opening value soft coded
					checkee.RemoveAllEffects(x =>
						x.GetSubtype<IncreasedBranchChance>()?.BranchSkill(itemAsSkill) == true);
					newTraits.Add(item.Trait);
				}
			}

			if (trait != null && chTrait == null)
			{
				var itemAsSkill = trait as ISkillDefinition;
				if (itemAsSkill?.CanLearn(checkeeAsCharacter) != true)
				{
					return new CheckOutcome
					{
						IsAbjectFailure = abject,
						AcquiredTraits = newTraits,
						ImprovedTraits = improvedTraits,
						Outcome = outcome
					};
				}

				var branchCheckOutcome = Gameworld.GetCheck(CheckType.TraitBranchCheck)
				                                  .Check(checkee, difficulty, itemAsSkill, target);
				var branchOutcome = branchCheckOutcome.Outcome;
				Gameworld.LogManager.CustomLogEntry(LogEntryType.SkillBranch, checkee, itemAsSkill,
					branchCheckOutcome);
				if (!branchOutcome.IsPass())
				{
					return new CheckOutcome
					{
						IsAbjectFailure = abject,
						AcquiredTraits = newTraits,
						ImprovedTraits = improvedTraits,
						Outcome = outcome
					};
				}

				checkee.AddTrait(
					trait, branchOutcome.SuccessDegrees() * Gameworld.GetStaticDouble("SkillBranchBaseValue"));
				newTraits.Add(trait);
				checkee.RemoveAllEffects(x => x.GetSubtype<IncreasedBranchChance>()?.BranchSkill(itemAsSkill) == true);
			}
		}

		return new CheckOutcome
		{
			IsAbjectFailure = abject,
			AcquiredTraits = newTraits,
			ImprovedTraits = improvedTraits,
			Outcome = outcome
		};
	}

	public virtual bool WouldBeAbjectFailure(IPerceivableHaveTraits checkee, ITraitDefinition trait)
	{
		if (TargetNumberExpression.Parameters.Any() &&
		    ((FailIfTraitMissing == FailIfTraitMissingType.FailIfAnyMissing &&
		      TargetNumberExpression.Parameters.Values.Any(x => !checkee.HasTrait(x.Trait))) ||
		     (FailIfTraitMissing == FailIfTraitMissingType.FailIfAllMissing &&
		      TargetNumberExpression.Parameters.Values.All(x => !checkee.HasTrait(x.Trait)))))
		{
			return true;
		}

		if (trait != null && FailIfTraitMissing != FailIfTraitMissingType.DoNotFail &&
		    !checkee.HasTrait(trait))
		{
			return true;
		}

		return false;
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

	public static ICheck LoadCheck(Check check, IFuturemud game)
	{
		switch (check.CheckTemplate.CheckMethod)
		{
			case "Standard":
				return new StandardCheck(check, game);
			case "OGL":
				return new OGLCheck(check, game);
			case "Timebound":
				return new TimeboundCheck(check, game);
			case "Limb":
				return new LimbCheck(check, game);
			case "Static":
				return new StaticCheck(check, game);
			case "Branch":
				return new BranchCheck(check, game);
			case "PassivePerception":
			case "Passive Perception":
				return new PassivePerceptionCheck(check, game);
			case "BonusAbsent":
				return new BonusAbsentCheck(check, game);
			default:
				return null;
		}
	}

	#region ICheck Members

	public static Difficulty[] AllDifficulties =
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
			var difficulty = bonus > 0
				? originalDifficulty.StageDown((int)(bonus / BonusesPerDifficultyLevel))
				: originalDifficulty.StageUp((int)(-1 * bonus / BonusesPerDifficultyLevel));

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
			var successes = RandomUtilities.EvaluateConsecutiveSuccesses(rolls, targetNumber + difficultyModifier);
			var outcome = GetOutcome(successes);
			var result = HandleStandardCheck(checkee, target, outcome, difficulty, trait,
				evaluatedDifficulty == referenceDifficulty, traitUseType);

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
			result.Rolls = rolls;

			results[evaluatedDifficulty] = result;
		}

		return results;
	}

	public virtual Tuple<CheckOutcome, CheckOutcome> MultiDifficultyCheck(IPerceivableHaveTraits checkee,
		Difficulty difficulty1, Difficulty difficulty2, IPerceivable target = null, ITraitDefinition trait = null,
		double externalBonus = 0.0, TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
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

		var oldBonus =
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

#if DEBUG
		if (oldBonus != bonus)
		{
			throw new ApplicationException("New bonus math doesn't match old.");
		}
#endif
		var originalDifficulty1 = difficulty1;
		var originalDifficultyModifier1 = Modifiers[originalDifficulty1];
		var originalDifficulty2 = difficulty2;
		var originalDifficultyModifier2 = Modifiers[originalDifficulty2];
		difficulty1 = bonus > 0
			? difficulty1.StageDown((int)(bonus / BonusesPerDifficultyLevel))
			: difficulty1.StageUp((int)(-1 * bonus / BonusesPerDifficultyLevel));

		difficulty2 = bonus > 0
			? difficulty2.StageDown((int)(bonus / BonusesPerDifficultyLevel))
			: difficulty2.StageUp((int)(-1 * bonus / BonusesPerDifficultyLevel));

		var difficulty1plusburden = difficulty1;
		var difficulty2plusburden = difficulty2;

		var burden = 0;
		if (checkeeAsCharacter != null && Type.IsOffensiveCombatAction())
		{
			burden = checkeeAsCharacter.CombatBurdenOffense;
			difficulty1plusburden = difficulty1plusburden.StageUp(burden);
			difficulty2plusburden = difficulty2plusburden.StageUp(burden);
		}
		else if (checkeeAsCharacter != null && Type.IsDefensiveCombatAction())
		{
			burden = checkeeAsCharacter.CombatBurdenDefense;
			difficulty1plusburden = difficulty1plusburden.StageUp(burden);
			difficulty2plusburden = difficulty2plusburden.StageUp(burden);
		}

		var rolls = new[]
		{
			Constants.Random.NextDouble() * 100.0,
			Constants.Random.NextDouble() * 100.0,
			Constants.Random.NextDouble() * 100.0
		};

		var basetarget = TargetNumberExpression.EvaluateWith(checkee, trait, values: customParameters);
		var difficultyModifier1 = Modifiers[difficulty1plusburden];
		var difficultyModifier2 = Modifiers[difficulty2plusburden];

		var successes1 = RandomUtilities.EvaluateConsecutiveSuccesses(rolls, basetarget + difficultyModifier1);

		var successes2 = RandomUtilities.EvaluateConsecutiveSuccesses(rolls, basetarget + difficultyModifier2);
		var outcome1 = GetOutcome(successes1);
		var outcome2 = GetOutcome(successes2);

		var result1 = HandleStandardCheck(checkee, target, outcome1, difficulty1, trait, true, traitUseType);
		var result2 = HandleStandardCheck(checkee, target, outcome2, difficulty2, trait, true, traitUseType);

		result1.CheckTemplateName = CheckTemplateName;
		result1.CheckType = Type;
		result1.FinalBonus = bonus;
		result1.ActiveBonuses = bonuses;
		result1.OriginalDifficulty = originalDifficulty1;
		result1.OriginalDifficultyModifier = originalDifficultyModifier1;
		result1.FinalDifficulty = difficulty1plusburden;
		result1.FinalDifficultyModifier = difficultyModifier1;
		result1.Burden = burden;
		result1.TargetNumber = basetarget + difficultyModifier1;
		result1.Rolls = rolls.Select(x => (double)x).ToList();

		result2.CheckTemplateName = CheckTemplateName;
		result2.CheckType = Type;
		result2.FinalBonus = bonus;
		result2.ActiveBonuses = bonuses;
		result2.OriginalDifficulty = originalDifficulty2;
		result2.OriginalDifficultyModifier = originalDifficultyModifier2;
		result2.FinalDifficulty = difficulty2plusburden;
		result2.FinalDifficultyModifier = difficultyModifier2;
		result2.Burden = burden;
		result2.TargetNumber = basetarget + difficultyModifier2;
		result2.Rolls = rolls.Select(x => (double)x).ToList();

		return Tuple.Create(result1, result2);
	}

	public virtual CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty,
		IPerceivable target = null, IUseTrait tool = null, double externalBonus = 0.0,
		TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
		return Check(checkee, difficulty, tool?.Trait, target, externalBonus, traitUseType, customParameters);
	}

	public virtual double TargetNumber(IPerceivableHaveTraits checkee, Difficulty difficulty,
		ITraitDefinition trait,
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
			? difficulty.StageDown((int)(bonus / BonusesPerDifficultyLevel))
			: difficulty.StageUp((int)(-1 * bonus / BonusesPerDifficultyLevel));

		var difficultyplusburden = difficulty;

		if (checkeeAsCharacter != null && Type.IsOffensiveCombatAction())
		{
			difficultyplusburden = difficultyplusburden.StageUp(checkeeAsCharacter.CombatBurdenOffense);
		}
		else if (checkeeAsCharacter != null && Type.IsDefensiveCombatAction())
		{
			difficultyplusburden = difficultyplusburden.StageUp(checkeeAsCharacter.CombatBurdenDefense);
		}

		return TargetNumberExpression.EvaluateWith(checkee, trait, values: customParameters) +
		       Modifiers[difficultyplusburden];
	}

	public virtual CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty, ITraitDefinition trait,
		IPerceivable target = null, double externalBonus = 0.0,
		TraitUseType traitUseType = TraitUseType.Practical,
		params (string Parameter, object value)[] customParameters)
	{
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

		if (Type.IsVisionInfluencedCheck())
		{
			var visionPercentage = checkeeAsCharacter.VisionPercentage;
			if (visionPercentage < 1.0)
			{
				bonuses.Add(Tuple.Create("vision",
					Gameworld.GetStaticDouble("BlindnessCheckBonus") * (1.0 - visionPercentage)));
			}
		}

		var bonus = bonuses.Sum(x => x.Item2);

		var originalDifficulty = difficulty;
		var originalModifier = Modifiers[originalDifficulty];
		difficulty = bonus > 0
			? difficulty.StageDown((int)(bonus / BonusesPerDifficultyLevel))
			: difficulty.StageUp((int)(-1 * bonus / BonusesPerDifficultyLevel));

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

		// Roll versus target number w/difficulty mods, store outcome
		var targetNumber = TargetNumberExpression.EvaluateWith(checkee, trait, values: customParameters);
		var difficultyModifier = Modifiers[difficultyplusburden];
		var outcome = RollAgainst(targetNumber + difficultyModifier, out var rolls);

		var result = HandleStandardCheck(checkee, target, outcome, difficulty, trait, true, traitUseType);

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
		result.Rolls = rolls;

		return result;
	}

	#endregion
}

public static class StandardCheckExtensions
{
	public static Difficulty ApplyBonus(this Difficulty difficulty, double bonus)
	{
		return difficulty.StageDown((int)(bonus / StandardCheck.BonusesPerDifficultyLevel));
	}
}
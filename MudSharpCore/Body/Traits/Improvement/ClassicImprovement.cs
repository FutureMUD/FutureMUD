using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ExpressionEngine;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Body.Traits.Improvement;

/// <summary>
///     Classic Improvement has a fixed % chance on failure to improve, and improves by LV each time it does so
/// </summary>
public class ClassicImprovement : ImprovementModel
{
	// Todo - proper constructor
	public ClassicImprovement(double chance, string expression, Dictionary<string, ITraitDefinition> lvparam)
	{
		ImprovementChance = chance;
		ImprovementExpression = new TraitExpression(new Expression(expression), lvparam);
	}

	public ClassicImprovement(Improver improver)
	{
		_id = improver.Id;
		_name = improver.Name;
		LoadFromXml(XElement.Parse(improver.Definition));
	}

	public double ImprovementChance { get; set; }

	public TraitExpression ImprovementExpression { get; set; }

	public string NoGainSecondsDiceExpression { get; set; }

	public bool ImproveOnFail { get; set; }

	public bool ImproveOnSuccess { get; set; }

	public double DifficultyThresholdInterval { get; set; }

	private Difficulty ThresholdDifficulty(double value)
	{
		if (DifficultyThresholdInterval == 0)
		{
			return Difficulty.Automatic;
		}

		var ratio = (int)(value / DifficultyThresholdInterval);
		if (ratio < 0)
		{
			return Difficulty.Automatic;
		}

		if (ratio > 10)
		{
			return Difficulty.Impossible;
		}

		return (Difficulty)ratio;
	}

	/// <inheritdoc />
	public override bool CanImprove(IHaveTraits person, ITrait trait, Difficulty difficulty, TraitUseType useType,
		bool ignoreTemporaryBlockers)
	{
		if (!ignoreTemporaryBlockers && person is IHaveEffects ihe && ihe.AffectedBy<INoTraitGainEffect>(trait.Definition))
		{
			return false;
		}

		if (trait is null)
		{
			return true;
		}

		if (trait.Value >= person.TraitMaxValue(trait.Definition))
		{
			return false;
		}

		if (ThresholdDifficulty(trait.RawValue) > difficulty)
		{
			return false;
		}

		return true;
	}

	public override double GetImprovement(IHaveTraits person, ITrait trait, Difficulty difficulty, Outcome outcome,
		TraitUseType usetype)
	{
		if (person is IHaveEffects ihe && ihe.AffectedBy<INoTraitGainEffect>(trait.Definition))
		{
			trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
				"-- NoGain [NoTraitGain Effect]");
			return 0.0;
		}

		if (!ImproveOnSuccess && outcome.IsPass())
		{
			trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
				"-- NoGain [No Improvement on Success]");
			return 0.0;
		}

		if (!ImproveOnFail && outcome.IsFail())
		{
			trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
				"-- NoGain [No Improvement on Failure]");
			return 0.0;
		}

		if (trait.Value >= person.TraitMaxValue(trait.Definition))
		{
			trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
				"-- NoGain [Trait above maximum value]");
			return 0.0;
		}

		var chance = ImprovementChance;
		if (person is IPerceivableHaveTraits ipht)
		{
			chance *= ipht.Merits
			              .OfType<ITraitLearningMerit>()
			              .Where(x => x.Applies(ipht))
			              .Select(x =>
				              x.SkillLearningChanceModifier(ipht, trait.Definition, outcome, difficulty, usetype))
			              .Aggregate(1.0, (accum, mult) => accum * mult);
		}

		if (Constants.Random.NextDouble() > chance)
		{
			trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
				$"-- NoGain [Failed roll {chance:P4} chance]");
			return 0.0;
		}

		if (ThresholdDifficulty(trait.RawValue) > difficulty)
		{
			trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
				$"-- NoGain [Difficulty below {ThresholdDifficulty(trait.RawValue)} threshold]");
			return 0.0;
		}

		var noGainTimespan = TimeSpan.FromSeconds(Dice.Roll(NoGainSecondsDiceExpression));
		if (person is IHaveEffects phe && noGainTimespan.TotalSeconds > 0)
		{
			phe.AddEffect(new NoTraitGain((IPerceivable)person, trait.Definition), noGainTimespan);
		}

		var gain = Dice.Roll(1, (int)Math.Max(1.0, ImprovementExpression.Evaluate(person, trait.Definition)) * 10) /
		           10.0;
		trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement, $"-- Skill Gain of {gain:N4}");
		return gain;
	}

	private void LoadFromXml(XElement root)
	{
		ImprovementChance = Convert.ToDouble(root.Attribute("Chance").Value);
		ImprovementExpression =
			new TraitExpression(new Expression(root.Attribute("Expression").Value),
				new Dictionary<string, ITraitDefinition>());
		ImproveOnFail = root.Attribute("ImproveOnFail") != null
			? bool.Parse(root.Attribute("ImproveOnFail").Value)
			: true;
		ImproveOnSuccess = root.Attribute("ImproveOnSuccess") != null
			? bool.Parse(root.Attribute("ImproveOnSuccess").Value)
			: true;
		DifficultyThresholdInterval = root.Attribute("DifficultyThresholdInterval") != null
			? double.Parse(root.Attribute("DifficultyThresholdInterval").Value)
			: 0.0;
		NoGainSecondsDiceExpression = root.Attribute("NoGainSecondsDiceExpression") != null
			? root.Attribute("NoGainSecondsDiceExpression").Value
			: "0";
	}
}
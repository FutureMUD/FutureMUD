using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using ExpressionEngine;
using MudSharp.Effects;

namespace MudSharp.Body.Traits.Improvement;

public class TheoreticalImprovementModel : ImprovementModel
{
	// Todo - proper constructor
	public TheoreticalImprovementModel(double chance, string expression, Dictionary<string, ITraitDefinition> lvparam)
	{
		ImprovementChance = chance;
		ImprovementExpression = new Expression(expression);
	}

	public TheoreticalImprovementModel(Improver improver)
	{
		_id = improver.Id;
		_name = improver.Name;
		LoadFromXml(XElement.Parse(improver.Definition));
	}

	public double ImprovementChance { get; set; }

	public Expression ImprovementExpression { get; set; }

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

	#region Overrides of ImprovementModel

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

		if (ThresholdDifficulty(trait.RawValue) > difficulty)
		{
			return false;
		}

		switch (useType)
		{
			case TraitUseType.Practical:
				if (((trait as TheoreticalSkill)?.PracticalValue ?? trait.Value) > person.TraitMaxValue(trait))
				{
					return false;
				}

				break;
			case TraitUseType.Theoretical:
				if (((trait as TheoreticalSkill)?.TheoreticalValue ?? trait.Value) > person.TraitMaxValue(trait))
				{
					return false;
				}

				break;
		}

		return true;
	}

	#endregion

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

		switch (usetype)
		{
			case TraitUseType.Practical:
				if (((trait as TheoreticalSkill)?.PracticalValue ?? trait.Value) > person.TraitMaxValue(trait))
				{
					trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
						"-- NoGain [Practical above maximum value]");
					return 0.0;
				}

				break;
			case TraitUseType.Theoretical:
				if (((trait as TheoreticalSkill)?.TheoreticalValue ?? trait.Value) > person.TraitMaxValue(trait))
				{
					trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
						"-- NoGain [Theoretical above maximum value]");
					return 0.0;
				}

				break;
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

		switch (usetype)
		{
			case TraitUseType.Practical:
				if (ThresholdDifficulty((trait as TheoreticalSkill)?.PracticalValue ??
				                        trait.Value) > difficulty)
				{
					trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
						$"-- NoGain [Difficulty below practical {ThresholdDifficulty(trait.RawValue)} threshold]");
					return 0.0;
				}

				break;
			case TraitUseType.Theoretical:
				if (ThresholdDifficulty((trait as TheoreticalSkill)?.TheoreticalValue ??
				                        trait.Value) > difficulty)
				{
					trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
						$"-- NoGain [Difficulty below theoretical {ThresholdDifficulty(trait.RawValue)} threshold]");
					return 0.0;
				}

				break;
		}

		if (ThresholdDifficulty(trait.Value) > difficulty)
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

		switch (usetype)
		{
			case TraitUseType.Practical:
				ImprovementExpression.Parameters["value"] = (trait as TheoreticalSkill)?.PracticalValue ??
				                                            trait.Value;
				break;
			case TraitUseType.Theoretical:
				ImprovementExpression.Parameters["value"] = (trait as TheoreticalSkill)?.TheoreticalValue ??
				                                            trait.Value;
				break;
		}

		var gain = Dice.Roll(1, (int)Math.Max(1.0, (double)ImprovementExpression.Evaluate()) * 10) / 10.0;
		trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement, $"-- Skill Gain of {gain:N4}");
		return gain;
	}

	private void LoadFromXml(XElement root)
	{
		ImprovementChance = Convert.ToDouble(root.Attribute("Chance").Value);
		ImprovementExpression =
			new Expression(root.Attribute("Expression").Value);
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
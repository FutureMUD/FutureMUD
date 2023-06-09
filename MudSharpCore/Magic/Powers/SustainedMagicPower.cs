﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ExpressionEngine;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.Powers;

public abstract class SustainedMagicPower : MagicPowerBase
{
	protected SustainedMagicPower(Models.MagicPower power, IFuturemud gameworld) : base(power, gameworld)
	{
		var root = XElement.Parse(power.Definition);
		var element = root.Element("ConcentrationPointsToSustain");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no ConcentrationPointsToSustain in the definition XML for power {Id} ({Name}).");
		}

		ConcentrationPointsToSustain = double.Parse(element.Value);

		element = root.Element("SustainPenalty");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no SustainPenalty in the definition XML for power {Id} ({Name}).");
		}

		SustainPenalty = double.Parse(element.Value);

		element = root.Element("DetectableWithDetectMagic");
		if (element == null)
		{
			throw new ApplicationException(
				$"There was no DetectableWithDetectMagic in the definition XML for power {Id} ({Name}).");
		}

		DetectableWithDetectMagic = (Difficulty)int.Parse(element.Value);

		element = root.Element("Duration");
		if (element == null)
		{
			_durationExpression = new Expression("-1");
			HasDuration = false;
		}
		else
		{
			HasDuration = true;
			_durationExpression = new Expression(element.Value);
		}

		element = root.Element("SustainResourceCosts");
		if (element == null)
		{
			SustainCostsPerMinute = Enumerable.Empty<(IMagicResource Resource, double Cost)>();
		}
		else
		{
			var list = new List<(IMagicResource Resources, double Cost)>();
			foreach (var sub in element.Elements())
			{
				var which = sub.Attribute("resource")?.Value;
				if (string.IsNullOrWhiteSpace(which))
				{
					throw new ApplicationException(
						$"There was no resource attribute in the SustainResourceCost in the definition XML for power {Id} ({Name}).");
				}

				var resource = long.TryParse(which, out var value)
					? gameworld.MagicResources.Get(value)
					: gameworld.MagicResources.GetByName(which);
				if (resource == null)
				{
					throw new ApplicationException(
						$"Could not load the resource referred to by '{which}' in the SustainResourceCost in the definition XML for power {Id} ({Name}).");
				}

				if (!double.TryParse(sub.Value, out var dvalue))
				{
					throw new ApplicationException(
						$"Could not convert the amount in the SustainResourceCost in the definition XML for power {Id} ({Name}).");
				}

				list.Add((resource, dvalue));
			}

			SustainCostsPerMinute = list;
		}
	}

	public double ConcentrationPointsToSustain { get; protected set; }
	public double SustainPenalty { get; protected set; }
	public Difficulty DetectableWithDetectMagic { get; protected set; }
	public bool HasDuration { get; protected set; }

	public IEnumerable<(IMagicResource Resource, double Cost)> SustainCostsPerMinute { get; }

	public void DoSustainCostsTick(ICharacter actor)
	{
		var remove = false;
		foreach (var (resource, cost) in SustainCostsPerMinute)
		{
			if (!actor.UseResource(resource, cost))
			{
				remove = true;
			}
		}

		if (remove)
		{
			ExpireSustainedEffect(actor);
		}
	}

	protected abstract void ExpireSustainedEffect(ICharacter actor);

	private readonly Expression _durationExpression;

	public TimeSpan GetDuration(int successDegrees)
	{
		if (HasDuration)
		{
			_durationExpression.Parameters["degrees"] = successDegrees;
			return TimeSpan.FromMilliseconds(Convert.ToDouble(_durationExpression.Evaluate()));
		}

		return TimeSpan.MinValue;
	}

	#region Overrides of MagicPowerBase

	public override string ShowHelp(ICharacter voyeur)
	{
		var sb = new StringBuilder(base.ShowHelp(voyeur));
		sb.AppendLine();
		if (SustainCostsPerMinute.Any())
		{
			sb.AppendLine("Costs to Sustain:");
			sb.AppendLine(
				$"\t{SustainCostsPerMinute.Select(x => $"{x.Cost.ToString("N2", voyeur)} {x.Resource.ShortName}".ColourValue()).ListToString()}");
		}

		if (HasDuration)
		{
			sb.AppendLine($"Duration Formula: {_durationExpression.OriginalExpression.ColourCommand()}");
		}

		sb.AppendLine($"Concentration to Sustain: {ConcentrationPointsToSustain.ToString("N2", voyeur).ColourValue()}");
		sb.AppendLine(
			$"Sustain Check Penalty: {(SustainPenalty / Gameworld.GetStaticDouble("CheckBonusPerDifficultyLevel")).ToString("N2", voyeur).ColourValue()} degrees");
		sb.AppendLine($"Detect Difficulty: {DetectableWithDetectMagic.Describe().ColourValue()}");
		return sb.ToString();
	}

	#endregion
}
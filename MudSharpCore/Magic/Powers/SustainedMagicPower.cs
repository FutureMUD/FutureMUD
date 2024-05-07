using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ExpressionEngine;
using MoreLinq.Extensions;
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

    protected XElement SaveSustainedDefinition(XElement root)
    {
        root.Add(new XElement("ConcentrationPointsToSustain", ConcentrationPointsToSustain));
        root.Add(new XElement("SustainPenalty", SustainPenalty));
        root.Add(new XElement("DetectableWithDetectMagic", DetectableWithDetectMagic));
        if (HasDuration)
        {
            root.Add(new XElement("Duration", _durationExpression.OriginalExpression));
        }
        root.Add(new XElement("SustainResourceCosts",
            from item in SustainCostsPerMinute
            select new XElement("Cost",
                new XAttribute("resource", item.Resource.Id),
                item.Cost
            )
        ));
        return root;
    }

    /// <inheritdoc />
    public override string HelpText => $@"You can use the following options with this magic power:

    #3name <name>#0 - renames the magic power
    #3school <which>#0 - sets the school the power belongs to
    #3blurb <blurb>#0 - sets the blurb for power list
    #3can <prog>#0 - sets a prog that controls if the power can be used
    #3why <prog>#0 - sets a prog that controls an error message if prog can't be used
    #3help#0 - drops you into an editor to write the player help file
    #3cost <verb> <which> <number>#0 - sets the cost of using a particular verb
{SubtypeHelpText}";

    /// <inheritdoc />
    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
			case "detect":
                return BuildingCommandDetect(actor, command);
			case "sustainpenalty":
				return BuildingCommandSustainPenalty(actor, command);
			case "concentration":
				return BuildingCommandConcentration(actor, command);
			case "sustaincost":
                return BuildingCommandSustainCost(actor, command);
			case "duration":
				return BuildingCommandDuration(actor, command);
        }
        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandDuration(ICharacter actor, StringStack command)
    {
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a formula for a duration, or #30#0 to last indefinitely.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("0"))
        throw new NotImplementedException();
    }

    private bool BuildingCommandSustainCost(ICharacter actor, StringStack command)
    {
        throw new NotImplementedException();
    }

    private bool BuildingCommandConcentration(ICharacter actor, StringStack command)
    {
        throw new NotImplementedException();
    }

    private bool BuildingCommandSustainPenalty(ICharacter actor, StringStack command)
    {
        throw new NotImplementedException();
    }

    private bool BuildingCommandDetect(ICharacter actor, StringStack command)
    {
        throw new NotImplementedException();
    }

    #endregion
}
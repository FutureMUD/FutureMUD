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
			DurationExpression = new Expression("-1");
			HasDuration = false;
		}
		else
		{
			HasDuration = true;
			DurationExpression = new Expression(element.Value);
		}

		element = root.Element("SustainResourceCosts");
		if (element == null)
		{
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

				_sustainCostsPerMinute.Add((resource, dvalue));
			}
		}
	}

	public double ConcentrationPointsToSustain { get; protected set; }
	public double SustainPenalty { get; protected set; }
	public Difficulty DetectableWithDetectMagic { get; protected set; }
	public bool HasDuration { get; protected set; }

    private readonly List<(IMagicResource Resource, double Cost)> _sustainCostsPerMinute = new();
    public IEnumerable<(IMagicResource Resource, double Cost)> SustainCostsPerMinute => _sustainCostsPerMinute;

	public void DoSustainCostsTick(ICharacter actor, double multiplier = 1.0)
	{
		var remove = false;
		foreach (var (resource, cost) in SustainCostsPerMinute)
		{
			if (!actor.UseResource(resource, cost * multiplier))
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

	private Expression DurationExpression { get; set; }

	public TimeSpan GetDuration(int successDegrees)
	{
		if (HasDuration)
		{
			DurationExpression.Parameters["degrees"] = successDegrees;
			return TimeSpan.FromMilliseconds(Convert.ToDouble(DurationExpression.Evaluate()));
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
			sb.AppendLine($"Duration Formula: {DurationExpression.OriginalExpression.ColourCommand()}");
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
            root.Add(new XElement("Duration", DurationExpression.OriginalExpression));
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
    #3sustaincost <which> <number>#0 - sets the sustain cost per minute of the power
    #3duration <expression>#0 - sets the duration the power will last
    #3concentration <points>#0 - sets the number of concentration points this power occupies
    #3sustainpenalty <degrees>#0 - sets the skill check penalty for sustaining this power
    #3detect <difficulty>#0 - sets the difficulty to detect this power
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
        {
            DurationExpression = new Expression("-1");
            HasDuration = false;
            Changed = true;
            actor.OutputHandler.Send("This power now lasts as long as it is sustained.");
            return true;
        }

        var expression = new Expression(command.SafeRemainingArgument);
        if (expression.HasErrors())
        {
            actor.OutputHandler.Send(expression.Error);
            return false;
        }

        DurationExpression = expression;
        Changed = true;
        actor.OutputHandler.Send($"This power will now last for {expression.OriginalExpression.ColourCommand()} seconds.");
        return true;
    }

    private bool BuildingCommandSustainCost(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"Which magic resource do you want to edit costs for?");
            return false;
        }

        var resource = Gameworld.MagicResources.GetByIdOrName(command.PopSpeech());
        if (resource is null)
        {
            actor.OutputHandler.Send($"There is no magic resource identified by {command.Last.ColourCommand()}.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"How much {resource.Name.ColourValue()} per minute should be consumed to sustain this power?");
            return false;
        }

        if (!double.TryParse(command.SafeRemainingArgument, out var value))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
            return false;
        }

        if (value == 0.0)
        {
            _sustainCostsPerMinute.RemoveAll(x => x.Resource == resource);
            Changed = true;
            actor.OutputHandler.Send($"This power will no longer cost any {resource.Name.ColourValue()} to sustain.");
            return true;
        }

        _sustainCostsPerMinute.RemoveAll(x => x.Resource == resource);
        _sustainCostsPerMinute.Add((resource, value));
        Changed = true;
        actor.OutputHandler.Send($"This power will now cost {value.ToBonusString(actor)} {resource.Name.ColourValue()} per minute to sustain.");
        return true;
    }

    private bool BuildingCommandConcentration(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("How many concentration points should this power consume?");
            return false;
        }

        if (!double.TryParse(command.SafeRemainingArgument, out var value))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
            return false;
        }

        ConcentrationPointsToSustain = value;
        Changed = true;
        actor.OutputHandler.Send($"Sustaining this power will now use up {value.ToBonusString(actor)} concentration point slots.");
        return true;
    }

    private bool BuildingCommandSustainPenalty(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What should be the skill check penalty for sustaining this magical power?");
            return false;
        }

        if (!double.TryParse(command.SafeRemainingArgument, out var value))
        {
            actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
            return false;
        }

        SustainPenalty = value;
        Changed = true;
        actor.OutputHandler.Send($"Sustaining this power will now cause a {value.ToBonusString(actor)} penalty to all skill checks.");
        return true;
    }

    private bool BuildingCommandDetect(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"What difficulty should it be to detect this ongoing power with detect magic abilities?\nHint: See {"show difficulties".MXPSend("show difficulties")} for a list.");
            return false;
        }

        if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty value))
        {
            actor.OutputHandler.Send($"That is not a valid difficulty.\nHint: See {"show difficulties".MXPSend("show difficulties")} for a list.");
            return false;
        }

        DetectableWithDetectMagic = value;
        Changed = true;
        actor.OutputHandler.Send($"This power will now be {value.DescribeColoured()} to detect with detect magic spells and abilities.");
        return true;
    }

    #endregion
}
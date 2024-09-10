using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.RPG.Law.PunishmentStrategies;

internal class PunishmentStrategyJail : PunishmentStrategyBase
{
	public MudTimeSpan MinimumSentenceLength { get; set; }
	public MudTimeSpan MaximumSentenceLength { get; set; }

	public PunishmentStrategyJail(IFuturemud gameworld) : base(gameworld)
	{
		MinimumSentenceLength = MudTimeSpan.FromDays(Gameworld.GetStaticDouble("DefaultMinimumJailDays"));
		MaximumSentenceLength = MudTimeSpan.FromDays(Gameworld.GetStaticDouble("DefaultMaximumJailDays"));
	}

	public PunishmentStrategyJail(IFuturemud gameworld, XElement root) : base(gameworld, root)
	{
		MinimumSentenceLength = MudTimeSpan.Parse(root.Element("Minimum").Value);
		MaximumSentenceLength = MudTimeSpan.Parse(root.Element("Maximum").Value);
	}

	public override string TypeSpecificHelpText => @"
	minimumsentence <length> - sets the minimum custodial sentence
	maximumsentence <length> - sets the maximum custodial sentence";

	public override bool BuildingCommand(ICharacter actor, ILegalAuthority authority, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "minimum":
			case "min":
			case "minsentence":
			case "minimumsentence":
				return BuildingCommandMinimumSentence(actor, command);
			case "maximum":
			case "max":
			case "maxsentence":
			case "maximumsentence":
				return BuildingCommandMaximumSentence(actor, command);
		}

		return base.BuildingCommand(actor, authority, command.GetUndo());
	}

	private bool BuildingCommandMaximumSentence(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the maximum sentence length imposed?");
			return false;
		}

		if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, out var ts))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid time span.");
			return false;
		}

		MaximumSentenceLength = ts;
		if (MinimumSentenceLength < MaximumSentenceLength)
		{
			MinimumSentenceLength = MaximumSentenceLength;
		}

		return true;
	}

	private bool BuildingCommandMinimumSentence(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the minimum sentence length imposed?");
			return false;
		}

		if (!MudTimeSpan.TryParse(command.SafeRemainingArgument, out var ts))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid time span.");
			return false;
		}

		MinimumSentenceLength = ts;
		if (MinimumSentenceLength > MaximumSentenceLength)
		{
			MaximumSentenceLength = MinimumSentenceLength;
		}

		return true;
	}

	public override string Describe(IPerceiver voyeur)
	{
		return MinimumSentenceLength == MaximumSentenceLength
			? $"a custodial prison sentence of {MinimumSentenceLength.Describe(voyeur).ColourValue()}"
			: $"a custodial prison sentence of between {MinimumSentenceLength.Describe(voyeur).ColourValue()} and {MaximumSentenceLength.Describe(voyeur).ColourValue()}";
	}

	public override PunishmentResult GetResult(ICharacter actor, ICrime crime, double severity = 0)
	{
		// TODO - some 
		return new PunishmentResult { CustodialSentence = MinimumSentenceLength + MudTimeSpan.FromDays(severity * (MaximumSentenceLength - MinimumSentenceLength).TotalDays)};
	}

	protected override void SaveSpecificType(XElement root)
	{
		root.Add(new XAttribute("type", "jail"));
		root.Add(new XElement("Minimum", MinimumSentenceLength.GetRoundTripParseText));
		root.Add(new XElement("Maximum", MaximumSentenceLength.GetRoundTripParseText));
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Jail Sentence".ColourName());
		sb.AppendLine($"Minimum Jail Term: {MinimumSentenceLength.Describe(actor).ColourValue()}");
		sb.AppendLine($"Maximum Jail Term: {MaximumSentenceLength.Describe(actor).ColourValue()}");
		BaseShowText(actor, sb);
		return sb.ToString();
	}
}
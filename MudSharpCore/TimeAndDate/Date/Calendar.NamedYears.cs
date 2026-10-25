#nullable enable

using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.TimeAndDate.Date;

public partial class Calendar
{
	public NamedYearCycle? YearCycle { get; private set; }

	private void LoadNamedYearCycle(XElement? element)
	{
		YearCycle = null;
		if (element is null) return;
		if (!int.TryParse(element.Attribute("epochyear")?.Value, NumberStyles.Integer,
		        System.Globalization.CultureInfo.InvariantCulture, out var epoch))
		{
			throw new XmlException("A named year cycle requires an integer epochyear.");
		}

		try
		{
			YearCycle = new NamedYearCycle(element.Attribute("name")?.Value ?? "", epoch,
				element.Elements("year").Select(x => x.Value));
		}
		catch (ArgumentException ex)
		{
			throw new XmlException(ex.Message, ex);
		}
	}

	private XElement? SaveNamedYearCycle() => YearCycle is not { } cycle ? null :
		new XElement("namedyearcycle", new XAttribute("name", cycle.Name),
			new XAttribute("epochyear", cycle.EpochYear), cycle.YearNames.Select(x => new XElement("year", x)));

	private const string YearCycleHelp = @"Named year cycle commands:
	#3yearcycle create <epoch-year> <cycle-name> <year-name> [year-name ...]#0 - use quotes for names containing spaces
	#3yearcycle add <year-name>#0 - appends a year
	#3yearcycle set <number> <year-name>#0 - renames a year
	#3yearcycle remove <number>#0 - removes a year
	#3yearcycle epoch <year>#0 - sets year one of cycle one
	#3yearcycle name <name>#0 - renames the cycle
	#3yearcycle clear#0 - removes the cycle
	#3yearcycle show#0 - lists the ordered year names
Date masks: #3$yn#0 year name, #3$yi#0 year within cycle, #3$ya#0 cycle number, #3$yc#0 cycle name.";

	private bool BuildingCommandYearCycle(ICharacter actor, StringStack command)
	{
		var operation = command.PopForSwitch();
		if (operation == "clear" && command.IsFinished)
		{
			YearCycle = null;
			Changed = true;
			actor.OutputHandler.Send("Named year cycle removed.");
			return true;
		}

		if (operation is "show" or "list" && command.IsFinished)
		{
			actor.OutputHandler.Send(YearCycle is { } shown
				? $"{shown.Name.ColourName()} (epoch {shown.EpochYear.ToStringN0(actor)}):\n" +
				  string.Join("\n", shown.YearNames.Select((x, i) => $"{(i + 1).ToStringN0(actor)}. {x.ColourName()}"))
				: "This calendar has no named year cycle.");
			return true;
		}

		var names = YearCycle?.YearNames.ToList() ?? [];
		var epoch = YearCycle?.EpochYear ?? 1;
		var name = YearCycle?.Name ?? "";
		if (operation == "create")
		{
			if (!int.TryParse(command.PopSpeech(), out epoch)) return YearCycleSyntax(actor);
			name = command.PopSpeech();
			names.Clear();
			while (!command.IsFinished) names.Add(command.PopSpeech());
		}
		else if (YearCycle is null)
		{
			return YearCycleSyntax(actor);
		}
		else
		{
			switch (operation)
			{
				case "add":
					names.Add(command.SafeRemainingArgument);
					break;
				case "set":
				case "remove":
					if (!int.TryParse(command.PopSpeech(), out var index) || index < 1 || index > names.Count)
						return YearCycleSyntax(actor);
					if (operation == "set") names[index - 1] = command.SafeRemainingArgument;
					else
					{
						if (!command.IsFinished) return YearCycleSyntax(actor);
						names.RemoveAt(index - 1);
					}
					break;
				case "epoch":
					if (!int.TryParse(command.PopSpeech(), out epoch) || !command.IsFinished) return YearCycleSyntax(actor);
					break;
				case "name":
					name = command.SafeRemainingArgument;
					break;
				default:
					return YearCycleSyntax(actor);
			}
		}

		try
		{
			var replacement = new NamedYearCycle(name, epoch, names);
			YearCycle = replacement;
			Changed = true;
			actor.OutputHandler.Send($"Named year cycle {name.ColourName()} now contains {names.Count.ToStringN0(actor).ColourValue()} years.");
			return true;
		}
		catch (ArgumentException ex)
		{
			actor.OutputHandler.Send(ex.Message.ColourError());
			return false;
		}
	}

	private static bool YearCycleSyntax(ICharacter actor)
	{
		actor.OutputHandler.Send(YearCycleHelp.SubstituteANSIColour());
		return false;
	}
}

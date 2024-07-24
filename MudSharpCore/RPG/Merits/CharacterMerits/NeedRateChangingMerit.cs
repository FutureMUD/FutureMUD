using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class NeedRateChangingMerit : CharacterMeritBase, INeedRateChangingMerit
{
	protected NeedRateChangingMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		HungerMultiplier = double.Parse(definition.Attribute("hunger")?.Value ?? "1.0");
		ThirstMultiplier = double.Parse(definition.Attribute("thirst")?.Value ?? "1.0");
		DrunkennessMultiplier = double.Parse(definition.Attribute("alcohol")?.Value ?? "1.0");
	}

	protected NeedRateChangingMerit(){}

	protected NeedRateChangingMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Needs Rate Change", "@ have|has an altered needs rate")
	{
		HungerMultiplier = 1.0;
		ThirstMultiplier = 1.0;
		DrunkennessMultiplier = 1.0;
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("hunger", HungerMultiplier));
		root.Add(new XAttribute("thirst", ThirstMultiplier));
		root.Add(new XAttribute("alcohol", DrunkennessMultiplier));
		return root;
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Needs Rate Change",
			(merit, gameworld) => new NeedRateChangingMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Needs Rate Change", (gameworld, name) => new NeedRateChangingMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Needs Rate Change", "Changes hunger/thirst/drunk rates", new NeedRateChangingMerit().HelpText);
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Hunger Rate Multiplier: {HungerMultiplier.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Thirst Rate Multiplier: {ThirstMultiplier.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Drunkenness Rate Multiplier: {DrunkennessMultiplier.ToString("P2", actor).ColourValue()}");
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => @$"{base.SubtypeHelp}
	#3hunger <%>#0 - sets the hunger rate multiplier
	#3thirst <%>#0 - sets the thirst rate multiplier
	#3drunk <%>#0 - sets the drunkenness rate multiplier";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "hunger":
			case "hungry":
				return BuildingCommandHunger(actor, command);
			case "thirst":
			case "thirsty":
				return BuildingCommandThirst(actor, command);
			case "drunk":
			case "drunken":
			case "drunkenness":
				return BuildingCommandDrunkenness(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandDrunkenness(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What percentage of the base drunkenness rate should this merit confer?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		DrunkennessMultiplier = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit now makes characters gain drunkenness at a {value.ToString("P2", actor).ColourValue()} rate compared to base.");
		return true;
	}

	private bool BuildingCommandThirst(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What percentage of the base thirst rate should this merit confer?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		ThirstMultiplier = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit now makes characters gain thirst at a {value.ToString("P2", actor).ColourValue()} rate compared to base.");
		return true;
	}

	private bool BuildingCommandHunger(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What percentage of the base hunger rate should this merit confer?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		HungerMultiplier = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit now makes characters gain hunger at a {value.ToString("P2", actor).ColourValue()} rate compared to base.");
		return true;
	}

	#region Implementation of INeedRateChangingMerit

	public double HungerMultiplier { get; set; }
	public double ThirstMultiplier { get; set; }
	public double DrunkennessMultiplier { get; set; }

	#endregion
}
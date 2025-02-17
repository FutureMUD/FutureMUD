using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class SwimmingStaminaMerit : CharacterMeritBase
{
	protected SwimmingStaminaMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		Multiplier = double.Parse(definition.Attribute("multiplier")?.Value ?? "1.0");
	}

	protected SwimmingStaminaMerit() { }

	protected SwimmingStaminaMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Swimming Stamina Multiplier", "@ have|has a stamina modifier to swimming")
	{
		Multiplier = 1.0;
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("multiplier", Multiplier));
		return root;
	}

	public double Multiplier { get; set; }

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Swimming Stamina Multiplier",
			(merit, gameworld) => new SwimmingStaminaMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Swimming Stamina Multiplier", (gameworld, name) => new SwimmingStaminaMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Swimming Stamina Multiplier", "Multiplies swimming stamina usage", new SwimmingStaminaMerit().HelpText);
	}

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "multiplier":
			case "mult":
				return BuildingCommandMultiplier(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandMultiplier(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What multiplier should be applied to stamina usage?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		Multiplier = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now multiply stamina usage by {value.ToString("P2", actor).ColourValue()} when it applies.");
		return true;
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3multiplier <%>#0 - sets the percentage multiplier for stamina usage";

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Stamina Usage Multiplier: {Multiplier.ToString("P2", actor).ColourValue()}");
	}
}
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

internal class AllSpeedMovesSpeedMerit : CharacterMeritBase, IMovementSpeedMerit
{
	protected AllSpeedMovesSpeedMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		Multiplier = double.Parse(definition.Attribute("multiplier")?.Value ?? "0.0");
	}

	protected AllSpeedMovesSpeedMerit(){}

	protected AllSpeedMovesSpeedMerit(IFuturemud gameworld, string name) : base(gameworld, name, "All Speed Multiplier", "@ have|has a bonus to movement speed")
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

	public double SpeedMultiplier(IMoveSpeed speed)
	{
		return Multiplier;
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
			actor.OutputHandler.Send("What multiplier should be applied to speeds?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		Multiplier = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now multiply speed by {value.ToString("P2", actor).ColourValue()} when it applies.");
		return true;
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3multiplier <%>#0 - sets the percentage multiplier for speed";

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Speed Multiplier: {Multiplier.ToString("P2", actor).ColourValue()}");
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("All Speed Multiplier",
			(merit, gameworld) => new AllSpeedMovesSpeedMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("All Speed Multiplier", (gameworld, name) => new AllSpeedMovesSpeedMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("All Speed Multiplier", "Multiplies move speed", new AllSpeedMovesSpeedMerit().HelpText);
	}
}
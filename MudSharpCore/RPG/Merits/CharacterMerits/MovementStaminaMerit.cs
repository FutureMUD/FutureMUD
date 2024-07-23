using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Character;
using System.Text;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class MovementStaminaMerit : CharacterMeritBase, IMovementStaminaMerit
{
	protected MovementStaminaMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		Multiplier = double.Parse(definition.Attribute("multiplier")?.Value ?? "0.0");
	}

	protected MovementStaminaMerit(){}

	protected MovementStaminaMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Movement Stamina Multiplier", "@ have|has a stamina modifier to movement")
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

	public double StaminaMultiplier(IMoveSpeed speed)
	{
		return Multiplier;
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Movement Stamina Multiplier",
			(merit, gameworld) => new MovementStaminaMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Movement Stamina Multiplier", (gameworld, name) => new MovementStaminaMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Movement Stamina Multiplier", "Multiplies movement stamina usage", new MovementStaminaMerit().HelpText);
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
using System;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Character;
using System.Text;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Commands.Trees;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class MovementStaminaMerit : CharacterMeritBase, IMovementStaminaMerit
{
	protected MovementStaminaMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		Multiplier = double.Parse(definition.Attribute("multiplier")?.Value ?? "0.0");
		MoveTypes = (MovementType)int.Parse(definition.Attribute("movetypes")?.Value ?? ((int)MovementType.All).ToString("F0"));
	}

	protected MovementStaminaMerit(){}

	protected MovementStaminaMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Movement Stamina Multiplier", "@ have|has a stamina modifier to movement")
	{
		Multiplier = 1.0;
		MoveTypes = MovementType.All;
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("multiplier", Multiplier));
		root.Add(new XAttribute("movetypes", (int)MoveTypes));
		return root;
	}

	public double Multiplier { get; set; }
	public MovementType MoveTypes { get; set; }

	public double StaminaMultiplier(IMoveSpeed speed)
	{
		switch (speed.Position)
		{
			case PositionStanding:
				return MoveTypes.HasFlag(MovementType.Upright) ? Multiplier : 1.0;
			case PositionClimbing:
				return MoveTypes.HasFlag(MovementType.Climbing) ? Multiplier : 1.0;
			case PositionSwimming:
				return MoveTypes.HasFlag(MovementType.Swimming) ? Multiplier : 1.0;
			case PositionFlying:
				return MoveTypes.HasFlag(MovementType.Flying) ? Multiplier : 1.0;
			case PositionProne:
				return MoveTypes.HasFlag(MovementType.Crawling) ? Multiplier : 1.0;
			case PositionProstrate:
				return MoveTypes.HasFlag(MovementType.Prostrate) ? Multiplier : 1.0;
			default:
				return 1.0;
		}
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
			case "movetype":
				return BuildingCommandMoveType(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandMoveType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which movement types should this stamina multiplier apply to?\nValid choices are {Enum.GetValues<MovementType>().ListToColouredString()}.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum<MovementType>(out var mt))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()}is not a valid movement type.\nValid choices are {Enum.GetValues<MovementType>().ListToColouredString()}.");
			return false;
		}

		Changed = true;
		if (mt == MovementType.All)
		{
			MoveTypes = MovementType.All;
			actor.OutputHandler.Send("This stamina multiplier now applies to all types of movement.");
			return true;
		}

		if (mt == MovementType.None)
		{
			MoveTypes = MovementType.None;
			actor.OutputHandler.Send($"This stamina multiplier now applies to none of the types of movement.");
			return true;
		}

		if (MoveTypes.HasFlag(mt))
		{
			MoveTypes &= ~mt;
			actor.OutputHandler.Send($"This stamina multiplier no longer applies to the {mt.DescribeEnum().ColourValue()} movement type.\nThe types it applies to now is {MoveTypes.DescribeEnum(colour: Telnet.Green)}.");
			return true;
		}

		MoveTypes |= mt;
		actor.OutputHandler.Send($"This stamina multiplier now applies to the {mt.DescribeEnum().ColourValue()} movement type.\nThe types it applies to now is {MoveTypes.DescribeEnum(colour: Telnet.Green)}.");
		return true;
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
	#3multiplier <%>#0 - sets the percentage multiplier for stamina usage
	#3movetype <none|upright|crawling|prostrate|flying|swimming|climbing|all>#0 - toggles the bonus applying to a type of movement";

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Stamina Usage Multiplier: {Multiplier.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Movement Types: {MoveTypes.DescribeEnum(colour: Telnet.Green)}");
	}
}
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.RPG.Merits.Interfaces;
using MoveSpeed = MudSharp.Movement.MoveSpeed;
using MudSharp.Character;
using System.Text;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class SpecificSpeedMovementMultiplierMerit : CharacterMeritBase, IMovementSpeedMerit
{
	protected SpecificSpeedMovementMultiplierMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		Multiplier = double.Parse(definition.Attribute("multiplier")?.Value ?? "0.0");
		MoveSpeedIds = (from element in definition.Element("Speeds")?.Elements() ?? Enumerable.Empty<XElement>()
		                select long.Parse(element.Attribute("id")?.Value ?? "0")).ToList();
	}

	protected SpecificSpeedMovementMultiplierMerit(){}

	protected SpecificSpeedMovementMultiplierMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Specific Speed Multiplier", "@ have|has a bonus to a specific speed")
	{
		Multiplier = 1.0;
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("multiplier", Multiplier));
		root.Add(new XElement("Speeds",
			from item in MoveSpeedIds
			select new XElement("Speed", new XAttribute("id", item))
		));
		return root;
	}

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "multiplier":
			case "mult":
				return BuildingCommandMultiplier(actor, command);
			case "speed":
				return BuildingCommandSpeed(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandSpeed(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which speed do you want to toggle being affected? See {"show speeds".MXPSend()} for a list.");
			return false;
		}

		var speed = Gameworld.MoveSpeeds.GetByIdOrName(command.SafeRemainingArgument);
		if (speed is null)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid move speed. See {"show speeds".MXPSend()} for a list.");
			return false;
		}

		Changed = true;
		if (MoveSpeedIds.Remove(speed.Id))
		{
			actor.OutputHandler.Send($"This merit will no longer apply to the {speed.Name.ColourValue()} speed (#{speed.Id.ToString("N0", actor)}).");
			return true;
		}

		MoveSpeedIds.Add(speed.Id);
		actor.OutputHandler.Send($"This merit will now apply to the {speed.Name.ColourValue()} speed (#{speed.Id.ToString("N0", actor)}).");
		return true;
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
	#3multiplier <%>#0 - sets the percentage multiplier for speed
	#3speed <which>#0 - toggles a speed as being affected by this merit";

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Speed Multiplier: {Multiplier.ToString("P2", actor).ColourValue()}");
		sb.AppendLine($"Speeds:");
		sb.AppendLine();
		foreach (var id in MoveSpeedIds.ToArray())
		{
			var speed = Gameworld.MoveSpeeds.Get(id);
			if (speed is null)
			{
				MoveSpeedIds.Remove(id);
				Changed = true;
				continue;
			}

			sb.AppendLine($"\t{speed.Name.TitleCase().ColourValue()} (#{speed.Id.ToString("N0", actor)}){Gameworld.BodyPrototypes.FirstOrDefault(x => x.Speeds.Contains(speed))?.Name.SquareBracketsSpace().Colour(Telnet.Magenta) ?? ""}");
		}
	}

	public List<long> MoveSpeedIds { get; } = new();
	public double Multiplier { get; set; }

	public double SpeedMultiplier(IMoveSpeed speed)
	{
		return MoveSpeedIds.Contains(speed.Id) ? Multiplier : 1.0;
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Specific Speed Multiplier",
			(merit, gameworld) => new SpecificSpeedMovementMultiplierMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Specific Speed Multiplier", (gameworld, name) => new SpecificSpeedMovementMultiplierMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Specific Speed Multiplier", "Multiplies specific move speeds", new SpecificSpeedMovementMultiplierMerit().HelpText);
	}
}
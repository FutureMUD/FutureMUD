using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class FixedBloodTypeMerit : CharacterMeritBase, IFixedBloodTypeMerit
{
	protected FixedBloodTypeMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		Bloodtype = gameworld.Bloodtypes.Get(long.Parse(XElement.Parse(merit.Definition).Attribute("bloodtype").Value));
	}

	protected FixedBloodTypeMerit(){}

	protected FixedBloodTypeMerit(IFuturemud gameworld, string name) : base(gameworld, name, "FixedBloodtype", "@ have|has a fixed blood type")
	{
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("bloodtype", Bloodtype?.Id ?? 0));
		return root;
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Fixed Bloodtype: {Bloodtype?.Name.ColourValue() ?? "None".ColourError()}");
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("FixedBloodtype",
			(merit, gameworld) => new FixedBloodTypeMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("FixedBloodtype", (gameworld, name) => new FixedBloodTypeMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("FixedBloodtype", "Causes a character to start with a fixed blood type", new FixedBloodTypeMerit().HelpText);
	}

	public IBloodtype Bloodtype { get; private set; }

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3blood <type>#0 - sets the blood type this merit forces";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "type":
			case "bloodtype":
			case "blood":
				return BuildingCommandBloodType(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandBloodType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which blood type should this merit force the character to have?");
			return false;
		}

		var bloodtype = Gameworld.Bloodtypes.GetByIdOrName(command.SafeRemainingArgument);
		if (bloodtype is null)
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} does not refer to any existing blood type.");
			return false;
		}

		Bloodtype = bloodtype;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now force its character to have the {Bloodtype.Name.ColourValue()} blood type at character generation.");
		return true;
	}
}
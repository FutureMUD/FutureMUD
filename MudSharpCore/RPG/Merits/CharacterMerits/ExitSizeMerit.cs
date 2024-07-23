using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class ExitSizeMerit : CharacterMeritBase, IContextualSizeMerit
{
	public int SizeOffset { get; set; }

	protected ExitSizeMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		SizeOffset = int.Parse(definition.Element("SizeOffset")?.Value ?? "0");
	}

	protected ExitSizeMerit(){}

	protected ExitSizeMerit(IFuturemud gameworld, string name) : base(gameworld, name, "ExitSize", "@ have|has an altered size for the purpose of exits")
	{
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("SizeOffset", SizeOffset));
		return root;
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("ExitSize",
			(merit, gameworld) => new ExitSizeMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("ExitSize", (gameworld, name) => new ExitSizeMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("ExitSize", "Changes the effective size for exits", new ExitSizeMerit().HelpText);
	}

	public SizeCategory ContextualSize(SizeCategory original, SizeContext context)
	{
		if (context != SizeContext.CellExit)
		{
			return original;
		}

		return (SizeCategory)Math.Min((int)SizeCategory.Titanic, Math.Max(0, (int)original + SizeOffset));
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Size Category Change for Exits: {SizeOffset.ToBonusString(actor)}");
		sb.AppendLine();
		sb.AppendLine("Effective Sizes (Original -> New):");
		sb.AppendLine();
		foreach (var value in Enum.GetValues<SizeCategory>())
		{
			sb.AppendLine($"\t{value.DescribeEnum().ColourValue()} -> {value.Stage(SizeOffset).DescribeEnum().ColourValue()}");
		}
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3bonus <##>#0 - set the number of steps difference for exit sizes";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "bonus":
				return BuildingCommandBonus(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandBonus(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What number of bonus steps should this merit apply to effective size?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		SizeOffset = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now alter the effective size for exits by {SizeOffset.ToString("N0", actor).ColourValue()} steps.");
		return true;
	}
}
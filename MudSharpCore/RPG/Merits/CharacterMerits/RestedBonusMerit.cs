using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class RestedBonusMerit : CharacterMeritBase, IRestedBonusMerit
{
	protected RestedBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		var element = definition.Element("Multiplier");
		Multiplier = double.Parse(element?.Value ?? "1.0");
	}

	protected RestedBonusMerit(){}

	protected RestedBonusMerit(IFuturemud gameworld, string name) : base(gameworld, name, "RestedBonus", "@ have|has a multiplier for &0's rested bonuses")
	{
		Multiplier = 1.0;
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("Multiplier", Multiplier));
		return root;
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Rested Bonus Multiplier: {Multiplier.ToString("P2", actor).ColourValue()}");
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("RestedBonus",
			(merit, gameworld) => new RestedBonusMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("RestedBonus", (gameworld, name) => new RestedBonusMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("RestedBonus", "Multiplies the impact of rested bonuses", new RestedBonusMerit().HelpText);
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3multiplier <%>#0 - sets the bonus multiplier";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "multiplier":
			case "modifier":
			case "mult":
			case "mod":
				return BuildingCommandMultiplier(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandMultiplier(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What multiplier should be applied to rested bonuses?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		Multiplier = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now multiply rested bonus effect by {value.ToString("P2", actor).ColourValue()} when it applies.");
		return true;
	}

	#region Implementation of IRestedBonusMerit

	public double Multiplier { get; protected set; }

	#endregion
}
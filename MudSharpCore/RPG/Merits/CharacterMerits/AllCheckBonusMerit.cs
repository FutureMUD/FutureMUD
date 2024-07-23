using System.Text;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class AllCheckBonusMerit : CharacterMeritBase, ICheckBonusMerit
{
	protected AllCheckBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		SpecificBonus = double.Parse(definition.Attribute("bonus")?.Value ?? "0");
	}

	protected AllCheckBonusMerit(IFuturemud gameworld, string name) : base(gameworld, name, "All Check Bonus", "@ have|has a bonus to all checks")
	{
		SpecificBonus = 0.0;
		DoDatabaseInsert();
	}

	protected AllCheckBonusMerit()
	{
	}

	public double SpecificBonus { get; set; }

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("bonus", SpecificBonus));
		return root;
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Bonus for All Checks: {SpecificBonus.ToBonusString(actor)}");
	}

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
			actor.OutputHandler.Send("What should the bonus be when this merit applies?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		SpecificBonus = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now add a bonus of {SpecificBonus.ToBonusString(actor)} to all checks when it applies.");
		return true;
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3bonus <##>#0 - sets the bonus for this merit";

	#region Implementation of ICheckBonusMerit

	public double CheckBonus(ICharacter ch, IPerceivable target, CheckType type)
	{
		return SpecificBonus;
	}

	#endregion

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("All Check Bonus",
			(merit, gameworld) => new AllCheckBonusMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("All Check Bonus", (gameworld, name) => new AllCheckBonusMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("All Check Bonus", "Adds a bonus or penalty to all checks rolled", new AllCheckBonusMerit().HelpText);
	}
}
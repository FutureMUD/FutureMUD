using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using System.Text;
using MudSharp.FutureProg;
using System.Collections.Generic;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class SpecificCheckBonusMerit : CharacterMeritBase, ICheckBonusMerit
{
	protected SpecificCheckBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		CheckType = (CheckType)int.Parse(definition.Attribute("type")?.Value ?? "0");
		SpecificBonus = double.Parse(definition.Attribute("bonus")?.Value ?? "0");
	}

	protected SpecificCheckBonusMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Specific Check Bonus", "@ have|has a bonus to a specific check")
	{
		CheckType = CheckType.HideCheck;
		SpecificBonus = 0.0;
		DoDatabaseInsert();
	}

	protected SpecificCheckBonusMerit()
	{
	}

	public CheckType CheckType { get; set; }
	public double SpecificBonus { get; set; }

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("bonus", SpecificBonus));
		root.Add(new XAttribute("type", (int)CheckType));
		return root;
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Check Type: {CheckType.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Bonus for Check: {SpecificBonus.ToBonusString(actor)}");
	}

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "bonus":
				return BuildingCommandBonus(actor, command);
			case "check":
				return BuildingCommandCheck(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandCheck(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which check type should this merit's bonus apply to? See {"show checks".MXPSend()} for a complete list.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out CheckType type))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid check type. See {"show checks".MXPSend()} for a complete list.");
			return false;
		}

		CheckType = type;
		Changed = true;
		actor.OutputHandler.Send($"This merit now applies its bonus to the {CheckType.DescribeEnum().ColourValue()} check.");
		return true;
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
		actor.OutputHandler.Send($"This merit will now add a bonus of {SpecificBonus.ToBonusString(actor)} to the affected check when it applies.");
		return true;
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3bonus <##>#0 - sets the bonus for this merit
	#3type <check>#0 - sets which check this merit applies to";

	#region Implementation of ICheckBonusMerit
	protected override IEnumerable<IEnumerable<ProgVariableTypes>> AppliesProgValidTypes => [[ProgVariableTypes.Character], [ProgVariableTypes.Character, ProgVariableTypes.Perceivable], [ProgVariableTypes.Character, ProgVariableTypes.Character]];
	public double CheckBonus(ICharacter ch, IPerceivable target, CheckType type)
	{
		return type == CheckType ? SpecificBonus : 0.0;
	}

	#endregion

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Specific Check Bonus",
			(merit, gameworld) => new SpecificCheckBonusMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Specific Check Bonus", (gameworld, name) => new SpecificCheckBonusMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Specific Check Bonus", "Adds a bonus or penalty to a specific check rolled", new SpecificCheckBonusMerit().HelpText);
	}
}
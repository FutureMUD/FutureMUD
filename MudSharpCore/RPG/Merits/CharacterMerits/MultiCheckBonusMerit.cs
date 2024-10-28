using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using System.Text;
using MudSharp.FutureProg;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class MultiCheckBonusMerit : CharacterMeritBase, ICheckBonusMerit
{
	protected MultiCheckBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		_checkTypes.AddRange(from element in definition.Element("Checks")?.Elements() ?? Enumerable.Empty<XElement>()
		              select (CheckType)int.Parse(element.Attribute("type")?.Value ?? "0"));
		SpecificBonus = double.Parse(definition.Attribute("bonus")?.Value ?? "0");
	}

	protected MultiCheckBonusMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Multi Check Bonus", "@ have|has a bonus to multiple check types")
	{
		DoDatabaseInsert();
	}

	protected MultiCheckBonusMerit()
	{

	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add("Checks", 
			from item in CheckTypes
			select new XElement("Check", new XAttribute("type", (int)item))
		);
		root.Add(new XAttribute("bonus", SpecificBonus));
		return root;
	}

	private readonly List<CheckType> _checkTypes = new();
	public IEnumerable<CheckType> CheckTypes => _checkTypes;
	public double SpecificBonus { get; set; }

	#region Implementation of ICheckBonusMerit
	protected override IEnumerable<IEnumerable<ProgVariableTypes>> AppliesProgValidTypes => [[ProgVariableTypes.Character], [ProgVariableTypes.Character, ProgVariableTypes.Perceivable], [ProgVariableTypes.Character, ProgVariableTypes.Character]];
	public double CheckBonus(ICharacter ch, IPerceivable target, CheckType type)
	{
		return CheckTypes.Contains(type) ? SpecificBonus : 0.0;
	}

	#endregion

	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3bonus <##>#0 - sets the bonus for this merit
	#3check <which>#0 - toggles a particular check type applying";

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
			actor.OutputHandler.Send($"Which check type do you want to toggle? See {"show checks".MXPSend()} for a complete list.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParseEnum(out CheckType type))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid check type. See {"show checks".MXPSend()} for a complete list.");
			return false;
		}

		Changed = true;
		if (_checkTypes.Remove(type))
		{
			actor.OutputHandler.Send($"This merit will no longer give a bonus to the {type.DescribeEnum().ColourValue()} check.");
			return true;
		}

		_checkTypes.Add(type);
		actor.OutputHandler.Send($"This merit now applies its bonus to the {type.DescribeEnum().ColourValue()} check.");
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

	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Bonus for Check: {SpecificBonus.ToBonusString(actor)}");
		sb.AppendLine($"Checks: {CheckTypes.Select(x => x.DescribeEnum().ColourValue()).ListToString()}");
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Multi Check Bonus",
			(merit, gameworld) => new MultiCheckBonusMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Multi Check Bonus", (gameworld, name) => new MultiCheckBonusMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Multi Check Bonus", "Adds a bonus or penalty to specific checks rolled", new MultiCheckBonusMerit().HelpText);
	}
}
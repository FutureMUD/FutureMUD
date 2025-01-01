using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using Chargen = MudSharp.CharacterCreation.Chargen;
using System.Text;
using MudSharp.FutureProg;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class CheckCategoryBonusMerit : CharacterMeritBase, ICheckBonusMerit
{
	protected CheckCategoryBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		SpecificBonus = double.Parse(definition.Attribute("bonus")?.Value ?? "0");
		AppliesToHealingChecks = bool.Parse(definition.Attribute("healing")?.Value ?? "false");
		AppliesToFriendlyChecks = bool.Parse(definition.Attribute("friendly")?.Value ?? "false");
		AppliesToHostileChecks = bool.Parse(definition.Attribute("hostile")?.Value ?? "false");
		AppliesToActiveChecks = bool.Parse(definition.Attribute("active")?.Value ?? "false");
		AppliesToPerceptionChecks = bool.Parse(definition.Attribute("perception")?.Value ?? "false");
		AppliesToLanguageChecks = bool.Parse(definition.Attribute("language")?.Value ?? "false");
	}

	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("bonus", SpecificBonus));
		root.Add(new XAttribute("healing", AppliesToHealingChecks));
		root.Add(new XAttribute("friendly", AppliesToFriendlyChecks));
		root.Add(new XAttribute("hostile", AppliesToHostileChecks));
		root.Add(new XAttribute("active", AppliesToActiveChecks));
		root.Add(new XAttribute("perception", AppliesToPerceptionChecks));
		root.Add(new XAttribute("language", AppliesToLanguageChecks));
		return root;
	}

	protected CheckCategoryBonusMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Check Category Bonus", "@ have|has a bonus to specific categories of checks")
	{
		DoDatabaseInsert();
	}

	protected CheckCategoryBonusMerit()
	{

	}

	public double SpecificBonus { get; set; }

	public bool AppliesToHealingChecks { get; set; }
	public bool AppliesToFriendlyChecks { get; set; }
	public bool AppliesToHostileChecks { get; set; }
	public bool AppliesToActiveChecks { get; set; }
	public bool AppliesToPerceptionChecks { get; set; }
	public bool AppliesToLanguageChecks { get; set; }

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Bonus for Checks: {SpecificBonus.ToBonusString(actor)}");
		sb.AppendLine($"Applies to Healing: {AppliesToHealingChecks.ToColouredString()}");
		sb.AppendLine($"Applies to Friendly: {AppliesToFriendlyChecks.ToColouredString()}");
		sb.AppendLine($"Applies to Hostile: {AppliesToHostileChecks.ToColouredString()}");
		sb.AppendLine($"Applies to Active: {AppliesToActiveChecks.ToColouredString()}");
		sb.AppendLine($"Applies to Perception: {AppliesToPerceptionChecks.ToColouredString()}");
		sb.AppendLine($"Applies to Language: {AppliesToLanguageChecks.ToColouredString()}");
	}

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "bonus":
				return BuildingCommandBonus(actor, command);
			case "healing":
				return BuildingCommandHealing(actor);
			case "friendly":
				return BuildingCommandFriendly(actor);
			case "hostile":
				return BuildingCommandHostile(actor);
			case "active":
				return BuildingCommandActive(actor);
			case "perception":
				return BuildingCommandPerception(actor);
			case "language":
				return BuildingCommandLanguage(actor);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandLanguage(ICharacter actor)
	{
		AppliesToLanguageChecks = !AppliesToLanguageChecks;
		Changed = true;
		actor.OutputHandler.Send($"The bonus will {AppliesToLanguageChecks.NowNoLonger()} apply to language checks.");
		return true;
	}

	private bool BuildingCommandPerception(ICharacter actor)
	{
		AppliesToPerceptionChecks = !AppliesToPerceptionChecks;
		Changed = true;
		actor.OutputHandler.Send($"The bonus will {AppliesToPerceptionChecks.NowNoLonger()} apply to perception checks.");
		return true;
	}

	private bool BuildingCommandActive(ICharacter actor)
	{
		AppliesToActiveChecks = !AppliesToActiveChecks;
		Changed = true;
		actor.OutputHandler.Send($"The bonus will {AppliesToActiveChecks.NowNoLonger()} apply to active checks.");
		return true;
	}

	private bool BuildingCommandHostile(ICharacter actor)
	{
		AppliesToHostileChecks = !AppliesToHostileChecks;
		Changed = true;
		actor.OutputHandler.Send($"The bonus will {AppliesToHostileChecks.NowNoLonger()} apply to hostile checks.");
		return true;
	}

	private bool BuildingCommandFriendly(ICharacter actor)
	{
		AppliesToFriendlyChecks = !AppliesToFriendlyChecks;
		Changed = true;
		actor.OutputHandler.Send($"The bonus will {AppliesToFriendlyChecks.NowNoLonger()} apply to friendly checks.");
		return true;
	}

	private bool BuildingCommandHealing(ICharacter actor)
	{
		AppliesToHealingChecks = !AppliesToHealingChecks;
		Changed = true;
		actor.OutputHandler.Send($"The bonus will {AppliesToHealingChecks.NowNoLonger()} apply to healing checks.");
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
		actor.OutputHandler.Send($"This merit will now add a bonus of {SpecificBonus.ToBonusString(actor)} to all affected checks when it applies.");
		return true;
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3bonus <##>#0 - sets the bonus for this merit
	#3healing#0 - toggles applying to healing checks
	#3friendly#0 - toggles applying to friendly checks
	#3active#0 - toggles applying to active checks
	#3hostile#0 - toggles applying to hostile checks
	#3language#0 - toggles applying to language checks
	#3perception#0 - toggles applying to perception checks";

	#region Implementation of ICheckBonusMerit
	protected override IEnumerable<IEnumerable<ProgVariableTypes>> AppliesProgValidTypes => [[ProgVariableTypes.Character], [ProgVariableTypes.Character, ProgVariableTypes.Perceivable], [ProgVariableTypes.Character, ProgVariableTypes.Character]];
	public double CheckBonus(ICharacter ch, IPerceivable target, CheckType type)
	{
		if ((AppliesToHealingChecks && type.IsHealingCheck()) ||
		    (AppliesToFriendlyChecks && type.IsTargettedFriendlyCheck()) ||
		    (AppliesToHostileChecks && type.IsTargettedHostileCheck()) ||
		    (AppliesToActiveChecks && type.IsGeneralActivityCheck()) ||
		    (AppliesToPerceptionChecks && type.IsPerceptionCheck()) ||
		    (AppliesToLanguageChecks && type.IsLanguageCheck()))
		{
			return SpecificBonus;
		}

		return 0.0;
	}

	#endregion

	public override bool Applies(IHaveMerits owner)
	{
		return Applies(owner, default(ICharacter));
	}

	public override bool Applies(IHaveMerits owner, IPerceivable target)
	{
		if (owner is ICharacter ownerAsCharacter)
		{
			return ApplicabilityProg?.ExecuteBool(ownerAsCharacter, target) ?? true;
		}

		if (owner is IBody ownerAsBody)
		{
			return ApplicabilityProg?.ExecuteBool(ownerAsBody.Actor, target) ?? true;
		}

		return owner is Chargen ownerAsChargen;
	}

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Check Category Bonus",
			(merit, gameworld) => new CheckCategoryBonusMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Check Category Bonus", (gameworld, name) => new CheckCategoryBonusMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Check Category Bonus", "Adds a bonus or penalty to specific categories of checks rolled", new CheckCategoryBonusMerit().HelpText);
	}
}
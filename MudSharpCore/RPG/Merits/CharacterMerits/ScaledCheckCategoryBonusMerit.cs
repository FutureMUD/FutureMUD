using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using Chargen = MudSharp.CharacterCreation.Chargen;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class ScaledCheckCategoryBonusMerit : CharacterMeritBase, ICheckBonusMerit
{
	protected ScaledCheckCategoryBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		GetBonusAmountProg = gameworld.FutureProgs.Get(long.Parse(definition.Attribute("bonusprog")?.Value ?? "0"));
		AppliesToHealingChecks = bool.Parse(definition.Attribute("healing")?.Value ?? "false");
		AppliesToFriendlyChecks = bool.Parse(definition.Attribute("friendly")?.Value ?? "false");
		AppliesToHostileChecks = bool.Parse(definition.Attribute("hostile")?.Value ?? "false");
		AppliesToActiveChecks = bool.Parse(definition.Attribute("active")?.Value ?? "false");
		AppliesToPerceptionChecks = bool.Parse(definition.Attribute("perception")?.Value ?? "false");
		AppliesToLanguageChecks = bool.Parse(definition.Attribute("language")?.Value ?? "false");
	}

	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("bonusprog", GetBonusAmountProg.Id));
		root.Add(new XAttribute("healing", AppliesToHealingChecks));
		root.Add(new XAttribute("friendly", AppliesToFriendlyChecks));
		root.Add(new XAttribute("hostile", AppliesToHostileChecks));
		root.Add(new XAttribute("active", AppliesToActiveChecks));
		root.Add(new XAttribute("perception", AppliesToPerceptionChecks));
		root.Add(new XAttribute("language", AppliesToLanguageChecks));
		return root;
	}

	protected ScaledCheckCategoryBonusMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Scaled Check Category Bonus", "@ have|has a bonus to specific categories of checks")
	{
		GetBonusAmountProg = Gameworld.AlwaysZeroProg;
		DoDatabaseInsert();
	}

	protected ScaledCheckCategoryBonusMerit()
	{

	}

	public bool AppliesToHealingChecks { get; set; }
	public bool AppliesToFriendlyChecks { get; set; }
	public bool AppliesToHostileChecks { get; set; }
	public bool AppliesToActiveChecks { get; set; }
	public bool AppliesToPerceptionChecks { get; set; }
	public bool AppliesToLanguageChecks { get; set; }

	public IFutureProg GetBonusAmountProg { get; set; }

	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Bonus for Checks Prog: {GetBonusAmountProg.MXPClickableFunctionName()}");
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
			actor.OutputHandler.Send("Which prog should determine the bonus when this merit applies?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Number, [
			[ProgVariableTypes.Character],
			[ProgVariableTypes.Character, ProgVariableTypes.Character],
			[ProgVariableTypes.Character, ProgVariableTypes.Perceivable],
		]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		GetBonusAmountProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now add a bonus based on the return value of the {prog.MXPClickableFunctionName()} prog to all affected checks when it applies.");
		return true;
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3bonus <##>#0 - sets the prog that determines the bonus for this merit
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
			return GetBonusAmountProg.ExecuteDouble(0.0, ch, target);
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
		MeritFactory.RegisterMeritInitialiser("Scaled Check Category Bonus",
			(merit, gameworld) => new ScaledCheckCategoryBonusMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Scaled Check Category Bonus", (gameworld, name) => new ScaledCheckCategoryBonusMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Scaled Check Category Bonus", "Adds a scaled bonus or penalty to specific categories of checks rolled", new ScaledCheckCategoryBonusMerit().HelpText);
	}
}
using System.Xml.Linq;
using MudSharp.Body.Traits;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Character;
using System.Text;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class AllSkillBonusMerit : CharacterMeritBase, ITraitBonusMerit
{
	protected AllSkillBonusMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		SpecificBonus = double.Parse(definition.Attribute("bonus")?.Value ?? "0.0");
	}

	protected AllSkillBonusMerit()
	{

	}

	protected AllSkillBonusMerit(IFuturemud gameworld, string name) : base(gameworld, name, "All Skill Bonus", "@ have|has a bonus to all skills")
	{

	}

	public double SpecificBonus { get; set; }

	#region Implementation of ITraitBonusMerit

	public double BonusForTrait(ITraitDefinition trait, TraitBonusContext context)
	{
		return trait.TraitType == TraitType.Skill ? SpecificBonus : 0.0;
	}

	#endregion

	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("bonus", SpecificBonus));
		return root;
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Bonus for All Skills: {SpecificBonus.ToBonusString(actor)}");
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
		actor.OutputHandler.Send($"This merit will now add a bonus of {SpecificBonus.ToBonusString(actor)} to all skills when it applies.");
		return true;
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3bonus <##>#0 - sets the bonus for this merit";

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("All Skill Bonus",
			(merit, gameworld) => new AllSkillBonusMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("All Skill Bonus", (gameworld, name) => new AllSkillBonusMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("All Skill Bonus", "Adds a bonus or penalty to all skills", new AllSkillBonusMerit().HelpText);
	}
}
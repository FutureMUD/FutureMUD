using System.Xml.Linq;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Character;
using System.Text;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class AllInfectionResistanceMerit : CharacterMeritBase, IInfectionResistanceMerit
{
	protected AllInfectionResistanceMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		SpecificBonus = int.Parse(definition.Attribute("bonus")?.Value ?? "0");
	}

	protected AllInfectionResistanceMerit()
	{

	}

	protected AllInfectionResistanceMerit(IFuturemud gameworld, string name) : base(gameworld, name, "All Infection Bonus", "@ have|has a bonus against infections")
	{
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("bonus", SpecificBonus));
		return root;
	}

	protected int SpecificBonus { get; set; }

	public Difficulty GetNewInfectionDifficulty(Difficulty original, InfectionType type)
	{
		return original.StageDown(SpecificBonus);
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Bonus against Infections: {SpecificBonus.ToBonusString(actor)} {"Step".Pluralise(SpecificBonus.Abs() == 1)}");
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
			actor.OutputHandler.Send("What should the bonus number of steps be when this merit applies?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		SpecificBonus = value;
		Changed = true;
		actor.OutputHandler.Send($"This merit will now add a bonus of {SpecificBonus.ToBonusString(actor)} {"step".Pluralise(value.Abs() == 1)} to all infection checks when it applies.");
		return true;
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3bonus <##>#0 - sets the bonus steps for this merit";

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("All Infection Bonus",
			(merit, gameworld) => new AllInfectionResistanceMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("All Infection Bonus", (gameworld, name) => new AllInfectionResistanceMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("All Infection Bonus", "Adds a bonus or penalty to infection checks", new AllInfectionResistanceMerit().HelpText);
	}
}
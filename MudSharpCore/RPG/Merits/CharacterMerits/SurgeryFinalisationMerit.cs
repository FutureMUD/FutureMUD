using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Models;
using MudSharp.Framework;
using MudSharp.FutureProg.Functions.BuiltIn;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.RPG.Merits.CharacterMerits;

public class SurgeryFinalisationMerit : CharacterMeritBase, ISurgeryFinalisationMerit
{
	protected SurgeryFinalisationMerit(Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		BonusDegrees = int.Parse(definition.Attribute("bonus")?.Value ?? "0.0");
	}

	protected SurgeryFinalisationMerit(){}

	protected SurgeryFinalisationMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Surgery Finalisation", "@ are|is a skilled surgeon")
	{
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XAttribute("bonus", BonusDegrees));
		return root;
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Recovery Bonus Degrees: {BonusDegrees.ToBonusString(actor)}");
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3bonus <##>#0 - sets the degrees of bonus on surgery recovery checks for patients";

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
			actor.OutputHandler.Send($"What do you want the bonus to be to patients recovering from surgeries performed by people with this merit?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		BonusDegrees = value;
		Changed = true;
		actor.OutputHandler.Send($"It will now be {value.ToBonusString(actor)} degrees {(value >= 0 ? "easier" : "harder")} for patients to recover from surgery performed by this surgeon.");
		return true;
	}

	public int BonusDegrees { get; set; }

	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Surgery Finalisation",
			(merit, gameworld) => new SurgeryFinalisationMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Surgery Finalisation", (gameworld, name) => new SurgeryFinalisationMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Surgery Finalisation", "Alters patient's chances of recover from surgery from this doctor", new SurgeryFinalisationMerit().HelpText);
	}
}
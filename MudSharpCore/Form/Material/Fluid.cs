using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.Health;

namespace MudSharp.Form.Material;

public abstract class Fluid : Material, IFluid
{
	/// <inheritdoc />
	protected Fluid(Models.Liquid material, IFuturemud gameworld) : base(material, gameworld)
	{
	}

	/// <inheritdoc />
	protected Fluid(Models.Gas material, IFuturemud gameworld) : base(material, gameworld)
	{
	}

	/// <inheritdoc />
	protected Fluid(string name, MaterialBehaviourType behaviour, IFuturemud gameworld) : base(name, behaviour,
		gameworld)
	{
		SmellText = "It doesn't really smell like anything.";
		VagueSmellText = "It doesn't really smell like anything.";
		SmellIntensity = 100;
		switch (behaviour)
		{
			case MaterialBehaviourType.Liquid:
				Viscosity = 1;
				break;
			case MaterialBehaviourType.Gas:
				Viscosity = 1;
				break;
		}
	}

	protected Fluid(Fluid rhs, string newName, MaterialBehaviourType materialBehaviour) : base(rhs, newName,
		materialBehaviour)
	{
		DisplayColour = rhs.DisplayColour;
		Viscosity = rhs.Viscosity;
		SmellText = rhs.SmellText;
		SmellIntensity = rhs.SmellIntensity;
		VagueSmellText = rhs.VagueSmellText;
		Drug = rhs.Drug;
		DrugGramsPerUnitVolume = rhs.DrugGramsPerUnitVolume;
	}

	#region Implementation of IFluid

	/// <inheritdoc />
	public ANSIColour DisplayColour { get; protected set; }

	/// <inheritdoc />
	public double Viscosity { get; protected set; }

	/// <inheritdoc />
	public double SmellIntensity { get; protected set; }

	/// <inheritdoc />
	public string SmellText { get; protected set; }

	/// <inheritdoc />
	public string VagueSmellText { get; protected set; }

	/// <inheritdoc />
	public IDrug Drug { get; protected set; }

	/// <inheritdoc />
	public double DrugGramsPerUnitVolume { get; protected set; }

	#endregion

	#region Overrides of Material

	/// <inheritdoc />
	protected override string HelpText => $@"{base.HelpText}
	#3colour <ansi>#0 - sets the display colour
	#3drug <which>#0 - sets the contained drug
	#3drug none#0 - clears the drug
	#3drugvolume <amount>#0 - sets the drug volume
	#3viscosity <viscosity>#0 - sets the viscosity in cSt
	#3smell <intensity> <smell> [<vague smell>]#0 - sets the smell";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "colour":
			case "color":
				return BuildingCommandColour(actor, command);
			case "drug":
				return BuildingCommandDrug(actor, command);
			case "drugvolume":
			case "volume":
				return BuildingCommandDrugVolume(actor, command);
			case "viscosity":
			case "visc":
				return BuildingCommandViscosity(actor, command);
			case "smell":
				return BuildingCommandSmell(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandSmell(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should the intensity of this smell be, relative to water?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var intensity) || intensity < 0)
		{
			actor.OutputHandler.Send("The smell intensity must be a valid positive value.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What should the smell of this {MaterialNoun} be?");
			return false;
		}

		var smell = command.PopSpeech().ProperSentences();
		var vague = smell;
		if (!command.IsFinished)
		{
			vague = command.SafeRemainingArgument.ProperSentences();
		}

		SmellIntensity = intensity;
		SmellText = smell;
		VagueSmellText = vague;
		Changed = true;
		actor.OutputHandler.Send($@"This {MaterialNoun} now has the following smell parameters:

	Intensity: {intensity.ToString("N2", actor).ColourValue()}
	Smell Text: {SmellText.Colour(DisplayColour)}
	Vague Smell: {VagueSmellText.Colour(DisplayColour)}");
		return true;
	}

	private bool BuildingCommandViscosity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must enter a valid positive number.");
			return false;
		}

		Viscosity = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This {MaterialNoun} now has a viscosity of {value.ToString("N3", actor).ColourValue()} cSt.");
		return true;
	}

	private bool BuildingCommandDrugVolume(ICharacter actor, StringStack command)
	{
		if (Drug is null)
		{
			actor.OutputHandler.Send("You must first set a drug.");
			return false;
		}

		if (command.IsFinished ||
		    command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value) || value < 0.0 ||
		    value > 1.0)
		{
			actor.OutputHandler.Send($"You must enter a valid percentage for the ratio of drug to liquid.");
			return false;
		}

		DrugGramsPerUnitVolume = value * Gameworld.UnitManager.BaseFluidToLitres /
		                         Gameworld.UnitManager.BaseWeightToKilograms;
		Changed = true;
		actor.OutputHandler.Send($"This {MaterialNoun} is now {value.ToString("P3", actor).ColourValue()} drug dose.");
		return true;
	}

	private bool BuildingCommandDrug(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a drug or the text {"none".ColourCommand()} to clear an existing one.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "delete", "remove"))
		{
			actor.OutputHandler.Send($"This {MaterialNoun} no longer has any drug dose.");
			Changed = true;
			Drug = null;
			DrugGramsPerUnitVolume = 0.0;
			return true;
		}

		var drug = Gameworld.Drugs.GetByIdOrName(command.SafeRemainingArgument);
		if (drug is null)
		{
			actor.OutputHandler.Send($"There is no such drug.");
			return false;
		}

		Drug = drug;
		DrugGramsPerUnitVolume = Gameworld.UnitManager.BaseFluidToLitres / Gameworld.UnitManager.BaseWeightToKilograms;
		Changed = true;
		actor.OutputHandler.Send($"This {MaterialNoun} now contains the drug {Drug.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandColour(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What colour should this {MaterialNoun} be? The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		var colour = Telnet.GetColour(command.SafeRemainingArgument);
		if (colour == null)
		{
			actor.OutputHandler.Send(
				$"That is not a valid colour. The options are as follows:\n\n{Telnet.GetColourOptions.Select(x => x.Colour(Telnet.GetColour(x))).ListToLines(true)}");
			return false;
		}

		DisplayColour = colour;
		Changed = true;
		actor.OutputHandler.Send($"This {MaterialNoun} now has a display colour of {colour.Name.Colour(colour)}.");
		return true;
	}

	#endregion
}
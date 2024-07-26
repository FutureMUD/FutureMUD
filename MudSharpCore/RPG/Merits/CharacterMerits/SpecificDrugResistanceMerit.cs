using System.Linq;
using System.Text;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Merits.Interfaces;
using System.Xml.Linq;
using MudSharp.Character;
using System;

namespace MudSharp.RPG.Merits.CharacterMerits;

internal class SpecificDrugResistanceMerit : CharacterMeritBase, ISpecificDrugResistanceMerit
{
	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Specific Drug Resistance",
			(merit, gameworld) => new SpecificDrugResistanceMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Specific Drug Resistance", (gameworld, name) => new SpecificDrugResistanceMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Specific Drug Resistance", "Gives resistance to a specific drug", new SpecificDrugResistanceMerit().HelpText);
	}

	protected SpecificDrugResistanceMerit(Models.Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		foreach (var item in definition.Element("Resistances").Elements("Resistance"))
		{
			_drugMultipliers[long.Parse(item.Attribute("drug").Value)] = double.Parse(item.Attribute("multiplier").Value);
		}
	}

	protected SpecificDrugResistanceMerit(){}

	protected SpecificDrugResistanceMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Specific Drug Resistance", "@ have|has a bonus to resisting a specific drug")
	{
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("Resistances",
			from item in _drugMultipliers
			select new XElement("Resistance", 
				new XAttribute("drug", item.Key),
				new XAttribute("multiplier", item.Value)
			)
		));
		return root;
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Drug Resistances:");
		sb.AppendLine();
		foreach (var item in _drugMultipliers)
		{
			sb.AppendLine($"\t{Gameworld.Drugs.Get(item.Key)?.Name.ColourValue() ?? "Error".ColourError()}: {item.Value.ToBonusPercentageString(actor)}");
		}
	}

	/// <inheritdoc />
	protected override string SubtypeHelp => $@"{base.SubtypeHelp}
	#3resist <drug> <%>#0 - changes the resistance for a drug type";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "resist":
			case "resistance":
			case "drug":
				return BuildingCommandResistance(actor, command);
		}
		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandResistance(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which drug type would you like to edit the resistances for? The valid values are {Enum.GetValues<DrugType>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
			return false;
		}

		var drug = Gameworld.Drugs.GetByIdOrName(command.PopSpeech());
		if (drug is null)
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid drug. See {"drug list".MXPSend()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How resistant should this merit make an individual? Negative numbers are permissible to make them less resistance. A value of 0 resets to default.");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		_drugMultipliers[drug.Id] = value;
		Changed = true;
		if (value == 0.0)
		{
			_drugMultipliers.Remove(drug.Id);
			actor.OutputHandler.Send($"This merit will no longer change the resistance to the {drug.Name.ColourValue()} drug.");
			return true;
		}

		actor.OutputHandler.Send($"This merit will now confer a {value.ToBonusPercentageString(actor)} resistance to the {drug.Name.ColourValue()} drug.");
		return true;
	}

	private readonly DictionaryWithDefault<long, double> _drugMultipliers = new();

	public double MultiplierForDrug(IDrug drug)
	{
		if (_drugMultipliers.ContainsKey(drug.Id))
		{
			return _drugMultipliers[drug.Id];
		}

		return 1.0;
	}
}

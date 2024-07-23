using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Merits.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Humanizer;
using MudSharp.Character;

namespace MudSharp.RPG.Merits.CharacterMerits;

internal class DrugEffectResistanceMerit : CharacterMeritBase, IDrugEffectResistanceMerit
{
	public static void RegisterMeritInitialiser()
	{
		MeritFactory.RegisterMeritInitialiser("Drug Effect Resistance",
			(merit, gameworld) => new DrugEffectResistanceMerit(merit, gameworld));
		MeritFactory.RegisterBuilderMeritInitialiser("Drug Effect Resistance", (gameworld, name) => new DrugEffectResistanceMerit(gameworld, name));
		MeritFactory.RegisterMeritHelp("Drug Effect Resistance", "Gives resistance to certain drug types", new DrugEffectResistanceMerit().HelpText);
	}

	protected DrugEffectResistanceMerit(Models.Merit merit, IFuturemud gameworld) : base(merit, gameworld)
	{
		var definition = XElement.Parse(merit.Definition);
		foreach (var item in definition.Element("Resistances").Elements("Resistance"))
		{
			DrugResistances[(DrugType)int.Parse(item.Attribute("type").Value)] = double.Parse(item.Attribute("value").Value);
		}
	}

	protected DrugEffectResistanceMerit(){}

	protected DrugEffectResistanceMerit(IFuturemud gameworld, string name) : base(gameworld, name, "Drug Effect Resistance", "@ are|is resistant to certain types of drugs")
	{
		DoDatabaseInsert();
	}

	/// <inheritdoc />
	protected override XElement SaveSubtypeDefinition(XElement root)
	{
		root.Add(new XElement("Resistances",
			from item in DrugResistances
			select new XElement("Resistance", 
				new XAttribute("type", (int)item.Key),
				new XAttribute("value", item.Value)
			)
		));
		return root;
	}

	/// <inheritdoc />
	protected override void SubtypeShow(ICharacter actor, StringBuilder sb)
	{
		sb.AppendLine($"Drug Resistances:");
		sb.AppendLine();
		foreach (var item in DrugResistances)
		{
			sb.AppendLine($"\t{item.Key.DescribeEnum().ColourValue()}: {item.Value.ToBonusPercentageString(actor)}");
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

		if (!command.PopSpeech().TryParseEnum(out DrugType type))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid drug type. The valid values are {Enum.GetValues<DrugType>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
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

		DrugResistances[type] = value;
		Changed = true;
		if (value == 0.0)
		{
			DrugResistances.Remove(type);
			actor.OutputHandler.Send($"This merit will no longer change the resistance to the {type.DescribeEnum().ColourValue()} effect.");
			return true;
		}

		actor.OutputHandler.Send($"This merit will now confer a {value.ToBonusPercentageString(actor)} resistance to the {type.DescribeEnum().ColourValue()} drug type.");
		return true;
	}

	public Dictionary<DrugType, double> DrugResistances { get; } = new();
	IReadOnlyDictionary<DrugType, double> IDrugEffectResistanceMerit.DrugResistances => DrugResistances;
	public double ModifierForDrugType(DrugType drugType)
	{
		if (!DrugResistances.ContainsKey(drugType))
		{
			return 1.0;
		}

		return Math.Max(0.0, 1.0 - DrugResistances[drugType]);
	}
}

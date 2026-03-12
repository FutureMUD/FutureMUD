using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.Health;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class TopicalCreamGameItemComponentProto : GameItemComponentProto
{
	public class CreamDrug
	{
		public IDrug Drug { get; init; }
		public double GramsPerGram { get; set; }
		public double AbsorptionFraction { get; set; }
	}

	public List<CreamDrug> Drugs { get; } = new();
	public double TotalGrams { get; set; }
	public IFutureProg? OnApplyProg { get; set; }

	public override string TypeDescription => "TopicalCream";

	protected TopicalCreamGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected TopicalCreamGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "TopicalCream")
	{
		TotalGrams = 0.01;
		Changed = true;
	}

	protected override void LoadFromXml(XElement root)
	{
		TotalGrams = double.Parse(root.Element("TotalGrams")?.Value ?? "0");
		OnApplyProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnApplyProg")?.Value ?? "0"));
		Drugs.Clear();

		var drugs = root.Element("Drugs");
		if (drugs == null)
		{
			return;
		}

		foreach (var element in drugs.Elements("Drug"))
		{
			var drug = Gameworld.Drugs.Get(long.Parse(element.Attribute("id")?.Value ?? "0"));
			if (drug == null)
			{
				continue;
			}

			Drugs.Add(new CreamDrug
			{
				Drug = drug,
				GramsPerGram = double.Parse(element.Attribute("grams")?.Value ?? "0"),
				AbsorptionFraction = double.Parse(element.Attribute("absorption")?.Value ?? "0")
			});
		}
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("TotalGrams", TotalGrams),
			new XElement("OnApplyProg", OnApplyProg?.Id ?? 0),
			new XElement("Drugs",
				from drug in Drugs
				select new XElement("Drug",
					new XAttribute("id", drug.Drug.Id),
					new XAttribute("grams", drug.GramsPerGram),
					new XAttribute("absorption", drug.AbsorptionFraction)))
		).ToString();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		var drugs = Drugs.Any()
			? Drugs.Select(x =>
					$"{x.Drug.Name.Colour(Telnet.Cyan)} ({x.GramsPerGram:R} g/g, {x.AbsorptionFraction:P0})")
				.ListToString()
			: "none".ColourError();

		return
			$"{"Topical Cream Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})\n\n" +
			$"This topical cream has {Gameworld.UnitManager.DescribeExact(TotalGrams, UnitType.Mass, actor).Colour(Telnet.Green)} available.\n" +
			$"It delivers the following drugs: {drugs}.\n" +
			$"On apply prog: {OnApplyProg?.MXPClickableFunctionNameWithId() ?? "None".ColourError()}";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("topicalcream", true,
			(gameworld, account) => new TopicalCreamGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("cream", false,
			(gameworld, account) => new TopicalCreamGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("TopicalCream",
			(proto, gameworld) => new TopicalCreamGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("TopicalCream",
			$"A cream that can be {"[applied]".Colour(Telnet.Yellow)} to deliver drugs via touch",
			BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new TopicalCreamGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new TopicalCreamGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new TopicalCreamGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description\n\tquantity <weight> - sets total weight of cream\n\tdrug add <which> <grams per gram> <absorption> - adds or edits a drug\n\tdrug remove <which> - removes a drug\n\tprog <which> - sets a prog to execute when the cream is applied\n\tprog clear - removes the on-apply prog";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "quantity":
			case "weight":
				return BuildingCommandQuantity(actor, command);
			case "drug":
				return BuildingCommandDrug(actor, command);
			case "prog":
			case "onapply":
				return BuildingCommandOnApplyProg(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandQuantity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How much cream should this item contain?");
			return false;
		}

		var value = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, out var success);
		if (!success || value <= 0.0)
		{
			actor.Send("That is not a valid amount of cream.");
			return false;
		}

		TotalGrams = value;
		Changed = true;
		actor.Send(
			$"This item will now have {Gameworld.UnitManager.DescribeExact(TotalGrams, UnitType.Mass, actor).Colour(Telnet.Green)} of cream.");
		return true;
	}

	private bool BuildingCommandDrug(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Do you want to add or remove a drug?");
			return false;
		}

		return command.PopSpeech().ToLowerInvariant() switch
		{
			"add" or "set" => BuildingCommandDrugAdd(actor, command),
			"remove" => BuildingCommandDrugRemove(actor, command),
			_ => BuildingCommandDrugFallback(actor)
		};
	}

	private bool BuildingCommandDrugAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which drug do you want to add?");
			return false;
		}

		var drug = Gameworld.Drugs.GetByIdOrName(command.PopSpeech());
		if (drug == null)
		{
			actor.Send("There is no such drug.");
			return false;
		}

		if (!drug.DrugVectors.HasFlag(DrugVector.Touched))
		{
			actor.Send(
				$"You cannot use {drug.Name.Colour(Telnet.Cyan)} because it does not have the 'touched' delivery vector.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send("How many grams of drug per gram of cream?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var grams) || grams <= 0.0)
		{
			actor.Send("You must enter a valid positive amount of drug per gram of cream.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send("What fraction of the drug is absorbed? (0-1)");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var absorption) || absorption < 0.0 || absorption > 1.0)
		{
			actor.Send("You must enter a valid absorption fraction between 0 and 1.");
			return false;
		}

		var existing = Drugs.FirstOrDefault(x => x.Drug == drug);
		if (existing == null)
		{
			Drugs.Add(new CreamDrug { Drug = drug, GramsPerGram = grams, AbsorptionFraction = absorption });
		}
		else
		{
			existing.GramsPerGram = grams;
			existing.AbsorptionFraction = absorption;
		}

		Changed = true;
		actor.Send(
			$"This cream will now deliver {drug.Name.Colour(Telnet.Cyan)} at {grams:R} g/g with {absorption:P0} absorption.");
		return true;
	}

	private bool BuildingCommandDrugRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which drug do you want to remove?");
			return false;
		}

		var drug = Gameworld.Drugs.GetByIdOrName(command.SafeRemainingArgument);
		if (drug == null)
		{
			actor.Send("There is no such drug.");
			return false;
		}

		if (Drugs.RemoveAll(x => x.Drug == drug) == 0)
		{
			actor.Send($"This cream does not currently deliver {drug.Name.Colour(Telnet.Cyan)}.");
			return false;
		}

		Changed = true;
		actor.Send($"This cream will no longer deliver {drug.Name.Colour(Telnet.Cyan)}.");
		return true;
	}

	private bool BuildingCommandDrugFallback(ICharacter actor)
	{
		actor.Send("You must specify whether you want to add or remove a drug.");
		return false;
	}

	private bool BuildingCommandOnApplyProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				$"You must either specify a prog to use for the OnApply prog, or specify {"clear".Colour(Telnet.Yellow)} to clear it.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("clear", "none"))
		{
			OnApplyProg = null;
			Changed = true;
			actor.Send("This cream will no longer execute a prog when it is applied.");
			return true;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog == null)
		{
			actor.Send("There is no such prog to use as an OnApply prog.");
			return false;
		}

		if (!prog.MatchesParameters(new[]
		    {
			    ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Number
		    }))
		{
			actor.Send("The OnApply prog must accept a character, a bodypart name and an amount applied.");
			return false;
		}

		OnApplyProg = prog;
		Changed = true;
		actor.Send(
			$"This cream will now execute the {OnApplyProg.MXPClickableFunctionName()} prog when it is applied.");
		return true;
	}
}

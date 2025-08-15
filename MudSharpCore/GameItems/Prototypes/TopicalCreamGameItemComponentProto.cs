using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.Health;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class TopicalCreamGameItemComponentProto : GameItemComponentProto {
    public class CreamDrug {
        public IDrug Drug { get; init; }
        public double GramsPerGram { get; set; }
        public double AbsorptionFraction { get; set; }
    }

    public List<CreamDrug> Drugs { get; } = new();
    public double TotalGrams { get; set; }

    public override string TypeDescription => "TopicalCream";

    protected override void LoadFromXml(XElement root) {
        TotalGrams = double.Parse(root.Element("TotalGrams")?.Value ?? "0");
        var drugs = root.Element("Drugs");
        if (drugs != null) {
            foreach (var element in drugs.Elements("Drug")) {
                Drugs.Add(new CreamDrug {
                    Drug = Gameworld.Drugs.Get(long.Parse(element.Attribute("id").Value)),
                    GramsPerGram = double.Parse(element.Attribute("grams").Value),
                    AbsorptionFraction = double.Parse(element.Attribute("absorption").Value)
                });
            }
        }
    }

    protected override string SaveToXml() {
        return new XElement("Definition",
            new XElement("TotalGrams", TotalGrams),
            new XElement("Drugs",
                from drug in Drugs
                select new XElement("Drug",
                    new XAttribute("id", drug.Drug?.Id ?? 0),
                    new XAttribute("grams", drug.GramsPerGram),
                    new XAttribute("absorption", drug.AbsorptionFraction)))
        ).ToString();
    }

    public override string ComponentDescriptionOLC(ICharacter actor) {
        var list = Drugs.Any()
            ? Drugs.Select(x => $"{x.Drug?.Name.Colour(Telnet.Cyan) ?? "none"} ({x.GramsPerGram:R}g/g, {x.AbsorptionFraction:P0})").ListToString() : "none";
        return string.Format(actor,
            "{0} (#{1:N0}r{2:N0}, {3})\n\nThis topical cream has {4} remaining and the following drugs: {5}.",
            "Topical Cream Item Component".Colour(Telnet.Cyan),
            Id,
            RevisionNumber,
            Name,
            Gameworld.UnitManager.DescribeExact(TotalGrams, UnitType.Mass, actor).Colour(Telnet.Green),
            list);
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager) {
        manager.AddBuilderLoader("topicalcream", true, (gameworld, account) => new TopicalCreamGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("TopicalCream", (proto, gameworld) => new TopicalCreamGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo("TopicalCream", $"A cream that can be {"[applied]".Colour(Telnet.Yellow)} to deliver drugs via touch", BuildingHelpText);
    }

    protected TopicalCreamGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld) { }

    protected TopicalCreamGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "TopicalCream") {
        TotalGrams = 0.01;
        Changed = true;
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false) {
        return new TopicalCreamGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent) {
        return new TopicalCreamGameItemComponent(component, this, parent);
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator) {
        return CreateNewRevision(initiator, (proto, gameworld) => new TopicalCreamGameItemComponentProto(proto, gameworld));
    }

    private const string BuildingHelpText =
        "You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description\n\tquantity <weight> - sets total weight of cream\n\tdrug add <which> <grams per gram> <absorption> - adds or edits a drug\n\tdrug remove <which> - removes a drug";

    public override string ShowBuildingHelp => BuildingHelpText;

    public override bool BuildingCommand(ICharacter actor, StringStack command) {
        switch (command.Pop().ToLowerInvariant()) {
            case "quantity":
            case "weight":
                return BuildingCommandQuantity(actor, command);
            case "drug":
                return BuildingCommandDrug(actor, command);
            default:
                return base.BuildingCommand(actor, command);
        }
    }

    private bool BuildingCommandQuantity(ICharacter actor, StringStack command) {
        if (command.IsFinished) {
            actor.Send("How much cream should this item contain?");
            return false;
        }
        var value = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, out var success);
        if (!success || value <= 0) {
            actor.Send("That is not a valid amount of cream.");
            return false;
        }
        TotalGrams = value;
        Changed = true;
        actor.Send($"This item will now have {Gameworld.UnitManager.DescribeExact(TotalGrams, UnitType.Mass, actor).Colour(Telnet.Green)} of cream.");
        return true;
    }

    private bool BuildingCommandDrug(ICharacter actor, StringStack command) {
        if (command.IsFinished) {
            actor.Send("Do you want to add or remove a drug?");
            return false;
        }
        var sub = command.Pop().ToLowerInvariant();
        if (sub == "add" || sub == "set") {
            if (command.IsFinished) { actor.Send("Which drug do you want to add?"); return false; }
            var drugText = command.PopSpeech();
            var drug = long.TryParse(drugText, out var value) ? Gameworld.Drugs.Get(value) : Gameworld.Drugs.GetByName(drugText);
            if (drug == null) { actor.Send("There is no such drug."); return false; }
            if (!drug.DrugVectors.HasFlag(DrugVector.Touched)) {
                actor.Send($"You cannot use {drug.Name.Colour(Telnet.Cyan)} because it does not have the 'touched' delivery vector.");
                return false;
            }
            if (command.IsFinished) { actor.Send("How many grams of drug per gram of cream?"); return false; }
            var grams = double.Parse(command.PopSpeech());
            if (command.IsFinished) { actor.Send("What fraction of the drug is absorbed? (0-1)"); return false; }
            var absorption = double.Parse(command.PopSpeech());
            var existing = Drugs.FirstOrDefault(x => x.Drug == drug);
            if (existing == null) {
                Drugs.Add(new CreamDrug { Drug = drug, GramsPerGram = grams, AbsorptionFraction = absorption });
            } else {
                existing.GramsPerGram = grams;
                existing.AbsorptionFraction = absorption;
            }
            Changed = true;
            actor.Send($"This cream will now deliver {drug.Name.Colour(Telnet.Cyan)} at {grams:R}g/g with {absorption:P0} absorption.");
            return true;
        }
        if (sub == "remove") {
            var drugText = command.SafeRemainingArgument;
            var drug = long.TryParse(drugText, out var value) ? Gameworld.Drugs.Get(value) : Gameworld.Drugs.GetByName(drugText);
            if (drug == null) { actor.Send("There is no such drug."); return false; }
            Drugs.RemoveAll(x => x.Drug == drug);
            Changed = true;
            actor.Send($"This cream will no longer deliver {drug.Name.Colour(Telnet.Cyan)}.");
            return true;
        }
        actor.Send("You must specify add or remove.");
        return false;
    }
}

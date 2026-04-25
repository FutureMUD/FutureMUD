#nullable enable
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using System;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class FaxMachineGameItemComponentProto : TelephoneGameItemComponentProto
{
    public FaxMachineGameItemComponentProto(IFuturemud gameworld, IAccount originator)
        : base(gameworld, originator, "FaxMachine")
    {
        InkCartridgePrototypeId = 0;
        SpentInkCartridgePrototypeId = 0;
        MaximumCharactersPrintedPerCartridge = 2400000;
        PaperWeightCapacity = 1.5 / Gameworld.UnitManager.BaseWeightToKilograms;
    }

    public FaxMachineGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld)
        : base(proto, gameworld)
    {
    }

    public override string TypeDescription => "Fax Machine";
    public long InkCartridgePrototypeId { get; protected set; }
    public long SpentInkCartridgePrototypeId { get; protected set; }
    public int MaximumCharactersPrintedPerCartridge { get; protected set; }
    public double PaperWeightCapacity { get; protected set; }

    protected override void LoadFromXml(XElement root)
    {
        base.LoadFromXml(root);
        InkCartridgePrototypeId = long.Parse(root.Element("InkCartridgePrototypeId")?.Value ?? "0");
        SpentInkCartridgePrototypeId = long.Parse(root.Element("SpentInkCartridgePrototypeId")?.Value ?? "0");
        MaximumCharactersPrintedPerCartridge =
            int.Parse(root.Element("MaximumCharactersPrintedPerCartridge")?.Value ?? "2400000");
        PaperWeightCapacity = double.Parse(root.Element("PaperWeightCapacity")?.Value ?? "0");
    }

    protected override string SaveToXml()
    {
        XElement root = XElement.Parse(base.SaveToXml());
        root.Add(new XElement("InkCartridgePrototypeId", InkCartridgePrototypeId));
        root.Add(new XElement("SpentInkCartridgePrototypeId", SpentInkCartridgePrototypeId));
        root.Add(new XElement("MaximumCharactersPrintedPerCartridge", MaximumCharactersPrintedPerCartridge));
        root.Add(new XElement("PaperWeightCapacity", PaperWeightCapacity));
        return root.ToString();
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
    {
        return new FaxMachineGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
    {
        return new FaxMachineGameItemComponent(component, this, parent);
    }

    public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("faxmachine", true,
            (gameworld, account) => new FaxMachineGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("FaxMachine",
            (proto, gameworld) => new FaxMachineGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo(
            "FaxMachine",
            $"Connects an item to a {"[telecommunications grid]".Colour(Telnet.BoldBlue)} and lets it send readable documents as faxes while printing or queueing incoming faxes.",
            BuildingHelpText
        );
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new FaxMachineGameItemComponentProto(proto, gameworld));
    }

    private const string BuildingHelpText =
        "\tink <proto> - sets an item proto to be the ink cartridge\n\tspent <proto> - sets an item proto to be loaded as the spent ink cartridge\n\tuses <amount> - the number of characters of fax output to be printed by a full cartridge\n\tpaper <weight> - the weight of paper this fax machine can hold";

    public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}\n{BuildingHelpText}";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "ink":
                return BuildingCommandInkCartridge(actor, command);
            case "spent":
                return BuildingCommandSpentCartridge(actor, command);
            case "uses":
                return BuildingCommandUses(actor, command);
            case "paper":
            case "capacity":
                return BuildingCommandCapacity(actor, command);
            default:
                return base.BuildingCommand(actor,
                    new StringStack($"\"{command.Last}\" {command.RemainingArgument}".Trim()));
        }
    }

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return
            $"{base.ComponentDescriptionOLC(actor)}\nIt can hold {Gameworld.UnitManager.DescribeMostSignificantExact(PaperWeightCapacity, Framework.Units.UnitType.Mass, actor).ColourValue()} of paper.\nIt uses {Gameworld.ItemProtos.Get(InkCartridgePrototypeId)?.EditHeader() ?? "Not yet set".Colour(Telnet.Red)} as an ink cartridge.\nIt uses {Gameworld.ItemProtos.Get(SpentInkCartridgePrototypeId)?.EditHeader() ?? "Not yet set".Colour(Telnet.Red)} as a spent ink cartridge.\nIt prints {MaximumCharactersPrintedPerCartridge.ToString("N0", actor).ColourValue()} characters per cartridge refill (approximately {(MaximumCharactersPrintedPerCartridge / 2400).ToString("N0", actor).ColourValue()} pages).";
    }

    public override bool CanSubmit()
    {
        if (InkCartridgePrototypeId == 0)
        {
            return false;
        }

        return base.CanSubmit();
    }

    public override string WhyCannotSubmit()
    {
        if (InkCartridgePrototypeId == 0)
        {
            return "You must set an ink cartridge prototype.";
        }

        return base.WhyCannotSubmit();
    }

    private bool BuildingCommandInkCartridge(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which item prototype should serve as the ink cartridge for this fax machine?");
            return false;
        }

        if (!long.TryParse(command.PopSpeech(), out long value))
        {
            actor.OutputHandler.Send("You must enter a valid ID number.");
            return false;
        }

        IGameItemProto proto = Gameworld.ItemProtos.Get(value);
        if (proto == null)
        {
            actor.OutputHandler.Send("There is no such item prototype.");
            return false;
        }

        if (proto.Status != RevisionStatus.Current)
        {
            actor.OutputHandler.Send(
                "There is no currently approved version of that prototype, and it is therefore invalid.");
            return false;
        }

        InkCartridgePrototypeId = proto.Id;
        Changed = true;
        actor.OutputHandler.Send(
            $"This fax machine will now consume instances of {proto.EditHeader()} to refill its ink.");
        return true;
    }

    private bool BuildingCommandSpentCartridge(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                "Which item prototype should serve as the spent ink cartridge for this fax machine?");
            return false;
        }

        if (!long.TryParse(command.PopSpeech(), out long value))
        {
            actor.OutputHandler.Send("You must enter a valid ID number.");
            return false;
        }

        IGameItemProto proto = Gameworld.ItemProtos.Get(value);
        if (proto == null)
        {
            actor.OutputHandler.Send("There is no such item prototype.");
            return false;
        }

        if (proto.Status != RevisionStatus.Current)
        {
            actor.OutputHandler.Send(
                "There is no currently approved version of that prototype, and it is therefore invalid.");
            return false;
        }

        SpentInkCartridgePrototypeId = proto.Id;
        Changed = true;
        actor.OutputHandler.Send(
            $"This fax machine will now produce instances of {proto.EditHeader()} when its spent ink cartridges are removed.");
        return true;
    }

    private bool BuildingCommandUses(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                "How many characters of basic text should this fax machine be able to print before requiring an ink cartridge refill?");
            return false;
        }

        if (!int.TryParse(command.PopSpeech(), out int value) || value <= 0)
        {
            actor.OutputHandler.Send("You must enter a valid number.");
            return false;
        }

        MaximumCharactersPrintedPerCartridge = value;
        Changed = true;
        actor.OutputHandler.Send(
            $"This fax machine will now have {MaximumCharactersPrintedPerCartridge.ToString("N0", actor).ColourValue()} uses between ink cartridge refills (approximately {(MaximumCharactersPrintedPerCartridge / 2400).ToString("N0", actor).ColourValue()} pages).");
        return true;
    }

    private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("How much weight of paper should this fax machine be able to hold?");
            return false;
        }

        double amount = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, Framework.Units.UnitType.Mass,
            out bool success);
        if (!success)
        {
            actor.OutputHandler.Send("That is not a valid weight.");
            return false;
        }

        PaperWeightCapacity = amount;
        Changed = true;
        actor.OutputHandler.Send(
            $"This fax machine can now hold {Gameworld.UnitManager.DescribeMostSignificantExact(PaperWeightCapacity, Framework.Units.UnitType.Mass, actor).ColourValue()} of paper.");
        return true;
    }
}

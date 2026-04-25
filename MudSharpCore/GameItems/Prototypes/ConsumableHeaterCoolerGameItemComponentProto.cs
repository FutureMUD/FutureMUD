#nullable enable
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class ConsumableHeaterCoolerGameItemComponentProto : ThermalSourceGameItemComponentProto
{
    protected ConsumableHeaterCoolerGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
        originator, "ConsumableHeaterCooler")
    {
        SecondsOfFuel = 600;
        FuelExpendedEcho = "$0 have|has burned out.";
        Changed = true;
    }

    protected ConsumableHeaterCoolerGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
        proto, gameworld)
    {
    }

    public override string TypeDescription => "ConsumableHeaterCooler";
    public int SecondsOfFuel { get; protected set; }
    public IGameItemProto? SpentItemProto { get; protected set; }
    public string FuelExpendedEcho { get; protected set; } = string.Empty;

    protected override void LoadFromXml(XElement root)
    {
        LoadThermalDefinitionFromXml(root);
        SecondsOfFuel = int.Parse(root.Element("SecondsOfFuel")?.Value ?? "600");
        SpentItemProto = Gameworld.ItemProtos.Get(long.Parse(root.Element("SpentItemProto")?.Value ?? "0"));
        FuelExpendedEcho = root.Element("FuelExpendedEcho")?.Value ?? "$0 have|has burned out.";
    }

    protected override string SaveToXml()
    {
        return SaveThermalDefinitionToXml(new XElement("Definition",
            new XElement("SecondsOfFuel", SecondsOfFuel),
            new XElement("SpentItemProto", SpentItemProto?.Id ?? 0),
            new XElement("FuelExpendedEcho", new XCData(FuelExpendedEcho)))).ToString();
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
    {
        return new ConsumableHeaterCoolerGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
    {
        return new ConsumableHeaterCoolerGameItemComponent(component, this, parent);
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("consumableheatercooler", true,
            (gameworld, account) => new ConsumableHeaterCoolerGameItemComponentProto(gameworld, account));
        manager.AddBuilderLoader("consumable heater cooler", false,
            (gameworld, account) => new ConsumableHeaterCoolerGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("ConsumableHeaterCooler",
            (proto, gameworld) => new ConsumableHeaterCoolerGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo(
            "ConsumableHeaterCooler",
            $"A temporary thermal source that auto-starts and then burns away",
            BuildingHelpText
        );
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new ConsumableHeaterCoolerGameItemComponentProto(proto, gameworld));
    }

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "ambient":
            case "intimate":
            case "immediate":
            case "proximate":
            case "proximity":
            case "distant":
            case "verydistant":
            case "very-distant":
            case "very_distant":
                return BuildingCommandThermalProfile(actor, command.Last, command);
            case "activedesc":
                return BuildingCommandStateDescription(actor, true, command);
            case "inactivedesc":
                return BuildingCommandStateDescription(actor, false, command);
            case "fuel":
            case "duration":
            case "seconds":
                return BuildingCommandFuel(actor, command);
            case "spent":
            case "spentitem":
                return BuildingCommandSpent(actor, command);
            case "expended":
            case "expired":
                return BuildingCommandExpended(actor, command);
            default:
                return base.BuildingCommand(actor, command.GetUndo());
        }
    }

    private bool BuildingCommandFuel(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("How many seconds should this consumable source remain active?");
            return false;
        }

        if (!int.TryParse(command.PopSpeech(), out int value) || value < 1)
        {
            actor.OutputHandler.Send("You must enter a positive number of seconds.");
            return false;
        }

        SecondsOfFuel = value;
        Changed = true;
        actor.OutputHandler.Send(
            $"This consumable source will now burn for {SecondsOfFuel.ToString("N0", actor).ColourValue()} seconds ({TimeSpan.FromSeconds(SecondsOfFuel).Describe(actor)}).");
        return true;
    }

    private bool BuildingCommandSpent(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which item prototype should this become when spent? Use none to delete it instead.");
            return false;
        }

        if (command.PeekSpeech().EqualToAny("none", "clear"))
        {
            SpentItemProto = null;
            Changed = true;
            actor.OutputHandler.Send("This consumable source will now simply delete itself when spent.");
            return true;
        }

        IGameItemProto? proto = long.TryParse(command.SafeRemainingArgument, out long value)
            ? Gameworld.ItemProtos.Get(value)
            : Gameworld.ItemProtos.GetByName(command.SafeRemainingArgument);
        if (proto is null)
        {
            actor.OutputHandler.Send("There is no such item prototype.");
            return false;
        }

        SpentItemProto = proto;
        Changed = true;
        actor.OutputHandler.Send($"This consumable source will now become {proto.EditHeader().ColourName()} when spent.");
        return true;
    }

    private bool BuildingCommandExpended(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What emote should be shown when this consumable source is spent?");
            return false;
        }

        Emote emote = new(command.SafeRemainingArgument, actor, actor);
        if (!emote.Valid)
        {
            actor.OutputHandler.Send(emote.ErrorMessage);
            return false;
        }

        FuelExpendedEcho = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send($"That spent emote is now {FuelExpendedEcho.ColourCommand()}.");
        return true;
    }

    private const string BuildingHelpText =
        "You can use the following options with this component:\n\t#3name <name>#0 - sets the name of the component\n\t#3desc <desc>#0 - sets the description of the component\n\t#3fuel <seconds>#0 - sets the burn duration\n\t#3spent <item proto>|none#0 - sets the replacement item when spent\n\t#3expended <emote>#0 - sets the spent emote" +
        ThermalBuildingHelpText;

    public override string ShowBuildingHelp => BuildingHelpText;

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return
            $"{"Consumable Heater/Cooler Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})\n\nThis is a consumable thermal source that auto-starts and burns for {TimeSpan.FromSeconds(SecondsOfFuel).Describe(actor)}.\nThermal Profile: {ThermalProfileDisplay(actor)}\nSpent Item: {SpentItemProto?.EditHeader().ColourName() ?? "None".ColourError()}\nSpent Emote: {FuelExpendedEcho.ColourCommand()}";
    }
}

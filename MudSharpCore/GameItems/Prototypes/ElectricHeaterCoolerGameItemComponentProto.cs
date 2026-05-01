#nullable enable
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class ElectricHeaterCoolerGameItemComponentProto : SwitchableThermalSourceGameItemComponentProto, IConsumePowerPrototype
{
    protected ElectricHeaterCoolerGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
        originator, "ElectricHeaterCooler")
    {
        Wattage = 650.0;
        Changed = true;
    }

    protected ElectricHeaterCoolerGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
        proto, gameworld)
    {
    }

    public override string TypeDescription => "ElectricHeaterCooler";
    public double Wattage { get; protected set; }

    protected override void LoadFromXml(XElement root)
    {
        LoadSwitchableThermalDefinitionFromXml(root);
        Wattage = double.Parse(root.Element("Wattage")?.Value ?? "650");
    }

    protected override string SaveToXml()
    {
        return SaveSwitchableThermalDefinitionToXml(new XElement("Definition",
            new XElement("Wattage", Wattage))).ToString();
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
    {
        return new ElectricHeaterCoolerGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
    {
        return new ElectricHeaterCoolerGameItemComponent(component, this, parent);
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("electricheatercooler", true,
            (gameworld, account) => new ElectricHeaterCoolerGameItemComponentProto(gameworld, account));
        manager.AddBuilderLoader("electric heater cooler", false,
            (gameworld, account) => new ElectricHeaterCoolerGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("ElectricHeaterCooler",
            (proto, gameworld) => new ElectricHeaterCoolerGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo(
            "ElectricHeaterCooler",
            $"A powered thermal source that alters indoor room temperature and nearby targets",
            BuildingHelpText
        );
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new ElectricHeaterCoolerGameItemComponentProto(proto, gameworld));
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
            case "onemote":
                return BuildingCommandSwitchEmote(actor, true, command);
            case "offemote":
                return BuildingCommandSwitchEmote(actor, false, command);
            case "watts":
            case "wattage":
            case "watt":
                return BuildingCommandWattage(actor, command);
            default:
                return base.BuildingCommand(actor, command.GetUndo());
        }
    }

    private bool BuildingCommandWattage(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("How many watts should this heater/cooler draw while switched on?");
            return false;
        }

        if (!double.TryParse(command.PopSpeech(), out double value) || value < 0.0)
        {
            actor.OutputHandler.Send("You must enter a non-negative wattage.");
            return false;
        }

        Wattage = value;
        Changed = true;
        actor.OutputHandler.Send($"This heater/cooler now draws {Wattage.ToString("N2", actor).ColourValue()} watts while switched on.");
        return true;
    }

    private const string BuildingHelpText =
        "You can use the following options with this component:\n\t#3name <name>#0 - sets the name of the component\n\t#3desc <desc>#0 - sets the description of the component\n\t#3wattage <watts>#0 - sets the electrical draw" +
        SwitchableThermalBuildingHelpText;

    public override string ShowBuildingHelp => BuildingHelpText;

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return
            $"{"Electric Heater/Cooler Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})\n\nThis is an electric thermal source that draws {Wattage.ToString("N2", actor).ColourValue()} watts while switched on.\nThermal Profile: {ThermalProfileDisplay(actor)}\nSwitch On Emote: {SwitchOnEmote.ColourCommand()}\nSwitch Off Emote: {SwitchOffEmote.ColourCommand()}";
    }
}

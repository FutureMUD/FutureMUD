#nullable enable
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using System;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class SolidFuelHeaterCoolerGameItemComponentProto : SwitchableThermalSourceGameItemComponentProto
{
    protected SolidFuelHeaterCoolerGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
        originator, "SolidFuelHeaterCooler")
    {
        MaximumFuelWeight = 25.0;
        SecondsPerUnitWeight = 600.0;
        Changed = true;
    }

    protected SolidFuelHeaterCoolerGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
        proto, gameworld)
    {
    }

    public override string TypeDescription => "SolidFuelHeaterCooler";
    public ITag? FuelTag { get; protected set; }
    public double MaximumFuelWeight { get; protected set; }
    public double SecondsPerUnitWeight { get; protected set; }

    protected override void LoadFromXml(XElement root)
    {
        LoadSwitchableThermalDefinitionFromXml(root);
        FuelTag = Gameworld.Tags.Get(long.Parse(root.Element("FuelTag")?.Value ?? "0"));
        MaximumFuelWeight = double.Parse(root.Element("MaximumFuelWeight")?.Value ?? "25");
        SecondsPerUnitWeight = double.Parse(root.Element("SecondsPerUnitWeight")?.Value ?? "600");
    }

    protected override string SaveToXml()
    {
        return SaveSwitchableThermalDefinitionToXml(new XElement("Definition",
            new XElement("FuelTag", FuelTag?.Id ?? 0),
            new XElement("MaximumFuelWeight", MaximumFuelWeight),
            new XElement("SecondsPerUnitWeight", SecondsPerUnitWeight))).ToString();
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
    {
        return new SolidFuelHeaterCoolerGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
    {
        return new SolidFuelHeaterCoolerGameItemComponent(component, this, parent);
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("solidfuelheatercooler", true,
            (gameworld, account) => new SolidFuelHeaterCoolerGameItemComponentProto(gameworld, account));
        manager.AddBuilderLoader("solid fuel heater cooler", false,
            (gameworld, account) => new SolidFuelHeaterCoolerGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("SolidFuelHeaterCooler",
            (proto, gameworld) => new SolidFuelHeaterCoolerGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo(
            "SolidFuelHeaterCooler",
            $"A switchable thermal source that burns tagged solid fuel items one at a time",
            BuildingHelpText
        );
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new SolidFuelHeaterCoolerGameItemComponentProto(proto, gameworld));
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
            case "fueltag":
            case "tag":
                return BuildingCommandFuelTag(actor, command);
            case "capacity":
            case "maxfuel":
                return BuildingCommandCapacity(actor, command);
            case "rate":
            case "secondsperweight":
                return BuildingCommandSecondsPerWeight(actor, command);
            default:
                return base.BuildingCommand(actor, command.GetUndo());
        }
    }

    private bool BuildingCommandFuelTag(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which item tag should count as valid fuel?");
            return false;
        }

        ITag? tag = long.TryParse(command.SafeRemainingArgument, out long value)
            ? Gameworld.Tags.Get(value)
            : Gameworld.Tags.GetByName(command.SafeRemainingArgument);
        if (tag is null)
        {
            actor.OutputHandler.Send("There is no such tag.");
            return false;
        }

        FuelTag = tag;
        Changed = true;
        actor.OutputHandler.Send($"This heater/cooler now accepts fuel tagged {tag.Name.ColourName()}.");
        return true;
    }

    private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What maximum fuel weight should it hold?");
            return false;
        }

        if (!double.TryParse(command.PopSpeech(), out double value) || value <= 0.0)
        {
            actor.OutputHandler.Send("You must enter a positive weight.");
            return false;
        }

        MaximumFuelWeight = value;
        Changed = true;
        actor.OutputHandler.Send($"This heater/cooler now holds up to {MaximumFuelWeight.ToString("N2", actor).ColourValue()} weight units of fuel.");
        return true;
    }

    private bool BuildingCommandSecondsPerWeight(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("How many seconds should one unit of fuel weight burn for?");
            return false;
        }

        if (!double.TryParse(command.PopSpeech(), out double value) || value <= 0.0)
        {
            actor.OutputHandler.Send("You must enter a positive number of seconds.");
            return false;
        }

        SecondsPerUnitWeight = value;
        Changed = true;
        actor.OutputHandler.Send($"Fuel will now burn for {SecondsPerUnitWeight.ToString("N2", actor).ColourValue()} seconds per unit weight.");
        return true;
    }

    private const string BuildingHelpText =
        "You can use the following options with this component:\n\t#3name <name>#0 - sets the name of the component\n\t#3desc <desc>#0 - sets the description of the component\n\t#3fueltag <tag>#0 - sets the tag of valid fuel items\n\t#3capacity <weight>#0 - sets the maximum stored fuel weight\n\t#3secondsperweight <seconds>#0 - sets burn time per unit weight" +
        SwitchableThermalBuildingHelpText;

    public override string ShowBuildingHelp => BuildingHelpText;

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return
            $"{"Solid Fuel Heater/Cooler Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})\n\nThis is a solid-fuel thermal source that accepts items tagged {FuelTag?.Name.ColourName() ?? "None".ColourError()} and burns them for {SecondsPerUnitWeight.ToString("N2", actor).ColourValue()} seconds per unit weight.\nMaximum Fuel Weight: {MaximumFuelWeight.ToString("N2", actor).ColourValue()}\nThermal Profile: {ThermalProfileDisplay(actor)}";
    }
}

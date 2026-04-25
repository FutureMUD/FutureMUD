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

public class CellPhoneTowerGameItemComponentProto : GameItemComponentProto
{
    public override string TypeDescription => "CellPhoneTower";
    public double Wattage { get; set; }

    protected CellPhoneTowerGameItemComponentProto(IFuturemud gameworld, IAccount originator)
        : base(gameworld, originator, "CellPhoneTower")
    {
        Wattage = 250.0;
    }

    protected CellPhoneTowerGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld)
        : base(proto, gameworld)
    {
    }

    protected override void LoadFromXml(XElement root)
    {
        Wattage = double.Parse(root.Element("Wattage")?.Value ?? "250.0");
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition", new XElement("Wattage", Wattage)).ToString();
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
    {
        return new CellPhoneTowerGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
    {
        return new CellPhoneTowerGameItemComponent(component, this, parent);
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("CellPhoneTower".ToLowerInvariant(), true,
            (gameworld, account) => new CellPhoneTowerGameItemComponentProto(gameworld, account));
        manager.AddBuilderLoader("Cell Phone Tower".ToLowerInvariant(), false,
            (gameworld, account) => new CellPhoneTowerGameItemComponentProto(gameworld, account));
        manager.AddBuilderLoader("CellTower".ToLowerInvariant(), false,
            (gameworld, account) => new CellPhoneTowerGameItemComponentProto(gameworld, account));
        manager.AddBuilderLoader("Cell Tower".ToLowerInvariant(), false,
            (gameworld, account) => new CellPhoneTowerGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("CellPhoneTower",
            (proto, gameworld) => new CellPhoneTowerGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo(
            "CellPhoneTower",
            $"A {"[telecommunications grid]".Colour(Telnet.BoldBlue)} antenna that provides cellular service to its {"[zone]".Colour(Telnet.BoldGreen)}",
            BuildingHelpText
        );
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new CellPhoneTowerGameItemComponentProto(proto, gameworld));
    }

    private const string BuildingHelpText =
        "You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\twatts <#> - sets how much power the tower draws while switched on";

    public override string ShowBuildingHelp => BuildingHelpText;

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "watts":
            case "watt":
            case "wattage":
                return BuildingCommandWatts(actor, command);
            default:
                return base.BuildingCommand(actor, command);
        }
    }

    private bool BuildingCommandWatts(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("How many watts should this cell tower draw while switched on?");
            return false;
        }

        if (!double.TryParse(command.PopSpeech(), out double value) || value < 0.0)
        {
            actor.Send("You must enter a non-negative number of watts.");
            return false;
        }

        Wattage = value;
        Changed = true;
        actor.Send($"This cell tower will now draw {Wattage:N2} watts while switched on.");
        return true;
    }

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return string.Format(actor,
            "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a cell phone tower that draws {4:N2} watts while switched on.",
            "Cell Phone Tower Game Item Component".Colour(Telnet.Cyan),
            Id,
            RevisionNumber,
            Name,
            Wattage);
    }
}

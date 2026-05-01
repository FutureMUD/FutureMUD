using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

#nullable enable annotations

namespace MudSharp.GameItems.Prototypes;

public class GridPowerSupplyGameItemComponentProto : GameItemComponentProto, IProducePowerPrototype, IConsumePowerPrototype, ICanConnectToElectricalGridPrototype
{
    public override string TypeDescription => "GridPowerSupply";
    public double Wattage { get; set; }

    #region Constructors

    protected GridPowerSupplyGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
        originator, "GridPowerSupply")
    {
        Wattage = 20.0;
    }

    protected GridPowerSupplyGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
        : base(proto, gameworld)
    {
    }

    protected override void LoadFromXml(XElement root)
    {
        Wattage = double.Parse(root.Element("Wattage")?.Value ?? "20.0");
    }

    #endregion

    #region Saving

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("Wattage", Wattage)
        ).ToString();
    }

    #endregion

    #region Component Instance Initialising Functions

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
    {
        return new GridPowerSupplyGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
    {
        return new GridPowerSupplyGameItemComponent(component, this, parent);
    }

    #endregion

    #region Initialisation Tasks

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("GridPowerSupply".ToLowerInvariant(), true,
            (gameworld, account) => new GridPowerSupplyGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("GridPowerSupply",
            (proto, gameworld) => new GridPowerSupplyGameItemComponentProto(proto, gameworld));

        manager.AddTypeHelpInfo(
            "GridPowerSupply",
            $"This item {"[provides power]".Colour(Telnet.BoldMagenta)} directly from an {"[electric grid]".Colour(Telnet.BoldOrange)}. Can combine with a {"[connectable]".Colour(Telnet.BoldBlue)} to create a power point",
            BuildingHelpText
        );
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new GridPowerSupplyGameItemComponentProto(proto, gameworld));
    }

    #endregion

    #region Building Commands

    private const string BuildingHelpText =
        "You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\twatts <#> - sets the maximum wattage the item can draw from the grid";

    public override string ShowBuildingHelp => BuildingHelpText;

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "watts":
            case "watt":
            case "wattage":
                return BuildingCommandWattage(actor, command);
            default:
                return base.BuildingCommand(actor, command);
        }
    }

    private bool BuildingCommandWattage(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("How many watts should this grid power supply be able to provide?");
            return false;
        }

        if (!double.TryParse(command.PopSpeech(), out double value))
        {
            actor.Send("You must enter a valid number of watts.");
            return false;
        }

        if (value < 0.0)
        {
            actor.Send("You must enter a non-negative wattage.");
            return false;
        }

        Wattage = value;
        Changed = true;
        actor.Send($"This item will now be able to provide up to {Wattage:N2} watts from the grid.");
        return true;
    }

    #endregion

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return string.Format(actor,
            "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item supplies up to {4:N2} watts directly from the grid.",
            "GridPowerSupply Game Item Component".Colour(Telnet.Cyan),
            Id,
            RevisionNumber,
            Name,
            Wattage
        );
    }
}

#nullable enable
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class GridLiquidSourceGameItemComponentProto : GameItemComponentProto, IConnectableItemProto
{
    public override string TypeDescription => "GridLiquidSource";
    public List<ConnectorType> Connections { get; } = [];
    IEnumerable<ConnectorType> IConnectableItemProto.Connections => Connections;

    protected GridLiquidSourceGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
        originator, "GridLiquidSource")
    {
        Connections.Add(new ConnectorType(Form.Shape.Gender.Male, "LiquidLine", false));
    }

    protected GridLiquidSourceGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld)
        : base(proto, gameworld)
    {
    }

    protected override void LoadFromXml(System.Xml.Linq.XElement root)
    {
        Connections.Clear();
        XElement? element = root.Element("Connectors");
        if (element != null)
        {
            foreach (XElement item in element.Elements("Connection"))
            {
                Connections.Add(new ConnectorType((Gender)Convert.ToSByte(item.Attribute("gender")!.Value),
                    item.Attribute("type")!.Value, bool.Parse(item.Attribute("powered")!.Value)));
            }
        }
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("Connectors",
                from connector in Connections
                select new XElement("Connection",
                    new XAttribute("gender", (short)connector.Gender),
                    new XAttribute("type", connector.ConnectionType),
                    new XAttribute("powered", connector.Powered)))
        ).ToString();
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
    {
        return new GridLiquidSourceGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
    {
        return new GridLiquidSourceGameItemComponent(component, this, parent);
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("GridLiquidSource".ToLowerInvariant(), true,
            (gameworld, account) => new GridLiquidSourceGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("GridLiquidSource",
            (proto, gameworld) => new GridLiquidSourceGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo(
            "GridLiquidSource",
            $"Supplies liquid from a {"[liquid grid]".Colour(Telnet.BoldCyan)} through the existing {"[liquid container]".Colour(Telnet.BoldBlue)} interaction surface and can expose a physical connector",
            BuildingHelpText
        );
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new GridLiquidSourceGameItemComponentProto(proto, gameworld));
    }

    private const string BuildingHelpText =
        "You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tconnection add <gender> <type> <powered> - adds a connection type\n\tconnection remove <gender> <type> - removes a connection type";

    public override string ShowBuildingHelp => BuildingHelpText;

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "connection":
            case "connector":
            case "connections":
            case "connectors":
                return BuildingCommandConnection(actor, command);
            default:
                return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"));
        }
    }

    private bool BuildingCommandConnection(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("Do you want to add or remove a connection type for this liquid source?");
            return false;
        }

        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "add":
                return BuildingCommandConnectionAdd(actor, command);
            case "remove":
            case "rem":
            case "del":
            case "delete":
                return BuildingCommandConnectionRemove(actor, command);
        }

        actor.Send("Do you want to add or remove a connection type for this liquid source?");
        return false;
    }

    private bool BuildingCommandConnectionAdd(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What gender of connection do you want to add?");
            return false;
        }

        Gendering gendering = Gendering.Get(command.PopSpeech());
        if (gendering.Enum == Form.Shape.Gender.Indeterminate)
        {
            actor.Send("You can either set the connection type to male, female or neuter.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.Send("What type of connection do you want this connector to be?");
            return false;
        }

        string type = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.Send("Should the connection be powered?");
            return false;
        }

        if (!bool.TryParse(command.PopSpeech(), out bool powered))
        {
            actor.Send("Should the connection be powered? You must answer true or false.");
            return false;
        }

        if (gendering.Enum == Form.Shape.Gender.Neuter && powered)
        {
            actor.Send("Ungendered connections cannot also be powered.");
            return false;
        }

        Connections.Add(new ConnectorType(gendering.Enum, type, powered));
        actor.Send(
            $"This liquid source will now have an additional {(powered ? "powered" : "unpowered")} connection of type {type.Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
        Changed = true;
        return true;
    }

    private bool BuildingCommandConnectionRemove(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What gender of connection do you want to remove?");
            return false;
        }

        Gendering gendering = Gendering.Get(command.PopSpeech());
        if (gendering.Enum == Form.Shape.Gender.Indeterminate)
        {
            actor.Send("Connection types can be male, female or neuter.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.Send("What type of connection do you want to remove?");
            return false;
        }

        string type = command.SafeRemainingArgument;
        ConnectorType? connector = Connections.FirstOrDefault(x =>
            x.ConnectionType.Equals(type, StringComparison.InvariantCultureIgnoreCase) && x.Gender == gendering.Enum);
        if (connector == null)
        {
            actor.Send("There is no connection like that to remove.");
            return false;
        }

        Connections.Remove(connector);
        actor.Send(
            $"This liquid source now has one fewer connection of type {type.TitleCase().Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
        Changed = true;
        return true;
    }

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return string.Format(actor,
            "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item supplies liquid directly from a connected liquid grid.\r\nIt has the following connections: {4}.",
            "GridLiquidSource Game Item Component".Colour(Telnet.Cyan),
            Id,
            RevisionNumber,
            Name,
            Connections.Select(x => $"{x.ConnectionType.Colour(Telnet.Green)} {(x.Powered ? "[P]" : "")} ({Gendering.Get(x.Gender).GenderClass(true).Proper().Colour(Telnet.Green)})").ListToString()
        );
    }

    public override bool CanSubmit()
    {
        return Connections.Any() && base.CanSubmit();
    }

    public override string WhyCannotSubmit()
    {
        return Connections.Any()
            ? base.WhyCannotSubmit()
            : "You must first add at least one connector type.";
    }
}

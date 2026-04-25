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

public class TelecommunicationsGridOutletGameItemComponentProto : GameItemComponentProto, IConnectableItemProto
{
    public override string TypeDescription => "TelecommunicationsGridOutlet";
    public List<ConnectorType> Connections { get; } = [];
    IEnumerable<ConnectorType> IConnectableItemProto.Connections => Connections;

    protected TelecommunicationsGridOutletGameItemComponentProto(IFuturemud gameworld, IAccount originator)
        : base(gameworld, originator, "TelecommunicationsGridOutlet")
    {
        Connections.Add(new ConnectorType(MudSharp.Form.Shape.Gender.Female, "telephone", true));
    }

    protected TelecommunicationsGridOutletGameItemComponentProto(Models.GameItemComponentProto proto,
        IFuturemud gameworld) : base(proto, gameworld)
    {
    }

    protected override void LoadFromXml(XElement root)
    {
        XElement? element = root.Element("Connectors");
        if (element == null)
        {
            Connections.Add(new ConnectorType(MudSharp.Form.Shape.Gender.Female, "telephone", true));
            return;
        }

        foreach (XElement item in element.Elements("Connection"))
        {
            Connections.Add(new ConnectorType((MudSharp.Form.Shape.Gender)Convert.ToSByte(item.Attribute("gender")?.Value ?? "0"),
                item.Attribute("type")?.Value ?? "telephone",
                bool.Parse(item.Attribute("powered")?.Value ?? "true")));
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
                    new XAttribute("powered", connector.Powered))
            )
        ).ToString();
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
    {
        return new TelecommunicationsGridOutletGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
    {
        return new TelecommunicationsGridOutletGameItemComponent(component, this, parent);
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("TelecommunicationsGridOutlet".ToLowerInvariant(), true,
            (gameworld, account) => new TelecommunicationsGridOutletGameItemComponentProto(gameworld, account));
        manager.AddBuilderLoader("Telecommunications Grid Outlet".ToLowerInvariant(), false,
            (gameworld, account) => new TelecommunicationsGridOutletGameItemComponentProto(gameworld, account));
        manager.AddTypeHelpInfo(
            "TelecommunicationsGridOutlet",
            $"Item {"[provides telephone service and grid power]".Colour(Telnet.BoldMagenta)} to connected telecom devices from a {"[telecommunications grid]".Colour(Telnet.BoldBlue)}",
            BuildingHelpText
        );
        manager.AddDatabaseLoader("TelecommunicationsGridOutlet",
            (proto, gameworld) => new TelecommunicationsGridOutletGameItemComponentProto(proto, gameworld));
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new TelecommunicationsGridOutletGameItemComponentProto(proto, gameworld));
    }

    private const string BuildingHelpText =
        "You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tconnection add <gender> <type> <powered> - adds a connection\n\tconnection remove <gender> <type> - removes a connection";

    public override string ShowBuildingHelp => BuildingHelpText;

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "type":
            case "connection":
            case "connection type":
                return BuildingCommandConnectionType(actor, command);
            default:
                return base.BuildingCommand(actor, command);
        }
    }

    private bool BuildingCommandConnectionType(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("Do you want to add or remove a connection type for this outlet?");
            return false;
        }

        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "add":
                return BuildingCommandConnectionTypeAdd(actor, command);
            case "remove":
            case "rem":
            case "del":
            case "delete":
                return BuildingCommandConnectionTypeRemove(actor, command);
            default:
                actor.Send("Do you want to add or remove a connection type for this outlet?");
                return false;
        }
    }

    private bool BuildingCommandConnectionTypeAdd(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What gender of connection do you want to add?");
            return false;
        }

        Gendering gendering = Gendering.Get(command.PopSpeech());
        if (gendering.Enum == MudSharp.Form.Shape.Gender.Indeterminate)
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
        type =
            Gameworld.ItemComponentProtos.Except(this)
                     .OfType<IConnectableItemProto>()
                     .SelectMany(x => x.Connections.Select(y => y.ConnectionType))
                     .FirstOrDefault(x => x.EqualTo(type) && !x.Equals(type, StringComparison.InvariantCulture)) ??
            type;

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

        if (gendering.Enum == MudSharp.Form.Shape.Gender.Neuter && powered)
        {
            actor.Send("Ungendered connections cannot also be powered.");
            return false;
        }

        Connections.Add(new ConnectorType(gendering.Enum, type, powered));
        Changed = true;
        actor.Send(
            $"This outlet now has an additional {(powered ? "powered" : "unpowered")} connection of type {type.Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
        return true;
    }

    private bool BuildingCommandConnectionTypeRemove(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What gender of connection do you want to remove?");
            return false;
        }

        Gendering gendering = Gendering.Get(command.PopSpeech());
        if (gendering.Enum == MudSharp.Form.Shape.Gender.Indeterminate)
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
        ConnectorType? connection = Connections.FirstOrDefault(x =>
            x.ConnectionType.Equals(type, StringComparison.InvariantCultureIgnoreCase) &&
            x.Gender == gendering.Enum);
        if (connection == null)
        {
            actor.Send("There is no connection like that to remove.");
            return false;
        }

        Connections.Remove(connection);
        Changed = true;
        actor.Send(
            $"This outlet now has one fewer connection of type {type.TitleCase().Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
        return true;
    }

    public override bool CanSubmit()
    {
        return Connections.Any() &&
               Connections.Count(x => x.Powered && x.Gender == MudSharp.Form.Shape.Gender.Male) <= 1 &&
               base.CanSubmit();
    }

    public override string WhyCannotSubmit()
    {
        if (Connections.All(x => x.Gender != MudSharp.Form.Shape.Gender.Female))
        {
            return "You must first add at least one female connector type.";
        }

        return Connections.Count(x => x.Powered && x.Gender == MudSharp.Form.Shape.Gender.Male) > 1
            ? "You can only have one powered male connection at a time."
            : base.WhyCannotSubmit();
    }

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return string.Format(actor,
            "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item provides a telecommunications outlet with the following connections: {4}.",
            "Telecommunications Grid Outlet Game Item Component".Colour(Telnet.Cyan),
            Id,
            RevisionNumber,
            Name,
            Connections.Select(
                           x =>
                               $"{x.ConnectionType.Colour(Telnet.Green)} {(x.Powered ? "[P]" : "")} ({Gendering.Get(x.Gender).GenderClass(true).Proper().Colour(Telnet.Green)})")
                       .ListToString());
    }
}

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class ExternalInhalerGameItemComponentProto : GameItemComponentProto, IConnectableItemProto
{
    protected ExternalInhalerGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "ExternalInhaler")
    {
        Connector = new ConnectorType(Form.Shape.Gender.Female, "inhaler", false);
        GasPerPuff = 0.0;
        Changed = true;
    }

    protected ExternalInhalerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld)
    {
    }

    public double GasPerPuff { get; set; }
    public string CanisterType { get; set; }
    public ConnectorType Connector { get; set; }

    public override string TypeDescription => "ExternalInhaler";

    public IEnumerable<ConnectorType> Connections => new[] { Connector };

    protected override void LoadFromXml(XElement root)
    {
        GasPerPuff = double.Parse(root.Element("GasPerPuff")?.Value ?? "0.0");
        CanisterType = root.Element("CanisterType")?.Value ?? "";
        var elem = root.Element("Connector");
        Connector = new ConnectorType((Form.Shape.Gender)Convert.ToSByte(elem?.Attribute("gender")?.Value ?? "2"), elem?.Attribute("type")?.Value ?? "inhaler", bool.Parse(elem?.Attribute("powered")?.Value ?? "false"));
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("GasPerPuff", GasPerPuff),
            new XElement("CanisterType", CanisterType ?? ""),
            new XElement("Connector",
                new XAttribute("gender", (sbyte)Connector.Gender),
                new XAttribute("type", Connector.ConnectionType),
                new XAttribute("powered", Connector.Powered))
        ).ToString();
    }

    private const string BuildingHelpText = "You can use the following options with this component:\n\tname <name>\n\tdesc <desc>\n\tpuff <amount>\n\ttype <string> - sets canister type\n\tconnectortype <type>\n\tconnectorgender <male|female|neuter>";

    public override string ShowBuildingHelp => BuildingHelpText;

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "puff":
                return BuildingCommandPuff(actor, command);
            case "type":
                return BuildingCommandType(actor, command);
            case "connectortype":
                return BuildingCommandConnectorType(actor, command);
            case "connectorgender":
                return BuildingCommandConnectorGender(actor, command);
        }
        return base.BuildingCommand(actor, command);
    }

    private bool BuildingCommandPuff(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("How much gas should be consumed per puff? The units are litres at 1 atmosphere.");
            return false;
        }
        var amount = Gameworld.UnitManager.GetBaseUnits(command.PopSpeech(), UnitType.FluidVolume, out var success);
        if (!success)
        {
            actor.OutputHandler.Send("That is not a valid volume.");
            return false;
        }
        GasPerPuff = amount;
        actor.OutputHandler.Send($"Each puff will now consume {Gameworld.UnitManager.DescribeMostSignificantExact(GasPerPuff, UnitType.FluidVolume, actor).ColourValue()} of gas.");
        Changed = true;
        return true;
    }

    private bool BuildingCommandType(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What canister type should this inhaler use?");
            return false;
        }
        CanisterType = command.SafeRemainingArgument;
        actor.Send($"This inhaler now uses canister type {CanisterType.Colour(Telnet.Cyan)}.");
        Changed = true;
        return true;
    }

    private bool BuildingCommandConnectorType(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What is the name of the connector type?");
            return false;
        }
        Connector = new ConnectorType(Connector.Gender, command.PopSpeech(), Connector.Powered);
        actor.Send($"This inhaler will have a connector of type {Connector.ConnectionType.Colour(Telnet.Cyan)}.");
        Changed = true;
        return true;
    }

    private bool BuildingCommandConnectorGender(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("Which gender should this connector be?");
            return false;
        }
        var gender = Gendering.Get(command.Pop());
        if (gender.Enum == Form.Shape.Gender.Indeterminate)
        {
            actor.Send("That is not a valid gender.");
            return false;
        }
        Connector = new ConnectorType(gender.Enum, Connector.ConnectionType, Connector.Powered);
        actor.Send($"This inhaler now has a {gender.Enum.ToString().ToLowerInvariant()} connector.");
        Changed = true;
        return true;
    }

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return string.Format(actor,
            "{0} (#{1:N0}r{2:N0}, {3})\n\nEach puff consumes {4}. Uses canister type {5} and has a {6} connector.",
            "External Inhaler Item Component".Colour(Telnet.Cyan),
            Id,
            RevisionNumber,
            Name,
            Gameworld.UnitManager.DescribeMostSignificantExact(GasPerPuff, UnitType.FluidVolume, actor).ColourValue(),
            CanisterType?.Colour(Telnet.Green) ?? "none",
            Connector.ConnectionType.Colour(Telnet.Green));
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
    {
        return new ExternalInhalerGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
    {
        return new ExternalInhalerGameItemComponent(component, this, parent);
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator, (proto, gameworld) => new ExternalInhalerGameItemComponentProto(proto, gameworld));
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("ExternalInhaler".ToLowerInvariant(), true, (gameworld, account) => new ExternalInhalerGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("ExternalInhaler", (proto, gameworld) => new ExternalInhalerGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo("ExternalInhaler", "An inhaler that uses attachable gas canisters.", BuildingHelpText);
    }
}

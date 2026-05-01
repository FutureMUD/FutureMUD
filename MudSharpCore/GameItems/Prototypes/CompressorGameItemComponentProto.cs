using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
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

public class CompressorGameItemComponentProto : PoweredMachineBaseGameItemComponentProto, IConnectableItemProto, IConnectablePrototype
{
    public override string TypeDescription => "Compressor";

    public List<ConnectorType> Connections { get; } = new();

    IEnumerable<ConnectorType> IConnectableItemProto.Connections => Connections;

    public double FlowRate { get; protected set; }

    #region Constructors

    protected CompressorGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
        "Compressor")
    {
        Connections.Add(new ConnectorType(Form.Shape.Gender.Female,
            Gameworld.GetStaticConfiguration("DefaultGasSocketType"), false));
        Connections.Add(new ConnectorType(Form.Shape.Gender.Male,
            Gameworld.GetStaticConfiguration("DefaultGasSocketType"), false));
        FlowRate = 210 / Gameworld.UnitManager.BaseFluidToLitres;
    }

    protected CompressorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
        base(proto, gameworld)
    {
    }

    protected override void LoadFromXml(XElement root)
    {
        base.LoadFromXml(root);
        XElement element = root.Element("Connectors");
        if (element != null)
        {
            foreach (XElement item in element.Elements("Connection"))
            {
                Connections.Add(new ConnectorType((Gender)Convert.ToSByte(item.Attribute("gender").Value),
                    item.Attribute("type").Value, bool.Parse(item.Attribute("powered").Value)));
            }
        }

        FlowRate = double.Parse(root.Element("FlowRate").Value);
    }

    #endregion

    #region Saving

    protected override XElement SaveSubtypeToXml(XElement root)
    {
        root.Add(new XElement("Connectors",
            from connector in Connections
            select
                new XElement("Connection",
                    new XAttribute("gender", (short)connector.Gender),
                    new XAttribute("type", connector.ConnectionType),
                    new XAttribute("powered", connector.Powered)
                )
        ));
        root.Add(new XElement("FlowRate", FlowRate));
        return root;
    }

    #endregion

    #region Component Instance Initialising Functions

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
    {
        return new CompressorGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
    {
        return new CompressorGameItemComponent(component, this, parent);
    }

    #endregion

    #region Initialisation Tasks

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("Compressor".ToLowerInvariant(), true,
            (gameworld, account) => new CompressorGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("Compressor",
            (proto, gameworld) => new CompressorGameItemComponentProto(proto, gameworld));

        manager.AddTypeHelpInfo(
            "Compressor",
            $"This is a {"[powered]".Colour(Telnet.Magenta)} machine that fills {"[gas containers]".Colour(Telnet.BoldGreen)} with atmosphere",
            BuildingHelpText
        );
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new CompressorGameItemComponentProto(proto, gameworld));
    }

    #endregion

    #region Building Commands

    public override string ShowBuildingHelp => $@"{base.ShowBuildingHelp}
    #3type add <male|female|neuter> <type name>#0 - adds a connection of the specified gender and name
	#3type remove <male|female|neuter> <type name>#0 - removes a connection of the specified gender and name
    #3flow <rate>#0 - sets the flow rate per 10 seconds";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "type":
            case "connection":
            case "connection type":
                return BuildingCommandConnectionType(actor, command);
            case "flow":
            case "flowrate":
            case "flow_rate":
            case "flow rate":
            case "rate":
                return BuildingCommandFlowRate(actor, command);
            default:
                return base.BuildingCommand(actor, command.GetUndo());
        }
    }

    private bool BuildingCommandFlowRate(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What should be the flow rate when switched on, in volume per 10 seconds?");
            return false;
        }

        double rate = Gameworld.UnitManager.GetBaseUnits(command.PopSpeech(), Framework.Units.UnitType.FluidVolume,
            out bool success);
        if (!success)
        {
            actor.OutputHandler.Send("That is not a valid fluid volume.");
            return false;
        }

        FlowRate = rate;
        Changed = true;
        actor.OutputHandler.Send(
            $"When switched on, this compressor will now compress {Gameworld.UnitManager.DescribeMostSignificantExact(FlowRate, Framework.Units.UnitType.FluidVolume, actor).ColourValue()} of 1-atmosphere gas per 10 seconds.");
        return true;
    }

    private bool BuildingCommandConnectionTypeAdd(ICharacter actor, StringStack command)
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
            actor.Send("What type of connection do you want this compressor to be?");
            return false;
        }

        string type = command.PopSpeech();
        type =
            Gameworld.ItemComponentProtos.Except(this)
                     .OfType<IConnectableItemProto>()
                     .SelectMany(x => x.Connections.Select(y => y.ConnectionType))
                     .FirstOrDefault(x => x.EqualTo(type) && !x.Equals(type, StringComparison.InvariantCulture)) ??
            type;

        Connections.Add(new ConnectorType(gendering.Enum, type, false));
        actor.Send(
            $"This compressor will now have an additional connection of type {type.Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
        Changed = true;
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
        if (
            Connections.Any(
                x =>
                    x.ConnectionType.Equals(type, StringComparison.InvariantCultureIgnoreCase) &&
                    x.Gender == gendering.Enum))
        {
            Connections.Remove(
                Connections.First(
                    x =>
                        x.ConnectionType.Equals(type,
                            StringComparison.InvariantCultureIgnoreCase) && x.Gender == gendering.Enum));
            actor.Send(
                $"This compressor now has one fewer connection of type {type.TitleCase().Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
            Changed = true;
            return true;
        }

        actor.Send("There is no connection like that to remove.");
        return false;
    }

    private bool BuildingCommandConnectionType(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("Do you want to add or remove a connection type for this compressor?");
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
        }

        actor.Send("Do you want to add or remove a connection type for this compressor?");
        return false;
    }

    #endregion

    protected override string ComponentDescriptionOLCByline =>
        "This is a machine that compresses air from an input or the atmosphere into a gas canister";

    protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
    {
        return string.Format(
            "It compresses {0} of 1-atmosphere gas per 10 seconds.\nIt has the following connections: {1}",
            Gameworld.UnitManager.DescribeMostSignificantExact(FlowRate, Framework.Units.UnitType.FluidVolume, actor)
                     .ColourValue(),
            Connections
                .Select(x =>
                    $"{x.ConnectionType.Colour(Telnet.Green)} {(x.Powered ? "[P]" : "")} ({Gendering.Get(x.Gender).GenderClass(true).Proper().Colour(Telnet.Green)})")
                .ListToString());
    }
}

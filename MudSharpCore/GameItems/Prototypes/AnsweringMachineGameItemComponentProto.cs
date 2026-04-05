#nullable enable
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Audio;
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
using ShapeGender = MudSharp.Form.Shape.Gender;

namespace MudSharp.GameItems.Prototypes;

public class AnsweringMachineGameItemComponentProto : GameItemComponentProto, IConnectableItemProto
{
    public override string TypeDescription => "AnsweringMachine";
    public double Wattage { get; set; }
    public string RingEmote { get; set; }
    public string TransmitPremote { get; set; }
    public AudioVolume RingVolume { get; set; }
    public int DefaultAutoAnswerRings { get; set; }
    public List<ConnectorType> Connections { get; } = [];
    IEnumerable<ConnectorType> IConnectableItemProto.Connections => Connections;

    protected AnsweringMachineGameItemComponentProto(IFuturemud gameworld, IAccount originator)
        : base(gameworld, originator, "AnsweringMachine")
    {
        Wattage = 6.0;
        RingEmote = "@ ring|rings insistently.";
        TransmitPremote = "@ speak|speaks into $1 and say|says";
        RingVolume = AudioVolume.Loud;
        DefaultAutoAnswerRings = 4;
        Connections.Add(new ConnectorType(ShapeGender.Male, "TelephoneLine", true));
        Connections.Add(new ConnectorType(ShapeGender.Female, "TelephoneLine", true));
    }

    protected AnsweringMachineGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld)
        : base(proto, gameworld)
    {
    }

    protected override void LoadFromXml(XElement root)
    {
        Wattage = double.Parse(root.Element("Wattage")?.Value ?? "6.0");
        RingEmote = root.Element("RingEmote")?.Value ?? "@ ring|rings insistently.";
        TransmitPremote = root.Element("TransmitPremote")?.Value ?? "@ speak|speaks into $1 and say|says";
        RingVolume = TelephoneRingSettings.NormaliseVolume(ParseAudioVolume(root.Element("RingVolume")?.Value, AudioVolume.Loud), false);
        DefaultAutoAnswerRings = int.TryParse(root.Element("DefaultAutoAnswerRings")?.Value, out int rings) ? rings : 4;
        Connections.Clear();

        XElement? connectors = root.Element("Connectors");
        if (connectors != null)
        {
            foreach (XElement item in connectors.Elements("Connection"))
            {
                Connections.Add(new ConnectorType(
                    (ShapeGender)Convert.ToSByte(item.Attribute("gender")?.Value ?? "0"),
                    item.Attribute("type")?.Value ?? "telephone",
                    bool.Parse(item.Attribute("powered")?.Value ?? "true")));
            }
        }

        if (!Connections.Any())
        {
            Connections.Add(new ConnectorType(ShapeGender.Male, "TelephoneLine", true));
            Connections.Add(new ConnectorType(ShapeGender.Female, "TelephoneLine", true));
        }
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("Wattage", Wattage),
            new XElement("RingEmote", new XCData(RingEmote)),
            new XElement("TransmitPremote", new XCData(TransmitPremote)),
            new XElement("RingVolume", (int)RingVolume),
            new XElement("DefaultAutoAnswerRings", DefaultAutoAnswerRings),
            new XElement("Connectors",
                from connector in Connections
                select new XElement("Connection",
                    new XAttribute("gender", (short)connector.Gender),
                    new XAttribute("type", connector.ConnectionType),
                    new XAttribute("powered", connector.Powered)))
        ).ToString();
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
    {
        return new AnsweringMachineGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
    {
        return new AnsweringMachineGameItemComponent(component, this, parent);
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("AnsweringMachine".ToLowerInvariant(), true,
            (gameworld, account) => new AnsweringMachineGameItemComponentProto(gameworld, account));
        manager.AddBuilderLoader("Answering Machine".ToLowerInvariant(), false,
            (gameworld, account) => new AnsweringMachineGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("AnsweringMachine",
            (proto, gameworld) => new AnsweringMachineGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo(
            "AnsweringMachine",
            $"A daisy-chained {"[telephone]".Colour(Telnet.BoldBlue)} that can play a greeting, record messages to a {"[tape]".Colour(Telnet.BoldGreen)}, and expose downstream handsets",
            BuildingHelpText
        );
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new AnsweringMachineGameItemComponentProto(proto, gameworld));
    }

    private const string BuildingHelpText =
        "You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\twatts <#> - sets how much power the answering machine draws while switched on\n\tring <emote> - sets the emote used when it rings\n\tringvolume <quiet|normal|loud> - sets the default player-selectable ring setting\n\tpremote <emote> - sets the emote prepended when someone transmits into the line\n\trings <#> - sets the default number of rings before the machine answers\n\tconnection add <gender> <type> <powered> - adds a connector type\n\tconnection remove <gender> <type> - removes a connector type";

    public override string ShowBuildingHelp => BuildingHelpText;

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "watts":
            case "watt":
            case "wattage":
                return BuildingCommandWatts(actor, command);
            case "ring":
            case "ringemote":
                return BuildingCommandRing(actor, command);
            case "ringvolume":
            case "volume":
                return BuildingCommandRingVolume(actor, command);
            case "premote":
            case "transmit":
            case "transmitemote":
                return BuildingCommandPremote(actor, command);
            case "rings":
            case "autoanswer":
                return BuildingCommandRings(actor, command);
            case "connection":
            case "connector":
            case "connections":
            case "connectors":
                return BuildingCommandConnection(actor, command);
            default:
                return base.BuildingCommand(actor, command);
        }
    }

    private bool BuildingCommandConnection(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("Do you want to add or remove a connection type for this answering machine?");
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
            default:
                actor.Send("Do you want to add or remove a connection type for this answering machine?");
                return false;
        }
    }

    private bool BuildingCommandConnectionAdd(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What gender of connection do you want to add?");
            return false;
        }

        Gendering gendering = Gendering.Get(command.PopSpeech());
        if (gendering.Enum == ShapeGender.Indeterminate)
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
        type = Gameworld.ItemComponentProtos.Except(this)
                        .OfType<IConnectableItemProto>()
                        .SelectMany(x => x.Connections.Select(y => y.ConnectionType))
                        .FirstOrDefault(x => x.EqualTo(type) && !x.Equals(type, StringComparison.InvariantCulture)) ?? type;

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

        if (gendering.Enum == ShapeGender.Neuter && powered)
        {
            actor.Send("Ungendered connections cannot also be powered.");
            return false;
        }

        Connections.Add(new ConnectorType(gendering.Enum, type, powered));
        Changed = true;
        actor.Send($"This answering machine now has an additional {(powered ? "powered" : "unpowered")} connection of type {type.Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
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
        if (gendering.Enum == ShapeGender.Indeterminate)
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
            x.ConnectionType.Equals(type, StringComparison.InvariantCultureIgnoreCase) &&
            x.Gender == gendering.Enum);
        if (connector == null)
        {
            actor.Send("There is no connection like that to remove.");
            return false;
        }

        Connections.Remove(connector);
        Changed = true;
        actor.Send($"This answering machine now has one fewer connection of type {type.TitleCase().Colour(Telnet.Green)} and gender {gendering.GenderClass(true).Colour(Telnet.Green)}.");
        return true;
    }

    private bool BuildingCommandWatts(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("How many watts should this answering machine draw while switched on?");
            return false;
        }

        if (!double.TryParse(command.PopSpeech(), out double value) || value < 0.0)
        {
            actor.Send("You must enter a non-negative number of watts.");
            return false;
        }

        Wattage = value;
        Changed = true;
        actor.Send($"This answering machine will now draw {Wattage.ToString("N2", actor).ColourValue()} watts while switched on.");
        return true;
    }

    private bool BuildingCommandRing(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What emote should this answering machine use when it rings?");
            return false;
        }

        RingEmote = command.SafeRemainingArgument;
        Changed = true;
        actor.Send($"The ring emote is now: {RingEmote}");
        return true;
    }

    private bool BuildingCommandPremote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What premote should be used when someone transmits into the answering machine?");
            return false;
        }

        TransmitPremote = command.SafeRemainingArgument;
        Changed = true;
        actor.Send($"The transmit premote is now: {TransmitPremote}");
        return true;
    }

    private bool BuildingCommandRingVolume(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send(
                $"What ring setting should this answering machine use by default? Valid values are {TelephoneRingSettings.LandlineSettings.Select(x => x.ColourValue()).ListToString()}.");
            return false;
        }

        if (!TelephoneRingSettings.TryParseBuilderSetting(command.SafeRemainingArgument, false, out AudioVolume volume))
        {
            actor.Send(
                $"That is not a valid ring setting. Valid values are {TelephoneRingSettings.LandlineSettings.Select(x => x.ColourValue()).ListToString()}.");
            return false;
        }

        RingVolume = volume;
        Changed = true;
        actor.Send($"This answering machine will now use the {TelephoneRingSettings.DescribeSetting(RingVolume, false).ColourValue()} ring setting by default.");
        return true;
    }

    private bool BuildingCommandRings(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("How many rings should this answering machine wait before it answers?");
            return false;
        }

        if (!int.TryParse(command.SafeRemainingArgument, out int value) || value <= 0)
        {
            actor.Send("You must enter a positive number of rings.");
            return false;
        }

        DefaultAutoAnswerRings = value;
        Changed = true;
        actor.Send($"This answering machine will now answer after {DefaultAutoAnswerRings.ToString("N0", actor).ColourValue()} rings.");
        return true;
    }

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return
            $"{"Answering Machine Game Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nThis answering machine draws {Wattage.ToString("N2", actor).ColourValue()} watts while on, answers after {DefaultAutoAnswerRings.ToString("N0", actor).ColourValue()} rings by default, and has the following connections: {Connections.Select(x => $"{x.ConnectionType.Colour(Telnet.Green)} {(x.Powered ? "[P]" : "")} ({Gendering.Get(x.Gender).GenderClass(true).Proper().Colour(Telnet.Green)})").ListToString()}.";
    }

    public override bool CanSubmit()
    {
        return Connections.Any() && base.CanSubmit();
    }

    public override string WhyCannotSubmit()
    {
        return Connections.Any() ? base.WhyCannotSubmit() : "You must first add at least one connector type.";
    }

    private static AudioVolume ParseAudioVolume(string? value, AudioVolume fallback)
    {
        return TryParseAudioVolume(value, out AudioVolume volume) ? volume : fallback;
    }

    private static bool TryParseAudioVolume(string? value, out AudioVolume volume)
    {
        volume = AudioVolume.Decent;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (int.TryParse(value, out int rawValue) && Enum.IsDefined(typeof(AudioVolume), rawValue))
        {
            volume = (AudioVolume)rawValue;
            return true;
        }

        string normalised = new(value.Where(char.IsLetterOrDigit).ToArray());
        return Enum.TryParse(normalised, true, out volume);
    }
}

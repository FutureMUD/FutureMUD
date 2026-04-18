#nullable enable
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class CellularPhoneGameItemComponentProto : GameItemComponentProto
{
    public override string TypeDescription => "CellularPhone";
    public double Wattage { get; set; }
    public string RingEmote { get; set; }
    public string TransmitPremote { get; set; }
    public AudioVolume RingVolume { get; set; }

    protected CellularPhoneGameItemComponentProto(IFuturemud gameworld, IAccount originator)
        : base(gameworld, originator, "CellularPhone")
    {
        Wattage = 2.0;
        RingEmote = "@ chirp|chirps insistently.";
        TransmitPremote = "@ speak|speaks into $1 and say|says";
        RingVolume = AudioVolume.Decent;
    }

    protected CellularPhoneGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld)
        : base(proto, gameworld)
    {
    }

    protected override void LoadFromXml(XElement root)
    {
        Wattage = double.Parse(root.Element("Wattage")?.Value ?? "2.0");
        RingEmote = root.Element("RingEmote")?.Value ?? "@ chirp|chirps insistently.";
        TransmitPremote = root.Element("TransmitPremote")?.Value ?? "@ speak|speaks into $1 and say|says";
        RingVolume = TelephoneRingSettings.NormaliseVolume(
            ParseAudioVolume(root.Element("RingVolume")?.Value, AudioVolume.Decent),
            true);
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("Wattage", Wattage),
            new XElement("RingEmote", new XCData(RingEmote)),
            new XElement("TransmitPremote", new XCData(TransmitPremote)),
            new XElement("RingVolume", (int)RingVolume)
        ).ToString();
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
    {
        return new CellularPhoneGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
    {
        return new CellularPhoneGameItemComponent(component, this, parent);
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("CellularPhone".ToLowerInvariant(), true,
            (gameworld, account) => new CellularPhoneGameItemComponentProto(gameworld, account));
        manager.AddBuilderLoader("Cellular Phone".ToLowerInvariant(), false,
            (gameworld, account) => new CellularPhoneGameItemComponentProto(gameworld, account));
        manager.AddBuilderLoader("CellPhone".ToLowerInvariant(), false,
            (gameworld, account) => new CellularPhoneGameItemComponentProto(gameworld, account));
        manager.AddBuilderLoader("Cell Phone".ToLowerInvariant(), false,
            (gameworld, account) => new CellularPhoneGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("CellularPhone",
            (proto, gameworld) => new CellularPhoneGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo(
            "CellularPhone",
            $"A {"[powered]".Colour(Telnet.BoldMagenta)} telephone that relies on a {"[cell phone tower]".Colour(Telnet.BoldGreen)} in the same zone",
            BuildingHelpText
        );
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new CellularPhoneGameItemComponentProto(proto, gameworld));
    }

    private const string BuildingHelpText =
        "You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\twatts <#> - sets how much power the phone draws when switched on\n\tring <emote> - sets the emote used when the phone rings\n\tringvolume <silent|quiet|normal|loud> - sets the default player-selectable ring setting for phones using this prototype\n\tpremote <emote> - sets the emote prepended when a character transmits speech into the phone";

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
            default:
                return base.BuildingCommand(actor, command);
        }
    }

    private bool BuildingCommandWatts(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("How many watts should this cellular phone draw while switched on?");
            return false;
        }

        if (!double.TryParse(command.PopSpeech(), out double value) || value < 0.0)
        {
            actor.Send("You must enter a non-negative number of watts.");
            return false;
        }

        Wattage = value;
        Changed = true;
        actor.Send($"This cellular phone will now draw {Wattage:N2} watts while switched on.");
        return true;
    }

    private bool BuildingCommandRing(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What emote should this cellular phone use when it rings?");
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
            actor.Send("What premote should be used when someone transmits into the cellular phone?");
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
                $"What ring setting should this cellular phone use by default? Valid values are {TelephoneRingSettings.CellularSettings.Select(x => x.ColourValue()).ListToString()}.");
            return false;
        }

        if (!TelephoneRingSettings.TryParseBuilderSetting(command.SafeRemainingArgument, true, out AudioVolume volume))
        {
            actor.Send(
                $"That is not a valid ring setting. Valid values are {TelephoneRingSettings.CellularSettings.Select(x => x.ColourValue()).ListToString()}.");
            return false;
        }

        RingVolume = volume;
        Changed = true;
        actor.Send($"This cellular phone will now use the {TelephoneRingSettings.DescribeSetting(RingVolume, true).ColourValue()} ring setting by default.");
        return true;
    }

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return string.Format(actor,
            "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a cellular phone that draws {4:N2} watts while on and rings at {5} volume by default.",
            "Cellular Phone Game Item Component".Colour(Telnet.Cyan),
            Id,
            RevisionNumber,
            Name,
            Wattage,
            TelephoneRingSettings.DescribeSetting(RingVolume, true).ColourValue());
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

#nullable enable
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using System.Linq;

namespace MudSharp.GameItems.Prototypes;

public class TelecommunicationsGridCreatorGameItemComponentProto : GameItemComponentProto
{
    public override string TypeDescription => "TelecommunicationsGridCreator";
    public string Prefix { get; set; } = null!;
    public int NumberLength { get; set; }
    public bool HostedVoicemailEnabled { get; set; }
    public string HostedVoicemailAccessCode { get; set; } = null!;

    protected TelecommunicationsGridCreatorGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(
        gameworld, originator, "TelecommunicationsGridCreator")
    {
        Prefix = "555";
        NumberLength = 4;
        HostedVoicemailEnabled = false;
        HostedVoicemailAccessCode = "9999";
    }

    protected TelecommunicationsGridCreatorGameItemComponentProto(Models.GameItemComponentProto proto,
        IFuturemud gameworld) : base(proto, gameworld)
    {
    }

    protected override void LoadFromXml(System.Xml.Linq.XElement root)
    {
        Prefix = root.Element("Prefix")?.Value ?? "555";
        NumberLength = int.Parse(root.Element("NumberLength")?.Value ?? "4");
        HostedVoicemailEnabled = bool.Parse(root.Element("HostedVoicemailEnabled")?.Value ?? "false");
        HostedVoicemailAccessCode = root.Element("HostedVoicemailAccessCode")?.Value ?? "9999";
    }

    protected override string SaveToXml()
    {
        return new System.Xml.Linq.XElement("Definition",
            new System.Xml.Linq.XElement("Prefix", Prefix),
            new System.Xml.Linq.XElement("NumberLength", NumberLength),
            new System.Xml.Linq.XElement("HostedVoicemailEnabled", HostedVoicemailEnabled),
            new System.Xml.Linq.XElement("HostedVoicemailAccessCode", HostedVoicemailAccessCode)
        ).ToString();
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
    {
        return new TelecommunicationsGridCreatorGameItemComponent(this, parent, loader, temporary);
    }

    public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
    {
        return new TelecommunicationsGridCreatorGameItemComponent(component, this, parent);
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("TelecommunicationsGridCreator".ToLowerInvariant(), true,
            (gameworld, account) => new TelecommunicationsGridCreatorGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("TelecommunicationsGridCreator",
            (proto, gameworld) => new TelecommunicationsGridCreatorGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo(
            "TelecommunicationsGridCreator",
            $"When put in a room, creates a {"[telecommunications grid]".Colour(Telnet.BoldBlue)}",
            BuildingHelpText
        );
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new TelecommunicationsGridCreatorGameItemComponentProto(proto, gameworld));
    }

    private const string BuildingHelpText =
        "You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tprefix <digits> - sets the number prefix for telephones on this grid\n\tdigits <#> - sets the number of subscriber digits after the prefix\n\tvoicemail on|off - enables or disables hosted voicemail on the exchange\n\tvoicemail number <digits> - sets the service access digits appended after the prefix";

    public override string ShowBuildingHelp => BuildingHelpText;

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "prefix":
                return BuildingCommandPrefix(actor, command);
            case "digits":
            case "length":
            case "numberlength":
                return BuildingCommandDigits(actor, command);
            case "voicemail":
                return BuildingCommandVoicemail(actor, command);
            default:
                return base.BuildingCommand(actor, command);
        }
    }

    private bool BuildingCommandVoicemail(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("Do you want to turn hosted voicemail on, off, or set its access number?");
            return false;
        }

        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "on":
                HostedVoicemailEnabled = true;
                Changed = true;
                actor.Send("This telecommunications grid will now enable hosted voicemail.");
                return true;
            case "off":
                HostedVoicemailEnabled = false;
                Changed = true;
                actor.Send("This telecommunications grid will now disable hosted voicemail.");
                return true;
            case "number":
            case "access":
            case "code":
                return BuildingCommandVoicemailNumber(actor, command);
            default:
                actor.Send("Do you want to turn hosted voicemail on, off, or set its access number?");
                return false;
        }
    }

    private bool BuildingCommandVoicemailNumber(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What service access digits should callers dial after the exchange prefix to reach hosted voicemail?");
            return false;
        }

        string digits = new(command.SafeRemainingArgument.Where(char.IsDigit).ToArray());
        if (string.IsNullOrEmpty(digits))
        {
            actor.Send("You must enter at least one digit for the hosted voicemail access number.");
            return false;
        }

        HostedVoicemailAccessCode = digits;
        Changed = true;
        actor.Send($"Hosted voicemail will now be reached by dialing {(Prefix + HostedVoicemailAccessCode).ColourValue()}.");
        return true;
    }

    private bool BuildingCommandPrefix(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What numeric prefix should telephones on this grid use?");
            return false;
        }

        string prefix = new(command.PopSpeech().Where(char.IsDigit).ToArray());
        if (string.IsNullOrEmpty(prefix))
        {
            actor.Send("The prefix must contain at least one digit.");
            return false;
        }

        Prefix = prefix;
        Changed = true;
        actor.Send($"This telecommunications grid will now use the prefix {Prefix.ColourValue()}.");
        return true;
    }

    private bool BuildingCommandDigits(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("How many subscriber digits should telephones on this grid have after the prefix?");
            return false;
        }

        if (!int.TryParse(command.PopSpeech(), out int value) || value < 1)
        {
            actor.Send("You must enter a positive whole number of digits.");
            return false;
        }

        NumberLength = value;
        Changed = true;
        actor.Send($"This telecommunications grid will now use {NumberLength.ToString("N0", actor).ColourValue()} subscriber digits.");
        return true;
    }

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return string.Format(actor,
            "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item creates a telecommunications grid with prefix {4} and {5:N0} subscriber digits. Hosted voicemail is {6} and uses access number {7}.",
            "TelecommunicationsGridCreator Game Item Component".Colour(Telnet.Cyan),
            Id,
            RevisionNumber,
            Name,
            Prefix.ColourValue(),
            NumberLength,
            HostedVoicemailEnabled ? "enabled".ColourValue() : "disabled".ColourError(),
            (Prefix + HostedVoicemailAccessCode).ColourValue()
        );
    }
}

#nullable enable
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class TapeGameItemComponentProto : GameItemComponentProto
{
    public override string TypeDescription => "Tape";
    public TimeSpan Capacity { get; set; }

    protected TapeGameItemComponentProto(IFuturemud gameworld, IAccount originator)
        : base(gameworld, originator, "Tape")
    {
        Capacity = TimeSpan.FromMinutes(30);
    }

    protected TapeGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld)
        : base(proto, gameworld)
    {
    }

    protected override void LoadFromXml(XElement root)
    {
        Capacity = TimeSpan.FromMilliseconds(long.TryParse(root.Element("CapacityMs")?.Value, out long capacityMs)
            ? capacityMs
            : (long)TimeSpan.FromMinutes(30).TotalMilliseconds);
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("CapacityMs", (long)Capacity.TotalMilliseconds)
        ).ToString();
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
    {
        return new TapeGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
    {
        return new TapeGameItemComponent(component, this, parent);
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("Tape".ToLowerInvariant(), true,
            (gameworld, account) => new TapeGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("Tape",
            (proto, gameworld) => new TapeGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo(
            "Tape",
            $"A reusable {"[audio storage medium]".Colour(Telnet.BoldGreen)} that can hold recorded speech for answering machines and future devices",
            BuildingHelpText
        );
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new TapeGameItemComponentProto(proto, gameworld));
    }

    private const string BuildingHelpText =
        "You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tcapacity <minutes> - sets how many minutes of audio the tape can store";

    public override string ShowBuildingHelp => BuildingHelpText;

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "capacity":
            case "minutes":
                return BuildingCommandCapacity(actor, command);
            default:
                return base.BuildingCommand(actor, command);
        }
    }

    private bool BuildingCommandCapacity(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("How many minutes of audio should this tape be able to store?");
            return false;
        }

        if (!double.TryParse(command.SafeRemainingArgument, out double minutes) || minutes <= 0.0)
        {
            actor.Send("You must enter a positive number of minutes.");
            return false;
        }

        Capacity = TimeSpan.FromMinutes(minutes);
        Changed = true;
        actor.Send($"This tape can now store {Capacity.TotalMinutes.ToString("N1", actor).ColourValue()} minutes of audio.");
        return true;
    }

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return
            $"{"Tape Game Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nThis tape can store {Capacity.TotalMinutes.ToString("N1", actor).ColourValue()} minutes of recorded audio.";
    }
}

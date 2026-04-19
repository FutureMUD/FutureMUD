#nullable enable
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine.Parsers;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public abstract class SwitchableThermalSourceGameItemComponentProto : ThermalSourceGameItemComponentProto
{
    protected SwitchableThermalSourceGameItemComponentProto(IFuturemud gameworld, IAccount originator, string type) : base(
        gameworld, originator, type)
    {
        SwitchOnEmote = "@ begin|begins operating";
        SwitchOffEmote = "@ stop|stops operating";
    }

    protected SwitchableThermalSourceGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
        proto, gameworld)
    {
    }

    public string SwitchOnEmote { get; protected set; } = string.Empty;
    public string SwitchOffEmote { get; protected set; } = string.Empty;

    protected void LoadSwitchableThermalDefinitionFromXml(XElement root)
    {
        LoadThermalDefinitionFromXml(root);
        SwitchOnEmote = root.Element("SwitchOnEmote")?.Value ?? "@ begin|begins operating";
        SwitchOffEmote = root.Element("SwitchOffEmote")?.Value ?? "@ stop|stops operating";
    }

    protected XElement SaveSwitchableThermalDefinitionToXml(XElement root)
    {
        SaveThermalDefinitionToXml(root);
        root.Add(
            new XElement("SwitchOnEmote", new XCData(SwitchOnEmote)),
            new XElement("SwitchOffEmote", new XCData(SwitchOffEmote))
        );
        return root;
    }

    protected bool BuildingCommandSwitchEmote(ICharacter actor, bool on, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"What emote should be shown when this source switches {(on ? "on" : "off")}?");
            return false;
        }

        Emote emote = new(command.SafeRemainingArgument, actor, actor);
        if (!emote.Valid)
        {
            actor.OutputHandler.Send(emote.ErrorMessage);
            return false;
        }

        if (on)
        {
            SwitchOnEmote = command.SafeRemainingArgument;
        }
        else
        {
            SwitchOffEmote = command.SafeRemainingArgument;
        }

        Changed = true;
        actor.OutputHandler.Send($"That switch {(on ? "on" : "off")} emote is now {command.SafeRemainingArgument.ColourCommand()}.");
        return true;
    }

    protected const string SwitchableThermalBuildingHelpText =
        ThermalBuildingHelpText +
        "\n\t#3onemote <emote>#0 - sets the emote when the source switches on" +
        "\n\t#3offemote <emote>#0 - sets the emote when the source switches off";
}

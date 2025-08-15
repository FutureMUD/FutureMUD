using System;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Form.Material;
using MudSharp.GameItems.Components;
using MudSharp.Health;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class InhalerGasCanisterGameItemComponentProto : GameItemComponentProto
{
    protected InhalerGasCanisterGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "InhalerGasCanister")
    {
        Changed = true;
    }

    protected InhalerGasCanisterGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld)
    {
    }

    public string CanisterType { get; set; }
    public IGas InitialGas { get; set; }

    public override string TypeDescription => "InhalerGasCanister";

    protected override void LoadFromXml(XElement root)
    {
        CanisterType = root.Element("CanisterType")?.Value ?? "";
        InitialGas = Gameworld.Gases.Get(long.Parse(root.Element("InitialGas")?.Value ?? "0"));
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("CanisterType", CanisterType ?? ""),
            new XElement("InitialGas", InitialGas?.Id ?? 0)
        ).ToString();
    }

    private const string BuildingHelpText = "You can use the following options with this component:\n\tname <name>\n\tdesc <desc>\n\ttype <string> - sets canister type\n\tgas <which> - sets starting gas\n\tgas clear - starts empty";

    public override string ShowBuildingHelp => BuildingHelpText;

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "type":
                return BuildingCommandType(actor, command);
            case "gas":
                return BuildingCommandGas(actor, command);
        }
        return base.BuildingCommand(actor, command);
    }

    private bool BuildingCommandType(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What canister type should this canister be?");
            return false;
        }
        CanisterType = command.SafeRemainingArgument;
        actor.Send($"This canister is now type {CanisterType.Colour(Telnet.Cyan)}.");
        Changed = true;
        return true;
    }

    private bool BuildingCommandGas(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which gas should this canister start filled with? Use 'clear' to start empty.");
            return false;
        }
        if (command.Peek().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
        {
            InitialGas = null;
            actor.OutputHandler.Send("This canister will now start empty.");
            Changed = true;
            return true;
        }
        var gas = long.TryParse(command.PopSpeech(), out var value) ? Gameworld.Gases.Get(value) : Gameworld.Gases.GetByName(command.Last);
        if (gas == null)
        {
            actor.OutputHandler.Send("There is no such gas.");
            return false;
        }
        InitialGas = gas;
        actor.OutputHandler.Send($"This canister will now be created full of {gas.Name.Colour(gas.DisplayColour)}.");
        Changed = true;
        return true;
    }

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return string.Format(actor,
            "{0} (#{1:N0}r{2:N0}, {3})\n\nCanister type {4}. Starts with gas {5}.",
            "Inhaler Gas Canister Item Component".Colour(Telnet.Cyan),
            Id,
            RevisionNumber,
            Name,
            CanisterType?.Colour(Telnet.Green) ?? "none",
            InitialGas?.Name.Colour(Telnet.Green) ?? "none");
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
    {
        return new InhalerGasCanisterGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
    {
        return new InhalerGasCanisterGameItemComponent(component, this, parent);
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator, (proto, gameworld) => new InhalerGasCanisterGameItemComponentProto(proto, gameworld));
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("InhalerGasCanister".ToLowerInvariant(), true, (gameworld, account) => new InhalerGasCanisterGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("InhalerGasCanister", (proto, gameworld) => new InhalerGasCanisterGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo("InhalerGasCanister", "A gas canister for inhalers.", BuildingHelpText);
    }
}

using System;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.Form.Material;
using MudSharp.GameItems.Components;
using MudSharp.Health;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class IntegratedInhalerGameItemComponentProto : GameItemComponentProto
{
    protected IntegratedInhalerGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "IntegratedInhaler")
    {
        Changed = true;
    }

    protected IntegratedInhalerGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld)
    {
    }

    public double GasPerPuff { get; set; }
    public IGas InitialGas { get; set; }

    public override string TypeDescription => "IntegratedInhaler";

    protected override void LoadFromXml(XElement root)
    {
        GasPerPuff = double.Parse(root.Element("GasPerPuff")?.Value ?? "0.0");
        InitialGas = Gameworld.Gases.Get(long.Parse(root.Element("InitialGas")?.Value ?? "0"));
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("GasPerPuff", GasPerPuff),
            new XElement("InitialGas", InitialGas?.Id ?? 0)
        ).ToString();
    }

    private const string BuildingHelpText = "You can use the following options with this component:\n\tname <name>\n\tdesc <desc>\n\tpuff <amount> - gas volume per puff\n\tgas <which> - sets initial gas\n\tgas clear - clears initial gas";

    public override string ShowBuildingHelp => BuildingHelpText;

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "puff":
            case "amount":
                return BuildingCommandPuff(actor, command);
            case "gas":
                return BuildingCommandGas(actor, command);
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

    private bool BuildingCommandGas(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which gas should this inhaler be loaded with? Use 'clear' to start empty.");
            return false;
        }
        if (command.Peek().Equals("clear", StringComparison.InvariantCultureIgnoreCase))
        {
            InitialGas = null;
            actor.OutputHandler.Send("This inhaler will now start empty.");
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
        actor.OutputHandler.Send($"This inhaler will now be created full of {gas.Name.Colour(gas.DisplayColour)}.");
        Changed = true;
        return true;
    }

    public override string ComponentDescriptionOLC(ICharacter actor)
    {
        return string.Format(actor,
            "{0} (#{1:N0}r{2:N0}, {3})\n\nEach puff consumes {4}. It starts with gas {5}.",
            "Integrated Inhaler Item Component".Colour(Telnet.Cyan),
            Id,
            RevisionNumber,
            Name,
            Gameworld.UnitManager.DescribeMostSignificantExact(GasPerPuff, UnitType.FluidVolume, actor).ColourValue(),
            InitialGas?.Name.Colour(Telnet.Green) ?? "none");
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
    {
        return new IntegratedInhalerGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
    {
        return new IntegratedInhalerGameItemComponent(component, this, parent);
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator, (proto, gameworld) => new IntegratedInhalerGameItemComponentProto(proto, gameworld));
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("IntegratedInhaler".ToLowerInvariant(), true, (gameworld, account) => new IntegratedInhalerGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("IntegratedInhaler", (proto, gameworld) => new IntegratedInhalerGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo("IntegratedInhaler", "An inhaler with built in gas store.", BuildingHelpText);
    }
}

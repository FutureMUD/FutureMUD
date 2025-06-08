using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class RevolverGameItemComponentProto : GameItemComponentProto
{
    public override string TypeDescription => "Revolver";

    public string LoadEmote { get; set; }
    public string UnloadEmote { get; set; }
    public string FireEmote { get; set; }
    public string FireEmoteNoRound { get; set; }

    private IRangedWeaponType _rangedWeaponType;
    public IRangedWeaponType RangedWeaponType
    {
        get => _rangedWeaponType;
        set
        {
            _rangedWeaponType = value;
            LoadTemplate = new InventoryPlanTemplate(Gameworld, new[]
            {
                new InventoryPlanPhaseTemplate(1, new[]
                {
                    InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0, item =>
                    {
                        var ammo = item.GetItemType<IAmmo>();
                        if (ammo == null)
                        {
                            return false;
                        }
                        if (!ammo.AmmoType.RangedWeaponTypes.Contains(RangedWeaponType.ModernFirearm))
                        {
                            return false;
                        }
                        return ammo.AmmoType.SpecificType.Equals(_rangedWeaponType.SpecificAmmunitionGrade, StringComparison.InvariantCultureIgnoreCase);
                    }, null, 1, originalReference: "loaditem"),
                    InventoryPlanAction.LoadAction(Gameworld, DesiredItemState.Held, 0, 0, item => item.GetItemType<IRangedWeapon>()?.Prototype == this, null)
                })
            });
        }
    }

    public IWeaponType MeleeWeaponType { get; set; }
    public int Chambers { get; set; }
    public IInventoryPlanTemplate LoadTemplate { get; set; }

    protected RevolverGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator, "Revolver")
    {
        LoadEmote = "@ load|loads $2 into one of the chambers on $1.";
        UnloadEmote = "@ remove|removes $2 from $1.";
        FireEmote = "@ pull|pulls the trigger on $1.";
        FireEmoteNoRound = "@ pull|pulls the trigger on $1 but nothing happens.";
        Chambers = 6;
        MeleeWeaponType = gameworld.WeaponTypes.Get(gameworld.GetStaticLong("DefaultGunMeleeWeaponType"));
        RangedWeaponType = gameworld.RangedWeaponTypes.First();
    }

    protected RevolverGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(proto, gameworld) { }

    protected override void LoadFromXml(XElement root)
    {
        LoadEmote = root.Element("LoadEmote").Value;
        UnloadEmote = root.Element("UnloadEmote").Value;
        FireEmote = root.Element("FireEmote").Value;
        FireEmoteNoRound = root.Element("FireEmoteNoRound").Value;
        Chambers = int.Parse(root.Element("Chambers").Value);
        RangedWeaponType = Gameworld.RangedWeaponTypes.Get(long.Parse(root.Element("RangedWeaponType").Value));
        var element = root.Element("MeleeWeaponType");
        if (element != null)
        {
            MeleeWeaponType = Gameworld.WeaponTypes.Get(long.Parse(element.Value));
        }
        else
        {
            MeleeWeaponType = Gameworld.WeaponTypes.Get(Gameworld.GetStaticLong("DefaultGunMeleeWeaponType"));
        }
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("LoadEmote", new XCData(LoadEmote)),
            new XElement("UnloadEmote", new XCData(UnloadEmote)),
            new XElement("FireEmote", new XCData(FireEmote)),
            new XElement("FireEmoteNoRound", new XCData(FireEmoteNoRound)),
            new XElement("Chambers", Chambers),
            new XElement("RangedWeaponType", RangedWeaponType?.Id ?? 0),
            new XElement("MeleeWeaponType", MeleeWeaponType?.Id ?? 0)
        ).ToString();
    }

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
    {
        return new RevolverGameItemComponent(this, parent, temporary);
    }

    public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
    {
        return new RevolverGameItemComponent(component, this, parent);
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("Revolver".ToLowerInvariant(), true, (gameworld, account) => new RevolverGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("Revolver", (proto, gameworld) => new RevolverGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo("Revolver", $"Makes an item a {"[ranged weapon]".Colour(Telnet.BoldCyan)} with revolver mechanics", $"You can use the following options:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\tranged <ranged type> - sets the ranged weapon type\n\tchambers <number> - sets the number of chambers");
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator, (proto, gameworld) => new RevolverGameItemComponentProto(proto, gameworld));
    }

    public override string ShowBuildingHelp =>
        $@"You can use the following options:

name <name> - sets the name of the component
desc <desc> - sets the description of the component
ranged <ranged type> - sets the ranged weapon type for this component. See {{"show ranges".FluentTagMXP("send", "href='show ranges'")} for a list.
chambers <number> - sets the number of chambers for the revolver";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "ranged":
            case "type":
            case "rangedtype":
                return BuildingCommandType(actor, command);
            case "chambers":
                return BuildingCommandChambers(actor, command);
            case "load":
                return BuildingCommandLoad(actor, command);
            case "unload":
                return BuildingCommandUnload(actor, command);
            case "fire":
                return BuildingCommandFire(actor, command);
            case "firenoround":
            case "fireempty":
                return BuildingCommandFireNoRound(actor, command);
            default:
                return base.BuildingCommand(actor, command);
        }
    }

    private bool BuildingCommandChambers(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !int.TryParse(command.SafeRemainingArgument, out var value))
        {
            actor.Send("You must specify the number of chambers.");
            return false;
        }
        if (value <= 0)
        {
            actor.Send("The revolver must have at least one chamber.");
            return false;
        }
        Chambers = value;
        Changed = true;
        actor.OutputHandler.Send($"This revolver will have {value} chambers.");
        return true;
    }

    private bool BuildingCommandType(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("Which ranged weapon type do you want to set?");
            return false;
        }
        var type = long.TryParse(command.PopSpeech(), out var value)
            ? Gameworld.RangedWeaponTypes.Get(value)
            : Gameworld.RangedWeaponTypes.GetByName(command.Last);
        if (type == null)
        {
            actor.Send("That is not a valid ranged weapon type.");
            return false;
        }
        RangedWeaponType = type;
        Changed = true;
        actor.Send($"This revolver now uses the {type.Name} ranged weapon type.");
        return true;
    }

    private bool BuildingCommandLoad(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("You must supply an emote.");
            return false;
        }
        LoadEmote = command.SafeRemainingArgument;
        Changed = true;
        actor.Send("Load emote set.");
        return true;
    }

    private bool BuildingCommandUnload(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("You must supply an emote.");
            return false;
        }
        UnloadEmote = command.SafeRemainingArgument;
        Changed = true;
        actor.Send("Unload emote set.");
        return true;
    }

    private bool BuildingCommandFire(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("You must supply an emote.");
            return false;
        }
        FireEmote = command.SafeRemainingArgument;
        Changed = true;
        actor.Send("Fire emote set.");
        return true;
    }

    private bool BuildingCommandFireNoRound(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("You must supply an emote.");
            return false;
        }
        FireEmoteNoRound = command.SafeRemainingArgument;
        Changed = true;
        actor.Send("Fire empty emote set.");
        return true;
    }
}

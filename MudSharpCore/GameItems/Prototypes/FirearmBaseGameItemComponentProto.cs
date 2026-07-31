using MudSharp.Accounts;
using MudSharp.Combat;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;

namespace MudSharp.GameItems.Prototypes;

public abstract class FirearmBaseGameItemComponentProto : GameItemComponentProto, IRangedWeaponPrototype, ISwitchablePrototype, IMeleeWeaponPrototype, IConditionDegradingComponentPrototype
{
    protected FirearmBaseGameItemComponentProto(IFuturemud gameworld, IAccount originator, string type)
        : base(gameworld, originator, type)
    {
        FireModes.Add(new FirearmFireMode(FirearmFireModeType.Single, 1, 0.0, 0.0, 0.0));
    }
    protected FirearmBaseGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
        : base(proto, gameworld)
    {
    }

    protected override void LoadFromXml(XElement root)
    {
        LoadEmote = root.Element("LoadEmote").Value;
        ReadyEmote = root.Element("ReadyEmote").Value;
        UnloadEmote = root.Element("UnloadEmote").Value;
        UnreadyEmote = root.Element("UnreadyEmote").Value;
        UnreadyEmoteNoChamberedRound = root.Element("UnreadyEmoteNoChamberedRound").Value;
        FireEmote = root.Element("FireEmote").Value;
        FireEmoteNoChamberedRound = root.Element("FireEmoteNoChamberedRound").Value;
        _rangedWeaponType = Gameworld.RangedWeaponTypes.Get(long.Parse(root.Element("RangedWeaponType").Value));
        ClipType = root.Element("ClipType")?.Value ?? Gameworld.GetStaticConfiguration("DefaultGunClipType");

        CanWieldProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("CanWieldProg")?.Value ?? "0"));
        WhyCannotWieldProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("WhyCannotWieldProg")?.Value ?? "0"));
        XElement element = root.Element("MeleeWeaponType");
        if (element != null)
        {
            MeleeWeaponType = Gameworld.WeaponTypes.Get(long.Parse(element.Value));
        }
        else
        {
            MeleeWeaponType = Gameworld.WeaponTypes.Get(Gameworld.GetStaticLong("DefaultGunMeleeWeaponType"));
        }
        ConditionMaintenance.LoadFromXml(root);
        AttachmentSlots.Clear();
        foreach (var slot in root.Element("AttachmentSlots")?.Elements("Slot") ?? [])
        {
            var slotTypeText = slot.Attribute("type")?.Value;
            if (slotTypeText is null ||
                !slotTypeText.TryParseEnum<FirearmAttachmentSlotType>(out var slotType))
            {
                continue;
            }

            AttachmentSlots.Add(new FirearmAttachmentSlot(
                slot.Attribute("name")?.Value ?? "attachment",
                slotType,
                slot.Attribute("formFactor")?.Value ?? "universal"));
        }

        FireModes.Clear();
        foreach (var mode in root.Element("FireModes")?.Elements("Mode") ?? [])
        {
            var modeTypeText = mode.Attribute("type")?.Value;
            if (modeTypeText is null ||
                !modeTypeText.TryParseEnum<FirearmFireModeType>(out var modeType))
            {
                continue;
            }

            FireModes.Add(new FirearmFireMode(
                modeType,
                Math.Clamp((int?)mode.Attribute("rounds") ?? 1, 1, FirearmFireMode.MaximumRoundsPerTrigger),
                (double?)mode.Attribute("recoil") ?? 0.0,
                (double?)mode.Attribute("stamina") ?? 0.0,
                (double?)mode.Attribute("delay") ?? 0.0));
        }

        if (FireModes.Count == 0)
        {
            FireModes.Add(new FirearmFireMode(FirearmFireModeType.Single, 1, 0.0, 0.0, 0.0));
        }

        CycleType = root.Element("CycleType")?.Value.TryParseEnum<FirearmCycleType>(out var cycleType) == true
            ? cycleType
            : FirearmCycleType.SelfLoading;
    }

    public List<FirearmAttachmentSlot> AttachmentSlots { get; } = [];
    public List<FirearmFireMode> FireModes { get; } = [];
    public FirearmCycleType CycleType { get; set; } = FirearmCycleType.SelfLoading;

    public IWeaponType MeleeWeaponType { get; set; }
    public ConditionMaintenanceProfile ConditionMaintenance { get; } = new(ConditionMaintenanceProfile.DefaultRangedOrMeleeUseExpression);

    public IInventoryPlanTemplate LoadTemplate { get; set; }

    public IInventoryPlanTemplate LoadTemplateIgnoreEmpty { get; set; }

    private IRangedWeaponType _rangedWeaponType;
    public IRangedWeaponType RangedWeaponType
    {
        get => _rangedWeaponType;
        set
        {
            _rangedWeaponType = value;
            RecalculateInventoryPlans();
        }
    }

    public string LoadEmote { get; set; }

    public string ReadyEmote { get; set; }

    public string UnloadEmote { get; set; }

    public string UnreadyEmote { get; set; }

    public string UnreadyEmoteNoChamberedRound { get; set; }

    public string FireEmote { get; set; }
    public string FireEmoteNoChamberedRound { get; set; }

    public string ClipType { get; set; }

#nullable enable
    public IFutureProg? CanWieldProg { get; private set; }
    public IFutureProg? WhyCannotWieldProg { get; private set; }
#nullable restore

    protected abstract void RecalculateInventoryPlans();

    public static string BuildingHelpText => $@"You can use the following options:

	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3ranged <ranged type>#0 - sets the ranged weapon type for this component. See {"show ranges".FluentTagMXP("send", "href='show ranges'")} for a list.
	#3canwield <prog>#0 - sets a prog controlling if this can be wielded
	#3canwield none#0 - removes a canwield prog
	#3whycantwield <prog>#0 - sets a prog giving the error message if canwield fails
	#3whycantwield none#0 - clears the whycantwield prog
	#3load <emote>#0 - sets the emote for loading this weapon. $0 is the loader, $1 is the gun, $2 is the clip/round.
	#3unload <emote>#0 - sets the emote for unloading this weapon. $0 is the loader, $1 is the gun, $2 is the clip/round.
	#3ready <emote>#0 - sets the emote for readying this gun. $0 is the loader, $1 is the gun.
	#3unready <emote>#0 - sets the emote for unreadying this gun. $0 is the loader, $1 is the gun and $2 is the chambered round.
	#3unreadyempty <emote>#0 - sets the emote for unreadying this gun when there is no chambered round. $0 is the loader, $1 is the gun.
	#3fire <emote>#0 - sets the emote for firing the gun. $0 is the firer, $1 is the target, $2 is the gun.
	#3fireempty <emote>#0 - sets the emote for firing the gun when it is empty. $0 is the firer, $1 is the target, $2 is the gun.
	#3slot add <name> <category> <form-factor>#0 - adds an attachment slot
	#3slot remove <name>#0 - removes an attachment slot
	#3mode add <single|burst|automatic> <rounds> <recoil> <extra stamina> <extra delay>#0 - adds or replaces a fire mode
	#3mode remove <single|burst|automatic>#0 - removes a fire mode
	#3cycle <manual|self-loading>#0 - sets the weapon's action cycle
	#3condition <option>#0 - configures optional condition degradation.";

    public override string ShowBuildingHelp =>
        BuildingHelpText;

    #region Building Commands

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "ranged":
            case "ranged type":
            case "rangedtype":
            case "type":
                return BuildingCommandType(actor, command);
            case "load":
                return BuildingCommandLoadEmote(actor, command);
            case "unload":
                return BuildingCommandUnloadEmote(actor, command);
            case "ready":
                return BuildingCommandReadyEmote(actor, command);
            case "unready":
                return BuildingCommandUnreadyEmote(actor, command);
            case "unreadynoround":
            case "unreadynochamberedround":
            case "unreadynochambered":
            case "unreadyempty":
                return BuildingCommandUnreadyEmoteNoChamberedRound(actor, command);
            case "fire":
                return BuildingCommandFireEmote(actor, command);
            case "firenoround":
            case "firenochamberedround":
            case "firenochambered":
            case "fireempty":
                return BuildingCommandFireEmoteNoChamberedRound(actor, command);
            case "melee":
            case "meleetype":
            case "melee type":
            case "melee_type":
                return BuildingCommand_Melee(actor, command);
            case "canwield":
            case "canwieldprog":
                return BuildingCommandCanWieldProg(actor, command);
            case "whycantwield":
            case "whycantwieldprog":
            case "whycannotwield":
            case "whycannotwieldprog":
                return BuildingCommandWhyCannotWieldProg(actor, command);
            case "condition":
                return ConditionMaintenance.BuildingCommand(actor, command, () => Changed = true);
            case "slot":
            case "attachment":
                return BuildingCommandSlot(actor, command);
            case "mode":
            case "firemode":
                return BuildingCommandMode(actor, command);
            case "cycle":
            case "action":
                return BuildingCommandCycle(actor, command);
            default:
                return base.BuildingCommand(actor, command);
        }
    }

    private bool BuildingCommandSlot(ICharacter actor, StringStack command)
    {
        var action = command.PopForSwitch();
        if (action is "remove" or "delete")
        {
            if (command.IsFinished)
            {
                actor.OutputHandler.Send("Which attachment slot do you want to remove?");
                return false;
            }

            var slot = AttachmentSlots.FirstOrDefault(x => x.Name.EqualTo(command.SafeRemainingArgument));
            if (slot is null)
            {
                actor.OutputHandler.Send("There is no attachment slot with that name.");
                return false;
            }

            AttachmentSlots.Remove(slot);
            Changed = true;
            actor.OutputHandler.Send($"The {slot.Name.ColourName()} attachment slot has been removed.");
            return true;
        }

        if (action != "add")
        {
            actor.OutputHandler.Send(
                "Use #3slot add <name> <category> <form-factor>#0 or #3slot remove <name>#0."
                    .SubstituteANSIColour());
            return false;
        }

        var name = command.PopSpeech();
        var categoryText = command.PopSpeech();
        var formFactor = command.SafeRemainingArgument;
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(formFactor) ||
            !categoryText.TryParseEnum<FirearmAttachmentSlotType>(out var category))
        {
            actor.OutputHandler.Send(
                $"You must specify a unique name, one of {Enum.GetValues<FirearmAttachmentSlotType>().Select(x => x.DescribeEnum().ColourName()).ListToString()}, and a form factor.");
            return false;
        }

        if (AttachmentSlots.Any(x => x.Name.EqualTo(name)))
        {
            actor.OutputHandler.Send("Attachment slot names must be unique.");
            return false;
        }

        AttachmentSlots.Add(new FirearmAttachmentSlot(name, category, formFactor));
        Changed = true;
        actor.OutputHandler.Send(
            $"Added the {name.ColourName()} {category.DescribeEnum().ColourName()} slot using form factor {formFactor.ColourValue()}.");
        return true;
    }

    private bool BuildingCommandMode(ICharacter actor, StringStack command)
    {
        var action = command.PopForSwitch();
        if (action is "remove" or "delete")
        {
            if (command.IsFinished ||
                !command.SafeRemainingArgument.TryParseEnum<FirearmFireModeType>(out var removeMode))
            {
                actor.OutputHandler.Send("Which fire mode do you want to remove?");
                return false;
            }

            var existing = FireModes.FirstOrDefault(x => x.Type == removeMode);
            if (existing is null || FireModes.Count == 1 ||
                CycleType == FirearmCycleType.Manual && removeMode == FirearmFireModeType.Single)
            {
                actor.OutputHandler.Send(existing is null
                    ? "This firearm does not have that fire mode."
                    : FireModes.Count == 1
                        ? "A firearm must retain at least one fire mode."
                        : "A manual-action firearm must retain single fire.");
                return false;
            }

            FireModes.Remove(existing);
            Changed = true;
            actor.OutputHandler.Send($"The {removeMode.DescribeEnum().ColourName()} fire mode has been removed.");
            return true;
        }

        if (action != "add")
        {
            actor.OutputHandler.Send(
                "Use #3mode add <single|burst|automatic> <rounds> <recoil> <extra stamina> <extra delay>#0 or #3mode remove <mode>#0."
                    .SubstituteANSIColour());
            return false;
        }

        var typeText = command.PopSpeech();
        var roundsText = command.PopSpeech();
        var recoilText = command.PopSpeech();
        var staminaText = command.PopSpeech();
        var delayText = command.PopSpeech();
        if (!typeText.TryParseEnum<FirearmFireModeType>(out var type) ||
            !int.TryParse(roundsText, out var rounds) ||
            rounds is < 1 or > FirearmFireMode.MaximumRoundsPerTrigger ||
            !double.TryParse(recoilText, out var recoil) ||
            !double.TryParse(staminaText, out var stamina) ||
            !double.TryParse(delayText, out var delay) ||
            recoil < 0.0 || stamina < 0.0 || delay < 0.0)
        {
            actor.OutputHandler.Send(
                $"Specify a mode, 1-{FirearmFireMode.MaximumRoundsPerTrigger} rounds, and non-negative recoil, extra stamina and extra delay values.");
            return false;
        }

        if (type == FirearmFireModeType.Single && rounds != 1)
        {
            actor.OutputHandler.Send("Single fire mode must fire exactly one round.");
            return false;
        }

        var oldMode = FireModes.FirstOrDefault(x => x.Type == type);
        if (oldMode is not null)
        {
            FireModes.Remove(oldMode);
        }

        FireModes.Add(new FirearmFireMode(type, rounds, recoil, stamina, delay));
        Changed = true;
        actor.OutputHandler.Send(
            $"The {type.DescribeEnum().ColourName()} mode now fires {rounds.ToString("N0", actor).ColourValue()} round{(rounds == 1 ? string.Empty : "s")} per trigger pull.");
        return true;
    }

    private bool BuildingCommandCycle(ICharacter actor, StringStack command)
    {
        var cycleText = command.SafeRemainingArgument.Replace("-", " ");
        if (command.IsFinished ||
            !cycleText.TryParseEnum<FirearmCycleType>(out var value))
        {
            actor.OutputHandler.Send("You must specify either #3manual#0 or #3self-loading#0."
                .SubstituteANSIColour());
            return false;
        }

        CycleType = value;
        if (CycleType == FirearmCycleType.Manual &&
            FireModes.All(x => x.Type != FirearmFireModeType.Single))
        {
            FireModes.Add(new FirearmFireMode(FirearmFireModeType.Single, 1, 0.0, 0.0, 0.0));
        }
        Changed = true;
        actor.OutputHandler.Send($"This firearm now uses a {CycleType.DescribeEnum(true).ColourName()} action.");
        return true;
    }

    protected IEnumerable<XElement> SaveFirearmConfiguration()
    {
        yield return new XElement("AttachmentSlots",
            AttachmentSlots.Select(x => new XElement("Slot",
                new XAttribute("name", x.Name),
                new XAttribute("type", x.Type),
                new XAttribute("formFactor", x.FormFactor))));
        yield return new XElement("FireModes",
            FireModes.Select(x => new XElement("Mode",
                new XAttribute("type", x.Type),
                new XAttribute("rounds", x.RoundsPerTrigger),
                new XAttribute("recoil", x.RecoilPenalty),
                new XAttribute("stamina", x.ExtraStaminaPerRound),
                new XAttribute("delay", x.ExtraDelayPerRound))));
        yield return new XElement("CycleType", CycleType);
    }

    protected string DescribeFirearmConfiguration(ICharacter actor)
    {
        var slots = AttachmentSlots.Any()
            ? AttachmentSlots.Select(x =>
                    $"{x.Name.ColourName()} ({x.Type.DescribeEnum().ColourName()}, {x.FormFactor.ColourValue()})")
                .ListToString()
            : "None".ColourError();
        var modes = FireModes.Select(x =>
                $"{x.Type.DescribeEnum().ColourName()} [{x.RoundsPerTrigger.ToString("N0", actor).ColourValue()}]")
            .ListToString();
        return
            $"Action Cycle: {CycleType.DescribeEnum(true).ColourName()}\nFire Modes: {modes}\nAttachment Slots: {slots}";
    }

    private bool BuildingCommandCanWieldProg(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"You must either specify a prog, or the keyword #3none#0 to remove one.".SubstituteANSIColour());
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("none"))
        {
            CanWieldProg = null;
            Changed = true;
            actor.OutputHandler.Send($"This item will no longer use a prog to determine if it can be wielded.");
            return true;
        }

        IFutureProg prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Boolean,
            [
                [ProgVariableTypes.Character],
                [ProgVariableTypes.Character, ProgVariableTypes.Item]
            ]
        ).LookupProg();
        if (prog is null)
        {
            return false;
        }

        CanWieldProg = prog;
        Changed = true;
        actor.OutputHandler.Send($"This item will now use the {prog.MXPClickableFunctionName()} prog to determine if it can be wielded.");
        return true;
    }

    private bool BuildingCommandWhyCannotWieldProg(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send($"You must either specify a prog, or the keyword #3none#0 to remove one.".SubstituteANSIColour());
            return false;
        }

        if (command.SafeRemainingArgument.EqualTo("none"))
        {
            CanWieldProg = null;
            Changed = true;
            actor.OutputHandler.Send($"This item will no longer use a prog to generate an error message if it cannot be wielded.");
            return true;
        }

        IFutureProg prog = new ProgLookupFromBuilderInput(actor, command.SafeRemainingArgument, ProgVariableTypes.Text,
            [
                [ProgVariableTypes.Character],
                [ProgVariableTypes.Character, ProgVariableTypes.Item]
            ]
        ).LookupProg();
        if (prog is null)
        {
            return false;
        }

        WhyCannotWieldProg = prog;
        Changed = true;
        actor.OutputHandler.Send($"This item will now use the {prog.MXPClickableFunctionName()} prog to generate an error message if it cannot be wielded.");
        return true;
    }

    private bool BuildingCommand_Melee(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("Which melee weapon type do you want to set for this component?");
            return false;
        }

        IWeaponType type = long.TryParse(command.PopSpeech(), out long value)
            ? actor.Gameworld.WeaponTypes.Get(value)
            : actor.Gameworld.WeaponTypes.GetByName(command.Last);
        if (type == null)
        {
            actor.Send("There is no such melee weapon type.");
            return false;
        }

        MeleeWeaponType = type;
        Changed = true;
        actor.Send(
            $"This component will now use the melee weapon type {MeleeWeaponType.Name.TitleCase().Colour(Telnet.Green)}.");
        return true;
    }

    private bool BuildingCommandLoadEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What emote do you want to set for when people load a clip into this gun?");
            actor.Send("Hint: $0 is the loader, $1 is the gun, $2 is the clip.".Colour(Telnet.Yellow));
            return false;
        }

        LoadEmote = command.RemainingArgument.Fullstop();
        Changed = true;
        actor.Send($"The following emote will now be used when this gun is loaded:\n\n{LoadEmote}\n");
        return true;
    }

    private bool BuildingCommandReadyEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What emote do you want to set for when people ready this gun?");
            actor.Send("Hint: $0 is the loader, $1 is the gun.".Colour(Telnet.Yellow));
            return false;
        }

        ReadyEmote = command.RemainingArgument.Fullstop();
        Changed = true;
        actor.Send($"The following emote will now be used when this gun is readied:\n\n{ReadyEmote}\n");
        return true;
    }

    private bool BuildingCommandUnloadEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What emote do you want to set for when people unload a clip or round from this gun?");
            actor.Send("Hint: $0 is the loader, $1 is the gun, $2 is the clip.".Colour(Telnet.Yellow));
            return false;
        }

        UnloadEmote = command.RemainingArgument.Fullstop();
        Changed = true;
        actor.Send($"The following emote will now be used when this gun is unloaded:\n\n{UnloadEmote}\n");
        return true;
    }

    private bool BuildingCommandUnreadyEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What emote do you want to set for when people empty the chamber on this gun?");
            actor.Send("Hint: $0 is the loader, $1 is the gun, $2 is the round in the chamber.".Colour(Telnet.Yellow));
            return false;
        }

        UnreadyEmote = command.RemainingArgument.Fullstop();
        Changed = true;
        actor.Send(
            $"The following emote will now be used when the chamber of this gun is emptied:\n\n{UnreadyEmote}\n");
        return true;
    }

    private bool BuildingCommandUnreadyEmoteNoChamberedRound(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send(
                "What emote do you want to set for when people empty the chamber, but there is no chambered round?");
            actor.Send("Hint: $0 is the loader, $1 is the gun.".Colour(Telnet.Yellow));
            return false;
        }

        UnreadyEmoteNoChamberedRound = command.RemainingArgument.Fullstop();
        Changed = true;
        actor.Send(
            $"The following emote will now be used when the chambered of this gun is emptied when already empty:\n\n{UnreadyEmoteNoChamberedRound}\n");
        return true;
    }

    private bool BuildingCommandFireEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What emote do you want to set for when people fire this gun?");
            actor.Send("Hint: $0 is the loader, $1 is the target, $2 is the gun.".Colour(Telnet.Yellow));
            return false;
        }

        FireEmote = command.RemainingArgument.Fullstop();
        Changed = true;
        actor.Send($"The following emote will now be used when this gun is fired:\n\n{FireEmote}\n");
        return true;
    }

    private bool BuildingCommandFireEmoteNoChamberedRound(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("What emote do you want to set for when people fire the gun while the chamber is empty?");
            actor.Send("Hint: $0 is the loader, $1 is the target, $2 is the gun.".Colour(Telnet.Yellow));
            return false;
        }

        FireEmoteNoChamberedRound = command.RemainingArgument.Fullstop();
        Changed = true;
        actor.Send(
            $"The following emote will now be used when this gun is fired while empty:\n\n{FireEmoteNoChamberedRound}\n");
        return true;
    }

    private bool BuildingCommandType(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send(
                $"What Ranged Weapon Type do you want to use for this gun? See {"show ranged".Colour(Telnet.Yellow)} for a list of ranged weapon types.");
            return false;
        }

        IRangedWeaponType type = long.TryParse(command.PopSpeech(), out long value)
            ? actor.Gameworld.RangedWeaponTypes.Get(value)
            : actor.Gameworld.RangedWeaponTypes.GetByName(command.Last);
        if (type == null)
        {
            actor.Send("There is no such ranged weapon type.");
            return false;
        }

        if (type.RangedWeaponType != Combat.RangedWeaponType.ModernFirearm)
        {
            actor.Send("You can only give modern firearms a ranged weapon type that is suitable for them.");
            return false;
        }

        RangedWeaponType = type;
        actor.Send(
            $"This gun will now be of type {RangedWeaponType.Name.TitleCase().Colour(Telnet.Green)}.");
        Changed = true;
        return true;
    }

    #endregion

    #region Overrides of EditableItem

    public override bool CanSubmit()
    {
        if (MeleeWeaponType == null)
        {
            return false;
        }

        if (RangedWeaponType == null)
        {
            return false;
        }

        return base.CanSubmit();
    }

    public override string WhyCannotSubmit()
    {
        if (MeleeWeaponType == null)
        {
            return "You must give this component a melee weapon type.";
        }

        if (RangedWeaponType == null)
        {
            return "You must give this component a ranged weapon type.";
        }

        return base.WhyCannotSubmit();
    }

    #endregion
}

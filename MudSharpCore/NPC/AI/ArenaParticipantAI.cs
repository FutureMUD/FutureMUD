#nullable enable
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.NPC.AI;

public class ArenaParticipantAI : PathingAIBase
{
    public bool UseAmbushMode { get; private set; }
    public bool HideDuringPreparation { get; private set; }
    public bool UseSubtleSneak { get; private set; }
    public string EngageDelayDiceExpression { get; private set; } = "250+1d500";
    public string EngageEmote { get; private set; } = string.Empty;

    protected ArenaParticipantAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
    {
    }

    private ArenaParticipantAI()
    {
    }

    private ArenaParticipantAI(IFuturemud gameworld, string name) : base(gameworld, name, "ArenaParticipant")
    {
        UseAmbushMode = false;
        HideDuringPreparation = false;
        UseSubtleSneak = false;
        OpenDoors = true;
        UseKeys = false;
        SmashLockedDoors = false;
        CloseDoorsBehind = false;
        UseDoorguards = false;
        MoveEvenIfObstructionInWay = false;
        DatabaseInitialise();
    }

    public static void RegisterLoader()
    {
        RegisterAIType("ArenaParticipant", (ai, gameworld) => new ArenaParticipantAI(ai, gameworld));
        RegisterAIBuilderInformation("arenaparticipant",
            (gameworld, name) => new ArenaParticipantAI(gameworld, name),
            new ArenaParticipantAI().HelpText);
    }

    protected override void LoadFromXML(XElement root)
    {
        base.LoadFromXML(root);
        UseAmbushMode = bool.Parse(root.Element("UseAmbushMode")?.Value ?? "false");
        HideDuringPreparation = bool.Parse(root.Element("HideDuringPreparation")?.Value ?? "false");
        UseSubtleSneak = bool.Parse(root.Element("UseSubtleSneak")?.Value ?? "false");
        EngageDelayDiceExpression = root.Element("EngageDelayDiceExpression")?.Value ?? "250+1d500";
        EngageEmote = root.Element("EngageEmote")?.Value ?? string.Empty;
    }

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("UseAmbushMode", UseAmbushMode),
            new XElement("HideDuringPreparation", HideDuringPreparation),
            new XElement("UseSubtleSneak", UseSubtleSneak),
            new XElement("EngageDelayDiceExpression", new XCData(EngageDelayDiceExpression)),
            new XElement("EngageEmote", new XCData(EngageEmote)),
            new XElement("OpenDoors", OpenDoors),
            new XElement("UseKeys", UseKeys),
            new XElement("SmashLockedDoors", SmashLockedDoors),
            new XElement("CloseDoorsBehind", CloseDoorsBehind),
            new XElement("UseDoorguards", UseDoorguards),
            new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay)
        ).ToString();
    }

    public override string Show(ICharacter actor)
    {
        StringBuilder sb = new(base.Show(actor));
        sb.AppendLine($"Ambush Mode: {UseAmbushMode.ToColouredString()}");
        sb.AppendLine($"Hide In Preparation: {HideDuringPreparation.ToColouredString()}");
        sb.AppendLine($"Use Subtle Sneak: {UseSubtleSneak.ToColouredString()}");
        sb.AppendLine($"Engage Delay: {EngageDelayDiceExpression.ColourValue()} ms");
        sb.AppendLine($"Engage Emote: {EngageEmote.ColourCommand()}");
        return sb.ToString();
    }

    protected override string TypeHelpText => $@"{base.TypeHelpText}
	#3ambush#0 - toggles stealth/ambush preparation behaviour
	#3hide#0 - toggles hiding during arena preparation
	#3subtle#0 - toggles subtle sneak when ambushing
	#3delay <expression>#0 - sets the engage delay in milliseconds
	#3emote <emote>#0 - sets an optional engage emote
	#3emote clear#0 - clears the engage emote";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "ambush":
                return BuildingCommandAmbush(actor);
            case "hide":
                return BuildingCommandHide(actor);
            case "subtle":
                return BuildingCommandSubtle(actor);
            case "delay":
                return BuildingCommandDelay(actor, command);
            case "emote":
                return BuildingCommandEmote(actor, command);
        }

        return base.BuildingCommand(actor, command.GetUndo());
    }

    private bool BuildingCommandAmbush(ICharacter actor)
    {
        UseAmbushMode = !UseAmbushMode;
        Changed = true;
        actor.OutputHandler.Send($"This AI will {UseAmbushMode.NowNoLonger()} try to prepare stealthily before arena bouts.");
        return true;
    }

    private bool BuildingCommandHide(ICharacter actor)
    {
        HideDuringPreparation = !HideDuringPreparation;
        Changed = true;
        actor.OutputHandler.Send($"This AI will {HideDuringPreparation.NowNoLonger()} attempt to hide while preparing.");
        return true;
    }

    private bool BuildingCommandSubtle(ICharacter actor)
    {
        UseSubtleSneak = !UseSubtleSneak;
        Changed = true;
        actor.OutputHandler.Send($"This AI will {UseSubtleSneak.NowNoLonger()} use subtle sneaking.");
        return true;
    }

    private bool BuildingCommandDelay(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !Dice.IsDiceExpression(command.SafeRemainingArgument))
        {
            actor.OutputHandler.Send("You must enter a valid dice expression for the arena engage delay.");
            return false;
        }

        EngageDelayDiceExpression = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send($"This AI will now wait {EngageDelayDiceExpression.ColourValue()} milliseconds before engaging.");
        return true;
    }

    private bool BuildingCommandEmote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must supply an emote or use #3clear#0.");
            return false;
        }

        if (command.SafeRemainingArgument.EqualToAny("clear", "none", "remove", "delete"))
        {
            EngageEmote = string.Empty;
            Changed = true;
            actor.OutputHandler.Send("This AI will no longer emote when it engages arena opponents.");
            return true;
        }

        Emote emote = new(command.SafeRemainingArgument, new DummyPerceiver(), new DummyPerceivable(),
            new DummyPerceivable());
        if (!emote.Valid)
        {
            actor.OutputHandler.Send(emote.ErrorMessage);
            return false;
        }

        EngageEmote = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send($"This AI will now emote {EngageEmote.ColourCommand()} when it engages.");
        return true;
    }

    public override bool HandleEvent(EventType type, params dynamic[] arguments)
    {
        if (type == EventType.TenSecondTick)
        {
            ICharacter? character = arguments[0] as ICharacter;
            if (character is null || character.State.IsDead() || character.State.IsInStatis())
            {
                return false;
            }

            EvaluateArenaState(character);
        }

        return base.HandleEvent(type, arguments);
    }

    public override bool HandlesEvent(params EventType[] types)
    {
        foreach (EventType type in types)
        {
            if (type == EventType.TenSecondTick)
            {
                return true;
            }
        }

        return base.HandlesEvent(types);
    }

    internal static IArenaParticipant? GetParticipant(IArenaEvent arenaEvent, ICharacter character)
    {
        return arenaEvent.Participants.FirstOrDefault(x => x.Character?.Id == character.Id);
    }

    internal static IReadOnlyCollection<IArenaParticipant> GetOpponents(IArenaEvent arenaEvent, ICharacter character)
    {
        IArenaParticipant? participant = GetParticipant(arenaEvent, character);
        if (participant is null)
        {
            return Array.Empty<IArenaParticipant>();
        }

        return arenaEvent.Participants
            .Where(x => x.SideIndex != participant.SideIndex)
            .Where(x => x.Character is not null)
            .ToList();
    }

    private void EvaluateArenaState(ICharacter character)
    {
        ArenaNpcPreparationEffect? prepEffect = character.CombinedEffectsOfType<ArenaNpcPreparationEffect>()
            .OrderByDescending(x => x.IsParticipating)
            .FirstOrDefault();
        if (prepEffect is null)
        {
            return;
        }

        IArenaEvent? arenaEvent = Gameworld.CombatArenas
            .SelectMany(x => x.ActiveEvents)
            .FirstOrDefault(x => x.Id == prepEffect.EventId);
        if (arenaEvent is null)
        {
            return;
        }

        if (!prepEffect.IsParticipating || arenaEvent.State == ArenaEventState.Preparing)
        {
            PrepareForArena(character);
            return;
        }

        EngageArenaOpponents(character, arenaEvent);
    }

    private void PrepareForArena(ICharacter character)
    {
        TryReadyWeapons(character);
        if (!UseAmbushMode)
        {
            return;
        }

        if (!character.AffectedBy<ISneakEffect>())
        {
            character.AddEffect(UseSubtleSneak ? new SneakSubtle(character) : new Sneak(character));
        }

        if (HideDuringPreparation && !character.AffectedBy<IHideEffect>())
        {
            character.ExecuteCommand("hide");
        }
    }

    private void EngageArenaOpponents(ICharacter character, IArenaEvent arenaEvent)
    {
        if (!character.State.IsAble() || character.Combat != null || character.Movement != null)
        {
            return;
        }

        if (character.Effects.Any(x => x.IsBlockingEffect("combat-engage") || x.IsBlockingEffect("general")))
        {
            return;
        }

        List<ICharacter> opponents = GetOpponents(arenaEvent, character)
            .Select(x => x.Character)
            .Where(x => x is not null)
            .Cast<ICharacter>()
            .Where(x => !x.State.IsDead())
            .ToList();
        if (!opponents.Any())
        {
            return;
        }

        if (TryEngageVisibleOpponent(character, opponents))
        {
            return;
        }

        CheckPathingEffect(character, true);
    }

    private bool TryEngageVisibleOpponent(ICharacter character, IEnumerable<ICharacter> opponents)
    {
        ICharacter? meleeTarget = opponents
            .Where(x => ReferenceEquals(x.Location, character.Location))
            .Where(x => character.CanSee(x))
            .Where(x => character.CanEngage(x))
            .FirstOrDefault();
        if (meleeTarget is not null)
        {
            BeginArenaEngagement(character, meleeTarget);
            return true;
        }

        int rangedRange = character.Body.WieldedItems
            .SelectNotNull(x => x.GetItemType<IRangedWeapon>())
            .Where(x => x.IsReadied || x.CanReady(character))
            .Select(x => (int)x.WeaponType.DefaultRangeInRooms)
            .DefaultIfEmpty(0)
            .Max();
        if (rangedRange <= 0)
        {
            return false;
        }

        ICharacter? rangedTarget = character.Location.CellsInVicinity((uint)rangedRange, true, true)
            .SelectMany(x => x.Characters)
            .OfType<ICharacter>()
            .Where(opponents.Contains)
            .Where(x => character.CanSee(x))
            .Where(x => character.CanEngage(x))
            .FirstOrDefault();
        if (rangedTarget is null)
        {
            return false;
        }

        BeginArenaEngagement(character, rangedTarget);
        return true;
    }

    private void BeginArenaEngagement(ICharacter character, ICharacter target)
    {
        character.AddEffect(
            new BlockingDelayedAction(character,
                _ => ExecuteArenaEngagement(character, target),
                "preparing to engage an arena opponent",
                new[] { "general", "combat-engage", "movement" },
                null),
            TimeSpan.FromMilliseconds(Dice.Roll(EngageDelayDiceExpression)));
    }

    private void ExecuteArenaEngagement(ICharacter character, ICharacter target)
    {
        if (!character.CanEngage(target) || target.State.IsDead())
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(EngageEmote))
        {
            character.OutputHandler.Send(new EmoteOutput(new Emote(EngageEmote, character, character, target)));
        }

        bool shouldUseRanged = character.Body.WieldedItems
            .SelectNotNull(x => x.GetItemType<IRangedWeapon>())
            .Any(x => x.IsReadied || x.CanReady(character));
        character.Engage(target, shouldUseRanged);
    }

    private static void TryReadyWeapons(ICharacter character)
    {
        foreach (IRangedWeapon? weapon in character.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>()))
        {
            if (!weapon.IsReadied && weapon.CanReady(character))
            {
                weapon.Ready(character);
            }
        }
    }

    protected override bool WouldMove(ICharacter ch)
    {
        ArenaNpcPreparationEffect? prepEffect = ch.CombinedEffectsOfType<ArenaNpcPreparationEffect>()
            .FirstOrDefault(x => x.IsParticipating);
        return prepEffect is not null && ch.Combat is null;
    }

    protected override (ICell Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
    {
        ArenaNpcPreparationEffect? prepEffect = ch.CombinedEffectsOfType<ArenaNpcPreparationEffect>()
            .FirstOrDefault(x => x.IsParticipating);
        if (prepEffect is null)
        {
            return (null, Enumerable.Empty<ICellExit>());
        }

        IArenaEvent? arenaEvent = Gameworld.CombatArenas
            .SelectMany(x => x.ActiveEvents)
            .FirstOrDefault(x => x.Id == prepEffect.EventId);
        if (arenaEvent is null)
        {
            return (null, Enumerable.Empty<ICellExit>());
        }

        List<ICharacter> opponents = GetOpponents(arenaEvent, ch)
            .Select(x => x.Character)
            .Where(x => x is not null)
            .Cast<ICharacter>()
            .Where(x => !x.State.IsDead())
            .Where(x => !ReferenceEquals(x.Location, ch.Location))
            .Where(x => x.Location is not null && arenaEvent.Arena.ArenaCells.Contains(x.Location))
            .ToList();
        if (!opponents.Any())
        {
            return (null, Enumerable.Empty<ICellExit>());
        }

        (ICharacter Opponent, List<ICellExit> Path) bestPath = opponents
            .Select(opponent => (Opponent: opponent, Path: ch.PathBetween(opponent.Location, 30, GetSuitabilityFunction(ch)).ToList()))
            .Where(x => x.Path.Any())
            .OrderBy(x => x.Path.Count)
            .FirstOrDefault();

        return bestPath.Path is null || bestPath.Path.Count == 0
            ? (null, Enumerable.Empty<ICellExit>())
            : (bestPath.Opponent.Location, bestPath.Path);
    }
}

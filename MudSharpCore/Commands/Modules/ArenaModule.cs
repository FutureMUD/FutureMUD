#nullable enable
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Payment;
using MudSharp.Framework;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MudSharp.Commands.Modules;

internal class ArenaModule : Module<ICharacter>
{
    private ArenaModule()
            : base("Arena")
    {
        IsNecessary = true;
    }

    public static ArenaModule Instance { get; } = new();

    private const string ArenaManagerHelp = @"The #3arena#0 command is used to interact with combat arenas. All of these commands need to be done from the arena itself.

	#3arena list#0 - list known arenas
	#3arena show [<arena>]#0 - show details for an arena
	#3arena events [<arena>]#0 - list active and scheduled events
	#3arena observe list#0 - show events observable from your location
	#3arena observe begin [<event>]#0 - begin observing an event
	#3arena observe stop [<event>]#0 - stop observing an event
	#3arena signup [<event>] [<side>] [<class>]#0 - sign up for an event (omitted values auto-select)
	#3arena withdraw [<event>]#0 - withdraw from an event
	#3arena surrender [<event>]#0 - surrender your current bout
	#3arena bet odds [<side>|draw] [<event>]#0 - view betting quotes
	#3arena bet place <side|draw> <amount> [<event>]#0 - place a wager
	#3arena bet cancel [<event>]#0 - cancel your wager
	#3arena bet pools [<event>]#0 - view pari-mutuel pools
	#3arena bet list#0 - list active wagers and payouts
	#3arena bet history [<count>]#0 - show betting history
	#3arena bet collect [<event>]#0 - collect outstanding payouts
	#3arena ratings [<class>]#0 - show your arena ratings

If only one event applies, you can omit #6<event>#0 in the above commands.

Manager Only Commands:

	#3arena manager phase <event> <state>#0 - force an event to a phase using normal transitions
	#3arena manager autoschedule <eventtype> show#0 - show recurring settings
	#3arena manager autoschedule <eventtype> off#0 - disable recurring creation
	#3arena manager autoschedule <eventtype> every <interval> [from] <reference>#0 - enable recurring creation
	#3arena manager deposit [<arena>] <amount>#0 - deposit physical cash into arena cash reserve
	#3arena manager withdraw [<arena>] <amount>#0 - withdraw physical cash from arena cash reserve
	#3arena manager stable [<arena>] [class <class>] [search <text>] [top <count>]#0 - show stable NPC roster by class
	#3arena manager rating [<arena>] <character>#0 - show ratings for a character in an arena
	#3arena manager ratings [<arena>] [class <class>] [search <text>] [min <rating>] [max <rating>] [sort <name|class|rating|updated>] [desc] [top <count>]#0 - list arena ratings";

    private const string ArenaHelp =
        @"The #3arena#0 command is used to interact with combat arenas. All of these commands need to be done from the arena itself.

	#3arena events [<arena>]#0 - list active and scheduled events
	#3arena observe list#0 - show events observable from your location
	#3arena observe begin [<event>]#0 - begin observing an event
	#3arena observe stop [<event>]#0 - stop observing an event
	#3arena signup [<event>] [<side>] [<class>]#0 - sign up for an event (omitted values auto-select)
	#3arena withdraw [<event>]#0 - withdraw from an event
	#3arena surrender [<event>]#0 - surrender your current bout
	#3arena bet odds [<side>|draw] [<event>]#0 - view betting quotes
	#3arena bet place <side|draw> <amount> [<event>]#0 - place a wager
	#3arena bet cancel [<event>]#0 - cancel your wager
	#3arena bet pools [<event>]#0 - view pari-mutuel pools
	#3arena bet list#0 - list active wagers and payouts
	#3arena bet history [<count>]#0 - show betting history
	#3arena bet collect [<event>]#0 - collect outstanding payouts
	#3arena ratings [<class>]#0 - show your arena ratings

If only one event applies, you can omit #6<event>#0 in the above commands.";

    [PlayerCommand("Arena", "arena")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [HelpInfo("arena", ArenaHelp, AutoHelp.HelpArg)]
    protected static void Arena(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        if (ss.IsFinished)
        {
            ShowGeneralHelp(actor);
            return;
        }

        string action = ss.PopForSwitch();
        if (actor.Combat is not null && !action.EqualTo("surrender"))
        {
            actor.OutputHandler.Send(
                    "You can only use #3arena surrender#0 while you are already in combat.".SubstituteANSIColour());
            return;
        }

        switch (action)
        {
            case "list":
                ArenaList(actor);
                return;
            case "show":
                ArenaShow(actor, ss);
                return;
            case "events":
                ArenaEvents(actor, ss);
                return;
            case "observe":
                ArenaObserve(actor, ss);
                return;
            case "signup":
                ArenaSignup(actor, ss);
                return;
            case "withdraw":
                ArenaWithdraw(actor, ss);
                return;
            case "surrender":
                ArenaSurrender(actor, ss);
                return;
            case "bet":
                ArenaBet(actor, ss);
                return;
            case "ratings":
                ArenaRatings(actor, ss);
                return;
            case "manager":
            case "manage":
                ArenaManager(actor, ss);
                return;
            default:
                ShowGeneralHelp(actor);
                return;
        }
    }

    private static void ShowGeneralHelp(ICharacter actor)
    {
        if (actor.IsAdministrator() || actor.Gameworld.CombatArenas.Any(x => x.Managers.Contains(actor)))
        {
            actor.OutputHandler.Send(ArenaManagerHelp.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength));
            return;
        }
        actor.OutputHandler.Send(ArenaHelp.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength));
    }

    private static void ArenaList(ICharacter actor)
    {
        if (!actor.IsAdministrator() && !actor.Gameworld.CombatArenas.Any(x => x.Managers.Contains(actor)))
        {
            ShowGeneralHelp(actor);
            return;
        }

        List<ICombatArena> arenas = actor.Gameworld.CombatArenas.ToList();
        if (!arenas.Any())
        {
            actor.OutputHandler.Send("There are no combat arenas configured.".ColourError());
            return;
        }

        string[] header = new[] { "Id", "Arena", "Zone", "Currency" };
        List<string[]> rows = arenas.Select(arena => new[]
        {
                        arena.Id.ToString("N0", actor),
                        arena.Name.ColourName(),
                        arena.EconomicZone.Name.ColourName(),
                        arena.Currency.Name.ColourValue()
                }).ToList();

        actor.OutputHandler.Send(StringUtilities.GetTextTable(rows, header, actor, Telnet.Yellow));
    }

    private static void ArenaShow(ICharacter actor, StringStack ss)
    {
        if (!actor.IsAdministrator() && !actor.Gameworld.CombatArenas.Any(x => x.Managers.Contains(actor)))
        {
            ShowGeneralHelp(actor);
            return;
        }

        bool hasArgument = !ss.IsFinished;
        ICombatArena? arena = GetArena(actor, hasArgument ? ss.PopSpeech() : null);
        if (arena is null)
        {
            if (hasArgument)
            {
                actor.OutputHandler.Send("There is no arena matching that description.".ColourError());
            }
            else
            {
                actor.OutputHandler.Send("Which arena do you want to view? You can omit this if you're in an arena room.".ColourCommand());
            }
            return;
        }

        actor.Gameworld.ArenaCommandService.ShowArena(actor, arena);
    }

    private static void ArenaEvents(ICharacter actor, StringStack ss)
    {
        bool hasArgument = !ss.IsFinished;
        ICombatArena? arena = GetArena(actor, hasArgument ? ss.PopSpeech() : null);
        if (arena is null)
        {
            if (hasArgument)
            {
                actor.OutputHandler.Send("There is no arena matching that description.".ColourError());
            }
            else
            {
                actor.OutputHandler.Send("Which arena's events do you want to view? You can omit this if you're in an arena room.".ColourCommand());
            }
            return;
        }

        List<IArenaEvent> concreteEvents = arena.ActiveEvents.ToList();
        List<(IArenaEventType EventType, DateTime ScheduledFor)> projectedEvents = GetProjectedAutoScheduledEvents(arena, concreteEvents).ToList();
        if (!concreteEvents.Any() && !projectedEvents.Any())
        {
            actor.OutputHandler.Send("There are no active or scheduled events for that arena.".ColourError());
            return;
        }

        string[] header = new[] { "Id", "Name", "Type", "State", "Scheduled" };
        List<string[]> rows = concreteEvents
            .Select(evt => new
            {
                evt.ScheduledAt,
                Id = evt.Id.ToString("N0", actor),
                Name = evt.Name.ColourName(),
                Type = evt.EventType.Name.ColourName(),
                State = evt.State.DescribeEnum().ColourValue(),
                Scheduled = evt.ScheduledAt.GetLocalDate(actor).ToString("f", actor).ColourValue()
            })
            .Concat(projectedEvents.Select(evt => new
            {
                ScheduledAt = evt.ScheduledFor,
                Id = "Auto".ColourCommand(),
                Name = $"{evt.EventType.Name} Event".ColourName(),
                Type = evt.EventType.Name.ColourName(),
                State = "Scheduled (Auto)".ColourCommand(),
                Scheduled = evt.ScheduledFor.GetLocalDate(actor).ToString("f", actor).ColourValue()
            }))
            .OrderBy(x => x.ScheduledAt)
            .ThenBy(x => x.Name)
            .Select(x => new[] { x.Id, x.Name, x.Type, x.State, x.Scheduled })
            .ToList();

        actor.OutputHandler.Send(StringUtilities.GetTextTable(rows, header, actor, Telnet.Cyan));
    }

    private static void ArenaManager(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                "Do you want to #3phase#0, #3autoschedule#0, #3deposit#0, #3withdraw#0, #3stable#0, #3rating#0 or #3ratings#0? Use #3help arena#0 for syntax."
                    .SubstituteANSIColour());
            return;
        }

        switch (ss.PopForSwitch())
        {
            case "phase":
            case "state":
                ArenaManagerPhase(actor, ss);
                return;
            case "autoschedule":
            case "schedule":
                ArenaManagerAutoSchedule(actor, ss);
                return;
            case "deposit":
                ArenaManagerDeposit(actor, ss);
                return;
            case "withdraw":
                ArenaManagerWithdraw(actor, ss);
                return;
            case "stable":
                ArenaManagerStable(actor, ss);
                return;
            case "rating":
                ArenaManagerRating(actor, ss);
                return;
            case "ratings":
                ArenaManagerRatings(actor, ss);
                return;
            default:
                actor.OutputHandler.Send(
                    "Valid options are #3phase#0, #3autoschedule#0, #3deposit#0, #3withdraw#0, #3stable#0, #3rating#0 and #3ratings#0."
                        .SubstituteANSIColour());
                return;
        }
    }

    private static void ArenaManagerPhase(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which event do you want to change the phase for?".ColourCommand());
            return;
        }

        string eventText = ss.PopSpeech();
        IArenaEvent? arenaEvent = GetArenaEvent(actor, eventText);
        if (arenaEvent is null)
        {
            actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
            return;
        }

        if (!arenaEvent.Arena.IsManager(actor))
        {
            actor.OutputHandler.Send("You are not a manager of that arena.".ColourError());
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"Which state should it move to? Valid options are {Enum.GetValues<ArenaEventState>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}.");
            return;
        }

        if (!TryParseArenaState(ss.PopSpeech(), out ArenaEventState targetState))
        {
            actor.OutputHandler.Send(
                $"That is not a valid state. Valid options are {Enum.GetValues<ArenaEventState>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}."
                    .ColourError());
            return;
        }

        ArenaEventState currentState = arenaEvent.State;
        if (currentState == targetState)
        {
            actor.OutputHandler.Send("That event is already in that state.".ColourError());
            return;
        }

        if (targetState != ArenaEventState.Aborted && targetState < currentState)
        {
            actor.OutputHandler.Send(
                $"You can only move forward from {currentState.DescribeEnum().ColourValue()} with this command.".ColourError());
            return;
        }

        actor.Gameworld.ArenaLifecycleService.Transition(arenaEvent, targetState);
        if (arenaEvent.State == currentState)
        {
            actor.OutputHandler.Send("That transition was not valid for the event's current state.".ColourError());
            return;
        }

        actor.OutputHandler.Send(
            $"You move {arenaEvent.Name.ColourName()} from {currentState.DescribeEnum().ColourValue()} to {arenaEvent.State.DescribeEnum().ColourValue()}."
                .Colour(Telnet.Green));
    }

    private static void ArenaManagerAutoSchedule(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which event type do you want to configure auto scheduling for?".ColourCommand());
            return;
        }

        string eventTypeText = ss.PopSpeech();
        IArenaEventType? eventType = GetArenaEventType(actor, eventTypeText);
        if (eventType is null)
        {
            actor.OutputHandler.Send("There is no arena event type matching that description.".ColourError());
            return;
        }

        if (!eventType.Arena.IsManager(actor))
        {
            actor.OutputHandler.Send("You are not a manager of that arena.".ColourError());
            return;
        }

        if (ss.IsFinished || ss.PeekSpeech().EqualTo("show"))
        {
            if (!ss.IsFinished)
            {
                ss.PopSpeech();
            }

            actor.OutputHandler.Send(
                $"Auto scheduling for {eventType.Name.ColourName()} is {DescribeAutoSchedule(eventType, actor)}.");
            return;
        }

        string firstToken = ss.PopForSwitch();
        if (firstToken.EqualToAny("off", "none", "disable", "clear", "remove"))
        {
            eventType.ConfigureAutoSchedule(null, null);
            actor.OutputHandler.Send(
                $"Auto scheduling is now disabled for {eventType.Name.ColourName()}.".Colour(Telnet.Green));
            return;
        }

        string intervalText = firstToken;
        if (firstToken.EqualTo("every"))
        {
            if (ss.IsFinished)
            {
                actor.OutputHandler.Send("You must specify a recurrence interval.".ColourError());
                return;
            }

            intervalText = ss.PopSpeech();
        }

        if (!MudTimeSpan.TryParse(intervalText, actor, out MudTimeSpan? intervalMud))
        {
            actor.OutputHandler.Send(
                "That is not a valid interval. Examples: #36h#0, #390m#0, #31d 2h#0.".SubstituteANSIColour());
            return;
        }

        TimeSpan interval = intervalMud.AsTimeSpan();
        if (interval <= TimeSpan.Zero)
        {
            actor.OutputHandler.Send("The interval must be greater than zero.".ColourError());
            return;
        }

        if (!ss.IsFinished && ss.PeekSpeech().EqualToAny("from", "at", "start", "starting"))
        {
            ss.PopSpeech();
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("What reference date/time should this recurrence use?".ColourCommand());
            return;
        }

        if (!DateUtilities.TryParseDateTimeOrRelative(ss.SafeRemainingArgument, actor.Account, false, out DateTime referenceUtc))
        {
            actor.OutputHandler.Send(
                "That is not a valid reference date/time. Examples: #310:00#0 or #32026-02-18 10:00#0."
                    .SubstituteANSIColour());
            return;
        }

        eventType.ConfigureAutoSchedule(interval, referenceUtc);
        actor.OutputHandler.Send(
            $"Auto scheduling for {eventType.Name.ColourName()} is now {DescribeAutoSchedule(eventType, actor)}."
                .Colour(Telnet.Green));
    }

    private static void ArenaManagerDeposit(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                "How much cash do you want to deposit? Use #3arena manager deposit [<arena>] <amount>#0."
                    .SubstituteANSIColour());
            return;
        }

        string originalArguments = ss.SafeRemainingArgument;
        string firstToken = ss.PopSpeech();
        ICombatArena? explicitArena = GetArena(actor, firstToken);
        if (explicitArena is not null && ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"How much cash do you want to deposit into {explicitArena.Name.ColourName()}?");
            return;
        }

        ICombatArena? arena = explicitArena is not null && !ss.IsFinished ? explicitArena : GetArena(actor, null);
        string amountText = explicitArena is not null && !ss.IsFinished ? ss.SafeRemainingArgument : originalArguments;
        if (arena is null)
        {
            actor.OutputHandler.Send(
                "Which arena do you want to deposit cash to? You can omit this if you're in an arena room."
                    .ColourCommand());
            return;
        }

        if (!arena.IsManager(actor))
        {
            actor.OutputHandler.Send("You are not a manager of that arena.".ColourError());
            return;
        }

        if (!arena.Currency.TryGetBaseCurrency(amountText, out decimal amount) || amount <= 0.0m)
        {
            actor.OutputHandler.Send("That is not a valid amount.".ColourError());
            return;
        }

        OtherCashPayment payment = new(arena.Currency, actor);
        decimal available = payment.AccessibleMoneyForPayment();
        if (available < amount)
        {
            actor.OutputHandler.Send(
                $"You only have {arena.Currency.Describe(available, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} of that currency available.");
            return;
        }

        payment.TakePayment(amount);
        arena.CreditCash(amount, $"Arena manager cash deposit by {actor.Id:N0}");
        actor.OutputHandler.Send(
            $"You deposit {arena.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} into {arena.Name.ColourName()}'s cash reserve."
                .Colour(Telnet.Green));
    }

    private static void ArenaManagerWithdraw(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                "How much cash do you want to withdraw? Use #3arena manager withdraw [<arena>] <amount>#0."
                    .SubstituteANSIColour());
            return;
        }

        string originalArguments = ss.SafeRemainingArgument;
        string firstToken = ss.PopSpeech();
        ICombatArena? explicitArena = GetArena(actor, firstToken);
        if (explicitArena is not null && ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"How much cash do you want to withdraw from {explicitArena.Name.ColourName()}?");
            return;
        }

        ICombatArena? arena = explicitArena is not null && !ss.IsFinished ? explicitArena : GetArena(actor, null);
        string amountText = explicitArena is not null && !ss.IsFinished ? ss.SafeRemainingArgument : originalArguments;
        if (arena is null)
        {
            actor.OutputHandler.Send(
                "Which arena do you want to withdraw cash from? You can omit this if you're in an arena room."
                    .ColourCommand());
            return;
        }

        if (!arena.IsManager(actor))
        {
            actor.OutputHandler.Send("You are not a manager of that arena.".ColourError());
            return;
        }

        if (!arena.Currency.TryGetBaseCurrency(amountText, out decimal amount) || amount <= 0.0m)
        {
            actor.OutputHandler.Send("That is not a valid amount.".ColourError());
            return;
        }

        (bool Truth, string Reason) ensureCash = arena.EnsureCashFunds(amount);
        if (!ensureCash.Truth)
        {
            actor.OutputHandler.Send(ensureCash.Reason.ColourError());
            return;
        }

        arena.DebitCash(amount, $"Arena manager cash withdrawal by {actor.Id:N0}");
        GameItems.IGameItem pile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(arena.Currency,
            arena.Currency.FindCoinsForAmount(amount, out _));
        if (actor.Body is not null && actor.Body.CanGet(pile, 0))
        {
            actor.Body.Get(pile, silent: true);
        }
        else if (actor.Location is not null)
        {
            actor.Location.Insert(pile, true);
            actor.OutputHandler.Send("You couldn't hold that money, so it is on the ground.".Colour(Telnet.Yellow));
        }
        else
        {
            pile.Delete();
        }

        actor.OutputHandler.Send(
            $"You withdraw {arena.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} from {arena.Name.ColourName()}'s cash reserve."
                .Colour(Telnet.Green));
    }

    private static void ArenaManagerStable(ICharacter actor, StringStack ss)
    {
        ICombatArena? arena = ResolveOptionalArenaArgument(actor, ss, "class", "search", "top");
        if (arena is null)
        {
            actor.OutputHandler.Send(
                "Which arena do you want to inspect? You can omit this if you're in an arena room."
                    .ColourCommand());
            return;
        }

        if (!arena.IsManager(actor))
        {
            actor.OutputHandler.Send("You are not a manager of that arena.".ColourError());
            return;
        }

        ICombatantClass? classFilter = null;
        string? search = null;
        int top = 100;
        while (!ss.IsFinished)
        {
            switch (ss.PopForSwitch())
            {
                case "class":
                    if (ss.IsFinished)
                    {
                        actor.OutputHandler.Send("Which combatant class do you want to filter by?");
                        return;
                    }

                    classFilter = FindCombatantClass(arena.CombatantClasses, ss.PopSpeech());
                    if (classFilter is null)
                    {
                        actor.OutputHandler.Send("There is no combatant class matching that filter.".ColourError());
                        return;
                    }

                    break;
                case "search":
                    if (ss.IsFinished)
                    {
                        actor.OutputHandler.Send("What text do you want to search for?");
                        return;
                    }

                    search = ss.PopSpeech();
                    break;
                case "top":
                    if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out top) || top <= 0)
                    {
                        actor.OutputHandler.Send("You must specify a positive whole number for the top count.");
                        return;
                    }

                    break;
                default:
                    actor.OutputHandler.Send(
                        "Valid options are #3class <class>#0, #3search <text>#0, and #3top <count>#0."
                            .SubstituteANSIColour());
                    return;
            }
        }

        List<ICharacter> stableNpcs = arena.NpcStablesCells
            .SelectMany(x => x.Characters)
            .Where(x => !x.IsPlayerCharacter)
            .GroupBy(x => x.Id)
            .Select(x => x.First())
            .ToList();
        if (!stableNpcs.Any())
        {
            actor.OutputHandler.Send("There are no NPCs currently in the stable cells for that arena.".ColourError());
            return;
        }

        List<ICombatantClass> classes = classFilter is null
            ? arena.CombatantClasses.OrderBy(x => x.Name).ToList()
            : new List<ICombatantClass> { classFilter };
        List<(string ClassName, string NpcName, decimal Rating, string Location)> rows = new();
        foreach (ICombatantClass combatantClass in classes)
        {
            foreach (ICharacter npc in stableNpcs)
            {
                if (!IsEligibleForCombatantClass(npc, combatantClass))
                {
                    continue;
                }

                string npcName = npc.HowSeen(actor, flags: PerceiveIgnoreFlags.TrueDescription);
                if (!string.IsNullOrWhiteSpace(search) &&
                    !npcName.Contains(search, StringComparison.InvariantCultureIgnoreCase) &&
                    !combatantClass.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                decimal rating = actor.Gameworld.ArenaRatingsService.GetRating(npc, combatantClass);
                rows.Add((
                    combatantClass.Name,
                    npcName,
                    rating,
                    npc.Location?.GetFriendlyReference(actor) ?? "Nowhere"));
            }
        }

        if (!rows.Any())
        {
            actor.OutputHandler.Send("No stable NPCs matched your filters.".ColourError());
            return;
        }

        rows = rows
            .OrderBy(x => x.ClassName)
            .ThenByDescending(x => x.Rating)
            .ThenBy(x => x.NpcName)
            .Take(top)
            .ToList();
        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            rows.Select(x => new[]
            {
                x.ClassName.ColourName(),
                x.NpcName.ColourName(),
                x.Rating.ToString("N2", actor).ColourValue(),
                x.Location.ColourName()
            }).ToList(),
            new[] { "Class", "NPC", "Rating", "Location" },
            actor,
            Telnet.Cyan));
    }

    private static void ArenaManagerRating(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                "Which character do you want to look up? Use #3arena manager rating [<arena>] <character>#0."
                    .SubstituteANSIColour());
            return;
        }

        string originalArguments = ss.SafeRemainingArgument;
        string firstToken = ss.PopSpeech();
        ICombatArena? explicitArena = GetArena(actor, firstToken);
        ICombatArena? arena = explicitArena is not null && !ss.IsFinished ? explicitArena : GetArena(actor, null);
        string characterText = explicitArena is not null && !ss.IsFinished ? ss.SafeRemainingArgument : originalArguments;
        if (arena is null)
        {
            actor.OutputHandler.Send(
                "Which arena do you want to inspect? You can omit this if you're in an arena room."
                    .ColourCommand());
            return;
        }

        if (!arena.IsManager(actor))
        {
            actor.OutputHandler.Send("You are not a manager of that arena.".ColourError());
            return;
        }

        ICharacter? target = ResolveCharacter(actor, characterText);
        if (target is null)
        {
            actor.OutputHandler.Send("There is no character matching that description.".ColourError());
            return;
        }

        Dictionary<long, ArenaRatingSummary> summaries = actor.Gameworld.ArenaRatingsService.GetCharacterRatings(arena, target)
            .ToDictionary(x => x.CombatantClassId, x => x);
        List<string[]> rows = arena.CombatantClasses
            .OrderBy(x => x.Name)
            .Select(combatantClass =>
            {
                if (summaries.TryGetValue(combatantClass.Id, out ArenaRatingSummary? summary))
                {
                    return new[]
                    {
                        combatantClass.Name.ColourName(),
                        summary.Rating.ToString("N2", actor).ColourValue(),
                        (summary.LastUpdatedAt?.ToString("g", actor) ?? "Never").ColourValue()
                    };
                }

                return new[]
                {
                    combatantClass.Name.ColourName(),
                    actor.Gameworld.ArenaRatingsService.GetRating(target, combatantClass).ToString("N2", actor)
                        .ColourValue(),
                    "Never".ColourValue()
                };
            })
            .ToList();

        if (!rows.Any())
        {
            actor.OutputHandler.Send("That arena has no combatant classes to report ratings for.".ColourError());
            return;
        }

        actor.OutputHandler.Send(
            $"Ratings for {target.HowSeen(actor, flags: PerceiveIgnoreFlags.TrueDescription).ColourName()} in {arena.Name.ColourName()}:\n" +
            StringUtilities.GetTextTable(rows, new[] { "Class", "Rating", "Updated" }, actor, Telnet.Green));
    }

    private static void ArenaManagerRatings(ICharacter actor, StringStack ss)
    {
        ICombatArena? arena = ResolveOptionalArenaArgument(actor, ss, "class", "search", "min", "max", "sort", "desc", "top");
        if (arena is null)
        {
            actor.OutputHandler.Send(
                "Which arena do you want to inspect? You can omit this if you're in an arena room."
                    .ColourCommand());
            return;
        }

        if (!arena.IsManager(actor))
        {
            actor.OutputHandler.Send("You are not a manager of that arena.".ColourError());
            return;
        }

        long? classId = null;
        string? search = null;
        decimal? min = null;
        decimal? max = null;
        string sort = "rating";
        bool desc = true;
        int top = 100;
        while (!ss.IsFinished)
        {
            switch (ss.PopForSwitch())
            {
                case "class":
                    if (ss.IsFinished)
                    {
                        actor.OutputHandler.Send("Which combatant class do you want to filter by?");
                        return;
                    }

                    ICombatantClass? combatantClass = FindCombatantClass(arena.CombatantClasses, ss.PopSpeech());
                    if (combatantClass is null)
                    {
                        actor.OutputHandler.Send("There is no combatant class matching that filter.".ColourError());
                        return;
                    }

                    classId = combatantClass.Id;
                    break;
                case "search":
                    if (ss.IsFinished)
                    {
                        actor.OutputHandler.Send("What text do you want to search for?");
                        return;
                    }

                    search = ss.PopSpeech();
                    break;
                case "min":
                    if (ss.IsFinished || !decimal.TryParse(ss.PopSpeech(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal minRating))
                    {
                        actor.OutputHandler.Send("You must specify a numeric minimum rating.");
                        return;
                    }

                    min = minRating;
                    break;
                case "max":
                    if (ss.IsFinished || !decimal.TryParse(ss.PopSpeech(), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal maxRating))
                    {
                        actor.OutputHandler.Send("You must specify a numeric maximum rating.");
                        return;
                    }

                    max = maxRating;
                    break;
                case "sort":
                    if (ss.IsFinished)
                    {
                        actor.OutputHandler.Send("You must specify one of #3name#0, #3class#0, #3rating#0, or #3updated#0."
                            .SubstituteANSIColour());
                        return;
                    }

                    sort = ss.PopForSwitch();
                    if (!sort.EqualToAny("name", "class", "rating", "updated"))
                    {
                        actor.OutputHandler.Send("Sort must be one of #3name#0, #3class#0, #3rating#0, or #3updated#0."
                            .SubstituteANSIColour());
                        return;
                    }

                    break;
                case "desc":
                    desc = true;
                    break;
                case "top":
                    if (ss.IsFinished || !int.TryParse(ss.PopSpeech(), out top) || top <= 0)
                    {
                        actor.OutputHandler.Send("You must specify a positive whole number for the top count.");
                        return;
                    }

                    break;
                default:
                    actor.OutputHandler.Send(
                        "Valid options are #3class#0, #3search#0, #3min#0, #3max#0, #3sort#0, #3desc#0, and #3top#0."
                            .SubstituteANSIColour());
                    return;
            }
        }

        IEnumerable<ArenaRatingSummary> results = actor.Gameworld.ArenaRatingsService.GetArenaRatings(arena).AsEnumerable();
        if (classId.HasValue)
        {
            results = results.Where(x => x.CombatantClassId == classId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            results = results.Where(x =>
                x.CharacterName.Contains(search, StringComparison.InvariantCultureIgnoreCase) ||
                x.CombatantClassName.Contains(search, StringComparison.InvariantCultureIgnoreCase));
        }

        if (min.HasValue)
        {
            results = results.Where(x => x.Rating >= min.Value);
        }

        if (max.HasValue)
        {
            results = results.Where(x => x.Rating <= max.Value);
        }

        results = sort switch
        {
            "name" => desc
                ? results.OrderByDescending(x => x.CharacterName).ThenBy(x => x.CombatantClassName)
                : results.OrderBy(x => x.CharacterName).ThenBy(x => x.CombatantClassName),
            "class" => desc
                ? results.OrderByDescending(x => x.CombatantClassName).ThenBy(x => x.CharacterName)
                : results.OrderBy(x => x.CombatantClassName).ThenBy(x => x.CharacterName),
            "updated" => desc
                ? results.OrderByDescending(x => x.LastUpdatedAt).ThenBy(x => x.CharacterName)
                : results.OrderBy(x => x.LastUpdatedAt).ThenBy(x => x.CharacterName),
            _ => desc
                ? results.OrderByDescending(x => x.Rating).ThenBy(x => x.CharacterName)
                : results.OrderBy(x => x.Rating).ThenBy(x => x.CharacterName)
        };

        List<ArenaRatingSummary> list = results.Take(top).ToList();
        if (!list.Any())
        {
            actor.OutputHandler.Send("No ratings matched your filters.".ColourError());
            return;
        }

        List<string[]> rows = list.Select(x => new[]
        {
            x.CharacterName.ColourName(),
            x.CombatantClassName.ColourName(),
            x.Rating.ToString("N2", actor).ColourValue(),
            (x.LastUpdatedAt?.ToString("g", actor) ?? "Never").ColourValue()
        }).ToList();
        actor.OutputHandler.Send(StringUtilities.GetTextTable(rows, new[] { "Character", "Class", "Rating", "Updated" },
            actor, Telnet.Yellow));
    }

    private static void ArenaObserve(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to #3list#0, #3begin#0, or #3stop#0 observation?".SubstituteANSIColour());
            return;
        }

        string action = ss.PopForSwitch();
        switch (action)
        {
            case "list":
                ArenaObserveList(actor);
                return;
            case "enter":
            case "begin":
            case "start":
                ArenaObserveEnter(actor, ss);
                return;
            case "leave":
            case "stop":
            case "cease":
                ArenaObserveLeave(actor, ss);
                return;
            default:
                actor.OutputHandler.Send("Valid options are #3list#0, #3begin#0, or #3stop#0.".SubstituteANSIColour());
                return;
        }
    }

    private static void ArenaObserveList(ICharacter actor)
    {
        if (actor.Location is not ICell cell)
        {
            actor.OutputHandler.Send("You must be in a room to observe arena events.".ColourError());
            return;
        }

        List<ICombatArena> arenas = actor.Gameworld.CombatArenas.Where(x => x.ObservationCells.Contains(cell)).ToList();
        if (!arenas.Any())
        {
            actor.OutputHandler.Send("This location is not an arena observation room.".ColourError());
            return;
        }

        List<IArenaEvent> events = arenas.SelectMany(x => x.ActiveEvents)
                .Where(x => x.State >= ArenaEventState.RegistrationOpen && x.State < ArenaEventState.Completed)
                .Distinct()
                .ToList();

        if (!events.Any())
        {
            actor.OutputHandler.Send("There are no events available to observe from here.".ColourError());
            return;
        }

        string[] header = new[] { "Id", "Arena", "Name", "State" };
        List<string[]> rows = events.Select(evt => new[]
        {
                        evt.Id.ToString("N0", actor),
                        evt.Arena.Name.ColourName(),
                        evt.Name.ColourName(),
                        evt.State.DescribeEnum().ColourValue()
                }).ToList();

        actor.OutputHandler.Send(StringUtilities.GetTextTable(rows, header, actor, Telnet.Green));
    }

    private static void ArenaObserveEnter(ICharacter actor, StringStack ss)
    {
        if (actor.Location is not ICell cell)
        {
            actor.OutputHandler.Send("You must be in a room to observe an event.".ColourError());
            return;
        }

        string? eventText = ss.IsFinished ? null : ss.PopSpeech();
        IArenaEvent? arenaEvent = GetArenaEvent(actor, eventText);
        if (arenaEvent is null)
        {
            if (string.IsNullOrWhiteSpace(eventText))
            {
                actor.OutputHandler.Send("Which event do you want to observe? You can omit this if only one event is active.".ColourCommand());
                return;
            }

            actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
            return;
        }

        (bool canObserve, string? reason) = actor.Gameworld.ArenaObservationService.CanObserve(actor, arenaEvent);
        if (!canObserve)
        {
            actor.OutputHandler.Send(reason.ColourError());
            return;
        }

        actor.Gameworld.ArenaObservationService.StartObserving(actor, arenaEvent, cell);
        actor.OutputHandler.Handle(new EmoteOutput(new Emote($"@ begin|begins observing the $1 event.", actor, actor, new DummyPerceivable(arenaEvent.Name.ColourName()))));
    }

    private static void ArenaObserveLeave(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            foreach (ICombatArena? arena in actor.Gameworld.CombatArenas)
            {
                foreach (IArenaEvent activeEvent in arena.ActiveEvents)
                {
                    actor.Gameworld.ArenaObservationService.StopObserving(actor, activeEvent);
                }
            }

            actor.OutputHandler.Send("You stop observing all arena events.".Colour(Telnet.Green));
            return;
        }

        IArenaEvent? arenaEvent = GetArenaEvent(actor, ss.PopSpeech());
        if (arenaEvent is null)
        {
            actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
            return;
        }

        actor.Gameworld.ArenaObservationService.StopObserving(actor, arenaEvent);
        actor.OutputHandler.Send($"You stop observing {arenaEvent.Name.ColourName()}.".Colour(Telnet.Green));
    }

    private static void ArenaSignup(ICharacter actor, StringStack ss)
    {
        IArenaEvent? arenaEvent = null;
        int? requestedSideIndex = null;
        string? requestedClass = null;
        bool invalidEventReference = false;

        if (ss.IsFinished)
        {
            arenaEvent = GetArenaEvent(actor, null);
        }
        else
        {
            string originalArguments = ss.SafeRemainingArgument;
            int argumentCount = ss.CountRemainingArguments();
            string firstArg = ss.PopSpeech();

            if (ArenaSideIndexUtilities.TryParseDisplayIndex(firstArg, out int parsedSideIndex))
            {
                requestedSideIndex = parsedSideIndex;
                if (!ss.IsFinished)
                {
                    requestedClass = ss.PopSpeech();
                    if (ss.IsFinished)
                    {
                        IArenaEvent? explicitEvent = GetArenaEvent(actor, requestedClass);
                        if (explicitEvent is not null)
                        {
                            arenaEvent = explicitEvent;
                            requestedClass = null;
                        }
                    }
                    else
                    {
                        string eventText = ss.SafeRemainingArgument;
                        arenaEvent = GetArenaEvent(actor, eventText);
                        if (arenaEvent is null && argumentCount >= 3)
                        {
                            StringStack fallback = new(originalArguments);
                            IArenaEvent? fallbackEvent = GetArenaEvent(actor, fallback.PopSpeech());
                            if (fallbackEvent is not null && !fallback.IsFinished &&
                                ArenaSideIndexUtilities.TryParseDisplayIndex(fallback.PopSpeech(),
                                    out int fallbackSideIndex))
                            {
                                arenaEvent = fallbackEvent;
                                requestedSideIndex = fallbackSideIndex;
                                requestedClass = fallback.IsFinished ? null : fallback.PopSpeech();
                            }
                        }

                        if (arenaEvent is null && !string.IsNullOrWhiteSpace(eventText))
                        {
                            invalidEventReference = true;
                        }
                    }
                }
            }
            else
            {
                arenaEvent = GetArenaEvent(actor, firstArg);
                if (arenaEvent is null)
                {
                    requestedClass = firstArg;
                    string? eventText = ss.IsFinished ? null : ss.SafeRemainingArgument;
                    arenaEvent = GetArenaEvent(actor, eventText);
                    if (arenaEvent is null && !string.IsNullOrWhiteSpace(eventText))
                    {
                        invalidEventReference = true;
                    }
                }
                else if (!ss.IsFinished)
                {
                    string secondArg = ss.PopSpeech();
                    if (ArenaSideIndexUtilities.TryParseDisplayIndex(secondArg, out parsedSideIndex))
                    {
                        requestedSideIndex = parsedSideIndex;
                        requestedClass = ss.IsFinished ? null : ss.PopSpeech();
                    }
                    else
                    {
                        requestedClass = secondArg;
                    }
                }
            }
        }

        if (arenaEvent is null && !invalidEventReference)
        {
            arenaEvent = GetArenaEvent(actor, null);
        }
        if (arenaEvent is null)
        {
            if (invalidEventReference)
            {
                actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
                return;
            }

            actor.OutputHandler.Send(
                "Which event do you want to sign up for? You can omit this if only one event is active.".ColourCommand());
            return;
        }

        (int? sideIndex, ICombatantClass? combatantClass, string? error) = ResolveSignupSelection(actor, arenaEvent, requestedSideIndex, requestedClass);
        if (!sideIndex.HasValue || combatantClass is null)
        {
            actor.OutputHandler.Send(error.ColourError());
            return;
        }

        try
        {
            arenaEvent.SignUp(actor, sideIndex.Value, combatantClass);
            actor.OutputHandler.Send(
                $"You sign up for {arenaEvent.Name.ColourName()} on side {ArenaSideIndexUtilities.ToDisplayString(actor, sideIndex.Value).ColourValue()} as {combatantClass.Name.ColourName()}".Colour(Telnet.Green));
        }
        catch (Exception ex)
        {
            actor.OutputHandler.Send(ex.Message.ColourError());
        }
    }

    private static void ArenaWithdraw(ICharacter actor, StringStack ss)
    {
        string? eventText = ss.IsFinished ? null : ss.PopSpeech();
        IArenaEvent? arenaEvent = GetArenaEvent(actor, eventText);
        if (arenaEvent is null)
        {
            if (string.IsNullOrWhiteSpace(eventText))
            {
                actor.OutputHandler.Send("Which event do you want to withdraw from? You can omit this if only one event is active.".ColourCommand());
                return;
            }

            actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
            return;
        }

        try
        {
            arenaEvent.Withdraw(actor);
            actor.OutputHandler.Send($"You withdraw from {arenaEvent.Name.ColourName()}.".Colour(Telnet.Green));
        }
        catch (Exception ex)
        {
            actor.OutputHandler.Send(ex.Message.ColourError());
        }
    }

    private static void ArenaSurrender(ICharacter actor, StringStack ss)
    {
        IArenaEvent? arenaEvent;
        if (ss.IsFinished)
        {
            List<IArenaEvent> matchingEvents = actor.Gameworld.CombatArenas
                .SelectMany(x => x.ActiveEvents)
                .Where(x => x.State == ArenaEventState.Live)
                .Where(x => x.Participants.Any(p => p.Character?.Id == actor.Id))
                .ToList();

            switch (matchingEvents.Count)
            {
                case 0:
                    actor.OutputHandler.Send("You are not currently participating in any live arena event.".ColourError());
                    return;
                case > 1:
                    actor.OutputHandler.Send("You are participating in multiple live arena events. Which event do you want to surrender from?"
                        .ColourCommand());
                    return;
                default:
                    arenaEvent = matchingEvents[0];
                    break;
            }
        }
        else
        {
            arenaEvent = GetArenaEvent(actor, ss.PopSpeech());
            if (arenaEvent is null)
            {
                actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
                return;
            }
        }

        (bool truth, string? reason) = arenaEvent.CanSurrender(actor);
        if (!truth)
        {
            actor.OutputHandler.Send(reason.ColourError());
            return;
        }

        arenaEvent.Surrender(actor);
        actor.OutputHandler.Send($"You surrender in {arenaEvent.Name.ColourName()}.".Colour(Telnet.Green));
    }

    private static void ArenaBet(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to #3odds#0, #3place#0, #3cancel#0, #3pools#0, #3list#0, #3history#0, or #3collect#0?".SubstituteANSIColour());
            return;
        }

        switch (ss.PopForSwitch())
        {
            case "odds":
                ArenaBetOdds(actor, ss);
                return;
            case "place":
                ArenaBetPlace(actor, ss);
                return;
            case "cancel":
                ArenaBetCancel(actor, ss);
                return;
            case "pools":
                ArenaBetPools(actor, ss);
                return;
            case "list":
                ArenaBetList(actor, ss);
                return;
            case "history":
                ArenaBetHistory(actor, ss);
                return;
            case "collect":
                ArenaBetCollect(actor, ss);
                return;
            default:
                actor.OutputHandler.Send("Valid options are #3odds#0, #3place#0, #3cancel#0, #3pools#0, #3list#0, #3history#0, or #3collect#0.".SubstituteANSIColour());
                return;
        }
    }

    private static void ArenaBetOdds(ICharacter actor, StringStack ss)
    {
        string? sideText = null;
        IArenaEvent? arenaEvent = null;
        if (ss.IsFinished)
        {
            arenaEvent = GetArenaEvent(actor, null);
            if (arenaEvent is null)
            {
                actor.OutputHandler.Send("Which event do you want to get odds for? You can omit this if only one event is active.".ColourCommand());
                return;
            }
        }
        else
        {
            string firstArg = ss.PopSpeech();
            if (TryParseSideSpecifier(firstArg, out _))
            {
                sideText = firstArg;
                string? eventText = ss.IsFinished ? null : ss.SafeRemainingArgument;
                arenaEvent = GetArenaEvent(actor, eventText);
                if (arenaEvent is null)
                {
                    if (string.IsNullOrWhiteSpace(eventText) || TryParseSideSpecifier(eventText, out _))
                    {
                        IArenaEvent? explicitEvent = GetArenaEvent(actor, firstArg);
                        if (explicitEvent is not null)
                        {
                            arenaEvent = explicitEvent;
                            sideText = string.IsNullOrWhiteSpace(eventText) ? null : eventText;
                        }
                    }

                    if (arenaEvent is null)
                    {
                        if (string.IsNullOrWhiteSpace(eventText))
                        {
                            actor.OutputHandler.Send("Which event do you want to get odds for? You can omit this if only one event is active.".ColourCommand());
                            return;
                        }

                        actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
                        return;
                    }
                }
            }
            else
            {
                arenaEvent = GetArenaEvent(actor, firstArg);
                if (arenaEvent is null)
                {
                    actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
                    return;
                }

                sideText = ss.IsFinished ? null : ss.SafeRemainingArgument;
            }
        }

        (int? Value, bool Invalid, string Error) sideIndex = ParseSideIndex(arenaEvent, sideText, actor);
        if (sideIndex.Invalid)
        {
            actor.OutputHandler.Send(sideIndex.Error.ColourError());
            return;
        }

        (decimal? FixedOdds, (decimal Pool, decimal TakeRate)? PariMutuel) quote = actor.Gameworld.ArenaBettingService.GetQuote(arenaEvent, sideIndex.Value);
        StringBuilder sb = new();
        sb.AppendLine($"Betting for {arenaEvent.Name.ColourName()} ({arenaEvent.EventType.Name.ColourName()}):");
        if (quote.FixedOdds is { } odds)
        {
            sb.AppendLine($"Fixed Odds: {odds:0.00} to 1");
        }

        if (quote.PariMutuel is { } pool)
        {
            sb.AppendLine($"Pool: {DescribeCurrency(arenaEvent.Arena, pool.Pool)}, Take Rate: {pool.TakeRate:P0}");
        }

        if (quote.FixedOdds is null && quote.PariMutuel is null)
        {
            sb.AppendLine("No odds are currently available.".ColourError());
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ArenaBetPlace(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which side (or draw) do you want to bet on?".ColourCommand());
            return;
        }

        string originalArguments = ss.SafeRemainingArgument;
        int argumentCount = ss.CountRemainingArguments();
        string firstArg = ss.PopSpeech();
        IArenaEvent? arenaEvent = null;
        string sideText = string.Empty;
        string amountText = string.Empty;

        if (TryParseSideSpecifier(firstArg, out _))
        {
            sideText = firstArg;
            if (ss.IsFinished)
            {
                actor.OutputHandler.Send("How much do you want to stake?".ColourCommand());
                return;
            }

            amountText = ss.PopSpeech();
            string? eventText = ss.IsFinished ? null : ss.SafeRemainingArgument;
            arenaEvent = GetArenaEvent(actor, eventText);
            if (arenaEvent is null && !string.IsNullOrWhiteSpace(eventText) && argumentCount >= 3)
            {
                StringStack fallback = new(originalArguments);
                arenaEvent = GetArenaEvent(actor, fallback.PopSpeech());
                if (arenaEvent is null)
                {
                    actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
                    return;
                }

                if (fallback.IsFinished)
                {
                    actor.OutputHandler.Send("Which side (or draw) do you want to bet on?".ColourCommand());
                    return;
                }

                sideText = fallback.PopSpeech();
                if (fallback.IsFinished)
                {
                    actor.OutputHandler.Send("How much do you want to stake?".ColourCommand());
                    return;
                }

                amountText = fallback.PopSpeech();
            }
            else if (arenaEvent is null)
            {
                if (string.IsNullOrWhiteSpace(eventText))
                {
                    actor.OutputHandler.Send("Which event do you want to bet on? You can omit this if only one event is active.".ColourCommand());
                    return;
                }

                actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
                return;
            }
        }
        else
        {
            arenaEvent = GetArenaEvent(actor, firstArg);
            if (arenaEvent is null)
            {
                actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
                return;
            }

            if (ss.IsFinished)
            {
                actor.OutputHandler.Send("Which side (or draw) do you want to bet on?".ColourCommand());
                return;
            }

            sideText = ss.PopSpeech();
            if (ss.IsFinished)
            {
                actor.OutputHandler.Send("How much do you want to stake?".ColourCommand());
                return;
            }

            amountText = ss.PopSpeech();
        }

        if (arenaEvent is null)
        {
            actor.OutputHandler.Send("Which event do you want to bet on? You can omit this if only one event is active.".ColourCommand());
            return;
        }

        (int? Value, bool Invalid, string Error) sideIndex = ParseSideIndex(arenaEvent, sideText, actor);
        if (sideIndex.Invalid)
        {
            actor.OutputHandler.Send(sideIndex.Error.ColourError());
            return;
        }

        if (!arenaEvent.Arena.Currency.TryGetBaseCurrency(amountText, out decimal amount) || amount <= 0)
        {
            actor.OutputHandler.Send("That is not a valid stake amount.".ColourError());
            return;
        }

        try
        {
            actor.Gameworld.ArenaBettingService.PlaceBet(actor, arenaEvent, sideIndex.Value, amount);
            actor.OutputHandler.Send(
                $"You stake {DescribeCurrency(arenaEvent.Arena, amount)} on {DescribeSide(arenaEvent, sideIndex.Value, actor)}.".Colour(Telnet.Green));
        }
        catch (Exception ex)
        {
            actor.OutputHandler.Send(ex.Message.ColourError());
        }
    }

    private static void ArenaBetCancel(ICharacter actor, StringStack ss)
    {
        string? eventText = ss.IsFinished ? null : ss.PopSpeech();
        IArenaEvent? arenaEvent = GetArenaEvent(actor, eventText);
        if (arenaEvent is null)
        {
            if (string.IsNullOrWhiteSpace(eventText))
            {
                actor.OutputHandler.Send("Which event do you want to cancel your bet for? You can omit this if only one event is active.".ColourCommand());
                return;
            }

            actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
            return;
        }

        try
        {
            actor.Gameworld.ArenaBettingService.CancelBet(actor, arenaEvent);
            actor.OutputHandler.Send(
                    $"You cancel your wager on {arenaEvent.Name.ColourName()} and receive your stake back.".Colour(Telnet.Green));
        }
        catch (Exception ex)
        {
            actor.OutputHandler.Send(ex.Message.ColourError());
        }
    }

    private static void ArenaBetPools(ICharacter actor, StringStack ss)
    {
        string? eventText = ss.IsFinished ? null : ss.PopSpeech();
        IArenaEvent? arenaEvent = GetArenaEvent(actor, eventText);
        if (arenaEvent is null)
        {
            if (string.IsNullOrWhiteSpace(eventText))
            {
                actor.OutputHandler.Send("Which event's pools do you want to view? You can omit this if only one event is active.".ColourCommand());
                return;
            }

            actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
            return;
        }

        if (arenaEvent.EventType.BettingModel != BettingModel.PariMutuel)
        {
            actor.OutputHandler.Send("That event does not use pari-mutuel pools.".ColourError());
            return;
        }

        using (new FMDB())
        {
            FuturemudDatabaseContext context = FMDB.Context;
            List<Models.ArenaBetPool> pools = context.ArenaBetPools.Where(x => x.ArenaEventId == arenaEvent.Id).ToList();
            if (!pools.Any())
            {
                actor.OutputHandler.Send("There are no active pools for that event.".ColourError());
                return;
            }

            string[] header = new[] { "Side", "Pool" };
            List<string[]> rows = pools.Select(pool => new[]
            {
                pool.SideIndex.HasValue ? ArenaSideIndexUtilities.ToDisplayString(actor, pool.SideIndex.Value) : "Draw",
                                DescribeCurrency(arenaEvent.Arena, pool.TotalStake)
                        }).ToList();

            actor.OutputHandler.Send(StringUtilities.GetTextTable(rows, header, actor, Telnet.Green));
        }
    }

    private static void ArenaBetList(ICharacter actor, StringStack ss)
    {
        if (!ss.IsFinished)
        {
            actor.OutputHandler.Send("The #3arena bet list#0 command does not take any additional arguments.".SubstituteANSIColour());
            return;
        }

        List<ArenaBetSummary> bets = actor.Gameworld.ArenaBettingService.GetActiveBets(actor).ToList();
        List<ArenaBetPayoutSummary> payouts = actor.Gameworld.ArenaBettingService.GetOutstandingPayouts(actor).ToList();

        if (!bets.Any() && !payouts.Any())
        {
            actor.OutputHandler.Send("You have no active wagers or outstanding payouts.".ColourError());
            return;
        }

        StringBuilder sb = new();
        if (bets.Any())
        {
            sb.AppendLine("Active Wagers:");
            string[] header = new[] { "Event", "Arena", "Side", "Stake", "State", "Placed" };
            List<string[]> rows = bets.Select(bet => new[]
            {
                                DescribeEvent(bet.EventTypeName, bet.ArenaEventId, actor),
                                bet.ArenaName.ColourName(),
                                DescribeBetSide(bet.SideIndex, actor),
                                DescribeCurrency(actor, bet.ArenaId, bet.Stake),
                                bet.EventState.DescribeEnum().ColourValue(),
                                bet.PlacedAt.ToString("g", actor).ColourValue()
                        }).ToList();

            sb.AppendLine(StringUtilities.GetTextTable(rows, header, actor, Telnet.Yellow));
        }

        if (payouts.Any())
        {
            if (sb.Length > 0)
            {
                sb.AppendLine();
            }

            sb.AppendLine("Outstanding Payouts:");
            string[] header = new[] { "Event", "Arena", "Type", "Amount", "Status", "Recorded" };
            List<string[]> rows = payouts.Select(payout => new[]
            {
                        DescribeEvent(payout.EventTypeName, payout.ArenaEventId, actor),
                        payout.ArenaName.ColourName(),
                        payout.PayoutType.DescribeEnum().ColourValue(),
                        DescribeCurrency(actor, payout.ArenaId, payout.Amount),
                        DescribePayoutStatus(payout),
                        payout.CreatedAt.ToString("g", actor).ColourValue()
                        }).ToList();

            sb.AppendLine(StringUtilities.GetTextTable(rows, header, actor, Telnet.Green));
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ArenaBetHistory(ICharacter actor, StringStack ss)
    {
        int count = 20;
        if (!ss.IsFinished)
        {
            if (!int.TryParse(ss.PopSpeech(), NumberStyles.Integer, CultureInfo.InvariantCulture, out count) || count <= 0)
            {
                actor.OutputHandler.Send("You must specify a positive number of wagers to show.".ColourError());
                return;
            }
        }

        List<ArenaBetSummary> bets = actor.Gameworld.ArenaBettingService.GetBetHistory(actor, count).ToList();
        if (!bets.Any())
        {
            actor.OutputHandler.Send("You have no betting history to display.".ColourError());
            return;
        }

        string[] header = new[] { "Event", "Arena", "Side", "Stake", "Result", "Placed" };
        List<string[]> rows = bets.Select(bet => new[]
        {
                        DescribeEvent(bet.EventTypeName, bet.ArenaEventId, actor),
                        bet.ArenaName.ColourName(),
                        DescribeBetSide(bet.SideIndex, actor),
                        DescribeCurrency(actor, bet.ArenaId, bet.Stake),
                        DescribeBetResult(actor, bet),
                        bet.PlacedAt.ToString("g", actor).ColourValue()
                }).ToList();

        actor.OutputHandler.Send(StringUtilities.GetTextTable(rows, header, actor, Telnet.Cyan));
    }

    private static void ArenaBetCollect(ICharacter actor, StringStack ss)
    {
        long? eventId = null;
        if (!ss.IsFinished)
        {
            string eventText = ss.PopSpeech();
            if (!long.TryParse(eventText, NumberStyles.Integer, CultureInfo.InvariantCulture, out long parsed))
            {
                IArenaEvent? arenaEvent = GetArenaEvent(actor, eventText);
                if (arenaEvent is null)
                {
                    actor.OutputHandler.Send("That is not a valid event identifier.".ColourError());
                    return;
                }

                parsed = arenaEvent.Id;
            }

            eventId = parsed;
        }

        ICombatArena? arena = null;
        if (eventId.HasValue)
        {
            ArenaBetPayoutSummary? payout = actor.Gameworld.ArenaBettingService.GetOutstandingPayouts(actor)
                    .FirstOrDefault(x => x.ArenaEventId == eventId.Value);
            arena = payout is null ? null : actor.Gameworld.CombatArenas.Get(payout.ArenaId);
        }

        ArenaBetCollectionSummary result = actor.Gameworld.ArenaBettingService.CollectOutstandingPayouts(actor, eventId);
        if (result.CollectedCount == 0 && result.FailedCount == 0 && result.BlockedCount == 0)
        {
            string message = eventId.HasValue
                    ? $"You have no outstanding payouts for event #{eventId.Value.ToString("N0", actor)}."
                    : "You have no outstanding payouts to collect.";
            actor.OutputHandler.Send(message.ColourError());
            return;
        }

        StringBuilder sb = new();
        if (result.CollectedCount > 0)
        {
            string collectedText = eventId.HasValue && arena is not null
                    ? $"You collect {DescribeCurrency(arena, result.CollectedAmount)} from {result.CollectedCount.ToString("N0", actor).ColourValue()} payout(s)."
                    : $"You collect {result.CollectedCount.ToString("N0", actor).ColourValue()} payout(s).";
            sb.AppendLine(collectedText.Colour(Telnet.Green));
        }

        if (result.FailedCount > 0)
        {
            string failedText = eventId.HasValue && arena is not null
                    ? $"{result.FailedCount.ToString("N0", actor).ColourValue()} payout(s) totaling {DescribeCurrency(arena, result.FailedAmount)} could not be disbursed."
                    : $"{result.FailedCount.ToString("N0", actor).ColourValue()} payout(s) could not be disbursed.";
            sb.AppendLine(failedText.ColourError());
        }

        if (result.BlockedCount > 0)
        {
            string blockedText = eventId.HasValue && arena is not null
                    ? $"{result.BlockedCount.ToString("N0", actor).ColourValue()} payout(s) totaling {DescribeCurrency(arena, result.BlockedAmount)} are still blocked pending arena funds."
                    : $"{result.BlockedCount.ToString("N0", actor).ColourValue()} payout(s) are still blocked pending arena funds.";
            sb.AppendLine(blockedText.Colour(Telnet.Yellow));
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ArenaRatings(ICharacter actor, StringStack ss)
    {
        if (!ss.IsFinished && ss.PeekSpeech().EqualTo("show"))
        {
            ss.PopSpeech();
        }

        string? filter = ss.IsFinished ? null : ss.PopSpeech();
        List<ICombatantClass> classes = actor.Gameworld.CombatArenas
                .SelectMany(x => x.EventTypes)
                .SelectMany(x => x.Sides)
                .SelectMany(x => x.EligibleClasses)
                .Distinct()
                .ToList();

        if (!classes.Any())
        {
            actor.OutputHandler.Send("There are no combatant classes defined for arenas.".ColourError());
            return;
        }

        if (!string.IsNullOrEmpty(filter))
        {
            classes = classes.Where(x => x.Name.Equals(filter, StringComparison.InvariantCultureIgnoreCase) ||
                                          x.Name.StartsWith(filter, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (!classes.Any())
            {
                actor.OutputHandler.Send("There is no combatant class matching that filter.".ColourError());
                return;
            }
        }

        string[] header = new[] { "Class", "Rating" };
        List<string[]> rows = classes.Select(cls => new[]
        {
                        cls.Name.ColourName(),
                        actor.Gameworld.ArenaRatingsService.GetRating(actor, cls).ToString("N2", actor).ColourValue()
                }).ToList();

        actor.OutputHandler.Send(StringUtilities.GetTextTable(rows, header, actor, Telnet.Cyan));
    }

    private static ICombatArena? GetArena(ICharacter actor, string? text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            return actor.Gameworld.CombatArenas.GetByIdOrName(text);
        }

        return GetArenaFromLocation(actor);
    }

    private static ICombatArena? GetArenaFromLocation(ICharacter actor)
    {
        if (actor.Location is not ICell cell)
        {
            return null;
        }

        return actor.Gameworld.CombatArenas
            .FirstOrDefault(arena =>
                arena.WaitingCells.Contains(cell) ||
                arena.ArenaCells.Contains(cell) ||
                arena.ObservationCells.Contains(cell) ||
                arena.InfirmaryCells.Contains(cell) ||
                    arena.AfterFightCells.Contains(cell) ||
                    arena.NpcStablesCells.Contains(cell));
    }

    private static ICombatArena? ResolveOptionalArenaArgument(ICharacter actor, StringStack ss, params string[] optionKeywords)
    {
        if (ss.IsFinished)
        {
            return GetArena(actor, null);
        }

        string firstToken = ss.PeekSpeech();
        if (optionKeywords.Any(x => firstToken.EqualTo(x)))
        {
            return GetArena(actor, null);
        }

        ICombatArena? explicitArena = GetArena(actor, firstToken);
        if (explicitArena is null)
        {
            return GetArena(actor, null);
        }

        ss.PopSpeech();
        return explicitArena;
    }

    private static ICharacter? ResolveCharacter(ICharacter actor, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        ICharacter? localMatch = actor.TargetActor(text);
        if (localMatch is not null)
        {
            return localMatch;
        }

        if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out long id))
        {
            return actor.Gameworld.TryGetCharacter(id, true);
        }

        return actor.Gameworld.Characters.GetByIdOrName(text);
    }

    private static bool IsEligibleForCombatantClass(ICharacter character, ICombatantClass combatantClass)
    {
        try
        {
            return combatantClass.EligibilityProg.Execute<bool?>(character) != false;
        }
        catch
        {
            return false;
        }
    }

    private static IArenaEvent? GetArenaEvent(ICharacter actor, string? text)
    {
        ICombatArena? localArena = GetArenaFromLocation(actor);
        List<IArenaEvent> allEvents = actor.Gameworld.CombatArenas.SelectMany(x => x.ActiveEvents).ToList();
        List<IArenaEvent> localEvents = localArena is null ? allEvents : localArena.ActiveEvents.ToList();

        IArenaEvent? ResolveImplicitEvent(IEnumerable<IArenaEvent> events)
        {
            List<IArenaEvent> eventList = events.ToList();
            List<IArenaEvent> currentEvents = eventList.Where(IsCurrentArenaEvent).ToList();
            if (currentEvents.Count == 1)
            {
                return currentEvents[0];
            }

            return eventList.Count == 1 ? eventList[0] : null;
        }

        if (string.IsNullOrWhiteSpace(text))
        {
            IArenaEvent? implicitMatch = ResolveImplicitEvent(localEvents);
            if (implicitMatch is not null)
            {
                return implicitMatch;
            }

            if (localArena is not null)
            {
                return ResolveImplicitEvent(allEvents);
            }

            return null;
        }

        bool isIdSearch = long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out long id);

        IArenaEvent? FindEvent(IEnumerable<IArenaEvent> events)
        {
            if (isIdSearch)
            {
                return events.FirstOrDefault(x => x.Id == id);
            }

            return events.FirstOrDefault(x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase)) ??
                   events.FirstOrDefault(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
        }

        IArenaEvent? localMatch = FindEvent(localEvents);
        if (localMatch != null || localArena is null)
        {
            return localMatch;
        }

        return FindEvent(allEvents);
    }

    private static IArenaEventType? GetArenaEventType(ICharacter actor, string? text)
    {
        ICombatArena? localArena = GetArenaFromLocation(actor);
        List<IArenaEventType> allTypes = actor.Gameworld.CombatArenas.SelectMany(x => x.EventTypes).ToList();
        List<IArenaEventType> localTypes = localArena is null ? allTypes : localArena.EventTypes.ToList();
        if (string.IsNullOrWhiteSpace(text))
        {
            if (localTypes.Count == 1)
            {
                return localTypes[0];
            }

            return localArena is not null && allTypes.Count == 1 ? allTypes[0] : null;
        }

        bool isIdSearch = long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out long id);

        IArenaEventType? FindType(IEnumerable<IArenaEventType> types)
        {
            if (isIdSearch)
            {
                return types.FirstOrDefault(x => x.Id == id);
            }

            return types.FirstOrDefault(x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase)) ??
                   types.FirstOrDefault(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
        }

        IArenaEventType? localMatch = FindType(localTypes);
        if (localMatch != null || localArena is null)
        {
            return localMatch;
        }

        return FindType(allTypes);
    }

    private static bool TryParseArenaState(string text, out ArenaEventState state)
    {
        state = ArenaEventState.Draft;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        if (text.TryParseEnum<ArenaEventState>(out state))
        {
            return true;
        }

        switch (text.CollapseString())
        {
            case "registration":
            case "reg":
            case "open":
                state = ArenaEventState.RegistrationOpen;
                return true;
            case "prep":
            case "prepare":
            case "preparation":
                state = ArenaEventState.Preparing;
                return true;
            case "stage":
            case "staged":
                state = ArenaEventState.Staged;
                return true;
            case "live":
            case "fight":
            case "fighting":
                state = ArenaEventState.Live;
                return true;
            case "resolve":
            case "resolving":
                state = ArenaEventState.Resolving;
                return true;
            case "cleanup":
            case "clean":
                state = ArenaEventState.Cleanup;
                return true;
            case "complete":
            case "completed":
                state = ArenaEventState.Completed;
                return true;
            case "abort":
            case "aborted":
                state = ArenaEventState.Aborted;
                return true;
        }

        return false;
    }

    private static string DescribeAutoSchedule(IArenaEventType eventType, ICharacter actor)
    {
        if (!eventType.AutoScheduleEnabled || !eventType.AutoScheduleInterval.HasValue ||
            !eventType.AutoScheduleReferenceTime.HasValue)
        {
            return "disabled".ColourError();
        }

        return
            $"every {eventType.AutoScheduleInterval.Value.Describe(actor).ColourValue()} from {eventType.AutoScheduleReferenceTime.Value.ToString("f", actor).ColourValue()}";
    }

    private static bool IsCurrentArenaEvent(IArenaEvent arenaEvent)
    {
        return arenaEvent.State > ArenaEventState.Scheduled &&
               arenaEvent.State < ArenaEventState.Completed;
    }

    private static IEnumerable<(IArenaEventType EventType, DateTime ScheduledFor)> GetProjectedAutoScheduledEvents(
        ICombatArena arena,
        IEnumerable<IArenaEvent> existingEvents)
    {
        DateTime now = DateTime.UtcNow;
        List<IArenaEvent> concreteEvents = existingEvents.ToList();
        foreach (IArenaEventType eventType in arena.EventTypes)
        {
            if (!IsRecurringEnabled(eventType))
            {
                continue;
            }

            DateTime scheduledFor = ResolveNextRecurringTrigger(eventType, now);
            if (concreteEvents.Any(evt =>
                    ReferenceEquals(evt.EventType, eventType) &&
                    Math.Abs((evt.ScheduledAt - scheduledFor).TotalSeconds) < 1.0))
            {
                continue;
            }

            yield return (eventType, scheduledFor);
        }
    }

    private static bool IsRecurringEnabled(IArenaEventType eventType)
    {
        return eventType.AutoScheduleEnabled &&
               eventType.AutoScheduleInterval.HasValue &&
               eventType.AutoScheduleInterval.Value > TimeSpan.Zero &&
               eventType.AutoScheduleReferenceTime.HasValue;
    }

    private static DateTime ResolveNextRecurringTrigger(IArenaEventType eventType, DateTime now)
    {
        DateTime reference = eventType.AutoScheduleReferenceTime!.Value;
        TimeSpan interval = eventType.AutoScheduleInterval!.Value;
        if (reference >= now)
        {
            return reference;
        }

        long elapsedTicks = now.Ticks - reference.Ticks;
        long intervalTicks = interval.Ticks;
        if (intervalTicks <= 0)
        {
            return now;
        }

        long cycles = elapsedTicks / intervalTicks;
        DateTime next = reference.AddTicks(cycles * intervalTicks);
        if (next < now)
        {
            next = next.AddTicks(intervalTicks);
        }

        return next;
    }

    private static bool TryParseSideSpecifier(string? text, out int? sideIndex)
    {
        sideIndex = null;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        if (text.Equals("draw", StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        if (ArenaSideIndexUtilities.TryParseDisplayIndex(text, out int value))
        {
            sideIndex = value;
            return true;
        }

        return false;
    }

    private static (int? SideIndex, ICombatantClass? CombatantClass, string Error) ResolveSignupSelection(
        ICharacter actor,
        IArenaEvent arenaEvent,
        int? requestedSideIndex,
        string? requestedClass)
    {
        List<IArenaEventTypeSide> sides = arenaEvent.EventType.Sides
            .OrderBy(x => x.Index)
            .ToList();
        if (requestedSideIndex.HasValue)
        {
            sides = sides.Where(x => x.Index == requestedSideIndex.Value).ToList();
            if (!sides.Any())
            {
                return (null, null, "That side does not exist for the selected event.");
            }
        }

        string firstFailureReason = string.Empty;
        bool checkedAnyCombination = false;
        bool classMatchedAnySide = false;
        foreach (IArenaEventTypeSide side in sides)
        {
            IEnumerable<ICombatantClass> classes;
            if (!string.IsNullOrWhiteSpace(requestedClass))
            {
                ICombatantClass? explicitClass = FindCombatantClass(side.EligibleClasses, requestedClass);
                if (explicitClass is null)
                {
                    continue;
                }

                classMatchedAnySide = true;
                classes = [explicitClass];
            }
            else
            {
                classes = side.EligibleClasses;
            }

            foreach (ICombatantClass combatantClass in classes)
            {
                checkedAnyCombination = true;
                (bool allowed, string? reason) = arenaEvent.CanSignUp(actor, side.Index, combatantClass);
                if (allowed)
                {
                    return (side.Index, combatantClass, string.Empty);
                }

                if (string.IsNullOrEmpty(firstFailureReason))
                {
                    firstFailureReason = reason;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(requestedClass) && !classMatchedAnySide)
        {
            string reason = requestedSideIndex.HasValue
                ? "That combatant class is not eligible for this side."
                : "That combatant class is not eligible for any side in that event.";
            return (null, null, reason);
        }

        if (!checkedAnyCombination)
        {
            string reason = requestedSideIndex.HasValue
                ? "There are no eligible combatant classes for that side."
                : "There are no eligible side and class combinations for that event.";
            return (null, null, reason);
        }

        return (null, null, firstFailureReason.IfNullOrWhiteSpace("You cannot sign up for that event."));
    }

    private static (int? Value, bool Invalid, string Error) ParseSideIndex(IArenaEvent arenaEvent, string? text,
            ICharacter actor)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Equals("draw", StringComparison.InvariantCultureIgnoreCase))
        {
            return (null, false, string.Empty);
        }

        if (!ArenaSideIndexUtilities.TryParseDisplayIndex(text, out int value))
        {
            return (null, true, "You must specify a side number starting at 1 or the word draw.");
        }

        if (arenaEvent.EventType.Sides.All(x => x.Index != value))
        {
            return (null, true, "That side does not exist for this event.");
        }

        return (value, false, string.Empty);
    }

    private static ICombatantClass? FindCombatantClass(IEnumerable<ICombatantClass> classes, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        if (long.TryParse(text, out long id))
        {
            return classes.FirstOrDefault(x => x.Id == id);
        }

        return classes.FirstOrDefault(x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase)) ??
               classes.FirstOrDefault(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
    }

    private static string DescribeEvent(string eventTypeName, long eventId, ICharacter actor)
    {
        string idText = eventId.ToString("N0", actor).ColourValue();
        return $"{eventTypeName.ColourName()} #{idText}";
    }

    private static string DescribeBetSide(int? sideIndex, ICharacter actor)
    {
        return sideIndex.HasValue
    ? $"Side {ArenaSideIndexUtilities.ToDisplayString(actor, sideIndex.Value).ColourValue()}"
                : "Draw".ColourValue();
    }

    private static string DescribeBetResult(ICharacter actor, ArenaBetSummary bet)
    {
        if (bet.IsCancelled)
        {
            return "Cancelled".Colour(Telnet.Yellow);
        }

        if (bet.BlockedPayout > 0.0m)
        {
            string amountText = DescribeCurrency(actor, bet.ArenaId, bet.BlockedPayout);
            return $"{"Blocked".ColourError()} ({amountText})";
        }

        if (bet.OutstandingPayout > 0.0m)
        {
            string amountText = DescribeCurrency(actor, bet.ArenaId, bet.OutstandingPayout);
            return $"{"Owed".Colour(Telnet.Yellow)} ({amountText})";
        }

        if (bet.CollectedPayout > 0.0m)
        {
            string amountText = DescribeCurrency(actor, bet.ArenaId, bet.CollectedPayout);
            return $"{"Paid".Colour(Telnet.Green)} ({amountText})";
        }

        return bet.EventState >= ArenaEventState.Resolving
                ? "Lost".ColourError()
                : "Open".ColourValue();
    }

    private static string DescribePayoutStatus(ArenaBetPayoutSummary payout)
    {
        return payout.IsBlocked
                ? "Blocked".ColourError()
                : "Owed".Colour(Telnet.Yellow);
    }

    private static string DescribeCurrency(ICharacter actor, long arenaId, decimal amount)
    {
        ICombatArena? arena = actor.Gameworld.CombatArenas.Get(arenaId);
        return arena is null
                ? amount.ToString("N2", actor).ColourValue()
                : DescribeCurrency(arena, amount);
    }

    private static string DescribeCurrency(ICombatArena arena, decimal amount)
    {
        return arena.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
    }

    private static string DescribeSide(IArenaEvent arenaEvent, int? sideIndex, ICharacter actor)
    {
        if (!sideIndex.HasValue)
        {
            return "draw".ColourValue();
        }

        IArenaEventTypeSide? side = arenaEvent.EventType.Sides.FirstOrDefault(x => x.Index == sideIndex.Value);
        string displayIndex = ArenaSideIndexUtilities.ToDisplayString(actor, sideIndex.Value).ColourValue();
        return side is null
            ? $"Side {displayIndex}"
            : $"Side {displayIndex} ({side.Policy.DescribeEnum().ColourValue()})";
    }
}

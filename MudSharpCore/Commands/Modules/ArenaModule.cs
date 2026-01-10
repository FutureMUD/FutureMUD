#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Commands.Modules;

internal class ArenaModule : Module<ICharacter>
{
        private ArenaModule()
                : base("Arena")
        {
                IsNecessary = true;
        }

        public static ArenaModule Instance { get; } = new();

        private const string ArenaHelp =
                @"The #3arena#0 command provides access to combat arena features.

Managers:
	#3arena list#0 - lists known arenas
	#3arena show [<arena>]#0 - shows detailed arena information
	#3arena events [<arena>]#0 - lists scheduled and live events for an arena

Players:
        #3arena observe list#0 - shows events observable from your location
        #3arena observe enter <event>#0 - begin observing an event
        #3arena observe leave [<event>]#0 - stop observing events
        #3arena signup <event> <side> <class>#0 - sign up for an event
        #3arena withdraw <event>#0 - withdraw from an event
        #3arena bet odds <event> [<side>|draw]#0 - see betting quote
        #3arena bet place <event> <side|draw> <amount>#0 - place a wager
        #3arena bet cancel <event>#0 - cancel your wager
        #3arena bet pools <event>#0 - view pari-mutuel pools
        #3arena bet list#0 - view your current wagers and payouts
        #3arena bet history [<count>]#0 - view recent wagers
        #3arena bet collect [<event>]#0 - collect outstanding payouts
        #3arena ratings show [<class>]#0 - view your arena ratings";

        [PlayerCommand("Arena", "arena")]
        [RequiredCharacterState(CharacterState.Conscious)]
        [NoCombatCommand]
        [HelpInfo("arena", ArenaHelp, AutoHelp.HelpArg)]
        protected static void Arena(ICharacter actor, string command)
        {
                var ss = new StringStack(command.RemoveFirstWord());
                if (ss.IsFinished)
                {
                        ShowGeneralHelp(actor);
                        return;
                }

                switch (ss.PopForSwitch())
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
                        case "bet":
                                ArenaBet(actor, ss);
                                return;
                        case "ratings":
                                ArenaRatings(actor, ss);
                                return;
                        default:
                                ShowGeneralHelp(actor);
                                return;
                }
        }

        private static void ShowGeneralHelp(ICharacter actor)
        {
                actor.OutputHandler.Send(ArenaHelp.SubstituteANSIColour().Wrap(actor.InnerLineFormatLength));
        }

        private static void ArenaList(ICharacter actor)
        {
                var arenas = actor.Gameworld.CombatArenas.ToList();
                if (!arenas.Any())
                {
                        actor.OutputHandler.Send("There are no combat arenas configured.".ColourError());
                        return;
                }

                var header = new[] { "Id", "Arena", "Zone", "Currency" };
                var rows = arenas.Select(arena => new[]
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
		var hasArgument = !ss.IsFinished;
		var arena = GetArena(actor, hasArgument ? ss.PopSpeech() : null);
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
		var hasArgument = !ss.IsFinished;
		var arena = GetArena(actor, hasArgument ? ss.PopSpeech() : null);
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

                var events = arena.ActiveEvents.ToList();
                if (!events.Any())
                {
                        actor.OutputHandler.Send("There are no active or scheduled events for that arena.".ColourError());
                        return;
                }

                var header = new[] { "Id", "Name", "Type", "State", "Scheduled" };
                var rows = events.Select(evt => new[]
                {
                        evt.Id.ToString("N0", actor),
                        evt.Name.ColourName(),
                        evt.EventType.Name.ColourName(),
                        evt.State.DescribeEnum().ColourValue(),
                        evt.ScheduledAt.ToString("f", actor).ColourValue()
                }).ToList();

                actor.OutputHandler.Send(StringUtilities.GetTextTable(rows, header, actor, Telnet.Cyan));
        }

        private static void ArenaObserve(ICharacter actor, StringStack ss)
        {
                if (ss.IsFinished)
                {
                        actor.OutputHandler.Send("Do you want to #3list#0, #3enter#0, or #3leave#0 observation?".SubstituteANSIColour());
                        return;
                }

                var action = ss.PopForSwitch();
                switch (action)
                {
                        case "list":
                                ArenaObserveList(actor);
                                return;
                        case "enter":
                                ArenaObserveEnter(actor, ss);
                                return;
                        case "leave":
                                ArenaObserveLeave(actor, ss);
                                return;
                        default:
                                actor.OutputHandler.Send("Valid options are #3list#0, #3enter#0, or #3leave#0.".SubstituteANSIColour());
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

                var arenas = actor.Gameworld.CombatArenas.Where(x => x.ObservationCells.Contains(cell)).ToList();
                if (!arenas.Any())
                {
                        actor.OutputHandler.Send("This location is not an arena observation room.".ColourError());
                        return;
                }

                var events = arenas.SelectMany(x => x.ActiveEvents)
                        .Where(x => x.State >= ArenaEventState.Staged && x.State < ArenaEventState.Completed)
                        .Distinct()
                        .ToList();

                if (!events.Any())
                {
                        actor.OutputHandler.Send("There are no events available to observe from here.".ColourError());
                        return;
                }

                var header = new[] { "Id", "Arena", "Name", "State" };
                var rows = events.Select(evt => new[]
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

                if (ss.IsFinished)
                {
                        actor.OutputHandler.Send("Which event do you want to observe?".ColourCommand());
                        return;
                }

                var arenaEvent = GetArenaEvent(actor, ss.PopSpeech());
                if (arenaEvent is null)
                {
                        actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
                        return;
                }

                var (canObserve, reason) = actor.Gameworld.ArenaObservationService.CanObserve(actor, arenaEvent);
                if (!canObserve)
                {
                        actor.OutputHandler.Send(reason.ColourError());
                        return;
                }

                actor.Gameworld.ArenaObservationService.StartObserving(actor, arenaEvent, cell);
                actor.OutputHandler.Handle(new EmoteOutput(new Emote(
                        $"@ begin|begins observing the $1 event.", actor, actor, new DummyPerceivable(arenaEvent.Name.ColourName()))));
        }

        private static void ArenaObserveLeave(ICharacter actor, StringStack ss)
        {
                if (ss.IsFinished)
                {
                        foreach (var arena in actor.Gameworld.CombatArenas)
                        {
                                foreach (var activeEvent in arena.ActiveEvents)
                                {
                                        actor.Gameworld.ArenaObservationService.StopObserving(actor, activeEvent);
                                }
                        }

                        actor.OutputHandler.Send("You stop observing all arena events.".Colour(Telnet.Green));
                        return;
                }

                var arenaEvent = GetArenaEvent(actor, ss.PopSpeech());
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
                if (ss.IsFinished)
                {
                        actor.OutputHandler.Send("Which event do you want to sign up for?".ColourCommand());
                        return;
                }

                var arenaEvent = GetArenaEvent(actor, ss.PopSpeech());
                if (arenaEvent is null)
                {
                        actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
                        return;
                }

                if (ss.IsFinished)
                {
                        actor.OutputHandler.Send("Which side do you want to sign up for?".ColourCommand());
                        return;
                }

		if (!ArenaSideIndexUtilities.TryParseDisplayIndex(ss.PopSpeech(), out var sideIndex))
		{
			actor.OutputHandler.Send("You must specify the numeric side index starting at 1.".ColourError());
			return;
		}

                var side = arenaEvent.EventType.Sides.FirstOrDefault(x => x.Index == sideIndex);
                if (side is null)
                {
                        actor.OutputHandler.Send("That side does not exist for the selected event.".ColourError());
                        return;
                }

                if (ss.IsFinished)
                {
                        actor.OutputHandler.Send("Which combatant class do you want to use?".ColourCommand());
                        return;
                }

                var classArg = ss.PopSpeech();
                var combatantClass = FindCombatantClass(side.EligibleClasses, classArg);
                if (combatantClass is null)
                {
                        actor.OutputHandler.Send("That combatant class is not eligible for this side.".ColourError());
                        return;
                }

                try
                {
                        var (allowed, reason) = arenaEvent.CanSignUp(actor, sideIndex, combatantClass);
                        if (!allowed)
                        {
                                actor.OutputHandler.Send(reason.ColourError());
                                return;
                        }

                        arenaEvent.SignUp(actor, sideIndex, combatantClass);
			actor.OutputHandler.Send(
				$"You sign up for {arenaEvent.Name.ColourName()} on side {ArenaSideIndexUtilities.ToDisplayString(actor, sideIndex).ColourValue()} as {combatantClass.Name.ColourName()}".Colour(Telnet.Green));
                }
                catch (Exception ex)
                {
                        actor.OutputHandler.Send(ex.Message.ColourError());
                }
        }

        private static void ArenaWithdraw(ICharacter actor, StringStack ss)
        {
                if (ss.IsFinished)
                {
                        actor.OutputHandler.Send("Which event do you want to withdraw from?".ColourCommand());
                        return;
                }

                var arenaEvent = GetArenaEvent(actor, ss.PopSpeech());
                if (arenaEvent is null)
                {
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
                if (ss.IsFinished)
                {
                        actor.OutputHandler.Send("Which event do you want to get odds for?".ColourCommand());
                        return;
                }

                var arenaEvent = GetArenaEvent(actor, ss.PopSpeech());
                if (arenaEvent is null)
                {
                        actor.OutputHandler.Send("There is no arena event matching that description.".ColourError());
                        return;
                }

                var sideIndex = ParseSideIndex(arenaEvent, ss.IsFinished ? null : ss.PopSpeech(), actor);
                if (sideIndex.Invalid)
                {
                        actor.OutputHandler.Send(sideIndex.Error.ColourError());
                        return;
                }

                var quote = actor.Gameworld.ArenaBettingService.GetQuote(arenaEvent, sideIndex.Value);
                var sb = new StringBuilder();
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
                        actor.OutputHandler.Send("Which event do you want to bet on?".ColourCommand());
                        return;
                }

                var arenaEvent = GetArenaEvent(actor, ss.PopSpeech());
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

                var sideIndex = ParseSideIndex(arenaEvent, ss.PopSpeech(), actor);
                if (sideIndex.Invalid)
                {
                        actor.OutputHandler.Send(sideIndex.Error.ColourError());
                        return;
                }

                if (ss.IsFinished)
                {
                        actor.OutputHandler.Send("How much do you want to stake?".ColourCommand());
                        return;
                }

                var amountText = ss.PopSpeech();
                if (!arenaEvent.Arena.Currency.TryGetBaseCurrency(amountText, out var amount) || amount <= 0)
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
                if (ss.IsFinished)
                {
                        actor.OutputHandler.Send("Which event do you want to cancel your bet for?".ColourCommand());
                        return;
                }

                var arenaEvent = GetArenaEvent(actor, ss.PopSpeech());
                if (arenaEvent is null)
                {
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
                if (ss.IsFinished)
                {
                        actor.OutputHandler.Send("Which event's pools do you want to view?".ColourCommand());
                        return;
                }

                var arenaEvent = GetArenaEvent(actor, ss.PopSpeech());
                if (arenaEvent is null)
                {
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
                        var context = FMDB.Context;
                        var pools = context.ArenaBetPools.Where(x => x.ArenaEventId == arenaEvent.Id).ToList();
                        if (!pools.Any())
                        {
                                actor.OutputHandler.Send("There are no active pools for that event.".ColourError());
                                return;
                        }

                        var header = new[] { "Side", "Pool" };
                        var rows = pools.Select(pool => new[]
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

                var bets = actor.Gameworld.ArenaBettingService.GetActiveBets(actor).ToList();
                var payouts = actor.Gameworld.ArenaBettingService.GetOutstandingPayouts(actor).ToList();

                if (!bets.Any() && !payouts.Any())
                {
                        actor.OutputHandler.Send("You have no active wagers or outstanding payouts.".ColourError());
                        return;
                }

                var sb = new StringBuilder();
                if (bets.Any())
                {
                        sb.AppendLine("Active Wagers:");
                        var header = new[] { "Event", "Arena", "Side", "Stake", "State", "Placed" };
                        var rows = bets.Select(bet => new[]
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
                        var header = new[] { "Event", "Arena", "Amount", "Status", "Recorded" };
                        var rows = payouts.Select(payout => new[]
                        {
                                DescribeEvent(payout.EventTypeName, payout.ArenaEventId, actor),
                                payout.ArenaName.ColourName(),
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
                var count = 20;
                if (!ss.IsFinished)
                {
                        if (!int.TryParse(ss.PopSpeech(), NumberStyles.Integer, CultureInfo.InvariantCulture, out count) || count <= 0)
                        {
                                actor.OutputHandler.Send("You must specify a positive number of wagers to show.".ColourError());
                                return;
                        }
                }

                var bets = actor.Gameworld.ArenaBettingService.GetBetHistory(actor, count).ToList();
                if (!bets.Any())
                {
                        actor.OutputHandler.Send("You have no betting history to display.".ColourError());
                        return;
                }

                var header = new[] { "Event", "Arena", "Side", "Stake", "Result", "Placed" };
                var rows = bets.Select(bet => new[]
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
                        var eventText = ss.PopSpeech();
                        if (!long.TryParse(eventText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
                        {
                                var arenaEvent = GetArenaEvent(actor, eventText);
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
                        var payout = actor.Gameworld.ArenaBettingService.GetOutstandingPayouts(actor)
                                .FirstOrDefault(x => x.ArenaEventId == eventId.Value);
                        arena = payout is null ? null : actor.Gameworld.CombatArenas.Get(payout.ArenaId);
                }

                var result = actor.Gameworld.ArenaBettingService.CollectOutstandingPayouts(actor, eventId);
                if (result.CollectedCount == 0 && result.FailedCount == 0 && result.BlockedCount == 0)
                {
                        var message = eventId.HasValue
                                ? $"You have no outstanding payouts for event #{eventId.Value.ToString("N0", actor)}."
                                : "You have no outstanding payouts to collect.";
                        actor.OutputHandler.Send(message.ColourError());
                        return;
                }

                var sb = new StringBuilder();
                if (result.CollectedCount > 0)
                {
                        var collectedText = eventId.HasValue && arena is not null
                                ? $"You collect {DescribeCurrency(arena, result.CollectedAmount)} from {result.CollectedCount.ToString("N0", actor).ColourValue()} payout(s)."
                                : $"You collect {result.CollectedCount.ToString("N0", actor).ColourValue()} payout(s).";
                        sb.AppendLine(collectedText.Colour(Telnet.Green));
                }

                if (result.FailedCount > 0)
                {
                        var failedText = eventId.HasValue && arena is not null
                                ? $"{result.FailedCount.ToString("N0", actor).ColourValue()} payout(s) totaling {DescribeCurrency(arena, result.FailedAmount)} could not be disbursed."
                                : $"{result.FailedCount.ToString("N0", actor).ColourValue()} payout(s) could not be disbursed.";
                        sb.AppendLine(failedText.ColourError());
                }

                if (result.BlockedCount > 0)
                {
                        var blockedText = eventId.HasValue && arena is not null
                                ? $"{result.BlockedCount.ToString("N0", actor).ColourValue()} payout(s) totaling {DescribeCurrency(arena, result.BlockedAmount)} are still blocked pending arena funds."
                                : $"{result.BlockedCount.ToString("N0", actor).ColourValue()} payout(s) are still blocked pending arena funds.";
                        sb.AppendLine(blockedText.Colour(Telnet.Yellow));
                }

                actor.OutputHandler.Send(sb.ToString());
        }

        private static void ArenaRatings(ICharacter actor, StringStack ss)
        {
                var filter = ss.IsFinished ? null : ss.PopSpeech();
                var classes = actor.Gameworld.CombatArenas
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

                var header = new[] { "Class", "Rating" };
                var rows = classes.Select(cls => new[]
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

		var arenas = actor.Gameworld.CombatArenas
			.Where(arena =>
				arena.WaitingCells.Contains(cell) ||
				arena.ArenaCells.Contains(cell) ||
				arena.ObservationCells.Contains(cell) ||
				arena.InfirmaryCells.Contains(cell) ||
				arena.AfterFightCells.Contains(cell) ||
				arena.NpcStablesCells.Contains(cell))
			.ToList();

		return arenas.Count == 1 ? arenas[0] : null;
	}

	private static IArenaEvent? GetArenaEvent(ICharacter actor, string text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}

		var localArena = GetArenaFromLocation(actor);
		var allEvents = actor.Gameworld.CombatArenas.SelectMany(x => x.ActiveEvents).ToList();
		var localEvents = localArena is null ? allEvents : localArena.ActiveEvents.ToList();
		var isIdSearch = long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id);

		IArenaEvent? FindEvent(IEnumerable<IArenaEvent> events)
		{
			if (isIdSearch)
			{
				return events.FirstOrDefault(x => x.Id == id);
			}

			return events.FirstOrDefault(x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase)) ??
			       events.FirstOrDefault(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		}

		var localMatch = FindEvent(localEvents);
		if (localMatch != null || localArena is null)
		{
			return localMatch;
		}

		return FindEvent(allEvents);
	}

        private static (int? Value, bool Invalid, string Error) ParseSideIndex(IArenaEvent arenaEvent, string? text,
                ICharacter actor)
        {
                if (string.IsNullOrWhiteSpace(text) || text.Equals("draw", StringComparison.InvariantCultureIgnoreCase))
                {
                        return (null, false, string.Empty);
                }

		if (!ArenaSideIndexUtilities.TryParseDisplayIndex(text, out var value))
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

                if (long.TryParse(text, out var id))
                {
                        return classes.FirstOrDefault(x => x.Id == id);
                }

                return classes.FirstOrDefault(x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase)) ??
                       classes.FirstOrDefault(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
        }

        private static string DescribeEvent(string eventTypeName, long eventId, ICharacter actor)
        {
                var idText = eventId.ToString("N0", actor).ColourValue();
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
                        var amountText = DescribeCurrency(actor, bet.ArenaId, bet.BlockedPayout);
                        return $"{"Blocked".ColourError()} ({amountText})";
                }

                if (bet.OutstandingPayout > 0.0m)
                {
                        var amountText = DescribeCurrency(actor, bet.ArenaId, bet.OutstandingPayout);
                        return $"{"Owed".Colour(Telnet.Yellow)} ({amountText})";
                }

                if (bet.CollectedPayout > 0.0m)
                {
                        var amountText = DescribeCurrency(actor, bet.ArenaId, bet.CollectedPayout);
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
                var arena = actor.Gameworld.CombatArenas.Get(arenaId);
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

                var side = arenaEvent.EventType.Sides.FirstOrDefault(x => x.Index == sideIndex.Value);
		var displayIndex = ArenaSideIndexUtilities.ToDisplayString(actor, sideIndex.Value).ColourValue();
		return side is null
			? $"Side {displayIndex}"
			: $"Side {displayIndex} ({side.Policy.DescribeEnum().ColourValue()})";
        }
}

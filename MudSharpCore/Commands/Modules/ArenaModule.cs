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
        #3arena show <arena>#0 - shows detailed arena information
        #3arena events <arena>#0 - lists scheduled and live events for an arena

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
                actor.OutputHandler.Send(ArenaHelp.Wrap(actor.InnerLineFormatLength));
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
                if (ss.IsFinished)
                {
                        actor.OutputHandler.Send("Which arena do you want to view?".ColourCommand());
                        return;
                }

                var arena = GetArena(actor, ss.PopSpeech());
                if (arena is null)
                {
                        actor.OutputHandler.Send("There is no arena matching that description.".ColourError());
                        return;
                }

                actor.Gameworld.ArenaCommandService.ShowArena(actor, arena);
        }

        private static void ArenaEvents(ICharacter actor, StringStack ss)
        {
                if (ss.IsFinished)
                {
                        actor.OutputHandler.Send("Which arena's events do you want to view?".ColourCommand());
                        return;
                }

                var arena = GetArena(actor, ss.PopSpeech());
                if (arena is null)
                {
                        actor.OutputHandler.Send("There is no arena matching that description.".ColourError());
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
                        actor.OutputHandler.Send("Do you want to #3list#0, #3enter#0, or #3leave#0 observation?".ColourCommand());
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
                                actor.OutputHandler.Send("Valid options are #3list#0, #3enter#0, or #3leave#0.".ColourError());
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
                        $"@ begin|begins observing the {arenaEvent.Name.ColourName()} event.", actor)));
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

                if (!int.TryParse(ss.PopSpeech(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var sideIndex))
                {
                        actor.OutputHandler.Send("You must specify the numeric side index.".ColourError());
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
                                $"You sign up for {arenaEvent.Name.ColourName()} on side {sideIndex.ToString(actor).ColourValue()} as {combatantClass.Name.ColourName()}".Colour(Telnet.Green));
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
                        actor.OutputHandler.Send("Do you want to #3odds#0, #3place#0, #3cancel#0, or #3pools#0?".ColourCommand());
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
                        default:
                                actor.OutputHandler.Send("Valid options are #3odds#0, #3place#0, #3cancel#0, or #3pools#0.".ColourError());
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
                                $"You stake {DescribeCurrency(arenaEvent.Arena, amount)} on {DescribeSide(arenaEvent, sideIndex.Value)}.".Colour(Telnet.Green));
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
                                pool.SideIndex?.ToString(actor) ?? "Draw",
                                DescribeCurrency(arenaEvent.Arena, pool.TotalStake)
                        }).ToList();

                actor.OutputHandler.Send(StringUtilities.GetTextTable(rows, header, actor, Telnet.Green));
                }
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

        private static ICombatArena? GetArena(ICharacter actor, string text)
        {
                if (string.IsNullOrWhiteSpace(text))
                {
                        return null;
                }

                return actor.Gameworld.CombatArenas.GetByIdOrName(text);
        }

        private static IArenaEvent? GetArenaEvent(ICharacter actor, string text)
        {
                if (string.IsNullOrWhiteSpace(text))
                {
                        return null;
                }

                if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                {
                        return actor.Gameworld.CombatArenas.SelectMany(x => x.ActiveEvents).FirstOrDefault(x => x.Id == id);
                }

                return actor.Gameworld.CombatArenas.SelectMany(x => x.ActiveEvents)
                        .FirstOrDefault(x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase)) ??
                       actor.Gameworld.CombatArenas.SelectMany(x => x.ActiveEvents)
                               .FirstOrDefault(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
        }

        private static (int? Value, bool Invalid, string Error) ParseSideIndex(IArenaEvent arenaEvent, string? text,
                ICharacter actor)
        {
                if (string.IsNullOrWhiteSpace(text) || text.Equals("draw", StringComparison.InvariantCultureIgnoreCase))
                {
                        return (null, false, string.Empty);
                }

                if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
                {
                        return (null, true, "You must specify a numeric side or the word draw.");
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

        private static string DescribeCurrency(ICombatArena arena, decimal amount)
        {
                return arena.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
        }

        private static string DescribeSide(IArenaEvent arenaEvent, int? sideIndex)
        {
                if (!sideIndex.HasValue)
                {
                        return "draw".ColourValue();
                }

                var side = arenaEvent.EventType.Sides.FirstOrDefault(x => x.Index == sideIndex.Value);
                return side is null
                        ? sideIndex.Value.ToString(CultureInfo.InvariantCulture).ColourValue()
                        : $"{sideIndex.Value.ToString(CultureInfo.InvariantCulture).ColourValue()} ({side.Policy.DescribeEnum().ColourValue()})";
        }
}

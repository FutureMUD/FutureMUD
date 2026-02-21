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
using MudSharp.TimeAndDate;

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
		@"The #3arena#0 command provides access to combat arena systems.

General:
	#3arena list#0 - list known arenas
	#3arena show [<arena>]#0 - show details for an arena
	#3arena events [<arena>]#0 - list active and scheduled events

Participant Commands:
	If only one event applies, you can omit #6<event>#0.
	#3arena observe list#0 - show events observable from your location
	#3arena observe enter [<event>]#0 - begin observing an event
	#3arena observe leave [<event>]#0 - stop observing an event
	#3arena signup [<event>] [<side>] [<class>]#0 - sign up for an event (omitted values auto-select)
	#3arena withdraw [<event>]#0 - withdraw from an event
	#3arena bet odds [<side>|draw] [<event>]#0 - view betting quotes
	#3arena bet place <side|draw> <amount> [<event>]#0 - place a wager
	#3arena bet cancel [<event>]#0 - cancel your wager
	#3arena bet pools [<event>]#0 - view pari-mutuel pools
	#3arena bet list#0 - list active wagers and payouts
	#3arena bet history [<count>]#0 - show betting history
	#3arena bet collect [<event>]#0 - collect outstanding payouts
	#3arena ratings [<class>]#0 - show your arena ratings

Manager Commands:
	You must be a manager of the arena owning the target event or event type.
	#3arena manager phase <event> <state>#0 - force an event to a phase using normal transitions
	#3arena manager autoschedule <eventtype> show#0 - show recurring settings
	#3arena manager autoschedule <eventtype> off#0 - disable recurring creation
	#3arena manager autoschedule <eventtype> every <interval> [from] <reference>#0 - enable recurring creation";

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

		var concreteEvents = arena.ActiveEvents.ToList();
		var projectedEvents = GetProjectedAutoScheduledEvents(arena, concreteEvents).ToList();
		if (!concreteEvents.Any() && !projectedEvents.Any())
		{
			actor.OutputHandler.Send("There are no active or scheduled events for that arena.".ColourError());
			return;
		}

		var header = new[] { "Id", "Name", "Type", "State", "Scheduled" };
		var rows = concreteEvents
			.Select(evt => new
			{
				evt.ScheduledAt,
				Id = evt.Id.ToString("N0", actor),
				Name = evt.Name.ColourName(),
				Type = evt.EventType.Name.ColourName(),
				State = evt.State.DescribeEnum().ColourValue(),
				Scheduled = evt.ScheduledAt.ToString("f", actor).ColourValue()
			})
			.Concat(projectedEvents.Select(evt => new
			{
				ScheduledAt = evt.ScheduledFor,
				Id = "Auto".ColourCommand(),
				Name = $"{evt.EventType.Name} Event".ColourName(),
				Type = evt.EventType.Name.ColourName(),
				State = "Scheduled (Auto)".ColourCommand(),
				Scheduled = evt.ScheduledFor.ToString("f", actor).ColourValue()
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
				"Do you want to #3phase#0 or #3autoschedule#0? Use #3help arena#0 for syntax.".SubstituteANSIColour());
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
			default:
				actor.OutputHandler.Send(
					"Valid options are #3phase#0 and #3autoschedule#0.".SubstituteANSIColour());
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

		var eventText = ss.PopSpeech();
		var arenaEvent = GetArenaEvent(actor, eventText);
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

		if (!TryParseArenaState(ss.PopSpeech(), out var targetState))
		{
			actor.OutputHandler.Send(
				$"That is not a valid state. Valid options are {Enum.GetValues<ArenaEventState>().Select(x => x.DescribeEnum().ColourValue()).ListToString()}."
					.ColourError());
			return;
		}

		var currentState = arenaEvent.State;
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

		var eventTypeText = ss.PopSpeech();
		var eventType = GetArenaEventType(actor, eventTypeText);
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

		var firstToken = ss.PopForSwitch();
		if (firstToken.EqualToAny("off", "none", "disable", "clear", "remove"))
		{
			eventType.ConfigureAutoSchedule(null, null);
			actor.OutputHandler.Send(
				$"Auto scheduling is now disabled for {eventType.Name.ColourName()}.".Colour(Telnet.Green));
			return;
		}

		var intervalText = firstToken;
		if (firstToken.EqualTo("every"))
		{
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("You must specify a recurrence interval.".ColourError());
				return;
			}

			intervalText = ss.PopSpeech();
		}

		if (!MudTimeSpan.TryParse(intervalText, actor, out var intervalMud))
		{
			actor.OutputHandler.Send(
				"That is not a valid interval. Examples: #36h#0, #390m#0, #31d 2h#0.".SubstituteANSIColour());
			return;
		}

		var interval = intervalMud.AsTimeSpan();
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

		if (!DateUtilities.TryParseDateTimeOrRelative(ss.SafeRemainingArgument, actor.Account, false, out var referenceUtc))
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
                        .Where(x => x.State >= ArenaEventState.RegistrationOpen && x.State < ArenaEventState.Completed)
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

                var eventText = ss.IsFinished ? null : ss.PopSpeech();
                var arenaEvent = GetArenaEvent(actor, eventText);
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
		IArenaEvent? arenaEvent = null;
		int? requestedSideIndex = null;
		string? requestedClass = null;
		var invalidEventReference = false;

		if (ss.IsFinished)
		{
			arenaEvent = GetArenaEvent(actor, null);
		}
		else
		{
			var originalArguments = ss.SafeRemainingArgument;
			var argumentCount = ss.CountRemainingArguments();
			var firstArg = ss.PopSpeech();

			if (ArenaSideIndexUtilities.TryParseDisplayIndex(firstArg, out var parsedSideIndex))
			{
				requestedSideIndex = parsedSideIndex;
				if (!ss.IsFinished)
				{
					requestedClass = ss.PopSpeech();
					if (ss.IsFinished)
					{
						var explicitEvent = GetArenaEvent(actor, requestedClass);
						if (explicitEvent is not null)
						{
							arenaEvent = explicitEvent;
							requestedClass = null;
						}
					}
					else
					{
						var eventText = ss.SafeRemainingArgument;
						arenaEvent = GetArenaEvent(actor, eventText);
						if (arenaEvent is null && argumentCount >= 3)
						{
							var fallback = new StringStack(originalArguments);
							var fallbackEvent = GetArenaEvent(actor, fallback.PopSpeech());
							if (fallbackEvent is not null && !fallback.IsFinished &&
							    ArenaSideIndexUtilities.TryParseDisplayIndex(fallback.PopSpeech(),
								    out var fallbackSideIndex))
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
					var eventText = ss.IsFinished ? null : ss.SafeRemainingArgument;
					arenaEvent = GetArenaEvent(actor, eventText);
					if (arenaEvent is null && !string.IsNullOrWhiteSpace(eventText))
					{
						invalidEventReference = true;
					}
				}
				else if (!ss.IsFinished)
				{
					var secondArg = ss.PopSpeech();
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

		var (sideIndex, combatantClass, error) = ResolveSignupSelection(actor, arenaEvent, requestedSideIndex, requestedClass);
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
                var eventText = ss.IsFinished ? null : ss.PopSpeech();
                var arenaEvent = GetArenaEvent(actor, eventText);
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
			var firstArg = ss.PopSpeech();
			if (TryParseSideSpecifier(firstArg, out _))
			{
				sideText = firstArg;
				var eventText = ss.IsFinished ? null : ss.SafeRemainingArgument;
				arenaEvent = GetArenaEvent(actor, eventText);
				if (arenaEvent is null)
				{
					if (string.IsNullOrWhiteSpace(eventText) || TryParseSideSpecifier(eventText, out _))
					{
						var explicitEvent = GetArenaEvent(actor, firstArg);
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

                var sideIndex = ParseSideIndex(arenaEvent, sideText, actor);
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
                        actor.OutputHandler.Send("Which side (or draw) do you want to bet on?".ColourCommand());
                        return;
                }

		var originalArguments = ss.SafeRemainingArgument;
		var argumentCount = ss.CountRemainingArguments();
		var firstArg = ss.PopSpeech();
		IArenaEvent? arenaEvent = null;
		var sideText = string.Empty;
		var amountText = string.Empty;

		if (TryParseSideSpecifier(firstArg, out _))
		{
			sideText = firstArg;
			if (ss.IsFinished)
			{
				actor.OutputHandler.Send("How much do you want to stake?".ColourCommand());
				return;
			}

			amountText = ss.PopSpeech();
			var eventText = ss.IsFinished ? null : ss.SafeRemainingArgument;
			arenaEvent = GetArenaEvent(actor, eventText);
			if (arenaEvent is null && !string.IsNullOrWhiteSpace(eventText) && argumentCount >= 3)
			{
				var fallback = new StringStack(originalArguments);
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

                var sideIndex = ParseSideIndex(arenaEvent, sideText, actor);
                if (sideIndex.Invalid)
                {
                        actor.OutputHandler.Send(sideIndex.Error.ColourError());
                        return;
                }

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
                var eventText = ss.IsFinished ? null : ss.PopSpeech();
                var arenaEvent = GetArenaEvent(actor, eventText);
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
                var eventText = ss.IsFinished ? null : ss.PopSpeech();
                var arenaEvent = GetArenaEvent(actor, eventText);
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
                if (!ss.IsFinished && ss.PeekSpeech().EqualTo("show"))
                {
                        ss.PopSpeech();
                }

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

		return actor.Gameworld.CombatArenas
			.FirstOrDefault(arena =>
				arena.WaitingCells.Contains(cell) ||
				arena.ArenaCells.Contains(cell) ||
				arena.ObservationCells.Contains(cell) ||
				arena.InfirmaryCells.Contains(cell) ||
				arena.AfterFightCells.Contains(cell) ||
				arena.NpcStablesCells.Contains(cell));
	}

	private static IArenaEvent? GetArenaEvent(ICharacter actor, string? text)
	{
		var localArena = GetArenaFromLocation(actor);
		var allEvents = actor.Gameworld.CombatArenas.SelectMany(x => x.ActiveEvents).ToList();
		var localEvents = localArena is null ? allEvents : localArena.ActiveEvents.ToList();

		IArenaEvent? ResolveImplicitEvent(IEnumerable<IArenaEvent> events)
		{
			var eventList = events.ToList();
			var currentEvents = eventList.Where(IsCurrentArenaEvent).ToList();
			if (currentEvents.Count == 1)
			{
				return currentEvents[0];
			}

			return eventList.Count == 1 ? eventList[0] : null;
		}

		if (string.IsNullOrWhiteSpace(text))
		{
			var implicitMatch = ResolveImplicitEvent(localEvents);
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

	private static IArenaEventType? GetArenaEventType(ICharacter actor, string? text)
	{
		var localArena = GetArenaFromLocation(actor);
		var allTypes = actor.Gameworld.CombatArenas.SelectMany(x => x.EventTypes).ToList();
		var localTypes = localArena is null ? allTypes : localArena.EventTypes.ToList();
		if (string.IsNullOrWhiteSpace(text))
		{
			if (localTypes.Count == 1)
			{
				return localTypes[0];
			}

			return localArena is not null && allTypes.Count == 1 ? allTypes[0] : null;
		}

		var isIdSearch = long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id);

		IArenaEventType? FindType(IEnumerable<IArenaEventType> types)
		{
			if (isIdSearch)
			{
				return types.FirstOrDefault(x => x.Id == id);
			}

			return types.FirstOrDefault(x => x.Name.Equals(text, StringComparison.InvariantCultureIgnoreCase)) ??
			       types.FirstOrDefault(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		}

		var localMatch = FindType(localTypes);
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
		var now = DateTime.UtcNow;
		var concreteEvents = existingEvents.ToList();
		foreach (var eventType in arena.EventTypes)
		{
			if (!IsRecurringEnabled(eventType))
			{
				continue;
			}

			var scheduledFor = ResolveNextRecurringTrigger(eventType, now);
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
		var reference = eventType.AutoScheduleReferenceTime!.Value;
		var interval = eventType.AutoScheduleInterval!.Value;
		if (reference >= now)
		{
			return reference;
		}

		var elapsedTicks = now.Ticks - reference.Ticks;
		var intervalTicks = interval.Ticks;
		if (intervalTicks <= 0)
		{
			return now;
		}

		var cycles = elapsedTicks / intervalTicks;
		var next = reference.AddTicks(cycles * intervalTicks);
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

		if (ArenaSideIndexUtilities.TryParseDisplayIndex(text, out var value))
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
		var sides = arenaEvent.EventType.Sides
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

		var firstFailureReason = string.Empty;
		var checkedAnyCombination = false;
		var classMatchedAnySide = false;
		foreach (var side in sides)
		{
			IEnumerable<ICombatantClass> classes;
			if (!string.IsNullOrWhiteSpace(requestedClass))
			{
				var explicitClass = FindCombatantClass(side.EligibleClasses, requestedClass);
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

			foreach (var combatantClass in classes)
			{
				checkedAnyCombination = true;
				var (allowed, reason) = arenaEvent.CanSignUp(actor, side.Index, combatantClass);
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
			var reason = requestedSideIndex.HasValue
				? "That combatant class is not eligible for this side."
				: "That combatant class is not eligible for any side in that event.";
			return (null, null, reason);
		}

		if (!checkedAnyCombination)
		{
			var reason = requestedSideIndex.HasValue
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

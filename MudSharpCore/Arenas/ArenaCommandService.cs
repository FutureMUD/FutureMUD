#nullable enable
using System;
using System.Linq;
using System.Text;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.Character.Name;

namespace MudSharp.Arenas;

/// <summary>
///     Provides formatted arena information for commands and builders.
/// </summary>
public class ArenaCommandService : IArenaCommandService
{
        private readonly IFuturemud _gameworld;

        public ArenaCommandService(IFuturemud gameworld)
        {
                _gameworld = gameworld ?? throw new ArgumentNullException(nameof(gameworld));
        }

        /// <inheritdoc />
        public void ShowArena(ICharacter actor, ICombatArena arena)
        {
                if (actor is null)
                {
                        throw new ArgumentNullException(nameof(actor));
                }

                if (arena is null)
                {
                        throw new ArgumentNullException(nameof(arena));
                }

                actor.OutputHandler.Send(arena.ShowToManager(actor));
        }

        /// <inheritdoc />
        public void ShowEvent(ICharacter actor, IArenaEvent arenaEvent)
        {
                if (actor is null)
                {
                        throw new ArgumentNullException(nameof(actor));
                }

                if (arenaEvent is null)
                {
                        throw new ArgumentNullException(nameof(arenaEvent));
                }

                var sb = new StringBuilder();
                sb.AppendLine($"Event {arenaEvent.Name.ColourName()} ({arenaEvent.EventType.Name.ColourName()})");
                sb.AppendLine($"Arena: {arenaEvent.Arena.Name.ColourName()}");
                sb.AppendLine($"State: {arenaEvent.State.DescribeEnum().ColourValue()}");
                sb.AppendLine($"Scheduled: {arenaEvent.ScheduledAt.ToString("f", actor).ColourValue()}");
                if (arenaEvent.RegistrationOpensAt is { } reg)
                {
                        sb.AppendLine($"Registration Opens: {reg.ToString("f", actor).ColourValue()}");
                }
                if (arenaEvent.StartedAt is { } started)
                {
                        sb.AppendLine($"Started: {started.ToString("f", actor).ColourValue()}");
                }
                if (arenaEvent.ResolvedAt is { } resolved)
                {
                        sb.AppendLine($"Resolved: {resolved.ToString("f", actor).ColourValue()}");
                }
                if (arenaEvent.CompletedAt is { } completed)
                {
                        sb.AppendLine($"Completed: {completed.ToString("f", actor).ColourValue()}");
                }

                sb.AppendLine();
                sb.AppendLine("Participants:".Colour(Telnet.Cyan));
                var grouped = arenaEvent.Participants.GroupBy(x => x.SideIndex).OrderBy(x => x.Key);
                foreach (var group in grouped)
                {
                        var side = arenaEvent.EventType.Sides.FirstOrDefault(x => x.Index == group.Key);
                        var title = side is null
                                ? $"Side {group.Key}".Colour(Telnet.Yellow)
                                : side.Index.ToString(actor).ColourValue() + $" - {side.Policy.DescribeEnum().ColourValue()}";
                        sb.AppendLine(title);
                        foreach (var participant in group)
                        {
                                var name = participant.Character?.PersonalName?.GetName(NameStyle.FullName) ??
                                           participant.StageName ??
                                           participant.Character?.Name ?? "NPC";
                                var className = participant.CombatantClass?.Name ?? "Unknown";
                                sb.AppendLine(
                                        $"\t{name.ColourName()} ({className.ColourName()})" +
                                        (participant.IsNpc ? " [NPC]".Colour(Telnet.Yellow) : string.Empty));
                        }
                }

                actor.OutputHandler.Send(sb.ToString());
        }

        /// <inheritdoc />
        public void ShowEventType(ICharacter actor, IArenaEventType eventType)
        {
                if (actor is null)
                {
                        throw new ArgumentNullException(nameof(actor));
                }

                if (eventType is null)
                {
                        throw new ArgumentNullException(nameof(eventType));
                }

                var sb = new StringBuilder();
                sb.AppendLine($"Event Type {eventType.Name.ColourName()}");
                sb.AppendLine($"Arena: {eventType.Arena.Name.ColourName()}");
                sb.AppendLine($"Bring Your Own: {eventType.BringYourOwn.ToColouredString()}");
                sb.AppendLine(
                        $"Registration: {eventType.RegistrationDuration.Describe(actor).ColourValue()}, Preparation: {eventType.PreparationDuration.Describe(actor).ColourValue()}");
                sb.AppendLine(eventType.TimeLimit is null
                        ? "Time Limit: None".Colour(Telnet.Green)
                        : $"Time Limit: {eventType.TimeLimit.Value.Describe(actor).ColourValue()}");
                sb.AppendLine($"Betting: {eventType.BettingModel.DescribeEnum().ColourValue()}");
                sb.AppendLine($"Appearance Fee: {DescribeCurrency(eventType.Arena, eventType.AppearanceFee)}");
                sb.AppendLine($"Victory Fee: {DescribeCurrency(eventType.Arena, eventType.VictoryFee)}");

                sb.AppendLine();
                sb.AppendLine("Sides:".Colour(Telnet.Cyan));
                foreach (var side in eventType.Sides.OrderBy(x => x.Index))
                {
                        sb.AppendLine($"Side {side.Index.ToString(actor).ColourValue()} (Capacity {side.Capacity.ToString(actor).ColourValue()})");
                        sb.AppendLine($"\tPolicy: {side.Policy.DescribeEnum().ColourValue()}");
                        sb.AppendLine($"\tAllow NPC Signup: {side.AllowNpcSignup.ToColouredString()}");
                        sb.AppendLine($"\tAuto Fill NPC: {side.AutoFillNpc.ToColouredString()}");
                        if (side.EligibleClasses.Any())
                        {
                                sb.AppendLine($"\tEligible Classes: {side.EligibleClasses.Select(x => x.Name.ColourName()).ListToString()}");
                        }
                        else
                        {
                                sb.AppendLine("\tEligible Classes: None".Colour(Telnet.Red));
                        }
                }

                actor.OutputHandler.Send(sb.ToString());
        }

        private static string DescribeCurrency(ICombatArena arena, decimal amount)
        {
                return arena.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
        }
}

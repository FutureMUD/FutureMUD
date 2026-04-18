#nullable enable
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Arenas;

/// <summary>
/// Provides finance guardrails and deferred payout handling for arenas.
/// </summary>
public class ArenaFinanceService : IArenaFinanceService
{
    private readonly Func<FuturemudDatabaseContext>? _contextFactory;

    public ArenaFinanceService(Func<FuturemudDatabaseContext>? contextFactory = null)
    {
        _contextFactory = contextFactory;
    }

    /// <inheritdoc />
    public (bool Truth, string Reason) IsSolvent(ICombatArena arena, decimal required)
    {
        if (arena is null)
        {
            throw new ArgumentNullException(nameof(arena));
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        decimal outstanding = context.ArenaBetPayouts
            .Where(x => x.IsBlocked && x.CollectedAt == null && x.ArenaEvent.ArenaId == arena.Id)
            .Sum(x => (decimal?)x.Amount) ?? 0.0m;
        decimal available = arena.AvailableFunds();
        decimal net = available - outstanding;
        if (net >= required)
        {
            return (true, string.Empty);
        }

        decimal shortage = required - net;
        string message = $"Arena requires {required:n2} but is short {shortage:n2} after liabilities.";
        return (false, message);
    }

    /// <inheritdoc />
    public void WithholdTax(ICombatArena arena, decimal amount, string reference)
    {
        if (arena is null)
        {
            throw new ArgumentNullException(nameof(arena));
        }

        if (amount <= 0)
        {
            return;
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        ArenaFinanceSnapshot snapshot = GetOrCreateSnapshot(context, arena.Id, null, CurrentPeriod());
        snapshot.TaxWithheld += amount;
        snapshot.Costs += amount;
        snapshot.Profit = snapshot.Revenue - snapshot.Costs - snapshot.TaxWithheld;
        context.SaveChanges();

        arena.Debit(amount, reference);
    }

    /// <inheritdoc />
    public void PostProfitLoss(ICombatArena arena, string reference)
    {
        if (arena is null)
        {
            throw new ArgumentNullException(nameof(arena));
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        ArenaFinanceSnapshot snapshot = GetOrCreateSnapshot(context, arena.Id, null, CurrentPeriod());
        snapshot.Profit = snapshot.Revenue - snapshot.Costs - snapshot.TaxWithheld;
        snapshot.CreatedAt = DateTime.UtcNow;
        context.SaveChanges();
    }

    /// <inheritdoc />
    public void BlockPayout(ICombatArena arena, IArenaEvent arenaEvent, IEnumerable<(ICharacter Winner, decimal Amount)> payouts)
    {
        if (arena is null)
        {
            throw new ArgumentNullException(nameof(arena));
        }

        if (arenaEvent is null)
        {
            throw new ArgumentNullException(nameof(arenaEvent));
        }

        if (payouts is null)
        {
            return;
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        foreach ((ICharacter? winner, decimal amount) in payouts)
        {
            if (winner is null || amount <= 0)
            {
                continue;
            }

            int payoutType = (int)ArenaPayoutType.Bet;
            ArenaBetPayout? existing = context.ArenaBetPayouts.FirstOrDefault(x =>
                x.ArenaEventId == arenaEvent.Id &&
                x.CharacterId == winner.Id &&
                x.PayoutType == payoutType &&
                x.IsBlocked &&
                x.CollectedAt == null);
            if (existing is null)
            {
                context.ArenaBetPayouts.Add(new ArenaBetPayout
                {
                    ArenaEventId = arenaEvent.Id,
                    CharacterId = winner.Id,
                    Amount = amount,
                    PayoutType = payoutType,
                    IsBlocked = true,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                existing.Amount += amount;
            }
        }

        context.SaveChanges();
    }
    /// <inheritdoc />
    public void UnblockPayouts(ICombatArena arena)
    {
        if (arena is null)
        {
            throw new ArgumentNullException(nameof(arena));
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        List<ArenaBetPayout> blocked = context.ArenaBetPayouts
            .Where(x => x.IsBlocked && x.CollectedAt == null && x.ArenaEvent.ArenaId == arena.Id)
            .OrderBy(x => x.CreatedAt)
            .ToList();

        foreach (ArenaBetPayout? payout in blocked)
        {
            (bool Truth, string Reason) ensure = arena.EnsureFunds(payout.Amount);
            if (!ensure.Truth)
            {
                break;
            }

            (bool Truth, string Reason) solvency = IsSolvent(arena, payout.Amount);
            if (!solvency.Truth)
            {
                break;
            }

            arena.Debit(payout.Amount, $"Arena payout funding for event #{payout.ArenaEventId}");
            payout.IsBlocked = false;
        }

        context.SaveChanges();
    }

    /// <inheritdoc />
    public void AccrueAppearancePayouts(IArenaEvent arenaEvent)
    {
        if (arenaEvent is null)
        {
            throw new ArgumentNullException(nameof(arenaEvent));
        }

        if (arenaEvent.AppearanceFee <= 0.0m)
        {
            return;
        }

        int payoutType = (int)ArenaPayoutType.Appearance;
        List<IArenaParticipant> participants = arenaEvent.Participants
            .Where(x => x.Character is not null)
            .Where(x => !x.IsNpc || arenaEvent.PayNpcAppearanceFee)
            .GroupBy(x => x.Character!.Id)
            .Select(x => x.First())
            .ToList();
        if (participants.Count == 0)
        {
            return;
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        Dictionary<long, ArenaBetPayout> existingPayouts = context.ArenaBetPayouts
            .Where(x => x.ArenaEventId == arenaEvent.Id && x.PayoutType == payoutType)
            .ToList()
            .ToDictionary(x => x.CharacterId, x => x);

        foreach (IArenaParticipant? participant in participants)
        {
            ICharacter character = participant.Character!;
            if (existingPayouts.ContainsKey(character.Id))
            {
                continue;
            }

            decimal amount = arenaEvent.AppearanceFee;
            (bool Truth, string Reason) ensure = arenaEvent.Arena.EnsureFunds(amount);
            (bool Truth, string Reason) solvency = IsSolvent(arenaEvent.Arena, amount);
            bool blocked = !ensure.Truth || !solvency.Truth;
            context.ArenaBetPayouts.Add(new ArenaBetPayout
            {
                ArenaEventId = arenaEvent.Id,
                CharacterId = character.Id,
                Amount = amount,
                PayoutType = payoutType,
                IsBlocked = blocked,
                CreatedAt = DateTime.UtcNow,
                CollectedAt = null
            });

            if (!blocked)
            {
                arenaEvent.Arena.Debit(amount, $"Arena appearance payout accrual for event #{arenaEvent.Id}");
            }
        }

        context.SaveChanges();
    }

    /// <inheritdoc />
    public decimal GetUnclaimedMoney(ICombatArena arena)
    {
        if (arena is null)
        {
            throw new ArgumentNullException(nameof(arena));
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        return context.ArenaBetPayouts
            .Where(x => x.ArenaEvent.ArenaId == arena.Id)
            .Where(x => x.CollectedAt == null)
            .Where(x => !x.IsBlocked)
            .Where(x => x.PayoutType == (int)ArenaPayoutType.Bet || x.PayoutType == (int)ArenaPayoutType.Appearance)
            .Sum(x => (decimal?)x.Amount) ?? 0.0m;
    }

    private IDisposable? BeginContext(out FuturemudDatabaseContext context)
    {
        if (_contextFactory is not null)
        {
            context = _contextFactory();
            return null;
        }

        FMDB scope = new();
        context = FMDB.Context;
        return scope;
    }

    private static ArenaFinanceSnapshot GetOrCreateSnapshot(FuturemudDatabaseContext context, long arenaId, long? eventId, string period)
    {
        ArenaFinanceSnapshot? snapshot = context.ArenaFinanceSnapshots.FirstOrDefault(x => x.ArenaId == arenaId && x.ArenaEventId == eventId && x.Period == period);
        if (snapshot is null)
        {
            snapshot = new ArenaFinanceSnapshot
            {
                ArenaId = arenaId,
                ArenaEventId = eventId,
                Period = period,
                CreatedAt = DateTime.UtcNow
            };
            context.ArenaFinanceSnapshots.Add(snapshot);
        }

        return snapshot;
    }

    private static string CurrentPeriod()
    {
        return DateTime.UtcNow.ToString("yyyy-MM");
    }
}

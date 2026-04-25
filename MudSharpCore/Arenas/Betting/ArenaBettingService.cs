#nullable enable
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MudSharp.Arenas;

/// <summary>
/// Handles wager lifecycle for combat arena events.
/// </summary>
public class ArenaBettingService : IArenaBettingService
{
    private readonly IFuturemud _gameworld;
    private readonly IArenaFinanceService _financeService;
    private readonly IArenaBetPaymentService _paymentService;
    private readonly Func<FuturemudDatabaseContext>? _contextFactory;

    public ArenaBettingService(IFuturemud gameworld, IArenaFinanceService financeService,
    IArenaBetPaymentService? paymentService = null, Func<FuturemudDatabaseContext>? contextFactory = null)
    {
        _gameworld = gameworld ?? throw new ArgumentNullException(nameof(gameworld));
        _financeService = financeService ?? throw new ArgumentNullException(nameof(financeService));
        _paymentService = paymentService ?? new ArenaBetPaymentService();
        _contextFactory = contextFactory;
    }

    /// <inheritdoc />
    public (bool Truth, string Reason) CanBet(ICharacter actor, IArenaEvent arenaEvent)
    {
        if (actor is null)
        {
            throw new ArgumentNullException(nameof(actor));
        }

        if (arenaEvent is null)
        {
            throw new ArgumentNullException(nameof(arenaEvent));
        }

        switch (arenaEvent.State)
        {
            case ArenaEventState.RegistrationOpen:
            case ArenaEventState.Preparing:
            case ArenaEventState.Staged:
                break;
            default:
                return (false, "Betting is closed for this event.");
        }

        if (arenaEvent.Participants.Any(x => x.Character == actor))
        {
            return (false, "Participants cannot bet on their own bout.");
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        bool hasExisting = context.ArenaBets.Any(x => x.ArenaEventId == arenaEvent.Id && x.CharacterId == actor.Id && !x.IsCancelled);
        return hasExisting ? (false, "You already have an active wager on this event.") : (true, string.Empty);
    }

    /// <inheritdoc />
    public void PlaceBet(ICharacter actor, IArenaEvent arenaEvent, int? sideIndex, decimal stake)
    {
        if (actor is null)
        {
            throw new ArgumentNullException(nameof(actor));
        }

        if (arenaEvent is null)
        {
            throw new ArgumentNullException(nameof(arenaEvent));
        }

        if (stake <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stake));
        }

        (bool allowed, string? reason) = CanBet(actor, arenaEvent);
        if (!allowed)
        {
            throw new InvalidOperationException(reason);
        }

        (bool Success, string Error) stakeResult = _paymentService.CollectStake(actor, arenaEvent, stake);
        if (!stakeResult.Success)
        {
            throw new InvalidOperationException(stakeResult.Error);
        }

        BettingModel bettingModel = arenaEvent.EventType.BettingModel;
        if (sideIndex.HasValue && arenaEvent.EventType.Sides.All(x => x.Index != sideIndex.Value))
        {
            throw new ArgumentOutOfRangeException(nameof(sideIndex));
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        DateTime now = DateTime.UtcNow;
        ArenaBet bet = new()
        {
            ArenaEventId = arenaEvent.Id,
            CharacterId = actor.Id,
            SideIndex = sideIndex,
            Stake = stake,
            PlacedAt = now,
            IsCancelled = false,
            ModelSnapshot = string.Empty
        };

        switch (bettingModel)
        {
            case BettingModel.FixedOdds:
                decimal odds = ComputeFixedOdds(arenaEvent, sideIndex);
                bet.FixedDecimalOdds = odds;
                bet.ModelSnapshot = JsonSerializer.Serialize(new
                {
                    Model = "FixedOdds",
                    Odds = odds,
                    CapturedAt = now
                });
                break;
            case BettingModel.PariMutuel:
                ArenaBetPool pool = GetOrCreatePool(context, arenaEvent.Id, sideIndex);
                pool.TotalStake += stake;
                bet.FixedDecimalOdds = null;
                bet.ModelSnapshot = JsonSerializer.Serialize(new
                {
                    Model = "PariMutuel",
                    PoolStake = pool.TotalStake,
                    pool.TakeRate,
                    CapturedAt = now
                });
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(bettingModel));
        }

        context.ArenaBets.Add(bet);
        context.SaveChanges();

        arenaEvent.Arena.Credit(stake, $"Arena bet stake for event #{arenaEvent.Id}");
    }

    /// <inheritdoc />
    public void CancelBet(ICharacter actor, IArenaEvent arenaEvent)
    {
        if (actor is null)
        {
            throw new ArgumentNullException(nameof(actor));
        }

        if (arenaEvent is null)
        {
            throw new ArgumentNullException(nameof(arenaEvent));
        }

        if (arenaEvent.State >= ArenaEventState.Live)
        {
            throw new InvalidOperationException("Bets cannot be cancelled once the fight is live.");
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        ArenaBet? bet = context.ArenaBets.FirstOrDefault(x => x.ArenaEventId == arenaEvent.Id && x.CharacterId == actor.Id && !x.IsCancelled);
        if (bet is null)
        {
            return;
        }

        bet.IsCancelled = true;
        bet.CancelledAt = DateTime.UtcNow;

        if (arenaEvent.EventType.BettingModel == BettingModel.PariMutuel)
        {
            ArenaBetPool? pool = context.ArenaBetPools.FirstOrDefault(x => x.ArenaEventId == arenaEvent.Id && x.SideIndex == bet.SideIndex);
            pool?.TotalStake = Math.Max(0.0m, pool.TotalStake - bet.Stake);
        }

        context.SaveChanges();
        arenaEvent.Arena.Debit(bet.Stake, $"Arena bet refund for event #{arenaEvent.Id}");
        if (!_paymentService.TryDisburse(actor, arenaEvent, bet.Stake))
        {
            RecordOutstandingPayout(context, arenaEvent, bet.CharacterId, bet.Stake);
            string owedText = arenaEvent.Arena.Currency.Describe(bet.Stake, CurrencyDescriptionPatternType.ShortDecimal)
                                     .ColourValue();
            actor.OutputHandler.Send(
                $"The arena owes you {owedText} from your cancelled wager. A payout has been recorded for later collection."
                    .Colour(Telnet.Yellow));
        }
    }

    /// <inheritdoc />
    public void Settle(IArenaEvent arenaEvent, ArenaOutcome outcome, IEnumerable<int> winningSides)
    {
        if (arenaEvent is null)
        {
            throw new ArgumentNullException(nameof(arenaEvent));
        }

        List<int>? winningList = winningSides?.ToList();
        arenaEvent.RecordOutcome(outcome, winningList);

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        List<ArenaBet> activeBets = context.ArenaBets.Where(x => x.ArenaEventId == arenaEvent.Id && !x.IsCancelled).ToList();
        if (!activeBets.Any())
        {
            return;
        }

        HashSet<int> winningSet = winningList?.ToHashSet() ?? new HashSet<int>();
        List<ArenaBetPool> pools = context.ArenaBetPools.Where(x => x.ArenaEventId == arenaEvent.Id).ToList();
        List<(ICharacter Winner, decimal Amount, bool Online)> payouts = new();

        foreach (ArenaBet? bet in activeBets)
        {
            if (!IsWinningBet(bet, outcome, winningSet))
            {
                continue;
            }

            ICharacter? winner = _gameworld.TryGetCharacter(bet.CharacterId, true);
            if (winner is null)
            {
                continue;
            }

            decimal payout = CalculatePayout(arenaEvent, bet, pools);
            if (payout <= 0)
            {
                continue;
            }

            bool isOnline = _gameworld.Characters.Has(winner);
            payouts.Add((winner, payout, isOnline));
        }

        if (!payouts.Any())
        {
            return;
        }

        List<(ICharacter Winner, decimal Amount)> financePayouts = payouts.Select(x => (x.Winner, x.Amount)).ToList();
        decimal totalPayout = financePayouts.Sum(x => x.Amount);
        (bool Truth, string Reason) solvency = _financeService.IsSolvent(arenaEvent.Arena, totalPayout);
        if (!solvency.Truth)
        {
            _financeService.BlockPayout(arenaEvent.Arena, arenaEvent, financePayouts);
            return;
        }

        (bool Truth, string Reason) ensure = arenaEvent.Arena.EnsureFunds(totalPayout);
        if (!ensure.Truth)
        {
            _financeService.BlockPayout(arenaEvent.Arena, arenaEvent, financePayouts);
            return;
        }

        foreach ((ICharacter Winner, decimal Amount, bool Online) payout in payouts)
        {
            arenaEvent.Arena.Debit(payout.Amount, $"Arena payout for event #{arenaEvent.Id}");
            if (payout.Online && _paymentService.TryDisburse(payout.Winner, arenaEvent, payout.Amount))
            {
                string payoutText = arenaEvent.Arena.Currency
                                          .Describe(payout.Amount, CurrencyDescriptionPatternType.ShortDecimal)
                                          .ColourValue();
                payout.Winner.OutputHandler.Send(
                    $"You receive {payoutText} from betting on {arenaEvent.Name.ColourName()}.".Colour(Telnet.Green));
                ArenaBetPayout record = new()
                {
                    ArenaEventId = arenaEvent.Id,
                    CharacterId = payout.Winner.Id,
                    Amount = payout.Amount,
                    PayoutType = (int)ArenaPayoutType.Bet,
                    IsBlocked = false,
                    CreatedAt = DateTime.UtcNow,
                    CollectedAt = DateTime.UtcNow
                };
                context.ArenaBetPayouts.Add(record);
                continue;
            }

            RecordOutstandingPayout(context, arenaEvent, payout.Winner.Id, payout.Amount);
            if (payout.Online)
            {
                string owedText = arenaEvent.Arena.Currency
                                         .Describe(payout.Amount, CurrencyDescriptionPatternType.ShortDecimal)
                                         .ColourValue();
                payout.Winner.OutputHandler.Send(
                    $"The arena owes you {owedText} from betting on {arenaEvent.Name.ColourName()}. A payout has been recorded for later collection."
                        .Colour(Telnet.Yellow));
            }
        }

        foreach (ArenaBetPool? pool in pools)
        {
            pool.TotalStake = 0.0m;
        }

        context.SaveChanges();
    }

    /// <inheritdoc />
    public void RefundAll(IArenaEvent arenaEvent, string reason)
    {
        if (arenaEvent is null)
        {
            throw new ArgumentNullException(nameof(arenaEvent));
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        List<ArenaBet> bets = context.ArenaBets.Where(x => x.ArenaEventId == arenaEvent.Id && !x.IsCancelled).ToList();
        if (!bets.Any())
        {
            return;
        }

        foreach (ArenaBet? bet in bets)
        {
            bet.IsCancelled = true;
            bet.CancelledAt = DateTime.UtcNow;
            arenaEvent.Arena.Debit(bet.Stake, $"Arena bet refund ({reason}) for event #{arenaEvent.Id}");
            ICharacter? bettor = _gameworld.Characters.Get(bet.CharacterId);
            if (bettor is not null && _paymentService.TryDisburse(bettor, arenaEvent, bet.Stake))
            {
                string refundText = arenaEvent.Arena.Currency
                                           .Describe(bet.Stake, CurrencyDescriptionPatternType.ShortDecimal)
                                           .ColourValue();
                bettor.OutputHandler.Send(
                    $"Your stake of {refundText} has been refunded because {reason.Colour(Telnet.Yellow)} for {arenaEvent.Name.ColourName()}.");
                continue;
            }

            RecordOutstandingPayout(context, arenaEvent, bet.CharacterId, bet.Stake);
            if (bettor is not null)
            {
                string owedText = arenaEvent.Arena.Currency
                                         .Describe(bet.Stake, CurrencyDescriptionPatternType.ShortDecimal)
                                         .ColourValue();
                bettor.OutputHandler.Send(
                    $"The arena owes you {owedText} from {arenaEvent.Name.ColourName()} due to {reason.Colour(Telnet.Yellow)}. You can collect it later."
                        .Colour(Telnet.Yellow));
            }
        }

        List<ArenaBetPool> pools = context.ArenaBetPools.Where(x => x.ArenaEventId == arenaEvent.Id).ToList();
        foreach (ArenaBetPool? pool in pools)
        {
            pool.TotalStake = 0.0m;
        }

        context.SaveChanges();
    }

    /// <inheritdoc />
    public (decimal? FixedOdds, (decimal Pool, decimal TakeRate)? PariMutuel) GetQuote(IArenaEvent arenaEvent, int? sideIndex)
    {
        if (arenaEvent is null)
        {
            throw new ArgumentNullException(nameof(arenaEvent));
        }

        if (sideIndex.HasValue && arenaEvent.EventType.Sides.All(x => x.Index != sideIndex.Value))
        {
            throw new ArgumentOutOfRangeException(nameof(sideIndex));
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        return arenaEvent.EventType.BettingModel switch
        {
            BettingModel.FixedOdds => (ComputeFixedOdds(arenaEvent, sideIndex), null),
            BettingModel.PariMutuel => (null, GetPoolQuote(context, arenaEvent.Id, sideIndex)),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <inheritdoc />
    public IReadOnlyCollection<ArenaBetSummary> GetActiveBets(ICharacter actor)
    {
        if (actor is null)
        {
            throw new ArgumentNullException(nameof(actor));
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        Dictionary<long, PayoutTotals> payoutTotals = GetPayoutTotals(context, actor.Id);
        var bets = context.ArenaBets
                          .Where(x => x.CharacterId == actor.Id)
                          .Where(x => !x.IsCancelled)
                          .Where(x => x.ArenaEvent.State < (int)ArenaEventState.Resolving)
                          .OrderByDescending(x => x.PlacedAt)
                          .Select(x => new
                          {
                              Bet = x,
                              ArenaEvent = x.ArenaEvent,
                              ArenaName = x.ArenaEvent.Arena.Name,
                              EventTypeName = x.ArenaEvent.ArenaEventType.Name
                          })
                          .ToList();

        return bets.Select(x => BuildSummary(x.Bet, x.ArenaEvent, x.ArenaName, x.EventTypeName, payoutTotals)).ToList();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<ArenaBetSummary> GetBetHistory(ICharacter actor, int count)
    {
        if (actor is null)
        {
            throw new ArgumentNullException(nameof(actor));
        }

        if (count <= 0)
        {
            return Array.Empty<ArenaBetSummary>();
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        Dictionary<long, PayoutTotals> payoutTotals = GetPayoutTotals(context, actor.Id);
        var bets = context.ArenaBets
                          .Where(x => x.CharacterId == actor.Id)
                          .OrderByDescending(x => x.PlacedAt)
                          .Take(count)
                          .Select(x => new
                          {
                              Bet = x,
                              ArenaEvent = x.ArenaEvent,
                              ArenaName = x.ArenaEvent.Arena.Name,
                              EventTypeName = x.ArenaEvent.ArenaEventType.Name
                          })
                          .ToList();

        return bets.Select(x => BuildSummary(x.Bet, x.ArenaEvent, x.ArenaName, x.EventTypeName, payoutTotals)).ToList();
    }

    /// <inheritdoc />
    public IReadOnlyCollection<ArenaBetPayoutSummary> GetOutstandingPayouts(ICharacter actor)
    {
        if (actor is null)
        {
            throw new ArgumentNullException(nameof(actor));
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        var payouts = context.ArenaBetPayouts
                             .Where(x => x.CharacterId == actor.Id && x.CollectedAt == null)
                             .OrderBy(x => x.CreatedAt)
                             .Select(x => new
                             {
                                 x.ArenaEventId,
                                 ArenaId = x.ArenaEvent.ArenaId,
                                 ArenaName = x.ArenaEvent.Arena.Name,
                                 EventTypeName = x.ArenaEvent.ArenaEventType.Name,
                                 x.Amount,
                                 x.PayoutType,
                                 x.IsBlocked,
                                 x.CreatedAt
                             })
                             .ToList();

        return payouts.Select(x => new ArenaBetPayoutSummary(
                x.ArenaEventId,
                x.ArenaId,
                x.ArenaName,
                x.EventTypeName,
                x.Amount,
                Enum.IsDefined(typeof(ArenaPayoutType), x.PayoutType) ? (ArenaPayoutType)x.PayoutType : ArenaPayoutType.Bet,
                x.IsBlocked,
                x.CreatedAt))
                      .ToList();
    }

    /// <inheritdoc />
    public ArenaBetCollectionSummary CollectOutstandingPayouts(ICharacter actor, long? arenaEventId = null)
    {
        if (actor is null)
        {
            throw new ArgumentNullException(nameof(actor));
        }

        using IDisposable? scope = BeginContext(out FuturemudDatabaseContext? context);
        IQueryable<ArenaBetPayout> query = context.ArenaBetPayouts
                           .Where(x => x.CharacterId == actor.Id && x.CollectedAt == null);
        if (arenaEventId.HasValue)
        {
            query = query.Where(x => x.ArenaEventId == arenaEventId.Value);
        }

        var payouts = query
                      .Select(x => new
                      {
                          Payout = x,
                          ArenaId = x.ArenaEvent.ArenaId
                      })
                      .ToList();

        if (!payouts.Any())
        {
            return new ArenaBetCollectionSummary(0, 0, 0, 0.0m, 0.0m, 0.0m);
        }

        int collectedCount = 0;
        int failedCount = 0;
        int blockedCount = 0;
        decimal collectedAmount = 0.0m;
        decimal failedAmount = 0.0m;
        decimal blockedAmount = 0.0m;

        foreach (var entry in payouts)
        {
            ArenaBetPayout payout = entry.Payout;
            if (payout.IsBlocked)
            {
                blockedCount++;
                blockedAmount += payout.Amount;
                continue;
            }

            ICombatArena? arena = _gameworld.CombatArenas.Get(entry.ArenaId);
            if (arena is null)
            {
                failedCount++;
                failedAmount += payout.Amount;
                continue;
            }

            if (!_paymentService.TryDisburse(actor, arena, payout.Amount))
            {
                failedCount++;
                failedAmount += payout.Amount;
                continue;
            }

            payout.CollectedAt = DateTime.UtcNow;
            collectedCount++;
            collectedAmount += payout.Amount;
        }

        context.SaveChanges();

        return new ArenaBetCollectionSummary(collectedCount, failedCount, blockedCount, collectedAmount, failedAmount, blockedAmount);
    }

    private void RecordOutstandingPayout(FuturemudDatabaseContext context, IArenaEvent arenaEvent, long characterId, decimal amount)
    {
        if (amount <= 0)
        {
            return;
        }

        int payoutType = (int)ArenaPayoutType.Bet;
        ArenaBetPayout? existing = context.ArenaBetPayouts.FirstOrDefault(x =>
            x.ArenaEventId == arenaEvent.Id &&
            x.CharacterId == characterId &&
            x.PayoutType == payoutType &&
            !x.IsBlocked &&
            x.CollectedAt == null);
        if (existing is null)
        {
            context.ArenaBetPayouts.Add(new ArenaBetPayout
            {
                ArenaEventId = arenaEvent.Id,
                CharacterId = characterId,
                Amount = amount,
                PayoutType = payoutType,
                IsBlocked = false,
                CreatedAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.Amount += amount;
        }
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

    private static bool IsWinningBet(ArenaBet bet, ArenaOutcome outcome, IReadOnlySet<int> winningSides)
    {
        return outcome switch
        {
            ArenaOutcome.Win => bet.SideIndex.HasValue && winningSides.Contains(bet.SideIndex.Value),
            ArenaOutcome.Draw => !bet.SideIndex.HasValue,
            _ => false
        };
    }

    private decimal CalculatePayout(IArenaEvent arenaEvent, ArenaBet bet, IReadOnlyCollection<ArenaBetPool> pools)
    {
        switch (arenaEvent.EventType.BettingModel)
        {
            case BettingModel.FixedOdds:
                decimal odds = bet.FixedDecimalOdds ?? ComputeFixedOdds(arenaEvent, bet.SideIndex);
                return bet.Stake * odds;
            case BettingModel.PariMutuel:
                ArenaBetPool? pool = pools.FirstOrDefault(x => x.SideIndex == bet.SideIndex);
                if (pool is null || pool.TotalStake <= 0)
                {
                    return 0.0m;
                }

                decimal totalPool = pools.Sum(x => x.TotalStake);
                if (totalPool <= 0.0m)
                {
                    return 0.0m;
                }

                // Winners always get at least their original stake back.
                decimal losingPool = Math.Max(0.0m, totalPool - pool.TotalStake);
                decimal share = bet.Stake / pool.TotalStake;
                decimal takeRate = Clamp(pool.TakeRate, 0.0m, 1.0m);
                decimal distributableLosingPool = losingPool * (1.0m - takeRate);
                return bet.Stake + share * distributableLosingPool;
            default:
                return 0.0m;
        }
    }

    private decimal ComputeFixedOdds(IArenaEvent arenaEvent, int? sideIndex)
    {
        List<IArenaEventTypeSide> sides = arenaEvent.EventType.Sides.ToList();
        if (!sides.Any())
        {
            return 2.0m;
        }

        if (sides.Count == 1)
        {
            return sideIndex.HasValue ? 1.1m : 3.0m;
        }

        Dictionary<int, decimal> sideRatings = sides.ToDictionary(side => side.Index, side => ResolveSideRating(arenaEvent, side.Index));
        IReadOnlyDictionary<int, decimal> sideWinProbabilities = ComputeSideWinProbabilities(sideRatings);
        decimal drawProbability = ComputeDrawProbability(sideRatings);
        decimal sideWinShare = Clamp(1.0m - drawProbability, 0.05m, 0.97m);

        decimal probability;
        if (!sideIndex.HasValue)
        {
            probability = drawProbability;
        }
        else if (sideWinProbabilities.TryGetValue(sideIndex.Value, out decimal sideProbability))
        {
            probability = sideProbability * sideWinShare;
        }
        else
        {
            probability = sideWinShare / Math.Max(1, sides.Count);
        }

        probability = Clamp(probability, 0.02m, 0.95m);
        const decimal houseEdge = 0.05m;
        decimal odds = (1.0m - houseEdge) / probability;
        return Math.Round(Clamp(odds, 1.1m, 20.0m), 2, MidpointRounding.AwayFromZero);
    }

    private decimal ResolveSideRating(IArenaEvent arenaEvent, int sideIndex)
    {
        List<decimal> ratings = arenaEvent.Participants
            .Where(x => x.SideIndex == sideIndex)
            .Select(ResolveParticipantRating)
            .Where(x => x.HasValue)
            .Select(x => x.GetValueOrDefault())
            .ToList();
        if (ratings.Count == 0)
        {
            return ArenaRatingsService.DefaultRating;
        }

        return ratings.Average();
    }

    private decimal? ResolveParticipantRating(IArenaParticipant participant)
    {
        if (participant.StartingRating.HasValue)
        {
            return participant.StartingRating.Value;
        }

        return participant.Character is null
            ? null
            : _gameworld.ArenaRatingsService.GetRating(participant.Character, participant.CombatantClass);
    }

    private static IReadOnlyDictionary<int, decimal> ComputeSideWinProbabilities(
        IReadOnlyDictionary<int, decimal> sideRatings)
    {
        if (sideRatings.Count == 0)
        {
            return new Dictionary<int, decimal>();
        }

        Dictionary<int, double> strengths = sideRatings.ToDictionary(
            x => x.Key,
            x => Math.Pow(10.0, (double)(x.Value / 400.0m)));
        double totalStrength = strengths.Sum(x => x.Value);
        if (totalStrength <= 0.0)
        {
            decimal equalShare = 1.0m / sideRatings.Count;
            return sideRatings.Keys.ToDictionary(x => x, _ => equalShare);
        }

        return strengths.ToDictionary(x => x.Key, x => (decimal)(x.Value / totalStrength));
    }

    private static decimal ComputeDrawProbability(IReadOnlyDictionary<int, decimal> sideRatings)
    {
        if (sideRatings.Count < 2)
        {
            return 0.0m;
        }

        decimal spread = sideRatings.Values.Max() - sideRatings.Values.Min();
        decimal baseline = sideRatings.Count == 2 ? 0.18m : 0.10m;
        decimal reduction = spread / 4000.0m;
        decimal minimum = sideRatings.Count == 2 ? 0.03m : 0.01m;
        decimal maximum = sideRatings.Count == 2 ? 0.25m : 0.15m;
        return Clamp(baseline - reduction, minimum, maximum);
    }

    private static (decimal Pool, decimal TakeRate)? GetPoolQuote(FuturemudDatabaseContext context, long eventId, int? sideIndex)
    {
        ArenaBetPool? pool = context.ArenaBetPools.FirstOrDefault(x => x.ArenaEventId == eventId && x.SideIndex == sideIndex);
        return pool is null ? null : (pool.TotalStake, pool.TakeRate);
    }

    private ArenaBetPool GetOrCreatePool(FuturemudDatabaseContext context, long eventId, int? sideIndex)
    {
        ArenaBetPool? pool = context.ArenaBetPools.FirstOrDefault(x => x.ArenaEventId == eventId && x.SideIndex == sideIndex);
        if (pool is not null)
        {
            return pool;
        }

        pool = new ArenaBetPool
        {
            ArenaEventId = eventId,
            SideIndex = sideIndex,
            TakeRate = ResolveTakeRate(),
            TotalStake = 0.0m
        };
        context.ArenaBetPools.Add(pool);
        return pool;
    }

    private static decimal ResolveTakeRate()
    {
        return 0.10m;
    }

    private static decimal Clamp(decimal value, decimal min, decimal max)
    {
        if (value < min)
        {
            return min;
        }

        return value > max ? max : value;
    }

    private static Dictionary<long, PayoutTotals> GetPayoutTotals(FuturemudDatabaseContext context, long characterId)
    {
        return context.ArenaBetPayouts
                          .Where(x => x.CharacterId == characterId)
                          .Where(x => x.PayoutType == (int)ArenaPayoutType.Bet)
                          .GroupBy(x => x.ArenaEventId)
                      .Select(group => new
                      {
                          EventId = group.Key,
                          Collected = group.Where(x => x.CollectedAt != null).Sum(x => (decimal?)x.Amount) ?? 0.0m,
                          Outstanding = group.Where(x => !x.IsBlocked && x.CollectedAt == null).Sum(x => (decimal?)x.Amount) ?? 0.0m,
                          Blocked = group.Where(x => x.IsBlocked && x.CollectedAt == null).Sum(x => (decimal?)x.Amount) ?? 0.0m
                      })
                      .ToDictionary(x => x.EventId, x => new PayoutTotals(x.Collected, x.Outstanding, x.Blocked));
    }

    private static ArenaBetSummary BuildSummary(ArenaBet bet, MudSharp.Models.ArenaEvent arenaEvent, string arenaName,
        string eventTypeName, IReadOnlyDictionary<long, PayoutTotals> payoutTotals)
    {
        PayoutTotals totals = payoutTotals.TryGetValue(bet.ArenaEventId, out PayoutTotals? value)
            ? value
            : new PayoutTotals(0.0m, 0.0m, 0.0m);

        return new ArenaBetSummary(
            bet.Id,
            bet.ArenaEventId,
            arenaEvent.ArenaId,
            arenaName,
            eventTypeName,
            arenaEvent.ScheduledAt,
            bet.SideIndex,
            bet.Stake,
            bet.FixedDecimalOdds,
            (BettingModel)arenaEvent.BettingModel,
            bet.PlacedAt,
            bet.IsCancelled,
            bet.CancelledAt,
            (ArenaEventState)arenaEvent.State,
            totals.Collected,
            totals.Outstanding,
            totals.Blocked);
    }

    private sealed record PayoutTotals(decimal Collected, decimal Outstanding, decimal Blocked);
}

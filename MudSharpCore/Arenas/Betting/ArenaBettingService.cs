#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Models;

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

		using var scope = BeginContext(out var context);
		var hasExisting = context.ArenaBets.Any(x => x.ArenaEventId == arenaEvent.Id && x.CharacterId == actor.Id && !x.IsCancelled);
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

		var (allowed, reason) = CanBet(actor, arenaEvent);
		if (!allowed)
		{
			throw new InvalidOperationException(reason);
		}

		var stakeResult = _paymentService.CollectStake(actor, arenaEvent, stake);
		if (!stakeResult.Success)
		{
			throw new InvalidOperationException(stakeResult.Error);
		}

		var bettingModel = arenaEvent.EventType.BettingModel;
		if (sideIndex.HasValue && arenaEvent.EventType.Sides.All(x => x.Index != sideIndex.Value))
		{
			throw new ArgumentOutOfRangeException(nameof(sideIndex));
		}

		using var scope = BeginContext(out var context);
		var now = DateTime.UtcNow;
		var bet = new ArenaBet
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
			var odds = ComputeFixedOdds(arenaEvent, sideIndex);
			bet.FixedDecimalOdds = odds;
			bet.ModelSnapshot = JsonSerializer.Serialize(new
			{
				Model = "FixedOdds",
				Odds = odds,
				CapturedAt = now
			});
			break;
			case BettingModel.PariMutuel:
			var pool = GetOrCreatePool(context, arenaEvent.Id, sideIndex);
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

		using var scope = BeginContext(out var context);
		var bet = context.ArenaBets.FirstOrDefault(x => x.ArenaEventId == arenaEvent.Id && x.CharacterId == actor.Id && !x.IsCancelled);
		if (bet is null)
		{
			return;
		}

		bet.IsCancelled = true;
		bet.CancelledAt = DateTime.UtcNow;

		if (arenaEvent.EventType.BettingModel == BettingModel.PariMutuel)
		{
			var pool = context.ArenaBetPools.FirstOrDefault(x => x.ArenaEventId == arenaEvent.Id && x.SideIndex == bet.SideIndex);
			if (pool is not null)
			{
				pool.TotalStake = Math.Max(0.0m, pool.TotalStake - bet.Stake);
			}
		}

		context.SaveChanges();
		arenaEvent.Arena.Debit(bet.Stake, $"Arena bet refund for event #{arenaEvent.Id}");
		if (!_paymentService.TryDisburse(actor, arenaEvent, bet.Stake))
		{
			RecordOutstandingPayout(context, arenaEvent, bet.CharacterId, bet.Stake);
			var owedText = arenaEvent.Arena.Currency.Describe(bet.Stake, CurrencyDescriptionPatternType.ShortDecimal)
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

		var winningList = winningSides?.ToList();
		arenaEvent.RecordOutcome(outcome, winningList);

		using var scope = BeginContext(out var context);
		var activeBets = context.ArenaBets.Where(x => x.ArenaEventId == arenaEvent.Id && !x.IsCancelled).ToList();
		if (!activeBets.Any())
		{
			return;
		}

		var winningSet = winningList?.ToHashSet() ?? new HashSet<int>();
		var pools = context.ArenaBetPools.Where(x => x.ArenaEventId == arenaEvent.Id).ToList();
		var payouts = new List<(ICharacter Winner, decimal Amount, bool Online)>();

		foreach (var bet in activeBets)
		{
			if (!IsWinningBet(bet, outcome, winningSet))
			{
				continue;
			}

			var winner = _gameworld.TryGetCharacter(bet.CharacterId, true);
			if (winner is null)
			{
				continue;
			}

			var payout = CalculatePayout(arenaEvent, bet, pools);
			if (payout <= 0)
			{
				continue;
			}

			var isOnline = _gameworld.Characters.Has(winner);
			payouts.Add((winner, payout, isOnline));
		}

		if (!payouts.Any())
		{
			return;
		}

		var financePayouts = payouts.Select(x => (x.Winner, x.Amount)).ToList();
		var totalPayout = financePayouts.Sum(x => x.Amount);
		var solvency = _financeService.IsSolvent(arenaEvent.Arena, totalPayout);
		if (!solvency.Truth)
		{
			_financeService.BlockPayout(arenaEvent.Arena, arenaEvent, financePayouts);
			return;
		}

		var ensure = arenaEvent.Arena.EnsureFunds(totalPayout);
		if (!ensure.Truth)
		{
			_financeService.BlockPayout(arenaEvent.Arena, arenaEvent, financePayouts);
			return;
		}

		foreach (var payout in payouts)
		{
			arenaEvent.Arena.Debit(payout.Amount, $"Arena payout for event #{arenaEvent.Id}");
			if (payout.Online && _paymentService.TryDisburse(payout.Winner, arenaEvent, payout.Amount))
			{
				var payoutText = arenaEvent.Arena.Currency
				                          .Describe(payout.Amount, CurrencyDescriptionPatternType.ShortDecimal)
				                          .ColourValue();
				payout.Winner.OutputHandler.Send(
					$"You receive {payoutText} from betting on {arenaEvent.Name.ColourName()}.".Colour(Telnet.Green));
					var record = new ArenaBetPayout
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
				var owedText = arenaEvent.Arena.Currency
				                         .Describe(payout.Amount, CurrencyDescriptionPatternType.ShortDecimal)
				                         .ColourValue();
				payout.Winner.OutputHandler.Send(
					$"The arena owes you {owedText} from betting on {arenaEvent.Name.ColourName()}. A payout has been recorded for later collection."
						.Colour(Telnet.Yellow));
			}
		}

		foreach (var pool in pools)
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

		using var scope = BeginContext(out var context);
		var bets = context.ArenaBets.Where(x => x.ArenaEventId == arenaEvent.Id && !x.IsCancelled).ToList();
		if (!bets.Any())
		{
			return;
		}

		foreach (var bet in bets)
		{
			bet.IsCancelled = true;
			bet.CancelledAt = DateTime.UtcNow;
			arenaEvent.Arena.Debit(bet.Stake, $"Arena bet refund ({reason}) for event #{arenaEvent.Id}");
			var bettor = _gameworld.Characters.Get(bet.CharacterId);
			if (bettor is not null && _paymentService.TryDisburse(bettor, arenaEvent, bet.Stake))
			{
				var refundText = arenaEvent.Arena.Currency
				                           .Describe(bet.Stake, CurrencyDescriptionPatternType.ShortDecimal)
				                           .ColourValue();
				bettor.OutputHandler.Send(
					$"Your stake of {refundText} has been refunded because {reason.Colour(Telnet.Yellow)} for {arenaEvent.Name.ColourName()}.");
				continue;
			}

			RecordOutstandingPayout(context, arenaEvent, bet.CharacterId, bet.Stake);
			if (bettor is not null)
			{
				var owedText = arenaEvent.Arena.Currency
				                         .Describe(bet.Stake, CurrencyDescriptionPatternType.ShortDecimal)
				                         .ColourValue();
				bettor.OutputHandler.Send(
					$"The arena owes you {owedText} from {arenaEvent.Name.ColourName()} due to {reason.Colour(Telnet.Yellow)}. You can collect it later."
						.Colour(Telnet.Yellow));
			}
		}

		var pools = context.ArenaBetPools.Where(x => x.ArenaEventId == arenaEvent.Id).ToList();
		foreach (var pool in pools)
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

		using var scope = BeginContext(out var context);
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

		using var scope = BeginContext(out var context);
		var payoutTotals = GetPayoutTotals(context, actor.Id);
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

		using var scope = BeginContext(out var context);
		var payoutTotals = GetPayoutTotals(context, actor.Id);
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

		using var scope = BeginContext(out var context);
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

		using var scope = BeginContext(out var context);
		var query = context.ArenaBetPayouts
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

		var collectedCount = 0;
		var failedCount = 0;
		var blockedCount = 0;
		var collectedAmount = 0.0m;
		var failedAmount = 0.0m;
		var blockedAmount = 0.0m;

		foreach (var entry in payouts)
		{
			var payout = entry.Payout;
			if (payout.IsBlocked)
			{
				blockedCount++;
				blockedAmount += payout.Amount;
				continue;
			}

			var arena = _gameworld.CombatArenas.Get(entry.ArenaId);
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

		var payoutType = (int)ArenaPayoutType.Bet;
		var existing = context.ArenaBetPayouts.FirstOrDefault(x =>
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

		var scope = new FMDB();
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
			var odds = bet.FixedDecimalOdds ?? ComputeFixedOdds(arenaEvent, bet.SideIndex);
			return bet.Stake * odds;
			case BettingModel.PariMutuel:
			var pool = pools.FirstOrDefault(x => x.SideIndex == bet.SideIndex);
			if (pool is null || pool.TotalStake <= 0)
			{
				return 0.0m;
			}

                        var totalPool = pools.Sum(x => x.TotalStake);
                        if (totalPool <= 0.0m)
                        {
                                return 0.0m;
                        }

                        var netPool = totalPool * (1.0m - pool.TakeRate);
                        return bet.Stake / pool.TotalStake * netPool;
			default:
			return 0.0m;
		}
	}

	private decimal ComputeFixedOdds(IArenaEvent arenaEvent, int? sideIndex)
	{
		var sides = arenaEvent.EventType.Sides.ToList();
		if (!sides.Any())
		{
			return 2.0m;
		}

		if (sides.Count == 1)
		{
			return sideIndex.HasValue ? 1.1m : 3.0m;
		}

		var sideRatings = sides.ToDictionary(side => side.Index, side => ResolveSideRating(arenaEvent, side.Index));
		var sideWinProbabilities = ComputeSideWinProbabilities(sideRatings);
		var drawProbability = ComputeDrawProbability(sideRatings);
		var sideWinShare = Clamp(1.0m - drawProbability, 0.05m, 0.97m);

		decimal probability;
		if (!sideIndex.HasValue)
		{
			probability = drawProbability;
		}
		else if (sideWinProbabilities.TryGetValue(sideIndex.Value, out var sideProbability))
		{
			probability = sideProbability * sideWinShare;
		}
		else
		{
			probability = sideWinShare / Math.Max(1, sides.Count);
		}

		probability = Clamp(probability, 0.02m, 0.95m);
		const decimal houseEdge = 0.05m;
		var odds = (1.0m - houseEdge) / probability;
		return Math.Round(Clamp(odds, 1.1m, 20.0m), 2, MidpointRounding.AwayFromZero);
	}

	private decimal ResolveSideRating(IArenaEvent arenaEvent, int sideIndex)
	{
		var ratings = arenaEvent.Participants
			.Where(x => x.SideIndex == sideIndex)
			.Select(ResolveParticipantRating)
			.Where(x => x.HasValue)
			.Select(x => x.Value)
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

		var strengths = sideRatings.ToDictionary(
			x => x.Key,
			x => Math.Pow(10.0, (double)(x.Value / 400.0m)));
		var totalStrength = strengths.Sum(x => x.Value);
		if (totalStrength <= 0.0)
		{
			var equalShare = 1.0m / sideRatings.Count;
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

		var spread = sideRatings.Values.Max() - sideRatings.Values.Min();
		var baseline = sideRatings.Count == 2 ? 0.18m : 0.10m;
		var reduction = spread / 4000.0m;
		var minimum = sideRatings.Count == 2 ? 0.03m : 0.01m;
		var maximum = sideRatings.Count == 2 ? 0.25m : 0.15m;
		return Clamp(baseline - reduction, minimum, maximum);
	}

	private static (decimal Pool, decimal TakeRate)? GetPoolQuote(FuturemudDatabaseContext context, long eventId, int? sideIndex)
	{
		var pool = context.ArenaBetPools.FirstOrDefault(x => x.ArenaEventId == eventId && x.SideIndex == sideIndex);
                return pool is null ? null : (pool.TotalStake, pool.TakeRate);
	}

	private ArenaBetPool GetOrCreatePool(FuturemudDatabaseContext context, long eventId, int? sideIndex)
	{
		var pool = context.ArenaBetPools.FirstOrDefault(x => x.ArenaEventId == eventId && x.SideIndex == sideIndex);
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
		var totals = payoutTotals.TryGetValue(bet.ArenaEventId, out var value)
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

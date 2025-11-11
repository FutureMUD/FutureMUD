#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Database;
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
	private readonly Func<FuturemudDatabaseContext>? _contextFactory;

	public ArenaBettingService(IFuturemud gameworld, IArenaFinanceService financeService,
	Func<FuturemudDatabaseContext>? contextFactory = null)
	{
		_gameworld = gameworld ?? throw new ArgumentNullException(nameof(gameworld));
		_financeService = financeService ?? throw new ArgumentNullException(nameof(financeService));
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
	}

	/// <inheritdoc />
	public void Settle(IArenaEvent arenaEvent, ArenaOutcome outcome, IEnumerable<int> winningSides)
	{
		if (arenaEvent is null)
		{
			throw new ArgumentNullException(nameof(arenaEvent));
		}

		using var scope = BeginContext(out var context);
		var activeBets = context.ArenaBets.Where(x => x.ArenaEventId == arenaEvent.Id && !x.IsCancelled).ToList();
		if (!activeBets.Any())
		{
			return;
		}

		var winningSet = winningSides?.ToHashSet() ?? new HashSet<int>();
		var pools = context.ArenaBetPools.Where(x => x.ArenaEventId == arenaEvent.Id).ToList();
		var payouts = new List<(ICharacter Winner, decimal Amount)>();

		foreach (var bet in activeBets)
		{
			if (!IsWinningBet(bet, outcome, winningSet))
			{
				continue;
			}

			var winner = _gameworld.Characters.Get(bet.CharacterId);
			if (winner is null)
			{
				continue;
			}

			var payout = CalculatePayout(arenaEvent, bet, pools);
			if (payout <= 0)
			{
				continue;
			}

			payouts.Add((winner, payout));
		}

		if (!payouts.Any())
		{
			return;
		}

		var totalPayout = payouts.Sum(x => x.Amount);
		var solvency = _financeService.IsSolvent(arenaEvent.Arena, totalPayout);
		if (!solvency.Truth)
		{
			_financeService.BlockPayout(arenaEvent.Arena, arenaEvent, payouts);
			return;
		}

		var ensure = arenaEvent.Arena.EnsureFunds(totalPayout);
		if (!ensure.Truth)
		{
			_financeService.BlockPayout(arenaEvent.Arena, arenaEvent, payouts);
			return;
		}

		foreach (var payout in payouts)
		{
			arenaEvent.Arena.Debit(payout.Amount, $"Arena payout for event #{arenaEvent.Id}");
			var record = new ArenaBetPayout
			{
				ArenaEventId = arenaEvent.Id,
				CharacterId = payout.Winner.Id,
				Amount = payout.Amount,
				IsBlocked = false,
				CreatedAt = DateTime.UtcNow,
				CollectedAt = DateTime.UtcNow
			};
			context.ArenaBetPayouts.Add(record);
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

	private static decimal ComputeFixedOdds(IArenaEvent arenaEvent, int? sideIndex)
	{
		var sides = arenaEvent.EventType.Sides.ToList();
		if (!sides.Any())
		{
			return 2.0m;
		}

		if (!sideIndex.HasValue)
		{
			return 3.0m;
		}

		var sideParticipants = arenaEvent.Participants.Count(x => x.SideIndex == sideIndex.Value);
		if (sideParticipants <= 0)
		{
			sideParticipants = 1;
		}

		var totalParticipants = sides.Sum(x => Math.Max(1, arenaEvent.Participants.Count(y => y.SideIndex == x.Index)));
		if (totalParticipants <= 0)
		{
			totalParticipants = sides.Count;
		}

		var probability = (decimal)sideParticipants / totalParticipants;
		probability = Clamp(probability, 0.05m, 0.95m);
		const decimal houseEdge = 0.05m;
		var odds = (1.0m - houseEdge) / probability;
		return Math.Round(Clamp(odds, 1.1m, 10.0m), 2, MidpointRounding.AwayFromZero);
	}

	private static (decimal Pool, decimal TakeRate)? GetPoolQuote(FuturemudDatabaseContext context, long eventId, int? sideIndex)
	{
		var pool = context.ArenaBetPools.FirstOrDefault(x => x.ArenaEventId == eventId && x.SideIndex == sideIndex);
		return pool is null ? (decimal Pool, decimal TakeRate)?null : (pool.TotalStake, pool.TakeRate);
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
}

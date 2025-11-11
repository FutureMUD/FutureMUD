#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Arenas;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Models;

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

		using var scope = BeginContext(out var context);
		var outstanding = context.ArenaBetPayouts
		.Where(x => x.IsBlocked && x.CollectedAt == null && x.ArenaEvent.ArenaId == arena.Id)
		.Sum(x => (decimal?)x.Amount) ?? 0.0m;
		var available = arena.AvailableFunds();
		var net = available - outstanding;
		if (net >= required)
		{
			return (true, string.Empty);
		}

		var shortage = required - net;
		var message = $"Arena requires {required:n2} but is short {shortage:n2} after liabilities.";
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

		using var scope = BeginContext(out var context);
		var snapshot = GetOrCreateSnapshot(context, arena.Id, null, CurrentPeriod());
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

		using var scope = BeginContext(out var context);
		var snapshot = GetOrCreateSnapshot(context, arena.Id, null, CurrentPeriod());
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

		using var scope = BeginContext(out var context);
		foreach (var (winner, amount) in payouts)
		{
			if (winner is null || amount <= 0)
			{
				continue;
			}

			var existing = context.ArenaBetPayouts.FirstOrDefault(x => x.ArenaEventId == arenaEvent.Id && x.CharacterId == winner.Id && x.IsBlocked && x.CollectedAt == null);
			if (existing is null)
			{
				context.ArenaBetPayouts.Add(new ArenaBetPayout
				{
					ArenaEventId = arenaEvent.Id,
					CharacterId = winner.Id,
					Amount = amount,
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

		using var scope = BeginContext(out var context);
		var blocked = context.ArenaBetPayouts
		.Where(x => x.IsBlocked && x.CollectedAt == null && x.ArenaEvent.ArenaId == arena.Id)
		.OrderBy(x => x.CreatedAt)
		.ToList();

		foreach (var payout in blocked)
		{
			var ensure = arena.EnsureFunds(payout.Amount);
			if (!ensure.Truth)
			{
				break;
			}

			arena.Debit(payout.Amount, $"Arena payout for event #{payout.ArenaEventId}");
			payout.IsBlocked = false;
			payout.CollectedAt = DateTime.UtcNow;
		}

		context.SaveChanges();
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

	private static ArenaFinanceSnapshot GetOrCreateSnapshot(FuturemudDatabaseContext context, long arenaId, long? eventId, string period)
	{
		var snapshot = context.ArenaFinanceSnapshots.FirstOrDefault(x => x.ArenaId == arenaId && x.ArenaEventId == eventId && x.Period == period);
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

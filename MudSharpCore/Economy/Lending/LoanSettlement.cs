#nullable enable
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Server;

namespace MudSharp.Economy.Lending;

internal static partial class LoanService
{
	// Lending commands and the save queue both run synchronously on the game thread.
	private sealed class Settlement
	{
		internal Stack<Action> Undo { get; } = new();
		internal List<Action> Committed { get; } = new();
		internal (long ContractId, string OperationKey)? Receipt { get; set; }
	}

	[ThreadStatic] private static Settlement? _settlement;
	internal static bool GuardingSettlement => _settlement != null;
	internal static void OnRollback(Action action) => _settlement?.Undo.Push(action);
	internal static void AfterCommit(Action action)
	{
		if (_settlement == null) action();
		else _settlement.Committed.Add(action);
	}

	private static string GuardedSettlement(IFuturemud world, Func<string> action)
	{
		using var caller = new FMDB();
		var relational = FMDB.Context.Database.IsRelational();
		if (relational && FMDB.Context.Database.CurrentTransaction != null)
			return "Finish the current transaction before attempting this settlement.";

		world.SaveManager.Flush();
		var settlement = new Settlement();
		var commitSent = false;
		string result = string.Empty;
		_settlement = settlement;
		try
		{
			using var independent = relational ? FMDB.BeginIndependentScope(requireWrites: true) : null;
			using var database = new FMDB();
			var db = FMDB.Context;
			using var transaction = relational ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			try
			{
				result = action();
				world.SaveManager.Flush();
				db.SaveChanges();
				commitSent = transaction != null;
				transaction?.Commit();
			}
			catch
			{
				// Native saves can call SaveChanges several times. Never reuse their tracked loan,
				// title or account rows after the database has discarded the transaction.
				db.ChangeTracker.Clear();
				throw;
			}
		}
		catch (Exception failure)
		{
			_settlement = null;
			if (!commitSent || settlement.Receipt is not { } receipt || !ReceiptCommitted(receipt))
			{
				foreach (var undo in settlement.Undo) undo();
				if (!IsDatabaseFailure(failure)) throw;
				ExceptionReporter.WriteFallback(null, ExceptionReporter.Format(failure, "Lending settlement rolled back", false));
				return "The settlement could not be completed. Your payment was not taken. You can try again.";
			}
		}
		finally { _settlement = null; }

		// Observers and removal of empty piles can run game code. The financial transaction
		// is already durable, even if a notification or cosmetic cleanup fails.
		foreach (var publish in settlement.Committed)
		{
			try { publish(); }
			catch (Exception error)
			{
				ExceptionReporter.WriteFallback(null, ExceptionReporter.Format(error, "Committed lending settlement notification", false));
			}
		}
		return result;
	}

	private static bool IsDatabaseFailure(Exception failure)
	{
		for (Exception? cause = failure; cause != null; cause = cause.InnerException)
			if (cause is DbException or DbUpdateException or TimeoutException or System.IO.IOException or System.Net.Sockets.SocketException) return true;
		return false;
	}

	private static bool ReceiptCommitted((long ContractId, string OperationKey) receipt)
	{
		// A lost COMMIT reply can mean either outcome. A locking read on the unique
		// posting waits for the writer to finish even if its original connection was lost.
		// While the authoritative database is unreachable the game thread must not
		// run a save or let either party spend an indeterminate balance.
		return VerifyCommit(() =>
		{
			using var independent = FMDB.BeginIndependentScope(requireWrites: true);
			using var handle = new FMDB();
			var db = FMDB.Context;
			using var tx = db.Database.BeginTransaction(IsolationLevel.Serializable);
			return db.Database.SqlQuery<long>($"""
				SELECT Id FROM LendingPostings
				WHERE ContractId = {receipt.ContractId} AND OperationKey = {receipt.OperationKey}
				FOR UPDATE
				""").ToList().Count != 0;
		}, () => System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2)));
	}

	internal static bool VerifyCommit(Func<bool> readReceipt, Action waitForDatabase)
	{
		var logged = false;
		while (true)
		{
			try { return readReceipt(); }
			catch (Exception error) when (IsDatabaseFailure(error))
			{
				if (!logged)
					ExceptionReporter.WriteFallback(null, ExceptionReporter.Format(error, "Waiting to verify lending commit", false));
				logged = true;
				waitForDatabase();
			}
		}
	}

	internal static void AdjustReserves(IBank bank, ICurrency currency, decimal amount)
	{
		var previous = bank.CurrencyReserves[currency];
		if (GuardingSettlement) OnRollback(() => bank.CurrencyReserves[currency] = previous);
		bank.CurrencyReserves[currency] = previous + amount;
		bank.Changed = true;
	}

	private static void TakeSettlementCash(ICharacter actor, ICurrency currency, decimal amount)
	{
		var available = actor.Body.ExternalItems.RecursiveGetItems<ICurrencyPile>(true).ToList();
		var selection = currency.FindCurrency(available, amount);
		var tender = selection.Sum(x => x.Value.Sum(coin => coin.Key.Value * coin.Value));
		var change = currency.FindCoinsForAmount(tender - amount, out var exact);
		if (selection.Count == 0 || tender < amount || tender > amount && !exact)
			throw new InvalidOperationException("The borrower can no longer tender the quoted payment.");

		// Prefer the borrower's own tendered pile. A borrowed purse may fund a payment,
		// but the change belongs to the borrower.
		var returnPile = tender == amount ? null : selection.Keys.Concat(available).FirstOrDefault(x =>
			x.Currency == currency && x.Parent.IsOwnedBy(actor) && !SpellOwnedItemValuePolicy.ContainsTemporaryValue(x.Parent));
		if (returnPile != null) selection.TryAdd(returnPile, new Dictionary<ICoin, int>());
		foreach (var (source, debit) in selection)
		{
			var pile = (CurrencyGameItemComponent)source;
			ForeignCustodyTransferContext.EnsureItem(pile.Parent, destructive: true);
			var before = pile.Coins.ToDictionary(x => x.Item1, x => x.Item2);
			var after = new Dictionary<ICoin, int>(before);
			foreach (var (coin, count) in debit)
			{
				after[coin] -= count;
				if (after[coin] == 0) after.Remove(coin);
			}
			if (source == returnPile)
				foreach (var (coin, count) in change)
					after[coin] = checked(after.GetValueOrDefault(coin) + count);

			OnRollback(() => pile.ReplacePreparedCoins(before));
			pile.ReplacePreparedCoins(after);
			pile.MarkPreparedCoinsChanged();
			AfterCommit(() =>
			{
				if (!pile.Coins.Any() && pile.Parent is GameItem item)
					item.DeleteCommittedEmptyCurrency(() => !pile.Coins.Any(), () => { });
				else pile.NotifyCurrencyTransferDebit();
			});
		}
		if (tender > amount && returnPile == null) PrepareBorrowerChange(actor, currency, change);
	}

	private static void PrepareBorrowerChange(ICharacter actor, ICurrency currency, Dictionary<ICoin, int> change)
	{
		// Give a new pile a durable place in the world before handing it over. A crash
		// between commit and pickup then leaves the borrower's change at the counter.
		var point = RouteSpatialService.Instance.GetEffectiveLocation(actor);
		var room = (Room)point.Room;
		var pile = (GameItem)CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency, change, temporary: true);
		pile.SetCurrencyPreviewOwner(actor);
		pile.SetPreparedCurrencyCustody(null, null, point);
		OnRollback(() =>
		{
			room.SetPreparedCurrencyRoomMembership(pile, false);
			actor.Gameworld.SaveManager.Abort(pile);
			foreach (var component in pile.Components) actor.Gameworld.SaveManager.Abort(component);
		});
		pile.ActivateCurrencySplit();
		room.SetPreparedCurrencyRoomMembership(pile, true);
		room.ContentsChanged = true;
		AfterCommit(() =>
		{
			actor.Gameworld.Add(pile);
			RouteSpatialService.Instance.TrackPerceivable(pile);
			pile.FinishCurrencySplitLoading(() => !pile.Deleted && !pile.Destroyed);
			if (pile.Deleted || pile.Destroyed || pile.DirectLocation != room) return;
			if (actor.Location == room && actor.Body.CanGet(pile, 0) && actor.Body.GetWithoutMerge(pile) != null) return;
			room.FinishPreparedCurrencyInsertion(pile, point);
			actor.OutputHandler.Send("Your change is on the ground at the counter.");
		});
	}
}

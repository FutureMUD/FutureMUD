#nullable enable

using System.Data;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

namespace MudSharp.Magic.Lifecycle;

public sealed class SpellMarkedRoomUseService(IFuturemud world) : ISpellMarkedRoomUseService
{
	public void Debit(long itemId, long bodyId, Guid parentIdentity, long parentSpellId, long roomId, int expectedUses, Func<bool> inputsCurrent)
	{
		void Confirm() { if (!inputsCurrent()) throw new InvalidOperationException("The exact prepared Arcane Mark changed."); }
		Confirm(); world.SaveManager.Flush(); Confirm();
		using var isolated = FMDB.BeginIndependentScope(requireWrites: true); using var db = new FMDB();
		using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable);
		Confirm(); SpellMarkedRoomDebit.Apply(FMDB.Context, itemId, bodyId, parentIdentity, parentSpellId, roomId, expectedUses);
		FMDB.Context.SaveChanges(); Confirm(); transaction.Commit();
		// Commit uncertainty propagates to the paid-operation journal; callers never retry the debit.
	}
}

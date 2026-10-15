#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.Magic.Lifecycle;

public sealed partial class SpellOwnedCreation
{
	/// <summary>Debit exactly one persisted marked-room use in the topology creation transaction.</summary>
	internal void DebitMarkedRoomUse(long itemId, long bodyId, Guid parentIdentity, long parentSpellId,
		long savedRoomId, int expectedUses)
	{
		if (_gate is null) throw new InvalidOperationException("The exact owned gate creation is required.");
		var item = SpellMarkedRoomDebit.Apply(Context, itemId, bodyId, parentIdentity, parentSpellId, savedRoomId, expectedUses);
		CaptureBorrowedChange(item);
	}
}

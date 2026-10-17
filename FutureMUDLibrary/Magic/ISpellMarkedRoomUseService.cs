#nullable enable

using System;

namespace MudSharp.Magic;

/// <summary>Commit one exact saved Arcane Mark use without owning the existing item or destination.</summary>
public interface ISpellMarkedRoomUseService
{
	void Debit(long itemId, long bodyId, Guid parentIdentity, long parentSpellId, long roomId, int expectedUses, Func<bool> inputsCurrent);
}

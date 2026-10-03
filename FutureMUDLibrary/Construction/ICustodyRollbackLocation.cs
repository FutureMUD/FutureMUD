#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.GameItems;

namespace MudSharp.Construction;

/// <summary>Captures exact item membership for a callback-free native custody rollback.</summary>
public interface ICustodyRollbackLocation
{
	Action CaptureCustodyMembershipRollback(IReadOnlyCollection<IGameItem> items);
}

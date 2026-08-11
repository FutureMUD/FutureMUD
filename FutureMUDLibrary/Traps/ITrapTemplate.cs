#nullable enable

using MudSharp.Framework.Revision;
using System;
using System.Collections.Generic;

namespace MudSharp.Traps;

/// <summary>
/// A revisable definition for a family of deployed traps.
/// </summary>
public interface ITrapTemplate : IEditableRevisableItem
{
	TrapSourceKind SourceKind { get; }
	IReadOnlyList<ITrapTrigger> Triggers { get; }
	IReadOnlyList<ITrapPayload> Payloads { get; }
	TrapDisarmPolicy DisarmPolicy { get; }
	TrapLifecyclePolicy LifecyclePolicy { get; }
	int Charges { get; }
	TimeSpan Cooldown { get; }
	TimeSpan? Lifespan { get; }
}

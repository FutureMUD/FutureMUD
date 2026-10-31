#nullable enable

using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Framework;
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
	TimeSpan SetupTime { get; }
	TimeSpan DisarmTime { get; }
	TimeSpan RecoveryTime { get; }
	IFutureProg? KnowledgeProg { get; }
	/// <summary>Optional replacement for the mundane setup check: character, anchor, matched items.</summary>
	IFutureProg? SetupProg { get; }
	/// <summary>Anchor to a closed door/container item, retaining held containers in inventory.</summary>
	bool ClosedItemAnchor { get; }
	/// <summary>Intrinsic traps are created by progs/spells and need no separately installed components.</summary>
	bool Preassembled { get; }
	IReadOnlyList<ITrapComponentRequirement> ComponentRequirements { get; }
}

public interface ITrapComponentRequirement
{
	long TagId { get; }
	ITag? Tag { get; }
	TrapComponentRole Role { get; }
	double SpentRecoveryChance { get; }
	double QualityWeight { get; }
}

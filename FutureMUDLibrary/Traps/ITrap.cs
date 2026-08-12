#nullable enable

using MudSharp.Effects;
using MudSharp.Character;
using MudSharp.GameItems;
using System.Collections.Generic;
using System;

namespace MudSharp.Traps;

/// <summary>
/// A deployed, durable trap. Live traps are represented by saving effects rather than separate world entities.
/// </summary>
public interface ITrap : IEffect
{
	Guid InstanceId { get; }
	TrapState State { get; }
	TrapSourceKind SourceKind { get; }
	long TemplateId { get; }
	int TemplateRevisionNumber { get; }
	long CreatorId { get; }
	int RemainingCharges { get; }
	long? BoundExitId { get; }
	long? BoundExitOriginId { get; }
	IReadOnlyList<ITrapComponentBinding> Components { get; }
	bool IsKnownBy(ICharacter character);
}

public interface ITrapComponentBinding
{
	long ItemId { get; }
	IGameItem? Item { get; }
	TrapComponentRole Role { get; }
	double SpentRecoveryChance { get; }
	double QualityWeight { get; }
}

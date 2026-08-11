#nullable enable

using MudSharp.Effects;
using MudSharp.Character;
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
	bool IsKnownBy(ICharacter character);
}

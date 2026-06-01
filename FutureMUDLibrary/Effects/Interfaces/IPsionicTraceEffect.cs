#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Magic;
using MudSharp.RPG.Checks;
using System;

namespace MudSharp.Effects.Interfaces;

public interface IPsionicTraceEffect : IMagicEffect
{
	Guid TraceId { get; }
	long SourceCharacterId { get; }
	long? TargetCharacterId { get; }
	long? SourceCellId { get; }
	ICharacter? SourceCharacter { get; }
	ICharacter? TargetCharacter { get; }
	ICell? SourceCell { get; }
	PsionicActivityKind ActivityKind { get; }
	string ActivityDescription { get; }
	string UnknownIdentityDescription { get; }
	DateTime CreatedUtc { get; }
	TimeSpan TraceDuration { get; }
	Difficulty ReadDifficulty { get; }
	int ConcealmentDifficultyStages { get; }
	bool Involves(ICharacter character);
}

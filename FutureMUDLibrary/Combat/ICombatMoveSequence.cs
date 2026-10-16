#nullable enable

using System.Collections.Generic;
using MudSharp.Character;

namespace MudSharp.Combat;

/// <summary>A single scheduled action whose children complete in order, each with its own response.</summary>
public interface ICombatMoveSequence : ICombatMove
{
	ICombat OriginalCombat { get; }
	ICharacter Target { get; }
	double StaminaBudget { get; }
	IReadOnlyList<ICombatMove> Children { get; }
	IReadOnlyList<CombatMoveResolution> Resolutions { get; }
	bool TryPrepare();
	bool CanExecute(ICombatMove child);
	void RecordResolution(CombatMoveResolution resolution);
	CombatMoveResult CompleteResolution();
}

/// <summary>Resolution only: turn effects, tactics, combat ending and recovery belong to CombatAction.</summary>
public interface ICombatSequenceResolver
{
	CombatMoveResult ResolveSequence(ICombatMoveSequence sequence);
}

public sealed record CombatMoveResolution(ICombatMove Move, ICombatMove? Response,
	CombatMoveResult Result, bool Executed, bool Committed, double AttackerStamina, double DefenderStamina);

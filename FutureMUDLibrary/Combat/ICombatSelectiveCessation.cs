#nullable enable
using System;
using MudSharp.Framework;

namespace MudSharp.Combat;

[Flags]
public enum CombatCessationChanges
{
	None = 0,
	SubjectRemoved = 1,
	OpponentPairCleared = 2
}

/// <summary>Opaque, nonpersistent admission. Validation performs no gameplay callbacks.</summary>
public interface ICombatCessationAdmission
{
	bool IsCurrent { get; }
}

/// <summary>Optional capability; existing ICombat callers retain their ordinary leave contract.</summary>
public interface ICombatSelectiveCessation
{
	ICombatCessationAdmission? PrepareCessation(IPerceiver subject, IPerceiver? opponent, ICombat expectedCombat);
	CombatCessationChanges CeaseCombatFor(ICombatCessationAdmission admission);
}

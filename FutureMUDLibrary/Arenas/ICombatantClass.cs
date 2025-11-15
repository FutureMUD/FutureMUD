#nullable enable

using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;

namespace MudSharp.Arenas;

/// <summary>
/// Defines a combatant archetype that governs eligibility, NPC loading, and identity metadata.
/// </summary>
public interface ICombatantClass : IFrameworkItem, ISaveable {
	/// <summary>Eligibility prog returns bool and accepts the candidate character.</summary>
	IFutureProg EligibilityProg { get; }
	/// <summary>Optional admin NPC loader prog; accepts slots needed and returns characters.</summary>
	IFutureProg? AdminNpcLoaderProg { get; }
	/// <summary>Automatically resurrect arena NPCs on death when true.</summary>
	bool ResurrectNpcOnDeath { get; }
	/// <summary>Optional default stage name template for participants.</summary>
	string? DefaultStageNameTemplate { get; }
	/// <summary>Optional ANSI colour token applied to participant names.</summary>
	string? DefaultSignatureColour { get; }
}

#nullable enable

using System;
using MudSharp.Character;

namespace MudSharp.Combat;

/// <summary>Resolves an explicitly authored extra action without replacing the actor's scheduled turn.</summary>
public interface IAdditionalCombatActionResolver
{
	bool TryResolveAdditionalAction(ICharacter actor, ICombatMove move);
	/// <summary>The guard also covers executable admission/selection callbacks.</summary>
	bool TryResolveAdditionalAction(ICharacter actor, Func<ICombatMove?> selectMove);
}

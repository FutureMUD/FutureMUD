using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.GameItems.Interfaces;

#nullable enable

namespace MudSharp.Framework;

public static class TargetExtensions
{
	/// <summary>
	/// Targets an actor first, then a corpse, and returns the corresponding body target details when found.
	/// </summary>
	public static BodyTargetResult? TargetActorOrCorpseBody(this ITarget targeter, string keyword,
		PerceiveIgnoreFlags ignoreFlags = PerceiveIgnoreFlags.None)
	{
		ICharacter? character = targeter.TargetActor(keyword, ignoreFlags);
		if (character is not null)
		{
			return new BodyTargetResult(character, character, character, null);
		}

		ICorpse? corpse = targeter.TargetCorpse(keyword, ignoreFlags);
		return corpse?.Body is null ? null : new BodyTargetResult(corpse.Parent, corpse, corpse.OriginalCharacter, corpse);
	}
}

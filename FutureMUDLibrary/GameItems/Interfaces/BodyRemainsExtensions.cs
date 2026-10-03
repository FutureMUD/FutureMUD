#nullable enable

using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces;

public static class BodyRemainsExtensions
{
	/// <summary>Resolves a final corpse's owner only when character operations address that corpse's exact body.</summary>
	public static ICharacter? GetOriginalCharacterWithMatchingBody(this ICorpse? corpse)
	{
		if (corpse is not { RepresentsFinalCharacterDeath: true } || corpse.OriginalBody is not { } body ||
		    corpse.OriginalCharacter is not { Body: { } currentBody } character)
		{
			return null;
		}

		return ReferenceEquals(body, currentBody) || body.Id > 0 && body.Id == currentBody.Id ? character : null;
	}
}

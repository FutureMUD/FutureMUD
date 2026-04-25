using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Magic.SpellTriggers;

internal static class CastingTriggerExitHelper
{
	public static ICellExit? ResolveExit(ICharacter actor, string text)
	{
		return actor.Location.ExitsFor(actor).GetFromItemListByKeyword(text, actor);
	}
}

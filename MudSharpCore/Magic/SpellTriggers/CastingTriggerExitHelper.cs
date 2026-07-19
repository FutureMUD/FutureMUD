using MudSharp.Construction.Boundary;

#nullable enable

namespace MudSharp.Magic.SpellTriggers;

internal static class CastingTriggerExitHelper
{
	public static ICellExit? ResolveExit(ICharacter actor, string text)
	{
		return actor.Location.ExitsFor(actor).GetFromItemListByKeyword(text, actor);
	}
}

using MudSharp.Construction.Boundary;

#nullable enable

namespace MudSharp.Magic.SpellTriggers;

internal static class CastingTriggerExitHelper
{
	public static IRoomExit? ResolveExit(ICharacter actor, string text)
	{
		return actor.Location.GetExitKeyword(text, actor);
	}
}

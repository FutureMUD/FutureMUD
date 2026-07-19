#nullable enable

using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Commands.Modules;

internal enum ForceTargetScope
{
	All,
	Players,
	Npcs
}

internal static class ForceTargetResolver
{
	public static IEnumerable<ICharacter> Resolve(IFuturemud gameworld, ForceTargetScope scope)
	{
		return scope switch
		{
			ForceTargetScope.All => gameworld.Actors,
			ForceTargetScope.Players => gameworld.Characters,
			ForceTargetScope.Npcs => gameworld.NPCs,
			_ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null)
		};
	}
}

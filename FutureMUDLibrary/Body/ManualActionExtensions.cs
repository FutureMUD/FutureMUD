using System.Linq;
using MudSharp.Character;

#nullable enable

namespace MudSharp.Body;

/// <summary>Checks the actor's ability to perform an ordinary manual action, independent of hand occupancy.</summary>
public static class ManualActionExtensions
{
	public static bool CanPerformManualAction(this IBody body, out string reason)
	{
		if (body.Actor?.EffectsOfType<MudSharp.Magic.ISpellProjectionBoundary>().Any() == true)
		{
			reason = "This projected presence cannot manipulate physical objects.";
			return false;
		}
		if (body.HoldLocs.Any(x => body.CanUseBodypart(x) == CanUseBodypartResult.CanUse))
		{
			reason = string.Empty;
			return true;
		}

		reason = $"You do not have any functioning {body.WielderDescriptionPlural?.ToLowerInvariant() ?? "manipulators"} with which to do that.";
		return false;
	}

	public static bool CanPerformManualAction(this ICharacter actor, out string reason)
	{
		return actor.Body.CanPerformManualAction(out reason);
	}
}

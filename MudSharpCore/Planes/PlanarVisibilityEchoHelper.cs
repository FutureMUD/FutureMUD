using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Planes;

internal static class PlanarVisibilityEchoHelper
{
	internal static IReadOnlyDictionary<IPerceiver, string> CaptureVisibleObservers(IPerceivable target)
	{
		return ObserversFor(target)
		       .Where(x => x.CanSee(target))
		       .Distinct()
		       .ToDictionary(x => x, x => target.HowSeen(x, true));
	}

	internal static void EchoVisibilityChanges(IPerceivable target, IReadOnlyDictionary<IPerceiver, string> before)
	{
		foreach (var observer in before.Keys.Concat(ObserversFor(target)).Distinct())
		{
			var couldSee = before.TryGetValue(observer, out var previousDescription);
			var canSee = observer.CanSee(target);
			if (couldSee == canSee)
			{
				continue;
			}

			var description = canSee ? target.HowSeen(observer, true) : previousDescription;
			observer.OutputHandler.Send(canSee
				? $"{description} fades into view."
				: $"{description} fades from view.");
		}
	}

	private static IEnumerable<IPerceiver> ObserversFor(IPerceivable target)
	{
		return target.Location?
		             .Characters
		             .Where(x => !x.IsSelf(target))
		             .Cast<IPerceiver>() ?? Enumerable.Empty<IPerceiver>();
	}
}

using System;
using System.Linq;

namespace MudSharp.GameItems.Interfaces;

#nullable enable

/// <summary>
/// The elapsed-time processes that can be modified by the environment of a contained item.
/// </summary>
public enum ItemTimeRateType
{
	PreparedFoodFreshness,
	BiologicalDecay,
	Morph,
	SurfaceLiquidDrying
}

/// <summary>
/// Supplies an elapsed-time multiplier to items contained beneath this component.
/// A null result means that the component does not modify that process.
/// </summary>
public interface IItemTimeRateModifier : IGameItemComponent
{
	double? RateMultiplierFor(ItemTimeRateType type);
}

/// <summary>
/// A component whose accumulated state must be resolved before its containing environment changes.
/// </summary>
public interface IItemTimeRateSensitive : IGameItemComponent
{
	void ResolveTimeRate(DateTime utcNow);
}

public static class ItemTimeRateExtensions
{
	/// <summary>
	/// Finds the nearest containing component that modifies the requested process.
	/// </summary>
	public static double TimeRateMultiplier(this IGameItem item, ItemTimeRateType type)
	{
		var container = item.ContainedIn;
		while (container is not null)
		{
			foreach (var modifier in container.Components.OfType<IItemTimeRateModifier>())
			{
				var rate = modifier.RateMultiplierFor(type);
				if (rate is not null)
				{
					return Math.Max(0.0, rate.Value);
				}
			}

			container = container.ContainedIn;
		}

		return 1.0;
	}
}

public static class ItemTimeRateMath
{
	public static double RefrigerationRate(bool powered, bool open, double poweredClosed, double poweredOpen,
		double unpoweredClosed, double unpoweredOpen)
	{
		return (powered, open) switch
		{
			(true, false) => poweredClosed,
			(true, true) => poweredOpen,
			(false, false) => unpoweredClosed,
			_ => unpoweredOpen
		};
	}

	public static TimeSpan EffectiveElapsed(TimeSpan wallElapsed, double rate)
	{
		return TimeSpan.FromTicks((long)(wallElapsed.Ticks * Math.Max(0.0, rate)));
	}

	public static TimeSpan PreservedMorphRemaining(TimeSpan wallRemaining, bool refrigerationSensitive,
		double scheduledRate)
	{
		if (wallRemaining <= TimeSpan.Zero)
		{
			return TimeSpan.Zero;
		}

		return refrigerationSensitive
			? EffectiveElapsed(wallRemaining, scheduledRate)
			: wallRemaining;
	}

	public static TimeSpan? WallDuration(TimeSpan effectiveDuration, double rate)
	{
		return rate <= 0.0
			? null
			: TimeSpan.FromTicks((long)(effectiveDuration.Ticks / rate));
	}
}

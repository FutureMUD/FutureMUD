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
	SurfaceLiquidDrying,
	LiquidFreshness
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
/// Contract for a future package component that modifies consumable freshness until it has been opened.
/// Gate 2 deliberately supplies only the contract; logistics owns the eventual component and opening lifecycle.
/// </summary>
public interface IPackageFreshnessModifier : IItemTimeRateModifier
{
	bool FreshnessProtectionActive { get; }
	bool IrreversiblyOpened { get; }
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
					return double.IsFinite(rate.Value) ? Math.Max(0.0, rate.Value) : 0.0;
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
		if (!double.IsFinite(rate) || rate <= 0.0)
		{
			return TimeSpan.Zero;
		}

		var ticks = wallElapsed.Ticks * rate;
		if (ticks >= long.MaxValue) return TimeSpan.MaxValue;
		if (ticks <= long.MinValue) return TimeSpan.MinValue;
		return TimeSpan.FromTicks((long)ticks);
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
		if (!double.IsFinite(rate) || rate <= 0.0) return null;
		if (effectiveDuration <= TimeSpan.Zero) return TimeSpan.Zero;
		var ticks = effectiveDuration.Ticks / rate;
		return !double.IsFinite(ticks) || ticks >= long.MaxValue ? null : TimeSpan.FromTicks((long)ticks);
	}
}

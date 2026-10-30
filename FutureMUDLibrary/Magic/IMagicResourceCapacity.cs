using System;
using System.Linq;
using MudSharp.Character;
using MudSharp.Health;

#nullable enable
namespace MudSharp.Magic;

public sealed record MagicResourceAttributeCapacity(long AttributeId, long ExpressionId, bool UseRawAttribute = false);

/// <summary>Optional diagnostic capacity evaluation; ordinary resource cap implementations remain compatible.</summary>
public interface IMagicResourceCapacity
{
	bool TryGetResourceCap(IHaveMagicResource holder, out double cap, out string? error);
}

public static class MagicResourceCapacity
{
	/// <summary>Pure evaluation at accounting boundaries. Invalid capacities never become usable balances.</summary>
	public static bool TryGetCap(IMagicResource resource, IHaveMagicResource holder, out double cap, out string? error)
	{
		cap = double.NaN;
		error = null;
		try
		{
			var hasAdmissionCapacity = MagicResourceCapacityAdmission.TryTakeCapacity(resource, holder, out cap);
			if (!hasAdmissionCapacity && resource is IMagicResourceCapacity configured)
			{
				if (!configured.TryGetResourceCap(holder, out cap, out error)) return false;
			}
			else if (!hasAdmissionCapacity) cap = resource.ResourceCap(holder);
			if (!double.IsFinite(cap) || cap < 0.0)
			{
				error = "Capacity must be finite and non-negative.";
				return false;
			}
			if (!hasAdmissionCapacity && holder is ICharacter actor && actor.Body is not null)
			{
				var modifiers = actor.Body.EffectsOfType<IMagicResourceCapacityBonusEffect>()
					.Where(x => x.MagicResourceId == resource.Id).ToArray();
				if (modifiers.Any(x => !double.IsFinite(x.MagicResourceCapacityBonus) ||
				                      !double.IsFinite(x.MagicResourceCapacityMultiplier) || x.MagicResourceCapacityMultiplier < 0.0))
				{
					error = "Capacity modifiers must be finite and multipliers non-negative.";
					return false;
				}
				cap = (cap + modifiers.Sum(x => x.MagicResourceCapacityBonus)) * modifiers.Aggregate(1.0,
					(current, x) => current * x.MagicResourceCapacityMultiplier);
				if (double.IsFinite(cap)) cap = Math.Max(0.0, cap);
			}
			if (double.IsFinite(cap) && cap >= 0) return true;
			error = "Capacity must be finite and non-negative.";
			return false;
		}
		catch (Exception ex)
		{
			error = $"Capacity evaluation failed: {ex.GetBaseException().Message}";
			return false;
		}
	}
}

using System;

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
			if (MagicResourceCapacityAdmission.TryTakeCapacity(resource, holder, out cap)) return true;
			if (resource is IMagicResourceCapacity configured)
			{
				if (!configured.TryGetResourceCap(holder, out cap, out error)) return false;
			}
			else cap = resource.ResourceCap(holder);
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

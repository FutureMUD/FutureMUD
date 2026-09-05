#nullable enable

using System;

namespace MudSharp.Magic;

public readonly record struct MagicResourceTransferResult(double Removed, double Received);

public static class MagicResourceTransfer
{
	/// <summary>Transfers only available resources, accounting for receiver capacity before debiting the donor.</summary>
	public static MagicResourceTransferResult Transfer(IHaveMagicResource donor, IHaveMagicResource recipient,
		IMagicResource resource, double requested, double lossFraction)
	{
		if (!double.IsFinite(requested) || requested <= 0 || !double.IsFinite(lossFraction) ||
		    lossFraction < 0 || lossFraction >= 1 || ReferenceEquals(donor, recipient))
		{
			return default;
		}

		donor.MagicResourceAmounts.TryGetValue(resource, out var available);
		recipient.MagicResourceAmounts.TryGetValue(resource, out var current);
		var cap = resource.ResourceCap(recipient);
		if (!double.IsFinite(available) || !double.IsFinite(current) || !double.IsFinite(cap))
		{
			return default;
		}

		var efficiency = 1 - lossFraction;
		var removed = Math.Min(requested, Math.Min(Math.Max(0, available), Math.Max(0, cap - current) / efficiency));
		if (removed <= 0 || !donor.UseResource(resource, removed))
		{
			return default;
		}

		var received = removed * efficiency;
		recipient.AddResource(resource, received);
		return new MagicResourceTransferResult(removed, received);
	}
}

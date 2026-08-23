#nullable enable

namespace MudSharp.Economy.Shops;

/// <summary>
/// Pure, deterministic rules shared by restaurant command handling and queue processing.
/// Keeping these calculations separate from the live shop makes their consent, payment and
/// timing guarantees easy to regression-test without a running gameworld.
/// </summary>
public static class RestaurantServiceRules
{
	public static bool CanAutomaticallyJoin(bool sharesCurrentParty,
		bool existingParticipantConsidersRequesterAlly)
	{
		return sharesCurrentParty || existingParticipantConsidersRequesterAlly;
	}

	public static TimeSpan EstimateWait(TimeSpan preparationTime, TimeSpan handlingTime, int queuedOrdersAhead,
		TimeSpan maximumBatchWait, int quantity)
	{
		var units = Math.Max(1, quantity);
		var preparation = preparationTime < TimeSpan.Zero ? TimeSpan.Zero : preparationTime;
		var handling = handlingTime < TimeSpan.Zero ? TimeSpan.Zero : handlingTime;
		var batching = maximumBatchWait < TimeSpan.Zero ? TimeSpan.Zero : maximumBatchWait;
		var queueCount = Math.Max(0, queuedOrdersAhead);
		return TimeSpan.FromTicks(preparation.Ticks * units) +
		       TimeSpan.FromTicks(handling.Ticks * (queueCount + 1L)) + batching;
	}

	public static TimeSpan PreparationTime(TimeSpan configuredPreparationTime, IEnumerable<TimeSpan>? craftPhaseLengths)
	{
		var result = configuredPreparationTime < TimeSpan.Zero ? TimeSpan.Zero : configuredPreparationTime;
		foreach (var phaseLength in craftPhaseLengths ?? Enumerable.Empty<TimeSpan>())
		{
			if (phaseLength > TimeSpan.Zero)
			{
				result += phaseLength;
			}
		}

		return result;
	}

	public static IReadOnlyDictionary<long, decimal> SuggestEqualSplit(IEnumerable<long> acceptedParticipantIds,
		decimal outstandingBalance, decimal minimumPaymentUnit = 1.0M)
	{
		var participantIds = acceptedParticipantIds.Distinct().OrderBy(x => x).ToList();
		if (!participantIds.Any() || outstandingBalance <= 0.0M)
		{
			return new Dictionary<long, decimal>();
		}

		var unit = minimumPaymentUnit <= 0.0M ? 1.0M : minimumPaymentUnit;
		var baseAmount = Math.Floor(outstandingBalance / (participantIds.Count * unit)) * unit;
		var result = participantIds.ToDictionary(x => x, _ => baseAmount);
		var remainder = outstandingBalance - baseAmount * participantIds.Count;
		foreach (var participantId in participantIds)
		{
			if (remainder <= 0.0M)
			{
				break;
			}

			var allocation = Math.Min(unit, remainder);
			result[participantId] += allocation;
			remainder -= allocation;
		}

		return result;
	}

	public static bool IsBatchReady(bool allRelatedItemsReady, DateTime? readyAtUtc, TimeSpan maximumBatchWait,
		DateTime utcNow)
	{
		if (allRelatedItemsReady)
		{
			return true;
		}

		return readyAtUtc.HasValue && utcNow - readyAtUtc.Value >= maximumBatchWait;
	}

	public static bool IsTableCleanupDue(DateTime? lastSweepAtUtc, TimeSpan cleanupInterval, DateTime utcNow)
	{
		if (cleanupInterval <= TimeSpan.Zero || !lastSweepAtUtc.HasValue)
		{
			return true;
		}

		return utcNow >= lastSweepAtUtc.Value && utcNow - lastSweepAtUtc.Value >= cleanupInterval;
	}

	/// <summary>
	/// Calculates how much of a stack appeared after the restaurant captured its pre-craft snapshot.
	/// This preserves a real craft result even when the cell's normal stack-merging rules combine it
	/// with existing restaurant stock.
	/// </summary>
	public static int NewlyProducedQuantity(int currentQuantity, int quantityBeforeCraft)
	{
		return Math.Max(0, currentQuantity - Math.Max(0, quantityBeforeCraft));
	}

	public static decimal PaymentToApply(decimal requestedAmount, decimal outstandingBalance)
	{
		return Math.Max(0.0M, Math.Min(requestedAmount, outstandingBalance));
	}

	public static decimal OutstandingLiability(RestaurantOrderStatus status, decimal price, decimal grossAmountPaid)
	{
		return status is RestaurantOrderStatus.Cancelled or RestaurantOrderStatus.Failed or RestaurantOrderStatus.Refunded
			? 0.0M
			: Math.Max(0.0M, price - grossAmountPaid);
	}

	public static bool ValidateFulfilmentConfiguration(RestaurantFulfilmentMode fulfilmentMode,
		bool dineInAvailable, bool takeawayAvailable, bool hasCraft, bool craftIsValidAndProducesOutput,
		bool itemCanBeOpened, bool hasServingContainer, bool servingContainerIsCompatible,
		bool hasTakeawayContainer, bool takeawayContainerIsCompatible, bool hasTakeawayBag,
		bool takeawayBagIsCompatible, out string reason)
	{
		if (!dineInAvailable && !takeawayAvailable)
		{
			reason = "it is available to neither dine-in nor takeaway customers";
			return false;
		}

		if (fulfilmentMode is RestaurantFulfilmentMode.CraftAndBring or RestaurantFulfilmentMode.CraftAndPlate)
		{
			if (!hasCraft)
			{
				reason = "its craft fulfilment mode has no craft assigned";
				return false;
			}

			if (!craftIsValidAndProducesOutput)
			{
				reason = "its configured craft does not produce the menu merchandise";
				return false;
			}
		}

		if (fulfilmentMode == RestaurantFulfilmentMode.OpenAndBring && !itemCanBeOpened)
		{
			reason = "its opening fulfilment mode is configured for an item that cannot be opened";
			return false;
		}

		if (fulfilmentMode == RestaurantFulfilmentMode.CraftAndPlate && !hasServingContainer)
		{
			reason = "its plating fulfilment mode has no serving container assigned";
			return false;
		}

		if (hasServingContainer && !servingContainerIsCompatible)
		{
			reason = "its serving container prototype is not a container";
			return false;
		}

		if (hasTakeawayContainer && !takeawayContainerIsCompatible)
		{
			reason = "its takeaway container prototype is not a container";
			return false;
		}

		if (hasTakeawayBag && !takeawayBagIsCompatible)
		{
			reason = "its takeaway bag prototype is not a container";
			return false;
		}

		if (fulfilmentMode == RestaurantFulfilmentMode.PackageTakeaway && !hasTakeawayContainer)
		{
			reason = "its takeaway packaging fulfilment mode has no inner container assigned";
			return false;
		}

		if (fulfilmentMode == RestaurantFulfilmentMode.PackageTakeaway && !takeawayAvailable)
		{
			reason = "its takeaway packaging fulfilment mode is not available for takeaway";
			return false;
		}

		reason = string.Empty;
		return true;
	}
}

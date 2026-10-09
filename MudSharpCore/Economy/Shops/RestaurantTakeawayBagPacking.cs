using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

#nullable enable

namespace MudSharp.Economy.Shops;

/// <summary>
/// Packs a prepared takeaway order, using a bounded exact search for small collections
/// and a linear greedy fallback for large collections.
/// </summary>
internal static class RestaurantTakeawayBagPacking
{
	private const double WeightTolerance = 0.000001;
	internal const int MaximumExactItems = 24;
	internal const int MaximumSearchSteps = 10000;
	private sealed class SearchBudget(int remaining)
	{
		public int Remaining { get; private set; } = remaining;
		public bool Spend() => Remaining-- > 0;
	}

	public static bool TryPlan(IGameItemProto bagPrototype, IReadOnlyCollection<IGameItem> items,
		out IReadOnlyList<IReadOnlyList<IGameItem>> plan, out string reason,
		int maximumSearchSteps = MaximumSearchSteps)
	{
		plan = [];
		reason = string.Empty;
		if (!items.Any())
		{
			return true;
		}

		var containers = bagPrototype.Components.OfType<ContainerGameItemComponentProto>().ToList();
		if (containers.Count != 1)
		{
			reason = "The configured takeaway bag must have exactly one standard Container component.";
			return false;
		}

		var container = containers[0];

		if (container.WeightLimit <= 0.0)
		{
			reason = "The configured takeaway bag has no usable weight capacity.";
			return false;
		}

		var orderedItems = items
			.OrderByDescending(x => x.Weight)
			.ThenByDescending(x => x.Size)
			.ThenBy(x => x.Id)
			.ToList();
		foreach (var item in orderedItems)
		{
			if (!CanBagContain(container, item))
			{
				reason = $"The configured takeaway bag cannot hold {item.HowSeen(null)}.";
				return false;
			}
		}

		if (orderedItems.Count > MaximumExactItems)
		{
			plan = NextFitDecreasing(orderedItems, container.WeightLimit);
			return true;
		}

		var upperBound = FirstFitDecreasing(orderedItems, container.WeightLimit);
		var minimumBagCount = LowerBound(orderedItems, container.WeightLimit);
		if (upperBound.Count == minimumBagCount)
		{
			plan = upperBound;
			return true;
		}

		var budget = new SearchBudget(Math.Clamp(maximumSearchSteps, 0, MaximumSearchSteps));
		for (var bagCount = minimumBagCount; bagCount < upperBound.Count && budget.Remaining > 0; bagCount++)
		{
			if (!TryPackInto(orderedItems, container.WeightLimit, bagCount, budget, out var exactPlan))
			{
				continue;
			}

			plan = exactPlan;
			return true;
		}

		plan = upperBound;
		return true;
	}

	private static bool CanBagContain(ContainerGameItemComponentProto container, IGameItem item)
	{
		return !container.BlockedTags.Any(item.IsA) &&
		       (!container.AllowedTags.Any() || container.AllowedTags.Any(item.IsA)) &&
		       (item.Size <= container.MaximumContentsSize || item.IsItemType<ICommodity>()) &&
		       item.Weight <= container.WeightLimit + WeightTolerance;
	}

	private static int LowerBound(IReadOnlyCollection<IGameItem> items, double capacity)
	{
		var totalWeight = items.Sum(x => x.Weight);
		var weightBound = (int)Math.Ceiling((totalWeight - WeightTolerance) / capacity);
		var largeItemBound = items.Count(x => x.Weight > capacity / 2.0 + WeightTolerance);
		return Math.Max(1, Math.Max(weightBound, largeItemBound));
	}

	private static IReadOnlyList<IReadOnlyList<IGameItem>> FirstFitDecreasing(IEnumerable<IGameItem> items,
		double capacity)
	{
		var bags = new List<List<IGameItem>>();
		var weights = new List<double>();
		foreach (var item in items)
		{
			var index = weights.FindIndex(x => x + item.Weight <= capacity + WeightTolerance);
			if (index < 0)
			{
				bags.Add([item]);
				weights.Add(item.Weight);
				continue;
			}

			bags[index].Add(item);
			weights[index] += item.Weight;
		}

		return bags.Select(x => (IReadOnlyList<IGameItem>)x).ToList();
	}

	private static bool TryPackInto(IReadOnlyList<IGameItem> items, double capacity, int bagCount,
		SearchBudget budget, out IReadOnlyList<IReadOnlyList<IGameItem>> plan)
	{
		var bags = Enumerable.Range(0, bagCount).Select(_ => new List<IGameItem>()).ToList();
		var weights = new double[bagCount];

		bool PlaceNext(int itemIndex)
		{
			if (itemIndex == items.Count)
			{
				return true;
			}

			var item = items[itemIndex];
			var triedLoads = new HashSet<long>();
			for (var bagIndex = 0; bagIndex < bags.Count; bagIndex++)
			{
				if (!budget.Spend()) return false;
				if (weights[bagIndex] + item.Weight > capacity + WeightTolerance)
				{
					continue;
				}

				var loadKey = (long)Math.Round(weights[bagIndex] / WeightTolerance);
				if (!triedLoads.Add(loadKey))
				{
					continue;
				}

				var wasEmpty = bags[bagIndex].Count == 0;
				bags[bagIndex].Add(item);
				weights[bagIndex] += item.Weight;
				if (PlaceNext(itemIndex + 1))
				{
					return true;
				}

				weights[bagIndex] -= item.Weight;
				bags[bagIndex].RemoveAt(bags[bagIndex].Count - 1);
				if (wasEmpty)
				{
					break;
				}
			}

			return false;
		}

		if (PlaceNext(0))
		{
			plan = bags
				.Where(x => x.Any())
				.Select(x => (IReadOnlyList<IGameItem>)x.ToList())
				.ToList();
			return true;
		}

		plan = [];
		return false;
	}

	private static IReadOnlyList<IReadOnlyList<IGameItem>> NextFitDecreasing(IEnumerable<IGameItem> items,
		double capacity)
	{
		var bags = new List<IReadOnlyList<IGameItem>>();
		List<IGameItem>? current = null;
		var currentWeight = 0.0;
		foreach (var item in items)
		{
			var weight = item.Weight;
			if (current is null || currentWeight + weight > capacity + WeightTolerance)
			{
				current = [];
				bags.Add(current);
				currentWeight = 0.0;
			}

			current.Add(item);
			currentWeight += weight;
		}

		return bags;
	}
}

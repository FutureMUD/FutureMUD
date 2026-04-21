#nullable enable
using MudSharp.Character;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Economy.Markets;

internal static class MarketImpactExpansion
{
	public static IReadOnlyCollection<ExpandedMarketImpact> ExpandImpactToLeafCategories(MarketImpact impact)
	{
		return ExpandCategoryToLeafWeights(impact.MarketCategory)
			.Select(component => new ExpandedMarketImpact(
				impact.MarketCategory,
				component.MarketCategory,
				component.Weight,
				impact.SupplyImpact * (double)component.Weight,
				impact.DemandImpact * (double)component.Weight,
				impact.FlatPriceImpact * (double)component.Weight))
			.ToList();
	}

	public static string DescribeCombinationImpactPreview(IEnumerable<MarketImpact> impacts, ICharacter actor)
	{
		var expandedImpacts = impacts
			.Where(x => x.MarketCategory.CategoryType == MarketCategoryType.Combination)
			.SelectMany(ExpandImpactToLeafCategories)
			.OrderBy(x => x.SourceCategory.Name)
			.ThenBy(x => x.LeafCategory.Name)
			.ToList();
		if (!expandedImpacts.Any())
		{
			return string.Empty;
		}

		return StringUtilities.GetTextTable(
			from impact in expandedImpacts
			select new List<string>
			{
				impact.SourceCategory.Name.ColourName(),
				impact.LeafCategory.Name.ColourName(),
				impact.NormalizedWeight.ToString("P2", actor).ColourValue(),
				impact.SupplyImpact.ToBonusPercentageString(actor),
				impact.DemandImpact.ToBonusPercentageString(actor),
				impact.FlatPriceImpact.ToBonusPercentageString(actor)
			},
			[
				"Target",
				"Leaf",
				"Normalized",
				"Supply",
				"Demand",
				"Flat Price"
			],
			actor,
			Telnet.BoldYellow);
	}

	public static IReadOnlyCollection<MarketCategoryComponent> ExpandCategoryToLeafWeights(IMarketCategory category)
	{
		Dictionary<long, MarketCategoryComponent> results = [];
		ExpandCategoryToLeafWeights(category, 1.0m, [], results);
		return results.Values.ToList();
	}

	private static void ExpandCategoryToLeafWeights(IMarketCategory category, decimal weight, HashSet<long> visiting,
		Dictionary<long, MarketCategoryComponent> results)
	{
		if (weight <= 0.0m)
		{
			return;
		}

		if (!visiting.Add(category.Id))
		{
			return;
		}

		try
		{
			var normalizedComponents = GetNormalizedComponents(category);
			if (category.CategoryType != MarketCategoryType.Combination || normalizedComponents.Count == 0)
			{
				if (results.TryGetValue(category.Id, out var existing))
				{
					results[category.Id] = existing with { Weight = existing.Weight + weight };
				}
				else
				{
					results[category.Id] = new MarketCategoryComponent
					{
						MarketCategory = category,
						Weight = weight
					};
				}

				return;
			}

			foreach (var component in normalizedComponents)
			{
				ExpandCategoryToLeafWeights(component.MarketCategory, weight * component.Weight, visiting, results);
			}
		}
		finally
		{
			visiting.Remove(category.Id);
		}
	}

	public static IReadOnlyCollection<MarketCategoryComponent> GetNormalizedComponents(IMarketCategory category)
	{
		if (category.CategoryType != MarketCategoryType.Combination)
		{
			return [];
		}

		var validComponents = category.CombinationComponents
			.Where(x => x.Weight > 0.0m && x.MarketCategory is not null && x.MarketCategory.Id != category.Id)
			.ToList();
		if (!validComponents.Any())
		{
			return [];
		}

		var totalWeight = validComponents.Sum(x => x.Weight);
		if (totalWeight <= 0.0m)
		{
			return [];
		}

		return validComponents
			.Select(x => x with { Weight = x.Weight / totalWeight })
			.ToList();
	}
}

internal sealed record ExpandedMarketImpact(
	IMarketCategory SourceCategory,
	IMarketCategory LeafCategory,
	decimal NormalizedWeight,
	double SupplyImpact,
	double DemandImpact,
	double FlatPriceImpact);

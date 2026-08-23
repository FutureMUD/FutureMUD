using System.Globalization;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Shops;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Work.Crafts.Inputs;

#nullable enable

namespace MudSharp.Economy.Employment;

/// <summary>
/// A native restaurant-manager condition. It derives the consumable requirements directly from
/// each active craft-backed menu item, rather than requiring builders to duplicate every recipe
/// as a separate stock rule.
/// </summary>
public sealed record RestaurantIngredientStockCondition(
	int MealCount,
	string SupplierSelector,
	decimal? MaximumLineAmount) : IEmploymentTaskCondition
{
	private const string KeyPrefix = "restaurantstock:v1";

	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.RestaurantIngredientStock;
	public EmploymentAuthoritySet RequiredAuthority => new(EmploymentAuthority.ManageStockRules |
		EmploymentAuthority.ApprovePurchases |
		EmploymentAuthority.ManageDeliveryRoutes);

	public string Key => CreateKey(SupplierSelector);

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		var deficits = RestaurantIngredientStockGoalPlanner.Deficits(context, Math.Max(1, MealCount), out reason);
		if (deficits.Any())
		{
			reason = string.Empty;
			return true;
		}

		if (string.IsNullOrWhiteSpace(reason))
		{
			reason = $"Restaurant ingredient storage already covers {Math.Max(1, MealCount).ToString("N0", CultureInfo.InvariantCulture)} meal(s) of every active craftable menu item.";
		}

		return false;
	}

	public static string CreateKey(string? supplierSelector)
	{
		return $"{KeyPrefix}|supplier={Uri.EscapeDataString(string.IsNullOrWhiteSpace(supplierSelector) ? "any" : supplierSelector.Trim())}";
	}

	public static RestaurantIngredientStockCondition FromRecord(string key, int mealCount,
		decimal? maximumLineAmount)
	{
		var supplier = "any";
		if (!string.IsNullOrWhiteSpace(key) &&
		    key.StartsWith(KeyPrefix, StringComparison.InvariantCultureIgnoreCase))
		{
			var supplierValue = key.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
				.Skip(1)
				.Select(x => x.Split('=', 2, StringSplitOptions.TrimEntries))
				.FirstOrDefault(x => x.Length == 2 && x[0].EqualTo("supplier"));
			if (supplierValue is not null && !string.IsNullOrWhiteSpace(supplierValue[1]))
			{
				supplier = Uri.UnescapeDataString(supplierValue[1]);
			}
		}

		return new RestaurantIngredientStockCondition(Math.Max(1, mealCount), supplier, maximumLineAmount);
	}

	public static string Describe(RestaurantIngredientStockCondition condition, IFormatProvider voyeur)
	{
		var maximum = condition.MaximumLineAmount.HasValue
			? $"; max {condition.MaximumLineAmount.Value.ToString("N2", voyeur)} per purchase line"
			: string.Empty;
		return $"restaurant ingredients for {Math.Max(1, condition.MealCount).ToString("N0", voyeur)} meal(s) from {condition.SupplierSelector}{maximum}";
	}
}

internal enum RestaurantIngredientRequirementKind
{
	Item,
	Commodity
}

internal sealed record RestaurantIngredientRequirement(
	RestaurantIngredientRequirementKind Kind,
	string Key,
	string Description,
	EmploymentItemSelector? ItemSelector,
	int ItemQuantity,
	string? CommodityDescriptor,
	string? CommodityMaterial,
	string? CommodityTag,
	IReadOnlyDictionary<string, string>? CommodityCharacteristics,
	double CommodityWeight);

internal sealed record RestaurantIngredientStockDeficit(
	RestaurantIngredientRequirement Requirement,
	int TargetItemQuantity,
	int CurrentItemQuantity,
	double TargetCommodityWeight,
	double CurrentCommodityWeight)
{
	public int MissingItemQuantity => Math.Max(0, TargetItemQuantity - CurrentItemQuantity);
	public double MissingCommodityWeight => Math.Max(0.0, TargetCommodityWeight - CurrentCommodityWeight);
	public bool IsCommodity => Requirement.Kind == RestaurantIngredientRequirementKind.Commodity;
	public bool IsMissing => IsCommodity ? MissingCommodityWeight > 0.000001 : MissingItemQuantity > 0;
}

internal static class RestaurantIngredientStockGoalPlanner
{
	public static bool IsRestaurantStockGoal(ManagerGoalType goalType)
	{
		return goalType == ManagerGoalType.MaintainRestaurantIngredientStock;
	}

	public static bool IsConfigurationBlocker(string reason)
	{
		return !string.IsNullOrWhiteSpace(reason) &&
		       (reason.Contains("can only", StringComparison.InvariantCultureIgnoreCase) ||
		        reason.Contains("no usable ingredient storage", StringComparison.InvariantCultureIgnoreCase) ||
		        reason.Contains("unsupported", StringComparison.InvariantCultureIgnoreCase));
	}

	public static bool ShouldDeferWithoutTask(ManagerGoalType goalType, string reason)
	{
		return false;
	}

	public static IReadOnlyCollection<RestaurantIngredientStockDeficit> Deficits(IEmploymentTaskContext context,
		int mealCount, out string reason)
	{
		reason = string.Empty;
		if (context.Employer is not Restaurant restaurant)
		{
			reason = "Restaurant ingredient stock conditions can only be evaluated for restaurant employment hosts.";
			return [];
		}

		var storage = IngredientStorageContainers(restaurant).ToList();
		if (!storage.Any())
		{
			reason = $"{restaurant.Name} has no usable ingredient storage container in a configured kitchen cell.";
			return [];
		}

		var requirements = AggregateRequirements(restaurant, Math.Max(1, mealCount), out reason);
		if (!string.IsNullOrWhiteSpace(reason) || !requirements.Any())
		{
			if (string.IsNullOrWhiteSpace(reason))
			{
				reason = $"{restaurant.Name} has no active craftable menu ingredients to stock.";
			}

			return [];
		}

		var available = storage
			.SelectMany(x => x.Container.Contents.SelectMany(DeepItemsOrSelf))
			.DistinctBy(x => x.Id)
			.ToList();
		return requirements
			.Select(requirement => DeficitFor(context, available, requirement))
			.Where(x => x.IsMissing)
			.ToList();
	}

	public static bool TryBuildActionPlan(IManagerGoal goal, IEmploymentTaskContext context,
		out EmploymentActionPlan? actionPlan, out string reason)
	{
		actionPlan = null;
		reason = string.Empty;
		if (context.Employer is not Restaurant restaurant)
		{
			reason = "Restaurant ingredient stock manager goals can only run for restaurants.";
			return false;
		}

		var condition = goal.Configuration.Conditions?
			.OfType<RestaurantIngredientStockCondition>()
			.LastOrDefault() ??
			new RestaurantIngredientStockCondition(30, "any", null);
		var deficits = Deficits(context, condition.MealCount, out reason);
		if (!deficits.Any())
		{
			return false;
		}

		var destinations = IngredientStorageContainers(restaurant)
			.OrderBy(x => x.Item.Id)
			.ToList();
		if (!destinations.Any())
		{
			reason = $"{restaurant.Name} has no usable ingredient storage container to receive purchased stock.";
			return false;
		}

		var destination = destinations[0];

		var lineLimit = ResolveLineLimit(restaurant, goal.Policy, condition, deficits.Count);
		if (lineLimit <= 0.0M)
		{
			reason = $"{restaurant.Name} has no available funds or configured line maximum for ingredient purchases.";
			return false;
		}

		var steps = new List<IEmploymentActionStep>();
		foreach (var deficit in deficits)
		{
			var amount = new MoneyAmount(restaurant.Currency, lineLimit);
			var quantity = deficit.IsCommodity
				? deficit.MissingCommodityWeight.ToString("N2", CultureInfo.InvariantCulture)
				: deficit.MissingItemQuantity.ToString("N0", CultureInfo.InvariantCulture);
			var description = $"restaurant ingredient stock: {quantity} {deficit.Requirement.Description}";
			steps.Add(new CataloguedActionShellStep("authorise", description, amount, destination.Location));
			steps.Add(new CataloguedActionShellStep("reserve", description, amount, destination.Location));
			steps.Add(deficit.IsCommodity
				? new PurchaseActionStep(deficit.MissingCommodityWeight, deficit.Requirement.CommodityDescriptor!,
					condition.SupplierSelector, restaurant.Currency, amount)
				: new PurchaseActionStep(deficit.MissingItemQuantity, deficit.Requirement.ItemSelector!,
					condition.SupplierSelector, restaurant.Currency, amount));
			steps.Add(new DeliverItemsActionStep(destination.Location,
				EmploymentItemSelector.ForItemId(destination.Item.Id)));
		}

		actionPlan = new EmploymentActionPlan(steps);
		reason = $"Prepared purchase and kitchen-storage work for {deficits.Count.ToString("N0", CultureInfo.InvariantCulture)} restaurant ingredient line(s).";
		return true;
	}

	private static RestaurantIngredientStockDeficit DeficitFor(IEmploymentTaskContext context,
		IReadOnlyCollection<IGameItem> available, RestaurantIngredientRequirement requirement)
	{
		if (requirement.Kind == RestaurantIngredientRequirementKind.Item)
		{
			var current = available
				.Where(x => ItemThresholdCondition.MatchesSelector(context, x, requirement.ItemSelector!))
				.Sum(x => x.Quantity);
			return new RestaurantIngredientStockDeficit(requirement, requirement.ItemQuantity, current, 0.0, 0.0);
		}

		var commodityWeight = available.Sum(x => context.CommodityWeight(x, requirement.CommodityMaterial!,
			requirement.CommodityTag, requirement.CommodityCharacteristics!));
		return new RestaurantIngredientStockDeficit(requirement, 0, 0, requirement.CommodityWeight, commodityWeight);
	}

	private static IReadOnlyCollection<RestaurantIngredientRequirement> AggregateRequirements(Restaurant restaurant,
		int mealCount, out string reason)
	{
		var requirements = new Dictionary<string, RestaurantIngredientRequirement>(StringComparer.InvariantCultureIgnoreCase);
		var unsupported = new List<string>();
		foreach (var menuItem in restaurant.MenuItems
			.Where(x => x.IsActive)
			.Where(x => x.FulfilmentMode is RestaurantFulfilmentMode.CraftAndBring or RestaurantFulfilmentMode.CraftAndPlate)
			.Where(x => x.Craft is { CraftIsValid: true }))
		{
			foreach (var input in menuItem.Craft!.Inputs)
			{
				switch (input)
				{
					case SimpleItemInput simpleItem when simpleItem.TargetItemId > 0 && simpleItem.Quantity > 0:
						AddItemRequirement(requirements, EmploymentItemSelector.ForPrototype(simpleItem.TargetItemId),
							simpleItem.Quantity * mealCount);
						break;
					case TagInput tagInput when tagInput.TargetTag is not null && tagInput.Quantity > 0:
						AddItemRequirement(requirements, EmploymentItemSelector.ForTag(tagInput.TargetTag.Name),
							tagInput.Quantity * mealCount);
						break;
					case CommodityInput commodityInput:
						if (TryAddCommodityRequirement(requirements, commodityInput, mealCount, out var commodityReason))
						{
							break;
						}

						unsupported.Add($"{menuItem.Name}: {commodityReason}");
						break;
					case CommodityTagInput:
						unsupported.Add($"{menuItem.Name}: commodity material-tag inputs cannot select one exact purchasable material");
						break;
					default:
						unsupported.Add($"{menuItem.Name}: {input.InputType} inputs are not purchasable restaurant ingredients");
						break;
				}
			}
		}

		if (unsupported.Any())
		{
			reason = $"Unsupported restaurant craft input configuration: {unsupported.Distinct().ListToString()}.";
			return [];
		}

		reason = string.Empty;
		return requirements.Values.ToList();
	}

	private static void AddItemRequirement(IDictionary<string, RestaurantIngredientRequirement> requirements,
		EmploymentItemSelector selector, int quantity)
	{
		var key = $"item:{ItemThresholdCondition.EncodeSelector(selector)}";
		if (requirements.TryGetValue(key, out var existing))
		{
			requirements[key] = existing with { ItemQuantity = existing.ItemQuantity + quantity };
			return;
		}

		requirements[key] = new RestaurantIngredientRequirement(RestaurantIngredientRequirementKind.Item, key,
			EmploymentItemSelectorResolver.Describe(selector), selector, quantity, null, null, null, null, 0.0);
	}

	private static bool TryAddCommodityRequirement(IDictionary<string, RestaurantIngredientRequirement> requirements,
		CommodityInput input, int mealCount, out string reason)
	{
		reason = string.Empty;
		if (input.Material is null || input.Weight <= 0.0)
		{
			reason = "commodity inputs must specify a material and positive weight";
			return false;
		}

		if (input.CharacteristicRequirements.RequireNoCharacteristics ||
		    input.CharacteristicRequirements.Requirements.Any(x => x.Value is null))
		{
			reason = "commodity inputs with any-value or no-characteristics requirements cannot be represented by a supplier purchase";
			return false;
		}

		var characteristics = input.CharacteristicRequirements.Requirements
			.ToDictionary(x => x.Key.Name, x => x.Value!.GetValue, StringComparer.InvariantCultureIgnoreCase);
		var descriptorParts = new List<string> { input.Material.Name };
		if (input.CommodityPileTag is not null)
		{
			descriptorParts.Add(input.CommodityPileTag.Name);
		}

		descriptorParts.AddRange(characteristics
			.OrderBy(x => x.Key, StringComparer.InvariantCultureIgnoreCase)
			.Select(x => $"{x.Key}={x.Value}"));
		var descriptor = string.Join("|", descriptorParts);
		var key = $"commodity:{descriptor}";
		var quantity = input.Weight * mealCount;
		if (requirements.TryGetValue(key, out var existing))
		{
			requirements[key] = existing with { CommodityWeight = existing.CommodityWeight + quantity };
			return true;
		}

		requirements[key] = new RestaurantIngredientRequirement(RestaurantIngredientRequirementKind.Commodity,
			key, descriptor, null, 0, descriptor, input.Material.Name, input.CommodityPileTag?.Name,
			characteristics, quantity);
		return true;
	}

	private static IEnumerable<(IGameItem Item, IContainer Container, ICell Location)> IngredientStorageContainers(
		Restaurant restaurant)
	{
		var kitchenIds = restaurant.KitchenCells.Select(x => x.Id).ToHashSet();
		foreach (var storage in restaurant.StorageContainers
			.Where(x => x.Roles.HasFlag(RestaurantStorageRole.Ingredients))
			.OrderBy(x => x.GameItemId))
		{
			var item = restaurant.Gameworld.TryGetItem(storage.GameItemId, true);
			var container = item?.GetItemType<IContainer>();
			var location = item?.TrueLocations.FirstOrDefault(x => kitchenIds.Contains(x.Id));
			if (item is not null && container is not null && location is not null)
			{
				yield return (item, container, location);
			}
		}
	}

	private static IEnumerable<IGameItem> DeepItemsOrSelf(IGameItem item)
	{
		yield return item;
		foreach (var nested in item.DeepItems ?? [])
		{
			yield return nested;
		}
	}

	private static decimal ResolveLineLimit(Restaurant restaurant, ManagerGoalPolicy policy,
		RestaurantIngredientStockCondition condition, int deficitLineCount)
	{
		if (condition.MaximumLineAmount is > 0.0M)
		{
			return condition.MaximumLineAmount.Value;
		}

		var budget = policy.BudgetLimits.FirstOrDefault(x => x.Currency.Id == restaurant.Currency.Id);
		var total = budget?.Amount ?? restaurant.AvailableCashFromAllSources();
		return deficitLineCount <= 0 ? 0.0M : Math.Max(0.0M, total / deficitLineCount);
	}
}

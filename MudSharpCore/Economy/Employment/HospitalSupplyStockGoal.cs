using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Hospitals;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;

#nullable enable

namespace MudSharp.Economy.Employment;

public sealed record HospitalSupplyStockCondition(
	HospitalServiceSupplyItemType ItemType,
	int ProcedureCount,
	string SupplierSelector,
	decimal? MaximumLineAmount) : IEmploymentTaskCondition
{
	private const string KeyPrefix = "hospitalstock:v1";

	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.HospitalSupplyStock;
	public EmploymentAuthoritySet RequiredAuthority => new(EmploymentAuthority.ManageStockRules |
		EmploymentAuthority.ApprovePurchases |
		EmploymentAuthority.ManageDeliveryRoutes);

	public string Key => CreateKey(ItemType, SupplierSelector);

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		var deficits = HospitalSupplyStockGoalPlanner.Deficits(context, ItemType, Math.Max(1, ProcedureCount), out reason);
		if (deficits.Any())
		{
			reason = string.Empty;
			return true;
		}

		if (string.IsNullOrWhiteSpace(reason))
		{
			reason = $"Hospital {ItemType.DescribeEnum()} stock already covers {Math.Max(1, ProcedureCount).ToString("N0", CultureInfo.InvariantCulture)} procedure repeat(s).";
		}

		return false;
	}

	public static string CreateKey(HospitalServiceSupplyItemType itemType, string? supplierSelector)
	{
		return $"{KeyPrefix}|type={itemType}|supplier={Uri.EscapeDataString(string.IsNullOrWhiteSpace(supplierSelector) ? "any" : supplierSelector.Trim())}";
	}

	public static HospitalSupplyStockCondition FromRecord(string key, int procedureCount, decimal? maximumLineAmount)
	{
		var itemType = HospitalServiceSupplyItemType.Consumable;
		var supplier = "any";
		var values = ParseKeyValues(key);
		if (values is not null)
		{
			if (values.TryGetValue("type", out var typeText) &&
			    typeText.TryParseEnum<HospitalServiceSupplyItemType>(out var parsedType))
			{
				itemType = parsedType;
			}

			if (values.TryGetValue("supplier", out var supplierText) && !string.IsNullOrWhiteSpace(supplierText))
			{
				supplier = Uri.UnescapeDataString(supplierText);
			}
		}

		return new HospitalSupplyStockCondition(itemType, Math.Max(1, procedureCount), supplier, maximumLineAmount);
	}

	public static string Describe(HospitalSupplyStockCondition condition, IFormatProvider voyeur)
	{
		var max = condition.MaximumLineAmount.HasValue
			? $"; max {condition.MaximumLineAmount.Value.ToString("N2", voyeur)} per purchase line"
			: string.Empty;
		return $"hospital {condition.ItemType.DescribeEnum()} stock for {Math.Max(1, condition.ProcedureCount).ToString("N0", voyeur)} procedure repeat(s) from {condition.SupplierSelector}{max}";
	}

	private static Dictionary<string, string>? ParseKeyValues(string key)
	{
		if (string.IsNullOrWhiteSpace(key) ||
		    !key.StartsWith(KeyPrefix, StringComparison.InvariantCultureIgnoreCase))
		{
			return null;
		}

		return key.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		          .Skip(1)
		          .Select(x => x.Split('=', 2, StringSplitOptions.TrimEntries))
		          .Where(x => x.Length == 2)
		          .ToDictionary(x => x[0], x => x[1], StringComparer.InvariantCultureIgnoreCase);
	}
}

public sealed record HospitalTheatreStockCondition(
	HospitalServiceSupplyItemType ItemType,
	int ProcedureCount) : IEmploymentTaskCondition
{
	private const string KeyPrefix = "hospitaltheatre:v1";

	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.HospitalTheatreStock;
	public EmploymentAuthoritySet RequiredAuthority => new(EmploymentAuthority.ManageStockRules |
		EmploymentAuthority.ManageDeliveryRoutes);

	public string Key => CreateKey(ItemType);

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		var deficits = HospitalTheatreStockGoalPlanner.Deficits(context, ItemType, Math.Max(1, ProcedureCount), out reason);
		if (deficits.Any())
		{
			reason = string.Empty;
			return true;
		}

		if (string.IsNullOrWhiteSpace(reason))
		{
			reason = $"Hospital operating theatres already stage {ItemType.DescribeEnum()} supplies for {Math.Max(1, ProcedureCount).ToString("N0", CultureInfo.InvariantCulture)} procedure repeat(s).";
		}

		return false;
	}

	public static string CreateKey(HospitalServiceSupplyItemType itemType)
	{
		return $"{KeyPrefix}|type={itemType}";
	}

	public static HospitalTheatreStockCondition FromRecord(string key, int procedureCount)
	{
		var itemType = HospitalServiceSupplyItemType.Consumable;
		var values = ParseKeyValues(key);
		if (values is not null &&
		    values.TryGetValue("type", out var typeText) &&
		    typeText.TryParseEnum<HospitalServiceSupplyItemType>(out var parsedType))
		{
			itemType = parsedType;
		}

		return new HospitalTheatreStockCondition(itemType, Math.Max(1, procedureCount));
	}

	public static string Describe(HospitalTheatreStockCondition condition, IFormatProvider voyeur)
	{
		return $"operating-theatre {condition.ItemType.DescribeEnum()} staging for {Math.Max(1, condition.ProcedureCount).ToString("N0", voyeur)} procedure repeat(s)";
	}

	private static Dictionary<string, string>? ParseKeyValues(string key)
	{
		if (string.IsNullOrWhiteSpace(key) ||
		    !key.StartsWith(KeyPrefix, StringComparison.InvariantCultureIgnoreCase))
		{
			return null;
		}

		return key.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		          .Skip(1)
		          .Select(x => x.Split('=', 2, StringSplitOptions.TrimEntries))
		          .Where(x => x.Length == 2)
		          .ToDictionary(x => x[0], x => x[1], StringComparer.InvariantCultureIgnoreCase);
	}
}

internal sealed record HospitalSupplyStockDeficit(
	EmploymentItemSelector Selector,
	HospitalServiceSupplyItemType ItemType,
	int TargetQuantity,
	int CurrentQuantity,
	int MissingQuantity)
{
	public string Description => EmploymentItemSelectorResolver.Describe(Selector);
}

internal sealed record HospitalTheatreStockDeficit(
	ICell Theatre,
	EmploymentItemSelector? Selector,
	TreatmentType? TreatmentType,
	HospitalServiceSupplyItemType ItemType,
	int TargetQuantity,
	int CurrentQuantity,
	int MissingQuantity)
{
	public string Description => Selector is not null
		? EmploymentItemSelectorResolver.Describe(Selector)
		: $"{(TreatmentType is { } treatmentType ? treatmentType.DescribeEnum() : "unknown")} treatment supplies";
}

internal static class HospitalSupplyStockGoalPlanner
{
	public static bool IsHospitalStockGoal(ManagerGoalType goalType)
	{
		return IsHospitalSupplyStockGoal(goalType) || HospitalTheatreStockGoalPlanner.IsHospitalTheatreStockGoal(goalType);
	}

	public static bool IsHospitalSupplyStockGoal(ManagerGoalType goalType)
	{
		return goalType is ManagerGoalType.MaintainHospitalConsumableStock or
			ManagerGoalType.MaintainHospitalReusableEquipmentStock;
	}

	public static HospitalServiceSupplyItemType ItemTypeForGoal(ManagerGoalType goalType)
	{
		return goalType is ManagerGoalType.MaintainHospitalReusableEquipmentStock or
			ManagerGoalType.MaintainHospitalTheatreReusableEquipmentStock
			? HospitalServiceSupplyItemType.ReusableTool
			: HospitalServiceSupplyItemType.Consumable;
	}

	public static bool IsConfigurationBlocker(string reason)
	{
		return !string.IsNullOrWhiteSpace(reason) &&
		       (reason.Contains("can only", StringComparison.InvariantCultureIgnoreCase) ||
		        reason.Contains("no supply rooms", StringComparison.InvariantCultureIgnoreCase) ||
		        reason.Contains("no operating theatres", StringComparison.InvariantCultureIgnoreCase));
	}

	public static bool ShouldDeferWithoutTask(ManagerGoalType goalType, string reason)
	{
		return HospitalTheatreStockGoalPlanner.IsDynamicStockShortage(goalType, reason);
	}

	public static bool IsGenericStockSelector(EmploymentItemSelector selector)
	{
		return selector.Kind switch
		{
			EmploymentItemSelectorKind.PrototypeId => true,
			EmploymentItemSelectorKind.Tag => true,
			EmploymentItemSelectorKind.Keyword => !selector.Id.HasValue && selector.Item is null &&
			                                      !string.IsNullOrWhiteSpace(selector.Text),
			_ => false
		};
	}

	public static IReadOnlyCollection<HospitalSupplyStockDeficit> Deficits(IEmploymentTaskContext context,
		HospitalServiceSupplyItemType itemType, int procedureCount, out string reason)
	{
		reason = string.Empty;
		if (context.Employer is not IHospital hospital)
		{
			reason = "Hospital stock conditions can only be evaluated for hospital employment hosts.";
			return [];
		}

		var supplyRooms = hospital.SupplyRooms.OrderBy(x => x.Id).ToList();
		if (!supplyRooms.Any())
		{
			reason = $"{hospital.Name} has no supply rooms configured.";
			return [];
		}

		var requirements = AggregateRequirements(hospital, itemType, Math.Max(1, procedureCount));
		if (!requirements.Any())
		{
			reason = $"{hospital.Name} has no active service {itemType.DescribeEnum()} requirements.";
			return [];
		}

		var available = AvailableStockItems(hospital, itemType, context).DistinctBy(x => x.Id).ToList();
		return requirements
		       .Select(x =>
		       {
			       var current = available.Count(item => ItemThresholdCondition.MatchesSelector(context, item, x.Selector));
			       return new HospitalSupplyStockDeficit(x.Selector, itemType, x.TargetQuantity, current,
				       Math.Max(0, x.TargetQuantity - current));
		       })
		       .Where(x => x.MissingQuantity > 0)
		       .ToList();
	}

	public static bool TryBuildActionPlan(IManagerGoal goal, IEmploymentTaskContext context,
		out EmploymentActionPlan? actionPlan, out string reason)
	{
		if (HospitalTheatreStockGoalPlanner.IsHospitalTheatreStockGoal(goal.GoalType))
		{
			return HospitalTheatreStockGoalPlanner.TryBuildActionPlan(goal, context, out actionPlan, out reason);
		}

		actionPlan = null;
		reason = string.Empty;
		if (context.Employer is not IHospital hospital)
		{
			reason = "Hospital stock manager goals can only run for hospitals.";
			return false;
		}

		var itemType = ItemTypeForGoal(goal.GoalType);
		var condition = goal.Configuration.Conditions?
		                    .OfType<HospitalSupplyStockCondition>()
		                    .LastOrDefault(x => x.ItemType == itemType) ??
		                new HospitalSupplyStockCondition(itemType, 30, "any", null);
		var deficits = Deficits(context, itemType, condition.ProcedureCount, out reason);
		if (!deficits.Any())
		{
			return false;
		}

		var supplyRoom = hospital.SupplyRooms.OrderBy(x => x.Id).FirstOrDefault();
		if (supplyRoom is null)
		{
			reason = $"{hospital.Name} has no supply room to receive purchased stock.";
			return false;
		}

		var lineLimit = ResolveLineLimit(hospital, goal.Policy, condition, deficits.Count);
		if (lineLimit <= 0.0M)
		{
			reason = $"{hospital.Name} has no available funds or configured line maximum for hospital stock purchases.";
			return false;
		}

		var steps = new List<IEmploymentActionStep>();
		foreach (var deficit in deficits)
		{
			var amount = new MoneyAmount(hospital.Currency, lineLimit);
			var description = $"hospital {deficit.ItemType.DescribeEnum()} stock: {deficit.MissingQuantity.ToString("N0", CultureInfo.InvariantCulture)}x {deficit.Description}";
			steps.Add(new CataloguedActionShellStep("authorise", description, amount, supplyRoom));
			steps.Add(new CataloguedActionShellStep("reserve", description, amount, supplyRoom));
			steps.Add(new PurchaseActionStep(deficit.MissingQuantity, deficit.Selector, condition.SupplierSelector,
				hospital.Currency, amount));
			steps.Add(new DeliverItemsActionStep(supplyRoom));
		}

		actionPlan = new EmploymentActionPlan(steps);
		reason = $"Prepared purchase work for {deficits.Count.ToString("N0", CultureInfo.InvariantCulture)} hospital stock line(s).";
		return true;
	}

	private static decimal ResolveLineLimit(IHospital hospital, ManagerGoalPolicy policy,
		HospitalSupplyStockCondition condition, int deficitLineCount)
	{
		if (condition.MaximumLineAmount is > 0.0M)
		{
			return condition.MaximumLineAmount.Value;
		}

		var budget = policy.BudgetLimits.FirstOrDefault(x => x.Currency.Id == hospital.Currency.Id);
		var total = budget?.Amount ?? hospital.AvailableFunds;
		return deficitLineCount <= 0 ? 0.0M : Math.Max(0.0M, total / deficitLineCount);
	}

	private static IReadOnlyCollection<(EmploymentItemSelector Selector, int TargetQuantity)> AggregateRequirements(
		IHospital hospital, HospitalServiceSupplyItemType itemType, int procedureCount)
	{
		var requirements = new Dictionary<string, (EmploymentItemSelector Selector, int TargetQuantity)>(StringComparer.InvariantCultureIgnoreCase);
		foreach (var requirement in hospital.ActiveServices
		                                    .SelectMany(x => x.RequiredEquipment)
		                                    .Where(x => x.ItemType == itemType)
		                                    .Where(x => IsGenericStockSelector(x.Selector)))
		{
			var key = SelectorKey(requirement.Selector);
			var existing = requirements.GetValueOrDefault(key);
			requirements[key] = (requirement.Selector,
				existing.TargetQuantity + Math.Max(1, requirement.Quantity) * procedureCount);
		}

		return requirements.Values.ToList();
	}

	private static IEnumerable<IGameItem> AvailableStockItems(IHospital hospital, HospitalServiceSupplyItemType itemType,
		IEmploymentTaskContext context)
	{
		foreach (var item in hospital.SupplyRooms.SelectMany(room => ItemsInRoom(room, context)))
		{
			yield return item;
		}

		if (itemType != HospitalServiceSupplyItemType.ReusableTool)
		{
			yield break;
		}

		foreach (var item in hospital.OperatingTheatres.SelectMany(room => ItemsInRoom(room, context)))
		{
			yield return item;
		}

		foreach (var contract in hospital.ActiveEmploymentContracts()
		                                 .Where(x => x.Authority.Contains(EmploymentAuthority.PerformMedicalServices)))
		{
			foreach (var item in EmploymentWorkerItemLocator.HeldOrWieldedItems(contract.Employee).SelectMany(DeepItemsOrSelf))
			{
				yield return item;
			}
		}
	}

	private static IEnumerable<IGameItem> ItemsInRoom(ICell room, IEmploymentTaskContext context)
	{
		var contextItems = context.AvailableItems(room).SelectMany(DeepItemsOrSelf);
		var physicalItems = (room.GameItems ?? []).SelectMany(DeepItemsOrSelf);
		return contextItems.Concat(physicalItems).DistinctBy(x => x.Id);
	}

	private static IEnumerable<IGameItem> DeepItemsOrSelf(IGameItem item)
	{
		yield return item;
		foreach (var inner in item.DeepItems ?? [])
		{
			yield return inner;
		}
	}

	private static string SelectorKey(EmploymentItemSelector selector)
	{
		return $"{selector.Kind}|{selector.Id?.ToString("F0", CultureInfo.InvariantCulture) ?? string.Empty}|{selector.Text ?? string.Empty}";
	}
}

internal static class HospitalTheatreStockGoalPlanner
{
	private sealed record TheatreStockRequirement(
		EmploymentItemSelector? Selector,
		TreatmentType? TreatmentType,
		HospitalServiceSupplyItemType ItemType,
		int TargetQuantity)
	{
		public string Key => Selector is not null
			? $"selector:{Selector.Kind}:{Selector.Id?.ToString("F0", CultureInfo.InvariantCulture) ?? string.Empty}:{Selector.Text ?? string.Empty}"
			: $"treatment:{TreatmentType?.ToString() ?? "unknown"}";
	}

	public static bool IsHospitalTheatreStockGoal(ManagerGoalType goalType)
	{
		return goalType is ManagerGoalType.MaintainHospitalTheatreConsumableStock or
			ManagerGoalType.MaintainHospitalTheatreReusableEquipmentStock;
	}

	public static bool IsDynamicStockShortage(ManagerGoalType goalType, string reason)
	{
		return IsHospitalTheatreStockGoal(goalType) &&
		       !string.IsNullOrWhiteSpace(reason) &&
		       reason.Contains("no matching supply-room stock", StringComparison.InvariantCultureIgnoreCase);
	}

	public static IReadOnlyCollection<HospitalTheatreStockDeficit> Deficits(IEmploymentTaskContext context,
		HospitalServiceSupplyItemType itemType, int procedureCount, out string reason)
	{
		reason = string.Empty;
		if (context.Employer is not IHospital hospital)
		{
			reason = "Hospital theatre stock conditions can only be evaluated for hospital employment hosts.";
			return [];
		}

		var theatres = hospital.OperatingTheatres.OrderBy(x => x.Id).ToList();
		if (!theatres.Any())
		{
			reason = $"{hospital.Name} has no operating theatres configured.";
			return [];
		}

		if (!hospital.SupplyRooms.Any())
		{
			reason = $"{hospital.Name} has no supply rooms configured.";
			return [];
		}

		var requirements = AggregateRequirements(hospital, itemType, Math.Max(1, procedureCount));
		if (!requirements.Any())
		{
			reason = $"{hospital.Name} has no active service {itemType.DescribeEnum()} theatre-staging requirements.";
			return [];
		}

		var deficits = new List<HospitalTheatreStockDeficit>();
		foreach (var theatre in theatres)
		{
			var available = ItemsInRoom(theatre, context).DistinctBy(x => x.Id).ToList();
			foreach (var requirement in requirements)
			{
				var current = available.Count(item => MatchesRequirement(context, item, requirement));
				var missing = Math.Max(0, requirement.TargetQuantity - current);
				if (missing <= 0)
				{
					continue;
				}

				deficits.Add(new HospitalTheatreStockDeficit(theatre, requirement.Selector, requirement.TreatmentType,
					itemType, requirement.TargetQuantity, current, missing));
			}
		}

		return deficits;
	}

	public static bool TryBuildActionPlan(IManagerGoal goal, IEmploymentTaskContext context,
		out EmploymentActionPlan? actionPlan, out string reason)
	{
		actionPlan = null;
		reason = string.Empty;
		if (context.Employer is not IHospital hospital)
		{
			reason = "Hospital theatre stock manager goals can only run for hospitals.";
			return false;
		}

		var itemType = HospitalSupplyStockGoalPlanner.ItemTypeForGoal(goal.GoalType);
		var condition = goal.Configuration.Conditions?
		                    .OfType<HospitalTheatreStockCondition>()
		                    .LastOrDefault(x => x.ItemType == itemType) ??
		                new HospitalTheatreStockCondition(itemType, 1);
		var deficits = Deficits(context, itemType, condition.ProcedureCount, out reason);
		if (!deficits.Any())
		{
			return false;
		}

		var selections = SelectSupplyRoomItems(hospital, context, deficits).ToList();
		if (!selections.Any())
		{
			reason = $"{hospital.Name} has theatre-staging deficits but no matching supply-room stock is available.";
			return false;
		}

		var steps = new List<IEmploymentActionStep>();
		foreach (var group in selections.GroupBy(x => x.Theatre.Id).OrderBy(x => x.Key))
		{
			var theatre = group.First().Theatre;
			var selected = group.DistinctBy(x => x.Item.Id).ToList();
			steps.Add(new GetItemsByIdActionStep(selected.Count, Enumerable.Empty<long>(),
				selected.Select(x => x.Source).DistinctBy(x => x.Id),
				selected.Select(x => x.Item.Id)));
			steps.Add(new DeliverItemsActionStep(theatre));
		}

		actionPlan = new EmploymentActionPlan(steps);
		reason = $"Prepared operating-theatre staging work for {selections.Select(x => x.Item.Id).Distinct().Count().ToString("N0", CultureInfo.InvariantCulture)} hospital stock item(s).";
		return true;
	}

	private static IReadOnlyCollection<TheatreStockRequirement> AggregateRequirements(IHospital hospital,
		HospitalServiceSupplyItemType itemType, int procedureCount)
	{
		var requirements = new Dictionary<string, TheatreStockRequirement>(StringComparer.InvariantCultureIgnoreCase);
		foreach (var requirement in hospital.ActiveServices
		                                    .SelectMany(x => x.RequiredEquipment)
		                                    .Where(x => x.ItemType == itemType)
		                                    .Where(x => HospitalSupplyStockGoalPlanner.IsGenericStockSelector(x.Selector)))
		{
			var targetQuantity = Math.Max(1, requirement.Quantity) * procedureCount;
			var next = new TheatreStockRequirement(requirement.Selector, null, itemType, targetQuantity);
			requirements[next.Key] = requirements.TryGetValue(next.Key, out var existing)
				? next with { TargetQuantity = existing.TargetQuantity + targetQuantity }
				: next;
		}

		if (itemType != HospitalServiceSupplyItemType.Consumable)
		{
			return requirements.Values.ToList();
		}

		foreach (var treatmentType in hospital.ActiveServices
		                                  .Where(HospitalMedicalServiceRunner.UsesCommandRoutedWoundCare)
		                                  .SelectMany(HospitalMedicalServiceRunner.ImplicitTreatmentSupplyTypes))
		{
			var next = new TheatreStockRequirement(null, treatmentType, itemType, procedureCount);
			requirements[next.Key] = requirements.TryGetValue(next.Key, out var existing)
				? next with { TargetQuantity = existing.TargetQuantity + procedureCount }
				: next;
		}

		return requirements.Values.ToList();
	}

	private static IEnumerable<(ICell Theatre, ICell Source, IGameItem Item)> SelectSupplyRoomItems(IHospital hospital,
		IEmploymentTaskContext context, IEnumerable<HospitalTheatreStockDeficit> deficits)
	{
		var used = new HashSet<long>();
		var supplyItems = hospital.SupplyRooms
		                          .OrderBy(x => x.Id)
		                          .SelectMany(room => ItemsInRoom(room, context).Select(item => (Source: room, Item: item)))
		                          .DistinctBy(x => x.Item.Id)
		                          .ToList();
		foreach (var deficit in deficits.OrderBy(x => x.Theatre.Id).ThenBy(x => x.Description))
		{
			var requirement = new TheatreStockRequirement(deficit.Selector, deficit.TreatmentType, deficit.ItemType,
				deficit.MissingQuantity);
			var selected = supplyItems
			               .Where(x => !used.Contains(x.Item.Id))
			               .Where(x => MatchesRequirement(context, x.Item, requirement))
			               .Take(deficit.MissingQuantity)
			               .ToList();
			foreach (var item in selected)
			{
				used.Add(item.Item.Id);
				yield return (deficit.Theatre, item.Source, item.Item);
			}
		}
	}

	private static bool MatchesRequirement(IEmploymentTaskContext context, IGameItem item,
		TheatreStockRequirement requirement)
	{
		if (requirement.Selector is not null)
		{
			return ItemThresholdCondition.MatchesSelector(context, item, requirement.Selector);
		}

		return requirement.TreatmentType is { } treatmentType &&
		       item.GetItemType<ITreatment>() is { } treatment &&
		       treatment.IsTreatmentType(treatmentType);
	}

	private static IEnumerable<IGameItem> ItemsInRoom(ICell room, IEmploymentTaskContext context)
	{
		var contextItems = context.AvailableItems(room).SelectMany(DeepItemsOrSelf);
		var physicalItems = (room.GameItems ?? []).SelectMany(DeepItemsOrSelf);
		return contextItems.Concat(physicalItems).DistinctBy(x => x.Id);
	}

	private static IEnumerable<IGameItem> DeepItemsOrSelf(IGameItem item)
	{
		yield return item;
		foreach (var inner in item.DeepItems ?? [])
		{
			yield return inner;
		}
	}
}

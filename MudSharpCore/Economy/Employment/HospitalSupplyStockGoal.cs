using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.GameItems;

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

internal sealed record HospitalSupplyStockDeficit(
	EmploymentItemSelector Selector,
	HospitalServiceSupplyItemType ItemType,
	int TargetQuantity,
	int CurrentQuantity,
	int MissingQuantity)
{
	public string Description => EmploymentItemSelectorResolver.Describe(Selector);
}

internal static class HospitalSupplyStockGoalPlanner
{
	public static bool IsHospitalStockGoal(ManagerGoalType goalType)
	{
		return goalType is ManagerGoalType.MaintainHospitalConsumableStock or
			ManagerGoalType.MaintainHospitalReusableEquipmentStock;
	}

	public static HospitalServiceSupplyItemType ItemTypeForGoal(ManagerGoalType goalType)
	{
		return goalType == ManagerGoalType.MaintainHospitalReusableEquipmentStock
			? HospitalServiceSupplyItemType.ReusableTool
			: HospitalServiceSupplyItemType.Consumable;
	}

	public static bool IsConfigurationBlocker(string reason)
	{
		return !string.IsNullOrWhiteSpace(reason) &&
		       (reason.Contains("can only", StringComparison.InvariantCultureIgnoreCase) ||
		        reason.Contains("no supply rooms", StringComparison.InvariantCultureIgnoreCase));
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
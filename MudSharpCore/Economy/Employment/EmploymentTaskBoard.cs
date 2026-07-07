using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.Vehicles;

#nullable enable

namespace MudSharp.Economy.Employment;

public sealed class EmploymentTaskContext : IEmploymentTaskContext
{
	private sealed record CommodityProfile(
		long ItemId,
		string MaterialName,
		string? TagName,
		IReadOnlyDictionary<string, string> Characteristics,
		double Weight);

	private readonly Dictionary<string, bool> _manualOrders = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly Dictionary<string, int> _stockLevels = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly Dictionary<string, decimal> _accountBalances = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly HashSet<IEmploymentActionStep> _paymentAuthorisations = new();
	private readonly HashSet<string> _allowedCommands = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly HashSet<long> _unreachableCellIds = new();
	private readonly Dictionary<long, List<IGameItem>> _locationItems = new();
	private readonly HashSet<long> _configuredLocationItems = new();
	private readonly Dictionary<long, List<IGameItem>> _carriedTaskItems = new();
	private readonly Dictionary<long, HashSet<long>> _contextManagedCarriedTaskItemIds = new();
	private readonly Dictionary<long, List<IGameItem>> _containerContents = new();
	private readonly Dictionary<long, HashSet<long>> _loadedTaskItemIds = new();
	private readonly Dictionary<long, HashSet<string>> _itemTags = new();
	private readonly List<CommodityProfile> _commodityProfiles = new();
	private readonly HashSet<long> _transportBundleIds = new();
	private readonly bool _usePhysicalItemMovement;
	private readonly HashSet<long> _additionalLogisticsLocationIds;
	private IEmploymentActiveTask? _currentTask;
	private int _currentStepIndex;

	public EmploymentTaskContext(IEmploymentHost employer, bool usePhysicalItemMovement = false,
		IEnumerable<ICell>? additionalLogisticsLocations = null)
	{
		Employer = employer;
		_usePhysicalItemMovement = usePhysicalItemMovement;
		_additionalLogisticsLocationIds = additionalLogisticsLocations?.Select(x => x.Id).ToHashSet() ?? [];
	}

	public IEmploymentHost Employer { get; }
	internal IEmploymentActiveTask? CurrentTask => _currentTask;
	internal int CurrentStepIndex => _currentStepIndex;

	public void SetManualOrder(string key, bool active)
	{
		_manualOrders[key] = active;
	}

	public bool ManualOrderActive(string key)
	{
		return _manualOrders.TryGetValue(key, out var active) && active;
	}

	public void SetStockLevel(string stockKey, int level)
	{
		_stockLevels[stockKey] = level;
	}

	public int StockLevel(string stockKey)
	{
		return _stockLevels.GetValueOrDefault(stockKey);
	}

	public void SetAccountBalance(string accountKey, decimal balance)
	{
		_accountBalances[accountKey] = balance;
	}

	public decimal AccountBalance(string accountKey)
	{
		return _accountBalances.GetValueOrDefault(accountKey);
	}

	public void AuthorisePaymentFor(IEmploymentActionStep step, ICharacter? actor = null, Guid? correlationId = null,
		bool recordRegister = true)
	{
		if (!_paymentAuthorisations.Add(step) || !recordRegister)
		{
			return;
		}

		RecordRegister(EmploymentRegisterEntryType.PaymentAuthorisationGranted, actor,
			$"Payment authorised for {step.StepType} action.", correlationId);
	}

	public bool PaymentAuthorised(IEmploymentActionStep step)
	{
		return !step.RequiresPaymentAuthorisation || _paymentAuthorisations.Contains(step);
	}

	public void AllowCommand(string commandName)
	{
		_allowedCommands.Add(commandName);
	}

	public bool CommandAllowed(string commandName)
	{
		return _allowedCommands.Contains(commandName);
	}

	public void SetPathBlocked(ICell cell)
	{
		_unreachableCellIds.Add(cell.Id);
	}

	public bool CanPath(ICharacter actor, ICell? destination)
	{
		return destination is null || !_unreachableCellIds.Contains(destination.Id);
	}

	private bool HasHostLogisticsBoundary => _usePhysicalItemMovement && Employer.EmploymentHostLocations().Any();

	private bool IsHostLogisticsLocation(ICell? location)
	{
		return !HasHostLogisticsBoundary ||
		       location is null ||
		       Employer.EmploymentHostLocations().Any(x => x.Id == location.Id) ||
		       _additionalLogisticsLocationIds.Contains(location.Id);
	}

	private bool TryRequireHostLogisticsLocation(ICell? location, string action, out string reason)
	{
		if (IsHostLogisticsLocation(location))
		{
			reason = string.Empty;
			return true;
		}

		reason = $"Employment logistics can only {action} within this host's assigned work locations.";
		return false;
	}

	public void SetAvailableItems(ICell location, IEnumerable<IGameItem> items)
	{
		_locationItems[location.Id] = items.ToList();
		_configuredLocationItems.Add(location.Id);
	}

	public IReadOnlyCollection<IGameItem> AvailableItems(ICell location)
	{
		if (_usePhysicalItemMovement && !_configuredLocationItems.Contains(location.Id))
		{
			if (!IsHostLogisticsLocation(location))
			{
				return [];
			}

			return location.GameItems
			               .SelectMany(x => x.DeepItems)
			               .DistinctBy(x => x.Id)
			               .ToList();
		}

		return LocationItems(location);
	}

	public IReadOnlyCollection<IGameItem> CarriedTaskItems(ICharacter actor)
	{
		if (!_carriedTaskItems.TryGetValue(CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor), out var items))
		{
			return [];
		}

		if (_usePhysicalItemMovement && CanUseInventoryPlan(actor))
		{
			items.RemoveAll(x => !IsContextManagedCarriedTaskItem(actor, x) && !ActorCarriesItem(actor, x));
			AddHeldTransportBundles(actor, items);
			RemoveTransportBundleContents(items);
		}

		return items;
	}

	private void AddHeldTransportBundles(ICharacter actor, List<IGameItem> items)
	{
		if (!_transportBundleIds.Any())
		{
			return;
		}

		foreach (var bundle in EmploymentWorkerItemLocator.HeldOrWieldedItems(actor)
		                                      .Where(x => _transportBundleIds.Contains(x.Id)))
		{
			if (items.All(x => x.Id != bundle.Id))
			{
				items.Add(bundle);
			}
		}
	}

	private void RemoveTransportBundleContents(List<IGameItem> items)
	{
		var bundledItemIds = items
		                     .Where(x => _transportBundleIds.Contains(x.Id))
		                     .SelectMany(x => x.GetItemType<PileGameItemComponent>()?.Contents ?? Enumerable.Empty<IGameItem>())
		                     .Select(x => x.Id)
		                     .ToHashSet();
		if (!bundledItemIds.Any())
		{
			return;
		}

		items.RemoveAll(x => bundledItemIds.Contains(x.Id) && !_transportBundleIds.Contains(x.Id));
	}

	internal IReadOnlyCollection<long> ContextManagedCarriedTaskItemIds(ICharacter actor)
	{
		return _contextManagedCarriedTaskItemIds.TryGetValue(CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor),
			out var items)
			? items
			: [];
	}

	internal bool IsContextManagedCarriedTaskItem(ICharacter actor, IGameItem item)
	{
		return _contextManagedCarriedTaskItemIds.TryGetValue(CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor),
			       out var items) &&
		       items.Contains(item.Id);
	}

	public IReadOnlyCollection<IGameItem> ContainedItems(IGameItem container)
	{
		return _containerContents.TryGetValue(container.Id, out var items) ? items : [];
	}

	public IReadOnlyCollection<IGameItem> LoadedTaskItems(ICharacter actor, IGameItem container)
	{
		return ResolveLoadedTaskItems(actor, container).DistinctBy(x => x.Id).ToList();
	}

	public bool CanAssignVehicle(ICharacter actor, IVehicle vehicle, out string reason)
	{
		if (vehicle.Disabled || vehicle.Destroyed)
		{
			reason = $"{vehicle.Name} is not available for employment vehicle work.";
			return false;
		}

		var vehicleLocation = vehicle.Location;
		if (vehicleLocation is null)
		{
			reason = $"{vehicle.Name} is not currently at a reachable location.";
			return false;
		}

		if (!CanPath(actor, vehicleLocation))
		{
			reason = "The assigned employee cannot path to the vehicle.";
			return false;
		}

		if (TryGetActiveVehicleAssignment(vehicle.Id, out var taskName, out var driverId))
		{
			reason = $"{vehicle.Name} is already assigned to employment task {taskName} by driver #{driverId:N0}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public bool TryAssignVehicle(ICharacter actor, IVehicle vehicle, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!CanAssignVehicle(actor, vehicle, out reason))
		{
			return false;
		}

		var actorId = CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor);
		operationalState = new EmploymentActionStepOperationalState(
			OperationalPayload: $"vehicle-status=assigned;vehicle={vehicle.Id:F0};driver={actorId:F0}",
			SelectedResources: FormatVehicleAssignment(vehicle.Id, actorId));
		RecordRegister(EmploymentRegisterEntryType.AuditActionRecorded, actor,
			$"Assigned vehicle {vehicle.Name} to driver {actor.Name} for employment vehicle work.",
			CurrentTask?.CorrelationId);
		reason = string.Empty;
		return true;
	}

	public void HydrateTaskState(IEmploymentActiveTask task, int currentStepIndex)
	{
		_currentTask = task;
		_currentStepIndex = currentStepIndex;
		var previousCarriedItems = new List<IGameItem>();
		var previousContextManagedItemIds = new HashSet<long>();
		if (_usePhysicalItemMovement && task.AssignedEmployee is not null)
		{
			var assignedEmployeeKey = CharacterInstanceIdentityComparer.PhysicalInstanceKey(task.AssignedEmployee);
			if (_carriedTaskItems.TryGetValue(assignedEmployeeKey, out var existingCarried))
			{
				previousCarriedItems = existingCarried.ToList();
			}

			if (_contextManagedCarriedTaskItemIds.TryGetValue(assignedEmployeeKey, out var existingContextManaged))
			{
				previousContextManagedItemIds = existingContextManaged.ToHashSet();
			}

			_carriedTaskItems.Remove(assignedEmployeeKey);
			_contextManagedCarriedTaskItemIds.Remove(assignedEmployeeKey);
		}

		void HydrateCarriedItems(ICharacter employee, IEnumerable<IGameItem> sourceItems, IEnumerable<long> managedIds)
		{
			var itemList = sourceItems.DistinctBy(x => x.Id).ToList();
			if (!itemList.Any())
			{
				return;
			}

			var contextManagedIds = managedIds.ToHashSet();
			var contextManagedItems = itemList.Where(x => contextManagedIds.Contains(x.Id)).ToList();
			var physicalItems = _usePhysicalItemMovement
				? itemList
				  .Where(x => !contextManagedIds.Contains(x.Id) && ActorCarriesItem(employee, x))
				  .ToList()
				: itemList.Where(x => !contextManagedIds.Contains(x.Id)).ToList();

			AddCarriedTaskItems(employee, physicalItems);
			AddCarriedTaskItems(employee, contextManagedItems, contextManaged: true);
		}

		var stateCount = Math.Max(0, currentStepIndex);
		if (currentStepIndex >= 0 &&
		    currentStepIndex < task.StepStates.Count &&
		    task.StepStates[currentStepIndex] is EmploymentActionStepStatus.InProgress or EmploymentActionStepStatus.Blocked)
		{
			stateCount++;
		}

		foreach (var state in task.StepOperationalStates.Take(stateCount))
		{
			if (TryParseTaskItemCustody(state.SelectedResources, out var custodyOperation, out _, out var custodyItemIds,
				    out var custodyBundleIds, out var custodyContextManagedItemIds) &&
			    task.AssignedEmployee is not null)
			{
				var items = custodyItemIds
				            .Concat(custodyBundleIds)
				            .Select(x => task.AssignedEmployee.Gameworld?.TryGetItem(x, true) ??
				                         previousCarriedItems.FirstOrDefault(y => y.Id == x))
				            .Where(x => x is not null)
				            .Cast<IGameItem>()
				            .DistinctBy(x => x.Id)
				            .ToList();
				if (custodyOperation.EqualTo("collect") || custodyOperation.EqualTo("unload"))
				{
					foreach (var bundleId in custodyBundleIds)
					{
						_transportBundleIds.Add(bundleId);
					}

					var contextManagedIds = custodyContextManagedItemIds
					                        .Concat(previousContextManagedItemIds.Where(x => custodyItemIds.Contains(x)))
					                        .ToHashSet();
					HydrateCarriedItems(task.AssignedEmployee, items, contextManagedIds);
				}
				else if (custodyOperation.EqualTo("load"))
				{
					RemoveCarriedTaskItems(task.AssignedEmployee, items);
				}
				else if (custodyOperation.EqualTo("deliver") || custodyOperation.EqualTo("return"))
				{
					RemoveCarriedTaskItems(task.AssignedEmployee, items);
					foreach (var bundleId in custodyBundleIds.Concat(custodyItemIds))
					{
						_transportBundleIds.Remove(bundleId);
					}
				}
			}

			if (!TryParseLoadedAssets(state.LoadedAssets, out var operation, out var containerId, out var itemIds))
			{
				continue;
			}

			if (!_loadedTaskItemIds.TryGetValue(containerId, out var loadedIds))
			{
				loadedIds = new HashSet<long>();
				_loadedTaskItemIds[containerId] = loadedIds;
			}

			if (operation.EqualTo("load"))
			{
				foreach (var itemId in itemIds)
				{
					loadedIds.Add(itemId);
				}

				if (task.AssignedEmployee is not null)
				{
					var items = itemIds
					            .Select(x => task.AssignedEmployee.Gameworld?.TryGetItem(x, true))
					            .Where(x => x is not null)
					            .Cast<IGameItem>()
					            .ToList();
					RemoveCarriedTaskItems(task.AssignedEmployee, items);
				}

				continue;
			}

			foreach (var itemId in itemIds)
			{
				loadedIds.Remove(itemId);
			}

			if (operation.EqualTo("unload") && task.AssignedEmployee is not null)
			{
				var items = itemIds
				            .Select(x => task.AssignedEmployee.Gameworld?.TryGetItem(x, true))
				            .Where(x => x is not null)
				            .Cast<IGameItem>()
				            .ToList();
				HydrateCarriedItems(task.AssignedEmployee, items,
					previousContextManagedItemIds.Where(x => itemIds.Contains(x)));
			}
		}
	}
	public bool CanReserveFunds(MoneyAmount amount, out string reason)
	{
		return EmploymentFinanceService.CanReserveFunds(this, amount, out reason);
	}

	public bool TryReserveFunds(MoneyAmount amount, ICharacter actor, string description, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		return EmploymentFinanceService.TryReserveFunds(this, amount, actor, description, out reason, out operationalState);
	}

	public bool HasReservedFunds(MoneyAmount amount, out string reason)
	{
		return EmploymentFinanceService.HasReservedFunds(this, amount, out reason);
	}

	public bool TryReleaseReservedFunds(ICharacter actor, string selector, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		return EmploymentFinanceService.TryReleaseReservedFunds(this, actor, selector, out reason, out operationalState);
	}

	public bool CanBankDeposit(MoneyAmount amount, out string reason)
	{
		return EmploymentFinanceService.CanBankDeposit(this, amount, out reason);
	}

	public bool CanBankWithdrawal(MoneyAmount amount, out string reason)
	{
		return EmploymentFinanceService.CanBankWithdrawal(this, amount, out reason);
	}

	public bool TryBankDeposit(ICharacter actor, MoneyAmount amount, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		return EmploymentFinanceService.TryBankDeposit(this, actor, amount, out reason, out operationalState);
	}

	public bool TryBankWithdrawal(ICharacter actor, MoneyAmount amount, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		return EmploymentFinanceService.TryBankWithdrawal(this, actor, amount, out reason, out operationalState);
	}

	public bool CanBankTransfer(string targetAccountKey, MoneyAmount amount, out string reason)
	{
		return EmploymentFinanceService.CanBankTransfer(this, targetAccountKey, amount, out reason);
	}

	public bool TryBankTransfer(ICharacter actor, string targetAccountKey, MoneyAmount amount, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		return EmploymentFinanceService.TryBankTransfer(this, actor, targetAccountKey, amount, out reason,
			out operationalState);
	}
	public bool CanHostSettlement(string targetHostKey, MoneyAmount amount, out string reason)
	{
		return EmploymentFinanceService.CanHostSettlement(this, targetHostKey, amount, out reason);
	}

	public bool TryHostSettlement(ICharacter actor, string targetHostKey, MoneyAmount amount, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		return EmploymentFinanceService.TryHostSettlement(this, actor, targetHostKey, amount, out reason,
			out operationalState);
	}
	public bool CanPurchase(IEmploymentActionStep step, out string reason)
	{
		return EmploymentFinanceService.CanPurchase(this, step, out reason);
	}

	public bool TryPurchase(ICharacter actor, IEmploymentActionStep step, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		return EmploymentFinanceService.TryPurchase(this, actor, step, out reason, out operationalState);
	}

	public bool CanStoreAccountPayment(string accountKey, MoneyAmount amount, out string reason)
	{
		return EmploymentFinanceService.CanStoreAccountPayment(this, accountKey, amount, out reason);
	}

	public bool TryStoreAccountPayment(ICharacter actor, string accountKey, MoneyAmount amount, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		return EmploymentFinanceService.TryStoreAccountPayment(this, actor, accountKey, amount, out reason,
			out operationalState);
	}

	public bool CanPayTaxes(MoneyAmount? maximumAmount, out string reason, out MoneyAmount? amount)
	{
		return EmploymentFinanceService.CanPayTaxes(this, maximumAmount, out reason, out amount);
	}

	public bool TryPayTaxes(ICharacter actor, MoneyAmount? maximumAmount, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		return EmploymentFinanceService.TryPayTaxes(this, actor, maximumAmount, out reason, out operationalState);
	}

	public bool CanAdjustShopFloat(MoneyAmount amount, bool fillRegister, EmploymentItemSelector? registerSelector,
		out string reason)
	{
		return EmploymentFinanceService.CanAdjustShopFloat(this, amount, fillRegister, registerSelector, out reason);
	}

	public bool TryAdjustShopFloat(ICharacter actor, MoneyAmount amount, bool fillRegister,
		EmploymentItemSelector? registerSelector, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		return EmploymentFinanceService.TryAdjustShopFloat(this, actor, amount, fillRegister, registerSelector,
			out reason, out operationalState);
	}

	public bool CanHandlePhysicalFloat(PhysicalFloatOperation operation, MoneyAmount? amount, string targetKind,
		EmploymentItemSelector? targetSelector, out string reason)
	{
		return EmploymentFinanceService.CanHandlePhysicalFloat(this, operation, amount, targetKind, targetSelector,
			out reason);
	}

	public bool TryHandlePhysicalFloat(ICharacter actor, PhysicalFloatOperation operation, MoneyAmount? amount,
		string targetKind, EmploymentItemSelector? targetSelector, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		return EmploymentFinanceService.TryHandlePhysicalFloat(this, actor, operation, amount, targetKind,
			targetSelector, out reason, out operationalState);
	}

	public bool CanUseCraftStation(ICharacter actor, string stationSelector, out string reason)
	{
		return EmploymentCraftService.CanUseCraftStation(this, actor, stationSelector, out reason);
	}

	public bool TryUseCraftStation(ICharacter actor, string stationSelector, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		return EmploymentCraftService.TryUseCraftStation(this, actor, stationSelector, out reason,
			out operationalState);
	}

	public bool CanStartCraft(string craftSelector, ICharacter actor, out string reason)
	{
		return EmploymentCraftService.CanStartCraft(this, craftSelector, actor, out reason);
	}

	public bool TryStartCraft(ICharacter actor, string craftSelector, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		return EmploymentCraftService.TryStartCraft(this, actor, craftSelector, out reason, out operationalState);
	}

	public void SetItemTags(IGameItem item, params string[] tags)
	{
		_itemTags[item.Id] = new HashSet<string>(tags, StringComparer.InvariantCultureIgnoreCase);
	}

	public bool ItemHasTag(IGameItem item, string tagName)
	{
		if (string.IsNullOrWhiteSpace(tagName))
		{
			return false;
		}

		if (_itemTags.TryGetValue(item.Id, out var configuredTags) && configuredTags.Contains(tagName))
		{
			return true;
		}

		return item.Tags.Any(x =>
			x.Name.EqualTo(tagName) ||
			x.FullName.EqualTo(tagName) ||
			x.Id.ToString("F0").EqualTo(tagName));
	}

	public void SetCommodityWeight(IGameItem item, string materialName, double weight, string? tagName = null,
		IReadOnlyDictionary<string, string>? characteristics = null)
	{
		_commodityProfiles.RemoveAll(x => x.ItemId == item.Id);
		_commodityProfiles.Add(new CommodityProfile(
			item.Id,
			materialName,
			tagName,
			new Dictionary<string, string>(characteristics ?? new Dictionary<string, string>(),
				StringComparer.InvariantCultureIgnoreCase),
			weight));
	}

	public double CommodityWeight(IGameItem item, string materialName, string? tagName,
		IReadOnlyDictionary<string, string> characteristics)
	{
		var configured = _commodityProfiles.FirstOrDefault(x =>
			x.ItemId == item.Id &&
			x.MaterialName.EqualTo(materialName) &&
			(string.IsNullOrWhiteSpace(tagName) || (x.TagName?.EqualTo(tagName) ?? false)) &&
			CharacteristicsMatch(x.Characteristics, characteristics));
		if (configured is not null)
		{
			return configured.Weight;
		}

		var commodity = item.GetItemType<ICommodity>();
		if (commodity is null)
		{
			return 0.0;
		}

		if (!commodity.Material.Name.EqualTo(materialName) &&
		    !commodity.Material.Id.ToString("F0").EqualTo(materialName))
		{
			return 0.0;
		}

		if (!string.IsNullOrWhiteSpace(tagName) &&
		    !(commodity.Tag?.Name.EqualTo(tagName) == true ||
		      commodity.Tag?.FullName.EqualTo(tagName) == true ||
		      commodity.Tag?.Id.ToString("F0").EqualTo(tagName) == true))
		{
			return 0.0;
		}

		if (!CommodityCharacteristicsMatch(commodity, characteristics))
		{
			return 0.0;
		}

		return commodity.Weight;
	}

	public bool TryCollectTaskItem(ICharacter actor, IGameItem item, ICell source, out string reason)
	{
		return TryCollectTaskItems(actor, [(item, source)], out reason);
	}

	public bool TryCollectTaskItems(ICharacter actor, IReadOnlyCollection<(IGameItem Item, ICell Source)> items,
		out string reason)
	{
		if (!items.Any())
		{
			reason = "There are no task items to collect.";
			return false;
		}

		foreach (var source in items.Select(x => x.Source).DistinctBy(x => x.Id))
		{
			if (!TryRequireHostLogisticsLocation(source, "collect task items", out reason))
			{
				return false;
			}

			if (!CanPath(actor, source))
			{
				reason = "The assigned employee cannot path to the source location.";
				return false;
			}
		}

		var resolvedItems = new List<(ICell Source, IGameItem Item)>();
		foreach (var (item, source) in items)
		{
			var sourceItems = LocationItems(source);
			var index = sourceItems.FindIndex(x => x.Id == item.Id);
			if (index < 0 && (!_usePhysicalItemMovement || !source.GameItems.SelectMany(x => x.DeepItems).Any(x => x.Id == item.Id)))
			{
				reason = "The item is no longer at the source location.";
				return false;
			}

			resolvedItems.Add((source, index >= 0
				? sourceItems[index]
				: source.GameItems.SelectMany(x => x.DeepItems).First(x => x.Id == item.Id)));
		}

		var contextManagedCollection = !_usePhysicalItemMovement ||
		                               resolvedItems.Any(x => _configuredLocationItems.Contains(x.Source.Id)) ||
		                               !CanUseInventoryPlan(actor);
		if (contextManagedCollection)
		{
			foreach (var (source, item) in resolvedItems)
			{
				LocationItems(source).RemoveAll(x => x.Id == item.Id);
				if (_usePhysicalItemMovement && !_configuredLocationItems.Contains(source.Id))
				{
					item.InInventoryOf?.Take(item);
					item.ContainedIn?.Take(item);
					source.Extract(item);
				}
			}

			AddCarriedTaskItems(actor, resolvedItems.Select(x => x.Item), contextManaged: contextManagedCollection);
			reason = string.Empty;
			return true;
		}

		var physicalSources = resolvedItems.Select(x => x.Source).DistinctBy(x => x.Id).ToList();
		if (physicalSources.Count > 1)
		{
			reason = "A worker can only physically collect task items from one source location at a time.";
			return false;
		}

		var physicalSource = physicalSources.Single();
		if (actor.Location?.Id != physicalSource.Id)
		{
			reason = "The assigned employee must be at the source location to collect task items.";
			return false;
		}

		var selectedItems = resolvedItems.Select(x => x.Item).DistinctBy(x => x.Id).ToList();
		if (selectedItems.Count > 1 &&
		    selectedItems.All(x => x.CanBeBundled) &&
		    PileGameItemComponentProto.ItemPrototype is not null)
		{
			return TryBundleAndCollectTaskItems(actor, physicalSource, selectedItems, out reason);
		}

		if (!TryHoldTaskItemsWithInventoryPlan(actor, selectedItems, out var collectedItems, out reason))
		{
			return false;
		}

		foreach (var item in selectedItems)
		{
			LocationItems(physicalSource).RemoveAll(x => x.Id == item.Id);
		}

		AddCarriedTaskItems(actor, collectedItems);
		reason = string.Empty;
		return true;
	}

	public bool TryDeliverTaskItems(ICharacter actor, ICell destination, IGameItem? container, string? containerTag,
		out string reason)
	{
		if (!TryRequireHostLogisticsLocation(destination, "deliver task items", out reason))
		{
			return false;
		}

		if (!CanPath(actor, destination))
		{
			reason = "The assigned employee cannot path to the delivery destination.";
			return false;
		}

		if (!_carriedTaskItems.TryGetValue(CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor), out var carried) || carried.Count == 0)
		{
			reason = "The assigned employee is not carrying any task items to deliver.";
			return false;
		}

		if (_usePhysicalItemMovement &&
		    !_configuredLocationItems.Contains(destination.Id) &&
		    CanUseInventoryPlan(actor) &&
		    carried.All(x => !IsContextManagedCarriedTaskItem(actor, x)))
		{
			carried.RemoveAll(x => !ActorCarriesItem(actor, x));
			if (carried.Count == 0)
			{
				reason = "The assigned employee is no longer carrying any task items to deliver.";
				return false;
			}
		}

		var destinationItems = LocationItems(destination);
		var targetContainer = container;
		if (targetContainer is null && !string.IsNullOrWhiteSpace(containerTag))
		{
			targetContainer = AvailableItems(destination).FirstOrDefault(x => ItemHasTag(x, containerTag));
		}

		if (targetContainer is null && !string.IsNullOrWhiteSpace(containerTag))
		{
			reason = $"There is no destination container tagged {containerTag}.";
			return false;
		}

		if (_usePhysicalItemMovement &&
		    !_configuredLocationItems.Contains(destination.Id) &&
		    CanUseInventoryPlan(actor) &&
		    carried.All(x => !IsContextManagedCarriedTaskItem(actor, x)))
		{
			return TryDeliverPhysicalTaskItems(actor, destination, targetContainer, destinationItems, carried, out reason);
		}

		var carriedDeliveryItems = carried
		                           .SelectMany(DeliveryItemsFor)
		                           .ToList();
		if (targetContainer is null)
		{
			destinationItems.AddRange(carriedDeliveryItems);
		}
		else
		{
			var containerComponent = targetContainer.GetItemType<IContainer>();
			if (containerComponent is null)
			{
				reason = $"{targetContainer.Name} is not a container.";
				return false;
			}

			var rejected = carriedDeliveryItems.FirstOrDefault(x => !containerComponent.CanPut(x));
			if (rejected is not null)
			{
				reason = $"{targetContainer.Name} cannot contain {rejected.Name}.";
				return false;
			}

			if (!_containerContents.TryGetValue(targetContainer.Id, out var contents))
			{
				contents = new List<IGameItem>();
				_containerContents[targetContainer.Id] = contents;
			}

			contents.AddRange(carriedDeliveryItems);
		}

		RemoveCarriedTaskItems(actor, carried.ToList());
		reason = string.Empty;
		return true;
	}

	public bool TryLoadCarriedTaskItems(ICharacter actor, IGameItem targetContainer, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!_carriedTaskItems.TryGetValue(CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor), out var carried) || carried.Count == 0)
		{
			reason = "The assigned employee is not carrying any task items to load.";
			return false;
		}

		var containerComponent = targetContainer.GetItemType<IContainer>();
		if (containerComponent is null)
		{
			reason = $"{targetContainer.Name} is not a container.";
			return false;
		}

		if (_usePhysicalItemMovement &&
		    EmploymentInventoryPlanLogistics.CanUseInventoryPlan(actor) &&
		    carried.All(x => !IsContextManagedCarriedTaskItem(actor, x)))
		{
			carried.RemoveAll(x => !ActorCarriesItem(actor, x));
			if (carried.Count == 0)
			{
				reason = "The assigned employee is no longer carrying any task items to load.";
				return false;
			}

			if (!EmploymentInventoryPlanLogistics.TryPutItemsIntoContainer(actor, carried.ToList(), targetContainer,
				    out var loadedItems, out reason))
			{
				return false;
			}

			MarkLoadedTaskItems(targetContainer, loadedItems);
			RemoveCarriedTaskItems(actor, loadedItems);
			RecordContainerContents(targetContainer, loadedItems);
			operationalState = new EmploymentActionStepOperationalState(
				LoadedAssets: FormatLoadedAssets("load", targetContainer.Id, loadedItems),
				ReservationReference: FormatLoadReservation("reserve", targetContainer.Id, loadedItems));
			return true;
		}

		var rejected = carried.FirstOrDefault(x => !containerComponent.CanPut(x));
		if (rejected is not null)
		{
			reason = $"{targetContainer.Name} cannot contain {rejected.Name}.";
			return false;
		}

		var loaded = carried.ToList();
		MarkLoadedTaskItems(targetContainer, loaded);
		RecordContainerContents(targetContainer, loaded);
		RemoveCarriedTaskItems(actor, carried.ToList());
		operationalState = new EmploymentActionStepOperationalState(
			LoadedAssets: FormatLoadedAssets("load", targetContainer.Id, loaded),
			ReservationReference: FormatLoadReservation("reserve", targetContainer.Id, loaded));
		reason = string.Empty;
		return true;
	}

	public bool TryUnloadTaskItems(ICharacter actor, IGameItem sourceContainer, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		var loadedItems = ResolveLoadedTaskItems(actor, sourceContainer).DistinctBy(x => x.Id).ToList();
		if (!loadedItems.Any())
		{
			reason = $"There are no task-loaded items recorded in {sourceContainer.Name}.";
			return false;
		}

		if (_usePhysicalItemMovement && EmploymentInventoryPlanLogistics.CanUseInventoryPlan(actor))
		{
			if (!EmploymentInventoryPlanLogistics.TryHoldItems(actor, loadedItems, out var unloadedItems, out reason))
			{
				return false;
			}

			RemoveLoadedTaskItems(sourceContainer, unloadedItems);
			AddCarriedTaskItems(actor, unloadedItems);
			RemoveContainerContents(sourceContainer, unloadedItems);
			operationalState = new EmploymentActionStepOperationalState(
				LoadedAssets: FormatLoadedAssets("unload", sourceContainer.Id, unloadedItems),
				ReservationReference: FormatLoadReservation("consume", sourceContainer.Id, unloadedItems));
			return true;
		}

		var containerComponent = sourceContainer.GetItemType<IContainer>();
		if (containerComponent is null)
		{
			reason = $"{sourceContainer.Name} is not a container.";
			return false;
		}

		RemoveLoadedTaskItems(sourceContainer, loadedItems);
		RemoveContainerContents(sourceContainer, loadedItems);
		AddCarriedTaskItems(actor, loadedItems);
		operationalState = new EmploymentActionStepOperationalState(
			LoadedAssets: FormatLoadedAssets("unload", sourceContainer.Id, loadedItems),
			ReservationReference: FormatLoadReservation("consume", sourceContainer.Id, loadedItems));
		reason = string.Empty;
		return true;
	}

	public bool TryReturnContainer(ICharacter actor, IGameItem container, ICell destination, IGameItem? destinationContainer,
		string? destinationContainerTag, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!TryRequireHostLogisticsLocation(destination, "return task containers", out reason))
		{
			return false;
		}

		if (!CanPath(actor, destination))
		{
			reason = "The assigned employee cannot path to the container return destination.";
			return false;
		}

		var targetContainer = destinationContainer;
		if (targetContainer is null && !string.IsNullOrWhiteSpace(destinationContainerTag))
		{
			targetContainer = AvailableItems(destination).FirstOrDefault(x => ItemHasTag(x, destinationContainerTag));
		}

		if (targetContainer is null && !string.IsNullOrWhiteSpace(destinationContainerTag))
		{
			reason = $"There is no destination container tagged {destinationContainerTag}.";
			return false;
		}

		if (_usePhysicalItemMovement && EmploymentInventoryPlanLogistics.CanUseInventoryPlan(actor))
		{
			if (actor.Location?.Id != destination.Id)
			{
				reason = "The assigned employee must be at the return destination to return the container.";
				return false;
			}

			if (!ActorCarriesItem(actor, container))
			{
				reason = $"The assigned employee is not carrying {container.Name}.";
				return false;
			}

			IReadOnlyCollection<IGameItem> placedItems;
			var moved = targetContainer is null
				? EmploymentInventoryPlanLogistics.TryDropItems(actor, [container], out placedItems, out reason)
				: EmploymentInventoryPlanLogistics.TryPutItemsIntoContainer(actor, [container], targetContainer,
					out placedItems, out reason);
			if (!moved)
			{
				return false;
			}

			RemoveCarriedTaskItems(actor, placedItems);
			if (targetContainer is null)
			{
				LocationItems(destination).AddRange(placedItems);
			}
			else
			{
				RecordContainerContents(targetContainer, placedItems);
			}

			operationalState = new EmploymentActionStepOperationalState(
				LoadedAssets: FormatLoadedAssets("return", targetContainer?.Id ?? destination.Id, placedItems),
				SelectedResources: $"Returned container {container.Id} to {(targetContainer is null ? $"cell {destination.Id}" : $"container {targetContainer.Id}")}");
			reason = string.Empty;
			return true;
		}

		if (!_carriedTaskItems.TryGetValue(CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor), out var carried) || carried.All(x => x.Id != container.Id))
		{
			reason = $"The assigned employee is not carrying {container.Name}.";
			return false;
		}

		if (targetContainer is null)
		{
			RemoveCarriedTaskItems(actor, [container]);
			LocationItems(destination).Add(container);
		}
		else
		{
			if (targetContainer.GetItemType<IContainer>() is not { } targetContainerComponent)
			{
				reason = $"{targetContainer.Name} is not a container.";
				return false;
			}

			if (!targetContainerComponent.CanPut(container))
			{
				reason = $"{targetContainer.Name} cannot contain {container.Name}.";
				return false;
			}

			RemoveCarriedTaskItems(actor, [container]);
			RecordContainerContents(targetContainer, [container]);
		}

		operationalState = new EmploymentActionStepOperationalState(
			LoadedAssets: FormatLoadedAssets("return", targetContainer?.Id ?? destination.Id, [container]),
			SelectedResources: $"Returned container {container.Id} to {(targetContainer is null ? $"cell {destination.Id}" : $"container {targetContainer.Id}")}");
		reason = string.Empty;
		return true;
	}

	private bool TryDeliverPhysicalTaskItems(ICharacter actor, ICell destination, IGameItem? targetContainer,
		List<IGameItem> destinationItems, List<IGameItem> carried, out string reason)
	{
		if (actor.Location?.Id != destination.Id)
		{
			reason = "The assigned employee must be at the delivery destination to deliver task items.";
			return false;
		}

		var containerComponent = targetContainer?.GetItemType<IContainer>();
		if (targetContainer is not null && containerComponent is null)
		{
			reason = $"{targetContainer.Name} is not a container.";
			return false;
		}

		foreach (var item in carried)
		{
			if (_transportBundleIds.Contains(item.Id) && item.GetItemType<PileGameItemComponent>() is { } pile)
			{
				var contents = pile.Contents.ToList();
				var rejected = containerComponent is null
					? null
					: contents.FirstOrDefault(x => !containerComponent.CanPut(x));
				if (rejected is not null)
				{
					reason = $"{targetContainer!.Name} cannot contain {rejected.Name}.";
					return false;
				}

				continue;
			}

			if (containerComponent is null)
			{
				if (!actor.Body.CanDrop(item, 0))
				{
					reason = actor.Body.WhyCannotDrop(item, 0);
					return false;
				}

				continue;
			}

			if (!actor.Body.CanPut(item, targetContainer!, null, 0, false))
			{
				reason = actor.Body.WhyCannotPut(item, targetContainer!, null, 0, false);
				return false;
			}
		}

		foreach (var item in carried.ToList())
		{
			if (_transportBundleIds.Contains(item.Id) && item.GetItemType<PileGameItemComponent>() is { } pile)
			{
				var contents = pile.Contents.ToList();
				pile.Empty(actor, containerComponent!, null);
				_transportBundleIds.Remove(item.Id);
				if (containerComponent is null)
				{
					destinationItems.AddRange(contents);
				}
				else
				{
					if (!_containerContents.TryGetValue(targetContainer!.Id, out var contained))
					{
						contained = new List<IGameItem>();
						_containerContents[targetContainer.Id] = contained;
					}

					contained.AddRange(contents);
				}

				continue;
			}

			if (containerComponent is null)
			{
				actor.Body.Drop(item, 0, false, null, false);
				destinationItems.Add(item);
				continue;
			}

			actor.Body.Put(item, targetContainer!, null, 0, null, false, false);
			if (!_containerContents.TryGetValue(targetContainer!.Id, out var targetContents))
			{
				targetContents = new List<IGameItem>();
				_containerContents[targetContainer.Id] = targetContents;
			}

			targetContents.Add(item);
		}

		RemoveCarriedTaskItems(actor, carried.ToList());
		reason = string.Empty;
		return true;
	}

	private IEnumerable<IGameItem> DeliveryItemsFor(IGameItem item)
	{
		if (_transportBundleIds.Contains(item.Id) && item.GetItemType<PileGameItemComponent>() is { } pile)
		{
			return pile.Contents;
		}

		return [item];
	}

	private bool TryBundleAndCollectTaskItems(ICharacter actor, ICell source, IReadOnlyCollection<IGameItem> items,
		out string reason)
	{
		var bundle = PileGameItemComponentProto.CreateNewBundle(items);
		actor.Gameworld.Add(bundle);
		bundle.RoomLayer = actor.RoomLayer;
		source.Insert(bundle, true);

		if (!TryHoldTaskItemsWithInventoryPlan(actor, [bundle], out var collectedItems, out reason))
		{
			return false;
		}

		foreach (var item in items)
		{
			LocationItems(source).RemoveAll(x => x.Id == item.Id);
		}

		LocationItems(source).RemoveAll(x => x.Id == bundle.Id);
		_transportBundleIds.Add(bundle.Id);
		AddCarriedTaskItems(actor, collectedItems);
		reason = string.Empty;
		return true;
	}

	private static bool TryHoldTaskItemsWithInventoryPlan(ICharacter actor, IReadOnlyCollection<IGameItem> items,
		out IReadOnlyCollection<IGameItem> collectedItems, out string reason)
	{
		return EmploymentInventoryPlanLogistics.TryHoldItems(actor, items, out collectedItems, out reason);
	}

	private void AddCarriedTaskItems(ICharacter actor, IEnumerable<IGameItem> items, bool contextManaged = false)
	{
		var itemList = items.DistinctBy(x => x.Id).ToList();
		if (!itemList.Any())
		{
			return;
		}

		var actorKey = CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor);
		if (!_carriedTaskItems.TryGetValue(actorKey, out var carried))
		{
			carried = new List<IGameItem>();
			_carriedTaskItems[actorKey] = carried;
		}

		HashSet<long>? contextManagedIds = null;
		if (contextManaged)
		{
			if (!_contextManagedCarriedTaskItemIds.TryGetValue(actorKey, out contextManagedIds))
			{
				contextManagedIds = [];
				_contextManagedCarriedTaskItemIds[actorKey] = contextManagedIds;
			}
		}
		else
		{
			_contextManagedCarriedTaskItemIds.TryGetValue(actorKey, out contextManagedIds);
		}

		foreach (var item in itemList)
		{
			if (carried.All(x => x.Id != item.Id))
			{
				carried.Add(item);
			}

			if (contextManaged)
			{
				contextManagedIds!.Add(item.Id);
			}
			else
			{
				contextManagedIds?.Remove(item.Id);
			}
		}
	}

	private void RemoveCarriedTaskItems(ICharacter actor, IEnumerable<IGameItem> items)
	{
		var itemIds = items.Select(x => x.Id).ToHashSet();
		if (!itemIds.Any())
		{
			return;
		}

		var actorKey = CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor);
		if (_carriedTaskItems.TryGetValue(actorKey, out var carried))
		{
			carried.RemoveAll(x => itemIds.Contains(x.Id));
		}

		if (_contextManagedCarriedTaskItemIds.TryGetValue(actorKey, out var contextManagedIds))
		{
			foreach (var itemId in itemIds)
			{
				contextManagedIds.Remove(itemId);
			}

			if (!contextManagedIds.Any())
			{
				_contextManagedCarriedTaskItemIds.Remove(actorKey);
			}
		}
	}

	private void MarkLoadedTaskItems(IGameItem container, IEnumerable<IGameItem> items)
	{
		if (!_loadedTaskItemIds.TryGetValue(container.Id, out var loadedIds))
		{
			loadedIds = new HashSet<long>();
			_loadedTaskItemIds[container.Id] = loadedIds;
		}

		foreach (var item in items)
		{
			loadedIds.Add(item.Id);
		}
	}

	private void RemoveLoadedTaskItems(IGameItem container, IEnumerable<IGameItem> items)
	{
		if (!_loadedTaskItemIds.TryGetValue(container.Id, out var loadedIds))
		{
			return;
		}

		foreach (var item in items)
		{
			loadedIds.Remove(item.Id);
		}
	}

	private IEnumerable<IGameItem> ResolveLoadedTaskItems(ICharacter actor, IGameItem container)
	{
		var loadedIds = _loadedTaskItemIds.TryGetValue(container.Id, out var ids)
			? ids
			: new HashSet<long>();

		foreach (var item in _containerContents.TryGetValue(container.Id, out var contents) ? contents : [])
		{
			if (!loadedIds.Any() || loadedIds.Contains(item.Id))
			{
				yield return item;
			}
		}

		foreach (var itemId in loadedIds)
		{
			var item = actor.Gameworld?.TryGetItem(itemId, true);
			if (item is not null)
			{
				yield return item;
			}
		}

		foreach (var item in container.GetItemType<IContainer>()?.Contents ?? [])
		{
			if (loadedIds.Contains(item.Id))
			{
				yield return item;
			}
		}
	}

	private void RecordContainerContents(IGameItem container, IEnumerable<IGameItem> items)
	{
		if (!_containerContents.TryGetValue(container.Id, out var contents))
		{
			contents = new List<IGameItem>();
			_containerContents[container.Id] = contents;
		}

		foreach (var item in items)
		{
			if (contents.All(x => x.Id != item.Id))
			{
				contents.Add(item);
			}
		}
	}

	private void RemoveContainerContents(IGameItem container, IEnumerable<IGameItem> items)
	{
		if (!_containerContents.TryGetValue(container.Id, out var contents))
		{
			return;
		}

		var itemIds = items.Select(x => x.Id).ToHashSet();
		contents.RemoveAll(x => itemIds.Contains(x.Id));
	}

	private static string FormatLoadedAssets(string operation, long containerId, IEnumerable<IGameItem> items)
	{
		return $"operation={operation};container={containerId};items={items.Select(x => x.Id.ToString("F0")).ListToCommaSeparatedValues()}";
	}

	private string FormatLoadReservation(string operation, long containerId, IEnumerable<IGameItem> items)
	{
		var itemIds = items.Select(x => x.Id).Distinct().ToList();
		return $"op={operation};type=load;task={CurrentTask?.Id.ToString("D") ?? string.Empty};container={containerId};items={itemIds.Select(x => x.ToString("F0")).ListToCommaSeparatedValues()};count={itemIds.Count.ToString(CultureInfo.InvariantCulture)}";
	}

	internal string FormatTaskItemCustodyForActor(string operation, ICharacter actor, IEnumerable<IGameItem> items,
		IEnumerable<long>? transportBundleIds = null)
	{
		var itemList = items.DistinctBy(x => x.Id).ToList();
		return FormatTaskItemCustody(operation, CharacterInstanceIdentityComparer.PhysicalInstanceKey(actor), itemList,
			transportBundleIds, itemList.Where(x => IsContextManagedCarriedTaskItem(actor, x)).Select(x => x.Id));
	}

	internal static string FormatTaskItemCustody(string operation, long actorId, IEnumerable<IGameItem> items,
		IEnumerable<long>? transportBundleIds = null, IEnumerable<long>? contextManagedItemIds = null)
	{
		var itemIds = items.Select(x => x.Id).Distinct().ToList();
		var bundleIds = (transportBundleIds ?? [])
		                .Distinct()
		                .ToList();
		var contextItemIds = (contextManagedItemIds ?? [])
		                     .Where(x => itemIds.Contains(x))
		                     .Distinct()
		                     .ToList();
		var result =
			$"operation={operation};actor={actorId};items={itemIds.Select(x => x.ToString("F0")).ListToCommaSeparatedValues()}";
		if (bundleIds.Any())
		{
			result = $"{result};bundles={bundleIds.Select(x => x.ToString("F0")).ListToCommaSeparatedValues()}";
		}

		return contextItemIds.Any()
			? $"{result};managed={contextItemIds.Select(x => x.ToString("F0")).ListToCommaSeparatedValues()}"
			: result;
	}

	private bool TryGetActiveVehicleAssignment(long vehicleId, out string taskName, out long driverId)
	{
		taskName = string.Empty;
		driverId = 0;
		foreach (var task in Employer.TaskBoard.ActiveTasks.Where(x =>
			         x.Status is EmploymentTaskStatus.Pending or EmploymentTaskStatus.Assigned or
				         EmploymentTaskStatus.InProgress or EmploymentTaskStatus.Blocked))
		{
			if (CurrentTask is not null && task.CorrelationId == CurrentTask.CorrelationId)
			{
				continue;
			}

			for (var i = 0; i < task.StepOperationalStates.Count; i++)
			{
				if (i >= task.StepStates.Count || task.StepStates[i] != EmploymentActionStepStatus.Completed)
				{
					continue;
				}

				if (!TryParseVehicleAssignment(task.StepOperationalStates[i].SelectedResources, out var assignedVehicleId,
					    out var assignedDriverId) ||
				    assignedVehicleId != vehicleId)
				{
					continue;
				}

				taskName = task.Name;
				driverId = assignedDriverId;
				return true;
			}
		}

		return false;
	}

	internal static string FormatVehicleAssignment(long vehicleId, long driverId)
	{
		return $"operation=vehicleassign;vehicle={vehicleId:F0};driver={driverId:F0}";
	}

	internal static bool TryParseVehicleAssignment(string? text, out long vehicleId, out long driverId)
	{
		vehicleId = 0;
		driverId = 0;
		if (string.IsNullOrWhiteSpace(text) ||
		    !text.Contains("operation=vehicleassign", StringComparison.InvariantCultureIgnoreCase))
		{
			return false;
		}

		var parts = text.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		                .Select(x => x.Split('=', 2, StringSplitOptions.TrimEntries))
		                .Where(x => x.Length == 2)
		                .ToDictionary(x => x[0], x => x[1], StringComparer.InvariantCultureIgnoreCase);
		return parts.TryGetValue("vehicle", out var vehicleText) &&
		       long.TryParse(vehicleText, out vehicleId) &&
		       parts.TryGetValue("driver", out var driverText) &&
		       long.TryParse(driverText, out driverId);
	}

	internal static bool TryParseTaskItemCustody(string? text, out string operation, out long actorId,
		out long[] itemIds, out long[] transportBundleIds)
	{
		return TryParseTaskItemCustody(text, out operation, out actorId, out itemIds, out transportBundleIds,
			out _);
	}

	internal static bool TryParseTaskItemCustody(string? text, out string operation, out long actorId,
		out long[] itemIds, out long[] transportBundleIds, out long[] contextManagedItemIds)
	{
		operation = string.Empty;
		actorId = 0;
		itemIds = [];
		transportBundleIds = [];
		contextManagedItemIds = [];
		if (string.IsNullOrWhiteSpace(text) ||
		    !text.Contains("operation=", StringComparison.InvariantCultureIgnoreCase) ||
		    !text.Contains("items=", StringComparison.InvariantCultureIgnoreCase))
		{
			return false;
		}

		var parts = text.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		                .Select(x => x.Split('=', 2, StringSplitOptions.TrimEntries))
		                .Where(x => x.Length == 2)
		                .ToDictionary(x => x[0], x => x[1], StringComparer.InvariantCultureIgnoreCase);
		if (!parts.TryGetValue("operation", out var operationText) ||
		    !parts.TryGetValue("actor", out var actorText) ||
		    !long.TryParse(actorText, out actorId) ||
		    !parts.TryGetValue("items", out var itemsText))
		{
			return false;
		}

		operation = operationText;
		itemIds = itemsText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		                   .Select(x => long.TryParse(x, out var value) ? value : 0)
		                   .Where(x => x > 0)
		                   .ToArray();
		if (parts.TryGetValue("bundles", out var bundlesText))
		{
			transportBundleIds = bundlesText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			                                .Select(x => long.TryParse(x, out var value) ? value : 0)
			                                .Where(x => x > 0)
			                                .ToArray();
		}

		if (parts.TryGetValue("managed", out var managedText))
		{
			contextManagedItemIds = managedText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			                                .Select(x => long.TryParse(x, out var value) ? value : 0)
			                                .Where(x => x > 0)
			                                .ToArray();
		}

		return itemIds.Any();
	}

	internal static bool TryParseLoadedAssets(string? text, out string operation, out long containerId, out long[] itemIds)
	{
		operation = "load";
		containerId = 0;
		itemIds = [];
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		var parts = text.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		                .Select(x => x.Split('=', 2, StringSplitOptions.TrimEntries))
		                .Where(x => x.Length == 2)
		                .ToDictionary(x => x[0], x => x[1], StringComparer.InvariantCultureIgnoreCase);
		operation = parts.GetValueOrDefault("operation") ?? "load";
		if (!parts.TryGetValue("container", out var containerText) ||
		    !long.TryParse(containerText, out containerId) ||
		    !parts.TryGetValue("items", out var itemsText))
		{
			return false;
		}

		itemIds = itemsText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		                   .Select(x => long.TryParse(x, out var value) ? value : 0)
		                   .Where(x => x > 0)
		                   .ToArray();
		return itemIds.Any();
	}

	private static bool ActorCarriesItem(ICharacter actor, IGameItem item)
	{
		return EmploymentWorkerItemLocator.IsHeldOrWielded(actor, item);
	}

	private static bool CanUseInventoryPlan(ICharacter actor)
	{
		return EmploymentInventoryPlanLogistics.CanUseInventoryPlan(actor);
	}

	public void RecordRegister(EmploymentRegisterEntryType entryType, ICharacter? actor, string description,
		Guid? correlationId = null)
	{
		Employer.EmploymentRegister.Record(entryType, actor, description, correlationId);
	}

	public void RecordLedger(EmploymentLedgerEntryType entryType, ICharacter? actor, MoneyAmount? amount,
		string description, Guid? correlationId = null)
	{
		Employer.BusinessLedger.Record(entryType, actor, amount, description, correlationId);
	}

	private List<IGameItem> LocationItems(ICell location)
	{
		if (!_locationItems.TryGetValue(location.Id, out var items))
		{
			items = location.GameItems?.SelectMany(x => x.DeepItems).DistinctBy(x => x.Id).ToList() ?? [];
			_locationItems[location.Id] = items;
		}

		return items;
	}

	private static bool CharacteristicsMatch(IReadOnlyDictionary<string, string> configured,
		IReadOnlyDictionary<string, string> required)
	{
		foreach (var characteristic in required)
		{
			if (!configured.TryGetValue(characteristic.Key, out var value) || !value.EqualTo(characteristic.Value))
			{
				return false;
			}
		}

		return true;
	}

	private static bool CommodityCharacteristicsMatch(ICommodity commodity,
		IReadOnlyDictionary<string, string> required)
	{
		foreach (var characteristic in required)
		{
			var match = commodity.CommodityCharacteristics.Any(x =>
				x.Key.Name.EqualTo(characteristic.Key) &&
				(
					x.Value.Name.EqualTo(characteristic.Value) ||
					x.Value.GetValue.EqualTo(characteristic.Value) ||
					x.Value.GetBasicValue.EqualTo(characteristic.Value) ||
					x.Value.GetFancyValue.EqualTo(characteristic.Value)
				));
			if (!match)
			{
				return false;
			}
		}

		return true;
	}
}

public sealed record ManualOrderCondition(string Key) : IEmploymentTaskCondition
{
	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.ManualOrder;
	public EmploymentAuthoritySet RequiredAuthority => EmploymentAuthoritySet.Empty;

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		var satisfied = context.ManualOrderActive(Key);
		reason = satisfied ? string.Empty : $"Manual order {Key} is not active.";
		return satisfied;
	}
}

public sealed record TimeWindowCondition(TimeSpan EarliestTime, TimeSpan LatestTime) : IEmploymentTaskCondition
{
	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.TimeWindow;
	public EmploymentAuthoritySet RequiredAuthority => EmploymentAuthoritySet.Empty;

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		var current = now.TimeOfDay;
		var satisfied = EarliestTime <= LatestTime
			? current >= EarliestTime && current <= LatestTime
			: current >= EarliestTime || current <= LatestTime;
		reason = satisfied ? string.Empty : $"Current time {current} is outside the task window.";
		return satisfied;
	}
}

public sealed record StockThresholdCondition(string StockKey, int Threshold, bool BelowThreshold) : IEmploymentTaskCondition
{
	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.StockThreshold;
	public EmploymentAuthoritySet RequiredAuthority => EmploymentAuthority.ManageStockRules;

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		if (!TryGetStockLevel(context, StockKey, out var current, out reason))
		{
			return false;
		}

		var satisfied = BelowThreshold ? current < Threshold : current >= Threshold;
		reason = satisfied ? string.Empty : $"Stock {StockKey} is {current}, which does not satisfy threshold {Threshold}.";
		return satisfied;
	}

	private static bool TryGetStockLevel(IEmploymentTaskContext context, string stockKey, out int current,
		out string reason)
	{
		if (!stockKey.StartsWith("merch:", StringComparison.InvariantCultureIgnoreCase))
		{
			current = context.StockLevel(stockKey.StartsWith("key:", StringComparison.InvariantCultureIgnoreCase)
				? stockKey["key:".Length..]
				: stockKey);
			reason = string.Empty;
			return true;
		}

		if (context.Employer is not IShop shop)
		{
			current = 0;
			reason = $"{context.Employer.EmploymentHostName} does not expose shop merchandise stock levels.";
			return false;
		}

		var selector = stockKey["merch:".Length..];
		var merchandise = long.TryParse(selector, out var id)
			? shop.Merchandises.FirstOrDefault(x => x.Id == id)
			: shop.Merchandises.FirstOrDefault(x => x.Name.EqualTo(selector));
		if (merchandise is null)
		{
			current = 0;
			reason = $"There is no merchandise matching {selector}.";
			return false;
		}

		var stocktake = shop.StocktakeMerchandise(merchandise);
		current = stocktake.OnFloorCount + stocktake.InStockroomCount;
		reason = string.Empty;
		return true;
	}
}

public sealed record AccountBalanceCondition(string AccountKey, decimal Threshold, bool BelowThreshold) : IEmploymentTaskCondition
{
	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.AccountBalance;
	public EmploymentAuthoritySet RequiredAuthority => EmploymentAuthority.CreateScheduledRules;

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		if (!TryGetAccountBalance(context, AccountKey, out var current, out reason))
		{
			return false;
		}

		var satisfied = BelowThreshold ? current < Threshold : current >= Threshold;
		reason = satisfied
			? string.Empty
			: $"Account {AccountKey} balance {current:N2} does not satisfy threshold {Threshold:N2}.";
		return satisfied;
	}

	private static bool TryGetAccountBalance(IEmploymentTaskContext context, string accountKey, out decimal current,
		out string reason)
	{
		reason = string.Empty;
		if (accountKey.StartsWith("key:", StringComparison.InvariantCultureIgnoreCase))
		{
			current = context.AccountBalance(accountKey["key:".Length..]);
			return true;
		}

		if (context is EmploymentTaskContext concrete &&
		    EmploymentFinanceService.TryGetConditionBalance(concrete, accountKey, out current, out reason))
		{
			return true;
		}

		current = context.AccountBalance(accountKey);
		if (current != 0.0M)
		{
			reason = string.Empty;
			return true;
		}

		reason = string.IsNullOrWhiteSpace(reason)
			? $"There is no account balance source matching {accountKey}."
			: reason;
		return false;
	}
}

public sealed record PayrollLiabilityCondition(string Metric, decimal Threshold, bool AboveThreshold) : IEmploymentTaskCondition
{
	public const string OutstandingMetric = "outstanding";
	public const string AmountMetric = "amount";
	public const string OverdueMetric = "overdue";

	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.PayrollLiability;
	public EmploymentAuthoritySet RequiredAuthority => EmploymentAuthority.ManagePayroll;

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		context.Employer.Payroll.EvaluatePayroll(now);
		var metric = NormaliseMetric(Metric);
		var value = metric switch
		{
			OutstandingMetric => context.Employer.Payroll.OutstandingLiabilities.Count,
			AmountMetric => context.Employer.Payroll.OutstandingLiabilities.Sum(x => x.Amount.Amount),
			OverdueMetric => context.Employer.Payroll.MaximumOverdueDays(now),
			_ => decimal.MinValue
		};

		if (value == decimal.MinValue)
		{
			reason = $"Payroll metric {Metric} is not supported.";
			return false;
		}

		var satisfied = AboveThreshold ? value > Threshold : value < Threshold;
		reason = satisfied
			? string.Empty
			: $"Payroll {DescribeMetric(metric)} is {value:N2}, which is not {(AboveThreshold ? "above" : "below")} {Threshold:N2}.";
		return satisfied;
	}

	public static string NormaliseMetric(string metric)
	{
		return metric.CollapseString().ToLowerInvariant() switch
		{
			"outstanding" or "count" or "payables" or "liabilities" => OutstandingMetric,
			"amount" or "total" or "money" or "value" => AmountMetric,
			"overdue" or "days" or "overduedays" or "maxoverdue" => OverdueMetric,
			var other => other
		};
	}

	public static string DescribeMetric(string metric)
	{
		return NormaliseMetric(metric) switch
		{
			OutstandingMetric => "outstanding payable count",
			AmountMetric => "outstanding amount",
			OverdueMetric => "maximum overdue days",
			_ => metric
		};
	}
}

public sealed record ItemThresholdCondition(string ItemKey, int Threshold, bool BelowThreshold) : IEmploymentTaskCondition
{
	private const string KeyPrefix = "item:v1";

	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.ItemThreshold;
	public EmploymentAuthoritySet RequiredAuthority => EmploymentAuthority.ManageStockRules;

	public static string CreateKey(EmploymentItemSelector itemSelector, long locationId,
		EmploymentItemSelector? containerSelector)
	{
		return $"{KeyPrefix}|location={locationId}|item={EncodeSelector(itemSelector)}|container={EncodeSelector(containerSelector)}";
	}

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		if (!TryParseKey(ItemKey, out var itemSelector, out var locationId, out var containerSelector))
		{
			reason = "The item condition key is invalid.";
			return false;
		}

		var location = ResolveLocation(context.Employer, locationId);
		if (location is null)
		{
			reason = $"There is no room/cell #{locationId:N0} available to this employment host.";
			return false;
		}

		var candidates = ItemsForCondition(context, location, containerSelector, out reason)
		                 .DistinctBy(x => x.Id)
		                 .ToList();
		if (!string.IsNullOrWhiteSpace(reason))
		{
			return false;
		}

		var current = candidates.Count(x => MatchesSelector(context, x, itemSelector));
		var satisfied = BelowThreshold ? current < Threshold : current >= Threshold;
		reason = satisfied
			? string.Empty
			: $"There are {current:N0} matching items at {location.Name}; threshold is {(BelowThreshold ? "below" : "at least")} {Threshold:N0}.";
		return satisfied;
	}

	internal static bool TryParseKey(string key, out EmploymentItemSelector itemSelector, out long locationId,
		out EmploymentItemSelector? containerSelector)
	{
		itemSelector = EmploymentItemSelector.ForPrototype(0);
		locationId = 0;
		containerSelector = null;
		var values = ParseKeyValues(key, KeyPrefix);
		if (values is null ||
		    !values.TryGetValue("location", out var locationText) ||
		    !long.TryParse(locationText, out locationId) ||
		    !values.TryGetValue("item", out var selectorText) ||
		    DecodeSelector(selectorText) is not { } parsedSelector)
		{
			return false;
		}

		itemSelector = parsedSelector;
		if (values.TryGetValue("container", out var containerText))
		{
			containerSelector = DecodeSelector(containerText);
		}

		return true;
	}

	internal static string DescribeKey(string key)
	{
		return TryParseKey(key, out var selector, out var locationId, out var container)
			? $"{EmploymentItemSelectorResolver.Describe(selector)} in room #{locationId:N0}{(container is null ? string.Empty : $" inside {EmploymentItemSelectorResolver.Describe(container)}")}"
			: key;
	}

	internal static string EncodeSelector(EmploymentItemSelector? selector)
	{
		if (selector is null)
		{
			return "none";
		}

		return selector.Kind switch
		{
			EmploymentItemSelectorKind.PrototypeId => $"proto:{selector.Id?.ToString("F0") ?? "0"}",
			EmploymentItemSelectorKind.ItemId => $"item:{selector.Id?.ToString("F0") ?? "0"}",
			EmploymentItemSelectorKind.Tag => $"tag:{Uri.EscapeDataString(selector.Text ?? string.Empty)}",
			EmploymentItemSelectorKind.Keyword =>
				$"keyword:{selector.Id?.ToString("F0") ?? "0"}:{Uri.EscapeDataString(selector.Text ?? string.Empty)}",
			_ => "none"
		};
	}

	internal static EmploymentItemSelector? DecodeSelector(string text)
	{
		if (string.IsNullOrWhiteSpace(text) || text.EqualTo("none"))
		{
			return null;
		}

		var parts = text.Split(':', 3);
		return parts[0].CollapseString().ToLowerInvariant() switch
		{
			"proto" when parts.Length >= 2 && long.TryParse(parts[1], out var id) =>
				EmploymentItemSelector.ForPrototype(id),
			"item" when parts.Length >= 2 && long.TryParse(parts[1], out var id) =>
				EmploymentItemSelector.ForItemId(id),
			"tag" when parts.Length >= 2 =>
				EmploymentItemSelector.ForTag(Uri.UnescapeDataString(parts[1])),
			"keyword" when parts.Length >= 3 && long.TryParse(parts[1], out var id) && id > 0 =>
				new EmploymentItemSelector(EmploymentItemSelectorKind.Keyword, id,
					Uri.UnescapeDataString(parts[2])),
			"keyword" when parts.Length >= 2 =>
				EmploymentItemSelector.ForKeyword(Uri.UnescapeDataString(parts[1])),
			_ => null
		};
	}

	internal static bool MatchesSelector(IEmploymentTaskContext context, IGameItem item,
		EmploymentItemSelector selector)
	{
		return selector.Kind switch
		{
			EmploymentItemSelectorKind.PrototypeId => item.Prototype?.Id == selector.Id,
			EmploymentItemSelectorKind.ItemId => item.Id == selector.Id,
			EmploymentItemSelectorKind.Keyword when selector.Id.HasValue => item.Id == selector.Id.Value,
			EmploymentItemSelectorKind.Keyword => !string.IsNullOrWhiteSpace(selector.Text) &&
			                                      item.Name.Contains(selector.Text,
				                                      StringComparison.InvariantCultureIgnoreCase),
			EmploymentItemSelectorKind.Tag => !string.IsNullOrWhiteSpace(selector.Text) &&
			                                  context.ItemHasTag(item, selector.Text),
			_ => false
		};
	}

	internal static IEnumerable<IGameItem> ItemsForCondition(IEmploymentTaskContext context, ICell location,
		EmploymentItemSelector? containerSelector, out string reason)
	{
		reason = string.Empty;
		var roomItems = ExpandItems(context.AvailableItems(location)).DistinctBy(x => x.Id).ToList();
		if (containerSelector is null)
		{
			return roomItems;
		}

		var containers = roomItems
		                 .Where(x => MatchesSelector(context, x, containerSelector))
		                 .DistinctBy(x => x.Id)
		                 .ToList();
		if (!containers.Any())
		{
			reason = $"There is no container matching {EmploymentItemSelectorResolver.Describe(containerSelector)} at {location.Name}.";
			return [];
		}

		return containers
		       .SelectMany(x => ContainerContents(context, x))
		       .DistinctBy(x => x.Id)
		       .ToList();
	}

	private static IEnumerable<IGameItem> ContainerContents(IEmploymentTaskContext context, IGameItem container)
	{
		if (context is EmploymentTaskContext concrete)
		{
			foreach (var item in concrete.ContainedItems(container).SelectMany(DeepItemsOrSelf))
			{
				yield return item;
			}
		}

		foreach (var item in container.GetItemType<IContainer>()?.Contents.SelectMany(DeepItemsOrSelf) ?? [])
		{
			yield return item;
		}
	}

	private static IEnumerable<IGameItem> ExpandItems(IEnumerable<IGameItem> items)
	{
		foreach (var item in items)
		{
			foreach (var deepItem in DeepItemsOrSelf(item))
			{
				yield return deepItem;
			}
		}
	}

	private static IEnumerable<IGameItem> DeepItemsOrSelf(IGameItem item)
	{
		var deepItems = item.DeepItems?.ToList();
		return deepItems?.Any() == true ? deepItems : [item];
	}

	internal static ICell? ResolveLocation(IEmploymentHost host, long locationId)
	{
		return host.EmploymentHostLocations().FirstOrDefault(x => x.Id == locationId) ??
		       (host as IHaveFuturemud)?.Gameworld.Cells.Get(locationId);
	}

	private static Dictionary<string, string>? ParseKeyValues(string key, string prefix)
	{
		if (string.IsNullOrWhiteSpace(key) ||
		    !key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
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

public sealed record CommodityThresholdCondition(string CommodityKey, decimal ThresholdWeight, bool BelowThreshold)
	: IEmploymentTaskCondition
{
	private const string KeyPrefix = "commodity:v1";

	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.CommodityThreshold;
	public EmploymentAuthoritySet RequiredAuthority => EmploymentAuthority.ManageStockRules;

	public static string CreateKey(string materialName, string? tagName,
		IReadOnlyDictionary<string, string> characteristics, long locationId,
		EmploymentItemSelector? containerSelector)
	{
		var characteristicText = string.Join(";",
			characteristics.OrderBy(x => x.Key, StringComparer.InvariantCultureIgnoreCase)
			               .Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value)}"));
		return $"{KeyPrefix}|location={locationId}|material={Uri.EscapeDataString(materialName)}|tag={Uri.EscapeDataString(tagName ?? string.Empty)}|chars={characteristicText}|container={ItemThresholdCondition.EncodeSelector(containerSelector)}";
	}

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		if (!TryParseKey(CommodityKey, out var material, out var tag, out var characteristics, out var locationId,
			    out var containerSelector))
		{
			reason = "The commodity condition key is invalid.";
			return false;
		}

		var location = ItemThresholdCondition.ResolveLocation(context.Employer, locationId);
		if (location is null)
		{
			reason = $"There is no room/cell #{locationId:N0} available to this employment host.";
			return false;
		}

		var candidates = ItemThresholdCondition.ItemsForCondition(context, location, containerSelector, out reason)
		                                      .DistinctBy(x => x.Id)
		                                      .ToList();
		if (!string.IsNullOrWhiteSpace(reason))
		{
			return false;
		}

		var current = candidates.Sum(x => (decimal)context.CommodityWeight(x, material, tag, characteristics));
		var satisfied = BelowThreshold ? current < ThresholdWeight : current >= ThresholdWeight;
		reason = satisfied
			? string.Empty
			: $"There is {current:N2} matching commodity weight at {location.Name}; threshold is {(BelowThreshold ? "below" : "at least")} {ThresholdWeight:N2}.";
		return satisfied;
	}

	internal static bool TryParseKey(string key, out string material, out string? tag,
		out IReadOnlyDictionary<string, string> characteristics, out long locationId,
		out EmploymentItemSelector? containerSelector)
	{
		material = string.Empty;
		tag = null;
		characteristics = new Dictionary<string, string>();
		locationId = 0;
		containerSelector = null;
		var values = ParseKeyValues(key, KeyPrefix);
		if (values is null ||
		    !values.TryGetValue("location", out var locationText) ||
		    !long.TryParse(locationText, out locationId) ||
		    !values.TryGetValue("material", out var materialText))
		{
			return false;
		}

		material = Uri.UnescapeDataString(materialText);
		if (string.IsNullOrWhiteSpace(material))
		{
			return false;
		}

		if (values.TryGetValue("tag", out var tagText) && !string.IsNullOrWhiteSpace(tagText))
		{
			tag = Uri.UnescapeDataString(tagText);
		}

		if (values.TryGetValue("chars", out var characteristicText) &&
		    !string.IsNullOrWhiteSpace(characteristicText))
		{
			characteristics = characteristicText.Split(';',
					StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			                                    .Select(x => x.Split('=', 2, StringSplitOptions.TrimEntries))
			                                    .Where(x => x.Length == 2)
			                                    .ToDictionary(x => Uri.UnescapeDataString(x[0]),
				                                    x => Uri.UnescapeDataString(x[1]),
				                                    StringComparer.InvariantCultureIgnoreCase);
		}

		if (values.TryGetValue("container", out var containerText))
		{
			containerSelector = ItemThresholdCondition.DecodeSelector(containerText);
		}

		return true;
	}

	internal static string DescribeKey(string key)
	{
		if (!TryParseKey(key, out var material, out var tag, out var characteristics, out var locationId,
			    out var container))
		{
			return key;
		}

		var descriptor = material;
		if (!string.IsNullOrWhiteSpace(tag))
		{
			descriptor = $"{descriptor}|{tag}";
		}

		foreach (var characteristic in characteristics.OrderBy(x => x.Key, StringComparer.InvariantCultureIgnoreCase))
		{
			descriptor = $"{descriptor}|{characteristic.Key}={characteristic.Value}";
		}

		return $"{descriptor} in room #{locationId:N0}{(container is null ? string.Empty : $" inside {EmploymentItemSelectorResolver.Describe(container)}")}";
	}

	private static Dictionary<string, string>? ParseKeyValues(string key, string prefix)
	{
		if (string.IsNullOrWhiteSpace(key) ||
		    !key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
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

public sealed record ShopAccountOwingCondition(string AccountKey, decimal Threshold, bool AboveThreshold)
	: IEmploymentTaskCondition
{
	private const string KeyPrefix = "shopaccount:v1";

	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.ShopAccountOwing;
	public EmploymentAuthoritySet RequiredAuthority => EmploymentAuthority.CreateScheduledRules;

	public static string CreateKey(IShop shop, ILineOfCreditAccount account)
	{
		return $"{KeyPrefix}|shop={shop.Id}|account={account.Id}";
	}

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		if (!TryParseKey(AccountKey, out var shopId, out var accountId))
		{
			reason = "The shop account condition key is invalid.";
			return false;
		}

		var shop = ResolveShop(context.Employer, shopId);
		if (shop is null)
		{
			reason = $"There is no shop #{shopId:N0} available to this employment host.";
			return false;
		}

		var account = shop.LineOfCreditAccounts.FirstOrDefault(x => x.Id == accountId);
		if (account is null)
		{
			reason = $"{shop.Name} does not have line-of-credit account #{accountId:N0}.";
			return false;
		}

		var current = account.OutstandingBalance;
		var satisfied = AboveThreshold ? current > Threshold : current <= Threshold;
		reason = satisfied
			? string.Empty
			: $"Shop account {account.AccountName} owes {current:N2}, which does not satisfy {(AboveThreshold ? "more than" : "no more than")} {Threshold:N2}.";
		return satisfied;
	}

	internal static bool TryParseKey(string key, out long shopId, out long accountId)
	{
		shopId = 0;
		accountId = 0;
		var values = ParseKeyValues(key, KeyPrefix);
		return values is not null &&
		       values.TryGetValue("shop", out var shopText) &&
		       long.TryParse(shopText, out shopId) &&
		       values.TryGetValue("account", out var accountText) &&
		       long.TryParse(accountText, out accountId);
	}

	internal static string DescribeKey(string key, IEmploymentHost host)
	{
		if (!TryParseKey(key, out var shopId, out var accountId))
		{
			return key;
		}

		var shop = ResolveShop(host, shopId);
		var account = shop?.LineOfCreditAccounts.FirstOrDefault(x => x.Id == accountId);
		return shop is null
			? $"shop #{shopId:N0} account #{accountId:N0}"
			: $"{shop.Name} / {(account is null ? $"account #{accountId:N0}" : account.AccountName)}";
	}

	private static IShop? ResolveShop(IEmploymentHost host, long shopId)
	{
		if (host is IShop shop && shop.Id == shopId)
		{
			return shop;
		}

		return (host as IHaveFuturemud)?.Gameworld.Shops.Get(shopId);
	}

	private static Dictionary<string, string>? ParseKeyValues(string key, string prefix)
	{
		if (string.IsNullOrWhiteSpace(key) ||
		    !key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
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

public sealed record ShopFloatThresholdCondition(string FloatKey, decimal Threshold, bool BelowThreshold)
	: IEmploymentTaskCondition
{
	private const string KeyPrefix = "float:v1";

	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.ShopFloatThreshold;
	public EmploymentAuthoritySet RequiredAuthority => EmploymentAuthority.CreateScheduledRules;

	public static string CreateKey(EmploymentItemSelector? registerSelector)
	{
		return $"{KeyPrefix}|register={ItemThresholdCondition.EncodeSelector(registerSelector)}";
	}

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		if (context.Employer is not IPermanentShop shop)
		{
			reason = $"{context.Employer.EmploymentHostName} is not a permanent shop with cash register/till items.";
			return false;
		}

		if (!TryParseKey(FloatKey, out var registerSelector))
		{
			reason = "The shop float condition key is invalid.";
			return false;
		}

		var tills = shop.TillItems.ToList();
		if (registerSelector is not null)
		{
			tills = tills
			        .Where(x => ItemThresholdCondition.MatchesSelector(context, x, registerSelector))
			        .ToList();
		}

		if (!tills.Any())
		{
			reason = registerSelector is null
				? $"{shop.Name} does not have any configured till items."
				: $"{shop.Name} does not have a till matching {EmploymentItemSelectorResolver.Describe(registerSelector)}.";
			return false;
		}

		var current = tills
		              .SelectMany(x => x.RecursiveGetItems<ICurrencyPile>(false))
		              .Where(x => x.Currency == shop.Currency)
		              .Sum(x => x.TotalValue);
		var satisfied = BelowThreshold ? current < Threshold : current >= Threshold;
		reason = satisfied
			? string.Empty
			: $"Shop float is {shop.Currency.Describe(current, CurrencyDescriptionPatternType.ShortDecimal)}, which does not satisfy {(BelowThreshold ? "below" : "at least")} {shop.Currency.Describe(Threshold, CurrencyDescriptionPatternType.ShortDecimal)}.";
		return satisfied;
	}

	internal static bool TryParseKey(string key, out EmploymentItemSelector? registerSelector)
	{
		registerSelector = null;
		var values = ParseKeyValues(key, KeyPrefix);
		if (values is null || !values.TryGetValue("register", out var selectorText))
		{
			return false;
		}

		registerSelector = ItemThresholdCondition.DecodeSelector(selectorText);
		return true;
	}

	internal static string DescribeKey(string key)
	{
		return TryParseKey(key, out var selector)
			? selector is null ? "all cash registers" : EmploymentItemSelectorResolver.Describe(selector)
			: key;
	}

	private static Dictionary<string, string>? ParseKeyValues(string key, string prefix)
	{
		if (string.IsNullOrWhiteSpace(key) ||
		    !key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
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

public sealed record TaxOwingCondition(decimal Threshold, bool AboveThreshold) : IEmploymentTaskCondition
{
	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.TaxOwing;
	public EmploymentAuthoritySet RequiredAuthority => EmploymentAuthority.CreateScheduledRules;

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		if (context is not EmploymentTaskContext concrete)
		{
			reason = "Tax owing conditions require a production employment task context.";
			return false;
		}

		if (!EmploymentFinanceService.TryGetTaxOwing(concrete, out var amount, out reason))
		{
			return false;
		}

		var satisfied = AboveThreshold ? amount.Amount > Threshold : amount.Amount < Threshold;
		reason = satisfied
			? string.Empty
			: $"Supported host taxes owing are {amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal)}, which is not {(AboveThreshold ? "above" : "below")} {amount.Currency.Describe(Threshold, CurrencyDescriptionPatternType.ShortDecimal)}.";
		return satisfied;
	}
}

public sealed record MarketPriceCondition(string PriceKey, decimal Threshold, bool AboveThreshold)
	: IEmploymentTaskCondition
{
	private const string KeyPrefix = "marketprice:v1";

	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.MarketPrice;
	public EmploymentAuthoritySet RequiredAuthority => EmploymentAuthority.CreateScheduledRules;

	public static string CreateMerchandiseKey(IMerchandise merchandise, string metric)
	{
		return $"{KeyPrefix}|kind=merch|id={merchandise.Id}|metric={Uri.EscapeDataString(metric.CollapseString().ToLowerInvariant())}";
	}

	public static string CreateItemKey(IGameItemProto prototype, string metric)
	{
		return $"{KeyPrefix}|kind=item|id={prototype.Id}|metric={Uri.EscapeDataString(metric.CollapseString().ToLowerInvariant())}";
	}

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		if (!TryGetValue(context.Employer, PriceKey, out var value, out var descriptor, out reason))
		{
			return false;
		}

		var satisfied = AboveThreshold ? value > Threshold : value < Threshold;
		reason = satisfied
			? string.Empty
			: $"{descriptor} is {value:N2}, which is not {(AboveThreshold ? "above" : "below")} {Threshold:N2}.";
		return satisfied;
	}

	internal static bool TryParseKey(string key, out string kind, out long id, out string metric)
	{
		kind = string.Empty;
		id = 0;
		metric = string.Empty;
		var values = ParseKeyValues(key, KeyPrefix);
		if (values is null ||
		    !values.TryGetValue("kind", out var parsedKind) ||
		    !values.TryGetValue("id", out var idText) ||
		    !long.TryParse(idText, out id) ||
		    !values.TryGetValue("metric", out var parsedMetric))
		{
			return false;
		}

		kind = parsedKind;
		metric = parsedMetric;
		metric = Uri.UnescapeDataString(metric).CollapseString().ToLowerInvariant();
		return IsAny(kind, "merch", "item") && IsAny(metric, "effective", "base", "multiplier", "flat");
	}

	internal static string DescribeKey(string key, IEmploymentHost host)
	{
		if (!TryParseKey(key, out var kind, out var id, out var metric))
		{
			return key;
		}

		if (kind.EqualTo("merch") && host is IShop shop)
		{
			var merchandise = shop.Merchandises.FirstOrDefault(x => x.Id == id);
			return $"{metric} price for {(merchandise?.Name ?? $"merchandise #{id:N0}")}";
		}

		var prototype = (host as IHaveFuturemud)?.Gameworld.ItemProtos.Get(id);
		return $"{metric} market price for {(prototype?.ShortDescription ?? $"item prototype #{id:N0}")}";
	}

	private static bool TryGetValue(IEmploymentHost host, string key, out decimal value, out string descriptor,
		out string reason)
	{
		value = 0.0M;
		descriptor = key;
		reason = string.Empty;
		if (!TryParseKey(key, out var kind, out var id, out var metric))
		{
			reason = "The market price condition key is invalid.";
			return false;
		}

		var market = host.Market;
		if (kind.EqualTo("merch"))
		{
			if (host is not IShop shop)
			{
				reason = $"{host.EmploymentHostName} is not a shop and cannot inspect merchandise prices.";
				return false;
			}

			var merchandise = shop.Merchandises.FirstOrDefault(x => x.Id == id);
			if (merchandise is null)
			{
				reason = $"{shop.Name} does not have merchandise #{id:N0}.";
				return false;
			}

			descriptor = $"{metric} price for {merchandise.Name}";
			switch (metric)
			{
				case "effective":
					value = merchandise.EffectivePrice;
					return true;
				case "base":
					value = merchandise.BasePrice;
					return true;
				case "multiplier":
					market ??= shop.MarketForPricingPurposes;
					if (market is null || merchandise.Item is null)
					{
						reason = $"{shop.Name} does not have market pricing for {merchandise.Name}.";
						return false;
					}

					value = market.PriceMultiplierForItem(merchandise.Item);
					return true;
				case "flat":
					market ??= shop.MarketForPricingPurposes;
					if (market is null || merchandise.Item is null)
					{
						reason = $"{shop.Name} does not have market pricing for {merchandise.Name}.";
						return false;
					}

					value = market.FlatPriceAdjustmentForItem(merchandise.Item);
					return true;
			}
		}

		if (!IsAny(metric, "multiplier", "flat"))
		{
			reason = "Item market price conditions only support multiplier or flat metrics.";
			return false;
		}

		var prototype = (host as IHaveFuturemud)?.Gameworld.ItemProtos.Get(id);
		if (prototype is null)
		{
			reason = $"There is no item prototype #{id:N0} available to this employment host.";
			return false;
		}

		market ??= host.Market;
		if (market is null)
		{
			reason = $"{host.EmploymentHostName} does not have an associated market.";
			return false;
		}

		descriptor = $"{metric} market price for {prototype.ShortDescription}";
		value = metric.EqualTo("multiplier")
			? market.PriceMultiplierForItem(prototype)
			: market.FlatPriceAdjustmentForItem(prototype);
		return true;
	}

	private static bool IsAny(string text, params string[] options)
	{
		return options.Any(x => text.EqualTo(x));
	}

	private static Dictionary<string, string>? ParseKeyValues(string key, string prefix)
	{
		if (string.IsNullOrWhiteSpace(key) ||
		    !key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
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

public sealed record StaffingLevelCondition(string StaffingKey, int Threshold, bool BelowThreshold) : IEmploymentTaskCondition
{
	private const string KeyPrefix = "staffing:v1";
	public const string ActiveMetric = "active";
	public const string OpenMetric = "open";
	public const string CombinedMetric = "combined";

	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.StaffingLevel;
	public EmploymentAuthoritySet RequiredAuthority => EmploymentAuthority.CreateJobOpenings;

	public static string CreateKey(EmploymentRole? role, string metric)
	{
		return $"{KeyPrefix}|role={(role?.ToString() ?? "any")}|metric={NormaliseMetric(metric)}";
	}

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		if (!TryParseKey(StaffingKey, out var role, out var metric))
		{
			reason = "The staffing condition key is invalid.";
			return false;
		}

		var active = context.Employer.EmploymentContracts
		                    .Count(x => x.Status == EmploymentStatus.Active && MatchesRole(x.Role, role));
		var open = context.Employer.JobOpenings
		                  .Where(x => x.Status == JobOpeningStatus.Open && MatchesRole(x.Role, role))
		                  .Sum(x => RemainingPositions(context.Employer, x));
		var value = metric switch
		{
			ActiveMetric => active,
			OpenMetric => open,
			CombinedMetric => active + open,
			_ => int.MinValue
		};

		if (value == int.MinValue)
		{
			reason = $"Staffing metric {metric} is not supported.";
			return false;
		}

		var satisfied = BelowThreshold ? value < Threshold : value >= Threshold;
		reason = satisfied
			? string.Empty
			: $"{DescribeKey(StaffingKey)} is {value:N0}, which is not {(BelowThreshold ? "below" : "at least")} {Threshold:N0}.";
		return satisfied;
	}

	internal static bool TryParseKey(string key, out EmploymentRole? role, out string metric)
	{
		role = null;
		metric = string.Empty;
		var values = ParseKeyValues(key, KeyPrefix);
		if (values is null ||
		    !values.TryGetValue("role", out var roleText) ||
		    !values.TryGetValue("metric", out var metricText))
		{
			return false;
		}

		if (!roleText.EqualTo("any"))
		{
			if (!roleText.TryParseEnum<EmploymentRole>(out var parsedRole))
			{
				return false;
			}

			role = parsedRole;
		}

		metric = NormaliseMetric(metricText);
		return metric is ActiveMetric or OpenMetric or CombinedMetric;
	}

	internal static string DescribeKey(string key)
	{
		return TryParseKey(key, out var role, out var metric)
			? $"{DescribeMetric(metric)} staffing for {(role?.DescribeEnum() ?? "any role")}"
			: key;
	}

	public static string NormaliseMetric(string metric)
	{
		return metric.CollapseString().ToLowerInvariant() switch
		{
			"employee" or "employees" or "contract" or "contracts" or "active" => ActiveMetric,
			"opening" or "openings" or "vacancy" or "vacancies" or "open" => OpenMetric,
			"coverage" or "covered" or "combined" or "total" => CombinedMetric,
			var other => other
		};
	}

	public static string DescribeMetric(string metric)
	{
		return NormaliseMetric(metric) switch
		{
			ActiveMetric => "active",
			OpenMetric => "open-position",
			CombinedMetric => "combined active/open",
			_ => metric
		};
	}

	private static bool MatchesRole(EmploymentRole actual, EmploymentRole? expected)
	{
		return expected is null || actual == expected;
	}

	private static int RemainingPositions(IEmploymentHost employer, IJobOpening opening)
	{
		var accepted = employer.Employment.Applications.Count(x =>
			x.Opening.Id == opening.Id &&
			x.Status == JobApplicationStatus.Accepted);
		return Math.Max(0, opening.MaxPositions - accepted);
	}

	private static Dictionary<string, string>? ParseKeyValues(string key, string prefix)
	{
		if (string.IsNullOrWhiteSpace(key) ||
		    !key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
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

public sealed record WeatherLevelCondition(string WeatherKey) : IEmploymentTaskCondition
{
	private const string KeyPrefix = "weather:v1";

	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.WeatherLevel;
	public EmploymentAuthoritySet RequiredAuthority => EmploymentAuthoritySet.Empty;

	public static string CreatePrecipitationKey(string precipitationSelector)
	{
		return $"{KeyPrefix}|kind=precip|level={Uri.EscapeDataString(precipitationSelector)}";
	}

	public static string CreateWindKey(WindLevel wind)
	{
		return $"{KeyPrefix}|kind=wind|level={wind}";
	}

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		if (!TryParseKey(WeatherKey, out var kind, out var selector))
		{
			reason = "The weather condition key is invalid.";
			return false;
		}

		var controller = context.Employer.EmploymentHostLocations()
		                        .Select(x => x.WeatherController)
		                        .FirstOrDefault(x => x is not null);
		if (controller is null)
		{
			reason = $"{context.Employer.EmploymentHostName} does not have a weather controller at any host location.";
			return false;
		}

		var weather = controller.CurrentWeatherEvent?.CountsAs ?? controller.CurrentWeatherEvent;
		if (weather is null)
		{
			reason = "There is no current weather event.";
			return false;
		}

		if (controller.ConsecutiveUnchangedPeriods != 0)
		{
			reason = $"Weather has not just changed; it has remained unchanged for {controller.ConsecutiveUnchangedPeriods:N0} period{(controller.ConsecutiveUnchangedPeriods == 1 ? string.Empty : "s")}.";
			return false;
		}

		var satisfied = kind.EqualTo("precip")
			? PrecipitationMatches(weather.Precipitation, selector)
			: WindMatches(weather.Wind, selector);
		reason = satisfied
			? string.Empty
			: $"Current weather is {weather.Precipitation.Describe()} with {weather.Wind.Describe()} wind.";
		return satisfied;
	}

	internal static bool TryParseKey(string key, out string kind, out string selector)
	{
		kind = string.Empty;
		selector = string.Empty;
		var values = ParseKeyValues(key, KeyPrefix);
		if (values is null ||
		    !values.TryGetValue("kind", out var parsedKind) ||
		    !values.TryGetValue("level", out var parsedSelector))
		{
			return false;
		}

		kind = parsedKind;
		selector = parsedSelector;
		selector = Uri.UnescapeDataString(selector);
		return kind.EqualTo("precip") || kind.EqualTo("wind");
	}

	internal static string DescribeKey(string key)
	{
		if (!TryParseKey(key, out var kind, out var selector))
		{
			return key;
		}

		return kind.EqualTo("precip")
			? $"precipitation {selector}"
			: $"wind {selector}";
	}

	private static bool PrecipitationMatches(PrecipitationLevel current, string selector)
	{
		if (selector.EqualTo("rain") || selector.EqualTo("raining"))
		{
			return current.IsRaining();
		}

		if (selector.EqualTo("snow") || selector.EqualTo("snowing"))
		{
			return current.IsSnowing();
		}

		return selector.TryParseEnum<PrecipitationLevel>(out var level) && current == level;
	}

	private static bool WindMatches(WindLevel current, string selector)
	{
		return selector.TryParseEnum<WindLevel>(out var level) && current >= level;
	}

	private static Dictionary<string, string>? ParseKeyValues(string key, string prefix)
	{
		if (string.IsNullOrWhiteSpace(key) ||
		    !key.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
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

internal static class EmploymentConditionExpressionEvaluator
{
	public static EmploymentConditionExpressionEvaluation Evaluate(
		EmploymentConditionExpression? expression,
		IReadOnlyList<IEmploymentTaskCondition> conditions,
		IEmploymentTaskContext context,
		DateTimeOffset now)
	{
		expression ??= EmploymentConditionExpression.All(
			Enumerable.Range(1, conditions.Count).Select(EmploymentConditionExpression.Condition));
		return EvaluateExpression(expression, conditions, context, now, new HashSet<string>(StringComparer.InvariantCultureIgnoreCase));
	}

	public static EmploymentAuthoritySet RequiredAuthority(EmploymentConditionExpression? expression,
		IReadOnlyList<IEmploymentTaskCondition> conditions, IEmploymentTaskBoard board)
	{
		expression ??= EmploymentConditionExpression.All(
			Enumerable.Range(1, conditions.Count).Select(EmploymentConditionExpression.Condition));
		return new EmploymentAuthoritySet(RequiredAuthority(expression, conditions, board,
			new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)));
	}

	public static bool Validate(EmploymentConditionExpression? expression, IReadOnlyList<IEmploymentTaskCondition> conditions,
		IEmploymentTaskBoard board, out string reason)
	{
		expression ??= EmploymentConditionExpression.All(
			Enumerable.Range(1, conditions.Count).Select(EmploymentConditionExpression.Condition));
		return Validate(expression, conditions, board, new HashSet<string>(StringComparer.InvariantCultureIgnoreCase),
			out reason);
	}

	public static string Describe(EmploymentConditionExpression? expression, IReadOnlyList<IEmploymentTaskCondition> conditions)
	{
		expression ??= EmploymentConditionExpression.All(
			Enumerable.Range(1, conditions.Count).Select(EmploymentConditionExpression.Condition));
		return expression.Kind switch
		{
			EmploymentConditionExpressionKind.Condition =>
				$"#{expression.ConditionNumber?.ToString("N0") ?? "?"}",
			EmploymentConditionExpressionKind.Predicate =>
				$"@{expression.PredicateName ?? "?"}",
			EmploymentConditionExpressionKind.Not =>
				$"not ({Describe(expression.ChildExpressions.FirstOrDefault(), conditions)})",
			EmploymentConditionExpressionKind.Any =>
				$"({string.Join(" or ", expression.ChildExpressions.Select(x => Describe(x, conditions)))})",
			_ => $"({string.Join(" and ", expression.ChildExpressions.Select(x => Describe(x, conditions)))})"
		};
	}

	private static EmploymentConditionExpressionEvaluation EvaluateExpression(
		EmploymentConditionExpression expression,
		IReadOnlyList<IEmploymentTaskCondition> conditions,
		IEmploymentTaskContext context,
		DateTimeOffset now,
		HashSet<string> predicateStack)
	{
		switch (expression.Kind)
		{
			case EmploymentConditionExpressionKind.Condition:
				return EvaluateCondition(expression.ConditionNumber, conditions, context, now);
			case EmploymentConditionExpressionKind.Predicate:
				return EvaluatePredicate(expression.PredicateName, context, now, predicateStack);
			case EmploymentConditionExpressionKind.Not:
				return EvaluateNot(expression, conditions, context, now, predicateStack);
			case EmploymentConditionExpressionKind.Any:
				return EvaluateAny(expression, conditions, context, now, predicateStack);
			case EmploymentConditionExpressionKind.All:
			default:
				return EvaluateAll(expression, conditions, context, now, predicateStack);
		}
	}

	private static EmploymentConditionExpressionEvaluation EvaluateCondition(int? conditionNumber,
		IReadOnlyList<IEmploymentTaskCondition> conditions, IEmploymentTaskContext context, DateTimeOffset now)
	{
		if (!conditionNumber.HasValue || conditionNumber.Value < 1 || conditionNumber.Value > conditions.Count)
		{
			var reason = $"Condition #{conditionNumber?.ToString("N0") ?? "?"} does not exist.";
			return Failed(reason, new EmploymentConditionLeafEvaluation(reason, false, reason));
		}

		var condition = conditions[conditionNumber.Value - 1];
		var satisfied = condition.IsSatisfied(context, now, out var conditionReason);
		var label = $"condition #{conditionNumber.Value:N0} ({condition.ConditionType.DescribeEnum()})";
		return new EmploymentConditionExpressionEvaluation(
			satisfied,
			satisfied ? string.Empty : conditionReason,
			[new EmploymentConditionLeafEvaluation(label, satisfied, conditionReason)]);
	}

	private static EmploymentConditionExpressionEvaluation EvaluatePredicate(string? predicateName,
		IEmploymentTaskContext context, DateTimeOffset now, HashSet<string> predicateStack)
	{
		if (string.IsNullOrWhiteSpace(predicateName))
		{
			return Failed("A named predicate reference is blank.",
				new EmploymentConditionLeafEvaluation("@?", false, "A named predicate reference is blank."));
		}

		if (!predicateStack.Add(predicateName))
		{
			var reason = $"Named predicate @{predicateName} is cyclic.";
			return Failed(reason, new EmploymentConditionLeafEvaluation($"@{predicateName}", false, reason));
		}

		var predicate = context.Employer.TaskBoard.ConditionPredicates
		                       .FirstOrDefault(x => x.Name.EqualTo(predicateName));
		if (predicate is null)
		{
			predicateStack.Remove(predicateName);
			var reason = $"Named predicate @{predicateName} does not exist.";
			return Failed(reason, new EmploymentConditionLeafEvaluation($"@{predicateName}", false, reason));
		}

		var result = EvaluateExpression(
			predicate.ConditionExpression ?? EmploymentConditionExpression.All(
				Enumerable.Range(1, predicate.Conditions.Count).Select(EmploymentConditionExpression.Condition)),
			predicate.Conditions.ToList(),
			context,
			now,
			predicateStack);
		predicateStack.Remove(predicateName);
		var leaves = result.Leaves
		                   .Select(x => x with { Label = $"@{predicateName} / {x.Label}" })
		                   .ToList();
		return result with { Leaves = leaves };
	}

	private static EmploymentConditionExpressionEvaluation EvaluateNot(EmploymentConditionExpression expression,
		IReadOnlyList<IEmploymentTaskCondition> conditions, IEmploymentTaskContext context, DateTimeOffset now,
		HashSet<string> predicateStack)
	{
		var child = expression.ChildExpressions.FirstOrDefault();
		if (child is null)
		{
			return Failed("A NOT expression must have one child.",
				new EmploymentConditionLeafEvaluation("not", false, "A NOT expression must have one child."));
		}

		var result = EvaluateExpression(child, conditions, context, now, predicateStack);
		return new EmploymentConditionExpressionEvaluation(
			!result.Satisfied,
			!result.Satisfied ? string.Empty : "Negated expression was satisfied.",
			result.Leaves);
	}

	private static EmploymentConditionExpressionEvaluation EvaluateAny(EmploymentConditionExpression expression,
		IReadOnlyList<IEmploymentTaskCondition> conditions, IEmploymentTaskContext context, DateTimeOffset now,
		HashSet<string> predicateStack)
	{
		var leaves = new List<EmploymentConditionLeafEvaluation>();
		var reasons = new List<string>();
		foreach (var child in expression.ChildExpressions)
		{
			var result = EvaluateExpression(child, conditions, context, now, predicateStack);
			leaves.AddRange(result.Leaves);
			if (result.Satisfied)
			{
				return new EmploymentConditionExpressionEvaluation(true, string.Empty, leaves);
			}

			if (!string.IsNullOrWhiteSpace(result.Reason))
			{
				reasons.Add(result.Reason);
			}
		}

		var reason = reasons.Any() ? string.Join("; ", reasons) : "No OR branch was satisfied.";
		return new EmploymentConditionExpressionEvaluation(false, reason, leaves);
	}

	private static EmploymentConditionExpressionEvaluation EvaluateAll(EmploymentConditionExpression expression,
		IReadOnlyList<IEmploymentTaskCondition> conditions, IEmploymentTaskContext context, DateTimeOffset now,
		HashSet<string> predicateStack)
	{
		var leaves = new List<EmploymentConditionLeafEvaluation>();
		foreach (var child in expression.ChildExpressions)
		{
			var result = EvaluateExpression(child, conditions, context, now, predicateStack);
			leaves.AddRange(result.Leaves);
			if (!result.Satisfied)
			{
				return new EmploymentConditionExpressionEvaluation(false, result.Reason, leaves);
			}
		}

		return new EmploymentConditionExpressionEvaluation(true, string.Empty, leaves);
	}

	private static EmploymentConditionExpressionEvaluation Failed(string reason,
		EmploymentConditionLeafEvaluation leaf)
	{
		return new EmploymentConditionExpressionEvaluation(false, reason, [leaf]);
	}

	private static EmploymentAuthority RequiredAuthority(EmploymentConditionExpression expression,
		IReadOnlyList<IEmploymentTaskCondition> conditions, IEmploymentTaskBoard board, HashSet<string> predicateStack)
	{
		switch (expression.Kind)
		{
			case EmploymentConditionExpressionKind.Condition:
				return expression.ConditionNumber is { } number && number >= 1 && number <= conditions.Count
					? conditions[number - 1].RequiredAuthority.Authorities
					: EmploymentAuthority.None;
			case EmploymentConditionExpressionKind.Predicate:
				if (string.IsNullOrWhiteSpace(expression.PredicateName) || !predicateStack.Add(expression.PredicateName))
				{
					return EmploymentAuthority.None;
				}

				var predicate = board.ConditionPredicates.FirstOrDefault(x => x.Name.EqualTo(expression.PredicateName));
				var authority = predicate is null
					? EmploymentAuthority.None
					: RequiredAuthority(
						predicate.ConditionExpression ?? EmploymentConditionExpression.All(
							Enumerable.Range(1, predicate.Conditions.Count).Select(EmploymentConditionExpression.Condition)),
						predicate.Conditions.ToList(),
						board,
						predicateStack);
				predicateStack.Remove(expression.PredicateName);
				return authority;
			default:
				return expression.ChildExpressions.Aggregate(EmploymentAuthority.None,
					(current, child) => current | RequiredAuthority(child, conditions, board, predicateStack));
		}
	}

	private static bool Validate(EmploymentConditionExpression expression,
		IReadOnlyList<IEmploymentTaskCondition> conditions, IEmploymentTaskBoard board, HashSet<string> predicateStack,
		out string reason)
	{
		reason = string.Empty;
		switch (expression.Kind)
		{
			case EmploymentConditionExpressionKind.Condition:
				if (expression.ConditionNumber is < 1 or null || expression.ConditionNumber > conditions.Count)
				{
					reason = $"Condition #{expression.ConditionNumber?.ToString("N0") ?? "?"} does not exist.";
					return false;
				}

				return true;
			case EmploymentConditionExpressionKind.Predicate:
				if (string.IsNullOrWhiteSpace(expression.PredicateName))
				{
					reason = "A named predicate reference is blank.";
					return false;
				}

				if (!predicateStack.Add(expression.PredicateName))
				{
					reason = $"Named predicate @{expression.PredicateName} is cyclic.";
					return false;
				}

				var predicate = board.ConditionPredicates.FirstOrDefault(x => x.Name.EqualTo(expression.PredicateName));
				if (predicate is null)
				{
					predicateStack.Remove(expression.PredicateName);
					reason = $"Named predicate @{expression.PredicateName} does not exist.";
					return false;
				}

				var expressionToCheck = predicate.ConditionExpression ?? EmploymentConditionExpression.All(
					Enumerable.Range(1, predicate.Conditions.Count).Select(EmploymentConditionExpression.Condition));
				var valid = Validate(expressionToCheck, predicate.Conditions.ToList(), board, predicateStack, out reason);
				predicateStack.Remove(expression.PredicateName);
				return valid;
			case EmploymentConditionExpressionKind.Not:
				if (expression.ChildExpressions.Count != 1)
				{
					reason = "A NOT expression must have exactly one child.";
					return false;
				}

				return Validate(expression.ChildExpressions.Single(), conditions, board, predicateStack, out reason);
			case EmploymentConditionExpressionKind.Any:
			case EmploymentConditionExpressionKind.All:
				if (expression.Kind == EmploymentConditionExpressionKind.Any && !expression.ChildExpressions.Any())
				{
					reason = $"{expression.Kind.DescribeEnum()} expressions must have at least one child.";
					return false;
				}

				foreach (var child in expression.ChildExpressions)
				{
					if (!Validate(child, conditions, board, predicateStack, out reason))
					{
						return false;
					}
				}

				return true;
			default:
				reason = "Unknown scheduled-rule condition expression node.";
				return false;
		}
	}
}

public sealed class EmploymentTaskBoard : IEmploymentTaskBoard
{
	private const string PhysicalCustodySuspensionReason =
		"The task is suspended for manager review because physical task items may still be in the assigned worker's custody.";

	private readonly IEmploymentHost _host;
	private readonly List<IEmploymentScheduledTaskRule> _scheduledRules = new();
	private readonly List<IEmploymentConditionPredicate> _conditionPredicates = new();
	private readonly List<IEmploymentScheduledRuleTemplate> _scheduledRuleTemplates = new();
	private readonly List<IEmploymentActiveTask> _activeTasks = new();
	private readonly IEmploymentPersistenceStore? _persistence;

	public EmploymentTaskBoard(IEmploymentHost host)
	{
		_host = host;
	}

	internal EmploymentTaskBoard(IEmploymentHost host, IEmploymentPersistenceStore persistence,
		IEnumerable<IEmploymentScheduledTaskRule> scheduledRules, IEnumerable<IEmploymentActiveTask> activeTasks,
		IEnumerable<IEmploymentConditionPredicate>? conditionPredicates = null,
		IEnumerable<IEmploymentScheduledRuleTemplate>? scheduledRuleTemplates = null)
	{
		_host = host;
		_persistence = persistence;
		_scheduledRules.AddRange(scheduledRules);
		_activeTasks.AddRange(activeTasks);
		_conditionPredicates.AddRange(conditionPredicates ?? []);
		_scheduledRuleTemplates.AddRange(scheduledRuleTemplates ?? []);
	}

	public IReadOnlyCollection<IEmploymentScheduledTaskRule> ScheduledRules => _scheduledRules;
	public IReadOnlyCollection<IEmploymentConditionPredicate> ConditionPredicates => _conditionPredicates;
	public IReadOnlyCollection<IEmploymentScheduledRuleTemplate> ScheduledRuleTemplates => _scheduledRuleTemplates;
	public IReadOnlyCollection<IEmploymentActiveTask> ActiveTasks => _activeTasks;

	private bool IsAuthorised(ICharacter? actor, EmploymentAuthority authority)
	{
		return actor is null || actor.IsAdministrator() || _host.HasAuthority(actor, authority);
	}

	private static string ActorName(ICharacter? actor)
	{
		return actor?.HowSeen(actor, colour: false) ?? "No actor";
	}

	private static EmploymentPrincipal PrincipalForActor(ICharacter? actor, string fallbackLabel)
	{
		return actor is null ? EmploymentPrincipal.HostSystem(fallbackLabel) : EmploymentPrincipal.ForCharacter(actor);
	}

	internal static EmploymentTaskProvenance CreateTaskProvenance(IEmploymentHost employer,
		EmploymentActionPlan actionPlan, Guid correlationId, EmploymentTaskSourceKind sourceKind,
		EmploymentPrincipal createdByPrincipal, EmploymentPrincipal authorisedByPrincipal, int priority,
		DateTimeOffset createdAt, DateTimeOffset? dueAt = null, Guid? sourceRuleId = null,
		long? sourceGoalId = null)
	{
		var (limits, allowsUnboundedFinancialSteps, paymentSourceScopes, counterpartyScopes) =
			AuthorisationLimitsFor(employer, actionPlan);
		var grant = new EmploymentAuthorisationGrant(
			Guid.NewGuid(),
			authorisedByPrincipal,
			actionPlan.RequiredAuthority,
			limits,
			allowsUnboundedFinancialSteps,
			createdAt,
			null,
			correlationId,
			paymentSourceScopes,
			counterpartyScopes);

		return new EmploymentTaskProvenance(
			sourceKind,
			sourceRuleId,
			sourceGoalId,
			createdByPrincipal,
			authorisedByPrincipal,
			grant,
			priority,
			createdAt,
			dueAt);
	}

	internal static EmploymentTaskProvenance CreateCompatibilityTaskProvenance(Guid correlationId,
		IEmploymentHost employer, string creatorLabel, string authoriserLabel)
	{
		var createdAt = EmploymentClock.CurrentInstant(employer);
		var createdBy = EmploymentPrincipal.HostSystem(creatorLabel);
		var authorisedBy = EmploymentPrincipal.HostSystem(authoriserLabel);
		var grant = new EmploymentAuthorisationGrant(
			Guid.NewGuid(),
			authorisedBy,
			EmploymentAuthoritySet.Empty,
			new Dictionary<long, decimal>(),
			false,
			createdAt,
			null,
			correlationId,
			new HashSet<string>(StringComparer.InvariantCultureIgnoreCase),
			new HashSet<string>(StringComparer.InvariantCultureIgnoreCase));

		return new EmploymentTaskProvenance(
			EmploymentTaskSourceKind.HostSystem,
			null,
			null,
			createdBy,
			authorisedBy,
			grant,
			0,
			createdAt);
	}

	private static (IReadOnlyDictionary<long, decimal> Limits, bool AllowsUnboundedFinancialSteps,
		IReadOnlySet<string> PaymentSourceScopes, IReadOnlySet<string> CounterpartyScopes)
		AuthorisationLimitsFor(IEmploymentHost employer, EmploymentActionPlan actionPlan)
	{
		var limits = new Dictionary<long, decimal>();
		var paymentSourceScopes = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		var counterpartyScopes = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
		var allowsUnboundedFinancialSteps = false;
		foreach (var authorisation in actionPlan.Steps
		                                     .OfType<CataloguedActionShellStep>()
		                                     .Where(x => x.ActionKey.EqualTo("authorise")))
		{
			if (authorisation.Amount is null)
			{
				allowsUnboundedFinancialSteps = true;
				continue;
			}

			limits[authorisation.Amount.Currency.Id] =
				limits.GetValueOrDefault(authorisation.Amount.Currency.Id) + authorisation.Amount.Amount;
		}

		foreach (var step in actionPlan.Steps.Where(x => x.RequiresPaymentAuthorisation))
		{
			var scopes = FinancialAuthorisationScopesFor(employer, step);
			AddFinancialScope(paymentSourceScopes, scopes.PaymentSourceScope);
			AddFinancialScope(counterpartyScopes, scopes.CounterpartyScope);
		}

		return (limits, allowsUnboundedFinancialSteps, paymentSourceScopes, counterpartyScopes);
	}

	internal sealed record EmploymentFinancialAuthorisationScopes(string? PaymentSourceScope, string? CounterpartyScope);

	private static void AddFinancialScope(HashSet<string> scopes, string? scope)
	{
		if (string.IsNullOrWhiteSpace(scope))
		{
			return;
		}

		scopes.Add(EmploymentAuthorisationGrant.NormaliseScope(scope));
	}

	internal static EmploymentFinancialAuthorisationScopes FinancialAuthorisationScopesFor(IEmploymentHost employer,
		IEmploymentActionStep step)
	{
		return step switch
		{
			PurchaseActionStep purchase => new EmploymentFinancialAuthorisationScopes(
				HostScope(employer, "available-funds"),
				$"purchase:{purchase.TargetKind}:{purchase.SupplierSelector ?? "any"}:{purchase.PurchaseDescription}"),
			BankDepositActionStep => new EmploymentFinancialAuthorisationScopes(
				HostScope(employer, "virtual-cash"),
				HostScope(employer, "linked-bank-account")),
			BankWithdrawalActionStep => new EmploymentFinancialAuthorisationScopes(
				HostScope(employer, "linked-bank-account"),
				HostScope(employer, "virtual-cash")),
			BankAccountTransferActionStep transfer => new EmploymentFinancialAuthorisationScopes(
				HostScope(employer, "linked-bank-account"),
				$"bank-account:{transfer.TargetAccountKey}"),
			BankAdministrationActionStep bankAdmin => bankAdmin.Operation switch
			{
				BankAdministrationActionKind.ReserveDeposit => new EmploymentFinancialAuthorisationScopes(
					HostScope(employer, "virtual-cash"),
					HostScope(employer, "bank-reserve")),
				BankAdministrationActionKind.ReserveWithdrawal => new EmploymentFinancialAuthorisationScopes(
					HostScope(employer, "bank-reserve"),
					HostScope(employer, "virtual-cash")),
				BankAdministrationActionKind.AccountCredit => new EmploymentFinancialAuthorisationScopes(
					HostScope(employer, "account-service"),
					$"bank-account:{bankAdmin.AccountSelector}"),
				_ => new EmploymentFinancialAuthorisationScopes(null, null)
			},
			HostSettlementActionStep settlement => new EmploymentFinancialAuthorisationScopes(
				HostScope(employer, "available-funds"),
				$"employment-host:{settlement.TargetHostKey}"),
			StoreAccountPaymentActionStep storePayment => new EmploymentFinancialAuthorisationScopes(
				HostScope(employer, "available-funds"),
				$"store-account:{storePayment.AccountName}"),
			TaxPaymentActionStep => new EmploymentFinancialAuthorisationScopes(
				HostScope(employer, "available-funds"),
				$"tax-authority:{employer.FrameworkItemType}:{employer.Id}"),
			ShopFloatAdjustmentActionStep shopFloat => shopFloat.FillRegister
				? new EmploymentFinancialAuthorisationScopes(HostScope(employer, "virtual-cash"), HostScope(employer, "cash-register"))
				: new EmploymentFinancialAuthorisationScopes(HostScope(employer, "cash-register"), HostScope(employer, "virtual-cash")),
			PhysicalFloatActionStep physicalFloat => physicalFloat.Operation switch
			{
				PhysicalFloatOperation.Issue => new EmploymentFinancialAuthorisationScopes(
					HostScope(employer, $"physical-float-source:{physicalFloat.TargetKind}"),
					$"task-custody:{physicalFloat.Operation}"),
				PhysicalFloatOperation.Return => new EmploymentFinancialAuthorisationScopes(
					$"task-custody:{physicalFloat.Operation}",
					HostScope(employer, $"physical-float-target:{physicalFloat.TargetKind}")),
				PhysicalFloatOperation.Settle => new EmploymentFinancialAuthorisationScopes(
					$"task-custody:{physicalFloat.Operation}",
					HostScope(employer, "virtual-cash")),
				_ => new EmploymentFinancialAuthorisationScopes(null, null)
			},
			CataloguedActionShellStep shell => new EmploymentFinancialAuthorisationScopes(
				HostScope(employer, $"catalogued:{shell.ActionKey}"),
				$"catalogued:{shell.ActionKey}:{shell.ActionDescription}"),
			_ => new EmploymentFinancialAuthorisationScopes(null, null)
		};
	}

	private static string HostScope(IEmploymentHost employer, string scope)
	{
		return $"host:{employer.FrameworkItemType}:{employer.Id}:{scope}";
	}

	public IEmploymentScheduledTaskRule CreateScheduledRule(string name, string idempotencyKey,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentActionPlan actionPlan, TimeSpan cooldown,
		ICharacter? authorisedBy)
	{
		return CreateScheduledRule(name, idempotencyKey, conditions, null, actionPlan, cooldown, authorisedBy);
	}

	public IEmploymentScheduledTaskRule CreateScheduledRule(string name, string idempotencyKey,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentConditionExpression? conditionExpression,
		EmploymentActionPlan actionPlan, TimeSpan cooldown, ICharacter? authorisedBy)
	{
		var conditionList = conditions.ToList();
		if (!IsAuthorised(authorisedBy, EmploymentAuthority.CreateScheduledRules))
		{
			throw new InvalidOperationException($"{ActorName(authorisedBy)} is not authorised to create scheduled task rules for {_host.EmploymentHostName}.");
		}

		if (!EmploymentConditionExpressionEvaluator.Validate(conditionExpression, conditionList, this, out var expressionReason))
		{
			throw new InvalidOperationException(expressionReason);
		}

		var conditionAuthority = EmploymentConditionExpressionEvaluator
		                         .RequiredAuthority(conditionExpression, conditionList, this)
		                         .Authorities;
		if (conditionAuthority != EmploymentAuthority.None &&
		    !IsAuthorised(authorisedBy, conditionAuthority))
		{
			throw new InvalidOperationException($"{ActorName(authorisedBy)} is not authorised to create scheduled task rules with {conditionAuthority.DescribeEnum()} condition authority for {_host.EmploymentHostName}.");
		}

		if (actionPlan.RequiredAuthority.Authorities != EmploymentAuthority.None &&
		    !IsAuthorised(authorisedBy, actionPlan.RequiredAuthority.Authorities))
		{
			throw new InvalidOperationException($"{ActorName(authorisedBy)} is not authorised to create scheduled task rules with {actionPlan.RequiredAuthority.Authorities.DescribeEnum()} authority for {_host.EmploymentHostName}.");
		}

		var rule = new EmploymentScheduledTaskRule(_host, name, idempotencyKey, conditionList, conditionExpression,
			actionPlan, cooldown);
		_scheduledRules.Add(rule);
		_persistence?.SaveScheduledRule(rule);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ScheduledRuleCreated, authorisedBy,
			$"Created scheduled task rule {name}.");
		_host.DebugEmployment($"Created scheduled task rule {name}.");
		return rule;
	}

	public IEmploymentConditionPredicate CreateConditionPredicate(string name,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentConditionExpression? conditionExpression,
		ICharacter? authorisedBy)
	{
		var conditionList = conditions.ToList();
		if (!IsAuthorised(authorisedBy, EmploymentAuthority.CreateScheduledRules))
		{
			throw new InvalidOperationException($"{ActorName(authorisedBy)} is not authorised to create scheduled condition predicates for {_host.EmploymentHostName}.");
		}

		if (_conditionPredicates.Any(x => x.Name.EqualTo(name)))
		{
			throw new InvalidOperationException($"A scheduled condition predicate named {name} already exists for {_host.EmploymentHostName}.");
		}

		if (!EmploymentConditionExpressionEvaluator.Validate(conditionExpression, conditionList, this, out var expressionReason))
		{
			throw new InvalidOperationException(expressionReason);
		}

		var conditionAuthority = EmploymentConditionExpressionEvaluator
		                         .RequiredAuthority(conditionExpression, conditionList, this)
		                         .Authorities;
		if (conditionAuthority != EmploymentAuthority.None && !IsAuthorised(authorisedBy, conditionAuthority))
		{
			throw new InvalidOperationException($"{ActorName(authorisedBy)} is not authorised to create scheduled condition predicates with {conditionAuthority.DescribeEnum()} condition authority for {_host.EmploymentHostName}.");
		}

		var predicate = new EmploymentConditionPredicate(_host, name, conditionList, conditionExpression);
		_conditionPredicates.Add(predicate);
		_persistence?.SaveConditionPredicate(predicate);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ScheduledRuleCreated, authorisedBy,
			$"Created scheduled condition predicate {name}.");
		_host.DebugEmployment($"Created scheduled condition predicate {name}.");
		return predicate;
	}

	public bool CancelConditionPredicate(IEmploymentConditionPredicate predicate, ICharacter? cancelledBy, string reason)
	{
		if (!IsAuthorised(cancelledBy, EmploymentAuthority.ModifyScheduledRules))
		{
			throw new InvalidOperationException($"{ActorName(cancelledBy)} is not authorised to cancel scheduled condition predicates for {_host.EmploymentHostName}.");
		}

		if (predicate is not EmploymentConditionPredicate concrete || !_conditionPredicates.Contains(predicate))
		{
			return false;
		}

		_conditionPredicates.Remove(predicate);
		_persistence?.DeleteConditionPredicate(concrete);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ScheduledRuleCancelled, cancelledBy,
			$"Cancelled scheduled condition predicate {predicate.Name}: {reason}");
		_host.DebugEmployment($"Cancelled scheduled condition predicate {predicate.Name}: {reason}", cancelledBy?.Gameworld);
		return true;
	}

	public IEmploymentScheduledRuleTemplate CreateScheduledRuleTemplate(string name, string idempotencyKeyPattern,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentConditionExpression? conditionExpression,
		EmploymentActionPlan actionPlan, TimeSpan cooldown, ICharacter? authorisedBy)
	{
		var conditionList = conditions.ToList();
		if (!IsAuthorised(authorisedBy, EmploymentAuthority.CreateScheduledRules))
		{
			throw new InvalidOperationException($"{ActorName(authorisedBy)} is not authorised to create scheduled rule templates for {_host.EmploymentHostName}.");
		}

		if (_scheduledRuleTemplates.Any(x => x.Name.EqualTo(name)))
		{
			throw new InvalidOperationException($"A scheduled rule template named {name} already exists for {_host.EmploymentHostName}.");
		}

		if (!EmploymentConditionExpressionEvaluator.Validate(conditionExpression, conditionList, this, out var expressionReason))
		{
			throw new InvalidOperationException(expressionReason);
		}

		var conditionAuthority = EmploymentConditionExpressionEvaluator
		                         .RequiredAuthority(conditionExpression, conditionList, this)
		                         .Authorities;
		if (conditionAuthority != EmploymentAuthority.None && !IsAuthorised(authorisedBy, conditionAuthority))
		{
			throw new InvalidOperationException($"{ActorName(authorisedBy)} is not authorised to create scheduled rule templates with {conditionAuthority.DescribeEnum()} condition authority for {_host.EmploymentHostName}.");
		}

		if (actionPlan.RequiredAuthority.Authorities != EmploymentAuthority.None &&
		    !IsAuthorised(authorisedBy, actionPlan.RequiredAuthority.Authorities))
		{
			throw new InvalidOperationException($"{ActorName(authorisedBy)} is not authorised to create scheduled rule templates with {actionPlan.RequiredAuthority.Authorities.DescribeEnum()} action authority for {_host.EmploymentHostName}.");
		}

		var template = new EmploymentScheduledRuleTemplate(_host, name, idempotencyKeyPattern, conditionList,
			conditionExpression, actionPlan, cooldown);
		_scheduledRuleTemplates.Add(template);
		_persistence?.SaveScheduledRuleTemplate(template);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ScheduledRuleCreated, authorisedBy,
			$"Created scheduled rule template {name}.");
		_host.DebugEmployment($"Created scheduled rule template {name}.");
		return template;
	}

	public bool CancelScheduledRuleTemplate(IEmploymentScheduledRuleTemplate template, ICharacter? cancelledBy,
		string reason)
	{
		if (!IsAuthorised(cancelledBy, EmploymentAuthority.ModifyScheduledRules))
		{
			throw new InvalidOperationException($"{ActorName(cancelledBy)} is not authorised to cancel scheduled rule templates for {_host.EmploymentHostName}.");
		}

		if (template is not EmploymentScheduledRuleTemplate concrete || !_scheduledRuleTemplates.Contains(template))
		{
			return false;
		}

		_scheduledRuleTemplates.Remove(template);
		_persistence?.DeleteScheduledRuleTemplate(concrete);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ScheduledRuleCancelled, cancelledBy,
			$"Cancelled scheduled rule template {template.Name}: {reason}");
		_host.DebugEmployment($"Cancelled scheduled rule template {template.Name}: {reason}", cancelledBy?.Gameworld);
		return true;
	}

	public IEmploymentActiveTask CreateActiveTask(string name, EmploymentActionPlan actionPlan, ICharacter? authorisedBy,
		Guid? correlationId = null, string? idempotencyKey = null, int priority = 0, DateTimeOffset? dueAt = null,
		EmploymentTaskProvenance? provenance = null)
	{
		if (!IsAuthorised(authorisedBy, EmploymentAuthority.AssignTasks))
		{
			throw new InvalidOperationException($"{ActorName(authorisedBy)} is not authorised to create tasks for {_host.EmploymentHostName}.");
		}

		if (actionPlan.RequiredAuthority.Authorities != EmploymentAuthority.None &&
		    !IsAuthorised(authorisedBy, actionPlan.RequiredAuthority.Authorities))
		{
			throw new InvalidOperationException($"{ActorName(authorisedBy)} is not authorised to create tasks with {actionPlan.RequiredAuthority.Authorities.DescribeEnum()} authority for {_host.EmploymentHostName}.");
		}

		var actualCorrelationId = correlationId ?? Guid.NewGuid();
		var createdAt = EmploymentClock.CurrentInstant(_host);
		var taskProvenance = provenance ?? CreateTaskProvenance(
			_host,
			actionPlan,
			actualCorrelationId,
			authorisedBy is null ? EmploymentTaskSourceKind.HostSystem : EmploymentTaskSourceKind.Manual,
			PrincipalForActor(authorisedBy, "Host system task creator"),
			PrincipalForActor(authorisedBy, "Host system task authoriser"),
			priority,
			createdAt,
			dueAt);
		var task = new EmploymentActiveTask(_host, name, actionPlan, actualCorrelationId, _persistence, taskProvenance)
		{
			IdempotencyKey = string.IsNullOrWhiteSpace(idempotencyKey) ? string.Empty : idempotencyKey.Trim()
		};
		_activeTasks.Add(task);
		_persistence?.SaveActiveTask(task);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskCreated, authorisedBy,
			$"Created active task {name}.", task.CorrelationId);
		_host.DebugEmployment($"Created active task {name} ({task.CorrelationId}).", authorisedBy?.Gameworld);
		return task;
	}

	public bool CancelActiveTask(IEmploymentActiveTask task, ICharacter? cancelledBy, string reason)
	{
		if (!IsAuthorised(cancelledBy, EmploymentAuthority.CancelTasks))
		{
			throw new InvalidOperationException($"{ActorName(cancelledBy)} is not authorised to cancel tasks for {_host.EmploymentHostName}.");
		}

		if (task is not EmploymentActiveTask concrete || !_activeTasks.Contains(task))
		{
			return false;
		}

		if (task.Status is EmploymentTaskStatus.Completed or EmploymentTaskStatus.Cancelled or EmploymentTaskStatus.Failed)
		{
			return false;
		}

		reason = string.IsNullOrWhiteSpace(reason) ? "Cancelled by a manager." : reason.Trim();
		concrete.Cancel(reason);
		EmploymentCraftService.ReleaseCraftReservations(concrete, cancelledBy?.Gameworld);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskCancelled, cancelledBy,
			$"Cancelled active task {task.Name}: {reason}", task.CorrelationId);
		_host.DebugEmployment($"Cancelled active task {task.Name}: {reason}", cancelledBy?.Gameworld);
		return true;
	}

	public bool RetryActiveTask(IEmploymentActiveTask task, ICharacter? retriedBy, string reason)
	{
		if (!IsAuthorised(retriedBy, EmploymentAuthority.AssignTasks))
		{
			throw new InvalidOperationException($"{ActorName(retriedBy)} is not authorised to retry tasks for {_host.EmploymentHostName}.");
		}

		if (task is not EmploymentActiveTask concrete || !_activeTasks.Contains(task))
		{
			return false;
		}

		if (task.Status is not EmploymentTaskStatus.Blocked and not EmploymentTaskStatus.Failed)
		{
			return false;
		}

		if (TryBlockAdministrativeCustodyRisk(concrete, retriedBy, "retry", out _))
		{
			return false;
		}

		reason = string.IsNullOrWhiteSpace(reason) ? "Retried by a manager." : reason.Trim();
		if (!concrete.RequeueForAdministration(reason, clearCurrentFailure: true, allowFailedStep: true))
		{
			return false;
		}

		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskRequeued, retriedBy,
			$"Retried active task {task.Name}: {reason}", task.CorrelationId);
		_host.DebugEmployment($"Retried active task {task.Name}: {reason}", retriedBy?.Gameworld);
		return true;
	}

	public bool RequeueActiveTask(IEmploymentActiveTask task, ICharacter? requeuedBy, string reason)
	{
		if (!IsAuthorised(requeuedBy, EmploymentAuthority.AssignTasks))
		{
			throw new InvalidOperationException($"{ActorName(requeuedBy)} is not authorised to requeue tasks for {_host.EmploymentHostName}.");
		}

		if (task is not EmploymentActiveTask concrete || !_activeTasks.Contains(task))
		{
			return false;
		}

		if (task.Status is EmploymentTaskStatus.Completed or EmploymentTaskStatus.Cancelled or EmploymentTaskStatus.Failed)
		{
			return false;
		}

		if (TryBlockAdministrativeCustodyRisk(concrete, requeuedBy, "requeue", out _))
		{
			return false;
		}

		reason = string.IsNullOrWhiteSpace(reason) ? "Requeued by a manager." : reason.Trim();
		if (!concrete.RequeueForAdministration(reason, clearCurrentFailure: false, allowFailedStep: false))
		{
			return false;
		}

		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskRequeued, requeuedBy,
			$"Requeued active task {task.Name}: {reason}", task.CorrelationId);
		_host.DebugEmployment($"Requeued active task {task.Name}: {reason}", requeuedBy?.Gameworld);
		return true;
	}

	public bool AssignActiveTask(IEmploymentActiveTask task, ICharacter employee, IEmploymentTaskContext context,
		ICharacter? assignedBy, string reason)
	{
		if (!IsAuthorised(assignedBy, EmploymentAuthority.AssignTasks))
		{
			throw new InvalidOperationException($"{ActorName(assignedBy)} is not authorised to assign tasks for {_host.EmploymentHostName}.");
		}

		if (task is not EmploymentActiveTask concrete || !_activeTasks.Contains(task))
		{
			return false;
		}

		if (task.Status is EmploymentTaskStatus.Completed or EmploymentTaskStatus.Cancelled or EmploymentTaskStatus.Failed)
		{
			return false;
		}

		if (!_host.EmploymentContracts.Any(x => x.Employee.Id == employee.Id && x.Status == EmploymentStatus.Active))
		{
			return false;
		}

		var nextStepIndex = concrete.NextStepIndex;
		if (nextStepIndex < 0 || nextStepIndex >= task.ActionPlan.Steps.Count)
		{
			return false;
		}

		if (task.AssignedEmployee is not null &&
		    task.AssignedEmployee.Id != employee.Id &&
		    TryBlockAdministrativeCustodyRisk(concrete, assignedBy, "reassign", out _))
		{
			return false;
		}

		context.HydrateTaskState(task, nextStepIndex);
		var nextStep = task.ActionPlan.Steps[nextStepIndex];
		if (!nextStep.CanExecute(context, employee, out var stepReason))
		{
			concrete.Block(stepReason);
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskBlocked, assignedBy,
				$"Could not assign active task {task.Name} to {employee.Name}: {stepReason}", task.CorrelationId);
			_host.DebugEmployment($"Could not assign active task {task.Name} to {employee.Name}: {stepReason}",
				assignedBy?.Gameworld);
			return false;
		}

		reason = string.IsNullOrWhiteSpace(reason) ? "Assigned by a manager." : reason.Trim();
		if (task.AssignedEmployee is not null && task.AssignedEmployee.Id != employee.Id)
		{
			concrete.RequeueForAdministration(reason, clearCurrentFailure: false, allowFailedStep: false);
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskRequeued, assignedBy,
				$"Released {task.Name} for reassignment: {reason}", task.CorrelationId);
		}

		concrete.Assign(employee);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskAssigned, employee,
			$"Assigned task {task.Name} by {ActorName(assignedBy)}: {reason}", task.CorrelationId);
		_host.DebugEmployment($"Assigned active task {task.Name} to {employee.Name}: {reason}", assignedBy?.Gameworld);
		return true;
	}

	private bool TryBlockAdministrativeCustodyRisk(EmploymentActiveTask task, ICharacter? actor, string operation,
		out string reason)
	{
		if (!TryGetUnsecuredTaskItemCustodyReason(task, out var custodyReason))
		{
			reason = string.Empty;
			return false;
		}

		reason = $"Cannot {operation} active task {task.Name}: {custodyReason}";
		task.Block(reason);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskBlocked, actor, reason, task.CorrelationId);
		_host.DebugEmployment(reason, actor?.Gameworld);
		return true;
	}

	public bool CancelScheduledRule(IEmploymentScheduledTaskRule rule, ICharacter? cancelledBy, string reason)
	{
		if (!IsAuthorised(cancelledBy, EmploymentAuthority.ModifyScheduledRules))
		{
			throw new InvalidOperationException($"{ActorName(cancelledBy)} is not authorised to cancel scheduled task rules for {_host.EmploymentHostName}.");
		}

		if (rule is not EmploymentScheduledTaskRule concrete || !_scheduledRules.Contains(rule))
		{
			return false;
		}

		reason = string.IsNullOrWhiteSpace(reason) ? "Cancelled by a manager." : reason.Trim();
		_scheduledRules.Remove(rule);
		_persistence?.DeleteScheduledRule(concrete);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ScheduledRuleCancelled, cancelledBy,
			$"Cancelled scheduled task rule {rule.Name}: {reason}");
		_host.DebugEmployment($"Cancelled scheduled task rule {rule.Name}: {reason}", cancelledBy?.Gameworld);
		return true;
	}

	public bool PauseScheduledRule(IEmploymentScheduledTaskRule rule, ICharacter? pausedBy, string reason)
	{
		if (!IsAuthorised(pausedBy, EmploymentAuthority.ModifyScheduledRules))
		{
			throw new InvalidOperationException($"{ActorName(pausedBy)} is not authorised to pause scheduled task rules for {_host.EmploymentHostName}.");
		}

		if (rule is not EmploymentScheduledTaskRule concrete || !_scheduledRules.Contains(rule))
		{
			return false;
		}

		if (concrete.Status == EmploymentScheduledRuleStatus.Paused)
		{
			return true;
		}

		reason = string.IsNullOrWhiteSpace(reason) ? "Paused by a manager." : reason.Trim();
		concrete.Pause();
		_persistence?.SaveScheduledRuleState(concrete);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ScheduledRulePaused, pausedBy,
			$"Paused scheduled task rule {rule.Name}: {reason}");
		_host.DebugEmployment($"Paused scheduled task rule {rule.Name}: {reason}", pausedBy?.Gameworld);
		return true;
	}

	public bool ResumeScheduledRule(IEmploymentScheduledTaskRule rule, ICharacter? resumedBy, string reason)
	{
		if (!IsAuthorised(resumedBy, EmploymentAuthority.ModifyScheduledRules))
		{
			throw new InvalidOperationException($"{ActorName(resumedBy)} is not authorised to resume scheduled task rules for {_host.EmploymentHostName}.");
		}

		if (rule is not EmploymentScheduledTaskRule concrete || !_scheduledRules.Contains(rule))
		{
			return false;
		}

		if (concrete.Status == EmploymentScheduledRuleStatus.Active)
		{
			return true;
		}

		reason = string.IsNullOrWhiteSpace(reason) ? "Resumed by a manager." : reason.Trim();
		concrete.Resume();
		_persistence?.SaveScheduledRuleState(concrete);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ScheduledRuleResumed, resumedBy,
			$"Resumed scheduled task rule {rule.Name}: {reason}");
		_host.DebugEmployment($"Resumed scheduled task rule {rule.Name}: {reason}", resumedBy?.Gameworld);
		return true;
	}

	public IReadOnlyCollection<EmploymentTaskAssignmentAuditResult> AuditActiveTaskAssignments()
	{
		EmploymentTaskAssignmentAuditResult ResolveAssignmentProblem(EmploymentActiveTask task, string reason)
		{
			if (TryGetUnsecuredTaskItemCustodyReason(task, out var itemReason))
			{
				var blockedReason =
					$"{reason} {itemReason}";
				task.Block(blockedReason);
				_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskBlocked, task.AssignedEmployee,
					blockedReason, task.CorrelationId);
				_host.DebugEmployment($"Suspended active task {task.Name}: {blockedReason}",
					task.AssignedEmployee?.Gameworld);
				return new EmploymentTaskAssignmentAuditResult(task.Id, task.Name,
					EmploymentTaskAssignmentAuditOutcome.Blocked, blockedReason);
			}

			var requeueReason = $"{reason} The task has been returned to pending for reassignment.";
			var previousEmployee = task.AssignedEmployee;
			task.ReleaseAssignment(requeueReason);
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskRequeued, previousEmployee,
				requeueReason, task.CorrelationId);
			_host.DebugEmployment($"Requeued active task {task.Name}: {requeueReason}",
				previousEmployee?.Gameworld);
			return new EmploymentTaskAssignmentAuditResult(task.Id, task.Name,
				EmploymentTaskAssignmentAuditOutcome.Requeued, requeueReason);
		}

		var results = new List<EmploymentTaskAssignmentAuditResult>();
		foreach (var task in _activeTasks.OfType<EmploymentActiveTask>()
		                                 .Where(x => x.Status is EmploymentTaskStatus.Assigned or
			                                 EmploymentTaskStatus.InProgress or EmploymentTaskStatus.Blocked)
		                                 .ToList())
		{
			if (task.Status == EmploymentTaskStatus.Blocked &&
			    task.BlockedReason?.Contains(PhysicalCustodySuspensionReason,
				    StringComparison.InvariantCultureIgnoreCase) == true)
			{
				continue;
			}

			if (TryGetResourceCustodyAuditReason(task, out var custodyReason))
			{
				task.Block(custodyReason);
				_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskBlocked, task.AssignedEmployee,
					custodyReason, task.CorrelationId);
				_host.DebugEmployment($"Suspended active task {task.Name}: {custodyReason}",
					task.AssignedEmployee?.Gameworld);
				results.Add(new EmploymentTaskAssignmentAuditResult(task.Id, task.Name,
					EmploymentTaskAssignmentAuditOutcome.Blocked, custodyReason));
				continue;
			}

			if (TryGetLogisticsResourceAuditReason(task, out var logisticsReason))
			{
				results.Add(ResolveAssignmentProblem(task, logisticsReason));
				continue;
			}

			if (!TryGetAssignmentAuditReason(task, out var reason))
			{
				continue;
			}

			results.Add(ResolveAssignmentProblem(task, reason));
		}

		return results;
	}

	public IReadOnlyCollection<IEmploymentActiveTask> EvaluateScheduledRules(IEmploymentTaskContext context, DateTimeOffset now)
	{
		AuditActiveTaskAssignments();
		var spawned = new List<IEmploymentActiveTask>();
		foreach (var rule in _scheduledRules.OfType<EmploymentScheduledTaskRule>())
		{
			spawned.AddRange(EvaluateScheduledRule(rule, context, now));
		}

		return spawned;
	}

	public IReadOnlyCollection<IEmploymentActiveTask> EvaluateScheduledRule(IEmploymentScheduledTaskRule rule,
		IEmploymentTaskContext context, DateTimeOffset now)
	{
		if (rule is not EmploymentScheduledTaskRule concrete || !_scheduledRules.Contains(rule))
		{
			return [];
		}

		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ScheduledRuleEvaluated, null,
			$"Evaluated scheduled task rule {concrete.Name}.");
		_host.DebugEmployment($"Evaluating scheduled task rule {concrete.Name}.");
		if (!concrete.CanSpawn(context, now, out var spawnReason))
		{
			_host.DebugEmployment($"Scheduled task rule {concrete.Name} did not spawn: {spawnReason}");
			return [];
		}

		if (HasBlockingActiveTask(concrete.IdempotencyKey))
		{
			_host.DebugEmployment(
				$"Scheduled task rule {concrete.Name} did not spawn because an active task with idempotency key {concrete.IdempotencyKey} already exists.");
			return [];
		}

		var correlationId = Guid.NewGuid();
		var rulePrincipal = EmploymentPrincipal.ForScheduledRule(concrete);
		var task = new EmploymentActiveTask(_host, concrete.Name, concrete.ActionPlan, correlationId, _persistence,
			CreateTaskProvenance(
				_host,
				concrete.ActionPlan,
				correlationId,
				EmploymentTaskSourceKind.ScheduledRule,
				rulePrincipal,
				rulePrincipal,
				0,
				now,
				sourceRuleId: concrete.Id))
		{
			IdempotencyKey = concrete.IdempotencyKey
		};
		_activeTasks.Add(task);
		_persistence?.SaveActiveTask(task);
		concrete.MarkSpawned(now);
		_persistence?.SaveScheduledRuleState(concrete);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskCreated, null,
			$"Spawned active task {concrete.Name} from scheduled rule.", task.CorrelationId);
		_host.DebugEmployment($"Scheduled task rule {concrete.Name} spawned active task {task.CorrelationId}.");
		return [task];
	}

	public bool HasBlockingActiveTask(string idempotencyKey)
	{
		if (string.IsNullOrWhiteSpace(idempotencyKey))
		{
			return false;
		}

		AuditActiveTaskAssignments();
		return _activeTasks.OfType<EmploymentActiveTask>().Any(x =>
			x.IdempotencyKey.Equals(idempotencyKey, StringComparison.InvariantCultureIgnoreCase) &&
			x.Status is EmploymentTaskStatus.Pending or EmploymentTaskStatus.Assigned or EmploymentTaskStatus.InProgress or EmploymentTaskStatus.Blocked);
	}

	private bool TryGetAssignmentAuditReason(EmploymentActiveTask task, out string reason)
	{
		reason = string.Empty;
		var employee = task.AssignedEmployee;
		if (employee is null)
		{
			reason = "The task no longer has an assigned employee.";
			return true;
		}

		if (employee.State.IsDead())
		{
			reason = $"{employee.Name} is dead.";
			return true;
		}

		if (employee.State.IsInStatis())
		{
			reason = $"{employee.Name} is in stasis.";
			return true;
		}

		var activeContracts = _host.ActiveEmploymentContracts()
		                           .Where(x => x.Employee.Id == employee.Id)
		                           .ToList();
		if (!activeContracts.Any())
		{
			reason = $"{employee.Name} no longer has an active employment contract.";
			return true;
		}

		var nextStepIndex = task.NextStepIndex;
		if (nextStepIndex < 0 || nextStepIndex >= task.ActionPlan.Steps.Count)
		{
			return false;
		}

		var currentStep = task.ActionPlan.Steps[nextStepIndex];
		if (!activeContracts.Any(x => x.Authority.ContainsAll(currentStep.RequiredAuthority)) &&
		    !CurrentStepCanExecuteForAssignmentAudit(task, nextStepIndex, currentStep, employee))
		{
			reason = $"{employee.Name} no longer has the delegated authority required for this task step.";
			return true;
		}

		if (employee is not INPC npc)
		{
			return false;
		}

		var workerAis = npc.AIs.OfType<EmploymentWorkerAI>().ToList();
		if (!workerAis.Any())
		{
			reason = $"{employee.Name} no longer has an EmploymentWorkerAI.";
			return true;
		}

		var usableAi = workerAis.Any(ai =>
			ai.TaskingEnabled &&
			(ai.HostTypeFilter is null || ai.HostTypeFilter.Value == _host.EmploymentHostType) &&
			currentStep.RequiredCapabilities.All(x => ai.Capabilities.Contains(x)));
		if (!usableAi && !CurrentStepCanExecuteForAssignmentAudit(task, nextStepIndex, currentStep, employee))
		{
			reason = $"{employee.Name}'s EmploymentWorkerAI can no longer execute this task step.";
			return true;
		}

		return false;
	}

	private bool CurrentStepCanExecuteForAssignmentAudit(EmploymentActiveTask task, int nextStepIndex,
		IEmploymentActionStep currentStep, ICharacter employee)
	{
		var context = new EmploymentTaskContext(_host);
		context.HydrateTaskState(task, nextStepIndex);
		return currentStep.CanExecute(context, employee, out _);
	}

	private bool TryGetLogisticsResourceAuditReason(EmploymentActiveTask task, out string reason)
	{
		reason = string.Empty;
		var gameworld = task.AssignedEmployee?.Gameworld ?? (_host as IHaveFuturemud)?.Gameworld;
		if (gameworld is null)
		{
			return false;
		}

		var checkedVehicles = new HashSet<long>();
		var checkedMounts = new HashSet<long>();
		var checkedStables = new HashSet<long>();
		var checkedDestinations = new HashSet<long>();
		for (var i = 0; i < task.ActionPlan.Steps.Count && i < task.StepStates.Count; i++)
		{
			if (task.StepStates[i] != EmploymentActionStepStatus.Completed)
			{
				continue;
			}

			var state = task.StepOperationalStates.ElementAtOrDefault(i);
			if (!TryParseOperationalKeyValues(state?.SelectedResources, out var values) ||
			    !values.TryGetValue("operation", out var operation))
			{
				continue;
			}

			if (operation.EqualToAny("vehicleassign", "vehiclecargo") &&
			    TryGetLongValue(values, "vehicle", out var vehicleId) &&
			    checkedVehicles.Add(vehicleId) &&
			    !TryValidateAssignedVehicle(gameworld, vehicleId, out reason))
			{
				return true;
			}

			if (operation.EqualToAny("animallead", "animalride", "animallodge", "animalreturn"))
			{
				if ((TryGetLongValue(values, "mount", out var mountId) ||
				     TryGetLongValue(values, "originalmount", out mountId)) &&
				    checkedMounts.Add(mountId) &&
				    !TryValidateAssignedAnimal(gameworld, mountId, out reason))
				{
					return true;
				}

				if (TryGetLongValue(values, "stable", out var stableId) &&
				    checkedStables.Add(stableId) &&
				    !TryValidateStable(gameworld, stableId, out reason))
				{
					return true;
				}
			}

			foreach (var destinationId in LogisticsDestinationIds(values))
			{
				if (checkedDestinations.Add(destinationId) &&
				    !TryValidateLogisticsDestination(gameworld, destinationId, out reason))
				{
					return true;
				}
			}
		}

		return false;
	}

	private static bool TryValidateAssignedVehicle(IFuturemud gameworld, long vehicleId, out string reason)
	{
		reason = string.Empty;
		var vehicle = gameworld.Vehicles?.Get(vehicleId);
		if (vehicle is null)
		{
			reason = $"Assigned vehicle #{vehicleId:N0} is no longer loaded in the world.";
			return false;
		}

		if (vehicle.Destroyed)
		{
			reason = $"Assigned vehicle {vehicle.Name} (#{vehicleId:N0}) is destroyed.";
			return false;
		}

		if (vehicle.Disabled)
		{
			reason = $"Assigned vehicle {vehicle.Name} (#{vehicleId:N0}) is disabled.";
			return false;
		}

		if (vehicle.Location is null)
		{
			reason = $"Assigned vehicle {vehicle.Name} (#{vehicleId:N0}) no longer has a valid location.";
			return false;
		}

		return true;
	}

	private static bool TryValidateAssignedAnimal(IFuturemud gameworld, long animalId, out string reason)
	{
		reason = string.Empty;
		var animal = gameworld.TryGetCharacter(animalId, true);
		if (animal is null)
		{
			reason = $"Assigned animal #{animalId:N0} is no longer loaded in the world.";
			return false;
		}

		if (animal.State.IsDead())
		{
			reason = $"Assigned animal {animal.Name} (#{animalId:N0}) is dead.";
			return false;
		}

		if (animal.State.IsInStatis())
		{
			reason = $"Assigned animal {animal.Name} (#{animalId:N0}) is in stasis.";
			return false;
		}

		if (animal.Location is null)
		{
			reason = $"Assigned animal {animal.Name} (#{animalId:N0}) no longer has a valid location.";
			return false;
		}

		return true;
	}

	private static bool TryValidateStable(IFuturemud gameworld, long stableId, out string reason)
	{
		reason = string.Empty;
		var stable = gameworld.Stables?.Get(stableId);
		if (stable is null)
		{
			reason = $"Assigned stable #{stableId:N0} is no longer loaded in the world.";
			return false;
		}

		if (stable.Location is null)
		{
			reason = $"Assigned stable {stable.Name} (#{stableId:N0}) no longer has a valid location.";
			return false;
		}

		return true;
	}

	private static bool TryValidateLogisticsDestination(IFuturemud gameworld, long destinationId, out string reason)
	{
		reason = string.Empty;
		if (gameworld.Cells?.Get(destinationId) is not null)
		{
			return true;
		}

		reason = $"Recorded logistics destination #{destinationId:N0} is no longer loaded in the world.";
		return false;
	}

	private static IEnumerable<long> LogisticsDestinationIds(IReadOnlyDictionary<string, string> values)
	{
		foreach (var key in new[] { "destination", "final" })
		{
			if (TryGetLongValue(values, key, out var id))
			{
				yield return id;
			}
		}

		if (!values.TryGetValue("stops", out var stops) ||
		    stops.EqualTo("none"))
		{
			yield break;
		}

		foreach (var part in stops.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
		{
			if (long.TryParse(part, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
			{
				yield return id;
			}
		}
	}

	private static bool TryParseOperationalKeyValues(string? text, out Dictionary<string, string> values)
	{
		values = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		foreach (var pair in text.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
		                         .Select(x => x.Split('=', 2, StringSplitOptions.TrimEntries))
		                         .Where(x => x.Length == 2 && !string.IsNullOrWhiteSpace(x[0])))
		{
			values[pair[0]] = pair[1];
		}

		return values.Count > 0;
	}

	private static bool TryGetLongValue(IReadOnlyDictionary<string, string> values, string key, out long value)
	{
		value = 0;
		return values.TryGetValue(key, out var text) &&
		       long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
	}
	private bool TryGetResourceCustodyAuditReason(EmploymentActiveTask task, out string reason)
	{
		reason = string.Empty;
		if (!TryBuildCustodySnapshot(task, out var carriedIds, out var contextManagedCarriedIds,
			    out var loadedItemIds, out _))
		{
			return false;
		}

		var physicalCarriedIds = carriedIds.Except(contextManagedCarriedIds).ToHashSet();
		var employee = task.AssignedEmployee;
		var gameworld = employee?.Gameworld ?? (_host as IHaveFuturemud)?.Gameworld;
		if (gameworld is null)
		{
			return false;
		}

		if (employee is not null)
		{
			foreach (var itemId in physicalCarriedIds.OrderBy(x => x))
			{
				var item = gameworld.TryGetItem(itemId, true);
				if (item is null)
				{
					reason = $"Task item #{itemId:N0} is no longer loaded in the world. {PhysicalCustodySuspensionReason}";
					return true;
				}

				if (!ActorCarriesItem(employee, item))
				{
					reason =
						$"{employee.Name} is no longer carrying task item {item.HowSeen(employee, colour: false)} (#{item.Id:N0}). {PhysicalCustodySuspensionReason}";
					return true;
				}
			}
		}
		else if (physicalCarriedIds.Any())
		{
			reason = $"The task has no assigned employee but still has carried task item custody recorded. {PhysicalCustodySuspensionReason}";
			return true;
		}

		foreach (var (containerId, itemIds) in loadedItemIds.OrderBy(x => x.Key))
		{
			var container = gameworld.TryGetItem(containerId, true);
			if (container is null)
			{
				reason =
					$"Task-loaded container #{containerId:N0} is no longer loaded in the world. {PhysicalCustodySuspensionReason}";
				return true;
			}

			var contents = container.DeepItems?.Select(x => x.Id).ToHashSet() ?? [];
			foreach (var itemId in itemIds.OrderBy(x => x))
			{
				if (contents.Contains(itemId))
				{
					continue;
				}

				var item = gameworld.TryGetItem(itemId, true);
				reason = item is null
					? $"Task-loaded item #{itemId:N0} is no longer loaded in the world. {PhysicalCustodySuspensionReason}"
					: $"Task-loaded item {item.Name} (#{itemId:N0}) is no longer inside {container.Name}. {PhysicalCustodySuspensionReason}";
				return true;
			}
		}

		return false;
	}

	private bool TryGetUnsecuredTaskItemCustodyReason(EmploymentActiveTask task, out string reason)
	{
		if (TryBuildCustodySnapshot(task, out var carriedIds, out var contextManagedCarriedIds,
			    out var loadedItemIds, out var inferredCarried) &&
		    (inferredCarried || carriedIds.Except(contextManagedCarriedIds).Any() ||
		     loadedItemIds.Any(x => x.Value.Any())))
		{
			reason = PhysicalCustodySuspensionReason;
			return true;
		}

		reason = string.Empty;
		return false;
	}

	private static bool TryBuildCustodySnapshot(EmploymentActiveTask task, out HashSet<long> carriedIds,
		out HashSet<long> contextManagedCarriedIds, out Dictionary<long, HashSet<long>> loadedItemIds,
		out bool inferredCarried)
	{
		carriedIds = [];
		contextManagedCarriedIds = [];
		loadedItemIds = new Dictionary<long, HashSet<long>>();
		inferredCarried = false;
		for (var i = 0; i < task.ActionPlan.Steps.Count && i < task.StepStates.Count; i++)
		{
			if (task.StepStates[i] != EmploymentActionStepStatus.Completed)
			{
				continue;
			}

			var state = task.StepOperationalStates.ElementAtOrDefault(i);
			if (state is null)
			{
				continue;
			}

			if (EmploymentTaskContext.TryParseTaskItemCustody(state.SelectedResources, out var custodyOperation, out _,
				    out var custodyItemIds, out var custodyBundleIds, out var custodyContextManagedItemIds))
			{
				if (custodyOperation.EqualTo("collect") || custodyOperation.EqualTo("unload"))
				{
					foreach (var itemId in custodyItemIds.Concat(custodyBundleIds))
					{
						carriedIds.Add(itemId);
					}

					foreach (var itemId in custodyContextManagedItemIds)
					{
						contextManagedCarriedIds.Add(itemId);
					}
				}
				else if (custodyOperation.EqualTo("load") ||
				         custodyOperation.EqualTo("deliver") ||
				         custodyOperation.EqualTo("return"))
				{
					foreach (var itemId in custodyItemIds.Concat(custodyBundleIds))
					{
						carriedIds.Remove(itemId);
						contextManagedCarriedIds.Remove(itemId);
					}
				}
			}

			if (EmploymentTaskContext.TryParseLoadedAssets(state.LoadedAssets, out var operation, out var containerId, out var loadedIds))
			{
				if (!loadedItemIds.TryGetValue(containerId, out var itemSet))
				{
					itemSet = [];
					loadedItemIds[containerId] = itemSet;
				}

				if (operation.EqualTo("load"))
				{
					foreach (var itemId in loadedIds)
					{
						itemSet.Add(itemId);
						carriedIds.Remove(itemId);
						contextManagedCarriedIds.Remove(itemId);
					}
				}
				else if (operation.EqualTo("unload"))
				{
					foreach (var itemId in loadedIds)
					{
						itemSet.Remove(itemId);
						carriedIds.Add(itemId);
					}
				}
				else if (operation.EqualTo("return"))
				{
					foreach (var itemId in loadedIds)
					{
						itemSet.Remove(itemId);
						carriedIds.Remove(itemId);
						contextManagedCarriedIds.Remove(itemId);
					}
				}
			}

			if (carriedIds.Any() || loadedItemIds.Any(x => x.Value.Any()))
			{
				continue;
			}

			switch (task.ActionPlan.Steps[i].StepType)
			{
				case EmploymentActionStepType.GetItemsById:
				case EmploymentActionStepType.GetItemsByTag:
				case EmploymentActionStepType.GetCommodity:
				case EmploymentActionStepType.UnloadItems:
					inferredCarried = true;
					break;
				case EmploymentActionStepType.LoadItems:
				case EmploymentActionStepType.DeliverItems:
				case EmploymentActionStepType.ReturnAsset:
					inferredCarried = false;
					break;
			}
		}

		return inferredCarried || carriedIds.Except(contextManagedCarriedIds).Any() ||
		       loadedItemIds.Any(x => x.Value.Any());
	}
	private static bool ActorCarriesItem(ICharacter actor, IGameItem item)
	{
		return EmploymentWorkerItemLocator.IsHeldOrWielded(actor, item);
	}
}

public sealed class EmploymentScheduledTaskRule : IEmploymentScheduledTaskRule
{
	private readonly List<IEmploymentTaskCondition> _conditions;

	public EmploymentScheduledTaskRule(IEmploymentHost employer, string name, string idempotencyKey,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentConditionExpression? conditionExpression,
		EmploymentActionPlan actionPlan, TimeSpan cooldown)
	{
		Id = Guid.NewGuid();
		Employer = employer;
		Name = name;
		IdempotencyKey = string.IsNullOrWhiteSpace(idempotencyKey) ? Id.ToString("N") : idempotencyKey.Trim();
		_conditions = conditions.ToList();
		ConditionExpression = conditionExpression;
		ActionPlan = actionPlan;
		Cooldown = cooldown;
	}

	internal EmploymentScheduledTaskRule(Guid id, IEmploymentHost employer, string name, string idempotencyKey,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentConditionExpression? conditionExpression,
		EmploymentActionPlan actionPlan, TimeSpan cooldown,
		DateTimeOffset? lastSpawnedAt, EmploymentScheduledRuleStatus status = EmploymentScheduledRuleStatus.Active)
	{
		Id = id;
		Employer = employer;
		Name = name;
		IdempotencyKey = string.IsNullOrWhiteSpace(idempotencyKey) ? Id.ToString("N") : idempotencyKey.Trim();
		_conditions = conditions.ToList();
		ConditionExpression = conditionExpression;
		ActionPlan = actionPlan;
		Cooldown = cooldown;
		LastSpawnedAt = lastSpawnedAt;
		Status = status;
	}

	public Guid Id { get; }
	public IEmploymentHost Employer { get; }
	public string Name { get; }
	public string IdempotencyKey { get; }
	public IReadOnlyCollection<IEmploymentTaskCondition> Conditions => _conditions;
	public EmploymentConditionExpression? ConditionExpression { get; }
	public EmploymentActionPlan ActionPlan { get; }
	public EmploymentScheduledRuleStatus Status { get; private set; } = EmploymentScheduledRuleStatus.Active;
	public TimeSpan Cooldown { get; }
	public DateTimeOffset? LastSpawnedAt { get; private set; }

	public bool CanSpawn(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		if (Status == EmploymentScheduledRuleStatus.Paused)
		{
			reason = "Rule is paused.";
			return false;
		}

		if (LastSpawnedAt.HasValue && now - LastSpawnedAt.Value < Cooldown)
		{
			reason = "Rule is still inside its cooldown window.";
			return false;
		}

		var result = EmploymentConditionExpressionEvaluator.Evaluate(ConditionExpression, _conditions, context, now);
		if (!result.Satisfied)
		{
			reason = result.Reason;
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public void MarkSpawned(DateTimeOffset now)
	{
		LastSpawnedAt = now;
	}

	public void Pause()
	{
		Status = EmploymentScheduledRuleStatus.Paused;
	}

	public void Resume()
	{
		Status = EmploymentScheduledRuleStatus.Active;
	}
}

public sealed class EmploymentConditionPredicate : IEmploymentConditionPredicate
{
	private readonly List<IEmploymentTaskCondition> _conditions;

	public EmploymentConditionPredicate(IEmploymentHost employer, string name,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentConditionExpression? conditionExpression)
		: this(Guid.NewGuid(), employer, name, conditions, conditionExpression)
	{
	}

	internal EmploymentConditionPredicate(Guid id, IEmploymentHost employer, string name,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentConditionExpression? conditionExpression)
	{
		Id = id;
		Employer = employer;
		Name = name.Trim();
		_conditions = conditions.ToList();
		ConditionExpression = conditionExpression;
		RequiredAuthority = new EmploymentAuthoritySet(_conditions.Aggregate(EmploymentAuthority.None,
			(current, condition) => current | condition.RequiredAuthority.Authorities));
	}

	public Guid Id { get; }
	public IEmploymentHost Employer { get; }
	public string Name { get; }
	public IReadOnlyCollection<IEmploymentTaskCondition> Conditions => _conditions;
	public EmploymentConditionExpression? ConditionExpression { get; }
	public EmploymentAuthoritySet RequiredAuthority { get; }
}

public sealed class EmploymentScheduledRuleTemplate : IEmploymentScheduledRuleTemplate
{
	private readonly List<IEmploymentTaskCondition> _conditions;

	public EmploymentScheduledRuleTemplate(IEmploymentHost employer, string name, string idempotencyKeyPattern,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentConditionExpression? conditionExpression,
		EmploymentActionPlan actionPlan, TimeSpan cooldown)
		: this(Guid.NewGuid(), employer, name, idempotencyKeyPattern, conditions, conditionExpression, actionPlan,
			cooldown)
	{
	}

	internal EmploymentScheduledRuleTemplate(Guid id, IEmploymentHost employer, string name, string idempotencyKeyPattern,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentConditionExpression? conditionExpression,
		EmploymentActionPlan actionPlan, TimeSpan cooldown)
	{
		Id = id;
		Employer = employer;
		Name = name.Trim();
		IdempotencyKeyPattern = string.IsNullOrWhiteSpace(idempotencyKeyPattern)
			? name.CollapseString().ToLowerInvariant()
			: idempotencyKeyPattern.Trim();
		_conditions = conditions.ToList();
		ConditionExpression = conditionExpression;
		ActionPlan = actionPlan;
		Cooldown = cooldown;
		RequiredAuthority = new EmploymentAuthoritySet(
			_conditions.Aggregate(EmploymentAuthority.None,
				(current, condition) => current | condition.RequiredAuthority.Authorities) |
			actionPlan.RequiredAuthority.Authorities);
	}

	public Guid Id { get; }
	public IEmploymentHost Employer { get; }
	public string Name { get; }
	public string IdempotencyKeyPattern { get; }
	public IReadOnlyCollection<IEmploymentTaskCondition> Conditions => _conditions;
	public EmploymentConditionExpression? ConditionExpression { get; }
	public EmploymentActionPlan ActionPlan { get; }
	public TimeSpan Cooldown { get; }
	public EmploymentAuthoritySet RequiredAuthority { get; }
}

public sealed class EmploymentActiveTask : IEmploymentActiveTask
{
	private readonly List<EmploymentActionStepStatus> _stepStates;
	private readonly List<EmploymentActionStepOperationalState> _stepOperationalStates;
	private readonly IEmploymentPersistenceStore? _persistence;

	public EmploymentActiveTask(IEmploymentHost employer, string name, EmploymentActionPlan actionPlan, Guid correlationId,
		EmploymentTaskProvenance? provenance = null)
	{
		Id = Guid.NewGuid();
		Employer = employer;
		Name = name;
		ActionPlan = actionPlan;
		CorrelationId = correlationId;
		Provenance = provenance ?? EmploymentTaskBoard.CreateCompatibilityTaskProvenance(
			correlationId,
			employer,
			"Legacy active task creator",
			"Legacy active task authoriser");
		_stepStates = actionPlan.Steps.Select(_ => EmploymentActionStepStatus.Pending).ToList();
		_stepOperationalStates = actionPlan.Steps.Select(_ => EmploymentActionStepOperationalState.Empty).ToList();
	}

	internal EmploymentActiveTask(IEmploymentHost employer, string name, EmploymentActionPlan actionPlan,
		Guid correlationId, IEmploymentPersistenceStore? persistence, EmploymentTaskProvenance? provenance = null)
		: this(employer, name, actionPlan, correlationId, provenance)
	{
		_persistence = persistence;
	}

	internal EmploymentActiveTask(Guid id, IEmploymentHost employer, string name, EmploymentActionPlan actionPlan,
		EmploymentTaskStatus status, ICharacter? assignedEmployee, string? blockedReason,
		IEnumerable<EmploymentActionStepStatus> stepStates,
		IEnumerable<EmploymentActionStepOperationalState> stepOperationalStates,
		Guid correlationId, string idempotencyKey,
		IEmploymentPersistenceStore persistence, EmploymentTaskProvenance? provenance = null)
	{
		Id = id;
		Employer = employer;
		Name = name;
		ActionPlan = actionPlan;
		Status = status;
		AssignedEmployee = assignedEmployee;
		BlockedReason = blockedReason;
		CorrelationId = correlationId;
		IdempotencyKey = idempotencyKey;
		_persistence = persistence;
		Provenance = provenance ?? EmploymentTaskBoard.CreateCompatibilityTaskProvenance(
			correlationId,
			employer,
			"Loaded active task creator",
			"Loaded active task authoriser");
		_stepStates = stepStates.ToList();
		if (_stepStates.Count != actionPlan.Steps.Count)
		{
			_stepStates = actionPlan.Steps.Select(_ => EmploymentActionStepStatus.Pending).ToList();
		}

		_stepOperationalStates = stepOperationalStates.ToList();
		if (_stepOperationalStates.Count != actionPlan.Steps.Count)
		{
			_stepOperationalStates = actionPlan.Steps.Select(_ => EmploymentActionStepOperationalState.Empty).ToList();
		}
	}

	public Guid Id { get; }
	public IEmploymentHost Employer { get; }
	public string Name { get; }
	public EmploymentActionPlan ActionPlan { get; }
	public EmploymentTaskStatus Status { get; private set; } = EmploymentTaskStatus.Pending;
	public ICharacter? AssignedEmployee { get; private set; }
	public string? BlockedReason { get; private set; }
	public IReadOnlyList<EmploymentActionStepStatus> StepStates => _stepStates;
	public IReadOnlyList<EmploymentActionStepOperationalState> StepOperationalStates => _stepOperationalStates;
	public Guid CorrelationId { get; }
	public string IdempotencyKey { get; init; } = string.Empty;
	public EmploymentTaskProvenance Provenance { get; private set; }
	public EmploymentTaskSourceKind SourceKind => Provenance.SourceKind;
	public Guid? SourceRuleId => Provenance.SourceRuleId;
	public long? SourceGoalId => Provenance.SourceGoalId;
	public EmploymentPrincipal CreatedByPrincipal => Provenance.CreatedByPrincipal;
	public EmploymentPrincipal AuthorisedByPrincipal => Provenance.AuthorisedByPrincipal;
	public EmploymentAuthorisationGrant AuthorisationGrant => Provenance.AuthorisationGrant;
	public int Priority => Provenance.Priority;
	public DateTimeOffset CreatedAt => Provenance.CreatedAt;
	public DateTimeOffset? DueAt => Provenance.DueAt;
	public DateTimeOffset? AssignedAt => Provenance.AssignedAt;

	public int NextStepIndex => _stepStates.FindIndex(x => x is EmploymentActionStepStatus.Pending or EmploymentActionStepStatus.InProgress or EmploymentActionStepStatus.Blocked);

	public void Assign(ICharacter employee)
	{
		AssignedEmployee = employee;
		Status = EmploymentTaskStatus.Assigned;
		BlockedReason = null;
		Provenance = Provenance with { AssignedAt = EmploymentClock.CurrentInstant(Employer) };
		_persistence?.SaveActiveTaskState(this);
	}

	public void Block(string reason)
	{
		Status = EmploymentTaskStatus.Blocked;
		BlockedReason = reason;
		_persistence?.SaveActiveTaskState(this);
	}

	internal void BlockForCompatibilityLoad(string reason)
	{
		Status = EmploymentTaskStatus.Blocked;
		BlockedReason = reason;
	}

	public void Cancel(string reason)
	{
		Status = EmploymentTaskStatus.Cancelled;
		BlockedReason = reason;
		_persistence?.SaveActiveTaskState(this);
	}

	public bool RequeueForAdministration(string reason, bool clearCurrentFailure, bool allowFailedStep)
	{
		var index = _stepStates.FindIndex(x =>
			x is EmploymentActionStepStatus.Pending or EmploymentActionStepStatus.InProgress or EmploymentActionStepStatus.Blocked ||
			allowFailedStep && x == EmploymentActionStepStatus.Failed);
		if (index < 0 || index >= _stepStates.Count)
		{
			return false;
		}

		if (_stepStates[index] == EmploymentActionStepStatus.Failed && !allowFailedStep)
		{
			return false;
		}

		if (_stepStates[index] is EmploymentActionStepStatus.InProgress or EmploymentActionStepStatus.Blocked or EmploymentActionStepStatus.Failed)
		{
			_stepStates[index] = EmploymentActionStepStatus.Pending;
			_stepOperationalStates[index] = clearCurrentFailure
				? _stepOperationalStates[index].WithoutFailure()
				: _stepOperationalStates[index].WithFailure(reason);
		}

		AssignedEmployee = null;
		Status = EmploymentTaskStatus.Pending;
		BlockedReason = reason;
		Provenance = Provenance with { AssignedAt = null };
		_persistence?.SaveActiveTaskState(this);
		return true;
	}
	public void ReleaseAssignment(string reason)
	{
		var index = NextStepIndex;
		if (index >= 0 &&
		    index < _stepStates.Count &&
		    _stepStates[index] == EmploymentActionStepStatus.InProgress)
		{
			_stepStates[index] = EmploymentActionStepStatus.Pending;
			_stepOperationalStates[index] = _stepOperationalStates[index].WithFailure(reason);
		}

		AssignedEmployee = null;
		Status = EmploymentTaskStatus.Pending;
		BlockedReason = reason;
		_persistence?.SaveActiveTaskState(this);
	}

	public void MarkStep(int index, EmploymentActionStepStatus status,
		EmploymentActionStepOperationalState? operationalState = null)
	{
		_stepStates[index] = status;
		var mergedState = _stepOperationalStates[index];
		if (operationalState is not null && !operationalState.IsEmpty)
		{
			mergedState = mergedState.Merge(operationalState);
		}

		if (status == EmploymentActionStepStatus.Completed)
		{
			mergedState = mergedState.WithoutFailure();
		}

		_stepOperationalStates[index] = mergedState;
		var now = EmploymentClock.CurrentInstant(Employer);
		if (status is EmploymentActionStepStatus.InProgress or EmploymentActionStepStatus.Completed &&
		    Provenance.StartedAt is null)
		{
			Provenance = Provenance with { StartedAt = now };
		}

		if (_stepStates.All(x => x == EmploymentActionStepStatus.Completed))
		{
			Status = EmploymentTaskStatus.Completed;
			BlockedReason = null;
			Provenance = Provenance with { CompletedAt = now };
		}
		else if (status == EmploymentActionStepStatus.Failed)
		{
			Status = EmploymentTaskStatus.Failed;
		}
		else if (status is EmploymentActionStepStatus.InProgress or EmploymentActionStepStatus.Completed &&
		         Status is EmploymentTaskStatus.Assigned or EmploymentTaskStatus.Blocked)
		{
			Status = EmploymentTaskStatus.InProgress;
			BlockedReason = null;
		}

		_persistence?.SaveActiveTaskState(this);
	}
}

public sealed class EmploymentTaskDispatcher
{
	public bool TryAssignTask(IEmploymentActiveTask task, IEnumerable<EmploymentCandidateProfile> candidates,
		IEmploymentTaskContext context, out string reason, bool blockWhenNoCandidateMatches = true)
	{
		if (task is not EmploymentActiveTask concrete)
		{
			reason = "Unsupported active task implementation.";
			context.Employer.DebugEmployment($"Could not assign task {task.Name}: {reason}");
			return false;
		}

		var rejectionReasons = new List<string>();
		foreach (var candidate in candidates)
		{
			if (!task.Employer.EmploymentContracts.Any(x =>
				    x.Employee.Id == candidate.Candidate.Id &&
				    x.Status == EmploymentStatus.Active))
			{
				rejectionReasons.Add($"{candidate.Candidate.Name}: no active employment contract");
				context.Employer.DebugEmployment(
					$"Skipped {candidate.Candidate.Name} for task {task.Name}: no active employment contract.");
				continue;
			}

			var nextStepIndex = concrete.NextStepIndex;
			if (nextStepIndex < 0)
			{
				rejectionReasons.Add($"{candidate.Candidate.Name}: task has no pending steps");
				context.Employer.DebugEmployment($"Skipped task {task.Name}: it has no pending steps.");
				continue;
			}

			var nextStep = task.ActionPlan.Steps[nextStepIndex];
			var missingCapabilities = nextStep.RequiredCapabilities
			                          .Where(x => !candidate.Capabilities.Contains(x))
			                          .ToList();
			if (missingCapabilities.Any())
			{
				rejectionReasons.Add(
					$"{candidate.Candidate.Name}: missing {missingCapabilities.Select(x => x.DescribeEnum()).ListToString()} capability");
				context.Employer.DebugEmployment(
					$"Skipped {candidate.Candidate.Name} for task {task.Name}: missing required AI capabilities for step {nextStepIndex + 1:N0}.");
				continue;
			}

			context.HydrateTaskState(task, nextStepIndex);
			if (!nextStep.CanExecute(context, candidate.Candidate, out var stepReason))
			{
				rejectionReasons.Add($"{candidate.Candidate.Name}: {stepReason}");
				context.Employer.DebugEmployment(
					$"Skipped {candidate.Candidate.Name} for task {task.Name}: next step cannot execute ({stepReason}).");
				continue;
			}

			concrete.Assign(candidate.Candidate);
			context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskAssigned, candidate.Candidate,
				$"Assigned task {task.Name}.", task.CorrelationId);
			context.Employer.DebugEmployment($"Assigned task {task.Name} to {candidate.Candidate.Name}.",
				candidate.Candidate.Gameworld);
			reason = string.Empty;
			return true;
		}

		reason = rejectionReasons.Any()
			? $"No active employee is eligible to complete the task: {rejectionReasons.ListToString()}."
			: "No active employee is eligible to complete the task.";
		if (!blockWhenNoCandidateMatches)
		{
			return false;
		}

		concrete.Block(reason);
		context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskBlocked, null, reason, task.CorrelationId);
		context.Employer.DebugEmployment($"Blocked task {task.Name}: {reason}");
		return false;
	}

	public EmploymentActionStepResult AdvanceTask(IEmploymentActiveTask task, IEmploymentTaskContext context)
	{
		if (task is not EmploymentActiveTask concrete)
		{
			context.Employer.DebugEmployment($"Could not advance task {task.Name}: unsupported active task implementation.");
			return EmploymentActionStepResult.Failed("Unsupported active task implementation.");
		}

		if (task.AssignedEmployee is null)
		{
			concrete.Block("Task has no assigned employee.");
			context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskBlocked, null, concrete.BlockedReason!, task.CorrelationId);
			context.Employer.DebugEmployment($"Blocked task {task.Name}: {concrete.BlockedReason}");
			return EmploymentActionStepResult.Blocked(concrete.BlockedReason!);
		}

		var index = concrete.NextStepIndex;
		if (index < 0)
		{
			context.Employer.DebugEmployment($"Task {task.Name} is already complete.");
			return EmploymentActionStepResult.CompletedResult("Task is already complete.");
		}

		var step = task.ActionPlan.Steps[index];
		ApplyDurablePaymentAuthorisations(concrete, context, index);
		context.HydrateTaskState(task, index);
		if (!step.CanExecute(context, task.AssignedEmployee, out var reason))
		{
			concrete.MarkStep(index, EmploymentActionStepStatus.Blocked,
				concrete.StepOperationalStates[index].WithFailure(reason));
			concrete.Block(reason);
			context.RecordRegister(EmploymentRegisterEntryType.ActionStepFailed, task.AssignedEmployee, reason, task.CorrelationId);
			context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskBlocked, task.AssignedEmployee, reason, task.CorrelationId);
			context.Employer.DebugEmployment(
				$"Blocked task {task.Name} at step {index + 1:N0} ({step.StepType.DescribeEnum()}): {reason}",
				task.AssignedEmployee.Gameworld);
			return EmploymentActionStepResult.Blocked(reason);
		}

		concrete.MarkStep(index, EmploymentActionStepStatus.InProgress);
		context.RecordRegister(EmploymentRegisterEntryType.ActionStepStarted, task.AssignedEmployee,
			$"Started {step.StepType} action.", task.CorrelationId);
		context.Employer.DebugEmployment(
			$"{task.AssignedEmployee.Name} started step {index + 1:N0} ({step.StepType.DescribeEnum()}) of task {task.Name}.",
			task.AssignedEmployee.Gameworld);
		var result = step.Execute(context, task.AssignedEmployee);
		var operationalState = result.OperationalState;
		if (!result.Success)
		{
			operationalState = concrete.StepOperationalStates[index]
			                           .Merge(result.OperationalState)
			                           .WithFailure(result.Message);
		}

		concrete.MarkStep(index, result.Success
			? result.Completed ? EmploymentActionStepStatus.Completed : EmploymentActionStepStatus.InProgress
			: result.Completed ? EmploymentActionStepStatus.Failed : EmploymentActionStepStatus.Blocked,
			operationalState);
		context.RecordRegister(result.Success && result.Completed
				? EmploymentRegisterEntryType.ActionStepCompleted
				: result.Success ? EmploymentRegisterEntryType.ActionStepStarted : EmploymentRegisterEntryType.ActionStepFailed,
			task.AssignedEmployee,
			result.Message,
			task.CorrelationId);
		if (!result.Success && !result.Completed)
		{
			concrete.Block(result.Message);
			context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskBlocked, task.AssignedEmployee,
				result.Message, task.CorrelationId);
		}

		context.Employer.DebugEmployment(
			$"{task.AssignedEmployee.Name} {(result.Success ? "completed" : "did not complete")} step {index + 1:N0} of task {task.Name}: {result.Message}",
			task.AssignedEmployee.Gameworld);
		if (result.Success &&
		    result.Completed &&
		    concrete.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Failed &&
		    TryReleaseAssignmentForNextStep(concrete, context, out var previousEmployee, out var requeueReason))
		{
			context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskRequeued, previousEmployee,
				requeueReason, task.CorrelationId);
			context.Employer.DebugEmployment(requeueReason, previousEmployee?.Gameworld);
		}

		if (concrete.Status == EmploymentTaskStatus.Completed)
		{
			EmploymentCraftService.ReleaseCraftReservations(concrete, task.AssignedEmployee.Gameworld);
			context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskCompleted, task.AssignedEmployee,
				$"Completed active task {task.Name}.", task.CorrelationId);
			context.Employer.DebugEmployment($"Completed active task {task.Name}.", task.AssignedEmployee.Gameworld);
		}
		else if (concrete.Status == EmploymentTaskStatus.Failed)
		{
			EmploymentCraftService.ReleaseCraftReservations(concrete, task.AssignedEmployee.Gameworld);
		}

		return result;
	}

	private static bool TryReleaseAssignmentForNextStep(EmploymentActiveTask task, IEmploymentTaskContext context,
		out ICharacter? previousEmployee, out string reason)
	{
		previousEmployee = task.AssignedEmployee;
		reason = string.Empty;
		if (previousEmployee is null)
		{
			return false;
		}

		var nextStepIndex = task.NextStepIndex;
		if (nextStepIndex < 0 || nextStepIndex >= task.ActionPlan.Steps.Count)
		{
			return false;
		}

		context.HydrateTaskState(task, nextStepIndex);
		ApplyDurablePaymentAuthorisations(task, context, nextStepIndex);
		context.HydrateTaskState(task, nextStepIndex);
		var nextStep = task.ActionPlan.Steps[nextStepIndex];
		if (nextStep.CanExecute(context, previousEmployee, out _))
		{
			return false;
		}

		if (context.CarriedTaskItems(previousEmployee).Any())
		{
			return false;
		}

		reason =
			$"Released {previousEmployee.Name} from task {task.Name} after step {nextStepIndex:N0} because step {nextStepIndex + 1:N0} requires another worker.";
		task.ReleaseAssignment(reason);
		return true;
	}

	private static void ApplyDurablePaymentAuthorisations(EmploymentActiveTask task, IEmploymentTaskContext context,
		int currentStepIndex)
	{
		var currentStep = task.ActionPlan.Steps[currentStepIndex];
		if (!currentStep.RequiresPaymentAuthorisation)
		{
			return;
		}

		if (TaskGrantAuthorisesStep(task, currentStep, currentStepIndex))
		{
			context.AuthorisePaymentFor(currentStep, null, task.CorrelationId, recordRegister: false);
		}
	}

	private static bool TaskGrantAuthorisesStep(EmploymentActiveTask task, IEmploymentActionStep currentStep,
		int currentStepIndex)
	{
		var grant = task.AuthorisationGrant;
		if (!grant.CoversAuthority(currentStep.RequiredAuthority))
		{
			return false;
		}

		var scopes = EmploymentTaskBoard.FinancialAuthorisationScopesFor(task.Employer, currentStep);
		if (!grant.CoversPaymentSource(scopes.PaymentSourceScope) ||
		    !grant.CoversCounterparty(scopes.CounterpartyScope))
		{
			return false;
		}

		if (!TryGetFinancialStepAmount(currentStep, out var currentAmount))
		{
			return grant.AllowsUnboundedFinancialSteps;
		}

		var alreadyUsed = new Dictionary<long, decimal>();
		for (var i = 0; i < currentStepIndex; i++)
		{
			if (task.StepStates[i] != EmploymentActionStepStatus.Completed)
			{
				continue;
			}

			var step = task.ActionPlan.Steps[i];
			if (!step.RequiresPaymentAuthorisation || !TryGetFinancialStepAmount(step, out var spent))
			{
				continue;
			}

			alreadyUsed[spent.Currency.Id] = alreadyUsed.GetValueOrDefault(spent.Currency.Id) + spent.Amount;
		}

		return grant.Covers(currentAmount, alreadyUsed);
	}

	internal static bool TryGetFinancialStepAmount(IEmploymentActionStep step, out MoneyAmount amount)
	{
		switch (step)
		{
			case PurchaseActionStep purchase:
				if (purchase.IsExecutablePurchase && purchase.MaximumAmount is null)
				{
					amount = null!;
					return false;
				}

				amount = purchase.Amount;
				return true;
			case BankDepositActionStep deposit:
				amount = deposit.Amount;
				return true;
			case BankWithdrawalActionStep withdrawal:
				amount = withdrawal.Amount;
				return true;
			case BankAccountTransferActionStep transfer:
				amount = transfer.Amount;
				return true;
			case BankAdministrationActionStep { Amount: not null } bankAdmin when bankAdmin.RequiresPaymentAuthorisation:
				amount = bankAdmin.Amount;
				return true;
			case HostSettlementActionStep settlement:
				amount = settlement.Amount;
				return true;
			case StoreAccountPaymentActionStep storePayment:
				amount = storePayment.Amount;
				return true;
			case TaxPaymentActionStep { MaximumAmount: not null } tax:
				amount = tax.MaximumAmount;
				return true;
			case ShopFloatAdjustmentActionStep shopFloat:
				amount = shopFloat.Amount;
				return true;
			case PhysicalFloatActionStep { Amount: not null } physicalFloat:
				amount = physicalFloat.Amount;
				return true;
			case CataloguedActionShellStep { RequiresPaymentAuthorisation: true, Amount: not null } shell:
				amount = shell.Amount;
				return true;
			default:
				amount = null!;
				return false;
		}
	}
}

public sealed class ManagerGoalBoard : IManagerGoalBoard
{
	private readonly IEmploymentHost _host;
	private readonly List<IManagerGoal> _goals = new();
	private readonly IEmploymentPersistenceStore? _persistence;
	private long _nextId;

	public ManagerGoalBoard(IEmploymentHost host)
	{
		_host = host;
	}

	internal ManagerGoalBoard(IEmploymentHost host, IEmploymentPersistenceStore persistence, IEnumerable<IManagerGoal> goals)
	{
		_host = host;
		_persistence = persistence;
		_goals.AddRange(goals);
		_nextId = _goals.Select(x => x.Id).DefaultIfEmpty().Max();
	}

	public IReadOnlyCollection<IManagerGoal> Goals => _goals;

	public IManagerGoal CreateGoal(ManagerGoalDefinition definition, ICharacter authorisedBy)
	{
		var conditions = definition.Configuration.Conditions?.ToList() ?? [];
		if (!EmploymentConditionExpressionEvaluator.Validate(definition.Configuration.ConditionExpression, conditions, _host.TaskBoard, out var expressionReason))
		{
			throw new InvalidOperationException(expressionReason);
		}

		var requiredAuthority = RequiredAuthorityFor(definition, _host.TaskBoard);
		if (!_host.HasAuthority(authorisedBy, EmploymentAuthority.CreateManagerGoals))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to create manager goals for {_host.EmploymentHostName}.");
		}

		if (!authorisedBy.IsAdministrator() &&
		    !_host.EmploymentContracts.Where(x => x.Employee.Id == authorisedBy.Id && x.Status == EmploymentStatus.Active)
		          .Any(x => x.Authority.ContainsAll(requiredAuthority)))
		{
			throw new InvalidOperationException("A manager cannot create a goal that requires authority they do not possess.");
		}

		var policy = definition.Policy ?? ManagerGoalPolicy.Default;
		if (!TryValidateGoalPolicy(definition.Configuration.ActionPlan, policy, out var policyReason))
		{
			throw new InvalidOperationException(policyReason);
		}

		var goal = new ManagerGoal(
			Interlocked.Increment(ref _nextId),
			_host,
			definition.GoalType,
			requiredAuthority,
			ManagerGoalStatus.Active,
			definition.Configuration,
			definition.Priority,
			definition.EvaluationCadence,
			policy,
			Guid.NewGuid(),
			_persistence);
		_goals.Add(goal);
		_persistence?.SaveManagerGoal(goal);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalCreated, authorisedBy,
			$"Created manager goal {definition.GoalType}.", goal.CorrelationId);
		_host.DebugEmployment($"Created manager goal {definition.GoalType.DescribeEnum()} #{goal.Id:N0}.",
			authorisedBy.Gameworld);
		return goal;
	}

	private static EmploymentAuthoritySet RequiredAuthorityFor(ManagerGoalDefinition definition, IEmploymentTaskBoard taskBoard)
	{
		var authority = definition.RequiredAuthority.Authorities |
		                (definition.Configuration.ActionPlan?.RequiredAuthority.Authorities ?? EmploymentAuthority.None);
		var conditions = definition.Configuration.Conditions?.ToList() ?? [];
		authority |= RequiredGoalConditionAuthority(EmploymentConditionExpressionEvaluator
			.RequiredAuthority(definition.Configuration.ConditionExpression, conditions, taskBoard)
			.Authorities);

		return new EmploymentAuthoritySet(authority);
	}

	private static EmploymentAuthority RequiredGoalConditionAuthority(EmploymentAuthority authority)
	{
		return authority & ~EmploymentAuthority.CreateScheduledRules;
	}

	public void CancelGoal(IManagerGoal goal, ICharacter cancelledBy, string reason)
	{
		if (goal is not ManagerGoal concrete || !ReferenceEquals(concrete.Employer, _host))
		{
			return;
		}

		if (!_host.HasAuthority(cancelledBy, EmploymentAuthority.ModifyManagerGoals))
		{
			throw new InvalidOperationException($"{cancelledBy.HowSeen(cancelledBy, colour: false)} is not authorised to cancel manager goals for {_host.EmploymentHostName}.");
		}

		concrete.Cancel(reason);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalCancelled, cancelledBy, reason,
			concrete.CorrelationId);
		_host.DebugEmployment($"Cancelled manager goal #{concrete.Id:N0}: {reason}", cancelledBy.Gameworld);
	}

	public bool ReactivateGoal(IManagerGoal goal, ICharacter reactivatedBy, string reason)
	{
		if (goal is not ManagerGoal concrete || !ReferenceEquals(concrete.Employer, _host))
		{
			return false;
		}

		if (!_host.HasAuthority(reactivatedBy, EmploymentAuthority.ModifyManagerGoals))
		{
			throw new InvalidOperationException($"{reactivatedBy.HowSeen(reactivatedBy, colour: false)} is not authorised to reactivate manager goals for {_host.EmploymentHostName}.");
		}

		if (concrete.Status is ManagerGoalStatus.Cancelled or ManagerGoalStatus.Completed)
		{
			return false;
		}

		concrete.MarkActive(EmploymentClock.CurrentInstant(_host), reason);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, reactivatedBy, reason,
			concrete.CorrelationId);
		_host.DebugEmployment($"Reactivated manager goal #{concrete.Id:N0}: {reason}", reactivatedBy.Gameworld);
		return true;
	}

	public IReadOnlyCollection<IEmploymentActiveTask> EvaluateGoals(IEmploymentTaskContext context, DateTimeOffset now)
	{
		var tasks = new List<IEmploymentActiveTask>();
		foreach (var goal in _goals
		                     .OfType<ManagerGoal>()
		                     .Where(x => x.Status is ManagerGoalStatus.Active or ManagerGoalStatus.Satisfied)
		                     .OrderByDescending(x => x.Priority)
		                     .ThenBy(x => x.Id))
		{
			tasks.AddRange(EvaluateGoal(goal, context, now));
		}

		return tasks;
	}

	public IReadOnlyCollection<IEmploymentActiveTask> EvaluateGoal(IManagerGoal goal, IEmploymentTaskContext context,
		DateTimeOffset now)
	{
		if (goal is not ManagerGoal concrete || !ReferenceEquals(concrete.Employer, _host))
		{
			return [];
		}

		if (concrete.Status is not ManagerGoalStatus.Active and not ManagerGoalStatus.Satisfied)
		{
			return [];
		}

		if (concrete.LastEvaluatedAt.HasValue && now - concrete.LastEvaluatedAt.Value < concrete.EvaluationCadence)
		{
			_host.DebugEmployment($"Skipped manager goal #{concrete.Id:N0}: evaluation cadence has not elapsed.");
			return [];
		}

		if (TryGetFailedTaskForGoal(concrete, out var failedTask))
		{
			concrete.Fail(now,
				$"Spawned task {failedTask.Name} failed: {TerminalTaskDiagnostic(failedTask)}");
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
				concrete.LastEvaluationResult!, concrete.CorrelationId);
			_host.DebugEmployment($"Manager goal #{concrete.Id:N0} failed: {concrete.LastEvaluationResult}");
			return [];
		}

		if (HospitalSupplyStockGoalPlanner.IsHospitalStockGoal(concrete.GoalType))
		{
			return EvaluateNativeHospitalStockGoal(concrete, context, now);
		}

		if (!GoalConditionsSatisfied(concrete, context, now, out var conditionReason))
		{
			concrete.MarkSatisfied(now, $"Goal is satisfied: {conditionReason}");
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
				concrete.LastEvaluationResult!, concrete.CorrelationId);
			_host.DebugEmployment($"Manager goal #{concrete.Id:N0} is satisfied: {concrete.LastEvaluationResult}");
			return [];
		}

		if (concrete.Status == ManagerGoalStatus.Satisfied)
		{
			concrete.MarkActive(now, "Conditions now require work.");
		}

		if (concrete.Configuration.ActionPlan is null)
		{
			concrete.Fail(now, "Goal had no action plan to create.");
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
				concrete.LastEvaluationResult!, concrete.CorrelationId);
			_host.DebugEmployment($"Manager goal #{concrete.Id:N0} failed: {concrete.LastEvaluationResult}");
			return [];
		}

		if (!TryValidateGoalPolicy(concrete.Configuration.ActionPlan, concrete.Policy, out var policyReason))
		{
			concrete.Block(policyReason);
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
				concrete.LastEvaluationResult!, concrete.CorrelationId);
			_host.DebugEmployment($"Manager goal #{concrete.Id:N0} was blocked: {concrete.LastEvaluationResult}");
			return [];
		}

		if (concrete.Policy.RiskLimits.MaximumActiveTasks is { } maximumActiveTasks &&
		    ActiveTaskCountFor(concrete) >= maximumActiveTasks)
		{
			concrete.MarkEvaluated(now,
				$"Risk limit permits {maximumActiveTasks.ToString("N0", CultureInfo.InvariantCulture)} active task(s), and that limit is already reached.");
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
				concrete.LastEvaluationResult!, concrete.CorrelationId);
			_host.DebugEmployment($"Manager goal #{concrete.Id:N0} did not create work: {concrete.LastEvaluationResult}");
			return [];
		}

		var idempotencyKey = GoalIdempotencyKey(concrete);
		if (_host.TaskBoard.HasBlockingActiveTask(idempotencyKey))
		{
			concrete.MarkEvaluated(now,
				$"An active task for manager goal #{concrete.Id:N0} already exists.");
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
				concrete.LastEvaluationResult!, concrete.CorrelationId);
			_host.DebugEmployment($"Manager goal #{concrete.Id:N0} did not create work: {concrete.LastEvaluationResult}");
			return [];
		}

		var goalPrincipal = EmploymentPrincipal.ForManagerGoal(concrete);
		var task = _host.TaskBoard.CreateActiveTask(concrete.Configuration.Description, concrete.Configuration.ActionPlan, null,
			concrete.CorrelationId, idempotencyKey, concrete.Priority, provenance: EmploymentTaskBoard.CreateTaskProvenance(
				_host,
				concrete.Configuration.ActionPlan,
				concrete.CorrelationId,
				EmploymentTaskSourceKind.ManagerGoal,
				goalPrincipal,
				goalPrincipal,
				concrete.Priority,
				now,
				sourceGoalId: concrete.Id));
		concrete.MarkEvaluated(now, $"Created active task {task.Name}.");
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
			concrete.LastEvaluationResult!, concrete.CorrelationId);
		_host.DebugEmployment($"Manager goal #{concrete.Id:N0} created active task {task.Name}.");
		return [task];
	}

	private IReadOnlyCollection<IEmploymentActiveTask> EvaluateNativeHospitalStockGoal(ManagerGoal concrete,
		IEmploymentTaskContext context, DateTimeOffset now)
	{
		if (!GoalConditionsSatisfied(concrete, context, now, out var conditionReason))
		{
			if (HospitalSupplyStockGoalPlanner.IsConfigurationBlocker(conditionReason))
			{
				concrete.Block(conditionReason);
				_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
					concrete.LastEvaluationResult!, concrete.CorrelationId);
				_host.DebugEmployment($"Manager goal #{concrete.Id:N0} was blocked: {concrete.LastEvaluationResult}");
				return [];
			}

			concrete.MarkSatisfied(now, $"Goal is satisfied: {conditionReason}");
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
				concrete.LastEvaluationResult!, concrete.CorrelationId);
			_host.DebugEmployment($"Manager goal #{concrete.Id:N0} is satisfied: {concrete.LastEvaluationResult}");
			return [];
		}

		if (concrete.Status == ManagerGoalStatus.Satisfied)
		{
			concrete.MarkActive(now, "Conditions now require work.");
		}

		if (!HospitalSupplyStockGoalPlanner.TryBuildActionPlan(concrete, context, out var actionPlan, out var planReason) ||
		    actionPlan is null)
		{
			concrete.Block(planReason);
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
				concrete.LastEvaluationResult!, concrete.CorrelationId);
			_host.DebugEmployment($"Manager goal #{concrete.Id:N0} was blocked: {concrete.LastEvaluationResult}");
			return [];
		}

		if (!TryValidateGoalPolicy(actionPlan, concrete.Policy, out var policyReason))
		{
			concrete.Block(policyReason);
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
				concrete.LastEvaluationResult!, concrete.CorrelationId);
			_host.DebugEmployment($"Manager goal #{concrete.Id:N0} was blocked: {concrete.LastEvaluationResult}");
			return [];
		}

		if (concrete.Policy.RiskLimits.MaximumActiveTasks is { } maximumActiveTasks &&
		    ActiveTaskCountFor(concrete) >= maximumActiveTasks)
		{
			concrete.MarkEvaluated(now,
				$"Risk limit permits {maximumActiveTasks.ToString("N0", CultureInfo.InvariantCulture)} active task(s), and that limit is already reached.");
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
				concrete.LastEvaluationResult!, concrete.CorrelationId);
			_host.DebugEmployment($"Manager goal #{concrete.Id:N0} did not create work: {concrete.LastEvaluationResult}");
			return [];
		}

		var idempotencyKey = GoalIdempotencyKey(concrete);
		if (_host.TaskBoard.HasBlockingActiveTask(idempotencyKey))
		{
			concrete.MarkEvaluated(now,
				$"An active task for manager goal #{concrete.Id:N0} already exists.");
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
				concrete.LastEvaluationResult!, concrete.CorrelationId);
			_host.DebugEmployment($"Manager goal #{concrete.Id:N0} did not create work: {concrete.LastEvaluationResult}");
			return [];
		}

		var goalPrincipal = EmploymentPrincipal.ForManagerGoal(concrete);
		var task = _host.TaskBoard.CreateActiveTask(concrete.Configuration.Description, actionPlan, null,
			concrete.CorrelationId, idempotencyKey, concrete.Priority, provenance: EmploymentTaskBoard.CreateTaskProvenance(
				_host,
				actionPlan,
				concrete.CorrelationId,
				EmploymentTaskSourceKind.ManagerGoal,
				goalPrincipal,
				goalPrincipal,
				concrete.Priority,
				now,
				sourceGoalId: concrete.Id));
		concrete.MarkEvaluated(now, $"Created active task {task.Name}.");
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
			concrete.LastEvaluationResult!, concrete.CorrelationId);
		_host.DebugEmployment($"Manager goal #{concrete.Id:N0} created active task {task.Name}.");
		return [task];
	}

	private int ActiveTaskCountFor(IManagerGoal goal)
	{
		return _host.TaskBoard.ActiveTasks.Count(x =>
			x.SourceGoalId == goal.Id &&
			x.Status is EmploymentTaskStatus.Pending or EmploymentTaskStatus.Assigned or
				EmploymentTaskStatus.InProgress or EmploymentTaskStatus.Blocked);
	}

	private bool TryGetFailedTaskForGoal(IManagerGoal goal, out IEmploymentActiveTask failedTask)
	{
		failedTask = _host.TaskBoard.ActiveTasks
			.Where(x => x.SourceGoalId == goal.Id && x.Status == EmploymentTaskStatus.Failed)
			.OrderByDescending(x => x.CreatedAt)
			.FirstOrDefault()!;
		return failedTask is not null;
	}

	private static string TerminalTaskDiagnostic(IEmploymentActiveTask task)
	{
		if (!string.IsNullOrWhiteSpace(task.BlockedReason))
		{
			return task.BlockedReason;
		}

		return task.StepOperationalStates
			.Select(x => x.FailureDiagnostic)
			.LastOrDefault(x => !string.IsNullOrWhiteSpace(x)) ??
			"No failure diagnostic was recorded.";
	}

	private static bool GoalConditionsSatisfied(IManagerGoal goal, IEmploymentTaskContext context, DateTimeOffset now,
		out string reason)
	{
		var conditions = goal.Configuration.Conditions?.ToList() ?? [];
		var result = EmploymentConditionExpressionEvaluator.Evaluate(
			goal.Configuration.ConditionExpression,
			conditions,
			context,
			now);
		reason = result.Reason;
		return result.Satisfied;
	}

	internal static bool TryValidateGoalPolicy(EmploymentActionPlan? actionPlan, ManagerGoalPolicy policy,
		out string reason)
	{
		reason = string.Empty;
		if (actionPlan is null)
		{
			return true;
		}

		if (policy.RiskLimits.MaximumActionSteps is { } maximumActionSteps &&
		    actionPlan.Steps.Count > maximumActionSteps)
		{
			reason = $"Manager goal risk limit permits {maximumActionSteps.ToString("N0", CultureInfo.InvariantCulture)} action step(s), but the plan has {actionPlan.Steps.Count.ToString("N0", CultureInfo.InvariantCulture)}.";
			return false;
		}

		var totals = new Dictionary<long, (ICurrency Currency, decimal Amount)>();
		foreach (var step in actionPlan.Steps.Where(x => x.RequiresPaymentAuthorisation))
		{
			if (!EmploymentTaskDispatcher.TryGetFinancialStepAmount(step, out var amount))
			{
				if (policy.RiskLimits.AllowsUnboundedFinancialSteps)
				{
					continue;
				}

				reason = $"Manager goal risk limit does not allow unbounded financial step {step.StepType.DescribeEnum()}.";
				return false;
			}

			var existing = totals.GetValueOrDefault(amount.Currency.Id, (Currency: amount.Currency, Amount: 0.0M));
			totals[amount.Currency.Id] = (amount.Currency, existing.Amount + amount.Amount);
		}

		var budgets = policy.BudgetLimits
		                    .GroupBy(x => x.Currency.Id)
		                    .ToDictionary(x => x.Key, x => x.First());
		foreach (var (currencyId, total) in totals)
		{
			if (!budgets.TryGetValue(currencyId, out var budget) || total.Amount <= budget.Amount)
			{
				continue;
			}

			reason = $"Manager goal budget for {total.Currency.Name} is {budget.Amount.ToString("N2", CultureInfo.InvariantCulture)}, but the action plan requires {total.Amount.ToString("N2", CultureInfo.InvariantCulture)}.";
			return false;
		}

		return true;
	}

	private static string GoalIdempotencyKey(IManagerGoal goal)
	{
		return $"manager-goal:{goal.CorrelationId:D}";
	}
}

public sealed class ManagerGoal : IManagerGoal
{
	private readonly IEmploymentPersistenceStore? _persistence;

	public ManagerGoal(long id, IEmploymentHost employer, ManagerGoalType goalType,
		EmploymentAuthoritySet requiredAuthority, ManagerGoalStatus status, ManagerGoalConfiguration configuration,
		int priority, TimeSpan evaluationCadence, ManagerGoalPolicy? policy, Guid correlationId)
	{
		Id = id;
		Employer = employer;
		GoalType = goalType;
		RequiredAuthority = requiredAuthority;
		Status = status;
		Configuration = configuration;
		Priority = priority;
		EvaluationCadence = evaluationCadence;
		Policy = policy ?? ManagerGoalPolicy.Default;
		CorrelationId = correlationId;
	}

	internal ManagerGoal(long id, IEmploymentHost employer, ManagerGoalType goalType,
		EmploymentAuthoritySet requiredAuthority, ManagerGoalStatus status, ManagerGoalConfiguration configuration,
		int priority, TimeSpan evaluationCadence, ManagerGoalPolicy? policy, Guid correlationId, IEmploymentPersistenceStore? persistence)
		: this(id, employer, goalType, requiredAuthority, status, configuration, priority, evaluationCadence, policy, correlationId)
	{
		_persistence = persistence;
	}

	internal ManagerGoal(long id, IEmploymentHost employer, ManagerGoalType goalType,
		EmploymentAuthoritySet requiredAuthority, ManagerGoalStatus status, ManagerGoalConfiguration configuration,
		int priority, TimeSpan evaluationCadence, ManagerGoalPolicy? policy, DateTimeOffset? lastEvaluatedAt, string? lastEvaluationResult,
		Guid correlationId, IEmploymentPersistenceStore persistence)
		: this(id, employer, goalType, requiredAuthority, status, configuration, priority, evaluationCadence, policy, correlationId)
	{
		LastEvaluatedAt = lastEvaluatedAt;
		LastEvaluationResult = lastEvaluationResult;
		_persistence = persistence;
	}

	public long Id { get; }
	public IEmploymentHost Employer { get; }
	public ManagerGoalType GoalType { get; }
	public EmploymentAuthoritySet RequiredAuthority { get; }
	public ManagerGoalStatus Status { get; private set; }
	public ManagerGoalConfiguration Configuration { get; }
	public int Priority { get; }
	public TimeSpan EvaluationCadence { get; }
	public ManagerGoalPolicy Policy { get; }
	public DateTimeOffset? LastEvaluatedAt { get; private set; }
	public string? LastEvaluationResult { get; private set; }
	public Guid CorrelationId { get; }

	public void MarkEvaluated(DateTimeOffset now, string result)
	{
		LastEvaluatedAt = now;
		LastEvaluationResult = result;
		_persistence?.SaveManagerGoalState(this);
	}

	public void MarkActive(DateTimeOffset now, string result)
	{
		Status = ManagerGoalStatus.Active;
		LastEvaluatedAt = now;
		LastEvaluationResult = result;
		_persistence?.SaveManagerGoalState(this);
	}

	public void MarkSatisfied(DateTimeOffset now, string result)
	{
		Status = ManagerGoalStatus.Satisfied;
		LastEvaluatedAt = now;
		LastEvaluationResult = result;
		_persistence?.SaveManagerGoalState(this);
	}

	public void Fail(DateTimeOffset now, string reason)
	{
		Status = ManagerGoalStatus.Failed;
		LastEvaluatedAt = now;
		LastEvaluationResult = reason;
		_persistence?.SaveManagerGoalState(this);
	}

	public void Cancel(string reason)
	{
		Status = ManagerGoalStatus.Cancelled;
		LastEvaluationResult = reason;
		_persistence?.SaveManagerGoalState(this);
	}

	public void Block(string reason)
	{
		Status = ManagerGoalStatus.Blocked;
		LastEvaluatedAt = EmploymentClock.CurrentInstant(Employer);
		LastEvaluationResult = reason;
		_persistence?.SaveManagerGoalState(this);
	}

	internal void BlockForCompatibilityLoad(DateTimeOffset now, string reason)
	{
		Status = ManagerGoalStatus.Blocked;
		LastEvaluatedAt = now;
		LastEvaluationResult = reason;
	}
}

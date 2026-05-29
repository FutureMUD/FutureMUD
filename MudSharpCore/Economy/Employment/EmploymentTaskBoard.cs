using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

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
	private readonly Dictionary<long, List<IGameItem>> _containerContents = new();
	private readonly Dictionary<long, HashSet<long>> _loadedTaskItemIds = new();
	private readonly Dictionary<long, HashSet<string>> _itemTags = new();
	private readonly List<CommodityProfile> _commodityProfiles = new();
	private readonly HashSet<long> _transportBundleIds = new();
	private readonly bool _usePhysicalItemMovement;

	public EmploymentTaskContext(IEmploymentHost employer, bool usePhysicalItemMovement = false)
	{
		Employer = employer;
		_usePhysicalItemMovement = usePhysicalItemMovement;
	}

	public IEmploymentHost Employer { get; }

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

	public void SetAvailableItems(ICell location, IEnumerable<IGameItem> items)
	{
		_locationItems[location.Id] = items.ToList();
		_configuredLocationItems.Add(location.Id);
	}

	public IReadOnlyCollection<IGameItem> AvailableItems(ICell location)
	{
		if (_usePhysicalItemMovement && !_configuredLocationItems.Contains(location.Id))
		{
			return location.GameItems
			               .SelectMany(x => x.DeepItems)
			               .DistinctBy(x => x.Id)
			               .ToList();
		}

		return LocationItems(location);
	}

	public IReadOnlyCollection<IGameItem> CarriedTaskItems(ICharacter actor)
	{
		return _carriedTaskItems.TryGetValue(actor.Id, out var items) ? items : [];
	}

	public IReadOnlyCollection<IGameItem> ContainedItems(IGameItem container)
	{
		return _containerContents.TryGetValue(container.Id, out var items) ? items : [];
	}

	public IReadOnlyCollection<IGameItem> LoadedTaskItems(ICharacter actor, IGameItem container)
	{
		return ResolveLoadedTaskItems(actor, container).DistinctBy(x => x.Id).ToList();
	}

	public void HydrateTaskState(IEmploymentActiveTask task, int currentStepIndex)
	{
		foreach (var state in task.StepOperationalStates.Take(Math.Max(0, currentStepIndex)))
		{
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
				AddCarriedTaskItems(task.AssignedEmployee, items);
			}
		}
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

		if (!_usePhysicalItemMovement ||
		    resolvedItems.Any(x => _configuredLocationItems.Contains(x.Source.Id)) ||
		    !CanUseInventoryPlan(actor))
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

			AddCarriedTaskItems(actor, resolvedItems.Select(x => x.Item));
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
		if (!CanPath(actor, destination))
		{
			reason = "The assigned employee cannot path to the delivery destination.";
			return false;
		}

		if (!_carriedTaskItems.TryGetValue(actor.Id, out var carried) || carried.Count == 0)
		{
			reason = "The assigned employee is not carrying any task items to deliver.";
			return false;
		}

		if (_usePhysicalItemMovement && !_configuredLocationItems.Contains(destination.Id) && CanUseInventoryPlan(actor))
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

		if (_usePhysicalItemMovement && !_configuredLocationItems.Contains(destination.Id) && CanUseInventoryPlan(actor))
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

		carried.Clear();
		reason = string.Empty;
		return true;
	}

	public bool TryLoadCarriedTaskItems(ICharacter actor, IGameItem targetContainer, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
		if (!_carriedTaskItems.TryGetValue(actor.Id, out var carried) || carried.Count == 0)
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

		if (_usePhysicalItemMovement && EmploymentInventoryPlanLogistics.CanUseInventoryPlan(actor))
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
				LoadedAssets: FormatLoadedAssets("load", targetContainer.Id, loadedItems));
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
		carried.Clear();
		operationalState = new EmploymentActionStepOperationalState(
			LoadedAssets: FormatLoadedAssets("load", targetContainer.Id, loaded));
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
				LoadedAssets: FormatLoadedAssets("unload", sourceContainer.Id, unloadedItems));
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
			LoadedAssets: FormatLoadedAssets("unload", sourceContainer.Id, loadedItems));
		reason = string.Empty;
		return true;
	}

	public bool TryReturnContainer(ICharacter actor, IGameItem container, ICell destination, IGameItem? destinationContainer,
		string? destinationContainerTag, out string reason,
		out EmploymentActionStepOperationalState operationalState)
	{
		operationalState = EmploymentActionStepOperationalState.Empty;
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

		if (!_carriedTaskItems.TryGetValue(actor.Id, out var carried) || carried.All(x => x.Id != container.Id))
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

		carried.Clear();
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

	private void AddCarriedTaskItems(ICharacter actor, IEnumerable<IGameItem> items)
	{
		if (!_carriedTaskItems.TryGetValue(actor.Id, out var carried))
		{
			carried = new List<IGameItem>();
			_carriedTaskItems[actor.Id] = carried;
		}

		foreach (var item in items)
		{
			if (carried.All(x => x.Id != item.Id))
			{
				carried.Add(item);
			}
		}
	}

	private void RemoveCarriedTaskItems(ICharacter actor, IEnumerable<IGameItem> items)
	{
		if (!_carriedTaskItems.TryGetValue(actor.Id, out var carried))
		{
			return;
		}

		var itemIds = items.Select(x => x.Id).ToHashSet();
		carried.RemoveAll(x => itemIds.Contains(x.Id));
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

	private static bool TryParseLoadedAssets(string? text, out string operation, out long containerId, out long[] itemIds)
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
		return item.InInventoryOf == actor.Body || actor.Inventory.Any(x => x.Id == item.Id);
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

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		var current = now.TimeOfDay;
		var satisfied = current >= EarliestTime && current <= LatestTime;
		reason = satisfied ? string.Empty : $"Current time {current} is outside the task window.";
		return satisfied;
	}
}

public sealed record StockThresholdCondition(string StockKey, int Threshold, bool BelowThreshold) : IEmploymentTaskCondition
{
	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.StockThreshold;

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		var current = context.StockLevel(StockKey);
		var satisfied = BelowThreshold ? current < Threshold : current >= Threshold;
		reason = satisfied ? string.Empty : $"Stock {StockKey} is {current}, which does not satisfy threshold {Threshold}.";
		return satisfied;
	}
}

public sealed record AccountBalanceCondition(string AccountKey, decimal Threshold, bool BelowThreshold) : IEmploymentTaskCondition
{
	public EmploymentTaskConditionType ConditionType => EmploymentTaskConditionType.AccountBalance;

	public bool IsSatisfied(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		var current = context.AccountBalance(AccountKey);
		var satisfied = BelowThreshold ? current < Threshold : current >= Threshold;
		reason = satisfied
			? string.Empty
			: $"Account {AccountKey} balance {current:N2} does not satisfy threshold {Threshold:N2}.";
		return satisfied;
	}
}

public sealed class EmploymentTaskBoard : IEmploymentTaskBoard
{
	private readonly IEmploymentHost _host;
	private readonly List<IEmploymentScheduledTaskRule> _scheduledRules = new();
	private readonly List<IEmploymentActiveTask> _activeTasks = new();
	private readonly IEmploymentPersistenceStore? _persistence;

	public EmploymentTaskBoard(IEmploymentHost host)
	{
		_host = host;
	}

	internal EmploymentTaskBoard(IEmploymentHost host, IEmploymentPersistenceStore persistence,
		IEnumerable<IEmploymentScheduledTaskRule> scheduledRules, IEnumerable<IEmploymentActiveTask> activeTasks)
	{
		_host = host;
		_persistence = persistence;
		_scheduledRules.AddRange(scheduledRules);
		_activeTasks.AddRange(activeTasks);
	}

	public IReadOnlyCollection<IEmploymentScheduledTaskRule> ScheduledRules => _scheduledRules;
	public IReadOnlyCollection<IEmploymentActiveTask> ActiveTasks => _activeTasks;

	public IEmploymentScheduledTaskRule CreateScheduledRule(string name, string idempotencyKey,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentActionPlan actionPlan, TimeSpan cooldown,
		ICharacter? authorisedBy)
	{
		if (authorisedBy is not null && !_host.HasAuthority(authorisedBy, EmploymentAuthority.CreateScheduledRules))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to create scheduled task rules for {_host.EmploymentHostName}.");
		}

		if (authorisedBy is not null && actionPlan.RequiredAuthority.Authorities != EmploymentAuthority.None &&
		    !_host.HasAuthority(authorisedBy, actionPlan.RequiredAuthority.Authorities))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to create scheduled task rules with {actionPlan.RequiredAuthority.Authorities.DescribeEnum()} authority for {_host.EmploymentHostName}.");
		}

		var rule = new EmploymentScheduledTaskRule(_host, name, idempotencyKey, conditions, actionPlan, cooldown);
		_scheduledRules.Add(rule);
		_persistence?.SaveScheduledRule(rule);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ScheduledRuleCreated, authorisedBy,
			$"Created scheduled task rule {name}.");
		_host.DebugEmployment($"Created scheduled task rule {name}.");
		return rule;
	}

	public IEmploymentActiveTask CreateActiveTask(string name, EmploymentActionPlan actionPlan, ICharacter? authorisedBy,
		Guid? correlationId = null)
	{
		if (authorisedBy is not null && !_host.HasAuthority(authorisedBy, EmploymentAuthority.AssignTasks))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to create tasks for {_host.EmploymentHostName}.");
		}

		if (authorisedBy is not null && actionPlan.RequiredAuthority.Authorities != EmploymentAuthority.None &&
		    !_host.HasAuthority(authorisedBy, actionPlan.RequiredAuthority.Authorities))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to create tasks with {actionPlan.RequiredAuthority.Authorities.DescribeEnum()} authority for {_host.EmploymentHostName}.");
		}

		var task = new EmploymentActiveTask(_host, name, actionPlan, correlationId ?? Guid.NewGuid(), _persistence);
		_activeTasks.Add(task);
		_persistence?.SaveActiveTask(task);
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskCreated, authorisedBy,
			$"Created active task {name}.", task.CorrelationId);
		_host.DebugEmployment($"Created active task {name} ({task.CorrelationId}).", authorisedBy?.Gameworld);
		return task;
	}

	public bool CancelActiveTask(IEmploymentActiveTask task, ICharacter? cancelledBy, string reason)
	{
		if (cancelledBy is not null && !_host.HasAuthority(cancelledBy, EmploymentAuthority.CancelTasks))
		{
			throw new InvalidOperationException($"{cancelledBy.HowSeen(cancelledBy, colour: false)} is not authorised to cancel tasks for {_host.EmploymentHostName}.");
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
		_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskCancelled, cancelledBy,
			$"Cancelled active task {task.Name}: {reason}", task.CorrelationId);
		_host.DebugEmployment($"Cancelled active task {task.Name}: {reason}", cancelledBy?.Gameworld);
		return true;
	}

	public IReadOnlyCollection<IEmploymentActiveTask> EvaluateScheduledRules(IEmploymentTaskContext context, DateTimeOffset now)
	{
		var spawned = new List<IEmploymentActiveTask>();
		foreach (var rule in _scheduledRules.OfType<EmploymentScheduledTaskRule>())
		{
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ScheduledRuleEvaluated, null,
				$"Evaluated scheduled task rule {rule.Name}.");
			_host.DebugEmployment($"Evaluating scheduled task rule {rule.Name}.");
			if (!rule.CanSpawn(context, now, out var spawnReason))
			{
				_host.DebugEmployment($"Scheduled task rule {rule.Name} did not spawn: {spawnReason}");
				continue;
			}

			if (_activeTasks.OfType<EmploymentActiveTask>().Any(x =>
				    x.IdempotencyKey.Equals(rule.IdempotencyKey, StringComparison.InvariantCultureIgnoreCase) &&
				    x.Status is EmploymentTaskStatus.Pending or EmploymentTaskStatus.Assigned or EmploymentTaskStatus.InProgress or EmploymentTaskStatus.Blocked))
			{
				_host.DebugEmployment(
					$"Scheduled task rule {rule.Name} did not spawn because an active task with idempotency key {rule.IdempotencyKey} already exists.");
				continue;
			}

			var task = new EmploymentActiveTask(_host, rule.Name, rule.ActionPlan, Guid.NewGuid(), _persistence)
			{
				IdempotencyKey = rule.IdempotencyKey
			};
			_activeTasks.Add(task);
			_persistence?.SaveActiveTask(task);
			rule.MarkSpawned(now);
			_persistence?.SaveScheduledRuleState(rule);
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskCreated, null,
				$"Spawned active task {rule.Name} from scheduled rule.", task.CorrelationId);
			_host.DebugEmployment($"Scheduled task rule {rule.Name} spawned active task {task.CorrelationId}.");
			spawned.Add(task);
		}

		return spawned;
	}
}

public sealed class EmploymentScheduledTaskRule : IEmploymentScheduledTaskRule
{
	private readonly List<IEmploymentTaskCondition> _conditions;

	public EmploymentScheduledTaskRule(IEmploymentHost employer, string name, string idempotencyKey,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentActionPlan actionPlan, TimeSpan cooldown)
	{
		Id = Guid.NewGuid();
		Employer = employer;
		Name = name;
		IdempotencyKey = string.IsNullOrWhiteSpace(idempotencyKey) ? Id.ToString("N") : idempotencyKey.Trim();
		_conditions = conditions.ToList();
		ActionPlan = actionPlan;
		Cooldown = cooldown;
	}

	internal EmploymentScheduledTaskRule(Guid id, IEmploymentHost employer, string name, string idempotencyKey,
		IEnumerable<IEmploymentTaskCondition> conditions, EmploymentActionPlan actionPlan, TimeSpan cooldown,
		DateTimeOffset? lastSpawnedAt)
	{
		Id = id;
		Employer = employer;
		Name = name;
		IdempotencyKey = string.IsNullOrWhiteSpace(idempotencyKey) ? Id.ToString("N") : idempotencyKey.Trim();
		_conditions = conditions.ToList();
		ActionPlan = actionPlan;
		Cooldown = cooldown;
		LastSpawnedAt = lastSpawnedAt;
	}

	public Guid Id { get; }
	public IEmploymentHost Employer { get; }
	public string Name { get; }
	public string IdempotencyKey { get; }
	public IReadOnlyCollection<IEmploymentTaskCondition> Conditions => _conditions;
	public EmploymentActionPlan ActionPlan { get; }
	public TimeSpan Cooldown { get; }
	public DateTimeOffset? LastSpawnedAt { get; private set; }

	public bool CanSpawn(IEmploymentTaskContext context, DateTimeOffset now, out string reason)
	{
		if (LastSpawnedAt.HasValue && now - LastSpawnedAt.Value < Cooldown)
		{
			reason = "Rule is still inside its cooldown window.";
			return false;
		}

		foreach (var condition in _conditions)
		{
			if (!condition.IsSatisfied(context, now, out reason))
			{
				return false;
			}
		}

		reason = string.Empty;
		return true;
	}

	public void MarkSpawned(DateTimeOffset now)
	{
		LastSpawnedAt = now;
	}
}

public sealed class EmploymentActiveTask : IEmploymentActiveTask
{
	private readonly List<EmploymentActionStepStatus> _stepStates;
	private readonly List<EmploymentActionStepOperationalState> _stepOperationalStates;
	private readonly IEmploymentPersistenceStore? _persistence;

	public EmploymentActiveTask(IEmploymentHost employer, string name, EmploymentActionPlan actionPlan, Guid correlationId)
	{
		Id = Guid.NewGuid();
		Employer = employer;
		Name = name;
		ActionPlan = actionPlan;
		CorrelationId = correlationId;
		_stepStates = actionPlan.Steps.Select(_ => EmploymentActionStepStatus.Pending).ToList();
		_stepOperationalStates = actionPlan.Steps.Select(_ => EmploymentActionStepOperationalState.Empty).ToList();
	}

	internal EmploymentActiveTask(IEmploymentHost employer, string name, EmploymentActionPlan actionPlan,
		Guid correlationId, IEmploymentPersistenceStore? persistence)
		: this(employer, name, actionPlan, correlationId)
	{
		_persistence = persistence;
	}

	internal EmploymentActiveTask(Guid id, IEmploymentHost employer, string name, EmploymentActionPlan actionPlan,
		EmploymentTaskStatus status, ICharacter? assignedEmployee, string? blockedReason,
		IEnumerable<EmploymentActionStepStatus> stepStates,
		IEnumerable<EmploymentActionStepOperationalState> stepOperationalStates,
		Guid correlationId, string idempotencyKey,
		IEmploymentPersistenceStore persistence)
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

	public int NextStepIndex => _stepStates.FindIndex(x => x is EmploymentActionStepStatus.Pending or EmploymentActionStepStatus.InProgress or EmploymentActionStepStatus.Blocked);

	public void Assign(ICharacter employee)
	{
		AssignedEmployee = employee;
		Status = EmploymentTaskStatus.Assigned;
		BlockedReason = null;
		_persistence?.SaveActiveTaskState(this);
	}

	public void Block(string reason)
	{
		Status = EmploymentTaskStatus.Blocked;
		BlockedReason = reason;
		_persistence?.SaveActiveTaskState(this);
	}

	public void Cancel(string reason)
	{
		Status = EmploymentTaskStatus.Cancelled;
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

		if (_stepStates.All(x => x == EmploymentActionStepStatus.Completed))
		{
			Status = EmploymentTaskStatus.Completed;
			BlockedReason = null;
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

			var missingCapabilities = task.ActionPlan.RequiredCapabilities
			                          .Where(x => !candidate.Capabilities.Contains(x))
			                          .ToList();
			if (missingCapabilities.Any())
			{
				rejectionReasons.Add(
					$"{candidate.Candidate.Name}: missing {missingCapabilities.Select(x => x.DescribeEnum()).ListToString()} capability");
				context.Employer.DebugEmployment(
					$"Skipped {candidate.Candidate.Name} for task {task.Name}: missing required AI capabilities.");
				continue;
			}

			var nextStepIndex = concrete.NextStepIndex;
			if (nextStepIndex < 0)
			{
				rejectionReasons.Add($"{candidate.Candidate.Name}: task has no pending steps");
				context.Employer.DebugEmployment($"Skipped task {task.Name}: it has no pending steps.");
				continue;
			}

			context.HydrateTaskState(task, nextStepIndex);
			if (!task.ActionPlan.Steps[nextStepIndex].CanExecute(context, candidate.Candidate, out var stepReason))
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
		if (concrete.Status == EmploymentTaskStatus.Completed)
		{
			context.RecordRegister(EmploymentRegisterEntryType.ActiveTaskCompleted, task.AssignedEmployee,
				$"Completed active task {task.Name}.", task.CorrelationId);
			context.Employer.DebugEmployment($"Completed active task {task.Name}.", task.AssignedEmployee.Gameworld);
		}

		return result;
	}

	private static void ApplyDurablePaymentAuthorisations(EmploymentActiveTask task, IEmploymentTaskContext context,
		int currentStepIndex)
	{
		var hasCompletedAuthorisation = task.ActionPlan.Steps
		                                .Take(currentStepIndex)
		                                .Select((step, index) => (Step: step, Index: index))
		                                .Any(x =>
			                                task.StepStates[x.Index] == EmploymentActionStepStatus.Completed &&
			                                x.Step is CataloguedActionShellStep { ActionKey: "authorise" });
		if (!hasCompletedAuthorisation)
		{
			return;
		}

		foreach (var step in task.ActionPlan.Steps.Skip(currentStepIndex).Where(x => x.RequiresPaymentAuthorisation))
		{
			context.AuthorisePaymentFor(step, task.AssignedEmployee, task.CorrelationId, recordRegister: false);
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
		if (!_host.HasAuthority(authorisedBy, EmploymentAuthority.CreateManagerGoals))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to create manager goals for {_host.EmploymentHostName}.");
		}

		if (!authorisedBy.IsAdministrator() &&
		    !_host.EmploymentContracts.Where(x => x.Employee.Id == authorisedBy.Id && x.Status == EmploymentStatus.Active)
		          .Any(x => x.Authority.ContainsAll(definition.RequiredAuthority)))
		{
			throw new InvalidOperationException("A manager cannot create a goal that requires authority they do not possess.");
		}

		var goal = new ManagerGoal(
			Interlocked.Increment(ref _nextId),
			_host,
			definition.GoalType,
			definition.RequiredAuthority,
			ManagerGoalStatus.Active,
			definition.Configuration,
			definition.Priority,
			definition.EvaluationCadence,
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

	public IReadOnlyCollection<IEmploymentActiveTask> EvaluateGoals(IEmploymentTaskContext context, DateTimeOffset now)
	{
		var tasks = new List<IEmploymentActiveTask>();
		foreach (var goal in _goals.OfType<ManagerGoal>().Where(x => x.Status == ManagerGoalStatus.Active))
		{
			if (goal.LastEvaluatedAt.HasValue && now - goal.LastEvaluatedAt.Value < goal.EvaluationCadence)
			{
				_host.DebugEmployment($"Skipped manager goal #{goal.Id:N0}: evaluation cadence has not elapsed.");
				continue;
			}

			if (goal.Configuration.Conditions?.All(x => x.IsSatisfied(context, now, out _)) == false)
			{
				goal.MarkEvaluated(now, "Conditions were not satisfied.");
				_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
					goal.LastEvaluationResult!, goal.CorrelationId);
				_host.DebugEmployment($"Manager goal #{goal.Id:N0} did not create work: {goal.LastEvaluationResult}");
				continue;
			}

			if (goal.Configuration.ActionPlan is null)
			{
				goal.MarkEvaluated(now, "Goal had no action plan to create.");
				_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
					goal.LastEvaluationResult!, goal.CorrelationId);
				_host.DebugEmployment($"Manager goal #{goal.Id:N0} did not create work: {goal.LastEvaluationResult}");
				continue;
			}

			var task = _host.TaskBoard.CreateActiveTask(goal.Configuration.Description, goal.Configuration.ActionPlan, null,
				goal.CorrelationId);
			tasks.Add(task);
			goal.MarkEvaluated(now, $"Created active task {task.Name}.");
			_host.EmploymentRegister.Record(EmploymentRegisterEntryType.ManagerGoalEvaluated, null,
				goal.LastEvaluationResult!, goal.CorrelationId);
			_host.DebugEmployment($"Manager goal #{goal.Id:N0} created active task {task.Name}.");
		}

		return tasks;
	}
}

public sealed class ManagerGoal : IManagerGoal
{
	private readonly IEmploymentPersistenceStore? _persistence;

	public ManagerGoal(long id, IEmploymentHost employer, ManagerGoalType goalType,
		EmploymentAuthoritySet requiredAuthority, ManagerGoalStatus status, ManagerGoalConfiguration configuration,
		int priority, TimeSpan evaluationCadence, Guid correlationId)
	{
		Id = id;
		Employer = employer;
		GoalType = goalType;
		RequiredAuthority = requiredAuthority;
		Status = status;
		Configuration = configuration;
		Priority = priority;
		EvaluationCadence = evaluationCadence;
		CorrelationId = correlationId;
	}

	internal ManagerGoal(long id, IEmploymentHost employer, ManagerGoalType goalType,
		EmploymentAuthoritySet requiredAuthority, ManagerGoalStatus status, ManagerGoalConfiguration configuration,
		int priority, TimeSpan evaluationCadence, Guid correlationId, IEmploymentPersistenceStore? persistence)
		: this(id, employer, goalType, requiredAuthority, status, configuration, priority, evaluationCadence, correlationId)
	{
		_persistence = persistence;
	}

	internal ManagerGoal(long id, IEmploymentHost employer, ManagerGoalType goalType,
		EmploymentAuthoritySet requiredAuthority, ManagerGoalStatus status, ManagerGoalConfiguration configuration,
		int priority, TimeSpan evaluationCadence, DateTimeOffset? lastEvaluatedAt, string? lastEvaluationResult,
		Guid correlationId, IEmploymentPersistenceStore persistence)
		: this(id, employer, goalType, requiredAuthority, status, configuration, priority, evaluationCadence, correlationId)
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
	public DateTimeOffset? LastEvaluatedAt { get; private set; }
	public string? LastEvaluationResult { get; private set; }
	public Guid CorrelationId { get; }

	public void MarkEvaluated(DateTimeOffset now, string result)
	{
		LastEvaluatedAt = now;
		LastEvaluationResult = result;
		_persistence?.SaveManagerGoalState(this);
	}

	public void Cancel(string reason)
	{
		Status = ManagerGoalStatus.Cancelled;
		LastEvaluationResult = reason;
		_persistence?.SaveManagerGoalState(this);
	}
}

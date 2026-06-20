using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.GameItems;

#nullable enable

namespace MudSharp.Economy.Employment;

public sealed class ShopEmploymentTaskService
{
	public EmploymentTaskContext CreatePhysicalContext(IPermanentShop shop)
	{
		return new EmploymentTaskContext(shop, usePhysicalItemMovement: true);
	}
	public EmploymentTaskContext CreatePhysicalTransferContext(IPermanentShop sourceShop, IPermanentShop targetShop)
	{
		return new EmploymentTaskContext(sourceShop, usePhysicalItemMovement: true, targetShop.AllShopCells);
	}

	public bool TryCreateStockTransferTask(ICharacter authorisedBy, IPermanentShop sourceShop,
		IMerchandise sourceMerchandise, int itemCount, IPermanentShop targetShop, IMerchandise targetMerchandise,
		out IEmploymentActiveTask? task, out string message, ICell? destination = null, IGameItem? container = null,
		string? containerTag = null)
	{
		task = null;
		if (authorisedBy is null)
		{
			message = "A stock transfer task must have an authorising manager.";
			return false;
		}

		if (!sourceShop.HasAuthority(authorisedBy, EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes))
		{
			message = $"You do not have delegated authority to create stock transfer tasks for {sourceShop.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (sourceShop.Id != targetShop.Id && !targetShop.HasAuthority(authorisedBy, EmploymentAuthority.ManageDeliveryRoutes))
		{
			message = $"You do not have delegated authority to deliver stock into {targetShop.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (itemCount <= 0)
		{
			message = "Stock transfer tasks must move at least one item.";
			return false;
		}

		if (!sourceShop.Merchandises.Any(x => x.Id == sourceMerchandise.Id))
		{
			message = $"{sourceMerchandise.Name.ColourName()} is not merchandise for {sourceShop.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (!targetShop.Merchandises.Any(x => x.Id == targetMerchandise.Id))
		{
			message = $"{targetMerchandise.Name.ColourName()} is not merchandise for {targetShop.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (sourceShop.StockroomCell is null)
		{
			message = $"{sourceShop.EmploymentHostName.ColourName()} does not have a stockroom.";
			return false;
		}

		var resolvedDestination = ResolveTransferDestination(targetShop, destination, container);
		if (resolvedDestination is null)
		{
			message = $"{targetShop.EmploymentHostName.ColourName()} does not have a stock-transfer destination.";
			return false;
		}

		if (!targetShop.AllShopCells.Any(x => x.Id == resolvedDestination.Id))
		{
			message = "Stock transfer tasks must deliver to one of the target shop's locations.";
			return false;
		}

		if (container is not null && !ContainerIsAtDestination(container, resolvedDestination))
		{
			message = $"{container.Name.ColourName()} is not at the selected stock-transfer destination.";
			return false;
		}

		if (!string.IsNullOrWhiteSpace(containerTag) && !DestinationHasContainerTag(targetShop, resolvedDestination, containerTag))
		{
			message = $"No display container tagged {containerTag.ColourCommand()} is available at the selected stock-transfer destination.";
			return false;
		}

		var matchingItems = StockroomItemsFor(sourceShop, sourceMerchandise)
		                    .Take(itemCount)
		                    .ToList();
		if (matchingItems.Count < itemCount)
		{
			message = $"{sourceShop.EmploymentHostName.ColourName()} only has {matchingItems.Count.ToString("N0", authorisedBy).ColourValue()} matching stockroom item{(matchingItems.Count == 1 ? string.Empty : "s")} to transfer.";
			return false;
		}

		var incompatibleItem = matchingItems.FirstOrDefault(x => !targetMerchandise.IsMerchandiseFor(x, true));
		if (incompatibleItem is not null)
		{
			message = $"{incompatibleItem.HowSeen(authorisedBy, colour: false).ColourName()} is not valid stock for target merchandise {targetMerchandise.Name.ColourName()}.";
			return false;
		}

		var itemPrototypeIds = matchingItems
		                       .Select(x => x.Prototype.Id)
		                       .Distinct()
		                       .ToList();
		var plan = new EmploymentActionPlan([
			new GetItemsByIdActionStep(itemCount, itemPrototypeIds, [sourceShop.StockroomCell]),
			new ShopStockTransferActionStep(sourceShop, targetShop, targetMerchandise, resolvedDestination, container,
				containerTag)
		]);

		try
		{
			task = sourceShop.TaskBoard.CreateActiveTask($"transfer {sourceMerchandise.Name} to {targetShop.Name}", plan,
				authorisedBy);
			if (sourceShop.Id != targetShop.Id)
			{
				targetShop.EmploymentRegister.Record(EmploymentRegisterEntryType.ActiveTaskCreated, authorisedBy,
					$"Incoming stock transfer from {sourceShop.Name}: {itemCount.ToString("N0", authorisedBy)} {sourceMerchandise.Name} item{(itemCount == 1 ? string.Empty : "s")} as {targetMerchandise.Name}.",
					task.CorrelationId);
			}

			message = $"You create a stock transfer task from {sourceShop.EmploymentHostName.ColourName()} to {targetShop.EmploymentHostName.ColourName()} for {itemCount.ToString("N0", authorisedBy).ColourValue()} {sourceMerchandise.Name.ColourName()} item{(itemCount == 1 ? string.Empty : "s")}.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public bool TryCreateStockroomRestockTask(ICharacter authorisedBy, IPermanentShop shop, IMerchandise merchandise,
		int itemCount, out IEmploymentActiveTask? task, out string message, ICell? destination = null,
		IGameItem? container = null, string? containerTag = null)
	{
		task = null;
		if (authorisedBy is null)
		{
			message = "A stockroom restock task must have an authorising manager.";
			return false;
		}

		if (!shop.HasAuthority(authorisedBy, EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes))
		{
			message = $"You do not have delegated authority to create stockroom restock tasks for {shop.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (itemCount <= 0)
		{
			message = "Stockroom restock tasks must move at least one item.";
			return false;
		}

		if (!shop.Merchandises.Any(x => x.Id == merchandise.Id))
		{
			message = $"{merchandise.Name.ColourName()} is not merchandise for {shop.EmploymentHostName.ColourName()}.";
			return false;
		}

		if (shop.StockroomCell is null)
		{
			message = $"{shop.EmploymentHostName.ColourName()} does not have a stockroom.";
			return false;
		}

		var resolvedDestination = ResolveDestination(shop, destination, container);
		if (resolvedDestination is null)
		{
			message = $"{shop.EmploymentHostName.ColourName()} does not have a shopfront destination for this restock task.";
			return false;
		}

		if (!shop.ShopfrontCells.Any(x => x.Id == resolvedDestination.Id))
		{
			message = "Stockroom restock tasks must deliver to one of the shop's shopfront locations.";
			return false;
		}

		if (container is not null && !ContainerIsAtDestination(container, resolvedDestination))
		{
			message = $"{container.Name.ColourName()} is not at the selected shopfront destination.";
			return false;
		}

		if (!string.IsNullOrWhiteSpace(containerTag) && !DestinationHasContainerTag(shop, resolvedDestination, containerTag))
		{
			message = $"No display container tagged {containerTag.ColourCommand()} is available at the selected shopfront destination.";
			return false;
		}

		var matchingItems = StockroomItemsFor(shop, merchandise)
		                    .Take(itemCount)
		                    .ToList();
		if (matchingItems.Count < itemCount)
		{
			message = $"{shop.EmploymentHostName.ColourName()} only has {matchingItems.Count.ToString("N0", authorisedBy).ColourValue()} matching stockroom item{(matchingItems.Count == 1 ? string.Empty : "s")} to move.";
			return false;
		}

		var itemPrototypeIds = matchingItems
		                       .Select(x => x.Prototype.Id)
		                       .Distinct()
		                       .ToList();
		var plan = new EmploymentActionPlan([
			new GetItemsByIdActionStep(itemCount, itemPrototypeIds, [shop.StockroomCell]),
			new DeliverItemsActionStep(resolvedDestination, container, containerTag)
		]);

		try
		{
			task = shop.TaskBoard.CreateActiveTask($"restock {merchandise.Name}", plan, authorisedBy);
			message = $"You create a stockroom restock task for {shop.EmploymentHostName.ColourName()} to move {itemCount.ToString("N0", authorisedBy).ColourValue()} {merchandise.Name.ColourName()} item{(itemCount == 1 ? string.Empty : "s")}.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	private static ICell? ResolveTransferDestination(IPermanentShop targetShop, ICell? destination, IGameItem? container)
	{
		if (destination is not null)
		{
			return destination;
		}

		var containerLocation = container?.TrueLocations.FirstOrDefault(x => targetShop.AllShopCells.Any(y => y.Id == x.Id));
		return containerLocation ?? targetShop.StockroomCell ?? targetShop.ShopfrontCells.FirstOrDefault();
	}
	private static ICell? ResolveDestination(IPermanentShop shop, ICell? destination, IGameItem? container)
	{
		if (destination is not null)
		{
			return destination;
		}

		var containerLocation = container?.TrueLocations.FirstOrDefault(x => shop.ShopfrontCells.Any(y => y.Id == x.Id));
		return containerLocation ?? shop.ShopfrontCells.FirstOrDefault();
	}

	private static bool ContainerIsAtDestination(IGameItem container, ICell destination)
	{
		return container.TrueLocations.Any(x => x.Id == destination.Id);
	}

	private static bool DestinationHasContainerTag(IPermanentShop shop, ICell destination, string containerTag)
	{
		return shop.DisplayContainers.Any(x => ContainerIsAtDestination(x, destination) && ItemMatchesTag(x, containerTag));
	}

	private static bool ItemMatchesTag(IGameItem item, string tagName)
	{
		return item.Tags.Any(x =>
			x.Name.EqualTo(tagName) ||
			x.FullName.EqualTo(tagName) ||
			x.Id.ToString("F0").EqualTo(tagName));
	}

	private static IEnumerable<IGameItem> StockroomItemsFor(IPermanentShop shop, IMerchandise merchandise)
	{
		var stockroomItemIds = shop.StockroomCell.GameItems
		                           .SelectMany(x => x.DeepItems)
		                           .Select(x => x.Id)
		                           .ToHashSet();
		return shop.StockedItems(merchandise)
		           .Where(x => stockroomItemIds.Contains(x.Id))
		           .OrderBy(x => x.Id);
	}
}

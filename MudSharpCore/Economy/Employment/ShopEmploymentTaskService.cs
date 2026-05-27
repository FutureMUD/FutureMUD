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

		var itemIds = StockroomItemsFor(shop, merchandise)
		              .Take(itemCount)
		              .Select(x => x.Id)
		              .ToList();
		if (itemIds.Count < itemCount)
		{
			message = $"{shop.EmploymentHostName.ColourName()} only has {itemIds.Count.ToString("N0", authorisedBy).ColourValue()} matching stockroom item{(itemIds.Count == 1 ? string.Empty : "s")} to move.";
			return false;
		}

		var plan = new EmploymentActionPlan([
			new GetItemsByIdActionStep(itemIds.Count, itemIds, [shop.StockroomCell]),
			new DeliverItemsActionStep(resolvedDestination, container, containerTag)
		]);

		try
		{
			task = shop.TaskBoard.CreateActiveTask($"restock {merchandise.Name}", plan, authorisedBy);
			message = $"You create a stockroom restock task for {shop.EmploymentHostName.ColourName()} to move {itemIds.Count.ToString("N0", authorisedBy).ColourValue()} {merchandise.Name.ColourName()} item{(itemIds.Count == 1 ? string.Empty : "s")}.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
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

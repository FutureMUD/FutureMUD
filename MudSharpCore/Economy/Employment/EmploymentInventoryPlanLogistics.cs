using MudSharp.Economy.Employment;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;

#nullable enable

namespace MudSharp.Economy.Employment;

internal static class EmploymentInventoryPlanLogistics
{
	public static bool CanUseInventoryPlan(ICharacter actor)
	{
		if (actor.Body is null || actor.Location is null || actor.Gameworld is null)
		{
			return false;
		}

		return actor.Body.HoldLocs is not null &&
		       actor.Body.WieldLocs is not null;
	}

	public static bool TryHoldItems(ICharacter actor, IReadOnlyCollection<IGameItem> items,
		out IReadOnlyCollection<IGameItem> heldItems, out string reason)
	{
		heldItems = [];
		if (!items.Any())
		{
			reason = "There are no task items to hold.";
			return false;
		}

		var plan = CreatePlan(actor, DesiredItemState.Held, items, null);
		var feasibility = plan.PlanIsFeasible();
		if (feasibility != InventoryPlanFeasibility.Feasible)
		{
			reason = InventoryPlanFeasibilityReason(feasibility);
			return false;
		}

		var resultItems = ExecutePlanForItems(plan, items, DesiredItemState.Held);
		if (resultItems.Count < items.Count || items.Any(x => resultItems.All(y => y.Id != x.Id)))
		{
			reason = "The assigned employee could not get all of the required task items.";
			return false;
		}

		var missing = resultItems.Where(x => !ActorCarriesItem(actor, x)).ToList();
		if (missing.Any())
		{
			reason = $"The assigned employee got {missing.First().Name}, but could not keep hold of it.";
			return false;
		}

		heldItems = resultItems;
		reason = string.Empty;
		return true;
	}

	public static bool TryPutItemsIntoContainer(ICharacter actor, IReadOnlyCollection<IGameItem> items,
		IGameItem targetContainer, out IReadOnlyCollection<IGameItem> loadedItems, out string reason)
	{
		loadedItems = [];
		if (!items.Any())
		{
			reason = "There are no task items to load.";
			return false;
		}

		var containerComponent = targetContainer.GetItemType<IContainer>();
		if (containerComponent is null)
		{
			reason = $"{targetContainer.Name} is not a container.";
			return false;
		}

		var rejected = items.FirstOrDefault(x => !containerComponent.CanPut(x));
		if (rejected is not null)
		{
			reason = $"{targetContainer.Name} cannot contain {rejected.Name}.";
			return false;
		}

		var plan = CreatePlan(actor, DesiredItemState.InContainer, items, targetContainer);
		var feasibility = plan.PlanIsFeasible();
		if (feasibility != InventoryPlanFeasibility.Feasible)
		{
			reason = InventoryPlanFeasibilityReason(feasibility);
			return false;
		}

		var resultItems = ExecutePlanForItems(plan, items, DesiredItemState.InContainer);
		if (resultItems.Count < items.Count || items.Any(x => resultItems.All(y => y.Id != x.Id)))
		{
			reason = "The assigned employee could not load all required task items.";
			return false;
		}

		var missing = resultItems.Where(x => !ContainerHasItem(containerComponent, x)).ToList();
		if (missing.Any())
		{
			reason = $"The assigned employee tried to load {missing.First().Name}, but it did not remain in {targetContainer.Name}.";
			return false;
		}

		loadedItems = resultItems;
		reason = string.Empty;
		return true;
	}

	public static bool TryDropItems(ICharacter actor, IReadOnlyCollection<IGameItem> items,
		out IReadOnlyCollection<IGameItem> droppedItems, out string reason)
	{
		droppedItems = [];
		if (!items.Any())
		{
			reason = "There are no task items to drop.";
			return false;
		}

		var plan = CreatePlan(actor, DesiredItemState.InRoom, items, null);
		var feasibility = plan.PlanIsFeasible();
		if (feasibility != InventoryPlanFeasibility.Feasible)
		{
			reason = InventoryPlanFeasibilityReason(feasibility);
			return false;
		}

		var resultItems = ExecutePlanForItems(plan, items, DesiredItemState.InRoom);
		if (resultItems.Count < items.Count || items.Any(x => resultItems.All(y => y.Id != x.Id)))
		{
			reason = "The assigned employee could not place all required task items.";
			return false;
		}

		droppedItems = resultItems;
		reason = string.Empty;
		return true;
	}

	public static string InventoryPlanFeasibilityReason(InventoryPlanFeasibility feasibility)
	{
		return feasibility switch
		{
			InventoryPlanFeasibility.NotFeasibleMissingItems => "The assigned employee cannot find the required task items or container.",
			InventoryPlanFeasibility.NotFeasibleNotEnoughHands => "The assigned employee does not have enough usable hands to arrange the task items.",
			InventoryPlanFeasibility.NotFeasibleNotEnoughWielders => "The assigned employee cannot free the required wielding bodyparts to arrange the task items.",
			_ => "The assigned employee cannot arrange their inventory for the task items."
		};
	}

	private static IInventoryPlan CreatePlan(ICharacter actor, DesiredItemState state,
		IReadOnlyCollection<IGameItem> items, IGameItem? secondaryTarget)
	{
		var actions = items.Select(item => InventoryPlanAction.LoadAction(
			actor.Gameworld,
			state,
			0,
			0,
			x => x == item,
			secondaryTarget is null ? null : x => x == secondaryTarget,
			originalReference: "employment logistics"));
		return new InventoryPlanTemplate(actor.Gameworld, actions).CreatePlan(actor);
	}

	private static List<IGameItem> ExecutePlanForItems(IInventoryPlan plan, IReadOnlyCollection<IGameItem> items,
		DesiredItemState desiredState)
	{
		var results = plan.ExecuteWholePlan().ToList();
		var requestedItemIds = items.Select(x => x.Id).ToHashSet();
		var resultItems = results
		                  .Where(x => x.ActionState == desiredState)
		                  .Select(x => x.PrimaryTarget)
		                  .Where(x => x is not null)
		                  .Where(x => requestedItemIds.Contains(x.Id))
		                  .DistinctBy(x => x.Id)
		                  .ToList();
		plan.FinalisePlanWithExemptions(resultItems);
		return resultItems;
	}

	private static bool ActorCarriesItem(ICharacter actor, IGameItem item)
	{
		return EmploymentWorkerItemLocator.IsHeldOrWielded(actor, item);
	}

	private static bool ContainerHasItem(IContainer container, IGameItem item)
	{
		return item.ContainedIn == container.Parent || container.Contents.Any(x => x.Id == item.Id);
	}
}

using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;

#nullable enable
namespace MudSharp.Magic.Vancian;

/// <summary>One scouted plan for all production costs, including aggregate quantity validation before any expenditure.</summary>
public sealed class VancianProductionPlan : IDisposable
{
	private readonly InventoryPlan _plan;
	public VancianProductionPlan(ICharacter actor, params IInventoryPlanTemplate[] templates)
	{
		var phases = templates.SelectMany(x => x.Phases).Select((x, i) =>
			(IInventoryPlanPhaseTemplate)new InventoryPlanPhaseTemplate(i + 1, x.Actions)).ToArray();
		_plan = new InventoryPlan(actor, new InventoryPlanTemplate(actor.Gameworld, phases.Length == 0 ? [new InventoryPlanPhaseTemplate(1, [])] : phases));
	}
	public string? Validate(params IGameItem[] protectedItems)
	{
		if (_plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible) return "The combined production and spell material plan is not feasible.";
		foreach (var group in _plan.Phases.Values.SelectMany(x => x.ScoutedItems).Where(x => x.Primary is not null).GroupBy(x => x.Primary.Id))
		{
			var item = group.First().Primary;
			var destructive = group.Where(x => x.Action.DesiredState is DesiredItemState.Consumed or DesiredItemState.ConsumeCommodity or DesiredItemState.ConsumeLiquid or DesiredItemState.Apply).ToArray();
			if (destructive.Length == 0) continue;
			if (protectedItems.Any(x => x.Id == group.Key)) return "The material plan must not consume the source or destination writing item.";
			var quantity = destructive.Where(x => x.Action is InventoryPlanActionConsume).Sum(x => (long)((InventoryPlanActionConsume)x.Action).Quantity);
			if (quantity > (item.GetItemType<IStackable>()?.Quantity ?? 1)) return "The combined plan would consume the same item quantity more than once.";
			var weight = destructive.Where(x => x.Action is InventoryPlanActionConsumeCommodity).Sum(x => ((InventoryPlanActionConsumeCommodity)x.Action).Weight);
			if (!double.IsFinite(weight) || weight > (item.GetItemType<ICommodity>()?.Weight ?? 0)) return "The combined plan exceeds the available commodity weight.";
			var liquids = destructive.Select(x => x.Action).OfType<InventoryPlanActionConsumeLiquid>().ToArray();
			if (liquids.Length > 0)
			{
				var mixture = item.GetItemType<ILiquidContainer>()?.LiquidMixture;
				if (mixture is null) return "A production liquid is unavailable.";
				foreach (var instance in mixture.Instances)
					if (liquids.SelectMany(x => x.LiquidToTake.Instances).Where(x => x.CanMergeWith(instance)).Sum(x => x.Amount) > instance.Amount)
						return "The combined plan exceeds the available quantity of a liquid.";
			}
			if (destructive.Any(x => x.Action is InventoryPlanActionApply) && destructive.Length > 1) return "An applied consumable cannot also fund another production action.";
			if (quantity > 0 && (weight > 0 || liquids.Length > 0 || group.Count() > destructive.Length)) return "A consumed item cannot also serve as a retained tool or a different material cost.";
		}
		return null;
	}
	public void Execute() => _plan.ExecuteWholePlan();
	public void Dispose() => _plan.FinalisePlan();
}

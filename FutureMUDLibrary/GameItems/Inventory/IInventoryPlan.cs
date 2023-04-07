using System.Collections.Generic;
using MudSharp.Effects.Interfaces;
using MudSharp.GameItems.Inventory.Plans;

namespace MudSharp.GameItems.Inventory
{
    public interface IInventoryPlan
    {
        List<IInventoryPlanItemEffect> AssociatedEffects { get; }
        InventoryPlanFeasibility PlanIsFeasible(int fromPhase, int toPhase);
        InventoryPlanFeasibility PlanIsFeasible(int fromPhase);
        InventoryPlanFeasibility PlanIsFeasible();
        InventoryPlanFeasibility CurrentPhaseIsFeasible();
        IEnumerable<(IInventoryPlanAction Action, InventoryPlanFeasibility Reason)> InfeasibleActions();

        /// <summary>
        /// Takes a peek at what the results WOULD be if this plan were executed, but does not take the actions or mark anything for restoration
        /// </summary>
        /// <returns>A collection of inventoryplanactionresults</returns>
        IEnumerable<InventoryPlanActionResult> PeekPlanResults();

        IEnumerable<InventoryPlanActionResult> ExecutePhase();
        IEnumerable<InventoryPlanActionResult> ExecuteWholePlan();
        IEnumerable<InventoryPlanActionResult> ExecutePlan(int fromPhase);
        IEnumerable<InventoryPlanActionResult> ExecutePlan(int fromPhase, int toPhase);
        IEnumerable<InventoryPlanActionResult> ScoutAllTargets();
        IEnumerable<InventoryPlanActionResult> ScoutAllTargets(int fromPhase);
        IEnumerable<InventoryPlanActionResult> ScoutAllTargets(int fromPhase, int toPhase);
        void FinalisePlan();
        void FinalisePlanNoRestore();
        void FinalisePlanWithExemptions(IList<IGameItem> exemptItems);
        int LastPhaseForItem(IGameItem item);
        bool IsItemFinished(IGameItem item);
        void SetPhase(int phase);
        bool IsFinished { get; }
    }
}
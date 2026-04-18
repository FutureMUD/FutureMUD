using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.Combat.Moves;

public class InventoryPlanMove : CombatMoveBase
{
    public IInventoryPlan Plan { get; init; }

    public Action<InventoryPlanActionResult> AfterPlanActions { get; init; }

    #region Overrides of CombatMoveBase

    public override double BaseDelay => 0.1;

    public override string Description => "Executing an inventory plan";

    public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
    {
        IEnumerable<InventoryPlanActionResult> results = Plan.ExecuteWholePlan();
        Plan.FinalisePlanNoRestore();
        foreach (InventoryPlanActionResult result in results)
        {
            AfterPlanActions?.Invoke(result);
        }

        return CombatMoveResult.Irrelevant;
    }

    #endregion
}
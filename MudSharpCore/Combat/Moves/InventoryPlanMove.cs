using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;

namespace MudSharp.Combat.Moves;

public class InventoryPlanMove : CombatMoveBase
{
	public IInventoryPlan Plan { get; set; }

	public Action<InventoryPlanActionResult> AfterPlanActions { get; set; }

	#region Overrides of CombatMoveBase

	public override double BaseDelay => 0.1;

	public override string Description => "Executing an inventory plan";

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var results = Plan.ExecuteWholePlan();
		Plan.FinalisePlanNoRestore();
		foreach (var result in results)
		{
			AfterPlanActions?.Invoke(result);
		}

		return CombatMoveResult.Irrelevant;
	}

	#endregion
}
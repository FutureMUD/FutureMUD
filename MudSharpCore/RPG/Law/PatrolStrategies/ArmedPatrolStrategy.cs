using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;

namespace MudSharp.RPG.Law.PatrolStrategies;

public class ArmedPatrolStrategy : RouteStrategyBase, IPatrolStrategy
{
	public override string Name => "ArmedPatrol";

	public ArmedPatrolStrategy(IFuturemud gameworld) : base(gameworld)
	{
		_weaponTemplate = new InventoryPlanTemplate(Gameworld, new List<IInventoryPlanPhaseTemplate>
		{
			new InventoryPlanPhaseTemplate(1, new List<IInventoryPlanAction>
			{
				new InventoryPlanActionSheath(Gameworld, 0, 0, item => item.IsItemType<IMeleeWeapon>(), null)
				{
					PrimaryItemFitnessScorer = item =>
						item.GetItemType<IMeleeWeapon>() is IMeleeWeapon mw &&
						mw.Classification.HasFlag(WeaponClassification.NonLethal)
							? 1.0
							: 100.0,
					ItemsAlreadyInPlaceMultiplier = 10.0
				},
				new InventoryPlanActionSheath(Gameworld, 0, 0,
						item => item.GetItemType<IMeleeWeapon>() is IMeleeWeapon mw &&
						        mw.Classification.HasFlag(WeaponClassification.NonLethal), null)
					{ ItemsAlreadyInPlaceOverrideFitnessScore = true }
			})
		});

		// TODO - this next one is pretty weak, revisit
		_armourTemplate = new InventoryPlanTemplate(Gameworld, new List<IInventoryPlanPhaseTemplate>
		{
			new InventoryPlanPhaseTemplate(1, new List<IInventoryPlanAction>
			{
				new InventoryPlanActionWear(Gameworld, 0, 0, item => item.IsItemType<IArmour>(), null)
					{ ItemsAlreadyInPlaceOverrideFitnessScore = true }
			})
		});

		_sheathTemplate = new InventoryPlanTemplate(Gameworld, new List<IInventoryPlanPhaseTemplate>
		{
			new InventoryPlanPhaseTemplate(1, new List<IInventoryPlanAction>
			{
				new InventoryPlanActionAttach(Gameworld, 0, 0, item => item.IsItemType<ISheath>(), null)
					{ ItemsAlreadyInPlaceOverrideFitnessScore = true },
				new InventoryPlanActionAttach(Gameworld, 0, 0, item => item.IsItemType<ISheath>(), null)
					{ ItemsAlreadyInPlaceOverrideFitnessScore = true }
			})
		});

		_beltTemplate = new InventoryPlanTemplate(Gameworld, new List<IInventoryPlanPhaseTemplate>
		{
			new InventoryPlanPhaseTemplate(1, new List<IInventoryPlanAction>
			{
				new InventoryPlanActionWear(Gameworld, 0, 0, item => item.IsItemType<IBelt>(), null)
					{ ItemsAlreadyInPlaceOverrideFitnessScore = true }
			})
		});
	}

	private IInventoryPlanTemplate _weaponTemplate;
	private IInventoryPlanTemplate _armourTemplate;
	private IInventoryPlanTemplate _sheathTemplate;
	private IInventoryPlanTemplate _beltTemplate;

	#region Overrides of RouteStrategyBase

	protected override void DoPreparationRoomAction(ICharacter member)
	{
		var plan = _armourTemplate.CreatePlan(member);
		if (plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		{
			plan.ExecuteWholePlan();
		}

		plan.FinalisePlanNoRestore();

		plan = _beltTemplate.CreatePlan(member);
		if (plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		{
			plan.ExecuteWholePlan();
		}

		plan.FinalisePlanNoRestore();

		plan = _sheathTemplate.CreatePlan(member);
		if (plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		{
			plan.ExecuteWholePlan();
		}

		plan.FinalisePlanNoRestore();

		plan = _weaponTemplate.CreatePlan(member);
		if (plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		{
			plan.ExecuteWholePlan();
		}

		plan.FinalisePlanNoRestore();
	}

	#endregion
}
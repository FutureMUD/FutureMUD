using System;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;

namespace MudSharp.Effects.Concrete;

public class InventoryPlanItemEffect : Effect, IInventoryPlanItemEffect
{
	public InventoryPlanItemEffect(IPerceivable owner, IInventoryPlan plan)
		: base(owner)
	{
		Plan = plan;
		Plan.AssociatedEffects.Add(this);
	}

	protected override string SpecificEffectType => "InventoryPlanItemEffect";

	public IInventoryPlan Plan { get; set; }

	public DesiredItemState DesiredState { get; set; }

	public IGameItem TargetItem { get; set; }

	public IGameItem SecondaryItem { get; set; }

	public override string Describe(IPerceiver voyeur)
	{
		return $"Desire to return {TargetItem.HowSeen(voyeur)} to state {DesiredState}.";
	}
}
using System;
using System.Linq;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;

namespace MudSharp.Effects.Concrete;

public class UnlockDoor : Effect, IEffectSubtype
{
	public IDoor Door { get; set; }
	public ICellExit Exit { get; set; }

	public ICharacter Character { get; set; }

	public UnlockDoor(ICharacter actor, IDoor door, ICellExit exit) : base(actor)
	{
		Character = actor;
		Door = door;
		Exit = exit;
	}

	protected override string SpecificEffectType => "Unlock Door";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Unlocking door: {Door.Parent.HowSeen(voyeur)}.";
	}

	public override void RemovalEffect()
	{
		var key =
			Character.Body.ExternalItems.SelectNotNull(y => y.GetItemType<IKey>())
			         .Concat(
				         Character.Body.ExternalItems
				                  .SelectNotNull(y => y.GetItemType<IContainer>())
				                  .Where(y => (y.Parent.GetItemType<IOpenable>()?.IsOpen ?? true) ||
				                              y.Parent.GetItemType<IOpenable>().CanOpen(Character.Body))
				                  .SelectMany(y => y.Contents.SelectNotNull(z => z.GetItemType<IKey>()))
			         ).FirstOrDefault(x => Door.Locks.Any(y => y.IsLocked && y.CanUnlock(Character, x)));

		if (key != null)
		{
			var template = new InventoryPlanTemplate(Character.Gameworld, new[]
			{
				new InventoryPlanPhaseTemplate(1, new[]
				{
					new InventoryPlanActionHold(Character.Gameworld, 0, 0, item => item == key.Parent, null)
				})
			});
			var plan = template.CreatePlan(Character);
			if (plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
			{
				plan.ExecuteWholePlan();
				foreach (var theLock in Door.Locks)
				{
					if (theLock.CanUnlock(Character, key))
					{
						theLock.Unlock(Character, key, Door.Parent, null);
					}
				}

				plan.FinalisePlan();
			}

			if (Door.CanOpen(Character.Body))
			{
				Character.Body.Open(Door, null, null);
				Character.Move(Exit);
				return;
			}

			Character.AddEffect(this, TimeSpan.FromSeconds(1.5));
		}
	}
}
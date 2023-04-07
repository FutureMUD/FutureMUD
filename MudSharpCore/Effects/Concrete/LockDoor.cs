using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;

namespace MudSharp.Effects.Concrete;

public class LockDoor : Effect, IEffectSubtype
{
	public override IEnumerable<string> Blocks => new[] { "movement" };

	public override string BlockingDescription(string blockingType, IPerceiver voyeur)
	{
		return
			$"locking a door behind {(Actor.IsSelf(voyeur) ? "yourself" : Actor.ApparentGender(voyeur).Reflexive())}";
	}

	public override bool IsBlockingEffect(string blockingType)
	{
		return blockingType.EqualTo("movement");
	}

	public ICharacter Actor { get; set; }
	public IDoor Door { get; set; }
	public Queue<Tuple<IKey, IInventoryPlan>> Plans { get; set; }

	public LockDoor(ICharacter actor, IDoor door, IEnumerable<Tuple<IKey, IInventoryPlan>> plans) : base(actor)
	{
		Actor = actor;
		Door = door;
		Plans = new Queue<Tuple<IKey, IInventoryPlan>>(Plans);
	}

	protected override string SpecificEffectType => "LockDoor";

	public override string Describe(IPerceiver voyeur)
	{
		return $"Locking door: {Door.Parent.HowSeen(voyeur)}.";
	}

	public override void RemovalEffect()
	{
		var next = Plans.Dequeue();
		if (next.Item2.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		{
			next.Item2.ExecuteWholePlan();
			foreach (var theLock in Door.Locks)
			{
				if (theLock.CanLock(Actor, next.Item1))
				{
					theLock.Lock(Actor, next.Item1, Door.Parent, null);
				}
			}

			next.Item2.FinalisePlan();
		}

		if (Plans.Any())
		{
			Actor.AddEffect(this, TimeSpan.FromSeconds(0.5));
		}
	}
}
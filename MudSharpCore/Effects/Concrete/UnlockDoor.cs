using MudSharp.Construction.Boundary;
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
        IKey key = Character.Body.ExternalItems
                            .SelectMany(x => x.ShallowAccessibleItems(Character))
                            .SelectNotNull(x => x.GetItemType<IKey>())
                            .FirstOrDefault(x => Door.Locks.Any(y => y.IsLocked && y.CanUnlock(Character, x)));

        if (key != null)
        {
            InventoryPlanTemplate template = new(Character.Gameworld, new[]
            {
                new InventoryPlanPhaseTemplate(1, new[]
                {
                    new InventoryPlanActionHold(Character.Gameworld, 0, 0, item => item == key.Parent, null)
                })
            });
            IInventoryPlan plan = template.CreatePlan(Character);
            if (plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
            {
                plan.ExecuteWholePlan();
                var unlockedDoor = false;
                foreach (ILock theLock in Door.Locks)
                {
                    if (theLock.CanUnlock(Character, key))
                    {
                        theLock.Unlock(Character, key, Door.Parent, null);
                        unlockedDoor = true;
                    }
                }

                plan.FinalisePlan();
                if (unlockedDoor)
                {
                    Character.EffectsOfType<FollowingPath>()
                             .FirstOrDefault(x => x.Exits.Any(y => ReferenceEquals(y.Exit, Exit.Exit)))
                             ?.RecordUnlockedExit(Exit);
                }
            }

            if (Door.CanOpen(Character.Body))
            {
                Character.Body.Open(Door, null, null);
                return;
            }

            Character.AddEffect(this, TimeSpan.FromSeconds(1.5));
        }
    }
}

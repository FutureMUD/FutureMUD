using Humanizer;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Movement;
using MudSharp.NPC.AI.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Effects.Concrete;

public class FollowingPath : Effect, IEffectSubtype, IRemoveOnCombatStart
{
    public Queue<ICellExit> Exits { get; set; }

    public FollowingPath(ICharacter owner, IEnumerable<ICellExit> exits) : base(owner, null)
    {
        Exits = new Queue<ICellExit>(exits);
    }

    protected override string SpecificEffectType => "FollowingPath";

    protected virtual bool RemoveWhenExitsEmpty => true;

    public override string Describe(IPerceiver voyeur)
    {
        string exitStrings = Exits.Select(x =>
        {
            if (x.OutboundDirection != CardinalDirection.Unknown)
            {
                return x.OutboundDirection.DescribeBrief();
            }

            return x is NonCardinalCellExit nc ? $"'{nc.Verb} {nc.PrimaryKeyword}'".ToLowerInvariant() : "??";
        }).Humanize();
        return $"Following a path: {exitStrings}";
    }

    public bool OpenDoors { get; set; }
    public bool UseKeys { get; set; }
    public bool SmashLockedDoors { get; set; }
    public bool UseDoorguards { get; set; }
	public bool CloseDoorsBehind { get; set; }

	public static FollowingPath CreateFullFriendlyPath(ICharacter owner, IEnumerable<ICellExit> exits,
		bool closeDoorsBehind = false)
	{
		return new FollowingPath(owner, exits)
		{
			UseDoorguards = true,
			UseKeys = true,
			OpenDoors = true,
			CloseDoorsBehind = closeDoorsBehind
		};
	}

    public virtual void FollowPathAction()
    {
        ICharacter ch = (ICharacter)Owner;

        if (ch.State.HasFlag(CharacterState.Dead) || ch.Corpse != null)
        {
            ch.RemoveEffect(this);
            return;
        }

        if (Exits.Count == 0)
        {
            ch.RemoveEffect(this);
            return;
        }

        ICellExit exit = Exits.Peek();
        if (ZeroGravityMovementHelper.IsZeroGravity(ch.Location, ch.RoomLayer, ch) &&
            !ZeroGravityMovementHelper.CanManeuver(ch))
        {
            ch.RemoveEffect(this);
            return;
        }

        if (exit.Origin != ch.Location)
        {
	        if (exit.Destination == ch.Location)
	        {
		        Exits.Dequeue();
		        if (CloseDoorsBehind)
		        {
			        CloseDoorBehind(ch, exit, UseKeys);
		        }

		        if (Exits.Count == 0 && RemoveWhenExitsEmpty)
		        {
			        ch.RemoveEffect(this);
		        }

		        return;
	        }

	        ch.RemoveEffect(this);
	        return;
        }

        if (!exit.WhichLayersExitAppears().Contains(ch.RoomLayer))
        {
            RoomLayer targetLayer = exit.WhichLayersExitAppears()
                                  .OrderBy(x => Math.Abs(ch.RoomLayer.LayerHeight() - x.LayerHeight()))
                                  .FirstOrDefault();
            if (!PathTowardsLayer(targetLayer))
            {
                ch.RemoveEffect(this);
                return;
            }
        }

        if (!ch.PositionState.Upright &&
            !ch.PositionState.MoveRestrictions.In(MovementAbility.Flying, MovementAbility.Swimming, MovementAbility.ZeroGravity))
        {
            IPositionState mobile = ch.MostUprightMobilePosition();
            if (mobile == null || !ch.CanMovePosition(mobile))
            {
                ch.RemoveEffect(this);
                return;
            }

            ch.MovePosition(mobile, null, null);
        }

        IMovementStrategy strategy = MovementStrategyFactory.GetStrategy(OpenDoors, UseKeys, SmashLockedDoors, UseDoorguards);
		switch (strategy.TryToMove(ch, exit))
		{
			case MovementStrategyResult.Moved:
				Exits.Dequeue();
				CloseDoorBehindAfterMovement(ch, exit);
				if (Exits.Count == 0 && RemoveWhenExitsEmpty)
				{
					ch.RemoveEffect(this);
				}

				return;
			case MovementStrategyResult.Waiting:
				return;
			default:
				ch.RemoveEffect(this);
				return;
		}
    }

	private void CloseDoorBehindAfterMovement(ICharacter ch, ICellExit exit)
	{
		if (!CloseDoorsBehind)
		{
			return;
		}

		if (ch.Location == exit.Destination)
		{
			CloseDoorBehind(ch, exit, UseKeys);
			return;
		}

		var movement = ch.Movement;
		var delay = movement is null
			? TimeSpan.FromSeconds(1)
			: movement.Duration + TimeSpan.FromTicks(movement.Duration.Ticks / 5) + TimeSpan.FromMilliseconds(100);
		ch.AddEffect(new DelayedAction(ch, _ =>
		{
			if (ch.Location == exit.Destination)
			{
				CloseDoorBehind(ch, exit, UseKeys);
			}
		}, "closing a door behind them"), delay);
	}

	public static void CloseDoorBehind(ICharacter ch, ICellExit exit, bool useKeys)
	{
		if (exit?.Exit.Door?.IsOpen != true)
		{
			return;
		}

		ch.Body.Close(exit.Exit.Door, null, null);
		if (!useKeys)
		{
			return;
		}

		var keys = ch.Body.ExternalItems
		             .SelectMany(x => x.ShallowAccessibleItems(ch))
		             .SelectNotNull(x => x.GetItemType<IKey>())
		             .ToList();
		var usableKeys = keys
		                 .Where(x => exit.Exit.Door.Locks.Any(y => !y.IsLocked && y.CanLock(ch, x)))
		                 .Select(x => Tuple.Create(x, GetHoldPlanForItem(ch, x.Parent)))
		                 .Where(x => x.Item2.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		                 .ToList();
		if (!usableKeys.Any())
		{
			return;
		}

		LockDoor effect = new(ch, exit.Exit.Door, usableKeys);
		ch.AddEffect(effect);
		ch.RemoveEffect(effect, true);
	}

	private static IInventoryPlan GetHoldPlanForItem(ICharacter ch, IGameItem item)
	{
		InventoryPlanTemplate template = new(ch.Gameworld, new[]
		{
			new InventoryPlanPhaseTemplate(1, new[]
			{
				new InventoryPlanActionHold(ch.Gameworld, 0, 0, x => x == item, null)
			})
		});
		return template.CreatePlan(ch);
	}

    public bool PathTowardsLayer(RoomLayer targetLayer)
    {
        ICharacter ch = (ICharacter)Owner;
        if (ZeroGravityMovementHelper.IsZeroGravity(ch.Location, ch.RoomLayer, ch) &&
            !ZeroGravityMovementHelper.HasIndependentPropulsion(ch))
        {
            return false;
        }

        if (ch.RoomLayer.IsLowerThan(targetLayer))
        {
            if (ch.PositionState != PositionSwimming.Instance && ch.Location.IsUnderwaterLayer(ch.RoomLayer))
            {
                ch.MovePosition(PositionSwimming.Instance, PositionModifier.None, null, null, null);
                return true;
            }

            if (ch.Location.IsUnderwaterLayer(ch.RoomLayer))
            {
                ((ISwim)ch).Ascend();
                return true;
            }

            if (ch.PositionState != PositionFlying.Instance && ch.CanFly().Truth)
            {
                ch.Fly();
                return true;
            }

            if (ch.PositionState == PositionFlying.Instance)
            {
                ((IFly)ch).Ascend();
                return true;
            }

            if (ch.CanClimbUp().Truth)
            {
                ch.ClimbUp();
                return true;
            }

            return false;
        }

        if (ch.PositionState != PositionSwimming.Instance && ch.Location.IsSwimmingLayer(ch.RoomLayer))
        {
            ch.MovePosition(PositionSwimming.Instance, PositionModifier.None, null, null, null);
            return true;
        }

        if (ch.Location.IsSwimmingLayer(ch.RoomLayer))
        {
            ((ISwim)ch).Dive();
            return true;
        }

        if (ch.PositionState == PositionFlying.Instance)
        {
            if (ch.RoomLayer == RoomLayer.GroundLevel)
            {
                ch.Land();
                return true;
            }

            ((IFly)ch).Dive();
            return true;
        }

        if (ch.CanClimbDown().Truth)
        {
            ch.ClimbDown();
            return true;
        }

        return false;
    }
}

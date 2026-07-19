using MudSharp.Body.Position;
using MudSharp.Commands.Socials;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Movement;

namespace MudSharp.NPC.AI.Strategies;

public class MovementStrategyFactory
{
    public static IMovementStrategy GetStrategy(bool openDoors, bool useKeys, bool smashDoors, bool useDoorguards)
    {
        if (!openDoors && !smashDoors && !useDoorguards)
        {
            return new MoveOnlyStrategy();
        }

        if (openDoors && !smashDoors && !useDoorguards && !useKeys)
        {
            return new OpenDoorsOnlyStrategy();
        }

        if (openDoors && useDoorguards && !useKeys)
        {
            return new OpenDoorsUseDoorguardsStrategy();
        }

        if (openDoors && useKeys && !useDoorguards)
        {
            return new UseKeysStrategy();
        }

        if (openDoors && useKeys)
        {
            return new FullFriendlyStrategy();
        }

        if (!openDoors && useDoorguards)
        {
            return new UseDoorguardsOnlyStrategy();
        }

        if (openDoors)
        {
            return new BreakDownDoorsStrategy();
        }

        return new BreakDownDoorsOnlyStrategy();
    }

    internal abstract class BaseStrategy : IMovementStrategy
    {
        public bool DoorGuardIsOpeningDoor(ICellExit exit)
        {
            return exit.Origin.Characters
                       .Concat(exit.Destination.Characters)
                       .Any(x => x.AffectedBy<IDoorguardOpeningDoorEffect>(exit));
        }

        public MovementStrategyResult CheckDoorGuard(ICharacter ch, ICharacter tch, ICellExit exit)
        {
            if (tch is not INPC tchNPC)
            {
                return MovementStrategyResult.Failed;
            }

            List<DoorguardAI> doorguardAI = tchNPC.AIs.OfType<DoorguardAI>().ToList();
            foreach (DoorguardAI ai in doorguardAI)
            {
                WouldOpenResponse response = ai.WouldOpen(tch, ch, exit);
                if (response.Response != WouldOpenResponseType.WontOpen)
                {
                    switch (response.Response)
                    {
                        case WouldOpenResponseType.WillOpenIfKnock:
                            exit.Exit.Door.Knock(ch);
                            return MovementStrategyResult.Waiting;
                        case WouldOpenResponseType.WillOpenIfMove:
                            return ch.Move(exit) ? MovementStrategyResult.Waiting : MovementStrategyResult.Failed;
                        case WouldOpenResponseType.WillOpenIfSocial:
                            ISocial social =
                                ch.Gameworld.Socials.FirstOrDefault(x => x.Applies(ch, response.Social, false));
                            if (social == null)
                            {
                                return MovementStrategyResult.Failed;
                            }

                            social.Execute(ch,
                                (response.DirectionRequired || !response.SocialTargetRequired
                                    ? Enumerable.Empty<IPerceivable>()
                                    : new[] { tch }).ToList(), response.DirectionRequired ? exit : null, null);
                            return MovementStrategyResult.Waiting;
                    }
                }
            }

            return MovementStrategyResult.Failed;
        }

        public IInventoryPlan GetHoldPlanForItem(ICharacter ch, IGameItem item)
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

        public bool CheckPosition(ICharacter ch)
        {
            if (!ch.CanMove(CanMoveFlags.IgnoreCancellableActionBlockers | CanMoveFlags.IgnoreSafeMovement | CanMoveFlags.IgnoreWhetherExitCanBeCrossed))
            {
                if (ch.PositionState.MoveRestrictions == MovementAbility.Restricted)
                {
                    IPositionState position = ch.MostUprightMobilePosition();
                    if (position == null)
                    {
                        return false;
                    }

                    ch.MovePosition(position, PositionModifier.None, null, null, null);

                    if (!ch.CanMove(CanMoveFlags.IgnoreCancellableActionBlockers | CanMoveFlags.IgnoreSafeMovement | CanMoveFlags.IgnoreWhetherExitCanBeCrossed))
                    {
                        return false;
                    }
                }

                return false;
            }

            return true;
        }

        public abstract MovementStrategyResult TryToMove(ICharacter ch, ICellExit exit);
    }

    internal class MoveOnlyStrategy : BaseStrategy
    {
        public override MovementStrategyResult TryToMove(ICharacter ch, ICellExit exit)
        {
            if (!CheckPosition(ch) || ch.Movement != null)
            {
                return MovementStrategyResult.Failed;
            }

            return ch.Move(exit) ? MovementStrategyResult.Moved : MovementStrategyResult.Failed;
        }
    }

    internal class OpenDoorsOnlyStrategy : BaseStrategy
    {
        public override MovementStrategyResult TryToMove(ICharacter character, ICellExit exit)
        {
            if (!CheckPosition(character) || character.Movement != null)
            {
                return MovementStrategyResult.Failed;
            }

            if (!character.CanMove(exit))
            {
                return MovementStrategyResult.Failed;
            }

            if (!(exit.Exit.Door?.IsOpen ?? true))
            {
                if (!character.Body.CanOpen(exit.Exit.Door))
                {
                    return MovementStrategyResult.Failed;
                }

                character.Body.Open(exit.Exit.Door, null, null);
            }

            return character.Move(exit) ? MovementStrategyResult.Moved : MovementStrategyResult.Failed;
        }
    }

    internal class UseKeysStrategy : BaseStrategy
    {
        public override MovementStrategyResult TryToMove(ICharacter ch, ICellExit exit)
        {
            if (!CheckPosition(ch) || ch.Movement != null)
            {
                return MovementStrategyResult.Failed;
            }

            if (!ch.CanMove(exit))
            {
                return MovementStrategyResult.Failed;
            }

            if (!(exit.Exit.Door?.IsOpen ?? true))
            {
                if (!ch.Body.CanOpen(exit.Exit.Door) && ch.Body.CouldOpen(exit.Exit.Door))
                {
                    if (exit.Exit.Door.Locks.Any(x => x.IsLocked))
                    {
                        UnlockDoor effect = new(ch, exit.Exit.Door, exit);
                        ch.AddEffect(effect);
                        ch.RemoveEffect(effect, true); // Add and instantly remove to trigger the first action
                        return MovementStrategyResult.Waiting;
                    }

                    return MovementStrategyResult.Failed;
                }

                ch.Body.Open(exit.Exit.Door, null, null);
            }

            return ch.Move(exit) ? MovementStrategyResult.Moved : MovementStrategyResult.Failed;
        }
    }

    internal class BreakDownDoorsStrategy : BaseStrategy
    {
        public override MovementStrategyResult TryToMove(ICharacter ch, ICellExit exit)
        {
            if (!CheckPosition(ch) || ch.Movement != null)
            {
                return MovementStrategyResult.Failed;
            }

            if (!ch.CanMove(exit))
            {
                return MovementStrategyResult.Failed;
            }

            if (!(exit.Exit.Door?.IsOpen ?? true))
            {
                if (!ch.Body.CanOpen(exit.Exit.Door))
                {
                    ch.AddEffect(new BreakDownDoor(ch, exit));
                    return MovementStrategyResult.Waiting;
                }

                ch.Body.Open(exit.Exit.Door, null, null);
            }

            return ch.Move(exit) ? MovementStrategyResult.Moved : MovementStrategyResult.Failed;
        }
    }

    internal class BreakDownDoorsOnlyStrategy : BaseStrategy
    {
        public override MovementStrategyResult TryToMove(ICharacter ch, ICellExit exit)
        {
            if (!CheckPosition(ch) || ch.Movement != null)
            {
                return MovementStrategyResult.Failed;
            }

            if (!ch.CanMove(exit))
            {
                return MovementStrategyResult.Failed;
            }

            if (!(exit.Exit.Door?.IsOpen ?? true))
            {
                ch.AddEffect(new BreakDownDoor(ch, exit));
                return MovementStrategyResult.Waiting;
            }

            return ch.Move(exit) ? MovementStrategyResult.Moved : MovementStrategyResult.Failed;
        }
    }

    internal class FullFriendlyStrategy : BaseStrategy
    {
        public override MovementStrategyResult TryToMove(ICharacter ch, ICellExit exit)
        {
            if (!CheckPosition(ch) || ch.Movement != null)
            {
                return MovementStrategyResult.Failed;
            }

            if (!ch.CanMove(exit))
            {
                return MovementStrategyResult.Failed;
            }

            if (!(exit.Exit.Door?.IsOpen ?? true))
            {
                if (!ch.Body.CanOpen(exit.Exit.Door))
                {
                    if (DoorGuardIsOpeningDoor(exit))
                    {
                        return MovementStrategyResult.Waiting;
                    }

                    MovementStrategyResult result = MovementStrategyResult.Failed;
                    foreach (ICharacter tch in ch.Location.Characters.Where(x =>
                                 x.AffectedBy<IDoorguardModeEffect>() && !x.AffectedBy<DoorguardOpeningDoor>()))
                    {
                        result = CheckDoorGuard(ch, tch, exit);
                        if (result != MovementStrategyResult.Failed)
                        {
                            break;
                        }
                    }

                    if (result == MovementStrategyResult.Failed)
                    {
                        foreach (ICharacter tch in exit.Destination.Characters.Where(x =>
                                     x.AffectedBy<IDoorguardModeEffect>() && !x.AffectedBy<DoorguardOpeningDoor>()))
                        {
                            result = CheckDoorGuard(ch, tch, exit);
                            if (result != MovementStrategyResult.Failed)
                            {
                                break;
                            }
                        }
                    }

                    if (result == MovementStrategyResult.Failed)
                    {
                        if (exit.Exit.Door.Locks.Any(x => x.IsLocked))
                        {
                            UnlockDoor effect = new(ch, exit.Exit.Door, exit);
                            ch.AddEffect(effect);
                            ch.RemoveEffect(effect, true); // Add and instantly remove to trigger the first action
                            return MovementStrategyResult.Waiting;
                        }

                        return MovementStrategyResult.Failed;
                    }

                    return result;
                }
                else
                {
                    ch.Body.Open(exit.Exit.Door, null, null, true);
                }
            }

            return ch.Move(exit) ? MovementStrategyResult.Moved : MovementStrategyResult.Failed;
        }
    }

    internal class OpenDoorsUseDoorguardsStrategy : BaseStrategy
    {
        public override MovementStrategyResult TryToMove(ICharacter ch, ICellExit exit)
        {
            if (!CheckPosition(ch) || ch.Movement != null)
            {
                return MovementStrategyResult.Failed;
            }

            if (!ch.CanMove(exit))
            {
                return MovementStrategyResult.Failed;
            }

            if (!(exit.Exit.Door?.IsOpen ?? true))
            {
                if (!ch.Body.CanOpen(exit.Exit.Door))
                {
                    if (DoorGuardIsOpeningDoor(exit))
                    {
                        return MovementStrategyResult.Waiting;
                    }

                    MovementStrategyResult result = MovementStrategyResult.Failed;
                    foreach (ICharacter tch in ch.Location.Characters.Where(x =>
                                 x.AffectedBy<IDoorguardModeEffect>() && !x.AffectedBy<DoorguardOpeningDoor>()))
                    {
                        result = CheckDoorGuard(ch, tch, exit);
                        if (result != MovementStrategyResult.Failed)
                        {
                            break;
                        }
                    }

                    if (result == MovementStrategyResult.Failed)
                    {
                        foreach (ICharacter tch in exit.Destination.Characters.Where(x =>
                                     x.AffectedBy<IDoorguardModeEffect>() && !x.AffectedBy<DoorguardOpeningDoor>()))
                        {
                            result = CheckDoorGuard(ch, tch, exit);
                            if (result != MovementStrategyResult.Failed)
                            {
                                break;
                            }
                        }
                    }

                    if (result == MovementStrategyResult.Failed)
                    {
                        return MovementStrategyResult.Failed;
                    }

                    return result;
                }
                else
                {
                    ch.Body.Open(exit.Exit.Door, null, null);
                }
            }

            return ch.Move(exit) ? MovementStrategyResult.Moved : MovementStrategyResult.Failed;
        }
    }

    internal class UseDoorguardsOnlyStrategy : BaseStrategy
    {
        public override MovementStrategyResult TryToMove(ICharacter ch, ICellExit exit)
        {
            if (!CheckPosition(ch) || ch.Movement != null)
            {
                return MovementStrategyResult.Failed;
            }

            if (!ch.CanMove(exit))
            {
                return MovementStrategyResult.Failed;
            }

            if (!(exit.Exit.Door?.IsOpen ?? true))
            {
                if (DoorGuardIsOpeningDoor(exit))
                {
                    return MovementStrategyResult.Waiting;
                }

                MovementStrategyResult result = MovementStrategyResult.Failed;
                foreach (ICharacter tch in ch.Location.Characters.Where(x =>
                             x.AffectedBy<IDoorguardModeEffect>() && !x.AffectedBy<DoorguardOpeningDoor>()))
                {
                    result = CheckDoorGuard(ch, tch, exit);
                    if (result != MovementStrategyResult.Failed)
                    {
                        break;
                    }
                }

                if (result == MovementStrategyResult.Failed)
                {
                    foreach (ICharacter tch in exit.Destination.Characters.Where(x =>
                                 x.AffectedBy<IDoorguardModeEffect>() && !x.AffectedBy<DoorguardOpeningDoor>()))
                    {
                        result = CheckDoorGuard(ch, tch, exit);
                        if (result != MovementStrategyResult.Failed)
                        {
                            break;
                        }
                    }
                }

                if (result == MovementStrategyResult.Failed)
                {
                    return MovementStrategyResult.Failed;
                }

                return result;
            }

            return ch.Move(exit) ? MovementStrategyResult.Moved : MovementStrategyResult.Failed;
        }
    }
}

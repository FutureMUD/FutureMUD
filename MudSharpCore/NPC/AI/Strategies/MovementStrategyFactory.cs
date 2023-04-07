using System;
using System.Linq;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;

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
		public bool CheckDoorGuard(ICharacter ch, ICharacter tch, ICellExit exit)
		{
			if (tch is not INPC tchNPC)
			{
				return false;
			}

			var doorguardAI = tchNPC.AIs.OfType<DoorguardAI>().ToList();
			foreach (var ai in doorguardAI)
			{
				var response = ai.WouldOpen(tch, ch, exit);
				if (response.Response != WouldOpenResponseType.WontOpen)
				{
					switch (response.Response)
					{
						case WouldOpenResponseType.WillOpenIfKnock:
							exit.Exit.Door.Knock(ch);
							return true;
						case WouldOpenResponseType.WillOpenIfMove:
							return true;
						case WouldOpenResponseType.WillOpenIfSocial:
							var social =
								ch.Gameworld.Socials.FirstOrDefault(x => x.Applies(ch, response.Social, false));
							if (social == null)
							{
								return false;
							}

							social.Execute(ch,
								(response.DirectionRequired || !response.SocialTargetRequired
									? Enumerable.Empty<IPerceivable>()
									: new[] { tch }).ToList(), response.DirectionRequired ? exit : null, null);
							return true;
					}
				}
			}

			return true;
		}

		public IInventoryPlan GetHoldPlanForItem(ICharacter ch, IGameItem item)
		{
			var template = new InventoryPlanTemplate(ch.Gameworld, new[]
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
			if (!ch.CanMove())
			{
				if (ch.PositionState.MoveRestrictions == MovementAbility.Restricted)
				{
					var position = ch.MostUprightMobilePosition();
					if (position == null)
					{
						return false;
					}

					ch.MovePosition(position, PositionModifier.None, null, null, null);

					if (!ch.CanMove())
					{
						return false;
					}
				}

				return false;
			}

			return true;
		}

		public abstract bool TryToMove(ICharacter ch, ICellExit exit);
	}

	internal class MoveOnlyStrategy : BaseStrategy
	{
		public override bool TryToMove(ICharacter ch, ICellExit exit)
		{
			if (!CheckPosition(ch) || ch.Movement != null)
			{
				return false;
			}

			return ch.Move(exit);
		}
	}

	internal class OpenDoorsOnlyStrategy : BaseStrategy
	{
		public override bool TryToMove(ICharacter character, ICellExit exit)
		{
			if (!CheckPosition(character) || character.Movement != null)
			{
				return false;
			}

			if (!character.CanMove(exit))
			{
				return false;
			}

			if (!(exit.Exit.Door?.IsOpen ?? true))
			{
				if (!character.Body.CanOpen(exit.Exit.Door))
				{
					return false;
				}

				character.Body.Open(exit.Exit.Door, null, null);
			}

			return character.Move(exit);
		}
	}

	internal class UseKeysStrategy : BaseStrategy
	{
		public override bool TryToMove(ICharacter ch, ICellExit exit)
		{
			if (!CheckPosition(ch) || ch.Movement != null)
			{
				return false;
			}

			if (!ch.CanMove(exit))
			{
				return false;
			}

			if (!(exit.Exit.Door?.IsOpen ?? true))
			{
				if (!ch.Body.CanOpen(exit.Exit.Door) && ch.Body.CouldOpen(exit.Exit.Door))
				{
					if (exit.Exit.Door.Locks.Any(x => x.IsLocked))
					{
						var effect = new UnlockDoor(ch, exit.Exit.Door, exit);
						ch.AddEffect(effect);
						ch.RemoveEffect(effect, true); // Add and instantly remove to trigger the first action
						return false;
					}

					return false;
				}

				ch.Body.Open(exit.Exit.Door, null, null);
			}

			return ch.Move(exit);
		}
	}

	internal class BreakDownDoorsStrategy : BaseStrategy
	{
		public override bool TryToMove(ICharacter ch, ICellExit exit)
		{
			if (!CheckPosition(ch) || ch.Movement != null)
			{
				return false;
			}

			if (!ch.CanMove(exit))
			{
				return false;
			}

			if (!(exit.Exit.Door?.IsOpen ?? true))
			{
				if (!ch.Body.CanOpen(exit.Exit.Door))
				{
					ch.AddEffect(new BreakDownDoor(ch, exit));
					return false;
				}

				ch.Body.Open(exit.Exit.Door, null, null);
			}

			return ch.Move(exit);
		}
	}

	internal class BreakDownDoorsOnlyStrategy : BaseStrategy
	{
		public override bool TryToMove(ICharacter ch, ICellExit exit)
		{
			if (!CheckPosition(ch) || ch.Movement != null)
			{
				return false;
			}

			if (!ch.CanMove(exit))
			{
				return false;
			}

			if (!(exit.Exit.Door?.IsOpen ?? true))
			{
				ch.AddEffect(new BreakDownDoor(ch, exit));
				return false;
			}

			return ch.Move(exit);
		}
	}

	internal class FullFriendlyStrategy : BaseStrategy
	{
		public override bool TryToMove(ICharacter ch, ICellExit exit)
		{
			if (!CheckPosition(ch) || ch.Movement != null)
			{
				return false;
			}

			if (!ch.CanMove(exit))
			{
				return false;
			}

			if (!(exit.Exit.Door?.IsOpen ?? true))
			{
				if (!ch.Body.CanOpen(exit.Exit.Door))
				{
					var result = false;
					foreach (var tch in ch.Location.Characters.Where(x =>
						         x.AffectedBy<DoorguardMode>() && !x.AffectedBy<DoorguardOpeningDoor>()))
					{
						result = CheckDoorGuard(ch, tch, exit);
						if (result)
						{
							break;
						}
					}

					if (!result)
					{
						foreach (var tch in exit.Destination.Characters.Where(x =>
							         x.AffectedBy<DoorguardMode>() && !x.AffectedBy<DoorguardOpeningDoor>()))
						{
							result = CheckDoorGuard(ch, tch, exit);
							if (result)
							{
								break;
							}
						}
					}

					if (!result)
					{
						if (exit.Exit.Door.Locks.Any(x => x.IsLocked))
						{
							var effect = new UnlockDoor(ch, exit.Exit.Door, exit);
							ch.AddEffect(effect);
							ch.RemoveEffect(effect, true); // Add and instantly remove to trigger the first action
						}

						return false;
					}
				}
				else
				{
					ch.Body.Open(exit.Exit.Door, null, null, true);
				}
			}

			return ch.Move(exit);
		}
	}

	internal class OpenDoorsUseDoorguardsStrategy : BaseStrategy
	{
		public override bool TryToMove(ICharacter ch, ICellExit exit)
		{
			if (!CheckPosition(ch) || ch.Movement != null)
			{
				return false;
			}

			if (!ch.CanMove(exit))
			{
				return false;
			}

			if (!(exit.Exit.Door?.IsOpen ?? true))
			{
				if (!ch.Body.CanOpen(exit.Exit.Door))
				{
					var result = false;
					foreach (var tch in ch.Location.Characters.Where(x =>
						         x.AffectedBy<DoorguardMode>() && !x.AffectedBy<DoorguardOpeningDoor>()))
					{
						result = CheckDoorGuard(ch, tch, exit);
						if (result)
						{
							break;
						}
					}

					if (!result)
					{
						foreach (var tch in exit.Destination.Characters.Where(x =>
							         x.AffectedBy<DoorguardMode>() && !x.AffectedBy<DoorguardOpeningDoor>()))
						{
							result = CheckDoorGuard(ch, tch, exit);
							if (result)
							{
								break;
							}
						}
					}

					if (!result)
					{
						return false;
					}
				}
				else
				{
					ch.Body.Open(exit.Exit.Door, null, null);
				}
			}

			return ch.Move(exit);
		}
	}

	internal class UseDoorguardsOnlyStrategy : BaseStrategy
	{
		public override bool TryToMove(ICharacter ch, ICellExit exit)
		{
			if (!CheckPosition(ch) || ch.Movement != null)
			{
				return false;
			}

			if (!ch.CanMove(exit))
			{
				return false;
			}

			if (!(exit.Exit.Door?.IsOpen ?? true))
			{
				var result = false;
				foreach (var tch in ch.Location.Characters.Where(x =>
					         x.AffectedBy<DoorguardMode>() && !x.AffectedBy<DoorguardOpeningDoor>()))
				{
					result = CheckDoorGuard(ch, tch, exit);
					if (result)
					{
						break;
					}
				}

				if (!result)
				{
					foreach (var tch in exit.Destination.Characters.Where(x =>
						         x.AffectedBy<DoorguardMode>() && !x.AffectedBy<DoorguardOpeningDoor>()))
					{
						result = CheckDoorGuard(ch, tch, exit);
						if (result)
						{
							break;
						}
					}
				}

				if (!result)
				{
					return false;
				}
			}

			return ch.Move(exit);
		}
	}
}
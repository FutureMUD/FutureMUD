using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.NPC.AI.Strategies;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Interfaces;
using MudSharp.PerceptionEngine;

namespace MudSharp.NPC.AI;

public abstract class PathingAIBase : ArtificialIntelligenceBase
{
	#region Constructors and Initialisation

	protected PathingAIBase(Models.ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
		LoadFromXML(XElement.Parse(ai.Definition));
	}

	protected virtual void LoadFromXML(XElement root)
	{
	}

	#endregion

	public bool MoveEvenIfObstructionInWay { get; protected set; }

	public bool OpenDoors { get; protected set; }

	public bool UseKeys { get; protected set; }

	public bool SmashLockedDoors { get; protected set; }

	public bool CloseDoorsBehind { get; protected set; }

	public bool UseDoorguards { get; protected set; }

	protected virtual bool IsPathingEnabled(ICharacter character)
	{
		return true;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		switch (type)
		{
			case EventType.FiveSecondTick:
				var ch = (ICharacter)arguments[0];
				FiveSecondTick(ch);
				return false;
			case EventType.CharacterEnterCellFinish:
			case EventType.LeaveCombat:
				ch = (ICharacter)arguments[0];
				CheckPathingEffect(ch, true);
				return false;
			case EventType.CharacterStopMovementClosedDoor:
				ClosedDoor((ICharacter)arguments[0], (ICellExit)arguments[2]);
				return false;
			case EventType.CommandDelayExpired:
				return CommandDelaySmash((ICharacter)arguments[0], (IEnumerable<string>)arguments[1]);
			case EventType.CharacterEnterCell:
				CheckCloseDoor((ICharacter)arguments[0], (ICellExit)arguments[2]);
				return false;
			case EventType.MinuteTick:
				ch = (ICharacter)arguments[0];
				CheckPathingEffect(ch, true);
				return false;
		}

		return false;
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		foreach (var type in types)
		{
			switch (type)
			{
				case EventType.FiveSecondTick:
				case EventType.CharacterEnterCellFinish:
				case EventType.LeaveCombat:
				case EventType.CharacterStopMovementClosedDoor:
				case EventType.CommandDelayExpired:
				case EventType.CharacterEnterCell:
				case EventType.MinuteTick:
					return true;
			}
		}

		return false;
	}

	protected void FiveSecondTick(ICharacter ch)
	{
		if (ch.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		if (ch.Movement != null || ch.Combat != null)
		{
			return;
		}

		if (ch.AffectedBy<BreakDownDoor>())
		{
			CheckSmash(ch);
			return;
		}

		if (ch.AffectedBy<UnlockDoor>() || ch.AffectedBy<LockDoor>())
		{
			return;
		}

		CheckPathingEffect(ch, false);
	}

	protected IInventoryPlan GetHoldPlanForItem(ICharacter ch, IGameItem item)
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

	protected void CheckCloseDoor(ICharacter ch, ICellExit exit)
	{
		if (ch.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		if (CloseDoorsBehind && exit.Exit.Door?.IsOpen == true)
		{
			ch.Body.Close(exit.Exit.Door, null, null);
			if (UseKeys)
			{
				var keys =
					ch.Body.ExternalItems.SelectNotNull(y => y.GetItemType<IKey>())
					  .Concat(
						  ch.Body.ExternalItems
						    .SelectNotNull(y => y.GetItemType<IContainer>())
						    .Where(y => (y.Parent.GetItemType<IOpenable>()?.IsOpen ?? true) ||
						                y.Parent.GetItemType<IOpenable>().CanOpen(ch.Body))
						    .SelectMany(y => y.Contents.SelectNotNull(z => z.GetItemType<IKey>()))
					  ).ToList();
				var importantKeys = keys.Where(x => exit.Exit.Door.Locks.Any(y => !y.IsLocked && y.CanUnlock(ch, x)))
				                        .ToList();
				var usableKeys = importantKeys.Select(x => Tuple.Create(x, GetHoldPlanForItem(ch, x.Parent)))
				                              .Where(x => x.Item2.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
				                              .ToList();

				var effect = new LockDoor(ch, exit.Exit.Door, usableKeys);
				ch.AddEffect(effect);
				ch.RemoveEffect(effect, true);
			}
		}
	}

	protected void Smash(ICharacter ch, ICellExit exit)
	{
		if (ch.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		var unarmedSmashes = ch.Race
		                       .UsableNaturalWeaponAttacks(ch, exit.Exit.Door.Parent, false,
			                       BuiltInCombatMoveType.UnarmedSmashItem)
		                       .ToList();
		if (!ch.Race.CombatSettings.CanUseWeapons)
		{
			if (!unarmedSmashes.Any())
			{
				ch.RemoveAllEffects(x => x.IsEffectType<BreakDownDoor>());
				return;
			}

			var attack = unarmedSmashes.GetWeightedRandom(x => x.Attack.Weighting);
			if (ch.Combat == null)
			{
				var nmove = new UnarmedSmashItemAttack(attack.Attack)
					{ Assailant = ch, Target = exit.Exit.Door.Parent, ParentItem = null, NaturalAttack = attack };
				nmove.ResolveMove(null);
				ch.SpendStamina(nmove.StaminaCost);
				ch.AddEffect(
					new CommandDelay(ch, "Smash",
						onExpireAction: () => { ch.Send("You feel as if you could smash something again."); }),
					TimeSpan.FromSeconds(10));
			}
			else
			{
				ch.TakeOrQueueCombatAction(
					SelectedCombatAction.GetEffectSmashItemUnarmed(ch, exit.Exit.Door.Parent, null, attack));
			}

			return;
		}

		var weapons = ch.Body.HeldOrWieldedItems
		                .Concat(ch.Body.ExternalItems.SelectNotNull(x => x.GetItemType<ISheath>())
		                          .SelectNotNull(x => x.Content?.Parent))
		                .SelectNotNull(x => x.GetItemType<IMeleeWeapon>()).ToList();
		var weaponSmashes = weapons.Select(x => Tuple.Create(x,
			                           x.WeaponType.UsableAttacks(ch, x.Parent, exit.Exit.Door.Parent,
				                           ch.Body.WieldedHandCount(x.Parent) == 1
					                           ? AttackHandednessOptions.OneHandedOnly
					                           : AttackHandednessOptions.TwoHandedOnly, false,
				                           BuiltInCombatMoveType.MeleeWeaponSmashItem)))
		                           .Where(x => x.Item2.Any()).ToList();

		var options = new List<Tuple<Tuple<IMeleeWeapon, IEnumerable<IWeaponAttack>>, IInventoryPlan>>();
		while (weaponSmashes.Any())
		{
			var action = weaponSmashes.GetRandomElement();
			var template = new InventoryPlanTemplate(ch.Gameworld,
				new[]
				{
					new InventoryPlanPhaseTemplate(1,
						new[]
						{
							new InventoryPlanActionWield(ch.Gameworld, 0, 0,
								item => item == action.Item1.Parent,
								null
							)
						}
					)
				}
			);
			var plan = template.CreatePlan(ch);
			weaponSmashes.RemoveAll(x => x.Item1 == action.Item1);
			if (plan.PlanIsFeasible() != InventoryPlanFeasibility.Feasible)
			{
				continue;
			}

			options.Add(Tuple.Create(action, plan));
		}

		if (!options.Any())
		{
			if (unarmedSmashes.Any())
			{
				var attack = unarmedSmashes.GetWeightedRandom(x => x.Attack.Weighting);
				if (ch.Combat == null)
				{
					var nmove = new UnarmedSmashItemAttack(attack.Attack)
						{ Assailant = ch, Target = exit.Exit.Door.Parent, ParentItem = null, NaturalAttack = attack };
					nmove.ResolveMove(null);
					ch.SpendStamina(nmove.StaminaCost);
					ch.AddEffect(
						new CommandDelay(ch, "Smash",
							onExpireAction: () => { ch.Send("You feel as if you could smash something again."); }),
						TimeSpan.FromSeconds(10));
				}
				else
				{
					ch.TakeOrQueueCombatAction(
						SelectedCombatAction.GetEffectSmashItemUnarmed(ch, exit.Exit.Door.Parent, null, attack));
				}

				return;
			}

			ch.RemoveAllEffects(x => x.IsEffectType<BreakDownDoor>());
			return;
		}

		var weaponoption = options.GetWeightedRandom(x => x.Item1.Item2.Sum(y => y.Weighting));
		weaponoption.Item2.ExecuteWholePlan();
		weaponoption.Item2.FinalisePlanNoRestore();

		var option = weaponoption.Item1.Item2.GetWeightedRandom(x => x.Weighting);
		if (ch.Combat == null)
		{
			var nmove = new MeleeWeaponSmashItemAttack(option)
				{ Assailant = ch, Target = exit.Exit.Door.Parent, ParentItem = null, Attack = option };
			nmove.ResolveMove(null);
			ch.SpendStamina(nmove.StaminaCost);
			ch.AddEffect(
				new CommandDelay(ch, "Smash",
					onExpireAction: () => { ch.Send("You feel as if you could smash something again."); }),
				TimeSpan.FromSeconds(10));
		}
		else
		{
			ch.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectSmashItem(ch, exit.Exit.Door.Parent, null,
				weaponoption.Item1.Item1, option));
		}
	}

	protected bool CommandDelaySmash(ICharacter ch, IEnumerable<string> commands)
	{
		if (!commands.Contains("smash"))
		{
			return false;
		}

		return CheckSmash(ch);
	}

	private bool CheckSmash(ICharacter ch)
	{
		if (!ch.AffectedBy<BreakDownDoor>())
		{
			return false;
		}

		if (ch.Effects.Any(x => x.IsBlockingEffect("general") ||
		                        x.GetSubtype<ICommandDelay>()?.IsDelayed("smash") == true))
		{
			return false;
		}

		var exit = ch.EffectsOfType<BreakDownDoor>().First().Exit;

		if (exit.Origin != ch.Location)
		{
			ch.RemoveAllEffects(x => x.IsEffectType<BreakDownDoor>());
			CheckPathingEffect(ch, false);
			return true;
		}

		if (exit.Exit.Door?.IsOpen != false)
		{
			ch.RemoveAllEffects(x => x.IsEffectType<BreakDownDoor>());
			CheckPathingEffect(ch, false);
			return true;
		}

		if (OpenDoors && ch.Body.CanOpen(exit.Exit.Door))
		{
			ch.Body.Open(exit.Exit.Door, null, null);
			ch.RemoveAllEffects(x => x.IsEffectType<BreakDownDoor>());
			CheckPathingEffect(ch, false);
			return true;
		}

		Smash(ch, ch.EffectsOfType<BreakDownDoor>().First().Exit);
		return true;
	}

	protected void ClosedDoor(ICharacter ch, ICellExit exit)
	{
		if (ch.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		CheckPathingEffect(ch, false);
	}

	protected void CheckPathingEffect(ICharacter ch, bool createIfNotPathing)
	{
		if (ch.State.HasFlag(CharacterState.Dead) || !ch.State.IsAble())
		{
			return;
		}

		if (ch.Movement != null || ch.Combat != null ||
		    ch.Effects.Any(x => x.IsBlockingEffect("movement") || x.IsBlockingEffect("combat-engage")))
		{
			return;
		}

		if (!WouldMove(ch))
		{
			return;
		}

		var path = ch.EffectsOfType<FollowingPath>().FirstOrDefault();
		if (path == null)
		{
			if (createIfNotPathing && IsPathingEnabled(ch))
			{
				CreatePathingEffect(ch);
			}

			return;
		}

		FollowPathAction(ch, path);
	}

	protected Func<ICellExit, bool> GetSuitabilityFunction(ICharacter ch, bool requireSuccess = true)
	{
		var keys = UseKeys
			? ch.Body.ExternalItems.SelectNotNull(y => y.GetItemType<IKey>())
			    .Concat(
				    ch.Body.ExternalItems
				      .SelectNotNull(y => y.GetItemType<IContainer>())
				      .Where(y => (y.Parent.GetItemType<IOpenable>()?.IsOpen ?? true) ||
				                  y.Parent.GetItemType<IOpenable>().CanOpen(ch.Body))
				      .SelectMany(y => y.Contents.SelectNotNull(z => z.GetItemType<IKey>()))
			    ).ToList()
			: new List<IKey>();

		return x =>
		{
			if (ch.CanCross(x).Success && ch.CanMove(x))
			{
				return true;
			}

			if (OpenDoors && x.Exit.Door?.IsOpen == false)
			{
				if (x.Exit.Door.CanOpen(ch.Body))
				{
					return true;
				}

				if (UseKeys && x.Exit.Door.Locks.Any())
				{
					if (x.Exit.Door.Locks.All(y => !y.IsLocked || keys.Any(z => y.CanUnlock(ch, z))))
					{
						return true;
					}
				}
			}

			if (SmashLockedDoors && x.Exit.Door != null)
			{
				return true;
			}

			if (!requireSuccess && MoveEvenIfObstructionInWay)
			{
				return true;
			}

			return false;
		};
	}

	protected void CreatePathingEffect(ICharacter ch)
	{
		if (ch.State.HasFlag(CharacterState.Dead) || ch.Corpse != null)
		{
			return;
		}

		var path = GetPath(ch).ToList();
		if (!path.Any())
		{
			return;
		}

		var effect = new FollowingPath(ch, path);
		ch.AddEffect(effect);
		FollowPathAction(ch, effect);
	}

	public void FollowPathAction(ICharacter ch, FollowingPath path)
	{
		path.OpenDoors = OpenDoors;
		path.UseKeys = UseKeys;
		path.SmashLockedDoors = SmashLockedDoors;
		path.UseDoorguards = UseDoorguards;
		path.FollowPathAction();
	}

	protected abstract IEnumerable<ICellExit> GetPath(ICharacter ch);

	protected virtual bool WouldMove(ICharacter ch)
	{
		return true;
	}
}
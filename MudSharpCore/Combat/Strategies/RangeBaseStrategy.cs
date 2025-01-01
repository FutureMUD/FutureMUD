using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.RPG.Checks;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;

namespace MudSharp.Combat.Strategies;

public abstract class RangeBaseStrategy : StrategyBase
{
	#region Implementation of ICombatStrategy

	protected virtual ICombatMove ResponseToChargeToMelee(ChargeToMeleeMove move, ICharacter defender,
		IPerceiver assailant)
	{
		if (defender.CombatTarget == assailant || !defender.MeleeRange)
		{
			var receivingWeapons =
				defender.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>())
				        .Where(
					        x =>
						        x.WeaponType.Attacks.Any(
							        y =>
								        y.MoveType == BuiltInCombatMoveType.ReceiveCharge &&
								        (y.UsabilityProg?.ExecuteBool(defender, move.Assailant, x.Parent) ?? true)))
				        .ToList();
			if (receivingWeapons.Any())
			{
				var weapon = receivingWeapons.WhereMax(x => x.WeaponType.Reach).First();
				return new ReceiveChargeMove(defender, weapon,
					weapon.WeaponType.Attacks.Where(
						      x =>
							      x.MoveType == BuiltInCombatMoveType.ReceiveCharge &&
							      (x.UsabilityProg?.ExecuteBool(defender, move.Assailant, weapon.Parent) ?? true))
					      .GetWeightedRandom(x => x.Weighting), assailant as ICharacter);
			}
		}

		return null;
	}

	protected virtual ICombatMove ResponseToMoveToMelee(MoveToMeleeMove move, ICharacter defender,
		IPerceiver assailant)
	{
		return null;
	}

	protected virtual ICombatMove ResponseToFireAndAdvance(FireAndAdvanceToMeleeMove move, ICharacter defender,
		IPerceiver assailant)
	{
		return null;
	}

	protected virtual ICombatMove ResponseToRangedWeaponAttack(IRangedWeaponAttackMove move, ICharacter defender)
	{
		if (defender.Cover != null)
		{
			return new HelplessDefenseMove { Assailant = defender };
		}

		var assailant = move.Assailant;

		var shield =
			defender.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IShield>())
					.FirstMax(x => x.ShieldType.BlockBonus);
		var shieldBonus = shield != null
			? BlockMove.GetBlockBonus(move, defender.Body.AlignmentOf(shield.Parent), shield)
			: 0;
		var finalBlockDifficulty = move.Weapon.BaseBlockDifficulty.ApplyBonus(shieldBonus);
		var finalDodgeDifficulty = move.Weapon.BaseDodgeDifficulty;

		var finalBlockTargetNumber = defender.Gameworld.GetCheck(CheckType.BlockCheck).TargetNumber(defender, finalBlockDifficulty, shield?.ShieldType.BlockTrait, assailant);
		var finalDodgeTargetNumber = defender.Gameworld.GetCheck(CheckType.DodgeCheck).TargetNumber(defender, finalDodgeDifficulty, null, assailant);

		var canBlock = shield is not null && move.Weapon.BaseBlockDifficulty != Difficulty.Impossible && defender.CanSpendStamina(BlockMove.MoveStaminaCost(assailant, defender, shield));
		var canDodge = move.Weapon.BaseDodgeDifficulty != Difficulty.Impossible && defender.CanSpendStamina(DodgeRangeMove.MoveStaminaCost(defender));

		switch (defender.PreferredDefenseType)
		{
			case DefenseType.Block:
				if (canBlock)
				{
					return new BlockMove { Assailant = defender, Shield = shield, PrimaryTarget = assailant };
				}
				break;
			case DefenseType.Dodge:
				if (canDodge)
				{
					return new DodgeRangeMove { Assailant = defender };
				}
				break;
		}

		// Otherwise fall back to default handling
		// Calculate the success chance of the various defense types
		var availableDefenses = new List<(DefenseType Defense, double Chance)>(3);
		if (canBlock)
		{
			availableDefenses.Add((DefenseType.Block, finalBlockTargetNumber));
		}
		if (canDodge)
		{
			availableDefenses.Add((DefenseType.Dodge, finalDodgeTargetNumber));
		}

		if (availableDefenses.Count == 0)
		{
			return new HelplessDefenseMove
			{
				Assailant = defender
			};
		}

		// Act based on the best one
		switch (availableDefenses.OrderByDescending(x => x.Chance).First().Defense)
		{
			case DefenseType.Block:
				return new BlockMove { Assailant = defender, Shield = shield, PrimaryTarget = assailant };
			case DefenseType.Dodge:
				return new DodgeRangeMove { Assailant = defender };
		}

		return new HelplessDefenseMove
		{
			Assailant = defender
		};
	}

	public override ICombatMove ResponseToMove(ICombatMove move, IPerceiver defender, IPerceiver assailant)
	{
		if (!(defender is ICharacter defenseCharacter))
		{
			return null; // TODO - item specific helpless defense
		}

		if (move is ScreechAttackMove)
		{
			return new HelplessDefenseMove { Assailant = defenseCharacter };
		}

		var moveAsWeaponAttack = move as IWeaponAttackMove;
		var moveAsRangedAttack = move as IRangedWeaponAttackMove;

		if (!defenseCharacter.Race.CombatSettings.CanDefend)
		{
			return new HelplessDefenseMove { Assailant = defenseCharacter };
		}

		if (defenseCharacter.State.HasFlag(CharacterState.Sleeping) ||
		    defenseCharacter.State.HasFlag(CharacterState.Unconscious) ||
		    defenseCharacter.State.HasFlag(CharacterState.Paralysed) ||
		    defenseCharacter.Effects.Any(x => x.IsBlockingEffect("general")))
		{
			return moveAsWeaponAttack != null || moveAsRangedAttack != null
				? new HelplessDefenseMove { Assailant = defenseCharacter }
				: null;
		}

		if (move is IRangedWeaponAttackMove rwam)
		{
			return ResponseToRangedWeaponAttack(rwam, defenseCharacter);
		}

		if (move is ChargeToMeleeMove chargeToMeleeMove)
		{
			return ResponseToChargeToMelee(chargeToMeleeMove, defenseCharacter, assailant);
		}

		if (move is MoveToMeleeMove moveToMeleeMove)
		{
			return ResponseToMoveToMelee(moveToMeleeMove, defenseCharacter, assailant);
		}

		if (move is FireAndAdvanceToMeleeMove fireAndAdvanceMove)
		{
			return ResponseToFireAndAdvance(fireAndAdvanceMove, defenseCharacter, assailant);
		}

		return null;
	}

	private ICombatMove DoRangedWeaponAttack(ICharacter combatant)
	{
		var rangedWeapons =
			GetReadyRangedWeapons(combatant).ToList();
		if ((combatant.CombatSettings.RangedManagement == AutomaticRangedSettings.FullyAutomatic ||
		     (combatant.CombatSettings.RangedManagement == AutomaticRangedSettings.ContinueFiringOnly &&
		      combatant.EffectsOfType<OpenedFire>().Any())) &&
		    (rangedWeapons.Any() || GetNotReadyButLoadableWeapons(combatant).Any()))
		{
			return AttemptUseRangedWeapon(combatant);
		}

		if (combatant.CombatSettings.FallbackToUnarmedIfNoWeapon)
		{
			return AttemptUseRangedNaturalAttack(combatant);
		}

		return null;
	}

	protected virtual ICombatMove HandleGeneralAttacks(ICharacter combatant)
	{
		var roll = Constants.Random.NextDouble();
		if (combatant.CombatSettings.WeaponUsePercentage > 0 &&
		    roll <= combatant.CombatSettings.WeaponUsePercentage)
		{
			var move = DoRangedWeaponAttack(combatant);
			if (move != null)
			{
				return move;
			}

			if (combatant.CombatSettings.MoveToMeleeIfCannotEngageInRangedCombat &&
			    !GetReadyRangedWeapons(combatant).Any() &&
			    !GetNotReadyButLoadableWeapons(combatant).Any() &&
			    AttemptGetRangedWeapon(combatant) == null
			   )
			{
				CannotMakeAValidRangedAttack(combatant);
				return null;
			}
		}

		roll -= combatant.CombatSettings.WeaponUsePercentage;
		if (combatant.CombatSettings.NaturalWeaponPercentage > 0.0 &&
		    roll <= combatant.CombatSettings.NaturalWeaponPercentage)
		{
			return AttemptUseRangedNaturalAttack(combatant);
		}

		roll -= combatant.CombatSettings.NaturalWeaponPercentage;
		if (combatant.CombatSettings.MagicUsePercentage > 0 && roll <= combatant.CombatSettings.MagicUsePercentage)
		{
			return AttemptUseMagic(combatant);
		}

		roll -= combatant.CombatSettings.MagicUsePercentage;
		if (combatant.CombatSettings.PsychicUsePercentage > 0 && roll <= combatant.CombatSettings.PsychicUsePercentage)
		{
			return AttemptUsePsychicAbility(combatant);
		}

		return null;
	}

	protected override ICombatMove HandleAttacks(IPerceiver combatant)
	{
		if (!(combatant is ICharacter ch))
		{
			return null;
		}

		if (!WillAttackTarget(ch))
		{
			return null;
		}

		return HandleGeneralAttacks(ch);
	}

	protected virtual ICombatMove HandleMoveToMelee(ICharacter ch)
	{
		if (ch.CombatSettings.MovementManagement == AutomaticMovementSettings.FullyManual)
		{
			return null;
		}

		return null;
	}

	protected override ICombatMove HandleObligatoryCombatMoves(IPerceiver combatant)
	{
		var result = base.HandleObligatoryCombatMoves(combatant);
		if (result != null)
		{
			return result;
		}

		if (combatant is ICharacter ch && !combatant.EffectsOfType<PassiveInterdiction>().Any() &&
		    !combatant.EffectsOfType<Rescue>().Any())
		{
			var guard = combatant.EffectsOfType<IGuardCharacterEffect>().FirstOrDefault();
			if (guard == null || !guard.Interdicting || !guard.Targets.Any())
			{
				return null;
			}

			var targetNeedsRescuing =
				guard?.Targets.FirstOrDefault(guardedCh =>
					guardedCh.Location == ch.Location &&
					ch.CombatTarget != guardedCh &&
					ch.Combat.Combatants.Any(
						combatCh => combatCh != ch &&
						            combatCh.Aim?.Target == guardedCh &&
						            ch.CanSee(combatCh)));
			if (targetNeedsRescuing != null)
			{
				return new InterposeMove(ch, targetNeedsRescuing);
			}
		}

		return null;
	}

	#endregion

	protected void CannotMakeAValidRangedAttack(ICharacter ch)
	{
		if (ch.CombatSettings.MoveToMeleeIfCannotEngageInRangedCombat)
		{
			ch.OutputHandler.Send(
				"You realise that there is no way for you to engage your opponent at range, and resolve to get into the melee.");
			ch.CombatStrategyMode = CombatStrategyMode.FullAdvance;
		}
	}

	protected virtual ICombatMove HandleChangeLayer(ICharacter ch)
	{
		if (ch.RoomLayer.IsLowerThan(ch.CombatTarget.RoomLayer))
		{
			if (ch.Location.IsUnderwaterLayer(ch.RoomLayer) && ((ISwim)ch).CanAscend().Truth)
			{
				return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.SwimUp);
			}

			if (ch.CanFly().Truth && ch.PositionState != PositionFlying.Instance)
			{
				return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.Fly);
			}

			if (ch.PositionState == PositionFlying.Instance && ((IFly)ch).CanAscend().Truth)
			{
				return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.FlyUp);
			}

			if (ch.CanClimbUp().Truth)
			{
				return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.ClimbUp);
			}
		}
		else
		{
			if (ch.Location.IsSwimmingLayer(ch.RoomLayer) && ((ISwim)ch).CanDive().Truth)
			{
				return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.SwimDown);
			}

			if (ch.CanLand().Truth && ch.RoomLayer.IsLowerThan(RoomLayer.InAir) &&
			    ch.PositionState == PositionFlying.Instance)
			{
				return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.Land);
			}

			if (ch.PositionState == PositionFlying.Instance && ((IFly)ch).CanDive().Truth)
			{
				return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.FlyDown);
			}

			if (ch.CanClimbDown().Truth)
			{
				return new LayerChangeMove(ch, LayerChangeMove.DesiredLayerChange.ClimbDown);
			}
		}

		return null;
	}

	public Func<ICellExit, bool> GetPathFunction(ICharacter ch)
	{
		return exit =>
		{
			if (exit.Exit.Door?.IsOpen == false)
			{
				if (!ch.Body.CouldOpen(exit.Exit.Door))
				{
					return false;
				}
			}

			var (transition, layer) = exit.MovementTransition(ch);
			switch (transition)
			{
				case CellMovementTransition.NoViableTransition:
				case CellMovementTransition.FallExit:
					return false;
				case CellMovementTransition.SwimOnly:
					return ch.Gameworld.GetCheck(CheckType.SwimStayAfloatCheck).WouldBeAbjectFailure(ch);
			}

			return true;
		};
	}
}
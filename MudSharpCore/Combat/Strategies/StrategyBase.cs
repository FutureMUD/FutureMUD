using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Combat.Strategies;

public abstract class StrategyBase : ICombatStrategy
{
	protected virtual IEnumerable<IRangedWeapon> GetNotReadyButLoadableWeapons(ICharacter shooter)
	{
		return
			shooter.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>())
			       .Where(x =>
				       !x.ReadyToFire && (
					       (!x.IsLoaded && x.CanLoad(shooter, true)) || (!x.IsReadied && x.CanReady(shooter))));
	}

	protected virtual IEnumerable<IRangedWeapon> GetReadyRangedWeapons(ICharacter shooter)
	{
		return
			shooter.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IRangedWeapon>())
			       .Where(x => x.ReadyToFire && shooter.CanSpendStamina(x.WeaponType.StaminaToFire))
			       .OrderBy(x => x.Parent.IsItemType<IMeleeWeapon>());
	}

	/// <summary>
	/// Checks to see whether there are any core reasons why combat moves cannot proceed.
	/// </summary>
	/// <param name="combatant">The combatant being checked</param>
	/// <returns>true if the combatant should not choose a move</returns>
	protected bool CheckCoreCombatStoppers(IPerceiver combatant)
	{
		if (combatant.Effects.Any(x => x.IsBlockingEffect("combat")))
		{
			return true;
		}

		if (combatant.CombatSettings == null)
		{
			return true;
		}

		if (combatant is ICharacter ch)
		{
			if (ch.Movement != null)
			{
				return true;
			}

			if (ch.State.HasFlag(CharacterState.Unconscious))
			{
				return true;
			}

			if (ch.State.HasFlag(CharacterState.Paralysed))
			{
				return true;
			}
		}

		return false;
	}

	protected virtual bool ShouldCharacterStand(ICharacter ch)
	{
		if (!ch.PositionState.In(PositionFlying.Instance, PositionSwimming.Instance, PositionClimbing.Instance,
			    PositionStanding.Instance) &&
		    ch.CanMovePosition(PositionStanding.Instance, PositionModifier.None, null, false) &&
		    ch.CanSpendStamina(StandMove.StandStaminaCost(ch.Gameworld))
		   )
		{
			return true;
		}

		return false;
	}

	public virtual bool WillAttack(ICharacter ch, ICharacter tch)
	{
		if (!ch.CombatSettings.AttackHelpless)
		{
			if (tch.IsHelpless)
			{
				return false;
			}
		}

		if (!ch.CombatSettings.AttackCriticallyInjured && tch.HealthStrategy.IsCriticallyInjured(tch)
		   )
		{
			return false;
		}

		if (!ch.CombatSettings.AttackDisarmed)
		{
			if (tch.Race.CombatSettings.CanUseWeapons &&
				!tch.CombatSettings.FallbackToUnarmedIfNoWeapon &&
			    !tch.Body.HeldOrWieldedItems.Any(x =>
				    x.IsItemType<IMeleeWeapon>() || x.IsItemType<IRangedWeapon>()))
			{
				return false;
			}
		}

		return true;
	}


	public virtual string WhyWontAttack(ICharacter ch, ICharacter tch)
	{
		if (!ch.CombatSettings.AttackHelpless)
		{
			if (tch.IsHelpless)
			{
				return $"because {tch.ApparentGender(ch).Subjective()} is helpless.";
			}
		}

		if (!ch.CombatSettings.AttackCriticallyInjured && tch.HealthStrategy.IsCriticallyInjured(tch)
		   )
		{
			return $"because {tch.ApparentGender(ch).Subjective()} is critically injured.";
		}

		if (!ch.CombatSettings.AttackDisarmed)
		{
			if (tch.Race.CombatSettings.CanUseWeapons &&
			    !tch.CombatSettings.FallbackToUnarmedIfNoWeapon &&
			    !tch.Body.HeldOrWieldedItems.Any(x =>
				    x.IsItemType<IMeleeWeapon>() || x.IsItemType<IRangedWeapon>()))
			{
				return $"because {tch.ApparentGender(ch).Subjective()} is disarmed.";
			}
		}

		return string.Empty;
	}

	public string WhyWontAttack(ICharacter ch)
	{
		if (!ch.Race.CombatSettings.CanAttack)
		{
			return "You won't attack because your race cannot attack.";
		}

		if (ch.CombatTarget == null)
		{
			return "You won't attack because you don't have a combat target.";
		}

		if (!ch.Race.CombatSettings.CanUseWeapons && ch.CombatSettings.WeaponUsePercentage >= 1.0 &&
		    !ch.CombatSettings.FallbackToUnarmedIfNoWeapon)
		{
			return "You won't attack because you won't fallback to an unarmed attack and don't have a weapon.";
		}

		if (ch.Body.EffectsOfType<IPacifismEffect>().Any(x => x.IsSuperPeaceful))
		{
			return "You won't attack because you are way too peaceful.";
		}

		if (ch.CombatTarget is ICharacter tch)
		{
			if (!WillAttack(ch, tch))
			{
				return "You won't attack because you won't attack your current target.";
			}
		}

		return string.Empty;
	}

	protected bool WillAttackTarget(ICharacter ch)
	{
		if (!ch.Race.CombatSettings.CanAttack)
		{
			return false;
		}

		if (ch.CombatTarget == null)
		{
			return false;
		}

		if (!ch.Race.CombatSettings.CanUseWeapons && ch.CombatSettings.WeaponUsePercentage >= 1.0 &&
		    !ch.CombatSettings.FallbackToUnarmedIfNoWeapon)
		{
			return false;
		}

		if (ch.Body.EffectsOfType<IPacifismEffect>().Any(x => x.IsSuperPeaceful))
		{
			return false;
		}

		if (ch.CombatTarget is ICharacter tch)
		{
			if (!WillAttack(ch, tch))
			{
				return false;
			}
		}


		return true;
	}

	#region Helper Methods

	protected bool IsUseableWeapon(ICharacter ch, IMeleeWeapon weapon)
	{
		if (weapon == null)
		{
			return false;
		}

		if (!ch.CombatSettings.ClassificationsAllowed.Contains(weapon.Classification))
		{
			return false;
		}

		if (ch.Combat.Friendly &&
		    !weapon.Classification.In(WeaponClassification.Training, WeaponClassification.NonLethal))
		{
			return false;
		}

		var options = weapon.WeaponType
		                    .UseableHandednessOptions(ch, weapon.Parent, ch.CombatTarget,
			                    CombatExtensions.StandardMeleeWeaponAttacks)
		                    .ToList();

		if (options.Any(x => x == AttackHandednessOptions.Any))
		{
			return ch.Body.CanWield(weapon.Parent, ItemCanWieldFlags.IgnoreFreeHands);
		}

		if (options.Any(x => x == AttackHandednessOptions.SwordAndBoardOnly))
		{
			if (ch.Body.CanWield(weapon.Parent, ItemCanWieldFlags.IgnoreFreeHands) &&
			    ch.Body.WieldedItems.Any(x => x.IsItemType<IShield>()))
			{
				return true;
			}
		}

		if (options.Any(x => x == AttackHandednessOptions.OneHandedOnly))
		{
			if (ch.Body.CanWield(weapon.Parent,
				    ItemCanWieldFlags.IgnoreFreeHands | ItemCanWieldFlags.RequireOneHand))
			{
				return true;
			}
		}

		if (options.Any(x => x == AttackHandednessOptions.TwoHandedOnly))
		{
			if (ch.Body.CanWield(weapon.Parent,
				    ItemCanWieldFlags.IgnoreFreeHands | ItemCanWieldFlags.RequireTwoHands))
			{
				return true;
			}
		}
		// TODO - dual wield

		return false;
	}

	protected bool IsUseableWeapon(ICharacter ch, IRangedWeapon weapon)
	{
		return weapon != null && ch.Body.CanWield(weapon.Parent, ItemCanWieldFlags.IgnoreFreeHands) &&
		       ch.CombatSettings.ClassificationsAllowed.Contains(weapon.Classification) &&
		       (!ch.Combat.Friendly ||
		        weapon.Classification.In(WeaponClassification.Training, WeaponClassification.NonLethal)) &&
		       (weapon.ReadyToFire || (weapon.IsLoaded && (weapon.IsReadied || weapon.CanReady(ch))) ||
		        weapon.CanLoad(ch, true));
	}

	protected bool IsUseableWeapon(ICharacter ch, IShield shield)
	{
		return shield != null &&
		       ch.Body.CanWield(shield.Parent, ItemCanWieldFlags.IgnoreFreeHands);
	}

	#endregion

	protected virtual ICombatMove HandleObligatoryCombatMoves(IPerceiver combatant)
	{
		if (combatant is ICharacter ch)
		{
			// Sleeping characters can't do anything until they wake up
			if (ch.State.HasFlag(CharacterState.Sleeping))
			{
				return new WakeMove { Assailant = ch };
			}

			// Characters who have nominated to take a manual action should do that first
			var manualAction = ch.EffectsOfType<SelectedCombatAction>().FirstOrDefault();
			if (manualAction != null)
			{
				ch.RemoveEffect(manualAction);
				return manualAction.GetMove(ch);
			}

			// Characters who are not manually managing their positions should see if they need to stand.
			if (!ch.CombatSettings.ManualPositionManagement)
			{
				if (ShouldCharacterStand(ch))
				{
					return new StandMove { Assailant = ch };
				}
			}

			// Characters who are guarding someone should be set to rescue if their ward needs a rescue and they are not already rescuing
			if (!ch.EffectsOfType<Rescue>().Any())
			{
				var guardEffect = ch.EffectsOfType<IGuardCharacterEffect>().FirstOrDefault();
				var targetNeedsRescuing =
					guardEffect?.Targets.FirstOrDefault(guardedCh =>
						guardedCh.Location == ch.Location &&
						ch.CombatTarget != guardedCh &&
						ch.Combat.Combatants.Any(
							combatCh => combatCh != ch &&
							            combatCh.CombatTarget == guardedCh &&
							            combatCh.Location == guardedCh.Location &&
							            combatCh.MeleeRange));
				if (targetNeedsRescuing != null)
				{
					ch.AddEffect(new Rescue(ch, targetNeedsRescuing));
				}
			}

			// Characters who are trying to rescue someone should do that if they can (must be standing)
			var rescueEffect = ch.EffectsOfType<Rescue>().FirstOrDefault();
			if (rescueEffect != null &&
			    ch.CombatTarget != rescueEffect.RescueTarget &&
			    ch.PositionState.CompareTo(PositionStanding.Instance) == PositionHeightComparison.Equivalent
			   )
			{
				// To rescue, we must find a combatant that isn't ourself or our rescue target, that is attacking
				// our rescue target, and is in melee combat. 
				if (ch.Combat.Combatants.Any(
					    x => x != ch &&
					         x.CombatTarget == rescueEffect.RescueTarget &&
					         x.Location == rescueEffect.RescueTarget.Location &&
					         x.Location == ch.Location &&
					         x.MeleeRange))
				{
					return new RescueMove { Assailant = ch, Target = rescueEffect.RescueTarget };
				}

				if (ch.Combat.Combatants.Any(x => x != ch &&
				                                  x.CombatTarget == rescueEffect.RescueTarget))
					// Explain why we're not attempting rescues
				{
					ch.OutputHandler.Send(
						$"There are no nearby enemies engaged in melee combat with {rescueEffect.RescueTarget.HowSeen(ch)} to rescue {rescueEffect.RescueTarget.ApparentGender(ch).Objective()} from.");
				}
			}
		}

		return null;
	}

	protected virtual ICombatMove HandleClinchBreaking(ICharacter ch, bool canMove)
	{
		var clinchers =
			ch.Combat.Combatants.OfType<ICharacter>()
			  .Where(
				  x =>
					  (x.CombatTarget == ch && x.EffectsOfType<ClinchEffect>().Any(y => y.Target == ch)) ||
					  ch.EffectsOfType<ClinchEffect>().Any(y => y.Target == x))
			  .ToList();
		if (!clinchers.Any())
		{
			return null;
		}

		var breakers = ch.Race.UsableNaturalWeaponAttacks(ch, clinchers.First(), false,
			BuiltInCombatMoveType.StaggeringBlowClinch, BuiltInCombatMoveType.UnbalancingBlowClinch).ToList();
		var wbreakers = ch.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>())
		                  .Select(x => Tuple.Create(x,
			                  x.WeaponType.UsableAttacks(ch, x.Parent, clinchers.First(), x.HandednessForWeapon(ch),
				                  false,
				                  BuiltInCombatMoveType.StaggeringBlow, BuiltInCombatMoveType.UnbalancingBlow)))
		                  .Where(x => x.Item2.Any())
		                  .ToList();

		// Prefer armed staggering or unbalancing blows first
		if ((ch.CombatSettings.PreferToFightArmed || !breakers.Any()) && wbreakers.Any(x => x.Item2.Any()) &&
		    !ch.CombatSettings.PreferNonContactClinchBreaking)
		{
			var weapon = wbreakers.GetRandomElement();
			return CombatMoveFactory.CreateWeaponAttack(ch, weapon.Item1,
				weapon.Item2.GetWeightedRandom(x => x.Weighting), clinchers.First());
		}

		// Secondly, prefer unarmed staggering or unbalancing blows
		if (breakers.Any() && !ch.CombatSettings.PreferNonContactClinchBreaking)
		{
			return CombatMoveFactory.CreateNaturalWeaponAttack(ch, breakers.GetWeightedRandom(x => x.Attack.Weighting),
				clinchers.First());
		}

		// Finally, use BreakClinchMove as a fall back
		if (ch.CanSpendStamina(BreakClinchMove.MoveStaminaCost(ch)) && canMove)
		{
			return new BreakClinchMove(ch, clinchers.GetRandomElement());
		}

		return null;
	}

	protected virtual ICombatMove HandleCombatMovement(IPerceiver combatant)
	{
		if (combatant is ICharacter ch)
		{
			ICombatMove move;
			var canMove = ch.CanMove(CanMoveFlags.IgnoreCancellableActionBlockers);

			// Everyone tries to break free of clinch first if they can
			if ((move = HandleClinchBreaking(ch, canMove)) != null)
			{
				return move;
			}
		}

		return null;
	}

	protected virtual Func<IGameItem, double> MeleeWeaponFitnessFunction(ICharacter ch)
	{
		double InternalFunction(IGameItem item)
		{
			var weapon = item.GetItemType<IMeleeWeapon>();
			if (weapon == null)
			{
				return 0.0;
			}

			if (item.IsItemType<IShield>())
			{
				return double.Epsilon;
			}

			if (item.IsItemType<IRangedWeapon>())
			{
				if (weapon.Classification.HasFlag(WeaponClassification.Improvised) ||
				    weapon.Classification.HasFlag(WeaponClassification.Training))
				{
					return double.Epsilon;
				}

				if (!ch.CombatSettings.PreferredRangedMode.IsRangedStartDesiringStrategy())
				{
					return double.Epsilon;
				}
			}

			if (ch.CombatSettings.PreferFavouriteWeapon &&
			    ch.EffectsOfType<ICombatGetItemEffect>().Any(x => x.TargetItem == item))
			{
				return double.MaxValue;
			}

			return (ch.GetTrait(weapon.WeaponType.AttackTrait)?.Value ?? 0.1) * (int)item.Quality;
		}

		return InternalFunction;
	}

	protected virtual Func<IGameItem, double> RangedWeaponFitnessFunction(ICharacter ch)
	{
		double InternalFunction(IGameItem item)
		{
			var weapon = item.GetItemType<IRangedWeapon>();
			if (weapon == null)
			{
				return 0.0;
			}

			if (ch.CombatSettings.PreferFavouriteWeapon &&
			    ch.EffectsOfType<ICombatGetItemEffect>().Any(x => x.TargetItem == item))
			{
				return double.MaxValue;
			}

			return (ch.GetTrait(weapon.WeaponType.FireTrait)?.Value ?? 0.1) * (int)item.Quality;
		}

		return InternalFunction;
	}

	protected virtual Func<IGameItem, double> ShieldFitnessFunction(ICharacter ch)
	{
		double InternalFunction(IGameItem item)
		{
			var weapon = item.GetItemType<IShield>();
			if (weapon == null)
			{
				return 0.0;
			}

			if (ch.CombatSettings.PreferFavouriteWeapon &&
			    ch.EffectsOfType<ICombatGetItemEffect>().Any(x => x.TargetItem == item))
			{
				return double.MaxValue;
			}

			return (ch.GetTrait(weapon.ShieldType.BlockTrait)?.Value ?? 0.1) * (int)item.Quality;
		}

		return InternalFunction;
	}

	private AttackHandednessOptions EvaluatePreferredHandedness(ICharacter ch, IGameItem item)
	{
		if (!(item?.GetItemType<IMeleeWeapon>() is IMeleeWeapon mw))
		{
			return AttackHandednessOptions.Any;
		}

		switch (ch.CombatSettings.PreferredWeaponSetup)
		{
			case AttackHandednessOptions.SwordAndBoardOnly:
			case AttackHandednessOptions.DualWieldOnly:
			case AttackHandednessOptions.OneHandedOnly:
				return AttackHandednessOptions.OneHandedOnly;
			case AttackHandednessOptions.TwoHandedOnly:
				return AttackHandednessOptions.TwoHandedOnly;
		}

		if (mw.WeaponType
		      .UseableHandednessOptions(ch, item, ch.CombatTarget, CombatExtensions.StandardMeleeWeaponAttacks)
		      .Any(x => x == AttackHandednessOptions.Any || x == AttackHandednessOptions.OneHandedOnly))
		{
			return AttackHandednessOptions.OneHandedOnly;
		}

		return AttackHandednessOptions.TwoHandedOnly;
	}

	protected ICombatMove AttemptGetWeapon(ICharacter ch)
	{
		var plan = new InventoryPlanTemplate(ch.Gameworld, new[]
		{
			new InventoryPlanPhaseTemplate(1, new[]
			{
				new InventoryPlanActionWield(ch.Gameworld, 0, 0,
					item => IsUseableWeapon(ch, item.GetItemType<IMeleeWeapon>()), null, EvaluatePreferredHandedness)
				{
					PrimaryItemFitnessScorer = MeleeWeaponFitnessFunction(ch),
					OriginalReference = "weapon",
					ItemsAlreadyInPlaceOverrideFitnessScore = false,
					ItemsAlreadyInPlaceMultiplier = 1.5
				}
			})
		})
		{
			Options = ch.CombatSettings.InventoryManagement == AutomaticInventorySettings.FullyAutomatic
				? InventoryPlanOptions.None
				: InventoryPlanOptions.DoNotClearHands
		}.CreatePlan(ch);
		if (plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		{
			return new InventoryPlanMove
			{
				Assailant = ch,
				Plan = plan,
				AfterPlanActions = result =>
				{
					if (result.OriginalReference?.ToString().EqualTo("weapon") != true)
					{
						return;
					}

					result.PrimaryTarget.RemoveAllEffects(x => x is CombatNoGetEffect);
					ch.RemoveAllEffects(x => (x as ICombatGetItemEffect)?.TargetItem == result.PrimaryTarget);
				}
			};
		}

		return null;
	}

	protected virtual ICombatMove AttemptGetRangedWeapon(ICharacter ch)
	{
		return BaseAttemptGetRangedWeapon(ch);
	}

	protected ICombatMove BaseAttemptGetRangedWeapon(ICharacter ch)
	{
		var plan = new InventoryPlanTemplate(ch.Gameworld, new[]
		{
			new InventoryPlanPhaseTemplate(1, new[]
			{
				new InventoryPlanActionWield(ch.Gameworld, 0, 0,
					item => IsUseableWeapon(ch, item.GetItemType<IRangedWeapon>()), null)
				{
					PrimaryItemFitnessScorer = RangedWeaponFitnessFunction(ch),
					ItemsAlreadyInPlaceOverrideFitnessScore = false,
					ItemsAlreadyInPlaceMultiplier = 1.5
				}
			})
		})
		{
			Options = ch.CombatSettings.InventoryManagement == AutomaticInventorySettings.FullyAutomatic
				? InventoryPlanOptions.None
				: InventoryPlanOptions.DoNotClearHands
		}.CreatePlan(ch);
		if (plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		{
			return new InventoryPlanMove
			{
				Assailant = ch,
				Plan = plan,
				AfterPlanActions = result =>
				{
					if (!result.ActionState.In(DesiredItemState.Wielded, DesiredItemState.WieldedOneHandedOnly,
						    DesiredItemState.WieldedTwoHandedOnly))
					{
						return;
					}

					result.PrimaryTarget.RemoveAllEffects(x => x is CombatNoGetEffect);
					ch.RemoveAllEffects(x => (x as ICombatGetItemEffect)?.TargetItem == result.PrimaryTarget);
				}
			};
		}

		return null;
	}

	protected ICombatMove AttemptGetShield(ICharacter ch)
	{
		var plan = new InventoryPlanTemplate(ch.Gameworld, new[]
		{
			new InventoryPlanPhaseTemplate(1, new[]
			{
				new InventoryPlanActionWield(ch.Gameworld, 0, 0,
					item => IsUseableWeapon(ch, item.GetItemType<IShield>()), null)
				{
					PrimaryItemFitnessScorer = ShieldFitnessFunction(ch),
					ItemsAlreadyInPlaceOverrideFitnessScore = false,
					ItemsAlreadyInPlaceMultiplier = 1.5
				}
			})
		})
		{
			Options = ch.CombatSettings.InventoryManagement == AutomaticInventorySettings.FullyAutomatic
				? InventoryPlanOptions.None
				: InventoryPlanOptions.DoNotClearHands
		}.CreatePlan(ch);
		if (plan.PlanIsFeasible() == InventoryPlanFeasibility.Feasible)
		{
			return new InventoryPlanMove
			{
				Assailant = ch,
				Plan = plan,
				AfterPlanActions = result =>
				{
					if (!result.ActionState.In(DesiredItemState.Wielded, DesiredItemState.WieldedOneHandedOnly,
						    DesiredItemState.WieldedTwoHandedOnly))
					{
						return;
					}

					result.PrimaryTarget.RemoveAllEffects(x => x is CombatNoGetEffect);
					ch.RemoveAllEffects(x => (x as ICombatGetItemEffect)?.TargetItem == result.PrimaryTarget);
				}
			};
		}

		return null;
	}

	protected virtual ICombatMove AttemptUseRangedWeapon(ICharacter combatant)
	{
		if (combatant.Aim == null)
		{
			var readyRangedWeapons =
				GetReadyRangedWeapons(combatant).ToList();
			if (readyRangedWeapons.Any())
			{
				if (combatant.CombatTarget.Location == combatant.Location)
				{
					combatant.Aim = new AimInformation(combatant.CombatTarget, combatant, Enumerable.Empty<ICellExit>(),
						readyRangedWeapons.FirstMax(x => x.Parent.Quality));
				}
				else
				{
					var path = combatant.PathBetween(combatant.CombatTarget,
						readyRangedWeapons.Max(x => x.WeaponType.DefaultRangeInRooms),
						false, false, true).ToList();
					var weapon = readyRangedWeapons.Where(x => x.WeaponType.DefaultRangeInRooms >= path.Count)
					                               .FirstMax(x => x.Parent.Quality);
					if (path.Any() && weapon != null)
					{
						combatant.Aim = new AimInformation(combatant.CombatTarget, combatant, path, weapon);
					}
				}
			}
		}

		if (combatant.Aim?.Weapon.ReadyToFire == true)
		{
			if (combatant.CanSpendStamina(combatant.Aim.Weapon.WeaponType.StaminaToFire) &&
			    combatant.Aim.AimPercentage >= combatant.CombatSettings.RequiredMinimumAim)
			{
				return new RangedWeaponAttackMove(combatant, combatant.CombatTarget as ICharacter,
					combatant.Aim.Weapon);
			}

			return new AimRangedWeaponMove(combatant, combatant.CombatTarget as ICharacter, combatant.Aim.Weapon);
		}

		var nonReadyWeapons = GetNotReadyButLoadableWeapons(combatant).ToList();
		if (nonReadyWeapons.Any())
		{
			if (combatant.EffectsOfType<FirearmNeedsReloading>().Any(x => nonReadyWeapons.Contains(x.Firearm)))
			{
				return new ReloadFirearmMove
				{
					Assailant = combatant,
					Weapon = combatant.EffectsOfType<FirearmNeedsReloading>()
					                  .First(x => nonReadyWeapons.Contains(x.Firearm)).Firearm
				};
			}

			if (nonReadyWeapons.Any(x =>
				    x.IsLoaded &&
				    combatant.CanSpendStamina(x.WeaponType.StaminaPerLoadStage * 2 + x.WeaponType.StaminaToFire)))
				//Make sure we have the stamina for 2 Ready tick + firing of the weapon before readying it
			{
				return new ReadyRangedWeaponMove
				{
					Assailant = combatant,
					Weapon = nonReadyWeapons
						.First(x => combatant.CanSpendStamina(x.WeaponType.StaminaPerLoadStage * 2 +
						                                      x.WeaponType.StaminaToFire))
				};
			}

			if (nonReadyWeapons.Any(x => !x.IsLoaded))
			{
				return new LoadRangedWeaponMove
					{ Assailant = combatant, Weapon = nonReadyWeapons.First(x => !x.IsLoaded) };
			}
		}

		return null;
	}

	protected virtual ICombatMove AttemptUseRangedNaturalAttack(ICharacter combatant)
	{
		var possibleAttacks =
			combatant.Race
			         .UsableNaturalWeaponAttacks(combatant, combatant.CombatTarget, false,
				         BuiltInCombatMoveType.ScreechAttack).ToList();
		var attacks = possibleAttacks
		              .Where(x => combatant.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(combatant, x.Attack)))
		              .ToList();
		if (!attacks.Any())
		{
			return null;
		}

		var preferredAttacks =
			attacks.Where(x => x.Attack.Intentions.HasFlag(combatant.CombatSettings.PreferredIntentions)).ToList();
		if (preferredAttacks.Any() && Dice.Roll(1, 2) == 1)
		{
			attacks = preferredAttacks;
		}


		var attack = attacks.GetWeightedRandom(x => x.Attack.Weighting);
		if (attack == null)
		{
			return null;
		}

		if (combatant.CombatTarget is ICharacter charTarget)
		{
			return CombatMoveFactory.CreateNaturalWeaponAttack(combatant, attack, charTarget);
		}

		if (combatant.CombatTarget is IGameItem itemTarget)
		{
			return CombatMoveFactory.CreateNaturalWeaponAttack(combatant, attack, itemTarget);
		}

		throw new NotImplementedException(
			"Unimplemented Combatant type in StrategyBase.AttemptUseNaturalRangedAttack - " +
			combatant.CombatTarget.GetType().FullName);
	}

	protected virtual ICombatMove AttemptUseMagic(ICharacter combatant)
	{
		// TODO
		return null;
	}

	protected virtual ICombatMove AttemptUsePsychicAbility(ICharacter combatant)
	{
		// TODO
		return null;
	}

	protected virtual ICombatMove CheckWeaponryLoadout(ICharacter ch)
	{
		// The default strategy checks that they have a weapon if they use a weapon, and a shield if they use a shield.
		// It will also switch to more desirable weapons that are within easy reach
		if (ch.CombatSettings.WeaponUsePercentage > 0.0 &&
		    ch.Race.CombatSettings.CanUseWeapons &&
		    !ch.Body.WieldedItems.Any(x =>
			    x.GetItemType<IMeleeWeapon>() is IMeleeWeapon mw &&
			    ch.CombatSettings.ClassificationsAllowed.Contains(mw.Classification) &&
			    !x.IsItemType<IShield>()) &&
		    !ch.Body.Implants.Any(x =>
			    x is IImplantMeleeWeapon imw &&
			    imw.WeaponIsActive &&
			    ch.CombatSettings.ClassificationsAllowed.Contains(imw.Classification))
		    &&
		    !ch.Body.Prosthetics.Any(x =>
			    x is IMeleeWeapon mw &&
			    ch.CombatSettings.ClassificationsAllowed.Contains(mw.Classification)
		    )
		   )
		{
			return AttemptGetWeapon(ch);
		}

		if (ch.CombatSettings.PreferShieldUse && ch.Race.CombatSettings.CanUseWeapons &&
		    !ch.Body.WieldedItems.Any(x => x.IsItemType<IShield>()))
		{
			return AttemptGetShield(ch);
		}

		return null;
	}

	protected virtual ICombatMove HandleInventoryMoves(IPerceiver combatant)
	{
		if (combatant.CombatSettings.InventoryManagement == AutomaticInventorySettings.FullyManual)
		{
			return null;
		}

		var getEffects = combatant.EffectsOfType<ICombatGetItemEffect>().ToList();
		if (combatant is ICharacter ch && ch.CombatSettings.PreferToFightArmed && ch.Race.CombatSettings.CanUseWeapons)
		{
			ICombatMove move;
			if (getEffects.Any(x =>
				    x.TargetItem.Location == ch.Location &&
				    IsUseableWeapon(ch, x.TargetItem.GetItemType<IMeleeWeapon>())))
			{
				if ((move = AttemptGetWeapon(ch)) != null)
				{
					return move;
				}
			}

			if (getEffects.Any(x =>
				    x.TargetItem.Location == ch.Location &&
				    IsUseableWeapon(ch, x.TargetItem.GetItemType<IRangedWeapon>())))
			{
				if ((move = AttemptGetRangedWeapon(ch)) != null)
				{
					return move;
				}
			}

			if (getEffects.Any(x =>
				    x.TargetItem.Location == ch.Location && IsUseableWeapon(ch, x.TargetItem.GetItemType<IShield>())))
			{
				if ((move = AttemptGetShield(ch)) != null)
				{
					return move;
				}
			}

			if ((move = CheckWeaponryLoadout(ch)) != null)
			{
				return move;
			}
		}

		return null;
	}

	protected virtual ICombatMove HandleAttacks(IPerceiver combatant)
	{
		return null;
	}

	#region Implementation of ICombatStrategy

	public abstract CombatStrategyMode Mode { get; }

	public abstract ICombatMove ResponseToMove(ICombatMove move, IPerceiver defender, IPerceiver assailant);

	public virtual ICombatMove ChooseMove(IPerceiver combatant)
	{
		if (CheckCoreCombatStoppers(combatant))
		{
			return null;
		}

		ICombatMove move;
		if ((move = HandleObligatoryCombatMoves(combatant)) != null)
		{
			return move;
		}

		if ((move = HandleInventoryMoves(combatant)) != null)
		{
			return move;
		}

		if ((move = HandleCombatMovement(combatant)) != null)
		{
			return move;
		}

		if ((move = HandleAttacks(combatant)) != null)
		{
			return move;
		}

		return null;
	}

	#endregion
}
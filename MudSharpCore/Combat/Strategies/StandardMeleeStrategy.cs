using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Effects;
using MudSharp.PerceptionEngine;

namespace MudSharp.Combat.Strategies;

public class StandardMeleeStrategy : StrategyBase
{
	public static ICombatStrategy Instance { get; } = new StandardMeleeStrategy();

	protected StandardMeleeStrategy()
	{
	}

	#region Implementation of ICombatStrategy

	protected override IEnumerable<IRangedWeapon> GetNotReadyButLoadableWeapons(ICharacter shooter)
	{
		return Enumerable.Empty<IRangedWeapon>();
	}

	protected override IEnumerable<IRangedWeapon> GetReadyRangedWeapons(ICharacter shooter)
	{
		return Enumerable.Empty<IRangedWeapon>();
	}

	protected override ICombatMove AttemptGetRangedWeapon(ICharacter ch)
	{
		return null;
	}

	public override CombatStrategyMode Mode => CombatStrategyMode.StandardMelee;

	#region ResponseToMove Subfunctions

	protected virtual ICombatMove ResponseToStartClinch(StartClinchMove move, ICharacter defender,
		IPerceiver assailant)
	{
		// TODO - Specific mounted version of avoid clinch
		if (defender.RidingMount is not null)
		{
			return new HelplessDefenseMove
			{
				Assailant = defender,
				PrimaryTarget = assailant
			};
		}

		// Can't defend against clinch if you're not standing or can't afford the stamina
		if (defender.PositionState.CompareTo(PositionStanding.Instance) != PositionHeightComparison.Equivalent ||
			defender.CanSpendStamina(DodgeMove.MoveStaminaCost(defender)))
		{
			return new HelplessDefenseMove
			{
				Assailant = defender,
				PrimaryTarget = assailant
			};
		}

		return new DodgeMove { Assailant = defender };
	}

	protected virtual ICombatMove ResponseToBreakClinch(BreakClinchMove move, ICharacter defender,
		IPerceiver assailant)
	{
		return new HelplessDefenseMove { Assailant = defender, PrimaryTarget = assailant };
	}

	protected virtual ICombatMove ResponseToInitiateGrapple(InitiateGrappleMove move, ICharacter defender,
		IPerceiver assailant)
	{
		switch (defender.CombatSettings.GrappleResponse)
		{
			case GrappleResponse.Ignore:
				return new HelplessDefenseMove { Assailant = defender, PrimaryTarget = assailant };
			case GrappleResponse.Counter:
				return new CounterGrappleMove { Assailant = defender, PrimaryTarget = assailant };
			default:
				return new DodgeMove { Assailant = defender, PrimaryTarget = assailant };
		}
	}

	protected virtual ICombatMove ResponseToExtendGrapple(ExtendGrappleMove move, ICharacter defender,
		IPerceiver assailant)
	{
		switch (defender.CombatSettings.GrappleResponse)
		{
			case GrappleResponse.Ignore:
				return new HelplessDefenseMove { Assailant = defender, PrimaryTarget = assailant };
			case GrappleResponse.Counter:
				return new CounterGrappleMove { Assailant = defender, PrimaryTarget = assailant };
			case GrappleResponse.Throw:
			// TODO
			default:
				return new DodgeMove { Assailant = defender, PrimaryTarget = assailant };
		}
	}

	protected virtual ICombatMove ResponseToRangedAttack(IRangedWeaponAttackMove move, ICharacter defender,
		IPerceiver assailant)
	{
		if (defender.Cover is not null)
		{
			return new HelplessDefenseMove { Assailant = defender, PrimaryTarget = assailant };
		}

		var shield =
			defender.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IShield>())
					.FirstOrDefault(x => defender.CanSpendStamina(x.ShieldType.StaminaPerBlock));
		var shieldBonus = shield != null
			? BlockMove.GetBlockBonus(move, defender.Body.AlignmentOf(shield.Parent), shield)
			: 0;
		var finalBlockDifficulty = move.Weapon.BaseBlockDifficulty.ApplyBonus(shieldBonus);
		var finalDodgeDifficulty = move.Weapon.BaseDodgeDifficulty;

		if (shield != null)
		{
			switch (defender.PreferredDefenseType)
			{
				case DefenseType.Block:
					return new BlockMove { Assailant = defender, Shield = shield, PrimaryTarget = assailant };
				case DefenseType.None:
					if (finalDodgeDifficulty.Difference(finalBlockDifficulty) > -1)
					{
						return new BlockMove { Assailant = defender, Shield = shield, PrimaryTarget = assailant };
					}

					break;
			}
		}

		// You can't dodge if you're mounted
		if (defender.RidingMount is not null)
		{
			return new HelplessDefenseMove
			{
				Assailant = defender,
				PrimaryTarget = assailant
			};
		}

		if (defender.CanSpendStamina(DodgeRangeMove.MoveStaminaCost(defender)))
		{
			return new DodgeRangeMove { Assailant = defender };
		}

		return new TooExhaustedMove
		{
			Assailant = defender
		};
	}

	public virtual ICombatMove ResponseToWeaponAttack(IWeaponAttackMove move, ICharacter defender,
		IPerceiver assailant)
	{
		var desperate = !defender.PositionState.Upright;
		var clinching = defender.EffectsOfType<ClinchEffect>()
								.Any(x => x.Clincher == defender.CombatTarget && x.Target == assailant);
		var shield =
			defender.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IShield>())
					.FirstMax(x => x.ShieldType.BlockBonus);
		var parry =
			defender.Body.WieldedItems.Where(x => !x.IsItemType<IShield>())
					.SelectNotNull(x => x.GetItemType<IMeleeWeapon>())
					.FirstMax(x => x.WeaponType.ParryBonus);
		var shieldBonus = shield != null
			? BlockMove.GetBlockBonus(move, defender.Body.AlignmentOf(shield.Parent), shield)
			: 0;

		var finalBlockDifficulty = move.Attack.Profile.BaseBlockDifficulty.ApplyBonus(shieldBonus);
		var finalParryDifficulty =
			move.Attack.Profile.BaseParryDifficulty.ApplyBonus(parry?.WeaponType.ParryBonus ?? 0.0);
		var finalDodgeDifficulty = move.Attack.Profile.BaseDodgeDifficulty;

		var finalBlockTargetNumber = defender.Gameworld.GetCheck(CheckType.BlockCheck).TargetNumber(defender, finalBlockDifficulty, shield?.ShieldType.BlockTrait, assailant);
		var finalParryTargetNumber = defender.Gameworld.GetCheck(CheckType.ParryCheck).TargetNumber(defender, finalParryDifficulty, parry?.WeaponType.ParryTrait, assailant);
		var finalDodgeTargetNumber = defender.Gameworld.GetCheck(CheckType.DodgeCheck).TargetNumber(defender, finalDodgeDifficulty, null, assailant);

		var canBlock = shield is not null && !clinching && move.Attack.Profile.BaseBlockDifficulty != Difficulty.Impossible && defender.CanSpendStamina(BlockMove.MoveStaminaCost(assailant, defender, shield));
		var canParry = parry is not null && !defender.EffectsOfType<IWardBeatenEffect>().Any() && !clinching && move.Attack.Profile.BaseParryDifficulty != Difficulty.Impossible && defender.CanSpendStamina(ParryMove.MoveStaminaCost(assailant, defender, parry));
		var canDodge = defender.RidingMount is null && move.Attack.Profile.BaseDodgeDifficulty != Difficulty.Impossible && defender.CanSpendStamina(DodgeMove.MoveStaminaCost(defender));

		// If they have a preference for a defense type, use that if possible
		switch (defender.PreferredDefenseType)
		{
			case DefenseType.Block:
				if (canBlock)
				{
					return desperate
						? new DesperateBlock { Assailant = defender, Shield = shield, PrimaryTarget = assailant }
						: new BlockMove { Assailant = defender, Shield = shield, PrimaryTarget = assailant };
				}
				break;
			case DefenseType.Parry:
				if (canParry)
				{
					return desperate
						? new DesperateParry { Assailant = defender, Weapon = parry, PrimaryTarget = assailant }
						: new ParryMove { Assailant = defender, Weapon = parry, PrimaryTarget = assailant };
				}
				break;
			case DefenseType.Dodge:
				if (canDodge)
				{
					return desperate || clinching
						? new DesperateDodge { Assailant = defender }
						: new DodgeMove { Assailant = defender };
				}
				break;
		}


		// Otherwise fall back to default handling
		// Calculate the success chance of the various defense types
		var availableDefenses = new List<(DefenseType Defense, double Chance)>(3);
		if (canBlock) {
			availableDefenses.Add((DefenseType.Block, finalBlockTargetNumber));
		}
		if (canParry)
		{
			availableDefenses.Add((DefenseType.Parry, finalParryTargetNumber));
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
				return desperate
						? new DesperateBlock { Assailant = defender, Shield = shield, PrimaryTarget = assailant }
						: new BlockMove { Assailant = defender, Shield = shield, PrimaryTarget = assailant };
			case DefenseType.Parry:
				return desperate
						? new DesperateParry { Assailant = defender, Weapon = parry, PrimaryTarget = assailant }
						: new ParryMove { Assailant = defender, Weapon = parry, PrimaryTarget = assailant };
			case DefenseType.Dodge:
				return desperate || clinching
				? new DesperateDodge { Assailant = defender }
				: new DodgeMove { Assailant = defender };
		}

		return new HelplessDefenseMove
		{
			Assailant = defender
		};
	}

	protected virtual ICombatMove ResponseToChargeToMelee(ChargeToMeleeMove move, ICharacter defender,
		IPerceiver assailant)
	{
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


	protected virtual ICombatMove ResponseToMagicPowerAttackMove(MagicPowerAttackMove move, ICharacter defender,
		IPerceiver assailant)
	{
		var desperate = !defender.PositionState.Upright;
		var clinching = defender.EffectsOfType<ClinchEffect>()
								.Any(x => x.Clincher == defender.CombatTarget && x.Target == assailant);
		var shield =
			defender.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IShield>())
					.FirstMax(x => x.ShieldType.BlockBonus);
		var parry =
			defender.Body.WieldedItems.Where(x => !x.IsItemType<IShield>())
					.SelectNotNull(x => x.GetItemType<IMeleeWeapon>())
					.FirstMax(x => x.WeaponType.ParryBonus);
		var shieldBonus = shield != null
			? BlockMove.GetBlockBonus(move, defender.Body.AlignmentOf(shield.Parent), shield)
			: 0;

		var finalBlockDifficulty = move.Attack.Profile.BaseBlockDifficulty.ApplyBonus(shieldBonus);
		var finalParryDifficulty =
			move.Attack.Profile.BaseParryDifficulty.ApplyBonus(parry?.WeaponType.ParryBonus ?? 0.0);
		var finalDodgeDifficulty = move.Attack.Profile.BaseDodgeDifficulty;

		var finalBlockTargetNumber = defender.Gameworld.GetCheck(CheckType.BlockCheck).TargetNumber(defender, finalBlockDifficulty, shield?.ShieldType.BlockTrait, assailant);
		var finalParryTargetNumber = defender.Gameworld.GetCheck(CheckType.ParryCheck).TargetNumber(defender, finalParryDifficulty, parry?.WeaponType.ParryTrait, assailant);
		var finalDodgeTargetNumber = defender.Gameworld.GetCheck(CheckType.DodgeCheck).TargetNumber(defender, finalDodgeDifficulty, null, assailant);

		var canBlock = shield is not null && !clinching && move.AttackPower.ValidDefenseTypes.Contains(DefenseType.Block) && move.Attack.Profile.BaseBlockDifficulty != Difficulty.Impossible && defender.CanSpendStamina(BlockMove.MoveStaminaCost(assailant, defender, shield));
		var canParry = parry is not null && !defender.EffectsOfType<IWardBeatenEffect>().Any() && !clinching && move.AttackPower.ValidDefenseTypes.Contains(DefenseType.Parry) && move.Attack.Profile.BaseParryDifficulty != Difficulty.Impossible && defender.CanSpendStamina(ParryMove.MoveStaminaCost(assailant, defender, parry));
		var canDodge = defender.RidingMount is not null && move.Attack.Profile.BaseDodgeDifficulty != Difficulty.Impossible && move.AttackPower.ValidDefenseTypes.Contains(DefenseType.Dodge) && defender.CanSpendStamina(DodgeMove.MoveStaminaCost(defender));
		
		// If they have a preference for a defense type, use that if possible
		switch (defender.PreferredDefenseType)
		{
			case DefenseType.Block:
				if (canBlock)
				{
					return desperate
						? new DesperateBlock { Assailant = defender, Shield = shield, PrimaryTarget = assailant }
						: new BlockMove { Assailant = defender, Shield = shield, PrimaryTarget = assailant };
				}
				break;
			case DefenseType.Parry:
				if (canParry)
				{
					return desperate
						? new DesperateParry { Assailant = defender, Weapon = parry, PrimaryTarget = assailant }
						: new ParryMove { Assailant = defender, Weapon = parry, PrimaryTarget = assailant };
				}
				break;
			case DefenseType.Dodge:
				if (canDodge)
				{
					return desperate || clinching
						? new DesperateDodge { Assailant = defender }
						: new DodgeMove { Assailant = defender };
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
		if (canParry)
		{
			availableDefenses.Add((DefenseType.Parry, finalParryTargetNumber));
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
				return desperate
						? new DesperateBlock { Assailant = defender, Shield = shield, PrimaryTarget = assailant }
						: new BlockMove { Assailant = defender, Shield = shield, PrimaryTarget = assailant };
			case DefenseType.Parry:
				return desperate
						? new DesperateParry { Assailant = defender, Weapon = parry, PrimaryTarget = assailant }
						: new ParryMove { Assailant = defender, Weapon = parry, PrimaryTarget = assailant };
			case DefenseType.Dodge:
				return desperate || clinching
				? new DesperateDodge { Assailant = defender }
				: new DodgeMove { Assailant = defender };
		}

		return new HelplessDefenseMove
		{
			Assailant = defender
		};
	}

	#endregion

	public override ICombatMove ResponseToMove(ICombatMove move, IPerceiver defender, IPerceiver assailant)
	{
		if (!(defender is ICharacter defenseCharacter))
		{
			return null; // TODO - item specific helpless defense
		}

		if (move is ScreechAttackMove || move is TakedownMove || move is WrenchingAttack)
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
			defenseCharacter.State.HasFlag(CharacterState.Paralysed))
		{
			return moveAsWeaponAttack != null || moveAsRangedAttack != null
				? new HelplessDefenseMove { Assailant = defenseCharacter }
				: null;
		}

		if (move is InitiateGrappleMove moveAsInitiateGrapple) // Should come before the blocking effects check
		{
			return ResponseToInitiateGrapple(moveAsInitiateGrapple, defenseCharacter, assailant);
		}

		if (move is ExtendGrappleMove moveAsExtendGrapple) // Should come before the blocking effects check
		{
			return ResponseToExtendGrapple(moveAsExtendGrapple, defenseCharacter, assailant);
		}

		if (defenseCharacter.CombinedEffectsOfType<IEffect>().Any(x => x.IsBlockingEffect("general")))
		{
			return moveAsWeaponAttack != null || moveAsRangedAttack != null
				? new HelplessDefenseMove { Assailant = defenseCharacter }
				: null;
		}

		if (move is MagicPowerAttackMove moveAsMagicPowerAttackMove)
		{
			return ResponseToMagicPowerAttackMove(moveAsMagicPowerAttackMove, defenseCharacter, assailant);
		}

		if (move is StartClinchMove moveAsStartClinch)
		{
			return ResponseToStartClinch(moveAsStartClinch, defenseCharacter, assailant);
		}

		if (move is BreakClinchMove moveAsBreakClinch)
		{
			return ResponseToBreakClinch(moveAsBreakClinch, defenseCharacter, assailant);
		}

		if (moveAsRangedAttack != null)
		{
			return ResponseToRangedAttack(moveAsRangedAttack, defenseCharacter, assailant);
		}

		if (move is ClinchAttackMove || move is ClinchNaturalAttackMove)
		{
			return defenseCharacter.CanSpendStamina(DodgeMove.MoveStaminaCost(defenseCharacter)) && defenseCharacter.RidingMount is null
				? (ICombatMove)new DodgeMove { Assailant = defenseCharacter }
				: new HelplessDefenseMove { Assailant = defenseCharacter };
		}

		if (moveAsWeaponAttack != null)
		{
			return ResponseToWeaponAttack(moveAsWeaponAttack, defenseCharacter, assailant);
		}

		if (move is ChargeToMeleeMove chargeToMeleeMove)
		{
			return ResponseToChargeToMelee(chargeToMeleeMove, defenseCharacter, assailant);
		}

		if (move is MoveToMeleeMove moveToMeleeMove)
		{
			return ResponseToMoveToMelee(moveToMeleeMove, defenseCharacter, assailant);
		}

		if (move is FireAndAdvanceToMeleeMove fireAndAdvanceToMeleeMove)
		{
			return ResponseToFireAndAdvance(fireAndAdvanceToMeleeMove, defenseCharacter, assailant);
		}

		return null;
	}

	protected ICombatMove CheckFixedAttacks(ICharacter ch)
	{
		if (!ch.MeleeRange)
		{
			return null;
		}

		var weapons = ch.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>()).ToList();
		var implants = ch.Body.Implants
						 .OfType<IImplantMeleeWeapon>()
						 .Where(x => x.WeaponIsActive)
						 .ToList();
		var prosthetics = ch.Body.Prosthetics
							.OfType<IProstheticMeleeWeapon>()
							.Where(x => x.WeaponIsActive)
							.ToList();
		if (!ch.Combat.Friendly &&
			!CharacterState.Able.HasFlag((ch.CombatTarget as ICharacter)?.State ?? CharacterState.Unable) &&
			ch.Combat.Combatants.Except(ch.CombatTarget).All(x => x.CombatTarget != ch))
		{
			foreach (var preference in ch.CombatSettings.MeleeAttackOrderPreferences)
			{
				switch (preference)
				{
					case MeleeAttackOrderPreference.Weapon:
						foreach (var weapon in weapons)
						{
							var cdge = ch.EffectsOfType<CombatCoupDeGrace>().FirstOrDefault(x => x.Weapon == weapon);
							if (cdge != null)
							{
								return new CoupDeGrace(cdge.Attack, (ICharacter)ch.CombatTarget)
								{
									Assailant = ch,
									Weapon = cdge.Weapon,
									Emote = cdge.Emote
								};
							}
						}

						break;
					case MeleeAttackOrderPreference.Implant:
						foreach (var implant in implants)
						{
							var cdge = ch.EffectsOfType<CombatCoupDeGrace>().FirstOrDefault(x => x.Weapon == implant);
							if (cdge != null)
							{
								return new CoupDeGrace(cdge.Attack, (ICharacter)ch.CombatTarget)
								{
									Assailant = ch,
									Weapon = cdge.Weapon,
									Emote = cdge.Emote
								};
							}
						}

						break;
					case MeleeAttackOrderPreference.Prosthetic:
						foreach (var prosthetic in prosthetics)
						{
							var cdge = ch.EffectsOfType<CombatCoupDeGrace>()
										 .FirstOrDefault(x => x.Weapon == prosthetic);
							if (cdge != null)
							{
								return new CoupDeGrace(cdge.Attack, (ICharacter)ch.CombatTarget)
								{
									Assailant = ch,
									Weapon = cdge.Weapon,
									Emote = cdge.Emote
								};
							}
						}

						break;
					case MeleeAttackOrderPreference.Magic:
						// TODO
						break;
					case MeleeAttackOrderPreference.Psychic:
						// TODO
						break;
				}
			}
		}

		var effect = ch.EffectsOfType<FixedCombatMoveType>().FirstOrDefault();
		if (effect == null)
		{
			return null;
		}

		var weaponAttacks = weapons.Select(x => Tuple.Create(x,
									   x.WeaponType
										.UsableAttacks(ch, x.Parent, ch.CombatTarget, x.HandednessForWeapon(ch), false,
											effect.FixedTypes.ToArray())
										.Where(y => ch.CanSpendStamina(MeleeWeaponAttack.MoveStaminaCost(ch, y)))
										.ToList()))
								   .Where(x => x.Item2.Any()).ToList();
		var implantAttacks = implants.Select(x => Tuple.Create(x,
										 x.WeaponType
										  .UsableAttacks(ch, x.Parent, ch.CombatTarget, x.HandednessForWeapon(ch),
											  false,
											  effect.FixedTypes.ToArray())
										  .Where(y => ch.CanSpendStamina(MeleeWeaponAttack.MoveStaminaCost(ch, y)))
										  .ToList()))
									 .Where(x => x.Item2.Any()).ToList();
		var prostheticAttacks = prosthetics.Select(x => Tuple.Create(x,
											   x.WeaponType
												.UsableAttacks(ch, x.Parent, ch.CombatTarget, x.HandednessForWeapon(ch),
													false,
													effect.FixedTypes.ToArray())
												.Where(
													y => ch.CanSpendStamina(MeleeWeaponAttack.MoveStaminaCost(ch, y)))
												.ToList()))
										   .Where(x => x.Item2.Any()).ToList();
		var unarmedAttacks = ch.Race.UsableNaturalWeaponAttacks(ch, ch.CombatTarget, false, effect.FixedTypes.ToArray())
							   .Where(x => ch.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(ch, x.Attack)))
							   .ToList();
		var charTarget = ch.CombatTarget as ICharacter;
		var itemTarget = ch.CombatTarget as IGameItem;

		if (ch.CombatSettings.PreferToFightArmed && ch.Race.CombatSettings.CanUseWeapons)
		{
			foreach (var preference in ch.CombatSettings.MeleeAttackOrderPreferences)
			{
				switch (preference)
				{
					case MeleeAttackOrderPreference.Weapon:
						if (!weaponAttacks.Any())
						{
							continue;
						}

						var attack = weaponAttacks.GetWeightedRandom(x => x.Item2.Sum(y => y.Weighting));
						var attackWeapon = attack.Item1;
						var attackProfile = attack.Item2.GetWeightedRandom(x => x.Weighting);
						if (!effect.Indefinite)
						{
							ch.RemoveEffect(effect);
						}

						if (charTarget != null)
						{
							return CombatMoveFactory.CreateWeaponAttack(ch, attackWeapon, attackProfile, charTarget);
						}

						if (itemTarget != null)
						{
							return CombatMoveFactory.CreateWeaponAttack(ch, attackWeapon, attackProfile, itemTarget);
						}

						throw new ApplicationException("Unknown target type in CheckFixedAttacks");
					case MeleeAttackOrderPreference.Implant:
						if (!implantAttacks.Any())
						{
							continue;
						}

						var implantAttack = implantAttacks.GetWeightedRandom(x => x.Item2.Sum(y => y.Weighting));
						attackWeapon = implantAttack.Item1;
						attackProfile = implantAttack.Item2.GetWeightedRandom(x => x.Weighting);
						if (!effect.Indefinite)
						{
							ch.RemoveEffect(effect);
						}

						if (charTarget != null)
						{
							return CombatMoveFactory.CreateWeaponAttack(ch, attackWeapon, attackProfile, charTarget);
						}

						if (itemTarget != null)
						{
							return CombatMoveFactory.CreateWeaponAttack(ch, attackWeapon, attackProfile, itemTarget);
						}

						throw new ApplicationException("Unknown target type in CheckFixedAttacks");
					case MeleeAttackOrderPreference.Prosthetic:
						if (!prostheticAttacks.Any())
						{
							continue;
						}

						var prostheticAttack = prostheticAttacks.GetWeightedRandom(x => x.Item2.Sum(y => y.Weighting));
						attackWeapon = prostheticAttack.Item1;
						attackProfile = prostheticAttack.Item2.GetWeightedRandom(x => x.Weighting);
						if (!effect.Indefinite)
						{
							ch.RemoveEffect(effect);
						}

						if (charTarget != null)
						{
							return CombatMoveFactory.CreateWeaponAttack(ch, attackWeapon, attackProfile, charTarget);
						}

						if (itemTarget != null)
						{
							return CombatMoveFactory.CreateWeaponAttack(ch, attackWeapon, attackProfile, itemTarget);
						}

						throw new ApplicationException("Unknown target type in CheckFixedAttacks");
					case MeleeAttackOrderPreference.Magic:
						continue;
					case MeleeAttackOrderPreference.Psychic:
						continue;
				}
			}
		}

		if (ch.CombatSettings.FallbackToUnarmedIfNoWeapon && unarmedAttacks.Any())
		{
			var attack = unarmedAttacks.GetWeightedRandom(x => x.Attack.Weighting);
			if (!effect.Indefinite)
			{
				ch.RemoveEffect(effect);
			}

			if (charTarget != null)
			{
				return CombatMoveFactory.CreateNaturalWeaponAttack(ch, attack, charTarget);
			}

			if (itemTarget != null)
			{
				return CombatMoveFactory.CreateNaturalWeaponAttack(ch, attack, itemTarget);
			}

			throw new ApplicationException("Unknown target type in CheckFixedAttacks");
		}

		foreach (var preference in ch.CombatSettings.MeleeAttackOrderPreferences)
		{
			switch (preference)
			{
				case MeleeAttackOrderPreference.Weapon:
					if (!weaponAttacks.Any())
					{
						continue;
					}

					var attack = weaponAttacks.GetWeightedRandom(x => x.Item2.Sum(y => y.Weighting));
					var attackWeapon = attack.Item1;
					var attackProfile = attack.Item2.GetWeightedRandom(x => x.Weighting);
					if (!effect.Indefinite)
					{
						ch.RemoveEffect(effect);
					}

					if (charTarget != null)
					{
						return CombatMoveFactory.CreateWeaponAttack(ch, attackWeapon, attackProfile, charTarget);
					}

					if (itemTarget != null)
					{
						return CombatMoveFactory.CreateWeaponAttack(ch, attackWeapon, attackProfile, itemTarget);
					}

					throw new ApplicationException("Unknown target type in CheckFixedAttacks");
				case MeleeAttackOrderPreference.Implant:
					if (!implantAttacks.Any())
					{
						continue;
					}

					var implantAttack = implantAttacks.GetWeightedRandom(x => x.Item2.Sum(y => y.Weighting));
					attackWeapon = implantAttack.Item1;
					attackProfile = implantAttack.Item2.GetWeightedRandom(x => x.Weighting);
					if (!effect.Indefinite)
					{
						ch.RemoveEffect(effect);
					}

					if (charTarget != null)
					{
						return CombatMoveFactory.CreateWeaponAttack(ch, attackWeapon, attackProfile, charTarget);
					}

					if (itemTarget != null)
					{
						return CombatMoveFactory.CreateWeaponAttack(ch, attackWeapon, attackProfile, itemTarget);
					}

					throw new ApplicationException("Unknown target type in CheckFixedAttacks");
				case MeleeAttackOrderPreference.Prosthetic:
					if (!prostheticAttacks.Any())
					{
						continue;
					}

					var prostheticAttack = prostheticAttacks.GetWeightedRandom(x => x.Item2.Sum(y => y.Weighting));
					attackWeapon = prostheticAttack.Item1;
					attackProfile = prostheticAttack.Item2.GetWeightedRandom(x => x.Weighting);
					if (!effect.Indefinite)
					{
						ch.RemoveEffect(effect);
					}

					if (charTarget != null)
					{
						return CombatMoveFactory.CreateWeaponAttack(ch, attackWeapon, attackProfile, charTarget);
					}

					if (itemTarget != null)
					{
						return CombatMoveFactory.CreateWeaponAttack(ch, attackWeapon, attackProfile, itemTarget);
					}

					throw new ApplicationException("Unknown target type in CheckFixedAttacks");
				case MeleeAttackOrderPreference.Magic:
					break;
				case MeleeAttackOrderPreference.Psychic:
					break;
			}
		}

		if (unarmedAttacks.Any())
		{
			var attack = unarmedAttacks.GetWeightedRandom(x => x.Attack.Weighting);
			if (!effect.Indefinite)
			{
				ch.RemoveEffect(effect);
			}

			if (charTarget != null)
			{
				return CombatMoveFactory.CreateNaturalWeaponAttack(ch, attack, charTarget);
			}

			if (itemTarget != null)
			{
				return CombatMoveFactory.CreateNaturalWeaponAttack(ch, attack, itemTarget);
			}

			throw new ApplicationException("Unknown target type in CheckFixedAttacks");
		}

		return null;
	}

	public List<IWeaponAttack> PossibleAttacksForWeapon(ICharacter combatant, IMeleeWeapon weapon, bool ignoreStamina)
	{
		var tch = combatant.CombatTarget as ICharacter;
		if (!combatant.Combat.Friendly && tch != null &&
			!CharacterState.Able.HasFlag(tch.State) &&
			combatant.Combat.Combatants.Except(tch).All(x => x.CombatTarget != combatant))
		{
			if (combatant.CombatSettings.AttackCriticallyInjured &&
				!combatant.CombatSettings.ForbiddenIntentions.HasFlag(CombatMoveIntentions.CoupDeGrace))
			{
				var cdgeAttacks = weapon.WeaponType.UsableAttacks(combatant, weapon.Parent, combatant.CombatTarget,
					weapon.HandednessForWeapon(combatant), false,
					BuiltInCombatMoveType.CoupDeGrace).ToList();
				if (cdgeAttacks.Any())
				{
					return cdgeAttacks;
				}
			}
		}

		var attackTypes = CombatExtensions.StandardMeleeWeaponAttacks;
		if (!combatant.Combat.Friendly && tch?.PositionState.Upright == false)
		{
			attackTypes = attackTypes.Plus(BuiltInCombatMoveType.DownedAttack).ToArray();
		}

		var possibleAttacks =
			weapon.WeaponType.UsableAttacks(combatant, weapon.Parent, combatant.CombatTarget,
				weapon.HandednessForWeapon(combatant), false, attackTypes);

		if (ignoreStamina)
		{
			return possibleAttacks.ToList();
		}

		return possibleAttacks.Where(x => combatant.CanSpendStamina(MeleeWeaponAttack.MoveStaminaCost(combatant, x)))
							  .ToList();
	}

	protected virtual ICombatMove AttemptUseWeapon(ICharacter combatant)
	{
		var possibleWeaponsAndAttacks =
			new List<(IMeleeWeapon Weapon, IWeaponAttack Attack, double Weight)>();
		var weaponsAndAttacks =
			new List<(IMeleeWeapon Weapon, IWeaponAttack Attack, double Weight)>();
		var iterations = 0;
		foreach (var preference in combatant.CombatSettings.MeleeAttackOrderPreferences.AsEnumerable().Reverse())
		{
			List<IMeleeWeapon> weaponsForThisIteration;
			switch (preference)
			{
				case MeleeAttackOrderPreference.Weapon:
					weaponsForThisIteration = combatant.Body.WieldedItems
													   .SelectNotNull(x => x.GetItemType<IMeleeWeapon>()).ToList();
					break;
				case MeleeAttackOrderPreference.Implant:
					weaponsForThisIteration = combatant.Body.Implants
													   .OfType<IImplantMeleeWeapon>()
													   .Where(x => x.WeaponIsActive)
													   .Cast<IMeleeWeapon>()
													   .ToList();
					break;
				case MeleeAttackOrderPreference.Prosthetic:
					weaponsForThisIteration = combatant.Body.Prosthetics
													   .OfType<IProstheticMeleeWeapon>()
													   .Where(x => x.WeaponIsActive)
													   .Cast<IMeleeWeapon>()
													   .ToList();
					break;
				case MeleeAttackOrderPreference.Magic:
					continue;
				case MeleeAttackOrderPreference.Psychic:
					continue;
				default:
					continue;
			}

			iterations++;
			var multiplier = iterations; // TODO - straight up multiplier or more complex?
			var possibleAttacksThisIteration = weaponsForThisIteration
											   .Select(x => (Weapon: x,
												   Attacks: PossibleAttacksForWeapon(combatant, x, true)))
											   .SelectMany(x =>
												   x.Attacks.Select(y => (Weapon: x.Weapon, Attack: y,
													   Weighting: y.Weighting * multiplier)))
											   .ToList();
			var staminaAttacksThisIteration = possibleAttacksThisIteration
											  .Where(x =>
												  combatant.CanSpendStamina(
													  MeleeWeaponAttack.MoveStaminaCost(combatant, x.Attack)))
											  .ToList();

			possibleWeaponsAndAttacks.AddRange(possibleAttacksThisIteration);
			weaponsAndAttacks.AddRange(staminaAttacksThisIteration);
		}

		if (!weaponsAndAttacks.Any())
		{
			return possibleWeaponsAndAttacks.Any() ? new TooExhaustedMove { Assailant = combatant } : null;
		}

		if (combatant.TargettedBodypart != null)
		{
			var onTargetAttacks = weaponsAndAttacks.Where(x =>
													   x.Attack.Alignment.WithinOffset(
														   combatant.TargettedBodypart.Alignment, 1) &&
													   x.Attack.Orientation.WithinOffset(
														   combatant.TargettedBodypart.Orientation, 1))
												   .ToList();
			if (onTargetAttacks.Any())
			{
				weaponsAndAttacks = onTargetAttacks;
			}
		}

		var preferredAttacks =
			weaponsAndAttacks.Where(x => x.Attack.Intentions.HasFlag(combatant.CombatSettings.PreferredIntentions))
							 .ToList();
		if (preferredAttacks.Any() && Dice.Roll(1, 2) == 1)
		{
			weaponsAndAttacks = preferredAttacks;
		}


		var attack = weaponsAndAttacks.GetWeightedRandom(x => x.Weight);
		if (attack == default)
		{
			return null;
		}

		if (combatant.CombatTarget is ICharacter charTarget)
		{
			return CombatMoveFactory.CreateWeaponAttack(combatant, attack.Weapon, attack.Attack, charTarget);
		}

		if (combatant.CombatTarget is IGameItem itemTarget)
		{
			return CombatMoveFactory.CreateWeaponAttack(combatant, attack.Weapon, attack.Attack, itemTarget);
		}

		throw new NotImplementedException("Unimplemented Combatant type in StandardMeleeStrategy.AttemptUseWeapon - " +
										  combatant.CombatTarget.GetType().FullName);
	}

	protected virtual ICombatMove AttemptUseNaturalAttack(ICharacter combatant)
	{
		if (combatant.CombatTarget == null)
		{
			return null;
		}

		var attackTypes = new[]
		{
			BuiltInCombatMoveType.NaturalWeaponAttack,
			BuiltInCombatMoveType.StaggeringBlowUnarmed,
			BuiltInCombatMoveType.UnbalancingBlowUnarmed,
			BuiltInCombatMoveType.ScreechAttack,
			BuiltInCombatMoveType.EnvenomingAttack
		};
		if (!combatant.Combat.Friendly && (combatant.CombatTarget as ICharacter)?.PositionState.Upright == false)
		{
			attackTypes = attackTypes.Plus(BuiltInCombatMoveType.DownedAttackUnarmed).ToArray();
		}

		var possibleAttacks = combatant.Race
									   .UsableNaturalWeaponAttacks(combatant, combatant.CombatTarget, false,
										   attackTypes).ToList();
		var attacks = possibleAttacks
					  .Where(x => combatant.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(combatant, x.Attack)))
					  .ToList();
		if (!attacks.Any())
		{
			return possibleAttacks.Any() ? new TooExhaustedMove { Assailant = combatant } : null;
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
			"Unimplemented Combatant type in StandardMeleeStrategy.AttemptUseNaturalAttack - " +
			combatant.CombatTarget.GetType().FullName);
	}

	protected virtual ICombatMove AttemptUseAuxilliaryAction(ICharacter combatant)
	{
		if (combatant.CombatTarget is not ICharacter tch)
		{
			return null;
		}

		var moves = combatant.Race.UsableAuxiliaryMoves(combatant, tch, false).ToList();
		var usableMoves = moves.Where(x => combatant.CanSpendStamina(x.StaminaCost)).ToList();
		if (!usableMoves.Any())
		{
			return moves.Any() ? new TooExhaustedMove { Assailant = combatant } : null;
		}

		var preferredMoves = usableMoves.Where(x => x.Intentions.HasFlag(combatant.CombatSettings.PreferredIntentions))
										.ToList();
		if (preferredMoves.Any() && Dice.Roll(1, 2) == 1)
		{
			usableMoves = preferredMoves;
		}

		var move = usableMoves.GetWeightedRandom(x => x.Weighting);
		if (move is null)
		{
			return null;
		}

		return new AuxiliaryMove(combatant, tch, move);
	}

	protected virtual ICombatMove HandleWeaponAttackRolled(ICharacter combatant)
	{
		var move = AttemptUseWeapon(combatant);
		if (move != null)
		{
			return move;
		}

		if (combatant.CombatSettings.FallbackToUnarmedIfNoWeapon && combatant.PositionState.Upright)
		{
			return AttemptUseNaturalAttack(combatant);
		}

		if (
			combatant.CombatStrategyMode == CombatStrategyMode.StandardMelee && 
		    !HasViableMeleeAttack(combatant) &&
			ClinchStrategy.Instance.HasViableClinchAttack(combatant)
			)
		{
			combatant.Send("You realise you have to close to clinching range to attack your target.");
			combatant.CombatStrategyMode = CombatStrategyMode.Clinch;
			return CombatStrategyFactory.GetStrategy(CombatStrategyMode.Clinch).ChooseMove(combatant);
		}

		return null;
	}

	public bool HasViableMeleeAttack(ICharacter combatant)
	{
		var tch = combatant.CombatTarget as ICharacter;

		foreach (var preference in combatant.CombatSettings.MeleeAttackOrderPreferences.AsEnumerable().Reverse())
		{
			var weapons = Enumerable.Empty<IMeleeWeapon>();
			switch (preference)
			{
				case MeleeAttackOrderPreference.Weapon:
					weapons = combatant.Body.WieldedItems.SelectNotNull(x => x.GetItemType<IMeleeWeapon>());
					break;
				case MeleeAttackOrderPreference.Implant:
					weapons = combatant.Body.Implants.OfType<IImplantMeleeWeapon>().Where(x => x.WeaponIsActive);
					break;
				case MeleeAttackOrderPreference.Prosthetic:
					weapons = combatant.Body.Prosthetics.OfType<IProstheticMeleeWeapon>().Where(x => x.WeaponIsActive);
					break;
				default:
					continue;
			}

			if (weapons.Any(x => PossibleAttacksForWeapon(combatant, x, true).Any()))
			{
				return true;
			}
		}

		var naturalTypes = new[]
		{
						BuiltInCombatMoveType.NaturalWeaponAttack,
						BuiltInCombatMoveType.StaggeringBlowUnarmed,
						BuiltInCombatMoveType.UnbalancingBlowUnarmed,
						BuiltInCombatMoveType.ScreechAttack,
						BuiltInCombatMoveType.EnvenomingAttack
				};

		if (!combatant.Combat.Friendly && tch?.PositionState.Upright == false)
		{
			naturalTypes = naturalTypes.Plus(BuiltInCombatMoveType.DownedAttackUnarmed).ToArray();
		}

		return combatant.Race.UsableNaturalWeaponAttacks(combatant, combatant.CombatTarget, false, naturalTypes).Any();
	}

	protected virtual ICombatMove HandleGeneralAttacks(ICharacter combatant)
	{
		var roll = Constants.Random.NextDouble();
		if (combatant.CombatSettings.WeaponUsePercentage > 0 &&
			roll <= combatant.CombatSettings.WeaponUsePercentage)
		{
			return HandleWeaponAttackRolled(combatant);
		}

		roll -= combatant.CombatSettings.WeaponUsePercentage;
		if (combatant.CombatSettings.NaturalWeaponPercentage > 0.0 &&
			roll <= combatant.CombatSettings.NaturalWeaponPercentage)
		{
			if (combatant.PositionState.Upright)
			{
				return AttemptUseNaturalAttack(combatant);
			}
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

		return AttemptUseAuxilliaryAction(combatant);
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

		ICombatMove move;
		if ((move = CheckFixedAttacks(ch)) != null)
		{
			return move;
		}

		if (ch.CurrentStamina < ch.CombatSettings.MinimumStaminaToAttack)
		{
			return null;
		}

		return HandleGeneralAttacks(ch);
	}

	#endregion
}
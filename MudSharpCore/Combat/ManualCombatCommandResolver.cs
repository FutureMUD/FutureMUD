using MudSharp.Combat.Moves;
using MudSharp.NPC.AI;

namespace MudSharp.Combat;

public sealed record ManualCombatMoveResolution(bool Success, ICombatMove Move, string Error,
	ManualCombatActionContext? Context = null)
{
	public static ManualCombatMoveResolution Failed(string error)
	{
		return new ManualCombatMoveResolution(false, null, error);
	}

	public static ManualCombatMoveResolution Resolved(ICombatMove move, ManualCombatActionContext? context = null)
	{
		return new ManualCombatMoveResolution(true, move, string.Empty, context);
	}
}

public static class ManualCombatCommandResolver
{
	public static ManualCombatMoveResolution TryResolve(
		ICharacter actor,
		IManualCombatCommand command,
		ICharacter target,
		bool returnTooExhaustedMove = false,
		ManualCombatActionContext? context = null)
	{
		if (target is null)
		{
			return ManualCombatMoveResolution.Failed("You must specify a target for that command.");
		}

		if (actor == target)
		{
			return ManualCombatMoveResolution.Failed("You cannot target yourself with that combat command.");
		}

		if (actor.Combat is null || !actor.Combat.Combatants.Contains(target))
		{
			if (command.ActionKind != ManualCombatActionKind.AuxiliaryAction ||
				command.AuxiliaryAction is not IPhysicalImpactAuxiliaryAction { PhysicalImpact: { InitiateCombat: true } impact } ||
				!TryInitiateImpact(actor, target, command, impact, context))
				return ManualCombatMoveResolution.Failed("You cannot initiate that combat action against this target.");
		}

		if (!actor.Combat.Combatants.Contains(target))
		{
			return ManualCombatMoveResolution.Failed($"{target.HowSeen(actor, true)} is not in your combat.");
		}

		if (!command.IsUsableBy(actor, target))
		{
			return ManualCombatMoveResolution.Failed("You cannot use that combat command right now.");
		}

		if (!CommandExecutionScope.TryContinue(actor))
			return ManualCombatMoveResolution.Failed("Your authority to execute that order is no longer valid.");

		var result = command.ActionKind switch
		{
			ManualCombatActionKind.AuxiliaryAction => ResolveAuxiliary(actor, command, target, returnTooExhaustedMove, context),
			ManualCombatActionKind.WeaponAttack => ResolveWeaponAttack(actor, command, target, returnTooExhaustedMove),
			_ => ManualCombatMoveResolution.Failed("That manual combat command is not configured with a valid action kind.")
		};
		if (result.Success)
		{
			if (!CommandExecutionScope.TryContinue(actor))
				return ManualCombatMoveResolution.Failed("Your authority to execute that order is no longer valid.");
			actor.CombatTarget = target;
		}

		return result;
	}

	private static bool TryInitiateImpact(ICharacter actor, ICharacter target, IManualCombatCommand command,
		IPhysicalImpactProfile profile, ManualCombatActionContext? context)
	{
		var action = command.AuxiliaryAction;
		var body = actor.Body; var targetBody = target.Body;
		var instance = actor.InstanceId; var targetInstance = target.InstanceId;
		var room = actor.Location; var layer = actor.RoomLayer; var targetLayer = target.RoomLayer;
		var state = actor.State; var targetState = target.State;
		bool Pinned() => ReferenceEquals(actor.Body, body) && ReferenceEquals(target.Body, targetBody) &&
			actor.InstanceId == instance && target.InstanceId == targetInstance && actor.State == state && target.State == targetState &&
			ReferenceEquals(actor.Location, room) && ReferenceEquals(target.Location, room) && actor.RoomLayer == layer && target.RoomLayer == targetLayer &&
			CommandExecutionScope.TryContinue(actor);
		var contextualEffects = action.AuxiliaryEffects.OfType<IContextualAuxiliaryEffect>().ToList();
		if (contextualEffects.Count > 0 && (context is null || contextualEffects.Any(x =>
			!x.IsValidContext(actor, target, context, out _)) || !Pinned())) return false;
		if (!Pinned() || !command.IsUsableBy(actor, target) || !Pinned() ||
			!PhysicalImpactResolver.CanInitiate(actor, target, action, profile) || !Pinned()) return false;
		if (!actor.Race.UsableAuxiliaryMoves(actor, target, false).Any(x => ReferenceEquals(x, action)) || !Pinned()) return false;
		if (!actor.CanEngage(target) || !Pinned() || !PhysicalImpactResolver.CanInitiate(actor, target, action, profile) || !Pinned()) return false;
		return actor.Engage(target, false) && Pinned();
	}

	public static double AiWeightMultiplier(ICharacter combatant, IWeaponAttack attack)
	{
		if (combatant.CombatTarget is not ICharacter target)
		{
			return 1.0;
		}

		var multipliers = combatant.Gameworld.ManualCombatCommands
		                           .Where(x => x.NpcUsable)
		                           .Where(x => x.ActionKind == ManualCombatActionKind.WeaponAttack)
		                           .Where(x => x.WeaponAttack?.Id == attack.Id)
		                           .Where(x => x.IsUsableBy(combatant, target))
		                           .Select(combatant.CombatSettings.ManualCombatCommandWeightMultiplier)
		                           .ToList();
		return multipliers.Any() ? multipliers.Max() : 1.0;
	}

	public static double AiWeightMultiplier(ICharacter combatant, IAuxiliaryCombatAction action)
	{
		if (combatant.CombatTarget is not ICharacter target)
		{
			return 1.0;
		}

		var multipliers = combatant.Gameworld.ManualCombatCommands
		                           .Where(x => x.NpcUsable)
		                           .Where(x => x.ActionKind == ManualCombatActionKind.AuxiliaryAction)
		                           .Where(x => x.AuxiliaryAction?.Id == action.Id)
		                           .Where(x => x.IsUsableBy(combatant, target))
		                           .Select(combatant.CombatSettings.ManualCombatCommandWeightMultiplier)
		                           .ToList();
		return multipliers.Any() ? multipliers.Max() : 1.0;
	}

	private static ManualCombatMoveResolution ResolveAuxiliary(
		ICharacter actor,
		IManualCombatCommand command,
		ICharacter target,
		bool returnTooExhaustedMove,
		ManualCombatActionContext? context)
	{
		var action = command.AuxiliaryAction;
		if (action is null)
		{
			return ManualCombatMoveResolution.Failed("That manual combat command does not have an auxiliary action configured.");
		}

		if (!actor.CanSee(target) || !AuxiliaryMove.CanReachTarget(actor, target))
		{
			return ManualCombatMoveResolution.Failed("You must be able to see and reach that target at melee range.");
		}

		var available = actor.Race.UsableAuxiliaryMoves(actor, target, false)
		                     .FirstOrDefault(x => x.Id == action.Id);
		if (!CommandExecutionScope.TryContinue(actor)) return ManualCombatMoveResolution.Failed("Your authority to execute that order is no longer valid.");
		if (available is null)
		{
			return ManualCombatMoveResolution.Failed("Your race cannot use that auxiliary move against that target right now.");
		}

		var contextualEffects = available.AuxiliaryEffects.OfType<IContextualAuxiliaryEffect>().ToList();
		if (contextualEffects.Count > 0)
		{
			if (context is null)
				return ManualCombatMoveResolution.Failed("You must specify a valid target location for that move.");

			foreach (var effect in contextualEffects)
			{
				if (!effect.IsValidContext(actor, target, context, out var contextError))
					return ManualCombatMoveResolution.Failed(contextError);
			}
		}

		if (!actor.CanSee(target) || !AuxiliaryMove.CanReachTarget(actor, target))
		{
			return ManualCombatMoveResolution.Failed("You can no longer see and reach that target at melee range.");
		}

		if (available is IPhysicalImpactAuxiliaryAction { PhysicalImpact: { } impact } &&
			!PhysicalImpactResolver.CanQueue(actor, target, available, impact))
			return ManualCombatMoveResolution.Failed("You cannot attempt that physical impact in your current condition.");
		if (!CommandExecutionScope.TryContinue(actor)) return ManualCombatMoveResolution.Failed("Your authority to execute that order is no longer valid.");

		if (!actor.CanSpendStamina(AuxiliaryMove.MoveStaminaCost(actor, available)))
		{
			return returnTooExhaustedMove
				? ManualCombatMoveResolution.Resolved(new TooExhaustedMove { Assailant = actor })
				: ManualCombatMoveResolution.Failed("You are too exhausted to use that auxiliary move right now.");
		}

		return ManualCombatMoveResolution.Resolved(MultiTargetCombatMove.WrapAuxiliaryAction(actor, target, available,
			secondaryTarget => new AuxiliaryMove(actor, secondaryTarget, available, context)), context);
	}

	private static ManualCombatMoveResolution ResolveWeaponAttack(
		ICharacter actor,
		IManualCombatCommand command,
		ICharacter target,
		bool returnTooExhaustedMove)
	{
		var attack = command.WeaponAttack;
		if (attack is null)
		{
			return ManualCombatMoveResolution.Failed("That manual combat command does not have a weapon attack configured.");
		}

		if (!NaturalAttack.IsValidTarget(attack, target))
		{
			return ManualCombatMoveResolution.Failed(
				"That aquatic attack can only target a combatant aboard a vehicle with an intact exterior.");
		}

		var possibleWeaponAttacks = WeaponSourcesInPreferenceOrder(actor)
		                            .SelectMany(x => x.Weapon.WeaponType
		                                             .UsableAttacks(actor, x.Weapon.Parent, target,
			                                             x.Weapon.HandednessForWeapon(actor), false, attack.MoveType)
		                                             .Where(y => y.Id == attack.Id)
		                                             .Select(y => (x.Weapon, Attack: y)))
		                            .ToList();
		if (!CommandExecutionScope.TryContinue(actor)) return ManualCombatMoveResolution.Failed("Your authority to execute that order is no longer valid.");
		var possibleNaturalAttacks = actor.Race
		                                  .UsableNaturalWeaponAttacks(actor, target, false, attack.MoveType)
		                                  .Where(x => x.Attack.Id == attack.Id)
		                                  .ToList();
		if (!CommandExecutionScope.TryContinue(actor)) return ManualCombatMoveResolution.Failed("Your authority to execute that order is no longer valid.");

		var weaponAttack = possibleWeaponAttacks
		                   .FirstOrDefault(x => actor.CanSpendStamina(MeleeWeaponAttack.MoveStaminaCost(actor, x.Attack)));
		if (weaponAttack.Attack is not null)
		{
			return ManualCombatMoveResolution.Resolved(
				CombatMoveFactory.CreateWeaponAttack(actor, weaponAttack.Weapon, weaponAttack.Attack, target));
		}

		var naturalAttack = possibleNaturalAttacks
		                    .FirstOrDefault(x => actor.CanSpendStamina(NaturalAttackMove.MoveStaminaCost(actor, x.Attack)));
		if (naturalAttack is not null)
		{
			return ManualCombatMoveResolution.Resolved(
				CombatMoveFactory.CreateNaturalWeaponAttack(actor, naturalAttack, target));
		}

		if (possibleWeaponAttacks.Any() || possibleNaturalAttacks.Any())
		{
			return returnTooExhaustedMove
				? ManualCombatMoveResolution.Resolved(new TooExhaustedMove { Assailant = actor })
				: ManualCombatMoveResolution.Failed("You are too exhausted to use that attack right now.");
		}

		return ManualCombatMoveResolution.Failed("You do not currently have an available attack matching that manual combat command.");
	}

	internal static IEnumerable<(IMeleeWeapon Weapon, int Preference)> WeaponSourcesInPreferenceOrder(ICharacter actor)
	{
		var preferenceIndex = 0;
		foreach (var preference in actor.CombatSettings.MeleeAttackOrderPreferences)
		{
			IEnumerable<IMeleeWeapon> weapons = preference switch
			{
				MeleeAttackOrderPreference.Weapon => actor.Body.WieldedItems
				                                           .SelectNotNull(x => x.GetItemType<IMeleeWeapon>()),
				MeleeAttackOrderPreference.Implant => actor.Body.Implants
				                                            .OfType<IImplantMeleeWeapon>()
				                                            .Where(x => x.WeaponIsActive)
				                                            .Cast<IMeleeWeapon>(),
				MeleeAttackOrderPreference.Prosthetic => actor.Body.Prosthetics
				                                               .OfType<IProstheticMeleeWeapon>()
				                                               .Where(x => x.WeaponIsActive)
				                                               .Cast<IMeleeWeapon>(),
				_ => Enumerable.Empty<IMeleeWeapon>()
			};

			foreach (var weapon in weapons.Where(x => CanUseWeaponSource(actor, x)))
			{
				yield return (weapon, preferenceIndex);
			}

			preferenceIndex++;
		}
	}

	private static bool CanUseWeaponSource(ICharacter actor, IMeleeWeapon weapon)
	{
		return weapon is not null &&
		       actor.CombatSettings.ClassificationsAllowed.Contains(weapon.Classification) &&
		       (actor.Combat?.Friendly != true ||
		        weapon.Classification.In(WeaponClassification.Training, WeaponClassification.NonLethal));
	}
}

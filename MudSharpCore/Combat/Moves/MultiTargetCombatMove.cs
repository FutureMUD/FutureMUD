#nullable enable

using MudSharp.Body;
using MudSharp.Character;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class MultiTargetCombatMove : CombatMoveBase
{
	private readonly IReadOnlyList<ICombatMove> _moves;
	private readonly IReadOnlyList<ICharacter> _moveTargets;
	private readonly List<MultiTargetCombatResolution> _resolutions = [];

	protected MultiTargetCombatMove(IReadOnlyList<ICombatMove> moves, IReadOnlyList<ICharacter> moveTargets)
	{
		if (moves.Count < 2 || moves.Count != moveTargets.Count)
		{
			throw new ArgumentException("A multi-target move requires matching moves for at least two targets.",
				nameof(moves));
		}

		_moves = moves;
		_moveTargets = moveTargets;
		Assailant = moves[0].Assailant;
		foreach (var target in moveTargets)
		{
			_characterTargets.Add(target);
		}
	}

	private ICombatMove PrimaryMove => _moves[0];
	internal IReadOnlyList<MultiTargetCombatResolution> Resolutions => _resolutions;

	public override string Description => $"{PrimaryMove.Description} against multiple opponents";
	public override double StaminaCost => PrimaryMove.StaminaCost;
	public override Difficulty CheckDifficulty => PrimaryMove.CheckDifficulty;
	public override CheckType Check => PrimaryMove.Check;
	public override ExertionLevel AssociatedExertion => PrimaryMove.AssociatedExertion;
	public override Difficulty RecoveryDifficultySuccess => PrimaryMove.RecoveryDifficultySuccess;
	public override Difficulty RecoveryDifficultyFailure => PrimaryMove.RecoveryDifficultyFailure;
	public override double BaseDelay => PrimaryMove.BaseDelay;

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		_resolutions.Clear();
		var results = new List<CombatMoveResult>(_moves.Count);
		var wounds = new List<IWound>();
		var selfWounds = new List<IWound>();
		for (var i = 0; i < _moves.Count; i++)
		{
			var move = _moves[i];
			var target = _moveTargets[i];
			if (target.Combat != Assailant.Combat)
			{
				continue;
			}

			var response = target.ResponseToMove(move, Assailant);
			var result = move.ResolveMove(response);
			_resolutions.Add(new MultiTargetCombatResolution(move, response, result));
			results.Add(result);
			wounds.AddRange(result.WoundsCaused);
			selfWounds.AddRange(result.SelfWoundsCaused);
			if (move is WeaponAttackMove attackMove)
			{
				attackMove.ResolveBloodSpray(result);
			}

			if (target.Race.RaceUsesStamina && (response?.UsesStaminaWithResult(result) ?? false))
			{
				target.SpendStamina(response!.StaminaCost);
			}
		}

		if (results.Count == 0)
		{
			return CombatMoveResult.Irrelevant;
		}

		return new CombatMoveResult
		{
			MoveWasSuccessful = results.Any(x => x.MoveWasSuccessful),
			RecoveryDifficulty = results.Max(x => x.RecoveryDifficulty),
			AttackerOutcome = results[0].AttackerOutcome,
			DefenderOutcome = results[0].DefenderOutcome,
			WoundsCaused = wounds,
			SelfWoundsCaused = selfWounds
		};
	}

	internal static ICombatMove WrapWeaponAttack(ICharacter assailant, ICharacter primaryTarget,
		IWeaponAttack attack, IGameItem? weapon, Func<ICharacter, ICombatMove> moveFactory)
	{
		if (attack.MoveType.In(BuiltInCombatMoveType.ScreechAttack, BuiltInCombatMoveType.BreathWeaponAttack,
			    BuiltInCombatMoveType.ExplosiveNaturalAttack, BuiltInCombatMoveType.AquaticVehicleAttack))
		{
			return moveFactory(primaryTarget);
		}

		var targets = SelectTargets(assailant, primaryTarget, attack.MaximumTargets, IsRangedAttack(attack), attack,
			candidate => attack.UsabilityProg?.ExecuteBool(assailant, weapon, candidate) ?? true)
			.ToList();
		if (targets.Count == 1)
		{
			return moveFactory(primaryTarget);
		}

		return new MultiTargetWeaponAttackMove(targets.Select(moveFactory).ToList(), targets);
	}

	internal static ICombatMove WrapAuxiliaryAction(ICharacter assailant, ICharacter primaryTarget,
		IAuxiliaryCombatAction action, Func<ICharacter, ICombatMove> moveFactory)
	{
		var targets = SelectTargets(assailant, primaryTarget, action.MaximumTargets, false, null,
			candidate => action.UsabilityProg?.ExecuteBool(assailant, null, candidate) ?? true).ToList();
		if (targets.Count == 1)
		{
			return moveFactory(primaryTarget);
		}

		return new MultiTargetCombatMove(targets.Select(moveFactory).ToList(), targets);
	}

	internal static IEnumerable<ICharacter> SelectTargets(ICharacter assailant, ICharacter primaryTarget,
		int maximumTargets, bool ranged, IWeaponAttack? attack,
		Func<ICharacter, bool>? candidateFilter = null)
	{
		yield return primaryTarget;
		if (maximumTargets <= 1 || assailant.Combat is null)
		{
			yield break;
		}

		var remaining = maximumTargets - 1;
		foreach (var candidate in assailant.Combat.Combatants
			         .OfType<ICharacter>()
			         .Where(x => x != assailant && x != primaryTarget && !assailant.IsAlly(x))
			         .Where(x => candidateFilter?.Invoke(x) ?? true))
		{
			if (ranged)
			{
				if (!assailant.CanSee(candidate) || !IsValidRangedSecondary(assailant, candidate, attack))
				{
					continue;
				}
			}
			else if (!assailant.ColocatedWith(candidate) || candidate.CombatTarget != assailant ||
			         !candidate.MeleeRange)
			{
				continue;
			}

			yield return candidate;
			remaining--;
			if (remaining == 0)
			{
				yield break;
			}
		}
	}

	private static bool IsRangedAttack(IWeaponAttack attack)
	{
		return attack is IRangedNaturalAttack ||
		       attack.MoveType.In(BuiltInCombatMoveType.PullToMelee, BuiltInCombatMoveType.PullToMeleeUnarmed);
	}

	private static bool IsValidRangedSecondary(ICharacter assailant, ICharacter target, IWeaponAttack? attack)
	{
		if (attack?.MoveType.In(BuiltInCombatMoveType.PullToMelee, BuiltInCombatMoveType.PullToMeleeUnarmed) == true)
		{
			return assailant.ColocatedWith(target) && target.CombatTarget == assailant;
		}

		return attack is IRangedNaturalAttack rangedAttack &&
		       NaturalRangedAttackMoveBase.TargetIsInRange(assailant, target, rangedAttack.RangeInRooms);
	}
}

internal sealed record MultiTargetCombatResolution(
	ICombatMove Move,
	ICombatMove? Response,
	CombatMoveResult Result);

internal sealed class MultiTargetWeaponAttackMove : MultiTargetCombatMove, IWeaponAttackMove
{
	private readonly IWeaponAttackMove _primaryMove;

	internal MultiTargetWeaponAttackMove(IReadOnlyList<ICombatMove> moves, IReadOnlyList<ICharacter> moveTargets)
		: base(moves, moveTargets)
	{
		_primaryMove = (IWeaponAttackMove)moves[0];
	}

	public IWeaponAttack Attack => _primaryMove.Attack;
	public IBodypart TargetBodypart => _primaryMove.TargetBodypart;
	public int Reach => _primaryMove.Reach;
	public IMeleeWeapon Weapon => _primaryMove.Weapon;
	public double BloodSprayMultiplier => _primaryMove.BloodSprayMultiplier;
}

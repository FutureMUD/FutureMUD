#nullable enable

using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.Health;
using MudSharp.Magic.Powers;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp.Combat.Moves;

public class MagicPowerAttackMove : WeaponAttackMove, IMagicPowerAttackMove
{
	public IMagicAttackPower AttackPower { get; }
	private bool _committed;
	public override BuiltInCombatMoveType MoveType => AttackPower.MoveType;
	public override int Reach => AttackPower.Reach;
	public override double BaseDelay => AttackPower.BaseDelay;
	public override double StaminaCost => AttackPower.StaminaCost;
	public override CheckType Check => CheckType.GenericSkillCheck;
	public override Difficulty CheckDifficulty => Attack.Profile.BaseAttackerDifficulty;
	public override Difficulty RecoveryDifficultySuccess => Attack.RecoveryDifficultySuccess;
	public override Difficulty RecoveryDifficultyFailure => Attack.RecoveryDifficultyFailure;
	public override ExertionLevel AssociatedExertion => AttackPower.ExertionLevel;
	public override string Description => $"Using the {AttackPower.Name} power";
	public override bool UsesStaminaWithResult(CombatMoveResult result) => _committed && !ReferenceEquals(result, CombatMoveResult.Irrelevant);

	public MagicPowerAttackMove(ICharacter attacker, ICharacter target, IMagicAttackPower power) : base(power.WeaponAttack)
	{
		Assailant = attacker;
		_characterTargets.Clear();
		PrimaryTarget = target;
		AttackPower = power;
	}

	public MagicPowerAttackMove(ICharacter attacker, IEnumerable<ICharacter> targets, IMagicAttackPower power)
		: this(attacker, targets.First(), power) { }

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		defenderMove = MagicDefenseMove.Revalidate(defenderMove, this);
		var target = PrimaryCharacterTarget;
		if (_committed || target is null || !AttackPower.CanInvokePower(Assailant, target)) return CombatMoveResult.Irrelevant;
		_committed = true;
		AttackPower.UseAttackPower(this);
		defenderMove ??= new HelplessDefenseMove { Assailant = target };
		var rolls = Gameworld.GetCheck(Check).CheckAgainstAllDifficulties(Assailant, CheckDifficulty,
			AttackPower.AttackerTrait, target, Assailant.OffensiveAdvantage);
		Assailant.OffensiveAdvantage = 0;
		var attackRoll = rolls[CheckDifficulty];
		DetermineTargetBodypart(defenderMove, attackRoll);
		var attackEmote = (AttackPower as MagicAttackPower)?.AttackEmote ?? "@ direct|directs a surge of force at $1.";
		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(attackEmote, Assailant, Assailant, target), style: OutputStyle.CombatMessage));
		if (attackRoll.IsFail()) return Failed(attackRoll.Outcome);
		if (this is IRangedAttackMove)
		{
			var cover = VehicleCombatService.Instance.ResolveEffectiveRangedCover(Assailant, target);
			var difficulty = cover?.Cover.MinimumRangedDifficulty.Highest(CheckDifficulty) ?? CheckDifficulty;
			if (cover is not null && rolls[difficulty].IsFail() &&
			    (cover.Cover.CoverType == CoverType.Hard || attackRoll.Outcome == Outcome.MajorPass || rolls[difficulty].Outcome == Outcome.MinorFail))
			{
				target.Send("Your cover intercepts the attack.");
				return Failed(attackRoll.Outcome);
			}
		}
		if (defenderMove is WardDefenseMove ward)
		{
			var result = ResolveWard(ward);
			if (result.WardSucceeded) { target.Send("Your ward holds the attack at bay."); return Failed(attackRoll.Outcome); }
			var beaten = new WardBeaten(target, target.Combat);
			target.AddEffect(beaten);
			try { defenderMove = target.ResponseToMove(this, Assailant); }
			finally { target.RemoveEffect(beaten); }
		}
		CheckOutcome? defenseRoll = null;
		switch (defenderMove)
		{
			case MagicDefenseMove magic:
				if (magic.TryDefend(this, attackRoll, out var defenseResult)) return defenseResult;
				break;
			case BlockMove block:
				defenseRoll = Gameworld.GetCheck(block.Check).Check(target,
					AttackPower.BaseBlockDifficulty.StageUp(block.DifficultStageUps), block.Shield.ShieldType.BlockTrait,
					Assailant, target.DefensiveAdvantage + block.Shield.ShieldType.BlockBonus);
				break;
			case ParryMove parry:
				defenseRoll = Gameworld.GetCheck(parry.Check).Check(target,
					AttackPower.BaseParryDifficulty.StageUp(parry.DifficultStageUps), parry.Weapon.WeaponType.ParryTrait,
					Assailant, target.DefensiveAdvantage);
				break;
			case DodgeMove dodge:
				defenseRoll = Gameworld.GetCheck(dodge.Check).Check(target,
					AttackPower.BaseDodgeDifficulty.StageUp(dodge.DifficultStageUps), null, Assailant, target.DefensiveAdvantage);
				break;
			case DodgeRangeMove dodge:
				defenseRoll = Gameworld.GetCheck(dodge.Check).Check(target,
					AttackPower.BaseDodgeDifficulty, null, Assailant, target.DefensiveAdvantage);
				break;
		}
		target.DefensiveAdvantage = 0;
		var opposed = new OpposedOutcome(attackRoll, defenseRoll?.Outcome ?? Outcome.NotTested);
		if (defenseRoll is not null && opposed.Outcome != OpposedOutcomeDirection.Proponent)
		{
			target.Send("You turn aside the attack.");
			return Failed(attackRoll.Outcome, defenseRoll.Outcome);
		}
		var wounds = new List<IWound>();
		if (AttackPower.DealsDamage)
		{
			IDamage? damage = new Damage
			{
				ActorOrigin = Assailant, Bodypart = TargetBodypart, DamageType = Attack.Profile.DamageType,
				AngleOfIncidentRadians = Attack.Profile.BaseAngleOfIncidence,
				DamageAmount = Math.Max(0, EvaluateAttackFormula(Attack.Profile.DamageExpression, Assailant, TraitBonusContext.ArmedDamageCalculation, (int)opposed.Degree, 5)),
				PainAmount = Math.Max(0, EvaluateAttackFormula(Attack.Profile.PainExpression, Assailant, TraitBonusContext.ArmedDamageCalculation, (int)opposed.Degree, 5)),
				StunAmount = Math.Max(0, EvaluateAttackFormula(Attack.Profile.StunExpression, Assailant, TraitBonusContext.ArmedDamageCalculation, (int)opposed.Degree, 5)),
				PenetrationOutcome = Gameworld.GetCheck(this is IRangedAttackMove ? CheckType.RangedWeaponPenetrateCheck : CheckType.MeleeWeaponPenetrateCheck)
					.Check(Assailant, Difficulty.Normal, AttackPower.AttackerTrait, target)
			};
			if (defenderMove is MagicDefenseMove magic) damage = magic.Absorb(damage);
			if (damage is null) return Failed(attackRoll.Outcome);
			wounds.AddRange(target.PassiveSufferDamage(damage));
			wounds.ProcessPassiveWounds();
		}
		MagicAttackEffectResolver.Apply(Assailant, target, AttackPower);
		AttackPower.ApplyAttackSpell(Assailant, target, attackRoll);
		return new CombatMoveResult { MoveWasSuccessful = true, AttackerOutcome = attackRoll.Outcome,
			DefenderOutcome = defenseRoll?.Outcome ?? Outcome.NotTested, WoundsCaused = wounds,
			RecoveryDifficulty = RecoveryDifficultySuccess };
	}

	private CombatMoveResult Failed(Outcome attack, Outcome defense = Outcome.NotTested) => new()
	{ AttackerOutcome = attack, DefenderOutcome = defense, RecoveryDifficulty = RecoveryDifficultyFailure };
}

public sealed class RangedMagicPowerAttackMove(ICharacter attacker, ICharacter target, IMagicAttackPower power)
	: MagicPowerAttackMove(attacker, target, power), IRangedAttackMove;

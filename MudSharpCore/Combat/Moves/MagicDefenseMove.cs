#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.Health;
using MudSharp.Magic.Powers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public sealed class MagicDefenseMove : CombatMoveBase, IDefenseMove
{
	public MagicDefense Effect { get; }
	public MagicDefensePower Power => Effect.Power;
	public bool Committed { get; private set; }
	private ICombatMove? _fallback;
	private bool _usingFallback;
	public int DifficultStageUps => 0;
	public override double StaminaCost => _usingFallback ? _fallback?.StaminaCost ?? 0 : Power.ReactionStamina;
	public override CheckType Check => CheckType.GenericSkillCheck;
	public override Difficulty CheckDifficulty => Power.DefenseDifficulty;
	public override string Description => $"Defending with {Power.Name}";
	public override bool UsesStaminaWithResult(CombatMoveResult result) => _usingFallback ? _fallback?.UsesStaminaWithResult(result) == true : Committed;
	public static ICombatMove? Revalidate(ICombatMove? defense, ICombatMove attack)
	{
		if (defense is not MagicDefenseMove magic || magic.Committed ||
		    magic.Effect.Available && magic.Assailant.Effects.Contains(magic.Effect) && magic.Power.CanDefend(magic.Assailant, attack)) return defense;
		magic._usingFallback = true;
		return magic._fallback ?? new HelplessDefenseMove { Assailant = magic.Assailant };
	}
	public MagicDefenseMove(ICharacter defender, MagicDefense effect) { Assailant = defender; Effect = effect; }
	public override CombatMoveResult ResolveMove(ICombatMove defenderMove) => throw new InvalidOperationException("A magical defense resolves against its incoming attack.");
	public void ResolveDefenseUsed(OpposedOutcome outcome) { }

	public bool TryDefend(ICombatMove attack, CheckOutcome roll, out CombatMoveResult result)
	{
		result = new CombatMoveResult { AttackerOutcome = roll.Outcome, RecoveryDifficulty = attack.RecoveryDifficultyFailure };
		if (Committed || !Effect.Available || !Assailant.Effects.Contains(Effect) || !Power.CanDefend(Assailant, attack)) return false;
		if (roll.IsFail()) return true;
		Committed = true;
		MagicDefenseDamageScope.Bind(this);
		Power.ConsumeReaction(Assailant);
		if (Power.DefenseMode == MagicDefenseMode.Absorption) return false;
		var defense = Gameworld.GetCheck(Check).Check(Assailant, CheckDifficulty, Power.DefenseTrait, attack.Assailant, Assailant.DefensiveAdvantage);
		Assailant.DefensiveAdvantage = 0;
		var won = new OpposedOutcome(roll, defense).Outcome != OpposedOutcomeDirection.Proponent;
		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(won ? Power.SuccessEmote : Power.FailEmote,
			Assailant, Assailant, attack.Assailant), style: OutputStyle.CombatMessage));
		if (won && Power.DefenseMode == MagicDefenseMode.Charged) Effect.UseCharge();
		result = new CombatMoveResult { AttackerOutcome = roll.Outcome, DefenderOutcome = defense.Outcome, RecoveryDifficulty = attack.RecoveryDifficultyFailure };
		return won;
	}

	public IDamage? Absorb(IDamage damage)
	{
		if (!Committed || Power.DefenseMode != MagicDefenseMode.Absorption ||
		    (Power.DamageTypes.Count > 0 && !Power.DamageTypes.Contains(damage.DamageType))) return damage;
		var incoming = Math.Max(damage.DamageAmount, Math.Max(damage.PainAmount, damage.StunAmount));
		if (incoming <= 0) return damage;
		var absorbed = Effect.UseCapacity(incoming);
		var scale = Math.Clamp(1 - absorbed / incoming, 0, 1);
		if (absorbed > 0) Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(Power.SuccessEmote,
			Assailant, Assailant, damage.ActorOrigin), style: OutputStyle.CombatMessage));
		return scale <= 0 ? null : new Damage(damage, scale);
	}

	public static MagicDefenseThreat Classify(ICombatMove attack) => attack switch
	{
		StartClinchMove or BreakClinchMove or InitiateGrappleMove or ExtendGrappleMove or AuxiliaryMove or ForcedMovementMove or TakedownMove or WrenchingAttack or StrangleAttack => MagicDefenseThreat.Control,
		MagicPowerAttackMove magic => MagicDefenseThreat.Magic | (magic is IRangedAttackMove ? MagicDefenseThreat.Ranged : MagicDefenseThreat.None),
		RangedWeaponAttackBase => MagicDefenseThreat.Weapon | MagicDefenseThreat.Ranged,
		NaturalRangedAttackMoveBase => MagicDefenseThreat.Natural | MagicDefenseThreat.Ranged,
		ScreechAttackMove => MagicDefenseThreat.None,
		ClinchNaturalAttackMove => MagicDefenseThreat.Natural,
		ClinchAttackMove => MagicDefenseThreat.Weapon,
		NaturalAttackMove => MagicDefenseThreat.Natural,
		MeleeWeaponAttack => MagicDefenseThreat.Weapon,
		_ => MagicDefenseThreat.None
	};

	public static ICombatMove? Select(ICharacter defender, ICombatMove attack, ICombatMove? mundane)
	{
		var candidates = defender.EffectsOfType<MagicDefense>()
			.Where(x => x.Available && x.Power.CanDefend(defender, attack))
			.Select(x => (Effect: x, Chance: x.Power.DefenseMode == MagicDefenseMode.Absorption ? 100.0 :
				defender.Gameworld.GetCheck(CheckType.GenericSkillCheck).TargetNumber(defender, x.Power.DefenseDifficulty, x.Power.DefenseTrait, attack.Assailant)))
			.OrderByDescending(x => x.Chance).ThenBy(x => x.Effect.Power.Id).ToList();
		if (candidates.Count == 0) return mundane;
		var preferred = defender.PreferredDefenseType;
		if (preferred != DefenseType.None && preferred != DefenseType.Magic && mundane is not null && mundane is not HelplessDefenseMove && mundane is not TooExhaustedMove) return mundane;
		if (preferred == DefenseType.Magic || candidates[0].Chance > MundaneChance(defender, attack, mundane)) return new MagicDefenseMove(defender, candidates[0].Effect) { _fallback = mundane };
		return mundane;
	}

	private static double MundaneChance(ICharacter actor, ICombatMove attack, ICombatMove? defense)
	{
		if (defense is null or HelplessDefenseMove or TooExhaustedMove) return double.NegativeInfinity;
		ITraitDefinition? trait = null;
		var difficulty = defense.CheckDifficulty;
		var profile = (attack as IWeaponAttackMove)?.Attack?.Profile;
		var ranged = (attack as IRangedWeaponAttackMove)?.Weapon;
		switch (defense)
		{
			case BlockMove block: trait = block.Shield.ShieldType.BlockTrait; difficulty = (profile?.BaseBlockDifficulty ?? ranged?.BaseBlockDifficulty ?? difficulty).ApplyBonus(block.Shield.ShieldType.BlockBonus); break;
			case ParryMove parry: trait = parry.Weapon.WeaponType.ParryTrait; difficulty = (profile?.BaseParryDifficulty ?? difficulty).ApplyBonus(parry.Weapon.WeaponType.ParryBonus); break;
			case DodgeMove or DodgeRangeMove: difficulty = profile?.BaseDodgeDifficulty ?? ranged?.BaseDodgeDifficulty ?? difficulty; break;
			case WardDefenseMove: return 100.0;
		}
		if (defense is IDefenseMove dm) difficulty = difficulty.StageUp(dm.DifficultStageUps);
		return actor.Gameworld.GetCheck(defense.Check).TargetNumber(actor, difficulty, trait, attack.Assailant);
	}
}

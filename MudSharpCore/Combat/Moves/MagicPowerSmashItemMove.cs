#nullable enable

using MudSharp.GameItems;
using MudSharp.Body;
using MudSharp.Health;
using MudSharp.Magic.Powers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;

namespace MudSharp.Combat.Moves;

public sealed class MagicPowerSmashItemMove : WeaponAttackMove, IMagicPowerAttackMove
{
	private bool _committed;
	public IMagicSmashPower Power { get; }
	public IMagicAttackPower AttackPower => Power;
	public IGameItem Target { get; }
	public MagicPowerSmashItemMove(ICharacter actor, IGameItem target, IMagicSmashPower power) : base(power.WeaponAttack)
	{ Assailant = actor; Target = target; Power = power; }
	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.MagicPowerSmashItem;
	public override double StaminaCost => Power.StaminaCost;
	public override double BaseDelay => Power.BaseDelay;
	public override int Reach => Power.Reach;
	public override ExertionLevel AssociatedExertion => Power.ExertionLevel;
	public override CheckType Check => CheckType.GenericSkillCheck;
	public override Difficulty CheckDifficulty => Attack.Profile.BaseAttackerDifficulty;
	public override Difficulty RecoveryDifficultySuccess => Attack.RecoveryDifficultySuccess;
	public override Difficulty RecoveryDifficultyFailure => Attack.RecoveryDifficultyFailure;
	public override string Description => $"Smashing {Target.HowSeen(Assailant)} with {Power.Name}";
	public override bool UsesStaminaWithResult(CombatMoveResult result) => _committed && !ReferenceEquals(result, CombatMoveResult.Irrelevant);
	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (_committed || !Power.CanInvokePower(Assailant, Target)) return CombatMoveResult.Irrelevant;
		_committed = true;
		Power.UseAttackPower(this);
		CrimeExtensions.CheckPossibleCrimeAllAuthorities(Assailant, CrimeTypes.Vandalism, null, Target, "");
		var roll = Gameworld.GetCheck(Check).Check(Assailant, CheckDifficulty, Power.AttackerTrait, Target);
		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote((Power as MagicAttackPower)?.AttackEmote ?? "@ direct|directs destructive force at $1.", Assailant, Assailant, Target), style: OutputStyle.CombatMessage));
		if (roll.IsFail()) return new CombatMoveResult { AttackerOutcome = roll.Outcome, RecoveryDifficulty = RecoveryDifficultyFailure };
		var wounds = new List<IWound>();
		Power.ApplyAttackSpell(Assailant, Target, roll);
		if (Power.DealsDamage && !Target.Deleted && !Target.Destroyed)
		{
			var degree = (int)new OpposedOutcome(roll, Outcome.NotTested).Degree;
			var damage = Math.Max(0, Attack.Profile.DamageExpression.EvaluateWith(Assailant, values: [("degree", degree), ("quality", 5)])) * 2 * Attack.Profile.BaseAngleOfIncidence / Math.PI;
			wounds.AddRange(Target.PassiveSufferDamage(new Damage { ActorOrigin = Assailant, DamageType = Attack.Profile.DamageType,
				DamageAmount = damage, AngleOfIncidentRadians = Attack.Profile.BaseAngleOfIncidence, PenetrationOutcome = Outcome.NotTested }));
			wounds.ProcessPassiveWounds();
			Assailant.Send($"Your attack causes {wounds.Select(x => x.Describe(WoundExaminationType.Glance, Outcome.MajorPass)).ListToString().IfNullOrWhiteSpace("no real damage")} to {Target.HowSeen(Assailant)}.");
		}
		return new CombatMoveResult { MoveWasSuccessful = true, AttackerOutcome = roll.Outcome, RecoveryDifficulty = RecoveryDifficultySuccess, WoundsCaused = wounds };
	}
}

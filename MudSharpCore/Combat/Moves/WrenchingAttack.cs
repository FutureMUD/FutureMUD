using MudSharp.Body;
using MudSharp.Health;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class WrenchingAttack : NaturalAttackMove
{
    public WrenchingAttack(ICharacter owner, INaturalAttack attack, ICharacter target, IBodypart targetBodypart) : base(
        owner, attack, target)
    {
        TargetBodypart = targetBodypart;
        CharacterTarget = target;
    }

    public ICharacter CharacterTarget { get; set; }

    public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.WrenchAttack;

    public override CheckType Check => CheckType.WrenchAttackCheck;

    public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
    {
        CheckOutcome attackRoll = Gameworld.GetCheck(Check)
                                  .Check(Assailant, CheckDifficulty, CharacterTarget, null,
                                      Assailant.OffensiveAdvantage);
        OpposedOutcome outcome = new(attackRoll, Outcome.NotTested);
        OpposedOutcomeDegree degree = outcome.Degree;
        string attackEmote =
            string.Format(
                      Gameworld.CombatMessageManager.GetMessageFor(Assailant, CharacterTarget, null, Attack,
                          BuiltInCombatMoveType.WrenchAttack, attackRoll.Outcome, null), Bodypart.FullDescription(),
                      TargetBodypart.FullDescription().ToLowerInvariant(),
                      CharacterTarget.Body.GetLimbFor(TargetBodypart).Name.ToLowerInvariant())
                  .Replace("@hand", Bodypart.Alignment.LeftRightOnly().Describe().ToLowerInvariant());
        Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(attackEmote, Assailant, Assailant, CharacterTarget),
            style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

        List<IWound> wounds = new();
        int formulaDegree = (int)degree;
        int quality = (int)Assailant.NaturalWeaponQuality(NaturalAttack);

        Damage finalDamage = new()
        {
            ActorOrigin = Assailant,
            LodgableItem = null,
            ToolOrigin = null,
            AngleOfIncidentRadians = Attack.Profile.BaseAngleOfIncidence,
            Bodypart = TargetBodypart,
            DamageAmount =
                Attack.Profile.DamageExpression.EvaluateWith(Assailant,
                    values: [("degree", formulaDegree), ("quality", quality)]),
            DamageType = DamageType.Wrenching,
            PainAmount =
                Attack.Profile.PainExpression.EvaluateWith(Assailant,
                    values: [("degree", formulaDegree), ("quality", quality)]),
            PenetrationOutcome = Outcome.NotTested,
            ShockAmount = 0,
            StunAmount = Attack.Profile.DamageExpression.EvaluateWith(Assailant,
                values: [("degree", formulaDegree), ("quality", quality)])
        };

        wounds.AddRange(CharacterTarget.SufferDamage(finalDamage));

        return new CombatMoveResult
        {
            MoveWasSuccessful = true,
            AttackerOutcome = attackRoll,
            DefenderOutcome = Outcome.NotTested,
            RecoveryDifficulty = attackRoll.IsPass() ? RecoveryDifficultySuccess : RecoveryDifficultyFailure,
            WoundsCaused = wounds
        };
    }
}

using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Health;
using MudSharp.RPG.Checks;

using MudSharp.Construction;

namespace MudSharp.Combat.Moves;

public class ScreechAttackMove : NaturalAttackMove
{
    public ScreechAttackMove(ICharacter owner, INaturalAttack attack, ICharacter target) : base(owner, attack, target)
    {
    }

    #region Overrides of NaturalAttackMove

    public override CheckType Check => CheckType.ScreechAttack;
    public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.ScreechAttack;

    #endregion

    #region Overrides of NaturalAttackMove

    public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
    {
        CheckOutcome attackRoll = Gameworld.GetCheck(Check)
                                  .Check(Assailant, CheckDifficulty, default(IPerceivable), null,
                                      Assailant.OffensiveAdvantage);

        string attackEmote =
            string.Format(
                      Gameworld.CombatMessageManager.GetMessageFor(Assailant, null, null, Attack,
                          MoveType, attackRoll.Outcome, Bodypart),
                      Bodypart.FullDescription())
                  .Replace("@hand", Bodypart.Alignment.LeftRightOnly().Describe().ToLowerInvariant());


        Form.Shape.IBodypartShape shape = ((IFixedBodypartWeaponAttack)Attack).Bodypart;
		List<ICharacter> targets = Assailant.LocalThingsAndProximities()
		                       .Where(x => x.Proximity <= Proximity.VeryDistant)
		                       .Select(x => x.Thing)
		                       .OfType<ICharacter>()
		                       .Where(x => x.RoomLayer == Assailant.RoomLayer)
                               .Where(x => x.Body.Bodyparts.Any(y => y.Organs.Any(z => z is EarProto)))
			                       .Distinct()
                               .ToList();
        int formulaDegree = attackRoll.Outcome.CheckDegrees();
        int quality = (int)Assailant.NaturalWeaponQuality(NaturalAttack);

        Damage baseDamage = new()
        {
            ActorOrigin = Assailant,
            LodgableItem = null,
            ToolOrigin = null,
            AngleOfIncidentRadians = Attack.Profile.BaseAngleOfIncidence,
            Bodypart = null,
            DamageAmount =
                Attack.Profile.DamageExpression.EvaluateWith(Assailant,
                    values: [("degree", formulaDegree), ("quality", quality)]),
            DamageType = Attack.Profile.DamageType,
            PainAmount =
                Attack.Profile.PainExpression.EvaluateWith(Assailant,
                    values: [("degree", formulaDegree), ("quality", quality)]),
            PenetrationOutcome = Outcome.NotTested,
            ShockAmount = 0,
            StunAmount =
                Attack.Profile.DamageExpression.EvaluateWith(Assailant,
                    values: [("degree", formulaDegree), ("quality", quality)])
        };

        List<IWound> wounds = new();
        foreach (ICharacter target in targets)
        {
            foreach (IBodypart bodypart in target.Body.Bodyparts
                                       .Where(x => x.Shape == shape || x.Organs.Any(y => y.Shape == shape)).ToList())
            {
                Damage damage = new(baseDamage) { Bodypart = bodypart };
                wounds.AddRange(target.PassiveSufferDamage(damage));
            }
        }

        Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(attackEmote, Assailant, Assailant)));
        wounds.ProcessPassiveWounds();
        return new CombatMoveResult
        {
            AttackerOutcome = attackRoll,
            DefenderOutcome = Outcome.NotTested,
            MoveWasSuccessful = wounds.Any(),
            RecoveryDifficulty = attackRoll.IsPass() ? RecoveryDifficultySuccess : RecoveryDifficultyFailure,
            WoundsCaused = wounds
        };
    }

    #endregion
}

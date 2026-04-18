using MudSharp.Body;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        List<ICharacter> targets = Assailant.Location.LayerCharacters(Assailant.RoomLayer)
                               .Where(x => x.Body.Bodyparts.Any(y => y.Organs.Any(z => z is EarProto)))
                               .ToList();
        Attack.Profile.DamageExpression.Formula.Parameters["degree"] = attackRoll.Outcome.CheckDegrees();
        Attack.Profile.DamageExpression.Formula.Parameters["quality"] =
            (int)Assailant.NaturalWeaponQuality(NaturalAttack);
        Attack.Profile.StunExpression.Formula.Parameters["degree"] = attackRoll.Outcome.CheckDegrees();
        Attack.Profile.StunExpression.Formula.Parameters["quality"] =
            (int)Assailant.NaturalWeaponQuality(NaturalAttack);
        Attack.Profile.PainExpression.Formula.Parameters["degree"] = attackRoll.Outcome.CheckDegrees();
        Attack.Profile.PainExpression.Formula.Parameters["quality"] =
            (int)Assailant.NaturalWeaponQuality(NaturalAttack);

        Damage baseDamage = new()
        {
            ActorOrigin = Assailant,
            LodgableItem = null,
            ToolOrigin = null,
            AngleOfIncidentRadians = Attack.Profile.BaseAngleOfIncidence,
            Bodypart = null,
            DamageAmount =
                Attack.Profile.DamageExpression.Evaluate(Assailant),
            DamageType = Attack.Profile.DamageType,
            PainAmount =
                Attack.Profile.PainExpression.Evaluate(Assailant),
            PenetrationOutcome = Outcome.NotTested,
            ShockAmount = 0,
            StunAmount =
                Attack.Profile.DamageExpression.Evaluate(Assailant)
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
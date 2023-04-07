using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.RPG.Checks;
using MudSharp.Body;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.PerceptionEngine;
using MudSharp.Health;
using MudSharp.Framework;

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
		var attackRoll = Gameworld.GetCheck(Check)
		                          .Check(Assailant, CheckDifficulty, CharacterTarget, null,
			                          Assailant.OffensiveAdvantage);
		var outcome = new OpposedOutcome(attackRoll, Outcome.NotTested);
		var degree = outcome.Degree;
		var attackEmote =
			string.Format(
				      Gameworld.CombatMessageManager.GetMessageFor(Assailant, CharacterTarget, null, Attack,
					      BuiltInCombatMoveType.WrenchAttack, attackRoll.Outcome, null), Bodypart.FullDescription(),
				      TargetBodypart.FullDescription().ToLowerInvariant(),
				      CharacterTarget.Body.GetLimbFor(TargetBodypart).Name.ToLowerInvariant())
			      .Replace("@hand", Bodypart.Alignment.LeftRightOnly().Describe().ToLowerInvariant());
		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(attackEmote, Assailant, Assailant, CharacterTarget),
			style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

		var wounds = new List<IWound>();
		Attack.Profile.DamageExpression.Formula.Parameters["degree"] = (int)degree;
		Attack.Profile.DamageExpression.Formula.Parameters["quality"] =
			(int)Assailant.NaturalWeaponQuality(NaturalAttack);
		Attack.Profile.StunExpression.Formula.Parameters["degree"] = (int)degree;
		Attack.Profile.StunExpression.Formula.Parameters["quality"] =
			(int)Assailant.NaturalWeaponQuality(NaturalAttack);
		Attack.Profile.PainExpression.Formula.Parameters["degree"] = (int)degree;
		Attack.Profile.PainExpression.Formula.Parameters["quality"] =
			(int)Assailant.NaturalWeaponQuality(NaturalAttack);

		var finalDamage = new Damage
		{
			ActorOrigin = Assailant,
			LodgableItem = null,
			ToolOrigin = null,
			AngleOfIncidentRadians = Attack.Profile.BaseAngleOfIncidence,
			Bodypart = TargetBodypart,
			DamageAmount =
				Attack.Profile.DamageExpression.Evaluate(Assailant),
			DamageType = DamageType.Wrenching,
			PainAmount =
				Attack.Profile.PainExpression.Evaluate(Assailant),
			PenetrationOutcome = Outcome.NotTested,
			ShockAmount = 0,
			StunAmount = Attack.Profile.DamageExpression.Evaluate(Assailant)
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
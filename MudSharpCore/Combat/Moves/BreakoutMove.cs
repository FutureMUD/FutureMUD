using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Body.Traits;
using MudSharp.Character.Heritage;

namespace MudSharp.Combat.Moves;

public class BreakoutMove : CombatMoveBase
{
	private static TraitExpression _attackerStrengthExpression;
	private static TraitExpression _defenderStrengthExpression;

	public TraitExpression AttackerStrengthExpression => _attackerStrengthExpression ??= new TraitExpression(
		Gameworld.GetStaticConfiguration("BreakoutAttackerStrengthExpression"),
		Gameworld);

	public TraitExpression DefenderStrengthExpression => _defenderStrengthExpression ??= new TraitExpression(
		Gameworld.GetStaticConfiguration("BreakoutDefenderStrengthExpression"),
		Gameworld);


	public BreakoutMove(ICharacter assailant)
	{
		Assailant = assailant;
	}

	public override string Description => "Attempting to break out of a grapple by pure strength";

	private (Difficulty Breakout, Difficulty Oppose) GetBreakoutDifficulty(ICharacter grappler)
	{
		AttackerStrengthExpression.Formula.Parameters["weight"] = Assailant.Weight;
		AttackerStrengthExpression.Formula.Parameters["height"] = Assailant.Height;
		AttackerStrengthExpression.Formula.Parameters["limbs"] = Assailant
		                                                         .CombinedEffectsOfType<ILimbIneffectiveEffect>()
		                                                         .Count(
			                                                         x => x.Reason == LimbIneffectiveReason.Grappling);
		DefenderStrengthExpression.Formula.Parameters["weight"] = grappler.Weight;
		DefenderStrengthExpression.Formula.Parameters["height"] = grappler.Height;
		DefenderStrengthExpression.Formula.Parameters["limbs"] = Assailant
		                                                         .CombinedEffectsOfType<ILimbIneffectiveEffect>()
		                                                         .Count(
			                                                         x => x.Reason == LimbIneffectiveReason.Grappling);

		var attackerStrength =
			AttackerStrengthExpression.Evaluate(Assailant, context: TraitBonusContext.BreakoutFromGrapple);
		var defenderStrength =
			DefenderStrengthExpression.Evaluate(grappler, context: TraitBonusContext.OpposeBreakoutFromGrapple);

		var attackerSize = Assailant.CurrentContextualSize(SizeContext.GrappleAttack);
		var defenderSize = Assailant.CurrentContextualSize(SizeContext.GrappleDefense);

		var offset =
			(attackerStrength - defenderStrength) * Gameworld.GetStaticDouble("BreakoutOffsetPerStrengthDifference") +
			(attackerSize - defenderSize) * Gameworld.GetStaticDouble("BreakoutOffsetPerSizeDifference");
		return (Difficulty.Normal.ApplyBonus(offset), Difficulty.Normal.ApplyBonus(-1 * offset));
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		Assailant.SpendStamina(Assailant.Gameworld.GetStaticDouble("BreakoutFromGrappleStaminaCost"));
		var grapples = Assailant.CombinedEffectsOfType<IBeingGrappled>().ToList();
		var check = Assailant.Gameworld.GetCheck(CheckType.BreakoutCheck);
		var result = check.CheckAgainstAllDifficulties(Assailant, Difficulty.Normal, null);
		var emote = Gameworld.CombatMessageManager.GetMessageFor(Assailant, null, null, null,
			BuiltInCombatMoveType.Breakout, result[Difficulty.Normal], null);
		Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(emote, Assailant, Assailant)));
		var successes = false;
		foreach (var grapple in grapples)
		{
			grapple.Grappling.CharacterOwner.SpendStamina(
				Assailant.Gameworld.GetStaticDouble("OpposeBreakoutFromGrappleStaminaCost"));
			var opponentCheck = Assailant.Gameworld.GetCheck(CheckType.OpposeBreakoutCheck);
			var (breakoutDifficulty, opposeDifficulty) = GetBreakoutDifficulty(grapple.Grappling.CharacterOwner);
			var opponentResult = grapple.Grappling.CharacterOwner.CurrentStamina <= 0.0
				? CheckOutcome.NotTestedAllDifficulties(CheckType.OpposeBreakoutCheck)
				: opponentCheck.CheckAgainstAllDifficulties(grapple.Grappling.CharacterOwner, opposeDifficulty, null);

			var opposed = new OpposedOutcome(result, opponentResult, breakoutDifficulty, opposeDifficulty);
			if (opposed.Outcome == OpposedOutcomeDirection.Proponent ||
			    opposed.Outcome == OpposedOutcomeDirection.Stalemate)
			{
				var struggleResult = grapple.Grappling.StruggleResult(opposed.Degree);
				if (!struggleResult.StillGrappled)
				{
					Assailant.OutputHandler.Handle(new EmoteOutput(
						new Emote($"@ get|gets completely free from $1's grapple!", Assailant, Assailant,
							grapple.Grappling.Owner), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
					grapple.Grappling.CharacterOwner.RemoveEffect(grapple.Grappling, true);
				}
				else
				{
					Assailant.OutputHandler.Handle(new EmoteOutput(
						new Emote(
							$"@ manage|manages to get &0's {struggleResult.FreedLimbs.Select(x => x.Name.ToLowerInvariant()).ListToString()} free from $1's grapple!",
							Assailant, Assailant, grapple.Grappling.Owner), style: OutputStyle.CombatMessage,
						flags: OutputFlags.InnerWrap));
				}

				successes = true;
				continue;
			}

			Assailant.OutputHandler.Handle(new EmoteOutput(
				new Emote($"@ don't|doesn't manage to get any limbs free from $1's grapple!", Assailant, Assailant,
					grapple.Grappling.Owner), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		}

		if (Assailant.Combat == null)
		{
			Assailant.AddEffect(
				new CommandDelay(Assailant, "Struggle",
					onExpireAction: () =>
					{
						Assailant.Send("You feel as if you could try to struggle free of your captors again.");
					}), TimeSpan.FromSeconds(10));
		}

		return new CombatMoveResult
		{
			AttackerOutcome = result[Difficulty.Normal],
			DefenderOutcome = Outcome.NotTested,
			MoveWasSuccessful = successes,
			RecoveryDifficulty = Difficulty.Hard
		};
	}
}
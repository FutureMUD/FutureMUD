using System;
using System.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using static MudSharp.Effects.Concrete.Dragging;

namespace MudSharp.Combat.Moves;

public class StruggleMove : CombatMoveBase
{
	public override string Description => throw new NotImplementedException();

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var grapple = Assailant.CombinedEffectsOfType<IBeingGrappled>().FirstOrDefault();
		if (grapple != null)
		{
			return StruggleFreeFromGrapple(grapple);
		}

		var drag = Assailant.CombinedEffectsOfType<DragTarget>().FirstOrDefault()?.Drag;
		if (drag != null)
		{
			return StruggleFreeFromDrag(drag);
		}

		return CombatMoveResult.Irrelevant;
	}

	private Difficulty GetOpposeStruggleDifficulty(ICharacter struggler)
	{
		var baseDifficulty = Difficulty.Normal;
		// TODO - effects
		// TODO - merits
		return baseDifficulty;
	}

	private CombatMoveResult StruggleFreeFromGrapple(IBeingGrappled grapple)
	{
		Assailant.SpendStamina(Assailant.Gameworld.GetStaticDouble("StruggleFreeFromGrappleStaminaCost"));
		grapple.Grappling.CharacterOwner.SpendStamina(
			Assailant.Gameworld.GetStaticDouble("OpposeStruggleFreeFromGrappleStaminaCost"));

		var check = Assailant.Gameworld.GetCheck(CheckType.StruggleFreeFromGrapple);
		var result = check.Check(Assailant, grapple.StruggleDifficulty, grapple.Grappling.Owner);
		var opponentCheck = Assailant.Gameworld.GetCheck(CheckType.OpposeStruggleFreeFromGrapple);
		var opponentResult = grapple.Grappling.CharacterOwner.CurrentStamina <= 0.0
			? CheckOutcome.NotTested(CheckType.OpposeStruggleFreeFromGrapple)
			: opponentCheck.Check(grapple.Grappling.CharacterOwner, GetOpposeStruggleDifficulty(Assailant));

		var opposed = new OpposedOutcome(result, opponentResult);
		if (opposed.Outcome == OpposedOutcomeDirection.Proponent ||
		    opposed.Outcome == OpposedOutcomeDirection.Stalemate)
		{
			var struggleResult = grapple.Grappling.StruggleResult(opposed.Degree);
			if (!struggleResult.StillGrappled)
			{
				Assailant.OutputHandler.Handle(new EmoteOutput(
					new Emote($"@ struggle|struggles to break free from $1's grapple and get|gets completely free!",
						Assailant, Assailant, grapple.Grappling.Owner), style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap));
				grapple.Grappling.CharacterOwner.RemoveEffect(grapple.Grappling, true);
			}
			else
			{
				Assailant.OutputHandler.Handle(new EmoteOutput(
					new Emote(
						$"@ struggle|struggles to break free from $1's grapple and manage|manages to get &0's {struggleResult.Item2.Select(x => x.Name.ToLowerInvariant()).ListToString()} free!",
						Assailant, Assailant, grapple.Grappling.Owner), style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap));
			}

			return new CombatMoveResult
			{
				AttackerOutcome = result.Outcome,
				DefenderOutcome = opponentResult.Outcome,
				MoveWasSuccessful = true,
				RecoveryDifficulty = Difficulty.Easy
			};
		}

		Assailant.OutputHandler.Handle(new EmoteOutput(
			new Emote($"@ struggle|struggles to break free from $1's grapple, but cannot get free!", Assailant,
				Assailant, grapple.Grappling.Owner), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
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
			AttackerOutcome = result.Outcome,
			DefenderOutcome = opponentResult.Outcome,
			MoveWasSuccessful = false,
			RecoveryDifficulty = Difficulty.Hard
		};
	}

	private CombatMoveResult StruggleFreeFromDrag(IDragging drag)
	{
		Assailant.SpendStamina(Assailant.Gameworld.GetStaticDouble("StruggleFreeFromDragStaminaCost"));

		var struggleCheck = Assailant.Gameworld.GetCheck(CheckType.StruggleFreeFromDrag);
		var result = struggleCheck.Check(Assailant, Difficulty.Normal);
		var opponentResult = Outcome.MajorFail;
		var opponentCheck = Assailant.Gameworld.GetCheck(CheckType.OpposeStruggleFreeFromDrag);
		foreach (var dragger in drag.Draggers)
		{
			if (!dragger.Character.CanSpendStamina(
				    Assailant.Gameworld.GetStaticDouble("OpposeStruggleFreeFromDragStaminaCost")))
			{
				continue;
			}

			dragger.Character.SpendStamina(
				Assailant.Gameworld.GetStaticDouble("OpposeStruggleFreeFromDragStaminaCost"));

			var dResult =
				opponentCheck.Check(dragger.Character, dragger.Aid == null ? Difficulty.Hard : Difficulty.Easy);
			if (dResult.Outcome.CheckDegrees() > opponentResult.CheckDegrees())
			{
				opponentResult = dResult.Outcome;
			}
		}

		var opposed = new OpposedOutcome(result.Outcome, opponentResult);
		if (opposed.Outcome == OpposedOutcomeDirection.Proponent)
		{
			Assailant.OutputHandler.Handle(new EmoteOutput(
				new Emote(
					$"@ struggle|struggles to break free from $1{(drag.Helpers.Any() ? " and &1's helpers" : "")}, and is successful!",
					Assailant, Assailant, drag.Owner), flags: OutputFlags.InnerWrap));
			drag.RemovalEffect();
			return new CombatMoveResult
			{
				AttackerOutcome = result.Outcome,
				DefenderOutcome = opponentResult,
				MoveWasSuccessful = true,
				RecoveryDifficulty = Difficulty.Normal
			};
		}

		Assailant.OutputHandler.Handle(new EmoteOutput(
			new Emote(
				$"@ struggle|struggles to break free from $1{(drag.Helpers.Any() ? " and &1's helpers" : "")}, but is not successful!",
				Assailant, Assailant, drag.Owner), flags: OutputFlags.InnerWrap));
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
			AttackerOutcome = result.Outcome,
			DefenderOutcome = opponentResult,
			MoveWasSuccessful = false,
			RecoveryDifficulty = Difficulty.Normal
		};
	}
}
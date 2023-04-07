using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class FleeMove : CombatMoveBase
{
	public override string Description => "Fleeing from combat";

	public override double BaseDelay => 1.0; // TODO - loaded from config

	private bool _calculatedStamina = false;
	private double _staminaCost = 0.0;

	public override double StaminaCost
	{
		get
		{
			if (!_calculatedStamina)
			{
				_staminaCost = MoveStaminaCost(Assailant);
				_calculatedStamina = true;
			}

			return _staminaCost;
		}
	}

	public static double BaseStaminaCost(IFuturemud gameworld)
	{
		return 0.0;
		//return gameworld.GetStaticDouble("FleeMoveStaminaCost");
	}

	public static double MoveStaminaCost(ICharacter assailant)
	{
		return BaseStaminaCost(assailant.Gameworld) * CombatBase.GraceMoveStaminaMultiplier(assailant);
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (Assailant.PositionState.MoveRestrictions == MovementAbility.Restricted)
		{
			Assailant.MovePosition(Assailant.PositionState.TransitionOnMovement, PositionModifier.None, null, null,
				null, true);
		}

		if (!Assailant.PositionState.Upright && Assailant.Body.CanStand(false))
		{
			Assailant.MovePosition(PositionStanding.Instance, PositionModifier.None, null, null, null);
		}

		var possiblePursuers =
			Assailant.Combat.Combatants.Where(
				         x => x.CombatStrategyMode != CombatStrategyMode.Flee && x.CombatTarget == Assailant)
			         .OfType<ICharacter>()
			         .Where(x => x.CombatSettings.PursuitMode != PursuitMode.NeverPursue)
			         .GroupBy(x => x.Party)
			         .ToList();

		if (!possiblePursuers.Any())
		{
			return ResolveFlee(Enumerable.Empty<ICharacter>());
		}

		var actualPursuers = new List<ICharacter>();
		foreach (var group in possiblePursuers)
		{
			if (group.Key == null)
			{
				foreach (var pursuer in group)
				{
					if (pursuer.CombatSettings.PursuitMode == PursuitMode.OnlyAttemptToStop &&
					    (Assailant.Location != pursuer.Location || !pursuer.MeleeRange || !Assailant.MeleeRange))
					{
						continue;
					}

					actualPursuers.Add(pursuer);
				}
			}
			else
			{
				var allPursuing =
					group.Key.CharacterMembers.All(
						x =>
							(x.Combat == Assailant.Combat &&
							 x.CombatSettings.PursuitMode == PursuitMode.AlwaysPursue) ||
							x.CombatSettings.PursuitMode == PursuitMode.OnlyPursueIfWholeGroupPursue ||
							(x.CombatSettings.PursuitMode == PursuitMode.OnlyAttemptToStop &&
							 x.Location == Assailant.Location && x.MeleeRange && Assailant.MeleeRange));
				foreach (var pursuer in group)
				{
					if (!allPursuing &&
					    pursuer.CombatSettings.PursuitMode == PursuitMode.OnlyPursueIfWholeGroupPursue)
					{
						continue;
					}

					if (pursuer.CombatSettings.PursuitMode == PursuitMode.OnlyAttemptToStop &&
					    (Assailant.Location != pursuer.Location || !pursuer.MeleeRange || !Assailant.MeleeRange))
					{
						continue;
					}

					actualPursuers.Add(pursuer);
				}
			}
		}

		return ResolveFlee(actualPursuers);
	}

	private CombatMoveResult ResolveFlee(IEnumerable<ICharacter> pursuers)
	{
		if (Assailant.Combat.CanFreelyLeaveCombat(Assailant))
		{
			Assailant.Combat.LeaveCombat(Assailant);
			return new CombatMoveResult
			{
				MoveWasSuccessful = true
			};
		}

		if (!pursuers.Any())
		{
			DoFlee(pursuers);
			return new CombatMoveResult
			{
				MoveWasSuccessful = true
			};
		}

		// TODO - effect to remember old speed and reset on leaving combat
		Assailant.CurrentSpeeds[Assailant.PositionState] =
			Assailant.Speeds.Where(x => x.Position == Assailant.PositionState).FirstMin(x => x.Multiplier);
		foreach (var pursuer in pursuers)
		{
			pursuer.CurrentSpeeds[pursuer.PositionState] =
				pursuer.Speeds.Where(x => x.Position == pursuer.PositionState).FirstMin(x => x.Multiplier);
		}

		var assailantSpeed = Assailant.MoveSpeed(null);
		var speeds = pursuers.Select(x => Tuple.Create(x, x.MoveSpeed(null))).ToList();

		var fleeCheck = Gameworld.GetCheck(CheckType.FleeMeleeCheck);
		var fleeDifficulty = Difficulty.Easy.StageUp(speeds.Count(x => x.Item2 <= assailantSpeed));
		var fleeResult = fleeCheck.Check(Assailant, fleeDifficulty);
		var opposedFleeCheck = Gameworld.GetCheck(CheckType.OpposeFleeMeleeCheck);

		if (Assailant.MeleeRange)
		{
			var successfulOpponents = new Dictionary<ICharacter, OpposedOutcomeDegree>();
			foreach (var pursuer in pursuers)
			{
				var outcome = new OpposedOutcome(fleeResult,
					opposedFleeCheck.Check(pursuer,
						speeds.First(x => x.Item1 == pursuer).Item2 >= assailantSpeed
							? Difficulty.Normal
							: Difficulty.VeryHard, Assailant));
				if (outcome.Outcome == OpposedOutcomeDirection.Opponent)
				{
					successfulOpponents[pursuer] = outcome.Degree;
				}
			}

			if (successfulOpponents.Any())
			{
				DoFailFlee(pursuers.ToList(), successfulOpponents.WhereMax(x => x.Value).GetRandomElement().Key);
				return new CombatMoveResult { RecoveryDifficulty = Difficulty.Normal };
			}
		}

		if (speeds.All(x => x.Item2 > assailantSpeed))
		{
			DoFlee(pursuers);
			return new CombatMoveResult { RecoveryDifficulty = Difficulty.Easy, MoveWasSuccessful = true };
		}

		DoFlee(pursuers);
		return new CombatMoveResult { RecoveryDifficulty = Difficulty.Normal, MoveWasSuccessful = true };
	}

	private void DoFlee(IEnumerable<ICharacter> pursuers)
	{
		var i = 1;
		var perceiverArgs = new List<ICharacter> { Assailant };
		perceiverArgs.AddRange(pursuers);

		if (Assailant.MeleeRange)
		{
			Assailant.OutputHandler.Handle(pursuers.Any()
				? new EmoteOutput(
					new Emote(
						$"@ flee|flees from melee combat, though opposed by {pursuers.Select(x => $"${i++}").ListToString()}.",
						Assailant, perceiverArgs.ToArray()), style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap)
				: new EmoteOutput(new Emote($"@ flee|flees from melee combat unoppposed.", Assailant),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));

			Assailant.MeleeRange = false;
			foreach (var opponent in Assailant.Combat.Combatants.Where(x => x.CombatTarget == Assailant && x.MeleeRange)
			                                  .ToList())
			{
				opponent.MeleeRange = false;
			}

			return;
		}

		var directions = Assailant.Location.ExitsFor(Assailant).Where(x => Assailant.CanCross(x).Success).ToList();
		// TODO - try to crash through doors if stuck
		if (!directions.Any())
		{
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote($"@ attempt|attempts to flee from combat, but have|has nowhere to go!", Assailant),
					style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			return;
		}

		var preferredDirections =
			directions.Where(x => x.Destination.Characters.All(y => y.Combat != Assailant.Combat)).ToList();
		var exit = preferredDirections.Any()
			? preferredDirections.GetRandomElement()
			: directions.GetRandomElement();
		Assailant.RemoveAllEffects(x => x.IsEffectType<ISneakEffect>());
		if (Assailant.Move(exit, new Emote($"fleeing from combat", Assailant)))
		{
			foreach (var pursuer in pursuers)
			{
				if (pursuer.CombatSettings.PursuitMode == PursuitMode.OnlyPursueIfWholeGroupPursue &&
				    pursuer.Party != null && pursuer.Party.Leader != pursuer &&
				    pursuers.All(x => pursuer.Party.Members.Contains(x)))
				{
					continue;
				}

				if (pursuer.Location != Assailant.Location)
				{
					continue;
				}

				pursuer.Move(exit, new Emote($"in pursuit of $0", pursuer, Assailant));
			}
		}
	}

	private void DoFailFlee(IEnumerable<ICharacter> pursuers, ICharacter successful)
	{
		var i = 2;
		var perceiverArgs = new List<ICharacter> { Assailant, successful };
		perceiverArgs.AddRange(pursuers);
		Assailant.OutputHandler.Handle(pursuers.Any()
			? new EmoteOutput(
				new Emote(
					$"@ attempt|attempts to flee from melee combat, opposed by {pursuers.Select(x => $"${i++}").ListToString()}, but $1 successfully $1|prevent|prevents &0 from doing so!",
					Assailant, perceiverArgs.ToArray()), style: OutputStyle.CombatMessage,
				flags: OutputFlags.InnerWrap)
			: new EmoteOutput(
				new Emote($"@ attempt|attempts to flee from melee combat, but somehow manage|manages to fail.",
					Assailant), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
	}
}
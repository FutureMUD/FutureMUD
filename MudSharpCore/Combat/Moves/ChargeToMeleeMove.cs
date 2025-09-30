using System;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class ChargeToMeleeMove : CombatMoveBase
{
	public override string Description => "Charging into melee combat";

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
		return gameworld.GetStaticDouble("ChargeToMeleeStaminaCost");
	}

	public static double MoveStaminaCost(ICharacter assailant)
	{
		return BaseStaminaCost(assailant.Gameworld) * CombatBase.GraceMoveStaminaMultiplier(assailant);
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (Assailant.CombatTarget is not ICharacter target)
		{
			_delay = 0;
			return CombatMoveResult.Irrelevant;
		}

		if (!Assailant.ColocatedWith(target) || Assailant.MeleeRange || target.Combat != Assailant.Combat)
		{
			_delay = 0;
			return CombatMoveResult.Irrelevant;
		}

		var response = defenderMove;

		if (response is SkirmishResponseMove skirmish)
		{
			return HandleSkirmish(target, skirmish);
		}

		if (response is SkirmishAndFire skirmishAndFire)
		{
			var outcome = HandleSkirmish(target, skirmishAndFire);
			skirmishAndFire.ResolveMove(new HelplessDefenseMove { Assailant = Assailant });
			return outcome;
		}

		Assailant.MeleeRange = true;
		if (target.CombatTarget == Assailant)
		{
			target.MeleeRange = true;
		}
		// TODO - bonus for charging into combat

		if (response is ReceiveChargeMove receiveCharge)
		{
			HandleReceiveCharge(receiveCharge, target);
			_delay = 0;
			return new CombatMoveResult { MoveWasSuccessful = true };
		}

		if (response is StandAndFireMove standAndFire)
		{
			var message = Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
				BuiltInCombatMoveType.ChargeToMelee, Outcome.NotTested, null);
			Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(message, Assailant, Assailant, target),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			standAndFire.ResolveMove(new HelplessDefenseMove { Assailant = Assailant });
			_delay = 0;
			return new CombatMoveResult { MoveWasSuccessful = true };
		}

		if (response == null || response is HelplessDefenseMove)
		{
			// Unopposed - they may already be engaged in melee or just be ambivalent
			var message = Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
				BuiltInCombatMoveType.ChargeToMelee, Outcome.NotTested, null);
			Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(message, Assailant, Assailant, target),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			_delay = 0;
			return new CombatMoveResult { MoveWasSuccessful = true };
		}

		throw new NotImplementedException();
	}

	private void HandleReceiveCharge(ReceiveChargeMove receiveCharge, ICharacter target)
	{
		Assailant.OutputHandler.Handle(
			new EmoteOutput(
				new Emote(
					Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
						BuiltInCombatMoveType.ChargeToMelee, Outcome.MinorPass, null), Assailant, Assailant, target),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		var receiveChargeDefense = Assailant.ResponseToMove(receiveCharge, target);
		receiveCharge.ResolveMove(receiveChargeDefense);
	}

	private CombatMoveResult HandleSkirmish(ICharacter target, ICombatMove response)
	{
		if (response == null || response is HelplessDefenseMove || target.CurrentSpeed == null)
		{
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
							BuiltInCombatMoveType.ChargeToMelee, Outcome.MajorPass, null), Assailant, Assailant,
						target), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			Assailant.MeleeRange = true;
			target.MeleeRange = true;
			Assailant.OffensiveAdvantage += Gameworld.GetStaticDouble("OffensiveAdvantageFromCharge");
			_delay = 0;
			return new CombatMoveResult
			{
				MoveWasSuccessful = true
			};
		}

	        var assailantMover = Assailant.GetCombatMover();
	        var assailantPosition = assailantMover.PositionState;
	        var assailantHadSpeed = assailantMover.CurrentSpeeds.TryGetValue(assailantPosition, out var oldAssailantSpeed);
	        var assailantTempSpeed =
	                assailantMover.Speeds.Where(x => x.Position == assailantPosition).FirstMin(x => x.Multiplier);
	        if (assailantTempSpeed != null)
	        {
	                assailantMover.CurrentSpeeds[assailantPosition] = assailantTempSpeed;
	        }

                var speed = Assailant.ApplyMovementSpeedCheck(assailantMover.MoveSpeed(null), false);

	        if (assailantTempSpeed != null)
	        {
	                if (assailantHadSpeed)
	                {
	                        assailantMover.CurrentSpeeds[assailantPosition] = oldAssailantSpeed;
	                }
	                else
	                {
	                        assailantMover.CurrentSpeeds.Remove(assailantPosition);
	                }
	        }

	        var targetMover = target.GetCombatMover();
	        var targetPosition = targetMover.PositionState;
	        var targetHadSpeed = targetMover.CurrentSpeeds.TryGetValue(targetPosition, out var oldTargetSpeed);
	        var targetTempSpeed =
	                targetMover.Speeds.Where(x => x.Position == targetPosition).FirstMin(x => x.Multiplier);
	        if (targetTempSpeed != null)
	        {
	                targetMover.CurrentSpeeds[targetPosition] = targetTempSpeed;
	        }
	        double moveTypeMultiplier;
	        var locationMultiplier = target.CombatSettings.SkirmishToOtherLocations && target.Movement == null &&
	                targetMover.Movement == null
	                ? 1.0
	                : 1.25;
	        switch (target.CombatSettings.PreferredMeleeMode)
	        {
			case CombatStrategyMode.Skirmish:
			case CombatStrategyMode.Swooper:
				moveTypeMultiplier = 1.3;
				break;
			case CombatStrategyMode.FullSkirmish:
				moveTypeMultiplier = 1.05;
				break;
			case CombatStrategyMode.Flee:
				moveTypeMultiplier = 1.05;
				break;
			default:
				moveTypeMultiplier = 1.5;
				break;
		}

                var targetBaseSpeed = target.ApplyMovementSpeedCheck(targetMover.MoveSpeed(null), true);
                var targetspeed = targetBaseSpeed * moveTypeMultiplier * locationMultiplier;

	        if (targetTempSpeed != null)
	        {
	                if (targetHadSpeed)
	                {
	                        targetMover.CurrentSpeeds[targetPosition] = oldTargetSpeed;
	                }
	                else
	                {
	                        targetMover.CurrentSpeeds.Remove(targetPosition);
	                }
	        }

		if (speed <= targetspeed)
		{
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
							BuiltInCombatMoveType.ChargeToMelee, Outcome.MajorPass, null), Assailant, Assailant,
						target), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			Assailant.MeleeRange = true;
			if (target.CombatTarget == Assailant || target.CombatTarget == null)
			{
				target.MeleeRange = true;
			}

			Assailant.OffensiveAdvantage += Gameworld.GetStaticDouble("OffensiveAdvantageFromCharge");
			_delay = 0;
			return new CombatMoveResult
			{
				MoveWasSuccessful = true
			};
		}

		Assailant.OutputHandler.Handle(
			new EmoteOutput(
				new Emote(
					Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
						BuiltInCombatMoveType.ChargeToMelee, Outcome.MajorFail, null), Assailant, Assailant, target),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
	        if (target.CombatSettings.SkirmishToOtherLocations && target.Movement == null && targetMover.Movement == null &&
	            speed <= targetspeed * 1.25)
		{
			var exit = target.Location.ExitsFor(target).Where(x => target.CanCross(x).Success).GetRandomElement();
			if (exit != null)
			{
				target.Move(exit, new Emote("fleeing from $0", target, Assailant));
				foreach (
					var other in
					target.Combat.Combatants.OfType<ICharacter>()
					      .Where(
						      x =>
							      x.ColocatedWith(target) && x.CombatTarget == target &&
							      x.CombatSettings.PursuitMode == PursuitMode.AlwaysPursue && x.Movement == null)
					      .ToList())
				{
					other.Move(exit, new Emote("pursuing $0", other, target));
				}
			}
		}

		_delay = 1.0;
		return new CombatMoveResult
		{
			MoveWasSuccessful = false,
			RecoveryDifficulty = Difficulty.Normal
		};
	}

	#region Overrides of CombatMoveBase

	private double _delay;
	public override double BaseDelay => _delay;

	#endregion
}
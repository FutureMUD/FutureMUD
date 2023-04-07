using System;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class MoveToMeleeMove : CombatMoveBase
{
	public override string Description => "Moving into melee range";

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
		return gameworld.GetStaticDouble("MoveToMeleeStaminaCost");
	}

	public static double MoveStaminaCost(ICharacter assailant)
	{
		return BaseStaminaCost(assailant.Gameworld) * CombatBase.GraceMoveStaminaMultiplier(assailant);
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (Assailant.CombatTarget is not ICharacter target)
		{
			return CombatMoveResult.Irrelevant;
		}

		if (!Assailant.ColocatedWith(target) || Assailant.MeleeRange || target.Combat != Assailant.Combat)
		{
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
			skirmishAndFire.ResolveMove(Assailant.ResponseToMove(skirmishAndFire, skirmishAndFire.Assailant));
			return outcome;
		}

		Assailant.MeleeRange = true;
		target.MeleeRange = true;

		if (response is StandAndFireMove standAndFire)
		{
			var message = Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
				BuiltInCombatMoveType.MoveToMelee, Outcome.NotTested, null);
			Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(message, Assailant, Assailant, target),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			return standAndFire.ResolveMove(Assailant.ResponseToMove(standAndFire, standAndFire.Assailant));
		}

		if (response == null || response is HelplessDefenseMove)
		{
			// Unopposed - they may already be engaged in melee or just be ambivalent
			var message = Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
				BuiltInCombatMoveType.MoveToMelee, Outcome.NotTested, null);
			Assailant.OutputHandler.Handle(new EmoteOutput(new Emote(message, Assailant, Assailant, target),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			return new CombatMoveResult
			{
				MoveWasSuccessful = true
			};
		}

		throw new NotImplementedException($"Unknown response in MoveToMeleeMove: {response.Description}");
	}

	private CombatMoveResult HandleSkirmish(ICharacter target, ICombatMove response)
	{
		if (response == null || response is HelplessDefenseMove || target.CurrentSpeed == null)
		{
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
							BuiltInCombatMoveType.MoveToMelee, Outcome.MajorPass, null), Assailant, Assailant,
						target), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			Assailant.MeleeRange = true;
			target.MeleeRange = true;
			return new CombatMoveResult
			{
				MoveWasSuccessful = true
			};
		}

		var oldSpeed = Assailant.CurrentSpeeds[Assailant.PositionState];
		Assailant.CurrentSpeeds[Assailant.PositionState] =
			Assailant.Speeds.Where(x => x.Position == Assailant.PositionState).FirstMin(x => x.Multiplier);
		var speed = Assailant.MoveSpeed(null);
		Assailant.CurrentSpeeds[Assailant.PositionState] = oldSpeed;

		oldSpeed = target.CurrentSpeeds[target.PositionState];
		target.CurrentSpeeds[target.PositionState] =
			target.Speeds.Where(x => x.Position == target.PositionState).FirstMin(x => x.Multiplier);
		double moveTypeMultiplier;
		var locationMultiplier = target.CombatSettings.SkirmishToOtherLocations && target.Movement == null
			? 1.0
			: 1.25;
		switch (target.CombatSettings.PreferredMeleeMode)
		{
			case CombatStrategyMode.Skirmish:
				moveTypeMultiplier = 1.25;
				break;
			case CombatStrategyMode.FullSkirmish:
				moveTypeMultiplier = 1.0;
				break;
			case CombatStrategyMode.Flee:
				moveTypeMultiplier = 1.0;
				break;
			default:
				moveTypeMultiplier = 1.5;
				break;
		}

		var targetspeed = target.MoveSpeed(null) * moveTypeMultiplier * locationMultiplier;
		target.CurrentSpeeds[target.PositionState] = oldSpeed;

		if (speed <= targetspeed)
		{
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
							BuiltInCombatMoveType.MoveToMelee, Outcome.MajorPass, null), Assailant, Assailant,
						target), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			Assailant.MeleeRange = true;
			target.MeleeRange = true;
			return new CombatMoveResult
			{
				MoveWasSuccessful = true
			};
		}

		Assailant.OutputHandler.Handle(
			new EmoteOutput(
				new Emote(
					Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
						BuiltInCombatMoveType.MoveToMelee, Outcome.MajorFail, null), Assailant, Assailant, target),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
		if (target.CombatSettings.SkirmishToOtherLocations && target.Movement == null &&
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

		return new CombatMoveResult
		{
			MoveWasSuccessful = false,
			RecoveryDifficulty = Difficulty.Normal
		};
	}
}
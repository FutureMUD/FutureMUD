using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class FireAndAdvanceToMeleeMove : CombatMoveBase
{
	public FireAndAdvanceToMeleeMove(ICharacter assailant, ICharacter target, IRangedWeapon weapon)
	{
		Assailant = assailant;
		_characterTargets.Add(target);
		Weapon = weapon;
	}

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
		return gameworld.GetStaticDouble("FireAndAdvanceToMeleeStaminaCost");
	}

	public static double MoveStaminaCost(ICharacter assailant)
	{
		return BaseStaminaCost(assailant.Gameworld) * CombatBase.GraceMoveStaminaMultiplier(assailant);
	}

	public IRangedWeapon Weapon { get; set; }

	#region Overrides of CombatMoveBase

	public override double BaseDelay => 0.5;

	#endregion

	public override string Description
		=>
			$"Advancing to Melee and firing {Weapon.Parent.HowSeen(Assailant)} at {CharacterTargets.First().HowSeen(Assailant)}.";

	#region Overrides of RangedWeaponAttackBase

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var target = defenderMove?.Assailant ?? CharacterTargets.First();
		HandleSkirmish(target, defenderMove);

		var dummyMove = new RangedWeaponAttackMove(Assailant, target, Weapon) { SuppressAttackMessage = true };
		var shotResponse = target.ResponseToMove(dummyMove, Assailant);
		return dummyMove.ResolveMove(shotResponse);
	}

	#endregion

	private void HandleSkirmish(ICharacter target, ICombatMove response)
	{
		var fireAction = "fire|fires";
		switch (Weapon.WeaponType.RangedWeaponType)
		{
			case RangedWeaponType.Sling:
			case RangedWeaponType.Thrown:
				fireAction = "throw|throws";
				break;
		}

		if (response == null || response is HelplessDefenseMove || target.CurrentSpeed == null)
		{
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						string.Format(
							Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
								BuiltInCombatMoveType.AdvanceAndFire, Outcome.MajorPass, null), fireAction),
						Assailant, Assailant, target, Weapon.Parent), style: OutputStyle.CombatMessage,
					flags: OutputFlags.InnerWrap));
			Assailant.MeleeRange = true;
			target.MeleeRange = true;
			return;
		}

		var oldSpeed = Assailant.CurrentSpeed;
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
			case CombatStrategyMode.Swooper:
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
							BuiltInCombatMoveType.AdvanceAndFire, Outcome.MajorPass, null), Assailant, Assailant,
						target), style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			Assailant.MeleeRange = true;
			target.MeleeRange = true;
			return;
		}

		Assailant.OutputHandler.Handle(
			new EmoteOutput(
				new Emote(
					Gameworld.CombatMessageManager.GetMessageFor(Assailant, target, null, null,
						BuiltInCombatMoveType.AdvanceAndFire, Outcome.MajorFail, null), Assailant, Assailant, target),
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
	}
}
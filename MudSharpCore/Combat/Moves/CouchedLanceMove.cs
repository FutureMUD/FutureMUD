#nullable enable

using MudSharp.Character;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

/// <summary>
/// A lance thrust delivered while a rider closes on an opponent. The move is
/// only constructed by the charge path, which prevents it becoming a free
/// standing melee attack after the rider has stopped.
/// </summary>
public class CouchedLanceMove : MeleeWeaponAttack
{
	private const double MaximumChargeSpeed = 10.0;
	private const int MaximumChargeReach = 5;

	public CouchedLanceMove(ICharacter owner, IMeleeWeapon weapon, IWeaponAttack attack, ICharacter target)
		: base(owner, weapon, attack, target)
	{
	}

	// Couched attacks use the ordinary weapon-attack message family. Existing
	// worlds therefore need no new combat-message rows to remain playable.
	public override BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.UseWeaponAttack;
	public override string Description => "Couching a lance in a mounted charge";

	public static bool CanCouch(ICharacter assailant, IMeleeWeapon weapon, IWeaponAttack attack,
		ICharacter target)
	{
		return assailant.RidingMount is not null &&
		       attack.MoveType == BuiltInCombatMoveType.CouchedLanceAttack &&
		       attack.UsableAttack(assailant, weapon.Parent, target, weapon.HandednessForWeapon(assailant), false,
			       BuiltInCombatMoveType.CouchedLanceAttack);
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (Assailant.RidingMount is null)
		{
			Assailant.OutputHandler.Send("You can only couch a lance while mounted and charging.");
			return CombatMoveResult.Irrelevant;
		}

		var mountSpeed = Math.Clamp(Assailant.GetCombatMover().MoveSpeed(null!), 0.0, MaximumChargeSpeed);
		var reach = Math.Min(Weapon.WeaponType.Reach, MaximumChargeReach);
		Assailant.OffensiveAdvantage += mountSpeed * Math.Max(1, reach) / MaximumChargeReach;

		var result = base.ResolveMove(defenderMove);
		var target = CharacterTargets.FirstOrDefault();
		if (target?.RidingMount is not null && result.MoveWasSuccessful && result.AttackerOutcome.IsPass())
		{
			target.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ are|is knocked from $1 by the impact of the couched lance!", target, target, target.RidingMount),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			target.DoCombatKnockdown(Math.Max(1, result.AttackerOutcome.SuccessDegrees()));
		}

		return result;
	}
}

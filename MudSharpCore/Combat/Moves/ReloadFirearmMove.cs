using MudSharp.Effects.Concrete;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class ReloadFirearmMove : CombatMoveBase
{
	public override string Description => $"Reloading {Weapon.Parent.HowSeen(Assailant)}";

	public override double StaminaCost => Weapon.WeaponType.StaminaPerLoadStage;

	#region Overrides of CombatMoveBase

	public override double BaseDelay
		=> Weapon.WeaponType.LoadCombatDelay;

	#endregion

	public IRangedWeapon Weapon { get; init; }

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		Assailant.RemoveAllEffects(x => x.GetSubtype<FirearmNeedsReloading>()?.Firearm == Weapon);
		if (!Weapon.CanUnload(Assailant))
		{
			if (Assailant.Body.CanSheathe(Weapon.Parent, null))
			{
				Assailant.Body.Sheathe(Weapon.Parent, null);
			}
			else
			{
				Assailant.Body.Drop(Weapon.Parent);
			}

			return CombatMoveResult.Irrelevant;
		}

		foreach (var item in Weapon.Unload(Assailant))
		{
			Assailant.Body.Drop(item);
		}

		if (!Weapon.CanLoad(Assailant, true))
		{
			if (Assailant.Body.CanSheathe(Weapon.Parent, null))
			{
				Assailant.Body.Sheathe(Weapon.Parent, null);
			}
			else
			{
				Assailant.Body.Drop(Weapon.Parent);
			}

			return CombatMoveResult.Irrelevant;
		}

		Weapon.Load(Assailant, true);
		return new CombatMoveResult
		{
			RecoveryDifficulty = Difficulty.Easy,
			MoveWasSuccessful = true
		};
	}
}
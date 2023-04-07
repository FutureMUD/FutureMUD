using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;
using System;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Construction.Boundary;

namespace MudSharp.Combat.Moves;

public class AimRangedWeaponMove : CombatMoveBase
{
	private static double? _combatAimMajorSuccessPercentage;

	public static double CombatAimMajorSuccessPercentage
	{
		get
		{
			if (_combatAimMajorSuccessPercentage == null)
			{
				_combatAimMajorSuccessPercentage =
					Futuremud.Games.First().GetStaticDouble("CombatAimMajorSuccessPercentage");
			}

			return _combatAimMajorSuccessPercentage.Value;
		}
	}

	public AimRangedWeaponMove(ICharacter assailant, IPerceiver target, IRangedWeapon weapon)
	{
		Assailant = assailant;
		if (target is ICharacter tch)
		{
			_characterTargets.Add(tch);
		}

		if (target != null)
		{
			_targets.Add(target);
		}

		Weapon = weapon;
	}

	public override string Description => $"Aiming {Weapon.Parent.HowSeen(Assailant)}";

	public override double StaminaCost => Weapon.WeaponType.StaminaPerLoadStage;

	#region Overrides of CombatMoveBase

	public override double BaseDelay => Gameworld.GetStaticDouble("AimRangedWeaponMoveDelay");

	#endregion

	public IRangedWeapon Weapon { get; set; }

	//Aim improves faster when kneeling or prone
	protected static double GetShooterPositionBonus(IPositionState position)
	{
		if (position.CompareTo(PositionKneeling.Instance) == PositionHeightComparison.Equivalent)
		{
			return 1.10;
		}

		if (position.CompareTo(PositionKneeling.Instance) == PositionHeightComparison.Lower)
		{
			return 1.25;
		}

		return 1.0;
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		var check = Gameworld.GetCheck(CheckType.AimRangedWeapon);
		var difficulty = Assailant.GetDifficultyForTool(Weapon.Parent, Weapon.AimDifficulty);
		var target = CharacterTargets.FirstOrDefault();
		var postureBonus = 1.0;
		if (Assailant != null && target != null && target.Location != Assailant.Location)
		{
			var position = Assailant.Body.PositionState;
			postureBonus = GetShooterPositionBonus(position);

			//TODO - Bows shouldn't be usable from Prone, going prone with a bow should prevent/abort aiming
		}

		var firstTimeAim = false;
		if (Assailant.Aim == null)
		{
			firstTimeAim = true;
			Assailant.Aim = new AimInformation(target, Assailant,
				target == null
					? Enumerable.Empty<ICellExit>()
					: Assailant.PathBetween(target, Weapon.WeaponType.DefaultRangeInRooms, false, false, true), Weapon);
		}

		CheckOutcome result = null;
		if (Assailant.Aim.AimPercentage < 1.0)
		{
			result = check.Check(Assailant, difficulty, Weapon.WeaponType.FireTrait, target);
			var aimAmount = 0.0;
			switch (result.Outcome)
			{
				case Outcome.Fail:
					aimAmount = 0.03 * CombatAimMajorSuccessPercentage;
					break;
				case Outcome.MinorFail:
					aimAmount = 0.1 * CombatAimMajorSuccessPercentage;
					break;
				case Outcome.MinorPass:
					aimAmount = 0.2 * CombatAimMajorSuccessPercentage;
					break;
				case Outcome.Pass:
					aimAmount = 0.3 * CombatAimMajorSuccessPercentage;
					break;
				case Outcome.MajorPass:
					aimAmount = 0.5 * CombatAimMajorSuccessPercentage;
					break;
			}

			aimAmount = aimAmount * postureBonus;

			Assailant.Aim.AimPercentage = Assailant.Aim.AimPercentage + aimAmount;
		}
		else
		{
			result = new CheckOutcome() { Outcome = Outcome.MajorPass };
		}

		if (firstTimeAim)
		{
			Assailant.OutputHandler.Handle(
				new EmoteOutput(
					new Emote(
						Gameworld.CombatMessageManager.GetMessageFor(
							Assailant, CharacterTargets.First(), Weapon.Parent, null,
							BuiltInCombatMoveType.AimRangedWeapon, result.Outcome, null)
						, Assailant, Assailant,
						target ?? (IPerceivable)new DummyPerceivable("the air"), Weapon.Parent),
					style: OutputStyle.CombatMessage,
					flags: OutputFlags.SuppressObscured | OutputFlags.InnerWrap));
		}

		return new CombatMoveResult
			{ RecoveryDifficulty = Difficulty.Easy, MoveWasSuccessful = result.Outcome >= Outcome.MinorFail };
	}
}
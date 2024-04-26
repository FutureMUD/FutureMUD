using System;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp.Combat.Moves;

public class BlockMove : CombatMoveBase, IDefenseMove
{
	public IShield Shield { get; init; }

	public override string Description
		=>
			$"Blocking {CharacterTargets.First().HowSeen(Assailant, type: DescriptionType.Possessive, colour: false, flags: PerceiveIgnoreFlags.IgnoreSelf)} attack with {Shield.Parent.HowSeen(Assailant, colour: false)}.";

	public override Difficulty RecoveryDifficultySuccess => Difficulty.VeryEasy;

	public override Difficulty RecoveryDifficultyFailure => Difficulty.Easy;

	public override ExertionLevel AssociatedExertion => ExertionLevel.Heavy;

	private bool _calculatedStamina = false;
	private double _staminaCost = 0.0;

	public override double StaminaCost
	{
		get
		{
			if (!_calculatedStamina)
			{
				_staminaCost = MoveStaminaCost(CharacterTargets.First(), Assailant, Shield);
				_calculatedStamina = true;
			}

			return _staminaCost;
		}
	}

	public static double MoveStaminaCost(ICharacter attacker, ICharacter defender, IShield shield)
	{
		return shield.ShieldType.StaminaPerBlock * CombatBase.PowerMoveStaminaMultiplier(defender) *
		       CombatBase.RelativeStrengthDefenseStaminaMultiplier(attacker, defender);
	}

	public Alignment Alignment => Assailant.Body.WieldedHand(Shield.Parent).Alignment.LeftRightOnly();

	public override CheckType Check => CheckType.BlockCheck;


	protected static double GetBlockBonus(IBodypart part, Alignment shieldAlignment)
	{
		var blockBonus = 0.0;
		// blocking is much easier against attacks on the side it is wielded on, as well as attacks on the arms or centre
		if (part?.Alignment == shieldAlignment)
		{
			blockBonus += 4.0;
		}
		else
		{
			blockBonus -= 4.0;
		}

		switch (part?.Orientation)
		{
			case Orientation.Appendage:
			case Orientation.Centre:
				blockBonus += 4.0;
				break;
			case Orientation.High:
				blockBonus += 2.0;
				break;
			case Orientation.Highest:
				blockBonus += 1.0;
				break;
			case Orientation.Low:
				blockBonus -= 2.0;
				break;
			case Orientation.Lowest:
				blockBonus -= 4.0;
				break;
		}

		return blockBonus;
	}

	public static double GetBlockBonus(MagicPowerAttackMove move, Alignment shieldAlignment)
	{
		return GetBlockBonus(move.TargetBodypart, shieldAlignment);
	}

	public static double GetBlockBonus(IWeaponAttackMove move, Alignment shieldAlignment)
	{
		return GetBlockBonus(move?.TargetBodypart, shieldAlignment);
	}

	public static double GetBlockBonus(IRangedWeaponAttackMove move, Alignment shieldAlignment)
	{
		return GetBlockBonus(move?.TargetBodypart, shieldAlignment);
	}

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		// Defensive moves are only obliged to provide a difficulty for recovery, not undertake any other action
		throw new NotSupportedException("Defense Moves should not call ResolveMove.");
	}

	public virtual int DifficultStageUps => 0;

	public void ResolveDefenseUsed(OpposedOutcome outcome)
	{
		// Do nothing
	}

	public virtual BuiltInCombatMoveType MoveType => BuiltInCombatMoveType.Block;
}
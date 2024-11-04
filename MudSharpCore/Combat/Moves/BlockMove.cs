using System;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
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

	public static double MoveStaminaCost(IPerceiver attacker, ICharacter defender, IShield shield)
	{
		return
			shield.ShieldType.StaminaPerBlock *
			CombatBase.PowerMoveStaminaMultiplier(defender) *
			(attacker is ICharacter ach ? CombatBase.RelativeStrengthDefenseStaminaMultiplier(ach, defender) : 1.0)
			;
	}

	public Alignment Alignment => Assailant.Body.WieldedHand(Shield.Parent).Alignment.LeftRightOnly();

	public override CheckType Check => CheckType.BlockCheck;


	protected static double GetBlockBonus(IBodypart part, Alignment shieldAlignment, IShield shield)
	{
		var blockBonus = 0.0;
		// blocking is much easier against attacks on the side it is wielded on, as well as attacks on the arms or centre
		if (part?.Alignment == shieldAlignment)
		{
			blockBonus += shield.Gameworld.GetStaticDouble("ShieldSameSideBlockBonus");
		}
		else
		{
			blockBonus += shield.Gameworld.GetStaticDouble("ShieldOppositeSideBlockBonus");
		}

		switch (part?.Orientation)
		{
			case Orientation.Appendage:
				blockBonus += shield.Gameworld.GetStaticDouble("ShieldAppendageTargetedBlockBonus");
				break;
		}

		blockBonus += shield.Parent.Wounds.SelectNotNull(x => x.Lodged)
							 .Where(x => x.Size >= shield.Parent.Size)
							 .Sum(x => Math.Max(0, ((int)x.Size - (int)shield.Parent.Size))) *
							 shield.Parent.Gameworld.GetStaticDouble("ShieldPenaltyPerLodgedItemSizeDifference");

		return blockBonus;
	}

	public static double GetBlockBonus(MagicPowerAttackMove move, Alignment shieldAlignment, IShield shield)
	{
		return GetBlockBonus(move.TargetBodypart, shieldAlignment, shield);
	}

	public static double GetBlockBonus(IWeaponAttackMove move, Alignment shieldAlignment, IShield shield)
	{
		return GetBlockBonus(move?.TargetBodypart, shieldAlignment, shield);
	}

	public static double GetBlockBonus(IRangedWeaponAttackMove move, Alignment shieldAlignment, IShield shield)
	{
		return GetBlockBonus(move?.TargetBodypart, shieldAlignment, shield);
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
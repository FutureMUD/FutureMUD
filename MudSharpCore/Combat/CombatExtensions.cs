using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Framework;

namespace MudSharp.Combat;

public static class CombatExtensions
{
	/// <summary>
	/// An array of all the "standard" attacks to be included when looking at whether a weapon has any regular attacks
	/// </summary>
	public static BuiltInCombatMoveType[] StandardMeleeWeaponAttacks =
	{
		BuiltInCombatMoveType.UseWeaponAttack,
		BuiltInCombatMoveType.StaggeringBlow,
		BuiltInCombatMoveType.UnbalancingBlow
	};

	public static string Describe(this AttackHandednessOptions option)
	{
		switch (option)
		{
			case AttackHandednessOptions.Any:
				return "No Preference";
			case AttackHandednessOptions.OneHandedOnly:
				return "One Handed";
			case AttackHandednessOptions.TwoHandedOnly:
				return "Two Handed";
			case AttackHandednessOptions.DualWieldOnly:
				return "Dual Wield";
			case AttackHandednessOptions.SwordAndBoardOnly:
				return "Sword & Board";
		}

		return "Unknown";
	}

	public static string Describe(this AutomaticInventorySettings setting)
	{
		switch (setting)
		{
			case AutomaticInventorySettings.FullyAutomatic:
				return "Fully Automatic";
			case AutomaticInventorySettings.FullyManual:
				return "Fully Manual";
			case AutomaticInventorySettings.RetrieveOnly:
				return "Only Retrieve Lost Items";
			case AutomaticInventorySettings.AutomaticButDontDiscard:
				return "Automatic But No Discarding";
			default:
				throw new ArgumentOutOfRangeException(nameof(setting), setting, null);
		}
	}

	public static string Describe(this AutomaticRangedSettings setting)
	{
		switch (setting)
		{
			case AutomaticRangedSettings.FullyAutomatic:
				return "Fully Automatic";
			case AutomaticRangedSettings.FullyManual:
				return "Fully Manual";
			case AutomaticRangedSettings.ContinueFiringOnly:
				return "Continue Firing Only After Manual Initiation";
			default:
				throw new ArgumentOutOfRangeException(nameof(setting), setting, null);
		}
	}

	public static string Describe(this AutomaticMovementSettings setting)
	{
		switch (setting)
		{
			case AutomaticMovementSettings.FullyAutomatic:
				return "Fully Automatic";
			case AutomaticMovementSettings.FullyManual:
				return "Fully Manual";
			case AutomaticMovementSettings.SeekCoverOnly:
				return "Seek Cover Only";
			case AutomaticMovementSettings.KeepRange:
				return "Keep At Range Only";
			default:
				throw new ArgumentOutOfRangeException(nameof(setting), setting, null);
		}
	}

	public static bool IsFirearm(this RangedWeaponType type)
	{
		switch (type)
		{
			case RangedWeaponType.Thrown:
				return false;
			case RangedWeaponType.Firearm:
				return true;
			case RangedWeaponType.ModernFirearm:
				return true;
			case RangedWeaponType.Laser:
				return true;
			case RangedWeaponType.Bow:
				return false;
			case RangedWeaponType.Crossbow:
				return false;
			case RangedWeaponType.Sling:
				return false;
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
	}

	public static PositionState MinimumFiringPosition(this RangedWeaponType type)
	{
		switch (type)
		{
			case RangedWeaponType.Thrown:
				return PositionKneeling.Instance;
			case RangedWeaponType.Firearm:
				return PositionProne.Instance;
			case RangedWeaponType.ModernFirearm:
				return PositionProne.Instance;
			case RangedWeaponType.Laser:
				return PositionProne.Instance;
			case RangedWeaponType.Bow:
				return PositionStanding.Instance;
			case RangedWeaponType.Crossbow:
				return PositionProne.Instance;
			case RangedWeaponType.Sling:
				return PositionKneeling.Instance;
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
	}

	public static string Describe(this CoverExtent extent)
	{
		switch (extent)
		{
			case CoverExtent.Marginal:
				return "Marginal";
			case CoverExtent.Partial:
				return "Partial";
			case CoverExtent.NearTotal:
				return "Near Total";
			case CoverExtent.Total:
				return "Total";
			default:
				throw new ArgumentOutOfRangeException(nameof(extent), extent, null);
		}
	}

	public static string Describe(this CoverType type)
	{
		switch (type)
		{
			case CoverType.Soft:
				return "Soft";
			case CoverType.Hard:
				return "Hard";
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}
	}

	private static string InternalDescribe(DefenseType type, ref List<string> descriptions)
	{
		while (true)
		{
			if (type.HasFlag(DefenseType.Block))
			{
				descriptions.Add("Block");
				type = type ^ DefenseType.Block;
				continue;
			}

			if (type.HasFlag(DefenseType.Dodge))
			{
				descriptions.Add("Dodge");
				type = type ^ DefenseType.Dodge;
				continue;
			}

			if (type.HasFlag(DefenseType.Parry))
			{
				descriptions.Add("Parry");
				type = type ^ DefenseType.Parry;
				continue;
			}

			return descriptions.Any() ? descriptions.ListToString() : "None";
		}
	}

	public static string Describe(this DefenseType type)
	{
		var types = new List<string>();
		return InternalDescribe(type, ref types);
	}

	public static bool TryParseDefenseType(string text, out DefenseType type)
	{
		if (Enum.TryParse(text, true, out type))
		{
			return true;
		}

		var values = Enum.GetValues(typeof(DefenseType)).OfType<DefenseType>().ToList();
		if (values.Any(x => x.Describe().StartsWith(text, StringComparison.InvariantCultureIgnoreCase)))
		{
			type =
				values.FirstOrDefault(
					x => x.Describe().StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
			return true;
		}

		return false;
	}

	public static string Describe(this RangedWeaponType type)
	{
		switch (type)
		{
			case RangedWeaponType.Thrown:
				return "Thrown";
			case RangedWeaponType.Firearm:
				return "Firearm";
			case RangedWeaponType.ModernFirearm:
				return "ModernFirearm";
			case RangedWeaponType.Bow:
				return "Bow";
			case RangedWeaponType.Crossbow:
				return "Crossbow";
			case RangedWeaponType.Sling:
				return "Sling";
			case RangedWeaponType.Laser:
				return "Laser";
			default:
				throw new NotImplementedException("No RangedWeaponType description in Describe.");
		}
	}

	public static string Describe(this AmmunitionLoadType type)
	{
		switch (type)
		{
			case AmmunitionLoadType.Clip:
				return "Clip";
			case AmmunitionLoadType.Direct:
				return "Direct";
			case AmmunitionLoadType.Magazine:
				return "Magazine";
		}

		throw new NotImplementedException("No AmmunitionLoadType description in Describe.");
	}

	public static string Describe(this PursuitMode mode)
	{
		switch (mode)
		{
			case PursuitMode.NeverPursue:
				return "Never Pursue";
			case PursuitMode.OnlyAttemptToStop:
				return "Only Stop Leaving Melee";
			case PursuitMode.OnlyPursueIfWholeGroupPursue:
				return "Only Pursue With Group";
			case PursuitMode.AlwaysPursue:
				return "Always Pursue";
			default:
				throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
		}
	}

	public static bool IsGrappleMode(this CombatStrategyMode mode)
	{
		switch (mode)
		{
			case CombatStrategyMode.GrappleForControl:
			case CombatStrategyMode.GrappleForIncapacitation:
			case CombatStrategyMode.GrappleForKill:
				return true;
		}

		return false;
	}

	public static string Describe(this CombatStrategyMode mode)
	{
		switch (mode)
		{
			case CombatStrategyMode.StandardMelee:
				return "Standard Melee";
			case CombatStrategyMode.StandardRange:
				return "Standard Ranged";
			case CombatStrategyMode.FullCover:
				return "Full Cover";
			case CombatStrategyMode.FullDefense:
				return "Full Defense";
			case CombatStrategyMode.FullSkirmish:
				return "Full Skirmish";
			case CombatStrategyMode.Skirmish:
				return "Skirmish";
			case CombatStrategyMode.FullAdvance:
				return "Full Advance";
			case CombatStrategyMode.FireAndAdvance:
				return "Fire and Advance";
			case CombatStrategyMode.CoverAndAdvance:
				return "Cover and Advance";
			case CombatStrategyMode.CoveringFire:
				return "Covering Fire";
			case CombatStrategyMode.FireNoCover:
				return "Fire (No Cover)";
			case CombatStrategyMode.Ward:
				return "Ward";
			case CombatStrategyMode.Clinch:
				return "Clinch";
			case CombatStrategyMode.Flee:
				return "Flee";
			case CombatStrategyMode.GrappleForControl:
				return "Grapple for Control";
			case CombatStrategyMode.GrappleForIncapacitation:
				return "Grapple for Incapacitation";
			case CombatStrategyMode.GrappleForKill:
				return "Grapple for Kill";
			case CombatStrategyMode.MeleeShooter:
				return "Melee Shooter";
			case CombatStrategyMode.MeleeMagic:
				return "Melee Magic";
			default:
				throw new ApplicationException("Unknown CombatStrategyMode in Describe.");
		}
	}

	public static string DescribeWordy(this CombatStrategyMode mode)
	{
		switch (mode)
		{
			case CombatStrategyMode.StandardMelee:
				return "close with the opponent (if necessary) and engage them in ordinary melee combat";
			case CombatStrategyMode.StandardRange:
				return "seek light cover (if possible) and fire at the opponent at range";
			case CombatStrategyMode.FullCover:
				return "seek full cover and remain";
			case CombatStrategyMode.FullDefense:
				return "engage in full defense, making no attempt to strike back";
			case CombatStrategyMode.FullSkirmish:
				return "engage in cautious skirmishing ranged combat, firing only when sufficiently ranged";
			case CombatStrategyMode.Skirmish:
				return "engage in skirmishing combat, falling back when engaged and seeking very light cover";
			case CombatStrategyMode.FireAndAdvance:
				return "firing upon the opponent while advancing to melee combat";
			case CombatStrategyMode.FullAdvance:
				return "moving as swiftly as possible to engage the opponent in melee combat";
			case CombatStrategyMode.CoverAndAdvance:
				return "moving from cover to cover while advancing to melee combat";
			case CombatStrategyMode.CoveringFire:
				return "seek light cover (if possible) and provide covering fire to suppress opponents";
			case CombatStrategyMode.FireNoCover:
				return
					"stand out in the open and fire at the opponent at range, with no concern for personal safety";
			case CombatStrategyMode.Ward:
				return "assume a defensive posture and ward against attacks, using superior reach to control combat";
			case CombatStrategyMode.Clinch:
				return "close into extremely short melee range and unleash rapid, devastating attacks";
			case CombatStrategyMode.Flee:
				return "flee combat by any means necessary";
			case CombatStrategyMode.GrappleForControl:
				return
					"grapple with an opponent and try to get in control of all their limbs, to restrain and hold them";
			case CombatStrategyMode.GrappleForIncapacitation:
				return
					"grapple with an opponent and use the opportunity to incapacitate them by wrenching joints and throwing them down";
			case CombatStrategyMode.GrappleForKill:
				return "grapple with an opponent and attempt to deliver potentially lethal blows";
			case CombatStrategyMode.MeleeShooter:
				return "shoot people in melee with a pistol";
			case CombatStrategyMode.MeleeMagic:
				return "get into melee and use weapons or magic against the enemy";
			default:
				throw new ApplicationException("Unknown CombatStrategyMode in Describe.");
		}
	}

	/// <summary>
	///     Determines whether or not a CombatStrategyMode is a valid choice to be employed in melee combat
	/// </summary>
	/// <param name="mode">The mode in question</param>
	/// <returns>True if this CombatStrategyMode can be used in melee combat</returns>
	public static bool IsMeleeStrategy(this CombatStrategyMode mode)
	{
		switch (mode)
		{
			case CombatStrategyMode.StandardMelee:
			case CombatStrategyMode.FullDefense:
			case CombatStrategyMode.Ward:
			case CombatStrategyMode.Clinch:
			case CombatStrategyMode.FullSkirmish:
			case CombatStrategyMode.Skirmish:
			case CombatStrategyMode.Flee:
			case CombatStrategyMode.GrappleForKill:
			case CombatStrategyMode.GrappleForIncapacitation:
			case CombatStrategyMode.GrappleForControl:
			case CombatStrategyMode.MeleeShooter:
			case CombatStrategyMode.MeleeMagic:
				return true;
			default:
				return false;
		}
	}

	/// <summary>
	///     Determines whether or not a CombatStrategyMode is a valid choice to be employed in ranged combat
	/// </summary>
	/// <param name="mode">The mode in question</param>
	/// <returns>True if this CombatStrategyMode can be used in ranged combat</returns>
	public static bool IsRangedStrategy(this CombatStrategyMode mode)
	{
		switch (mode)
		{
			case CombatStrategyMode.CoveringFire:
			case CombatStrategyMode.FireAndAdvance:
			case CombatStrategyMode.FireNoCover:
			case CombatStrategyMode.Flee:
			case CombatStrategyMode.StandardRange:
			case CombatStrategyMode.CoverAndAdvance:
			case CombatStrategyMode.FullCover:
			case CombatStrategyMode.FullAdvance:
				return true;
			default:
				return false;
		}
	}

	/// <summary>
	///     Determines whether or not a CombatStrategyMode inherently desires to be in melee combat, for the purpose of
	///     determining starting position
	/// </summary>
	/// <param name="mode">The mode in question</param>
	/// <returns>True if this CombatStrategyMode prefers to be in melee combat</returns>
	public static bool IsMeleeDesiredStrategy(this CombatStrategyMode mode)
	{
		switch (mode)
		{
			case CombatStrategyMode.FireAndAdvance:
			case CombatStrategyMode.CoverAndAdvance:
			case CombatStrategyMode.FullAdvance:
			case CombatStrategyMode.StandardMelee:
			case CombatStrategyMode.FullDefense:
			case CombatStrategyMode.Ward:
			case CombatStrategyMode.Clinch:
			case CombatStrategyMode.GrappleForKill:
			case CombatStrategyMode.GrappleForIncapacitation:
			case CombatStrategyMode.GrappleForControl:
			case CombatStrategyMode.MeleeMagic:
			case CombatStrategyMode.MeleeShooter:
				return true;
			default:
				return false;
		}
	}

	public static bool IsRangedStartDesiringStrategy(this CombatStrategyMode mode)
	{
		switch (mode)
		{
			case CombatStrategyMode.CoveringFire:
			case CombatStrategyMode.FireAndAdvance:
			case CombatStrategyMode.FireNoCover:
			case CombatStrategyMode.Skirmish:
			case CombatStrategyMode.FullSkirmish:
			case CombatStrategyMode.Flee:
			case CombatStrategyMode.StandardRange:
				return true;
			default:
				return false;
		}
	}

	public static bool IsCoverDesiringStrategy(this CombatStrategyMode mode)
	{
		switch (mode)
		{
			case CombatStrategyMode.CoverAndAdvance:
			case CombatStrategyMode.CoveringFire:
			case CombatStrategyMode.FullCover:
			case CombatStrategyMode.StandardRange:
				return true;
			default:
				return false;
		}
	}

	public static string Describe(this MeleeWeaponVerb verb)
	{
		switch (verb)
		{
			case MeleeWeaponVerb.Bash:
				return "Bash";
			case MeleeWeaponVerb.Chop:
				return "Chop";
			case MeleeWeaponVerb.Jab:
				return "Jab";
			case MeleeWeaponVerb.Stab:
				return "Stab";
			case MeleeWeaponVerb.Swing:
				return "Swing";
			case MeleeWeaponVerb.Thrust:
				return "Thrust";
			case MeleeWeaponVerb.Whirl:
				return "Whirl";
			case MeleeWeaponVerb.Bite:
				return "Bite";
			case MeleeWeaponVerb.Claw:
				return "Claw";
			case MeleeWeaponVerb.Kick:
				return "Kick";
			case MeleeWeaponVerb.Punch:
				return "Punch";
			case MeleeWeaponVerb.Strike:
				return "Strike";
			case MeleeWeaponVerb.Beam:
				return "Beam";
			case MeleeWeaponVerb.Sweep:
				return "Sweep";
			case MeleeWeaponVerb.Grapple:
				return "Grapple";
			case MeleeWeaponVerb.Slam:
				return "Slam";
			case MeleeWeaponVerb.Blast:
				return "Blast";
			case MeleeWeaponVerb.Swipe:
				return "Swipe";
			default:
				throw new NotImplementedException("No MeleeWeaponVerb description in Describe.");
		}
	}

	public static string Describe(this WeaponClassification classification)
	{
		switch (classification)
		{
			case WeaponClassification.Natural:
				return "Natural";
			case WeaponClassification.NonLethal:
				return "Non-Lethal";
			case WeaponClassification.Lethal:
				return "Lethal";
			case WeaponClassification.Military:
				return "Military";
			case WeaponClassification.Exotic:
				return "Exotic";
			case WeaponClassification.Ceremonial:
				return "Ceremonial";
			case WeaponClassification.Training:
				return "Training";
			case WeaponClassification.Improvised:
				return "Improvised";
			case WeaponClassification.None:
				return "None";
			case WeaponClassification.Shield:
				return "Shield";
			default:
				return "Unknown";
		}
	}

	private static string InternalDescribe(CombatMoveIntentions type, ref List<string> descriptions, bool briefMode)
	{
		while (true)
		{
			if (type.HasFlag(CombatMoveIntentions.Attack))
			{
				descriptions.Add("Attack");
				type = type ^ CombatMoveIntentions.Attack;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Disarm))
			{
				descriptions.Add("Disarm");
				type = type ^ CombatMoveIntentions.Disarm;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Wound))
			{
				descriptions.Add("Wound");
				type = type ^ CombatMoveIntentions.Wound;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Kill))
			{
				descriptions.Add("Kill");
				type = type ^ CombatMoveIntentions.Kill;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Submit))
			{
				descriptions.Add("Submit");
				type = type ^ CombatMoveIntentions.Submit;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Grapple))
			{
				descriptions.Add("Grapple");
				type = type ^ CombatMoveIntentions.Grapple;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Disadvantage))
			{
				descriptions.Add("Disadvantage");
				type = type ^ CombatMoveIntentions.Disadvantage;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Advantage))
			{
				descriptions.Add("Advantage");
				type = type ^ CombatMoveIntentions.Advantage;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Attention))
			{
				descriptions.Add("Attention");
				type = type ^ CombatMoveIntentions.Attention;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Flank))
			{
				descriptions.Add("Flank");
				type = type ^ CombatMoveIntentions.Flank;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Trip))
			{
				descriptions.Add("Trip");
				type = type ^ CombatMoveIntentions.Trip;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Stun))
			{
				descriptions.Add("Stun");
				type = type ^ CombatMoveIntentions.Stun;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Pain))
			{
				descriptions.Add("Pain");
				type = type ^ CombatMoveIntentions.Pain;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.CoupDeGrace))
			{
				descriptions.Add("Coup de Grace");
				type = type ^ CombatMoveIntentions.CoupDeGrace;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Dirty))
			{
				descriptions.Add("Dirty");
				type = type ^ CombatMoveIntentions.Dirty;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Savage))
			{
				descriptions.Add("Savage");
				type = type ^ CombatMoveIntentions.Savage;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Training))
			{
				descriptions.Add("Training");
				type = type ^ CombatMoveIntentions.Training;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Flashy))
			{
				descriptions.Add("Flashy");
				type = type ^ CombatMoveIntentions.Flashy;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Distraction))
			{
				descriptions.Add("Distraction");
				type = type ^ CombatMoveIntentions.Distraction;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Hinder))
			{
				descriptions.Add("Hinder");
				type = type ^ CombatMoveIntentions.Hinder;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Cripple))
			{
				descriptions.Add("Cripple");
				type = type ^ CombatMoveIntentions.Cripple;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Risky))
			{
				descriptions.Add("Risky");
				type = type ^ CombatMoveIntentions.Risky;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Fast))
			{
				descriptions.Add("Fast");
				type = type ^ CombatMoveIntentions.Fast;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Slow))
			{
				descriptions.Add("Slow");
				type = type ^ CombatMoveIntentions.Slow;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Aggressive))
			{
				descriptions.Add("Aggressive");
				type = type ^ CombatMoveIntentions.Aggressive;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Defensive))
			{
				descriptions.Add("Defensive");
				type = type ^ CombatMoveIntentions.Defensive;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Cautious))
			{
				descriptions.Add("Cautious");
				type = type ^ CombatMoveIntentions.Cautious;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Cruel))
			{
				descriptions.Add("Cruel");
				type = type ^ CombatMoveIntentions.Cruel;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.SelfDamaging))
			{
				descriptions.Add("Self-Damaging");
				type = type ^ CombatMoveIntentions.SelfDamaging;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Hard))
			{
				descriptions.Add("Hard");
				type = type ^ CombatMoveIntentions.Hard;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Easy))
			{
				descriptions.Add("Easy");
				type = type ^ CombatMoveIntentions.Easy;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Shield))
			{
				descriptions.Add("Shield");
				type = type ^ CombatMoveIntentions.Shield;
				continue;
			}

			if (type.HasFlag(CombatMoveIntentions.Desperate))
			{
				descriptions.Add("Desperate");
				type = type ^ CombatMoveIntentions.Desperate;
				continue;
			}

			return descriptions.Any()
				? briefMode ? descriptions.ListToCommaSeparatedValues("|") : descriptions.ListToString()
				: "None";
		}
	}

	public static bool TryParseCombatMoveIntention(string text, out CombatMoveIntentions intention)
	{
		if (Enum.TryParse(text, true, out intention))
		{
			return true;
		}

		var values = Enum.GetValues(typeof(CombatMoveIntentions)).OfType<CombatMoveIntentions>().ToList();
		if (values.Any(x => x.Describe().StartsWith(text, StringComparison.InvariantCultureIgnoreCase)))
		{
			intention =
				values.FirstOrDefault(
					x => x.Describe().StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
			return true;
		}

		return false;
	}

	public static bool TryParseCombatStrategyMode(string text, out CombatStrategyMode intention)
	{
		if (Enum.TryParse(text, true, out intention))
		{
			return true;
		}

		var values = Enum.GetValues(typeof(CombatStrategyMode)).OfType<CombatStrategyMode>().ToList();
		if (values.Any(x => x.Describe().StartsWith(text, StringComparison.InvariantCultureIgnoreCase)))
		{
			intention =
				values.FirstOrDefault(
					x => x.Describe().StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
			return true;
		}

		return false;
	}

	public static bool TryParseBuiltInMoveType(string text, out BuiltInCombatMoveType move)
	{
		if (Enum.TryParse(text, true, out move))
		{
			return true;
		}

		var values = Enum.GetValues(typeof(BuiltInCombatMoveType)).OfType<BuiltInCombatMoveType>().ToList();
		if (values.Any(x => x.Describe().StartsWith(text, StringComparison.InvariantCultureIgnoreCase)))
		{
			move =
				values.FirstOrDefault(
					x => x.Describe().StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
			return true;
		}

		return false;
	}

	public static string Describe(this CombatMoveIntentions type)
	{
		var types = new List<string>();
		return InternalDescribe(type, ref types, false);
	}

	public static string DescribeBrief(this CombatMoveIntentions type)
	{
		var types = new List<string>();
		return InternalDescribe(type, ref types, true);
	}

	public static bool IsWeaponAttackType(this BuiltInCombatMoveType type)
	{
		switch (type)
		{
			case BuiltInCombatMoveType.UseWeaponAttack:
			case BuiltInCombatMoveType.NaturalWeaponAttack:
			case BuiltInCombatMoveType.Disarm:
			case BuiltInCombatMoveType.WardFreeAttack:
			case BuiltInCombatMoveType.WardFreeUnarmedAttack:
			case BuiltInCombatMoveType.ClinchAttack:
			case BuiltInCombatMoveType.ClinchUnarmedAttack:
			case BuiltInCombatMoveType.CoupDeGrace:
			case BuiltInCombatMoveType.MeleeWeaponSmashItem:
			case BuiltInCombatMoveType.UnarmedSmashItem:
			case BuiltInCombatMoveType.StaggeringBlow:
			case BuiltInCombatMoveType.StaggeringBlowUnarmed:
			case BuiltInCombatMoveType.UnbalancingBlow:
			case BuiltInCombatMoveType.UnbalancingBlowUnarmed:
			case BuiltInCombatMoveType.UnbalancingBlowClinch:
			case BuiltInCombatMoveType.StaggeringBlowClinch:
			case BuiltInCombatMoveType.WrenchAttack:
			case BuiltInCombatMoveType.StrangleAttack:
			case BuiltInCombatMoveType.BeamAttack:
			case BuiltInCombatMoveType.ScreechAttack:
			case BuiltInCombatMoveType.DownedAttack:
			case BuiltInCombatMoveType.DownedAttackUnarmed:
			case BuiltInCombatMoveType.MagicPowerAttack:
			case BuiltInCombatMoveType.InitiateGrapple:
			case BuiltInCombatMoveType.ExtendGrapple:
			case BuiltInCombatMoveType.StrangleAttackExtendGrapple:
			case BuiltInCombatMoveType.SwoopAttack:
			case BuiltInCombatMoveType.EnvenomingAttack:
			case BuiltInCombatMoveType.EnvenomingAttackClinch:
				return true;
			default:
				return false;
		}
	}

	public static string Describe(this BuiltInCombatMoveType type)
	{
		switch (type)
		{
			case BuiltInCombatMoveType.Block:
				return "Block";
			case BuiltInCombatMoveType.Disarm:
				return "Disarm";
			case BuiltInCombatMoveType.Dodge:
				return "Dodge";
			case BuiltInCombatMoveType.Flee:
				return "Flee";
			case BuiltInCombatMoveType.NaturalWeaponAttack:
				return "Natural Weapon Attack";
			case BuiltInCombatMoveType.Parry:
				return "Parry";
			case BuiltInCombatMoveType.RetrieveItem:
				return "Retrieve Item";
			case BuiltInCombatMoveType.UseWeaponAttack:
				return "Weapon Attack";
			case BuiltInCombatMoveType.ChargeToMelee:
				return "Charge to Melee";
			case BuiltInCombatMoveType.MoveToMelee:
				return "Move to Melee";
			case BuiltInCombatMoveType.AdvanceAndFire:
				return "Advance and Fire";
			case BuiltInCombatMoveType.ReceiveCharge:
				return "Receive Charge";
			case BuiltInCombatMoveType.WardDefense:
				return "Ward Defense";
			case BuiltInCombatMoveType.WardCounter:
				return "Ward Counter";
			case BuiltInCombatMoveType.WardFreeAttack:
				return "Ward Free Attack";
			case BuiltInCombatMoveType.WardFreeUnarmedAttack:
				return "Ward Free Unarmed Attack";
			case BuiltInCombatMoveType.StartClinch:
				return "Start Clinch";
			case BuiltInCombatMoveType.ResistClinch:
				return "Resist Start Clinch";
			case BuiltInCombatMoveType.BreakClinch:
				return "Break Clinch";
			case BuiltInCombatMoveType.ResistBreakClinch:
				return "Resist Break Clinch";
			case BuiltInCombatMoveType.ClinchAttack:
				return "Clinch Attack";
			case BuiltInCombatMoveType.ClinchUnarmedAttack:
				return "Clinch Unarmed Attack";
			case BuiltInCombatMoveType.ClinchDodge:
				return "Clinch Dodge";
			case BuiltInCombatMoveType.InitiateGrapple:
				return "Initiate Grapple";
			case BuiltInCombatMoveType.DodgeGrapple:
				return "Dodge Grapple";
			case BuiltInCombatMoveType.CounterGrapple:
				return "Counter Grapple";
			case BuiltInCombatMoveType.ExtendGrapple:
				return "Extend Grapple";
			case BuiltInCombatMoveType.WrenchAttack:
				return "Wrench Attack";
			case BuiltInCombatMoveType.StrangleAttack:
				return "Strangle Attack";
			case BuiltInCombatMoveType.ScreechAttack:
				return "Screech Attack";
			case BuiltInCombatMoveType.BeamAttack:
				return "Beam Attack";
			case BuiltInCombatMoveType.DodgeRange:
				return "Dodge Range";
			case BuiltInCombatMoveType.BlockRange:
				return "Block Range";
			case BuiltInCombatMoveType.StandAndFire:
				return "Stand and Fire";
			case BuiltInCombatMoveType.SkirmishAndFire:
				return "Skirmish and Fire";
			case BuiltInCombatMoveType.RangedWeaponAttack:
				return "Ranged Weapon Attack";
			case BuiltInCombatMoveType.AimRangedWeapon:
				return "Aim Ranged Weapon";
			case BuiltInCombatMoveType.CoupDeGrace:
				return "Coup de Grace";
			case BuiltInCombatMoveType.Rescue:
				return "Rescue";
			case BuiltInCombatMoveType.MeleeWeaponSmashItem:
				return "Smash Item";
			case BuiltInCombatMoveType.UnarmedSmashItem:
				return "Unarmed Smash Item";
			case BuiltInCombatMoveType.StaggeringBlow:
				return "Staggering Blow";
			case BuiltInCombatMoveType.StaggeringBlowUnarmed:
				return "Unarmed Staggering Blow";
			case BuiltInCombatMoveType.StaggeringBlowClinch:
				return "Clinch Staggering Blow";
			case BuiltInCombatMoveType.UnbalancingBlow:
				return "Unbalancing Blow";
			case BuiltInCombatMoveType.UnbalancingBlowUnarmed:
				return "Unarmed Unbalancing Blow";
			case BuiltInCombatMoveType.UnbalancingBlowClinch:
				return "Clinch Unbalancing Blow";
			case BuiltInCombatMoveType.DodgeExtendGrapple:
				return "Dodge Extend Grapple";
			case BuiltInCombatMoveType.OverpowerGrapple:
				return "Overpowering Grapple";
			case BuiltInCombatMoveType.StrangleAttackExtendGrapple:
				return "Strangle Attack Extend Grapple";
			case BuiltInCombatMoveType.DesperateDodge:
				return "Desperate Dodge";
			case BuiltInCombatMoveType.DesperateParry:
				return "Desperate Parry";
			case BuiltInCombatMoveType.DesperateBlock:
				return "Desperate Block";
			case BuiltInCombatMoveType.DownedAttack:
				return "Downed Attack";
			case BuiltInCombatMoveType.DownedAttackUnarmed:
				return "Unarmed Downed Attack";
			case BuiltInCombatMoveType.MagicPowerAttack:
				return "Magic Power Attack";
			case BuiltInCombatMoveType.TakedownMove:
				return "Takedown";
			case BuiltInCombatMoveType.Breakout:
				return "Breakout";
			case BuiltInCombatMoveType.SwoopAttack:
				return "Swoop Attack";
			case BuiltInCombatMoveType.SwoopAttackUnarmed:
				return "Unarmed Swoop Attack";
			case BuiltInCombatMoveType.EnvenomingAttack:
				return "Envenoming Attack";
			case BuiltInCombatMoveType.EnvenomingAttackClinch:
				return "Envenoming Clinch Attack";
			case BuiltInCombatMoveType.AuxiliaryMove:
				return "Auxiliary Move";
			default:
				throw new ArgumentOutOfRangeException(nameof(type), type, null);
		}

		return "Unknown";
	}
}
#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat;
using MudSharp.RPG.Checks;
using System;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RangedWeaponFeatureSourceTests
{
	[TestMethod]
	public void FireBlowgun_CheckClassification_MatchesRangedAttackChecks()
	{
		Assert.IsTrue(CheckType.FireBlowgun.IsPhysicalActivityCheck());
		Assert.IsTrue(CheckType.FireBlowgun.IsOffensiveCombatAction());
		Assert.IsTrue(CheckType.FireBlowgun.IsTargettedHostileCheck());
		Assert.IsTrue(CheckType.FireBlowgun.IsVisionInfluencedCheck());
		Assert.IsFalse(CheckType.FireBlowgun.IsDefensiveCombatAction());
		Assert.IsTrue((int)RangedWeaponType.Blowgun > (int)RangedWeaponType.Musket);
	}

	[TestMethod]
	public void RangedWeaponHiddenFire_DefaultsFalseAndBlowgunOptInPreservesHide()
	{
		string interfaceSource = ReadSource("FutureMUDLibrary", "GameItems", "Interfaces", "IRangedWeapon.cs");
		string blowgunSource = ReadSource("MudSharpCore", "GameItems", "Components", "BlowgunGameItemComponent.cs");
		string combatModuleSource = ReadSource("MudSharpCore", "Commands", "Modules", "CombatModule.cs");
		string characterCombatSource = ReadSource("MudSharpCore", "Character", "CharacterCombat.cs");
		string simpleCombatSource = ReadSource("MudSharpCore", "Combat", "SimpleMeleeCombat.cs");

		StringAssert.Contains(interfaceSource, "bool CanFireWhileHidden => false;");
		StringAssert.Contains(blowgunSource, "public bool CanFireWhileHidden => true;");
		StringAssert.Contains(blowgunSource, "OutputFlags.SuppressObscured | OutputFlags.InnerWrap");
		StringAssert.Contains(combatModuleSource, "aiming.Weapon.CanFireWhileHidden");
		StringAssert.Contains(characterCombatSource, "if (!preserveHide)");
		StringAssert.Contains(characterCombatSource, "OutputFlags engageFlags = preserveHide ? OutputFlags.SuppressObscured : OutputFlags.Normal;");
		StringAssert.Contains(simpleCombatSource, "preserveHide && x.IsEffectType<IHideEffect>()");
	}

	[TestMethod]
	public void BlowgunUse_RequiresBreathAndAnUncoveredMouth()
	{
		string blowgunSource = ReadSource("MudSharpCore", "GameItems", "Components", "BlowgunGameItemComponent.cs");
		string rangedAttackSource = ReadSource("MudSharpCore", "Combat", "Moves", "RangedWeaponAttackBase.cs");

		StringAssert.Contains(blowgunSource, "CanUseBreathToFire");
		StringAssert.Contains(blowgunSource, "actor.Body.Bodyparts.OfType<MouthProto>().FirstOrDefault()");
		StringAssert.Contains(blowgunSource, "!actor.Body.IsBreathing");
		StringAssert.Contains(blowgunSource, "actor.Body.WornItemsFor(mouth).FirstOrDefault()");
		StringAssert.Contains(blowgunSource, "LoadedAmmo != null && IsReadied && CanUseBreathToFire(actor).Success");
		StringAssert.Contains(rangedAttackSource, "Weapon.ReadyToFire && !Weapon.CanFire(Assailant, target)");
	}

	[TestMethod]
	public void ReadiedRangedWeaponStaminaDrain_IsSharedByBowsAndSlings()
	{
		string bowSource = ReadSource("MudSharpCore", "GameItems", "Components", "BowGameItemComponent.cs");
		string slingSource = ReadSource("MudSharpCore", "GameItems", "Components", "SlingGameItemComponent.cs");
		string effectSource = ReadSource("MudSharpCore", "Effects", "Concrete", "ReadiedRangedWeaponDrainStamina.cs");

		StringAssert.Contains(bowSource, "IReadiedRangedWeaponStaminaSource");
		StringAssert.Contains(slingSource, "IReadiedRangedWeaponStaminaSource");
		StringAssert.Contains(bowSource, "new ReadiedRangedWeaponDrainStamina(readier, this)");
		StringAssert.Contains(slingSource, "new ReadiedRangedWeaponDrainStamina(readier, this)");
		StringAssert.Contains(effectSource, "WeaponComponent.ReadiedUseRequiresFreeHand");
		StringAssert.Contains(effectSource, "WeaponComponent.Unready(null)");
	}

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts))));
	}
}

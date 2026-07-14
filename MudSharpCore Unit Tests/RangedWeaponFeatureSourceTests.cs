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

	[TestMethod]
	public void SlingAndBlowgunDamage_IsProvidedByLoadedAmmunition()
	{
		string slingSource = ReadSource("MudSharpCore", "GameItems", "Components", "SlingGameItemComponent.cs");
		string blowgunSource = ReadSource("MudSharpCore", "GameItems", "Components", "BlowgunGameItemComponent.cs");
		string ammunitionSource = ReadSource("MudSharpCore", "GameItems", "Components", "AmmunitionGameItemComponent.cs");

		StringAssert.Contains(slingSource, "ammo.Fire(actor, target, shotOutcome");
		StringAssert.Contains(blowgunSource, "ammo.Fire(actor, target, shotOutcome");
		StringAssert.Contains(ammunitionSource, "private Damage BuildDamage");
		Assert.IsFalse(slingSource.Contains("GetDamage("));
		Assert.IsFalse(blowgunSource.Contains("GetDamage("));
		StringAssert.Contains(slingSource, "plan.FinalisePlan();");
		StringAssert.Contains(blowgunSource, "plan.FinalisePlan();");
		Assert.IsFalse(slingSource.Contains("_ => throw new NotImplementedException()"));
		Assert.IsFalse(blowgunSource.Contains("_ => throw new NotImplementedException()"));
	}

	[TestMethod]
	public void NaturalRangedCombatResolution_FiresOnUseAfterResolutionAndSupportsItemTargets()
	{
		string combatSource = ReadSource("MudSharpCore", "Combat", "CombatBase.cs");
		string factorySource = ReadSource("MudSharpCore", "Combat", "CombatMoveFactory.cs");
		string naturalMoveSource = ReadSource("MudSharpCore", "Combat", "Moves", "NaturalRangedAttackMoveBase.cs");
		string weaponMoveSource = ReadSource("MudSharpCore", "Combat", "Moves", "WeaponAttackMove.cs");

		int resolveIndex = combatSource.IndexOf("CombatMoveResult result = move.ResolveMove(targetResponse);",
			StringComparison.Ordinal);
		int progIndex = combatSource.IndexOf("FireOnUseProg(move);", resolveIndex, StringComparison.Ordinal);
		Assert.IsTrue(resolveIndex >= 0 && progIndex > resolveIndex);
		StringAssert.Contains(factorySource,
			"return new RangedNaturalAttackMove(assailant, attack, target);");
		StringAssert.Contains(factorySource,
			"return new SpitAttackMove(assailant, attack, target);");
		StringAssert.Contains(naturalMoveSource, "target is not IHaveWounds targetWithWounds");
		StringAssert.Contains(naturalMoveSource, "return targetWithWounds.PassiveSufferDamage(damages.Item1)");
		StringAssert.Contains(weaponMoveSource, "attack.Attack.Profile.StunExpression.Evaluate(Assailant");
		StringAssert.Contains(weaponMoveSource, "attack.Attack.Profile.PainExpression.Evaluate(Assailant");
		StringAssert.Contains(weaponMoveSource, "? CheckType.RangedWeaponPenetrateCheck");
	}

	[TestMethod]
	public void NaturalRangedElementalBuildersAndExtinguishing_AreReachable()
	{
		string breathSource = ReadSource("MudSharpCore", "Combat", "BreathWeaponAttack.cs");
		string fireSource = ReadSource("MudSharpCore", "Combat", "FireProfile.cs");
		string liquidSource = ReadSource("MudSharpCore", "Form", "Material", "Liquid.cs");
		string bodySource = ReadSource("MudSharpCore", "Body", "Implementations", "Body.cs");
		string itemSource = ReadSource("MudSharpCore", "GameItems", "GameItem.cs");

		StringAssert.Contains(breathSource, "return BuildingCommandFire(actor, command);");
		StringAssert.Contains(fireSource, "case \"extinguish\":");
		StringAssert.Contains(fireSource, "case \"interval\":");
		StringAssert.Contains(liquidSource, "return BuildingCommandSurfaceReaction(actor, command);");
		StringAssert.Contains(liquidSource, "new LiquidSurfaceReaction(Gameworld)");
		StringAssert.Contains(bodySource, "OnFire.ExtinguishWith(Actor, mixture);");
		StringAssert.Contains(itemSource, "OnFire.ExtinguishWith(this, mixture);");
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

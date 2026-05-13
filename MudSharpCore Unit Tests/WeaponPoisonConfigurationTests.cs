#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WeaponPoisonConfigurationTests
{
	[TestMethod]
	public void ApplyPoisonCheck_IsPhysicalVisionInfluencedActivity()
	{
		Assert.IsTrue(CheckType.ApplyPoisonToWeapon.IsNonStaticCheck());
		Assert.IsTrue(CheckType.ApplyPoisonToWeapon.IsGeneralActivityCheck());
		Assert.IsTrue(CheckType.ApplyPoisonToWeapon.IsPhysicalActivityCheck());
		Assert.IsTrue(CheckType.ApplyPoisonToWeapon.IsVisionInfluencedCheck());
		Assert.IsFalse(CheckType.ApplyPoisonToWeapon.IsOffensiveCombatAction());
		Assert.IsFalse(CheckType.ApplyPoisonToWeapon.IsDefensiveCombatAction());
	}

	[TestMethod]
	public void StaticDefaults_IncludeWeaponPoisonTuningValues()
	{
		Dictionary<string, string> expected = new()
		{
			[WeaponPoisonDeliveryHelper.ApplyDefaultCapacityFraction] = "0.25",
			[WeaponPoisonDeliveryHelper.DipCapacityFraction] = "1.0",
			[WeaponPoisonDeliveryHelper.DosePerHitCapacityFraction] = "0.20",
			[WeaponPoisonDeliveryHelper.DeliveryMinimumChance] = "0.05",
			[WeaponPoisonDeliveryHelper.DeliveryMaximumChance] = "0.95",
			[WeaponPoisonDeliveryHelper.DipDifficultyStepsEasier] = "2",
			[WeaponPoisonDeliveryHelper.ContactDamageMultiplier] = "1.0",
			[WeaponPoisonDeliveryHelper.ExternalBleedingWoundMultiplier] = "1.10",
			[WeaponPoisonDeliveryHelper.ExternalNonBleedingWoundMultiplier] = "1.0",
			[WeaponPoisonDeliveryHelper.InternalWoundMultiplier] = "0.60"
		};

		foreach (var (key, value) in expected)
		{
			Assert.IsTrue(DefaultStaticSettings.DefaultStaticConfigurations.ContainsKey(key), $"Missing {key}.");
			Assert.AreEqual(value, DefaultStaticSettings.DefaultStaticConfigurations[key], key);
		}
	}

	[TestMethod]
	public void StaticDefaults_IncludeAllDamageTypeDeliveryMultipliers()
	{
		HashSet<DamageType> fullInjected =
		[
			DamageType.Piercing,
			DamageType.ArmourPiercing,
			DamageType.BallisticArmourPiercing
		];
		HashSet<DamageType> partialInjected =
		[
			DamageType.Slashing,
			DamageType.Chopping,
			DamageType.Shearing,
			DamageType.Bite,
			DamageType.Claw,
			DamageType.Ballistic,
			DamageType.Shrapnel
		];

		foreach (var type in Enum.GetValues<DamageType>())
		{
			var key = WeaponPoisonDeliveryHelper.InjectedDamageMultiplierConfiguration(type);
			Assert.IsTrue(DefaultStaticSettings.DefaultStaticConfigurations.ContainsKey(key), $"Missing {key}.");
			var expected = fullInjected.Contains(type)
				? "1.0"
				: partialInjected.Contains(type)
					? "0.75"
					: "0.0";
			Assert.AreEqual(expected, DefaultStaticSettings.DefaultStaticConfigurations[key], key);
		}
	}

	[TestMethod]
	public void StaticDefaults_IncludeAllSeverityDeliveryMultipliers()
	{
		Dictionary<WoundSeverity, string> expected = new()
		{
			[WoundSeverity.None] = "0.0",
			[WoundSeverity.Superficial] = "0.20",
			[WoundSeverity.Minor] = "0.35",
			[WoundSeverity.Small] = "0.50",
			[WoundSeverity.Moderate] = "0.70",
			[WoundSeverity.Severe] = "0.85",
			[WoundSeverity.VerySevere] = "0.95",
			[WoundSeverity.Grievous] = "0.95",
			[WoundSeverity.Horrifying] = "0.95"
		};

		foreach (var severity in Enum.GetValues<WoundSeverity>())
		{
			var key = WeaponPoisonDeliveryHelper.SeverityMultiplierConfiguration(severity);
			Assert.IsTrue(DefaultStaticSettings.DefaultStaticConfigurations.ContainsKey(key), $"Missing {key}.");
			Assert.AreEqual(expected[severity], DefaultStaticSettings.DefaultStaticConfigurations[key], key);
		}
	}
}

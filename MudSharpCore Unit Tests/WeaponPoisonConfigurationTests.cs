#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
	[DataTestMethod]
	[DataRow(DrugVector.Injected)]
	[DataRow(DrugVector.Touched)]
	public void DeliveryChance_NoneSeverityDoesNotReceiveMinimumChance(DrugVector vector)
	{
		var world = PoisonWorld();
		var wound = Mock.Of<IWound>(x => x.Severity == WoundSeverity.None && x.DamageType == DamageType.Piercing);
		Assert.AreEqual(0.0, WeaponPoisonDeliveryHelper.CalculateDeliveryChance(world.Object, wound, vector));
	}

	[DataTestMethod]
	[DataRow("severity", 0.0)]
	[DataRow("severity", -1.0)]
	[DataRow("severity", double.NaN)]
	[DataRow("damage", 0.0)]
	[DataRow("damage", -1.0)]
	[DataRow("nature", 0.0)]
	[DataRow("nature", -1.0)]
	[DataRow("nature", double.PositiveInfinity)]
	public void DeliveryChance_NonpositiveOrInvalidGateDisablesDelivery(string gate, double value)
	{
		var world = PoisonWorld();
		var key = gate switch
		{
			"severity" => WeaponPoisonDeliveryHelper.SeverityMultiplierConfiguration(WoundSeverity.Minor),
			"damage" => WeaponPoisonDeliveryHelper.ContactDamageMultiplier,
			_ => WeaponPoisonDeliveryHelper.ExternalNonBleedingWoundMultiplier
		};
		world.Setup(x => x.GetStaticDouble(key)).Returns(value);
		var wound = Mock.Of<IWound>(x => x.Severity == WoundSeverity.Minor && x.DamageType == DamageType.Piercing);
		Assert.AreEqual(0.0, WeaponPoisonDeliveryHelper.CalculateDeliveryChance(world.Object, wound, DrugVector.Touched));
	}

	[DataTestMethod]
	[DataRow(0.001, 0.05)]
	[DataRow(0.5, 0.5)]
	[DataRow(10.0, 0.95)]
	public void DeliveryChance_PositiveProductKeepsConfiguredClamp(double severity, double expected)
	{
		var world = PoisonWorld();
		world.Setup(x => x.GetStaticDouble(WeaponPoisonDeliveryHelper.SeverityMultiplierConfiguration(WoundSeverity.Minor))).Returns(severity);
		var wound = Mock.Of<IWound>(x => x.Severity == WoundSeverity.Minor && x.DamageType == DamageType.Piercing);
		Assert.AreEqual(expected, WeaponPoisonDeliveryHelper.CalculateDeliveryChance(world.Object, wound, DrugVector.Touched));
	}

	private static Mock<IFuturemud> PoisonWorld()
	{
		var world = new Mock<IFuturemud>();
		world.Setup(x => x.GetStaticDouble(It.IsAny<string>())).Returns((string key) =>
			double.Parse(DefaultStaticSettings.DefaultStaticConfigurations[key], System.Globalization.CultureInfo.InvariantCulture));
		return world;
	}

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

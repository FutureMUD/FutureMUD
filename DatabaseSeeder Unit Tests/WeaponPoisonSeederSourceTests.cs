#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.RPG.Checks;
using System;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WeaponPoisonSeederSourceTests
{
	[TestMethod]
	public void SkillSeeders_RegisterApplyPoisonCheck()
	{
		var skillSeeder = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "SkillSeeder.cs"));
		var packageSeeder = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "SkillPackageSeeder.cs"));
		var checkName = $"CheckType.{CheckType.ApplyPoisonToWeapon}";

		StringAssert.Contains(skillSeeder, checkName);
		StringAssert.Contains(packageSeeder, checkName);
		StringAssert.Contains(packageSeeder, "GetSkill(\"Poisoning\", \"Chemistry\", \"Herbalism\", \"Medicine\")");
		StringAssert.Contains(packageSeeder, "Expression = $\"poison:{poisonTrait.Id}\"");
	}

	[TestMethod]
	public void StaticConfigurationSource_SeedsAllWeaponPoisonDefaults()
	{
		var defaultSettings = File.ReadAllText(GetSourcePath("FutureMUDLibrary", "Framework", "DefaultStaticSettings.cs"));
		string[] requiredKeys =
		[
			"WeaponPoisonApplyDefaultCapacityFraction",
			"WeaponPoisonDipCapacityFraction",
			"WeaponPoisonDosePerHitCapacityFraction",
			"WeaponPoisonDeliveryMinimumChance",
			"WeaponPoisonDeliveryMaximumChance",
			"WeaponPoisonDipDifficultyStepsEasier",
			"WeaponPoisonContactDamageMultiplier",
			"WeaponPoisonExternalBleedingWoundMultiplier",
			"WeaponPoisonExternalNonBleedingWoundMultiplier",
			"WeaponPoisonInternalWoundMultiplier",
			"WeaponPoisonSeverityMultiplierNone",
			"WeaponPoisonSeverityMultiplierSuperficial",
			"WeaponPoisonSeverityMultiplierMinor",
			"WeaponPoisonSeverityMultiplierSmall",
			"WeaponPoisonSeverityMultiplierModerate",
			"WeaponPoisonSeverityMultiplierSevere",
			"WeaponPoisonSeverityMultiplierVerySevere",
			"WeaponPoisonSeverityMultiplierGrievous",
			"WeaponPoisonSeverityMultiplierHorrifying"
		];

		foreach (var key in requiredKeys)
		{
			StringAssert.Contains(defaultSettings, $"{{ \"{key}\",");
		}

		foreach (var damageType in Enum.GetNames(typeof(MudSharp.Health.DamageType)))
		{
			StringAssert.Contains(defaultSettings, $"{{ \"WeaponPoisonInjectedDamageMultiplier{damageType}\",");
		}
	}

	private static string GetSourcePath(params string[] parts)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts)));
	}
}

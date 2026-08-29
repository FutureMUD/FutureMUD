#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SkillPackageSeederTests
{
	[TestMethod]
	public void ResolveSeededSkillName_NonGerundEndurance_PreservesArmAttribute()
	{
		TraitDefinition[] existing = [new TraitDefinition { Name = "Endurance", Type = 1 }];
		SkillPackageSeeder.SkillDetails details = new(
			"Enduring", "Endurance", "Athletic", "con", "General", "General", true, 1.0);

		string result = SkillPackageSeeder.ResolveSeededSkillNameForTesting(existing, details, useGerund: false);

		Assert.AreEqual("Enduring", result);
		Assert.AreEqual(1, existing[0].Type);
	}

	[TestMethod]
	public void ResolveSeededSkillName_WhenBothNamesCollide_UsesStableSkillSuffix()
	{
		TraitDefinition[] existing = [new TraitDefinition { Name = "Perception", Type = 1 }];
		SkillPackageSeeder.SkillDetails details = new(
			"Perception", "Perception", "Perception", "per", "General", "General", true, 1.0);

		string result = SkillPackageSeeder.ResolveSeededSkillNameForTesting(existing, details, useGerund: false);

		Assert.AreEqual("Perception Skill", result);
	}

	[TestMethod]
	public void ResolveSeededSkillName_RerunReusesAlternateSkill()
	{
		TraitDefinition[] existing =
		[
			new TraitDefinition { Name = "Endurance", Type = 1 },
			new TraitDefinition { Name = "Enduring", Type = 0 }
		];
		SkillPackageSeeder.SkillDetails details = new(
			"Enduring", "Endurance", "Athletic", "con", "General", "General", true, 1.0);

		string result = SkillPackageSeeder.ResolveSeededSkillNameForTesting(existing, details, useGerund: false);

		Assert.AreEqual("Enduring", result);
	}

	[TestMethod]
	public void ComplexSkillPackage_IncludesDrivingForVehicleCombatChecks()
	{
		var names = new SkillPackageSeeder().ComplexNonGerundSkillNamesForTesting;

		CollectionAssert.Contains(names.ToList(), "Drive");
	}

	[TestMethod]
	public void ComplexSkillPackage_IncludesUniversalCombatSkills()
	{
		var names = new SkillPackageSeeder().ComplexNonGerundSkillNamesForTesting;

		foreach (var name in new[] { "Block", "Dodge", "Brawling", "Subdue", "Ward", "Throwing", "Gunnery", "Seafaring", "Veterancy" })
		{
			CollectionAssert.Contains(names.ToList(), name);
		}
	}

	[TestMethod]
	public void SkillPackageChecks_MountSprawlUsesRidingAndBalance()
	{
		string source = SeederSourceTestHelper.ReadSeederSource("SkillPackageSeeder.cs");

		StringAssert.Contains(source, "case CheckType.AvoidMountFallCheck:");
		StringAssert.Contains(source, "(0.7*ride:{ridingTrait.Id})+(0.3*balance:{balancingTrait.Id})");
	}

	[TestMethod]
	public void SkillPackageChecks_DefersOnlyTrapCheckTypesToTrapSeeder()
	{
		string source = SeederSourceTestHelper.ReadSeederSource("SkillPackageSeeder.cs");
		var handledCheckTypes = Regex.Matches(source, @"case\s+CheckType\.(?<name>\w+)")
			.Select(x => x.Groups["name"].Value)
			.ToHashSet(System.StringComparer.Ordinal);
		var deferredTrapCheckTypes = new[]
		{
			CheckType.SetTrapCheck,
			CheckType.SpotTrapCheck,
			CheckType.SearchForTrapCheck,
			CheckType.AvoidTrapCheck,
			CheckType.DisarmTrapCheck,
			CheckType.DispelTrapCheck,
			CheckType.EscapeTrapCheck
		};

		foreach (CheckType checkType in System.Enum.GetValues<CheckType>())
		{
			Assert.IsTrue(handledCheckTypes.Contains(checkType.ToString()),
				$"SkillPackageSeeder must explicitly account for {checkType}.");
		}

		foreach (CheckType checkType in deferredTrapCheckTypes)
		{
			StringAssert.Contains(source, "TrapSeeder owns the optional trap skill and its check formulas.");
			Assert.IsTrue(handledCheckTypes.Contains(checkType.ToString()));
		}
	}
}

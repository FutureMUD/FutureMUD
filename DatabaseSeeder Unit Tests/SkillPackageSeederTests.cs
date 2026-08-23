#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;
using System.Linq;

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
	public void SkillPackageChecks_MountSprawlUsesRidingAndBalance()
	{
		string source = SeederSourceTestHelper.ReadSeederSource("SkillPackageSeeder.cs");

		StringAssert.Contains(source, "case CheckType.AvoidMountFallCheck:");
		StringAssert.Contains(source, "(0.7*ride:{ridingTrait.Id})+(0.3*balance:{balancingTrait.Id})");
	}
}

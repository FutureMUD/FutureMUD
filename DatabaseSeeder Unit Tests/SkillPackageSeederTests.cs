#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;

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
}

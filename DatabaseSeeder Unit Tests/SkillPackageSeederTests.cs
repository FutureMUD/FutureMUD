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

#nullable enable

using System;
using System.IO;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatStrategySeederCompatibilityTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static FutureProg CreateFutureProg(long id, string functionName)
	{
		return new FutureProg
		{
			Id = id,
			FunctionName = functionName,
			FunctionComment = $"{functionName} test prog",
			FunctionText = "return true;",
			ReturnType = 0,
			Category = "Tests",
			Subcategory = "CombatSeeder",
			Public = true,
			AcceptsAnyParameters = false,
			StaticType = 0
		};
	}

	[TestMethod]
	public void CanonicalStrategyNames_ContainRequiredNonHumanoidCatalogue()
	{
		CollectionAssert.IsSubsetOf(
			new[]
			{
				"Melee (Auto)",
				"Beast Brawler",
				"Beast Clincher",
				"Beast Behemoth",
				"Beast Skirmisher",
				"Beast Swooper",
				"Beast Artillery",
				"Beast Coward",
				"Construct Brawler",
				"Construct Skirmisher",
				"Construct Artillery"
			},
			CombatStrategySeederHelper.CanonicalStrategyNames.ToArray());
	}

	[TestMethod]
	public void EnsureCombatStrategy_OlderWorldMissingCanonicalStrategy_AddsItByNameWithoutDuplicating()
	{
		using var context = BuildContext();
		context.FutureProgs.AddRange(
			CreateFutureProg(1, "AlwaysTrue"),
			CreateFutureProg(2, "IsHumanoid"));
		context.CharacterCombatSettings.Add(new CharacterCombatSetting
		{
			Id = 10,
			Name = "Legacy Setting",
			Description = "Existing world data",
			GlobalTemplate = true,
			ClassificationsAllowed = "1 2 3",
			MeleeAttackOrderPreference = "1 2 3"
		});
		context.SaveChanges();

		var first = CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Brawler");
		var second = CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Brawler");

		Assert.AreEqual(first.Id, second.Id);
		Assert.AreEqual(2, context.CharacterCombatSettings.Count());
		Assert.IsTrue(context.CharacterCombatSettings.Any(x => x.Name == "Legacy Setting"));
		Assert.IsTrue(context.CharacterCombatSettings.Any(x => x.Name == "Beast Brawler"));
	}

	[TestMethod]
	public void SeederSources_DependentSeeders_EnsureStrategiesByNameBeforeApplyingRaceDefaults()
	{
		var animalSource = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "AnimalSeeder.cs"));
		var mythicalSource = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "MythicalAnimalSeeder.cs"));
		var robotSource = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "RobotSeeder.Races.cs"));

		StringAssert.Contains(animalSource, "CombatStrategySeederHelper.EnsureCombatStrategy(_context, template.CombatStrategyKey)");
		StringAssert.Contains(mythicalSource, "CombatStrategySeederHelper.EnsureCombatStrategy(_context, template.CombatStrategyKey)");
		StringAssert.Contains(robotSource, "CombatStrategySeederHelper.EnsureCombatStrategy(_context, CombatStrategyFor(template))");
	}

	[TestMethod]
	public void CombatSeederSource_RerunsEnsureCanonicalStrategiesWithoutEmptyTableGuard()
	{
		var source = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "CombatSeeder.cs"));

		StringAssert.Contains(source, "SeedCombatStrategies(context, questionAnswers);");
		StringAssert.Contains(source, "CombatStrategySeederHelper.EnsureCombatStrategy(context, \"Beast Brawler\");");
		Assert.IsFalse(source.Contains("if (!context.CharacterCombatSettings.Any())"),
			"CombatSeeder should no longer skip combat strategy seeding just because the table is non-empty.");
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

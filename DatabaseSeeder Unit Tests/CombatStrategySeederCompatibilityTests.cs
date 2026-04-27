#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatStrategySeederCompatibilityTests
{
    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
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
                "Beast Drowner",
                "Beast Dropper",
                "Beast Physical Avoider",
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
        using FuturemudDatabaseContext context = BuildContext();
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

        CharacterCombatSetting first = CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Brawler");
        CharacterCombatSetting second = CombatStrategySeederHelper.EnsureCombatStrategy(context, "Beast Brawler");

        Assert.AreEqual(first.Id, second.Id);
        Assert.AreEqual(2, context.CharacterCombatSettings.Count());
        Assert.IsTrue(context.CharacterCombatSettings.Any(x => x.Name == "Legacy Setting"));
        Assert.IsTrue(context.CharacterCombatSettings.Any(x => x.Name == "Beast Brawler"));
    }

    [TestMethod]
    public void EnsureCombatStrategy_NewPredatorStrategies_CreateCanonicalSettings()
    {
        using FuturemudDatabaseContext context = BuildContext();
        context.FutureProgs.AddRange(
            CreateFutureProg(1, "AlwaysTrue"),
            CreateFutureProg(2, "IsHumanoid"));
        context.SaveChanges();

        foreach (string strategy in new[] { "Beast Drowner", "Beast Dropper", "Beast Physical Avoider" })
        {
            CharacterCombatSetting setting = CombatStrategySeederHelper.EnsureCombatStrategy(context, strategy);

            Assert.AreEqual(strategy, setting.Name);
            Assert.IsTrue(setting.GlobalTemplate);
        }
    }

    [TestMethod]
    public void SeederSources_DependentSeeders_EnsureStrategiesByNameBeforeApplyingRaceDefaults()
    {
        string animalSource = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "AnimalSeeder.cs"));
        string mythicalSource = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "MythicalAnimalSeeder.cs"));
        string robotSource = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "RobotSeeder.Races.cs"));

        StringAssert.Contains(animalSource, "CombatStrategySeederHelper.EnsureCombatStrategy(_context, template.CombatStrategyKey)");
        StringAssert.Contains(mythicalSource, "CombatStrategySeederHelper.EnsureCombatStrategy(_context, template.CombatStrategyKey)");
        StringAssert.Contains(robotSource, "CombatStrategySeederHelper.EnsureCombatStrategy(_context, CombatStrategyFor(template))");
    }

    [TestMethod]
    public void CombatSeederSource_RerunsEnsureCanonicalStrategiesWithoutEmptyTableGuard()
    {
        string source = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "CombatSeeder.cs"));

        StringAssert.Contains(source, "SeedCombatStrategies(context, questionAnswers);");
        StringAssert.Contains(source, "CombatStrategySeederHelper.EnsureCombatStrategy(context, \"Beast Brawler\");");
        StringAssert.Contains(source, "CombatStrategySeederHelper.EnsureCombatStrategy(context, \"Beast Drowner\");");
        StringAssert.Contains(source, "CombatStrategySeederHelper.EnsureCombatStrategy(context, \"Beast Dropper\");");
        StringAssert.Contains(source, "CombatStrategySeederHelper.EnsureCombatStrategy(context, \"Beast Physical Avoider\");");
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

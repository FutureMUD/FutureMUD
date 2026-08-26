#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat;
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
				"Dual Wielder",
				"Dual Wielder (Auto)",
				"Dual Wield Clincher",
				"Dual Wield Clincher (Auto)",
				"Polearm Warder",
				"Polearm Warder (Auto)",
				"Spear Warder",
				"Spear Warder (Auto)",
                "Beast Brawler",
                "Beast Clincher",
                "Beast Behemoth",
                "Beast Skirmisher",
                "Beast Swooper",
                "Beast Drowner",
                "Beast Dropper",
                "Beast Physical Avoider",
                "Beast Artillery",
				"Beast Aquatic Brawler",
				"Beast Aquatic Clincher",
				"Beast Aquatic Behemoth",
				"Beast Aquatic Skirmisher",
				"Beast Aquatic Artillery",
                "Beast Coward",
                "Construct Brawler",
                "Construct Skirmisher",
				"Construct Artillery",
				"Cavalry Charge",
				"Mounted Skirmisher",
				"Mounted Hit and Run"
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
	public void EnsureCombatStrategy_DualWieldVariants_CreateAndRepairCanonicalSettings()
	{
		using var context = BuildContext();
		context.FutureProgs.AddRange(
			CreateFutureProg(1, "AlwaysTrue"),
			CreateFutureProg(2, "IsHumanoid"));
		context.CharacterCombatSettings.Add(new CharacterCombatSetting
		{
			Name = "Dual Wielder (Auto)",
			Description = "Drifted setting",
			ClassificationsAllowed = "1 2 3",
			MeleeAttackOrderPreference = "1 2 3",
			PreferredWeaponSetup = (int)AttackHandednessOptions.Any,
			PreferShieldUse = true,
			WeaponUsePercentage = 1.0
		});
		context.SaveChanges();

		var manual = CombatStrategySeederHelper.EnsureCombatStrategy(context, "Dual Wielder");
		var automatic = CombatStrategySeederHelper.EnsureCombatStrategy(context, "Dual Wielder (Auto)");

		foreach (var setting in new[] { manual, automatic })
		{
			Assert.AreEqual((int)AttackHandednessOptions.DualWieldOnly, setting.PreferredWeaponSetup);
			Assert.AreEqual(0.9, setting.WeaponUsePercentage, 0.0001);
			Assert.AreEqual(0.1, setting.AuxiliaryPercentage, 0.0001);
			Assert.IsFalse(setting.PreferShieldUse);
			Assert.AreEqual("IsHumanoid", setting.AvailabilityProg.FunctionName);
		}

		Assert.AreEqual((int)AutomaticInventorySettings.AutomaticButDontDiscard, manual.InventoryManagement);
		Assert.AreEqual((int)AutomaticInventorySettings.FullyAutomatic, automatic.InventoryManagement);
		Assert.AreEqual(2, context.CharacterCombatSettings.Count(x => x.Name.StartsWith("Dual Wielder")));
	}

	[TestMethod]
	public void EnsureCombatStrategy_HumanTacticalVariants_PreserveCompatibleWeaponSetups()
	{
		using var context = BuildContext();
		context.FutureProgs.AddRange(
			CreateFutureProg(1, "AlwaysTrue"),
			CreateFutureProg(2, "IsHumanoid"));
		context.SaveChanges();

		var clincher = CombatStrategySeederHelper.EnsureCombatStrategy(context, "Dual Wield Clincher (Auto)");
		var warder = CombatStrategySeederHelper.EnsureCombatStrategy(context, "Polearm Warder (Auto)");
		var spearWarder = CombatStrategySeederHelper.EnsureCombatStrategy(context, "Spear Warder (Auto)");

		Assert.AreEqual((int)AttackHandednessOptions.DualWieldOnly, clincher.PreferredWeaponSetup);
		Assert.AreEqual((int)CombatStrategyMode.Clinch, clincher.PreferredMeleeMode);
		Assert.AreEqual((int)AttackHandednessOptions.TwoHandedOnly, warder.PreferredWeaponSetup);
		Assert.AreEqual((int)CombatStrategyMode.Ward, warder.PreferredMeleeMode);
		Assert.AreEqual((int)AttackHandednessOptions.SwordAndBoardOnly, spearWarder.PreferredWeaponSetup);
		Assert.AreEqual((int)CombatStrategyMode.Ward, spearWarder.PreferredMeleeMode);
		Assert.AreEqual(0.9, clincher.WeaponUsePercentage, 0.0001);
		Assert.AreEqual(0.1, clincher.AuxiliaryPercentage, 0.0001);
		Assert.AreEqual(0.85, warder.WeaponUsePercentage, 0.0001);
		Assert.AreEqual(0.15, warder.AuxiliaryPercentage, 0.0001);
		Assert.IsFalse(clincher.PreferShieldUse);
		Assert.IsFalse(warder.PreferShieldUse);
		Assert.IsTrue(spearWarder.PreferShieldUse);
		Assert.IsFalse(spearWarder.PreferNonContactClinchBreaking);
	}

	[TestMethod]
	public void EnsureCombatStrategy_AquaticVariants_DoNotPreferTerrestrialCombat()
	{
		using FuturemudDatabaseContext context = BuildContext();
		context.FutureProgs.AddRange(
			CreateFutureProg(1, "AlwaysTrue"),
			CreateFutureProg(2, "IsHumanoid"));
		context.SaveChanges();

		foreach (var strategy in new[]
		{
			"Beast Aquatic Brawler", "Beast Aquatic Clincher", "Beast Aquatic Behemoth",
			"Beast Aquatic Skirmisher", "Beast Aquatic Artillery"
		})
		{
			var setting = CombatStrategySeederHelper.EnsureCombatStrategy(context, strategy);
			Assert.IsFalse(setting.PreferTerrestrialCombat, strategy);
		}
	}

	[TestMethod]
	public void EnsureCombatStrategy_MountedVariants_UseMountedModesAtAllRanges()
	{
		using var context = BuildContext();
		context.FutureProgs.AddRange(
			CreateFutureProg(1, "AlwaysTrue"),
			CreateFutureProg(2, "IsHumanoid"));
		context.SaveChanges();

		foreach (var (name, mode) in new[]
		         {
			         ("Cavalry Charge", CombatStrategyMode.MountedCharge),
			         ("Mounted Skirmisher", CombatStrategyMode.MountedSkirmish),
			         ("Mounted Hit and Run", CombatStrategyMode.MountedHitAndRun)
		         })
		{
			var setting = CombatStrategySeederHelper.EnsureCombatStrategy(context, name);
			Assert.AreEqual((int)mode, setting.PreferredMeleeMode, name);
			Assert.AreEqual((int)mode, setting.PreferredRangedMode, name);
		}
	}

    [TestMethod]
    public void SeederSources_DependentSeeders_EnsureStrategiesByNameBeforeApplyingRaceDefaults()
    {
		string animalSource = SeederSourceTestHelper.ReadPartialFamily("AnimalSeeder");
        string mythicalSource = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "MythicalAnimalSeeder.cs"));
        string robotSource = File.ReadAllText(GetSourcePath("DatabaseSeeder", "Seeders", "RobotSeeder.Races.cs"));

        StringAssert.Contains(animalSource, "CombatStrategySeederHelper.EnsureCombatStrategy(_context, template.CombatStrategyKey)");
        StringAssert.Contains(mythicalSource, "CombatStrategySeederHelper.EnsureCombatStrategy(_context, template.CombatStrategyKey)");
        StringAssert.Contains(robotSource, "CombatStrategySeederHelper.EnsureCombatStrategy(_context, CombatStrategyFor(template))");
    }

    [TestMethod]
    public void CombatSeederSource_RerunsEnsureCanonicalStrategiesWithoutEmptyTableGuard()
    {
		string source = SeederSourceTestHelper.ReadPartialFamily("CombatSeeder");

        StringAssert.Contains(source, "SeedCombatStrategies(context, questionAnswers);");
		StringAssert.Contains(source, "CombatStrategySeederHelper.EnsureCombatStrategy(context, \"Dual Wielder\");");
		StringAssert.Contains(source, "CombatStrategySeederHelper.EnsureCombatStrategy(context, \"Dual Wielder (Auto)\");");
		StringAssert.Contains(source, "CombatStrategySeederHelper.EnsureCombatStrategy(context, \"Dual Wield Clincher (Auto)\");");
		StringAssert.Contains(source, "CombatStrategySeederHelper.EnsureCombatStrategy(context, \"Polearm Warder (Auto)\");");
		StringAssert.Contains(source, "CombatStrategySeederHelper.EnsureCombatStrategy(context, \"Spear Warder (Auto)\");");
        StringAssert.Contains(source, "CombatStrategySeederHelper.EnsureCombatStrategy(context, \"Beast Brawler\");");
        StringAssert.Contains(source, "CombatStrategySeederHelper.EnsureCombatStrategy(context, \"Beast Drowner\");");
		StringAssert.Contains(source, "CombatStrategySeederHelper.EnsureCombatStrategy(context, \"Beast Aquatic Brawler\");");
        StringAssert.Contains(source, "CombatStrategySeederHelper.EnsureCombatStrategy(context, \"Beast Dropper\");");
        StringAssert.Contains(source, "CombatStrategySeederHelper.EnsureCombatStrategy(context, \"Beast Physical Avoider\");");
		StringAssert.Contains(source, "EnsureMountedCombatMessages(context);");
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

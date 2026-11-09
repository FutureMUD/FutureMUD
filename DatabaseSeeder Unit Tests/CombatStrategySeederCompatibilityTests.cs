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
}

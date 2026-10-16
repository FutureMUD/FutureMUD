#nullable enable

using System;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CoreDataSeederStaticSettingsTests
{
	private static FuturemudDatabaseContext Context()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		var context = new FuturemudDatabaseContext(options);
		foreach (var name in new[] { "Deaths", "Petitions", "Typos" })
		{
			context.Boards.Add(new Board { Name = name });
		}

		foreach (var name in new[] { "Bites", "Residue" })
		{
			context.StackDecorators.Add(new StackDecorator
			{
				Name = name, Type = "Test", Definition = "", Description = name
			});
		}

		return context;
	}

	[DataTestMethod]
	[DataRow(false, false)]
	[DataRow(true, false)]
	[DataRow(true, true)]
	public void SeedStaticStringsAndSettings_InstallsMissingDefaultsAndPreservesExistingValues(
		bool prepopulateSettings, bool clearTracking)
	{
		using var context = Context();
		if (prepopulateSettings)
		{
			// These keys are inserted by AddEconomyAnalytics and shipped in the blank snapshot.
			// Deliberately use non-default values to also protect existing configuration.
			context.StaticConfigurations.AddRange(
				new StaticConfiguration { SettingName = "EconomyAnalyticsSnapshotsEnabled", Definition = "false" },
				new StaticConfiguration { SettingName = "EconomyAnalyticsSnapshotIntervalMinutes", Definition = "720" },
				new StaticConfiguration { SettingName = "EconomyAnalyticsRolloverSnapshotsEnabled", Definition = "false" });
		}

		context.SaveChanges();
		if (clearTracking)
		{
			context.ChangeTracker.Clear();
		}

		CoreDataSeeder.SeedStaticStringsAndSettings(context, "Test MUD",
			new FutureProg { Id = 41 }, new FutureProg { Id = 42 }, new ItemGroup { Id = 43 });
		context.SaveChanges();
		context.ChangeTracker.Clear();

		var settings = context.StaticConfigurations.ToDictionary(x => x.SettingName, x => x.Definition);
		foreach (var key in DefaultStaticSettings.DefaultStaticConfigurations.Keys
			.Where(x => x != DualWieldCoordinationSeederHelper.EnabledSetting))
		{
			Assert.IsTrue(settings.ContainsKey(key), $"Missing mandatory setting {key}.");
		}
		Assert.IsFalse(settings.ContainsKey(DualWieldCoordinationSeederHelper.EnabledSetting),
			"Core defaults must defer the initial Dual Wield opt-in to its bound combat installer.");

		Assert.AreEqual(prepopulateSettings ? "false" : "true", settings["EconomyAnalyticsSnapshotsEnabled"]);
		Assert.AreEqual(prepopulateSettings ? "720" : "1440", settings["EconomyAnalyticsSnapshotIntervalMinutes"]);
		Assert.AreEqual(prepopulateSettings ? "false" : "true", settings["EconomyAnalyticsRolloverSnapshotsEnabled"]);
		Assert.AreEqual("41", settings["PlayersCanCreateClansProg"]);
		Assert.AreEqual("42", settings["OnCreateClanProg"]);
		Assert.AreEqual("43", settings["TooManyItemsGameItemGroup"]);
		Assert.AreEqual(context.Boards.Single(x => x.Name == "Deaths").Id.ToString(), settings["DeathsBoardId"]);
		Assert.AreEqual("Test MUD", context.StaticStrings.Single(x => x.Id == "MudName").Text);
	}

	[DataTestMethod]
	[DataRow("", false)]
	[DataRow("false", false)]
	[DataRow("false", true)]
	[DataRow("true", true)]
	public void CoreDefaultsThenFreshCombatBinding_EnableOnlyAfterBindingAndPreserveExplicitSettings(
		string existingEnable, bool clearTracking)
	{
		using var context = Context();
		var skill = new TraitDefinition
		{
			Name = "Native Dual Wield", Type = 0, OwnerScope = 1, Alias = "dualwield",
			TraitGroup = "Combat", ChargenBlurb = "", ValueExpression = ""
		};
		context.TraitDefinitions.Add(skill);
		if (existingEnable.Length > 0)
		{
			context.StaticConfigurations.Add(new StaticConfiguration
			{
				SettingName = DualWieldCoordinationSeederHelper.EnabledSetting, Definition = existingEnable
			});
		}

		context.SaveChanges();
		if (clearTracking)
		{
			context.ChangeTracker.Clear();
		}

		CoreDataSeeder.SeedStaticStringsAndSettings(context, "Test MUD",
			new FutureProg { Id = 41 }, new FutureProg { Id = 42 }, new ItemGroup { Id = 43 });
		context.SaveChanges();
		Assert.AreEqual("false", DefaultStaticSettings.DefaultStaticConfigurations[DualWieldCoordinationSeederHelper.EnabledSetting]);
		var initialSetting = context.StaticConfigurations.SingleOrDefault(x =>
			x.SettingName == DualWieldCoordinationSeederHelper.EnabledSetting);
		Assert.AreEqual(existingEnable.Length > 0 ? existingEnable : null, initialSetting?.Definition);

		var result = DualWieldCoordinationSeederHelper.InstallFresh(context, skill);

		Assert.IsTrue(result.Success, result.Message);
		context.ChangeTracker.Clear();
		Assert.AreEqual(existingEnable.Length > 0 ? existingEnable : "true", context.StaticConfigurations.Single(x =>
			x.SettingName == DualWieldCoordinationSeederHelper.EnabledSetting).Definition);
		var check = context.Checks.Include(x => x.TraitExpression).ThenInclude(x => x.TraitExpressionParameters)
			.Single(x => x.Type == (int)MudSharp.RPG.Checks.CheckType.DualWieldCoordinationCheck);
		Assert.AreEqual(skill.Id, check.TraitExpression.TraitExpressionParameters.Single().TraitDefinitionId);
	}
}

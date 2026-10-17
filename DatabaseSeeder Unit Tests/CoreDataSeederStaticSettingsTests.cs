#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CoreDataSeederStaticSettingsTests
{
	[DataTestMethod]
	[DataRow(null)]
	[DataRow("false")]
	[DataRow("true")]
	public void SeedData_ExistingWorldRerun_PreservesBlindFightingEnableSetting(string? definition)
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
		context.SeederChoices.Add(new SeederChoice
		{
			Seeder = "Core", Version = "test", Choice = "gamename", Answer = "Existing MUD", DateTime = DateTime.UtcNow
		});
		// An installed world includes the bootstrap Void terrain, used by catalogue reconciliation.
		context.Terrains.Add(new Terrain
		{
			Id = 1, Name = "Void", DefaultTerrain = true,
			TerrainANSIColour = "7", TerrainEditorColour = "#FFFFFFFF", TerrainBehaviourMode = "outdoors",
			AtmosphereType = "Gas", TerrainEditorText = "Vo", TagInformation = string.Empty
		});
		context.CharacteristicValues.Add(new CharacteristicValue
		{
			Id = 1, Name = "prior", Value = "prior",
			Definition = new CharacteristicDefinition
			{
				Id = 9000, Name = "Prior stock characteristic", Pattern = "^prior$", Model = "standard", Description = "Prior stock"
			}
		});
		if (definition is not null)
		{
			context.StaticConfigurations.Add(new StaticConfiguration
			{
				SettingName = "BlindFightingEnabled", Definition = definition
			});
		}
		context.SaveChanges();
		context.ChangeTracker.Clear();

		new CoreDataSeeder().SeedData(context, new Dictionary<string, string>());

		var setting = context.StaticConfigurations.SingleOrDefault(x => x.SettingName == "BlindFightingEnabled");
		Assert.AreEqual(definition, setting?.Definition);
		Assert.AreEqual("false", DefaultStaticSettings.DefaultStaticConfigurations["BlindFightingEnabled"],
			"An existing world with no stored setting must continue to use the disabled runtime default.");
	}

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
	[DataRow(false, false, null)]
	[DataRow(true, false, "false")]
	[DataRow(true, true, "false")]
	[DataRow(true, false, "true")]
	[DataRow(true, true, "true")]
	public void SeedStaticStringsAndSettings_InstallsMissingDefaultsAndPreservesExistingValues(
		bool prepopulateSettings, bool clearTracking, string? blindFightingEnabled)
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

		if (blindFightingEnabled is not null)
		{
			context.StaticConfigurations.Add(new StaticConfiguration
			{
				SettingName = "BlindFightingEnabled", Definition = blindFightingEnabled
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
		Assert.AreEqual(blindFightingEnabled ?? "true", settings["BlindFightingEnabled"]);
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

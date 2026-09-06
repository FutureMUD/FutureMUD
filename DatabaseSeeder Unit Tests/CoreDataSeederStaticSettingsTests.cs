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
	[DataTestMethod]
	[DataRow(false, false)]
	[DataRow(true, false)]
	[DataRow(true, true)]
	public void SeedStaticStringsAndSettings_InstallsMissingDefaultsAndPreservesExistingValues(
		bool prepopulateSettings, bool clearTracking)
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		using var context = new FuturemudDatabaseContext(options);
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
		foreach (var key in DefaultStaticSettings.DefaultStaticConfigurations.Keys)
		{
			Assert.IsTrue(settings.ContainsKey(key), $"Missing mandatory setting {key}.");
		}

		Assert.AreEqual(prepopulateSettings ? "false" : "true", settings["EconomyAnalyticsSnapshotsEnabled"]);
		Assert.AreEqual(prepopulateSettings ? "720" : "1440", settings["EconomyAnalyticsSnapshotIntervalMinutes"]);
		Assert.AreEqual(prepopulateSettings ? "false" : "true", settings["EconomyAnalyticsRolloverSnapshotsEnabled"]);
		Assert.AreEqual("41", settings["PlayersCanCreateClansProg"]);
		Assert.AreEqual("42", settings["OnCreateClanProg"]);
		Assert.AreEqual("43", settings["TooManyItemsGameItemGroup"]);
		Assert.AreEqual(context.Boards.Single(x => x.Name == "Deaths").Id.ToString(), settings["DeathsBoardId"]);
		Assert.AreEqual("Test MUD", context.StaticStrings.Single(x => x.Id == "MudName").Text);
	}
}

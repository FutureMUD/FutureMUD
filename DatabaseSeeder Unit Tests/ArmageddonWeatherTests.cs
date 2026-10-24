#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Climate;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeederUnitTests;

[TestClass]
public class ArmageddonWeatherTests
{
	[TestMethod]
	public void MissingPrerequisites_DefaultNo_ExplicitYesFailsBeforeSettingWrites()
	{
		using var db = WeatherTimingTests.Context();
		Assert.AreEqual("no", ArmageddonWeatherInstaller.DefaultAnswer(db, new Dictionary<string, string>()));
		Assert.IsFalse(ArmageddonWeatherInstaller.ValidateAnswer("yes", db).Success);
		Assert.IsTrue(ArmageddonWeatherInstaller.ValidateAnswer("no", db).Success);
		Assert.ThrowsExactly<InvalidOperationException>(() => new ArmageddonSettingSeeder().SeedData(db,
			new Dictionary<string, string> { [ArmageddonSettingSeeder.FullInstallQuestion] = "yes", [ArmageddonSettingSeeder.WeatherQuestion] = "yes" }));
		Assert.IsFalse(db.ChangeTracker.HasChanges());
		Assert.AreEqual(0, db.SeederManagedRecords.Count());
	}

	[TestMethod]
	public void Install_NineBlocksHeatingConnectedDryGraphs_AndPreservingPartialRecovery()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>().UseInMemoryDatabase(
			$"arm-weather-{Guid.NewGuid():N}", x => x.EnableNullChecks(false)).ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options;
		FuturemudDatabaseContext Factory() => new(options);
		using (var db = Factory())
		{
			new TimeSeeder().SeedData(db, new Dictionary<string, string> { ["mode"] = "Armageddon", ["secondsmultiplier"] = "6", ["startyear"] = "1" });
			new CelestialSeeder().SeedData(db, new Dictionary<string, string> { ["installarmageddon"] = "yes" });
			Assert.AreEqual("no", ArmageddonWeatherInstaller.DefaultAnswer(db, new Dictionary<string, string>()));
			db.Gases.Add(new Gas { Name = "air" }); db.SaveChanges();
			Assert.AreEqual("", new WeatherSeeder().SeedData(db, new Dictionary<string, string> { ["rain"] = "none" }));
			Assert.AreEqual("yes", ArmageddonWeatherInstaller.DefaultAnswer(db, new Dictionary<string, string>()));
		}
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonWeatherInstaller.Install(Factory).Status);
		long seasonId, modelId, controllerId;
		string editedTable;
		using (var db = Factory())
		{
			var seasons = db.Seasons.Where(x => x.Name.StartsWith("armageddon_")).OrderBy(x => x.CelestialDayOnset).ToArray();
			Assert.AreEqual(9, seasons.Length);
			CollectionAssert.AreEqual(Enumerable.Range(0, 9).Select(i => i * 77).ToArray(), seasons.Select(x => x.CelestialDayOnset).ToArray());
			CollectionAssert.AreEqual(Enumerable.Range(0, 9).Select(ArmageddonWeatherInstaller.SeasonDisplay).ToArray(), seasons.Select(x => x.DisplayName).ToArray());
			Assert.AreEqual(3, XElement.Parse(db.Calendars.Single().Definition).Element("months")!.Elements().Count());
			var events = db.WeatherEvents.ToDictionary(x => x.Id);
			foreach (var profile in ArmageddonWeatherInstaller.Profiles)
			{
				var regional = db.RegionalClimates.Include(x => x.RegionalClimatesSeasons).Single(x => x.Name == profile.Name);
				Assert.AreEqual(2160, regional.TemperatureFluctuationPeriodMinutes);
				Assert.AreEqual(profile.StandardDeviation, regional.TemperatureFluctuationStandardDeviation);
				var temperatures = regional.RegionalClimatesSeasons.ToDictionary(x => x.SeasonId);
				for (var i = 0; i < seasons.Length; i++)
				{
					var min = profile.CoolMin + (profile.WarmMin - profile.CoolMin) * ArmageddonWeatherInstaller.Warmth[i];
					var max = profile.CoolMax + (profile.WarmMax - profile.CoolMax) * ArmageddonWeatherInstaller.Warmth[i];
					var values = XElement.Parse(temperatures[seasons[i].Id].TemperatureInfo).Elements().Select(x => (double)x).ToArray();
					Assert.AreEqual(9, values.Length);
					for (var hour = 0; hour < 9; hour++) Assert.AreEqual(min + (max - min) * ArmageddonWeatherInstaller.Heating[hour], values[hour], 1e-12);
					Assert.AreEqual(min, values[1], 1e-12); Assert.AreEqual(max, values[5], 1e-12);
				}
				Assert.AreEqual(temperatures[seasons[0].Id].TemperatureInfo, temperatures[seasons[8].Id].TemperatureInfo);
				var model = db.ClimateModels.Include(x => x.ClimateModelSeasons).ThenInclude(x => x.SeasonEvents).Single(x => x.Id == regional.ClimateModelId);
				Assert.AreEqual(9, model.ClimateModelSeasons.Count);
				foreach (var season in model.ClimateModelSeasons)
				{
					Assert.AreEqual(9, season.SeasonEvents.Count);
					var allowed = season.SeasonEvents.Select(x => x.WeatherEventId).ToHashSet();
					foreach (var row in season.SeasonEvents)
					{
						var weather = events[row.WeatherEventId];
						Assert.AreEqual((int)PrecipitationLevel.Dry, weather.Precipitation);
						Assert.IsTrue(weather.Wind <= (int)WindLevel.GaleWind);
						Assert.IsFalse(weather.Name.Contains("Lightning"));
						var targets = XElement.Parse(row.Transitions).Elements().Select(x => (long)x.Attribute("id")!).ToArray();
						Assert.AreEqual(8, targets.Length);
						Assert.IsTrue(targets.All(allowed.Contains));
						Assert.IsTrue(targets.Contains(events.Values.Single(x => x.Name == "DryStill").Id) || weather.Name == "DryStill");
					}
				}
			}
			var editedSeason = seasons[0]; seasonId = editedSeason.Id; editedSeason.DisplayName = "Builder's Season";
			var editedModel = db.ClimateModels.Single(x => x.Name == "Southern Desert"); modelId = editedModel.Id; editedModel.Name = "Builder's Desert";
			var rowToEdit = db.Set<ClimateModelSeasonEvent>().First(x => x.ClimateModelId == modelId); rowToEdit.ChangeChance = .987;
			var table = db.RegionalClimatesSeasons.First(x => x.RegionalClimate.Name == "Southern Desert");
			table.TemperatureInfo = editedTable = "<Temperatures><Value hour=\"0\">99</Value></Temperatures>";
			var deletedRow = db.Set<ClimateModelSeasonEvent>().First(x => x.ClimateModelId != modelId && x.SeasonId == seasonId);
			db.Remove(deletedRow);
			db.RegionalClimatesSeasons.Remove(db.RegionalClimatesSeasons.First(x => x.RegionalClimate.Name == "Northern Forest"));
			var controller = new MudSharp.Models.WeatherController { Name = "Builder's Controller", RegionalClimateId = table.RegionalClimateId, CurrentSeasonId = seasonId, CurrentWeatherEventId = rowToEdit.WeatherEventId };
			db.WeatherControllers.Add(controller); db.SaveChanges(); controllerId = controller.Id;
		}
		var result = ArmageddonWeatherInstaller.Install(Factory);
		Assert.IsTrue(result.Messages.Any(x => x.Contains("builder edit")));
		ArmageddonWeatherInstaller.Install(Factory);
		using (var db = Factory())
		{
			Assert.AreEqual("Builder's Season", db.Seasons.Find(seasonId)!.DisplayName);
			Assert.AreEqual("Builder's Desert", db.ClimateModels.Find(modelId)!.Name);
			Assert.IsTrue(db.Set<ClimateModelSeasonEvent>().Any(x => x.ClimateModelId == modelId && x.ChangeChance == .987));
			Assert.IsTrue(db.RegionalClimatesSeasons.Any(x => x.TemperatureInfo == editedTable));
			Assert.AreEqual(36, db.RegionalClimatesSeasons.Count(x => x.Season.Name.StartsWith("armageddon_")));
			Assert.AreEqual(324, db.Set<ClimateModelSeasonEvent>().Count(x => x.Season.Name.StartsWith("armageddon_")));
			Assert.AreEqual(seasonId, db.WeatherControllers.Find(controllerId)!.CurrentSeasonId);
			Assert.AreEqual(1, db.WeatherControllers.Count());
            var graphClaim = db.SeederManagedRecords.Single(x => x.StableKey == "arm.weather.graph.southern_desert.0");
            graphClaim.Retired = true; db.SaveChanges();
            Assert.IsFalse(ArmageddonWeatherInstaller.ValidateAnswer("yes", db).Success);
            var before = db.SeederManagedRecords.Count();
            Assert.ThrowsExactly<InvalidOperationException>(() => new ArmageddonSettingSeeder().SeedData(db,
                new Dictionary<string, string> { [ArmageddonSettingSeeder.FullInstallQuestion] = "yes", [ArmageddonSettingSeeder.WeatherQuestion] = "yes" }));
            Assert.AreEqual(before, db.SeederManagedRecords.Count());
            Assert.IsFalse(db.ChangeTracker.HasChanges());
            graphClaim.Retired = false; db.SaveChanges();

            var principalClaim = db.SeederManagedRecords.Single(x => x.StableKey == "arm.weather.model.southern_desert");
            db.SeederManagedRecords.Remove(principalClaim); db.SaveChanges();
            Assert.IsFalse(ArmageddonWeatherInstaller.ValidateAnswer("yes", db).Success, "Dependent graph claims require their principal identity even after a builder rename.");
            Assert.ThrowsExactly<InvalidOperationException>(() => ArmageddonWeatherInstaller.Install(Factory));
            db.SeederManagedRecords.Add(principalClaim); db.SaveChanges();

			db.ClimateModels.Add(new ClimateModel { Name = "Salt Flats" }); db.SaveChanges();
			var count = db.SeederManagedRecords.Count();
			Assert.IsFalse(ArmageddonWeatherInstaller.ValidateAnswer("yes", db).Success);
			Assert.ThrowsExactly<InvalidOperationException>(() => ArmageddonWeatherInstaller.Install(Factory));
			Assert.AreEqual(count, db.SeederManagedRecords.Count());
		}
	}
}

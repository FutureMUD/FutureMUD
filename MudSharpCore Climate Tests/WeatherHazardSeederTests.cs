#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Climate;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Climate_Tests;

[TestClass]
public class WeatherHazardSeederTests
{
	[TestMethod]
	public void HazardUpdate_AdoptsVerifiedLegacyGraphs_PreservesCustomGraphAndReruns()
	{
		using var context = Context();
		// Deliberately offset identities from the reference database.
		context.WeatherEvents.Add(new WeatherEvent { Name = "Builder's unrelated weather", AdditionalInfo = "<Event />" });
		context.SaveChanges();
		var seeder = new WeatherSeeder();
		Assert.AreEqual("", seeder.SeedBaseData(context, new Dictionary<string, string> { ["rain"] = "full" }));
		var custom = context.ClimateModels.Include(x => x.ClimateModelSeasons).ThenInclude(x => x.SeasonEvents).First();
		custom.Description = "Builder-authored climate policy";
		var customRows = custom.ClimateModelSeasons.SelectMany(x => x.SeasonEvents).Select(x => x.Transitions).ToArray();
		context.SaveChanges();
		Assert.AreEqual("", seeder.SeedData(context, new Dictionary<string, string> { ["operation"] = "hazards" }));
		Assert.AreEqual("Builder-authored climate policy", custom.Description);
		CollectionAssert.AreEqual(customRows, custom.ClimateModelSeasons.SelectMany(x => x.SeasonEvents).Select(x => x.Transitions).ToArray());
		Assert.IsTrue(context.SeederManagedRecords.Any(x => x.Module == "weather-hazards-v1" && x.EntityType == "ClimateModel"));
		var rain = context.WeatherEvents.First(x => x.Name.StartsWith("WeatherHazard_Lightning_"));
		Assert.AreEqual("rain", rain.WeatherEventType);
		Assert.IsNotNull(XElement.Parse(rain.AdditionalInfo).Element("Liquid"));
		var count = context.WeatherEvents.Count();
		var managed = context.ClimateModels.Include(x => x.ClimateModelSeasons).ThenInclude(x => x.SeasonEvents).First(x => x.Id != custom.Id);
		var edited = managed.ClimateModelSeasons.First().SeasonEvents.First();
		edited.ChangeChance = 0.12345;
		rain.WeatherDescription = "Builder-authored lightning";
		context.SaveChanges();
		Assert.AreEqual("", seeder.SeedData(context, new Dictionary<string, string> { ["operation"] = "hazards" }));
		Assert.AreEqual(count, context.WeatherEvents.Count());
		Assert.AreEqual(0.12345, edited.ChangeChance);
		Assert.AreEqual("Builder-authored lightning", rain.WeatherDescription);
		Assert.AreEqual(1, context.Gases.Count(x => x.Name == "dusty air"));
		Assert.AreEqual(1, context.Gases.Count(x => x.Name == "choking dust"));
		Assert.AreEqual(context.Gases.Single(x => x.Name == "air").Id, context.Gases.Single(x => x.Name == "dusty air").CountAsId);
		Assert.IsNull(context.Gases.Single(x => x.Name == "choking dust").CountAsId);
		Assert.IsNotNull(context.Gases.Single(x => x.Name == "dusty air").DrugId);
		var removed = context.Gases.Single(x => x.Name == "dusty air");
		var originalId = removed.Id;
		context.Gases.Remove(removed);
		context.SaveChanges();
		Assert.AreEqual("", seeder.SeedData(context, new Dictionary<string, string> { ["operation"] = "hazards" }));
		Assert.AreEqual(originalId, context.Gases.Single(x => x.Name == "dusty air").Id, "Restoring owned stock must preserve XML references to its gas ID.");
		context.Gases.Single(x => x.Id == originalId).Name = "Builder-renamed dusty air";
		context.SaveChanges();
		Assert.AreEqual("", seeder.SeedData(context, new Dictionary<string, string> { ["operation"] = "hazards" }));
		Assert.AreEqual("Builder-renamed dusty air", context.Gases.Single(x => x.Id == originalId).Name);
		Assert.AreEqual(0, context.Gases.Count(x => x.Name == "dusty air"));
	}

	[DataTestMethod]
	[DataRow("air")]
	[DataRow("Breathable Atmosphere")]
	public void FreshInstall_HazardsHaveValidReferencesAndAppropriateClimateTransitions(string gasName)
	{
		using var context = Context(gasName);
		Assert.AreEqual("", new WeatherSeeder().SeedData(context, new Dictionary<string, string> { ["operation"] = "install", ["rain"] = "full" }));
		var events = context.WeatherEvents.ToDictionary(x => x.Id);
		var dust = events.Values.Where(x => x.Name.StartsWith("WeatherHazard_Dust_") || x.Name.StartsWith("WeatherHazard_ChokingDust_")).Select(x => x.Id).ToHashSet();
		Assert.IsTrue(dust.Count > 0);
		var lightning = events.Values.Where(x => x.Name.StartsWith("WeatherHazard_Lightning_")).ToArray();
		Assert.IsTrue(lightning.Length > 0);
		Assert.IsTrue(lightning.All(x => WeatherHazardSettings.FromXml(XElement.Parse(x.AdditionalInfo).Element("Hazards")).GroundDamageFactor > 0));
		var desertDust = false;
		foreach (var climate in context.ClimateModels.Include(x => x.ClimateModelSeasons).ThenInclude(x => x.SeasonEvents))
		foreach (var row in climate.ClimateModelSeasons.SelectMany(x => x.SeasonEvents))
		{
			Assert.IsTrue(events.ContainsKey(row.WeatherEventId));
			foreach (var transition in XElement.Parse(row.Transitions).Elements())
			{
				var id = (long)transition.Attribute("id")!;
				Assert.IsTrue(events.ContainsKey(id));
				Assert.IsTrue((double)transition.Attribute("chance")! >= 0);
				if (dust.Contains(id)) { Assert.IsTrue(climate.Description.Contains("BWh") || climate.Description.Contains("BWk") || climate.Description.Contains("BSh") || climate.Description.Contains("BSk")); desertDust = true; }
				if (climate.Description.Contains("classification: EF") || climate.Description.Contains("classification: ET"))
					Assert.IsFalse(events[id].Name.StartsWith("WeatherHazard_"));
			}
		}
		Assert.IsTrue(desertDust);
	}

	[TestMethod]
	public void MissingAir_InstallThenRepair_AddsDustWithoutDuplicatingManagedLightning()
	{
		using var context = Context("Custom Unbreathable Gas");
		var seeder = new WeatherSeeder();
		Assert.AreEqual("", seeder.SeedData(context, new Dictionary<string, string> { ["operation"] = "install", ["rain"] = "full" }));
		Assert.IsFalse(context.WeatherEvents.Any(x => x.Name.StartsWith("WeatherHazard_Dust_")));
		var lightningCount = context.ClimateModels.SelectMany(x => x.ClimateModelSeasons).SelectMany(x => x.SeasonEvents)
			.Count(x => x.WeatherEvent.Name.StartsWith("WeatherHazard_Lightning_"));
		context.Gases.Single().Name = "Breathable Atmosphere";
		context.SaveChanges();
		Assert.AreEqual("", seeder.SeedData(context, new Dictionary<string, string> { ["operation"] = "hazards" }));
		Assert.IsTrue(context.ClimateModels.SelectMany(x => x.ClimateModelSeasons).SelectMany(x => x.SeasonEvents)
			.Any(x => x.WeatherEvent.Name.StartsWith("WeatherHazard_Dust_")));
		Assert.AreEqual(lightningCount, context.ClimateModels.SelectMany(x => x.ClimateModelSeasons).SelectMany(x => x.SeasonEvents)
			.Count(x => x.WeatherEvent.Name.StartsWith("WeatherHazard_Lightning_")));
		Assert.AreEqual("", seeder.SeedData(context, new Dictionary<string, string> { ["operation"] = "hazards" }));
		Assert.IsTrue(context.ClimateModels.SelectMany(x => x.ClimateModelSeasons).ToArray()
			.All(x => x.SeasonEvents.Select(e => e.WeatherEventId).Distinct().Count() == x.SeasonEvents.Count));
	}

	private static FuturemudDatabaseContext Context(string gasName = "air")
	{
		var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		context.Celestials.Add(new Celestial { Definition = "<Celestial />", CelestialType = "test", FeedClockId = 1 });
		context.Liquids.Add(new Liquid { Name = "rain water" });
		context.Gases.Add(new Gas { Name = gasName, Description = "air", DisplayColour = "white", SmellText = "", VagueSmellText = "" });
		context.SaveChanges();
		return context;
	}
}

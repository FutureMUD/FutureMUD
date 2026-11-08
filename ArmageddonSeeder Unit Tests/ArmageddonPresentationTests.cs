#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonPresentationTests
{
	[TestMethod]
	public void FoundationRefresh_UsesAdminIdentityAndDoesNotOverwritePlayableHuman()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		ArmageddonHeritage.Install(db, new List<string>());
		var playable = db.Races.Single(x => x.Name == "Human");
		var foundation = db.Races.Single(x => x.Id == 101);
		Assert.AreEqual("Admin Human", foundation.Name);
		Assert.AreEqual(2L, foundation.AvailabilityProgId);
		Assert.AreEqual(0.8, foundation.SweatRateInLitresPerMinute);
		var food = playable.MaximumFoodSatiatedHours;
		var drink = playable.MaximumDrinkSatiatedHours;
		foundation.MaximumFoodSatiatedHours = 999;
		foundation.Name = "Builder Admin";
		foundation.AvailabilityProgId = 43;
		db.SaveChanges();
		Assert.IsFalse(HumanSeeder.HumanFoundationRaces(db).Any(x => x.Id == playable.Id));
		Assert.IsTrue(HumanSeeder.RefreshHumanSatiationLimits(db));
		ArmageddonHeritage.Install(db, new List<string>()); db.ChangeTracker.Clear();
		Assert.AreEqual(food, db.Races.Single(x => x.Id == playable.Id).MaximumFoodSatiatedHours);
		Assert.AreEqual(drink, db.Races.Single(x => x.Id == playable.Id).MaximumDrinkSatiatedHours);
		Assert.AreEqual(HumanSeeder.HumanBaselineSatiationLimitsForTesting.MaximumFoodSatiatedHours,
			db.Races.Single(x => x.Id == foundation.Id).MaximumFoodSatiatedHours);
		Assert.AreEqual("Builder Admin", db.Races.Single(x => x.Id == foundation.Id).Name);
		Assert.AreEqual(43L, db.Races.Single(x => x.Id == foundation.Id).AvailabilityProgId);
	}

	[TestMethod]
	public void HeritageRerun_PreservesBuilderRenameWhenAnotherRaceUsesStockName()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		ArmageddonHeritage.Install(db, new List<string>());
		var dwarf = db.Races.Single(x => x.Name == "Dwarf");
		dwarf.Name = "Builder Dwarf";
		var other = new Race { Name = "Dwarf" };
		db.Races.Add(other); db.SaveChanges(); db.ChangeTracker.Clear();
		ArmageddonHeritage.Install(db, new List<string>()); db.ChangeTracker.Clear();
		Assert.AreEqual("Builder Dwarf", db.Races.Single(x => x.Id == dwarf.Id).Name);
		Assert.AreEqual(other.Id, db.Races.Single(x => x.Name == "Dwarf").Id);
	}

	private static void PreviousStock(MudSharp.Database.FuturemudDatabaseContext db, string key, object row,
		params (string Field, object Value)[] fields)
	{
		var record = db.SeederManagedRecords.Single(x => x.StableKey == key);
		var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
		foreach (var (field, value) in fields)
		{
			db.Entry(row).Property(field).CurrentValue = value;
			baseline[field] = JsonSerializer.Serialize(value);
		}
		record.SeedBaseline = JsonSerializer.Serialize(baseline);
		record.AppliedFingerprint = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(record.SeedBaseline))).ToLowerInvariant();
	}

	[TestMethod]
	public void HeritageUpgrade_RenamesAndEnablesStockWithStableIdsAndPreservesBuilderOverrides()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		var messages = new List<string>();
		ArmageddonHeritage.Install(db, messages);
		var ids = db.SeederManagedRecords.ToDictionary(x => x.StableKey, x => x.LogicalId);
		// Earlier installations recorded Tatlum under Templars. Heritage must update the
		// same rows without taking over their module or discarding a custom accent.
		foreach (var key in new[] { "arm.skill.tatlum", "arm.skill.tatlum.cap", "arm.language.tatlum", "arm.accent.tatlum.standard", "arm.accent.tatlum.learner" })
			db.SeederManagedRecords.Single(x => x.StableKey == key).Module = ArmageddonTemplarInstaller.Module;
		var tatlum = db.Languages.Single(x => x.Id == ids["arm.language.tatlum"]);
		PreviousStock(db, "arm.language.tatlum", tatlum, ("UnknownLanguageDescription", "an unfamiliar language"));
		var accent = db.Accents.Single(x => x.Id == ids["arm.accent.tatlum.standard"]);
		accent.Description = "Builder pronunciation";
		foreach (var profile in ArmageddonHeritage.Profiles)
		{
			var race = db.Races.Single(x => x.Id == ids["arm.race." + profile.Key]);
			PreviousStock(db, "arm.race." + profile.Key, race, ("Name", "Armageddon " + profile.Name), ("AvailabilityProgId", (long?)2L),
				("Description", "Mechanical profile; remains unavailable."));
			var ethnicity = db.Ethnicities.Single(x => x.Id == ids["arm.ethnicity." + profile.Key + ".default"]);
			PreviousStock(db, "arm.ethnicity." + profile.Key + ".default", ethnicity, ("Name", "Armageddon " + profile.Name));
		}
		foreach (var culture in ArmageddonHeritage.Cultures)
		{
			var row = db.Cultures.Single(x => x.Id == ids["arm.culture." + culture.Key]);
			PreviousStock(db, "arm.culture." + culture.Key, row, ("Name", "Armageddon " + culture.Name));
		}
		var giant = db.Races.Single(x => x.Id == ids["arm.race.half_giant"]);
		giant.Name = "Builder Giant";
		giant.Description = "Builder lore";
		giant.AvailabilityProgId = 42; // Explicit custom admission policy must survive an upgrade.
		var nomadic = db.FutureProgs.Single(x => x.FunctionName == "ArmageddonNomadicBackgroundApproved");
		PreviousStock(db, "arm.approval.nomadic", nomadic, ("FunctionText", "return false"));
		db.SaveChanges(); db.ChangeTracker.Clear();
		for (var attempt = 0; attempt < 2; attempt++)
		{
			ArmageddonHeritage.Install(db, messages); db.ChangeTracker.Clear();
			foreach (var name in new[] { "Human", "City Elf", "Desert Elf", "Dwarf", "Mul" })
			{
				var race = db.Races.Single(x => x.Name == name);
				Assert.AreEqual(1L, race.AvailabilityProgId);
				Assert.IsFalse(race.Description.Contains("profile", StringComparison.OrdinalIgnoreCase));
				Assert.AreEqual(1L, db.Ethnicities.Single(x => x.ParentRaceId == race.Id && x.Name == name).AvailabilityProgId);
			}
			var retained = db.Races.Single(x => x.Id == giant.Id);
			Assert.AreEqual("Builder Giant", retained.Name);
			Assert.AreEqual("Builder lore", retained.Description);
			Assert.AreEqual(42L, retained.AvailabilityProgId);
			Assert.AreEqual("return true", db.FutureProgs.Single(x => x.Id == nomadic.Id).FunctionText);
			Assert.AreEqual(ArmageddonTemplarInstaller.Module, db.SeederManagedRecords.Single(x => x.StableKey == "arm.language.tatlum").Module);
			Assert.AreNotEqual("an unfamiliar language", db.Languages.Single(x => x.Id == tatlum.Id).UnknownLanguageDescription);
			Assert.AreEqual("Builder pronunciation", db.Accents.Single(x => x.Id == accent.Id).Description);
			Assert.IsTrue(db.SeederManagedRecords.All(x => ids[x.StableKey] == x.LogicalId));
			Assert.AreEqual(ids.Count, db.SeederManagedRecords.Count());
		}
	}

	[TestMethod]
	public void NamingUpgrade_RemovesExtraPromptsButRetainsPoolsIdsAndBuilderDefinitions()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		ArmageddonHeritage.Install(db, new List<string>());
		var record = db.SeederManagedRecords.Single(x => x.StableKey == "arm.naming.elf");
		var culture = db.NameCultures.Single(x => x.Id == record.LogicalId);
		var xml = XElement.Parse(culture.Definition);
		xml.Element("Elements")!.Add(new XElement("Element", new XAttribute("Usage", 15), new XAttribute("Name", "Role Title"),
			new XAttribute("MinimumCount", 0), new XAttribute("MaximumCount", 1), new XCData("Old elaborate guidance")));
		PreviousStock(db, record.StableKey, culture, ("Name", "Armageddon Elf naming tradition"), ("Definition", xml.ToString()));
		var custom = db.NameCultures.Single(x => x.Id == db.SeederManagedRecords.Single(y => y.StableKey == "arm.naming.dwarf").LogicalId);
		custom.Definition = "<NameCulture><BuilderDefinition /></NameCulture>";
		var names = db.RandomNameProfilesElements.Count();
		db.SaveChanges(); db.ChangeTracker.Clear();
		ArmageddonHeritage.Install(db, new List<string>()); db.ChangeTracker.Clear();
		var updated = db.NameCultures.Single(x => x.Id == culture.Id);
		Assert.AreEqual("Elf naming tradition", updated.Name);
		Assert.AreEqual(3, XElement.Parse(updated.Definition).Element("Elements")!.Elements().Count());
		Assert.AreEqual(custom.Definition, db.NameCultures.Single(x => x.Id == custom.Id).Definition);
		Assert.AreEqual(names, db.RandomNameProfilesElements.Count());
	}

}

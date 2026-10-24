#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonPhysiologyTests
{
	[TestMethod]
	public void IntrinsicGrants_ReloadWithApprovedMetabolismAndReservesWithoutCharacterAssignments()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		ArmageddonHeritage.Install(db, new List<string>());
		db.ChangeTracker.Clear();
		var keys = db.SeederManagedRecords.Where(x => x.StableKey.StartsWith("arm.merit."))
			.ToDictionary(x => x.StableKey, x => x.LogicalId!.Value);
		var physiology = new[] { ArmageddonHeritage.NormalEnvironmentKey, ArmageddonHeritage.DesertEnvironmentKey,
			ArmageddonHeritage.GiantMetabolismKey, ArmageddonHeritage.DesertStaminaKey, ArmageddonHeritage.ReserveStaminaKey };
		foreach (var key in physiology)
		{
			var merit = db.Merits.Single(x => x.Id == keys[key]);
			Assert.AreEqual(1, merit.MeritScope);
			Assert.AreEqual("2", XElement.Parse(merit.Definition).Element("ChargenAvailableProg")!.Value);
		}
		foreach (var profile in ArmageddonHeritage.Profiles)
		{
			var race = db.Races.Single(x => x.Name == profile.Name);
			var expected = new List<long> { keys[profile.Key == "desert_elf" ? ArmageddonHeritage.DesertEnvironmentKey : ArmageddonHeritage.NormalEnvironmentKey] };
			if (profile.Key == "desert_elf") expected.Add(keys[ArmageddonHeritage.DesertStaminaKey]);
			if (profile.Key is "half_giant" or "mul") expected.Add(keys[ArmageddonHeritage.ReserveStaminaKey]);
			if (profile.Key == "half_giant") expected.Add(keys[ArmageddonHeritage.GiantMetabolismKey]);
			CollectionAssert.AreEquivalent(expected, db.RacesMerits.Where(x => x.RaceId == race.Id).Select(x => x.MeritId).ToList());
			Assert.AreEqual(1.0, race.HungerRate);
			Assert.AreEqual(1.0, race.ThirstRate, "The racial merit supplies the multiplier exactly once.");
		}
		var parent = db.Races.Single(x => x.Name == "Elf");
		Assert.IsFalse(db.RacesMerits.Any(x => x.RaceId == parent.Id), "Desert elves must not inherit the ordinary environment merit too.");
		var giant = db.Races.Single(x => x.Name == "Half-Giant");
		Assert.AreEqual(48.0, giant.MaximumFoodSatiatedHours);
		Assert.AreEqual(24.0, giant.MaximumDrinkSatiatedHours);
		var metabolism = XElement.Parse(db.Merits.Single(x => x.Id == keys[ArmageddonHeritage.GiantMetabolismKey]).Definition);
		Assert.AreEqual("3", metabolism.Attribute("hunger")!.Value);
		Assert.AreEqual("3", metabolism.Attribute("thirst")!.Value);
		Assert.AreEqual("1", metabolism.Attribute("alcohol")!.Value);
		Assert.AreEqual("60", XElement.Parse(db.Merits.Single(x => x.Id == keys[ArmageddonHeritage.DesertStaminaKey]).Definition).Element("Bonus")!.Value);
		Assert.AreEqual("20", XElement.Parse(db.Merits.Single(x => x.Id == keys[ArmageddonHeritage.ReserveStaminaKey]).Definition).Element("Bonus")!.Value);
		Assert.AreEqual(0, db.PerceiverMerits.Count(), "Intrinsic grants belong to races, not copied assignments.");
	}

	[TestMethod]
	public void Rerun_PreservesRemovedGrantCustomDefinitionAndRenamedTerrainBindings()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		db.Terrains.Add(new Terrain { Id = 900, Name = "Sandy Desert" });
		db.SaveChanges();
		var messages = new List<string>();
		ArmageddonHeritage.Install(db, messages);
		var dwarf = db.Races.Single(x => x.Name == "Dwarf");
		db.RacesMerits.RemoveRange(db.RacesMerits.Where(x => x.RaceId == dwarf.Id));
		var reserveId = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonHeritage.ReserveStaminaKey).LogicalId!.Value;
		var reserve = db.Merits.Single(x => x.Id == reserveId);
		var custom = XElement.Parse(reserve.Definition);
		custom.Element("Bonus")!.Value = "27";
		reserve.Definition = custom.ToString();
		db.Terrains.Single(x => x.Id == 900).Name = "Builder Desert";
		db.SaveChanges();
		var identities = db.SeederManagedRecords.ToDictionary(x => x.StableKey, x => x.LogicalId);
		for (var attempt = 0; attempt < 2; attempt++)
		{
			db.ChangeTracker.Clear();
			ArmageddonHeritage.Install(db, messages);
			db.ChangeTracker.Clear();
			Assert.IsFalse(db.RacesMerits.Any(x => x.RaceId == dwarf.Id));
			Assert.AreEqual("27", XElement.Parse(db.Merits.Single(x => x.Id == reserveId).Definition).Element("Bonus")!.Value);
			StringAssert.Contains(db.FutureProgs.Single(x => x.FunctionName == "ArmageddonDesertElfEnvironmentalThirst").FunctionText, "ToTerrain(900)");
			Assert.IsTrue(db.SeederManagedRecords.All(x => identities[x.StableKey] == x.LogicalId));
		}
	}
}

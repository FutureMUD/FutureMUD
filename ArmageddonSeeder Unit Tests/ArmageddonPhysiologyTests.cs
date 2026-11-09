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

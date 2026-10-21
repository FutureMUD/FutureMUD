#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonAttributeAdaptationTests
{
	[TestMethod]
	public void AttributeSelections_ProjectRawTraitsWithoutDuplicatingMentalOrDerivedBonuses()
	{
		Assert.AreEqual(-5, ArmageddonHeritage.AttributeBonusFor("half_giant", ArmageddonHeritage.AttributeSemantic("Wisdom", true)));
		Assert.AreEqual(0, ArmageddonHeritage.AttributeBonusFor("half_giant", ArmageddonHeritage.AttributeSemantic("Intelligence", true)), "DND must not receive both Wisdom and Intelligence penalties.");
		Assert.AreEqual(-5, ArmageddonHeritage.AttributeBonusFor("half_giant", ArmageddonHeritage.AttributeSemantic("Intelligence", false)));
		Assert.AreEqual(8, ArmageddonHeritage.AttributeBonusFor("half_giant", ArmageddonHeritage.AttributeSemantic("Physique", false)), "Use the existing combined-physical rounded-average convention.");
		foreach (var name in new[] { "Upper Body Strength", "Lower Body Strength" })
			Assert.AreEqual(10, ArmageddonHeritage.AttributeBonusFor("half_giant", ArmageddonHeritage.AttributeSemantic(name, true)));
		foreach (var name in new[] { "Hardiness", "Resilience", "Constitution", "Endurance" })
			Assert.AreEqual(5, ArmageddonHeritage.AttributeBonusFor("half_giant", ArmageddonHeritage.AttributeSemantic(name, true)));
		foreach (var name in new[] { "Dexterity", "Speed", "Agility" })
			Assert.AreEqual(-4, ArmageddonHeritage.AttributeBonusFor("half_giant", ArmageddonHeritage.AttributeSemantic(name, true)));
		Assert.AreEqual(0, ArmageddonHeritage.AttributeBonusFor("half_giant", ArmageddonHeritage.AttributeSemantic("Builder's Custom Trait", false)));
	}

	[TestMethod]
	public void ArmAttributeInstall_UsesFourExistingAttributesAndTheApprovedRawBudget()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		var endurance = db.TraitDefinitions.Single(x => x.Name == "Constitution");
		endurance.Name = "Endurance";
		var wisdom = db.TraitDefinitions.Single(x => x.Name == "Intelligence");
		wisdom.Name = "Wisdom";
		foreach (var extra in db.TraitDefinitions.Where(x => x.Type == 1 && (x.Name == "Dexterity" || x.Name == "Perception" || x.Name == "Willpower")))
			extra.Type = 3;
		db.SaveChanges();
		var originalIds = db.TraitDefinitions.Select(x => x.Id).ToHashSet();
		ArmageddonHeritage.Install(db, new List<string>());
		var giant = db.Races.Single(x => x.Name == "Armageddon Half-Giant");
		var links = db.RacesAttributes.Where(x => x.RaceId == giant.Id).ToArray();
		Assert.AreEqual(48, giant.AttributeTotalCap);
		Assert.AreEqual(4, links.Length);
		Assert.AreEqual(5.0, links.Single(x => x.AttributeId == endurance.Id).AttributeBonus);
		Assert.AreEqual(-5.0, links.Single(x => x.AttributeId == wisdom.Id).AttributeBonus);
		Assert.IsTrue(originalIds.SetEquals(db.TraitDefinitions.Select(x => x.Id)), "The package must use installed attributes rather than create duplicates.");
	}

	[TestMethod]
	public void AttributeAdoption_PreservesLinksCreatedBeforeCollectionOwnership()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		var messages = new List<string>();
		ArmageddonHeritage.Install(db, messages);
		var giant = db.Races.Single(x => x.Name == "Armageddon Half-Giant");
		var strength = db.TraitDefinitions.Single(x => x.Name == "Strength");
		// Reproduce an earlier installed scaffold with no attribute collection ownership yet.
		db.SeederManagedRecords.Remove(db.SeederManagedRecords.Single(x => x.StableKey == "arm.race.half_giant.attributes"));
		db.RacesAttributes.RemoveRange(db.RacesAttributes.Where(x => x.RaceId == giant.Id));
		db.SaveChanges();
		db.TraitDefinitions.Add(new TraitDefinition { Id = 9001, Name = "Builder Derived Trait", Type = 3 });
		db.RacesAttributes.AddRange(new RacesAttributes { RaceId = giant.Id, AttributeId = strength.Id, AttributeBonus = 17, DiceExpression = "2d10", IsHealthAttribute = true },
			new RacesAttributes { RaceId = giant.Id, AttributeId = 9001, AttributeBonus = 2, DiceExpression = "1d6" });
		db.SaveChanges();
		ArmageddonHeritage.Install(db, messages);
		ArmageddonHeritage.Install(db, messages);
		var adopted = db.RacesAttributes.Single(x => x.RaceId == giant.Id && x.AttributeId == strength.Id);
		Assert.AreEqual(17.0, adopted.AttributeBonus);
		Assert.AreEqual("2d10", adopted.DiceExpression);
		Assert.IsTrue(adopted.IsHealthAttribute);
		Assert.AreEqual(2.0, db.RacesAttributes.Single(x => x.RaceId == giant.Id && x.AttributeId == 9001).AttributeBonus);
		Assert.AreEqual(8, db.RacesAttributes.Count(x => x.RaceId == giant.Id));
		Assert.IsTrue(messages.Any(x => x.Contains("pre-existing builder attribute links")));
	}

	[TestMethod]
	public void AttributeInstall_UsesSelectedIdsAndPreservesRenamesBonusesRemovedLinksAndCaps()
	{
		using var db = ArmageddonHeritageTests.Fixture();
		var strength = db.TraitDefinitions.Single(x => x.Name == "Strength");
		var constitution = db.TraitDefinitions.Single(x => x.Name == "Constitution");
		var intelligence = db.TraitDefinitions.Single(x => x.Name == "Intelligence");
		db.TraitDefinitions.AddRange(new TraitDefinition { Id = 9001, Name = "Derived Average", Type = 3 },
			new TraitDefinition { Id = 9002, Name = "Strength", Type = 1, OwnerScope = 1 });
		db.RacesAttributes.Add(new RacesAttributes { RaceId = 101, AttributeId = constitution.Id, IsHealthAttribute = true, DiceExpression = "3d6+1" });
		db.SaveChanges();
		var messages = new List<string>();
		ArmageddonHeritage.Install(db, messages);
		var giant = db.Races.Single(x => x.Name == "Armageddon Half-Giant");
		Assert.AreEqual(84, giant.AttributeTotalCap, "Seven selected raw attributes retain a twelve-point-per-attribute budget.");
		Assert.AreEqual(20, giant.IndividualAttributeCap);
		Assert.AreEqual("3d6+1", giant.DiceExpression);
		var links = db.RacesAttributes.Where(x => x.RaceId == giant.Id).ToArray();
		Assert.AreEqual(7, links.Length);
		Assert.IsFalse(links.Any(x => x.AttributeId >= 9001), "Derived and non-body traits must not receive racial bonuses.");
		Assert.AreEqual(10.0, links.Single(x => x.AttributeId == strength.Id).AttributeBonus);
		Assert.AreEqual(-5.0, links.Single(x => x.AttributeId == intelligence.Id).AttributeBonus);
		Assert.IsTrue(links.Single(x => x.AttributeId == constitution.Id).IsHealthAttribute);
		strength.Name = "Builder Renamed Strength";
		links.Single(x => x.AttributeId == constitution.Id).AttributeBonus = 9;
		links.Single(x => x.AttributeId == constitution.Id).DiceExpression = "2d10";
		db.RacesAttributes.Remove(links.Single(x => x.AttributeId == intelligence.Id));
		giant.AttributeTotalCap = 99;
		db.SaveChanges();
		ArmageddonHeritage.Install(db, messages);
		Assert.AreEqual(99, giant.AttributeTotalCap);
		Assert.AreEqual(10.0, db.RacesAttributes.Single(x => x.RaceId == giant.Id && x.AttributeId == strength.Id).AttributeBonus);
		Assert.AreEqual(9.0, db.RacesAttributes.Single(x => x.RaceId == giant.Id && x.AttributeId == constitution.Id).AttributeBonus);
		Assert.AreEqual("2d10", db.RacesAttributes.Single(x => x.RaceId == giant.Id && x.AttributeId == constitution.Id).DiceExpression);
		Assert.IsFalse(db.RacesAttributes.Any(x => x.RaceId == giant.Id && x.AttributeId == intelligence.Id));
		Assert.AreEqual(1, db.RacesAttributes.Count(x => x.RaceId == 101), "Bootstrap Human remains untouched.");
		Assert.IsTrue(db.Races.Where(x => x.Name.StartsWith("Armageddon ")).All(x => x.AvailabilityProgId == 2));
	}
}

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonCatalogueCreatureFoundationTests
{
	[TestMethod]
	public void CreatureFoundations_InventoryHandsReceiveNativeStrikeAttacks()
	{
		using var db = Fixture();
		foreach (var hand in db.BodypartProtos.Where(x => x.BodypartType == (int)MudSharp.Body.BodypartTypeEnum.GrabbingWielding))
			hand.BodypartType = (int)MudSharp.Body.BodypartTypeEnum.Wielding;
		db.SaveChanges();
		var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var result = ArmageddonCatalogueCreatureFoundations.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		foreach (var blueprint in new ArmageddonStockCatalogue().Blueprints.Where(x => x.Kind == ArmageddonBlueprintKind.RaceBundle))
		{
			var raceId = result.Identities[blueprint.Key];
			var attacks = db.RacesWeaponAttacks.Where(x => x.RaceId == raceId).ToArray();
			Assert.AreEqual(2, attacks.Length);
			foreach (var attack in attacks)
				Assert.AreEqual((int)MudSharp.Body.BodypartTypeEnum.Wielding, db.BodypartProtos.Single(x => x.Id == attack.BodypartId).BodypartType);
		}
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueCreatureFoundations.Install(factory).Status);
	}

	[TestMethod]
	[TestCategory("Integration")]
	[DataRow(ArmageddonProgressionMode.Classic, false)][DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)][DataRow(ArmageddonProgressionMode.Simple, true)]
	public void CreatureFoundations_RerunPreservesManagedRecords(ArmageddonProgressionMode mode, bool classes)
	{
		using var db = Fixture(mode, classes); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var initialClaims = db.SeederManagedRecords.Count();
		var result = ArmageddonCatalogueCreatureFoundations.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		var before = db.SeederManagedRecords.ToDictionary(x => x.StableKey,
			x => (x.LogicalId, x.RevisionNumber, x.SeedBaseline, x.AppliedAt));
		Assert.IsTrue(before.Count > initialClaims, "Installation must create its own managed records.");
		Assert.AreEqual(ArmageddonInstallStatus.Completed, ArmageddonCatalogueCreatureFoundations.Install(factory).Status);
		db.ChangeTracker.Clear();
		Assert.AreEqual(before.Count, db.SeederManagedRecords.Count());
		foreach (var record in db.SeederManagedRecords)
			Assert.AreEqual(before[record.StableKey],
				(record.LogicalId, record.RevisionNumber, record.SeedBaseline, record.AppliedAt), record.StableKey);
		Assert.AreEqual(0, db.GameItems.Count());
		Assert.AreEqual(0, db.Characters.Count());
	}

	[TestMethod]
	public void CreatureFoundations_ChildAttributeBonusOverridesParentRatherThanAdding()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		db.Races.Add(new() { Id = 3, Name = "Source parent", DiceExpression = "3d6" });
		db.RacesAttributes.Add(new() { RaceId = 3, AttributeId = 1, AttributeBonus = 2, DiceExpression = "3d6" });
		db.TraitDefinitions.AddRange(new MudSharp.Models.TraitDefinition { Id = 20, Name = "Inherited ordinary", Type = 1 }, new MudSharp.Models.TraitDefinition { Id = 21, Name = "Inherited custom", Type = 1 });
		db.RacesAttributes.AddRange(new RacesAttributes { RaceId = 3, AttributeId = 20 }, new RacesAttributes { RaceId = 3, AttributeId = 21, DiceExpression = "2d10" });
		db.Races.Single(x => x.Id == 1).ParentRaceId = 3;
		db.Races.Single(x => x.Id == 1).DiceExpression = "4d6";
		db.RacesAttributes.Single(x => x.RaceId == 1).AttributeBonus = 3;
		db.SaveChanges(); db.ChangeTracker.Clear();
		var result = ArmageddonCatalogueCreatureFoundations.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		var raceIds = result.Identities.Where(x => new ArmageddonStockCatalogue().Blueprints.Any(b => b.Kind == ArmageddonBlueprintKind.RaceBundle && b.Key == x.Key))
			.Select(x => x.Value).ToArray();
		Assert.IsTrue(db.RacesAttributes.Where(x => raceIds.Contains(x.RaceId) && x.AttributeId == 1).All(x => x.AttributeBonus == 3));
		Assert.IsTrue(db.RacesAttributes.Where(x => raceIds.Contains(x.RaceId) && x.AttributeId == 20).All(x => x.DiceExpression == "4d6"));
		Assert.IsTrue(db.RacesAttributes.Where(x => raceIds.Contains(x.RaceId) && x.AttributeId == 21).All(x => x.DiceExpression == "2d10"));
	}

	[TestMethod]
	public void CreatureFoundations_BuilderOverridesRetainedAndStockFluidUpdatesInPlace()
	{
		using var db = Fixture(); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var first = ArmageddonCatalogueCreatureFoundations.Install(factory); Assert.AreEqual(ArmageddonInstallStatus.Completed, first.Status);
		db.ChangeTracker.Clear();
		var editedId = first.Identities["arm.race.air_guardian"];
		db.Races.Single(x => x.Id == editedId).Name = "Builder Guardian";
		db.RacesBreathableGases.Single(x => x.RaceId == editedId).Multiplier = 4;
		db.Cultures.Single(x => x.Id == first.Identities["arm.culture.constructs"]).PersonWordNeuter = "construct";
		var affinity = db.Merits.Single(x => x.Id == first.Identities["arm.race.air_guardian.affinity"]);
		var xml = XElement.Parse(affinity.Definition); xml.Element("ElementalAffinities")!.Add(new XElement("Affinity", "Water")); affinity.Definition = xml.ToString();
		db.RacesBreathableGases.Single(x => x.RaceId == 1).Multiplier = 2;
		db.SaveChanges(); db.ChangeTracker.Clear();
		var rerun = ArmageddonCatalogueCreatureFoundations.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages)); db.ChangeTracker.Clear();
		Assert.AreEqual("Builder Guardian", db.Races.Single(x => x.Id == editedId).Name);
		Assert.AreEqual(4.0, db.RacesBreathableGases.Single(x => x.RaceId == editedId).Multiplier);
		Assert.AreEqual(2.0, db.RacesBreathableGases.Single(x => x.RaceId == first.Identities["arm.race.earth_mount"]).Multiplier);
		Assert.AreEqual("construct", db.Cultures.Single(x => x.Id == first.Identities["arm.culture.constructs"]).PersonWordNeuter);
		Assert.IsTrue(LoadAffinity(db.Merits.Single(x => x.Id == affinity.Id)).ElementalAffinities.Contains("Water"));
	}

	[TestMethod]
	public void CreatureFoundations_MissingAnatomyBlocksBeforeNativeContent()
	{
		using var db = ArmageddonCatalogueAnatomyTests.Fixture();
		var result = ArmageddonCatalogueCreatureFoundations.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Blocked, result.Status); Assert.AreEqual(1, db.Races.Count()); Assert.AreEqual(0, db.Merits.Count());
	}

	internal static FuturemudDatabaseContext Fixture(ArmageddonProgressionMode mode = ArmageddonProgressionMode.Simple, bool classes = false, bool enableStockForNewInstall = false)
	{
		var db = ArmageddonCatalogueAnatomyTests.Fixture(mode, classes, enableStockForNewInstall);
		var race = db.Races.Single(x => x.Id == 1); race.NeedsToBreathe = true; race.BreathingModel = "simple"; race.HungerRate = 0.8; race.ThirstRate = 1.2;
		race.BloodModelId = 1;
		db.BloodModels.Add(new() { Id = 1, Name = "source blood" }); db.Bloodtypes.Add(new() { Id = 1, Name = "source type" });
		db.BloodModelsBloodtypes.Add(new() { BloodModelId = 1, BloodtypeId = 1 });
		db.PopulationBloodModels.Add(new() { Id = 1, Name = "source population" });
		db.PopulationBloodModelsBloodtypes.Add(new() { PopulationBloodModelId = 1, BloodtypeId = 1, Weight = 1 });
		db.Ethnicities.Add(new() { Id = 1, Name = "source ethnicity", ParentRaceId = 1, PopulationBloodModelId = 1 });
		db.RacesAttributes.Add(new() { RaceId = 1, AttributeId = 1, IsHealthAttribute = true, DiceExpression = "3d6" });
		db.Gases.Add(new() { Id = 1, Name = "air" }); db.RacesBreathableGases.Add(new() { RaceId = 1, GasId = 1, Multiplier = 1 });
		for (short gender = 1; gender <= 4; gender++) db.CulturesNameCultures.Add(new() { CultureId = 1, NameCultureId = 1, Gender = gender });
		db.SaveChanges();
		var anatomy = ArmageddonCatalogueAnatomy.Install(ArmageddonInstallationSettingsTests.Factory(db));
		Assert.AreEqual(ArmageddonInstallStatus.Completed, anatomy.Status, string.Join("\n", anatomy.Messages)); db.ChangeTracker.Clear();
		return db;
	}

	private static ElementalAffinityMerit LoadAffinity(Merit row)
	{
		var progs = new All<IFutureProg>();
		foreach (var id in new[] { 1L, 2L }) { var prog = new Mock<IFutureProg>(); prog.SetupGet(x => x.Id).Returns(id); progs.Add(prog.Object); }
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.FutureProgs).Returns(progs);
		return new ElementalAffinityMerit(row, world.Object);
	}

	private static double Evaluate(FuturemudDatabaseContext db, long id, ICharacter actor)
	{
		var traits = new All<ITraitDefinition>(); var trait = new Mock<ITraitDefinition>(); trait.SetupGet(x => x.Id).Returns(1); traits.Add(trait.Object);
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.Traits).Returns(traits);
		return new MudSharp.Body.Traits.TraitExpression(db.TraitExpressions.Include(x => x.TraitExpressionParameters).Single(x => x.Id == id), world.Object).Evaluate(actor);
	}
}

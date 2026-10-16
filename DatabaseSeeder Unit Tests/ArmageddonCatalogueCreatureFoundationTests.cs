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
	[DataTestMethod]
	[DataRow(ArmageddonProgressionMode.Classic, false)][DataRow(ArmageddonProgressionMode.Classic, true)]
	[DataRow(ArmageddonProgressionMode.Simple, false)][DataRow(ArmageddonProgressionMode.Simple, true)]
	public void CreatureFoundations_IndependentNativeBundlesAndExactGradeAttacks(ArmageddonProgressionMode mode, bool classes)
	{
		using var db = Fixture(mode, classes); var factory = ArmageddonInstallationSettingsTests.Factory(db);
		var result = ArmageddonCatalogueCreatureFoundations.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, result.Status, string.Join("\n", result.Messages));
		db.ChangeTracker.Clear();
		Assert.AreEqual(22, result.Identities.Count); Assert.AreEqual(8, db.Races.Count()); Assert.AreEqual(8, db.Ethnicities.Count());
		var culture = db.Cultures.Include(x => x.CulturesNameCultures).Single(x => x.Id == result.Identities["arm.culture.constructs"]);
		Assert.AreEqual(5, culture.CulturesNameCultures.Count); Assert.AreEqual(1L, culture.NativeLanguageId);
		foreach (var blueprint in new ArmageddonStockCatalogue().Blueprints.Where(x => x.Kind == ArmageddonBlueprintKind.RaceBundle))
		{
			var race = db.Races.Include(x => x.RacesAttributes).Include(x => x.RacesBreathableGases).Include(x => x.RacesWeaponAttacks)
				.Single(x => x.Id == result.Identities[blueprint.Key]);
			Assert.IsNull(race.ParentRaceId); Assert.AreNotEqual(2L, race.BaseBodyId); Assert.IsTrue(race.NeedsToBreathe);
			Assert.AreEqual(0.8, race.HungerRate); Assert.AreEqual(1.2, race.ThirstRate);
			Assert.AreEqual(1, race.RacesBreathableGases.Count); Assert.AreEqual(1L, race.DefaultHealthStrategyId);
			Assert.AreEqual(2, race.RacesWeaponAttacks.Count); Assert.IsTrue(race.RacesAttributes.Any(x => x.IsHealthAttribute));
			Assert.IsTrue(race.RacesWeaponAttacks.All(x => db.BodypartProtos.Any(p => p.Id == x.BodypartId && p.BodyId == race.BaseBodyId)));
			var merit = db.Merits.Single(x => x.Id == result.Identities[blueprint.Key + ".affinity"]);
			CollectionAssert.AreEqual(blueprint.Configuration.GetProperty("affinity").EnumerateArray().Select(x => x.GetString()!).ToArray(),
				LoadAffinity(merit).ElementalAffinities.ToArray());
			var attack = db.WeaponAttacks.Single(x => x.Id == race.RacesWeaponAttacks.First().WeaponAttackId);
			Assert.IsNull(attack.WeaponTypeId); Assert.AreEqual(5.0, attack.StaminaCost);
			for (var grade = 1; grade <= 7; grade++)
			{
				var actor = new Mock<ICharacter>();
				actor.Setup(x => x.TraitValue(It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>())).Returns(10 + 2 * grade);
				Assert.AreEqual(3.0 * grade, Evaluate(db, attack.DamageExpressionId, actor.Object), 0.000001);
				Assert.AreEqual((double)grade, Evaluate(db, attack.PainExpressionId, actor.Object), 0.000001);
				Assert.AreEqual(0.0, Evaluate(db, attack.StunExpressionId, actor.Object), 0.000001);
			}
		}
		var before = db.SeederManagedRecords.ToDictionary(x => x.StableKey, x => (x.LogicalId, x.SeedBaseline, x.AppliedAt));
		var rerun = ArmageddonCatalogueCreatureFoundations.Install(factory);
		Assert.AreEqual(ArmageddonInstallStatus.Completed, rerun.Status, string.Join("\n", rerun.Messages));
		db.ChangeTracker.Clear();
		foreach (var record in db.SeederManagedRecords) Assert.AreEqual(before[record.StableKey], (record.LogicalId, record.SeedBaseline, record.AppliedAt));
		ArmageddonInstallation.Prepare(factory, mode);
		Assert.AreEqual(0, db.Characters.Count()); Assert.AreEqual(0, db.Bodies.Count()); Assert.AreEqual(0, db.GameItems.Count()); Assert.AreEqual(0, db.NpcTemplates.Count());
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

	internal static FuturemudDatabaseContext Fixture(ArmageddonProgressionMode mode = ArmageddonProgressionMode.Simple, bool classes = false)
	{
		var db = ArmageddonCatalogueAnatomyTests.Fixture(mode, classes);
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

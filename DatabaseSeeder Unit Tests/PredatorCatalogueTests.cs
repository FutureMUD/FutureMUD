#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Models;
using WeaponAttack = MudSharp.Models.WeaponAttack;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PredatorCatalogueTests
{
	[DataTestMethod]
	[DataRow("Giant Spider", "Beast Skirmisher", "Beast Clincher")]
	[DataRow("Giant Spider", "My Custom Spider", "My Custom Spider")]
	[DataRow("Leopard", "Beast Skirmisher", "Beast Brawler")]
	[DataRow("Leopard", "My Custom Cat", "My Custom Cat")]
	[DataRow("Panther", "Beast Skirmisher", "Beast Brawler")]
	[DataRow("Panther", "My Custom Cat", "My Custom Cat")]
	[DataRow("Eagle", "Beast Dropper", "Beast Dropper")]
	[DataRow("Eagle", "My Custom Bird", "My Custom Bird")]
	[DataRow("Crocodile", "Beast Drowner", "Beast Drowner")]
	[DataRow("Crocodile", "My Custom Reptile", "My Custom Reptile")]
	[DataRow("Yacumama", "Beast Clincher", "Beast Drowner")]
	[DataRow("Yacumama", "My Custom Serpent", "My Custom Serpent")]
	public void PredatorDefaults_FreshContext_RepairLegacyApproachAndPreserveCustomSetting(string raceName, string original, string expected)
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options;
		using (var context = new FuturemudDatabaseContext(options))
		{
			context.FutureProgs.AddRange(new FutureProg { Id = 1, FunctionName = "AlwaysTrue" },
				new FutureProg { Id = 2, FunctionName = "IsHumanoid" });
			var race = new Race { Id = 1, Name = raceName, DefaultCombatSetting = new CharacterCombatSetting { Id = 5, Name = original } };
			var fang = new BodypartProto { Id = 1, Name = "rfang" };
			var bite = new WeaponAttack { Id = 1, Name = "Spider Bite", MoveType = (int)BuiltInCombatMoveType.NaturalWeaponAttack };
			context.Races.Add(race);
			context.RacesWeaponAttacks.Add(new RacesWeaponAttacks { Race = race, Bodypart = fang, WeaponAttack = bite });
			context.SaveChanges();
		}
		using var loaded = new FuturemudDatabaseContext(options);
		PredatorCombatSeederHelper.Ensure(loaded);
		Assert.AreEqual(expected, loaded.Races.Single().DefaultCombatSetting.Name);
		var fallbackIds = loaded.CombatMessages.Where(x => x.Priority == -100).OrderBy(x => x.Type).Select(x => x.Id).ToArray();
		Assert.AreEqual(3, fallbackIds.Length);
		PredatorCombatSeederHelper.Ensure(loaded);
		CollectionAssert.AreEqual(fallbackIds, loaded.CombatMessages.Where(x => x.Priority == -100).OrderBy(x => x.Type).Select(x => x.Id).ToArray());
		Assert.IsTrue(loaded.CombatMessages.Where(x => x.Priority == -100).All(x => x.Chance == 1 && !string.IsNullOrEmpty(x.FailureMessage)));
	}

	[TestMethod]
	public void StockAnimalBreathing_UsesCanonicalAirAndPreservesAuthoredTolerances()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options;
		using var context = new FuturemudDatabaseContext(options);
		var wrong = new Gas { Id = 1, Name = "Hydrogen" };
		var air = new Gas { Id = 9, Name = "Breathable Atmosphere" };
		var viper = new Race { Id = 1, Name = "Viper", BreathingModel = "simple" };
		var custom = new Race { Id = 2, Name = "Custom Snake", BreathingModel = "simple" };
		context.Gases.AddRange(wrong, air); context.Races.AddRange(viper, custom);
		context.RacesBreathableGases.Add(new RacesBreathableGases { Race = viper, RaceId = 1, Gas = wrong, GasId = 1, Multiplier = .5 });
		context.SaveChanges();
		AnimalSeeder.EnsureStockAirBreathing(context);
		AnimalSeeder.EnsureStockAirBreathing(context);
		Assert.AreEqual(1, context.RacesBreathableGases.Count(x => x.RaceId == 1 && x.GasId == 9));
		Assert.AreEqual(.5, context.RacesBreathableGases.Single(x => x.GasId == 1).Multiplier);
		Assert.IsFalse(context.RacesBreathableGases.Any(x => x.RaceId == 2));
	}

	[DataTestMethod]
	[DataRow("Viper", false)]
	[DataRow("Cobra", false)]
	[DataRow("Rattlesnake", false)]
	[DataRow("Viper", true)]
	[DataRow("Cobra", true)]
	[DataRow("Rattlesnake", true)]
	public void SerpentVenom_Rerun_AddsProneMeleeAndClinchWithoutChangingSource(string raceName, bool clinchOnly)
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options;
		using var context = new FuturemudDatabaseContext(options);
		var race = new Race { Id = 1, Name = raceName };
		var fang = new BodypartProto { Id = 1, Name = "rfang", BodypartShapeId = 3 };
		var venom = new WeaponAttack { Id = 1, Name = raceName + " Bite", MoveType = (int)(clinchOnly ? BuiltInCombatMoveType.EnvenomingAttackClinch : BuiltInCombatMoveType.EnvenomingAttack),
			RequiredPositionStateIds = "1 16 17 18", BodypartShapeId = 3, AdditionalInfo = "<Data><Liquid>42</Liquid></Data>" };
		context.Races.Add(race); context.BodypartProtos.Add(fang); context.WeaponAttacks.Add(venom);
		context.RacesWeaponAttacks.Add(new RacesWeaponAttacks { Race = race, RaceId = 1, Bodypart = fang, BodypartId = 1, WeaponAttack = venom, WeaponAttackId = 1 });
		context.SaveChanges();
		PredatorCombatSeederHelper.Ensure(context);
		var ids = context.WeaponAttacks.OrderBy(x => x.Id).Select(x => x.Id).ToArray();
		PredatorCombatSeederHelper.Ensure(context);
		CollectionAssert.AreEqual(ids, context.WeaponAttacks.OrderBy(x => x.Id).Select(x => x.Id).ToArray());
		var clones = context.WeaponAttacks.Where(x => x.Name.StartsWith("Wildlife - Serpent")).ToList();
		Assert.AreEqual(2, clones.Count);
		Assert.IsTrue(clones.All(x => x.RequiredPositionStateIds.Split(' ').Contains("6") && x.AdditionalInfo == venom.AdditionalInfo));
		Assert.AreEqual("1 16 17 18", venom.RequiredPositionStateIds);
		Assert.AreEqual(3, context.RacesWeaponAttacks.Count(x => x.BodypartId == 1));
	}

	[DataTestMethod]
	[DataRow("Beast Dropper", CombatStrategyMode.Dropper)]
	[DataRow("Beast Drowner", CombatStrategyMode.Drowner)]
	public void PredatorStrategy_LegacyRangeRepair_PreservesDifferentAuthoredApproach(string name, CombatStrategyMode mode)
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options;
		using var context = new FuturemudDatabaseContext(options);
		var setting = new CharacterCombatSetting { Name = name, PreferredMeleeMode = (int)mode, PreferredRangedMode = (int)CombatStrategyMode.StandardRange };
		context.CharacterCombatSettings.Add(setting); context.SaveChanges();
		CombatStrategySeederHelper.EnsureCombatStrategy(context, name);
		Assert.AreEqual((int)mode, setting.PreferredRangedMode);
		setting.PreferredRangedMode = (int)CombatStrategyMode.FullAdvance; context.SaveChanges();
		CombatStrategySeederHelper.EnsureCombatStrategy(context, name);
		Assert.AreEqual((int)CombatStrategyMode.FullAdvance, setting.PreferredRangedMode);
	}

	[DataTestMethod]
	[DataRow("Leopard", "Ambush", "Extract", "Desperate", "Balanced")]
	[DataRow("Tiger", "Ambush", "Fight", "Desperate", "Balanced")]
	[DataRow("Crocodile", "Ambush", "Extract", "Eligible", "Balanced")]
	[DataRow("Alligator", "Ambush", "Extract", "Desperate", "Balanced")]
	[DataRow("Caiman", "Ambush", "Extract", "Never", "Balanced")]
	[DataRow("Bunyip", "Ambush", "Extract", "Eligible", "Bold")]
	[DataRow("Eagle", "Direct", "Fight", "Never", "Cautious")]
	[DataRow("Griffin", "Direct", "Fight", "Eligible", "Bold")]
	[DataRow("Spider", "TrapWait", "VenomWithdrawal", "Never", "Cautious")]
	[DataRow("Tarantula", "TrapWait", "Fight", "Never", "Cautious")]
	[DataRow("Scorpion", "TrapWait", "VenomWithdrawal", "Never", "Cautious")]
	[DataRow("Adder", "Ambush", "VenomWithdrawal", "Never", "Cautious")]
	[DataRow("Giant Spider", "TrapWait", "VenomWithdrawal", "Eligible", "Bold")]
	[DataRow("Giant Scorpion", "TrapWait", "VenomWithdrawal", "Eligible", "Bold")]
	[DataRow("Giant Centipede", "TrapWait", "VenomWithdrawal", "Eligible", "Bold")]
	[DataRow("Ankheg", "TrapWait", "Fight", "Eligible", "Bold")]
	public void PredatorProfiles_SpeciesDefaults_MatchPolicy(string species, string opening, string followup, string people, string assessment)
	{
		var recommendation = WildlifeCatalogue.Recommendations.Single(x => x.RaceName == species);
		var profile = WildlifeCatalogue.IndividualProfiles.Single(x => x.Name == recommendation.IndividualAiTemplate);
		Assert.AreEqual(opening, profile.HuntOpening); Assert.AreEqual(followup, profile.HuntFollowup);
		Assert.AreEqual(people, profile.PeoplePrey); Assert.AreEqual(assessment, profile.HuntAssessment);
	}

	[TestMethod]
	public void PredatorProfiles_ExactRoster_GenericProfilesRemainLegacy()
	{
		string[] roster = ["Leopard", "Panther", "Jaguar", "Tiger", "Crocodile", "Alligator", "Caiman", "Eagle", "Spider", "Tarantula", "Scorpion",
			"Adder", "Cobra", "Coral Snake", "Mamba", "Moccasin", "Rattlesnake", "Viper", "Bunyip", "Yacumama", "Griffin", "Garuda", "Giant Eagle", "Wyvern", "Fell Beast",
			"Giant Spider", "Giant Scorpion", "Giant Centipede", "Ankheg", "Giant Worm", "Colossal Worm"];
		var advanced = WildlifeCatalogue.Recommendations.Where(x => WildlifeCatalogue.IndividualProfiles.Single(p => p.Name == x.IndividualAiTemplate).HuntOpening is not null)
			.Select(x => x.RaceName).ToArray();
		CollectionAssert.AreEquivalent(roster, advanced);
		Assert.IsNull(WildlifeCatalogue.IndividualProfiles.Single(x => x.Name == WildlifeCatalogue.GroundStalkingPredator).HuntOpening);
		Assert.AreEqual(0, WildlifeCatalogue.ValidateCatalogForTesting().Count);
	}

	[TestMethod]
	public void PredatorAttacks_Rerun_KeepsIdentitiesRepairsStockAndPreservesClones()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options;
		using var context = new FuturemudDatabaseContext(options);
		context.FutureProgs.AddRange(new FutureProg { Id = 1, FunctionName = "AlwaysTrue" },
			new FutureProg { Id = 2, FunctionName = "IsHumanoid" });
		var race = new Race { Id = 1, Name = "Leopard" };
		var part = new BodypartProto { Id = 2, Name = "rclaw", BodypartShapeId = 3 };
		var attack = new WeaponAttack { Id = 4, Name = "Claw Swipe", MoveType = (int)BuiltInCombatMoveType.NaturalWeaponAttack,
			RequiredPositionStateIds = "1 16 17", Weighting = 100, DamageExpressionId = 5, PainExpressionId = 5, StunExpressionId = 5, BodypartShapeId = 3 };
		context.Races.Add(race); context.BodypartProtos.Add(part); context.WeaponAttacks.Add(attack);
		context.RacesWeaponAttacks.Add(new RacesWeaponAttacks { Race = race, RaceId = 1, Bodypart = part, BodypartId = 2, WeaponAttack = attack, WeaponAttackId = 4, Quality = 3 });
		context.SaveChanges();
		PredatorCombatSeederHelper.Ensure(context);
		var originalIds = context.WeaponAttacks.OrderBy(x => x.Name).Select(x => x.Id).ToArray();
		var linkCount = context.RacesWeaponAttacks.Count();
		var ambush = context.WeaponAttacks.Single(x => x.MoveType == (int)BuiltInCombatMoveType.AmbushAttack);
		Assert.IsTrue(XElement.Parse(ambush.AdditionalInfo).Element("Seize")!.Value == "true");
		CollectionAssert.AreEquivalent(new[] { "1", "15", "16", "17" }, ambush.RequiredPositionStateIds.Split(' '));
		Assert.AreEqual("1 16 17", attack.RequiredPositionStateIds, "The source claw attack keeps its authored postures.");
		Assert.IsTrue(context.WeaponAttacks.Where(x => x.MoveType == (int)BuiltInCombatMoveType.InitiateGrapple ||
			x.MoveType == (int)BuiltInCombatMoveType.ExtendGrapple || x.MoveType == (int)BuiltInCombatMoveType.ForcedMovementUnarmed)
			.ToList().All(x => x.RequiredPositionStateIds.Split(' ').Contains("15")), "Control and carrying remain usable after tree extraction.");
		Assert.AreEqual("15", context.WeaponAttacks.Single(x => x.Name == "Wildlife - Climbing Claw Swipe").RequiredPositionStateIds);
		Assert.IsTrue(race.CanClimb);
		Assert.AreEqual("Beast Brawler", race.DefaultCombatSetting.Name);
		ambush.StaminaCost = 999;
		var custom = new WeaponAttack { Name = "My custom ambush", MoveType = (int)BuiltInCombatMoveType.AmbushAttack, StaminaCost = 17 };
		context.WeaponAttacks.Add(custom); context.SaveChanges();
		PredatorCombatSeederHelper.Ensure(context);
		CollectionAssert.AreEqual(originalIds, context.WeaponAttacks.Where(x => x.Id != custom.Id).OrderBy(x => x.Name).Select(x => x.Id).ToArray());
		Assert.AreEqual(linkCount, context.RacesWeaponAttacks.Count());
		Assert.AreEqual(6, ambush.StaminaCost); Assert.AreEqual(17, custom.StaminaCost);
		Assert.AreEqual(1, context.CombatMessages.Count(x => x.Type == (int)BuiltInCombatMoveType.AmbushAttack));
	}

	[TestMethod]
	public void OrganicCorpses_RerunRepairsMissingStockMaterialsAndPreservesAuthoredMaps()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options;
		using var context = new FuturemudDatabaseContext(options);
		context.Materials.AddRange(new Material { Id = 11, Name = "flesh" }, new Material { Id = 12, Name = "bony flesh" }, new Material { Id = 13, Name = "bone" });
		var animal = new CorpseModel { Id = 1, Name = "Organic Animal Corpse", Type = "Standard", Definition = "<CorpseModel><EdiblePercentage>0.35</EdiblePercentage></CorpseModel>" };
		var human = new CorpseModel { Id = 2, Name = "Organic Human Corpse", Type = "Standard", Definition = "<CorpseModel><CorpseMaterials><CorpseMaterial state='0'>99</CorpseMaterial></CorpseMaterials></CorpseModel>" };
		var custom = new CorpseModel { Id = 3, Name = "Custom Corpse", Type = "Standard", Definition = "<CorpseModel />" };
		context.CorpseModels.AddRange(animal, human, custom); context.SaveChanges();
		var authored = human.Definition;
		OrganicCorpseMaterialSeederHelper.EnsureStockModels(context);
		var xml = XElement.Parse(animal.Definition);
		Assert.AreEqual("0.35", xml.Element("EdiblePercentage")!.Value);
		Assert.AreEqual("11", xml.Element("CorpseMaterials")!.Elements().Single(x => x.Attribute("state")!.Value == "0").Value);
		Assert.AreEqual("13", xml.Element("CorpseMaterials")!.Elements().Single(x => x.Attribute("state")!.Value == "5").Value);
		Assert.AreEqual(6, xml.Element("CorpseMaterials")!.Elements().Count());
		var repaired = animal.Definition;
		OrganicCorpseMaterialSeederHelper.EnsureStockModels(context);
		Assert.AreEqual(repaired, animal.Definition); Assert.AreEqual(authored, human.Definition);
		Assert.AreEqual("<CorpseModel />", custom.Definition);
	}

	[TestMethod]
	public void GiantCentipede_UsesMixedArthropodVenomOnItsOwnMandibles()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options;
		using var context = new FuturemudDatabaseContext(options);
		var giant = new Race { Id = 1, Name = "Giant Centipede" };
		var spider = new Race { Id = 2, Name = "Spider" };
		var mandible = new BodypartProto { Id = 1, Name = "rmandible", BodypartShapeId = 7 };
		var fang = new BodypartProto { Id = 2, Name = "rfang", BodypartShapeId = 8 };
		var bite = new WeaponAttack { Id = 1, Name = "Mandible Bite", MoveType = (int)BuiltInCombatMoveType.NaturalWeaponAttack };
		var venom = new WeaponAttack { Id = 2, Name = "Spider Venom", MoveType = (int)BuiltInCombatMoveType.EnvenomingAttack,
			AdditionalInfo = "<Data><Liquid>42</Liquid><MaximumQuantity>0.0025</MaximumQuantity></Data>" };
		context.Races.AddRange(giant, spider); context.BodypartProtos.AddRange(mandible, fang);
		context.WeaponAttacks.AddRange(bite, venom);
		context.RacesWeaponAttacks.AddRange(
			new RacesWeaponAttacks { Race = giant, RaceId = 1, Bodypart = mandible, BodypartId = 1, WeaponAttack = bite, WeaponAttackId = 1 },
			new RacesWeaponAttacks { Race = spider, RaceId = 2, Bodypart = fang, BodypartId = 2, WeaponAttack = venom, WeaponAttackId = 2 });
		context.SaveChanges();
		PredatorCombatSeederHelper.Ensure(context);
		var attacks = context.RacesWeaponAttacks.Include(x => x.WeaponAttack).Where(x => x.RaceId == 1 &&
			(x.WeaponAttack.MoveType == (int)BuiltInCombatMoveType.EnvenomingAttack || x.WeaponAttack.MoveType == (int)BuiltInCombatMoveType.EnvenomingAttackClinch)).ToList();
		Assert.AreEqual(2, attacks.Count);
		Assert.IsTrue(attacks.All(x => x.BodypartId == mandible.Id && x.WeaponAttack.BodypartShapeId == 7 && x.WeaponAttack.AdditionalInfo == venom.AdditionalInfo));
	}
}

#nullable enable

using System;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NPCSkillPackageSeederTests
{
	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.Options;
		return new FuturemudDatabaseContext(options);
	}

	private static TraitDefinition Skill(long id, string name)
	{
		return new TraitDefinition
		{
			Id = id,
			Name = name,
			Alias = name,
			Type = 0,
			TraitGroup = "Tests",
			ChargenBlurb = string.Empty,
			ValueExpression = string.Empty
		};
	}

	[TestMethod]
	public void UniversalPackage_ResolvesSimpleAndComplexSkillVariants()
	{
		using var simple = BuildContext();
		simple.TraitDefinitions.AddRange(Skill(1, "Perception"), Skill(2, "Athletics"));
		simple.SaveChanges();
		NPCSkillPackageSeederHelper.EnsureUniversalPackage(simple);
		var simplePackage = simple.NpcSkillPackages.Include(x => x.Skills).Single();
		CollectionAssert.AreEquivalent(new long[] { 1, 2 }, simplePackage.Skills.Select(x => x.TraitDefinitionId).ToArray());
		Assert.IsTrue(simplePackage.Skills.All(x => x.Chance == 1.0 && x.Mean == 25.0 &&
			x.StandardDeviation == 5.0 && x.Skewness == 0.0));

		using var complex = BuildContext();
		complex.TraitDefinitions.AddRange(
			Skill(1, "Listen"), Skill(2, "Search"), Skill(3, "Scan"),
			Skill(4, "Climb"), Skill(5, "Balance"), Skill(6, "Run"));
		complex.SaveChanges();
		NPCSkillPackageSeederHelper.EnsureUniversalPackage(complex);
		Assert.AreEqual(6, complex.NpcSkillPackages.Include(x => x.Skills).Single().Skills.Count);
	}

	[TestMethod]
	public void CombatPackages_RerunUpdatesStockAndPreservesCustomPackage()
	{
		using var context = BuildContext();
		var brawling = Skill(1, "Brawling");
		var dodge = Skill(2, "Dodge");
		context.TraitDefinitions.AddRange(brawling, dodge);
		var custom = new NpcSkillPackage { Name = "Builder Custom" };
		custom.Skills.Add(new NpcSkillPackageSkill
		{
			TraitDefinition = brawling,
			Chance = 0.5,
			Mean = 99.0,
			StandardDeviation = 1.0,
			Skewness = 0.25
		});
		context.NpcSkillPackages.Add(custom);
		context.SaveChanges();

		var first = NPCSkillPackageSeederHelper.EnsureCombatPackages(context);
		var second = NPCSkillPackageSeederHelper.EnsureCombatPackages(context);

		Assert.AreEqual(5, first.PackagesChanged);
		Assert.AreEqual(0, second.PackagesChanged);
		Assert.AreEqual(6, context.NpcSkillPackages.Count());
		var preserved = context.NpcSkillPackages.Include(x => x.Skills).Single(x => x.Name == "Builder Custom");
		Assert.AreEqual(99.0, preserved.Skills.Single().Mean);
		Assert.IsTrue(context.NpcSkillPackages
			.Where(x => x.Name.EndsWith("Beast Attacker"))
			.All(x => x.Skills.Count == 2));
	}

	[TestMethod]
	public void CombatTierMapping_PlacesWargAndDragonAtRequiredThreatLevels()
	{
		var warg = new StockNPCSkillPackageRace("Warg", false, true, true, NonHumanCombatTier.SeriousThreat);
		var dragon = new StockNPCSkillPackageRace("Dragon", true, true, true, NonHumanCombatTier.PartyBoss);

		Assert.AreEqual("Terrifying Beast Attacker", NPCSkillPackageSeederHelper.CombatPackageFor(warg));
		Assert.AreEqual("Apex Beast Attacker", NPCSkillPackageSeederHelper.CombatPackageFor(dragon));
		Assert.AreEqual(NonHumanCombatTier.Avatar,
			NPCSkillPackageSeederHelper.TierForAnimal(SizeCategory.Titanic));
	}

	[TestMethod]
	public void StockDetection_AdvertisesDefinitionAndRaceLinkRepairs()
	{
		using var context = BuildContext();
		context.TraitDefinitions.AddRange(
			Skill(1, "Brawling"),
			Skill(2, "Dodge"),
			Skill(3, "Athletics"));
		var race = new Race
		{
			Id = 1,
			Name = "Test Flyer",
			Description = string.Empty,
			AllowedGenders = string.Empty,
			DiceExpression = string.Empty,
			CommunicationStrategyType = string.Empty,
			HandednessOptions = string.Empty,
			MaximumDragWeightExpression = string.Empty,
			MaximumLiftWeightExpression = string.Empty,
			EatCorpseEmoteText = string.Empty,
			BreathingVolumeExpression = string.Empty,
			HoldBreathLengthExpression = string.Empty
		};
		context.Races.Add(race);
		context.SaveChanges();
		var stockRaces = new[]
		{
			new StockNPCSkillPackageRace(race.Name, true, false, false, NonHumanCombatTier.SeriousThreat)
		};

		NPCSkillPackageSeederHelper.EnsureRacePackages(context, stockRaces);
		Assert.IsFalse(NPCSkillPackageSeederHelper.HasMissingStockRacePackages(context, stockRaces));

		var combatEntry = context.NpcSkillPackages
			.Include(x => x.Skills)
			.Single(x => x.Name == "Competent Beast Attacker")
			.Skills.First();
		combatEntry.Mean = 999.0;
		context.SaveChanges();
		Assert.IsTrue(NPCSkillPackageSeederHelper.HasMissingStockRacePackages(context, stockRaces));

		NPCSkillPackageSeederHelper.EnsureRacePackages(context, stockRaces);
		Assert.IsFalse(NPCSkillPackageSeederHelper.HasMissingStockRacePackages(context, stockRaces));

		context.Entry(race).Collection(x => x.NpcSkillPackages).Load();
		race.NpcSkillPackages.Remove(race.NpcSkillPackages.Single(x => x.Name == "Flying Race"));
		context.SaveChanges();
		Assert.IsTrue(NPCSkillPackageSeederHelper.HasMissingStockRacePackages(context, stockRaces));
	}

	[TestMethod]
	public void UniversalDetection_AdvertisesStaleDefinitionRepair()
	{
		using var context = BuildContext();
		context.TraitDefinitions.AddRange(Skill(1, "Perception"), Skill(2, "Athletics"));
		context.SaveChanges();
		NPCSkillPackageSeederHelper.EnsureUniversalPackage(context);
		Assert.IsFalse(NPCSkillPackageSeederHelper.HasMissingUniversalPackage(context));

		context.NpcSkillPackages
			.Include(x => x.Skills)
			.Single(x => x.Name == "Universal Common")
			.Skills.First().StandardDeviation = 99.0;
		context.SaveChanges();

		Assert.IsTrue(NPCSkillPackageSeederHelper.HasMissingUniversalPackage(context));
	}
}

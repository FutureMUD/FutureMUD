using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat;
using MudSharp.Health;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AnimalSeederTemplateTests
{
	private static int ParagraphCount(string text)
	{
		return text
			.Split(["\r\n\r\n", "\n\n"], System.StringSplitOptions.RemoveEmptyEntries)
			.Length;
	}

	[TestMethod]
	public void ValidateTemplateCatalogForTesting_CurrentCatalog_HasNoIssues()
	{
		var issues = AnimalSeeder.ValidateTemplateCatalogForTesting();
		Assert.AreEqual(0, issues.Count, string.Join("\n", issues));
	}

	[TestMethod]
	public void RaceTemplatesForTesting_WeaselAndRhinocerous_UseExpectedBodyAssignments()
	{
		var weasel = AnimalSeeder.RaceTemplatesForTesting["Weasel"];
		Assert.AreEqual("Toed Quadruped", weasel.BodyKey, "Weasel should use the toed quadruped body.");

		var rhino = AnimalSeeder.RaceTemplatesForTesting["Rhinocerous"];
		Assert.AreEqual("Ungulate", rhino.BodyKey, "Rhinocerous should continue to use the ungulate-derived body.");
		Assert.IsNotNull(rhino.AdditionalBodypartUsages, "Rhinocerous should define horn bodypart usages.");
		Assert.IsTrue(
			rhino.AdditionalBodypartUsages!.Any(x => x.BodypartAlias == "horn" && x.Usage == "general"),
			"Rhinocerous should expose the shared horn usage.");
	}

	[TestMethod]
	public void RaceTemplatesForTesting_CombatStrategyKeys_MapRepresentativeAnimalsToExpectedStyles()
	{
		Assert.AreEqual("Beast Coward", AnimalSeeder.RaceTemplatesForTesting["Rabbit"].CombatStrategyKey);
		Assert.AreEqual("Beast Skirmisher", AnimalSeeder.RaceTemplatesForTesting["Cheetah"].CombatStrategyKey);
		Assert.AreEqual("Beast Artillery", AnimalSeeder.RaceTemplatesForTesting["Llama"].CombatStrategyKey);
		Assert.AreEqual("Beast Swooper", AnimalSeeder.RaceTemplatesForTesting["Eagle"].CombatStrategyKey);
		Assert.AreEqual("Beast Clincher", AnimalSeeder.RaceTemplatesForTesting["Python"].CombatStrategyKey);
		Assert.AreEqual("Beast Behemoth", AnimalSeeder.RaceTemplatesForTesting["Elephant"].CombatStrategyKey);
	}

	[TestMethod]
	public void StockDescriptions_RaceEthnicityAndCulture_AreThreeParagraphsAndRepresentative()
	{
		var dogRaceDescription = AnimalSeeder.BuildRaceDescriptionForTesting(AnimalSeeder.RaceTemplatesForTesting["Dog"]);
		Assert.AreEqual(3, ParagraphCount(dogRaceDescription));
		StringAssert.Contains(dogRaceDescription, "Adults are most often represented");

		var dogEthnicityDescription = AnimalSeeder.BuildEthnicityDescriptionForTesting("Dog", "Retriever");
		Assert.AreEqual(3, ParagraphCount(dogEthnicityDescription));
		StringAssert.Contains(dogEthnicityDescription, "downed game");

		var bearEthnicityDescription = AnimalSeeder.BuildEthnicityDescriptionForTesting("Bear", "Brown Bear");
		Assert.AreEqual(3, ParagraphCount(bearEthnicityDescription));
		StringAssert.Contains(bearEthnicityDescription, "hump-backed shoulder power");

		Assert.AreEqual(3, ParagraphCount(AnimalSeeder.AnimalCultureDescriptionForTesting));
		StringAssert.Contains(AnimalSeeder.AnimalCultureDescriptionForTesting, "instinct, territory, routine");
	}

	[TestMethod]
	public void AttackLoadoutsForTesting_NewAnimalGroups_HaveExpectedAttacks()
	{
		var birdLoadout = AnimalSeeder.AttackLoadoutsForTesting["bird-small"];
		CollectionAssert.AreEquivalent(
			new[] { "beakpeck", "talonstrike", "beakbite" },
			birdLoadout.ShapeMatchedAttacks.Select(x => x.AttackKey).ToArray(),
			"Small birds should keep both free-fighting and clinch-capable avian attacks.");

		var insectLoadout = AnimalSeeder.AttackLoadoutsForTesting["insect-stinger"];
		Assert.IsTrue(insectLoadout.ShapeMatchedAttacks.Any(x => x.AttackKey == "mandiblebite"),
			"Stinging insects should still have a mundane mandible attack.");
		Assert.IsTrue(insectLoadout.ShapeMatchedAttacks.Any(x => x.AttackKey == "headram"),
			"Stinging insects should now have a non-clinch fallback attack.");
		Assert.IsTrue(insectLoadout.VenomAttacks?.Any(x => x.VenomProfileKey == "irritant") == true,
			"Stinging insects should seed irritant venom.");

		var whaleLoadout = AnimalSeeder.AttackLoadoutsForTesting["baleen-whale"];
		Assert.IsTrue(whaleLoadout.ShapeMatchedAttacks.Any(x => x.AttackKey == "headbutt"),
			"Baleen whales should now have a clinch-capable fallback attack.");
		Assert.IsTrue(whaleLoadout.AliasAttacks?.Any(x => x.AttackKey == "headram") == true,
			"Baleen whales should have a head ram attack.");
		Assert.IsTrue(whaleLoadout.AliasAttacks?.Any(x => x.AttackKey == "tailslap") == true,
			"Baleen whales should have a tail slap attack.");
	}

	[TestMethod]
	public void AttackLoadoutsForTesting_AllStockLoadouts_HaveClinchAndNonClinchCoverage()
	{
		foreach (var (key, loadout) in AnimalSeeder.AttackLoadoutsForTesting)
		{
			var attackKeys = loadout.ShapeMatchedAttacks
				.Select(x => x.AttackKey)
				.Concat(loadout.AliasAttacks?.Select(x => x.AttackKey) ?? Enumerable.Empty<string>())
				.ToList();
			var hasClinch = attackKeys.Any(x => x is "bite" or "carnivoreclinchbite" or "carnivoreclinchhighbite" or
				"carnivoreclinchhighestbite" or "herbivorebite" or "smallbite" or "smalllowbite" or "fishbite" or
				"fishquickbite" or "headbutt" or "beakbite" or "fangbite" or "mandiblebite" or "clawclamp") ||
			                (loadout.VenomAttacks?.Any(x => x.MoveType == BuiltInCombatMoveType.EnvenomingAttackClinch) ?? false);
			var hasNonClinch = attackKeys.Any(x => x is "carnivorebite" or "carnivoresmashbite" or "carnivorelowbite" or
				"carnivorehighbite" or "carnivorelowestbite" or "carnivoredownbite" or "herbivoresmashbite" or
				"smallsmashbite" or "smalldownedbite" or "clawswipe" or "clawsmashswipe" or "clawlowswipe" or
				"clawhighswipe" or "hoofstomp" or "hoofstompsmash" or "barge" or "bargesmash" or "clinchbarge" or
				"gorehorn" or "goreantler" or "goretusk" or "tusksweep" or "crabpinch" or "sharkbite" or
				"sharkreelbite" or "beakpeck" or "talonstrike" or "arachnidclaw" or "headram" or "tailslap" or
				"tendrillash") ||
			               (loadout.VenomAttacks?.Any(x => x.MoveType != BuiltInCombatMoveType.EnvenomingAttackClinch) ?? false);

			Assert.IsTrue(hasClinch, $"Attack loadout {key} should include a clinch-capable attack.");
			Assert.IsTrue(hasNonClinch, $"Attack loadout {key} should include a non-clinch attack.");
		}
	}

	[TestMethod]
	public void BodyAuditProfilesForTesting_NewFamilies_HaveExpectedCoverageRules()
	{
		var avian = AnimalSeeder.BodyAuditProfilesForTesting["avian"];
		CollectionAssert.Contains(avian.RequiredBones.ToList(), "rhumerus");
		CollectionAssert.Contains(avian.RequiredLimbs.ToList(), "Right Wing");

		var scorpion = AnimalSeeder.BodyAuditProfilesForTesting["scorpion"];
		CollectionAssert.Contains(scorpion.RequiredBodyparts.ToList(), "stinger");
		Assert.AreEqual(AnimalSeeder.AnimalBoneExpectation.Forbidden, scorpion.BoneExpectation,
			"Scorpions should not require bones.");

		var cephalopod = AnimalSeeder.BodyAuditProfilesForTesting["cephalopod"];
		CollectionAssert.Contains(cephalopod.RequiredBodyparts.ToList(), "arm8");
		Assert.AreEqual(AnimalSeeder.AnimalBoneExpectation.Forbidden, cephalopod.BoneExpectation,
			"Cephalopods should not require bones.");

		var serpent = AnimalSeeder.BodyAuditProfilesForTesting["serpent"];
		CollectionAssert.Contains(serpent.RequiredBones.ToList(), "cavertebrae");
		CollectionAssert.Contains(serpent.RequiredLimbs.ToList(), "Tail");

		var fish = AnimalSeeder.BodyAuditProfilesForTesting["fish"];
		CollectionAssert.Contains(fish.RequiredBodyparts.ToList(), "rgill");
		CollectionAssert.Contains(fish.RequiredBones.ToList(), "cavertebrae");
	}

	[TestMethod]
	public void BuildAuditPartLookup_DuplicateInheritedAliases_PrefersMostSpecificBody()
	{
		var parts = new List<BodypartProto>
		{
			new() { BodyId = 2, Name = "carapace", Description = "base carapace" },
			new() { BodyId = 1, Name = "carapace", Description = "derived carapace" },
			new() { BodyId = 2, Name = "mouth", Description = "base mouth" }
		};

		var lookup = AnimalSeeder.BuildAuditPartLookup(new long[] { 1, 2 }, parts);

		Assert.AreEqual("derived carapace", lookup["carapace"].Description);
		Assert.AreEqual("base mouth", lookup["mouth"].Description);
	}

	[TestMethod]
	public void GetAvianSpinalOrganAliasForLimb_KnownLimbNames_ReturnExpectedMappings()
	{
		Assert.AreEqual("uspinalcord", AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Torso"));
		Assert.AreEqual("mspinalcord", AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Right Wing"));
		Assert.AreEqual("mspinalcord", AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Left Wing"));
		Assert.AreEqual("lspinalcord", AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Left Leg"));
		Assert.AreEqual("lspinalcord", AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Tail"));
		Assert.IsNull(AnimalSeeder.GetAvianSpinalOrganAliasForLimb("Unknown Limb"));
	}

	[TestMethod]
	public void VenomProfilesForTesting_SeededProfiles_HaveExpectedEffects()
	{
		var neurotoxic = AnimalSeeder.VenomProfilesForTesting["neurotoxic"];
		Assert.IsTrue(neurotoxic.Effects.Any(x => x.DrugType == DrugType.Paralysis),
			"Neurotoxic venom should inflict paralysis.");

		var mixed = AnimalSeeder.VenomProfilesForTesting["mixed"];
		Assert.IsTrue(mixed.Effects.Any(x => x.DrugType == DrugType.BodypartDamage),
			"Mixed venom should include organ or bodypart damage.");

		foreach (var (_, profile) in AnimalSeeder.VenomProfilesForTesting)
		{
			Assert.IsTrue(profile.Effects.Count > 0, $"Venom profile {profile.Key} should have at least one effect.");
		}
	}
}

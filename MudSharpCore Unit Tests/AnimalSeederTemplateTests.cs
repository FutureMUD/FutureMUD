using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Health;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AnimalSeederTemplateTests
{
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
	public void AttackLoadoutsForTesting_NewAnimalGroups_HaveExpectedAttacks()
	{
		var birdLoadout = AnimalSeeder.AttackLoadoutsForTesting["bird-small"];
		CollectionAssert.AreEquivalent(
			new[] { "beakpeck", "talonstrike" },
			birdLoadout.ShapeMatchedAttacks.Select(x => x.AttackKey).ToArray(),
			"Small birds should peck and strike with talons.");

		var insectLoadout = AnimalSeeder.AttackLoadoutsForTesting["insect-stinger"];
		Assert.IsTrue(insectLoadout.ShapeMatchedAttacks.Any(x => x.AttackKey == "mandiblebite"),
			"Stinging insects should still have a mundane mandible attack.");
		Assert.IsTrue(insectLoadout.VenomAttacks?.Any(x => x.VenomProfileKey == "irritant") == true,
			"Stinging insects should seed irritant venom.");

		var whaleLoadout = AnimalSeeder.AttackLoadoutsForTesting["baleen-whale"];
		Assert.IsTrue(whaleLoadout.AliasAttacks?.Any(x => x.AttackKey == "headram") == true,
			"Baleen whales should have a head ram attack.");
		Assert.IsTrue(whaleLoadout.AliasAttacks?.Any(x => x.AttackKey == "tailslap") == true,
			"Baleen whales should have a tail slap attack.");
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

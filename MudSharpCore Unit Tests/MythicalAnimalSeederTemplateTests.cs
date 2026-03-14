using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MythicalAnimalSeederTemplateTests
{
	[TestMethod]
	public void ValidateTemplateCatalogForTesting_CurrentCatalog_HasNoIssues()
	{
		var issues = MythicalAnimalSeeder.ValidateTemplateCatalogForTesting();
		Assert.AreEqual(0, issues.Count, string.Join("\n", issues));
	}

	[TestMethod]
	public void BuildBodypartAliasLookup_DuplicateAliases_GroupsAndOrdersDeterministically()
	{
		var parts = new[]
		{
			new BodypartProto { Id = 3, Name = "tail", DisplayOrder = 20 },
			new BodypartProto { Id = 2, Name = "tail", DisplayOrder = 10 },
			new BodypartProto { Id = 1, Name = "tail", DisplayOrder = 10 },
			new BodypartProto { Id = 4, Name = "head", DisplayOrder = 5 }
		};

		var groupedLookup = SeederBodyUtilities.BuildBodypartAliasLookup(parts);
		CollectionAssert.AreEqual(new long[] { 1, 2, 3 }, groupedLookup["tail"].Select(x => x.Id).ToArray(),
			"Duplicate aliases should be retained in stable display-order then id order.");

		var lookup = SeederBodyUtilities.BuildBodypartLookup(parts);
		Assert.AreEqual(1L, lookup["tail"].Id,
			"Single-part lookups should resolve duplicate aliases to the earliest stable entry.");
	}

	[TestMethod]
	public void TemplatesForTesting_KeyRaces_UseExpectedBodyAssignments()
	{
		Assert.AreEqual("Toed Quadruped", MythicalAnimalSeeder.TemplatesForTesting["Dragon"].BodyKey,
			"Dragons should reuse the toed quadruped base body.");
		Assert.AreEqual("Eastern Dragon", MythicalAnimalSeeder.TemplatesForTesting["Eastern Dragon"].BodyKey,
			"Eastern dragons should use their dedicated wingless dragon body.");
		Assert.AreEqual("Griffin", MythicalAnimalSeeder.TemplatesForTesting["Griffin"].BodyKey,
			"Griffins should use their dedicated hybrid body.");
		Assert.AreEqual("Mermaid", MythicalAnimalSeeder.TemplatesForTesting["Mermaid"].BodyKey,
			"Merfolk should use the humanoid-piscine hybrid body.");
		Assert.AreEqual("Winged Humanoid", MythicalAnimalSeeder.TemplatesForTesting["Owlkin"].BodyKey,
			"Owlkin should use the shared winged humanoid body.");
		Assert.AreEqual("Centaur", MythicalAnimalSeeder.TemplatesForTesting["Centaur"].BodyKey,
			"Centaurs should use the dedicated centaur hybrid body.");
		Assert.AreEqual("Organic Humanoid", MythicalAnimalSeeder.TemplatesForTesting["Myconid"].BodyKey,
			"Myconids should share the stock humanoid body for equipment and surgery compatibility.");
		Assert.AreEqual("Ungulate", MythicalAnimalSeeder.TemplatesForTesting["Pegacorn"].BodyKey,
			"Pegacorns should reuse the ungulate body that already supports horns and wings.");
	}

	[TestMethod]
	public void TemplatesForTesting_HumanoidVarietyRaces_MatchExpectedCatalogue()
	{
		var humanoidVarietyRaces = MythicalAnimalSeeder.TemplatesForTesting
			.Where(x => x.Value.HumanoidVariety)
			.Select(x => x.Key)
			.ToArray();

		CollectionAssert.AreEquivalent(
			new[]
			{
				"Minotaur",
				"Naga",
				"Mermaid",
				"Selkie",
				"Owlkin",
				"Avian Person",
				"Centaur"
			},
			humanoidVarietyRaces,
			"The humanoid-form catalogue should cover the races expected to reuse human-style variation.");
		Assert.IsTrue(
			MythicalAnimalSeeder.TemplatesForTesting
				.Where(x => x.Value.HumanoidVariety)
				.All(x => x.Value.CanUseWeapons),
			"Humanoid variety races should continue to support weapon use.");
	}

	[TestMethod]
	public void TemplatesForTesting_LegacyFantasyRaces_KeepExpectedSapienceAndCharacteristics()
	{
		var myconid = MythicalAnimalSeeder.TemplatesForTesting["Myconid"];
		Assert.IsFalse(myconid.HumanoidVariety,
			"Myconids should not inherit the full human characteristic matrix.");
		Assert.IsTrue(myconid.CanUseWeapons,
			"Myconids should remain tool-using humanoid-bodied races.");
		Assert.IsTrue(
			myconid.AdditionalCharacteristics?.Any(x => x.DefinitionName == "Fungus Colour") == true,
			"Myconids should keep their legacy fungus colour characteristic.");

		var dragon = MythicalAnimalSeeder.TemplatesForTesting["Dragon"];
		Assert.IsTrue(
			dragon.AdditionalCharacteristics?.Any(x => x.DefinitionName == "Scale Colour") == true,
			"Dragons should retain their scale colour characteristic.");
	}

	[TestMethod]
	public void TemplatesForTesting_SignatureUsagesAndAttacks_ArePresentForMythicSpecialCases()
	{
		var unicorn = MythicalAnimalSeeder.TemplatesForTesting["Unicorn"];
		Assert.IsTrue(
			unicorn.BodypartUsages?.Any(x => x.BodypartAlias == "horn" && x.Usage == "general") == true,
			"Unicorns should expose their horn as a general-purpose additional bodypart.");
		CollectionAssert.AreEquivalent(
			new[] { "Hoof Stomp", "Horn Gore" },
			unicorn.Attacks.Select(x => x.AttackName).ToArray(),
			"Unicorns should stomp and gore.");

		var pegasus = MythicalAnimalSeeder.TemplatesForTesting["Pegasus"];
		CollectionAssert.AreEquivalent(
			new[] { "rwingbase", "lwingbase", "rwing", "lwing" },
			pegasus.BodypartUsages!.Select(x => x.BodypartAlias).ToArray(),
			"Pegasi should expose both wing roots and both wings.");

		var pegacorn = MythicalAnimalSeeder.TemplatesForTesting["Pegacorn"];
		CollectionAssert.AreEquivalent(
			new[] { "Hoof Stomp", "Horn Gore" },
			pegacorn.Attacks.Select(x => x.AttackName).ToArray(),
			"Pegacorns should preserve the combined pegasus and unicorn attack profile.");

		var phoenix = MythicalAnimalSeeder.TemplatesForTesting["Phoenix"];
		CollectionAssert.AreEquivalent(
			new[] { "Beak Peck", "Talon Strike" },
			phoenix.Attacks.Select(x => x.AttackName).ToArray(),
			"Phoenixes should keep the avian peck and talon loadout.");
	}
}

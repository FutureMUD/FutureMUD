#nullable enable

using DatabaseSeeder.Seeders;
using DatabaseSeeder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Planes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SupernaturalSeederTemplateTests
{
	private static bool PlanarElementContains(XElement? element, long planeId)
	{
		return element?
			.Elements("Plane")
			.Any(x => long.Parse(x.Attribute("id")?.Value ?? "0") == planeId) == true;
	}

	[TestMethod]
	public void ValidateTemplateCatalogForTesting_CurrentCatalog_HasNoIssues()
	{
		IReadOnlyList<string> issues = SupernaturalSeeder.ValidateTemplateCatalogForTesting();
		Assert.AreEqual(0, issues.Count, string.Join("\n", issues));
	}

	[TestMethod]
	public void SeederQuestions_ReusesSharedNonHumanQuestionAnswers()
	{
		Dictionary<string, SeederQuestion> questions = ((IDatabaseSeeder)new SupernaturalSeeder()).Questions
			.ToDictionary(x => x.Id, StringComparer.OrdinalIgnoreCase);

		Assert.AreEqual("nonhuman-health-model", questions["model"].SharedAnswerKey);
		Assert.AreEqual("damage-randomness", questions["random"].SharedAnswerKey);
		Assert.AreEqual("combat-message-style", questions["messagestyle"].SharedAnswerKey);
		Assert.IsTrue(questions.Values.All(x => x.AutoReuseLastAnswer));
	}

	[TestMethod]
	public void TemplatesForTesting_Catalogue_HasFortyFiveEntries()
	{
		Assert.AreEqual(45, SupernaturalSeeder.TemplatesForTesting.Count,
			"The supernatural catalogue should match the approved V1 scope.");
	}

	[TestMethod]
	public void TemplatesForTesting_AngelsFollowMaimonidesOrder()
	{
		CollectionAssert.AreEqual(
			new[]
			{
				"Chayot HaKodesh",
				"Ophanim",
				"Erelim",
				"Hashmallim",
				"Seraphim",
				"Malakhim",
				"Elohim",
				"Bene Elohim",
				"Cherubim",
				"Ishim"
			},
			SupernaturalSeeder.AngelicRankOrderForTesting.ToArray());

		CollectionAssert.IsSubsetOf(
			SupernaturalSeeder.AngelicRankOrderForTesting.ToArray(),
			SupernaturalSeeder.TemplatesForTesting.Keys.ToArray());
	}

	[TestMethod]
	public void TemplatesForTesting_DemonsMirrorFallenRanksAndIncludeCommonTypes()
	{
		foreach (string angelRank in SupernaturalSeeder.AngelicRankOrderForTesting)
		{
			string fallenName = $"Fallen {angelRank}";
			Assert.IsTrue(SupernaturalSeeder.TemplatesForTesting.ContainsKey(fallenName),
				$"Missing fallen rank {fallenName}.");
		}

		foreach (string demonName in SupernaturalSeeder.CommonDemonNamesForTesting)
		{
			Assert.IsTrue(SupernaturalSeeder.TemplatesForTesting.ContainsKey(demonName),
				$"Missing common demon {demonName}.");
		}
	}

	[TestMethod]
	public void TemplatesForTesting_SupportedUndeadSubset_IsIncluded()
	{
		foreach (string undeadName in SupernaturalSeeder.SupportedUndeadNamesForTesting)
		{
			Assert.IsTrue(SupernaturalSeeder.TemplatesForTesting.ContainsKey(undeadName),
				$"Missing supported undead or incorporeal undead entry {undeadName}.");
		}

		Assert.AreEqual("Supernatural Lich Skeleton", SupernaturalSeeder.TemplatesForTesting["Lich"].BodyKey);
		Assert.AreEqual("Supernatural Decayed Undead", SupernaturalSeeder.TemplatesForTesting["Zombie"].BodyKey);
		Assert.AreEqual("Supernatural Spirit Form", SupernaturalSeeder.TemplatesForTesting["Ghost"].BodyKey);
	}

	[TestMethod]
	public void TemplatesForTesting_AllRacesAreBuilderOnlyByDefault()
	{
		Assert.IsTrue(SupernaturalSeeder.TemplatesForTesting.Values.All(x => !x.PlayableDefault),
			"Supernatural races should be hidden from normal chargen until builders opt in.");
	}

	[TestMethod]
	public void TemplatesForTesting_RepresentativeBodyAssignments_MatchSupernaturalForms()
	{
		Assert.AreEqual("Supernatural Ophanic Wheel", SupernaturalSeeder.TemplatesForTesting["Ophanim"].BodyKey);
		Assert.AreEqual("Supernatural Many-Winged Angel", SupernaturalSeeder.TemplatesForTesting["Seraphim"].BodyKey);
		Assert.AreEqual("Supernatural Horned Fiend", SupernaturalSeeder.TemplatesForTesting["Fiend"].BodyKey);
		Assert.AreEqual("Supernatural Familiar", SupernaturalSeeder.TemplatesForTesting["Familiar"].BodyKey);
		Assert.AreEqual("Supernatural Werewolf Hybrid", SupernaturalSeeder.TemplatesForTesting["Werewolf Hybrid"].BodyKey);
		Assert.AreEqual("Supernatural Hellhound", SupernaturalSeeder.TemplatesForTesting["Hellhound"].BodyKey);
	}

	[TestMethod]
	public void TemplatesForTesting_NeedsProfiles_MatchSupportedMechanics()
	{
		string[] living = SupernaturalSeeder.TemplatesForTesting
			.Where(x => x.Value.NeedsProfile == SupernaturalSeeder.SupernaturalNeedsProfile.Living)
			.Select(x => x.Key)
			.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
			.ToArray();

		CollectionAssert.AreEquivalent(new[] { "Werewolf", "Werewolf Hybrid" }, living);
		Assert.IsTrue(SupernaturalSeeder.TemplatesForTesting.Values
			.Where(x => x.Family is not SupernaturalSeeder.SupernaturalFamily.Therianthrope)
			.All(x => x.NeedsProfile == SupernaturalSeeder.SupernaturalNeedsProfile.NonLiving));
	}

	[TestMethod]
	public void TemplatesForTesting_SupernaturalCharacteristics_ArePresentAndBroad()
	{
		string[] expectedDefinitions =
		[
			"Halo Radiance",
			"Wing Aspect",
			"Eye Motif",
			"Infernal Mark",
			"Horn Style",
			"Tail Form",
			"Death Mark",
			"Spirit Manifestation",
			"Corpse Condition",
			"Bestial Transformation Tell"
		];

		string[] actualDefinitions = SupernaturalSeeder.TemplatesForTesting.Values
			.SelectMany(x => x.Characteristics)
			.Select(x => x.DefinitionName)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();

		CollectionAssert.IsSubsetOf(expectedDefinitions, actualDefinitions);
		Assert.IsTrue(SupernaturalSeeder.TemplatesForTesting.Values.All(x => x.DescriptionVariants.Count >= 2));
	}

	[TestMethod]
	public void TemplatesForTesting_AttackKeys_CoverSupernaturalNaturalAttacks()
	{
		string[] expectedAttacks =
		[
			"Radiant Touch",
			"Radiant Gaze",
			"Wing Buffet",
			"Infernal Claw",
			"Horn Gore",
			"Fanged Bite",
			"Soul Chill",
			"Grave Claw",
			"Wheel Crush",
			"Spectral Touch",
			"Heavenly Choir",
			"Canticle of Awe",
			"Trumpet Peal",
			"Word of Command",
			"Crown of Stars",
			"Starfire Breath",
			"Seraphic Wingstorm",
			"Mercy-Searing Grasp",
			"Wheel of Judgment",
			"Many-Eyed Ray",
			"Hellfire Breath",
			"Brimstone Spit",
			"Infernal Trip",
			"Damnation Barge",
			"Hellish Headbutt",
			"Soul Hook",
			"Abyssal Chain Lash",
			"Sinner's Clinch",
			"Barbed Tail Slap",
			"Fallen Choir",
			"Wailing Dirge",
			"Grave Drag",
			"Grasp of the Dead",
			"Bone Rattle",
			"Crypt Dust Breath",
			"Deathly Pall",
			"Raking Maul",
			"Hamstring Snap",
			"Crushing Pounce",
			"Wolf Trip"
		];

		string[] actualAttacks = SupernaturalSeeder.SupernaturalAttackNamesForTesting
			.ToArray();

		CollectionAssert.IsSubsetOf(expectedAttacks, actualAttacks);
		Assert.AreEqual(40, actualAttacks.Length);
		Assert.IsTrue(actualAttacks.Length >= 30);
		Assert.AreEqual(expectedAttacks.Length, actualAttacks.Length);
	}

	[TestMethod]
	public void AttackDefinitionsForTesting_CategoriesCoverSpecialMoveFamilies()
	{
		string[] expectedCategories =
		[
			"sonic",
			"ranged",
			"breath",
			"spit",
			"trip",
			"unbalance",
			"stagger",
			"pushback",
			"clinch",
			"forced movement",
			"buffeting",
			"bite",
			"claw",
			"horn",
			"radiant",
			"infernal",
			"spirit",
			"undead",
			"therianthrope"
		];

		string[] actualCategories = SupernaturalSeeder.SupernaturalAttackDefinitionsForTesting.Values
			.SelectMany(x => x.Categories)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();

		CollectionAssert.IsSubsetOf(expectedCategories, actualCategories);
	}

	[TestMethod]
	public void AttackDefinitionsForTesting_SonicAttacksUseScreechAndMouthAliases()
	{
		string[] sonicAttackNames = SupernaturalSeeder.SupernaturalAttackDefinitionsForTesting
			.Where(x => x.Value.Categories.Contains("sonic", StringComparer.OrdinalIgnoreCase))
			.Select(x => x.Key)
			.ToArray();

		Assert.IsTrue(sonicAttackNames.Contains("Heavenly Choir", StringComparer.OrdinalIgnoreCase));
		Assert.IsTrue(sonicAttackNames.Contains("Canticle of Awe", StringComparer.OrdinalIgnoreCase));
		Assert.IsTrue(sonicAttackNames.Contains("Word of Command", StringComparer.OrdinalIgnoreCase));

		foreach (string attackName in sonicAttackNames)
		{
			SupernaturalSeeder.SupernaturalAttackDefinition definition =
				SupernaturalSeeder.SupernaturalAttackDefinitionsForTesting[attackName];

			Assert.AreEqual(BuiltInCombatMoveType.ScreechAttack, definition.MoveTypeOverride);
			Assert.AreEqual("Ear", definition.FixedTargetBodypartShape);
			Assert.IsFalse(definition.Message.Contains("$1", StringComparison.Ordinal),
				$"{attackName} should not reference a single defender.");
		}

		foreach (SupernaturalSeeder.SupernaturalAttackTemplate templateAttack in SupernaturalSeeder.TemplatesForTesting.Values
			         .SelectMany(x => x.Attacks)
			         .Where(x => sonicAttackNames.Contains(x.AttackName, StringComparer.OrdinalIgnoreCase)))
		{
			CollectionAssert.AreEqual(new[] { "mouth" }, templateAttack.BodypartAliases.ToArray(),
				$"{templateAttack.AttackName} should be voiced from the mouth.");
		}
	}

	[TestMethod]
	public void TemplatesForTesting_AngelsUseChoirsAndOphanimUseWheelAttacks()
	{
		foreach (SupernaturalSeeder.SupernaturalRaceTemplate template in SupernaturalSeeder.TemplatesForTesting.Values
			         .Where(x => x.Family == SupernaturalSeeder.SupernaturalFamily.Angel))
		{
			Assert.IsTrue(template.Attacks.Any(x => x.AttackName == "Heavenly Choir"),
				$"{template.Name} should have a heavenly choir attack.");
		}

		SupernaturalSeeder.SupernaturalRaceTemplate ophanim = SupernaturalSeeder.TemplatesForTesting["Ophanim"];
		Assert.IsTrue(ophanim.Attacks.Any(x => x.AttackName == "Wheel of Judgment"));
		Assert.IsTrue(ophanim.Attacks.Any(x => x.AttackName == "Many-Eyed Ray"));
		Assert.IsFalse(ophanim.Attacks.Any(x => x.AttackName == "Seraphic Wingstorm"));
	}

	[TestMethod]
	public void TemplatesForTesting_FallenRanksMirrorAngelicAttackExpansion()
	{
		foreach (string angelRank in SupernaturalSeeder.AngelicRankOrderForTesting)
		{
			string fallenName = $"Fallen {angelRank}";
			SupernaturalSeeder.SupernaturalRaceTemplate template = SupernaturalSeeder.TemplatesForTesting[fallenName];

			Assert.IsTrue(template.Attacks.Any(x => x.AttackName == "Fallen Choir"),
				$"{fallenName} should have a fallen choir attack.");
		}

		SupernaturalSeeder.SupernaturalRaceTemplate fallenOphanim = SupernaturalSeeder.TemplatesForTesting["Fallen Ophanim"];
		Assert.IsTrue(fallenOphanim.Attacks.Any(x => x.AttackName == "Wheel of Judgment"));
		Assert.IsTrue(fallenOphanim.Attacks.Any(x => x.AttackName == "Many-Eyed Ray"));
		Assert.IsFalse(fallenOphanim.Attacks.Any(x => x.AttackName == "Barbed Tail Slap"));
	}

	[TestMethod]
	public void TemplatesForTesting_CommonDemonsUndeadSpiritsAndWerewolvesHaveBroaderAttackCoverage()
	{
		foreach (string demonName in SupernaturalSeeder.CommonDemonNamesForTesting)
		{
			Assert.IsTrue(SupernaturalSeeder.TemplatesForTesting[demonName].Attacks.Count >= 8,
				$"{demonName} should have expanded demon attack coverage.");
		}

		foreach (string undeadName in SupernaturalSeeder.SupportedUndeadNamesForTesting
			         .Where(x => SupernaturalSeeder.TemplatesForTesting[x].Family == SupernaturalSeeder.SupernaturalFamily.Undead))
		{
			Assert.IsTrue(SupernaturalSeeder.TemplatesForTesting[undeadName].Attacks.Count >= 7,
				$"{undeadName} should have expanded undead attack coverage.");
		}

		Assert.IsTrue(SupernaturalSeeder.TemplatesForTesting["Ghost"].Attacks.Count >= 5);
		Assert.IsTrue(SupernaturalSeeder.TemplatesForTesting["Werewolf"].Attacks.Count >= 4);
		Assert.IsTrue(SupernaturalSeeder.TemplatesForTesting["Werewolf Hybrid"].Attacks.Any(x => x.AttackName == "Raking Maul"));
	}

	[TestMethod]
	public void BodyAdditionsForTesting_HornedFiendsAndFamiliarsExposeTailAliases()
	{
		string[] expectedTailAliases = ["utail", "mtail", "ltail"];

		CollectionAssert.AreEquivalent(expectedTailAliases,
			SupernaturalSeeder.SupernaturalBodyAdditionalAliasesForTesting["Supernatural Horned Fiend"]);
		CollectionAssert.AreEquivalent(expectedTailAliases,
			SupernaturalSeeder.SupernaturalBodyAdditionalAliasesForTesting["Supernatural Familiar"]);

		foreach (string demonName in new[] { "Fiend", "Imp", "Familiar", "Incubus", "Succubus" })
		{
			Assert.IsTrue(SupernaturalSeeder.TemplatesForTesting[demonName].Attacks.Any(x =>
					x.AttackName == "Barbed Tail Slap" &&
					expectedTailAliases.All(alias => x.BodypartAliases.Contains(alias, StringComparer.OrdinalIgnoreCase))),
				$"{demonName} should use the seeded tail aliases for Barbed Tail Slap.");
		}
	}

	[TestMethod]
	public void PlanarProfileXml_IncorporealProfile_IsVisibleButNotPhysicalOnPrime()
	{
		XElement root = SupernaturalSeeder.BuildPlanarProfileXmlForTesting(1, 2,
			SupernaturalSeeder.SupernaturalPlanarProfile.Incorporeal);

		Assert.IsTrue(PlanarElementContains(root.Element("Presence"), 2));
		Assert.IsTrue(PlanarElementContains(root.Element("VisibleTo"), 1));
		Assert.IsTrue(bool.Parse(root.Element("Flags")!.Attribute("suspendsPhysicalContact")!.Value));

		XElement physical = root.Element("Interactions")!
			.Elements("Interaction")
			.Single(x => x.Attribute("kind")?.Value == PlanarInteractionKind.Physical.ToString());
		Assert.IsFalse(PlanarElementContains(physical, 1));
		Assert.IsFalse(PlanarElementContains(physical, 2));
	}

	[TestMethod]
	public void PlanarProfileXml_DualNaturedProfile_IsPresentAndPhysicalOnBothPlanes()
	{
		XElement root = SupernaturalSeeder.BuildPlanarProfileXmlForTesting(1, 2,
			SupernaturalSeeder.SupernaturalPlanarProfile.DualNatured);

		Assert.IsTrue(PlanarElementContains(root.Element("Presence"), 1));
		Assert.IsTrue(PlanarElementContains(root.Element("Presence"), 2));
		XElement physical = root.Element("Interactions")!
			.Elements("Interaction")
			.Single(x => x.Attribute("kind")?.Value == PlanarInteractionKind.Physical.ToString());
		Assert.IsTrue(PlanarElementContains(physical, 1));
		Assert.IsTrue(PlanarElementContains(physical, 2));
	}

	[TestMethod]
	public void AdditionalBodyFormMeritXml_DefaultsToSafeBuilderControlledForms()
	{
		SupernaturalSeeder.SupernaturalFormTemplate template =
			SupernaturalSeeder.FormTemplatesForTesting.Single(x => x.MeritName == "Supernatural Werewolf Hybrid Form");

		XElement root = SupernaturalSeeder.BuildAdditionalBodyFormMeritDefinitionForTesting(
			template,
			42,
			1,
			2);

		Assert.AreEqual("42", root.Element("Race")!.Value);
		Assert.AreEqual("hybrid", root.Element("Alias")!.Value);
		Assert.AreEqual(((int)BodySwitchTraumaMode.Automatic).ToString(), root.Element("TraumaMode")!.Value);
		Assert.IsTrue(bool.Parse(root.Element("AllowVoluntarySwitch")!.Value));
		Assert.IsFalse(bool.Parse(root.Element("AutoTransformWhenApplicable")!.Value));
		Assert.AreEqual("2", root.Element("ChargenAvailableProg")!.Value);
	}
}

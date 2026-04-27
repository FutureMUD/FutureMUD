#nullable enable

using DatabaseSeeder.Seeders;
using DatabaseSeeder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Character;
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
			"Spectral Touch"
		];

		string[] actualAttacks = SupernaturalSeeder.TemplatesForTesting.Values
			.SelectMany(x => x.Attacks)
			.Select(x => x.AttackName)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToArray();

		CollectionAssert.IsSubsetOf(expectedAttacks, actualAttacks);
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

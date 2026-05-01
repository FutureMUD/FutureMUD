#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;
using System.Linq;
using TraitOwnerScope = MudSharp.Body.Traits.TraitOwnerScope;
using TraitType = MudSharp.Body.Traits.TraitType;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HumanSeederAdminAvatarLanguageTests
{
	[TestMethod]
	public void TraitDefinitionStoresOnCharacterForTesting_LegacySkillDefinition_IsCharacterScoped()
	{
		TraitDefinition legacySkill = new()
		{
			Type = (int)TraitType.Skill,
			OwnerScope = (int)TraitOwnerScope.Body
		};
		TraitDefinition bodyAttribute = new()
		{
			Type = (int)TraitType.Attribute,
			OwnerScope = (int)TraitOwnerScope.Body
		};

		Assert.IsTrue(HumanSeeder.TraitDefinitionStoresOnCharacterForTesting(legacySkill));
		Assert.IsFalse(HumanSeeder.TraitDefinitionStoresOnCharacterForTesting(bodyAttribute));
	}

	[TestMethod]
	public void AddAdminAvatarLanguageKnowledgeForTesting_SkillLanguage_AddsCharacterTrait()
	{
		(Character character, Body body, Language language, Accent accent, TraitDefinition adminSpeechTrait) =
			BuildAdminAvatarLanguageFixture();

		HumanSeeder.AddAdminAvatarLanguageKnowledgeForTesting(character, language);

		Assert.AreEqual(1, character.CharacterTraits.Count);
		Assert.AreEqual(0, body.Traits.Count);
		CharacterTrait trait = character.CharacterTraits.Single();
		Assert.AreSame(character, trait.Character);
		Assert.AreSame(adminSpeechTrait, trait.TraitDefinition);
		Assert.AreEqual(200.0, trait.Value);
		Assert.AreEqual(1, character.CharactersLanguages.Count);
		Assert.AreEqual(1, character.CharactersAccents.Count);
		Assert.AreSame(language, character.CurrentLanguage);
		Assert.AreSame(accent, character.CurrentAccent);
	}

	[TestMethod]
	public void AdminAvatarNeedsLanguageTraitRepairForTesting_StaleBodySkillTrait_IsRepairableMismatch()
	{
		(Character character, Body body, Language language, _, TraitDefinition adminSpeechTrait) =
			BuildAdminAvatarLanguageFixture();
		character.CharactersLanguages.Add(new CharactersLanguages { Character = character, Language = language });
		body.Traits.Add(new Trait
		{
			Body = body,
			TraitDefinition = adminSpeechTrait,
			TraitDefinitionId = adminSpeechTrait.Id,
			Value = 200
		});

		Assert.IsTrue(HumanSeeder.AdminAvatarNeedsLanguageTraitRepairForTesting(character));

		character.CharacterTraits.Add(new CharacterTrait
		{
			Character = character,
			TraitDefinition = adminSpeechTrait,
			TraitDefinitionId = adminSpeechTrait.Id,
			Value = 200
		});

		Assert.IsTrue(HumanSeeder.AdminAvatarNeedsLanguageTraitRepairForTesting(character),
			"Even with the character trait present, the stale body trait should still be removed.");

		body.Traits.Clear();

		Assert.IsFalse(HumanSeeder.AdminAvatarNeedsLanguageTraitRepairForTesting(character));
	}

	private static (Character Character, Body Body, Language Language, Accent Accent, TraitDefinition Trait)
		BuildAdminAvatarLanguageFixture()
	{
		TraitDefinition adminSpeechTrait = new()
		{
			Id = 42,
			Name = "Admin Speech",
			Type = (int)TraitType.Skill,
			OwnerScope = (int)TraitOwnerScope.Body
		};
		Language language = new()
		{
			Id = 12,
			Name = "Admin Speech",
			LinkedTrait = adminSpeechTrait,
			LinkedTraitId = adminSpeechTrait.Id
		};
		Accent accent = new()
		{
			Id = 13,
			Name = "foreign",
			Language = language,
			LanguageId = language.Id
		};
		language.DefaultLearnerAccent = accent;
		language.DefaultLearnerAccentId = accent.Id;
		Body body = new();
		Character character = new()
		{
			Body = body
		};
		return (character, body, language, accent, adminSpeechTrait);
	}
}

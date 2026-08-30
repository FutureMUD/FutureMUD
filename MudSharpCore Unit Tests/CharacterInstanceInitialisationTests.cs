#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Needs;
using MudSharp.Body.Traits;
using MudSharp.CharacterCreation;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CharacterClass = MudSharp.Character.Character;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CharacterInstanceInitialisationTests
{
	[TestMethod]
	public void ResolveSecondaryNeedsModel_IdentityHasNeedsModel_ReusesIdentityModel()
	{
		var identity = TestObjectFactory.CreateUninitialized<CharacterClass>();
		var needs = new Mock<INeedsModel>();
		SetNeedsModel(identity, needs.Object);

		var result = CharacterClass.ResolveSecondaryNeedsModel(identity);

		Assert.AreSame(needs.Object, result);
	}

	[TestMethod]
	public void ResolveSecondaryNeedsModel_IdentityNotYetLoaded_ReturnsNoNeedsFallback()
	{
		var identity = TestObjectFactory.CreateUninitialized<CharacterClass>();

		var result = CharacterClass.ResolveSecondaryNeedsModel(identity);

		Assert.IsInstanceOfType(result, typeof(NoNeedsModel));
	}

	[TestMethod]
	public void ResolveSignedLanguagesForTemplate_LinkedNpcSkillGrantsSignedLanguageOnce()
	{
		var linkedTrait = new Mock<ITraitDefinition>();
		var unrelatedTrait = new Mock<ITraitDefinition>();
		var auslan = new Mock<ISignedLanguage>();
		auslan.SetupGet(x => x.LinkedTrait).Returns(linkedTrait.Object);
		var repository = new Mock<IUneditableAll<ISignedLanguage>>();
		repository.Setup(x => x.GetEnumerator()).Returns(() => new List<ISignedLanguage> { auslan.Object }.GetEnumerator());
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.SignedLanguages).Returns(repository.Object);
		var template = new Mock<ICharacterTemplate>();
		template.SetupGet(x => x.SkillValues).Returns(
			[(linkedTrait.Object, 40.0), (linkedTrait.Object, 50.0), (unrelatedTrait.Object, 60.0)]);

		var result = CharacterClass.ResolveSignedLanguagesForTemplate(gameworld.Object, template.Object);

		Assert.AreEqual(1, result.Count);
		Assert.AreSame(auslan.Object, result.Single());
	}

	private static void SetNeedsModel(CharacterClass character, INeedsModel needsModel)
	{
		typeof(CharacterClass)
			.GetProperty(nameof(CharacterClass.NeedsModel), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
			.SetValue(character, needsModel);
	}
}

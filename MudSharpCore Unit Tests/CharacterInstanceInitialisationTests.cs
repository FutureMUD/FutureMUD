#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Needs;
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

	private static void SetNeedsModel(CharacterClass character, INeedsModel needsModel)
	{
		typeof(CharacterClass)
			.GetProperty(nameof(CharacterClass.NeedsModel), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
			.SetValue(character, needsModel);
	}
}

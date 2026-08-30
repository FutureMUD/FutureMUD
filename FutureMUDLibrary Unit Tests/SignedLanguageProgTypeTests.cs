#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Communication.Language;
using MudSharp.FutureProg;

namespace FutureMUDLibrary_Unit_Tests.FutureProg;

[TestClass]
public class SignedLanguageProgTypeTests
{
	[TestMethod]
	public void SignedTypes_ParseAndRoundTripStorage()
	{
		Assert.IsTrue(ProgVariableTypeRegistry.TryParse("signedlanguage", out var languageType));
		Assert.AreEqual(ProgVariableTypes.SignedLanguage, languageType);
		Assert.IsTrue(ProgVariableTypeRegistry.TryParse("signedvariety", out var varietyType));
		Assert.AreEqual(ProgVariableTypes.SignedVariety, varietyType);
		Assert.IsTrue(ProgVariableTypeRegistry.TryParse("signedlanguagevariety", out var compatibilityType));
		Assert.AreEqual(ProgVariableTypes.SignedVariety, compatibilityType);
		Assert.AreEqual(ProgVariableTypes.SignedVariety, ProgVariableTypes.SignedLanguageVariety);
		Assert.IsTrue(ProgVariableTypeRegistry.TryParse(varietyType.ToStorageString(), out var roundTripped));
		Assert.AreEqual(ProgVariableTypes.SignedVariety, roundTripped);
	}

	[TestMethod]
	public void SignedTypes_AreFirstClassCollectionItems()
	{
		Assert.IsTrue(ProgVariableTypes.CollectionItem.HasFlag(ProgVariableTypes.SignedLanguage));
		Assert.IsTrue(ProgVariableTypes.CollectionItem.HasFlag(ProgVariableTypes.SignedVariety));
		Assert.AreEqual(ProgTypeKind.SignedLanguage, ProgVariableTypes.SignedLanguage.ExactKind);
		Assert.AreEqual(ProgTypeKind.SignedVariety, ProgVariableTypes.SignedVariety.ExactKind);
		Assert.IsTrue(typeof(IProgVariable).IsAssignableFrom(typeof(ISignedLanguage)));
		Assert.IsTrue(typeof(IProgVariable).IsAssignableFrom(typeof(ISignedLanguageVariety)));
	}
}

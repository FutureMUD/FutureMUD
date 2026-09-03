#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Character.Name;
using MudSharp.FutureProg;

namespace FutureMUDLibrary_Unit_Tests.FutureProg;

[TestClass]
public class NameProgTypeTests
{
	[TestMethod]
	public void NameTypes_ParseAndRoundTripStorage()
	{
		Assert.IsTrue(ProgVariableTypeRegistry.TryParse("nameculture", out var nameCultureType));
		Assert.AreEqual(ProgVariableTypes.NameCulture, nameCultureType);
		Assert.IsTrue(ProgVariableTypeRegistry.TryParse("randomnameprofile", out var randomNameProfileType));
		Assert.AreEqual(ProgVariableTypes.RandomNameProfile, randomNameProfileType);
		Assert.IsTrue(ProgVariableTypeRegistry.TryParse("personalname", out var personalNameType));
		Assert.AreEqual(ProgVariableTypes.PersonalName, personalNameType);

		foreach (var type in new[]
		         {
			         ProgVariableTypes.NameCulture,
			         ProgVariableTypes.RandomNameProfile,
			         ProgVariableTypes.PersonalName
		         })
		{
			Assert.IsTrue(ProgVariableTypeRegistry.TryParse(type.ToStorageString(), out var roundTripped));
			Assert.AreEqual(type, roundTripped);
		}
	}

	[TestMethod]
	public void NameTypes_AreFirstClassCollectionItemsWithCorrectStorageKinds()
	{
		Assert.IsTrue(ProgVariableTypes.CollectionItem.HasFlag(ProgVariableTypes.NameCulture));
		Assert.IsTrue(ProgVariableTypes.CollectionItem.HasFlag(ProgVariableTypes.RandomNameProfile));
		Assert.IsTrue(ProgVariableTypes.CollectionItem.HasFlag(ProgVariableTypes.PersonalName));
		Assert.IsTrue(ProgVariableTypes.ReferenceType.HasFlag(ProgVariableTypes.NameCulture));
		Assert.IsTrue(ProgVariableTypes.ReferenceType.HasFlag(ProgVariableTypes.RandomNameProfile));
		Assert.IsFalse(ProgVariableTypes.ReferenceType.HasFlag(ProgVariableTypes.PersonalName));
		Assert.IsTrue(ProgVariableTypes.ValueType.HasFlag(ProgVariableTypes.PersonalName));
		Assert.IsTrue(ProgVariableTypes.Anything.HasFlag(ProgVariableTypes.PersonalName));

		Assert.AreEqual(ProgTypeKind.NameCulture, ProgVariableTypes.NameCulture.ExactKind);
		Assert.AreEqual(ProgTypeKind.RandomNameProfile, ProgVariableTypes.RandomNameProfile.ExactKind);
		Assert.AreEqual(ProgTypeKind.PersonalName, ProgVariableTypes.PersonalName.ExactKind);
		Assert.IsTrue(typeof(IProgVariable).IsAssignableFrom(typeof(INameCulture)));
		Assert.IsTrue(typeof(IProgVariable).IsAssignableFrom(typeof(IRandomNameProfile)));
		Assert.IsTrue(typeof(IProgVariable).IsAssignableFrom(typeof(IPersonalName)));
	}
}

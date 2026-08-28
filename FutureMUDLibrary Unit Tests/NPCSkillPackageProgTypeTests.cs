#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.FutureProg;

namespace FutureMUDLibrary_Unit_Tests.FutureProg;

[TestClass]
public class NPCSkillPackageProgTypeTests
{
	[TestMethod]
	public void NPCSkillPackageType_ParsesAndRoundTripsStorage()
	{
		Assert.IsTrue(ProgVariableTypeRegistry.TryParse("npcskillpackage", out var packageType));
		Assert.AreEqual(ProgVariableTypes.NPCSkillPackage, packageType);
		Assert.IsTrue(ProgVariableTypeRegistry.TryParse(packageType.ToStorageString(), out var roundTripped));
		Assert.AreEqual(ProgVariableTypes.NPCSkillPackage, roundTripped);
	}

	[TestMethod]
	public void NPCSkillPackageType_IsAvailableInCollections()
	{
		Assert.IsTrue(ProgVariableTypes.CollectionItem.HasFlag(ProgVariableTypes.NPCSkillPackage));
		Assert.AreEqual(ProgTypeKind.NPCSkillPackage, ProgVariableTypes.NPCSkillPackage.ExactKind);
	}
}

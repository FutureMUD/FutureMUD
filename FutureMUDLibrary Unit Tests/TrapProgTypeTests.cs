#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.FutureProg;

namespace FutureMUDLibrary_Unit_Tests.FutureProg;

[TestClass]
public class TrapProgTypeTests
{
	[TestMethod]
	public void TrapType_ParsesAndRoundTripsStorage()
	{
		Assert.IsTrue(ProgVariableTypeRegistry.TryParse("trap", out var trapType));
		Assert.AreEqual(ProgVariableTypes.Trap, trapType);
		Assert.IsTrue(ProgVariableTypeRegistry.TryParse(trapType.ToStorageString(), out var roundTripped));
		Assert.AreEqual(ProgVariableTypes.Trap, roundTripped);
	}

	[TestMethod]
	public void TrapType_IsAvailableInCollections()
	{
		Assert.IsTrue(ProgVariableTypes.CollectionItem.HasFlag(ProgVariableTypes.Trap));
		Assert.AreEqual(ProgTypeKind.Trap, ProgVariableTypes.Trap.ExactKind);
	}
}

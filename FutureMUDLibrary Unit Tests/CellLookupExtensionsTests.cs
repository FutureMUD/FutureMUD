#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoomLookupExtensionsTests
{
	[DataTestMethod]
	[DataRow(null, null), DataRow("", null), DataRow("  \t", null), DataRow("  Mirandola:Gate  ", "Mirandola:Gate")]
	public void Normalisation_TrimsOnlyEdges(string? input, string? expected) =>
		Assert.AreEqual(expected, RoomLookupExtensions.NormaliseUniqueName(input));

	[TestMethod]
	public void Validation_MatchesPrototypePermissivenessAndStorageLimit()
	{
		foreach (var numeric in new[] { "42", "-42", "+42", " 42 " }) Assert.IsFalse(RoomLookupExtensions.IsValidUniqueName(numeric));
		foreach (var key in new[] { "a b", "Église:entrée", "Gate😀", "here", "@1", "#gate", "a at b", "9223372036854775808", new string('x', 255) })
			Assert.IsTrue(RoomLookupExtensions.IsValidUniqueName(key), key);
		Assert.IsFalse(RoomLookupExtensions.IsValidUniqueName(new string('x', 256)));
		Assert.IsTrue(RoomLookupExtensions.IsValidUniqueName(null));
	}

	[TestMethod]
	public void Lookup_IsExactCaseInsensitiveBlankSafeAndRejectsAmbiguity()
	{
		var a = Room(1, "Mirandola:Gate");
		var rooms = new[] { a, Room(2, null), Room(3, "") };
		Assert.AreSame(a, rooms.FindByUniqueName(" mirandola:GATE "));
		Assert.IsNull(rooms.FindByUniqueName("Mirandola"));
		Assert.IsNull(rooms.FindByUniqueName(" "));
		Assert.IsNull(rooms.GetUniqueNameConflict("mirandola:gate", 1));
		Assert.AreSame(a, rooms.GetUniqueNameConflict("mirandola:gate", 2));
		Assert.ThrowsException<InvalidOperationException>(() => new[] { a, Room(4, "mirandola:gate") }.FindByUniqueName("Mirandola:Gate"));
	}

	[TestMethod]
	public void MixedLookup_PreservesNumericPrecedenceAndOriginalFallback()
	{
		var keyed = Room(5, "gate");
		var legacy = Room(7, null);
		var collection = new Mock<IUneditableAll<IRoom>>();
		collection.Setup(x => x.GetEnumerator()).Returns(() => ((IEnumerable<IRoom>)new[] { legacy, keyed }).GetEnumerator());
		collection.Setup(x => x.Get(7)).Returns(legacy);
		collection.Setup(x => x.GetByIdOrName("gate", true)).Returns(legacy);
		collection.Setup(x => x.GetByIdOrName("legacy", false)).Returns(legacy);
		Assert.AreSame(legacy, collection.Object.GetByIdOrUniqueNameOrName("7"));
		Assert.IsNull(collection.Object.GetByIdOrUniqueNameOrName("999"));
		Assert.AreSame(keyed, collection.Object.GetByIdOrUniqueNameOrName("gate"));
		Assert.AreSame(legacy, collection.Object.GetByIdOrUniqueNameOrName("legacy", false));
		collection.Verify(x => x.GetByIdOrName("gate", It.IsAny<bool>()), Times.Never);
	}

	private static IRoom Room(long id, string? key)
	{
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.Id).Returns(id);
		room.SetupGet(x => x.UniqueName).Returns(key);
		return room.Object;
	}
}

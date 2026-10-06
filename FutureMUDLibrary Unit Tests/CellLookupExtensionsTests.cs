#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CellLookupExtensionsTests
{
	[DataTestMethod]
	[DataRow(null, null), DataRow("", null), DataRow("  \t", null), DataRow("  Mirandola:Gate  ", "Mirandola:Gate")]
	public void Normalisation_TrimsOnlyEdges(string? input, string? expected) =>
		Assert.AreEqual(expected, CellLookupExtensions.NormaliseUniqueName(input));

	[TestMethod]
	public void Validation_MatchesPrototypePermissivenessAndStorageLimit()
	{
		foreach (var numeric in new[] { "42", "-42", "+42", " 42 " }) Assert.IsFalse(CellLookupExtensions.IsValidUniqueName(numeric));
		foreach (var key in new[] { "a b", "Église:entrée", "Gate😀", "here", "@1", "#gate", "a at b", "9223372036854775808", new string('x', 255) })
			Assert.IsTrue(CellLookupExtensions.IsValidUniqueName(key), key);
		Assert.IsFalse(CellLookupExtensions.IsValidUniqueName(new string('x', 256)));
		Assert.IsTrue(CellLookupExtensions.IsValidUniqueName(null));
	}

	[TestMethod]
	public void Lookup_IsExactCaseInsensitiveBlankSafeAndRejectsAmbiguity()
	{
		var a = Cell(1, "Mirandola:Gate");
		var cells = new[] { a, Cell(2, null), Cell(3, "") };
		Assert.AreSame(a, cells.FindByUniqueName(" mirandola:GATE "));
		Assert.IsNull(cells.FindByUniqueName("Mirandola"));
		Assert.IsNull(cells.FindByUniqueName(" "));
		Assert.IsNull(cells.GetUniqueNameConflict("mirandola:gate", 1));
		Assert.AreSame(a, cells.GetUniqueNameConflict("mirandola:gate", 2));
		Assert.ThrowsException<InvalidOperationException>(() => new[] { a, Cell(4, "mirandola:gate") }.FindByUniqueName("Mirandola:Gate"));
	}

	[TestMethod]
	public void MixedLookup_PreservesNumericPrecedenceAndOriginalFallback()
	{
		var keyed = Cell(5, "gate");
		var legacy = Cell(7, null);
		var collection = new Mock<IUneditableAll<ICell>>();
		collection.Setup(x => x.GetEnumerator()).Returns(() => ((IEnumerable<ICell>)new[] { legacy, keyed }).GetEnumerator());
		collection.Setup(x => x.Get(7)).Returns(legacy);
		collection.Setup(x => x.GetByIdOrName("gate", true)).Returns(legacy);
		collection.Setup(x => x.GetByIdOrName("legacy", false)).Returns(legacy);
		Assert.AreSame(legacy, collection.Object.GetByIdOrUniqueNameOrName("7"));
		Assert.IsNull(collection.Object.GetByIdOrUniqueNameOrName("999"));
		Assert.AreSame(keyed, collection.Object.GetByIdOrUniqueNameOrName("gate"));
		Assert.AreSame(legacy, collection.Object.GetByIdOrUniqueNameOrName("legacy", false));
		collection.Verify(x => x.GetByIdOrName("gate", It.IsAny<bool>()), Times.Never);
	}

	private static ICell Cell(long id, string? key)
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.UniqueName).Returns(key);
		return cell.Object;
	}
}

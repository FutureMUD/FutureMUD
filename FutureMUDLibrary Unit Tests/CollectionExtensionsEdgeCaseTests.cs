using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.Framework.Revision;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class CollectionExtensionsEdgeCaseTests
{
	[TestMethod]
	public void Sum2_EmptyCollection_ReturnsZeroes()
	{
		var (sum1, sum2) = Array.Empty<int>().Sum2(x => x, x => x * 2);
		Assert.AreEqual(0, sum1);
		Assert.AreEqual(0, sum2);
	}

	[TestMethod]
	public void MinMax_AllPositiveIntegers_ReturnsExtremes()
	{
		var data = new[] { 2, 5, 1 };
		var (min, max) = data.MinMax();
		Assert.AreEqual(1, min);
		Assert.AreEqual(5, max);
	}

	[TestMethod]
	public void MinMax_AllNegativeDoubles_ReturnsExtremes()
	{
		var data = new[] { -1.5, -3.25, -0.5 };
		var (min, max) = data.MinMax();
		Assert.AreEqual(-3.25, min, 1e-6);
		Assert.AreEqual(-0.5, max, 1e-6);
	}

	[TestMethod]
	public void MinMax_PredicateNoMatches_ReturnsDefaults()
	{
		var data = new[] { 1, 2, 3 };
		var (min, max) = data.MinMax(x => x < 0);
		Assert.AreEqual(0, min);
		Assert.AreEqual(0, max);
	}

	[TestMethod]
	public void FirstMax_DefaultValueOverload_WithItems_ReturnsMax()
	{
		var data = new[] { 1, 5, 3 };
		var result = data.FirstMax(x => x, 10);
		Assert.AreEqual(5, result);
	}

	[TestMethod]
	public void FirstMin_DefaultValueOverload_WithItems_ReturnsMin()
	{
		var data = new[] { 1, -2, 3 };
		var result = data.FirstMin(x => x, 0);
		Assert.AreEqual(-2, result);
	}

	[TestMethod]
	public void WhereNotNull_SelectorReturnsNull_SkipsItem()
	{
		string?[] data = { "a", "b" };
		var result = data.WhereNotNull(x => x == "a" ? null : x).ToList();
		CollectionAssert.AreEqual(new[] { "b" }, result);
	}

	[TestMethod]
	public void SelectNotNull_SelectorReturnsNull_SkipsItem()
	{
		string?[] data = { "a", "b" };
		var result = data.SelectNotNull(x => x == "a" ? null : x!.ToUpperInvariant()).ToList();
		CollectionAssert.AreEqual(new[] { "B" }, result);
	}

	[TestMethod]
	public void FindSequenceConsecutive_SizeLargerThanSource_ReturnsEmpty()
	{
		var data = new[] { 1, 2, 3 };
		var result = data.FindSequenceConsecutive(x => x > 0, 5);
		Assert.IsFalse(result.Any());
	}

	[TestMethod]
	public void GetByIdOrOrder_InvalidIndex_ReturnsNull()
	{
		var items = new[]
		{
			new FrameworkItemStub { Name = "Alpha", Id = 1 },
			new FrameworkItemStub { Name = "Beta", Id = 2 }
		};

		Assert.IsNull(items.GetByIdOrOrder("#0"));
		Assert.IsNull(items.GetByIdOrOrder("#-1"));
		Assert.IsNull(items.GetByIdOrOrder("#3"));
	}

	[TestMethod]
	public void GetById_InvalidText_ReturnsNull()
	{
		var items = new[]
		{
			new FrameworkItemStub { Name = "Alpha", Id = 1 }
		};

		Assert.IsNull(items.GetById("abc"));
	}

	[TestMethod]
	public void GetByRevisableId_InvalidText_ReturnsNull()
	{
		var items = new[]
		{
			new RevisableItemStub { Name = "Alpha", Id = 1, RevisionNumber = 1, Status = RevisionStatus.Current }
		};

		Assert.IsNull(items.GetByRevisableId("abc"));
	}

	[TestMethod]
	public void GetByIdOrName_AbbreviationMatch_ReturnsItem()
	{
		var items = new[]
		{
			new FrameworkItemStub { Name = "Gamma", Id = 1 },
			new FrameworkItemStub { Name = "Gamut", Id = 2 }
		};

		Assert.AreEqual(1, items.GetByIdOrName("Gam")?.Id);
	}

	[TestMethod]
	public void GetByIdOrNameRevisable_ContainsMatch_ReturnsCurrent()
	{
		var items = new[]
		{
			new RevisableItemStub { Name = "Stone Hammer", Id = 1, RevisionNumber = 1, Status = RevisionStatus.Current },
			new RevisableItemStub { Name = "Stone Hammer", Id = 1, RevisionNumber = 2, Status = RevisionStatus.UnderDesign }
		};

		Assert.AreEqual(1, items.GetByIdOrNameRevisable("Hammer")?.RevisionNumber);
	}

	[TestMethod]
	public void GetByIdOrNameRevisableForEditing_ContainsMatch_ReturnsPending()
	{
		var items = new[]
		{
			new RevisableItemStub { Name = "Stone Hammer", Id = 1, RevisionNumber = 1, Status = RevisionStatus.Current },
			new RevisableItemStub { Name = "Stone Hammer", Id = 1, RevisionNumber = 2, Status = RevisionStatus.PendingRevision }
		};

		Assert.AreEqual(2, items.GetByIdOrNameRevisableForEditing("Hammer")?.RevisionNumber);
	}

	[TestMethod]
	public void GetByIdOrNameRevisable_NoPreferredStatuses_ReturnsHighestRevision()
	{
		var items = new[]
		{
			new RevisableItemStub { Name = "Widget", Id = 1, RevisionNumber = 1, Status = RevisionStatus.Revised },
			new RevisableItemStub { Name = "Widget", Id = 1, RevisionNumber = 5, Status = RevisionStatus.Obsolete }
		};

		Assert.AreEqual(5, items.GetByIdOrNameRevisable("Widget")?.RevisionNumber);
	}
}

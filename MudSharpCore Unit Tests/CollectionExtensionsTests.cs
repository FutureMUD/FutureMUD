using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CollectionExtensionsTests
{
	[TestMethod]
	public void TestSum2Int()
	{
		var data = new[] { 1, 2, 3, 4 };
		var (s1, s2) = data.Sum2(x => x, x => x * 2);
		Assert.AreEqual(10, s1, "Sum1 incorrect");
		Assert.AreEqual(20, s2, "Sum2 incorrect");
	}

	[TestMethod]
	public void TestSum3Int()
	{
		var data = new[] { 1, 2, 3, 4 };
		var (s1, s2, s3) = data.Sum3(x => x, x => x * 2, x => x * 3);
		Assert.AreEqual(10, s1, "Sum1 incorrect");
		Assert.AreEqual(20, s2, "Sum2 incorrect");
		Assert.AreEqual(30, s3, "Sum3 incorrect");
	}

	[TestMethod]
	public void TestSum2Double()
	{
		var data = new[] { 1.0, 2.0, 3.0 };
		var (s1, s2) = data.Sum2(x => x, x => x * 0.5);
		Assert.AreEqual(6.0, s1, 1e-6, "Sum1 incorrect");
		Assert.AreEqual(3.0, s2, 1e-6, "Sum2 incorrect");
	}

	[TestMethod]
	public void TestSum3Double()
	{
		var data = new[] { 1.0, 2.0, 3.0 };
		var (s1, s2, s3) = data.Sum3(x => x, x => x * 2.0, x => x * 0.5);
		Assert.AreEqual(6.0, s1, 1e-6, "Sum1 incorrect");
		Assert.AreEqual(12.0, s2, 1e-6, "Sum2 incorrect");
		Assert.AreEqual(3.0, s3, 1e-6, "Sum3 incorrect");
	}

	[TestMethod]
	public void TestMinMaxInt()
	{
		var data = new[] { -5, 3, 10, -2 };
		var (min, max) = data.MinMax();
		Assert.AreEqual(-5, min, "Min incorrect");
		Assert.AreEqual(10, max, "Max incorrect");
	}

	[TestMethod]
	public void TestMinMaxIntPredicate()
	{
		var data = new[] { -5, 3, 10, -2 };
		var (min, max) = data.MinMax(x => x > 0);
		Assert.AreEqual(0, min, "Min incorrect");
		Assert.AreEqual(10, max, "Max incorrect");
	}

	[TestMethod]
	public void TestMinMaxDouble()
	{
		var data = new[] { -1.5, 0.5, 2.5 };
		var (min, max) = data.MinMax();
		Assert.AreEqual(-1.5, min, 1e-6, "Min incorrect");
		Assert.AreEqual(2.5, max, 1e-6, "Max incorrect");
	}

	[TestMethod]
	public void TestMinMaxDoublePredicate()
	{
		var data = new[] { -1.5, 0.5, 2.5 };
		var (min, max) = data.MinMax(x => x >= 0);
		Assert.AreEqual(0.0, min, 1e-6, "Min incorrect");
		Assert.AreEqual(2.5, max, 1e-6, "Max incorrect");
	}

	[TestMethod]
	public void TestFirstMaxValueType()
	{
		var data = new List<(int id, int score)>
		{
			(1, -5),
			(2, 0),
			(3, 3),
			(4, 3)
		};
		var result = data.FirstMax(x => x.score);
		Assert.IsNotNull(result);
		Assert.AreEqual(3, result.id, "Incorrect element returned");
	}

	[TestMethod]
	public void TestFirstMaxReferenceType()
	{
		var data = new List<Tuple<int,int>>
		{
			Tuple.Create(1, -5),
			Tuple.Create(2, 0),
			Tuple.Create(3, 3),
			Tuple.Create(4, 3),
		};
		var result = data.FirstMax(x => x.Item2);
		Assert.IsNotNull(result);
		Assert.AreEqual(3, result.Item1, "Incorrect element returned");
	}

	[TestMethod]
	public void TestFirstMinValueType()
	{
		var data = new List<(int id, int score)>
		{
			(1, -5),
			(2, 2),
			(3, -3),
			(4, 2)
		};
		var result = data.FirstMin(x => x.score);
		Assert.IsNotNull(result);
		Assert.AreEqual(1, result.id, "Incorrect element returned");
	}


	[TestMethod]
	public void TestFirstMinReferenceType()
	{
		var data = new List<Tuple<int, int>>
		{
			Tuple.Create(1, -5),
			Tuple.Create(2, 0),
			Tuple.Create(3, 3),
			Tuple.Create(4, 3),
		};
		var result = data.FirstMin(x => x.Item2);
		Assert.IsNotNull(result);
		Assert.AreEqual(1, result.Item1, "Incorrect element returned");
	}

	[TestMethod]
	public void TestFindSequenceConsecutive()
	{
		var data = new[] { 1, 2, 2, 2, 3 };
		var results = data.FindSequenceConsecutive(x => x == 2, 2).ToList();
		Assert.AreEqual(2, results.Count, "Unexpected sequence count");
		foreach (var seq in results)
		{
			CollectionAssert.AreEqual(new[] { 2, 2 }, seq.ToList(), "Sequence mismatch");
		}
	}

	[TestMethod]
	public void TestSwapByIndex()
	{
		var list = new List<int> { 1, 2, 3 };
		list.SwapByIndex(0, 2);
		CollectionAssert.AreEqual(new[] { 3, 2, 1 }, list);
	}

	[TestMethod]
	public void TestSwapByItem()
	{
		var list = new List<int> { 1, 2, 3 };
		list.Swap(1, 3);
		CollectionAssert.AreEqual(new[] { 3, 2, 1 }, list);
	}
}

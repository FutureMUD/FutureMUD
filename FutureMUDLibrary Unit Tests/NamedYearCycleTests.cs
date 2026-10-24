#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.TimeAndDate.Date;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NamedYearCycleTests
{
	[TestMethod]
	[DataRow(10, "First", 1, 1L)]
	[DataRow(12, "Third", 3, 1L)]
	[DataRow(13, "First", 1, 2L)]
	[DataRow(9, "Third", 3, 0L)]
	[DataRow(6, "Third", 3, -1L)]
	public void GetYear_WrapsInBothDirections(int year, string name, int index, long cycle)
	{
		var result = new NamedYearCycle("Cycle", 10, ["First", "Second", "Third"]).GetYear(year);
		Assert.AreEqual(name, result.Name);
		Assert.AreEqual(index, result.YearInCycle);
		Assert.AreEqual(cycle, result.CycleNumber);
	}

	[TestMethod]
	public void GetYear_ExtremeYears_DoNotOverflowOrExposeMutableNames()
	{
		string[] names = ["First", "Second"];
		var cycle = new NamedYearCycle("Cycle", int.MaxValue, names);
		names[0] = "Changed";
		Assert.AreEqual("First", cycle.GetYear(int.MaxValue).Name);
		Assert.AreEqual(new NamedYearInfo("Second", 2, -2147483647L), cycle.GetYear(int.MinValue));
	}

	[TestMethod]
	public void Constructor_RejectsEmptyAndAmbiguousYearLists()
	{
		Assert.ThrowsExactly<ArgumentException>(() => new NamedYearCycle("", 1, ["First"]));
		Assert.ThrowsExactly<ArgumentException>(() => new NamedYearCycle("Cycle", 1, []));
		Assert.ThrowsExactly<ArgumentException>(() => new NamedYearCycle("Cycle", 1, ["First", " "]));
		Assert.ThrowsExactly<ArgumentException>(() => new NamedYearCycle("Cycle", 1, ["First", "first"]));
	}
}

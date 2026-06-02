using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework.Units;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class UnitManagerParsingTests
{
	[TestMethod]
	[DataRow("++1m")]
	[DataRow("--1kg")]
	[DataRow("+-1m")]
	[DataRow("1.2.3m")]
	public void GetUnitMatches_MalformedUnitValue_ReturnsNoMatches(string pattern)
	{
		var matches = UnitManager.GetUnitMatches(pattern);

		Assert.IsFalse(matches.Any());
	}

	[TestMethod]
	public void TryParseUnitValue_SignedUnitValue_ParsesWithInvariantCulture()
	{
		var match = UnitManager.GetUnitMatches("-1.5m").Single();

		Assert.IsTrue(UnitManager.TryParseUnitValue(match, out var value));
		Assert.AreEqual(-1.5, value);
	}

	[TestMethod]
	public void GetUnitMatches_MultipleUnitValues_ReturnsAllMatches()
	{
		var matches = UnitManager.GetUnitMatches("5ft 2in");

		Assert.AreEqual(2, matches.Count);
		Assert.AreEqual("5", matches[0].Groups[1].Value);
		Assert.AreEqual("ft", matches[0].Groups[2].Value);
		Assert.AreEqual("2", matches[1].Groups[1].Value);
		Assert.AreEqual("in", matches[1].Groups[2].Value);
	}
}

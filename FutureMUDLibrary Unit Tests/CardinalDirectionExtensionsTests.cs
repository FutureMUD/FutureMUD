#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CardinalDirectionExtensionsTests
{
	[TestMethod]
	public void DescribeBrief_NorthWest_ReturnsNw()
	{
		Assert.AreEqual("nw", CardinalDirection.NorthWest.DescribeBrief());
	}

	[DataTestMethod]
	[DataRow("nw")]
	[DataRow("northwest")]
	[DataRow("north-west")]
	[DataRow("north west")]
	public void CardinalExitStrings_NorthWestAliasesResolve(string alias)
	{
		Assert.IsTrue(CardinalDirectionExtensions.CardinalExitStrings.TryGetValue(alias, out var direction));
		Assert.AreEqual(CardinalDirection.NorthWest, direction);
	}

	[DataTestMethod]
	[DataRow("nor")]
	[DataRow("northw")]
	[DataRow("northwe")]
	public void CardinalExitStrings_PartialPrefixesDoNotResolve(string alias)
	{
		Assert.IsFalse(CardinalDirectionExtensions.CardinalExitStrings.ContainsKey(alias));
		Assert.IsFalse(Constants.CardinalDirectionStringToDirection.ContainsKey(alias));
	}

	[TestMethod]
	public void CardinalDirectionStringToDirection_DownKeepsDnAlias()
	{
		Assert.IsTrue(Constants.CardinalDirectionStringToDirection.TryGetValue("dn", out var direction));
		Assert.AreEqual(CardinalDirection.Down, direction);
	}
}

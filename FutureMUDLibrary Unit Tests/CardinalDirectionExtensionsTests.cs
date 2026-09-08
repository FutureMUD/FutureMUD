#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy.Currency;
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

	[TestMethod]
	public void CountDirections_CardinalOpposingDiagonalVerticalAndUnknown_PreservesSignedAxesAndCountsUnknown()
	{
		var counts = new[]
		{
			CardinalDirection.North,
			CardinalDirection.South,
			CardinalDirection.NorthEast,
			CardinalDirection.Up,
			CardinalDirection.Down,
			CardinalDirection.Unknown
		}.CountDirections();

		Assert.AreEqual(1, counts.Northness);
		Assert.AreEqual(-1, counts.Southness);
		Assert.AreEqual(-1, counts.Westness);
		Assert.AreEqual(1, counts.Eastness);
		Assert.AreEqual(0, counts.Upness);
		Assert.AreEqual(0, counts.Downness);
		Assert.AreEqual(1, counts.Unknownness);
	}

	[TestMethod]
	public void DistanceAsCrowFlies_AllSpatialVariants_ReturnsNetEuclideanRoomsPlusUnknownHops()
	{
		Assert.AreEqual(0, Array.Empty<CardinalDirection>().DistanceAsCrowFlies());
		Assert.AreEqual(1, new[] { CardinalDirection.North }.DistanceAsCrowFlies());
		Assert.AreEqual(2, new[] { CardinalDirection.North, CardinalDirection.North }.DistanceAsCrowFlies());
		Assert.AreEqual(0, new[] { CardinalDirection.North, CardinalDirection.South }.DistanceAsCrowFlies());
		Assert.AreEqual(1, new[] { CardinalDirection.NorthEast }.DistanceAsCrowFlies());
		Assert.AreEqual(1, new[] { CardinalDirection.North, CardinalDirection.East }.DistanceAsCrowFlies());
		Assert.AreEqual(1, new[] { CardinalDirection.Up }.DistanceAsCrowFlies());
		Assert.AreEqual(0, new[] { CardinalDirection.Up, CardinalDirection.Down }.DistanceAsCrowFlies());
		Assert.AreEqual(2, new[] { CardinalDirection.North, CardinalDirection.East, CardinalDirection.Up }.DistanceAsCrowFlies());
		Assert.AreEqual(2, new[] { CardinalDirection.Unknown, CardinalDirection.Unknown }.DistanceAsCrowFlies());
		Assert.AreEqual(2, new[] { CardinalDirection.North, CardinalDirection.Unknown }.DistanceAsCrowFlies());
	}

	[TestMethod]
	public void PythagoreanDistance_AllSpatialVariants_UsesRequestedRoundingAndAddsUnknownHops()
	{
		Assert.AreEqual(0, CreatePath().PythagoreanDistance());
		Assert.AreEqual(1, CreatePath(CardinalDirection.North).PythagoreanDistance());
		Assert.AreEqual(2, CreatePath(CardinalDirection.North, CardinalDirection.North).PythagoreanDistance());
		Assert.AreEqual(0, CreatePath(CardinalDirection.North, CardinalDirection.South).PythagoreanDistance());
		Assert.AreEqual(1, CreatePath(CardinalDirection.NorthEast).PythagoreanDistance());
		Assert.AreEqual(1, CreatePath(CardinalDirection.North, CardinalDirection.East).PythagoreanDistance());
		Assert.AreEqual(1, CreatePath(CardinalDirection.Up).PythagoreanDistance());
		Assert.AreEqual(0, CreatePath(CardinalDirection.Up, CardinalDirection.Down).PythagoreanDistance());
		Assert.AreEqual(1, CreatePath(CardinalDirection.North, CardinalDirection.East, CardinalDirection.Up).PythagoreanDistance());
		Assert.AreEqual(1, CreatePath(CardinalDirection.North, CardinalDirection.East, CardinalDirection.Up)
			.PythagoreanDistance(RoundingMode.NoRounding));
		Assert.AreEqual(2, CreatePath(CardinalDirection.North, CardinalDirection.East, CardinalDirection.Up)
			.PythagoreanDistance(RoundingMode.Round));
		Assert.AreEqual(2, CreatePath(CardinalDirection.Unknown, CardinalDirection.Unknown).PythagoreanDistance());
		Assert.AreEqual(2, CreatePath(CardinalDirection.North, CardinalDirection.Unknown).PythagoreanDistance());
	}

	[TestMethod]
	public void MaximumAxialDistance_AllSpatialVariants_ReturnsLargestNetAxisPlusUnknownHops()
	{
		Assert.AreEqual(0, CreatePath().MaximumAxialDistance());
		Assert.AreEqual(1, CreatePath(CardinalDirection.North).MaximumAxialDistance());
		Assert.AreEqual(2, CreatePath(CardinalDirection.North, CardinalDirection.North).MaximumAxialDistance());
		Assert.AreEqual(0, CreatePath(CardinalDirection.North, CardinalDirection.South).MaximumAxialDistance());
		Assert.AreEqual(1, CreatePath(CardinalDirection.NorthEast).MaximumAxialDistance());
		Assert.AreEqual(1, CreatePath(CardinalDirection.North, CardinalDirection.East).MaximumAxialDistance());
		Assert.AreEqual(1, CreatePath(CardinalDirection.Up).MaximumAxialDistance());
		Assert.AreEqual(0, CreatePath(CardinalDirection.Up, CardinalDirection.Down).MaximumAxialDistance());
		Assert.AreEqual(2, CreatePath(CardinalDirection.North, CardinalDirection.North, CardinalDirection.East,
			CardinalDirection.Up).MaximumAxialDistance());
		Assert.AreEqual(2, CreatePath(CardinalDirection.Unknown, CardinalDirection.Unknown).MaximumAxialDistance());
		Assert.AreEqual(2, CreatePath(CardinalDirection.North, CardinalDirection.Unknown).MaximumAxialDistance());
	}

	private static List<ICellExit> CreatePath(params CardinalDirection[] directions)
	{
		return directions.Select(direction =>
		{
			var exit = new Mock<ICellExit>();
			exit.SetupGet(x => x.OutboundDirection).Returns(direction);
			return exit.Object;
		}).ToList();
	}
}

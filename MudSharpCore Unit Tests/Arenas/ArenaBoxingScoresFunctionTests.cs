#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.FutureProg.Functions.Arena;
using System.Collections.Generic;

namespace MudSharp_Unit_Tests.Arenas;

[TestClass]
public class ArenaBoxingScoresFunctionTests
{
	[TestMethod]
	public void CalculateScores_CountsOnlyLandedUndefendedHeadAndTorsoHits()
	{
		List<decimal> scores = ArenaBoxingScoresFunction.CalculateScores(
			new[] { 0, 1 },
			new[] { 0, 0, 1, 1, 1 },
			new[] { 1, 1, 1, 0, 1 },
			new[] { 1, 0, 1, 1, 1 },
			new[] { "head", "head", "torso", "torso", "arm" });

		CollectionAssert.AreEqual(new decimal[] { 1.0m, 1.0m }, scores);
	}

	[TestMethod]
	public void CalculateScores_PreservesZeroScoresForParticipatingSides()
	{
		List<decimal> scores = ArenaBoxingScoresFunction.CalculateScores(
			new[] { 0, 1, 2 },
			new[] { 2, 2 },
			new[] { 0, 1 },
			new[] { 1, 1 },
			new[] { "head", "leg" });

		CollectionAssert.AreEqual(new decimal[] { 0.0m, 0.0m, 0.0m }, scores);
	}
}

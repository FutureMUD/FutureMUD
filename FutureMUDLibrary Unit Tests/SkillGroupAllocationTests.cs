#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.CharacterCreation;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SkillGroupAllocationTests
{
	[TestMethod]
	public void Allocation_ExhaustiveSmallCandidateSets_MatchesBruteForce()
	{
		for (var a = 0; a < 8; a++)
		for (var b = 0; b < 8; b++)
		for (var c = 0; c < 8; c++)
		{
			var pools = new[] { a, b, c }.Select(mask => Enumerable.Range(0, 3).Where(x => (mask & (1 << x)) != 0).Select(x => (long)x).ToArray()).ToArray();
			var expected = pools[0].Any(x => pools[1].Any(y => y != x && pools[2].Any(z => z != x && z != y)));
			var groups = pools.Select((pool, i) => new SkillGroupAllocation(i.ToString(), 1, 1, pool, Array.Empty<long>()));
			Assert.AreEqual(expected, SkillGroupAllocation.IsFeasible(groups), $"{a}/{b}/{c}");
		}
	}

	[TestMethod]
	public void Allocation_MultipleSlots_ReassignsEarlierMatches()
	{
		Assert.IsTrue(SkillGroupAllocation.IsFeasible([
			new("broad", 2, 2, new long[] { 1, 2, 3 }, Array.Empty<long>()),
			new("narrow", 1, 1, new long[] { 1 }, Array.Empty<long>())]));
		Assert.IsFalse(SkillGroupAllocation.IsFeasible([
			new("broad", 2, 2, new long[] { 1, 2, 3 }, new long[] { 1 }),
			new("narrow", 1, 1, new long[] { 1 }, Array.Empty<long>())]));
	}
}

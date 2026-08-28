#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace FutureMUDLibrary_Unit_Tests.Framework;

[TestClass]
public class RandomSkewNormalTests
{
	[TestMethod]
	public void RandomSkewNormal_PreservesConfiguredMeanAndDeviation()
	{
		const double expectedMean = 42.0;
		const double expectedDeviation = 7.0;
		var samples = Enumerable.Range(0, 100_000)
			.Select(_ => RandomUtilities.RandomSkewNormal(expectedMean, expectedDeviation, 0.8))
			.ToArray();
		var actualMean = samples.Average();
		var actualDeviation = System.Math.Sqrt(samples.Average(x => System.Math.Pow(x - actualMean, 2.0)));

		Assert.AreEqual(expectedMean, actualMean, 0.15);
		Assert.AreEqual(expectedDeviation, actualDeviation, 0.15);
	}
}

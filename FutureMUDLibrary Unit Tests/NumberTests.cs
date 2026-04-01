using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NumberTests
{
	[TestMethod]
	public void TestModulus()
	{
		Assert.AreEqual(12.0, 372.0.Wrap(0.0, 360.0), "372 wrap 12");
		Assert.AreEqual(348.0, (-12.0).Wrap(0.0, 360.0), "372 wrap 12");
		Assert.AreEqual(4, 40.Modulus(6), "40 mod 6");
		Assert.AreEqual(-4, (-40) % 6, "-40 % 6");
		Assert.AreEqual(2, (-40).Modulus(6), "-40 mod 6");
		Assert.AreEqual(12.0, 372.0.Modulus(360.0), "372 mod 360");
		Assert.AreEqual(348.0, (-12.0).Modulus(360.0), "-12 mod 360");
		Assert.AreEqual(0.0, 0.0.Modulus(360.0), "0 mod 360");
		Assert.AreEqual(0.0, 360.0.Modulus(360.0), "360 mod 360");
		Assert.AreEqual(55.0, 5455.0.Modulus(360.0), "5455 mod 360");
		Assert.AreEqual(55.0, (-3499145.0).Modulus(360.0), "-3499145 mod 360");
	}

        [TestMethod]
        public void TestInvertSign()
        {
                Assert.AreEqual(12.0, (-12.0).InvertSign(), "-ve to +ve");
                Assert.AreEqual(0.0, 0.0.InvertSign(), "zero");
                Assert.AreEqual(-12.0, 12.0.InvertSign(), "+ve to -ve");
                Assert.AreEqual(double.MinValue, double.MaxValue.InvertSign(), "maxvalue to minvalue");
                Assert.AreEqual(double.MaxValue, double.MinValue.InvertSign(), "minvalue to maxvalue");
                Assert.AreEqual(-5.0M, 5.0M.InvertSign(), "decimal +ve to -ve");
                Assert.AreEqual(0.0M, 0.0M.InvertSign(), "decimal zero");
                Assert.AreEqual(5.0M, (-5.0M).InvertSign(), "decimal -ve to +ve");
                Assert.AreEqual(decimal.MinValue, decimal.MaxValue.InvertSign(), "decimal maxvalue to minvalue");
                Assert.AreEqual(decimal.MaxValue, decimal.MinValue.InvertSign(), "decimal minvalue to maxvalue");
        }

        [TestMethod]
        public void TestIsPowerOfTwo()
        {
                Assert.IsTrue(1.IsPowerOfTwo(), "1 is power of two");
                Assert.IsTrue(1024.IsPowerOfTwo(), "1024 is power of two");
                Assert.IsTrue(2048L.IsPowerOfTwo(), "2048L is power of two");
                Assert.IsFalse(0.IsPowerOfTwo(), "0 is not power of two");
                Assert.IsFalse(3.IsPowerOfTwo(), "3 is not power of two");
                Assert.IsFalse((-8).IsPowerOfTwo(), "-8 is not power of two");
        }

        [TestMethod]
        public void TestLongPower()
        {
                Assert.AreEqual(8L, 2L.LongPower(3), "2^3");
                Assert.AreEqual(1L, 5L.LongPower(0), "5^0");
                Assert.AreEqual(-27L, (-3L).LongPower(3), "-3^3");
        }

        [TestMethod]
        public void TestIfZero()
        {
                Assert.AreEqual(5.0, 0.0.IfZero(5.0), "double zero");
                Assert.AreEqual(-3.0, (-3.0).IfZero(5.0), "double non-zero");
                Assert.AreEqual(5, 0.IfZero(5), "int zero");
                Assert.AreEqual(-3, (-3).IfZero(5), "int non-zero");
                Assert.AreEqual(5.0M, 0.0M.IfZero(5.0M), "decimal zero");
                Assert.AreEqual(-3.0M, (-3.0M).IfZero(5.0M), "decimal non-zero");
        }

        [TestMethod]
        public void TestEvenOdd()
        {
                Assert.IsTrue(4.Even(), "4 even");
                Assert.IsFalse(5.Even(), "5 not even");
                Assert.IsTrue(5.Odd(), "5 odd");
                Assert.IsFalse(4.Odd(), "4 not odd");
                Assert.IsTrue(0.Even(), "0 even");
                Assert.IsFalse(0.Odd(), "0 not odd");
                Assert.IsTrue((-4).Even(), "-4 even");
                Assert.IsFalse((-3).Odd(), "-3 not odd");
        }

        [TestMethod]
        public void TestBetween()
        {
                Assert.IsTrue(5.0.Between(1.0, 10.0), "5 between 1 and 10");
                Assert.IsTrue(5.0.Between(10.0, 1.0), "5 between reversed range");
                Assert.IsFalse(0.0.Between(1.0, 10.0), "0 not between");
                Assert.IsTrue(1.0.Between(1.0, 10.0), "lower boundary");
                Assert.IsTrue((-5.0).Between(-10.0, -1.0), "negative range");
        }

        [TestMethod]
        public void TestDifferenceRatio()
        {
                Assert.AreEqual(0.5, NumberUtilities.DifferenceRatio(10.0, 15.0), 1e-6, "difference ratio");
                Assert.AreEqual(5.0, NumberUtilities.DifferenceRatio(0.0, 5.0), 1e-6, "zero comparison");
                Assert.AreEqual(0.25, NumberUtilities.DifferenceRatio(-4.0, -5.0), 1e-6, "negative numbers");
        }

        [TestMethod]
        public void TestApproximateInt()
        {
                Assert.AreEqual(40, 43.Approximate(5), "round down");
                Assert.AreEqual(0, 0.Approximate(5), "zero");
                Assert.AreEqual(43, 43.Approximate(0), "round 0");
                Assert.AreEqual(-40, (-43).Approximate(5), "negative");
        }

        [TestMethod]
        public void TestApproximateDouble()
        {
                Assert.AreEqual(5.4, 5.49.Approximate(0.1), 1e-6, "double round down");
                Assert.AreEqual(-5.5, (-5.1).Approximate(0.5), 1e-6, "double negative round");
        }

        [TestMethod]
        public void TestToApproximate()
        {
                Assert.AreEqual("0", 0.ToApproximate(100), "zero");
                Assert.AreEqual("1,234", 1234.ToApproximate(0), "no rounding");
                Assert.AreEqual("approximately 1,500", 1560.ToApproximate(100), "approximate");
                Assert.AreEqual("less than 100", 50.ToApproximate(100), "less than round");
        }

        [TestMethod]
        public void TestGetIntFromOrdinal()
        {
                Assert.AreEqual(1, "1st".GetIntFromOrdinal(), "1st");
                Assert.AreEqual(2, "2nd".GetIntFromOrdinal(), "2nd");
                Assert.AreEqual(3, "3rd".GetIntFromOrdinal(), "3rd");
                Assert.AreEqual(4, "4th".GetIntFromOrdinal(), "4th");
                Assert.IsNull("11th".GetIntFromOrdinal(), "11th invalid");
                Assert.IsNull("abc".GetIntFromOrdinal(), "invalid");
                Assert.IsNull(NumberUtilities.GetIntFromOrdinal(null), "null");
        }

        [TestMethod]
        public void TestDegreesRadians()
        {
                Assert.AreEqual(Math.PI, 180.0.DegreesToRadians(), 1e-10, "degrees to radians");
                Assert.AreEqual(180.0, Math.PI.RadiansToDegrees(), 1e-10, "radians to degrees");
                Assert.AreEqual(0.0, 0.0.DegreesToRadians(), "zero degrees");
                Assert.AreEqual(0.0, 0.0.RadiansToDegrees(), "zero radians");
                Assert.AreEqual(-Math.PI, (-180.0).DegreesToRadians(), 1e-10, "negative degrees");
        }

        [TestMethod]
        public void TestDescribeAsProbability()
        {
                Assert.AreEqual("Impossible", 0.0.DescribeAsProbability(), "zero");
                Assert.AreEqual("Extremely Unlikely", 0.05.DescribeAsProbability(), "0.05");
                Assert.AreEqual("Very Unlikely", 0.15.DescribeAsProbability(), "0.15");
                Assert.AreEqual("Unlikely", 0.3.DescribeAsProbability(), "0.3");
                Assert.AreEqual("Likely", 0.6.DescribeAsProbability(), "0.6");
                Assert.AreEqual("Very Likely", 0.85.DescribeAsProbability(), "0.85");
                Assert.AreEqual("Extremely Likely", 0.95.DescribeAsProbability(), "0.95");
                Assert.AreEqual("Certain", 1.0.DescribeAsProbability(), "1.0");
        }

        [TestMethod]
        public void TestGetPowerLawDistribution()
        {
                var distribution = NumberUtilities.GetPowerLawDistribution(100.0, 0.2, 10).ToList();
                Assert.AreEqual(100.0, distribution.Sum(), 1e-6, "sum to total");
                for (var i = 0; i < distribution.Count - 1; i++)
                {
                        Assert.IsTrue(distribution[i] >= distribution[i + 1], $"monotonic at {i}");
                }

                var edge = NumberUtilities.GetPowerLawDistribution(100.0, 1.0, 5).ToList();
                Assert.AreEqual(100.0, edge[0], 1e-6, "edge first");
                for (var i = 1; i < edge.Count; i++)
                {
                        Assert.AreEqual(0.0, edge[i], 1e-6, "edge others zero");
                }
        }
}

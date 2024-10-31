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
}
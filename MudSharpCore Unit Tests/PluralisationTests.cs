using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PluralisationTests
{
	[TestMethod]
	public void TestIsSingular()
	{
		Assert.AreEqual("tent".ContainsPlural(), false, "Expected tent to not be a plural");
		Assert.AreEqual("tents".ContainsPlural(), true, "Expected tents to be a plural");
		Assert.AreEqual("big brown dog".ContainsPlural(), false, "Expected big brown dog to not be a plural");
		Assert.AreEqual("chartreuse-eyed".ContainsPlural(), false, "Expected chartreuse-eyed to not be a plural");
		Assert.AreEqual("black afro-haired".ContainsPlural(), false, "Expected black afro-haired to not be a plural");
		Assert.AreEqual("denarii", "denarius".Pluralise());
	}
}
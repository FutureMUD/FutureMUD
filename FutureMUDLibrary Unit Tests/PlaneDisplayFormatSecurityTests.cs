#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Planes;
using System;

namespace FutureMUDLibrary_UnitTests;

[TestClass]
public class PlaneDisplayFormatSecurityTests
{
	[DataTestMethod]
	[DataRow("Astral Plane {0}", "Astral Plane Room")]
	[DataRow("{{{0}}}", "{Room}")]
	[DataRow("{0} {0,-8}", "Room Room    ")]
	public void BoundedFormatsPreserveNormalPresentation(string format, string expected)
	{
		Assert.IsTrue(PlaneDisplayFormat.TryFormat(format, "Room", out var result));
		Assert.AreEqual(expected, result);
	}

	[DataTestMethod]
	[DataRow("{0}{0,9999999}")]
	[DataRow("{0}{0,-9999999}")]
	[DataRow("{0}{0,2147483648}")]
	[DataRow("{0}{1}")]
	[DataRow("{0}{")]
	[DataRow("{{0}}")]
	public void UnsafeFormatsAreRejectedBeforeExpansion(string format)
	{
		Assert.IsFalse(PlaneDisplayFormat.TryFormat(format, "Room", out var result));
		Assert.AreEqual("Room", result);
	}

	[TestMethod]
	public void FormatFieldAndRenderedLengthBudgetsAreEnforced()
	{
		Assert.IsFalse(PlaneDisplayFormat.TryFormat(new string('x', 1024) + "{0}", "Room", out _));
		Assert.IsFalse(PlaneDisplayFormat.TryFormat(string.Concat(System.Linq.Enumerable.Repeat("{0}", 17)), "Room", out _));
		Assert.IsFalse(PlaneDisplayFormat.TryFormat("{0}{0}", new string('x', 10000), out _));
		Assert.IsTrue(PlaneDisplayFormat.TryFormat("{0,128}", "Room", out var result));
		Assert.AreEqual(128, result.Length);
	}
}

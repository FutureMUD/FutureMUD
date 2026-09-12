#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Computers;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ComputerMediaBuiltInApplicationExecutorTests
{
	[TestMethod]
	[DataRow("NaN")]
	[DataRow("Infinity")]
	[DataRow("1e100")]
	[DataRow("-1")]
	public void TryParseMediaTimestamp_InvalidTimestamp_ReturnsFalse(string text)
	{
		var result = MediaBuiltInApplicationExecutor.TryParseMediaTimestamp(text, out var timestamp);

		Assert.IsFalse(result);
		Assert.AreEqual(TimeSpan.Zero, timestamp);
	}

	[TestMethod]
	public void TryParseMediaTimestamp_RepresentableTimestamp_ReturnsTimeSpan()
	{
		var result = MediaBuiltInApplicationExecutor.TryParseMediaTimestamp("12.5", out var timestamp);

		Assert.IsTrue(result);
		Assert.AreEqual(TimeSpan.FromSeconds(12.5), timestamp);
	}
}

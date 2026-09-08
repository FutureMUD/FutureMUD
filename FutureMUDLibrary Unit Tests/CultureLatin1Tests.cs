#nullable enable

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureLatin1Tests
{
	[DataTestMethod]
	[DataRow("François", "François")]
	[DataRow("Þórðr", "Þórðr")]
	[DataRow("Æthelred", "Æthelred")]
	[DataRow("Łódź", "Lódz")]
	[DataRow("Dvořák", "Dvorák")]
	[DataRow("İbrahim", "Ibrahim")]
	[DataRow("Şahin", "Sahin")]
	[DataRow("Œuvre", "OEuvre")]
	[DataRow("e\u0301", "é")]
	[DataRow("Ƿulf", "Wulf")]
	[DataRow("Mușa", "Musa")]
	[DataRow("Ŋala", "Nala")]
	[DataRow("François d’Avila", "François d'Avila")]
	public void CF16_ReviewedDisplayFormsPreserveSupportedLetters(string source, string expected)
	{
		Assert.AreEqual(expected, source.ConvertToLatin1());
	}

	[TestMethod]
	public void CF16_IndependentFallbackBuffersDoNotOverwritePendingExpansions()
	{
		var fallback = StringExtensions.Latin1Encoder.EncoderFallback;
		var first = fallback.CreateFallbackBuffer();
		var second = fallback.CreateFallbackBuffer();
		Assert.AreNotSame(first, second);
		Assert.IsTrue(first.Fallback('Œ', 0));
		Assert.AreEqual('O', first.GetNextChar());
		Assert.IsTrue(second.Fallback('…', 0));
		Assert.AreEqual('E', first.GetNextChar());
		Assert.AreEqual('.', second.GetNextChar());
		Assert.IsTrue(first.MovePrevious());
		Assert.AreEqual('E', first.GetNextChar());
		first.Reset();
		Assert.AreEqual(0, first.Remaining);
		Assert.AreEqual(2, second.Remaining);
	}

	[TestMethod]
	public void CF16_ActualEncodersAreSafeInParallelAndExposeUnsupportedScalars()
	{
		Parallel.For(0, 1000, _ =>
		{
			var encoder = StringExtensions.Latin1Encoder.GetEncoder();
			var source = "ŒŁ…😀\U0010ffff".ToCharArray();
			var bytes = new byte[StringExtensions.Latin1Encoder.GetMaxByteCount(source.Length)];
			var count = encoder.GetBytes(source, 0, source.Length, bytes, 0, true);
			Assert.AreEqual("OEL...[U+1F600][U+10FFFF]", Encoding.Latin1.GetString(bytes, 0, count));
		});
		Assert.AreEqual("[U+4E2D]", "中".ConvertToLatin1());
	}

	[TestMethod]
	public void CF16_NormalizationPreservesProtocolAndAllLatin1Characters()
	{
		var latin1 = new string(Enumerable.Range(0, 256).Select(x => (char)x).ToArray());
		Assert.AreEqual(latin1, latin1.ConvertToLatin1());
		Assert.AreEqual("\u001b[32mé\u001b[0m", "\u001b[32me\u0301\u001b[0m".ConvertToLatin1());
	}
}

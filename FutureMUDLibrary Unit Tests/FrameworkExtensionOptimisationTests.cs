#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FrameworkExtensionOptimisationTests
{
	public TestContext TestContext { get; set; } = null!;

	[TestMethod]
	public void RawText_SpanAndStringPreserveMarkupStateMachine()
	{
		var random = new Random(7821);
		var alphabet = new[] { 'a', 'm', '\x1b', MXP.BeginMXPChar, MXP.EndMXPChar, '[', ' ', '\n', '\ud83d', '\ude00' };
		Span<char> output = stackalloc char[100];
		for (var sample = 0; sample < 500; sample++)
		{
			var text = new string(Enumerable.Range(0, random.Next(100)).Select(_ => alphabet[random.Next(alphabet.Length)]).ToArray());
			var expected = LegacyRawText(text);
			Assert.AreEqual(expected, text.RawText());
			Assert.AreEqual(expected.Length, text.RawTextLength());
			Assert.AreEqual(expected.Length, text.AsSpan().RawTextLength());
			Assert.IsTrue(text.AsSpan().TryCopyRawText(output, out var written));
			Assert.AreEqual(expected, output[..written].ToString());
		}

		Assert.AreSame("plain", "plain".RawText());
		Assert.AreEqual(0, ((string)null!).RawTextLength());
		var large = string.Concat(Enumerable.Repeat("a\x1b[31mb\x1b[0mc", 1000));
		Assert.AreEqual(LegacyRawText(large), large.RawText());
	}

	[TestMethod]
	public void CopyRawText_ReportsPartialWriteAndSupportsInPlaceCompaction()
	{
		Span<char> small = stackalloc char[2];
		Assert.IsFalse("abcd".AsSpan().TryCopyRawText(small, out var written));
		Assert.AreEqual(2, written);
		Assert.AreEqual("ab", small.ToString());
		var buffer = "a\x1b[31mb\x1b[0mc".ToCharArray();
		Assert.IsTrue(((ReadOnlySpan<char>)buffer).TryCopyRawText(buffer, out written));
		Assert.AreEqual("abc", buffer.AsSpan(0, written).ToString());
		Assert.IsTrue("\x1b[31m".AsSpan().TryCopyRawText(Span<char>.Empty, out written));
		Assert.AreEqual(0, written);
	}

	[TestMethod]
	public void StringSearchAndReplacement_PreserveEdgeCases()
	{
		Assert.IsTrue("CAFÉ".EqualToAny("other", "café"));
		Assert.IsTrue(((string)null!).EqualToAny(new string[] { "x", null! }));
		Assert.IsFalse("x".EqualToAny());
		Assert.ThrowsExactly<ArgumentNullException>(() => "x".EqualToAny((string[])null!));
		Assert.AreEqual("-abc", "abc".ReplaceFirst("", "-"));
		Assert.AreEqual("ac", "abc".ReplaceFirst("b", null!));
		Assert.AreEqual("xbc", "abc".ReplaceFirst('a', 'x'));
		Assert.AreEqual("abc", "abc".ReplaceFirst('x', 'y'));
		Assert.AreEqual("tail", "x \t tail".RemoveFirstCharacter());
		Assert.AreEqual("tail", "word \t tail".RemoveFirstWord());
		Assert.AreEqual("\tAbc\n", " _\tA-b_c\n ".CollapseString());
		Assert.AreSame("unchanged", "unchanged".CollapseString());
		var words = new List<string> { "Alpha", "BETA" };
		Assert.IsTrue(words.HasWord("al"));
		Assert.IsFalse(words.HasWord("al", false));
		Assert.IsTrue(words.HasWord(new[] { "ALPHA", "beta" }, false));
		Assert.IsTrue(words.HasWord(Array.Empty<string>()));
	}

	[TestMethod]
	public void ListFormatting_EnumeratesOnceAndPreservesSeparators()
	{
		Assert.AreEqual("the a, the b, and the c", new SingleUse<string>("a", "b", "c").ListToString(article: "the "));
		Assert.AreEqual("a, b and c", new SingleUse<string>("a", "b", "c").ListToString(oxfordComma: false));
		Assert.AreEqual("a, b, c or d", new SingleUse<string>("a", "b", "c", "d").ListToString(conjunction: " or ", oxfordComma: false));
		Assert.AreEqual("a\nb\nc", new SingleUse<string>("a", "b", "c").ListToLines());
		Assert.AreEqual("a|or b", new SingleUse<string>("a", "b").ListToString(twoItemJoiner: "|", conjunction: "or "));
		Assert.AreEqual("", new SingleUse<string>().ListToString());
		Assert.AreEqual("a", new SingleUse<string>("a").ListToString());
		Assert.AreEqual(", b, and ", new string[] { null!, "b", null! }.ListToString());
	}

	[TestMethod]
	public void Csv_PreservesEmptyFieldsAndQuotesCarriageReturns()
	{
		Assert.AreEqual(",,a,", new[] { "", "", "a", "" }.ListToCommaSeparatedValues());
		Assert.AreEqual(", , a, ", new[] { "", "", "a", "" }.ListToCommaSeparatedValues(options: StringListToCSVOptions.SpaceAfterComma));
		Assert.AreEqual(",\"a\rb\",\"a\"\"b\",", new string[] { "", "a\rb", "a\"b", null! }.ListToCommaSeparatedValues(options: StringListToCSVOptions.RFC4180Compliant));
	}

	[TestMethod]
	public void ListFormatting_IndexedAndStreamingPathsAgreeOnAllOptions()
	{
		foreach (var count in Enumerable.Range(0, 7))
		foreach (var article in new[] { null, "", "the " })
		foreach (var separator in new[] { null, ", ", "|" })
		foreach (var conjunction in new[] { null, "", "and ", " or " })
		foreach (var twoJoiner in new[] { null, " ", ":" })
		foreach (var oxford in new[] { false, true })
		{
			var items = Enumerable.Range(0, count).Select(x => x % 2 == 0 ? "word" : null!).ToArray();
			var expected = new SingleUse<string>(items).ListToString(article!, separator!, conjunction!, twoJoiner!, oxford);
			Assert.AreEqual(expected, items.ListToString(article!, separator!, conjunction!, twoJoiner!, oxford));
		}
	}

	[TestMethod]
	[DataRow("{0,10}")]
	[DataRow("{0,-10:N2}")]
	[DataRow("{{{0}}}")]
	[DataRow("{0:}")]
	[DataRow("{0 }")]
	[DataRow("{00}")]
	public void FormatValidation_RecognisesRequiredArgumentsInAllValidForms(string format)
	{
		Assert.IsTrue(format.IsValidFormatString(1));
		_ = string.Format(format, 1);
	}

	[TestMethod]
	public void FormatValidation_RejectsMissingAndInvalidArguments()
	{
		Assert.IsFalse("{{0}}".IsValidFormatString(1));
		Assert.IsFalse("{0} {2,10}".IsValidFormatString(2));
		Assert.IsFalse("{0}".IsValidFormatString(-1));
		Assert.IsFalse("{0}".IsValidFormatString(2, new[] { true }));
		Assert.IsTrue("{1}".IsValidFormatString(new[] { false, true }));
		Assert.IsTrue("{{braces}}".IsValidFormatString(0));
		var largeFormat = string.Join(" ", Enumerable.Range(0, 300).Select(x => "{" + x + "}"));
		Assert.IsTrue(largeFormat.IsValidFormatString(300));
		Assert.IsFalse(largeFormat.IsValidFormatString(301));
	}

	[TestMethod]
	public void WrapText_LongWordsAdvanceWithoutDuplicatingCharacters()
	{
		Assert.AreEqual(string.Join(Environment.NewLine, "abc", "def", "gh") + Environment.NewLine, "abcdefgh".WrapText(3));
		Assert.AreEqual(string.Join(Environment.NewLine, "a", "b", "c") + Environment.NewLine, "abc".WrapText(1));
		Assert.AreEqual(string.Join(Environment.NewLine, "  a", "  b", "  c") + Environment.NewLine, "abc".WrapText(1, "  "));
	}

	[TestMethod]
	public void RandomHelpers_AcceptSingleUseSourcesAndRejectImpossiblePicks()
	{
		Assert.AreEqual(42, new SingleUse<int>(42).GetRandomElement());
		Assert.AreEqual(0, new SingleUse<int>().GetRandomElement());
		Assert.AreEqual(42, new[] { 42 }.GetRandomElement());
		Assert.AreEqual(42, ((ReadOnlySpan<int>)new[] { 42 }).GetRandomElement());
		Assert.AreEqual(42, new SingleUse<(int, int)>((42, 1)).GetWeightedRandom());
		Assert.AreEqual(42, new SingleUse<(int, double)>((42, 1.0)).GetWeightedRandom());
		Assert.AreEqual(42, new SingleUse<int>(42).GetWeightedRandom(x => 1));
		Assert.AreEqual(42, new SingleUse<int>(42).GetWeightedRandom(x => 1.0));
		Assert.ThrowsExactly<ArgumentException>(() => new SingleUse<int>(1, 1).PickRandom(2));
		CollectionAssert.AreEquivalent(new[] { 1, 2 }, new SingleUse<int>(1, 1, 2).PickRandom(2).ToArray());
	}

	[TestMethod]
	public void OddAndDictionaryMembership_HandleNegativeAndNestedValues()
	{
		Assert.IsTrue((-3).Odd());
		Assert.IsFalse(int.MinValue.Odd());
		Assert.IsTrue(int.MaxValue.Odd());
		Assert.IsFalse(long.MinValue.IsPowerOfTwo());
		Assert.IsFalse(int.MinValue.IsPowerOfTwo());
		for (var power = 0; power < 63; power++)
		{
			Assert.IsTrue((1L << power).IsPowerOfTwo());
		}
		var dictionary = new CollectionDictionary<string, string>();
		dictionary.Add("first", "alpha");
		dictionary.Add("first", "beta");
		dictionary.Add("second", null!);
		Assert.IsTrue(dictionary.ContainsValue("beta"));
		Assert.IsTrue(dictionary.ContainsValue(null!));
		Assert.IsFalse(dictionary.ContainsValue("missing"));
		Assert.IsTrue(dictionary.AsReadOnlyCollectionDictionary().ContainsValue("alpha"));
	}

	[TestMethod]
	public void WeightedRandom_IntegerEvaluatorMatchesTupleDistributionAndDraws()
	{
		var expected = new Random(719);
		using var scope = Constants.PushRandom(new Random(719));
		for (var i = 0; i < 100; i++)
		{
			var result = expected.NextDouble() * 2 <= 1 ? "first" : "second";
			Assert.AreEqual(result, new[] { "first", "second" }.GetWeightedRandom(_ => 1));
		}
	}

	[TestMethod]
	public void WeightedRandom_OverloadsPreserveSeededDrawsAndEvaluateOnce()
	{
		var ints = new[] { (Value: "first", Weight: 1), (Value: "second", Weight: 3), (Value: "third", Weight: 5) };
		var doubles = ints.Select(x => (x.Value, Weight: (double)x.Weight)).ToArray();
		var expected = new Random(913);
		using var scope = Constants.PushRandom(new Random(913));
		for (var i = 0; i < 100; i++)
		{
			var roll = expected.NextDouble() * 9;
			var value = roll <= 1 ? "first" : roll <= 4 ? "second" : "third";
			var evaluations = 0;
			var actual = (i % 4) switch
			{
				0 => new SingleUse<(string, int)>(ints).GetWeightedRandom(),
				1 => new SingleUse<(string, double)>(doubles).GetWeightedRandom(),
				2 => new SingleUse<(string Value, int Weight)>(ints).GetWeightedRandom(x => { evaluations++; return x.Weight; }).Value,
				_ => new SingleUse<(string Value, double Weight)>(doubles).GetWeightedRandom(x => { evaluations++; return x.Weight; }).Value
			};
			Assert.AreEqual(value, actual);
			Assert.AreEqual(i % 4 >= 2 ? 3 : 0, evaluations);
		}
	}

	[TestMethod]
	public void SpanHotPaths_DoNotAllocateAfterWarmup()
	{
		var values = new[] { 42 };
		const string text = "a\x1b[31mb\x1b[0mc";
		Span<char> buffer = stackalloc char[3];
		var checksum = 0;
		for (var i = 0; i < 1000; i++)
		{
			_ = "beta".EqualToAny("alpha", "BETA", "gamma");
			_ = text.AsSpan().TryCopyRawText(buffer, out _);
			_ = text.AsSpan().RawTextLength();
			_ = values.GetRandomElement();
		}

		var before = GC.GetAllocatedBytesForCurrentThread();
		for (var i = 0; i < 1000; i++)
		{
			checksum += "beta".EqualToAny("alpha", "BETA", "gamma") ? 1 : 0;
			checksum += text.AsSpan().TryCopyRawText(buffer, out var written) ? written : 0;
			checksum += text.AsSpan().RawTextLength();
			checksum += values.GetRandomElement();
		}

		var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
		Assert.AreEqual(49000, checksum);
		Assert.AreEqual(0L, allocated);
	}

	[TestMethod]
	public void AllocationComparison_ReportsRepresentativeBeforeAndAfter()
	{
		const string text = "a\x1b[31m coloured message \x1b[0mhere";
		Compare("RawText", () => LegacyRawText(text), () => text.RawText());
		Compare("ReplaceFirst", () => text[..5] + "X" + text[6..], () => text.ReplaceFirst(text[5], 'X'));
		var items = new[] { "one", "two", "three", "four", "five" };
		Compare("ListToString", () => LegacyListToString(items), () => items.ListToString(article: "the "));
	}

	private void Compare(string label, Func<string> before, Func<string> after)
	{
		Assert.AreEqual(before(), after());
		var old = Measure(before);
		var current = Measure(after);
		TestContext.WriteLine($"{label}: old {old.Bytes / 10000.0:N1} B/op, {old.Ticks} ticks; new {current.Bytes / 10000.0:N1} B/op, {current.Ticks} ticks (10,000 calls).");
		Assert.IsTrue(current.Bytes < old.Bytes, label + " should allocate less than the previous implementation.");
	}

	private static (long Bytes, long Ticks) Measure(Func<string> action)
	{
		for (var i = 0; i < 1000; i++) _ = action();
		var bytes = GC.GetAllocatedBytesForCurrentThread();
		var start = Stopwatch.GetTimestamp();
		for (var i = 0; i < 10000; i++) GC.KeepAlive(action());
		return (GC.GetAllocatedBytesForCurrentThread() - bytes, Stopwatch.GetTimestamp() - start);
	}

	private static string LegacyRawText(string text)
	{
		var builder = new StringBuilder();
		bool colour = false, mxp = false;
		foreach (var ch in text)
		{
			colour |= ch == '\x1b';
			mxp |= ch == MXP.BeginMXPChar;
			if (!colour && !mxp) builder.Append(ch);
			colour &= ch != 'm';
			mxp &= ch != MXP.EndMXPChar;
		}
		return builder.ToString();
	}

	private static string LegacyListToString(IEnumerable<string> input)
	{
		var list = input.ToList();
		return string.Join(", ", list.GetRange(0, list.Count - 1).ConvertAll(x => x.Insert(0, "the "))) + ", and the " + list[^1];
	}

	private sealed class SingleUse<T>(params T[] items) : IEnumerable<T>
	{
		private bool _enumerated;
		public IEnumerator<T> GetEnumerator()
		{
			Assert.IsFalse(_enumerated, "Source was enumerated more than once.");
			_enumerated = true;
			return ((IEnumerable<T>)items).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}

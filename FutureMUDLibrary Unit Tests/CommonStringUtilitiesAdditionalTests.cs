using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CommonStringUtilitiesAdditionalTests
{
    private enum TestEnum
    {
        First,
        Second
    }

    [TestMethod]
    public void WrapText_RespectsWidthAndIndent()
    {
        const string text = "This is a simple wrapping test that should produce multiple lines.";
        string result = text.WrapText(10, "> ");
        string[] lines = result.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string line in lines)
        {
            Assert.IsTrue(line.Length <= 10, $"Line exceeded width: {line}");
            Assert.IsTrue(line.StartsWith("> "), $"Line missing indent: {line}");
        }
    }

    [TestMethod]
    public void WrapText_WidthLessThanOne_ReturnsOriginal()
    {
        const string text = "No wrapping needed.";
        Assert.AreEqual(text, text.WrapText(0));
    }

    [TestMethod]
    public void CultureFormat_UsesSpecifiedCulture()
    {
        decimal number = 12.5m;
        CultureInfo culture = CultureInfo.GetCultureInfo("fr-FR");
        string result = number.CultureFormat(culture);
        Assert.AreEqual("12,5", result);
    }

    [TestMethod]
    public void SplitTextIntoColumns_GenericList()
    {
        List<string> lines = new()
        {
            "Item 1","Item 2","Item 3","Item 4","Item 5","Item 6","Item 7","Item 8","Item 9","Item 10","Item 11"
        };
        string expected = string.Join("\n", lines).SplitTextIntoColumns(2, 20, 2);
        string actual = lines.SplitTextIntoColumns(2, 20, 2);
        Assert.AreEqual(expected, actual, $"Actual Output: [{actual}]");
    }

    [TestMethod]
    public void ArrangeStringsOntoLines_WrapsCorrectly()
    {
        List<string> items = new()
        { "Item1", "Item2", "Item3", "Item4", "Item5" };
        string expected = @"Item1     Item2
Item3     Item4
Item5
";
        string actual = items.ArrangeStringsOntoLines(2, 20);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ListToLines_BasicAndTabs()
    {
        List<string> items = new()
        { "One", "Two", "Three" };
        string expected = "One\nTwo\nThree";
        Assert.AreEqual(expected, items.ListToLines());

        string expectedTabs = "\tOne\n\tTwo\n\tThree";
        Assert.AreEqual(expectedTabs, items.ListToLines(true));
    }

    [TestMethod]
    public void ListToCommaSeparatedValues_Options()
    {
        List<string> items = new()
        { "a", "b", "c" };
        string spaced = items.ListToCommaSeparatedValues(options: StringListToCSVOptions.SpaceAfterComma);
        Assert.AreEqual("a, b, c", spaced);

        List<string> rfcItems = new()
        { "a", "b,c", "d\"e", "f\nx" };
        string rfc = rfcItems.ListToCommaSeparatedValues(options: StringListToCSVOptions.RFC4180Compliant);
        Assert.AreEqual("a,\"b,c\",\"d\"\"e\",\"f\nx\"", rfc);
    }

    [TestMethod]
    public void ListToColouredString_Enums()
    {
        TestEnum[] items = new[] { TestEnum.First, TestEnum.Second };
        string expected = $"{TestEnum.First.DescribeEnum().Colour(Telnet.Green)} and {TestEnum.Second.DescribeEnum().Colour(Telnet.Green)}";
        Assert.AreEqual(expected, items.ListToColouredString());
    }

    [TestMethod]
    public void ListToColouredStringOr_Enums()
    {
        TestEnum[] items = new[] { TestEnum.First, TestEnum.Second };
        string expected = $"{TestEnum.First.DescribeEnum().Colour(Telnet.Green)} or {TestEnum.Second.DescribeEnum().Colour(Telnet.Green)}";
        Assert.AreEqual(expected, items.ListToColouredStringOr());
    }

    [TestMethod]
    public void ListToColouredString_Strings()
    {
        string[] items = new[] { "alpha", "beta" };
        string expected = $"{"alpha".Colour(Telnet.Green)} and {"beta".Colour(Telnet.Green)}";
        Assert.AreEqual(expected, items.ListToColouredString());
    }

    [TestMethod]
    public void ListToColouredStringOr_Strings()
    {
        string[] items = new[] { "alpha", "beta" };
        string expected = $"{"alpha".Colour(Telnet.Green)} or {"beta".Colour(Telnet.Green)}";
        Assert.AreEqual(expected, items.ListToColouredStringOr());
    }

    [TestMethod]
    public void ListToString_VariousCounts_FormatsCorrectly()
    {
        Assert.AreEqual(string.Empty, Array.Empty<string>().ListToString());
        Assert.AreEqual("the cat", new[] { "cat" }.ListToString(article: "the "));
        Assert.AreEqual("the cat and the dog", new[] { "cat", "dog" }.ListToString(article: "the "));
        Assert.AreEqual("cat, dog and mouse", new[] { "cat", "dog", "mouse" }.ListToString(oxfordComma: false));
    }

    [TestMethod]
    public void ListToCompactString_DeduplicatesAndCounts()
    {
        string[] items = new[] { "apple", "apple", "banana" };
        Assert.AreEqual("apple (x2) and banana", items.ListToCompactString());
    }

    [TestMethod]
    public void ListToCommaSeparatedValues_DefaultAndNull()
    {
        Assert.AreEqual("a,b", new[] { "a", "b" }.ListToCommaSeparatedValues());
        List<string> items = null;
        Assert.AreEqual(string.Empty, items.ListToCommaSeparatedValues());
    }

    [TestMethod]
    public void SplitTextIntoColumns_InsertsColumnSpacing()
    {
        string input = string.Join("\n", new[] { "A", "B", "C", "D" });
        string result = input.SplitTextIntoColumns(2, 10, 2);
        string newline = Environment.NewLine;
        Assert.AreEqual("A   " + "  " + "C   " + newline + "B   " + "  " + "D   " + newline, result);
    }

    [TestMethod]
    public void ArrangeStringsOntoLines_NumberPerLineZero_SetsToOne()
    {
        List<string> items = new()
        { "A", "B" };
        string result = items.ArrangeStringsOntoLines(0, 10);
        string newline = Environment.NewLine;
        Assert.AreEqual($"A{newline}B{newline}", result);
    }

    [TestMethod]
    public void ToStringFormats_WithColourAndCulture()
    {
        CultureInfo culture = CultureInfo.InvariantCulture;
        Assert.AreEqual("1,234", 1234.ToStringN0(culture));
        Assert.AreEqual("1,234.50", 1234.5m.ToStringN2(culture));
        string percent = 0.1234m.ToString("P2", culture);
        Assert.AreEqual(percent, 0.1234m.ToStringP2(culture));
        Assert.AreEqual($"1,234".Colour(Telnet.Green), 1234.ToStringN0Colour(culture));
        Assert.AreEqual($"1,234.50".Colour(Telnet.Green), 1234.5m.ToStringN2Colour(culture));
        Assert.AreEqual(percent.Colour(Telnet.Green), 0.1234m.ToStringP2Colour(culture));
    }

    [TestMethod]
    public void WindowsLineEndings_ReplacesUnixNewlines()
    {
        string input = "Line1\nLine2\r\nLine3\n";
        Assert.AreEqual("Line1\r\nLine2\r\nLine3\r\n", input.WindowsLineEndings());
    }

    [TestMethod]
    public void Contains_WithComparison_ReturnsExpected()
    {
        Assert.IsTrue("Hello".Contains("he", StringComparison.OrdinalIgnoreCase));
        string source = null;
        Assert.IsTrue(CommonStringUtilities.Contains(source, "anything", StringComparison.Ordinal));
    }

    [TestMethod]
    public void GetWidthRuler_UsesCorrectSeparators()
    {
        string small = CommonStringUtilities.GetWidthRuler(80);
        Assert.AreEqual(2, small.Count(c => c == '|'));

        string large = CommonStringUtilities.GetWidthRuler(100);
        Assert.AreEqual(3, large.Count(c => c == '|'));
    }

    [TestMethod]
    public void ReplaceAt_ReplacesWithinBounds()
    {
        Assert.AreEqual("abZZef", "abcdef".ReplaceAt(2, 2, "ZZ"));
        Assert.AreEqual("aZ", "abc".ReplaceAt(1, 10, "Z"));
    }

    [TestMethod]
    public void AppendLineColumns_AppendsFormattedLines()
    {
        StringBuilder sb = new();
        sb.AppendLineColumns(10, 2, "A", "B", "C");
        string newline = Environment.NewLine;
        Assert.AreEqual($"A    B{newline}C{newline}", sb.ToString());
    }
}

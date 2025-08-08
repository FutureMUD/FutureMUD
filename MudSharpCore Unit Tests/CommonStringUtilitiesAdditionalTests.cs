using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

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
        var result = text.WrapText(10, "> ");
        var lines = result.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            Assert.IsTrue(line.Length <= 10, $"Line exceeded width: {line}");
            Assert.IsTrue(line.StartsWith("> "), $"Line missing indent: {line}");
        }
    }

    [TestMethod]
    public void CultureFormat_UsesSpecifiedCulture()
    {
        var number = 12.5m;
        var culture = CultureInfo.GetCultureInfo("fr-FR");
        var result = number.CultureFormat(culture);
        Assert.AreEqual("12,5", result);
    }

    [TestMethod]
    public void SplitTextIntoColumns_GenericList()
    {
        var lines = new List<string>
        {
            "Item 1","Item 2","Item 3","Item 4","Item 5","Item 6","Item 7","Item 8","Item 9","Item 10","Item 11"
        };
        var expected = string.Join("\n", lines).SplitTextIntoColumns(2, 20, 2);
        var actual = lines.SplitTextIntoColumns(2, 20, 2);
        Assert.AreEqual(expected, actual, $"Actual Output: [{actual}]");
    }

    [TestMethod]
    public void ArrangeStringsOntoLines_WrapsCorrectly()
    {
        var items = new List<string> { "Item1", "Item2", "Item3", "Item4", "Item5" };
        var expected = @"Item1     Item2
Item3     Item4
Item5
";
        var actual = items.ArrangeStringsOntoLines(2, 20);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void ListToLines_BasicAndTabs()
    {
        var items = new List<string> { "One", "Two", "Three" };
        var expected = "One\nTwo\nThree";
        Assert.AreEqual(expected, items.ListToLines());

        var expectedTabs = "\tOne\n\tTwo\n\tThree";
        Assert.AreEqual(expectedTabs, items.ListToLines(true));
    }

    [TestMethod]
    public void ListToCommaSeparatedValues_Options()
    {
        var items = new List<string> { "a", "b", "c" };
        var spaced = items.ListToCommaSeparatedValues(options: StringListToCSVOptions.SpaceAfterComma);
        Assert.AreEqual("a, b, c", spaced);

        var rfcItems = new List<string> { "a", "b,c", "d\"e", "f\nx" };
        var rfc = rfcItems.ListToCommaSeparatedValues(options: StringListToCSVOptions.RFC4180Compliant);
        Assert.AreEqual("a,\"b,c\",\"d\"\"e\",\"f\nx\"", rfc);
    }

    [TestMethod]
    public void ListToColouredString_Enums()
    {
        var items = new[] { TestEnum.First, TestEnum.Second };
        var expected = $"{TestEnum.First.DescribeEnum().Colour(Telnet.Green)} and {TestEnum.Second.DescribeEnum().Colour(Telnet.Green)}";
        Assert.AreEqual(expected, items.ListToColouredString());
    }

    [TestMethod]
    public void ListToColouredStringOr_Enums()
    {
        var items = new[] { TestEnum.First, TestEnum.Second };
        var expected = $"{TestEnum.First.DescribeEnum().Colour(Telnet.Green)} or {TestEnum.Second.DescribeEnum().Colour(Telnet.Green)}";
        Assert.AreEqual(expected, items.ListToColouredStringOr());
    }

    [TestMethod]
    public void ListToColouredString_Strings()
    {
        var items = new[] { "alpha", "beta" };
        var expected = $"{"alpha".Colour(Telnet.Green)} and {"beta".Colour(Telnet.Green)}";
        Assert.AreEqual(expected, items.ListToColouredString());
    }

    [TestMethod]
    public void ListToColouredStringOr_Strings()
    {
        var items = new[] { "alpha", "beta" };
        var expected = $"{"alpha".Colour(Telnet.Green)} or {"beta".Colour(Telnet.Green)}";
        Assert.AreEqual(expected, items.ListToColouredStringOr());
    }
}

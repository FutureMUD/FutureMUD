using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.Form.Colour;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StringColourExtensionsTests
{
    [TestMethod]
    public void ColourIncludingReset_ReplacesEmbeddedResets()
    {
        var input = $"start{Telnet.RESET}middle{Telnet.RESETALL}end";
        var expected = $"{Telnet.Red}start{Telnet.Red}middle{Telnet.Red}end{Telnet.RESET}";
        var actual = input.ColourIncludingReset(Telnet.Red);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetRGB_ReturnsCorrectValues()
    {
        Assert.AreEqual((255,0,0), BasicColour.Red.GetRGB());
        Assert.AreEqual((0,0,255), BasicColour.Blue.GetRGB());
        Assert.AreEqual((0,255,0), BasicColour.Green.GetRGB());
        Assert.AreEqual((0,0,0), BasicColour.Black.GetRGB());
    }

    [TestMethod]
    public void Colour_BasicColour_ProducesAnsiSequence()
    {
        const string text = "test";
        var result = text.Colour(BasicColour.Red);
        var expected = $"{Telnet.Black.BackgroundColour}{Telnet.Red.Name}{text}{Telnet.RESETALL}";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Colour_RGB_ProducesAnsiSequence()
    {
        const string text = "rgb";
        var result = text.Colour(1, 2, 3);
        Assert.AreEqual("\x1b[38;2;1;2;3mrgb\x1B[0m", result);
    }
}

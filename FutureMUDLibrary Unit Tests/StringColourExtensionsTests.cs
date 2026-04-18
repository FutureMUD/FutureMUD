using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Form.Colour;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StringColourExtensionsTests
{
    [TestMethod]
    public void ColourIncludingReset_ReplacesEmbeddedResets()
    {
        string input = $"start{Telnet.RESET}middle{Telnet.RESETALL}end";
        string expected = $"{Telnet.Red}start{Telnet.Red}middle{Telnet.Red}end{Telnet.RESET}";
        string actual = input.ColourIncludingReset(Telnet.Red);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetRGB_ReturnsCorrectValues()
    {
        Assert.AreEqual((255, 0, 0), BasicColour.Red.GetRGB());
        Assert.AreEqual((0, 0, 255), BasicColour.Blue.GetRGB());
        Assert.AreEqual((0, 255, 0), BasicColour.Green.GetRGB());
        Assert.AreEqual((0, 0, 0), BasicColour.Black.GetRGB());
    }

    [TestMethod]
    public void Colour_BasicColour_ProducesAnsiSequence()
    {
        const string text = "test";
        string result = text.Colour(BasicColour.Red);
        string expected = $"{Telnet.Black.BackgroundColour}{Telnet.Red.Name}{text}{Telnet.RESETALL}";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void Colour_RGB_ProducesAnsiSequence()
    {
        const string text = "rgb";
        string result = text.Colour(1, 2, 3);
        Assert.AreEqual("\x1b[38;2;1;2;3mrgb\x1B[0m", result);
    }

    [TestMethod]
    public void ColourCharacter_TogglesBasedOnBoolean()
    {
        const string text = "character";
        string coloured = text.ColourCharacter();
        string uncoloured = text.ColourCharacter(false);
        Assert.AreEqual($"{Telnet.Magenta}{text}{Telnet.RESET}", coloured);
        Assert.AreEqual(text, uncoloured);
    }

    [TestMethod]
    public void ColourObject_TogglesBasedOnBoolean()
    {
        const string text = "object";
        string coloured = text.ColourObject();
        string uncoloured = text.ColourObject(false);
        Assert.AreEqual($"{Telnet.Green}{text}{Telnet.RESET}", coloured);
        Assert.AreEqual(text, uncoloured);
    }

    [TestMethod]
    public void ColourValue_TogglesBasedOnBoolean()
    {
        const string text = "value";
        string coloured = text.ColourValue();
        string uncoloured = text.ColourValue(false);
        Assert.AreEqual($"{Telnet.Green}{text}{Telnet.RESET}", coloured);
        Assert.AreEqual(text, uncoloured);
    }

    [TestMethod]
    public void ColourRoom_TogglesBasedOnBoolean()
    {
        const string text = "room";
        string coloured = text.ColourRoom();
        string uncoloured = text.ColourRoom(false);
        Assert.AreEqual($"{Telnet.Cyan}{text}{Telnet.RESET}", coloured);
        Assert.AreEqual(text, uncoloured);
    }

    [TestMethod]
    public void ColourCommand_TogglesBasedOnBoolean()
    {
        const string text = "command";
        string coloured = text.ColourCommand();
        string uncoloured = text.ColourCommand(false);
        Assert.AreEqual($"{Telnet.Yellow}{text}{Telnet.RESET}", coloured);
        Assert.AreEqual(text, uncoloured);
    }

    [TestMethod]
    public void ColourName_TogglesBasedOnBoolean()
    {
        const string text = "name";
        string coloured = text.ColourName();
        string uncoloured = text.ColourName(false);
        Assert.AreEqual($"{Telnet.Cyan}{text}{Telnet.RESET}", coloured);
        Assert.AreEqual(text, uncoloured);
    }

    [TestMethod]
    public void ColourError_TogglesBasedOnBoolean()
    {
        const string text = "error";
        string coloured = text.ColourError();
        string uncoloured = text.ColourError(false);
        Assert.AreEqual($"{Telnet.Red}{text}{Telnet.RESET}", coloured);
        Assert.AreEqual(text, uncoloured);
    }

    [TestMethod]
    public void Colour_AnsiColour_WrapsAndResets()
    {
        const string text = "ansi";
        string result = text.Colour(Telnet.Red);
        Assert.AreEqual($"{Telnet.Red}{text}{Telnet.RESET}", result);
    }

    [TestMethod]
    public void ColourBold_WrapsAndResets()
    {
        const string text = "bold";
        string result = text.ColourBold(Telnet.Blue);
        Assert.AreEqual($"{Telnet.Blue.Bold}{text}{Telnet.RESET}", result);
    }

    [TestMethod]
    public void ColourBackground_WrapsAndResetsAll()
    {
        const string text = "background";
        string result = text.ColourBackground(Telnet.Red);
        Assert.AreEqual($"{Telnet.Red.BackgroundColour}{text}{Telnet.RESETALL}", result);
    }

    [TestMethod]
    public void ColourBoldBackground_WrapsAndResetsAll()
    {
        const string text = "boldbackground";
        string result = text.ColourBoldBackground(Telnet.Green);
        Assert.AreEqual($"{Telnet.Green.BoldBackgroundColour}{text}{Telnet.RESETALL}", result);
    }

    [TestMethod]
    public void Colour_WithResetColour_UsesResetColour()
    {
        const string text = "resetcolour";
        string result = text.Colour(Telnet.Red, Telnet.Green);
        Assert.AreEqual($"{Telnet.Red.Colour}{text}{Telnet.Green.Colour}", result);
    }

    [TestMethod]
    public void ColourBold_WithResetColour_UsesResetColour()
    {
        const string text = "boldreset";
        string result = text.ColourBold(Telnet.Blue, Telnet.Green);
        Assert.AreEqual($"{Telnet.Blue.Bold}{text}{Telnet.Green.Colour}", result);
    }

    [TestMethod]
    public void Colour_StringForegroundBackground_WrapsAndResetsAll()
    {
        const string text = "fgbg";
        string result = text.Colour(Telnet.RED, Telnet.GREENBACKGROUND);
        Assert.AreEqual($"{Telnet.GREENBACKGROUND}{Telnet.RED}{text}{Telnet.RESETALL}", result);
    }

    [TestMethod]
    public void ColourForegroundCustom_AddsResets()
    {
        const string text = "custom";
        string result = text.ColourForegroundCustom("123");
        string expected = $"{Telnet.RESETALL}\x1B[38;5;123m{text}{Telnet.RESETALL}";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void FluentColour_TogglesBasedOnBoolean()
    {
        const string text = "fluent";
        string coloured = text.FluentColour(Telnet.Red, true);
        string uncoloured = text.FluentColour(Telnet.Red, false);
        Assert.AreEqual($"{Telnet.Red}{text}{Telnet.RESET}", coloured);
        Assert.AreEqual(text, uncoloured);
    }

    [TestMethod]
    public void FluentColourIncludingReset_TogglesBasedOnBoolean()
    {
        const string text = "fluentreset";
        string coloured = text.FluentColourIncludingReset(Telnet.Blue, true);
        string uncoloured = text.FluentColourIncludingReset(Telnet.Blue, false);
        Assert.AreEqual($"{Telnet.Blue}{text}{Telnet.RESET}", coloured);
        Assert.AreEqual(text, uncoloured);
    }

    [TestMethod]
    public void ColourIfNotColoured_LeavesPreColouredText()
    {
        string coloured = $"{Telnet.Blue}pre{Telnet.RESET}";
        string result = coloured.ColourIfNotColoured(Telnet.Red);
        Assert.AreEqual(coloured, result);
    }

    [TestMethod]
    public void ColourIfNotColoured_ColoursPlainText()
    {
        const string text = "plain";
        string result = text.ColourIfNotColoured(Telnet.Red);
        Assert.AreEqual($"{Telnet.Red}{text}{Telnet.RESET}", result);
    }

    [TestMethod]
    public void Underline_WrapsWithCodes()
    {
        const string text = "under";
        string result = text.Underline();
        Assert.AreEqual($"{Telnet.UNDERLINE}{text}{Telnet.RESETUNDERLINE}", result);
    }

    [TestMethod]
    public void Blink_WrapsWithCodes()
    {
        const string text = "blink";
        string result = text.Blink();
        Assert.AreEqual($"{Telnet.BLINK}{text}{Telnet.RESETBLINK}", result);
    }
}

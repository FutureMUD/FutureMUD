using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StringUtilitiesTests
{
    [TestMethod]
    public void SubstituteANSIColour_BasicCodes()
    {
        var input = "This is #4blue#0 text.";
        var expected = $"This is {Telnet.Blue}blue{Telnet.RESETALL} text.";
        Assert.AreEqual(expected, input.SubstituteANSIColour());
    }

    [TestMethod]
    public void SubstituteANSIColour_RGBCodes()
    {
        var input = "Colour #`12;34;56;here#0.";
        var expected = $"Colour \x1b[38;2;12;34;56mhere{Telnet.RESETALL}.";
        Assert.AreEqual(expected, input.SubstituteANSIColour());
    }

    [TestMethod]
    public void StripANSIColour_RemovesSequences()
    {
        var coloured = "Test #4blue#0 text".SubstituteANSIColour();
        Assert.AreEqual("Test blue text", coloured.StripANSIColour());
    }

    [TestMethod]
    public void SubstituteANSIColour_EscapedHash()
    {
        var input = "Value ##4 should stay";
        var expected = "Value #4 should stay";
        Assert.AreEqual(expected, input.SubstituteANSIColour());
    }

    [TestMethod]
    public void SubstituteANSIColour_InvalidCode_Unchanged()
    {
        var input = "Unknown #z tag";
        Assert.AreEqual(input, input.SubstituteANSIColour());
    }

    [TestMethod]
    public void SubstituteANSIColour_NullInput_ReturnsEmpty()
    {
        string input = null;
        Assert.AreEqual(string.Empty, input.SubstituteANSIColour());
    }

    [TestMethod]
    public void SubstituteANSIColour_MultipleCodes()
    {
        var input = "Red #1r#0 and blue #4b#0.";
        var expected = $"Red {Telnet.Red}r{Telnet.RESETALL} and blue {Telnet.Blue}b{Telnet.RESETALL}.";
        Assert.AreEqual(expected, input.SubstituteANSIColour());
    }

    [TestMethod]
    public void StripANSIColour_NoSequences_ReturnsOriginal()
    {
        var input = "Plain text";
        Assert.AreEqual(input, input.StripANSIColour());
    }

    [TestMethod]
    public void StripANSIColour_ComplexString()
    {
        var coloured = "Mix #1red#0 and #4blue#0".SubstituteANSIColour();
        Assert.AreEqual("Mix red and blue", coloured.StripANSIColour());
    }
}

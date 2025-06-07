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
}

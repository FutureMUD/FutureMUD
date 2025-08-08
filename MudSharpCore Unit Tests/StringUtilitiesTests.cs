using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.Form.Colour;
using MudSharp.RPG.Checks;

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

    [TestMethod]
    public void StripANSIColour_NoStripParameter_ReturnsOriginal()
    {
        var coloured = "#1Red#0 Text".SubstituteANSIColour();
        Assert.AreEqual(coloured, coloured.StripANSIColour(strip: false));
    }

    [TestMethod]
    public void GetTextTable_TruncatesSpecifiedColumn()
    {
        var data = new[]
        {
            new[] {"short", "longerthanwidth", "mid"},
            new[] {"short2", "bigger", "mid2"}
        };
        var header = new[] {"Col1", "Col2", "Col3"};
        var table = StringUtilities.GetTextTable(data, header, 32, bColourTable: false, truncatableColumnIndex: 1)
            .StripANSIColour();
        var lines = table.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.IsTrue(lines[1].Contains("Col1"));
        Assert.AreEqual(lines[1].IndexOf("Col1"), lines[3].IndexOf("short"));
        Assert.IsTrue(lines[3].Contains("longer..."));
        Assert.IsTrue(lines[4].Contains("bigger"));
    }

    [TestMethod]
    public void SubstituteCheckTrait_SubstitutesAndFallsBack()
    {
        var traitDef = new Mock<ITraitDefinition>();
        traitDef.Setup(x => x.Name).Returns("strength");
        var traitRepo = new Mock<IUneditableAll<ITraitDefinition>>();
        traitRepo.Setup(x => x.GetByIdOrName("strength", true)).Returns(traitDef.Object);
        var game = new Mock<IFuturemud>();
        game.Setup(x => x.Traits).Returns(traitRepo.Object);

        var high = new Mock<IPerceiver>();
        high.As<IPerceivableHaveTraits>()
            .Setup(x => x.TraitValue(traitDef.Object, TraitBonusContext.None))
            .Returns(10);
        var low = new Mock<IPerceiver>();
        low.As<IPerceivableHaveTraits>()
            .Setup(x => x.TraitValue(traitDef.Object, TraitBonusContext.None))
            .Returns(1);

        const string input = "check{strength,5}{pass}{fail}";
        Assert.AreEqual("pass", input.SubstituteCheckTrait(high.Object, game.Object));
        Assert.AreEqual("fail", input.SubstituteCheckTrait(low.Object, game.Object));
    }

    [TestMethod]
    public void SubstituteWrittenLanguage_SubstitutesAndFallsBack()
    {
        var traitDef = new Mock<ITraitDefinition>();
        var language = new Mock<ILanguage>();
        language.Setup(x => x.Name).Returns("lang");
        language.Setup(x => x.LinkedTrait).Returns(traitDef.Object);
        var script = new Mock<IScript>();
        script.Setup(x => x.Name).Returns("script");
        script.Setup(x => x.KnownScriptDescription).Returns("known");
        script.Setup(x => x.UnknownScriptDescription).Returns("unknown");
        var langRepo = new Mock<IUneditableAll<ILanguage>>();
        langRepo.Setup(x => x.GetByName("lang")).Returns(language.Object);
        var scriptRepo = new Mock<IUneditableAll<IScript>>();
        scriptRepo.Setup(x => x.GetByName("script")).Returns(script.Object);
        var colour = new Mock<IColour>();
        colour.Setup(x => x.Name).Returns("white");
        var colourRepo = new Mock<IUneditableAll<IColour>>();
        colourRepo.Setup(x => x.Get(0)).Returns(colour.Object);
        var game = new Mock<IFuturemud>();
        game.Setup(x => x.Languages).Returns(langRepo.Object);
        game.Setup(x => x.Scripts).Returns(scriptRepo.Object);
        game.Setup(x => x.Colours).Returns(colourRepo.Object);
        game.Setup(x => x.GetStaticInt("DefaultWritingStyleInText")).Returns(0);
        game.Setup(x => x.GetStaticLong("DefaultWritingColourInText")).Returns(0);

        var trait = new Mock<ITrait>();
        trait.Setup(x => x.Value).Returns(10);

        var known = new Mock<ILanguagePerceiver>();
        known.Setup(x => x.Languages).Returns(new[] { language.Object });
        known.Setup(x => x.Scripts).Returns(new[] { script.Object });
        known.As<IHaveTraits>().Setup(x => x.GetTrait(traitDef.Object)).Returns(trait.Object);

        var unknown = new Mock<ILanguagePerceiver>();
        unknown.Setup(x => x.Languages).Returns(Array.Empty<ILanguage>());
        unknown.Setup(x => x.Scripts).Returns(Array.Empty<IScript>());
        unknown.As<IHaveTraits>().Setup(x => x.GetTrait(It.IsAny<ITraitDefinition>())).Returns((ITrait)null);

        const string input = "writing{lang,script}{hello}{alt}";
        Assert.IsTrue(input.SubstituteWrittenLanguage(known.Object, game.Object).Contains("hello"));
        Assert.IsTrue(input.SubstituteWrittenLanguage(unknown.Object, game.Object).Contains("alt"));
    }

    [TestMethod]
    public void IsValidFormatString_AllOverloads()
    {
        const string valid = "Value {0} {1}";
        Assert.IsTrue(valid.IsValidFormatString(2));
        Assert.IsTrue(valid.IsValidFormatString(new[] { true, true }.AsSpan()));
        Assert.IsTrue(valid.IsValidFormatString(2, new[] { true, true }.AsSpan()));

        const string invalid = "Value {0} {2}";
        Assert.IsFalse(invalid.IsValidFormatString(2));
        Assert.IsFalse(invalid.IsValidFormatString(new[] { true, true, true }.AsSpan()));
        Assert.IsFalse(invalid.IsValidFormatString(3, new[] { true, true, true }.AsSpan()));
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Communication.Language;
using MudSharp.Form.Colour;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StringUtilitiesTests
{
    [TestMethod]
    public void SubstituteANSIColour_BasicCodes()
    {
        string input = "This is #4blue#0 text.";
        string expected = $"This is {Telnet.Blue}blue{Telnet.RESETALL} text.";
        Assert.AreEqual(expected, input.SubstituteANSIColour());
    }

    [TestMethod]
    public void SubstituteANSIColour_RGBCodes()
    {
        string input = "Colour #`12;34;56;here#0.";
        string expected = $"Colour \x1b[38;2;12;34;56mhere{Telnet.RESETALL}.";
        Assert.AreEqual(expected, input.SubstituteANSIColour());
    }

    [TestMethod]
    public void StripANSIColour_RemovesSequences()
    {
        string coloured = "Test #4blue#0 text".SubstituteANSIColour();
        Assert.AreEqual("Test blue text", coloured.StripANSIColour());
    }

    [TestMethod]
    public void SubstituteANSIColour_EscapedHash()
    {
        string input = "Value ##4 should stay";
        string expected = "Value #4 should stay";
        Assert.AreEqual(expected, input.SubstituteANSIColour());
    }

    [TestMethod]
    public void SubstituteANSIColour_InvalidCode_Unchanged()
    {
        string input = "Unknown #z tag";
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
        string input = "Red #1r#0 and blue #4b#0.";
        string expected = $"Red {Telnet.Red}r{Telnet.RESETALL} and blue {Telnet.Blue}b{Telnet.RESETALL}.";
        Assert.AreEqual(expected, input.SubstituteANSIColour());
    }

    [TestMethod]
    public void StripANSIColour_NoSequences_ReturnsOriginal()
    {
        string input = "Plain text";
        Assert.AreEqual(input, input.StripANSIColour());
    }

    [TestMethod]
    public void StripANSIColour_ComplexString()
    {
        string coloured = "Mix #1red#0 and #4blue#0".SubstituteANSIColour();
        Assert.AreEqual("Mix red and blue", coloured.StripANSIColour());
    }

    [TestMethod]
    public void StripANSIColour_NoStripParameter_ReturnsOriginal()
    {
        string coloured = "#1Red#0 Text".SubstituteANSIColour();
        Assert.AreEqual(coloured, coloured.StripANSIColour(strip: false));
    }

    [TestMethod]
    public void GetTextTable_TruncatesSpecifiedColumn()
    {
        string[][] data = new[]
        {
            new[] {"short", "longerthanwidth", "mid"},
            new[] {"short2", "bigger", "mid2"}
        };
        string[] header = new[] { "Col1", "Col2", "Col3" };
        string table = StringUtilities.GetTextTable(data, header, 32, bColourTable: false, truncatableColumnIndex: 1)
            .StripANSIColour();
        string[] lines = table.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.IsTrue(lines[1].Contains("Col1"));
        Assert.AreEqual(lines[1].IndexOf("Col1"), lines[3].IndexOf("short"));
        Assert.IsTrue(lines[3].Contains("longert..."));
        Assert.IsTrue(lines[4].Contains("bigger"));
    }

    [TestMethod]
    public void SubstituteCheckTrait_SubstitutesAndFallsBack()
    {
        Mock<ITraitDefinition> traitDef = new();
        traitDef.Setup(x => x.Name).Returns("strength");
        Mock<IUneditableAll<ITraitDefinition>> traitRepo = new();
        traitRepo.Setup(x => x.GetByIdOrName("strength", true)).Returns(traitDef.Object);
        Mock<IFuturemud> game = new();
        game.Setup(x => x.Traits).Returns(traitRepo.Object);

        Mock<IPerceiver> high = new();
        high.As<IPerceivableHaveTraits>()
            .Setup(x => x.TraitValue(traitDef.Object, TraitBonusContext.None))
            .Returns(10);
        Mock<IPerceiver> low = new();
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
        Mock<ITraitDefinition> traitDef = new();
        Mock<ILanguage> language = new();
        language.Setup(x => x.Name).Returns("lang");
        language.Setup(x => x.LinkedTrait).Returns(traitDef.Object);
        Mock<IScript> script = new();
        script.Setup(x => x.Name).Returns("script");
        script.Setup(x => x.KnownScriptDescription).Returns("known");
        script.Setup(x => x.UnknownScriptDescription).Returns("unknown");
        Mock<IUneditableAll<ILanguage>> langRepo = new();
        langRepo.Setup(x => x.GetByName("lang")).Returns(language.Object);
        Mock<IUneditableAll<IScript>> scriptRepo = new();
        scriptRepo.Setup(x => x.GetByName("script")).Returns(script.Object);
        Mock<IColour> colour = new();
        colour.Setup(x => x.Name).Returns("white");
        Mock<IUneditableAll<IColour>> colourRepo = new();
        colourRepo.Setup(x => x.Get(0)).Returns(colour.Object);
        Mock<IFuturemud> game = new();
        game.Setup(x => x.Languages).Returns(langRepo.Object);
        game.Setup(x => x.Scripts).Returns(scriptRepo.Object);
        game.Setup(x => x.Colours).Returns(colourRepo.Object);
        game.Setup(x => x.GetStaticInt("DefaultWritingStyleInText")).Returns(0);
        game.Setup(x => x.GetStaticLong("DefaultWritingColourInText")).Returns(0);

        Mock<ITrait> trait = new();
        trait.Setup(x => x.Value).Returns(10);

        Mock<ILanguagePerceiver> known = new();
        known.Setup(x => x.Languages).Returns(new[] { language.Object });
        known.Setup(x => x.Scripts).Returns(new[] { script.Object });
        known.As<IHaveTraits>().Setup(x => x.GetTrait(traitDef.Object)).Returns(trait.Object);

        Mock<ILanguagePerceiver> unknown = new();
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

    [TestMethod]
    public void SubstituteCheckTrait_NoTraits_ReturnsPrimaryText()
    {
        Mock<IFuturemud> game = new();
        Mock<IPerceiver> perceiver = new();
        const string input = "check{strength,5}{pass}{fail}";
        Assert.AreEqual("pass", input.SubstituteCheckTrait(perceiver.Object, game.Object));
    }

    [TestMethod]
    public void SubstituteCheckTrait_TraitMissing_ReturnsPrimaryText()
    {
        Mock<IUneditableAll<ITraitDefinition>> traitRepo = new();
        traitRepo.Setup(x => x.GetByIdOrName("strength", true)).Returns((ITraitDefinition)null);
        Mock<IFuturemud> game = new();
        game.Setup(x => x.Traits).Returns(traitRepo.Object);

        Mock<IPerceiver> perceiver = new();
        perceiver.As<IPerceivableHaveTraits>();

        const string input = "check{strength,5}{pass}{fail}";
        Assert.AreEqual("pass", input.SubstituteCheckTrait(perceiver.Object, game.Object));
    }

    [TestMethod]
    public void SubstituteCheckTrait_InvalidDifficulty_ReturnsPrimaryText()
    {
        Mock<ITraitDefinition> traitDef = new();
        traitDef.Setup(x => x.Name).Returns("strength");
        Mock<IUneditableAll<ITraitDefinition>> traitRepo = new();
        traitRepo.Setup(x => x.GetByIdOrName("strength", true)).Returns(traitDef.Object);
        Mock<IFuturemud> game = new();
        game.Setup(x => x.Traits).Returns(traitRepo.Object);

        Mock<IPerceiver> perceiver = new();
        perceiver.As<IPerceivableHaveTraits>()
            .Setup(x => x.TraitValue(traitDef.Object, TraitBonusContext.None))
            .Returns(10);

        const string input = "check{strength,1..2}{pass}{fail}";
        Assert.AreEqual("pass", input.SubstituteCheckTrait(perceiver.Object, game.Object));
    }

    [TestMethod]
    public void SubstituteCheckTrait_AlternateMissing_ReturnsEmpty()
    {
        Mock<ITraitDefinition> traitDef = new();
        traitDef.Setup(x => x.Name).Returns("strength");
        Mock<IUneditableAll<ITraitDefinition>> traitRepo = new();
        traitRepo.Setup(x => x.GetByIdOrName("strength", true)).Returns(traitDef.Object);
        Mock<IFuturemud> game = new();
        game.Setup(x => x.Traits).Returns(traitRepo.Object);

        Mock<IPerceiver> perceiver = new();
        perceiver.As<IPerceivableHaveTraits>()
            .Setup(x => x.TraitValue(traitDef.Object, TraitBonusContext.None))
            .Returns(1);

        const string input = "check{strength,5}{pass}";
        Assert.AreEqual(string.Empty, input.SubstituteCheckTrait(perceiver.Object, game.Object));
    }

    [TestMethod]
    public void SubstituteWrittenLanguage_NoLanguagePerceiver_ReturnsPrimaryText()
    {
        Mock<IFuturemud> game = new();
        Mock<IPerceiver> perceiver = new();
        const string input = "writing{lang,script}{hello}{alt}";
        Assert.AreEqual("hello", input.SubstituteWrittenLanguage(perceiver.Object, game.Object));
    }

    [TestMethod]
    public void SubstituteWrittenLanguage_MissingScriptToken_ReturnsPrimaryText()
    {
        Mock<IFuturemud> game = new();
        Mock<ILanguagePerceiver> perceiver = new();
        const string input = "writing{lang}{hello}{alt}";
        Assert.AreEqual("hello", input.SubstituteWrittenLanguage(perceiver.Object, game.Object));
    }

    [TestMethod]
    public void SubstituteWrittenLanguage_LanguageMissing_ReturnsPrimaryText()
    {
        Mock<IUneditableAll<ILanguage>> langRepo = new();
        langRepo.Setup(x => x.GetByName("lang")).Returns((ILanguage)null);
        Mock<IFuturemud> game = new();
        game.Setup(x => x.Languages).Returns(langRepo.Object);

        Mock<ILanguagePerceiver> perceiver = new();
        const string input = "writing{lang,script}{hello}{alt}";
        Assert.AreEqual("hello", input.SubstituteWrittenLanguage(perceiver.Object, game.Object));
    }

    [TestMethod]
    public void SubstituteWrittenLanguage_ScriptMissing_ReturnsPrimaryText()
    {
        Mock<ILanguage> language = new();
        language.Setup(x => x.Name).Returns("lang");
        Mock<IUneditableAll<ILanguage>> langRepo = new();
        langRepo.Setup(x => x.GetByName("lang")).Returns(language.Object);
        Mock<IUneditableAll<IScript>> scriptRepo = new();
        scriptRepo.Setup(x => x.GetByName("script")).Returns((IScript)null);
        Mock<IFuturemud> game = new();
        game.Setup(x => x.Languages).Returns(langRepo.Object);
        game.Setup(x => x.Scripts).Returns(scriptRepo.Object);

        Mock<ILanguagePerceiver> perceiver = new();
        const string input = "writing{lang,script}{hello}{alt}";
        Assert.AreEqual("hello", input.SubstituteWrittenLanguage(perceiver.Object, game.Object));
    }

    [TestMethod]
    public void SubstituteWrittenLanguage_SkillTooLow_UsesAlternateText()
    {
        Mock<ITraitDefinition> traitDef = new();
        Mock<ILanguage> language = new();
        language.Setup(x => x.Name).Returns("lang");
        language.Setup(x => x.LinkedTrait).Returns(traitDef.Object);
        Mock<IScript> script = new();
        script.Setup(x => x.Name).Returns("script");
        script.Setup(x => x.KnownScriptDescription).Returns("known");
        script.Setup(x => x.UnknownScriptDescription).Returns("unknown");
        Mock<IUneditableAll<ILanguage>> langRepo = new();
        langRepo.Setup(x => x.GetByName("lang")).Returns(language.Object);
        Mock<IUneditableAll<IScript>> scriptRepo = new();
        scriptRepo.Setup(x => x.GetByName("script")).Returns(script.Object);
        Mock<IColour> colour = new();
        colour.Setup(x => x.Name).Returns("white");
        Mock<IUneditableAll<IColour>> colourRepo = new();
        colourRepo.Setup(x => x.Get(0)).Returns(colour.Object);
        Mock<IFuturemud> game = new();
        game.Setup(x => x.Languages).Returns(langRepo.Object);
        game.Setup(x => x.Scripts).Returns(scriptRepo.Object);
        game.Setup(x => x.Colours).Returns(colourRepo.Object);
        game.Setup(x => x.GetStaticInt("DefaultWritingStyleInText")).Returns(0);
        game.Setup(x => x.GetStaticLong("DefaultWritingColourInText")).Returns(0);

        Mock<ITrait> trait = new();
        trait.Setup(x => x.Value).Returns(1);

        Mock<ILanguagePerceiver> perceiver = new();
        perceiver.Setup(x => x.Languages).Returns(new[] { language.Object });
        perceiver.Setup(x => x.Scripts).Returns(new[] { script.Object });
        perceiver.As<IHaveTraits>().Setup(x => x.GetTrait(traitDef.Object)).Returns(trait.Object);

        const string input = "writing{lang,script,skill>5}{hello}{alt}";
        string result = input.SubstituteWrittenLanguage(perceiver.Object, game.Object);
        Assert.IsTrue(result.Contains("alt", StringComparison.Ordinal));
        Assert.IsTrue(result.Contains("Skill not high enough to understand.", StringComparison.Ordinal));
    }

    [TestMethod]
    public void SubstituteWrittenLanguage_MinSkillAlias_UsesAlternateText()
    {
        Mock<ITraitDefinition> traitDef = new();
        Mock<ILanguage> language = new();
        language.Setup(x => x.Name).Returns("lang");
        language.Setup(x => x.LinkedTrait).Returns(traitDef.Object);
        Mock<IScript> script = new();
        script.Setup(x => x.Name).Returns("script");
        script.Setup(x => x.KnownScriptDescription).Returns("known");
        script.Setup(x => x.UnknownScriptDescription).Returns("unknown");
        Mock<IUneditableAll<ILanguage>> langRepo = new();
        langRepo.Setup(x => x.GetByName("lang")).Returns(language.Object);
        Mock<IUneditableAll<IScript>> scriptRepo = new();
        scriptRepo.Setup(x => x.GetByName("script")).Returns(script.Object);
        Mock<IColour> colour = new();
        colour.Setup(x => x.Name).Returns("white");
        Mock<IUneditableAll<IColour>> colourRepo = new();
        colourRepo.Setup(x => x.Get(0)).Returns(colour.Object);
        Mock<IFuturemud> game = new();
        game.Setup(x => x.Languages).Returns(langRepo.Object);
        game.Setup(x => x.Scripts).Returns(scriptRepo.Object);
        game.Setup(x => x.Colours).Returns(colourRepo.Object);
        game.Setup(x => x.GetStaticInt("DefaultWritingStyleInText")).Returns(0);
        game.Setup(x => x.GetStaticLong("DefaultWritingColourInText")).Returns(0);

        Mock<ITrait> trait = new();
        trait.Setup(x => x.Value).Returns(1);

        Mock<ILanguagePerceiver> perceiver = new();
        perceiver.Setup(x => x.Languages).Returns(new[] { language.Object });
        perceiver.Setup(x => x.Scripts).Returns(new[] { script.Object });
        perceiver.As<IHaveTraits>().Setup(x => x.GetTrait(traitDef.Object)).Returns(trait.Object);

        const string input = "writing{lang,script,minskill>5}{hello}{alt}";
        string result = input.SubstituteWrittenLanguage(perceiver.Object, game.Object);
        Assert.IsTrue(result.Contains("alt", StringComparison.Ordinal));
        Assert.IsTrue(result.Contains("Skill not high enough to understand.", StringComparison.Ordinal));
    }

    [TestMethod]
    public void SubstituteWrittenLanguage_KnownScriptUnknownLanguage_UsesUnknownLanguageHint()
    {
        Mock<ILanguage> language = new();
        language.Setup(x => x.Name).Returns("lang");
        Mock<IScript> script = new();
        script.Setup(x => x.Name).Returns("script");
        script.Setup(x => x.KnownScriptDescription).Returns("known");
        script.Setup(x => x.UnknownScriptDescription).Returns("unknown");
        Mock<IUneditableAll<ILanguage>> langRepo = new();
        langRepo.Setup(x => x.GetByName("lang")).Returns(language.Object);
        Mock<IUneditableAll<IScript>> scriptRepo = new();
        scriptRepo.Setup(x => x.GetByName("script")).Returns(script.Object);
        Mock<IColour> colour = new();
        colour.Setup(x => x.Name).Returns("white");
        Mock<IUneditableAll<IColour>> colourRepo = new();
        colourRepo.Setup(x => x.Get(0)).Returns(colour.Object);
        Mock<IFuturemud> game = new();
        game.Setup(x => x.Languages).Returns(langRepo.Object);
        game.Setup(x => x.Scripts).Returns(scriptRepo.Object);
        game.Setup(x => x.Colours).Returns(colourRepo.Object);
        game.Setup(x => x.GetStaticInt("DefaultWritingStyleInText")).Returns(0);
        game.Setup(x => x.GetStaticLong("DefaultWritingColourInText")).Returns(0);

        Mock<ILanguagePerceiver> perceiver = new();
        perceiver.Setup(x => x.Languages).Returns(Array.Empty<ILanguage>());
        perceiver.Setup(x => x.Scripts).Returns(new[] { script.Object });

        const string input = "writing{lang,script}{hello}{alt}";
        string result = input.SubstituteWrittenLanguage(perceiver.Object, game.Object);
        Assert.IsTrue(result.Contains("alt", StringComparison.Ordinal));
        Assert.IsTrue(result.Contains("Language: Unknown, Script: known", StringComparison.Ordinal));
    }

    [TestMethod]
    public void SubstituteWrittenLanguage_UnknownScript_UsesUnknownScriptHint()
    {
        Mock<ILanguage> language = new();
        language.Setup(x => x.Name).Returns("lang");
        Mock<IScript> script = new();
        script.Setup(x => x.Name).Returns("script");
        script.Setup(x => x.KnownScriptDescription).Returns("known");
        script.Setup(x => x.UnknownScriptDescription).Returns("unknown");
        Mock<IUneditableAll<ILanguage>> langRepo = new();
        langRepo.Setup(x => x.GetByName("lang")).Returns(language.Object);
        Mock<IUneditableAll<IScript>> scriptRepo = new();
        scriptRepo.Setup(x => x.GetByName("script")).Returns(script.Object);
        Mock<IColour> colour = new();
        colour.Setup(x => x.Name).Returns("white");
        Mock<IUneditableAll<IColour>> colourRepo = new();
        colourRepo.Setup(x => x.Get(0)).Returns(colour.Object);
        Mock<IFuturemud> game = new();
        game.Setup(x => x.Languages).Returns(langRepo.Object);
        game.Setup(x => x.Scripts).Returns(scriptRepo.Object);
        game.Setup(x => x.Colours).Returns(colourRepo.Object);
        game.Setup(x => x.GetStaticInt("DefaultWritingStyleInText")).Returns(0);
        game.Setup(x => x.GetStaticLong("DefaultWritingColourInText")).Returns(0);

        Mock<ILanguagePerceiver> perceiver = new();
        perceiver.Setup(x => x.Languages).Returns(new[] { language.Object });
        perceiver.Setup(x => x.Scripts).Returns(Array.Empty<IScript>());

        const string input = "writing{lang,script}{hello}{alt}";
        string result = input.SubstituteWrittenLanguage(perceiver.Object, game.Object);
        Assert.IsTrue(result.Contains("alt", StringComparison.Ordinal));
        Assert.IsTrue(result.Contains("Script: unknown", StringComparison.Ordinal));
    }

    [TestMethod]
    public void SubstituteWrittenLanguage_EmptyAlternate_UsesDefaultAlternate()
    {
        Mock<ILanguage> language = new();
        language.Setup(x => x.Name).Returns("lang");
        Mock<IScript> script = new();
        script.Setup(x => x.Name).Returns("script");
        script.Setup(x => x.KnownScriptDescription).Returns("known");
        script.Setup(x => x.UnknownScriptDescription).Returns("unknown");
        Mock<IUneditableAll<ILanguage>> langRepo = new();
        langRepo.Setup(x => x.GetByName("lang")).Returns(language.Object);
        Mock<IUneditableAll<IScript>> scriptRepo = new();
        scriptRepo.Setup(x => x.GetByName("script")).Returns(script.Object);
        Mock<IColour> colour = new();
        colour.Setup(x => x.Name).Returns("white");
        Mock<IUneditableAll<IColour>> colourRepo = new();
        colourRepo.Setup(x => x.Get(0)).Returns(colour.Object);
        Mock<IFuturemud> game = new();
        game.Setup(x => x.Languages).Returns(langRepo.Object);
        game.Setup(x => x.Scripts).Returns(scriptRepo.Object);
        game.Setup(x => x.Colours).Returns(colourRepo.Object);
        game.Setup(x => x.GetStaticInt("DefaultWritingStyleInText")).Returns(0);
        game.Setup(x => x.GetStaticLong("DefaultWritingColourInText")).Returns(0);
        game.Setup(x => x.GetStaticString("DefaultAlternateTextValue")).Returns("default-alt");

        Mock<ILanguagePerceiver> perceiver = new();
        perceiver.Setup(x => x.Languages).Returns(Array.Empty<ILanguage>());
        perceiver.Setup(x => x.Scripts).Returns(Array.Empty<IScript>());

        const string input = "writing{lang,script}{hello}";
        string result = input.SubstituteWrittenLanguage(perceiver.Object, game.Object);
        Assert.IsTrue(result.Contains("default-alt", StringComparison.Ordinal));
    }

    [TestMethod]
    public void IsValidFormatString_MandatoryMask_AllowsOptionalMissing()
    {
        const string format = "Value {0}";
        Span<bool> mandatory = new[] { true, false }.AsSpan();
        Assert.IsTrue(format.IsValidFormatString(2, mandatory));
    }
}

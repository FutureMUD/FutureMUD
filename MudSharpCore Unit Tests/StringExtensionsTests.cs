using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StringExtensionsTests
{
    [TestMethod]
    public void SquareBrackets_AddsBrackets()
    {
        Assert.AreEqual("[abc]", "abc".SquareBrackets());
    }

    [TestMethod]
    public void StarRectangle_PadsAndBrackets()
    {
        Assert.AreEqual("[ab***   ]", "ab".StarRectangle(3, 8));
    }

    [TestMethod]
    public void Parentheses_VariousCases()
    {
        Assert.AreEqual("(test)", "test".Parentheses());
        Assert.AreEqual("test", "test".Parentheses(false));
    }

    [TestMethod]
    public void ParenthesesIfNot_OnlyWhenMissing()
    {
        Assert.AreEqual("(word)", "word".ParenthesesIfNot());
        Assert.AreEqual("(word)", "(word)".ParenthesesIfNot());
        Assert.AreEqual("word", "word".ParenthesesIfNot(false));
    }

    [TestMethod]
    public void ParenthesesSpace_AddsTrailingSpace()
    {
        Assert.AreEqual("(abc) ", "abc".ParenthesesSpace());
    }

    [TestMethod]
    public void ParenthesesSpacePrior_AddsLeadingSpace()
    {
        Assert.AreEqual(" (abc)", "abc".ParenthesesSpacePrior());
    }

    [TestMethod]
    public void FluentAppend_AppendsWhenTrue()
    {
        Assert.AreEqual("hello!", "hello".FluentAppend("!", true));
        Assert.AreEqual("hello", "hello".FluentAppend("!", false));
    }

    [TestMethod]
    public void DoubleQuotes_Optional()
    {
        Assert.AreEqual("\"hi\"", "hi".DoubleQuotes());
        Assert.AreEqual("hi", "hi".DoubleQuotes(false));
    }

    [TestMethod]
    public void RemoveFirstCharacter_Basic()
    {
        Assert.AreEqual("bc", "abc".RemoveFirstCharacter());
        Assert.AreEqual("", "a".RemoveFirstCharacter());
    }

    [TestMethod]
    public void RemoveLastCharacter_Basic()
    {
        Assert.AreEqual("ab", "abc".RemoveLastCharacter());
        Assert.AreEqual("", "x".RemoveLastCharacter());
    }

    [TestMethod]
    public void RemoveFirstWord_Basic()
    {
        Assert.AreEqual("bar baz", "foo bar baz".RemoveFirstWord());
        Assert.AreEqual("", "single".RemoveFirstWord());
    }

    [TestMethod]
    public void ParseSpecialCharacters_ParsesKnownEscapes()
    {
        Assert.AreEqual("\n", "\\n".ParseSpecialCharacters());
        Assert.AreEqual("\t", "\\t".ParseSpecialCharacters());
        Assert.AreEqual("\"", "\\\"".ParseSpecialCharacters());
        Assert.AreEqual("\\", "\\\\".ParseSpecialCharacters());
    }

    [TestMethod]
    public void Fullstop_AddsWhenMissing()
    {
        Assert.AreEqual("hello.", "hello".Fullstop());
        Assert.AreEqual("hi!", "hi!".Fullstop());
    }

    [TestMethod]
    public void Proper_CapitalisesFirstLetter()
    {
        Assert.AreEqual("Hello", "hello".Proper());
        Assert.AreEqual("", "".Proper());
    }

    [TestMethod]
    public void ReplaceFirst_StringOverload()
    {
        Assert.AreEqual("fooYYbarXX", "fooXXbarXX".ReplaceFirst("XX","YY"));
        Assert.AreEqual("foobar", "foobar".ReplaceFirst("XX","YY"));
    }

    [TestMethod]
    public void ReplaceFirst_CharOverload()
    {
        Assert.AreEqual("foZbar", "foobar".ReplaceFirst('o','Z'));
        Assert.AreEqual("abc", "abc".ReplaceFirst('x','y'));
    }

    [TestMethod]
    public void TitleCase_CapitalisesWords()
    {
        Assert.AreEqual("Hello World", "hello world".TitleCase());
    }

    [TestMethod]
    public void FluentProper_Conditional()
    {
        Assert.AreEqual("Hello", "hello".FluentProper(true));
        Assert.AreEqual("hello", "hello".FluentProper(false));
    }

    [TestMethod]
    public void IsInteger_ChecksParse()
    {
        Assert.IsTrue("123".IsInteger());
        Assert.IsFalse("12a".IsInteger());
    }

    [TestMethod]
    public void HasWord_SearchesList()
    {
        var words = new List<string>{"foo","bar"};
        Assert.IsTrue(words.HasWord("foo"));
        Assert.IsFalse(words.HasWord("f", abbreviated:false));
        Assert.IsTrue(words.HasWord(new[]{"foo","bar"}));
    }

    [TestMethod]
    public void IfEmpty_ReturnsAlternate()
    {
        Assert.AreEqual("hi", "".IfEmpty("hi"));
        Assert.AreEqual("bye", "bye".IfEmpty("hi"));
    }

    [TestMethod]
    public void ConcatVariants_WorkCorrectly()
    {
        Assert.AreEqual("a b", "a".SpaceIfNotEmpty());
        Assert.AreEqual(" a", "a".LeadingSpaceIfNotEmpty());
        Assert.AreEqual("abc", "ab".ConcatIfNotEmpty("c"));
        Assert.AreEqual("cde", "de".LeadingConcatIfNotEmpty("c"));
    }

    [TestMethod]
    public void EqualTo_CaseOptions()
    {
        Assert.IsTrue("abc".EqualTo("ABC"));
        Assert.IsFalse("abc".EqualTo("ABC", false));
    }

    [TestMethod]
    public void EqualToAny_ChecksCollection()
    {
        Assert.IsTrue("a".EqualToAny("b","A"));
        Assert.IsFalse("a".EqualToAny("b","c"));
    }

    [TestMethod]
    public void Strip_RemovesChars()
    {
        Assert.AreEqual("ace", "abcde".Strip(c => c=='b' || c=='d'));
    }

    [TestMethod]
    public void AppendPrepend_Works()
    {
        Assert.AreEqual("abc!", "abc".Append("!"));
        Assert.AreEqual("!abc", "abc".Prepend("!"));
    }

    [TestMethod]
    public void NormaliseSpacing_RemovesExtra()
    {
        Assert.AreEqual("a b c", "a  b   c".NormaliseSpacing());
    }

    [TestMethod]
    public void IfNullOrWhiteSpace_ReturnsFallback()
    {
        Assert.AreEqual("x", "".IfNullOrWhiteSpace("x"));
        Assert.AreEqual("y", "y".IfNullOrWhiteSpace("x"));
    }

    [TestMethod]
    public void RawText_StripsAnsiAndMxp()
    {
        var text = $"{Telnet.RED}abc{Telnet.RESET}{MXP.BeginMXP}tag{MXP.EndMXP}d";
        Assert.AreEqual("abcd", text.RawText());
    }

    [TestMethod]
    public void RawTextLength_IgnoresCodes()
    {
        var text = $"{Telnet.RED}abc{Telnet.RESET}";
        Assert.AreEqual(3, text.RawTextLength());
    }

    [TestMethod]
    public void RawTextSubstring_HandlesCodes()
    {
        var text = $"{Telnet.RED}abc{Telnet.RESET}de";
        Assert.AreEqual("cd", text.RawTextSubstring(2,2));
    }

    [TestMethod]
    public void RawTextPadLeftRight_PadsProperly()
    {
        var raw = $"{Telnet.RED}ab{Telnet.RESET}";
        Assert.IsTrue(raw.RawTextPadLeft(4).EndsWith(raw));
        Assert.IsTrue(raw.RawTextPadRight(4).StartsWith(raw));
    }

    [TestMethod]
    public void Sanitise_EscapesBraces()
    {
        Assert.AreEqual("{{x}}", "{x}".Sanitise());
    }

    [TestMethod]
    public void SanitiseExceptNumbered_AllowsWithinLimit()
    {
        Assert.AreEqual("{0}", "{0}".SanitiseExceptNumbered(1));
        Assert.AreEqual("{{2}}", "{2}".SanitiseExceptNumbered(1));
    }

    [TestMethod]
    public void SplitCamelCase_SplitsWords()
    {
        Assert.AreEqual("Camel Case", "CamelCase".SplitCamelCase());
        Assert.AreEqual("CamelCase", "CamelCase".SplitCamelCase(false));
    }

    [TestMethod]
    public void IncrementNumberOrAddNumber_Behaviour()
    {
        Assert.AreEqual("item1", "item".IncrementNumberOrAddNumber());
        Assert.AreEqual("item6", "item5".IncrementNumberOrAddNumber());
    }

    [TestMethod]
    public void NameOrAppendNumberToName_AddsNext()
    {
        var list = new List<string>{"item","item1","item3"};
        Assert.AreEqual("item4", list.NameOrAppendNumberToName("item"));
        Assert.AreEqual("new", list.NameOrAppendNumberToName("new"));
    }

    [TestMethod]
    public void ToColouredString_ReturnsColourCodes()
    {
        Assert.AreEqual($"True".Colour(Telnet.Green), true.ToColouredString());
        Assert.AreEqual($"False".Colour(Telnet.Red), false.ToColouredString());
    }

    [TestMethod]
    public void BonusFormatting_Works()
    {
        Assert.AreEqual($"+1.00%".Colour(Telnet.Green), 0.01.ToBonusPercentageString());
        Assert.AreEqual($"-1".Colour(Telnet.Red), (-1).ToBonusString());
    }

    [TestMethod]
    public void Wrap_Simple()
    {
        var wrapped = "a b c d".Wrap(3);
        Assert.IsTrue(wrapped.Contains("\n"));
    }

    [TestMethod]
    public void NoWrap_PrefixesChar()
    {
        Assert.AreEqual($"{Telnet.NoWordWrap}text", "text".NoWrap());
    }

    [TestMethod]
    public void NormaliseOutputSentences_Capitalises()
    {
        var result = "hello world. this is a test.".NormaliseOutputSentences();
        Assert.AreEqual("Hello world. This is a test.", result);
    }

    [TestMethod]
    public void ConvertEncodings_Work()
    {
        var latin1 = "café".ConvertToLatin1();
        Assert.IsNotNull(latin1);
        var ascii = "café".ConvertToAscii();
        Assert.IsTrue(ascii.Contains("cafe"));
    }

    [TestMethod]
    public void ToTitleCaseAP_FormatsAccordingToRules()
    {
        var result = "the quick brown fox jumps over the lazy dog".ToTitleCaseAP();
        Assert.AreEqual("The Quick Brown Fox Jumps over the Lazy Dog", result);
    }
}

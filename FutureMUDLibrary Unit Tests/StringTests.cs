using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MudSharp_Unit_Tests;
/// <summary>
/// Summary description for StringTests
/// </summary>
[TestClass]
public class StringTests
{
	public StringTests()
	{
	}

	private TestContext testContextInstance;

	/// <summary>
	///Gets or sets the test context which provides
	///information about and functionality for the current test run.
	///</summary>
	public TestContext TestContext
	{
		get
		{
			return testContextInstance;
		}
		set
		{
			testContextInstance = value;
		}
	}

	#region Additional test attributes
	//
	// You can use the following additional attributes as you write your tests:
	//
	// Use ClassInitialize to run code before running the first test in the class
	// [ClassInitialize()]
	// public static void MyClassInitialize(TestContext testContext) { }
	//
	// Use ClassCleanup to run code after all tests in a class have run
	// [ClassCleanup()]
	// public static void MyClassCleanup() { }
	//
	// Use TestInitialize to run code before running each test 
	// [TestInitialize()]
	// public void MyTestInitialize() { }
	//
	// Use TestCleanup to run code after each test has run
	// [TestCleanup()]
	// public void MyTestCleanup() { }
	//
	#endregion

	[TestMethod]
	public void TestExplodedStrings()
	{
		var explode1 = new ExplodedString("This is a simple sentence to explode.");
		var explode2 = new ExplodedString("This one-has some \"nuances involved\"");
		var explode3 = new ExplodedString("a simple leather-bound book titled \"Grave Mistakes\"");
		var explode4 = new ExplodedString("");

		Assert.AreEqual(7, explode1.Words.Count);
		Assert.AreEqual("simple", explode1.Words[3]);
		Assert.AreEqual("explode", explode1.Words[6]);

		Assert.AreEqual(8, explode3.Words.Count);
		Assert.AreEqual("bound", explode3.Words[3]);
		Assert.AreEqual("book", explode3.Words[4]);
		Assert.AreEqual("Grave", explode3.Words[6]);
		Assert.AreEqual("Mistakes", explode3.Words[7]);
	}

	[TestMethod]
	public void TestColumns()
	{
		var input =
@"Item 1
Item 2
Item 3
Item 4
Item 5
Item 6
Item 7
Item 8
Item 9
Item 10
Item 11";

		var expected =
@"Item 1     Item 7   
Item 2     Item 8   
Item 3     Item 9   
Item 4     Item 10  
Item 5     Item 11  
Item 6     
";

		var actual = input.SplitTextIntoColumns(2, 20, 2);

				var separators = new[] { "\r\n", "\n" };
		var expectedLines = expected.Split(separators, StringSplitOptions.None)
			.Select(line => line.TrimEnd())
			.ToList();
		var actualLines = actual.Split(separators, StringSplitOptions.None)
			.Select(line => line.TrimEnd())
			.ToList();

		CollectionAssert.AreEqual(expectedLines, actualLines, $"Actual Output: [{actual}]");
	}

	[TestMethod]
	public void TestSanitisation()
	{
		var input =
			"this is some test player input containing a { character and also the double of that, which is {{.";
		Assert.AreEqual("this is some test player input containing a {{ character and also the double of that, which is {{{{.", input.Sanitise(), "The two strings were not equal.");
	}

	[TestMethod]
	public void TestProperSentences()
	{
		var noChangeText1 = "This text does not have more than one sentence.";
		var noChangeText2 = "This text does has more than one sentence. This is the 2nd sentence.";
		var capitaliseText = "This text does has more than one sentence! this is the 2nd sentence.";
		var ansiText = "This text \x1B[31mhas colour\x1B[0m. and a second sentence.\x1B[31m Which starts\x1B[0m with colour.";
		var mxpText = "This text has two sentences. the \x03italic\x04second sentence\x03/italic\x04 has italics in a bit. Third sentence!";

		Assert.AreEqual("This text does not have more than one sentence.", noChangeText1.ProperSentences());
		Assert.AreEqual("This text does has more than one sentence. This is the 2nd sentence.", noChangeText2.ProperSentences());
		Assert.AreEqual("This text does has more than one sentence! This is the 2nd sentence.", capitaliseText.ProperSentences());
		Assert.AreEqual("This text \x1B[31mhas colour\x1B[0m. And a second sentence.\x1B[31m Which starts\x1B[0m with colour.", ansiText.ProperSentences());
		Assert.AreEqual("This text has two sentences. The \x03italic\x04second sentence\x03/italic\x04 has italics in a bit. Third sentence!", mxpText.ProperSentences());
	}

	[TestMethod]
	public void TestNewProperSentences()
	{
		var noChangeText1 = "This text does not have more than one sentence.";
		var noChangeText2 = "This text does has more than one sentence. This is the 2nd sentence.";
		var capitaliseText = "This text does has more than one sentence!. this is the 2nd sentence.";
		var ansiText = "This text \x1B[31mhas colour\x1B[0m. and a second sentence...\x1B[31m Which starts\x1B[0m with colour.";
		var mxpText = "This text has two sentences. the \x03italic\x04second sentence\x03/italic\x04 has italics in a bit. Third sentence!";

		Assert.AreEqual("This text does not have more than one sentence.", noChangeText1.NormaliseOutputSentences());
		Assert.AreEqual("This text does has more than one sentence. This is the 2nd sentence.", noChangeText2.NormaliseOutputSentences());
		Assert.AreEqual("This text does has more than one sentence! This is the 2nd sentence.", capitaliseText.NormaliseOutputSentences());
		Assert.AreEqual("This text \x1B[31mhas colour\x1B[0m. And a second sentence...\x1B[31m Which starts\x1B[0m with colour.", ansiText.NormaliseOutputSentences());
		Assert.AreEqual("This text has two sentences. The \x03italic\x04second sentence\x03/italic\x04 has italics in a bit. Third sentence!", mxpText.NormaliseOutputSentences());

		Assert.AreEqual("Here is some text...with some ellipses", "Here is some text...with some ellipses".NormaliseOutputSentences());
		Assert.AreEqual("Here is some text.With some ellipses", "Here is some text..with some ellipses".NormaliseOutputSentences());
		Assert.AreEqual("Here is some text--with some dashes", "Here is some text--with some dashes".NormaliseOutputSentences());
		Assert.AreEqual("Here is some text?! With an interrobang.", "Here is some text?! with an interrobang.".NormaliseOutputSentences());
	}

	[TestMethod]
	public void TestStringListFunctions()
	{
		var list1 = new List<string> { "badger", "tycoon", "crumpet", "handle" };
		Assert.AreEqual("item", list1.NameOrAppendNumberToName("item"), $"Assertion #1: Expected text \"item\" from NameOrAppendNumberToName, but got \"{list1.NameOrAppendNumberToName("item")}\" instead.");

		var list2 = new List<string> { "badger", "tycoon", "crumpet", "handle", "item" };
		Assert.AreEqual("item1", list2.NameOrAppendNumberToName("item"), $"Assertion #2: Expected text \"item1\" from NameOrAppendNumberToName, but got \"{list2.NameOrAppendNumberToName("item")}\" instead.");
		Assert.AreEqual("Item1", list2.NameOrAppendNumberToName("Item"), $"Assertion #3: Expected text \"Item1\" from NameOrAppendNumberToName, but got \"{list2.NameOrAppendNumberToName("Item")}\" instead.");

		var list3 = new List<string> { "badger", "item1", "Item3", "tycoon", "crumpet", "handle", "item" };
		Assert.AreEqual("item4", list3.NameOrAppendNumberToName("item"), $"Assertion #4: Expected text \"item4\" from NameOrAppendNumberToName, but got \"{list3.NameOrAppendNumberToName("item")}\" instead.");
	}

	[TestMethod]
	public void TestStringCasingFunctions()
	{
		Assert.AreEqual("This is how I expect the string to look.", "this is how I expect the string to look.".Proper(), "Failure of Proper() function with single sentence.");
		Assert.AreEqual("This is how I expect the string to look.", "this is how I expect the string to look.".FluentProper(true), "Failure of FluentProper(true) function with single sentence.");
		Assert.AreEqual("This is how I expect the string to look. It has multiple sentences! Some of them have, non-standard punctuation.", "this is how I expect the string to look. it has multiple sentences! some of them have, non-standard punctuation.".ProperSentences(), "Failure of ProperSentences() function.");
		Assert.AreEqual("How Much Wood Could A Wood Chuck Chuck", "how much wood could a wood chuck chuck".TitleCase(), "Title Case Failure");
	}

	[TestMethod]
	public void TestMXPAnsiTextCode()
	{
		var dirtyAnsiString = "\x1B[32mAn example item with big shiny lights\x1B[0m is here, doing a whole bunch of Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
		var dirtyAnsiMXPString = "\x1B[32mAn example item with big shiny lights\x1B[0m is here, doing a \x03send href=\x06dance monkey\x06\x04whole bunch\x03/send\x04 of Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
		var dirtyAnsiMXPString2 = "\x1B[32mAn example item with big shiny lights\x1B[0m is here, doing a \x03send href=\x06dance monkey\x06\x04whole bunch of Lorem ipsum\x03/send\x04 dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

		Assert.AreEqual(dirtyAnsiString.RawTextLength(), dirtyAnsiMXPString.RawTextLength());
		Assert.AreEqual(dirtyAnsiString.Wrap(80), dirtyAnsiMXPString.Wrap(80).SanitiseMXP(new MXPSupport()));
		Assert.AreEqual(dirtyAnsiString.Wrap(80), dirtyAnsiMXPString2.Wrap(80).SanitiseMXP(new MXPSupport()));
		Trace.WriteLine(dirtyAnsiMXPString2.Wrap(80));
	}

	[TestMethod]
	public void TestStringSplittingFunctions()
	{
		Assert.AreEqual("This Is A Camel Case String", "ThisIsACamelCaseString".SplitCamelCase());
		Assert.AreEqual("Another Camel Case String", "AnotherCamelCaseString".SplitCamelCase());
	}

	[TestMethod]
	public void TestListToString()
	{
		var list1 = new[] { "one" };
		var list2 = new[] { "one", "two" };
		var list3 = new[] { "one", "two", "three" };
		var list4 = new[] { "one", "two", "three", "four", "two", "two", "three" };

		Assert.AreEqual(list1.ListToString(), "one", "Expected simply first item in list for single item collection");
		Assert.AreEqual(list1.ListToString(article: "the "), "the one", "Expected article to be applied correctly for single item collection");

		Assert.AreEqual(list2.ListToString(), "one and two", "Expected simple two item list to be x and y");
		Assert.AreEqual(list2.ListToString(conjunction: "or "), "one or two", "Expected article changed two item list to be x or y");

		Assert.AreEqual(list3.ListToString(), "one, two, and three", "Expected simple three item list to be in a particular format");
		Assert.AreEqual(list3.ListToString(oxfordComma: false, conjunction: " and "), "one, two and three", "Expected oxford comma altered three item list to be in a particular format");

		Assert.AreEqual(list4.ListToString(), "one, two, three, four, two, two, and three", "Expected long list to be in particular format");
		Assert.AreEqual(list4.ListToCompactString(), "one, two (x3), three (x2), and four", "Expected long list to be in particular format");
	}

	[TestMethod]
	public void TestListToStringPerformance()
	{
		var list = new[] { "one", "two", "three", "four", "two", "two", "three" };
		var sw = new Stopwatch();
		string result;
		GC.Collect();
		if (GC.TryStartNoGCRegion(65536, false))
		{
			try
			{
				sw.Start();
				result = list.ListToString();
				sw.Stop();
			}
			finally
			{
				GC.EndNoGCRegion();
			}
			Trace.WriteLine($"ListToString duration: {sw.ElapsedTicks:N0} ticks, {sw.ElapsedMilliseconds:N0}ms");
			Assert.IsTrue(sw.ElapsedMilliseconds < 5, "ListToString took 5ms or more to run.");
		}
		else
		{
			Assert.Inconclusive("Could not enter no garbage collection mode.");
		}
		if (GC.TryStartNoGCRegion(65536, false))
		{
			try
			{
				sw.Restart();
				result = list.ListToCompactString();
				sw.Stop();
			}
			finally
			{
				GC.EndNoGCRegion();
			}
			Trace.WriteLine($"ListToCompactString duration: {sw.ElapsedTicks:N0} ticks, {sw.ElapsedMilliseconds:N0}ms");
			Assert.IsTrue(sw.ElapsedMilliseconds < 5, "ListToCompactString took 5ms or more to run.");
		}
		else
		{
			Assert.Inconclusive("Could not enter no garbage collection mode.");
		}
	}

	[TestMethod]
	public void TestStringStack()
	{
		var ssEmpty = new StringStack("");
		var ssThreePlainArgs = new StringStack("one two three");
		var ssThreePlainArgs2 = new StringStack("one two three");
		var ssNonPlainArgs = new StringStack("one \"two three\" (four five six)");
		var ssNonPlainArgs2 = new StringStack("one \"two three\" (four five six)");
		var ssTestRemainingArg = new StringStack("one two \"three four five\" six");
		var ssUnbalanced = new StringStack("this \"stringstack has\" \"unbalanced quotes");
		var ssSafeRemaining = new StringStack(@"one two ""three four five""");

		Assert.IsFalse(ssSafeRemaining.IsFinished, "SafeRemaining StringStack started with true IsFinished");
		Assert.AreEqual("one", ssSafeRemaining.PopSpeech(), $"SafeRemaining StringStack expected value \"one\" but instead got \"{ssSafeRemaining.Last}\"");
		Assert.AreEqual("two", ssSafeRemaining.PopSpeech(), $"SafeRemaining StringStack expected value \"two\" but instead got \"{ssSafeRemaining.Last}\"");
		Assert.AreEqual("three four five", ssSafeRemaining.SafeRemainingArgument, $"SafeRemaining StringStack expected value \"three four five\" but instead got \"{ssSafeRemaining.Last}\"");
		Assert.AreEqual("two", ssSafeRemaining.Last, $"SafeRemaining StringStack expected value \"two\" but instead got \"{ssSafeRemaining.Last}\"");
		Assert.IsFalse(ssSafeRemaining.IsFinished, "SafeRemaining StringStack finished with true IsFinished");

		Assert.IsFalse(ssUnbalanced.IsFinished, "Unbalanced StringStack started with true IsFinished");
		Assert.AreEqual("this", ssUnbalanced.PopSpeech(), $"Unbalanced StringStack expected value \"this\" but instead got \"{ssUnbalanced.Last}\"");
		Assert.AreEqual("stringstack has", ssUnbalanced.PopSpeech(), $"Unbalanced StringStack expected value \"stringstack has\" but instead got \"{ssUnbalanced.Last}\"");
		Assert.AreEqual("\"unbalanced", ssUnbalanced.PopSpeech(), $"Unbalanced StringStack expected value \"\"unbalanced\" but instead got \"{ssUnbalanced.Last}\"");
		Assert.AreEqual("quotes", ssUnbalanced.PopSpeech(), $"Unbalanced StringStack expected value \"quotes\" but instead got \"{ssUnbalanced.Last}\"");
		Assert.IsTrue(ssUnbalanced.IsFinished, "Unbalanced StringStack finished with false IsFinished");

		Assert.IsTrue(ssEmpty.IsFinished, "Empty StringStack had false IsFinished.");
		Assert.AreEqual(string.Empty, ssEmpty.Pop(), $"Empty StringStack had non-empty Pop() result: {ssEmpty.Last}");
		Assert.AreEqual(string.Empty, ssEmpty.PopSpeech(), $"Empty StringStack had non-empty PopSpeech() result: {ssEmpty.Last}");

		Assert.IsFalse(ssThreePlainArgs.IsFinished, "ThreePlainArgs StringStack started with true IsFinished.");
		Assert.AreEqual("one", ssThreePlainArgs.Pop(), $"ThreePlainArgs StringStack expected value \"one\" but instead got \"{ssThreePlainArgs.Last}\".");
		Assert.AreEqual("two", ssThreePlainArgs.Pop(), $"ThreePlainArgs StringStack expected value \"two\" but instead got \"{ssThreePlainArgs.Last}\".");
		Assert.AreEqual("three", ssThreePlainArgs.Pop(), $"ThreePlainArgs StringStack expected value \"three\" but instead got \"{ssThreePlainArgs.Last}\".");
		Assert.IsTrue(ssThreePlainArgs.IsFinished, "ThreePlainArgs StringStack finished with false IsFinished.");

		var undo = ssThreePlainArgs.GetUndo();
		Assert.AreEqual("\"three\"", undo.Pop(), $"Undo StringStack expected value \"three\" but instead got \"{undo.Last}\".");

		Assert.AreEqual("one", ssThreePlainArgs2.PopSpeech(), $"ThreePlainArgs2 StringStack expected value \"one\" but instead got \"{ssThreePlainArgs2.Last}\".");
		Assert.AreEqual("two", ssThreePlainArgs2.PopSpeech(), $"ThreePlainArgs2 StringStack expected value \"two\" but instead got \"{ssThreePlainArgs2.Last}\".");
		Assert.AreEqual("three", ssThreePlainArgs2.PopSpeech(), $"ThreePlainArgs2 StringStack expected value \"three\" but instead got \"{ssThreePlainArgs2.Last}\".");
		Assert.IsTrue(ssThreePlainArgs2.IsFinished, "ThreePlainArgs2 StringStack finished with false IsFinished.");

		Assert.IsFalse(ssNonPlainArgs.IsFinished, "NonPlainArgs StringStack started with true IsFinished.");
		Assert.AreEqual("one", ssNonPlainArgs.PopSpeech(), $"NonPlainArgs StringStack expected value \"one\" but instead got \"{ssNonPlainArgs.Last}\".");
		Assert.AreEqual("two three", ssNonPlainArgs.PopSpeech(), $"NonPlainArgs StringStack expected value \"two three\" but instead got \"{ssNonPlainArgs.Last}\".");
		Assert.AreEqual(string.Empty, ssNonPlainArgs.PopSafe(), $"NonPlainArgs StringStack expected value string.Empty but instead got \"{ssNonPlainArgs.Last}\".");
		Assert.AreEqual("four five six", ssNonPlainArgs.PopParentheses(), $"NonPlainArgs StringStack expected value \"four five six\" but instead got \"{ssNonPlainArgs.Last}\".");
		Assert.IsTrue(ssNonPlainArgs.IsFinished, "NonPlainArgs StringStack finished with false IsFinished.");

		Assert.IsFalse(ssNonPlainArgs2.IsFinished, "NonPlainArgs2 StringStack started with true IsFinished.");
		Assert.AreEqual("one", ssNonPlainArgs2.PopSpeech(), $"NonPlainArgs2 StringStack expected value \"one\" but instead got \"{ssNonPlainArgs2.Last}\".");
		Assert.AreEqual("two three (four five six)", ssNonPlainArgs2.SafeRemainingArgument, $"NonPlainArgs2 StringStack expected value \"two three (four five six)\" but instead got \"{ssNonPlainArgs2.Last}\".");
		Assert.IsFalse(ssNonPlainArgs2.IsFinished, "NonPlainArgs2 StringStack finished with true IsFinished.");

		//var ssTestRemainingArg = new StringStack("one two \"three four five\" six");
		Assert.AreEqual("one", ssTestRemainingArg.PopSpeech(), $"TestRemainingArg StringStack expected value \"one\" but instead got \"{ssTestRemainingArg.Last}\".");
		Assert.AreEqual("two \"three four five\" six", ssTestRemainingArg.RemainingArgument, $"TestRemainingArg StringStack expected value \"two \"three four five\" six\" but instead got \"{ssTestRemainingArg.Last}\".");
		Assert.AreEqual("two three four five six", ssTestRemainingArg.SafeRemainingArgument, $"TestRemainingArg StringStack expected value \"two three four five six\" but instead got \"{ssTestRemainingArg.Last}\".");

	}

	[TestMethod]
	public void TestRegexTransform()
	{
		Assert.IsTrue(new Regex("colou?r1").TransformRegexIntoPattern().EqualTo("colour1"));
		Assert.IsTrue(new Regex("^colou?r1").TransformRegexIntoPattern().EqualTo("colour1"));
		Assert.IsTrue(new Regex("skin(colou?r|tone)").TransformRegexIntoPattern().EqualTo("skincolour"));
	}

	[TestMethod]
	public void IsValidFormatString_WithValidStrings_ReturnsTrue()
	{
		// Arrange
		string validFormat1 = "This is a test: {0}, {1:N0}";
		string validFormat2 = "Just a single parameter: {0} works fine.";
		string validFormat3 = "No parameters works fine too.";

		// Act
		bool result1 = validFormat1.IsValidFormatString(2);
		bool result2 = validFormat2.IsValidFormatString(1);
		bool result3 = validFormat3.IsValidFormatString(0);

		// Assert
		Assert.IsTrue(result1, "Expected to return true for valid format string with 2 parameters.");
		Assert.IsTrue(result2, "Expected to return true for valid format string with 1 parameter.");
		Assert.IsTrue(result3, "Expected to return true for valid format string with no parameters.");
	}

	[TestMethod]
	public void IsValidFormatString_WithInvalidStrings_ReturnsFalse()
	{
		// Arrange
		string invalidFormat1 = "This is a test: {0}, {2:N0}";
		string invalidFormat2 = "This {2} is out of range for a single parameter.";
		string invalidFormat3 = "This is {1} missing.";

		// Act
		bool result1 = invalidFormat1.IsValidFormatString(2);
		bool result2 = invalidFormat2.IsValidFormatString(1);
		bool result3 = invalidFormat3.IsValidFormatString(2);

		// Assert
		Assert.IsFalse(result1, "Expected to return false for invalid format string with a missing {1}.");
		Assert.IsFalse(result2, "Expected to return false for invalid format string with out of range parameter.");
		Assert.IsFalse(result3, "Expected to return false for invalid format string with skipped parameter.");
	}

	[TestMethod]
	public void IsValidFormatString_WithEdgeCases_ReturnsCorrectly()
	{
		// Arrange
		string edgeCaseFormat1 = "Edge case with escaped braces: {{1}} {0}";
		string edgeCaseFormat2 = "Edge case with double usage: {0} {0:N0}";

		// Act
		bool result1 = edgeCaseFormat1.IsValidFormatString(1);
		bool result2 = edgeCaseFormat2.IsValidFormatString(1);

		// Assert
		Assert.IsTrue(result1, "Expected to return true for valid format string with escaped braces.");
		Assert.IsTrue(result2, "Expected to return true for valid format string with double usage of the same parameter.");
	}

	[TestMethod]
	public void IsValidFormatString_WithTruthMask()
	{
		// Arrange
		var format1 = "This text has {0} and {2} as mandatory";
		var format2 = "This text has {0}, {1}, {2}, {3}";
		var format3 = "This text has no parameters";
		var format4 = "This text has {0} and {{1}}";

		// Act
		var result1 = format1.IsValidFormatString(new bool[] { true, false, true }.AsSpan());
		var result2 = format1.IsValidFormatString(new bool[] { false, false, true }.AsSpan());
		var result3 = format1.IsValidFormatString(new bool[] { true, true, true }.AsSpan());
		var result4 = format2.IsValidFormatString(new bool[] { true, true, true, true }.AsSpan());
		var result5 = format2.IsValidFormatString(new bool[] { true, true, true, true, true }.AsSpan());
		var result6 = format2.IsValidFormatString(new bool[] { true, true, true, true, false }.AsSpan());
		var result7 = format3.IsValidFormatString(new bool[] { }.AsSpan());
		var result8 = format3.IsValidFormatString(new bool[] { false, false }.AsSpan());
		var result9 = format4.IsValidFormatString(new bool[] { true }.AsSpan());
		var result10 = format4.IsValidFormatString(new bool[] { true, false }.AsSpan());
		var result11 = format4.IsValidFormatString(new bool[] { true, true }.AsSpan());

		// Assert
		Assert.IsTrue(result1, "Expected to return true for case 1");
		Assert.IsTrue(result2, "Expected to return true for case 2");
		Assert.IsFalse(result3, "Expected to return false for case 3");
		Assert.IsTrue(result4, "Expected to return true for case 4");
		Assert.IsFalse(result5, "Expected to return false for case 5");
		Assert.IsTrue(result6, "Expected to return true for case 6");
		Assert.IsTrue(result7, "Expected to return true for case 7");
		Assert.IsTrue(result8, "Expected to return true for case 8");
		Assert.IsTrue(result9, "Expected to return true for case 9");
		Assert.IsTrue(result10, "Expected to return true for case 10");
		Assert.IsFalse(result11, "Expected to return false for case 11");
	}

	[TestMethod]
	public void TestConvertToAscii()
	{
		
        Assert.AreEqual("Tuile", "Tuil\u00E9".ConvertToAscii());
        Assert.AreEqual("Yavie", "Y\u0101vi\u00E9".ConvertToAscii());
        Assert.AreEqual("Aryabhata", "Aryabha\u1E6Da".ConvertToAscii());
		Assert.AreEqual("Nitobe Inazo", "Nitobe Inazo".ConvertToAscii()); 
		Assert.AreEqual("Gis", "Gis".ConvertToAscii());
        Assert.AreEqual("Cafe", "Caf\u00E9".ConvertToAscii());
	}

	[TestMethod]
	public void TestConvertToLatin1()
	{
        Assert.AreEqual("2 Tuil\u00E9", "2 Tuil\u00E9".ConvertToLatin1());
        Assert.AreEqual("2 Yavi\u00E9", "2 Y\u0101vi\u00E9".ConvertToLatin1());
        Assert.AreEqual("2 Aryabhata", "2 Aryabha\u1E6Da".ConvertToLatin1());
		Assert.AreEqual("2 Nitobe Inazo", "2 Nitobe Inazo".ConvertToLatin1());
		Assert.AreEqual("2 Gis", "2 Gis".ConvertToLatin1());
        Assert.AreEqual("2 Caf\u00E9", "2 Caf\u00E9".ConvertToLatin1());

        var unicodeTable = "\u2554\u2550\u2557\n\u2551 ID \u2551\n\u255a\u2550\u255d";
        var asciiTable = "+-+\n| ID |\n+-+";
        Assert.AreEqual(asciiTable, unicodeTable.ConvertToLatin1());
        Assert.AreEqual("TestTest", "Test\u0301\u0301Test".ConvertToLatin1());
	}

	[TestMethod]
	public void TestRomanNumerals()
	{
		// Testing values within standard Roman numeral range
		Assert.AreEqual("I", 1.ToRomanNumeral());
		Assert.AreEqual("II", 2.ToRomanNumeral());
		Assert.AreEqual("III", 3.ToRomanNumeral());
		Assert.AreEqual("IV", 4.ToRomanNumeral());
		Assert.AreEqual("V", 5.ToRomanNumeral());
		Assert.AreEqual("IX", 9.ToRomanNumeral());
		Assert.AreEqual("X", 10.ToRomanNumeral());
		Assert.AreEqual("XL", 40.ToRomanNumeral());
		Assert.AreEqual("L", 50.ToRomanNumeral());
		Assert.AreEqual("XC", 90.ToRomanNumeral());
		Assert.AreEqual("C", 100.ToRomanNumeral());
		Assert.AreEqual("CD", 400.ToRomanNumeral());
		Assert.AreEqual("D", 500.ToRomanNumeral());
		Assert.AreEqual("CM", 900.ToRomanNumeral());
		Assert.AreEqual("M", 1000.ToRomanNumeral());
		Assert.AreEqual("MMMCMXCIX", 3999.ToRomanNumeral());

		// Testing values using the Apostrophus method for extended range
		Assert.AreEqual("|IV|", 4000.ToRomanNumeral());
		Assert.AreEqual("|IV|D", 4500.ToRomanNumeral());
		Assert.AreEqual("|V|CDXXXII", 5432.ToRomanNumeral());
		Assert.AreEqual("|X|", 10000.ToRomanNumeral());
	}

	[TestMethod]
	public void Test_SimpleSentence()
	{
		string input = "an in-depth look at the problem";
		string expected = "An In-Depth Look at the Problem";
		string actual = input.ToTitleCaseAP();
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void Test_FirstAndLastWordsCapitalized()
	{
		string input = "the quick brown fox jumps over the lazy dog";
		string expected = "The Quick Brown Fox Jumps Over the Lazy Dog";
		string actual = input.ToTitleCaseAP();
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void Test_LowercaseWordsNotCapitalized()
	{
		string input = "this is a test of the emergency broadcast system";
		string expected = "This Is a Test of the Emergency Broadcast System";
		string actual = input.ToTitleCaseAP();
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void Test_AllUppercaseWordPreserved()
	{
		string input = "NASA launches new satellite";
		string expected = "NASA Launches New Satellite";
		string actual = input.ToTitleCaseAP();
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void Test_InternalUppercaseLettersPreserved()
	{
		string input = "apple introduces new iPhone model";
		string expected = "Apple Introduces New iPhone Model";
		string actual = input.ToTitleCaseAP();
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void Test_Apostrophes()
	{
		string input = "it's a dog's life";
		string expected = "It's a Dog's Life";
		string actual = input.ToTitleCaseAP();
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void Test_HyphenatedWords()
	{
		string input = "the state-of-the-art equipment";
		string expected = "The State-of-the-Art Equipment";
		string actual = input.ToTitleCaseAP();
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void Test_EmptyString()
	{
		string input = "";
		string expected = "";
		string actual = input.ToTitleCaseAP();
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void Test_NullString()
	{
		string input = null;
		string expected = null;
		string actual = input.ToTitleCaseAP();
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void Test_SingleWord()
	{
		string input = "hello";
		string expected = "Hello";
		string actual = input.ToTitleCaseAP();
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void Test_HyphenatedLowercaseWords()
	{
		string input = "checking out-of-the-box solutions";
		string expected = "Checking Out-of-the-Box Solutions";
		string actual = input.ToTitleCaseAP();
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void Test_SpecialCharacters()
	{
		string input = "hello, world! welcome to c# programming.";
		string expected = "Hello, World! Welcome to C# Programming.";
		string actual = input.ToTitleCaseAP();
		Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void Test_NumericValues()
	{
		string input = "top 10 reasons to learn c# in 2023";
		string expected = "Top 10 Reasons to Learn C# in 2023";
		string actual = input.ToTitleCaseAP();
		Assert.AreEqual(expected, actual);
	}
}




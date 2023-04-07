using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using System.Diagnostics;

namespace MudSharp_Unit_Tests {
    /// <summary>
    /// Summary description for StringTests
    /// </summary>
    [TestClass]
    public class StringTests {
        public StringTests() {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
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
@"Item 1   Item 7   
Item 2   Item 8   
Item 3   Item 9   
Item 4   Item 10  
Item 5   Item 11  
Item 6   
";

            var actual = input.SplitTextIntoColumns(2, 20, 2);

            Assert.AreEqual(true, expected.Equals(actual), $"Actual Output: [{actual}]");
        }

        [TestMethod]
        public void TestSanitisation() {
            var input =
                "this is some test player input containing a { character and also the double of that, which is {{.";
            Assert.AreEqual("this is some test player input containing a {{ character and also the double of that, which is {{{{.", input.Sanitise(), "The two strings were not equal.");
        }

        [TestMethod]
        public void TestStringListFunctions() {
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
        public void TestListToString() {
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
        public void TestListToStringPerformance() {
            var list = new[] { "one", "two", "three", "four", "two", "two", "three" };
            var sw = new Stopwatch();
            string result;
            GC.Collect();
            if (GC.TryStartNoGCRegion(65536, false)) {
                try {
                    sw.Start();
                    result = list.ListToString();
                    sw.Stop();
                }
                finally {
                    GC.EndNoGCRegion();
                }
                Trace.WriteLine($"ListToString duration: {sw.ElapsedTicks:N0} ticks, {sw.ElapsedMilliseconds:N0}ms");
                Assert.IsTrue(sw.ElapsedMilliseconds < 5, "ListToString took 5ms or more to run.");
            }
            else {
                Assert.Inconclusive("Could not enter no garbage collection mode.");
            }
            if (GC.TryStartNoGCRegion(65536, false)) {
                try {
                    sw.Restart();
                    result = list.ListToCompactString();
                    sw.Stop();
                }
                finally {
                    GC.EndNoGCRegion();
                }
                Trace.WriteLine($"ListToCompactString duration: {sw.ElapsedTicks:N0} ticks, {sw.ElapsedMilliseconds:N0}ms");
                Assert.IsTrue(sw.ElapsedMilliseconds < 5, "ListToCompactString took 5ms or more to run.");
            }
            else {
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

            Assert.IsTrue(ssEmpty.IsFinished, "Empty StringStack had false IsFinished.");
            Assert.AreEqual(string.Empty, ssEmpty.Pop(), $"Empty StringStack had non-empty Pop() result: {ssEmpty.Last}");
            Assert.AreEqual(string.Empty, ssEmpty.PopSpeech(), $"Empty StringStack had non-empty PopSpeech() result: {ssEmpty.Last}");

            Assert.IsFalse(ssThreePlainArgs.IsFinished, "ThreePlainArgs StringStack started with true IsFinished.");
            Assert.AreEqual("one", ssThreePlainArgs.Pop(), $"ThreePlainArgs StringStack expected value \"one\" but instead got \"{ssThreePlainArgs.Last}\".");
            Assert.AreEqual("two", ssThreePlainArgs.Pop(), $"ThreePlainArgs StringStack expected value \"two\" but instead got \"{ssThreePlainArgs.Last}\".");
            Assert.AreEqual("three", ssThreePlainArgs.Pop(), $"ThreePlainArgs StringStack expected value \"three\" but instead got \"{ssThreePlainArgs.Last}\".");
            Assert.IsTrue(ssThreePlainArgs.IsFinished, "ThreePlainArgs StringStack finished with false IsFinished.");

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
    }
}

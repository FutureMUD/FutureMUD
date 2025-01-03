using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using NCalc.Domain;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace MudSharp_Unit_Tests;

/// <summary>
/// Summary description for ProgTests
/// </summary>
[TestClass]
public class ProgTests
{
	public ProgTests()
	{
		//
		// TODO: Add constructor logic here
		//
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
	public void TestCompatibleWith()
	{
		var type1 = ProgVariableTypes.Accent;
		var type2 = ProgVariableTypes.Text;
		var type3 = ProgVariableTypes.Text | ProgVariableTypes.Literal;
		var type4 = ProgVariableTypes.Character;
		var type5 = ProgVariableTypes.Toon;
		var type6 = ProgVariableTypes.Character | ProgVariableTypes.Collection;
		var type7 = ProgVariableTypes.Toon | ProgVariableTypes.Collection;

		Assert.AreEqual(type1.CompatibleWith(type2), false, "Accent and Text were considered equal to one another.");
		Assert.AreEqual(type2.CompatibleWith(type3), true, "Text should be able to be assigned to a text literal.");
		Assert.AreEqual(type3.CompatibleWith(type2), true, "Text literals should be able to be assigned to text variables.");
		Assert.AreEqual(type4.CompatibleWith(type5), true, "Characters should be able to be assigned to toon variables.");
		Assert.AreEqual(type5.CompatibleWith(type4), false, "Toons should not be able to be assigned to character variables.");
		Assert.AreEqual(type6.CompatibleWith(type4), false, "Character collections should not be able to be assigned to character variables.");
		Assert.AreEqual(type4.CompatibleWith(type6), false, "Characters should not be able to be assigned to character collections.");
		Assert.AreEqual(type6.CompatibleWith(type7), true, "Character collections should be able to be assigned to toon collections.");
		Assert.AreEqual(type6.CompatibleWith(type6), true, "Character collections should be able to be assigned to other character collections.");
		Assert.AreEqual(type7.CompatibleWith(type6), false, "Toon collections should not be able to be assigned to character colletions.");
		Assert.AreEqual(ProgVariableTypes.Gender.CompatibleWith(ProgVariableTypes.CollectionItem), true, "Gender was not compatible with collection item.");
	}

	[TestMethod]
	public void TestFunctionParsing()
	{
		var rbs = FunctionHelper.ReverseBinarySplit("1.2", '.');

		Assert.IsTrue(rbs.IsFound);
		Assert.IsFalse(rbs.IsError);
		Assert.AreEqual("1", rbs.LHS);
		Assert.AreEqual("2", rbs.RHS);
		Assert.AreEqual(".", rbs.MatchedSplitString);


		var split = FunctionHelper.NonFunctionStringSplit("1.2");
		Assert.IsTrue(split.IsFound);
		Assert.IsFalse(split.IsError);
		Assert.AreEqual("1.2", split.StringValue);
		Assert.AreEqual(FunctionHelper.NonFunctionType.NumberLiteral, split.Type);

		var text = FunctionHelper.NonFunctionStringSplit(@"""This is a simple string""");
		Assert.IsTrue(text.IsFound);
		Assert.IsFalse(text.IsError);
		Assert.AreEqual(FunctionHelper.NonFunctionType.TextLiteral, text.Type);
		Assert.AreEqual("This is a simple string", text.StringValue);
	}

	[TestMethod]
	public void TestExecute()
	{
		var gameworld = new GameworldStub().ToMock();
		FutureProg.Initialise();
		var prog1 = new FutureProg(gameworld, "AddNumbers", ProgVariableTypes.Number, new[]
			{
				Tuple.Create(ProgVariableTypes.Number, "number1"),
				Tuple.Create(ProgVariableTypes.Number, "number2")
			},
			@"return @number1 + @number2");
		prog1.Compile();
		Assert.IsTrue(string.IsNullOrEmpty(prog1.CompileError), $"The AddNumbers prog did not compile: {prog1.CompileError}");
		Assert.AreEqual(8, prog1.ExecuteInt(0, 2, 6), "2+6 did not equal 8");
		Assert.AreEqual(650, prog1.ExecuteInt(0, 600, 50), "600+50 did not equal 650");
		Assert.AreEqual(8.0, prog1.ExecuteDouble(0.0, 2.0, 6.0), "2.0+6.0 did not equal 8.0");

		var prog2 = new FutureProg(gameworld, "AddNumbersNull", ProgVariableTypes.Number, new[]
			{
				Tuple.Create(ProgVariableTypes.Number, "number1"),
				Tuple.Create(ProgVariableTypes.Number, "number2")
			},
			@"return null(""number"")");
		prog2.Compile();
		Assert.IsTrue(string.IsNullOrEmpty(prog2.CompileError), $"The AddNumbersNull prog did not compile: {prog2.CompileError}");
		Assert.AreEqual(0, prog2.ExecuteInt(0, 2, 6), "AddNumbersNull: Default value was not 0");
		Assert.AreEqual(0.0M, prog2.Execute(2, 6), "AddNumbersNull: Returned value was not 0.0M");

		var prog3 = new FutureProg(gameworld, "ReturnNullCh", ProgVariableTypes.Character, new List<Tuple<ProgVariableTypes, string>>(),
			@"return null(""character"")");
		prog3.Compile();
		Assert.IsTrue(string.IsNullOrEmpty(prog3.CompileError), $"The ReturnNullCh prog did not compile: {prog3.CompileError}");
		Assert.AreEqual(0, prog3.ExecuteInt(0), "ReturnNullCh: Default value was not 0");
		Assert.AreEqual(null, prog3.Execute(), "ReturnNullCh: Returned value was not null");

		var prog4 = new FutureProg(gameworld, 
			"LoadItem", 
			ProgVariableTypes.Item, 
			new List<Tuple<ProgVariableTypes, string>>(),
			@"var dyecolour = ""blue""
var item = LoadItem(780, ""dyecolour=\""""+@dyecolour+""\"""")
return @item");
		prog4.Compile();
		Assert.IsTrue(string.IsNullOrEmpty(prog4.CompileError), $"The LoadItem prog did not compile: {prog4.CompileError}");
	}

	[TestMethod]
	public void TestLiterals()
	{
		var resultNumberLiteral = FunctionHelper.NonFunctionStringSplit("14");
		var resultNumberLiteralDecimal = FunctionHelper.NonFunctionStringSplit("14.7");
		var resultNumberLiteralError = FunctionHelper.NonFunctionStringSplit("14.7.7");
		var resultTextLiteral = FunctionHelper.NonFunctionStringSplit(@"""This is some text""");
		var resultEmptyTextLiteral = FunctionHelper.NonFunctionStringSplit(@"""""");
		var resultTextLiteralWithQuotes = FunctionHelper.NonFunctionStringSplit(@"""This text has \""some quotes\"".""");
		var resultVariableReference = FunctionHelper.NonFunctionStringSplit("@input");
		var resultTimeSpan1 = FunctionHelper.NonFunctionStringSplit("3d");
		var resultTimeSpan2 = FunctionHelper.NonFunctionStringSplit("12m 7s");
		var resultTimeSpan3 = FunctionHelper.NonFunctionStringSplit("3d 51m 12s 9f");

		Assert.IsTrue(resultNumberLiteral.IsFound, "Number Literal was not found");
		Assert.IsTrue(!resultNumberLiteral.IsError, "Number Literal was an error");
		Assert.IsTrue(resultNumberLiteral.Type == FunctionHelper.NonFunctionType.NumberLiteral, "Number Literal was not identified as a Number Literal");

		Assert.IsTrue(resultNumberLiteralDecimal.IsFound, "Number Literal Decimal was not found");
		Assert.IsTrue(!resultNumberLiteralDecimal.IsError, "Number Literal Decimal was an error");
		Assert.IsTrue(resultNumberLiteralDecimal.Type == FunctionHelper.NonFunctionType.NumberLiteral, "Number Literal Decimal was not identified as a Number Literal");

		Assert.IsTrue(!resultNumberLiteralError.IsFound, "Number Literal Error was found");
		Assert.IsTrue(resultNumberLiteralError.IsError, "Number Literal was not an error");
		Assert.IsTrue(resultNumberLiteralError.Type == FunctionHelper.NonFunctionType.None, "Number Literal Error was not identified as a None Type");

		Assert.IsTrue(resultTextLiteral.IsFound, "Text Literal was not found");
		Assert.IsTrue(!resultTextLiteral.IsError, "Text Literal was an error");
		Assert.IsTrue(resultTextLiteral.Type == FunctionHelper.NonFunctionType.TextLiteral, "Text Literal was not identified as a Text Literal");
		Assert.AreEqual("This is some text", resultTextLiteral.StringValue, "Text Literal had mismatching string value");

		Assert.IsTrue(resultEmptyTextLiteral.IsFound, "Text Literal Empty was not found");
		Assert.IsTrue(!resultEmptyTextLiteral.IsError, "Text Literal Empty was an error");
		Assert.IsTrue(resultEmptyTextLiteral.Type == FunctionHelper.NonFunctionType.TextLiteral, "Text Literal Empty was not identified as a Text Literal");
		Assert.AreEqual("", resultEmptyTextLiteral.StringValue, "Text Literal Empty had mismatching string value");

		Assert.IsTrue(resultTextLiteralWithQuotes.IsFound, "Text Literal Quotes was not found");
		Assert.IsTrue(!resultTextLiteralWithQuotes.IsError, "Text Literal Quotes was an error");
		Assert.IsTrue(resultTextLiteralWithQuotes.Type == FunctionHelper.NonFunctionType.TextLiteral, "Text Literal Quotes was not identified as a Text Literal");
		Assert.AreEqual(@"This text has ""some quotes"".", resultTextLiteralWithQuotes.StringValue, "Text Literal Quotes had mismatching string value");

		Assert.IsTrue(resultVariableReference.IsFound, "Variable Reference Literal was not found");
		Assert.IsTrue(!resultVariableReference.IsError, "Variable Reference Literal was an error");
		Assert.IsTrue(resultVariableReference.Type == FunctionHelper.NonFunctionType.VariableReference, "Variable Reference Literal was not identified as a Variable Reference Literal");
		Assert.AreEqual("input", resultVariableReference.StringValue, "Variable Reference Literal had mismatching string value");

		Assert.IsTrue(resultTimeSpan1.IsFound, "TimeSpan 1 Literal was not found");
		Assert.IsTrue(!resultTimeSpan1.IsError, "TimeSpan 1e Literal was an error");
		Assert.IsTrue(resultTimeSpan1.Type == FunctionHelper.NonFunctionType.TimeSpanLiteral, "TimeSpan 1 Literal was not identified as a Time Span Literal");

		Assert.IsTrue(resultTimeSpan2.IsFound, "TimeSpan 2 Literal was not found");
		Assert.IsTrue(!resultTimeSpan2.IsError, "TimeSpan 2 Literal was an error");
		Assert.IsTrue(resultTimeSpan2.Type == FunctionHelper.NonFunctionType.TimeSpanLiteral, "TimeSpan 2 Literal was not identified as a Time Span Literal");

		Assert.IsTrue(resultTimeSpan3.IsFound, "TimeSpan 3 Literal was not found");
		Assert.IsTrue(!resultTimeSpan3.IsError, "TimeSpan 3 Literal was an error");
		Assert.IsTrue(resultTimeSpan3.Type == FunctionHelper.NonFunctionType.TimeSpanLiteral, "TimeSpan 3 Literal was not identified as a Time Span Literal");
	}

	[TestMethod]
	public void TestSplit()
	{
		var text = "This is some text\nAnd some more text\n\nText after a blank line";
		var gameworld = new GameworldStub().ToMock();
		FutureProg.Initialise();
		var prog1 = new FutureProg(gameworld, "TestSplit", ProgVariableTypes.Text | ProgVariableTypes.Collection, new[]
			{
				Tuple.Create(ProgVariableTypes.Text, "text"),
				Tuple.Create(ProgVariableTypes.Text, "split")
			},
			@"return SplitText(@text, @split)");
		prog1.Compile();
		Assert.IsTrue(string.IsNullOrEmpty(prog1.CompileError), $"The TestSplit prog did not compile: {prog1.CompileError}");

		var result = prog1.ExecuteCollection<string>(text, "\n").ToList();
		Assert.AreEqual(4, result.Count, $"Results count was {result.Count} instead of 4");
		Assert.AreEqual("This is some text", result[0], $"Result #1 was {result[0]}");
		Assert.AreEqual("And some more text", result[1], $"Result #1 was {result[1]}");
		Assert.AreEqual("", result[2], $"Result #1 was {result[2]}");
		Assert.AreEqual("Text after a blank line", result[3], $"Result #1 was {result[3]}");
	}
}
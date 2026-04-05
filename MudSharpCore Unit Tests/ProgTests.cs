using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using NCalc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace MudSharp_Unit_Tests;

/// <summary>
/// Summary description for ProgTests
/// </summary>
[TestClass]
public class ProgTests
{
    private static IFuturemud _gameworld;

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
    public TestContext TestContext
    {
        get => testContextInstance; set => testContextInstance = value;
    }

    #region Additional test attributes
    //
    // You can use the following additional attributes as you write your tests:
    //
    // Use ClassInitialize to run code before running the first test in the class
    [ClassInitialize()]
    public static void MyClassInitialize(TestContext testContext)
    {
        FutureProgTestBootstrap.EnsureInitialised();
        _gameworld = FutureProgTestBootstrap.Gameworld;
    }
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
        ProgVariableTypes type1 = ProgVariableTypes.Accent;
        ProgVariableTypes type2 = ProgVariableTypes.Text;
        ProgVariableTypes type3 = ProgVariableTypes.Text | ProgVariableTypes.Literal;
        ProgVariableTypes type4 = ProgVariableTypes.Character;
        ProgVariableTypes type5 = ProgVariableTypes.Toon;
        ProgVariableTypes type6 = ProgVariableTypes.Character | ProgVariableTypes.Collection;
        ProgVariableTypes type7 = ProgVariableTypes.Toon | ProgVariableTypes.Collection;

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
        FunctionHelper.BinarySplit rbs = FunctionHelper.ReverseBinarySplit("1.2", '.');

        Assert.IsTrue(rbs.IsFound);
        Assert.IsFalse(rbs.IsError);
        Assert.AreEqual("1", rbs.LHS);
        Assert.AreEqual("2", rbs.RHS);
        Assert.AreEqual(".", rbs.MatchedSplitString);


        FunctionHelper.NonFunctionSplit split = FunctionHelper.NonFunctionStringSplit("1.2");
        Assert.IsTrue(split.IsFound);
        Assert.IsFalse(split.IsError);
        Assert.AreEqual("1.2", split.StringValue);
        Assert.AreEqual(FunctionHelper.NonFunctionType.NumberLiteral, split.Type);

        FunctionHelper.NonFunctionSplit text = FunctionHelper.NonFunctionStringSplit(@"""This is a simple string""");
        Assert.IsTrue(text.IsFound);
        Assert.IsFalse(text.IsError);
        Assert.AreEqual(FunctionHelper.NonFunctionType.TextLiteral, text.Type);
        Assert.AreEqual("This is a simple string", text.StringValue);
    }

    [TestMethod]
    public void TestExecute()
    {
        FutureProg prog1 = new(_gameworld, "AddNumbers", ProgVariableTypes.Number, new[]
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

        FutureProg prog2 = new(_gameworld, "AddNumbersNull", ProgVariableTypes.Number, new[]
            {
                Tuple.Create(ProgVariableTypes.Number, "number1"),
                Tuple.Create(ProgVariableTypes.Number, "number2")
            },
            @"return null(""number"")");
        prog2.Compile();
        Assert.IsTrue(string.IsNullOrEmpty(prog2.CompileError), $"The AddNumbersNull prog did not compile: {prog2.CompileError}");
        Assert.AreEqual(0, prog2.ExecuteInt(0, 2, 6), "AddNumbersNull: Default value was not 0");
        Assert.AreEqual(0.0M, prog2.Execute(2, 6), "AddNumbersNull: Returned value was not 0.0M");

        FutureProg prog3 = new(_gameworld, "ReturnNullCh", ProgVariableTypes.Character, new List<Tuple<ProgVariableTypes, string>>(),
            @"return null(""character"")");
        prog3.Compile();
        Assert.IsTrue(string.IsNullOrEmpty(prog3.CompileError), $"The ReturnNullCh prog did not compile: {prog3.CompileError}");
        Assert.AreEqual(0, prog3.ExecuteInt(0), "ReturnNullCh: Default value was not 0");
        Assert.AreEqual(null, prog3.Execute(), "ReturnNullCh: Returned value was not null");

        FutureProg prog4 = new(_gameworld,
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
        FunctionHelper.NonFunctionSplit resultNumberLiteral = FunctionHelper.NonFunctionStringSplit("14");
        FunctionHelper.NonFunctionSplit resultNumberLiteralDecimal = FunctionHelper.NonFunctionStringSplit("14.7");
        FunctionHelper.NonFunctionSplit resultNumberLiteralError = FunctionHelper.NonFunctionStringSplit("14.7.7");
        FunctionHelper.NonFunctionSplit resultTextLiteral = FunctionHelper.NonFunctionStringSplit(@"""This is some text""");
        FunctionHelper.NonFunctionSplit resultEmptyTextLiteral = FunctionHelper.NonFunctionStringSplit(@"""""");
        FunctionHelper.NonFunctionSplit resultTextLiteralWithQuotes = FunctionHelper.NonFunctionStringSplit(@"""This text has \""some quotes\"".""");
        FunctionHelper.NonFunctionSplit resultVariableReference = FunctionHelper.NonFunctionStringSplit("@input");
        FunctionHelper.NonFunctionSplit resultTimeSpan1 = FunctionHelper.NonFunctionStringSplit("3d");
        FunctionHelper.NonFunctionSplit resultTimeSpan2 = FunctionHelper.NonFunctionStringSplit("12m 7s");
        FunctionHelper.NonFunctionSplit resultTimeSpan3 = FunctionHelper.NonFunctionStringSplit("3d 51m 12s 9f");

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
        string text = "This is some text\nAnd some more text\n\nText after a blank line";
        FutureProg prog1 = new(_gameworld, "TestSplit", ProgVariableTypes.Text | ProgVariableTypes.Collection, new[]
            {
                Tuple.Create(ProgVariableTypes.Text, "text"),
                Tuple.Create(ProgVariableTypes.Text, "split")
            },
            @"return SplitText(@text, @split)");
        prog1.Compile();
        Assert.IsTrue(string.IsNullOrEmpty(prog1.CompileError), $"The TestSplit prog did not compile: {prog1.CompileError}");

        List<string> result = prog1.ExecuteCollection<string>(text, "\n").ToList();
        Assert.AreEqual(4, result.Count, $"Results count was {result.Count} instead of 4");
        Assert.AreEqual("This is some text", result[0], $"Result #1 was {result[0]}");
        Assert.AreEqual("And some more text", result[1], $"Result #1 was {result[1]}");
        Assert.AreEqual("", result[2], $"Result #1 was {result[2]}");
        Assert.AreEqual("Text after a blank line", result[3], $"Result #1 was {result[3]}");
    }

    [TestMethod]
    public void TestGetTypeByName()
    {
        foreach (ProgVariableTypes type in ProgVariableTypes.Anything.GetAllFlags())
        {
            if (type.LegacyCode is ProgVariableTypeCode.Error or ProgVariableTypeCode.Void or ProgVariableTypeCode.Dictionary or ProgVariableTypeCode.Collection or ProgVariableTypeCode.CollectionDictionary)
            {
                continue;
            }

            Assert.AreNotEqual(ProgVariableTypes.Void, FutureProg.GetTypeByName(type.DescribeEnum()), $"There was no round-trip GetTypeByName for type {type.DescribeEnum()}");
        }
    }
}

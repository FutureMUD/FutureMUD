﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using NCalc.Domain;

namespace MudSharp_Unit_Tests
{
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
            var type1 = FutureProgVariableTypes.Accent;
            var type2 = FutureProgVariableTypes.Text;
            var type3 = FutureProgVariableTypes.Text | FutureProgVariableTypes.Literal;
            var type4 = FutureProgVariableTypes.Character;
            var type5 = FutureProgVariableTypes.Toon;
            var type6 = FutureProgVariableTypes.Character | FutureProgVariableTypes.Collection;
            var type7 = FutureProgVariableTypes.Toon | FutureProgVariableTypes.Collection;

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
            Assert.AreEqual(FutureProgVariableTypes.Gender.CompatibleWith(FutureProgVariableTypes.CollectionItem), true, "Gender was not compatible with collection item.");
        }

        [TestMethod]
        public void TestFunctionParsing()
        {
	        var rbs = FunctionHelper.ReverseBinarySplit("1.2", '.');
	        var split = FunctionHelper.NonFunctionStringSplit("1.2");
	        Assert.IsTrue(true);
        }

        [TestMethod]
		public void TestExecute()
		{
			var gameworld = new GameworldStub().ToMock();
            FutureProg.Initialise();
			var prog1 = new FutureProg(gameworld, "AddNumbers", FutureProgVariableTypes.Number, new[]
				{
					Tuple.Create(FutureProgVariableTypes.Number, "number1"),
					Tuple.Create(FutureProgVariableTypes.Number, "number2")
				},
				@"return @number1 + @number2");
			prog1.Compile();
            Assert.IsTrue(string.IsNullOrEmpty(prog1.CompileError), $"The AddNumbers prog did not compile: {prog1.CompileError}");
            Assert.AreEqual(8, prog1.ExecuteInt(0, 2, 6), "2+6 did not equal 8");
            Assert.AreEqual(650, prog1.ExecuteInt(0, 600, 50), "600+50 did not equal 650");
            Assert.AreEqual(8.0, prog1.ExecuteDouble(0.0, 2.0, 6.0), "2.0+6.0 did not equal 8.0");

            var prog2 = new FutureProg(gameworld, "AddNumbersNull", FutureProgVariableTypes.Number, new[]
	            {
		            Tuple.Create(FutureProgVariableTypes.Number, "number1"),
		            Tuple.Create(FutureProgVariableTypes.Number, "number2")
	            },
	            @"return null(""number"")");
            prog2.Compile();
            Assert.IsTrue(string.IsNullOrEmpty(prog2.CompileError), $"The AddNumbersNull prog did not compile: {prog2.CompileError}");
            Assert.AreEqual(0, prog2.ExecuteInt(0, 2, 6), "AddNumbersNull: Default value was not 0");
            Assert.AreEqual(0.0M, prog2.Execute(2, 6), "AddNumbersNull: Returned value was not 0.0M");

            var prog3 = new FutureProg(gameworld, "ReturnNullCh", FutureProgVariableTypes.Character, new List<Tuple<FutureProgVariableTypes, string>>(),
	            @"return null(""character"")");
            prog3.Compile();
            Assert.IsTrue(string.IsNullOrEmpty(prog3.CompileError), $"The ReturnNullCh prog did not compile: {prog3.CompileError}");
            Assert.AreEqual(0, prog3.ExecuteInt(0), "ReturnNullCh: Default value was not 0");
            Assert.AreEqual(null, prog3.Execute(), "ReturnNullCh: Returned value was not null");
		}
    }
}

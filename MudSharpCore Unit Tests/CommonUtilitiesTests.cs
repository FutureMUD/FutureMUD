using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests
{
    [TestClass]
    public class CommonUtilitiesTests
    {
        [Flags]
        private enum TestEnum
        {
            None = 0,
            First = 1,
            Second = 2,
            Third = 4,
            Fourth = 8,
            Fifth = 16,
            Sixth = 32,
            Seventh = 64,
            Eighth = 128,
            SevenAndEight = Seventh | Eighth
        }

        [TestMethod]
        public void TestEnums() {
            var value1 = TestEnum.Third | TestEnum.Fourth | TestEnum.Seventh;
            var value2 = TestEnum.Second;

            Assert.AreEqual(true, value1.HasFlag(TestEnum.Fourth), "Expected HasFlag to return true.");
            Assert.AreEqual(true, value1.HasFlag(TestEnum.Third), "Expected HasFlag to return true.");
            Assert.AreEqual(true, value1.HasFlag(TestEnum.Seventh), "Expected HasFlag to return true.");
            Assert.AreEqual(false, value1.HasFlag(TestEnum.First), "Expected HasFlag to return false.");
            Assert.AreEqual(false, value1.HasFlag(TestEnum.Eighth), "Expected HasFlag to return false.");
            Assert.AreEqual(false, value1.HasFlag(TestEnum.Fifth), "Expected HasFlag to return false.");


            Assert.AreEqual(true, value2.HasFlag(TestEnum.Second), "Expected HasFlag to return true.");
            Assert.AreEqual(false, value2.HasFlag(TestEnum.First), "Expected HasFlag to return false.");
            Assert.AreEqual(false, value2.HasFlag(TestEnum.Eighth), "Expected HasFlag to return false.");
            Assert.AreEqual(false, value2.HasFlag(TestEnum.Fifth), "Expected HasFlag to return false.");

            var values = value1.GetFlags().ToList();
            Assert.AreEqual(true, values.Contains(TestEnum.Third), "GetFlags didn't pick up TestEnum.Third");
            Assert.AreEqual(true, values.Contains(TestEnum.Fourth), "GetFlags didn't pick up TestEnum.Fourth");
            Assert.AreEqual(true, values.Contains(TestEnum.Seventh), "GetFlags didn't pick up TestEnum.Seventh");
            Assert.AreEqual(true, values.Count == 3, "GetFlags picked up more than 3 elements");

            var values2 = value2.GetFlags().ToList();
            Assert.AreEqual(true, values2.Contains(TestEnum.Second), "GetFlags didn't pick up TestEnum.Second");
            Assert.AreEqual(true, values2.Count == 1, "GetFlags contained more than 1 element");

            Assert.AreEqual("Fourth", TestEnum.Fourth.DescribeEnum());
            Assert.AreEqual("Dark Red", ConsoleColor.DarkRed.DescribeEnum(true));

            var values1a = value1.GetAllFlags();
            Assert.AreEqual(true, values1a.Count() == 4, "Value1a did not have a count of 4, had " + values1a.Count());
            Assert.AreEqual(true, TestEnum.SevenAndEight.GetAllFlags().Count() == 4, $"SevenAndEight.GetAllFlags had {TestEnum.SevenAndEight.GetAllFlags().Count()} flags.");
            Assert.AreEqual(true, TestEnum.SevenAndEight.GetSingleFlags().Count() == 2, $"SevenAndEight.GetSingleFlags had {TestEnum.SevenAndEight.GetSingleFlags().Count()} flags.");
        }

        [TestMethod]
        public void TestTryParseEnum()
        {
            if (!"Gigantic".TryParseEnum<SizeCategory>(out var value))
            {
                Assert.Fail("\"Gigantic\" argument failed to return a value");
            }
            
            Assert.AreEqual(SizeCategory.Gigantic, value, "Expected \"Gigantic\" to return SizeCategory.Gigantic");

            if (!"2".TryParseEnum(out value))
            {
                Assert.Fail("\"Gigantic\" argument failed to return a value for SizeCategory");
            }

            Assert.AreEqual(SizeCategory.Miniscule, value, "Expected \"2\" to return SizeCategory.Miniscule");

            if (!"Ground Level".TryParseEnum<RoomLayer>(out var rlvalue))
            {
                Assert.Fail("\"Room Layer\" argument failed to return a value");
            }

            Assert.AreEqual(RoomLayer.GroundLevel, rlvalue, "Expected \"Ground Level\" to return RoomLayer.GroundLevel");

            if (!"0".TryParseEnum<RoomLayer>(out rlvalue))
            {
                Assert.Fail("\"0\" argument failed to return a value for RoomLayer");
            }

            Assert.AreEqual(RoomLayer.GroundLevel, rlvalue, "Expected \"0\" to return RoomLayer.GroundLevel");

            if (!"Terrain".TryParseEnum<FutureProgVariableTypes>(out var fpvalue))
            {
                Assert.Fail("\"Terrain\" argument failed to return a value");
            }

            Assert.AreEqual(FutureProgVariableTypes.Terrain, fpvalue, "Expected \"Terrain\" to return FutureProgVariableTypes.Terrain");

            if (!"4398046511104".TryParseEnum<FutureProgVariableTypes>(out fpvalue))
            {
                Assert.Fail("\"Terrain\" argument failed to return a value");
            }

            Assert.AreEqual(FutureProgVariableTypes.Terrain, fpvalue, "Expected \"4398046511104\" to return FutureProgVariableTypes.Terrain");
        }

        [TestMethod]
        public void TestCounter()
        {
            var counter = new Counter<string>();
            try
            {
                Assert.AreEqual(0, counter.Count("balloon"), $"Expected 0 instances of string \"balloon\", but got {counter.Count("balloon")}.");
                counter.Increment("balloon");
                Assert.AreEqual(1, counter.Count("balloon"), $"Expected 1 instances of string \"balloon\", but got {counter.Count("balloon")}.");
                counter.Add("balloon", 4);
                Assert.AreEqual(5, counter.Count("balloon"), $"Expected 5 instances of string \"balloon\", but got {counter.Count("balloon")}.");
                Assert.AreEqual(0, counter.Count("Balloon"), $"Expected 0 instances of string \"Balloon\", but got {counter.Count("Balloon")}.");
                counter.Add("Balloon", 2);
                Assert.AreEqual(2, counter.Count("Balloon"), $"Expected 2 instances of string \"Balloon\", but got {counter.Count("Balloon")}.");

                var caseIntensitiveCounter = new Counter<string>(StringComparer.OrdinalIgnoreCase) { { "balloon", 4 } };
                Assert.AreEqual(4, caseIntensitiveCounter.Count("balloon"), $"Expected 4 instances of string \"balloon\", but got {caseIntensitiveCounter.Count("balloon")}.");
                Assert.AreEqual(4, caseIntensitiveCounter.Count("Balloon"), $"Expected 4 instances of string \"Balloon\", but got {caseIntensitiveCounter.Count("Balloon")}.");

            }
            catch (Exception e)
            {
                Assert.Fail($"Encountered an exception while testing counter: {e}");
            }
        }

        private class IncrementingCounter
        {
            public int Value { get; set; }
        }

        [TestMethod]
        public void TestApplyToAdjacent()
        {
            var array = new IncrementingCounter[10, 10];
            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    array[i, j] = new IncrementingCounter();
                }
            }

            array.ApplyActionToAdjacents(5, 5, value => {
                value.Value++;
            });
            Assert.AreEqual(0, array[3, 4].Value, "Element at [3,4] was not 0");
            Assert.AreEqual(0, array[5, 5].Value, "Element at [5,5] was not 0");
            Assert.AreEqual(1, array[5, 4].Value, "Element at [5,4] was not 1");
            Assert.AreEqual(1, array[5, 6].Value, "Element at [5,6] was not 1");
            Assert.AreEqual(1, array[4, 5].Value, "Element at [4,5] was not 1");
            Assert.AreEqual(1, array[6, 5].Value, "Element at [6,5] was not 1");
            Assert.AreEqual(1, array[4, 4].Value, "Element at [4,4] was not 1");
            Assert.AreEqual(1, array[4, 6].Value, "Element at [4,6] was not 1");
            Assert.AreEqual(1, array[6, 6].Value, "Element at [6,6] was not 1");
            Assert.AreEqual(1, array[6, 4].Value, "Element at [6,4] was not 1");

            var count = array.ApplyFunctionToAdjacentsReturnCount(5,5, value => {
                value.Value++;
                return true;
            });

            Assert.AreEqual(8, count, "Count from ApplyFunctionToAdjacentsReturnCount was not 8");
            Assert.AreEqual(0, array[3, 4].Value, "Element at [3,4] was not 0");
            Assert.AreEqual(0, array[5, 5].Value, "Element at [5,5] was not 0");
            Assert.AreEqual(2, array[5, 4].Value, "Element at [5,4] was not 2");
            Assert.AreEqual(2, array[5, 6].Value, "Element at [5,6] was not 2");
            Assert.AreEqual(2, array[4, 5].Value, "Element at [4,5] was not 2");
            Assert.AreEqual(2, array[6, 5].Value, "Element at [6,5] was not 2");
            Assert.AreEqual(2, array[4, 4].Value, "Element at [4,4] was not 2");
            Assert.AreEqual(2, array[4, 6].Value, "Element at [4,6] was not 2");
            Assert.AreEqual(2, array[6, 6].Value, "Element at [6,6] was not 2");
            Assert.AreEqual(2, array[6, 4].Value, "Element at [6,4] was not 2");
        }

        [TestMethod]
        public void TestFindCoordinates()
        {
            var array = new string[10, 10];
            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    array[i, j] = ((i * 10) + j + 1).ToWordyNumber();
                }
            }

            var (x, y) = array.GetCoordsOfElement("twenty four");
            Assert.AreEqual((2, 3), (x,y), $"Expected: 2,3 - Found: {x},{y}");
            (x, y) = array.GetCoordsOfElement("three");
            Assert.AreEqual((0, 2), (x, y), $"Expected: 0,2 - Found: {x},{y}");
            (x, y) = array.GetCoordsOfElement("ninety nine");
            Assert.AreEqual((9, 8), (x, y), $"Expected: 9,8 - Found: {x},{y}");
        }

        [TestMethod]
        public void TestBetterRandomRNG()
        {
	        var rng = new BetterRandom(0, 8675309);
	        var results = new List<int>();
	        var count = new Counter<bool>();
	        for (var i = 0; i < 50000; i++)
	        {
		        var result = rng.Next32i();
                results.Add(result);
                count[result >= 0] += 1;
	        }

            Assert.IsTrue(results[0] == 2111733193);
            Assert.IsTrue(results[1] == 1331655554);
            Assert.IsTrue(results[2] == -1551408077);
            Assert.IsTrue(results[3] == 447604737);
            Assert.IsTrue(results[4] == 1076125057);

            var rng2 = new BetterRandom(3, 8675309);
            Assert.IsTrue(rng2.Next32i() == 447604737);

            double value;
            for (var i = 0; i < 500000; i++)
            {
	            value = rng.NextDouble();
	            if (value < 0.0 || value > 1.0)
	            {
		            Assert.Fail();
	            }
            }
        }

        [TestMethod]
        public void TestBetterRandomDice()
        {
	        var rng = new BetterRandom(0, 8675309);
	        var results = new List<int>();
	        var count = new Counter<int>();
	        int result;
	        for (var i = 0; i < 50000; i++)
	        {
		        result = rng.Next32i(1, 7);
		        results.Add(result);
		        count[result] += 1;
		        if (result < 1 || result > 6)
		        {
			        Assert.Fail();
		        }
	        }
        }

        [TestMethod]
        public void TestRandDice()
        {
	        var rng = new Random(8675309);
	        var results = new List<int>();
	        var count = new Counter<int>();
	        int result;
	        for (var i = 0; i < 50000; i++)
	        {
		        result = rng.Next(1, 7);
		        results.Add(result);
		        count[result] += 1;
		        if (result < 1 || result > 6)
		        {
			        Assert.Fail();
		        }
	        }
        }

        [TestMethod]
        public void TestXLinqExtensions()
        {
            var element = XElement.Parse(@"<Strategy type=""hierarchy""><Member prog=""24"" ><Strategy type=""jail"" ><Minimum>1 weeks</Minimum><Maximum>2 months</Maximum></Strategy></Member><Member prog=""1"" ><Strategy type=""bond"" ><Length>12 months</Length></Strategy></Member></Strategy>");
            Assert.AreEqual<string>(@"<Strategy type=""jail""><Minimum>1 weeks</Minimum><Maximum>2 months</Maximum></Strategy>", element.Elements("Member").First().InnerXML());
        }

        [TestMethod]
        public void TestDiceExpression()
        {
	        var expression1 = "1d10";
	        var expression2 = "4d6k3";
	        var expression3 = "10d6e6";
	        var expression4 = "3d6m8";
	        var expression5 = "1d10e10M15";

			var sb = new StringBuilder();
	        var counter = new Counter<int>();
	        for (var i = 0; i < 10000; i++)
	        {
		        counter[Dice.Roll(expression1)]++;
	        }

	        sb.AppendLine($"==={expression1}===");
	        sb.AppendLine();
			foreach (var value in counter.OrderBy(x => x.Key))
			{
				sb.AppendLine($"[{value.Key}] = {value.Value}");
			}

            Assert.IsTrue(counter.Keys.Min() >= 1);
            Assert.IsTrue(counter.Keys.Max() <= 10);
            Assert.IsTrue(counter.Keys.Count == 10);

			counter = new Counter<int>();
			for (var i = 0; i < 10000; i++)
			{
				counter[Dice.Roll(expression2)]++;
			}

			sb.AppendLine();
			sb.AppendLine($"==={expression2}===");
			sb.AppendLine();
			foreach (var value in counter.OrderBy(x => x.Key))
			{
				sb.AppendLine($"[{value.Key}] = {value.Value}");
			}

			Assert.IsTrue(counter.Keys.Min() >= 3);
			Assert.IsTrue(counter.Keys.Max() <= 18);
			Assert.IsTrue(counter.Keys.Count == 16);

			counter = new Counter<int>();
			for (var i = 0; i < 10000; i++)
			{
				counter[Dice.Roll(expression3)]++;
			}

			sb.AppendLine();
			sb.AppendLine($"==={expression3}===");
			sb.AppendLine();
			foreach (var value in counter.OrderBy(x => x.Key))
			{
				sb.AppendLine($"[{value.Key}] = {value.Value}");
			}

			Assert.IsTrue(counter.Keys.Min() >= 10);
			Assert.IsTrue(counter.Keys.Max() > 60);

			counter = new Counter<int>();
			for (var i = 0; i < 10000; i++)
			{
				counter[Dice.Roll(expression4)]++;
			}

			sb.AppendLine();
			sb.AppendLine($"==={expression4}===");
			sb.AppendLine();
			foreach (var value in counter.OrderBy(x => x.Key))
			{
				sb.AppendLine($"[{value.Key}] = {value.Value}");
			}

			Assert.IsTrue(counter.Keys.Min() >= 8);
			Assert.IsTrue(counter.Keys.Max() <= 18);

			counter = new Counter<int>();
			for (var i = 0; i < 10000; i++)
			{
				counter[Dice.Roll(expression5)]++;
			}

			sb.AppendLine();
			sb.AppendLine($"==={expression5}===");
			sb.AppendLine();
			foreach (var value in counter.OrderBy(x => x.Key))
			{
				sb.AppendLine($"[{value.Key}] = {value.Value}");
			}

			Assert.IsTrue(counter.Keys.Min() >= 1);
			Assert.IsTrue(counter.Keys.Max() <= 15);
			Assert.IsTrue(counter.Keys.Max() >= 10);

			var test = sb.ToString();
        }
    }    
}

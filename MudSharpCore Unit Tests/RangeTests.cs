using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests
{
    [TestClass]
    public class RangeTests
    {
        [TestMethod]
        public void TestRange()
        {
            var range = new RankedRange<string>();
            range.Add("First", 0.0, 2.0);
            range.Add("Second", 2.0, 5.0);
            range.Add("Third", 5.0, 15.0);
            range.Add("Fourth", 15.0, 100.0);

            Assert.AreEqual(4, range.Count, $"Expected RankedRange.Count to give 4 items: instead got {range.Count}");
            Assert.AreEqual("Second", range.Find(3.5), "Expected to find string \"Second\" at position 3.5");
            Assert.AreEqual("Fourth", range.Find(30.5), "Expected to find string \"Fourth\" at position 30.5");
            Assert.AreEqual("Fourth", range.Find(300), "Expected to find string \"Fourth\" due to open upper bounds at position 300.0");
            Assert.AreEqual("First", range.Find(-0.5), "Expected to find string \"First\" due to open lower bounds at position -0.5");
        }

        [TestMethod]
        public void TestCircularRange()
        {
            var range = new CircularRange<string>(365.0, new[] { ("Autumn", 60.0), ("Winter", 150.0), ("Spring", 240.0), ("Summer", 330.0)});

            Assert.AreEqual("Autumn", range.Get(70.0), $"Expected 70th day of year to be autumn but it was {range.Get(70.0)}.");
            Assert.AreEqual("Summer", range.Get(2.0), $"Expected 2nd day of year to be summer but it was {range.Get(2.0)}.");
            Assert.AreEqual("Spring", range.Get(280.0), $"Expected 280th day of year to be spring but it was {range.Get(280.0)}.");
        }
    }
}

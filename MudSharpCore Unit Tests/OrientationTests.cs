using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Body;

namespace MudSharp_Unit_Tests
{
    /// <summary>
    /// Summary description for OrientationTests
    /// </summary>
    [TestClass]
    public class OrientationTests
    {
        public OrientationTests()
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
        public void TestWithin()
        {
            Assert.AreEqual(Orientation.Centre.WithinOffset(Orientation.High, 1), true, "Centre Orientation was not within 1 degree of offset with High.");
            Assert.AreEqual(Orientation.Centre.WithinOffset(Orientation.Highest, 1), false, "Centre Orientation was within 1 degree of offset with Highest.");
            Assert.AreEqual(Alignment.Front.WithinOffset(Alignment.FrontLeft, 1), true, "Front alignment was not within 1 degree offset to FrontLeft.");
            Assert.AreEqual(Alignment.Front.WithinOffset(Alignment.Left, 1), false, "Front alignment was within 1 degree offset to Left.");
            Assert.AreEqual(Alignment.Front.WithinOffset(Alignment.RearRight, 3), true, "Front alignment was not within 3 degree offset to RearRight.");
        }
    }
}

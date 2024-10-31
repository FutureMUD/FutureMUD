using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.ThirdPartyCode;

namespace MudSharp_Unit_Tests;

/// <summary>
/// Summary description for SchedulerTests
/// </summary>
[TestClass]
public class SchedulerTests
{
	public SchedulerTests()
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
	public void TestSchedulerOrder()
	{
		var scheduler = new RandomAccessPriorityQueue<DateTime, string>();
		var baseTime = DateTime.UtcNow;

		scheduler.Add(new KeyValuePair<DateTime, string>(baseTime + TimeSpan.FromSeconds(10), "10 Seconds"));
		scheduler.Add(new KeyValuePair<DateTime, string>(baseTime + TimeSpan.FromSeconds(5), "5 Seconds"));
		scheduler.Add(new KeyValuePair<DateTime, string>(baseTime + TimeSpan.FromSeconds(15), "15 Seconds"));
		scheduler.Add(new KeyValuePair<DateTime, string>(baseTime + TimeSpan.FromSeconds(11), "11 Seconds"));
		scheduler.Add(new KeyValuePair<DateTime, string>(baseTime + TimeSpan.FromSeconds(1), "1 Seconds"));
		scheduler.Add(new KeyValuePair<DateTime, string>(baseTime + TimeSpan.FromSeconds(3), "3 Seconds"));
		scheduler.Add(new KeyValuePair<DateTime, string>(baseTime + TimeSpan.FromSeconds(27), "27 Seconds"));
		scheduler.Add(new KeyValuePair<DateTime, string>(baseTime + TimeSpan.FromSeconds(190), "190 Seconds"));
		scheduler.Add(new KeyValuePair<DateTime, string>(baseTime + TimeSpan.FromSeconds(9), "9 Seconds"));
		scheduler.Add(new KeyValuePair<DateTime, string>(baseTime + TimeSpan.FromSeconds(0), "0 Seconds"));
		scheduler.Add(new KeyValuePair<DateTime, string>(baseTime + TimeSpan.FromSeconds(0.5), "0.5 Seconds"));
		scheduler.Add(new KeyValuePair<DateTime, string>(baseTime + TimeSpan.FromSeconds(100), "100 Seconds"));
		scheduler.Add(new KeyValuePair<DateTime, string>(baseTime + TimeSpan.FromSeconds(17000), "17000 Seconds"));

		Assert.AreEqual(true, scheduler.Peek().Key >= baseTime);
		Assert.AreEqual("0 Seconds", scheduler.Dequeue().Value);
		Assert.AreEqual(true, scheduler.Peek().Key > baseTime);
		Assert.AreEqual("0.5 Seconds", scheduler.Dequeue().Value);
		Assert.AreEqual(true, scheduler.Peek().Key > baseTime);
		Assert.AreEqual("1 Seconds", scheduler.Dequeue().Value);
		Assert.AreEqual(true, scheduler.Peek().Key > baseTime);
		Assert.AreEqual("3 Seconds", scheduler.Dequeue().Value);
		Assert.AreEqual(true, scheduler.Peek().Key > baseTime);
		Assert.AreEqual("5 Seconds", scheduler.Dequeue().Value);
		Assert.AreEqual("9 Seconds", scheduler.Dequeue().Value);

		scheduler.Add(new KeyValuePair<DateTime, string>(baseTime + TimeSpan.FromSeconds(3), "3 Seconds"));
		Assert.AreEqual("3 Seconds", scheduler.Dequeue().Value);
		Assert.AreEqual("10 Seconds", scheduler.Dequeue().Value);
		Assert.AreEqual("11 Seconds", scheduler.Dequeue().Value);

		scheduler.Remove(new KeyValuePair<DateTime, string>(baseTime + TimeSpan.FromSeconds(27), "27 Seconds"));
		Assert.AreEqual(true, scheduler.Peek().Key == (baseTime + TimeSpan.FromSeconds(15)));

		Assert.AreEqual("15 Seconds", scheduler.Dequeue().Value);
		Assert.AreEqual(true, scheduler.Peek().Key == (baseTime + TimeSpan.FromSeconds(100)));
		Assert.AreEqual("100 Seconds", scheduler.Dequeue().Value);
		Assert.AreEqual("190 Seconds", scheduler.Dequeue().Value);
		Assert.AreEqual("17000 Seconds", scheduler.Dequeue().Value);
		Assert.AreEqual(true, scheduler.IsEmpty);
	}
}
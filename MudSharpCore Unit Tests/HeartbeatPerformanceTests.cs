using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Diagnostics;
using MudSharp.Framework.Scheduling;
using System;
using System.Collections.Generic;
using System.Text;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HeartbeatPerformanceTests
{
	[TestMethod]
	public void ManuallyFireHeartbeatSecond_DiagnosticsEnabled_PreservesSubscriberOrder()
	{
		var monitor = new RuntimePerformanceMonitor();
		monitor.Enable();
		var gameworld = new Mock<IFuturemud>();
		gameworld.As<IRuntimePerformanceMonitorProvider>()
			.SetupGet(x => x.RuntimePerformanceMonitor)
			.Returns(monitor);
		var heartbeat = new HeartbeatManager(gameworld.Object);
		var fired = new List<string>();
		heartbeat.SecondHeartbeat += () => fired.Add("first");
		heartbeat.SecondHeartbeat += () => fired.Add("second");

		heartbeat.ManuallyFireHeartbeatSecond();

		CollectionAssert.AreEqual(new[] { "first", "second" }, fired);
		var report = new StringBuilder();
		monitor.AppendReport(report, System.Globalization.CultureInfo.InvariantCulture);
		StringAssert.Contains(report.ToString(), "Slowest heartbeat callbacks:");
	}

	[TestMethod]
	public void ManuallyFireHeartbeatSecond_SubscriberThrows_StopsFollowingSubscribers()
	{
		var heartbeat = new HeartbeatManager(new Mock<IFuturemud>().Object);
		var fired = false;
		heartbeat.SecondHeartbeat += () => throw new InvalidOperationException();
		heartbeat.SecondHeartbeat += () => fired = true;

		Assert.ThrowsException<InvalidOperationException>(heartbeat.ManuallyFireHeartbeatSecond);
		Assert.IsFalse(fired);
	}

	[TestMethod]
	public void ManualHeartbeatMethods_FireEveryHardAndFuzzyCadenceOnce()
	{
		var heartbeat = new HeartbeatManager(new Mock<IFuturemud>().Object);
		var fired = new List<string>();
		heartbeat.SecondHeartbeat += () => fired.Add("second");
		heartbeat.TenSecondHeartbeat += () => fired.Add("ten-second");
		heartbeat.ThirtySecondHeartbeat += () => fired.Add("thirty-second");
		heartbeat.MinuteHeartbeat += () => fired.Add("minute");
		heartbeat.HourHeartbeat += () => fired.Add("hour");
		heartbeat.FuzzyFiveSecondHeartbeat += () => fired.Add("fuzzy-five-second");
		heartbeat.FuzzyTenSecondHeartbeat += () => fired.Add("fuzzy-ten-second");
		heartbeat.FuzzyThirtySecondHeartbeat += () => fired.Add("fuzzy-thirty-second");
		heartbeat.FuzzyMinuteHeartbeat += () => fired.Add("fuzzy-minute");
		heartbeat.FuzzyHourHeartbeat += () => fired.Add("fuzzy-hour");

		heartbeat.ManuallyFireHeartbeatSecond();
		heartbeat.ManuallyFireHeartbeat5Second();
		heartbeat.ManuallyFireHeartbeat10Second();
		heartbeat.ManuallyFireHeartbeat30Second();
		heartbeat.ManuallyFireHeartbeatMinute();
		heartbeat.ManuallyFireHeartbeatHour();

		CollectionAssert.AreEquivalent(new[]
		{
			"second", "ten-second", "thirty-second", "minute", "hour", "fuzzy-five-second",
			"fuzzy-ten-second", "fuzzy-thirty-second", "fuzzy-minute", "fuzzy-hour"
		}, fired);
	}

	[TestMethod]
	public void HeartbeatSubscription_DuplicateDelegate_FiresOnce()
	{
		var heartbeat = new HeartbeatManager(new Mock<IFuturemud>().Object);
		var count = 0;
		HeartbeatManagerDelegate callback = () => count++;
		heartbeat.SecondHeartbeat += callback;
		heartbeat.SecondHeartbeat += callback;

		heartbeat.ManuallyFireHeartbeatSecond();

		Assert.AreEqual(1, count);
	}
}

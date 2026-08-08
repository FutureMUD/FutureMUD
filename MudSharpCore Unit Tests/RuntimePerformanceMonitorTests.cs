using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework.Diagnostics;
using System.Globalization;
using System.Text;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RuntimePerformanceMonitorTests
{
	[TestMethod]
	public void AppendReport_EnabledMonitor_IncludesRecordedLoopStatistics()
	{
		var monitor = new RuntimePerformanceMonitor();
		monitor.Enable();
		monitor.RecordLoopPhase(RuntimeLoopPhase.Scheduler, 100, 20);
		monitor.RecordLoopIteration(500, 30, overrun: false);

		var report = new StringBuilder();
		monitor.AppendReport(report, CultureInfo.InvariantCulture);

		StringAssert.Contains(report.ToString(), "Loop phases:");
		StringAssert.Contains(report.ToString(), "Scheduler:");
	}

	[TestMethod]
	public void AppendReport_NetworkSource_IncludesBoundedTransportStatistics()
	{
		var source = new TestNetworkPerformanceSource();
		var monitor = new RuntimePerformanceMonitor(source);
		monitor.Enable();

		var report = new StringBuilder();
		monitor.AppendReport(report, CultureInfo.InvariantCulture);

		Assert.AreEqual(1, source.ResetCount);
		StringAssert.Contains(report.ToString(), "Network transport:");
		StringAssert.Contains(report.ToString(), "accepted 2");
		StringAssert.Contains(report.ToString(), "queue high-water 16 commands");
	}

	private sealed class TestNetworkPerformanceSource : IRuntimeNetworkPerformanceSource
	{
		public int ResetCount { get; private set; }

		public RuntimeNetworkPerformanceSnapshot GetNetworkPerformanceSnapshot()
		{
			return new RuntimeNetworkPerformanceSnapshot(2, 1, 1, 100, 200, 3, 4, 16, 4096, 1, 0, 0, 0);
		}

		public void ResetNetworkPerformanceCounters()
		{
			ResetCount++;
		}
	}
}

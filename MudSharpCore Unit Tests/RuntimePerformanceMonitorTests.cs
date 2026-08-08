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
}

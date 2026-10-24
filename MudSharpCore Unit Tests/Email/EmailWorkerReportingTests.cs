#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Email;
using MudSharp.Server;

namespace MudSharp_Unit_Tests.Email;

[TestClass]
public class EmailWorkerReportingTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public async Task Worker_UnexpectedQueueFailure_ReportsOnceAndContainsLoggerFailure(bool brokenLogger)
	{
		var reports = new List<string>();
		var helper = new EmailHelper(report =>
		{
			reports.Add(report);
			if (brokenLogger) throw new IOException("fixture logging failure");
		});
		// Force a real outer worker failure without involving a mail provider or a global singleton.
		var signal = (SemaphoreSlim)typeof(EmailHelper).GetField("_queueSignal", BindingFlags.Instance | BindingFlags.NonPublic)!
			.GetValue(helper)!;
		signal.Dispose();

		await helper.EmailDelegateAsync(CancellationToken.None).WaitAsync(TimeSpan.FromSeconds(2));

		Assert.AreEqual(1, reports.Count);
		StringAssert.Contains(reports[0], "Email worker stopped unexpectedly");
		StringAssert.Contains(reports[0], "System.ObjectDisposedException");
		StringAssert.Contains(reports[0], "EmailDelegateAsync");
		StringAssert.Contains(reports[0], "Process terminating: False");
		StringAssert.Contains(reports[0], "Engine version:");
		helper.EndEmailThread();
		Assert.AreEqual(1, reports.Count);
	}

	[TestMethod]
	public async Task Worker_CancellationDuringWait_ProducesNoFailureReport()
	{
		var reports = new List<string>();
		var helper = new EmailHelper(reports.Add);
		using var cancellation = new CancellationTokenSource();
		var worker = helper.EmailDelegateAsync(cancellation.Token);
		Assert.IsFalse(worker.IsCompleted);
		cancellation.Cancel();
		await worker.WaitAsync(TimeSpan.FromSeconds(2));
		Assert.AreEqual(0, reports.Count);
	}

	[TestMethod]
	public void PrivateReport_ProviderMessagesAndData_AreOmittedThroughoutExceptionTree()
	{
		var error = new AggregateException("subject-secret", new Exception("recipient-secret"),
			new InvalidOperationException("oauth-secret"));
		error.Data["RequestId"] = "payload-secret";
		var report = ExceptionReporter.FormatPrivateFailure(error, "Email worker stopped unexpectedly");
		Assert.IsFalse(report.Contains("secret"));
		StringAssert.Contains(report, "System.AggregateException");
		StringAssert.Contains(report, "System.InvalidOperationException");
		StringAssert.Contains(report, "root/aggregate[1]");
	}
}

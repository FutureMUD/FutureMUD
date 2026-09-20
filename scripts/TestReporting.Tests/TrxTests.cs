using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestReporting;

namespace TestReporting.Tests;

[TestClass]
public class TrxTests
{
	private static string Fixture => Path.Combine(AppContext.BaseDirectory, "Fixtures", "expression-engine.trx");
	private static string TempFixture(Action<XDocument>? mutate = null)
	{
		var path = Path.Combine(Path.GetTempPath(), "futuremud-trx-" + Guid.NewGuid().ToString("N") + ".trx");
		var doc = XDocument.Load(Fixture);
		var times = doc.Root!.Element(doc.Root.Name.Namespace + "Times")!;
		var now = DateTimeOffset.UtcNow.ToString("O");
		foreach (var name in new[] { "creation", "queuing", "start", "finish" }) times.SetAttributeValue(name, now);
		mutate?.Invoke(doc);
		doc.Save(path);
		return path;
	}
	private static ParsedTrx Parse(string path) => Trx.Parse(path, "ExpressionEngine Unit Tests/ExpressionEngine Unit Tests.csproj", "net10.0", DateTimeOffset.UtcNow.AddMinutes(-1));

	[TestMethod]
	public void RealMstestReportCountsDataRowsOnce()
	{
		var path = TempFixture();
		try
		{
			var result = Parse(path);
			Assert.IsNull(result.Problem);
			Assert.AreEqual(36, result.Counts.Executed);
			Assert.AreEqual(36, result.Records.Count);
			Assert.IsTrue(result.Records.Count(x => x.Name.Contains(" (")) > 5);
			Assert.IsTrue(result.Records.All(x => !string.IsNullOrEmpty(x.FullyQualifiedName)));
			Assert.AreEqual(36, result.Records.Select(x => x.Id).Distinct().Count());
		}
		finally { File.Delete(path); }
	}

	[TestMethod]
	public void FailedAndSkippedRecordsRemainDistinct()
	{
		var path = TempFixture(doc =>
		{
			var ns = doc.Root!.Name.Namespace;
			var rows = doc.Root.Element(ns + "Results")!.Elements(ns + "UnitTestResult").ToList();
			rows[0].SetAttributeValue("outcome", "Failed");
			rows[0].Add(new XElement(ns + "Output", new XElement(ns + "ErrorInfo", new XElement(ns + "Message", "expected 2, got 3"), new XElement(ns + "StackTrace", "at fixture"))));
			rows[1].SetAttributeValue("outcome", "NotExecuted");
			var counters = doc.Root.Element(ns + "ResultSummary")!.Element(ns + "Counters")!;
			counters.SetAttributeValue("passed", "34");
			counters.SetAttributeValue("failed", "1");
			counters.SetAttributeValue("notExecuted", "1");
		});
		try
		{
			var result = Parse(path);
			Assert.IsNull(result.Problem);
			Assert.AreEqual(1, result.Counts.Failed);
			Assert.AreEqual(1, result.Counts.Skipped);
			Assert.AreEqual(35, result.Counts.Executed);
			StringAssert.Contains(result.Records[0].Message!, "expected 2");
			Assert.AreNotEqual(result.Records[0].Id, result.Records[1].Id);
		}
		finally { File.Delete(path); }
	}

	[TestMethod]
	public void NativeMstestIgnoreIsNotExecutedEvenWhenNativeCounterSaysZero()
	{
		var fixture = Path.Combine(AppContext.BaseDirectory, "Fixtures", "native-skip.trx");
		var path = Path.Combine(Path.GetTempPath(), "futuremud-native-skip-" + Guid.NewGuid().ToString("N") + ".trx");
		File.Copy(fixture, path);
		File.SetLastWriteTimeUtc(path, DateTime.UtcNow);
		try
		{
			var result = Parse(path);
			Assert.IsNull(result.Problem);
			Assert.AreEqual(1, result.Counts.Passed);
			Assert.AreEqual(1, result.Counts.Skipped);
			Assert.AreEqual(1, result.Counts.Executed);
			Assert.AreEqual("0", result.NativeCounters["notExecuted"]);
		}
		finally { File.Delete(path); }
	}

	[TestMethod]
	public void UnknownOutcomeAndContradictoryCountersAreVisible()
	{
		var path = TempFixture(doc =>
		{
			var ns = doc.Root!.Name.Namespace;
			doc.Root.Element(ns + "Results")!.Element(ns + "UnitTestResult")!.SetAttributeValue("outcome", "NewNativeOutcome");
		});
		try
		{
			var result = Parse(path);
			Assert.AreEqual("OTHER", result.Records[0].Outcome);
			StringAssert.Contains(result.Problem!, "counters disagree");
		}
		finally { File.Delete(path); }
	}

	[TestMethod]
	public void ForeignAssemblyReportIsRejected()
	{
		var path = TempFixture();
		try
		{
			var result = Trx.Parse(path, "Other.Tests.csproj", "net10.0", DateTimeOffset.UtcNow.AddMinutes(-1), "Other.Tests.dll");
			StringAssert.Contains(result.Problem!, "another assembly");
		}
		finally { File.Delete(path); }
	}

	[TestMethod]
	public void MissingMalformedAndDtdReportsAreRejected()
	{
		var missing = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
		Assert.AreEqual("REPORT_MISSING", Parse(missing).Problem);
		var malformed = Path.GetTempFileName();
		try
		{
			File.WriteAllText(malformed, "<TestRun");
			StringAssert.StartsWith(Parse(malformed).Problem!, "REPORT_INVALID");
			File.WriteAllText(malformed, "<!DOCTYPE TestRun [<!ENTITY x SYSTEM 'file:///etc/passwd'>]><TestRun id='x'>&x;</TestRun>");
			StringAssert.StartsWith(Parse(malformed).Problem!, "REPORT_INVALID");
		}
		finally { File.Delete(malformed); }
	}
}

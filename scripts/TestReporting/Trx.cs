using System.Xml;
using System.Xml.Linq;

namespace TestReporting;

internal sealed class ParsedTrx
{
	public List<TestRecord> Records { get; } = [];
	public Counts Counts { get; } = new();
	public Dictionary<string, string> NativeCounters { get; } = [];
	public string? Problem { get; set; }
}

internal static class Trx
{
	public static ParsedTrx Parse(string path, string project, string framework, DateTimeOffset phaseStart, string? expectedAssembly = null)
	{
		var parsed = new ParsedTrx();
		if (!File.Exists(path))
		{
			parsed.Problem = "REPORT_MISSING";
			return parsed;
		}

		try
		{
			if (File.GetLastWriteTimeUtc(path) < phaseStart.UtcDateTime.AddSeconds(-2))
			{
				parsed.Problem = "REPORT_INVALID: report predates invocation";
				return parsed;
			}
			using var stream = File.OpenRead(path);
			using var reader = XmlReader.Create(stream, new XmlReaderSettings
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null,
				MaxCharactersInDocument = 100_000_000
			});
			var document = XDocument.Load(reader, LoadOptions.None);
			var root = document.Root;
			if (root?.Name.LocalName != "TestRun" || root.Attribute("id") is null)
			{
				parsed.Problem = "REPORT_INVALID: missing TestRun identity";
				return parsed;
			}
			var ns = root.Name.Namespace;
			if (expectedAssembly is not null)
			{
				var creation = (string?)root.Element(ns + "Times")?.Attribute("creation");
				if (!DateTimeOffset.TryParse(creation, out var reportCreated) || reportCreated < phaseStart.AddMinutes(-1) || reportCreated > DateTimeOffset.UtcNow.AddMinutes(1))
				{
					parsed.Problem = "REPORT_INVALID: report run time does not match invocation";
					return parsed;
				}
			}
			var unitDefinitions = root.Element(ns + "TestDefinitions")?.Elements(ns + "UnitTest").ToList() ?? [];
			if (expectedAssembly is not null && unitDefinitions.Any(definition =>
			{
				var storage = (string?)definition.Attribute("storage");
				var codeBase = (string?)definition.Element(ns + "TestMethod")?.Attribute("codeBase");
				return !string.Equals(Path.GetFileName(storage), expectedAssembly, StringComparison.OrdinalIgnoreCase) ||
					!string.Equals(Path.GetFileName(codeBase), expectedAssembly, StringComparison.OrdinalIgnoreCase);
			}))
			{
				parsed.Problem = "REPORT_INVALID: report belongs to another assembly";
				return parsed;
			}
			var definitions = unitDefinitions
				.Where(x => x.Attribute("id") is not null)
				.ToDictionary(x => (string)x.Attribute("id")!, x => (string?)x.Element(ns + "TestMethod")?.Attribute("className") + "." + (string?)x.Element(ns + "TestMethod")?.Attribute("name"));
			var results = root.Element(ns + "Results")?.Elements(ns + "UnitTestResult").ToList() ?? [];
			if (expectedAssembly is not null && results.Count > 0 && unitDefinitions.Count == 0)
			{
				parsed.Problem = "REPORT_INVALID: test definitions absent";
				return parsed;
			}
			var ordinal = 0;
			foreach (var result in results)
			{
				ordinal++;
				var native = (string?)result.Attribute("outcome") ?? "Unknown";
				var outcome = native switch
				{
					"Passed" => "PASSED",
					"Failed" => "FAILED",
					"NotExecuted" => "SKIPPED",
					"Inconclusive" or "Aborted" or "Timeout" => "INCONCLUSIVE",
					_ => "OTHER"
				};
				var testId = (string?)result.Attribute("testId") ?? "";
				var executionId = (string?)result.Attribute("executionId") ?? "";
				var output = result.Element(ns + "Output");
				var error = output?.Element(ns + "ErrorInfo");
				var record = new TestRecord
				{
					Id = $"{project}|{framework}|1|{ordinal}",
					Project = project,
					Framework = framework,
					TestId = testId,
					ExecutionId = executionId,
					Name = (string?)result.Attribute("testName") ?? "",
					FullyQualifiedName = definitions?.GetValueOrDefault(testId),
					NativeOutcome = native,
					Outcome = outcome,
					Message = (string?)error?.Element(ns + "Message"),
					Stack = (string?)error?.Element(ns + "StackTrace"),
					Report = path
				};
				parsed.Records.Add(record);
				switch (outcome)
				{
					case "PASSED": parsed.Counts.Passed++; parsed.Counts.Executed++; break;
					case "FAILED": parsed.Counts.Failed++; parsed.Counts.Executed++; break;
					case "SKIPPED": parsed.Counts.Skipped++; break;
					case "INCONCLUSIVE": parsed.Counts.Inconclusive++; parsed.Counts.Executed++; break;
					default: parsed.Counts.Other++; parsed.Counts.Executed++; break;
				}
			}
			var counters = root.Element(ns + "ResultSummary")?.Element(ns + "Counters");
			if (counters is null)
			{
				parsed.Problem = "REPORT_INVALID: counters absent";
				return parsed;
			}
			foreach (var attribute in counters.Attributes())
			{
				parsed.NativeCounters[attribute.Name.LocalName] = attribute.Value;
			}
			if (!int.TryParse((string?)counters.Attribute("total"), out var total) || total != parsed.Counts.Total ||
				!int.TryParse((string?)counters.Attribute("passed"), out var passed) || passed != parsed.Counts.Passed ||
				!int.TryParse((string?)counters.Attribute("failed"), out var failed) || failed != parsed.Counts.Failed)
			{
				parsed.Problem = "REPORT_INVALID: counters disagree with test records";
			}
			var runOutcome = (string?)root.Element(ns + "ResultSummary")?.Attribute("outcome");
			if (runOutcome is "Aborted" or "Inconclusive" || runOutcome is not ("Completed" or "Failed"))
			{
				parsed.Problem ??= "REPORT_INVALID: run did not complete";
			}
			if (runOutcome == "Failed" && parsed.Counts.Failed == 0) parsed.Problem ??= "REPORT_INVALID: run outcome contradicts records";
		}
		catch (Exception ex) when (ex is XmlException or IOException or InvalidOperationException or ArgumentException)
		{
			parsed.Problem = "REPORT_INVALID: " + ex.Message;
		}
		return parsed;
	}
}

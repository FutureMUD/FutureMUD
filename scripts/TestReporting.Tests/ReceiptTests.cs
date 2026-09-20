using System.Text;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestReporting;

namespace TestReporting.Tests;

[TestClass]
public class ReceiptTests
{
	[TestMethod]
	public void ExtremelyLongScopeAndArtifactPathsStillProduceValidBoundedReceipts()
	{
		var summary = new Summary
		{
			RunId = "fixture-run", Status = "FAIL", Suite = "fast", Repository = Path.GetTempPath(),
			Head = "fixture-head", Projects = Enumerable.Range(0, 100).Select(i => "project-" + i + new string('x', 200)).ToList(),
			Artifacts = new Dictionary<string, string>
			{
				["summary"] = Path.Combine(Path.GetTempPath(), new string('s', 10_000)),
				["failures"] = Path.Combine(Path.GetTempPath(), new string('f', 10_000))
			},
			Issues = Enumerable.Range(0, 50).Select(_ => new Issue { Kind = "TEST_FAILED", Detail = new string('d', 1000) }).ToList()
		};
		var records = new List<TestRecord> { new() { Outcome = "FAILED", Name = "fixture", Message = new string('m', 100_000) } };
		var json = Program.JsonReceipt(summary, records);
		Assert.IsTrue(Encoding.UTF8.GetByteCount(json + Environment.NewLine) <= 8192);
		using var parsed = JsonDocument.Parse(json);
		Assert.AreEqual("FAIL", parsed.RootElement.GetProperty("status").GetString());
		Assert.AreEqual(97, parsed.RootElement.GetProperty("projects_omitted").GetInt32());
		Assert.IsTrue(parsed.RootElement.GetProperty("artifacts").GetProperty("path_omitted").GetBoolean());
		var compact = Program.Receipt(summary, records);
		Assert.IsTrue(Encoding.UTF8.GetByteCount(compact + Environment.NewLine) <= 8192);
		StringAssert.Contains(compact, "fixture-run");
	}
}

using System.Diagnostics;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestReporting.Tests;

[TestClass]
[DoNotParallelize]
public class RunnerTests
{
	private static string TestRoot
	{
		get
		{
			var directory = new DirectoryInfo(AppContext.BaseDirectory);
			while (directory.Name != "TestReporting.Tests") directory = directory.Parent ?? throw new InvalidOperationException("Test root absent");
			return directory.FullName;
		}
	}
	private static string Fixture => Path.Combine(TestRoot, "Fixtures", "expression-engine.trx");
	private static string Configuration => new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
	private static string Fake => Path.Combine(TestRoot, "FakeDotnet", "bin", Configuration, "net10.0", OperatingSystem.IsWindows() ? "FakeDotnet.exe" : "FakeDotnet");
	private static string Reporter => Path.Combine(TestRoot, "..", "TestReporting", "bin", Configuration, "net10.0", "TestReporting.dll");

	private sealed class Repo : IDisposable
	{
		public string PathValue { get; } = Path.Combine(Path.GetTempPath(), "futuremud-reporting-test-" + Guid.NewGuid().ToString("N"));
		public Repo()
		{
			Directory.CreateDirectory(Path.Combine(PathValue, "Sample Tests"));
			File.WriteAllText(Path.Combine(PathValue, "Sample Tests", "Sample Tests.csproj"), "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup><PackageReference Include=\"Microsoft.NET.Test.Sdk\" Version=\"17.9.0\" /></ItemGroup></Project>");
			File.WriteAllText(Path.Combine(PathValue, "input.txt"), "initial");
			File.WriteAllText(Path.Combine(PathValue, ".gitignore"), ".artifacts/\n**/bin/\n**/obj/\n");
			Command("git", "init", "-q");
			Command("git", "add", ".");
			Command("git", "-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid", "commit", "-qm", "fixture");
		}
		private void Command(string file, params string[] args)
		{
			var info = new ProcessStartInfo(file) { WorkingDirectory = PathValue, UseShellExecute = false, RedirectStandardError = true };
			foreach (var arg in args) info.ArgumentList.Add(arg);
			using var process = Process.Start(info)!;
			process.WaitForExit();
			if (process.ExitCode != 0) throw new InvalidOperationException(process.StandardError.ReadToEnd());
		}
		public void Dispose()
		{
			if (!Directory.Exists(PathValue)) return;
			foreach (var file in Directory.EnumerateFiles(PathValue, "*", SearchOption.AllDirectories)) File.SetAttributes(file, FileAttributes.Normal);
			Directory.Delete(PathValue, recursive: true);
		}
	}

	private static async Task<(int Exit, string Output, JsonDocument Json)> Run(Repo repo, string scenario, params string[] extra)
	{
		var start = new ProcessStartInfo("dotnet") { WorkingDirectory = repo.PathValue, UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
		var timeout = scenario == "timeout" ? "3" : "30";
		foreach (var arg in new[] { Reporter, "--repo", repo.PathValue, "--suite", "fast", "--project", "Sample Tests/Sample Tests.csproj", "--output-mode", "json", "--timeout-seconds", timeout }.Concat(extra)) start.ArgumentList.Add(arg);
		start.Environment["FUTUREMUD_TEST_DOTNET"] = scenario == "missing-sdk" ? Path.Combine(repo.PathValue, "no-such-dotnet") : Fake;
		start.Environment["FM_FAKE_SCENARIO"] = scenario;
		start.Environment["FM_FAKE_TRX"] = Fixture;
		start.Environment["DOTNET_PROCESSOR_COUNT"] = "2";
		using var process = Process.Start(start)!;
		using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(60));
		try { await process.WaitForExitAsync(deadline.Token); }
		catch (OperationCanceledException) { process.Kill(entireProcessTree: true); throw new AssertFailedException("Reporter hung."); }
		var output = await process.StandardOutput.ReadToEndAsync();
		var error = await process.StandardError.ReadToEndAsync();
		Assert.IsTrue(System.Text.Encoding.UTF8.GetByteCount(output + error) <= 8192, "Receipt exceeded 8 KiB.");
		Assert.IsTrue(string.IsNullOrEmpty(error), error);
		return (process.ExitCode, output, JsonDocument.Parse(output));
	}

	[TestMethod]
	public async Task PassFailAndContradictoryExitUseStructuredEvidence()
	{
		foreach (var scenario in new[] { ("pass", 0, "PASS"), ("fail", 1, "FAIL"), ("contradiction", 3, "INCONCLUSIVE") })
		{
			using var repo = new Repo();
			var result = await Run(repo, scenario.Item1);
			using (result.Json)
			{
				Assert.AreEqual(scenario.Item2, result.Exit, result.Output);
				Assert.AreEqual(scenario.Item3, result.Json.RootElement.GetProperty("status").GetString());
				Assert.IsTrue(File.Exists(result.Json.RootElement.GetProperty("artifacts").GetProperty("summary").GetString()));
				if (scenario.Item1 == "fail") Assert.AreEqual(1, result.Json.RootElement.GetProperty("failures").GetArrayLength());
			}
		}
	}

	[TestMethod]
	public async Task IncompleteReportsAndZeroTestsNeverPass()
	{
		foreach (var scenario in new[] { "missing", "malformed", "stale", "zero", "timeout", "changed" })
		{
			using var repo = new Repo();
			var result = await Run(repo, scenario);
			using (result.Json)
			{
				Assert.AreEqual(3, result.Exit, scenario);
				Assert.AreEqual("INCONCLUSIVE", result.Json.RootElement.GetProperty("status").GetString(), scenario);
			}
		}
	}

	[TestMethod]
	public async Task BuildFailureAndMissingRestoreKeepTestsNotRun()
	{
		foreach (var scenario in new[] { ("buildfail", 1, "FAIL"), ("prereq", 2, "BLOCKED"), ("missing-sdk", 2, "BLOCKED") })
		{
			using var repo = new Repo();
			var result = await Run(repo, scenario.Item1);
			using (result.Json)
			{
				Assert.AreEqual(scenario.Item2, result.Exit);
				Assert.AreEqual(scenario.Item3, result.Json.RootElement.GetProperty("status").GetString());
				var summary = JsonDocument.Parse(File.ReadAllText(result.Json.RootElement.GetProperty("artifacts").GetProperty("summary").GetString()!));
				using (summary) Assert.AreEqual("NOT_RUN", summary.RootElement.GetProperty("invocations")[0].GetProperty("status").GetString());
			}
		}
	}

	[TestMethod]
	public async Task SkipsAndLargeNativeOutputRemainBounded()
	{
		using (var repo = new Repo())
		{
			var result = await Run(repo, "skip", "--fail-on-skipped");
			using (result.Json)
			{
				Assert.AreEqual(3, result.Exit);
				Assert.AreEqual(1, result.Json.RootElement.GetProperty("counts").GetProperty("skipped").GetInt32());
			}
		}
		using (var repo = new Repo())
		{
			var result = await Run(repo, "failhuge");
			using (result.Json)
			{
				Assert.AreEqual(1, result.Exit);
				var summary = JsonDocument.Parse(File.ReadAllText(result.Json.RootElement.GetProperty("artifacts").GetProperty("summary").GetString()!));
				using (summary)
				{
					var log = summary.RootElement.GetProperty("invocations")[0].GetProperty("test").GetProperty("stdout").GetString()!;
					Assert.IsTrue(new FileInfo(log).Length >= 200_000);
				}
			}
		}
	}

	[TestMethod]
	public async Task CooperativeLockBlocksSecondRun()
	{
		using var repo = new Repo();
		var lockPath = Path.Combine(repo.PathValue, ".artifacts", "test-runs", ".worktree.lock");
		Directory.CreateDirectory(Path.GetDirectoryName(lockPath)!);
		using var ownership = new FileStream(lockPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
		File.WriteAllText(lockPath + ".owner", "active-fixture-run");
		using var result = (await Run(repo, "pass")).Json;
		Assert.AreEqual("BLOCKED", result.RootElement.GetProperty("status").GetString());
		StringAssert.Contains(result.RootElement.GetProperty("issues")[0].GetProperty("detail").GetString()!, "active-fixture-run");
		Assert.AreEqual(JsonValueKind.Null, result.RootElement.GetProperty("source_stable").ValueKind);
		ownership.Dispose();
		var released = await Run(repo, "pass");
		using (released.Json) Assert.AreEqual(0, released.Exit, released.Output);
	}

	[TestMethod]
	public async Task ConcurrentHostsDrainBothOutputStreams()
	{
		using var repo = new Repo();
		var extra = new List<string>();
		for (var index = 1; index < 8; index++)
		{
			var project = $"Project {index}/Sample Tests.csproj";
			Directory.CreateDirectory(Path.Combine(repo.PathValue, $"Project {index}"));
			File.Copy(Path.Combine(repo.PathValue, "Sample Tests", "Sample Tests.csproj"), Path.Combine(repo.PathValue, project));
			extra.AddRange(["--project", project]);
		}

		var result = await Run(repo, "concurrent-output", extra.ToArray());
		using (result.Json)
		{
			Assert.AreEqual(0, result.Exit, result.Output);
			Assert.AreEqual(8 * 36, result.Json.RootElement.GetProperty("counts").GetProperty("passed").GetInt32());
			using var summary = JsonDocument.Parse(File.ReadAllText(result.Json.RootElement.GetProperty("artifacts").GetProperty("summary").GetString()!));
			foreach (var invocation in summary.RootElement.GetProperty("invocations").EnumerateArray())
			{
				foreach (var stream in new[] { "stdout", "stderr" })
				{
					var log = File.ReadAllText(invocation.GetProperty("test").GetProperty(stream).GetString()!);
					Assert.IsTrue(log.Length >= 200_000, stream);
					StringAssert.EndsWith(log.TrimEnd(), "output-complete");
				}
			}
		}
	}

	[TestMethod]
	public async Task SavedFailureCanBeInspectedWithoutAnotherRun()
	{
		using var repo = new Repo();
		var result = await Run(repo, "fail");
		using (result.Json)
		{
			var failure = result.Json.RootElement.GetProperty("failures")[0].GetProperty("id").GetString()!;
			var runDir = Path.GetDirectoryName(result.Json.RootElement.GetProperty("artifacts").GetProperty("summary").GetString())!;
			var start = new ProcessStartInfo("dotnet") { WorkingDirectory = repo.PathValue, RedirectStandardOutput = true, UseShellExecute = false };
			foreach (var arg in new[] { Reporter, "inspect", "--run", runDir, "--failure", failure }) start.ArgumentList.Add(arg);
			using var process = Process.Start(start)!;
			var output = await process.StandardOutput.ReadToEndAsync();
			await process.WaitForExitAsync();
			Assert.AreEqual(0, process.ExitCode);
			using var inspected = JsonDocument.Parse(output);
			Assert.AreEqual(failure, inspected.RootElement[0].GetProperty("id").GetString());
			StringAssert.Contains(inspected.RootElement[0].GetProperty("message").GetString()!, "expected 2");
			Assert.AreEqual(1, Directory.GetDirectories(Path.Combine(repo.PathValue, ".artifacts", "test-runs")).Count(x => !Path.GetFileName(x).Equals("bootstrap", StringComparison.OrdinalIgnoreCase)));
		}
	}

	[TestMethod]
	public async Task FilterMetacharactersArePreservedAsOneLiteralArgument()
	{
		using var repo = new Repo();
		const string filter = "Name~αβ; $(echo unsafe) & 'quoted'";
		var result = await Run(repo, "zero", "--filter", filter);
		using (result.Json)
		{
			Assert.AreEqual(3, result.Exit);
			var summaryPath = result.Json.RootElement.GetProperty("artifacts").GetProperty("summary").GetString()!;
			using var summary = JsonDocument.Parse(File.ReadAllText(summaryPath));
			var args = summary.RootElement.GetProperty("invocations")[0].GetProperty("test").GetProperty("arguments").EnumerateArray().Select(x => x.GetString()).ToArray();
			var index = Array.IndexOf(args, "--filter");
			Assert.IsTrue(index >= 0);
			Assert.AreEqual(filter, args[index + 1]);
			Assert.AreEqual("NO_TESTS", summary.RootElement.GetProperty("issues")[0].GetProperty("kind").GetString());
		}
	}

	[TestMethod]
	public async Task CustomResultsRootDoesNotChangeSourceFingerprint()
	{
		using var repo = new Repo();
		var root = Path.Combine(repo.PathValue, "reports");
		var result = await Run(repo, "pass", "--results-root", root);
		using (result.Json)
		{
			Assert.AreEqual(0, result.Exit);
			Assert.IsTrue(result.Json.RootElement.GetProperty("source_stable").GetBoolean());
			StringAssert.StartsWith(result.Json.RootElement.GetProperty("artifacts").GetProperty("summary").GetString()!, root);
		}
	}
}

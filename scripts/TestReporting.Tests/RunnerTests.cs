using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
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
		private void Command(string file, params string[] args) => Output(file, args);
		public string Output(string file, params string[] args)
		{
			var info = new ProcessStartInfo(file) { WorkingDirectory = PathValue, UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
			foreach (var arg in args) info.ArgumentList.Add(arg);
			using var process = Process.Start(info)!;
			var stdout = Task.Factory.StartNew(() => process.StandardOutput.ReadToEnd(), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
			var stderr = Task.Factory.StartNew(() => process.StandardError.ReadToEnd(), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
			try
			{
				Assert.IsTrue(process.WaitForExit(15000), "Fixture command timed out.");
				Assert.IsTrue(Task.WaitAll([stdout, stderr], 5000), "Fixture output incomplete.");
				if (process.ExitCode != 0) throw new InvalidOperationException(stderr.Result);
				return stdout.Result;
			}
			finally
			{
				if (!process.HasExited) { process.Kill(entireProcessTree: true); Assert.IsTrue(process.WaitForExit(5000)); }
			}
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
		var timeout = scenario == "timeout" ? "3" : scenario.Contains("hang", StringComparison.Ordinal) ? "5" : "30";
		foreach (var arg in new[] { Reporter, "--repo", repo.PathValue, "--suite", "fast", "--project", "Sample Tests/Sample Tests.csproj", "--output-mode", "json", "--timeout-seconds", timeout }.Concat(extra)) start.ArgumentList.Add(arg);
		start.Environment["FUTUREMUD_TEST_DOTNET"] = scenario == "missing-sdk" ? Path.Combine(repo.PathValue, "no-such-dotnet") : Fake;
		start.Environment["FM_FAKE_SCENARIO"] = scenario.StartsWith("git-", StringComparison.Ordinal) ? scenario == "git-end-hang-fail" ? "fail" : "pass" : scenario;
		if (scenario.StartsWith("git-", StringComparison.Ordinal))
		{
			var control = Path.Combine(repo.PathValue, ".artifacts", "git-control");
			Directory.CreateDirectory(control);
			start.Environment["FUTUREMUD_TEST_GIT"] = scenario == "git-missing" ? Path.Combine(control, "no-such-git") : GitProcessTests.Fake;
			start.Environment["FM_FAKE_GIT_SCENARIO"] = scenario[4..];
			start.Environment["FM_FAKE_GIT_CONTROL"] = control;
			start.Environment["FM_FAKE_DOTNET_ACTIVITY"] = Path.Combine(control, "dotnet-activity");
		}
		start.Environment["FM_FAKE_TRX"] = Fixture;
		start.Environment["DOTNET_PROCESSOR_COUNT"] = "2";
		using var process = Process.Start(start)!;
		var stdout = Task.Factory.StartNew(() => process.StandardOutput.ReadToEnd(), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		var stderr = Task.Factory.StartNew(() => process.StandardError.ReadToEnd(), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(60));
		try { await process.WaitForExitAsync(deadline.Token); await Task.WhenAll(stdout, stderr).WaitAsync(TimeSpan.FromSeconds(5)); }
		catch (OperationCanceledException)
		{
			if (!process.HasExited) process.Kill(entireProcessTree: true);
			await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5));
			await Task.WhenAll(stdout, stderr).WaitAsync(TimeSpan.FromSeconds(5));
			throw new AssertFailedException("Reporter hung. " + stdout.Result + stderr.Result);
		}
		var output = stdout.Result;
		var error = stderr.Result;
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

	[DataTestMethod]
	[DataRow("git-missing", 2, "BLOCKED", "PREREQUISITE_UNAVAILABLE")]
	[DataRow("git-failure", 3, "INCONCLUSIVE", "REPORTING_ERROR")]
	[DataRow("git-initial-hang", 3, "INCONCLUSIVE", "TIMEOUT")]
	[DataRow("git-end-hang", 3, "INCONCLUSIVE", "TIMEOUT")]
	[DataRow("git-end-hang-fail", 1, "FAIL", "TIMEOUT")]
	public async Task InvalidGitEvidenceCannotBecomePassingVerification(string scenario, int exit, string status, string issue)
	{
		using var repo = new Repo();
		var elapsed = Stopwatch.StartNew();
		var result = await Run(repo, scenario);
		using (result.Json)
		{
			Assert.AreEqual(exit, result.Exit, result.Output);
			var receipt = result.Json.RootElement;
			Assert.AreEqual(status, receipt.GetProperty("status").GetString());
			Assert.AreEqual(JsonValueKind.Null, receipt.GetProperty("source_stable").ValueKind);
			Assert.AreEqual(JsonValueKind.Null, receipt.GetProperty("source_end").ValueKind);
			Assert.IsTrue(receipt.GetProperty("issues").EnumerateArray().Any(x => x.GetProperty("kind").GetString() == issue), result.Output);
			var control = Path.Combine(repo.PathValue, ".artifacts", "git-control");
			if (scenario.Contains("end-hang", StringComparison.Ordinal))
			{
				Assert.IsTrue(receipt.GetProperty("counts").GetProperty(scenario.EndsWith("fail", StringComparison.Ordinal) ? "failed" : "passed").GetInt32() > 0);
				Assert.IsTrue(receipt.GetProperty("issues").EnumerateArray().Any(x => x.GetProperty("kind").GetString() == "SOURCE_CHANGED"));
				Assert.AreEqual(2, File.ReadAllLines(Path.Combine(control, "invocations")).Count(x => x == "diff --binary HEAD --"));
			}
			else
			{
				Assert.AreEqual("", receipt.GetProperty("source_start").GetString());
				Assert.IsFalse(File.Exists(Path.Combine(control, "dotnet-activity")), "Build/test/SDK started without source evidence.");
				if (File.Exists(Path.Combine(control, "invocations")))
					Assert.AreEqual(scenario == "git-initial-hang" ? 3 : 1, File.ReadAllLines(Path.Combine(control, "invocations")).Length, "Repeated failed source capture.");
			}
			if (scenario.Contains("hang", StringComparison.Ordinal)) GitProcessTests.AssertExited(int.Parse(File.ReadAllText(Path.Combine(control, "pid"))));
			using var ownership = new FileStream(Path.Combine(repo.PathValue, ".artifacts", "test-runs", ".worktree.lock"), FileMode.Open, FileAccess.ReadWrite, FileShare.None);
			Assert.IsTrue(elapsed.Elapsed < TimeSpan.FromSeconds(30), elapsed.Elapsed.ToString());
		}
	}

	[TestMethod]
	public async Task SuccessfulGitWarningsDoNotChangeStableFingerprint()
	{
		using var repo = new Repo();
		var result = await Run(repo, "git-warnings");
		using (result.Json)
		{
			Assert.AreEqual(0, result.Exit, result.Output);
			Assert.IsTrue(result.Json.RootElement.GetProperty("source_stable").GetBoolean());
			Assert.AreEqual(result.Json.RootElement.GetProperty("source_start").GetString(), result.Json.RootElement.GetProperty("source_end").GetString());
		}
	}

	[TestMethod]
	public async Task RealGitFingerprintRetainsPreFixHashInputs()
	{
		using var repo = new Repo();
		File.AppendAllText(Path.Combine(repo.PathValue, "input.txt"), "\ntracked edit\n");
		File.WriteAllText(Path.Combine(repo.PathValue, "untracked file with spaces.txt"), "untracked\0content\n");
		File.WriteAllText(Path.Combine(repo.PathValue, "untracked.txt"), "second file");
		var root = Path.Combine(repo.PathValue, "reports");
		Directory.CreateDirectory(root);
		File.WriteAllText(Path.Combine(root, "excluded.json"), "excluded");
		using var expected = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
		void Add(string value) => expected.AppendData(Encoding.UTF8.GetBytes(value));
		Add(repo.Output("git", "rev-parse", "HEAD"));
		Add(repo.Output("git", "diff", "--binary", "HEAD", "--"));
		foreach (var relative in repo.Output("git", "ls-files", "--others", "--exclude-standard", "-z").Split('\0', StringSplitOptions.RemoveEmptyEntries).Order(StringComparer.Ordinal))
		{
			var full = Path.GetFullPath(relative, repo.PathValue);
			if (relative.StartsWith(".artifacts/", StringComparison.OrdinalIgnoreCase) || full.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) continue;
			Add(relative);
			expected.AppendData(SHA256.HashData(File.ReadAllBytes(full)));
		}
		const string project = "Sample Tests/Sample Tests.csproj";
		Add(Path.GetRelativePath(repo.PathValue, Path.Combine(repo.PathValue, project)));
		expected.AppendData(SHA256.HashData(File.ReadAllBytes(Path.Combine(repo.PathValue, project))));
		Assert.AreEqual(Convert.ToHexString(expected.GetHashAndReset()).ToLowerInvariant(),
			await Program.Fingerprint(repo.PathValue, [project], root, CancellationToken.None));
	}
}

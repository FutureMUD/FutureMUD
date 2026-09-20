using System.Diagnostics;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestReporting.Tests;

[TestClass]
[DoNotParallelize]
public class WrapperTests
{
	private static string Fake
	{
		get
		{
			var directory = new DirectoryInfo(AppContext.BaseDirectory);
			while (directory.Name != "TestReporting.Tests") directory = directory.Parent ?? throw new InvalidOperationException("Test root absent");
			var configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
			return Path.Combine(directory.FullName, "FakeDotnet", "bin", configuration, "net10.0", OperatingSystem.IsWindows() ? "FakeDotnet.exe" : "FakeDotnet");
		}
	}

	[TestMethod]
	public async Task BootstrapFailureAndDeadlineEmitBoundedJson()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory.Name != "TestReporting.Tests") directory = directory.Parent ?? throw new InvalidOperationException("Test root absent");
		var scripts = Path.GetFullPath(Path.Combine(directory.FullName, "..", "..", "scripts"));
		foreach (var scenario in new[] { ("buildfail", "REPORTING_ERROR"), ("bootstrap-hang", "TIMEOUT") })
		{
			var windows = OperatingSystem.IsWindows();
			var script = Path.Combine(scripts, windows ? "test-unit.ps1" : "test-unit.sh");
			var start = new ProcessStartInfo(windows ? "powershell.exe" : "bash") { WorkingDirectory = Path.GetDirectoryName(scripts)!, UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
			var budget = scenario.Item1 == "buildfail" ? "5" : "1";
			foreach (var argument in windows
				? new[] { "-NoProfile", "-ExecutionPolicy", "Bypass", "-File", script, "-OutputMode", "Json", "-TimeoutSeconds", budget }
				: new[] { script, "--output-mode", "json", "--timeout-seconds", budget }) start.ArgumentList.Add(argument);
			start.Environment["FUTUREMUD_REPORTER_BOOTSTRAP_DOTNET"] = Fake;
			start.Environment["FM_FAKE_SCENARIO"] = scenario.Item1;
			using var process = Process.Start(start)!;
			using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(20));
			try { await process.WaitForExitAsync(deadline.Token); }
			catch (OperationCanceledException) { process.Kill(entireProcessTree: true); throw new AssertFailedException("Bootstrap wrapper hung."); }
			var output = await process.StandardOutput.ReadToEndAsync();
			var error = await process.StandardError.ReadToEndAsync();
			Assert.AreEqual(3, process.ExitCode, error);
			Assert.IsTrue(string.IsNullOrWhiteSpace(error), error);
			Assert.IsTrue(System.Text.Encoding.UTF8.GetByteCount(output) <= 8192);
			using var json = JsonDocument.Parse(output);
			Assert.AreEqual("INCONCLUSIVE", json.RootElement.GetProperty("status").GetString());
			Assert.AreEqual(scenario.Item2, json.RootElement.GetProperty("issues")[0].GetProperty("kind").GetString());
		}
	}
	[TestMethod]
	public async Task EveryPlatformWrapperForwardsJsonAndBlockedExit()
	{
		var directory = new DirectoryInfo(AppContext.BaseDirectory);
		while (directory.Name != "TestReporting.Tests") directory = directory.Parent ?? throw new InvalidOperationException("Test root absent");
		var scripts = Path.GetFullPath(Path.Combine(directory.FullName, "..", "..", "scripts"));
		foreach (var suffix in new[] { "", "-core", "-climate" })
		{
			var windows = OperatingSystem.IsWindows();
			var script = Path.Combine(scripts, "test-unit" + suffix + (windows ? ".ps1" : ".sh"));
			var start = new ProcessStartInfo(windows ? "powershell.exe" : "bash")
			{
				WorkingDirectory = Path.GetDirectoryName(scripts)!, UseShellExecute = false,
				RedirectStandardOutput = true, RedirectStandardError = true
			};
			if (windows)
			{
				foreach (var argument in new[] { "-NoProfile", "-ExecutionPolicy", "Bypass", "-File", script, "-OutputMode", "Json", "-TimeoutSeconds", "0" }) start.ArgumentList.Add(argument);
			}
			else
			{
				foreach (var argument in new[] { script, "--output-mode", "json", "--timeout-seconds", "0" }) start.ArgumentList.Add(argument);
			}
			using var process = Process.Start(start)!;
			using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(30));
			try { await process.WaitForExitAsync(deadline.Token); }
			catch (OperationCanceledException) { process.Kill(entireProcessTree: true); throw new AssertFailedException("Wrapper bootstrap hung: " + script); }
			var output = await process.StandardOutput.ReadToEndAsync();
			var error = await process.StandardError.ReadToEndAsync();
			Assert.AreEqual(2, process.ExitCode, script + " " + error);
			Assert.IsTrue(string.IsNullOrWhiteSpace(error), script + " " + error);
			using var receipt = JsonDocument.Parse(output);
			Assert.AreEqual("BLOCKED", receipt.RootElement.GetProperty("status").GetString());
			Assert.AreEqual("INVALID_INVOCATION", receipt.RootElement.GetProperty("issues")[0].GetProperty("kind").GetString());
		}
	}
}

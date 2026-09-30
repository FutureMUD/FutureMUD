#nullable enable
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestReporting.Tests;

[TestClass]
[DoNotParallelize]
public class GitProcessTests
{
	internal static string TestRoot
	{
		get
		{
			var directory = new DirectoryInfo(AppContext.BaseDirectory);
			while (directory.Name != "TestReporting.Tests") directory = directory.Parent ?? throw new InvalidOperationException("Test root absent");
			return directory.FullName;
		}
	}
	internal static string Fake => Path.Combine(TestRoot, "FakeGit", "bin", new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name,
		"net10.0", OperatingSystem.IsWindows() ? "FakeGit.exe" : "FakeGit");
	private const string Tail = "alpha\0omega\nno-final-newline";

	private sealed class Control : IDisposable
	{
		public string DirectoryPath { get; } = Path.Combine(Path.GetTempPath(), "fm-git-" + Guid.NewGuid().ToString("N"));
		public Control() => Directory.CreateDirectory(DirectoryPath);
		public void Dispose()
		{
			foreach (var name in new[] { "pid", "descendant-pid" })
			{
				var marker = Path.Combine(DirectoryPath, name);
				if (File.Exists(marker)) KillOwned(int.Parse(File.ReadAllText(marker)));
			}
			Directory.Delete(DirectoryPath, recursive: true);
		}
	}

	internal static void KillOwned(int pid)
	{
		try
		{
			using var process = Process.GetProcessById(pid);
			if (!process.HasExited) process.Kill(entireProcessTree: true);
			Assert.IsTrue(process.WaitForExit(5000), "Owned fixture process could not be reaped: " + pid);
		}
		catch (ArgumentException) { /* Already exited and reaped. */ }
	}

	internal static void AssertExited(int pid)
	{
		try { using var process = Process.GetProcessById(pid); Assert.IsTrue(process.HasExited, "Owned process survived: " + pid); }
		catch (ArgumentException) { /* Process no longer exists. */ }
	}

	internal static async Task Ready(string marker)
	{
		using var watchdog = new CancellationTokenSource(TimeSpan.FromSeconds(15));
		while (!File.Exists(marker)) await Task.Delay(10, watchdog.Token);
	}

	private static Task<string> Read(Control control, string scenario, CancellationToken token = default, TimeSpan? ceiling = null,
		TimeSpan? drain = null) => GitProcess.Read(control.DirectoryPath, ["probe", scenario, control.DirectoryPath], token, Fake, ceiling, drain);

	[DataTestMethod]
	[DataRow("stderr-first")]
	[DataRow("stdout-first")]
	[DataRow("alternating")]
	public async Task SaturatedPipesPreserveEveryStdoutCharacter(string scenario)
	{
		using var control = new Control();
		var output = await Read(control, scenario).WaitAsync(TimeSpan.FromSeconds(60));
		var expected = (scenario == "stderr-first" ? "" : new string('W', 2 * 1024 * 1024)) + Tail;
		Assert.IsTrue(string.Equals(expected, output, StringComparison.Ordinal), $"Stdout mismatch: expected {expected.Length} characters, got {output.Length}.");
		AssertExited(int.Parse(File.ReadAllText(Path.Combine(control.DirectoryPath, "pid"))));
	}

	[TestMethod]
	public async Task OldSerialReaderStallsUnderBoundedWatchdog()
	{
		using var control = new Control();
		var start = new ProcessStartInfo(Fake) { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
		foreach (var arg in new[] { "probe", "stderr-first", control.DirectoryPath }) start.ArgumentList.Add(arg);
		using var process = Process.Start(start)!;
		// Deliberately reproduce the old stdout-first read while stderr is unread.
		var stdout = Task.Factory.StartNew(() => process.StandardOutput.ReadToEnd(), CancellationToken.None,
			TaskCreationOptions.LongRunning, TaskScheduler.Default);
		Task<string>? stderr = null;
		try
		{
			await Ready(Path.Combine(control.DirectoryPath, "ready"));
			await Assert.ThrowsExceptionAsync<TimeoutException>(() => stdout.WaitAsync(TimeSpan.FromSeconds(1)));
			Assert.IsFalse(process.HasExited, "Serial reader unexpectedly completed saturation fixture.");
		}
		finally
		{
			if (!process.HasExited) process.Kill(entireProcessTree: true);
			stderr = Task.Factory.StartNew(() => process.StandardError.ReadToEnd(), CancellationToken.None,
				TaskCreationOptions.LongRunning, TaskScheduler.Default);
			await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5));
			await Task.WhenAll(stdout, stderr).WaitAsync(TimeSpan.FromSeconds(5));
		}
	}

	[TestMethod]
	public async Task NonzeroExitRejectsPartialOutputAndSanitizesDiagnostics()
	{
		using var control = new Control();
		var error = await Assert.ThrowsExceptionAsync<IOException>(() => Read(control, "failure").WaitAsync(TimeSpan.FromSeconds(60)));
		StringAssert.Contains(error.Message, "exited 7");
		StringAssert.Contains(error.Message, "probe failure");
		Assert.IsFalse(error.Message.Contains("fixture-sensitive-value", StringComparison.Ordinal));
		Assert.IsFalse(error.Message.Contains('\u001b') || error.Message.Contains('\u0001'));
		Assert.IsTrue(error.Message.Length < 600, error.Message);
		Assert.IsFalse(error.Message.Contains("partial stdout", StringComparison.Ordinal));
	}

	[TestMethod]
	public async Task MissingExecutableIsPrerequisiteUnavailable()
	{
		using var control = new Control();
		var error = await Assert.ThrowsExceptionAsync<PrerequisiteException>(() => GitProcess.Read(control.DirectoryPath,
			["rev-parse", "HEAD"], default, Path.Combine(control.DirectoryPath, "no-such-git")));
		StringAssert.Contains(error.Message, "could not start");
		Assert.IsFalse(File.Exists(Path.Combine(control.DirectoryPath, "pid")));
	}

	[DataTestMethod]
	[DataRow("quiet", false)]
	[DataRow("output", false)]
	[DataRow("quiet", true)]
	[DataRow("output", true)]
	[DataRow("tree", true)]
	public async Task TimeoutAndCancellationReapOwnedProcesses(string scenario, bool callerCancellation)
	{
		using var control = new Control();
		using var cancellation = new CancellationTokenSource();
		var elapsed = Stopwatch.StartNew();
		var read = Read(control, scenario, cancellation.Token, TimeSpan.FromSeconds(callerCancellation ? 30 : 5));
		await Ready(Path.Combine(control.DirectoryPath, "ready"));
		if (callerCancellation) cancellation.Cancel();
		var error = await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => read.WaitAsync(TimeSpan.FromSeconds(30)));
		StringAssert.Contains(error.Message, callerCancellation ? "cancelled" : "Git operation ceiling");
		Assert.IsFalse(error.Message.Contains("cleanup", StringComparison.Ordinal), error.Message);
		Assert.IsTrue(elapsed.Elapsed < TimeSpan.FromSeconds(25));
		AssertExited(int.Parse(File.ReadAllText(Path.Combine(control.DirectoryPath, "pid"))));
		if (scenario == "tree") AssertExited(int.Parse(File.ReadAllText(Path.Combine(control.DirectoryPath, "descendant-pid"))));
	}

	[TestMethod]
	public async Task AlreadyCancelledTokenStartsNoChild()
	{
		using var control = new Control();
		using var cancellation = new CancellationTokenSource();
		cancellation.Cancel();
		await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => Read(control, "quiet", cancellation.Token));
		Assert.IsFalse(File.Exists(Path.Combine(control.DirectoryPath, "invocations")));
	}

	[TestMethod]
	public async Task InheritedPipeRejectsIncompleteEvidenceAndBoundsCleanup()
	{
		using var control = new Control();
		var elapsed = Stopwatch.StartNew();
		var error = await Assert.ThrowsExceptionAsync<IOException>(() => Read(control, "inherited", drain: TimeSpan.FromMilliseconds(300))
			.WaitAsync(TimeSpan.FromSeconds(30)));
		StringAssert.Contains(error.Message, "output drain incomplete");
		StringAssert.Contains(error.Message, "cleanup");
		Assert.IsTrue(elapsed.Elapsed < TimeSpan.FromSeconds(25), elapsed.Elapsed.ToString());
		AssertExited(int.Parse(File.ReadAllText(Path.Combine(control.DirectoryPath, "pid"))));
		// Parent exit cannot prove descendant exit. Control.Dispose owns its cleanup.
		var descendant = int.Parse(File.ReadAllText(Path.Combine(control.DirectoryPath, "descendant-pid")));
		using var process = Process.GetProcessById(descendant);
		Assert.IsFalse(process.HasExited, "Fixture did not retain the inherited pipe.");
	}
}

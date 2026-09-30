#nullable enable
using System.Diagnostics;
using System.Text;

namespace TestReporting;

internal static class GitProcess
{
	internal static readonly TimeSpan OperationCeiling = TimeSpan.FromSeconds(60);
	private static readonly TimeSpan DrainAllowance = TimeSpan.FromSeconds(5);
	private static readonly TimeSpan CleanupAllowance = TimeSpan.FromSeconds(10);
	private const int DiagnosticLimit = 4096;

	internal static async Task<string> Read(string repo, string[] arguments, CancellationToken cancellation,
		string executable = "git", TimeSpan? operationCeiling = null, TimeSpan? drainAllowance = null)
	{
		cancellation.ThrowIfCancellationRequested();
		using var ceiling = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
		ceiling.CancelAfter(operationCeiling ?? OperationCeiling);
		var operation = "git " + Program.Clean(string.Join(" ", arguments), 100);
		var start = new ProcessStartInfo(executable)
		{
			WorkingDirectory = repo, RedirectStandardOutput = true, RedirectStandardError = true,
			UseShellExecute = false, CreateNoWindow = true
		};
		foreach (var argument in arguments) start.ArgumentList.Add(argument);
		var process = new Process { StartInfo = start };
		try
		{
			ceiling.Token.ThrowIfCancellationRequested();
			if (!process.Start()) throw new InvalidOperationException("Process did not start.");
		}
		catch (OperationCanceledException) { process.Dispose(); throw; }
		catch (Exception ex) { process.Dispose(); throw new PrerequisiteException(operation + " could not start: " + Program.Clean(ex.Message, 180)); }

		Task<string>? stdout = null;
		Task<string>? stderr = null;
		try
		{
			// Retain StreamReader's existing text decoding and every stdout character.
			// Windows synchronous pipes must not occupy the deadline's pool workers.
			stdout = ReadStream(process.StandardOutput, retainAll: true);
			stderr = ReadStream(process.StandardError, retainAll: false);
			await process.WaitForExitAsync(ceiling.Token);
			await Task.WhenAll(stdout, stderr).WaitAsync(drainAllowance ?? DrainAllowance, ceiling.Token);
			ceiling.Token.ThrowIfCancellationRequested();
			if (process.ExitCode != 0)
				throw new IOException(operation + " exited " + process.ExitCode + ": " + Program.Clean(stderr.Result, 200));
			var result = stdout.Result;
			process.Dispose();
			return result;
		}
		catch (Exception ex)
		{
			var cleanup = await Cleanup(process, stdout, stderr);
			var reason = ex is OperationCanceledException
				? cancellation.IsCancellationRequested ? "exceeded the run deadline or was cancelled" : "exceeded its Git operation ceiling"
				: ex is TimeoutException ? "output drain incomplete" : Program.Clean(ex.Message, 250);
			var detail = operation + ": " + reason + cleanup;
			if (ex is OperationCanceledException) throw new OperationCanceledException(detail, ex, ceiling.Token);
			throw new IOException(detail, ex);
		}
	}

	private static Task<string> ReadStream(StreamReader reader, bool retainAll)
	{
		if (OperatingSystem.IsWindows())
			return Task.Factory.StartNew(() => ReadText(reader, retainAll), CancellationToken.None,
				TaskCreationOptions.LongRunning, TaskScheduler.Default);
		return ReadTextAsync(reader, retainAll);
	}

	private static string ReadText(StreamReader reader, bool retainAll)
	{
		var text = new StringBuilder();
		var buffer = new char[8192];
		int count;
		while ((count = reader.Read(buffer, 0, buffer.Length)) > 0) Append(text, buffer, count, retainAll);
		return text.ToString();
	}

	private static async Task<string> ReadTextAsync(StreamReader reader, bool retainAll)
	{
		var text = new StringBuilder();
		var buffer = new char[8192];
		int count;
		while ((count = await reader.ReadAsync(buffer.AsMemory())) > 0) Append(text, buffer, count, retainAll);
		return text.ToString();
	}

	private static void Append(StringBuilder text, char[] buffer, int count, bool retainAll)
	{
		// The excerpt cap never stops consumption of stderr.
		var retained = retainAll ? count : Math.Min(count, DiagnosticLimit - text.Length);
		if (retained > 0) text.Append(buffer, 0, retained);
	}

	private static async Task<string> Cleanup(Process process, Task<string>? stdout, Task<string>? stderr)
	{
		var clock = Stopwatch.StartNew();
		var notes = new List<string>();
		TimeSpan Remaining() => TimeSpan.FromTicks(Math.Max(1, (CleanupAllowance - clock.Elapsed).Ticks));
		try { if (!process.HasExited) process.Kill(entireProcessTree: true); }
		catch (Exception ex) { notes.Add("owned tree termination uncertain: " + Program.Clean(ex.Message, 100)); }
		try { await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5)); }
		catch (Exception ex) { notes.Add("owned parent reap incomplete: " + Program.Clean(ex.Message, 100)); }
		var readers = new[] { stdout, stderr }.Where(x => x is not null).Cast<Task<string>>().ToArray();
		var drains = Task.WhenAll(readers);
		Observe(drains);
		try { await drains.WaitAsync(Remaining()); }
		catch (Exception ex) { notes.Add("reader cleanup incomplete: " + Program.Clean(ex.Message, 100)); }
		// Closing a pipe can itself wait on a synchronous Windows read. Bound that
		// close too, and observe faults even when the OS cannot complete cleanup.
		var close = Task.Factory.StartNew(() =>
		{
			process.StandardOutput.Dispose();
			process.StandardError.Dispose();
			process.Dispose();
		}, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		Observe(close);
		try { await close.WaitAsync(Remaining()); }
		catch (Exception ex) { notes.Add("pipe close uncertain: " + Program.Clean(ex.Message, 100)); }
		if (!drains.IsCompleted) notes.Add("owned descendants may still hold pipes; readers remain incomplete");
		return notes.Count == 0 ? "" : "; cleanup: " + string.Join("; ", notes);
	}

	private static void Observe(Task task) => task.ContinueWith(completed => { _ = completed.Exception; },
		CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
}

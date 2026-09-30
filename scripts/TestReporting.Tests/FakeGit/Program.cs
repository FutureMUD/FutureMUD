#nullable enable
using System.Diagnostics;

namespace FakeGit;

internal static class Program
{
	private const string Tail = "alpha\0omega\nno-final-newline";
	private static async Task<int> Main(string[] args)
	{
		var probe = args.Length == 3 && args[0] == "probe";
		var scenario = probe ? args[1] : Environment.GetEnvironmentVariable("FM_FAKE_GIT_SCENARIO") ?? "warnings";
		var control = probe ? args[2] : Environment.GetEnvironmentVariable("FM_FAKE_GIT_CONTROL")!;
		Directory.CreateDirectory(control);
		var command = string.Join(" ", args);
		var valid = probe || command is "rev-parse HEAD" or "diff --binary HEAD --" or "ls-files --others --exclude-standard -z";
		if (!valid) { Console.Error.Write("unexpected Git arguments: " + command); return 91; }
		File.AppendAllText(Path.Combine(control, "invocations"), command + "\n");
		if (scenario != "descendant") File.WriteAllText(Path.Combine(control, "pid"), Environment.ProcessId.ToString());
		if (scenario == "descendant")
		{
			File.WriteAllText(Path.Combine(control, "descendant-pid"), Environment.ProcessId.ToString());
			File.WriteAllText(Path.Combine(control, "descendant-ready"), "ready");
			await Task.Delay(Timeout.Infinite);
		}
		if (scenario is "tree" or "inherited")
		{
			var child = new ProcessStartInfo(Environment.ProcessPath!) { UseShellExecute = false };
			foreach (var arg in new[] { "probe", "descendant", control }) child.ArgumentList.Add(arg);
			using var descendant = Process.Start(child)!;
			using var readyBudget = new CancellationTokenSource(TimeSpan.FromSeconds(15));
			while (!File.Exists(Path.Combine(control, "descendant-ready"))) await Task.Delay(10, readyBudget.Token);
		}
		var diffs = File.ReadAllLines(Path.Combine(control, "invocations")).Count(x => x == "diff --binary HEAD --");
		var hang = scenario is "quiet" or "output" or "tree" ||
			command == "diff --binary HEAD --" && (scenario == "initial-hang" || scenario is "end-hang" or "end-hang-fail" && diffs == 2);
		File.WriteAllText(Path.Combine(control, "ready"), "ready");
		if (hang)
		{
			if (scenario == "output") while (true) { Console.Out.Write(new string('W', 8192)); Console.Error.Write(new string('E', 8192)); await Task.Delay(10); }
			await Task.Delay(Timeout.Infinite);
		}
		if (scenario == "inherited") { Console.Out.Write(Tail); return 0; }
		if (scenario == "failure")
		{
			Console.Out.Write("partial stdout is not evidence");
			Console.Error.Write("\u001b[31msecret=fixture-sensitive-value\u001b[0m\u0001 diagnostic\n");
			Console.Error.Write(new string('E', 2 * 1024 * 1024));
			return 7;
		}
		if (scenario == "stderr-first") Console.Error.Write(new string('E', 2 * 1024 * 1024));
		if (scenario == "stdout-first")
		{
			Console.Out.Write(new string('W', 2 * 1024 * 1024));
			Console.Error.Write(new string('E', 2 * 1024 * 1024));
		}
		if (scenario == "alternating")
		{
			for (var i = 0; i < 256; i++) { Console.Out.Write(new string('W', 8192)); Console.Error.Write(new string('E', 8192)); }
		}
		if (probe) { Console.Out.Write(Tail); return 0; }
		Console.Error.Write("warning: fixture warning stream\n");
		Console.Out.Write(command switch { "rev-parse HEAD" => "fixture-head\n", "diff --binary HEAD --" => "fixture-diff\0tail", _ => "" });
		return 0;
	}
}

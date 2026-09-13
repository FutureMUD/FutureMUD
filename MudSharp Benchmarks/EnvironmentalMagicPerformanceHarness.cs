#nullable enable

using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.Json;
using MudSharp.Construction;
using MudSharp.Magic.Environment;
using MudSharp.Testing.EnvironmentalMagic;

namespace MudSharp_Benchmarks;

/// <summary>Opt-in sustained workload measurements, without a database or wall-clock sleeping.</summary>
internal static class EnvironmentalMagicPerformanceHarness
{
	public static void Run(string[] args)
	{
		var outputPath = ReadOption(args, "--output") ?? "environmental-magic-measurements.json";
		var sampleSeconds = int.Parse(ReadOption(args, "--sample-seconds") ?? "3720", CultureInfo.InvariantCulture);
		var warmupSeconds = int.Parse(ReadOption(args, "--warmup-seconds") ?? "120", CultureInfo.InvariantCulture);
		var sizes = (ReadOption(args, "--sizes") ?? "1000,10000,30000").Split(',')
			.Select(value => int.Parse(value, CultureInfo.InvariantCulture)).ToArray();
		var budget = double.Parse(ReadOption(args, "--budget-ms") ?? "5", CultureInfo.InvariantCulture);
		var visits = int.Parse(ReadOption(args, "--cell-visits") ?? "1024", CultureInfo.InvariantCulture);
		if (sampleSeconds < 1 || warmupSeconds < 0 || sizes.Any(size => size < 1))
			throw new ArgumentException("Positive sizes and sample seconds, and non-negative warm-up seconds, are required.");
		var options = new EnvironmentalMagicOptions { SoftBudgetMilliseconds = budget, MaximumCellVisits = visits };
		options.Validate();
		var report = new HarnessReport(DateTimeOffset.UtcNow, RuntimeInformation.FrameworkDescription,
			RuntimeInformation.OSDescription, RuntimeInformation.ProcessArchitecture.ToString(), System.Environment.ProcessorCount,
			GC.GetGCMemoryInfo().TotalAvailableMemoryBytes,
			System.Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "not reported",
			options, warmupSeconds, sampleSeconds,
			"In-process real Cell/CellOverlay/Terrain/SimpleMagicResource/EnvironmentalMagicGenerator, repository expressions, " +
			"native ForagableProfile or compiled non-static location FutureProg, and HeartbeatManager callback. " +
			"Monotonic/UTC test clock advances one simulated second per pump without sleeping. World facade uses Moq; " +
			"registries are dictionary-backed substitutes, and saves/explicit-operation store are in-memory sinks. " +
			"Reported allocations include the mock facade and fixture tick instrumentation. No database latency is included. " +
			"Formula counts count actual maximum/rate evaluations; sample counts count complete cell input snapshots. " +
			"Active-update age is a conservative bound: configured active cadence plus oldest ready-work lateness, " +
			"which can include a delayed audit/dirty queue. Zero means there was no accepted active work. " +
			"One pre-existing scheduler heartbeat entry is retained; environmental minute delegates remain zero.", new());
		foreach (var size in sizes)
		foreach (var outputs in new[] { 1, 3 })
		foreach (var activePercent in new[] { 0.0, 1.0, 10.0, 100.0 })
		foreach (var policy in new[] { "constant", "native-yield", "compiled-prog" })
		{
			Console.WriteLine($"Environment matrix: {size} cells, {outputs} outputs, {activePercent}% active, {policy}");
			using var world = new EnvironmentalMagicTestWorld(size, outputs, activePercent, policy, options);
			world.RunSeconds(warmupSeconds);
			world.ResetSavedFlags();
			report.Results.Add(Measure(world, "steady", size, outputs, activePercent, policy, sampleSeconds));
			WriteReport(outputPath, report);
		}

		foreach (var size in sizes)
		foreach (var outputs in new[] { 1, 3 })
		foreach (var policy in new[] { "constant", "native-yield", "compiled-prog" })
		{
			Console.WriteLine($"Environment bootstrap: {size} cells, {outputs} outputs, {policy}");
			using var bootstrap = new EnvironmentalMagicTestWorld(size, outputs, 100.0, policy, options, missingBalances: true);
			report.Results.Add(Measure(bootstrap, "all-empty-bootstrap", size, outputs, 100.0, policy, Math.Max(120, warmupSeconds)));
			WriteReport(outputPath, report);
		}

		foreach (var size in sizes)
		foreach (var outputs in new[] { 1, 3 })
		foreach (var policy in new[] { "constant", "native-yield", "compiled-prog" })
		{
			Console.WriteLine($"Environment bulk source change: {size} cells, {outputs} outputs, {policy}");
			using var burst = new EnvironmentalMagicTestWorld(size, outputs, 0.0, policy, options);
			burst.RunSeconds(warmupSeconds);
			burst.ResetSavedFlags();
			var changeWatch = Stopwatch.StartNew();
			if (policy == "native-yield")
			{
				foreach (var cell in burst.Cells.Cast<Cell>()) cell.ConsumeYield("herbs", 50.0);
			}
			else if (policy == "compiled-prog")
			{
				var prog = burst.Progs.Get(1)!;
				prog.FunctionText = "if (@where.id > 0)\nreturn 40 + 10\nend if\nreturn 0";
				if (!prog.Compile()) throw new InvalidOperationException(prog.CompileError);
				burst.Coordinator.SourceDefinitionChanged();
			}
			else
			{
				foreach (var resource in burst.Resources) burst.Edit($"output {resource.Id} basecapacity 50");
			}
			changeWatch.Stop();
			report.Results.Add(Measure(burst, "bulk-source-change", size, outputs, 0.0, policy,
				Math.Max(120, warmupSeconds)) with { SourceChangeMilliseconds = changeWatch.Elapsed.TotalMilliseconds });
			WriteReport(outputPath, report);
		}
		Console.WriteLine($"Environmental measurements written to {Path.GetFullPath(outputPath)} ({report.Results.Count} scenarios).");
	}

	private static HarnessMeasurement Measure(EnvironmentalMagicTestWorld world, string scenario, int size,
		int outputs, double activePercent, string policy, int seconds)
	{
		var before = world.Coordinator.Diagnostics;
		var formulasBefore = world.Profile.FormulaEvaluationCount;
		var savesBefore = world.SaveRequests;
		var callbacksBefore = world.HeartbeatPumps;
		var times = new double[seconds];
		var heartbeatAndFixtureMilliseconds = 0.0;
		var maximumReadyAge = 0.0;
		var maximumAuditAge = 0.0;
		var maximumActiveUpdateAgeBound = 0.0;
		var maximumVisits = 0;
		long visits = 0;
		long allocated = 0;
		var elapsed = Stopwatch.StartNew();
		for (var second = 0; second < seconds; second++)
		{
			var allocationStart = GC.GetAllocatedBytesForCurrentThread();
			var started = Stopwatch.GetTimestamp();
			world.Tick();
			heartbeatAndFixtureMilliseconds += Stopwatch.GetElapsedTime(started).TotalMilliseconds;
			allocated += GC.GetAllocatedBytesForCurrentThread() - allocationStart;
			var diagnostics = world.Coordinator.Diagnostics;
			times[second] = diagnostics.LastPumpMilliseconds;
			visits += diagnostics.LastCellVisits;
			maximumVisits = Math.Max(maximumVisits, diagnostics.LastCellVisits);
			maximumReadyAge = Math.Max(maximumReadyAge, diagnostics.OldestReadySeconds);
			maximumAuditAge = Math.Max(maximumAuditAge, diagnostics.OldestAuditSeconds);
			if (diagnostics.ActiveProduction + diagnostics.ActiveMaintenance > 0)
				maximumActiveUpdateAgeBound = Math.Max(maximumActiveUpdateAgeBound,
					world.Coordinator.Options.ActiveCadenceSeconds + diagnostics.OldestReadySeconds);
		}
		elapsed.Stop();
		var after = world.Coordinator.Diagnostics;
		Array.Sort(times);
		return new HarnessMeasurement(scenario, size, outputs, activePercent, policy, seconds,
			before.ActiveProduction, after.ActiveProduction, after.Dormant, after.Faulted,
			world.SecondSubscriptions, world.SchedulerEntries, world.EnvironmentalDelegateCount,
			world.HeartbeatPumps - callbacksBefore, visits, maximumVisits,
			after.TotalEvaluations - before.TotalEvaluations, world.Profile.FormulaEvaluationCount - formulasBefore,
			after.TotalInputProgExecutions - before.TotalInputProgExecutions,
			after.TotalWrites - before.TotalWrites, world.SaveRequests - savesBefore, allocated,
			times.Average(), times[(int)Math.Ceiling(seconds * 0.95) - 1], times[^1],
			elapsed.Elapsed.TotalSeconds, visits / elapsed.Elapsed.TotalSeconds,
			maximumReadyAge, maximumAuditAge, after.OldestReadySeconds, after.OldestAuditSeconds,
			after.BudgetLimitedPumps - before.BudgetLimitedPumps, after.DiscoveryRemaining, after.Dirty,
			world.Operations.Reads, world.Operations.Commits, world.Flushes, 0.0,
			heartbeatAndFixtureMilliseconds / seconds, maximumActiveUpdateAgeBound);
	}

	private static string? ReadOption(string[] args, string name)
	{
		var index = Array.IndexOf(args, name);
		if (index < 0) return null;
		if (index + 1 >= args.Length) throw new ArgumentException($"{name} requires a value.");
		return args[index + 1];
	}
	private static void WriteReport(string path, HarnessReport report)
	{
		var fullPath = Path.GetFullPath(path);
		Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
		File.WriteAllText(fullPath, JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
	}

	private sealed record HarnessReport(DateTimeOffset StartedUtc, string Runtime, string OperatingSystem,
		string Architecture, int LogicalProcessors, long AvailableMemoryBytes, string ProcessorIdentifier,
		EnvironmentalMagicOptions Options, int WarmupSeconds, int SteadySampleSeconds, string Method,
		List<HarnessMeasurement> Results);
	private sealed record HarnessMeasurement(string Scenario, int Cells, int Outputs, double RequestedActivePercent,
		string Policy, int SimulatedSeconds, int ActiveAtStart, int ActiveAtEnd, int DormantAtEnd, int FaultedAtEnd,
		int SecondHeartbeatSubscribers, int GlobalSchedulerEntries, int EnvironmentalPerHolderDelegates,
		long CentralCallbacks, long CellVisits, int MaximumVisitsPerPump, long InputSnapshots, long FormulaEvaluations,
		long InputProgExecutions, long BusinessWrites, long DirtySaveRequests, long AllocatedBytes,
		double AverageCallbackMilliseconds, double P95CallbackMilliseconds, double MaximumCallbackMilliseconds,
		double WallSeconds, double CellVisitsPerWallSecond, double MaximumReadyWorkAgeSeconds,
		double MaximumAuditAgeSeconds, double FinalReadyWorkAgeSeconds, double FinalAuditAgeSeconds,
		long BudgetLimitedPumps, int RemainingDiscoveryCells, int RemainingDirtyCells,
		int ExplicitOperationReads, int ExplicitOperationCommits, long SynchronousSaveFlushes,
		double SourceChangeMilliseconds, double AverageHeartbeatAndFixtureMilliseconds,
		double MaximumActiveUpdateAgeBoundSeconds);
}

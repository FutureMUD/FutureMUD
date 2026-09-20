namespace TestReporting;

internal sealed class Options
{
	public string Repository { get; set; } = "";
	public string Suite { get; set; } = "fast";
	public string Mode { get; set; } = "human";
	public string Configuration { get; set; } = "Debug";
	public string? Filter { get; set; }
	public string? ResultsRoot { get; set; }
	public int? TimeoutSeconds { get; set; }
	public bool FailOnSkipped { get; set; }
	public List<string> Projects { get; } = [];
}

internal sealed class Counts
{
	public int Executed { get; set; }
	public int Passed { get; set; }
	public int Failed { get; set; }
	public int Skipped { get; set; }
	public int Inconclusive { get; set; }
	public int Other { get; set; }
	public int Total => Passed + Failed + Skipped + Inconclusive + Other;
	public void Add(Counts other)
	{
		Executed += other.Executed;
		Passed += other.Passed;
		Failed += other.Failed;
		Skipped += other.Skipped;
		Inconclusive += other.Inconclusive;
		Other += other.Other;
	}
}

internal sealed class TestRecord
{
	public string Id { get; set; } = "";
	public string Project { get; set; } = "";
	public string Framework { get; set; } = "";
	public int Attempt { get; set; } = 1;
	public string TestId { get; set; } = "";
	public string ExecutionId { get; set; } = "";
	public string Name { get; set; } = "";
	public string? FullyQualifiedName { get; set; }
	public string NativeOutcome { get; set; } = "";
	public string Outcome { get; set; } = "";
	public string? Message { get; set; }
	public string? Stack { get; set; }
	public string Report { get; set; } = "";
}

internal sealed class Phase
{
	public string Name { get; set; } = "";
	public string[] Arguments { get; set; } = [];
	public string StartedUtc { get; set; } = "";
	public string? EndedUtc { get; set; }
	public int? ExitCode { get; set; }
	public string? TerminationReason { get; set; }
	public string Stdout { get; set; } = "";
	public string Stderr { get; set; } = "";
}

internal sealed class Invocation
{
	public string Project { get; set; } = "";
	public string Framework { get; set; } = "";
	public int Attempt { get; set; } = 1;
	public string Status { get; set; } = "NOT_RUN";
	public string? Reason { get; set; }
	public Phase? Build { get; set; }
	public Phase? Test { get; set; }
	public string? Report { get; set; }
	public string? Binary { get; set; }
	public Counts? Counts { get; set; }
	public Dictionary<string, string>? NativeCounters { get; set; }
}

internal sealed class Issue
{
	public string Kind { get; set; } = "";
	public string? Project { get; set; }
	public string? Detail { get; set; }
}

internal sealed class Summary
{
	public int SchemaVersion { get; set; } = 1;
	public string RunId { get; set; } = "";
	public string Repository { get; set; } = "";
	public string Head { get; set; } = "";
	public string StartedUtc { get; set; } = "";
	public string? EndedUtc { get; set; }
	public double? DurationSeconds { get; set; }
	public string SourceStart { get; set; } = "";
	public string? SourceEnd { get; set; }
	public bool? SourceStable { get; set; }
	public string Sdk { get; set; } = "";
	public string Runner { get; set; } = "VSTest/TRX";
	public string Suite { get; set; } = "";
	public string Configuration { get; set; } = "";
	public string? Filter { get; set; }
	public bool FailOnSkipped { get; set; }
	public bool ExecutionComplete { get; set; }
	public bool CountsComplete { get; set; }
	public string Status { get; set; } = "INCONCLUSIVE";
	public Counts? Counts { get; set; }
	public List<string> Projects { get; set; } = [];
	public List<Invocation> Invocations { get; set; } = [];
	public List<Issue> Issues { get; set; } = [];
	public Dictionary<string, string> Artifacts { get; set; } = [];
	public string[] BuildSwitches { get; set; } = ["-m:1", "-p:RestoreBuildInParallel=false", "-p:NuGetAudit=false", "-p:NoWarn=NU1902%3BNU1510"];
}

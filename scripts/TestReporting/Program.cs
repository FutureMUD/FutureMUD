using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace TestReporting;

internal static class Program
{
	private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
	private static readonly Regex Control = new(@"\x1B(?:\[[0-?]*[ -/]*[@-~]|\][^\a]*(?:\a|\x1B\\))|[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", RegexOptions.Compiled);
	private static readonly Regex Secrets = new(@"(?i)(password|token|secret|api[_-]?key)\s*[:=]\s*\S+", RegexOptions.Compiled);
	private static readonly Regex Prerequisite = new(@"(?i)(NETSDK\d+|NU1301|NU1101|SDK.*not found|unable to load the service index|No such file or directory|The system cannot find the file specified)", RegexOptions.Compiled);
	private static readonly Regex CompileError = new(@"(?i)(:\s*error\s+(CS|FS|BC)\d+|\berror\s+MSB\d+)", RegexOptions.Compiled);

	private static async Task<int> Main(string[] args)
	{
		try
		{
			if (args.Length > 0 && args[0] == "inspect") return Inspect(args[1..]);
			if (args.Contains("--help"))
			{
				Console.WriteLine("Options: --repo PATH --suite fast|core|climate --project PATH (repeatable, fast only) --output-mode human|compact|json --results-root PATH --configuration NAME --filter EXPR --timeout-seconds N --fail-on-skipped. Inspect: inspect --run DIRECTORY [--failure ID | --invocation N | --page N]");
				return 0;
			}
			var options = Parse(args);
			return await Run(options);
		}
		catch (ArgumentException ex)
		{
			var message = Clean(ex.Message, 350);
			if (args.Zip(args.Skip(1)).Any(x => x.First == "--output-mode" && x.Second.Equals("json", StringComparison.OrdinalIgnoreCase)))
				Console.WriteLine(JsonSerializer.Serialize(new { schema_version = 1, status = "BLOCKED", issues = new[] { new { kind = "INVALID_INVOCATION", detail = message } } }, JsonOptions));
			else Console.Error.WriteLine("BLOCKED INVALID_INVOCATION: " + message);
			return 2;
		}
		catch (Exception ex)
		{
			var message = Clean(ex.Message, 400);
			if (args.Zip(args.Skip(1)).Any(x => x.First == "--output-mode" && x.Second.Equals("json", StringComparison.OrdinalIgnoreCase)))
				Console.WriteLine(JsonSerializer.Serialize(new { schema_version = 1, status = "INCONCLUSIVE", issues = new[] { new { kind = "REPORTING_ERROR", detail = message } } }, JsonOptions));
			else Console.Error.WriteLine("Test reporting failed: " + message);
			return 3;
		}
	}

	private static Options Parse(string[] args)
	{
		var options = new Options();
		for (var i = 0; i < args.Length; i++)
		{
			var key = args[i];
			if (key == "--fail-on-skipped") { options.FailOnSkipped = true; continue; }
			if (i + 1 >= args.Length) throw new ArgumentException("Missing value for " + key);
			var value = args[++i];
			switch (key)
			{
				case "--repo": options.Repository = value; break;
				case "--suite": options.Suite = value; break;
				case "--output-mode": options.Mode = value.ToLowerInvariant(); break;
				case "--results-root": options.ResultsRoot = value; break;
				case "--configuration": options.Configuration = value; break;
				case "--filter": options.Filter = value; break;
				case "--timeout-seconds": options.TimeoutSeconds = int.TryParse(value, out var seconds) && seconds > 0 ? seconds : throw new ArgumentException("Timeout must be a positive integer."); break;
				case "--project": options.Projects.Add(value); break;
				default: throw new ArgumentException("Unknown option " + key);
			}
		}
		if (string.IsNullOrWhiteSpace(options.Repository) || !Directory.Exists(options.Repository)) throw new ArgumentException("Valid --repo is required.");
		if (options.Suite is not ("fast" or "core" or "climate")) throw new ArgumentException("Unknown suite.");
		if (options.Mode is not ("human" or "compact" or "json")) throw new ArgumentException("Unknown output mode.");
		if (string.IsNullOrWhiteSpace(options.Configuration) || options.Configuration.StartsWith('-')) throw new ArgumentException("Invalid configuration.");
		if (options.Suite != "fast" && options.Projects.Count > 0) throw new ArgumentException("Explicit projects are supported by the broad entry point only.");
		if (options.Filter is not null && string.IsNullOrWhiteSpace(options.Filter)) throw new ArgumentException("Filter cannot be empty.");
		if (options.Mode != "human") options.TimeoutSeconds ??= 1800;
		options.Repository = Path.GetFullPath(options.Repository);
		return options;
	}

	private static async Task<int> Run(Options options)
	{
		var repo = options.Repository;
		var selection = options.Projects.Count > 0 ? options.Projects : options.Suite switch
		{
			"core" => ["MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj"],
			"climate" => ["MudSharpCore Climate Tests/MudSharpCore Climate Tests.csproj"],
			_ => File.ReadAllLines(Path.Combine(repo, "scripts", "unit-test-projects.txt"))
				.Where(x => !string.IsNullOrWhiteSpace(x) && !x.TrimStart().StartsWith('#')).ToList()
		};
		var projects = selection.Select(x => ValidateProject(repo, x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
		if (projects.Count == 0 || options.Filter is not null && projects.Count != 1) throw new ArgumentException("Selection must be nonempty, and filters require exactly one project.");
		var root = Path.GetFullPath(options.ResultsRoot ?? Path.Combine(repo, ".artifacts", "test-runs"), repo);
		Directory.CreateDirectory(root);
		var runId = DateTimeOffset.UtcNow.ToString("yyyyMMddTHHmmssZ") + "-" + Guid.NewGuid().ToString("N")[..12];
		var dir = Path.Combine(root, runId);
		Directory.CreateDirectory(dir);
		var started = DateTimeOffset.UtcNow;
		var summary = new Summary
		{
			RunId = runId, Repository = repo, StartedUtc = started.ToString("O"), Suite = options.Suite,
			Configuration = options.Configuration, Filter = options.Filter, FailOnSkipped = options.FailOnSkipped,
			Projects = projects, Artifacts = new()
			{
				["run"] = Path.Combine(dir, "run.json"), ["summary"] = Path.Combine(dir, "summary.json"),
				["failures"] = Path.Combine(dir, "failures.json"), ["results"] = Path.Combine(dir, "test-results.json")
			}
		};
		var bootstrapLog = Environment.GetEnvironmentVariable("FUTUREMUD_REPORTER_BOOTSTRAP_LOG");
		if (!string.IsNullOrWhiteSpace(bootstrapLog)) summary.Artifacts["bootstrap"] = bootstrapLog;
		summary.Runner = "VSTest/TRX (" + string.Join(", ", projects.Select(x => RunnerPackage(Path.Combine(repo, x))).Distinct(StringComparer.Ordinal)) + ")";
		foreach (var project in projects)
		{
			foreach (var framework in Frameworks(Path.Combine(repo, project))) summary.Invocations.Add(new Invocation { Project = project, Framework = framework });
		}
		WriteJson(summary.Artifacts["run"], new { summary.RunId, summary.Repository, summary.StartedUtc, summary.Suite, summary.Projects, summary.Configuration, summary.Filter, summary.FailOnSkipped, options.TimeoutSeconds, summary.BuildSwitches });
		var records = new List<TestRecord>();
		var lockPath = Path.Combine(repo, ".artifacts", "test-runs", ".worktree.lock");
		var ownerPath = lockPath + ".owner";
		Directory.CreateDirectory(Path.GetDirectoryName(lockPath)!);
		FileStream? ownership = null;
		try
		{
			// FileShare.Read permits shared flock ownership on Unix. Exclude every
			// other opener and keep the diagnostic marker in a separate readable file.
			ownership = new FileStream(lockPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
			File.WriteAllText(ownerPath, runId);
		}
		catch (IOException)
		{
			ownership?.Dispose();
			ownership = null;
			string active;
			try
			{
				using var reader = new StreamReader(new FileStream(ownerPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
				active = reader.ReadToEnd();
			}
			catch { active = "unknown"; }
			summary.Issues.Add(new Issue { Kind = "RESOURCE_BUSY", Detail = "Worktree is owned by run " + Clean(active, 80) });
		}
		using (ownership)
		using (var deadline = options.TimeoutSeconds is { } t ? new CancellationTokenSource(TimeSpan.FromSeconds(t)) : new CancellationTokenSource())
		{
			if (ownership is not null)
			{
				try
				{
					summary.Head = (await Git(repo, "rev-parse", "HEAD")).Trim();
						summary.SourceStart = await Fingerprint(repo, projects, root);
					WriteJson(Path.Combine(dir, "source-start.json"), new { summary.Head, fingerprint = summary.SourceStart, utc = DateTimeOffset.UtcNow });
					var dotnet = Environment.GetEnvironmentVariable("FUTUREMUD_TEST_DOTNET") ?? "dotnet";
						summary.Sdk = (await RunVersion(dotnet, repo, dir, deadline.Token)).Trim();
						WriteJson(summary.Artifacts["run"], new { summary.RunId, summary.Repository, summary.Head, summary.SourceStart, summary.StartedUtc, summary.Sdk, summary.Runner, summary.Suite, summary.Projects, summary.Configuration, summary.Filter, summary.FailOnSkipped, options.TimeoutSeconds, summary.BuildSwitches });
					var buildFailed = false;
					foreach (var project in projects)
					{
						if (deadline.IsCancellationRequested) break;
						var invocations = summary.Invocations.Where(x => x.Project == project).ToList();
						var logDir = Path.Combine(dir, "invocations", SafeKey(project), "build");
						var args = new List<string> { "build", project, "-c", options.Configuration, "-m:1", "-p:RestoreBuildInParallel=false", "-p:NuGetAudit=false", "-p:NoWarn=NU1902%3BNU1510" };
						if (options.Suite != "fast") args.Add("--no-restore");
						var build = await Execute(dotnet, args.ToArray(), repo, logDir, "build", deadline.Token);
						foreach (var invocation in invocations)
						{
							invocation.Build = build;
							if (build.ExitCode == 0)
							{
								var candidate = ExpectedBinary(repo, invocation, options.Configuration);
								if (File.Exists(candidate)) invocation.Binary = candidate;
								else summary.Issues.Add(new Issue { Kind = "BINARY_UNAVAILABLE", Project = project, Detail = candidate });
							}
						}
						if (options.Mode == "human") PrintLogs(build);
						if (build.ExitCode != 0)
						{
							var kind = build.TerminationReason?.Split(';')[0] ?? ClassifyBuild(build);
							foreach (var invocation in invocations) { invocation.Status = "NOT_RUN"; invocation.Reason = kind; }
							summary.Issues.Add(new Issue { Kind = kind, Project = project, Detail = FirstDiagnostic(build) });
							buildFailed = true;
							break;
						}
					}
					if (!buildFailed && !deadline.IsCancellationRequested)
					{
						var tasks = summary.Invocations.Select(x => TestInvocation(dotnet, repo, dir, options, x, records, summary, deadline.Token));
						await Task.WhenAll(tasks);
					}
					if (deadline.IsCancellationRequested) summary.Issues.Add(new Issue { Kind = "TIMEOUT" });
				}
					catch (Exception ex)
					{
						summary.Issues.Add(new Issue { Kind = ex is PrerequisiteException ? "PREREQUISITE_UNAVAILABLE" : ex is OperationCanceledException ? "TIMEOUT" : "REPORTING_ERROR", Detail = Clean(ex.Message, 300) });
				}
				try
				{
						summary.SourceEnd = await Fingerprint(repo, projects, root);
					summary.SourceStable = summary.SourceStart == summary.SourceEnd;
					WriteJson(Path.Combine(dir, "source-end.json"), new { fingerprint = summary.SourceEnd, utc = DateTimeOffset.UtcNow });
					if (summary.SourceStable != true) summary.Issues.Add(new Issue { Kind = "SOURCE_CHANGED" });
				}
				catch (Exception ex) { summary.Issues.Add(new Issue { Kind = "SOURCE_CHANGED", Detail = Clean(ex.Message, 300) }); }
			}
		}
		summary.EndedUtc = DateTimeOffset.UtcNow.ToString("O");
		summary.DurationSeconds = (DateTimeOffset.UtcNow - started).TotalSeconds;
		summary.ExecutionComplete = summary.Invocations.All(x => x.Test is { ExitCode: not null } && x.Report is not null);
		summary.CountsComplete = summary.Invocations.All(x => x.Counts is not null && x.Reason is null);
		if (summary.Invocations.Any(x => x.Counts is not null))
		{
			summary.Counts = new Counts();
			foreach (var invocation in summary.Invocations.Where(x => x.Counts is not null)) summary.Counts.Add(invocation.Counts!);
		}
		if (records.Any(x => x.Outcome == "FAILED")) summary.Issues.Add(new Issue { Kind = "TEST_FAILED", Detail = records.Count(x => x.Outcome == "FAILED").ToString() + " failing test records" });
		if (records.Count > 0 && records.All(x => x.Outcome == "SKIPPED")) summary.Issues.Add(new Issue { Kind = "NO_TESTS", Detail = "All selected tests were skipped." });
		if (options.FailOnSkipped && records.Any(x => x.Outcome == "SKIPPED")) summary.Issues.Add(new Issue { Kind = "SKIPPED_STRICT" });
		summary.Status = Status(summary);
		try
		{
			WriteJson(summary.Artifacts["results"], records);
			WriteJson(summary.Artifacts["failures"], records.Where(x => x.Outcome == "FAILED").ToList());
			WriteJson(summary.Artifacts["summary"], summary);
			var receipt = Receipt(summary, records);
			File.WriteAllText(Path.Combine(dir, "summary.txt"), receipt + Environment.NewLine);
			Console.WriteLine(options.Mode == "json" ? JsonReceipt(summary, records) : receipt);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine("Unable to write test receipt: " + Clean(ex.Message, 300));
			return 3;
		}
		return options.Mode == "human" ? summary.Status == "PASS" ? 0 : 1 : summary.Status switch { "PASS" => 0, "FAIL" => 1, "BLOCKED" => 2, _ => 3 };
	}

	private static async Task TestInvocation(string dotnet, string repo, string dir, Options options, Invocation invocation, List<TestRecord> records, Summary summary, CancellationToken cancellation)
	{
		var folder = Path.Combine(dir, "invocations", SafeKey(invocation.Project), SafeKey(invocation.Framework), "1");
		Directory.CreateDirectory(folder);
		var args = new List<string> { "test", invocation.Project, "-c", options.Configuration, "--no-build", "--no-restore", "-m:1", "-f", invocation.Framework, "-p:NoWarn=NU1902%3BNU1510", "--logger", "trx;LogFileName=results.trx", "--results-directory", folder };
		if (options.Filter is not null) { args.Add("--filter"); args.Add(options.Filter); }
		var phase = await Execute(dotnet, args.ToArray(), repo, folder, "test", cancellation);
		invocation.Test = phase;
		if (options.Mode == "human") PrintLogs(phase);
		if (phase.ExitCode is null && phase.TerminationReason is not null)
		{
			invocation.Reason = phase.TerminationReason.Split(';')[0];
			lock (summary.Issues) summary.Issues.Add(new Issue { Kind = invocation.Reason, Project = invocation.Project, Detail = Clean(phase.TerminationReason, 200) });
			return;
		}
		var start = DateTimeOffset.Parse(phase.StartedUtc);
		var report = Path.Combine(folder, "results.trx");
		var parsed = Trx.Parse(report, invocation.Project, invocation.Framework, start, invocation.Binary is null ? null : Path.GetFileName(invocation.Binary));
		invocation.Report = File.Exists(report) ? report : null;
		invocation.NativeCounters = parsed.NativeCounters;
		invocation.Counts = parsed.Problem is null ? parsed.Counts : null;
		lock (records) records.AddRange(parsed.Records);
		if (parsed.Problem is not null)
		{
			invocation.Reason = parsed.Problem.Split(':')[0];
			lock (summary.Issues) summary.Issues.Add(new Issue { Kind = invocation.Reason, Project = invocation.Project, Detail = Clean(parsed.Problem, 200) });
		}
		if (phase.TerminationReason is not null)
		{
			invocation.Reason = phase.TerminationReason.Split(';')[0];
			lock (summary.Issues) summary.Issues.Add(new Issue { Kind = invocation.Reason, Project = invocation.Project, Detail = Clean(phase.TerminationReason, 200) });
		}
		else if (phase.ExitCode != 0 && parsed.Counts.Failed == 0)
		{
			var kind = ClassifyBuild(phase) is "RESTORE_FAILED" or "PREREQUISITE_UNAVAILABLE" ? ClassifyBuild(phase) : "HOST_CRASH";
			invocation.Reason = kind;
			lock (summary.Issues) summary.Issues.Add(new Issue { Kind = kind, Project = invocation.Project, Detail = FirstDiagnostic(phase) });
		}
		else if (parsed.Problem is null && parsed.Counts.Executed == 0)
		{
			invocation.Reason = "NO_TESTS";
			lock (summary.Issues) summary.Issues.Add(new Issue { Kind = "NO_TESTS", Project = invocation.Project });
		}
		else if (parsed.Problem is null && parsed.Counts.Inconclusive + parsed.Counts.Other > 0)
		{
			invocation.Reason = "NATIVE_INCONCLUSIVE";
			lock (summary.Issues) summary.Issues.Add(new Issue { Kind = "NATIVE_INCONCLUSIVE", Project = invocation.Project });
		}
		invocation.Status = parsed.Counts.Failed > 0 ? "FAIL" : invocation.Reason is not null ? "INCONCLUSIVE" : "PASS";
	}

	private static string Status(Summary summary)
	{
		if (summary.Issues.Any(x => x.Kind is "TEST_FAILED" or "BUILD_FAILED")) return "FAIL";
		if (summary.Issues.Count == 0 && summary.ExecutionComplete && summary.CountsComplete && summary.SourceStable == true && summary.Counts?.Executed > 0) return "PASS";
		var started = summary.Invocations.Any(x => x.Test is not null || x.Build is { ExitCode: not null });
		if (!started && summary.Issues.Count > 0 && summary.Issues.All(x => x.Kind is "RESOURCE_BUSY" or "PREREQUISITE_UNAVAILABLE" or "RESTORE_FAILED")) return "BLOCKED";
		if (summary.Issues.Count > 0 && summary.Issues.All(x => x.Kind is "PREREQUISITE_UNAVAILABLE" or "RESTORE_FAILED")) return "BLOCKED";
		return "INCONCLUSIVE";
	}

	private static string ValidateProject(string repo, string requested)
	{
		var full = Path.GetFullPath(requested, repo);
		if (!full.StartsWith(repo + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) || !full.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase) || !File.Exists(full)) throw new ArgumentException("Project must be an existing repository-owned .csproj: " + requested);
		var current = repo;
		foreach (var part in Path.GetRelativePath(repo, full).Split(Path.DirectorySeparatorChar))
		{
			current = Path.Combine(current, part);
			if ((File.GetAttributes(current) & FileAttributes.ReparsePoint) != 0) throw new ArgumentException("Project path must not traverse a symbolic link: " + requested);
		}
		var xml = XDocument.Load(full);
		if (!xml.Descendants().Any(x => x.Name.LocalName == "PackageReference" && (string?)x.Attribute("Include") == "Microsoft.NET.Test.Sdk")) throw new ArgumentException("Project is not a supported VSTest test project: " + requested);
		return Path.GetRelativePath(repo, full).Replace('\\', '/');
	}

	private static List<string> Frameworks(string path)
	{
		var xml = XDocument.Load(path);
		var value = xml.Descendants().FirstOrDefault(x => x.Name.LocalName is "TargetFrameworks" or "TargetFramework")?.Value;
		if (string.IsNullOrWhiteSpace(value) || value.Contains("$(")) throw new ArgumentException("Framework could not be determined for " + path);
		return value.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
	}
	private static string RunnerPackage(string projectPath)
	{
		var xml = XDocument.Load(projectPath);
		var reference = xml.Descendants().First(x => x.Name.LocalName == "PackageReference" && (string?)x.Attribute("Include") == "Microsoft.NET.Test.Sdk");
		return "Microsoft.NET.Test.Sdk " + ((string?)reference.Attribute("Version") ?? "version unknown");
	}
	private static string ExpectedBinary(string repo, Invocation invocation, string configuration)
	{
		var projectPath = Path.Combine(repo, invocation.Project);
		var xml = XDocument.Load(projectPath);
		var assembly = xml.Descendants().FirstOrDefault(x => x.Name.LocalName == "AssemblyName")?.Value ?? Path.GetFileNameWithoutExtension(projectPath);
		return Path.Combine(Path.GetDirectoryName(projectPath)!, "bin", configuration, invocation.Framework, assembly + ".dll");
	}

	private static string SafeKey(string value) => Regex.Replace(Path.GetFileNameWithoutExtension(value), "[^A-Za-z0-9_.-]", "_") + "-" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)))[..8].ToLowerInvariant();
	private static void WriteJson(string path, object value) => File.WriteAllText(path, JsonSerializer.Serialize(value, JsonOptions));
	private static string Clean(string? value, int limit)
	{
		var clean = Secrets.Replace(Control.Replace(value ?? "", " "), "$1=[redacted]");
		return clean.Length <= limit ? clean : clean[..limit] + "…";
	}

	private static object ReceiptObject(Summary summary, List<TestRecord> records)
	{
		var failed = records.Where(x => x.Outcome == "FAILED").Take(3).Select(x => new { x.Id, x.Project, x.Name, message = Clean(x.Message, 350) }).ToList();
		return new
		{
			schema_version = 1, run_id = summary.RunId, status = summary.Status, head = summary.Head,
			source_start = summary.SourceStart, source_end = summary.SourceEnd, source_stable = summary.SourceStable,
			suite = summary.Suite, projects = summary.Projects, filter = summary.Filter, configuration = summary.Configuration,
			execution_complete = summary.ExecutionComplete, counts_complete = summary.CountsComplete, counts = summary.Counts,
			skipped_verification_gap = records.Any(x => x.Outcome == "SKIPPED"),
			issues = summary.Issues.Take(5).Select(x => new { x.Kind, x.Project, detail = Clean(x.Detail, 220) }),
			issues_omitted = Math.Max(0, summary.Issues.Count - 5), failures = failed,
			failure_details_omitted = Math.Max(0, records.Count(x => x.Outcome == "FAILED") - failed.Count),
			artifacts = summary.Artifacts
		};
	}
	internal static string JsonReceipt(Summary summary, List<TestRecord> records)
	{
		var full = JsonSerializer.Serialize(ReceiptObject(summary, records), JsonOptions);
		if (Encoding.UTF8.GetByteCount(full) <= 7900) return full;
		var fallback = new
		{
			schema_version = 1, summary.RunId, summary.Status, summary.Head, summary.SourceStable,
			summary.Suite, project_count = summary.Projects.Count,
			projects = summary.Projects.Take(3).Select(x => Clean(x, 100)),
			projects_omitted = Math.Max(0, summary.Projects.Count - 3),
			summary.ExecutionComplete, summary.CountsComplete, summary.Counts,
			issues = summary.Issues.Take(3).Select(x => new { x.Kind, detail = Clean(x.Detail, 120) }),
			issues_omitted = Math.Max(0, summary.Issues.Count - 3),
			failure_count = records.Count(x => x.Outcome == "FAILED"),
			failure_details_omitted = records.Count(x => x.Outcome == "FAILED"),
			artifacts = new
			{
				summary = ShortArtifact(summary, "summary"), failures = ShortArtifact(summary, "failures"),
				path_omitted = summary.Artifacts.Values.Any(x => x.Length > 1000)
			}
		};
		return JsonSerializer.Serialize(fallback, JsonOptions);
	}

	private static string? ShortArtifact(Summary summary, string key)
	{
		if (!summary.Artifacts.TryGetValue(key, out var path)) return null;
		var relative = Path.GetRelativePath(summary.Repository, path);
		var best = relative.Length < path.Length ? relative : path;
		return best.Length <= 1000 ? best : null;
	}

	internal static string Receipt(Summary summary, List<TestRecord> records)
	{
		var counts = summary.Counts is null ? "counts incomplete" : $"{summary.Counts.Passed} passed, {summary.Counts.Failed} failed, {summary.Counts.Skipped} skipped";
		var line = $"{summary.Status} — {summary.Suite}; {summary.Projects.Count} project(s); {counts}; source stable: {summary.SourceStable?.ToString() ?? "unknown"}; run {summary.RunId}";
		var issues = string.Join("; ", summary.Issues.Take(4).Select(x => x.Kind + (x.Project is null ? "" : " " + x.Project) + (x.Detail is null ? "" : ": " + Clean(x.Detail, 180))));
		var failure = records.FirstOrDefault(x => x.Outcome == "FAILED");
		var full = line + (issues.Length == 0 ? "" : "\nIssues: " + issues) + (failure is null ? "" : "\nFailure: " + Clean(failure.Name + " " + failure.Message, 400)) + "\nSummary: " + summary.Artifacts["summary"] + "\nFailures: " + summary.Artifacts["failures"];
		if (Encoding.UTF8.GetByteCount(full) <= 7900) return full;
		return $"{summary.Status} — {summary.Suite}; {summary.Projects.Count} projects; {counts}; run {summary.RunId}\nIssues: {Clean(issues, 500)}\nSummary: {ShortArtifact(summary, "summary") ?? "path omitted; use supplied results root and run ID"}\nFailure details omitted: {records.Count(x => x.Outcome == "FAILED")}";
	}

	private static string ClassifyBuild(Phase phase)
	{
		var sample = ReadPrefix(phase.Stdout, 100_000) + ReadPrefix(phase.Stderr, 100_000);
		if (Prerequisite.IsMatch(sample)) return sample.Contains("NU1301", StringComparison.OrdinalIgnoreCase) || sample.Contains("NU1101", StringComparison.OrdinalIgnoreCase) ? "RESTORE_FAILED" : "PREREQUISITE_UNAVAILABLE";
		return CompileError.IsMatch(sample) ? "BUILD_FAILED" : "BUILD_INCONCLUSIVE";
	}
	private static string FirstDiagnostic(Phase phase)
	{
		var lines = (ReadPrefix(phase.Stderr, 12000) + "\n" + ReadPrefix(phase.Stdout, 12000)).Split('\n');
		return Clean(lines.FirstOrDefault(x => x.Contains("error", StringComparison.OrdinalIgnoreCase) || x.Contains("failed", StringComparison.OrdinalIgnoreCase)) ?? lines.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)), 250);
	}
	private static string ReadPrefix(string path, int maxChars)
	{
		if (!File.Exists(path)) return "";
		using var reader = new StreamReader(path);
		var buffer = new char[maxChars];
		return new string(buffer, 0, reader.Read(buffer, 0, maxChars));
	}
	private static void PrintLogs(Phase phase)
	{
		if (File.Exists(phase.Stdout)) CopyLog(phase.Stdout, Console.Out);
		if (File.Exists(phase.Stderr)) CopyLog(phase.Stderr, Console.Error);
	}
	private static void CopyLog(string path, TextWriter destination)
	{
		using var source = File.OpenText(path);
		var buffer = new char[8192];
		int count;
		while ((count = source.Read(buffer, 0, buffer.Length)) > 0) destination.Write(buffer, 0, count);
	}

	private static async Task<Phase> Execute(string file, string[] arguments, string repo, string folder, string name, CancellationToken cancellation)
	{
		Directory.CreateDirectory(folder);
		var phase = new Phase { Name = name, Arguments = arguments, StartedUtc = DateTimeOffset.UtcNow.ToString("O"), Stdout = Path.Combine(folder, name + ".stdout.log"), Stderr = Path.Combine(folder, name + ".stderr.log") };
		var start = new ProcessStartInfo(file) { WorkingDirectory = repo, UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
		foreach (var argument in arguments) start.ArgumentList.Add(argument);
		using var process = new Process { StartInfo = start };
		var started = false;
		try
		{
			process.Start();
			started = true;
			await using var stdout = File.Create(phase.Stdout);
			await using var stderr = File.Create(phase.Stderr);
			var outTask = CopyProcessLog(process.StandardOutput.BaseStream, stdout);
			var errTask = CopyProcessLog(process.StandardError.BaseStream, stderr);
			try { await process.WaitForExitAsync(cancellation); }
			catch (OperationCanceledException)
			{
				phase.TerminationReason = "TIMEOUT";
				try { process.Kill(entireProcessTree: true); } catch (Exception ex) { phase.TerminationReason = "TIMEOUT; cleanup failed: " + Clean(ex.Message, 120); }
				try { await process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5)); }
				catch (TimeoutException)
				{
					phase.TerminationReason += "; owned process did not exit after cleanup";
					process.StandardOutput.BaseStream.Dispose();
					process.StandardError.BaseStream.Dispose();
				}
			}
			try { await Task.WhenAll(outTask, errTask).WaitAsync(TimeSpan.FromSeconds(5)); }
			catch (Exception ex) when (ex is TimeoutException or IOException or ObjectDisposedException)
			{
				phase.TerminationReason = (phase.TerminationReason ?? "REPORTING_ERROR") + "; log drain incomplete: " + Clean(ex.Message, 100);
			}
			phase.ExitCode = process.HasExited ? process.ExitCode : null;
		}
		catch (Exception ex)
		{
			try { if (!process.HasExited) process.Kill(entireProcessTree: true); } catch { /* Process may not have started. */ }
			phase.TerminationReason = started ? "REPORTING_ERROR" : "PREREQUISITE_UNAVAILABLE";
			await File.WriteAllTextAsync(phase.Stderr, ex.Message);
		}
		phase.EndedUtc = DateTimeOffset.UtcNow.ToString("O");
		return phase;
	}

	private static Task CopyProcessLog(Stream source, Stream destination)
	{
		// Windows Process pipes are synchronous handles: CopyToAsync blocks a
		// pool worker per pipe. Dedicated readers keep parallel hosts from starving
		// process-exit and deadline continuations on small CI machines.
		return OperatingSystem.IsWindows()
			? Task.Factory.StartNew(() => source.CopyTo(destination), CancellationToken.None,
				TaskCreationOptions.LongRunning, TaskScheduler.Default)
			: source.CopyToAsync(destination);
	}

	private static async Task<string> RunVersion(string dotnet, string repo, string runDirectory, CancellationToken cancellation)
	{
		var phase = await Execute(dotnet, ["--version"], repo, Path.Combine(runDirectory, "sdk"), "sdk", cancellation);
		if (phase.TerminationReason?.StartsWith("TIMEOUT", StringComparison.Ordinal) == true) throw new OperationCanceledException("SDK check exceeded the deadline.");
		if (phase.TerminationReason?.StartsWith("REPORTING_ERROR", StringComparison.Ordinal) == true) throw new IOException("SDK evidence capture failed: " + phase.TerminationReason);
		if (phase.ExitCode != 0) throw new PrerequisiteException("dotnet SDK unavailable: " + FirstDiagnostic(phase));
		return ReadPrefix(phase.Stdout, 100).Trim();
	}

	private static async Task<string> Git(string repo, params string[] arguments)
	{
		var start = new ProcessStartInfo("git") { WorkingDirectory = repo, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
		foreach (var argument in arguments) start.ArgumentList.Add(argument);
		using var process = Process.Start(start)!;
		var output = await process.StandardOutput.ReadToEndAsync();
		var error = await process.StandardError.ReadToEndAsync();
		await process.WaitForExitAsync();
		if (process.ExitCode != 0) throw new InvalidOperationException("git failed: " + Clean(error, 200));
		return output;
	}

	private static async Task<string> Fingerprint(string repo, IEnumerable<string> projects, string resultsRoot)
	{
		using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
		void Add(string value) => hash.AppendData(Encoding.UTF8.GetBytes(value));
		Add(await Git(repo, "rev-parse", "HEAD"));
		Add(await Git(repo, "diff", "--binary", "HEAD", "--"));
		foreach (var relative in (await Git(repo, "ls-files", "--others", "--exclude-standard", "-z")).Split('\0', StringSplitOptions.RemoveEmptyEntries).Order(StringComparer.Ordinal))
		{
			if (relative.StartsWith(".artifacts/", StringComparison.OrdinalIgnoreCase)) continue;
			var full = Path.GetFullPath(relative, repo);
			if (full.StartsWith(resultsRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) continue;
			if (!File.Exists(full)) continue;
			Add(relative);
			await using var file = File.OpenRead(full);
			var digest = await SHA256.HashDataAsync(file);
			hash.AppendData(digest);
		}
		foreach (var project in projects.Order(StringComparer.Ordinal))
		{
			var projectDirectory = Path.GetDirectoryName(Path.Combine(repo, project))!;
			foreach (var path in Directory.EnumerateFiles(projectDirectory, "*", SearchOption.AllDirectories)
				.Where(path => !Path.GetRelativePath(projectDirectory, path).Split(Path.DirectorySeparatorChar).Any(part => part is "bin" or "obj" or "TestResults" or "results"))
				.Where(path => Path.GetExtension(path).ToLowerInvariant() is ".cs" or ".csproj" or ".json" or ".runsettings" or ".props" or ".targets" or ".config")
				.Order(StringComparer.Ordinal))
			{
				Add(Path.GetRelativePath(repo, path));
				await using var input = File.OpenRead(path);
				hash.AppendData(await SHA256.HashDataAsync(input));
			}
		}
		return Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
	}

	private static int Inspect(string[] args)
	{
		string? run = null, failure = null;
		int? invocation = null;
		var page = 0;
		for (var i = 0; i < args.Length; i++)
		{
			if (i + 1 >= args.Length) throw new ArgumentException("Missing inspect value.");
			var value = args[++i];
			switch (args[i - 1])
			{
				case "--run": run = value; break;
				case "--failure": failure = value; break;
				case "--invocation": invocation = int.Parse(value); break;
				case "--page": page = int.Parse(value); break;
				default: throw new ArgumentException("Unknown inspect option.");
			}
		}
		if (run is null || page < 0 || !File.Exists(Path.Combine(run, "summary.json"))) throw new ArgumentException("Existing run directory is required.");
		var summary = JsonSerializer.Deserialize<Summary>(File.ReadAllText(Path.Combine(run, "summary.json")), JsonOptions)!;
		var failures = JsonSerializer.Deserialize<List<TestRecord>>(File.ReadAllText(Path.Combine(run, "failures.json")), JsonOptions)!;
		object result;
		if (failure is not null) result = failures.Where(x => x.Id == failure).Select(x => new { x.Id, x.Project, x.Framework, x.Name, message = Clean(x.Message, 1800), stack = Clean(x.Stack, 1800), x.Report }).ToList();
		else if (invocation is not null) result = invocation >= 0 && invocation < summary.Invocations.Count ? summary.Invocations[invocation.Value] : throw new ArgumentException("Invocation index out of range.");
		else result = new { summary.RunId, summary.Status, total = failures.Count, page, items = failures.Skip(page * 5).Take(5).Select(x => new { x.Id, x.Project, x.Framework, x.Name, message = Clean(x.Message, 250) }) };
		var json = JsonSerializer.Serialize(result, JsonOptions);
		if (Encoding.UTF8.GetByteCount(json) > 7500) json = JsonSerializer.Serialize(new { summary.RunId, summary.Status, detail_omitted = true, failures = Path.Combine(run, "failures.json") }, JsonOptions);
		Console.WriteLine(json);
		return 0;
	}
}

internal sealed class PrerequisiteException(string message) : Exception(message);

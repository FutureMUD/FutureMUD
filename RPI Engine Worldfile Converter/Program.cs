#nullable enable

using System.Text.Json;
using System.Text.Json.Serialization;
using MudSharp.Database;

namespace RPI_Engine_Worldfile_Converter;

public sealed record ConverterAnalysisSummary(
	int TotalBlockCount,
	int ParsedItemCount,
	int FailureCount,
	int ParseWarningCount,
	string BaselineStatus,
	IReadOnlyDictionary<string, int> PerTypeCounts,
	IReadOnlyDictionary<string, int> PerStatusCounts,
	IReadOnlyDictionary<string, int> WarningCodeCounts,
	IReadOnlyDictionary<string, int> MissingDependencyCounts);

public sealed record ConverterExportItem(RpiItemRecord Source, ConvertedItemDefinition Converted);

public sealed record ConverterExportReport(
	DateTime GeneratedUtc,
	string SourceDirectory,
	ConverterAnalysisSummary Analysis,
	IReadOnlyList<RpiItemBlockFailure> Failures,
	IReadOnlyList<FutureMudValidationIssue> ValidationIssues,
	IReadOnlyList<ConverterExportItem> Items);

internal sealed record ConverterCliOptions(
	string Command,
	string RegionsDirectory,
	string? OutputPath,
	string? ConnectionString,
	bool Execute,
	bool UseBaseline);

internal sealed record BaselineLoadResult(
	FuturemudDatabaseContext? Context,
	FutureMudBaselineCatalog? Catalog,
	string Status);

internal static class Program
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		WriteIndented = true,
		Converters = { new JsonStringEnumConverter() }
	};

	private static async Task<int> Main(string[] args)
	{
		try
		{
			var options = ParseOptions(args);
			if (options is null)
			{
				return 1;
			}

			if (!Directory.Exists(options.RegionsDirectory))
			{
				Console.Error.WriteLine($"Could not find regions directory '{options.RegionsDirectory}'.");
				return 1;
			}

			var parser = new RpiWorldfileParser();
			var corpus = parser.ParseDirectory(options.RegionsDirectory);

			var baseline = LoadBaseline(options);
			using var baselineContext = baseline.Context;
			if (options.Command == "apply-items" && baseline.Catalog is null)
			{
				Console.Error.WriteLine($"Unable to load the FutureMUD baseline catalog: {baseline.Status}");
				return 3;
			}

			var transformer = new FutureMUDItemTransformer(baseline.Catalog);
			var converted = transformer.Convert(corpus.Items);
			var validationIssues = baseline.Catalog is not null
				? FutureMudItemValidation.Validate(baseline.Catalog, converted)
				: Array.Empty<FutureMudValidationIssue>();
			var summary = BuildAnalysisSummary(corpus, converted, validationIssues, baseline.Status);

			return options.Command switch
			{
				"analyze-items" => await RunAnalyze(summary, corpus, validationIssues),
				"export-items" => await RunExport(options, corpus, converted, validationIssues, summary),
				"apply-items" => RunApply(options, converted, baseline.Context!, baseline.Catalog!, summary),
				_ => 1
			};
		}
		catch (ArgumentException ex)
		{
			Console.Error.WriteLine(ex.Message);
			PrintUsage();
			return 1;
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine(ex);
			return 99;
		}
	}

	private static async Task<int> RunAnalyze(
		ConverterAnalysisSummary summary,
		RpiParsedCorpus corpus,
		IReadOnlyList<FutureMudValidationIssue> validationIssues)
	{
		Console.WriteLine($"Parsed item blocks: {summary.ParsedItemCount:N0} of {summary.TotalBlockCount:N0}");
		Console.WriteLine($"Parse failures: {summary.FailureCount:N0}");
		Console.WriteLine($"Parse warnings: {summary.ParseWarningCount:N0}");
		Console.WriteLine($"Baseline: {summary.BaselineStatus}");
		Console.WriteLine();

		PrintDictionary("Per-type totals", summary.PerTypeCounts);
		Console.WriteLine();
		PrintDictionary("Per-status totals", summary.PerStatusCounts);

		if (summary.WarningCodeCounts.Count > 0)
		{
			Console.WriteLine();
			PrintDictionary("Warning codes", summary.WarningCodeCounts);
		}

		if (summary.MissingDependencyCounts.Count > 0)
		{
			Console.WriteLine();
			PrintDictionary("Missing dependencies", summary.MissingDependencyCounts);
		}
		else if (validationIssues.Count == 0)
		{
			Console.WriteLine();
			Console.WriteLine("Missing dependencies: none reported");
		}

		if (corpus.Failures.Count > 0)
		{
			Console.WriteLine();
			Console.WriteLine("Sample parse failures:");
			foreach (var failure in corpus.Failures.Take(10))
			{
				Console.WriteLine($"- {Path.GetFileName(failure.SourceFile)} {failure.Header}: {failure.Message}");
			}
		}

		if (validationIssues.Count > 0)
		{
			Console.WriteLine();
			Console.WriteLine("Sample validation issues:");
			foreach (var issue in validationIssues.Take(15))
			{
				Console.WriteLine($"- [{issue.Severity}] {issue.SourceKey}: {issue.Message}");
			}
		}

		await Task.CompletedTask;
		return 0;
	}

	private static async Task<int> RunExport(
		ConverterCliOptions options,
		RpiParsedCorpus corpus,
		IReadOnlyList<ConvertedItemDefinition> converted,
		IReadOnlyList<FutureMudValidationIssue> validationIssues,
		ConverterAnalysisSummary summary)
	{
		var outputPath = ResolveOutputPath(options.OutputPath, "rpi-items-export.json");
		var convertedBySourceKey = converted.ToDictionary(x => x.SourceKey, StringComparer.OrdinalIgnoreCase);
		var export = new ConverterExportReport(
			DateTime.UtcNow,
			options.RegionsDirectory,
			summary,
			corpus.Failures,
			validationIssues,
			corpus.Items
				.OrderBy(x => x.Zone)
				.ThenBy(x => x.Vnum)
				.Select(x => new ConverterExportItem(x, convertedBySourceKey[x.SourceKey]))
				.ToList());

		await using var stream = File.Create(outputPath);
		await JsonSerializer.SerializeAsync(stream, export, JsonOptions);

		Console.WriteLine($"Wrote {export.Items.Count:N0} converted item records to {outputPath}");
		Console.WriteLine($"Baseline: {summary.BaselineStatus}");
		return 0;
	}

	private static int RunApply(
		ConverterCliOptions options,
		IReadOnlyList<ConvertedItemDefinition> converted,
		FuturemudDatabaseContext context,
		FutureMudBaselineCatalog catalog,
		ConverterAnalysisSummary summary)
	{
		var importer = new FutureMudItemImporter(context, catalog);
		var result = importer.Apply(converted, options.Execute);
		var fatalErrors = result.Issues.Count(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase));

		Console.WriteLine(options.Execute ? "Apply mode: execute" : "Apply mode: dry-run");
		Console.WriteLine($"Baseline: {summary.BaselineStatus}");
		Console.WriteLine($"Validation issues: {result.Issues.Count:N0} total, {fatalErrors:N0} error(s)");
		Console.WriteLine($"Existing imports skipped: {result.SkippedExistingCount:N0}");

		if (options.Execute)
		{
			Console.WriteLine($"Inserted items: {result.InsertedCount:N0}");
		}

		if (result.Issues.Count > 0)
		{
			Console.WriteLine();
			Console.WriteLine("Sample validation issues:");
			foreach (var issue in result.Issues.Take(20))
			{
				Console.WriteLine($"- [{issue.Severity}] {issue.SourceKey}: {issue.Message}");
			}
		}

		if (fatalErrors > 0)
		{
			Console.Error.WriteLine("Apply did not proceed because required baseline dependencies were missing.");
			return 2;
		}

		return 0;
	}

	private static ConverterAnalysisSummary BuildAnalysisSummary(
		RpiParsedCorpus corpus,
		IReadOnlyList<ConvertedItemDefinition> converted,
		IReadOnlyList<FutureMudValidationIssue> validationIssues,
		string baselineStatus)
	{
		return new ConverterAnalysisSummary(
			corpus.Items.Count + corpus.Failures.Count,
			corpus.Items.Count,
			corpus.Failures.Count,
			corpus.Items.Sum(x => x.ParseWarnings.Count),
			baselineStatus,
			ToSortedCounts(corpus.Items.GroupBy(x => x.ItemType.ToString())),
			ToSortedCounts(converted.GroupBy(x => x.Status.ToString())),
			ToSortedCounts(converted.SelectMany(x => x.Warnings).GroupBy(x => x.Code)),
			ToSortedCounts(validationIssues.GroupBy(x => x.Message)));
	}

	private static IReadOnlyDictionary<string, int> ToSortedCounts<T>(IEnumerable<IGrouping<string, T>> groups)
	{
		return groups
			.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.Count(), StringComparer.OrdinalIgnoreCase);
	}

	private static void PrintDictionary(string title, IReadOnlyDictionary<string, int> values)
	{
		Console.WriteLine(title);
		foreach (var value in values)
		{
			Console.WriteLine($"- {value.Key}: {value.Value:N0}");
		}
	}

	private static BaselineLoadResult LoadBaseline(ConverterCliOptions options)
	{
		if (!options.UseBaseline)
		{
			return new BaselineLoadResult(null, null, "Skipped baseline validation by request.");
		}

		FuturemudDatabaseContext? context = null;
		try
		{
			context = new FuturemudDatabaseContext();
			if (!string.IsNullOrWhiteSpace(options.ConnectionString))
			{
				context.ConnectionString = options.ConnectionString;
			}

			var catalog = FutureMudBaselineCatalog.Load(context);
			return new BaselineLoadResult(context, catalog, "Loaded baseline catalog from FutureMUD.");
		}
		catch (Exception ex)
		{
			context?.Dispose();
			return new BaselineLoadResult(null, null, ex.Message);
		}
	}

	private static ConverterCliOptions? ParseOptions(string[] args)
	{
		if (args.Length == 0 || args[0] is "--help" or "-h" or "help")
		{
			PrintUsage();
			return null;
		}

		var command = args[0];
		if (command is not ("analyze-items" or "export-items" or "apply-items"))
		{
			throw new ArgumentException($"Unknown command '{command}'.");
		}

		string? root = null;
		string? output = null;
		string? connectionString = null;
		var execute = false;
		var useBaseline = true;

		for (var i = 1; i < args.Length; i++)
		{
			switch (args[i])
			{
				case "--root":
					root = ReadOptionValue(args, ref i, "--root");
					break;
				case "--output":
					output = ReadOptionValue(args, ref i, "--output");
					break;
				case "--db-connection":
				case "--connection-string":
					connectionString = ReadOptionValue(args, ref i, args[i]);
					break;
				case "--execute":
					execute = true;
					break;
				case "--skip-baseline":
					useBaseline = false;
					break;
				default:
					throw new ArgumentException($"Unknown option '{args[i]}'.");
			}
		}

		if (command == "apply-items" && !useBaseline)
		{
			throw new ArgumentException("apply-items requires a seeded FutureMUD baseline and cannot be run with --skip-baseline.");
		}

		return new ConverterCliOptions(
			command,
			ResolveRegionsDirectory(root),
			output,
			connectionString,
			execute,
			useBaseline);
	}

	private static string ReadOptionValue(string[] args, ref int index, string optionName)
	{
		if (index + 1 >= args.Length)
		{
			throw new ArgumentException($"Option {optionName} requires a value.");
		}

		index++;
		return args[index];
	}

	private static string ResolveRegionsDirectory(string? configuredRoot)
	{
		if (!string.IsNullOrWhiteSpace(configuredRoot))
		{
			return Path.GetFullPath(configuredRoot);
		}

		var candidates = new[]
		{
			Path.Combine(Directory.GetCurrentDirectory(), "soiregions-main"),
			Path.Combine(Directory.GetCurrentDirectory(), "RPI Engine Worldfile Converter", "soiregions-main"),
			Path.Combine(AppContext.BaseDirectory, "soiregions-main"),
			Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "soiregions-main"),
			Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "RPI Engine Worldfile Converter", "soiregions-main")
		}
			.Select(Path.GetFullPath)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		return candidates.FirstOrDefault(Directory.Exists) ?? candidates[0];
	}

	private static string ResolveOutputPath(string? configuredOutput, string fallbackFileName)
	{
		return Path.GetFullPath(configuredOutput ?? Path.Combine(Directory.GetCurrentDirectory(), fallbackFileName));
	}

	private static void PrintUsage()
	{
		Console.WriteLine("RPI Engine Worldfile Converter");
		Console.WriteLine();
		Console.WriteLine("Commands:");
		Console.WriteLine("  analyze-items [--root <regions-dir>] [--db-connection <connection-string>] [--skip-baseline]");
		Console.WriteLine("  export-items [--root <regions-dir>] [--output <json-path>] [--db-connection <connection-string>] [--skip-baseline]");
		Console.WriteLine("  apply-items [--root <regions-dir>] [--db-connection <connection-string>] [--execute]");
		Console.WriteLine();
		Console.WriteLine("Notes:");
		Console.WriteLine("  apply-items defaults to dry-run mode unless --execute is supplied.");
		Console.WriteLine("  The default regions directory is the bundled soiregions-main corpus.");
	}
}

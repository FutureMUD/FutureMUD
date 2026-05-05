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
	string ClanSourceFile,
	string CraftSourceFile,
	string? OutputPath,
	string? ConnectionString,
	string? ZoneTemplate,
	bool Execute,
	bool UseBaseline);

internal sealed record ItemBaselineLoadResult(
	FuturemudDatabaseContext? Context,
	FutureMudBaselineCatalog? Catalog,
	string Status);

internal sealed record ClanBaselineLoadResult(
	FuturemudDatabaseContext? Context,
	FutureMudClanBaselineCatalog? Catalog,
	string Status);

internal sealed record CraftBaselineLoadResult(
	FuturemudDatabaseContext? Context,
	FutureMudCraftBaselineCatalog? Catalog,
	string Status);

internal sealed record RoomBaselineLoadResult(
	FuturemudDatabaseContext? Context,
	FutureMudRoomBaselineCatalog? Catalog,
	string Status);

internal sealed record NpcBaselineLoadResult(
	FuturemudDatabaseContext? Context,
	FutureMudNpcBaselineCatalog? Catalog,
	string Status);

internal static class Program
{
#if DEBUG
	private const string DebugDefaultConnectionString = "server=localhost;port=3307;database=rpi_engine;uid=futuremud;password=rpiengine2020;SslMode=None;AllowPublicKeyRetrieval=True;Default Command Timeout=300000;";
#endif

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

			if (IsItemCommand(options.Command))
			{
				return await RunItemCommand(options);
			}

			if (IsClanCommand(options.Command))
			{
				return await RunClanCommand(options);
			}

			if (IsCraftCommand(options.Command))
			{
				return await RunCraftCommand(options);
			}

			if (IsNpcCommand(options.Command))
			{
				return await RunNpcCommand(options);
			}

			return await RunRoomCommand(options);
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

	private static async Task<int> RunItemCommand(ConverterCliOptions options)
	{
		if (!Directory.Exists(options.RegionsDirectory))
		{
			Console.Error.WriteLine($"Could not find regions directory '{options.RegionsDirectory}'.");
			return 1;
		}

		var parser = new RpiWorldfileParser();
		var corpus = parser.ParseDirectory(options.RegionsDirectory);
		var baseline = LoadItemBaseline(options);
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
		var summary = BuildItemAnalysisSummary(corpus, converted, validationIssues, baseline.Status);

		return options.Command switch
		{
			"analyze-items" => await RunAnalyzeItems(summary, corpus, validationIssues),
			"export-items" => await RunExportItems(options, corpus, converted, validationIssues, summary),
			"apply-items" => RunApplyItems(options, converted, baseline.Context!, baseline.Catalog!, summary),
			_ => 1,
		};
	}

	private static async Task<int> RunClanCommand(ConverterCliOptions options)
	{
		if (!Directory.Exists(options.RegionsDirectory))
		{
			Console.Error.WriteLine($"Could not find regions directory '{options.RegionsDirectory}'.");
			return 1;
		}

		if (!File.Exists(options.ClanSourceFile))
		{
			Console.Error.WriteLine($"Could not find clan source file '{options.ClanSourceFile}'.");
			return 1;
		}

		var sourceParser = new RpiClanSourceParser();
		var sourceDocument = sourceParser.Parse(options.ClanSourceFile);
		var itemParser = new RpiWorldfileParser();
		var itemCorpus = itemParser.ParseDirectory(options.RegionsDirectory);
		var scanner = new RpiClanRegionReferenceScanner();
		var references = scanner.Scan(options.RegionsDirectory, sourceDocument, itemCorpus.Items);

		var baseline = LoadClanBaseline(options);
		using var baselineContext = baseline.Context;
		if (options.Command == "apply-clans" && baseline.Catalog is null)
		{
			Console.Error.WriteLine($"Unable to load the FutureMUD clan baseline: {baseline.Status}");
			return 3;
		}

		var transformer = new FutureMudClanTransformer();
		var conversion = transformer.Convert(sourceDocument, references);
		var validationIssues = baseline.Catalog is not null
			? FutureMudClanValidation.Validate(baseline.Catalog, conversion.ConvertedClans)
			: Array.Empty<FutureMudClanValidationIssue>();
		var summary = BuildClanAnalysisSummary(sourceDocument, conversion, validationIssues, baseline.Status);

		return options.Command switch
		{
			"analyze-clans" => await RunAnalyzeClans(sourceDocument, conversion, summary, validationIssues),
			"export-clans" => await RunExportClans(options, sourceDocument, conversion, validationIssues, summary),
			"apply-clans" => RunApplyClans(options, conversion.ConvertedClans, baseline.Context!, baseline.Catalog!, summary),
			_ => 1,
		};
	}

	private static async Task<int> RunRoomCommand(ConverterCliOptions options)
	{
		if (!Directory.Exists(options.RegionsDirectory))
		{
			Console.Error.WriteLine($"Could not find regions directory '{options.RegionsDirectory}'.");
			return 1;
		}

		var parser = new RpiRoomWorldfileParser();
		var corpus = parser.ParseDirectory(options.RegionsDirectory);
		var baseline = LoadRoomBaseline(options);
		using var baselineContext = baseline.Context;
		if (options.Command == "apply-rooms" && baseline.Catalog is null)
		{
			Console.Error.WriteLine($"Unable to load the FutureMUD room baseline: {baseline.Status}");
			return 3;
		}

		var transformer = new FutureMudRoomTransformer();
		var conversion = transformer.Convert(corpus.Rooms);
		var validationIssues = baseline.Catalog is not null
			? FutureMudRoomValidation.Validate(baseline.Catalog, conversion, options.ZoneTemplate)
			: Array.Empty<FutureMudRoomValidationIssue>();
		var summary = BuildRoomAnalysisSummary(corpus, conversion, validationIssues, baseline.Status);

		return options.Command switch
		{
			"analyze-rooms" => await RunAnalyzeRooms(summary, corpus, conversion, validationIssues),
			"export-rooms" => await RunExportRooms(options, corpus, conversion, validationIssues, summary),
			"apply-rooms" => await RunApplyRooms(options, conversion, baseline.Context!, baseline.Catalog!, summary),
			_ => 1,
		};
	}

	private static async Task<int> RunCraftCommand(ConverterCliOptions options)
	{
		if (!Directory.Exists(options.RegionsDirectory))
		{
			Console.Error.WriteLine($"Could not find regions directory '{options.RegionsDirectory}'.");
			return 1;
		}

		if (!File.Exists(options.CraftSourceFile))
		{
			Console.Error.WriteLine($"Could not find craft source file '{options.CraftSourceFile}'.");
			return 1;
		}

		var itemParser = new RpiWorldfileParser();
		var itemCorpus = itemParser.ParseDirectory(options.RegionsDirectory);
		var itemTransformer = new FutureMUDItemTransformer();
		var convertedItems = itemTransformer.Convert(itemCorpus.Items);

		var craftParser = new RpiCraftParser();
		var craftCorpus = craftParser.ParseFile(options.CraftSourceFile);

		var baseline = LoadCraftBaseline(options);
		using var baselineContext = baseline.Context;
		if (options.Command == "apply-crafts" && baseline.Catalog is null)
		{
			Console.Error.WriteLine($"Unable to load the FutureMUD craft baseline: {baseline.Status}");
			return 3;
		}

		var transformer = new FutureMudCraftTransformer(convertedItems);
		var conversion = transformer.Convert(craftCorpus.Crafts);
		var validationIssues = baseline.Catalog is not null
			? FutureMudCraftValidation.Validate(baseline.Catalog, conversion.Crafts)
			: Array.Empty<FutureMudCraftValidationIssue>();
		var summary = BuildCraftAnalysisSummary(craftCorpus, conversion, validationIssues, baseline.Status);

		return options.Command switch
		{
			"analyze-crafts" => await RunAnalyzeCrafts(summary, craftCorpus, conversion, validationIssues),
			"export-crafts" => await RunExportCrafts(options, craftCorpus, conversion, validationIssues, summary),
			"apply-crafts" => await RunApplyCrafts(options, conversion.Crafts, baseline.Context!, baseline.Catalog!, summary),
			_ => 1,
		};
	}

	private static async Task<int> RunNpcCommand(ConverterCliOptions options)
	{
		if (!Directory.Exists(options.RegionsDirectory))
		{
			Console.Error.WriteLine($"Could not find regions directory '{options.RegionsDirectory}'.");
			return 1;
		}

		var roomParser = new RpiRoomWorldfileParser();
		var roomCorpus = roomParser.ParseDirectory(options.RegionsDirectory);
		var roomTransformer = new FutureMudRoomTransformer();
		var roomConversion = roomTransformer.Convert(roomCorpus.Rooms);
		var zoneEvidence = FutureMudNpcTransformer.BuildZoneEvidence(roomConversion);

		var parser = new RpiNpcWorldfileParser();
		var corpus = parser.ParseDirectory(options.RegionsDirectory);
		var baseline = LoadNpcBaseline(options);
		using var baselineContext = baseline.Context;
		if (options.Command == "apply-npcs" && baseline.Catalog is null)
		{
			Console.Error.WriteLine($"Unable to load the FutureMUD NPC baseline: {baseline.Status}");
			return 3;
		}

		var transformer = new FutureMudNpcTransformer(zoneEvidence);
		var conversion = transformer.Convert(corpus.Npcs);
		var validationIssues = baseline.Catalog is not null
			? FutureMudNpcValidation.Validate(baseline.Catalog, conversion.Npcs)
			: Array.Empty<FutureMudNpcValidationIssue>();
		var summary = BuildNpcAnalysisSummary(corpus, conversion, validationIssues, baseline.Status);

		return options.Command switch
		{
			"analyze-npcs" => await RunAnalyzeNpcs(summary, corpus, conversion, validationIssues),
			"export-npcs" => await RunExportNpcs(options, corpus, conversion, validationIssues, summary),
			"apply-npcs" => await RunApplyNpcs(options, conversion.Npcs, baseline.Context!, baseline.Catalog!, summary),
			_ => 1,
		};
	}

	private static async Task<int> RunAnalyzeItems(
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

		PrintMissingDependencies(summary.MissingDependencyCounts, validationIssues.Count);

		if (corpus.Failures.Count > 0)
		{
			Console.WriteLine();
			Console.WriteLine("Sample parse failures:");
			foreach (var failure in corpus.Failures.Take(10))
			{
				Console.WriteLine($"- {Path.GetFileName(failure.SourceFile)} {failure.Header}: {failure.Message}");
			}
		}

		PrintItemValidationIssues(validationIssues);
		await Task.CompletedTask;
		return 0;
	}

	private static async Task<int> RunExportItems(
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

	private static int RunApplyItems(
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
		Console.WriteLine($"Skipped by design: {result.SkippedByDesignCount:N0}");
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

	private static async Task<int> RunAnalyzeClans(
		RpiClanSourceDocument sourceDocument,
		ClanConversionResult conversion,
		ClanAnalysisSummary summary,
		IReadOnlyList<FutureMudClanValidationIssue> validationIssues)
	{
		Console.WriteLine($"Clan source file: {sourceDocument.SourceFile}");
		Console.WriteLine($"Source clans: {summary.SourceClanCount:N0}");
		Console.WriteLine($"Imported clans: {summary.ImportedClanCount:N0}");
		Console.WriteLine($"Imported ranks: {summary.ImportedRankCount:N0}");
		Console.WriteLine($"Baseline: {summary.BaselineStatus}");
		Console.WriteLine();

		PrintDictionary("Per-clan rank totals", summary.PerClanRankCounts);
		Console.WriteLine();
		PrintDictionary("Per-path totals", summary.PerPathCounts);
		Console.WriteLine();
		PrintDictionary("Per-slot totals", summary.PerSlotCounts);

		if (summary.WarningCodeCounts.Count > 0)
		{
			Console.WriteLine();
			PrintDictionary("Warning codes", summary.WarningCodeCounts);
		}

		PrintMissingDependencies(summary.MissingDependencyCounts, validationIssues.Count);

		if (sourceDocument.ParseWarnings.Count > 0)
		{
			Console.WriteLine();
			Console.WriteLine("Parse warnings:");
			foreach (var warning in sourceDocument.ParseWarnings.Take(15))
			{
				Console.WriteLine($"- {warning}");
			}
		}

		if (summary.UnresolvedAliasCounts.Count > 0)
		{
			Console.WriteLine();
			PrintDictionary("Unresolved aliases", summary.UnresolvedAliasCounts);
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

		if (conversion.ConvertedClans.Count > 0)
		{
			Console.WriteLine();
			Console.WriteLine("Sample imported clans:");
			foreach (var clan in conversion.ConvertedClans.Take(10))
			{
				Console.WriteLine($"- {clan.FullName} ({clan.CanonicalAlias}) => {clan.Ranks.Count:N0} rank(s)");
			}
		}

		await Task.CompletedTask;
		return 0;
	}

	private static async Task<int> RunExportClans(
		ConverterCliOptions options,
		RpiClanSourceDocument sourceDocument,
		ClanConversionResult conversion,
		IReadOnlyList<FutureMudClanValidationIssue> validationIssues,
		ClanAnalysisSummary summary)
	{
		var outputPath = ResolveOutputPath(options.OutputPath, "rpi-clans-export.json");
		var convertedBySourceKey = conversion.ConvertedClans.ToDictionary(x => x.SourceKey, StringComparer.OrdinalIgnoreCase);
		var export = new ClanExportReport(
			DateTime.UtcNow,
			sourceDocument.SourceFile,
			options.RegionsDirectory,
			summary,
			sourceDocument.ParseWarnings,
			validationIssues,
			conversion.SourceClans
				.OrderBy(x => x.FullName, StringComparer.OrdinalIgnoreCase)
				.Select(x => new ConverterExportClan(x, convertedBySourceKey[x.SourceKey]))
				.ToList());

		await using var stream = File.Create(outputPath);
		await JsonSerializer.SerializeAsync(stream, export, JsonOptions);

		Console.WriteLine($"Wrote {export.Clans.Count:N0} converted clan records to {outputPath}");
		Console.WriteLine($"Baseline: {summary.BaselineStatus}");
		return 0;
	}

	private static int RunApplyClans(
		ConverterCliOptions options,
		IReadOnlyList<ConvertedClanDefinition> converted,
		FuturemudDatabaseContext context,
		FutureMudClanBaselineCatalog baseline,
		ClanAnalysisSummary summary)
	{
		var importer = new FutureMudClanImporter(context, baseline);
		var result = importer.Apply(converted, options.Execute);
		var fatalErrors = result.Issues.Count(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase));

		Console.WriteLine(options.Execute ? "Apply mode: execute" : "Apply mode: dry-run");
		Console.WriteLine($"Baseline: {summary.BaselineStatus}");
		Console.WriteLine($"Validation issues: {result.Issues.Count:N0} total, {fatalErrors:N0} error(s)");
		Console.WriteLine($"Existing imports skipped: {result.SkippedExistingCount:N0}");

		if (options.Execute)
		{
			Console.WriteLine($"Inserted clans: {result.InsertedCount:N0}");
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

	private static async Task<int> RunAnalyzeRooms(
		RoomAnalysisSummary summary,
		RpiParsedRoomCorpus corpus,
		RoomConversionResult conversion,
		IReadOnlyList<FutureMudRoomValidationIssue> validationIssues)
	{
		Console.WriteLine($"Parsed room blocks: {summary.ParsedRoomCount:N0} of {summary.TotalBlockCount:N0}");
		Console.WriteLine($"Parse failures: {summary.FailureCount:N0}");
		Console.WriteLine($"Parse warnings: {summary.ParseWarningCount:N0}");
		Console.WriteLine($"Xerox rooms resolved: {summary.XeroxResolvedCount:N0}");
		Console.WriteLine($"Hidden exits: {summary.HiddenExitCount:N0}");
		Console.WriteLine($"Trapped exits: {summary.TrappedExitCount:N0}");
		Console.WriteLine($"Climb exits: {summary.ClimbExitCount:N0}");
		Console.WriteLine($"Fall exits: {summary.FallExitCount:N0}");
		Console.WriteLine($"Baseline: {summary.BaselineStatus}");
		Console.WriteLine();

		PrintDictionary("Per-sector totals", summary.PerSectorCounts);
		Console.WriteLine();
		PrintDictionary("Per-terrain totals", summary.PerTerrainCounts);
		Console.WriteLine();
		PrintDictionary("Per-zone totals", summary.PerZoneCounts);

		if (summary.WarningCodeCounts.Count > 0)
		{
			Console.WriteLine();
			PrintDictionary("Warning codes", summary.WarningCodeCounts);
		}

		PrintMissingDependencies(summary.MissingDependencyCounts, validationIssues.Count);

		if (corpus.Failures.Count > 0)
		{
			Console.WriteLine();
			Console.WriteLine("Sample parse failures:");
			foreach (var failure in corpus.Failures.Take(10))
			{
				Console.WriteLine($"- {Path.GetFileName(failure.SourceFile)} {failure.Header}: {failure.Message}");
			}
		}

		var zoneWarnings = conversion.Zones.SelectMany(x => x.Warnings.Select(y => $"{x.ZoneName}: {y.Message}")).Take(15).ToList();
		if (zoneWarnings.Count > 0)
		{
			Console.WriteLine();
			Console.WriteLine("Zone warnings:");
			foreach (var warning in zoneWarnings)
			{
				Console.WriteLine($"- {warning}");
			}
		}

		if (validationIssues.Count > 0)
		{
			Console.WriteLine();
			Console.WriteLine("Sample validation issues:");
			foreach (var issue in validationIssues.Take(20))
			{
				Console.WriteLine($"- [{issue.Severity}] {issue.SourceKey}: {issue.Message}");
			}
		}

		await Task.CompletedTask;
		return 0;
	}

	private static async Task<int> RunAnalyzeCrafts(
		CraftAnalysisSummary summary,
		RpiCraftCorpus corpus,
		CraftConversionResult conversion,
		IReadOnlyList<FutureMudCraftValidationIssue> validationIssues)
	{
		Console.WriteLine($"Parsed craft blocks: {summary.ParsedCraftCount:N0} of {summary.TotalCraftCount:N0}");
		Console.WriteLine($"Parse failures: {summary.FailureCount:N0}");
		Console.WriteLine($"Parse warnings: {summary.ParseWarningCount:N0}");
		Console.WriteLine($"Total phases: {summary.TotalPhaseCount:N0}");
		Console.WriteLine($"Keyed crafts: {summary.KeyedCraftCount:N0}");
		Console.WriteLine($"Multi-check crafts: {summary.MultiCheckCount:N0}");
		Console.WriteLine($"Generated tags: {summary.GeneratedTagCount:N0}");
		Console.WriteLine($"Deferred crafts: {summary.DeferredCraftCount:N0}");
		Console.WriteLine($"Baseline: {summary.BaselineStatus}");
		Console.WriteLine();

		PrintDictionary("Per-status totals", summary.PerStatusCounts);
		Console.WriteLine();
		PrintDictionary("Feature totals", summary.FeatureCounts);

		if (conversion.DeferredReasonCounts.Count > 0)
		{
			Console.WriteLine();
			PrintDictionary("Deferred reasons", conversion.DeferredReasonCounts);
		}

		if (summary.WarningCodeCounts.Count > 0)
		{
			Console.WriteLine();
			PrintDictionary("Warning codes", summary.WarningCodeCounts);
		}

		PrintMissingDependencies(summary.MissingDependencyCounts, validationIssues.Count);

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
			foreach (var issue in validationIssues.Take(20))
			{
				Console.WriteLine($"- [{issue.Severity}] {issue.SourceKey}: {issue.Message}");
			}
		}

		await Task.CompletedTask;
		return 0;
	}

	private static async Task<int> RunAnalyzeNpcs(
		NpcAnalysisSummary summary,
		RpiParsedNpcCorpus corpus,
		NpcConversionResult conversion,
		IReadOnlyList<FutureMudNpcValidationIssue> validationIssues)
	{
		Console.WriteLine($"Parsed mob blocks: {summary.ParsedMobCount:N0} of {summary.TotalMobCount:N0}");
		Console.WriteLine($"Parse failures: {summary.FailureCount:N0}");
		Console.WriteLine($"Parse warnings: {summary.ParseWarningCount:N0}");
		Console.WriteLine($"Simple templates: {summary.SimpleTemplateCount:N0}");
		Console.WriteLine($"Variable templates: {summary.VariableTemplateCount:N0}");
		Console.WriteLine($"Deferred templates: {summary.DeferredTemplateCount:N0}");
		Console.WriteLine($"Resolved races: {summary.ResolvedRaceCount:N0}");
		Console.WriteLine($"Resolved ethnicities: {summary.ResolvedEthnicityCount:N0}");
		Console.WriteLine($"Resolved cultures: {summary.ResolvedCultureCount:N0}");
		Console.WriteLine($"Baseline: {summary.BaselineStatus}");
		Console.WriteLine();

		PrintDictionary("Classification totals", summary.ClassificationCounts);
		Console.WriteLine();
		PrintDictionary("Per-status totals", summary.StatusCounts);
		Console.WriteLine();
		PrintDictionary("Feature totals", summary.FeatureCounts);

		if (conversion.DeferredReasonCounts.Count > 0)
		{
			Console.WriteLine();
			PrintDictionary("Deferred reasons", conversion.DeferredReasonCounts);
		}

		if (summary.WarningCodeCounts.Count > 0)
		{
			Console.WriteLine();
			PrintDictionary("Warning codes", summary.WarningCodeCounts);
		}

		PrintMissingDependencies(summary.MissingDependencyCounts, validationIssues.Count);

		if (conversion.UnresolvedRaceCounts.Count > 0)
		{
			Console.WriteLine();
			PrintDictionary("Unresolved race zones", conversion.UnresolvedRaceCounts);
		}

		if (conversion.UnresolvedCultureCounts.Count > 0)
		{
			Console.WriteLine();
			PrintDictionary("Unresolved culture zones", conversion.UnresolvedCultureCounts);
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
			foreach (var issue in validationIssues.Take(20))
			{
				Console.WriteLine($"- [{issue.Severity}] {issue.SourceKey}: {issue.Message}");
			}
		}

		await Task.CompletedTask;
		return 0;
	}

	private static async Task<int> RunExportRooms(
		ConverterCliOptions options,
		RpiParsedRoomCorpus corpus,
		RoomConversionResult conversion,
		IReadOnlyList<FutureMudRoomValidationIssue> validationIssues,
		RoomAnalysisSummary summary)
	{
		var outputPath = ResolveOutputPath(options.OutputPath, "rpi-rooms-export.json");
		var auditPath = ResolveSidecarAuditPath(outputPath);
		var convertedBySourceKey = conversion.Rooms.ToDictionary(x => x.SourceKey, StringComparer.OrdinalIgnoreCase);
		var export = new RoomExportReport(
			DateTime.UtcNow,
			options.RegionsDirectory,
			summary,
			corpus.Failures,
			validationIssues,
			conversion.Zones,
			corpus.Rooms
				.OrderBy(x => x.Zone)
				.ThenBy(x => x.Vnum)
				.Select(x => new ConverterExportRoom(x, convertedBySourceKey[x.SourceKey]))
				.ToList());
		var audit = BuildRoomLogicalAudit(conversion);

		await using (var stream = File.Create(outputPath))
		{
			await JsonSerializer.SerializeAsync(stream, export, JsonOptions);
		}

		await using (var stream = File.Create(auditPath))
		{
			await JsonSerializer.SerializeAsync(stream, audit, JsonOptions);
		}

		Console.WriteLine($"Wrote {export.Rooms.Count:N0} converted room records to {outputPath}");
		Console.WriteLine($"Wrote logical room audit to {auditPath}");
		Console.WriteLine($"Baseline: {summary.BaselineStatus}");
		return 0;
	}

	private static async Task<int> RunExportCrafts(
		ConverterCliOptions options,
		RpiCraftCorpus corpus,
		CraftConversionResult conversion,
		IReadOnlyList<FutureMudCraftValidationIssue> validationIssues,
		CraftAnalysisSummary summary)
	{
		var outputPath = ResolveOutputPath(options.OutputPath, "rpi-crafts-export.json");
		var auditPath = ResolveSidecarAuditPath(outputPath);
		var convertedBySourceKey = conversion.Crafts.ToDictionary(x => x.SourceKey, StringComparer.OrdinalIgnoreCase);
		var export = new CraftExportReport(
			DateTime.UtcNow,
			options.CraftSourceFile,
			options.RegionsDirectory,
			summary,
			corpus.Failures,
			validationIssues,
			corpus.Crafts
				.OrderBy(x => x.CraftNumber)
				.Select(x => new ConverterExportCraft(x, convertedBySourceKey[x.SourceKey]))
				.ToList());
		var audit = BuildCraftExportAudit(conversion);

		await using (var stream = File.Create(outputPath))
		{
			await JsonSerializer.SerializeAsync(stream, export, JsonOptions);
		}

		await using (var stream = File.Create(auditPath))
		{
			await JsonSerializer.SerializeAsync(stream, audit, JsonOptions);
		}

		Console.WriteLine($"Wrote {export.Crafts.Count:N0} converted craft records to {outputPath}");
		Console.WriteLine($"Wrote craft audit to {auditPath}");
		Console.WriteLine($"Baseline: {summary.BaselineStatus}");
		return 0;
	}

	private static async Task<int> RunExportNpcs(
		ConverterCliOptions options,
		RpiParsedNpcCorpus corpus,
		NpcConversionResult conversion,
		IReadOnlyList<FutureMudNpcValidationIssue> validationIssues,
		NpcAnalysisSummary summary)
	{
		var outputPath = ResolveOutputPath(options.OutputPath, "rpi-npcs-export.json");
		var auditPath = ResolveSidecarAuditPath(outputPath);
		var convertedBySourceKey = conversion.Npcs.ToDictionary(x => x.SourceKey, StringComparer.OrdinalIgnoreCase);
		var export = new NpcExportReport(
			DateTime.UtcNow,
			options.RegionsDirectory,
			summary,
			corpus.Failures,
			validationIssues,
			corpus.Npcs
				.OrderBy(x => x.Zone)
				.ThenBy(x => x.Vnum)
				.Select(x => new ConverterExportNpc(x, convertedBySourceKey[x.SourceKey]))
				.ToList());
		var audit = BuildNpcExportAudit(conversion);

		await using (var stream = File.Create(outputPath))
		{
			await JsonSerializer.SerializeAsync(stream, export, JsonOptions);
		}

		await using (var stream = File.Create(auditPath))
		{
			await JsonSerializer.SerializeAsync(stream, audit, JsonOptions);
		}

		Console.WriteLine($"Wrote {export.Npcs.Count:N0} converted NPC records to {outputPath}");
		Console.WriteLine($"Wrote NPC audit to {auditPath}");
		Console.WriteLine($"Baseline: {summary.BaselineStatus}");
		return 0;
	}

	private static async Task<int> RunApplyRooms(
		ConverterCliOptions options,
		RoomConversionResult conversion,
		FuturemudDatabaseContext context,
		FutureMudRoomBaselineCatalog baseline,
		RoomAnalysisSummary summary)
	{
		var importer = new FutureMudRoomImporter(context, baseline, options.ZoneTemplate);
		var result = importer.Apply(conversion, options.Execute);
		var fatalErrors = result.Issues.Count(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase));
		var auditPath = ResolveOutputPath(
			options.OutputPath,
			options.Execute ? "rpi-rooms-apply-audit.json" : "rpi-rooms-dry-run-audit.json");

		await using (var stream = File.Create(auditPath))
		{
			await JsonSerializer.SerializeAsync(stream, result.Audit, JsonOptions);
		}

		Console.WriteLine(options.Execute ? "Apply mode: execute" : "Apply mode: dry-run");
		Console.WriteLine($"Baseline: {summary.BaselineStatus}");
		Console.WriteLine($"Defaults: {result.Audit.DefaultsDescription}");
		Console.WriteLine($"Validation issues: {result.Issues.Count:N0} total, {fatalErrors:N0} error(s)");
		Console.WriteLine($"Existing zone groups skipped: {result.SkippedExistingZoneCount:N0}");
		Console.WriteLine($"Audit output: {auditPath}");

		if (options.Execute)
		{
			Console.WriteLine($"Inserted zones: {result.InsertedZoneCount:N0}");
			Console.WriteLine($"Inserted rooms/cells/overlays: {result.InsertedRoomCount:N0}");
			Console.WriteLine($"Inserted exits: {result.InsertedExitCount:N0}");
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

	private static async Task<int> RunApplyCrafts(
		ConverterCliOptions options,
		IReadOnlyList<ConvertedCraftDefinition> converted,
		FuturemudDatabaseContext context,
		FutureMudCraftBaselineCatalog baseline,
		CraftAnalysisSummary summary)
	{
		var importer = new FutureMudCraftImporter(context, baseline);
		var result = importer.Apply(converted, options.Execute);
		var fatalErrors = result.Issues.Count(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase));
		var auditPath = ResolveOutputPath(
			options.OutputPath,
			options.Execute ? "rpi-crafts-apply-audit.json" : "rpi-crafts-dry-run-audit.json");

		await using (var stream = File.Create(auditPath))
		{
			await JsonSerializer.SerializeAsync(stream, result.Audit, JsonOptions);
		}

		Console.WriteLine(options.Execute ? "Apply mode: execute" : "Apply mode: dry-run");
		Console.WriteLine($"Baseline: {summary.BaselineStatus}");
		Console.WriteLine($"Validation issues: {result.Issues.Count:N0} total, {fatalErrors:N0} error(s)");
		Console.WriteLine($"Existing imports skipped: {result.SkippedExistingCount:N0}");
		Console.WriteLine($"Deferred crafts skipped: {result.SkippedDeferredCount:N0}");
		Console.WriteLine($"Invalid crafts skipped: {result.SkippedInvalidCount:N0}");
		Console.WriteLine($"Audit output: {auditPath}");

		if (options.Execute)
		{
			Console.WriteLine($"Inserted crafts: {result.InsertedCount:N0}");
			Console.WriteLine($"Created tags: {result.CreatedTagCount:N0}");
			Console.WriteLine($"Created FutureProgs: {result.CreatedProgCount:N0}");
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

	private static async Task<int> RunApplyNpcs(
		ConverterCliOptions options,
		IReadOnlyList<ConvertedNpcDefinition> converted,
		FuturemudDatabaseContext context,
		FutureMudNpcBaselineCatalog baseline,
		NpcAnalysisSummary summary)
	{
		var importer = new FutureMudNpcImporter(context, baseline);
		var result = importer.Apply(converted, options.Execute);
		var fatalErrors = result.Issues.Count(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase));
		var auditPath = ResolveOutputPath(
			options.OutputPath,
			options.Execute ? "rpi-npcs-apply-audit.json" : "rpi-npcs-dry-run-audit.json");

		await using (var stream = File.Create(auditPath))
		{
			await JsonSerializer.SerializeAsync(stream, result.Audit, JsonOptions);
		}

		Console.WriteLine(options.Execute ? "Apply mode: execute" : "Apply mode: dry-run");
		Console.WriteLine($"Baseline: {summary.BaselineStatus}");
		Console.WriteLine($"Validation issues: {result.Issues.Count:N0} total, {fatalErrors:N0} error(s)");
		Console.WriteLine($"Existing imports skipped: {result.SkippedExistingCount:N0}");
		Console.WriteLine($"Deferred NPCs skipped: {result.SkippedDeferredCount:N0}");
		Console.WriteLine($"Invalid NPCs skipped: {result.SkippedInvalidCount:N0}");
		Console.WriteLine($"Audit output: {auditPath}");

		if (options.Execute)
		{
			Console.WriteLine($"Inserted NPC templates: {result.InsertedCount:N0}");
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

	private static ConverterAnalysisSummary BuildItemAnalysisSummary(
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

	private static ClanAnalysisSummary BuildClanAnalysisSummary(
		RpiClanSourceDocument sourceDocument,
		ClanConversionResult conversion,
		IReadOnlyList<FutureMudClanValidationIssue> validationIssues,
		string baselineStatus)
	{
		var allWarnings = conversion.ConvertedClans
			.SelectMany(x => x.Warnings)
			.Concat(conversion.ConvertedClans.SelectMany(x => x.Ranks).SelectMany(x => x.Warnings));
		var duplicatedFullNames = conversion.ConvertedClans
			.GroupBy(x => x.FullName, StringComparer.OrdinalIgnoreCase)
			.Where(x => x.Count() > 1)
			.Select(x => x.Key)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		return new ClanAnalysisSummary(
			sourceDocument.SourceFile,
			conversion.SourceClans.Count,
			conversion.ConvertedClans.Count,
			conversion.ConvertedClans.Sum(x => x.Ranks.Count),
			baselineStatus,
			conversion.ConvertedClans
				.OrderBy(x => x.FullName, StringComparer.OrdinalIgnoreCase)
				.ThenBy(x => x.CanonicalAlias, StringComparer.OrdinalIgnoreCase)
				.ToDictionary(
					x => duplicatedFullNames.Contains(x.FullName)
						? $"{x.FullName} ({x.CanonicalAlias})"
						: x.FullName,
					x => x.Ranks.Count,
					StringComparer.OrdinalIgnoreCase),
			ToSortedCounts(conversion.ConvertedClans.SelectMany(x => x.Ranks).GroupBy(x => x.Path.ToString())),
			ToSortedCounts(conversion.ConvertedClans.SelectMany(x => x.Ranks).GroupBy(x => x.Slot.ToString())),
			ToSortedCounts(allWarnings.GroupBy(x => x.Code)),
			conversion.UnresolvedAliasCounts,
			ToSortedCounts(validationIssues.GroupBy(x => x.Message)));
	}

	private static RoomAnalysisSummary BuildRoomAnalysisSummary(
		RpiParsedRoomCorpus corpus,
		RoomConversionResult conversion,
		IReadOnlyList<FutureMudRoomValidationIssue> validationIssues,
		string baselineStatus)
	{
		var allWarnings = conversion.Rooms
			.SelectMany(x => x.Warnings)
			.Concat(conversion.Exits.SelectMany(x => x.Warnings))
			.Concat(conversion.Zones.SelectMany(x => x.Warnings));

		return new RoomAnalysisSummary(
			corpus.Rooms.Count + corpus.Failures.Count,
			corpus.Rooms.Count,
			corpus.Failures.Count,
			corpus.Rooms.Sum(x => x.ParseWarnings.Count),
			baselineStatus,
			conversion.Rooms.Count(x => x.XeroxResolved && x.XeroxSourceVnum is not null),
			conversion.Exits.Sum(x => Convert.ToInt32(x.Side1.Hidden) + Convert.ToInt32(x.Side2.Hidden)),
			conversion.Exits.Sum(x => Convert.ToInt32(x.Side1.Trapped) + Convert.ToInt32(x.Side2.Trapped)),
			conversion.Exits.Count(x => x.IsClimbExit),
			conversion.Exits.Count(x => x.FallFromRoomVnum is not null),
			ToSortedCounts(corpus.Rooms.GroupBy(x => x.SectorType.ToString())),
			ToSortedCounts(conversion.Rooms.GroupBy(x => x.TerrainName)),
			conversion.Zones
				.GroupBy(x => x.ZoneName, StringComparer.OrdinalIgnoreCase)
				.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
				.ToDictionary(x => x.Key, x => x.Sum(y => y.Rooms.Count), StringComparer.OrdinalIgnoreCase),
			ToSortedCounts(allWarnings.GroupBy(x => x.Code)),
			ToSortedCounts(validationIssues.GroupBy(x => x.Message)));
	}

	private static CraftAnalysisSummary BuildCraftAnalysisSummary(
		RpiCraftCorpus corpus,
		CraftConversionResult conversion,
		IReadOnlyList<FutureMudCraftValidationIssue> validationIssues,
		string baselineStatus)
	{
		return new CraftAnalysisSummary(
			corpus.Crafts.Count + corpus.Failures.Count,
			corpus.Crafts.Count,
			corpus.Failures.Count,
			corpus.Crafts.Sum(x => x.ParseWarnings.Count),
			corpus.Crafts.Sum(x => x.Phases.Count),
			corpus.Crafts.Count(x => x.VariantLink.StartKey is not null && x.VariantLink.EndKey is not null),
			corpus.Crafts.Count(x =>
				x.Phases.Count(y => y.SkillCheck is not null || y.AttributeCheck is not null) > 1),
			conversion.GeneratedTags.Count,
			conversion.Crafts.Count(x => x.Status == CraftConversionStatus.Deferred),
			baselineStatus,
			ToSortedCounts(conversion.Crafts.GroupBy(x => x.Status.ToString())),
			new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
			{
				["crafts-with-clan-requirements"] = corpus.Crafts.Count(x => x.Constraints.ClanRequirements.Count > 0),
				["crafts-with-failmobs"] = corpus.Crafts.Count(x => x.FailMobVnums.Count > 0),
				["crafts-with-failobjs"] = corpus.Crafts.Count(x => x.FailObjectVnums.Count > 0),
				["crafts-with-followers"] = corpus.Crafts.Count(x => x.Constraints.FollowersRequired > 0),
				["crafts-with-generated-progs"] = conversion.Crafts.Count(x => x.FutureProgPlans.Count > 0),
				["crafts-with-generated-tags"] = conversion.Crafts.Count(x => x.GeneratedTags.Count > 0),
				["crafts-with-give-output"] = conversion.Crafts.Count(x => x.Products.Any(y => y.LegacyGiveOutput)),
				["crafts-with-hit-cost"] = corpus.Crafts.Count(x => x.Phases.Any(y => (y.Cost?.Hits ?? 0) > 0)),
				["crafts-with-mob-outputs"] = corpus.Crafts.Count(x => x.FailMobVnums.Count > 0 || x.Phases.Any(y => y.LoadMobVnum is not null)),
				["crafts-with-opening-requirements"] = corpus.Crafts.Count(x => x.Constraints.OpeningSkillIds.Count > 0),
				["crafts-with-race-requirements"] = corpus.Crafts.Count(x => x.Constraints.RaceIds.Count > 0),
				["crafts-with-season-requirements"] = corpus.Crafts.Count(x => x.Constraints.Seasons.Count > 0),
				["crafts-with-weather-requirements"] = corpus.Crafts.Count(x => x.Constraints.WeatherStates.Count > 0),
			},
			ToSortedCounts(conversion.Crafts.SelectMany(x => x.Warnings).GroupBy(x => x.Code)),
			ToSortedCounts(validationIssues.GroupBy(x => x.Message)));
	}

	private static NpcAnalysisSummary BuildNpcAnalysisSummary(
		RpiParsedNpcCorpus corpus,
		NpcConversionResult conversion,
		IReadOnlyList<FutureMudNpcValidationIssue> validationIssues,
		string baselineStatus)
	{
		return new NpcAnalysisSummary(
			corpus.Npcs.Count + corpus.Failures.Count,
			corpus.Npcs.Count,
			corpus.Failures.Count,
			corpus.Npcs.Sum(x => x.ParseWarnings.Count),
			baselineStatus,
			conversion.Npcs.Count(x => x.TemplateKind == NpcTemplateKind.Simple),
			conversion.Npcs.Count(x => x.TemplateKind == NpcTemplateKind.Variable),
			conversion.Npcs.Count(x => x.Status == NpcConversionStatus.Deferred),
			conversion.Npcs.Count(x => !string.IsNullOrWhiteSpace(x.RaceName)),
			conversion.Npcs.Count(x => !string.IsNullOrWhiteSpace(x.EthnicityName)),
			conversion.Npcs.Count(x => !string.IsNullOrWhiteSpace(x.CultureName)),
			ToSortedCounts(conversion.Npcs.GroupBy(x => x.Classification.ToString())),
			ToSortedCounts(conversion.Npcs.GroupBy(x => x.Status.ToString())),
			new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
			{
				["npcs-with-ai"] = conversion.Npcs.Count(x => x.ArtificialIntelligenceNames.Count > 0),
				["npcs-with-clans"] = conversion.Npcs.Count(x => x.HasClanMemberships),
				["npcs-with-memory"] = conversion.Npcs.Count(x => (((RpiNpcActFlags)x.RawActFlags) & RpiNpcActFlags.Memory) != 0),
				["npcs-with-morph"] = conversion.Npcs.Count(x => x.HasMorphData),
				["npcs-with-shops"] = conversion.Npcs.Count(x => x.HasShopData),
				["npcs-with-source-name"] = conversion.Npcs.Count(x => x.UsesSourceDerivedName),
				["npcs-with-venom"] = conversion.Npcs.Count(x => x.HasVenomData),
				["npcs-with-vehicles"] = conversion.Npcs.Count(x => x.DeferredBehaviorFlags.Contains("vehicle", StringComparer.OrdinalIgnoreCase)),
				["npcs-with-wildlife"] = conversion.Npcs.Count(x => (((RpiNpcActFlags)x.RawActFlags) & RpiNpcActFlags.Wildlife) != 0),
			},
			ToSortedCounts(conversion.Npcs.SelectMany(x => x.Warnings).GroupBy(x => x.Code)),
			ToSortedCounts(validationIssues.GroupBy(x => x.Message)));
	}

	private static RoomExportAuditReport BuildRoomLogicalAudit(RoomConversionResult conversion)
	{
		var exitsByRoom = conversion.Exits
			.SelectMany(x => new[]
			{
				(roomVnum: x.RoomVnum1, exitKey: x.ExitKey),
				(roomVnum: x.RoomVnum2, exitKey: x.ExitKey),
			})
			.GroupBy(x => x.roomVnum)
			.ToDictionary(
				x => x.Key,
				x => (IReadOnlyList<string>)x.Select(y => y.exitKey).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(y => y, StringComparer.OrdinalIgnoreCase).ToList());

		return new RoomExportAuditReport(
			DateTime.UtcNow,
			conversion.Rooms
				.OrderBy(x => x.SourceZone)
				.ThenBy(x => x.Vnum)
				.Select(x => new RoomLogicalAuditEntry(
					x.SourceKey,
					x.Vnum,
					x.ZoneGroupKey,
					x.ZoneName,
					x.OverlayPackageName,
					x.Coordinates,
					exitsByRoom.TryGetValue(x.Vnum, out var exitKeys) ? exitKeys : Array.Empty<string>()))
				.ToList(),
			conversion.Exits
				.Select(x => x.ExitKey)
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.ToList());
	}

	private static CraftExportAuditReport BuildCraftExportAudit(CraftConversionResult conversion)
	{
		return new CraftExportAuditReport(
			DateTime.UtcNow,
			conversion.Crafts
				.OrderBy(x => x.CraftNumber)
				.Select(x => new CraftExportAuditEntry(
					x.SourceKey,
					x.CraftNumber,
					x.Status,
					x.GeneratedTags.Select(y => y.TagName).OrderBy(y => y, StringComparer.OrdinalIgnoreCase).ToList(),
					x.FutureProgPlans.Select(y => y.FunctionName).OrderBy(y => y, StringComparer.OrdinalIgnoreCase).ToList(),
					x.Warnings.Select(y => y.Code).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(y => y, StringComparer.OrdinalIgnoreCase).ToList()))
				.ToList(),
			conversion.GeneratedTags);
	}

	private static NpcExportAuditReport BuildNpcExportAudit(NpcConversionResult conversion)
	{
		return new NpcExportAuditReport(
			DateTime.UtcNow,
			conversion.Npcs
				.OrderBy(x => x.Zone)
				.ThenBy(x => x.Vnum)
				.Select(x => new NpcExportAuditEntry(
					x.SourceKey,
					x.Vnum,
					x.Status,
					x.TemplateKind,
					x.Classification,
					x.TemplateName,
					x.RaceName,
					x.EthnicityName,
					x.CultureName,
					x.ArtificialIntelligenceNames,
					x.Warnings
						.Select(y => y.Code)
						.Distinct(StringComparer.OrdinalIgnoreCase)
						.OrderBy(y => y, StringComparer.OrdinalIgnoreCase)
						.ToList()))
				.ToList());
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

	private static void PrintMissingDependencies(IReadOnlyDictionary<string, int> missingDependencies, int validationIssueCount)
	{
		if (missingDependencies.Count > 0)
		{
			Console.WriteLine();
			PrintDictionary("Missing dependencies", missingDependencies);
		}
		else if (validationIssueCount == 0)
		{
			Console.WriteLine();
			Console.WriteLine("Missing dependencies: none reported");
		}
	}

	private static void PrintItemValidationIssues(IReadOnlyList<FutureMudValidationIssue> validationIssues)
	{
		if (validationIssues.Count == 0)
		{
			return;
		}

		Console.WriteLine();
		Console.WriteLine("Sample validation issues:");
		foreach (var issue in validationIssues.Take(15))
		{
			Console.WriteLine($"- [{issue.Severity}] {issue.SourceKey}: {issue.Message}");
		}
	}

	private static ItemBaselineLoadResult LoadItemBaseline(ConverterCliOptions options)
	{
		if (!options.UseBaseline)
		{
			return new ItemBaselineLoadResult(null, null, "Skipped baseline validation by request.");
		}

		FuturemudDatabaseContext? context = null;
		try
		{
			context = CreateDatabaseContext(options);

			var catalog = FutureMudBaselineCatalog.Load(context);
			return new ItemBaselineLoadResult(context, catalog, "Loaded baseline catalog from FutureMUD.");
		}
		catch (Exception ex)
		{
			context?.Dispose();
			return new ItemBaselineLoadResult(null, null, ex.Message);
		}
	}

	private static ClanBaselineLoadResult LoadClanBaseline(ConverterCliOptions options)
	{
		if (!options.UseBaseline)
		{
			return new ClanBaselineLoadResult(null, null, "Skipped baseline validation by request.");
		}

		FuturemudDatabaseContext? context = null;
		try
		{
			context = CreateDatabaseContext(options);

			var catalog = FutureMudClanBaselineCatalog.Load(context);
			return new ClanBaselineLoadResult(context, catalog, "Loaded clan baseline from FutureMUD.");
		}
		catch (Exception ex)
		{
			context?.Dispose();
			return new ClanBaselineLoadResult(null, null, ex.Message);
		}
	}

	private static CraftBaselineLoadResult LoadCraftBaseline(ConverterCliOptions options)
	{
		if (!options.UseBaseline)
		{
			return new CraftBaselineLoadResult(null, null, "Skipped baseline validation by request.");
		}

		FuturemudDatabaseContext? context = null;
		try
		{
			context = CreateDatabaseContext(options);

			var catalog = FutureMudCraftBaselineCatalog.Load(context);
			return new CraftBaselineLoadResult(context, catalog, "Loaded craft baseline from FutureMUD.");
		}
		catch (Exception ex)
		{
			context?.Dispose();
			return new CraftBaselineLoadResult(null, null, ex.Message);
		}
	}

	private static RoomBaselineLoadResult LoadRoomBaseline(ConverterCliOptions options)
	{
		if (!options.UseBaseline)
		{
			return new RoomBaselineLoadResult(null, null, "Skipped baseline validation by request.");
		}

		FuturemudDatabaseContext? context = null;
		try
		{
			context = CreateDatabaseContext(options);

			var catalog = FutureMudRoomBaselineCatalog.Load(context);
			return new RoomBaselineLoadResult(context, catalog, "Loaded room baseline from FutureMUD.");
		}
		catch (Exception ex)
		{
			context?.Dispose();
			return new RoomBaselineLoadResult(null, null, ex.Message);
		}
	}

	private static NpcBaselineLoadResult LoadNpcBaseline(ConverterCliOptions options)
	{
		if (!options.UseBaseline)
		{
			return new NpcBaselineLoadResult(null, null, "Skipped baseline validation by request.");
		}

		FuturemudDatabaseContext? context = null;
		try
		{
			context = CreateDatabaseContext(options);

			var catalog = FutureMudNpcBaselineCatalog.Load(context);
			return new NpcBaselineLoadResult(context, catalog, "Loaded NPC baseline from FutureMUD.");
		}
		catch (Exception ex)
		{
			context?.Dispose();
			return new NpcBaselineLoadResult(null, null, ex.Message);
		}
	}

	private static FuturemudDatabaseContext CreateDatabaseContext(ConverterCliOptions options)
	{
		var context = new FuturemudDatabaseContext();
		if (!string.IsNullOrWhiteSpace(options.ConnectionString))
		{
			context.ConnectionString = options.ConnectionString;
		}
#if DEBUG
		else
		{
			context.ConnectionString = DebugDefaultConnectionString;
		}
#endif

		return context;
	}

	private static ConverterCliOptions? ParseOptions(string[] args)
	{
		if (args.Length == 0 || args[0] is "--help" or "-h" or "help")
		{
			PrintUsage();
			return null;
		}

		var command = args[0];
		if (!IsItemCommand(command) && !IsClanCommand(command) && !IsCraftCommand(command) && !IsRoomCommand(command) && !IsNpcCommand(command))
		{
			throw new ArgumentException($"Unknown command '{command}'.");
		}

		string? root = null;
		string? clanSource = null;
		string? craftSource = null;
		string? output = null;
		string? connectionString = null;
		string? zoneTemplate = null;
		var execute = false;
		var useBaseline = true;

		for (var i = 1; i < args.Length; i++)
		{
			switch (args[i])
			{
				case "--root":
					root = ReadOptionValue(args, ref i, "--root");
					break;
				case "--clan-source":
					clanSource = ReadOptionValue(args, ref i, "--clan-source");
					break;
				case "--craft-source":
					craftSource = ReadOptionValue(args, ref i, "--craft-source");
					break;
				case "--output":
					output = ReadOptionValue(args, ref i, "--output");
					break;
				case "--db-connection":
				case "--connection-string":
					connectionString = ReadOptionValue(args, ref i, args[i]);
					break;
				case "--zone-template":
					zoneTemplate = ReadOptionValue(args, ref i, "--zone-template");
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

		if ((command == "apply-items" || command == "apply-clans" || command == "apply-crafts" || command == "apply-rooms" || command == "apply-npcs") && !useBaseline)
		{
			throw new ArgumentException($"{command} requires a seeded FutureMUD baseline and cannot be run with --skip-baseline.");
		}

		var regionsDirectory = ResolveRegionsDirectory(root);

		return new ConverterCliOptions(
			command,
			regionsDirectory,
			ResolveClanSourceFile(clanSource),
			ResolveCraftSourceFile(craftSource, regionsDirectory),
			output,
			connectionString,
			zoneTemplate,
			execute,
			useBaseline);
	}

	private static bool IsItemCommand(string command)
	{
		return command is "analyze-items" or "export-items" or "apply-items";
	}

	private static bool IsClanCommand(string command)
	{
		return command is "analyze-clans" or "export-clans" or "apply-clans";
	}

	private static bool IsCraftCommand(string command)
	{
		return command is "analyze-crafts" or "export-crafts" or "apply-crafts";
	}

	private static bool IsRoomCommand(string command)
	{
		return command is "analyze-rooms" or "export-rooms" or "apply-rooms";
	}

	private static bool IsNpcCommand(string command)
	{
		return command is "analyze-npcs" or "export-npcs" or "apply-npcs";
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
			Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "RPI Engine Worldfile Converter", "soiregions-main"),
		}
			.Select(Path.GetFullPath)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		return candidates.FirstOrDefault(Directory.Exists) ?? candidates[0];
	}

	private static string ResolveClanSourceFile(string? configuredSource)
	{
		if (!string.IsNullOrWhiteSpace(configuredSource))
		{
			return Path.GetFullPath(configuredSource);
		}

		var candidates = new[]
		{
			Path.Combine(Directory.GetCurrentDirectory(), "Old SOI Code", "src", "clan.cpp"),
			Path.Combine(Directory.GetCurrentDirectory(), "RPI Engine Worldfile Converter", "Old SOI Code", "src", "clan.cpp"),
			Path.Combine(AppContext.BaseDirectory, "Old SOI Code", "src", "clan.cpp"),
			Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Old SOI Code", "src", "clan.cpp"),
			Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "RPI Engine Worldfile Converter", "Old SOI Code", "src", "clan.cpp"),
		}
			.Select(Path.GetFullPath)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		return candidates.FirstOrDefault(File.Exists) ?? candidates[0];
	}

	private static string ResolveCraftSourceFile(string? configuredSource, string regionsDirectory)
	{
		if (!string.IsNullOrWhiteSpace(configuredSource))
		{
			return Path.GetFullPath(configuredSource);
		}

		var candidates = new[]
		{
			Path.Combine(regionsDirectory, "crafts.txt"),
			Path.Combine(Directory.GetCurrentDirectory(), "crafts.txt"),
			Path.Combine(Directory.GetCurrentDirectory(), "soiregions-main", "crafts.txt"),
			Path.Combine(Directory.GetCurrentDirectory(), "RPI Engine Worldfile Converter", "soiregions-main", "crafts.txt"),
			Path.Combine(AppContext.BaseDirectory, "crafts.txt"),
			Path.Combine(AppContext.BaseDirectory, "soiregions-main", "crafts.txt"),
			Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "crafts.txt"),
			Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "soiregions-main", "crafts.txt"),
			Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "RPI Engine Worldfile Converter", "soiregions-main", "crafts.txt"),
		}
			.Select(Path.GetFullPath)
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		return candidates.FirstOrDefault(File.Exists) ?? candidates[0];
	}

	private static string ResolveOutputPath(string? configuredOutput, string fallbackFileName)
	{
		return Path.GetFullPath(configuredOutput ?? Path.Combine(Directory.GetCurrentDirectory(), fallbackFileName));
	}

	private static string ResolveSidecarAuditPath(string outputPath)
	{
		var directory = Path.GetDirectoryName(outputPath) ?? Directory.GetCurrentDirectory();
		var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(outputPath);
		return Path.Combine(directory, $"{fileNameWithoutExtension}.audit.json");
	}

	private static void PrintUsage()
	{
		Console.WriteLine("RPI Engine Worldfile Converter");
		Console.WriteLine();
		Console.WriteLine("Commands:");
		Console.WriteLine("  analyze-items [--root <regions-dir>] [--db-connection <connection-string>] [--skip-baseline]");
		Console.WriteLine("  export-items [--root <regions-dir>] [--output <json-path>] [--db-connection <connection-string>] [--skip-baseline]");
		Console.WriteLine("  apply-items [--root <regions-dir>] [--db-connection <connection-string>] [--execute]");
		Console.WriteLine("  analyze-clans [--root <regions-dir>] [--clan-source <clan.cpp>] [--db-connection <connection-string>] [--skip-baseline]");
		Console.WriteLine("  export-clans [--root <regions-dir>] [--clan-source <clan.cpp>] [--output <json-path>] [--db-connection <connection-string>] [--skip-baseline]");
		Console.WriteLine("  apply-clans [--root <regions-dir>] [--clan-source <clan.cpp>] [--db-connection <connection-string>] [--execute]");
		Console.WriteLine("  analyze-crafts [--root <regions-dir>] [--craft-source <crafts.txt>] [--db-connection <connection-string>] [--skip-baseline]");
		Console.WriteLine("  export-crafts [--root <regions-dir>] [--craft-source <crafts.txt>] [--output <json-path>] [--db-connection <connection-string>] [--skip-baseline]");
		Console.WriteLine("  apply-crafts [--root <regions-dir>] [--craft-source <crafts.txt>] [--output <audit-json>] [--db-connection <connection-string>] [--execute]");
		Console.WriteLine("  analyze-rooms [--root <regions-dir>] [--db-connection <connection-string>] [--skip-baseline]");
		Console.WriteLine("  export-rooms [--root <regions-dir>] [--output <json-path>] [--db-connection <connection-string>] [--skip-baseline]");
		Console.WriteLine("  apply-rooms [--root <regions-dir>] [--output <audit-json>] [--zone-template <zone>] [--db-connection <connection-string>] [--execute]");
		Console.WriteLine("  analyze-npcs [--root <regions-dir>] [--db-connection <connection-string>] [--skip-baseline]");
		Console.WriteLine("  export-npcs [--root <regions-dir>] [--output <json-path>] [--db-connection <connection-string>] [--skip-baseline]");
		Console.WriteLine("  apply-npcs [--root <regions-dir>] [--output <audit-json>] [--db-connection <connection-string>] [--execute]");
		Console.WriteLine();
		Console.WriteLine("Notes:");
		Console.WriteLine("  apply-items, apply-clans, apply-crafts, apply-rooms, and apply-npcs default to dry-run mode unless --execute is supplied.");
		Console.WriteLine("  Recommended execute order: apply-clans, apply-items, apply-rooms, apply-crafts, then apply-npcs.");
		Console.WriteLine("  The default regions directory is the bundled soiregions-main corpus.");
		Console.WriteLine("  The default clan source is the bundled Old SOI Code/src/clan.cpp file.");
		Console.WriteLine("  The default craft source is the bundled soiregions-main/crafts.txt file.");
		Console.WriteLine("  apply-rooms recommends --zone-template so new zones inherit seeded shard/timezone/weather defaults.");
	}
}

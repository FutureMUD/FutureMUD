#nullable enable

using System.IO;

namespace RPI_Engine_Worldfile_Converter;

public enum RpiCraftItemRole
{
	Unknown,
	InRoom,
	Give,
	Held,
	Wielded,
	Used,
	Produced,
	Worn,
}

public enum RpiCraftCheckKind
{
	Skill,
	Attribute,
}

public sealed record RpiCraftParseWarning(string Code, string Message, int? LineNumber = null);

public sealed record RpiCraftBlockFailure(string SourceFile, string Header, string Message, int? LineNumber = null);

public sealed record RpiCraftClanRequirement(string RankName, string ClanAlias);

public sealed record RpiCraftCheckRecord(
	RpiCraftCheckKind Kind,
	string SourceName,
	int Dice,
	int Sides,
	int PhaseNumber,
	string RawExpression);

public sealed record RpiCraftCostRecord(int Moves, int Hits);

public sealed record RpiCraftEchoSet(
	string? First,
	string? Second,
	string? Third,
	string? Self,
	string? Group,
	string? FirstFail,
	string? SecondFail,
	string? ThirdFail,
	string? GroupFail);

public sealed record RpiCraftItemList(
	int Slot,
	RpiCraftItemRole Role,
	int Quantity,
	IReadOnlyList<int> Vnums,
	string RawDefinition);

public sealed record RpiCraftVariantLink(int? StartKey, int? EndKey);

public sealed record RpiCraftConstraintSet(
	IReadOnlyList<string> Flags,
	IReadOnlyList<string> Sectors,
	IReadOnlyList<string> Seasons,
	IReadOnlyList<string> WeatherStates,
	IReadOnlyList<int> OpeningSkillIds,
	IReadOnlyList<int> RaceIds,
	IReadOnlyList<RpiCraftClanRequirement> ClanRequirements,
	int FollowersRequired,
	int IcDelay,
	int FailDelay);

public sealed record RpiCraftPhaseRecord
{
	public required int PhaseNumber { get; init; }
	public required int Seconds { get; init; }
	public required RpiCraftEchoSet Echoes { get; init; }
	public RpiCraftCheckRecord? SkillCheck { get; init; }
	public RpiCraftCheckRecord? AttributeCheck { get; init; }
	public RpiCraftCostRecord? Cost { get; init; }
	public IReadOnlyList<RpiCraftItemList> ItemLists { get; init; } = Array.Empty<RpiCraftItemList>();
	public IReadOnlyList<RpiCraftItemList> FailItemLists { get; init; } = Array.Empty<RpiCraftItemList>();
	public RpiCraftItemList? ToolList { get; init; }
	public int? LoadMobVnum { get; init; }
	public IReadOnlyList<string> MobileFlags { get; init; } = Array.Empty<string>();
	public string? SpellDefinition { get; init; }
	public string? OpenDefinition { get; init; }
	public string? RequirementDefinition { get; init; }
	public IReadOnlyList<string> RawLines { get; init; } = Array.Empty<string>();
}

public sealed record RpiCraftDefinition
{
	public required string SourceFile { get; init; }
	public required int SourceLineNumber { get; init; }
	public required int CraftNumber { get; init; }
	public required string CraftName { get; init; }
	public required string SubcraftName { get; init; }
	public required string Command { get; init; }
	public string? FailureMessage { get; init; }
	public IReadOnlyList<int> FailObjectVnums { get; init; } = Array.Empty<int>();
	public IReadOnlyList<int> FailMobVnums { get; init; } = Array.Empty<int>();
	public required RpiCraftConstraintSet Constraints { get; init; }
	public required RpiCraftVariantLink VariantLink { get; init; }
	public IReadOnlyList<RpiCraftPhaseRecord> Phases { get; init; } = Array.Empty<RpiCraftPhaseRecord>();
	public IReadOnlyList<string> RawLines { get; init; } = Array.Empty<string>();
	public IReadOnlyList<RpiCraftParseWarning> ParseWarnings { get; init; } = Array.Empty<RpiCraftParseWarning>();

	public string SourceKey => $"{CraftName}/{SubcraftName}/{Command}#{CraftNumber}";
	public string SourceHeader => $"craft {CraftName} subcraft {SubcraftName} command {Command}";
	public override string ToString() => $"{Path.GetFileName(SourceFile)}:{SourceLineNumber} {SourceHeader}";
}

public sealed record RpiCraftCorpus(
	IReadOnlyList<RpiCraftDefinition> Crafts,
	IReadOnlyList<RpiCraftBlockFailure> Failures);

public sealed record CraftAnalysisSummary(
	int TotalCraftCount,
	int ParsedCraftCount,
	int FailureCount,
	int ParseWarningCount,
	int TotalPhaseCount,
	int KeyedCraftCount,
	int MultiCheckCount,
	int GeneratedTagCount,
	int DeferredCraftCount,
	string BaselineStatus,
	IReadOnlyDictionary<string, int> PerStatusCounts,
	IReadOnlyDictionary<string, int> FeatureCounts,
	IReadOnlyDictionary<string, int> WarningCodeCounts,
	IReadOnlyDictionary<string, int> MissingDependencyCounts);

public sealed record ConverterExportCraft(RpiCraftDefinition Source, ConvertedCraftDefinition Converted);

public sealed record CraftExportAuditEntry(
	string SourceKey,
	int CraftNumber,
	CraftConversionStatus Status,
	IReadOnlyList<string> GeneratedTags,
	IReadOnlyList<string> FutureProgNames,
	IReadOnlyList<string> WarningCodes);

public sealed record CraftExportAuditReport(
	DateTime GeneratedUtc,
	IReadOnlyList<CraftExportAuditEntry> Crafts,
	IReadOnlyList<GeneratedCraftTagDefinition> GeneratedTags);

public sealed record CraftExportReport(
	DateTime GeneratedUtc,
	string SourceFile,
	string RegionsDirectory,
	CraftAnalysisSummary Analysis,
	IReadOnlyList<RpiCraftBlockFailure> Failures,
	IReadOnlyList<FutureMudCraftValidationIssue> ValidationIssues,
	IReadOnlyList<ConverterExportCraft> Crafts);

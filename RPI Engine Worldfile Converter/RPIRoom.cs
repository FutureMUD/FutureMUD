#nullable enable

using System.Globalization;
using System.IO;

namespace RPI_Engine_Worldfile_Converter;

[Flags]
public enum RpiRoomFlags : uint
{
	None = 0,
	Dark = 1u << 0,
	NoMob = 1u << 2,
	Indoors = 1u << 3,
	SafeQuit = 1u << 8,
	Fort = 1u << 9,
	Fall = 1u << 10,
	Climb = 1u << 15,
	Temporary = 1u << 22,
	Open = 1u << 23,
	Rocky = 1u << 24,
	Vegetated = 1u << 25,
	Light = 1u << 26,
	NoHide = 1u << 27,
}

public enum RpiRoomSectorType
{
	Inside = 0,
	City = 1,
	Road = 2,
	Trail = 3,
	Field = 4,
	Woods = 5,
	Forest = 6,
	Hills = 7,
	Mountain = 8,
	Swamp = 9,
	Dock = 10,
	CrowsNest = 11,
	Pasture = 12,
	Heath = 13,
	Pit = 14,
	LeanTo = 15,
	Lake = 16,
	River = 17,
	Ocean = 18,
	Reef = 19,
	Underwater = 20,
}

public enum RpiRoomDirection
{
	North = 0,
	East = 1,
	South = 2,
	West = 3,
	Up = 4,
	Down = 5,
}

public enum RpiRoomExitSectionType
{
	Normal,
	Hidden,
	Trapped,
	HiddenTrapped,
}

public enum RpiRoomDoorType
{
	None = 0,
	Door = 1,
	PickproofDoor = 2,
	Gate = 3,
}

public sealed record RpiRoomParseWarning(string Code, string Message, int? LineNumber = null);

public sealed record RpiRoomBlockFailure(string SourceFile, int Zone, string? Header, string Message);

public sealed record RpiRoomExtraDescriptionRecord(string Keyword, string Description);

public sealed record RpiRoomWrittenDescriptionRecord(int Language, string Description);

public sealed record RpiRoomProgRecord(string Command, string Keys, string ProgText);

public sealed record RpiRoomWeatherRecord(
	IReadOnlyList<string?> WeatherDescriptions,
	IReadOnlyList<string?> AlasDescriptions);

public sealed record RpiRoomSecretRecord(
	RpiRoomDirection Direction,
	int Difficulty,
	string SearchText);

public sealed record RpiRoomExitRecord(
	RpiRoomDirection Direction,
	RpiRoomExitSectionType SectionType,
	string GeneralDescription,
	string Keyword,
	RpiRoomDoorType DoorType,
	int KeyVnum,
	int PickPenalty,
	int DestinationVnum)
{
	public bool IsHidden => SectionType is RpiRoomExitSectionType.Hidden or RpiRoomExitSectionType.HiddenTrapped;
	public bool IsTrapped => SectionType is RpiRoomExitSectionType.Trapped or RpiRoomExitSectionType.HiddenTrapped;
	public bool AcceptsDoor => DoorType != RpiRoomDoorType.None;
	public bool IsPickProof => DoorType == RpiRoomDoorType.PickproofDoor;
}

public sealed record RpiRoomRecord
{
	public required int Vnum { get; init; }
	public required string SourceFile { get; init; }
	public required int Zone { get; init; }
	public required string Name { get; init; }
	public required string Description { get; init; }
	public required uint RawFlags { get; init; }
	public required RpiRoomFlags RoomFlags { get; init; }
	public required int RawSectorType { get; init; }
	public required RpiRoomSectorType SectorType { get; init; }
	public required int Deity { get; init; }
	public int? XeroxSourceVnum { get; init; }
	public int? Capacity { get; init; }
	public IReadOnlyList<RpiRoomExitRecord> Exits { get; init; } = Array.Empty<RpiRoomExitRecord>();
	public IReadOnlyList<RpiRoomSecretRecord> Secrets { get; init; } = Array.Empty<RpiRoomSecretRecord>();
	public IReadOnlyList<RpiRoomExtraDescriptionRecord> ExtraDescriptions { get; init; } = Array.Empty<RpiRoomExtraDescriptionRecord>();
	public IReadOnlyList<RpiRoomWrittenDescriptionRecord> WrittenDescriptions { get; init; } = Array.Empty<RpiRoomWrittenDescriptionRecord>();
	public IReadOnlyList<RpiRoomProgRecord> RoomProgs { get; init; } = Array.Empty<RpiRoomProgRecord>();
	public RpiRoomWeatherRecord? Weather { get; init; }
	public IReadOnlyList<string> LegacyTrailerLines { get; init; } = Array.Empty<string>();
	public IReadOnlyList<RpiRoomParseWarning> ParseWarnings { get; init; } = Array.Empty<RpiRoomParseWarning>();

	public string SourceKey => $"{Path.GetFileName(SourceFile)}#{Vnum}";

	public override string ToString()
	{
		return $"{SourceKey} {Name}";
	}
}

public sealed record RpiParsedRoomCorpus(
	IReadOnlyList<RpiRoomRecord> Rooms,
	IReadOnlyList<RpiRoomBlockFailure> Failures);

public sealed record RoomCoordinate(int X, int Y, int Z);

public sealed record RoomConversionWarning(string Code, string Message);

public sealed record RpiZoneGroupRule(
	string GroupKey,
	string? DisplayNameOverride,
	IReadOnlyList<int> SourceZones,
	string Evidence);

public sealed record ConvertedRoomExitSideDefinition(
	int RoomVnum,
	RpiRoomDirection Direction,
	bool Visible,
	string Keywords,
	string? PrimaryKeyword,
	string Description,
	bool Hidden,
	bool Trapped,
	int? SearchDifficulty,
	string? SearchText,
	int LegacyKeyVnum,
	int LegacyPickPenalty,
	bool IsPickProof,
	RpiRoomDoorType DoorType);

public sealed record ConvertedRoomExitDefinition(
	string ExitKey,
	int RoomVnum1,
	int RoomVnum2,
	ConvertedRoomExitSideDefinition Side1,
	ConvertedRoomExitSideDefinition Side2,
	bool AcceptsDoor,
	int? DoorSize,
	int MaximumSizeToEnter,
	int MaximumSizeToEnterUpright,
	bool IsClimbExit,
	int ClimbDifficulty,
	int? FallFromRoomVnum,
	int? FallToRoomVnum,
	IReadOnlyList<RoomConversionWarning> Warnings);

public sealed record ConvertedRoomDefinition
{
	public required int Vnum { get; init; }
	public required string SourceFile { get; init; }
	public required int SourceZone { get; init; }
	public required string SourceKey { get; init; }
	public required string ZoneGroupKey { get; init; }
	public required string ZoneName { get; init; }
	public required string OverlayPackageName { get; init; }
	public required string Name { get; init; }
	public required string RawDescription { get; init; }
	public required string EffectiveDescription { get; init; }
	public required string TerrainName { get; init; }
	public required string OutdoorsTypeName { get; init; }
	public required int OutdoorsTypeValue { get; init; }
	public required bool SafeQuit { get; init; }
	public required RoomCoordinate Coordinates { get; init; }
	public required uint RawFlags { get; init; }
	public required IReadOnlyList<string> RoomFlagNames { get; init; }
	public required RpiRoomSectorType SectorType { get; init; }
	public required int Deity { get; init; }
	public int? XeroxSourceVnum { get; init; }
	public bool XeroxResolved { get; init; }
	public int? Capacity { get; init; }
	public RpiRoomWeatherRecord? EffectiveWeather { get; init; }
	public IReadOnlyList<RpiRoomExtraDescriptionRecord> ExtraDescriptions { get; init; } = Array.Empty<RpiRoomExtraDescriptionRecord>();
	public IReadOnlyList<RpiRoomWrittenDescriptionRecord> WrittenDescriptions { get; init; } = Array.Empty<RpiRoomWrittenDescriptionRecord>();
	public IReadOnlyList<RpiRoomProgRecord> RoomProgs { get; init; } = Array.Empty<RpiRoomProgRecord>();
	public IReadOnlyList<RpiRoomSecretRecord> Secrets { get; init; } = Array.Empty<RpiRoomSecretRecord>();
	public IReadOnlyList<RoomConversionWarning> Warnings { get; init; } = Array.Empty<RoomConversionWarning>();
}

public sealed record ConvertedZoneDefinition(
	string GroupKey,
	string ZoneName,
	string OverlayPackageName,
	IReadOnlyList<int> SourceZones,
	string Evidence,
	IReadOnlyList<ConvertedRoomDefinition> Rooms,
	IReadOnlyList<RoomConversionWarning> Warnings);

public sealed record RoomConversionResult(
	IReadOnlyList<ConvertedZoneDefinition> Zones,
	IReadOnlyList<ConvertedRoomDefinition> Rooms,
	IReadOnlyList<ConvertedRoomExitDefinition> Exits);

public sealed record RoomAnalysisSummary(
	int TotalBlockCount,
	int ParsedRoomCount,
	int FailureCount,
	int ParseWarningCount,
	string BaselineStatus,
	int XeroxResolvedCount,
	int HiddenExitCount,
	int TrappedExitCount,
	int ClimbExitCount,
	int FallExitCount,
	IReadOnlyDictionary<string, int> PerSectorCounts,
	IReadOnlyDictionary<string, int> PerTerrainCounts,
	IReadOnlyDictionary<string, int> PerZoneCounts,
	IReadOnlyDictionary<string, int> WarningCodeCounts,
	IReadOnlyDictionary<string, int> MissingDependencyCounts);

public sealed record ConverterExportRoom(RpiRoomRecord Source, ConvertedRoomDefinition Converted);

public sealed record RoomLogicalAuditEntry(
	string SourceKey,
	int Vnum,
	string ZoneGroupKey,
	string ZoneName,
	string OverlayPackageName,
	RoomCoordinate Coordinates,
	IReadOnlyList<string> ExitKeys);

public sealed record RoomExportAuditReport(
	DateTime GeneratedUtc,
	IReadOnlyList<RoomLogicalAuditEntry> Rooms,
	IReadOnlyList<string> ExitKeys);

public sealed record RoomExportReport(
	DateTime GeneratedUtc,
	string SourceDirectory,
	RoomAnalysisSummary Analysis,
	IReadOnlyList<RpiRoomBlockFailure> Failures,
	IReadOnlyList<FutureMudRoomValidationIssue> ValidationIssues,
	IReadOnlyList<ConvertedZoneDefinition> Zones,
	IReadOnlyList<ConverterExportRoom> Rooms);

public static class RpiRoomDirections
{
	private static readonly IReadOnlyDictionary<RpiRoomDirection, RoomCoordinate> Deltas =
		new Dictionary<RpiRoomDirection, RoomCoordinate>
		{
			[RpiRoomDirection.North] = new(0, 1, 0),
			[RpiRoomDirection.East] = new(1, 0, 0),
			[RpiRoomDirection.South] = new(0, -1, 0),
			[RpiRoomDirection.West] = new(-1, 0, 0),
			[RpiRoomDirection.Up] = new(0, 0, 1),
			[RpiRoomDirection.Down] = new(0, 0, -1),
		};

	private static readonly IReadOnlyList<RpiRoomDirection> OrderedDirections =
	[
		RpiRoomDirection.North,
		RpiRoomDirection.East,
		RpiRoomDirection.South,
		RpiRoomDirection.West,
		RpiRoomDirection.Up,
		RpiRoomDirection.Down,
	];

	public static RoomCoordinate Delta(this RpiRoomDirection direction)
	{
		return Deltas[direction];
	}

	public static RpiRoomDirection Opposite(this RpiRoomDirection direction)
	{
		return direction switch
		{
			RpiRoomDirection.North => RpiRoomDirection.South,
			RpiRoomDirection.East => RpiRoomDirection.West,
			RpiRoomDirection.South => RpiRoomDirection.North,
			RpiRoomDirection.West => RpiRoomDirection.East,
			RpiRoomDirection.Up => RpiRoomDirection.Down,
			RpiRoomDirection.Down => RpiRoomDirection.Up,
			_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
		};
	}

	public static int SortOrder(this RpiRoomDirection direction)
	{
		for (var i = 0; i < OrderedDirections.Count; i++)
		{
			if (OrderedDirections[i] == direction)
			{
				return i;
			}
		}

		return int.MaxValue;
	}

	public static string Describe(this RpiRoomDirection direction)
	{
		return direction.ToString();
	}

	public static bool TryParse(string? text, out RpiRoomDirection direction)
	{
		direction = RpiRoomDirection.North;
		if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
		{
			return false;
		}

		if (!Enum.IsDefined(typeof(RpiRoomDirection), value))
		{
			return false;
		}

		direction = (RpiRoomDirection)value;
		return true;
	}
}

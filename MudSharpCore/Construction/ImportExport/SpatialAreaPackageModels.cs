#nullable enable

namespace MudSharp.Construction.ImportExport;

public sealed class SpatialAreaPackage
{
	public const string CurrentFormat = "futuremud-spatial-area";
	public const int CurrentVersion = 1;

	public string Format { get; set; } = CurrentFormat;
	public int Version { get; set; } = CurrentVersion;
	public string IntegritySha256 { get; set; } = string.Empty;
	public DateTime CreatedUtc { get; set; }
	public SpatialAreaPackageSource Source { get; set; } = new();
	public SpatialZoneDefinition Zone { get; set; } = new();
	public List<SpatialRoomDefinition> Rooms { get; set; } = [];
	public List<SpatialCellDefinition> Cells { get; set; } = [];
	public List<SpatialExitDefinition> Exits { get; set; } = [];
}

public sealed class SpatialAreaPackageSource
{
	public string ZoneName { get; set; } = string.Empty;
	public long ZoneId { get; set; }
	public string ShardName { get; set; } = string.Empty;
	public long ShardId { get; set; }
	public string OverlayPackageName { get; set; } = string.Empty;
	public long OverlayPackageId { get; set; }
	public int OverlayPackageRevision { get; set; }
}

public sealed class SpatialZoneDefinition
{
	public string Name { get; set; } = string.Empty;
	public double LatitudeRadians { get; set; }
	public double LongitudeRadians { get; set; }
	public double ElevationMetres { get; set; }
	public double AmbientLightPollution { get; set; }
	public SpatialNamedReference? ForagableProfile { get; set; }
	public SpatialNamedReference? WeatherController { get; set; }
	public string DefaultCellKey { get; set; } = string.Empty;
	public List<SpatialTimeZoneDefinition> TimeZones { get; set; } = [];
}

public sealed class SpatialTimeZoneDefinition
{
	public string ClockAlias { get; set; } = string.Empty;
	public string TimeZoneAlias { get; set; } = string.Empty;
	public string TimeZoneDescription { get; set; } = string.Empty;
}

public sealed class SpatialRoomDefinition
{
	public string Key { get; set; } = string.Empty;
	public long SourceId { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
}

public sealed class SpatialCellDefinition
{
	public string Key { get; set; } = string.Empty;
	public long SourceId { get; set; }
	public string RoomKey { get; set; } = string.Empty;
	public SpatialCellOverlayDefinition Overlay { get; set; } = new();
	public SpatialNamedReference? ForagableProfile { get; set; }
	public List<SpatialNamedReference> Tags { get; set; } = [];
	public List<SpatialNamedReference> RangedCovers { get; set; } = [];
	public List<SpatialMagicResourceDefinition> MagicResources { get; set; } = [];
}

public sealed class SpatialCellOverlayDefinition
{
	public string CellName { get; set; } = string.Empty;
	public string CellDescription { get; set; } = string.Empty;
	public SpatialNamedReference Terrain { get; set; } = new();
	public SpatialNamedReference? HearingProfile { get; set; }
	public SpatialFluidReference? Atmosphere { get; set; }
	public int OutdoorsType { get; set; }
	public double AmbientLightFactor { get; set; }
	public double AddedLight { get; set; }
	public bool SafeQuit { get; set; }
	public List<string> ExitKeys { get; set; } = [];
}

public sealed class SpatialMagicResourceDefinition
{
	public SpatialNamedReference Resource { get; set; } = new();
	public double Amount { get; set; }
}

public class SpatialNamedReference
{
	public long SourceId { get; set; }
	public string Name { get; set; } = string.Empty;
}

public sealed class SpatialFluidReference : SpatialNamedReference
{
	public string Kind { get; set; } = string.Empty;
}

public sealed class SpatialExitDefinition
{
	public string Key { get; set; } = string.Empty;
	public long SourceId { get; set; }
	public string Cell1Key { get; set; } = string.Empty;
	public string Cell2Key { get; set; } = string.Empty;
	public SpatialExitSideDefinition Side1 { get; set; } = new();
	public SpatialExitSideDefinition Side2 { get; set; } = new();
	public double TimeMultiplier { get; set; }
	public bool AcceptsDoor { get; set; }
	public int DoorSize { get; set; }
	public int MaximumSizeToEnter { get; set; }
	public int MaximumSizeToEnterUpright { get; set; }
	public bool IsClimbExit { get; set; }
	public int ClimbDifficulty { get; set; }
	public string? FallCellKey { get; set; }
	public List<int> BlockedLayers { get; set; } = [];
}

public sealed class SpatialExitSideDefinition
{
	public int Direction { get; set; }
	public string? Verb { get; set; }
	public string? PrimaryKeyword { get; set; }
	public string? Keywords { get; set; }
	public string? InboundDescription { get; set; }
	public string? InboundTarget { get; set; }
	public string? OutboundDescription { get; set; }
	public string? OutboundTarget { get; set; }
}

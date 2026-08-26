using System.Text.Json.Serialization;

namespace TerrainPlanner.Contracts;

public sealed class PlannerProject
{
	public const int CurrentSchemaVersion = 1;

	public int SchemaVersion { get; init; } = CurrentSchemaVersion;
	public string Name { get; set; } = "Untitled terrain plan";
	public int Width { get; set; } = 5;
	public int Height { get; set; } = 5;
	public string? CatalogueRevision { get; set; }
	public List<PlannerProjectCell> Cells { get; set; } = [];
	public Dictionary<long, string> TagColours { get; set; } = [];

	[JsonIgnore]
	public int CellCount => checked(Width * Height);
}

public sealed class PlannerProjectCell
{
	public int X { get; set; }
	public int Y { get; set; }
	public long TerrainId { get; set; }
	public List<PlannerTagReference> Tags { get; set; } = [];
	public List<string> UnresolvedFeatures { get; set; } = [];
}

public sealed record PlannerTagReference(long Id, string Name);

public readonly record struct GridCoordinate(int X, int Y);

public enum PlannerLayer
{
	Terrain,
	Tags
}

public enum PlannerTool
{
	Paint,
	Fill,
	Rectangle,
	Eyedropper,
	Erase
}

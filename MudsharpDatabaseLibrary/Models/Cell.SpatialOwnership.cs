#nullable enable

namespace MudSharp.Models;

public partial class Cell
{
	// Nullable during expansion. Room remains authoritative until the maintenance cutover.
	public long? ZoneId { get; set; }
	public int? X { get; set; }
	public int? Y { get; set; }
	public int? Z { get; set; }
}

public class AreasCells
{
	public long AreaId { get; set; }
	public long CellId { get; set; }
	public virtual Areas Area { get; set; } = null!;
	public virtual Cell Cell { get; set; } = null!;
}

/// <summary>Historical provenance, deliberately independent of live entity deletion.</summary>
public class CellRoomMigrationLedger
{
	public long RoomId { get; set; }
	public long? CellId { get; set; }
	public long ZoneId { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
	public string? Warning { get; set; }
}

public class CellRoomAreaMigrationLedger
{
	public long AreaId { get; set; }
	public long RoomId { get; set; }
	public long CellId { get; set; }
}

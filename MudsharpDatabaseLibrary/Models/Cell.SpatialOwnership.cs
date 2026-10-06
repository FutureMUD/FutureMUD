using System.Collections.Generic;

#nullable enable

namespace MudSharp.Models;

public partial class Cell
{
	// Direct stored ownership; hosted environmental projection is runtime-only.
	public long ZoneId { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
	public virtual Zone Zone { get; set; } = null!;
	public virtual ICollection<AreasCells> AreasCells { get; set; } = new List<AreasCells>();
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

/// <summary>Final authoritative Room snapshot taken under the contraction writer freeze.</summary>
public class CellRoomContractionLedger
{
	public long RoomId { get; set; }
	public long? CellId { get; set; }
	public long ZoneId { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
	public string? Warning { get; set; }
}

public class CellRoomAreaContractionLedger
{
	public long AreaId { get; set; }
	public long RoomId { get; set; }
	public long CellId { get; set; }
}

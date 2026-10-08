using System.Collections.Generic;

#nullable enable

namespace MudSharp.Models;

public partial class Room
{
	// Direct stored ownership; hosted environmental projection is runtime-only.
	public long ZoneId { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
	public virtual Zone Zone { get; set; } = null!;
	public virtual ICollection<AreasRooms> AreasRooms { get; set; } = new List<AreasRooms>();
}

public class AreasRooms
{
	public long AreaId { get; set; }
	public long RoomId { get; set; }
	public virtual Areas Area { get; set; } = null!;
	public virtual Room Room { get; set; } = null!;
}

/// <summary>Historical provenance, deliberately independent of live entity deletion.</summary>
public class RoomSpatialMigrationLedger
{
	public long LegacyRoomId { get; set; }
	public long? RoomId { get; set; }
	public long ZoneId { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
	public string? Warning { get; set; }
}

public class RoomSpatialAreaMigrationLedger
{
	public long AreaId { get; set; }
	public long LegacyRoomId { get; set; }
	public long? RoomId { get; set; }
}

/// <summary>Final authoritative Room snapshot taken under the contraction writer freeze.</summary>
public class RoomSpatialContractionLedger
{
	public long LegacyRoomId { get; set; }
	public long? RoomId { get; set; }
	public long ZoneId { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
	public int Z { get; set; }
	public string? Warning { get; set; }
}

public class RoomSpatialAreaContractionLedger
{
	public long AreaId { get; set; }
	public long LegacyRoomId { get; set; }
	public long? RoomId { get; set; }
}

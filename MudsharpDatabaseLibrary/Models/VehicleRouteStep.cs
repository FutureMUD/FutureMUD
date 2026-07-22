#nullable enable

namespace MudSharp.Models;

public class VehicleRouteStep
{
	public long Id { get; set; }
	public long VehicleRouteLegId { get; set; }
	public int Sequence { get; set; }
	public int StepType { get; set; }
	public long OriginCellId { get; set; }
	public int OriginRoomLayer { get; set; }
	public decimal? OriginRoutePositionMetres { get; set; }
	public long DestinationCellId { get; set; }
	public int DestinationRoomLayer { get; set; }
	public decimal? DestinationRoutePositionMetres { get; set; }
	public decimal? DistanceMetres { get; set; }
	public decimal RoomEquivalentCost { get; set; }
	public int? Direction { get; set; }
	public long? PinnedTopologyVersion { get; set; }
	public long? DestinationTopologyVersion { get; set; }
	public long? ExitId { get; set; }

	public virtual VehicleRouteLeg VehicleRouteLeg { get; set; } = null!;
	public virtual Cell OriginCell { get; set; } = null!;
	public virtual Cell DestinationCell { get; set; } = null!;
	public virtual Exit? Exit { get; set; }
}

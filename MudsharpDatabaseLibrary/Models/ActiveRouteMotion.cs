#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public partial class ActiveRouteMotion
{
	public ActiveRouteMotion()
	{
		ResourceLedger = new HashSet<RouteMotionResourceLedger>();
	}

	public long Id { get; set; }
	public int MoverType { get; set; }
	public long MoverId { get; set; }
	public long RouteCellId { get; set; }
	public int RoomLayer { get; set; }
	public decimal CheckpointPositionMetres { get; set; }
	public decimal TargetMinimumPositionMetres { get; set; }
	public decimal TargetMaximumPositionMetres { get; set; }
	public int Direction { get; set; }
	public decimal SpeedMetresPerSecond { get; set; }
	public long RemainingDurationMilliseconds { get; set; }
	public long TopologyVersion { get; set; }
	public int Status { get; set; }
	public string OperationId { get; set; } = null!;
	public long CheckpointSequence { get; set; }
	public long? SelectedExitId { get; set; }
	public string? StateData { get; set; }
	public DateTime CreatedDateTime { get; set; }
	public DateTime LastCheckpointDateTime { get; set; }

	public virtual RouteCell RouteCell { get; set; } = null!;
	public virtual Exit? SelectedExit { get; set; }
	public virtual ICollection<RouteMotionResourceLedger> ResourceLedger { get; set; }
}

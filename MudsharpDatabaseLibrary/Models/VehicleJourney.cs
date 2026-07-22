#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Models;

public class VehicleJourney
{
	public VehicleJourney()
	{
		Events = new HashSet<VehicleJourneyEvent>();
	}

	public long Id { get; set; }
	public string OperationId { get; set; } = null!;
	public long VehicleServiceId { get; set; }
	public long VehicleRouteId { get; set; }
	public int VehicleRouteRevision { get; set; }
	public long VehicleId { get; set; }
	public int State { get; set; }
	public long? CurrentStopId { get; set; }
	public long? NextStopId { get; set; }
	public string ScheduledDeparture { get; set; } = null!;
	public string ExpectedDeparture { get; set; } = null!;
	public long DelayMilliseconds { get; set; }
	public DateTime LastCheckpointUtc { get; set; }

	public virtual VehicleService VehicleService { get; set; } = null!;
	public virtual VehicleRoute VehicleRoute { get; set; } = null!;
	public virtual Vehicle Vehicle { get; set; } = null!;
	public virtual VehicleRouteStop? CurrentStop { get; set; }
	public virtual VehicleRouteStop? NextStop { get; set; }
	public virtual ICollection<VehicleJourneyEvent> Events { get; set; }
}

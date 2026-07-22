#nullable enable

using System.Collections.Generic;

namespace MudSharp.Models;

public class VehicleService
{
	public VehicleService()
	{
		Journeys = new HashSet<VehicleJourney>();
	}

	public long Id { get; set; }
	public string Name { get; set; } = null!;
	public string Keywords { get; set; } = null!;
	public long VehicleRouteId { get; set; }
	public int VehicleRouteRevision { get; set; }
	public long VehicleId { get; set; }
	public int OperatorMode { get; set; }
	public long RetryIntervalMilliseconds { get; set; }
	public long MaximumHoldMilliseconds { get; set; }
	public bool Enabled { get; set; }

	public virtual VehicleRoute VehicleRoute { get; set; } = null!;
	public virtual Vehicle Vehicle { get; set; } = null!;
	public virtual VehicleServiceSchedule Schedule { get; set; } = null!;
	public virtual ICollection<VehicleJourney> Journeys { get; set; }
}

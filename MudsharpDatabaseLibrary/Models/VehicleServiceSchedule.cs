#nullable enable

namespace MudSharp.Models;

public class VehicleServiceSchedule
{
	public long VehicleServiceId { get; set; }
	public string ReferenceDeparture { get; set; } = null!;
	public string NextDeparture { get; set; } = null!;
	public int RecurrenceType { get; set; }
	public int RecurrenceIntervalAmount { get; set; }
	public int RecurrenceModifier { get; set; }
	public int RecurrenceSecondaryModifier { get; set; }
	public int RecurrenceFallbackMode { get; set; }

	public virtual VehicleService VehicleService { get; set; } = null!;
}

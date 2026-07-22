#nullable enable

using System;

namespace MudSharp.Models;

public class VehicleJourneyEvent
{
	public long Id { get; set; }
	public long VehicleJourneyId { get; set; }
	public long Sequence { get; set; }
	public string IdempotencyKey { get; set; } = null!;
	public int EventType { get; set; }
	public int State { get; set; }
	public DateTime OccurredAtUtc { get; set; }
	public string? WorldTime { get; set; }
	public string Message { get; set; } = null!;

	public virtual VehicleJourney VehicleJourney { get; set; } = null!;
}

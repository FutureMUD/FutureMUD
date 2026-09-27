#nullable enable

using System;

namespace MudSharp.Models;

/// <summary>Authoritative versioned treatment checkpoint, including cancellation and pending-step evidence.</summary>
public class LandRejuvenationTreatment
{
	public Guid Id { get; set; }
	public long CellId { get; set; }
	public long Revision { get; set; }
	public string Status { get; set; } = string.Empty;
	public string Checkpoint { get; set; } = string.Empty;
}

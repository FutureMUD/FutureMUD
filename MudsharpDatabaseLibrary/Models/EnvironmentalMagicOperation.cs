using System;

#nullable enable

namespace MudSharp.Models;

/// <summary>Durable receipt for an explicit damage or repair operation; ordinary regeneration creates no receipt.</summary>
public class EnvironmentalMagicOperation
{
	public Guid Id { get; set; }
	public long CellId { get; set; }
	public string Kind { get; set; } = "";
	public double RequestedDamage { get; set; }
	public double RequestedPressure { get; set; }
	public double RequestedRepair { get; set; }
	public double AppliedDamage { get; set; }
	public double AppliedPressure { get; set; }
	public double AppliedRepair { get; set; }
	public DateTime AtUtc { get; set; }
	public long? ActorId { get; set; }
	public string Attribution { get; set; } = "";
	public string Status { get; set; } = "";
	public string Diagnostic { get; set; } = "";
}

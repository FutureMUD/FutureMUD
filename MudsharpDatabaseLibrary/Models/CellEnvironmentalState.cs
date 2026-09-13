using System;

#nullable enable

namespace MudSharp.Models;

/// <summary>Persistent environmental damage for one physical cell, independent of its resource balances and profile.</summary>
public class CellEnvironmentalState
{
	public long CellId { get; set; }
	public int SchemaVersion { get; set; } = 1;
	public long Revision { get; set; }
	public double ScarDamage { get; set; }
	public DateTime? LastDefileUtc { get; set; }
	public double RecentPressure { get; set; }
	public DateTime? PressureReferenceUtc { get; set; }
	public double PressureHalfLifeSeconds { get; set; } = 3600.0;
	public long? PressureProfileId { get; set; }
	public double PressureDecayAnchor { get; set; }

	public virtual Cell Cell { get; set; } = null!;
}

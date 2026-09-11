using System;
using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Health;

#nullable enable
namespace MudSharp.Magic;

public enum SubstanceLifecycle { Activation, Maintained, Periodic }
public enum SubstancePulseMode { Presence, Timed }
public enum SubstanceStacking { Aggregate, Replace, Strongest, Independent }
public enum SubstanceScaling { Duration, Magnitude }
public enum SubstanceCarrier { Liquid, Gas, Item }

/// <summary>Carrier quantities use engine fluid units, or fractions of one item.</summary>
public sealed record SubstanceBinding(SubstanceCarrier Carrier, long Id, double QuantityPerUnit);

public sealed class SubstanceEffectEntry
{
	public Guid Key { get; init; } = Guid.NewGuid();
	public long SpellId { get; set; }
	public SubstanceLifecycle Lifecycle { get; set; }
	public SubstancePulseMode PulseMode { get; set; }
	public SubstanceStacking Stacking { get; set; }
	public SubstanceScaling Scaling { get; set; }
	public double MinimumDose { get; set; }
	public double MaximumDose { get; set; } = 10.0;
	public double DurationSeconds { get; set; } = 60.0;
	public double MaximumDurationSeconds { get; set; } = 600.0;
	public double IntervalSeconds { get; set; } = 10.0;
}

public interface IMagicalSubstance : ISaveable, IEditableItem
{
	DrugVector Vectors { get; }
	SpellPower Power { get; }
	double ReferenceDose { get; }
	double ClearancePerTick { get; }
	IEnumerable<SubstanceBinding> Bindings { get; }
	IEnumerable<SubstanceEffectEntry> Entries { get; }
	IEnumerable<string> ReadinessErrors { get; }
}

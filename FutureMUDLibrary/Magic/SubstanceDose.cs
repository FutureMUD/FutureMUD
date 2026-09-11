using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Health;

#nullable enable
namespace MudSharp.Magic;

/// <summary>Per-portion provenance. Fractions survive transfer; spent entry identities never reset.</summary>
public sealed class SubstanceCharge
{
	public Guid Lot { get; init; } = Guid.NewGuid();
	public HashSet<Guid> Spent { get; } = new();
	public HashSet<Guid> Suppressed { get; } = new();
	public SubstanceCharge Copy()
	{
		var copy = new SubstanceCharge { Lot = Lot };
		copy.Spent.UnionWith(Spent);
		copy.Suppressed.UnionWith(Suppressed);
		return copy;
	}
	public bool CanMerge(SubstanceCharge other) => Lot == other.Lot &&
		Spent.SetEquals(other.Spent) && Suppressed.SetEquals(other.Suppressed);
	public XElement Save() => new("Charge", new XAttribute("lot", Lot),
		Spent.Select(x => new XElement("Spent", x)), Suppressed.Select(x => new XElement("Suppressed", x)));
	public static SubstanceCharge Load(XElement root)
	{
		var result = new SubstanceCharge { Lot = Guid.Parse(root.Attribute("lot")!.Value) };
		result.Spent.UnionWith(root.Elements("Spent").Select(x => Guid.Parse(x.Value)));
		result.Suppressed.UnionWith(root.Elements("Suppressed").Select(x => Guid.Parse(x.Value)));
		return result;
	}
}

public static class SubstanceDose
{
	public static bool IsPositive(double value) => double.IsFinite(value) && value > 0.0;
	public static double AbsorptionFraction(DrugVector vector) => vector switch
	{
		DrugVector.Ingested => 0.02,
		DrugVector.Inhaled => 0.4,
		DrugVector.Touched => 0.05,
		_ => 0.5
	};
	public static (double Latent, double Active) Advance(double latent, double active, DrugVector vector, double clearance)
	{
		var absorbed = Math.Min(latent, Math.Max(0.0001, latent * AbsorptionFraction(vector)));
		return (Math.Max(0, latent - absorbed), Math.Max(0, active + absorbed - clearance));
	}
}

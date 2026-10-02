using System;

#nullable enable
namespace MudSharp.Magic;

/// <summary>Opt-in source-shaped energy curve. External grades are 1–7, independently of native SpellPower.</summary>
public sealed record ControlledSpellEfficiency(double MinimumCost, double Scale)
{
	public bool IsValid => double.IsFinite(MinimumCost) && MinimumCost is >= 0 and <= 50 &&
		double.IsFinite(Scale) && Scale > 0;

	public double Cost(int controlledGrade, int requestedGrade)
	{
		if (!IsValid) throw new InvalidOperationException("Efficiency requires a minimum from 0 to 50 and a finite positive scale.");
		if (controlledGrade is < 1 or > 7) throw new ArgumentOutOfRangeException(nameof(controlledGrade));
		if (requestedGrade is < 1 or > 7) throw new ArgumentOutOfRangeException(nameof(requestedGrade));
		var cost = requestedGrade == controlledGrade ? 50.0 : requestedGrade > controlledGrade
			? Math.Max(100 - 50 / (requestedGrade - controlledGrade + 1), MinimumCost)
			: Math.Max(50 / (controlledGrade - requestedGrade + 1), MinimumCost);
		var scaled = cost * Scale;
		if (!double.IsFinite(scaled)) throw new InvalidOperationException("Efficiency returned a non-finite cost.");
		return scaled;
	}
}

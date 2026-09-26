using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp.Form.Material;

/// <summary>Pure arithmetic shared by the resolver, diagnostics and executable reference fixtures.</summary>
public static class ExposureArithmetic
{
	public readonly record struct Demand(double Intensity, double DamageRate, double PainRate,
		double StunRate, double ConsumptionRate);
	public readonly record struct Result(double Work, double Damage, double Pain, double Stun, double Consumed);

	public static IReadOnlyList<Result> Resolve(IReadOnlyList<Demand> demands, double seconds, double? available)
	{
		if (!Valid(seconds) || available is { } v && !Valid(v) || demands.Any(x =>
			!Valid(x.Intensity) || !Valid(x.DamageRate) || !Valid(x.PainRate) ||
			!Valid(x.StunRate) || !Valid(x.ConsumptionRate)))
		{
			throw new ArgumentOutOfRangeException(nameof(demands), "Exposure values must be finite and non-negative.");
		}

		var rate = demands.Sum(x => x.Intensity * x.ConsumptionRate);
		if (!Valid(rate)) throw new ArgumentOutOfRangeException(nameof(demands), "Exposure demand overflowed.");
		var active = available == 0.0 ? 0.0 : available is { } amount && rate > 0.0
			? Math.Min(seconds, amount / rate) : seconds;
		var results = demands.Select(x =>
		{
			var work = x.Intensity * active;
			return new Result(work, x.DamageRate * work, x.PainRate * work,
				x.StunRate * work, x.ConsumptionRate * work);
		}).ToArray();
		if (results.Any(x => !Valid(x.Work) || !Valid(x.Damage) || !Valid(x.Pain) || !Valid(x.Stun) || !Valid(x.Consumed)))
			throw new ArgumentOutOfRangeException(nameof(demands), "Exposure work overflowed.");
		return results;
	}

	public static bool Valid(double value) => double.IsFinite(value) && value >= 0.0;

	public static double ComponentIntensity(double componentVolume, double totalVolume, double capacity, double area)
	{
		if (!Valid(componentVolume) || !Valid(totalVolume) || !Valid(capacity) || !Valid(area) || totalVolume <= 0.0)
			return 0.0;
		return area * Math.Clamp(componentVolume / Math.Max(totalVolume, Math.Max(capacity, double.Epsilon)), 0.0, 1.0);
	}

	public static double ThermalRate(double temperature, double? threshold, double slope, double cap)
	{
		if (!double.IsFinite(temperature) || threshold is not { } t || !double.IsFinite(t) ||
			!Valid(slope) || !Valid(cap)) return 0.0;
		return Math.Min(cap, slope * Math.Max(0.0, temperature - t));
	}
}

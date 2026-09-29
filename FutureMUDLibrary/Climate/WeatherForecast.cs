#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.Celestial;

namespace MudSharp.Climate;

/// <summary>A scheduled climate-processing checkpoint, never a scheduled world side effect.</summary>
public sealed record WeatherForecastPoint(long Minute, long EventId, long SeasonId,
	double TemperatureFluctuation, int UnchangedPeriods, int LocalHour, TimeOfDay TimeOfDay);

public interface IWeatherForecastSource
{
	int ForecastHorizonDays { get; }
	long ForecastMinute { get; }
	long ForecastDay { get; }
	IReadOnlyList<WeatherForecastPoint> GetForecast();
	void InvalidateForecast();
}

/// <summary>Version-stable random stream whose complete state can be saved with the weather queue.</summary>
public sealed class WeatherRandom(ulong state) : Random
{
	public ulong State { get; private set; } = state;
	public override double NextDouble()
	{
		unchecked
		{
			var value = State += 0x9E3779B97F4A7C15UL;
			value = (value ^ (value >> 30)) * 0xBF58476D1CE4E5B9UL;
			value = (value ^ (value >> 27)) * 0x94D049BB133111EBUL;
			return ((value ^ (value >> 31)) >> 11) * (1.0 / 9007199254740992.0);
		}
	}
	protected override double Sample() => NextDouble();
	public override int Next(int maxValue) => maxValue < 0
		? throw new ArgumentOutOfRangeException(nameof(maxValue)) : (int)(NextDouble() * maxValue);
}

/// <summary>Weighted selection shared by live climate transitions and headless climate analysis.</summary>
public static class WeatherSelection
{
	public static T Choose<T>(IReadOnlyList<(T Event, double Chance)> options, double total, Func<double> next)
	{
		if (options.Count == 0) throw new ArgumentException("At least one weather choice is required.", nameof(options));
		if (total <= 0.0) return options[Math.Min(options.Count - 1, (int)(next() * options.Count))].Event;
		var roll = next() * total;
		foreach (var option in options)
		{
			if (option.Chance > 0.0 && (roll -= option.Chance) <= 0.0) return option.Event;
		}
		return options[^1].Event;
	}
}

#nullable enable

using MudSharp.TimeAndDate;

namespace MudSharp.Celestial;

public sealed class AstronomicalEventService : IAstronomicalEventService
{
	private const long DefaultStepSeconds = 1800;
	private const long DefaultMaximumSearchSeconds = 86400L * 800L;
	private const long RefinementToleranceSeconds = 1;

	public static AstronomicalEventService Instance { get; } = new();

	private AstronomicalEventService()
	{
	}

	public bool TryFindNext(AstronomicalEventType eventType, MudInstant reference, int occurrence,
		ICelestialEphemeris primary, GeographicCoordinate observer, out MudInstant instant, out string error,
		double targetLongitude = 0.0, ICelestialEphemeris? secondary = null)
	{
		instant = MudInstant.Never;
		error = string.Empty;

		if (reference.IsNever)
		{
			error = "The reference instant was Never.";
			return false;
		}

		if (occurrence < 1)
		{
			error = "The occurrence must be a positive number.";
			return false;
		}

		if (primary is null)
		{
			error = "No primary celestial ephemeris was supplied.";
			return false;
		}

		if (observer is null)
		{
			error = "No observer location was supplied.";
			return false;
		}

		var searchFrom = reference;
		for (var i = 0; i < occurrence; i++)
		{
			if (!TryFindSingle(eventType, searchFrom, primary, observer, secondary, targetLongitude, out instant, out error))
			{
				return false;
			}

			searchFrom = AddSeconds(instant, RefinementToleranceSeconds);
		}

		return true;
	}

	private static bool TryFindSingle(AstronomicalEventType eventType, MudInstant reference,
		ICelestialEphemeris primary, GeographicCoordinate observer, ICelestialEphemeris? secondary,
		double targetLongitude, out MudInstant instant, out string error)
	{
		if (eventType == AstronomicalEventType.VisibleCrescent)
		{
			return TryFindVisibleCrescent(reference, primary, secondary, observer, out instant, out error);
		}

		Func<MudInstant, double> value;
		Func<double, double, bool> isCrossing;
		switch (eventType)
		{
			case AstronomicalEventType.Sunrise:
				value = candidate => primary.ApparentAltitudeAt(candidate, observer);
				isCrossing = (former, current) => former < 0.0 && current >= 0.0;
				break;
			case AstronomicalEventType.Sunset:
				value = candidate => primary.ApparentAltitudeAt(candidate, observer);
				isCrossing = (former, current) => former > 0.0 && current <= 0.0;
				break;
			case AstronomicalEventType.SolarLongitude:
				if (primary is not ISolarEphemeris solar)
				{
					instant = MudInstant.Never;
					error = "The primary celestial does not expose a solar ephemeris.";
					return false;
				}

				value = candidate => SignedAngleDifference(solar.EclipticLongitudeAt(candidate), targetLongitude);
				isCrossing = CrossesZero;
				break;
			case AstronomicalEventType.LunarConjunction:
			case AstronomicalEventType.NewMoon:
				if (primary is not ILunarEphemeris newMoon)
				{
					instant = MudInstant.Never;
					error = "The primary celestial does not expose a lunar ephemeris.";
					return false;
				}

				value = candidate => SignedAngleDifference(newMoon.PhaseAngleAt(candidate), Math.PI);
				isCrossing = CrossesZero;
				break;
			case AstronomicalEventType.FullMoon:
				if (primary is not ILunarEphemeris fullMoon)
				{
					instant = MudInstant.Never;
					error = "The primary celestial does not expose a lunar ephemeris.";
					return false;
				}

				value = candidate => SignedAngleDifference(fullMoon.PhaseAngleAt(candidate), 0.0);
				isCrossing = CrossesZero;
				break;
			default:
				instant = MudInstant.Never;
				error = "That astronomical event type is not supported.";
				return false;
		}

		return TryBracketAndRefine(reference, value, isCrossing, out instant, out error);
	}

	private static bool TryBracketAndRefine(MudInstant reference, Func<MudInstant, double> value,
		Func<double, double, bool> isCrossing, out MudInstant instant, out string error)
	{
		instant = MudInstant.Never;
		error = string.Empty;
		var lower = AddSeconds(reference, 1);
		var lowerValue = value(lower);

		for (long elapsed = DefaultStepSeconds; elapsed <= DefaultMaximumSearchSeconds; elapsed += DefaultStepSeconds)
		{
			var upper = AddSeconds(reference, elapsed);
			var upperValue = value(upper);
			if (Math.Abs(upperValue) < 1.0E-10 || isCrossing(lowerValue, upperValue))
			{
				instant = Refine(lower, upper, value);
				return true;
			}

			lower = upper;
			lowerValue = upperValue;
		}

		error = "No matching astronomical event was found inside the bounded search window.";
		return false;
	}

	private static bool TryFindVisibleCrescent(MudInstant reference, ICelestialEphemeris primary,
		ICelestialEphemeris? secondary, GeographicCoordinate observer, out MudInstant instant, out string error)
	{
		instant = MudInstant.Never;
		if (primary is not ISolarEphemeris sun || secondary is not ILunarEphemeris moon)
		{
			error = "Visible crescent search requires a solar ephemeris and a lunar ephemeris.";
			return false;
		}

		var searchFrom = reference;
		for (var i = 0; i < 90; i++)
		{
			if (!TryFindSingle(AstronomicalEventType.Sunset, searchFrom, sun, observer, null, 0.0, out var sunset, out error))
			{
				return false;
			}

			var moonAltitude = moon.ApparentAltitudeAt(sunset, observer);
			var elongation = AngularSeparation(moon, sun, sunset);
			if (moonAltitude >= 5.0.DegreesToRadians() && elongation >= 10.0.DegreesToRadians())
			{
				instant = sunset;
				error = string.Empty;
				return true;
			}

			searchFrom = AddSeconds(sunset, 12 * 60 * 60);
		}

		error = "No deterministic visible crescent approximation was found inside the bounded search window.";
		return false;
	}

	private static MudInstant Refine(MudInstant lower, MudInstant upper, Func<MudInstant, double> value)
	{
		var lowerValue = value(lower);
		while (upper.Ticks - lower.Ticks > RefinementToleranceSeconds)
		{
			var middle = lower.WithTicks(lower.Ticks + (upper.Ticks - lower.Ticks) / 2);
			var middleValue = value(middle);
			if (CrossesZero(lowerValue, middleValue) || Math.Abs(middleValue) < 1.0E-10)
			{
				upper = middle;
			}
			else
			{
				lower = middle;
				lowerValue = middleValue;
			}
		}

		return upper;
	}

	private static bool CrossesZero(double former, double current)
	{
		return former == 0.0 || current == 0.0 || Math.Sign(former) != Math.Sign(current);
	}

	private static double SignedAngleDifference(double angle, double target)
	{
		var difference = (angle - target) % (2 * Math.PI);
		if (difference <= -Math.PI)
		{
			difference += 2 * Math.PI;
		}
		else if (difference > Math.PI)
		{
			difference -= 2 * Math.PI;
		}

		return difference;
	}

	private static double AngularSeparation(ICelestialEphemeris first, ICelestialEphemeris second, MudInstant instant)
	{
		var firstRightAscension = first.RightAscensionAt(instant);
		var firstDeclination = first.DeclinationAt(instant);
		var secondRightAscension = second.RightAscensionAt(instant);
		var secondDeclination = second.DeclinationAt(instant);
		var cosine = Math.Sin(firstDeclination) * Math.Sin(secondDeclination) +
		             Math.Cos(firstDeclination) * Math.Cos(secondDeclination) *
		             Math.Cos(firstRightAscension - secondRightAscension);
		return Math.Acos(Math.Clamp(cosine, -1.0, 1.0));
	}

	private static MudInstant AddSeconds(MudInstant instant, long seconds)
	{
		return instant.AddSeconds(seconds);
	}
}

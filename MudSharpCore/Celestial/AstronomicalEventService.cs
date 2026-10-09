#nullable enable

using MudSharp.TimeAndDate;

namespace MudSharp.Celestial;

public sealed class AstronomicalEventService : IAstronomicalEventService
{
	public const int MaximumPhysicalEventOccurrence = 32;
	public const int MaximumPhysicalEventSamples = 100_000;
	private const long DefaultStepSeconds = 1800;
	private const long DefaultMaximumSearchSeconds = 86400L * 800L;
	private const long RefinementToleranceSeconds = 1;

	public static AstronomicalEventService Instance { get; } = new();

	private AstronomicalEventService()
	{
	}

	public static bool IsSolar(ICelestialObject celestial) => celestial is ISolarEphemeris ||
		celestial is IAuthoredCelestial authored && authored.GetCapabilities().HasFlag(CelestialCapabilities.Solar);
	public static bool IsLunar(ICelestialObject celestial) => celestial is ILunarEphemeris ||
		celestial is IAuthoredCelestial authored && authored.GetCapabilities().HasFlag(CelestialCapabilities.Lunar);

	public bool TryFindNextForCelestial(AstronomicalEventType eventType, MudInstant reference, int occurrence,
		ICelestialObject primary, GeographicCoordinate observer, out MudInstant instant, out string error,
		double targetLongitude = 0, ICelestialObject? secondary = null)
	{
		var result = FindNextForCelestial(reference, new(eventType, occurrence, targetLongitude), primary, observer, secondary);
		instant = result.Instant;
		error = result.Found ? string.Empty : $"{result.Status}: {result.Error}";
		return result.Found;
	}

	public CelestialEventResult FindNextForCelestial(MudInstant reference, CelestialEventRequest request,
		ICelestialObject primary, GeographicCoordinate observer, ICelestialObject? secondary = null)
	{
		if (reference.IsNever || request.Occurrence < 1 || request.Occurrence > int.MaxValue ||
		    !double.IsFinite(request.TargetLongitude) || primary is null || observer is null)
			return CelestialEventResult.Failure(CelestialEventStatus.InvalidRequest, "Supply an instant, object, observer, finite longitude and positive integral occurrence within Int32 range.");
		try
		{
			if (request.Type == AstronomicalEventType.VisibleCrescent)
			{
				if (!IsSolar(primary) || secondary is null || !IsLunar(secondary))
					return CelestialEventResult.Failure(CelestialEventStatus.InvalidRequest, "Crescent queries require a sun and a moon, in that order.");
				if (secondary is IAuthoredCelestial moon)
				{
					if (primary is not ICelestialTimeContext solar || solar.Clock.Id != moon.Clock.Id)
						return CelestialEventResult.Failure(CelestialEventStatus.IncompatibleTimeContext, "Crescent participants have incompatible clocks.");
					if (primary is IAuthoredCelestial authoredSun) _ = authoredSun.EvaluateAt(reference);
					else if (reference.ToMudDateTime(solar.Calendar, solar.Clock, solar.Clock.PrimaryTimezone).Date is null)
						return CelestialEventResult.Failure(CelestialEventStatus.IncompatibleTimeContext, "Cannot convert the reference into the associated sun's calendar.");
					return moon.FindNext(reference, request with { AssociatedSunId = primary.Id });
				}
				if (primary is IAuthoredCelestial)
					return CelestialEventResult.Failure(CelestialEventStatus.Unsupported, "This physical moon has no authored crescent-marker capability.");
			}
			if (primary is IAuthoredCelestial authored) return authored.FindNext(reference, request);
			if (request.EventKey is not null || primary is not ICelestialEphemeris ephemeris || request.Type is not { } type)
				return CelestialEventResult.Failure(CelestialEventStatus.Unsupported, "The object does not support that event capability.");
			if (request.Occurrence > MaximumPhysicalEventOccurrence)
				return CelestialEventResult.Failure(CelestialEventStatus.InvalidRequest, $"Physical ephemeris queries support occurrences from 1 to {MaximumPhysicalEventOccurrence}.");
			if (primary is ICelestialTimeContext context && reference.HasSourceContext)
			{
				var converted = reference.ToMudDateTime(context.Calendar, context.Clock, context.Clock.PrimaryTimezone);
				if (converted.Date is null) return CelestialEventResult.Failure(CelestialEventStatus.IncompatibleTimeContext, "The source time context cannot be converted.");
				reference = MudInstant.FromMudDateTime(converted);
			}
			if (secondary is ICelestialTimeContext secondaryContext && primary is ICelestialTimeContext primaryContext && secondaryContext.Clock.Id != primaryContext.Clock.Id)
				return CelestialEventResult.Failure(CelestialEventStatus.IncompatibleTimeContext, "The celestial clocks cannot be converted.");
			return TryFindNext(type, reference, (int)request.Occurrence, ephemeris, observer, out var instant, out var error,
				request.TargetLongitude, secondary as ICelestialEphemeris)
				? new(CelestialEventStatus.Found, instant, string.Empty)
				: CelestialEventResult.Failure(error.Contains("bounded search window", StringComparison.Ordinal) ? CelestialEventStatus.SearchLimitReached : CelestialEventStatus.Unsupported, error);
		}
		catch (OverflowException) { return CelestialEventResult.Failure(CelestialEventStatus.OutOfRange, "The requested instant is outside representable time."); }
		catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
		{
			return CelestialEventResult.Failure(CelestialEventStatus.IncompatibleTimeContext, ex.Message);
		}
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

		if (occurrence < 1 || occurrence > MaximumPhysicalEventOccurrence)
		{
			error = $"The occurrence must be between 1 and {MaximumPhysicalEventOccurrence}.";
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

		var budget = new SearchBudget();
		var searchFrom = reference;
		try
		{
			for (var i = 0; i < occurrence; i++)
			{
				if (!TryFindSingle(eventType, searchFrom, primary, observer, secondary, targetLongitude, budget, out instant, out error))
				{
					return false;
				}

				searchFrom = AddSeconds(instant, RefinementToleranceSeconds);
			}
		}
		catch (SearchBudgetExceededException)
		{
			instant = MudInstant.Never;
			error = "The physical ephemeris sampling budget was exhausted inside the bounded search window.";
			return false;
		}

		return true;
	}

	private static bool TryFindSingle(AstronomicalEventType eventType, MudInstant reference,
		ICelestialEphemeris primary, GeographicCoordinate observer, ICelestialEphemeris? secondary,
		double targetLongitude, SearchBudget budget, out MudInstant instant, out string error)
	{
		if (eventType == AstronomicalEventType.VisibleCrescent)
		{
			return TryFindVisibleCrescent(reference, primary, secondary, observer, budget, out instant, out error);
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

		return TryBracketAndRefine(reference, candidate => budget.Sample(value, candidate), isCrossing, out instant, out error);
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
		ICelestialEphemeris? secondary, GeographicCoordinate observer, SearchBudget budget, out MudInstant instant, out string error)
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
			if (!TryFindSingle(AstronomicalEventType.Sunset, searchFrom, sun, observer, null, 0.0, budget, out var sunset, out error))
			{
				return false;
			}

			var moonAltitude = budget.Sample(candidate => moon.ApparentAltitudeAt(candidate, observer), sunset);
			var elongation = AngularSeparation(moon, sun, sunset, budget);
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

	private static double AngularSeparation(ICelestialEphemeris first, ICelestialEphemeris second, MudInstant instant, SearchBudget budget)
	{
		var firstRightAscension = budget.Sample(first.RightAscensionAt, instant);
		var firstDeclination = budget.Sample(first.DeclinationAt, instant);
		var secondRightAscension = budget.Sample(second.RightAscensionAt, instant);
		var secondDeclination = budget.Sample(second.DeclinationAt, instant);
		var cosine = Math.Sin(firstDeclination) * Math.Sin(secondDeclination) +
		             Math.Cos(firstDeclination) * Math.Cos(secondDeclination) *
		             Math.Cos(firstRightAscension - secondRightAscension);
		return Math.Acos(Math.Clamp(cosine, -1.0, 1.0));
	}

	private sealed class SearchBudget
	{
		private int _remaining = MaximumPhysicalEventSamples;

		public double Sample(Func<MudInstant, double> sample, MudInstant instant)
		{
			if (_remaining-- <= 0)
			{
				throw new SearchBudgetExceededException();
			}

			return sample(instant);
		}
	}

	private sealed class SearchBudgetExceededException : Exception;

	private static MudInstant AddSeconds(MudInstant instant, long seconds)
	{
		return instant.AddSeconds(seconds);
	}
}

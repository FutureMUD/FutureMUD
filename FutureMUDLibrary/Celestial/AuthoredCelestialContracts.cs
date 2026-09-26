#nullable enable

using System;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Celestial;

public enum CelestialMotion { Rising, Falling, Level, Stationary }
public enum CelestialEventStatus { Found, NoFutureOccurrence, Unsupported, InvalidRequest, IncompatibleTimeContext, OutOfRange, SearchLimitReached }

[Flags]
public enum CelestialCapabilities
{
	None = 0, Solar = 1, Lunar = 2, HorizonEvents = 4, PhaseEvents = 8,
	SyntheticLongitude = 16, AuthoredCrescent = 32, NamedEvents = 64, GeographyIndependent = 128
}

/// <summary>A unit direction in the East, North, Up observer frame; not an orbital coordinate.</summary>
public readonly record struct CelestialVector(double East, double North, double Up);
public readonly record struct CelestialPhase(double Turns, double IlluminationFraction, MoonPhase Name);
public readonly record struct CelestialState(CelestialVector DirectionVector, double Azimuth, double Elevation,
	double SourceLux, CelestialMotion Motion, CelestialMoveDirection Direction, CelestialPhase? Phase,
	long PathMinute, long AnnualMinute, double? SyntheticLongitude);

public readonly record struct CelestialEventRequest(AstronomicalEventType? Type, long Occurrence = 1,
	double TargetLongitude = 0, string? EventKey = null, long? AssociatedSunId = null);
public readonly record struct CelestialEventResult(CelestialEventStatus Status, MudInstant Instant, string Error)
{
	public bool Found => Status == CelestialEventStatus.Found;
	public static CelestialEventResult Failure(CelestialEventStatus status, string error) => new(status, MudInstant.Never, error);
}

public interface ICelestialTimeContext
{
	ICalendar Calendar { get; }
	IClock Clock { get; }
}

public interface ILunarPhase
{
	MoonPhase CurrentPhase();
}

/// <summary>Pure arbitrary-instant apparent state, with explicit time-context validation.</summary>
public interface IAuthoredCelestial : ICelestialTimeContext
{
	CelestialState EvaluateAt(MudInstant instant);
	CelestialEventResult FindNext(MudInstant reference, CelestialEventRequest request);
	CelestialCapabilities GetCapabilities();
	void ForgetSubscriber(object subscriber);
}

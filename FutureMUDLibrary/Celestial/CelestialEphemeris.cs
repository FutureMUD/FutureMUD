#nullable enable

using MudSharp.TimeAndDate;

namespace MudSharp.Celestial;

public enum AstronomicalEventType
{
	Sunrise,
	Sunset,
	SolarLongitude,
	LunarConjunction,
	NewMoon,
	FullMoon,
	VisibleCrescent
}

public interface ICelestialEphemeris
{
	double RightAscensionAt(MudInstant instant);

	double DeclinationAt(MudInstant instant);

	double ApparentAltitudeAt(MudInstant instant, GeographicCoordinate observer);

	double ApparentAzimuthAt(MudInstant instant, GeographicCoordinate observer);

	double IlluminationAt(MudInstant instant, GeographicCoordinate observer);
}

public interface ISolarEphemeris : ICelestialEphemeris
{
	double EclipticLongitudeAt(MudInstant instant);
}

public interface ILunarEphemeris : ICelestialEphemeris
{
	double EclipticLongitudeAt(MudInstant instant);

	double PhaseAngleAt(MudInstant instant);
}

public interface IAstronomicalEventService
{
	bool TryFindNext(AstronomicalEventType eventType, MudInstant reference, int occurrence,
		ICelestialEphemeris primary, GeographicCoordinate observer, out MudInstant instant, out string error,
		double targetLongitude = 0.0, ICelestialEphemeris? secondary = null);
}

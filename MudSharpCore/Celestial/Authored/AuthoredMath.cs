#nullable enable

using System.Diagnostics.CodeAnalysis;

namespace MudSharp.Celestial.Authored;

public static class AuthoredMath
{
	public const double HorizonTolerance = 1e-12;
	public const double Radians = Math.PI / 180.0;
	public const double Tau = Math.PI * 2;
	public static long FloorDiv(long value, long divisor)
	{
		if (divisor <= 0) throw new ArgumentOutOfRangeException(nameof(divisor));
		var quotient = value / divisor;
		return value % divisor < 0 ? quotient - 1 : quotient;
	}
	public static long Mod(long value, long divisor)
	{
		if (divisor <= 0) throw new ArgumentOutOfRangeException(nameof(divisor));
		var remainder = value % divisor;
		return remainder < 0 ? remainder + divisor : remainder;
	}
	public static double Wrap(double value, double period = Tau)
	{
		var remainder = value % period;
		return remainder < 0 ? remainder + period : remainder;
	}
	public static int Interval(long[] starts, long minute, long period, ref int cursor)
	{
		bool Contains(int index) => index >= 0 && index < starts.Length &&
			Mod(minute - starts[index], period) < (starts.Length == 1 ? period : Mod(starts[(index + 1) % starts.Length] - starts[index], period));
		if (Contains(cursor)) return cursor;
		var next = (cursor + 1) % starts.Length;
		if (Contains(next)) return cursor = next;
		var previous = (cursor + starts.Length - 1) % starts.Length;
		if (Contains(previous)) return cursor = previous;
		cursor = UpperBound(starts, minute) - 1;
		if (cursor < 0) cursor = starts.Length - 1;
		return cursor;
	}
	public static void Require([DoesNotReturnIf(false)] bool condition, string error)
	{
		if (!condition) throw new ArgumentException(error);
	}
	public static void Finite(double value, string field) => Require(double.IsFinite(value), $"{field} must be finite.");
	public static int UpperBound(long[] values, long value)
	{
		var low = 0;
		var high = values.Length;
		while (low < high)
		{
			var mid = low + (high - low) / 2;
			if (values[mid] <= value) low = mid + 1;
			else high = mid;
		}
		return low;
	}
	public static int Sign(double value, double tolerance = HorizonTolerance) => value > tolerance ? 1 : value < -tolerance ? -1 : 0;
	public static TimeOfDay TimeOfDay(CelestialState state, bool eligible)
	{
		if (!eligible || state.Elevation < -0.20944) return Celestial.TimeOfDay.Night;
		if (state.Elevation > 0.05) return state.Direction == CelestialMoveDirection.Ascending ? Celestial.TimeOfDay.Morning : Celestial.TimeOfDay.Afternoon;
		return state.Direction == CelestialMoveDirection.Ascending ? Celestial.TimeOfDay.Dawn : Celestial.TimeOfDay.Dusk;
	}
	public static CelestialPhase Phase(double turns)
	{
		var q = Wrap(turns, 1);
		var name = q < .0625 || q >= .9375 ? MoonPhase.Full : q < .1875 ? MoonPhase.WaningGibbous :
			q < .3125 ? MoonPhase.LastQuarter : q < .4375 ? MoonPhase.WaningCrescent :
			q < .5625 ? MoonPhase.New : q < .6875 ? MoonPhase.WaxingCrescent :
			q < .8125 ? MoonPhase.FirstQuarter : MoonPhase.WaxingGibbous;
		return new(q, (1 + Math.Cos(Tau * q)) / 2, name);
	}
}

internal readonly record struct SkyVector(double X, double Y, double Z)
{
	public static SkyVector operator +(SkyVector a, SkyVector b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
	public static SkyVector operator -(SkyVector a, SkyVector b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
	public static SkyVector operator *(SkyVector a, double b) => new(a.X * b, a.Y * b, a.Z * b);
	public double Dot(SkyVector b) => X * b.X + Y * b.Y + Z * b.Z;
	public SkyVector Cross(SkyVector b) => new(Y * b.Z - Z * b.Y, Z * b.X - X * b.Z, X * b.Y - Y * b.X);
	public double Length => Math.Sqrt(Dot(this));
	public SkyVector Unit => this * (1 / Length);
	public double Elevation => Math.Asin(Math.Clamp(Z, -1, 1));
	public double Azimuth
	{
		get
		{
			if (X * X + Y * Y < 1e-20) return 0;
			var bearing = AuthoredMath.Wrap(Math.Atan2(X, Y));
			return bearing > AuthoredMath.Tau - 1e-14 ? 0 : bearing;
		}
	}
	public CelestialVector Public => new(X, Y, Z);
	public static SkyVector FromDegrees(double azimuth, double elevation)
	{
		AuthoredMath.Finite(azimuth, "Bearing");
		AuthoredMath.Finite(elevation, "Elevation");
		AuthoredMath.Require(elevation >= -90 && elevation <= 90, "Elevation must be in -90..90 degrees.");
		var az = AuthoredMath.Wrap(azimuth, 360) * AuthoredMath.Radians;
		var el = elevation * AuthoredMath.Radians;
		return new(Math.Cos(el) * Math.Sin(az), Math.Cos(el) * Math.Cos(az), Math.Sin(el));
	}
}

/// <summary>Sorted recurrence offsets in clock seconds. Selection is independent of occurrence magnitude.</summary>
public sealed class AuthoredEventIndex
{
	private readonly long[] _offsets;
	public long PeriodSeconds { get; }
	public IReadOnlyList<long> Offsets => Array.AsReadOnly(_offsets);
	public int Count => _offsets.Length;
	public AuthoredEventIndex(long periodMinutes, IEnumerable<long> offsets, int secondsPerMinute)
	{
		AuthoredMath.Require(periodMinutes > 0 && secondsPerMinute > 0, "Event period and clock minute must be positive.");
		PeriodSeconds = checked(periodMinutes * secondsPerMinute);
		_offsets = offsets.Distinct().Order().Select(x =>
		{
			AuthoredMath.Require(x >= 0 && x < periodMinutes, "Event offset must lie in [0, period).");
			return checked(x * secondsPerMinute);
		}).ToArray();
	}
	public bool IsDue(long relativeSeconds) => Array.BinarySearch(_offsets, AuthoredMath.Mod(relativeSeconds, PeriodSeconds)) >= 0;
	public bool TryNext(long referenceTicks, long anchorTicks, long occurrence, out long ticks)
	{
		ticks = 0;
		AuthoredMath.Require(occurrence > 0, "Occurrence must be a positive integer.");
		if (_offsets.Length == 0) return false;
		checked
		{
			var relative = referenceTicks - anchorTicks;
			var cycle = AuthoredMath.FloorDiv(relative, PeriodSeconds);
			var within = AuthoredMath.Mod(relative, PeriodSeconds);
			var rank = AuthoredMath.UpperBound(_offsets, within) + occurrence - 1;
			var answerCycle = cycle + rank / _offsets.Length;
			// Int128 avoids false overflow in the intermediate product when the final timestamp fits.
			var result = (Int128)anchorTicks + (Int128)answerCycle * PeriodSeconds + _offsets[rank % _offsets.Length];
			ticks = (long)result;
			return true;
		}
	}
}

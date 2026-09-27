#nullable enable

namespace MudSharp.Celestial.Authored;

/// <summary>Constant-size circular segments; all evaluations use authoritative integer minute samples.</summary>
internal readonly record struct SkySegment(long Start, long Length, SkyVector Centre, SkyVector A, SkyVector B, double Angle)
{
	public SkyVector At(long offset)
	{
		var theta = Angle * ((double)offset / Length);
		return Centre + A * Math.Cos(theta) + B * Math.Sin(theta);
	}
	public IEnumerable<long> Extrema(bool differences = false)
	{
		if (Angle == 0 || (Math.Abs(A.Z) + Math.Abs(B.Z)) < 1e-16) yield break;
		var phi = Math.Atan2(B.Z, A.Z) + (differences ? Math.PI / 2 : 0);
		for (var n = -3; n <= 4; n++)
		{
			var theta = phi + n * Math.PI;
			var minute = theta / Angle * Length + (differences ? .5 : 0);
			if (minute < 0 || minute > Length) continue;
			var floor = (long)Math.Floor(minute);
			for (var d = -2; d <= 2; d++)
			{
				var candidate = floor + d;
				if (candidate > 0 && candidate < Length) yield return candidate;
			}
		}
	}
}

internal readonly record struct SignRun(long Start, int Sign);

public sealed class CompiledSkyPath
{
	private readonly SkySegment[] _segments;
	private readonly long[] _starts;
	private readonly SkyVector[]? _dense;
	private readonly long[] _directionStarts;
	private readonly CelestialMoveDirection[] _directions;
	private readonly int[] _verticalSigns;
	public long Period { get; }
	public double MaximumElevation { get; }
	public double? ViaMinute { get; }
	public int StoredEntries => _dense?.Length ?? _segments.Length;
	public IReadOnlyList<long> RiseOffsets { get; }
	public IReadOnlyList<long> SetOffsets { get; }

	public CompiledSkyPath(AuthoredPathDefinition source, int entryLimit)
	{
		Period = source.Period;
		AuthoredMath.Require(Period > 0, "Path period must be positive.");
		if (source.Mode == AuthoredPathMode.Dense)
		{
			AuthoredMath.Require(source.Keys.Count <= entryLimit && source.Keys.Count == Period, "Dense paths require exactly one supplied sample for every minute 0..P-1, within the entry limit.");
			_dense = new SkyVector[source.Keys.Count];
			for (var i = 0; i < _dense.Length; i++)
			{
				AuthoredMath.Require(source.Keys[i].Minute == i, "Dense samples must be ordered and numbered 0..P-1.");
				_dense[i] = SkyVector.FromDegrees(source.Keys[i].Azimuth, source.Keys[i].Elevation);
			}
			_segments = [];
			MaximumElevation = _dense.Max(x => x.Elevation) / AuthoredMath.Radians;
		}
		else if (source.Mode == AuthoredPathMode.Sparse)
		{
			AuthoredMath.Require(source.Keys.Count >= 2 && source.Keys.Count <= entryLimit, "Sparse paths need a start and explicit closure, within the entry limit.");
			var keys = source.Keys.OrderBy(x => x.Minute).ToArray();
			AuthoredMath.Require(keys[0].Minute == 0 && keys[^1].Minute == Period, "Sparse paths must start at zero and include a closure descriptor at Period.");
			var vectors = keys.Select(x => SkyVector.FromDegrees(x.Azimuth, x.Elevation)).ToArray();
			AuthoredMath.Require((vectors[0] - vectors[^1]).Length < 1e-10, "Path closure must match the position at minute zero.");
			_segments = new SkySegment[keys.Length - 1];
			for (var i = 0; i < _segments.Length; i++)
			{
				var length = keys[i + 1].Minute - keys[i].Minute;
				AuthoredMath.Require(length > 0, "Position keys must have unique increasing minute offsets.");
				var from = vectors[i];
				var to = vectors[i + 1];
				var mode = keys[i].Transition;
				AuthoredMath.Require(Enum.IsDefined(mode), "Unknown position transition.");
				var identical = (from - to).Length < 1e-12;
				AuthoredMath.Require(mode != TrackTransition.Hold || identical, "Hold requires identical endpoints; use Jump for a discontinuity.");
				if (mode != TrackTransition.Travel || identical)
				{
					_segments[i] = new(keys[i].Minute, length, default, from, default, 0);
					continue;
				}
				var dot = Math.Clamp(from.Dot(to), -1, 1);
				AuthoredMath.Require(dot > -1 + 1e-10, "Antipodal travel is ambiguous; supply an intermediate keyframe.");
				var angle = Math.Acos(dot);
				_segments[i] = new(keys[i].Minute, length, default, from, (to - from * dot) * (1 / Math.Sin(angle)), angle);
			}
			MaximumElevation = _segments.Max(MaximumAltitude) / AuthoredMath.Radians;
		}
		else
		{
			AuthoredMath.Require(source.Mode is AuthoredPathMode.SimpleRail or AuthoredPathMode.ThreePointRail, "Unknown path mode.");
			AuthoredMath.Require(source.Rise >= 0 && source.Rise < Period && source.Set >= 0 && source.Set < Period && source.Rise != source.Set, "Rail rise/set must be distinct offsets in [0, period).");
			var above = AuthoredMath.Mod(source.Set - source.Rise, Period);
			var r = SkyVector.FromDegrees(source.RiseBearing, 0);
			var end = SkyVector.FromDegrees(source.SetBearing, 0);
			SkyVector centre, a, b;
			double arc;
			if (source.Mode == AuthoredPathMode.SimpleRail)
			{
				AuthoredMath.Finite(source.MaximumElevation, "Maximum elevation");
				AuthoredMath.Require(source.MaximumElevation > 0 && source.MaximumElevation <= 90, "Simple rail maximum elevation must be in (0, 90].");
				AuthoredMath.Require((r + end).Length < 1e-10, "Simple rail endpoints must be opposite; use three-point mode.");
				var tilt = SkyVector.FromDegrees(source.TiltBearing, 0);
				AuthoredMath.Require(Math.Abs(r.Dot(tilt)) < 1e-10, "Tilt bearing must be perpendicular to the rise bearing.");
				centre = default;
				a = r;
				var h = source.MaximumElevation * AuthoredMath.Radians;
				b = tilt * Math.Cos(h) + new SkyVector(0, 0, Math.Sin(h));
				arc = Math.PI;
				ViaMinute = AuthoredMath.Wrap(source.Rise + above / 2.0, Period);
			}
			else
			{
				var via = SkyVector.FromDegrees(source.ViaBearing, source.ViaElevation);
				AuthoredMath.Require(source.ViaElevation > 0, "Three-point via must be strictly above the horizon.");
				var normal = (via - r).Cross(end - r);
				AuthoredMath.Require((r - end).Length > 1e-8 && normal.Length > 1e-8, "Three-point rail has coincident or ill-conditioned points.");
				normal = normal.Unit;
				centre = normal * normal.Dot(r);
				var radius = Math.Sqrt(Math.Max(0, 1 - centre.Dot(centre)));
				AuthoredMath.Require(radius > 1e-8, "Rail circle has zero or ill-conditioned radius.");
				a = r - centre;
				b = normal.Cross(a);
				double AngleOf(SkyVector p) => AuthoredMath.Wrap(Math.Atan2((p - centre).Dot(b), (p - centre).Dot(a)));
				arc = AngleOf(end);
				var viaAngle = AngleOf(via);
				if (viaAngle > arc)
				{
					b = b * -1;
					arc = AuthoredMath.Tau - arc;
					viaAngle = AuthoredMath.Tau - viaAngle;
				}
				AuthoredMath.Require(viaAngle > 1e-10 && viaAngle < arc - 1e-10, "Via does not define a distinct point on the rise-to-set arc.");
				ViaMinute = AuthoredMath.Wrap(source.Rise + above * viaAngle / arc, Period);
			}
			var ca = a * Math.Cos(arc) + b * Math.Sin(arc);
			var cb = a * -Math.Sin(arc) + b * Math.Cos(arc);
			var first = new SkySegment(source.Rise, above, centre, a, b, arc);
			var second = new SkySegment(source.Set, Period - above, centre, ca, cb, AuthoredMath.Tau - arc);
			AuthoredMath.Require((centre + a * Math.Cos(arc / 2) + b * Math.Sin(arc / 2)).Z > 0 &&
				(centre + ca * Math.Cos((AuthoredMath.Tau - arc) / 2) + cb * Math.Sin((AuthoredMath.Tau - arc) / 2)).Z < 0,
				"Rail must traverse above the horizon from rise to set and below on the return.");
			_segments = [first, second];
			Array.Sort(_segments, (x, y) => x.Start.CompareTo(y.Start));
			MaximumElevation = _segments.Max(MaximumAltitude) / AuthoredMath.Radians;
		}
		_starts = _segments.Select(x => x.Start).ToArray();
		var crossings = Crossings(0);
		RiseOffsets = Array.AsReadOnly(crossings.Rises);
		SetOffsets = Array.AsReadOnly(crossings.Sets);
		var motion = CompileRuns(true, 0);
		var last = motion.LastOrDefault(x => x.Sign != 0).Sign;
		if (last == 0) last = 1;
		_directionStarts = motion.Select(x => x.Start).ToArray();
		_verticalSigns = motion.Select(x => x.Sign).ToArray();
		_directions = new CelestialMoveDirection[motion.Count];
		for (var i = 0; i < motion.Count; i++)
		{
			if (motion[i].Sign != 0) last = motion[i].Sign;
			_directions[i] = last > 0 ? CelestialMoveDirection.Ascending : CelestialMoveDirection.Descending;
		}
	}

	private static double MaximumAltitude(SkySegment segment)
	{
		var maximum = Math.Max(segment.At(0).Z, segment.At(segment.Length).Z);
		var phi = AuthoredMath.Wrap(Math.Atan2(segment.B.Z, segment.A.Z));
		if (phi <= segment.Angle) maximum = Math.Max(maximum, segment.Centre.Z + Math.Sqrt(segment.A.Z * segment.A.Z + segment.B.Z * segment.B.Z));
		return Math.Asin(Math.Clamp(maximum, -1, 1));
	}

	internal SkyVector At(long minute) { var cursor = -1; return At(minute, ref cursor); }
	internal SkyVector At(long minute, ref int cursor)
	{
		minute = AuthoredMath.Mod(minute, Period);
		if (_dense is not null) return _dense[(int)minute];
		AuthoredMath.Interval(_starts, minute, Period, ref cursor);
		var segment = _segments[cursor];
		return segment.At(AuthoredMath.Mod(minute - segment.Start, Period));
	}

	internal (int VerticalSign, CelestialMoveDirection Direction) MotionAt(long minute, ref int cursor)
	{
		var index = AuthoredMath.Interval(_directionStarts, AuthoredMath.Mod(minute, Period), Period, ref cursor);
		return (_verticalSigns[index], _directions[index]);
	}

	internal (long[] Rises, long[] Sets) Crossings(double elevationDegrees)
	{
		var runs = CompileRuns(false, Math.Sin(elevationDegrees * AuthoredMath.Radians));
		var strict = runs.Where(x => x.Sign != 0).ToArray();
		if (strict.Length == 0) return ([], []);
		var rise = new List<long>();
		var set = new List<long>();
		for (var i = 0; i < runs.Count; i++)
		{
			if (runs[i].Sign == 0) continue;
			var previous = (i + runs.Count - 1) % runs.Count;
			var eventMinute = runs[i].Start;
			if (runs[previous].Sign == 0)
			{
				eventMinute = runs[previous].Start;
				previous = (previous + runs.Count - 1) % runs.Count;
			}
			if (runs[previous].Sign == 0 || runs[previous].Sign == runs[i].Sign) continue;
			(runs[i].Sign > 0 ? rise : set).Add(eventMinute);
		}
		return (rise.Distinct().Order().ToArray(), set.Distinct().Order().ToArray());
	}

	/// <summary>Compress sampled signs with monotonic subdivisions and integer binary search. No elapsed-minute scan.</summary>
	private List<SignRun> CompileRuns(bool differences, double threshold)
	{
		var samples = new SortedDictionary<long, int>();
		double Value(long minute)
		{
			var current = At(minute);
			if (!differences) return current.Z - threshold;
			if (_dense is null)
			{
				var index = AuthoredMath.UpperBound(_starts, minute) - 1;
				if (index < 0) index = _segments.Length - 1;
				var segment = _segments[index];
				var offset = AuthoredMath.Mod(minute - segment.Start, Period);
				if (offset > 0)
				{
					var amplitude = Math.Sqrt(segment.A.Z * segment.A.Z + segment.B.Z * segment.B.Z);
					if (segment.Angle == 0 || amplitude < 1e-15) return 0;
					// z(m)-z(m-1) = 2 sin(step/2) [-A.z sin(mid)+B.z cos(mid)].
					// Divide out the positive amplitude/step: very long valid cycles retain direction
					// without subtracting nearly equal doubles or treating slow travel as a plateau.
					var angle = segment.Angle * ((offset - .5) / segment.Length);
					return -segment.A.Z / amplitude * Math.Sin(angle) + segment.B.Z / amplitude * Math.Cos(angle);
				}
			}
			return current.Elevation - At(AuthoredMath.Mod(minute - 1, Period)).Elevation;
		}
		if (_dense is not null)
		{
			for (var i = 0; i < _dense.Length; i++) samples[i] = AuthoredMath.Sign(Value(i));
		}
		else
		{
			foreach (var segment in _segments)
			{
				var cuts = new SortedSet<long> { 0, Math.Min(1, segment.Length), segment.Length };
				foreach (var cut in segment.Extrema(differences)) cuts.Add(cut);
				var ordered = cuts.ToArray();
				long WrapLocal(long local) => (long)(((Int128)segment.Start + local) % Period);
				for (var i = 0; i < ordered.Length - 1; i++)
				{
					var start = ordered[i];
					var end = ordered[i + 1] - 1;
					if (start > end) continue;
					var loValue = Value(WrapLocal(start));
					var hiValue = Value(WrapLocal(end));
					samples[WrapLocal(start)] = AuthoredMath.Sign(loValue);
					var ascending = hiValue >= loValue;
					foreach (var level in new[] { -AuthoredMath.HorizonTolerance, AuthoredMath.HorizonTolerance })
					{
						var low = start;
						var high = end + 1;
						// First sample on the far side of each classification boundary.
						while (low < high)
						{
							var middle = low + (high - low) / 2;
							var value = Value(WrapLocal(middle));
							var far = ascending ? (level < 0 ? value >= level : value > level) : (level > 0 ? value <= level : value < level);
							if (far) high = middle; else low = middle + 1;
						}
						if (low <= end) samples[WrapLocal(low)] = AuthoredMath.Sign(Value(WrapLocal(low)));
					}
				}
			}
		}
		samples[0] = AuthoredMath.Sign(Value(0));
		var runs = new List<SignRun>();
		foreach (var (minute, sign) in samples)
		{
			if (runs.Count == 0 || runs[^1].Sign != sign) runs.Add(new(minute, sign));
		}
		// Merge the seam run at its true beginning (possibly near the end of the cycle).
		if (runs.Count > 1 && runs[0].Sign == runs[^1].Sign) runs.RemoveAt(0);
		return runs;
	}
}

#nullable enable

namespace MudSharp.Celestial.Authored;

public sealed class CompiledScalarTrack
{
	private readonly ScalarKey[] _keys;
	private readonly long[] _starts;
	public long Period { get; }
	public int StoredEntries => _keys.Length;
	public CompiledScalarTrack(AuthoredScalarTrack source, bool phase, int entryLimit)
	{
		Period = source.Period;
		AuthoredMath.Require(Period > 0 && source.Keys.Count >= 2 && source.Keys.Count <= entryLimit, "Scalar track requires a positive period, keys and explicit closure, within the entry limit.");
		_keys = source.Keys.OrderBy(x => x.Minute).ToArray();
		_starts = _keys.Take(_keys.Length - 1).Select(x => x.Minute).ToArray();
		AuthoredMath.Require(_keys[0].Minute == 0 && _keys[^1].Minute == Period, "Scalar track must start at zero and close at Period.");
		for (var i = 0; i < _keys.Length; i++)
		{
			AuthoredMath.Finite(_keys[i].Value, "Scalar key");
			AuthoredMath.Require(Enum.IsDefined(_keys[i].Transition), "Unknown scalar transition.");
			AuthoredMath.Require(phase || _keys[i].Value >= 0, "Source lux must be nonnegative.");
			if (i == _keys.Length - 1) continue;
			AuthoredMath.Require(_keys[i + 1].Minute > _keys[i].Minute, "Scalar keys must have unique increasing offsets.");
			var difference = _keys[i + 1].Value - _keys[i].Value;
			AuthoredMath.Require(_keys[i].Transition != TrackTransition.Hold || difference == 0, "Scalar Hold requires equal endpoints; use Jump for a discontinuity.");
			AuthoredMath.Require(!phase || _keys[i].Transition != TrackTransition.Travel || Math.Abs(difference) <= 1, "Phase Travel permits at most one turn per segment; add intermediate keys.");
		}
		var closure = _keys[^1].Value - _keys[0].Value;
		AuthoredMath.Require(phase ? Math.Abs(closure - Math.Round(closure)) < 1e-12 : closure == 0, "Scalar closure must match the initial value (phase may retain whole-turn winding).");
	}

	public double At(long minute) { var cursor = -1; return At(minute, ref cursor); }
	public double At(long minute, ref int cursor)
	{
		minute = AuthoredMath.Mod(minute, Period);
		var i = AuthoredMath.Interval(_starts, minute, Period, ref cursor);
		var a = _keys[i];
		var b = _keys[i + 1];
		return a.Transition == TrackTransition.Travel
			? a.Value + (b.Value - a.Value) * ((double)(minute - a.Minute) / (b.Minute - a.Minute))
			: a.Value;
	}

	public long[] PhaseMilestones(double landmark)
	{
		var events = new SortedSet<long>();
		for (var i = 0; i < _keys.Length - 1; i++)
		{
			var a = _keys[i];
			var b = _keys[i + 1];
			var start = AuthoredMath.Wrap(a.Value, 1);
			var end = AuthoredMath.Wrap(b.Value, 1);
			if (a.Transition == TrackTransition.Jump)
			{
				if (Math.Abs(end - landmark) < 1e-12 && Math.Abs(start - landmark) >= 1e-12) events.Add(b.Minute % Period);
				continue;
			}
			if (a.Transition == TrackTransition.Hold || a.Value == b.Value) continue;
			var delta = b.Value - a.Value;
			for (var winding = -1; winding <= 2; winding++)
			{
				var distance = landmark + winding - start;
				if (delta > 0 ? distance <= 0 || distance > delta : distance >= 0 || distance < delta) continue;
				var low = 1L;
				var high = b.Minute - a.Minute;
				while (low < high)
				{
					var mid = low + (high - low) / 2;
					var progress = delta * ((double)mid / (b.Minute - a.Minute));
					if (delta > 0 ? progress >= distance : progress <= distance) high = mid;
					else low = mid + 1;
				}
				events.Add((a.Minute + low) % Period);
			}
		}
		return events.ToArray();
	}
}

#nullable enable

using System;

namespace MudSharp_Unit_Tests;

/// <summary>
/// Deterministic test clock whose monotonic timestamp and UTC wall clock can be moved
/// independently. Route movement must use the former.
/// </summary>
internal sealed class ManualTimeProvider : TimeProvider
{
	private long _timestamp;
	private DateTimeOffset _utcNow;

	public ManualTimeProvider(DateTimeOffset? utcNow = null)
	{
		_utcNow = utcNow ?? DateTimeOffset.UnixEpoch;
	}

	public override long TimestampFrequency => TimeSpan.TicksPerSecond;

	public override long GetTimestamp()
	{
		return _timestamp;
	}

	public override DateTimeOffset GetUtcNow()
	{
		return _utcNow;
	}

	public void Advance(TimeSpan elapsed)
	{
		if (elapsed < TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException(nameof(elapsed));
		}

		_timestamp = checked(_timestamp + elapsed.Ticks);
		_utcNow += elapsed;
	}

	public void SetUtcNow(DateTimeOffset value)
	{
		_utcNow = value;
	}
}

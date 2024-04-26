using System;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.TimeAndDate.Listeners;

public class TimeListener : ListenerBase
{
	protected IClock _watchClock;

	protected int _watchForHour;

	protected int _watchForMinute;

	protected int _watchForSecond;

	public TimeListener(IClock watchClock, int watchForSecond, int watchForMinute, int watchForHour, int repeatTimes,
		Action<object[]> payload, object[] objects)
		: base(repeatTimes, payload, objects)
	{
		WatchClock = watchClock;
		WatchForSecond = watchForSecond;
		WatchForMinute = watchForMinute;
		WatchForHour = watchForHour;
		Subscribe();
	}

	public override string FrameworkItemType => "TimeListener";

	public int WatchForSecond
	{
		get => _watchForSecond;
		protected init => _watchForSecond = value;
	}

	public int WatchForMinute
	{
		get => _watchForMinute;
		protected init => _watchForMinute = value;
	}

	public int WatchForHour
	{
		get => _watchForHour;
		protected init => _watchForHour = value;
	}

	public IClock WatchClock
	{
		get => _watchClock;
		protected init => _watchClock = value;
	}

	protected bool TimeIsRight()
	{
		return
			(WatchClock.CurrentTime.Seconds == WatchForSecond || WatchForSecond == -1) &&
			(WatchClock.CurrentTime.Minutes == WatchForMinute || WatchForMinute == -1) &&
			(WatchClock.CurrentTime.Hours == WatchForHour || WatchForHour == -1);
	}

	public void TimeUpdated()
	{
		if (TimeIsRight())
		{
			Payload(Objects);
			if (RepeatTimes > 0)
			{
				RepeatTimes--;
			}
		}
	}

	public override void UnSubscribe()
	{
		if (WatchForSecond != -1)
		{
			WatchClock.SecondsUpdated -= TimeUpdated;
		}
		else if (WatchForMinute != -1)
		{
			WatchClock.MinutesUpdated -= TimeUpdated;
		}
		else if (WatchForHour != -1)
		{
			WatchClock.HoursUpdated -= TimeUpdated;
		}
	}

	public void Subscribe()
	{
		if (WatchForSecond != -1)
		{
			WatchClock.SecondsUpdated += TimeUpdated;
		}
		else if (WatchForMinute != -1)
		{
			WatchClock.MinutesUpdated += TimeUpdated;
		}
		else if (WatchForHour != -1)
		{
			WatchClock.HoursUpdated += TimeUpdated;
		}
		else
		{
			throw new Exception("Invalid TimeListener - no time to subscribe.");
		}
	}

	public override string ToString()
	{
		return $"Time Listener: {WatchForHour}h {WatchForMinute}m {WatchForSecond}s";
	}
}
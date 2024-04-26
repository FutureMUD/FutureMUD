using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.TimeAndDate.Listeners;

public class WeekdayTimeListener : ListenerBase
{
	protected ICalendar _watchCalendar;

	protected IClock _watchClock;

	protected int _watchForHour;

	protected int _watchForMinute;

	protected int _watchForSecond;

	protected List<string> _weekdays;

	public WeekdayTimeListener(IClock watchClock, int watchForSecond, int watchForMinute, int watchForHour,
		ICalendar watchCalendar, List<string> weekdays, int repeatTimes, Action<object[]> payload, object[] objects)
		: base(repeatTimes, payload, objects)
	{
		WatchClock = watchClock;
		WatchForSecond = watchForSecond;
		WatchForMinute = watchForMinute;
		WatchForHour = watchForHour;
		WatchCalendar = watchCalendar;
		Weekdays = weekdays;
		Subscribe();
	}

	public override string FrameworkItemType => "WeekdayTimeListener";

	public ICalendar WatchCalendar
	{
		get => _watchCalendar;
		protected init => _watchCalendar = value;
	}

	public List<string> Weekdays
	{
		get => _weekdays;
		protected init => _weekdays = value;
	}

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
		else if (Weekdays.Count > 0)
		{
			WatchCalendar.DaysUpdated += TimeUpdated;
		}
		else
		{
			throw new Exception("Invalid TimeListener - no time to subscribe.");
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
		else if (Weekdays.Count > 0)
		{
			WatchCalendar.DaysUpdated -= TimeUpdated;
		}
	}

	public void TimeUpdated()
	{
		if (WatchCalendar.CurrentDate.WeekdayIndex != -1 &&
		    Weekdays.Contains(WatchCalendar.CurrentDate.Weekday) &&
		    TimeIsRight()
		   )
		{
			Payload(Objects);
			if (RepeatTimes > 0)
			{
				RepeatTimes--;
			}
		}
	}

	public override string ToString()
	{
		return $"Weekday Time Listener: {Weekdays.ToList()} {WatchForHour}h {WatchForMinute}m {WatchForSecond}s";
	}
}
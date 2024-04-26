using System;
using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.TimeAndDate.Listeners;

public class WeekdayListener : ListenerBase
{
	protected ICalendar _watchCalendar;

	protected List<string> _weekdays;

	public WeekdayListener(ICalendar watchCalendar, List<string> weekdays, int repeatTimes, Action<object[]> payload,
		object[] objects) : base(repeatTimes, payload, objects)
	{
		WatchCalendar = watchCalendar;
		Weekdays = weekdays;
		Subscribe();
	}

	public override string FrameworkItemType => "WeekdayListener";

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

	public void Subscribe()
	{
		WatchCalendar.DaysUpdated += DateUpdated;
	}

	public override void UnSubscribe()
	{
		WatchCalendar.DaysUpdated -= DateUpdated;
	}

	public void DateUpdated()
	{
		if (WatchCalendar.CurrentDate.WeekdayIndex != -1 &&
		    Weekdays.Contains(WatchCalendar.CurrentDate.Weekday))
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
		return $"Weekday Listener: {Weekdays.ListToString()}";
	}
}
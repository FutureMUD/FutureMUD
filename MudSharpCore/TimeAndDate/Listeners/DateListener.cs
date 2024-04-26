using System;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.TimeAndDate.Listeners;

public class DateListener : ListenerBase
{
	protected ICalendar _watchCalendar;
	protected int _watchForDay;

	protected string _watchForMonth;

	protected int _watchForYear;

	public DateListener(ICalendar watchCalendar, int watchForDay, string watchForMonth, int watchForYear,
		int repeatTimes, Action<object[]> payload, object[] objects)
		: base(repeatTimes, payload, objects)
	{
		WatchCalendar = watchCalendar;
		WatchForDay = watchForDay;
		WatchForMonth = watchForMonth;
		WatchForYear = watchForYear;
		Subscribe();
	}

	public DateListener(MudDateTime datetime, int repeatTimes, Action<object[]> payload, object[] objects) : base(
		repeatTimes, payload, objects)
	{
		WatchCalendar = datetime.Calendar;
		WatchForDay = datetime.Date.Day;
		WatchForMonth = datetime.Date.Month.Alias;
		WatchForYear = datetime.Date.Year;
		Subscribe();
	}

	public override string FrameworkItemType => "DateListener";

	public int WatchForDay
	{
		get => _watchForDay;
		protected init => _watchForDay = value;
	}

	public string WatchForMonth
	{
		get => _watchForMonth;
		protected init => _watchForMonth = value;
	}

	public int WatchForYear
	{
		get => _watchForYear;
		protected init => _watchForYear = value;
	}

	public ICalendar WatchCalendar
	{
		get => _watchCalendar;
		protected init => _watchCalendar = value;
	}

	protected bool DateIsRight()
	{
		return
			(WatchCalendar.CurrentDate.Day == WatchForDay || WatchForDay == -1) &&
			(WatchCalendar.CurrentDate.Month.Alias == WatchForMonth || WatchForMonth.Length == 0) &&
			(WatchCalendar.CurrentDate.Year == WatchForYear || WatchForYear == -1);
	}

	public void DateUpdated()
	{
		if (DateIsRight())
		{
			Payload(Objects);
			RepeatTimes--;
		}
	}

	public void Subscribe()
	{
		if (WatchForDay != -1)
		{
			WatchCalendar.DaysUpdated += DateUpdated;
		}

		if (WatchForMonth.Length > 0)
		{
			WatchCalendar.MonthsUpdated += DateUpdated;
		}

		if (WatchForYear != -1)
		{
			WatchCalendar.YearsUpdated += DateUpdated;
		}
	}

	public override void UnSubscribe()
	{
		if (WatchForDay != -1)
		{
			WatchCalendar.DaysUpdated -= DateUpdated;
		}

		if (WatchForMonth.Length > 0)
		{
			WatchCalendar.MonthsUpdated -= DateUpdated;
		}

		if (WatchForYear != -1)
		{
			WatchCalendar.YearsUpdated -= DateUpdated;
		}
	}

	public override string ToString()
	{
		return $"Date Listener: {WatchForYear}y {WatchForMonth}M {WatchForDay}d";
	}
}
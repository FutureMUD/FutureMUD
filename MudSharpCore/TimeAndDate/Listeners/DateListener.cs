using System;
using System.Text;
using MudSharp.Framework;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.TimeAndDate.Listeners;

public class DateListener : ListenerBase
{
	protected ICalendar _watchCalendar;
	protected int _watchForDay;

	protected string _watchForMonth;

	protected int _watchForYear;
	private IMudTimeZone _watchForTimeZone;

	public DateListener(ICalendar watchCalendar, int watchForDay, string watchForMonth, int watchForYear,
		IMudTimeZone watchForTimeZone,
		int repeatTimes, Action<object[]> payload, object[] objects, string debuggerReference)
		: base(repeatTimes, payload, objects, debuggerReference)
	{
		WatchCalendar = watchCalendar;
		WatchForDay = watchForDay;
		WatchForMonth = watchForMonth;
		WatchForYear = watchForYear;
		_watchForTimeZone = watchForTimeZone;
		if (WatchForDay != -1 && WatchForYear != -1 && WatchForMonth.Length > 0)
		{
			_watchDate = new MudDateTime(WatchCalendar.GetDate($"{WatchForDay}-{WatchForMonth}-{WatchForYear}"), WatchCalendar.FeedClock.GetTime($"{watchForTimeZone.Alias} 0:0:0"), watchForTimeZone);
		}
		Subscribe();
	}

	public DateListener(MudDateTime datetime, int repeatTimes, Action<object[]> payload, object[] objects, string debuggerReference) : base(
		repeatTimes, payload, objects, debuggerReference)
	{
		WatchCalendar = datetime.Calendar;
		WatchForDay = datetime.Date.Day;
		WatchForMonth = datetime.Date.Month.Alias;
		WatchForYear = datetime.Date.Year;
		_watchForTimeZone = datetime.TimeZone;
		_watchDate = datetime;
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

	private readonly MudDateTime _watchDate = null;

	protected bool DateIsRight()
	{
		if (_watchDate is not null)
		{
			return WatchCalendar.CurrentDateTime >= _watchDate;
		}

		var currentDate = WatchCalendar.CurrentDateTime.GetByTimeZone(_watchForTimeZone);
		return
			(currentDate.Date.Day == WatchForDay || WatchForDay == -1) &&
			(currentDate.Date.Month.Alias == WatchForMonth || WatchForMonth.Length == 0) &&
			(currentDate.Date.Year == WatchForYear || WatchForYear == -1);
	}

	public void DateUpdated()
	{
		if (DateIsRight())
		{
			Payload?.Invoke(Objects);
			RepeatTimes--;
		}
	}

	public void Subscribe()
	{
		WatchCalendar.FeedClock.MinutesUpdated -= DateUpdated;
		WatchCalendar.DaysUpdated -= DateUpdated;
		WatchCalendar.MonthsUpdated -= DateUpdated;
		WatchCalendar.YearsUpdated -= DateUpdated;
		WatchCalendar.FeedClock.MinutesUpdated += DateUpdated;
		WatchCalendar.DaysUpdated += DateUpdated;
		WatchCalendar.MonthsUpdated += DateUpdated;
		WatchCalendar.YearsUpdated += DateUpdated;
	}

	public override void UnSubscribe()
	{
		Payload = null;
		WatchCalendar.FeedClock.MinutesUpdated -= DateUpdated;
		WatchCalendar.DaysUpdated -= DateUpdated;
		WatchCalendar.MonthsUpdated -= DateUpdated;
		WatchCalendar.YearsUpdated -= DateUpdated;
	}

	public override string ToString()
	{
		if (_watchDate is not null)
		{
			return $"Date Listener: {_watchDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()} - {DebuggerReference.ColourCommand()} - x{RepeatTimes}";
		}

		var sb = new StringBuilder();
		sb.Append("Date Listener: ");
		sb.Append(Telnet.Green.Colour);
		if (WatchForDay == -1)
		{
			sb.Append("Any day");
		}
		else
		{
			sb.Append($"The {WatchForDay.ToOrdinal()} day");
		}

		if (WatchForMonth.Length == 0)
		{
			sb.Append(" of any month");
		}
		else
		{
			sb.Append($" of the month {WatchForMonth}");
		}

		if (WatchForYear == -1)
		{
			sb.Append(" of any year");
		}
		else
		{
			sb.Append($" of the year {WatchForYear}");
		}

		sb.Append($" in the {_watchForTimeZone.Alias} timezone");

		sb.Append(Telnet.RESETALL);
		sb.Append($" - {DebuggerReference.ColourCommand()} -");

		sb.Append($" x{RepeatTimes}");
		return sb.ToString();
	}
}
using System;
using System.Text;
using MudSharp.Framework;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.TimeAndDate.Listeners;

public class DateListener : ListenerBase
{
	protected ICalendar _watchCalendar;
	protected int _watchForDay;

	protected string _watchForMonth;

	protected int _watchForYear;

	public DateListener(ICalendar watchCalendar, int watchForDay, string watchForMonth, int watchForYear,
		int repeatTimes, Action<object[]> payload, object[] objects, string debuggerReference)
		: base(repeatTimes, payload, objects, debuggerReference)
	{
		WatchCalendar = watchCalendar;
		WatchForDay = watchForDay;
		WatchForMonth = watchForMonth;
		WatchForYear = watchForYear;
		if (WatchForDay != -1 && WatchForYear != -1 && WatchForMonth.Length > 0)
		{
			_watchDate = WatchCalendar.GetDate($"{WatchForDay}-{WatchForMonth}-{WatchForYear}");
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
		if (WatchForDay != -1 && WatchForYear != -1 && WatchForMonth.Length > 0)
		{
			_watchDate = WatchCalendar.GetDate($"{WatchForDay}-{WatchForMonth}-{WatchForYear}");
		}
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

	private readonly MudDate _watchDate = null;

	protected bool DateIsRight()
	{
		if (_watchDate is not null)
		{
			return WatchCalendar.CurrentDate <= _watchDate;
		}

		return
			(WatchCalendar.CurrentDate.Day == WatchForDay || WatchForDay == -1) &&
			(WatchCalendar.CurrentDate.Month.Alias == WatchForMonth || WatchForMonth.Length == 0) &&
			(WatchCalendar.CurrentDate.Year == WatchForYear || WatchForYear == -1);
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
		WatchCalendar.DaysUpdated -= DateUpdated;
		WatchCalendar.MonthsUpdated -= DateUpdated;
		WatchCalendar.YearsUpdated -= DateUpdated;

		if (WatchForDay != -1)
		{
			WatchCalendar.DaysUpdated += DateUpdated;
			return;
		}

		if (WatchForMonth.Length > 0)
		{
			WatchCalendar.MonthsUpdated += DateUpdated;
			return;
		}

		if (WatchForYear != -1)
		{
			WatchCalendar.YearsUpdated += DateUpdated;
			return;
		}
	}

	public override void UnSubscribe()
	{
		Payload = null;
		WatchCalendar.DaysUpdated -= DateUpdated;
		WatchCalendar.MonthsUpdated -= DateUpdated;
		WatchCalendar.YearsUpdated -= DateUpdated;
	}

	public override string ToString()
	{
		if (_watchDate is not null)
		{
			return $"Date Listener: {_watchDate.Display(CalendarDisplayMode.Short).ColourValue()} - {DebuggerReference.ColourCommand()} - x{RepeatTimes}";
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

		sb.Append(Telnet.RESETALL);
		sb.Append($" - {DebuggerReference.ColourCommand()} -");

		sb.Append($" x{RepeatTimes}");
		return sb.ToString();
	}
}
using System;
using System.Linq;
using MudSharp.Framework;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.TimeAndDate.Date;
public class MUDDateException : Exception
{
	public MUDDateException(string message)
		: base(message)
	{
	}
}

public class MudDate : IComparable
{
	/// <summary>
	///     The alias of the calendar in which this date resides
	/// </summary>
	protected ICalendar _calendar;

	/// <summary>
	///     The day of the month of this date
	/// </summary>
	protected int _day;

	protected bool _isPrimaryDate;

	/// <summary>
	///     The month constructed for this date
	/// </summary>
	protected Month _month;

	/// <summary>
	///     The Year class object containing this date
	/// </summary>
	protected Year _thisYear;

	/// <summary>
	///     Day of the week. If blank, it is a non-weekday date (e.g. intercalaries in the Shire Calendar from the Hobbit)
	/// </summary>
	protected string _weekday;

	/// <summary>
	///     Stores the index of the last weekday. This field REMAINS the previous weekday during non-weekday intercalaries, so
	///     it can be incremented upon resumption of weekday flow.
	/// </summary>
	protected int _weekdayIndex;

	/// <summary>
	///     The numeric year of this date
	/// </summary>
	protected int _year;

	/// <summary>
	///     Date constructor
	/// </summary>
	/// <param name="calendar">the calendar in which this date is valid</param>
	/// <param name="day">the day of the month of this date</param>
	/// <param name="year">the numerical year of this date</param>
	/// <param name="month">the generated month for this date</param>
	public MudDate(ICalendar calendar, int day, int year, Month month, Year thisyear, bool isPrimary)
	{
		_calendar = calendar;
		_day = day;
		_year = year;
		_month = month;
		_thisYear = thisyear;
		IsPrimaryDate = isPrimary;

		SetWeekday();
	}


	public MudDate(MudDate copyDate)
	{
		_calendar = copyDate._calendar;
		_day = copyDate._day;
		_year = copyDate._year;
		_month = copyDate._month;
		// TODO: The copy constructor in this next statement may be unneccessary. I didn't want the possibility of something else modifying the Year.
		_thisYear = new Year(copyDate._thisYear);
		SetWeekday();
		IsPrimaryDate = false;
	}

	public static MudDate ParseFromText(string text, IFuturemud gameworld)
	{
		var splitText = text.Split('_');
		var calendar = gameworld.Calendars.Get(long.Parse(splitText[0]));
		return calendar.GetDate(splitText[1]);
	}

	public ICalendar Calendar => _calendar;

	public int Day
	{
		get { return _day; }
		protected set
		{
			_day = value;
			if (IsPrimaryDate)
			{
				Calendar.UpdateDays();
			}
		}
	}

	public int Year
	{
		get { return _year; }
		protected set
		{
			_year = value;
			if (IsPrimaryDate)
			{
				Calendar.UpdateYears();
			}
		}
	}

	public string Weekday => _weekday;

	public int WeekdayIndex => _weekdayIndex;

	public Year ThisYear => _thisYear;

	public Month Month
	{
		get { return _month; }
		protected set
		{
			_month = value;
			if (IsPrimaryDate)
			{
				Calendar.UpdateMonths();
			}
		}
	}

	public bool IsPrimaryDate
	{
		get { return _isPrimaryDate; }
		set { _isPrimaryDate = value; }
	}

	public bool IsIntercalaryDay()
	{
		return false; // TODO
	}

	#region IComparable Members

	public int CompareTo(object obj)
	{
		if (obj is not MudDate dateObject)
		{
			return -1;
		}

		if (this > dateObject)
		{
			return 1;
		}
		if (this < dateObject)
		{
			return -1;
		}
		return 0;
	}

	#endregion

	public MudDate GetDateByTime(MudTime time)
	{
		var newDate = new MudDate(this);
		newDate.AdvanceDays(time.DaysOffsetFromDatum);
		return newDate;
	}

	protected void SetWeekday()
	{
		if (_month.NonWeekdays.Contains(_day))
		{
			_weekday = "";
			_weekdayIndex = -1;
		}
		else
		{
			_weekdayIndex = (_thisYear.FirstWeekdayIndex +
							 _thisYear.Months.FindAll(x => x.TrueOrder < _month.TrueOrder)
								 .ToList()
								 .Sum(x => x.CountWeekdays()) + _day -
							 _month.NonWeekdays.FindAll(x => x <= _day).Count - 1) % _calendar.Weekdays.Count;
			_weekday = _calendar.Weekdays[_weekdayIndex];
		}
	}

	public string Display(CalendarDisplayMode mode)
	{
		return Calendar.DisplayDate(this, mode);
	}


	/// <summary>
	///     Returns a date string representing this date
	/// </summary>
	/// <returns></returns>
	public string GetDateString()
	{
		return Day + "/" + Month.Alias + "/" + Year;
	}

	/// <summary>
	/// Returns a string that will produce the same date if given to the MudDate.ParseFromText function
	/// </summary>
	/// <returns></returns>
	public string GetRoundtripString()
	{
		return $"{Calendar.Id:F}_{Day:F}/{Month.Alias}/{Year:F}";
	}

	public override string ToString()
	{
		return GetDateString();
	}

	public void AdvanceToNextWeekday(int whichWeekday, int times)
	{
		if ((whichWeekday < 0) || (whichWeekday > Calendar.Weekdays.Count - 1))
		{
			throw new ArgumentException("AdvanceToNextWeekday called with out of range weekday.");
		}

		while (times > 0)
		{
			while (true)
			{
				AdvanceDays(1);
				if (WeekdayIndex == whichWeekday)
				{
					break;
				}
			}
			times--;
		}
	}

	public void AdvanceDays(int numberOfDays)
	{
		if (numberOfDays == 0)
		{
			return;
		}

		var oldMonth = _month;
		var oldYear = _year;

		if (numberOfDays > 0)
		{
			InternalAdvanceDays(ref numberOfDays);
		}
		else
		{
			numberOfDays *= -1;
			InternalSubtractDays(ref numberOfDays);
		}
		SetWeekday();
		if (IsPrimaryDate)
		{
			Calendar.UpdateDays();
			if (_month != oldMonth || Year != oldYear)
			{
				Calendar.UpdateMonths();
			}

			if (Year != oldYear)
			{
				Calendar.UpdateYears();
			}
			Calendar.Changed = true;
		}
	}

	protected bool InternalSubtractDays(ref int numberOfDays)
	{
		if (Day > numberOfDays)
		{
			Day -= numberOfDays;
			return true;
		}

		var formerMonths = ThisYear.Months.TakeWhile(x => x != Month).Reverse();
		var passedDaysInYear = formerMonths.Sum(x => x.Days) + Day;
		if (numberOfDays < passedDaysInYear)
		{
			numberOfDays -= Day;
			foreach (var month in formerMonths)
			{
				if (numberOfDays <= month.Days)
				{
					_month = month;
					_day = month.Days - numberOfDays;
					return true;
				}

				numberOfDays -= month.Days;
			}
		}

		numberOfDays -= passedDaysInYear;
		while (true)
		{
			_thisYear = Calendar.CreateYear(--_year);
			_month = _thisYear.Months.Last();
			_day = _month.Days;
			if (InternalSubtractDays(ref numberOfDays))
			{
				return true;
			}
		}
	}

	protected bool InternalAdvanceDays(ref int numberOfDays)
	{
		if (Day + numberOfDays <= Month.Days)
		{
			Day += numberOfDays;
			return true;
		}

		var futureMonths = ThisYear.Months.SkipWhile(x => x != Month).Skip(1);
		var remainingDaysInYear = futureMonths.Sum(x => x.Days) + (Month.Days - Day);
		if (numberOfDays <= remainingDaysInYear)
		{
			numberOfDays -= Month.Days - Day;
			foreach (var month in futureMonths)
			{
				if (numberOfDays <= month.Days)
				{
					_month = month;
					_day = numberOfDays;
					return true;
				}

				numberOfDays -= month.Days;
			}
		}

		numberOfDays -= remainingDaysInYear;
		while (true)
		{
			_thisYear = Calendar.CreateYear(++_year);
			_month = _thisYear.Months.First();
			_day = 0;
			if (InternalAdvanceDays(ref numberOfDays))
			{
				return true;
			}
		}
	}

	protected void OldInternalAdvanceDays(ref int numberOfDays)
	{
		while (true)
		{
			if ((Day + numberOfDays <= Month.Days) && (Day + numberOfDays >= 1))
			{
				Day += numberOfDays;
			}
			else
			{
				if (numberOfDays < 0)
				{
					numberOfDays += Day;
					AdvanceMonths(-1, false, true);
					if (numberOfDays < 0)
					{
						continue;
					}
				}
				else
				{
					numberOfDays -= Month.Days - Day + 1;
					AdvanceMonths(1, false, true);
					if (numberOfDays > 0)
					{
						continue;
					}
				}
			}
			break;
		}
	}

	/// <summary>
	///     advances the current position of the calendar by the specified number of months
	/// </summary>
	/// <param name="numberOfMonths">How many months by which to advance the calendar</param>
	/// <param name="ignoreIntercalaries">
	///     Specifies whether or not to treat an intercalary month as a month for the purposes of
	///     this addition. If true, advancing over an intercalary month costs the algorithm 0 months. If false it costs 1
	/// </param>
	public void AdvanceMonths(int numberOfMonths, bool ignoreIntercalaries, bool preserveDay)
	{

		if (numberOfMonths == 0)
		{
			return;
		}

		var yearsChanged = false;
		while (numberOfMonths != 0)
		{
			if (numberOfMonths < 0)
			{
				var targetCount = Math.Abs(numberOfMonths);
				var previousMonths = _thisYear.Months.TakeWhile(x => x != Month).ToList();
				if (previousMonths.Count >= targetCount)
				{
					_month = previousMonths[Index.FromEnd(targetCount)];
					break;
				}

				numberOfMonths += previousMonths.Count + 1;
				_thisYear = Calendar.CreateYear(--_year);
				_month = _thisYear.Months.Last();
				yearsChanged = true;
				continue;
			}

			var remainingMonths = _thisYear.Months.SkipWhile(x => x != Month).Skip(1).ToList();
			if (remainingMonths.Count >= numberOfMonths)
			{
				_month = remainingMonths.ElementAt(numberOfMonths - 1);
				break;
			}

			numberOfMonths -= remainingMonths.Count + 1;
			_thisYear = Calendar.CreateYear(++_year);
			yearsChanged = true;
			_month = _thisYear.Months[0];
		}

		if (!preserveDay)
		{
			Day = 1;
		}
		else
		{
			if (Day > Month.Days)
			{
				Day = Month.Days;
			}
		}

		SetWeekday();
		if (IsPrimaryDate)
		{
			Calendar.Changed = true;
			Calendar.UpdateMonths();
			Calendar.UpdateDays();
			if (yearsChanged)
			{
				Calendar.UpdateYears();
			}
		}
	}

	public bool Equals(MudDate compareDate)
	{
		return
			(Day == compareDate.Day) &&
			(Month.Alias == compareDate.Month.Alias) &&
			(Year == compareDate.Year) &&
			(Calendar.Id == compareDate.Calendar.Id);
	}

	private static int DaysDifference(MudDate date1, MudDate date2)
	{
		if (date1 < date2)
		{
			(date1, date2) = (date2, date1);
		}

		if (date1.Calendar == date2.Calendar)
		{
			var daysThisYear = date1.Day + date1.ThisYear.Months.Where(x => x.TrueOrder < date1.Month.TrueOrder).Sum(x => x.Days);
			var daysCompareYear = date2.Day + date2.ThisYear.Months
				.Where(x => x.TrueOrder < date2.Month.TrueOrder).Sum(x => x.Days);
			var daysBetweenYears = date1.Calendar.CountDaysBetweenYears(date1.Year, date2.Year);
			return daysThisYear - daysCompareYear + daysBetweenYears;
		}

		return DaysDifference(date1.Calendar.CurrentDate, date1) -
			   DaysDifference(date2.Calendar.CurrentDate, date2);
	}

	public int DaysDifference(MudDate compareDate)
	{
		return DaysDifference(this, compareDate);
	}

	private static int YearsDifference(MudDate date1, MudDate date2)
	{
		if (date1 < date2)
		{
			(date1, date2) = (date2, date1);
		}

		if (date1.Calendar.Id == date2.Calendar.Id)
		{
			return date1.Year - date2.Year +
				   ((date1.Month.TrueOrder > date2.Month.TrueOrder) || (date1.Month.TrueOrder == date2.Month.TrueOrder && (date1.Day >= date2.Day)) ? 1 : 0);
		}

		return YearsDifference(date1.Calendar.CurrentDate, date1) -
			   YearsDifference(date2.Calendar.CurrentDate, date2);
	}

	public int YearsDifference(MudDate compareDate)
	{
		if (Calendar.Id == compareDate.Calendar.Id)
		{
			return Year - compareDate.Year +
				   ((Month.TrueOrder > compareDate.Month.TrueOrder) || (Month.TrueOrder == compareDate.Month.TrueOrder && (Day >= compareDate.Day)) ? 1 : 0);
		}
		return YearsDifference(Calendar.CurrentDate) -
			   compareDate.YearsDifference(compareDate.Calendar.CurrentDate);
	}

	/// <summary>
	///     Advances the current position of the calendar by the specified number of years. Due to how years work, this resets
	///     the date to the first day of that year too.
	/// </summary>
	/// <param name="numberOfYears">How many years by which to advance the calendar</param>
	/// <param name="normaliseDays">
	///     Whether or not to recalculate which day/month it is after advancing that many years.
	///     Usually true when this function is called directly, rather than from within AdvanceMonths
	/// </param>
	public void AdvanceYears(int numberOfYears, bool normaliseDays)
	{
		if (numberOfYears == 0)
		{
			return;
		}

		// Advance the year
		Year += numberOfYears;

		var oldYear = _thisYear;
		// Repopulate the month lists
		_thisYear = Calendar.CreateYear(Year);

		var day = Day;
		var month = Month;

		// Set the new current month
		var newMonth = _thisYear.Months.Find(x => x.Alias == Month.Alias);
		if (newMonth == null)
		{

			if (normaliseDays)
			{
				// Count the number of days from first of the year
				var days = oldYear.Months.TakeWhile(x => x != month).Sum(x => x.Days) + day;
				foreach (var tempMonth in _thisYear.Months)
				{
					if (tempMonth.Days > days)
					{
						newMonth = tempMonth;
						Month = newMonth;
						Day = days;
						break;
					}

					days -= tempMonth.Days;
				}

				if (newMonth is null)
				{
					newMonth = _thisYear.Months.Last();
					Month = newMonth;
					Day = Month.Days;
				}
			}
			else
			{
				if ((Month.NominalOrder == -1) && (Month.TrueOrder > _thisYear.Months.Max(x => x.TrueOrder)))
				{
					newMonth = _thisYear.Months.Find(x => x.TrueOrder == _thisYear.Months.Max(y => y.TrueOrder));
				}
				else if (Month.NominalOrder == -1)
				// In this case, the intercalary doesn't exist but it's not after the end of the year. In this case pick the same true order month
				{
					newMonth = _thisYear.Months.Find(x => x.TrueOrder == Month.TrueOrder);
				}
				else
				{
					throw new Exception("Non-intercalary month absent from year generated in AdvanceYears function.");
				}

				Month = newMonth;
				if (Day > Month.Days)
				{
					Day = Month.Days;
				}
			}
		}
		else
		{
			if (Day > newMonth.Days)
			{
				if (normaliseDays)
				{
					var found = false;
					// Count the number of days from first of the year
					var days = oldYear.Months.TakeWhile(x => x != month).Sum(x => x.Days) + day;
					foreach (var tempMonth in _thisYear.Months)
					{
						if (tempMonth.Days > days)
						{
							Month = tempMonth;
							Day = days;
							found = true;
							break;
						}

						days -= tempMonth.Days;
					}

					if (!found)
					{
						Month = _thisYear.Months.Last();
						Day = Month.Days;
					}
				}
				else
				{
					Day = newMonth.Days;
				}
			}
		}

		SetWeekday();
		if (IsPrimaryDate)
		{
			Calendar.Changed = true;
		}
	}

	public static bool operator <(MudDate date1, MudDate date2)
	{
		// Null date is never. Never is less than all dates except itself
		if ((date1 == null) && (date2 == null))
		{
			return false;
		}
		if (date2 == null)
		{
			return false;
		}
		if (date1 == null)
		{
			return true;
		}

		return (date1.Year < date2.Year) ||
			   ((date1.Year == date2.Year) && (date1.Month.TrueOrder < date2.Month.TrueOrder)) ||
			   ((date1.Year == date2.Year) && (date1.Month.TrueOrder == date2.Month.TrueOrder) &&
				(date1.Day < date2.Day));
	}

	public static bool operator >(MudDate date1, MudDate date2)
	{
		// Null date is never. Never is less than all dates except itself
		if ((date1 == null) && (date2 == null))
		{
			return false;
		}
		if (date2 == null)
		{
			return true;
		}
		if (date1 == null)
		{
			return false;
		}

		return (date1.Year > date2.Year) ||
			   ((date1.Year == date2.Year) && (date1.Month.TrueOrder > date2.Month.TrueOrder)) ||
			   ((date1.Year == date2.Year) && (date1.Month.TrueOrder == date2.Month.TrueOrder) &&
				(date1.Day > date2.Day));
	}

	public static bool operator <=(MudDate date1, MudDate date2)
	{
		if ((date1 == null) && (date2 == null))
		{
			return true;
		}
		if (date2 == null)
		{
			return false;
		}

		if (date1?.Equals(date2) != false)
		{
			return true;
		}

		return (date1.Year < date2.Year) ||
			   ((date1.Year == date2.Year) && (date1.Month.TrueOrder < date2.Month.TrueOrder)) ||
			   ((date1.Year == date2.Year) && (date1.Month.TrueOrder == date2.Month.TrueOrder) &&
				(date1.Day < date2.Day));
	}

	public static bool operator >=(MudDate date1, MudDate date2)
	{
		// Null date is never. Never is less than all dates except itself
		if ((date1 == null) && (date2 == null))
		{
			return true;
		}
		if (date2 == null)
		{
			return true;
		}
		if (date1 == null)
		{
			return false;
		}

		if (date1.Equals(date2))
		{
			return true;
		}

		return (date1.Year > date2.Year) ||
			   ((date1.Year == date2.Year) && (date1.Month.TrueOrder > date2.Month.TrueOrder)) ||
			   ((date1.Year == date2.Year) && (date1.Month.TrueOrder == date2.Month.TrueOrder) &&
				(date1.Day > date2.Day));
	}

	public static TimeSpan operator -(MudDate d1, MudDate d2)
	{
		return TimeSpan.FromDays(d2.DaysDifference(d1));
	}

	public override bool Equals(object obj)
	{
		if (obj is not MudDate objAsDate)
		{
			return false;
		}

		return Calendar.Equals(objAsDate.Calendar) && (Year == objAsDate.Year) && _month.Equals(objAsDate.Month) &&
			   (Day == objAsDate.Day);
	}

	public override int GetHashCode()
	{
		return Calendar.GetHashCode() + Year.GetHashCode() + Month.GetHashCode() + Day.GetHashCode();
	}

	public MudDateTime ToDateTime()
	{
		return new MudDateTime(this,
			new MudTime(0, 0, 0, Calendar.FeedClock.PrimaryTimezone, Calendar.FeedClock, false),
			Calendar.FeedClock.PrimaryTimezone);
	}
}
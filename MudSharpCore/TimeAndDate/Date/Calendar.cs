using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.TimeAndDate.Date;

public class Calendar : SaveableItem, ICalendar
{
	public override string FrameworkItemType => "Calendar";

	public void LoadFromXml(XElement root)
	{
		if (!root.HasElements)
		{
			throw new XmlException("Root without any elements in Calendar LoadFromXML.");
		}

		// Alias
		var element = root.Element("alias");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing alias value in Calendar LoadFromXML.");
		}

		// All check
		Alias = element.Value;

		// Description
		element = root.Element("description");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing description value in Calendar LoadFromXML.");
		}

		Description = element.Value;

		// Full Name
		element = root.Element("fullname");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing fullname value in Calendar LoadFromXML.");
		}

		FullName = element.Value;

		// ShortName
		element = root.Element("shortname");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing shortname value in Calendar LoadFromXML.");
		}

		ShortName = element.Value;

		// ShortString
		element = root.Element("shortstring");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing shortstring value in Calendar LoadFromXML.");
		}

		ShortString = element.Value;

		// LongString
		element = root.Element("longstring");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing longstring value in Calendar LoadFromXML.");
		}

		LongString = element.Value;

		// WordyString
		element = root.Element("wordystring");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing wordystring value in Calendar LoadFromXML.");
		}

		WordyString = element.Value;

		// Plane
		element = root.Element("plane");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing plane value in Calendar LoadFromXML.");
		}

		Plane = element.Value;

		// First Weekday at Epoch
		element = root.Element("weekdayatepoch");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing weekdayatepoch value in Calendar LoadFromXML.");
		}

		try
		{
			FirstWeekdayAtEpoch = int.Parse(element.Value);
		}
		catch
		{
			throw new XmlException("Value for weekdayatepoch in Calendar LoadFromXML is not a valid Integer");
		}

		// Era strings
		element = root.Element("ancienterashortstring");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing Ancient Era Short String in Calendar LoadFromXml.");
		}

		AncientEraShortString = element.Value;

		element = root.Element("ancienteralongstring");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing Ancient Era Long String in Calendar LoadFromXml.");
		}

		AncientEraLongString = element.Value;

		element = root.Element("modernerashortstring");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing Modern Era Short String in Calendar LoadFromXml.");
		}

		ModernEraShortString = element.Value;

		element = root.Element("moderneralongstring");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing Modern Era Long String in Calendar LoadFromXml.");
		}

		ModernEraLongString = element.Value;

		// Epoch Year
		element = root.Element("epochyear");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing epochyear value in Calendar LoadFromXML.");
		}

		try
		{
			EpochYear = int.Parse(element.Value);
		}
		catch
		{
			throw new XmlException("Value for epochyear in Calendar LoadFromXML is not a valid Integer");
		}

		// Weekdays
		element = root.Element("weekdays");
		if (element?.HasElements != true)
		{
			throw new XmlException("Calendar has no weekdays defined in Calendar LoadFromXml.");
		}

		foreach (var subElement in element.Elements("weekday"))
		{
			if (subElement.Value.Length == 0)
			{
				throw new XmlException("Malformed weekday in Calendar LoadFromXML.");
			}

			Weekdays.Add(subElement.Value);
		}

		// Months
		element = root.Element("months");
		if (element?.HasElements != true)
		{
			throw new XmlException("Missing months definition in Calendar LoadFromXml.");
		}

		foreach (var subElement in element.Elements("month"))
		{
			var month = new MonthDefinition();
			month.LoadFromXml(subElement);
			_months.Add(month);
		}

		// Intercalaries
		element = root.Element("intercalarymonths");
		if (element?.HasElements == true) // Intercalaries are not mandatory, unlike other fields.
		{
			foreach (var subElement in element.Elements("intercalarymonth"))
			{
				var intercalary = new IntercalaryMonth();
				intercalary.LoadFromXml(subElement);
				_intercalaries.Add(intercalary);
			}
		}
	}

	public XElement SaveToXml()
	{
		return new XElement("calendar", new XElement("alias", Alias), new XElement("shortname", ShortName),
			new XElement("fullname", FullName), new XElement("description", new XCData(Description)),
			new XElement("shortstring", ShortString), new XElement("longstring", LongString),
			new XElement("wordystring", WordyString), new XElement("plane", Plane),
			new XElement("feedclock", ClockID), new XElement("epochyear", EpochYear),
			new XElement("weekdayatepoch", FirstWeekdayAtEpoch),
			new XElement("ancienterashortstring", AncientEraShortString),
			new XElement("ancienteralongstring", AncientEraLongString),
			new XElement("modernerashortstring", ModernEraShortString),
			new XElement("modernerashortstring", ModernEraLongString), new XElement("weekdays",
				new object[]
				{
					from wd in Weekdays
					select new XElement("weekday", wd)
				}
			), new XElement("months",
				new object[]
				{
					from mon in _months
					select mon.SaveToXml()
				}
			), new XElement("intercalarymonths",
				new object[]
				{
					from ic in _intercalaries
					select ic.SaveToXml()
				}
			));
	}

	#region Overrides of FrameworkItem

	public override string Name => ShortName;

	#endregion

	#region ISaveable Members

	public override void Save()
	{
		using (new FMDB())
		{
			var dbcal = FMDB.Context.Calendars.Find(Id);
			dbcal.Date = CurrentDate.GetDateString();
			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	#endregion

	#region Events

	public event CalendarEventHandler DaysUpdated;
	public event CalendarEventHandler MonthsUpdated;
	public event CalendarEventHandler YearsUpdated;

	#endregion

	#region Event Handlers

	public void UpdateDays()
	{
		DaysUpdated?.Invoke();
	}

	public void UpdateMonths()
	{
		MonthsUpdated?.Invoke();
	}

	public void UpdateYears()
	{
		YearsUpdated?.Invoke();
	}

	#endregion

	#region Properties

	// Unique Alias of the Calendar
	protected string _alias;

	public string Alias
	{
		get => _alias;

		protected set => _alias = value;
	}

	// Short Name of the Calendar, i.e. Julian Calendar (Eng-DL)
	protected string _shortName;

	public string ShortName
	{
		get => _shortName;

		protected set => _shortName = value;
	}

	// Full Name of the Calendar, i.e. The Julian Calendar of of England Under Danelaw
	protected string _fullName;

	public string FullName
	{
		get => _fullName;

		protected set => _fullName = value;
	}

	IEnumerable<string> IHaveMultipleNames.Names => [Name, Alias, ShortName, FullName];

	// A verbose description of the calendar
	protected string _description;

	public string Description
	{
		get => _description;

		protected set => _description = value;
	}

	/// <summary>
	///     The alias of the plane in which this calendar is designed to act
	/// </summary>
	protected string _plane;

	public string Plane
	{
		get => _plane;
		protected set => _plane = value;
	}

	protected string _shortString;

	public string ShortString
	{
		get => _shortString;
		protected set => _shortString = value;
	}

	protected string _longString;

	public string LongString
	{
		get => _longString;
		protected set => _longString = value;
	}

	protected string _wordyString;

	public string WordyString
	{
		get => _wordyString;
		protected set => _wordyString = value;
	}

	protected IClock _feedClock;

	public IClock FeedClock
	{
		get => _feedClock;
		set
		{
			if (_feedClock != null)
			{
				_feedClock.DaysUpdated -= CurrentDate.AdvanceDays;
			}

			_feedClock = value;
			if (CurrentDate != null)
			{
				_feedClock.DaysUpdated += CurrentDate.AdvanceDays;
			}

			if (_clockID == 0)
			{
				_clockID = _feedClock.Id;
			}
		}
	}

	protected long _clockID;

	public long ClockID
	{
		get => _clockID;
		set
		{
			if (Gameworld.Clocks.Has(value))
			{
				_clockID = value;
				FeedClock = Gameworld.Clocks.Get(_clockID);
			}
		}
	}

	public string ModernEraShortString { get; set; }
	public string ModernEraLongString { get; set; }
	public string AncientEraShortString { get; set; }
	public string AncientEraLongString { get; set; }

	protected MudDate _yesterday;
	protected MudDate _tomorrow;

	protected MudDate _currentDate;

	public MudDate CurrentDate
	{
		get => _currentDate;
		protected set
		{
			if (_currentDate != null && FeedClock != null)
			{
				FeedClock.DaysUpdated -= _currentDate.AdvanceDays;
			}

			_currentDate = value;

			if (FeedClock != null)
			{
				FeedClock.DaysUpdated += _currentDate.AdvanceDays;
			}
		}
	}

	public MudDateTime CurrentDateTime => new(CurrentDate, FeedClock.CurrentTime, FeedClock.PrimaryTimezone);

	/// <summary>
	///     The year in which this calendar's other epoch data (such as first weekday) begins. This should be as close to the
	///     most commonly used dates as possible
	/// </summary>
	protected int _epochYear;

	public int EpochYear
	{
		get => _epochYear;
		protected set => _epochYear = value;
	}

	/// <summary>
	///     Index of the first weekday of the epoch year
	/// </summary>
	protected int _firstWeekdayAtEpoch;

	public int FirstWeekdayAtEpoch
	{
		get => _firstWeekdayAtEpoch;
		protected set => _firstWeekdayAtEpoch = value;
	}

	protected List<string> _weekdays = new();

	public List<string> Weekdays => _weekdays;

	/// <summary>
	///     This is the list of month definitions for a standard year in this calendar, ordered as the months themselves should
	///     be ordered
	/// </summary>
	protected List<MonthDefinition> _months = new();

	public List<MonthDefinition> Months
	{
		get => _months;
		protected set => _months = value;
	}

	/// <summary>
	///     This is the list of intercalary months for this calendar. The order is unimportant unless the intercalary months
	///     have rules that need to be applied in a particular order.
	/// </summary>
	protected List<IntercalaryMonth> _intercalaries = new();

	public List<IntercalaryMonth> Intercalaries
	{
		get => _intercalaries;
		protected set => _intercalaries = value;
	}

	#endregion

	#region Constructors

	public Calendar()
	{
	}

	public Calendar(XElement file)
	{
		LoadFromXml(file);
	}

	public Calendar(int id, IFuturemud game)
	{
		_id = id;
		Gameworld = game;
	}

	public Calendar(MudSharp.Models.Calendar calendar, IFuturemud game)
	{
		_id = calendar.Id;
		Gameworld = game;
		LoadFromXml(XElement.Parse(calendar.Definition));
		CurrentDate = GetDate(calendar.Date);
		CurrentDate.IsPrimaryDate = true;
		ClockID = calendar.FeedClockId;
	}

	#endregion

	#region Methods

	private Dictionary<int, Year> _cachedYears = new();

	/// <summary>
	///     Generates a new year (An ordered list of Months) accurate for the given calendar year
	/// </summary>
	/// <param name="whichYear">The numerical year to generate</param>
	/// <returns>A list of months generated correctly for that year</returns>
	public Year CreateYear(int whichYear)
	{
		if (_cachedYears.TryGetValue(whichYear, out var year))
		{
			return year;
		}

		var returnList = new List<Month>();
		returnList.AddRange(Months.Select(x => new Month(x, whichYear)));
		returnList.AddRange(
			Intercalaries.Where(x => x.Rule.IsIntercalaryYear(whichYear)).Select(x => new Month(x.Month, whichYear)));
		returnList = returnList.OrderBy(x => x.NominalOrder).ToList();
		var newYear = new Year(returnList, whichYear, this);
		_cachedYears[whichYear]	 = newYear;
		return newYear;
	}

	private Dictionary<int, int> _cachedWeekdaysInYear = new();

	/// <summary>
	///     This function counts the number of weekdays in a year - that is, days that are not specifically excluded from being
	///     a weekday. It is used internally in the function GetFirstWeekday
	/// </summary>
	/// <param name="whichYear">The year to evaluate</param>
	/// <returns>The number of weekdays in the year</returns>
	public int CountWeekdaysInYear(int whichYear)
	{
		if (_cachedWeekdaysInYear.TryGetValue(whichYear, out var year))
		{
			return year;
		}

		var sum = _months.Sum(x => x.NormalDays - x.NonWeekdays.Count)
		       +
		       Months.Sum(
			       x =>
				       x.Intercalaries.Sum(
					       y =>
						       y.Rule.IsIntercalaryYear(whichYear)
							       ? y.InsertNumnewDays - y.NonWeekdays.Count + y.RemoveNonWeekdays.Count
							       : 0))
		       +
		       Intercalaries.Sum(
			       x =>
				       x.Rule.IsIntercalaryYear(whichYear)
					       ? x.Month.NormalDays - x.Month.NonWeekdays.Count +
					         x.Month.Intercalaries.Sum(
						         y =>
							         y.Rule.IsIntercalaryYear(whichYear)
								         ? y.InsertNumnewDays - y.NonWeekdays.Count + y.RemoveNonWeekdays.Count
								         : 0)
					       : 0)
			;
		_cachedWeekdaysInYear[whichYear] = sum;
		return sum;
	}


	private Dictionary<int, int> _cachedDaysInYear = new();

	/// <summary>
	///     This function counts the number of days in a year.
	/// </summary>
	/// <param name="whichYear">The year to evaluate</param>
	/// <returns>The number of days in the year</returns>
	public int CountDaysInYear(int whichYear)
	{
		if (_cachedDaysInYear.TryGetValue(whichYear, out var year))
		{
			return year;
		}
		var sum = _months.Sum(x => x.NormalDays)
		       +
		       Months.Sum(
			       x => x.Intercalaries.Sum(y => y.Rule.IsIntercalaryYear(whichYear) ? y.InsertNumnewDays : 0))
		       +
		       Intercalaries.Sum(
			       x =>
				       x.Rule.IsIntercalaryYear(whichYear)
					       ? x.Month.NormalDays +
					         x.Month.Intercalaries.Sum(
						         y => y.Rule.IsIntercalaryYear(whichYear) ? y.InsertNumnewDays : 0)
					       : 0)
			;
		_cachedDaysInYear[whichYear] = sum;
		return sum;
	}

	private Dictionary<(int, int), int> _cachedDaysBetweenYears = new();
	/// <summary>
	///     This function returns the number of days between two years exclusive of the endYear
	/// </summary>
	/// <param name="startYear">The first year to count</param>
	/// <param name="endYear">The last year, which is not counted</param>
	/// <returns>The number of days in the year</returns>
	public int CountDaysBetweenYears(int startYear, int endYear)
	{
		if (startYear == endYear)
		{
			return 0;
		}

		if (startYear > endYear)
		{
			(startYear, endYear) = (endYear, startYear);
		}

		if (_cachedDaysBetweenYears.TryGetValue((startYear, endYear), out var count))
		{
			return count;
		}

		count = 0;
		while (startYear < endYear)
		{
			count += CountDaysInYear(startYear++);
		}

		_cachedDaysBetweenYears[(startYear, endYear)] = count;
		return count;
	}

	private Dictionary<int, int> _cachedFirstWeekday = new();
	/// <summary>
	///     Returns the index of the first weekday of the year for the specified year. This may not be the first day of the
	///     year if the first day is excluded from weekdays.
	/// </summary>
	/// <param name="whichYear">Which year to compare</param>
	/// <returns>The index of the first weekday of the year</returns>
	public int GetFirstWeekday(int whichYear)
	{
		if (whichYear == EpochYear)
		{
			return FirstWeekdayAtEpoch;
		}

		if (_cachedFirstWeekday.TryGetValue(whichYear, out var count))
		{
			return count;
		}

		var daysBetween = 0;
		var lowerYear = Math.Min(whichYear, EpochYear);
		var upperYear = Math.Max(whichYear, EpochYear);

		for (var i = lowerYear; i < upperYear; i++)
		{
			daysBetween += CountWeekdaysInYear(i);
		}

		var day = whichYear > EpochYear
			? (FirstWeekdayAtEpoch + daysBetween) % Weekdays.Count
			: Weekdays.Count - Math.Abs((FirstWeekdayAtEpoch - daysBetween) % Weekdays.Count) == 7
				? 0
				: Weekdays.Count - Math.Abs((FirstWeekdayAtEpoch - daysBetween) % Weekdays.Count);
		_cachedFirstWeekday[whichYear] = day;
		return day;
	}

	/// <summary>
	///     Sets the current position of the calendar to the specified date string. Throws exceptions if it runs into errors
	/// </summary>
	/// <param name="dateString">
	///     The date string is in format dd-mm-yyyy, where dd is the internal numerical value of the day,
	///     mm is the string alias of the month, and yyyy is the numerical year. For example, 1-mar-2012 is the 1st day of the
	///     month of march in the year 2012
	/// </param>
	public void SetDate(string dateString)
	{
		CurrentDate = GetDate(dateString);
		CurrentDate.IsPrimaryDate = true;
		_yesterday = GetDate(dateString);
		_yesterday.AdvanceDays(-1);
		_tomorrow = GetDate(dateString);
		_tomorrow.AdvanceDays(1);
	}

	public MudDate GetDateInYear(int day, string month, int year)
	{
		var MUDYear = CreateYear(year);
		var MUDMonth = MUDYear.Months.FirstOrDefault(x => x.Alias == month);
		if (MUDMonth == null)
		{
			throw new MUDDateException("There is no month with that alias in that year.");
		}

		if (day > MUDMonth.Days)
		{
			throw new MUDDateException("That month does not have that many days in that year.");
		}

		if (day < 1)
		{
			throw new MUDDateException("The day must be greater than or equal to 1.");
		}

		return new MudDate(this, day, year, MUDMonth, MUDYear, false);
	}

	public MudDate GetBirthday(int day, string month, int age)
	{
		MudDate date = null;
		try
		{
			date = GetDateInYear(day, month, CurrentDate.Year - age);
		}
		catch (MUDDateException)
		{
		}

		if (date == null || date.YearsDifference(CurrentDate) > age)
		{
			date = GetDateInYear(day, month, CurrentDate.Year - age + 1);
		}

		return date;
	}

	public MudDate GetRandomBirthday(int age)
	{
		// TODO - This isn't 100% correct, only nearly correct.
		var year = CreateYear(CurrentDate.Year - age);
		var month = year.Months.GetWeightedRandom(x => x.Days);
		return new MudDate(this, Constants.Random.Next(1, month.Days + 1), year.YearName, month, year, false);
	}

	/// <summary>
	///     Creates a new Date based on a date string. Throws an exception if it runs into an error.
	/// </summary>
	/// <param name="dateString">
	///     The date string is in format dd-mm-yyyy, where dd is the internal numerical value of the day,
	///     mm is the string alias of the month, and yyyy is the numerical year. For example, 1-mar-2012 is the 1st day of the
	///     month of march in the year 2012
	/// </param>
	/// <returns>A new object of class Date containing the date information for the specified date in this calendar</returns>
	public MudDate GetDate(string dateString)
	{
		// TODO - possibly make a culture specific version of this
		char[] splitOptions = { '-', '/', ' ' };
		var dateStringSplit = dateString.Split('/').ToList();
		if (dateStringSplit.Count != 3)
		{
			dateStringSplit = dateString.Split(splitOptions).ToList();
			if (dateStringSplit.Count != 3)
			{
				throw new MUDDateException(
					$"The date string {dateString} is not in a valid format. It must be in the format dd-mmm-yyyy or dd/mmm/yyyy- e.g. 12-march-2012, 12-03-2012 or 12/03/2012.");
			}
		}

		string dayText;
		string monthText;
		if (dateStringSplit[0].GetIntFromOrdinal() != null)
		{
			dayText = dateStringSplit[0];
			monthText = dateStringSplit[1];
		}
		else
		{
			if (dateStringSplit[1].GetIntFromOrdinal() != null)
			{
				dayText = dateStringSplit[1];
				monthText = dateStringSplit[0];
			}
			else
			{
				throw new MUDDateException("The day must be a number.");
			}
		}

		var setDay = dayText.GetIntFromOrdinal() ?? 0;

		if (setDay < 1)
		{
			throw new MUDDateException("The day must be a positive integer.");
		}

		if (!int.TryParse(dateStringSplit[2], out var setYear))
		{
			throw new MUDDateException("The year must be a number.");
		}

		// Generate the nominated year
		var newYear = CreateYear(setYear);

		// Check to see the month exists
		var month =
			newYear.Months.FirstOrDefault(
				x => x.Alias.Equals(monthText, StringComparison.InvariantCultureIgnoreCase)) ??
			newYear.Months.FirstOrDefault(
				x => x.FullName.Equals(monthText, StringComparison.InvariantCultureIgnoreCase)) ??
			newYear.Months.FirstOrDefault(
				x => x.ShortName.Equals(monthText, StringComparison.InvariantCultureIgnoreCase));

		if (month == null)
		{
			if (!int.TryParse(monthText, out var numMonth))
			{
				throw new MUDDateException("There will be no month with an alias of " + monthText + " in the year " +
				                           dateStringSplit[2]);
			}

			month = newYear.Months.FirstOrDefault(x => x.TrueOrder == numMonth);
			if (month == null)
			{
				throw new MUDDateException("There is no " + numMonth.ToOrdinal() + " month in the year " +
				                           dateStringSplit[2]);
			}
		}

		// Check to see the month contains that many days
		if (month.Days < setDay)
		{
			throw new MUDDateException("The month of " + month.FullName + " in the year " + dateStringSplit[2] +
			                           " does not have " + setDay + " days.");
		}

		return new MudDate(this, setDay, setYear, month, newYear, false);
	}

	public static string StandardDateParsingHelp => @"You can enter dates in one of several formats:

#3<day>/<month>/<year>#0 or #3<month>/<day>/<year>#0 are both fine if the month is the name or alias of the month, e.g. #312/Jan/2022#0 or #3July-04-1788#0

If you use all numbers, your input will be interpreted using the settings of your account culture - this may mean that it is read as #3day/month/year#0 (e.g. UK/Europe), #3month/day/year#0 (e.g. US) or #3year/month/day#0 (e.g. East Asia).

You can also use #3/#0, #3-#0 or spaces to separate the three parts of your date.";

	public bool TryGetDate(string dateString, IFormatProvider format, out MudDate date, out string error)
	{
		char[] splitOptions = { '-', '/', ' ' };
		var dateStringSplit = dateString.Split('/').ToList();
		if (dateStringSplit.Count != 3)
		{
			dateStringSplit = dateString.Split(splitOptions).ToList();
			if (dateStringSplit.Count != 3)
			{
				error =
					$"The date string {dateString.ColourCommand()} is not in a valid format.\n{StandardDateParsingHelp.SubstituteANSIColour()}";
				date = new MudDate(CurrentDate);
				return false;
			}
		}

		int dayNumber;
		int monthNumber;
		int yearNumber;
		Year newYear;
		Month newMonth;

		// All numerical dates are ambiguous between cultures, let's apply account culture to figure it out
		if (dateStringSplit[0].GetIntFromOrdinal() is not null && dateStringSplit[1].GetIntFromOrdinal() is not null && dateStringSplit[2].GetIntFromOrdinal() is not null)
		{
			var dtformat = format as System.Globalization.CultureInfo ??
			               format?.GetFormat(typeof(System.Globalization.CultureInfo)) as System.Globalization.CultureInfo ??
			               (format as IPerceiver)?.Account.Culture ??
			               System.Globalization.CultureInfo.InvariantCulture;
			DateUtilities.DateOrder treatment;
			try
			{
				treatment = DateUtilities.GetDateOrder(dtformat);
			}
			catch
			{
				treatment = DateUtilities.DateOrder.DayFirst;
			}

			switch (treatment)
			{
				case DateUtilities.DateOrder.DayFirst:
					dayNumber = dateStringSplit[0].GetIntFromOrdinal()!.Value;
					monthNumber = dateStringSplit[1].GetIntFromOrdinal()!.Value;
					yearNumber = dateStringSplit[2].GetIntFromOrdinal()!.Value;
					break;
				case DateUtilities.DateOrder.MonthFirst:
					dayNumber = dateStringSplit[1].GetIntFromOrdinal()!.Value;
					monthNumber = dateStringSplit[0].GetIntFromOrdinal()!.Value;
					yearNumber = dateStringSplit[2].GetIntFromOrdinal()!.Value;
					break;
				case DateUtilities.DateOrder.YearFirst:
					dayNumber = dateStringSplit[2].GetIntFromOrdinal()!.Value;
					monthNumber = dateStringSplit[1].GetIntFromOrdinal()!.Value;
					yearNumber = dateStringSplit[0].GetIntFromOrdinal()!.Value;
					break;
				default:
					goto case DateUtilities.DateOrder.DayFirst;
			}

			newYear = CreateYear(yearNumber);
			newMonth = newYear.Months.FirstOrDefault(x => x.TrueOrder == monthNumber);
			if (newMonth == null)
			{
				error = $"There is no {monthNumber.ToOrdinal().ColourValue()} month in the year {yearNumber.ToStringN0Colour(format)}.";
				date = new MudDate(CurrentDate);
				return false;
			}
		}
		else
		{
			string dayText;
			string monthText;
			if (dateStringSplit[0].GetIntFromOrdinal() != null)
			{
				dayText = dateStringSplit[0];
				monthText = dateStringSplit[1];
			}
			else
			{
				if (dateStringSplit[1].GetIntFromOrdinal() != null)
				{
					dayText = dateStringSplit[1];
					monthText = dateStringSplit[0];
				}
				else
				{
					error = "The day must be a number.";
					date = new MudDate(CurrentDate);
					return false;
				}
			}

			dayNumber = dayText.GetIntFromOrdinal() ?? 0;

			if (dayNumber < 1)
			{
				error = "The day must be a positive integer.";
				date = new MudDate(CurrentDate);
				return false;
			}

			if (!int.TryParse(dateStringSplit[2], out yearNumber))
			{
				error = "The year must be a number.";
				date = new MudDate(CurrentDate);
				return false;
			}

			// Generate the nominated year
			newYear = CreateYear(yearNumber);

			// Check to see the month exists
			newMonth =
				newYear.Months.FirstOrDefault(
					x => x.Alias.Equals(monthText, StringComparison.InvariantCultureIgnoreCase)) ??
				newYear.Months.FirstOrDefault(
					x => x.FullName.Equals(monthText, StringComparison.InvariantCultureIgnoreCase)) ??
				newYear.Months.FirstOrDefault(
					x => x.ShortName.Equals(monthText, StringComparison.InvariantCultureIgnoreCase));

			if (newMonth == null)
			{
				if (!int.TryParse(monthText, out monthNumber))
				{
					error = $"There will be no month with an alias of {monthText.ColourCommand()} in the year {yearNumber.ToStringN0Colour(format)}";
					date = new MudDate(CurrentDate);
					return false;
				}

				newMonth = newYear.Months.FirstOrDefault(x => x.TrueOrder == monthNumber);
				if (newMonth == null)
				{
					error = $"There is no {monthNumber.ToOrdinal().ColourValue()} month in the year {yearNumber.ToStringN0Colour(format)}";
					date = new MudDate(CurrentDate);
					return false;
				}
			}
		}

		// Check to see the month contains that many days
		if (newMonth.Days < dayNumber)
		{
			error = $"The month of {newMonth.FullName.ColourName()} in the year {yearNumber.ToStringN0Colour(format)} does not have {dayNumber.ToStringN0Colour(format)} days.";
			date = new MudDate(CurrentDate);
			return false;
		}

		error = string.Empty;
		date = new MudDate(this, dayNumber, yearNumber, newMonth, newYear, false);
		return true;
	}

	public bool TryGetDate(string dateString, out MudDate date, out string error)
	{
		// TODO - possibly make a culture specific version of this
		char[] splitOptions = { '-', '/', ' ' };
		var dateStringSplit = dateString.Split('/').ToList();
		if (dateStringSplit.Count != 3)
		{
			dateStringSplit = dateString.Split(splitOptions).ToList();
			if (dateStringSplit.Count != 3)
			{
				error =
					$"The date string {dateString} is not in a valid format. It must be in the format dd-mmm-yyyy or dd/mmm/yyyy- e.g. 12-march-2012, 12-03-2012 or 12/03/2012.";
				date = new MudDate(CurrentDate);
				return false;
			}
		}

		string dayText;
		string monthText;
		if (dateStringSplit[0].GetIntFromOrdinal() != null)
		{
			dayText = dateStringSplit[0];
			monthText = dateStringSplit[1];
		}
		else
		{
			if (dateStringSplit[1].GetIntFromOrdinal() != null)
			{
				dayText = dateStringSplit[1];
				monthText = dateStringSplit[0];
			}
			else
			{
				error = "The day must be a number.";
				date = new MudDate(CurrentDate);
				return false;
			}
		}

		var setDay = dayText.GetIntFromOrdinal() ?? 0;

		if (setDay < 1)
		{
			error = "The day must be a positive integer.";
			date = new MudDate(CurrentDate);
			return false;
		}

		if (!int.TryParse(dateStringSplit[2], out var setYear))
		{
			error = "The year must be a number.";
			date = new MudDate(CurrentDate);
			return false;
		}

		// Generate the nominated year
		var newYear = CreateYear(setYear);

		// Check to see the month exists
		var month =
			newYear.Months.FirstOrDefault(
				x => x.Alias.Equals(monthText, StringComparison.InvariantCultureIgnoreCase)) ??
			newYear.Months.FirstOrDefault(
				x => x.FullName.Equals(monthText, StringComparison.InvariantCultureIgnoreCase)) ??
			newYear.Months.FirstOrDefault(
				x => x.ShortName.Equals(monthText, StringComparison.InvariantCultureIgnoreCase));

		if (month == null)
		{
			if (!int.TryParse(monthText, out var numMonth))
			{
				error = $"There will be no month with an alias of {monthText} in the year {dateStringSplit[2]}";
				date = new MudDate(CurrentDate);
				return false;
			}

			month = newYear.Months.FirstOrDefault(x => x.TrueOrder == numMonth);
			if (month == null)
			{
				error = $"There is no {numMonth.ToOrdinal()} month in the year {dateStringSplit[2]}";
				date = new MudDate(CurrentDate);
				return false;
			}
		}

		// Check to see the month contains that many days
		if (month.Days < setDay)
		{
			error = $"The month of {month.FullName} in the year {dateStringSplit[2]} does not have {setDay} days.";
			date = new MudDate(CurrentDate);
			return false;
		}

		error = string.Empty;
		date = new MudDate(this, setDay, setYear, month, newYear, false);
		return true;
	}

	public string DisplayDate(CalendarDisplayMode mode)
	{
		return DisplayDate(CurrentDate, mode);
	}

	public string DisplayDate(string mask)
	{
		return DisplayDate(CurrentDate, mask);
	}

	public string DisplayDate(MudDate theDate, CalendarDisplayMode mode)
	{
		return DisplayDate(theDate,
			mode == CalendarDisplayMode.Short
				? ShortString
				: mode == CalendarDisplayMode.Long
					? LongString
					: WordyString);
	}

	private static readonly Regex DateRegex = new(@"\$(?<code>[a-z]{2,2})", RegexOptions.IgnoreCase);

	public string DisplayDate(MudDate theDate, string mask)
	{
		return DateRegex.Replace(mask, m =>
		                {
			                switch (m.Groups["code"].Value)
			                {
				                case "dd":
					                return theDate.Day.ToString();
				                case "DD":
					                return theDate.Day.ToWordyNumber();
				                case "dt":
					                return theDate.Day.ToOrdinal();
				                case "DT":
					                return theDate.Day.ToWordyOrdinal();
								case "dr":
									return theDate.Day.ToRomanNumeral();
				                case "nn":
					                return theDate.Month.GetDayName(theDate);
				                case "NN":
					                return theDate.Month.GetFullDayName(theDate);
				                case "ns":
					                return theDate.Month.GetDayName(theDate) == ""
						                ? ""
						                : $"{theDate.Month.GetDayName(theDate)} ";
				                case "NS":
					                return theDate.Month.GetFullDayName(theDate) == ""
						                ? ""
						                : $"{theDate.Month.GetFullDayName(theDate)} ";
				                case "nz":
					                return theDate.Month.GetDayName(theDate) == ""
						                ? ""
						                : $"{theDate.Month.GetDayName(theDate)}, ";
				                case "NZ":
					                return theDate.Month.GetFullDayName(theDate) == ""
						                ? ""
						                : $"{theDate.Month.GetFullDayName(theDate)}, ";
				                case "mm":
					                return theDate.Month.Alias;
				                case "ms":
					                return theDate.Month.ShortName;
				                case "mf":
					                return theDate.Month.FullName;
				                case "yy":
					                return theDate.Year < 1
						                ? (theDate.Year * -1 + 1).ToString()
						                : theDate.Year.ToString();
				                case "YY":
					                return theDate.Year < 1
						                ? (theDate.Year * -1 + 1).ToWordyNumber()
						                : theDate.Year.ToWordyNumber();
				                case "yo":
					                return theDate.Year < 1
						                ? (theDate.Year * -1 + 1).ToOrdinal()
						                : theDate.Year.ToOrdinal();
				                case "YO":
					                return theDate.Year < 1
						                ? (theDate.Year * -1 + 1).ToWordyOrdinal()
						                : theDate.Year.ToWordyOrdinal();
				                case "ww":
					                return theDate.Month.NonWeekdays.Contains(theDate.Day) ? "" : theDate.Weekday;
				                case "cc":
					                return theDate.Year < 1
						                ? ((theDate.Year * -1 + 1) / 100 + 1).ToOrdinal()
						                : (theDate.Year / 100 + 1).ToOrdinal();
				                case "CC":
					                return theDate.Year < 1
						                ? ((theDate.Year * -1 + 1) / 100 + 1).ToWordyOrdinal()
						                : (theDate.Year / 100 + 1).ToWordyOrdinal();
				                case "mi":
					                return theDate.Year < 1
						                ? ((theDate.Year * -1 + 1) / 1000 + 1).ToOrdinal()
						                : (theDate.Year / 1000 + 1).ToOrdinal();
				                case "MI":
					                return theDate.Year < 1
						                ? ((theDate.Year * -1 + 1) / 1000 + 1).ToWordyOrdinal()
						                : (theDate.Year / 1000 + 1).ToWordyOrdinal();
				                case "my":
					                return theDate.Year < 1
						                ? ((theDate.Year * -1 + 1) % 1000).ToOrdinal()
						                : (theDate.Year % 1000).ToOrdinal();
				                case "MY":
					                return theDate.Year < 1
						                ? ((theDate.Year * -1 + 1) % 1000).ToWordyOrdinal()
						                : (theDate.Year % 1000).ToWordyOrdinal();
				                case "mo":
					                return theDate.Month.NominalOrder.ToString();
				                case "MO":
					                return theDate.Month.NominalOrder.ToWordyNumber();
				                case "MT":
					                return theDate.Month.NominalOrder.ToWordyOrdinal();
				                case "ee":
					                return theDate.Year < 1 ? AncientEraShortString : ModernEraShortString;
				                case "EE":
					                return theDate.Year < 1 ? AncientEraLongString : ModernEraLongString;
			                }

			                return m.Value;
		                })
		                .NormaliseSpacing();
	}

	#endregion

	#region Purely for testing

	public void SetupTestData()
	{
		_id = 1;
		Alias = "gregorian";
		Description = "The calendar created by pope Gregory to replace the Julian calendar. English edition.";
		ShortName = "Gregorian Calendar (EN-UK)";
		FullName = "The Gregorian Calendar, in English with British Date Display, circa 2012";
		Plane = "earth";
		ClockID = 1;

		ShortString = "$dd/$mm/$yy";
		LongString = "$nz$ww the $dt of $mf, $yy A.D";
		WordyString = "$NZ$ww on this $DT day of the month of $mf, in the $YO year of our Lord";

		var janDayName = new Dictionary<int, DayName>();
		var decDayName = new Dictionary<int, DayName>();
		janDayName.Add(26, new DayName("Australia Day", "Australia Day"));
		decDayName.Add(25, new DayName("Christmas", "Christmas Day"));
		decDayName.Add(26, new DayName("Boxing Day", "Boxing Day"));
		decDayName.Add(31, new DayName("New Years Eve", "New Years Eve"));
		var febInter = new List<IntercalaryDay>();
		var feb29 = new IntercalaryDay { InsertNumnewDays = 1 };
		feb29.SpecialDayNames.Add(29, new DayName("Backwards Day", "Backwards Day"));
		feb29.Rule.DivisibleBy = 4;
		feb29.Rule.Offset = 0;
		var f29e = new IntercalaryRule(100, 0);
		f29e.Exceptions.Add(new IntercalaryRule(400, 0));
		feb29.Rule.Exceptions.Add(f29e);
		febInter.Add(feb29);
		feb29.Rule.IsIntercalaryYear(2012);
		Months.Add(new MonthDefinition("jan", "january", "January", 1, 31, janDayName, new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("feb", "february", "February", 2, 28, new Dictionary<int, DayName>(),
			febInter));
		Months.Add(new MonthDefinition("mar", "march", "March", 3, 31, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("apr", "april", "April", 4, 30, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("may", "may", "May", 5, 31, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("jun", "june", "June", 6, 30, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("jul", "july", "July", 7, 31, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("aug", "august", "August", 8, 31, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("sep", "september", "September", 9, 30, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("oct", "october", "October", 10, 31, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("nov", "november", "November", 11, 30, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("dec", "december", "December", 12, 31, decDayName, new List<IntercalaryDay>()));

		Weekdays.Add("Monday");
		Weekdays.Add("Tuesday");
		Weekdays.Add("Wednesday");
		Weekdays.Add("Thursday");
		Weekdays.Add("Friday");
		Weekdays.Add("Saturday");
		Weekdays.Add("Sunday");

		EpochYear = 2010;
		FirstWeekdayAtEpoch = 4;
	}

	public void SetupTestData2()
	{
		_id = 1;
		Alias = "imperial";
		Description = "The calendar created by the emperor of mankind";
		ShortName = "Imperial Calendar (c.a. 40000)";
		FullName = "The Imperial Calendar, in High Gothic with Official Date Display, circa 41000";
		Plane = "terra";
		ClockID = 1;

		ShortString = "$dd/$mm/$yy";
		LongString = "$nz$ww the $dt of $mf, year $yy";
		WordyString = "$NZ$ww on this $DT day of the month of $mf, in the $MY year of the $MI millenium";

		var janDayName = new Dictionary<int, DayName>();
		var decDayName = new Dictionary<int, DayName>();
		janDayName.Add(26, new DayName("Emperor Day", "Emperor Day"));
		decDayName.Add(25, new DayName("Abhor the Witchmas", "Abhor the Witchmas"));
		var febInter = new List<IntercalaryDay>();
		var feb29 = new IntercalaryDay { InsertNumnewDays = 1, Rule = { DivisibleBy = 4, Offset = 0 } };
		var f29e = new IntercalaryRule(100, 0);
		f29e.Exceptions.Add(new IntercalaryRule(400, 0));
		feb29.Rule.Exceptions.Add(f29e);
		febInter.Add(feb29);
		feb29.Rule.IsIntercalaryYear(2012);
		Months.Add(new MonthDefinition("jan", "january", "January", 1, 31, janDayName, new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("feb", "february", "February", 2, 28, new Dictionary<int, DayName>(),
			febInter));
		Months.Add(new MonthDefinition("mar", "march", "March", 3, 31, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("apr", "april", "April", 4, 30, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("may", "may", "May", 5, 31, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("jun", "june", "June", 6, 30, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("jul", "july", "July", 7, 31, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("aug", "august", "August", 8, 31, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("sep", "september", "September", 9, 30, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("oct", "october", "October", 10, 31, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("nov", "november", "November", 11, 30, new Dictionary<int, DayName>(),
			new List<IntercalaryDay>()));
		Months.Add(new MonthDefinition("dec", "december", "December", 12, 31, decDayName, new List<IntercalaryDay>()));

		Weekdays.Add("Monday");
		Weekdays.Add("Tuesday");
		Weekdays.Add("Wednesday");
		Weekdays.Add("Thursday");
		Weekdays.Add("Friday");
		Weekdays.Add("Saturday");
		Weekdays.Add("Sunday");

		EpochYear = 40800;
		FirstWeekdayAtEpoch = 4;
	}

	#endregion

	#region IFutureProgVariable implementation

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "date":
				return new MudDateTime(CurrentDate, FeedClock.CurrentTime, FeedClock.PrimaryTimezone);
			case "clock":
				return FeedClock;
			default:
				throw new NotSupportedException($"Unsupported property type {property} in Calendar.GetProperty");
		}
	}

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Calendar;

	public object GetObject => this;

	private static FutureProgVariableTypes DotReferenceHandler(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return FutureProgVariableTypes.Number;
			case "name":
				return FutureProgVariableTypes.Text;
			case "date":
				return FutureProgVariableTypes.MudDateTime;
			case "clock":
				return FutureProgVariableTypes.Clock;
			default:
				return FutureProgVariableTypes.Error;
		}
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", FutureProgVariableTypes.Number },
			{ "name", FutureProgVariableTypes.Text },
			{ "date", FutureProgVariableTypes.MudDateTime },
			{ "clock", FutureProgVariableTypes.Clock }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" },
			{ "date", "" },
			{ "clock", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Calendar, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}
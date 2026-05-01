using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Economy.Currency;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

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
        XElement element = root.Element("alias");
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

        foreach (XElement subElement in element.Elements("weekday"))
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

        foreach (XElement subElement in element.Elements("month"))
        {
            MonthDefinition month = new();
            month.LoadFromXml(subElement);
            _months.Add(month);
        }

        // Intercalaries
        element = root.Element("intercalarymonths");
        if (element?.HasElements == true) // Intercalaries are not mandatory, unlike other fields.
        {
            foreach (XElement subElement in element.Elements("intercalarymonth"))
            {
                IntercalaryMonth intercalary = new();
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
            new XElement("moderneralongstring", ModernEraLongString), new XElement("weekdays",
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
            Models.Calendar dbcal = FMDB.Context.Calendars.Find(Id);
            dbcal.Definition = SaveToXml().ToString();
            dbcal.Date = CurrentDate?.GetDateString() ?? StoredTimeFallbacks.FirstValidDate(this).GetDateString();
            dbcal.FeedClockId = FeedClock?.Id ?? ClockID;
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
            _feedClock?.DaysUpdated -= CurrentDate.AdvanceDays;

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

            FeedClock?.DaysUpdated += _currentDate.AdvanceDays;
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

    public Calendar(XElement file, IFuturemud game)
    {
        Gameworld = game;
        LoadFromXml(file);
    }

    public Calendar(int id, IFuturemud game)
    {
        _id = id;
        Gameworld = game;
    }

    public Calendar(IFuturemud game, string alias, string shortName, string fullName, IClock clock)
    {
        Gameworld = game;
        Alias = alias;
        ShortName = shortName;
        FullName = fullName;
        Description = fullName;
        Plane = "earth";
        ShortString = "$dd/$mm/$yy";
        LongString = "$nz$ww the $dt of $mf, $yy";
        WordyString = "$NZ$ww on this $DT day of the month of $mf, in the year $yy";
        AncientEraShortString = "B.E.";
        AncientEraLongString = "Before Epoch";
        ModernEraShortString = "A.E.";
        ModernEraLongString = "After Epoch";
        EpochYear = 1;
        FirstWeekdayAtEpoch = 0;
        Weekdays.AddRange(["Firstday", "Secondday", "Thirdday", "Fourthday", "Fifthday", "Sixthday", "Seventhday"]);
        Months.Add(new MonthDefinition("mon", "month", "Month", 1, 30, new Dictionary<int, DayName>(), []));
        ClockID = clock.Id;
        CurrentDate = StoredTimeFallbacks.FirstValidDate(this);
        CurrentDate.IsPrimaryDate = true;

        using (new FMDB())
        {
            Models.Calendar dbcal = new();
            FMDB.Context.Calendars.Add(dbcal);
            dbcal.Definition = SaveToXml().ToString();
            dbcal.Date = CurrentDate.GetDateString();
            dbcal.FeedClockId = clock.Id;
            FMDB.Context.SaveChanges();
            _id = dbcal.Id;
        }
    }

    private Calendar(Calendar rhs, string alias, string shortName, string fullName)
    {
        Gameworld = rhs.Gameworld;
        LoadFromXml(rhs.SaveToXml());
        Alias = alias;
        ShortName = shortName;
        FullName = fullName;
        Description = rhs.Description;
        ClockID = rhs.FeedClock.Id;
        CurrentDate = new MudDate(rhs.CurrentDate);
        CurrentDate.IsPrimaryDate = true;

        using (new FMDB())
        {
            Models.Calendar dbcal = new();
            FMDB.Context.Calendars.Add(dbcal);
            dbcal.Definition = SaveToXml().ToString();
            dbcal.Date = CurrentDate.GetDateString();
            dbcal.FeedClockId = FeedClock.Id;
            FMDB.Context.SaveChanges();
            _id = dbcal.Id;
        }
    }

    public ICalendar Clone(string alias, string shortName, string fullName)
    {
        return new Calendar(this, alias, shortName, fullName);
    }

    public Calendar(MudSharp.Models.Calendar calendar, IFuturemud game)
    {
        _id = calendar.Id;
        Gameworld = game;
        LoadFromXml(XElement.Parse(calendar.Definition));
        ClockID = calendar.FeedClockId;
        CurrentDate = this.GetStoredDateOrFallback(calendar.Date, StoredMudDateFallback.EpochStart, "Calendar",
            calendar.Id, ShortName, "CurrentDate");
        CurrentDate.IsPrimaryDate = true;
    }

    #endregion

    #region Methods

    private Dictionary<int, Year> _cachedYears = new();

    private void ClearDateCaches()
    {
        _cachedYears.Clear();
        _cachedWeekdaysInYear.Clear();
        _cachedDaysInYear.Clear();
        _cachedDaysBetweenYears.Clear();
        _cachedFirstWeekday.Clear();
    }

    private void NormaliseCurrentDate()
    {
        try
        {
            if (CurrentDate is not null)
            {
                CurrentDate = GetDate(CurrentDate.GetDateString());
                CurrentDate.IsPrimaryDate = true;
                return;
            }
        }
        catch
        {
            // Fall through to the first valid generated date.
        }

        CurrentDate = StoredTimeFallbacks.FirstValidDate(this);
        CurrentDate.IsPrimaryDate = true;
    }

    /// <summary>
    ///     Generates a new year (An ordered list of Months) accurate for the given calendar year
    /// </summary>
    /// <param name="whichYear">The numerical year to generate</param>
    /// <returns>A list of months generated correctly for that year</returns>
    public Year CreateYear(int whichYear)
    {
        if (_cachedYears.TryGetValue(whichYear, out Year year))
        {
            return year;
        }

        List<Month> returnList = new();
        returnList.AddRange(Months.Select(x => new Month(x, whichYear)));
        returnList.AddRange(
            Intercalaries.Where(x => x.Rule.IsIntercalaryYear(whichYear)).Select(x => new Month(x.Month, whichYear)));
        returnList = returnList.OrderBy(x => x.NominalOrder).ToList();
        Year newYear = new(returnList, whichYear, this);
        _cachedYears[whichYear] = newYear;
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
        if (_cachedWeekdaysInYear.TryGetValue(whichYear, out int year))
        {
            return year;
        }

        int sum = _months.Sum(x => x.NormalDays - x.NonWeekdays.Count)
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
        if (_cachedDaysInYear.TryGetValue(whichYear, out int year))
        {
            return year;
        }
        int sum = _months.Sum(x => x.NormalDays)
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

        if (_cachedDaysBetweenYears.TryGetValue((startYear, endYear), out int count))
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

        if (_cachedFirstWeekday.TryGetValue(whichYear, out int count))
        {
            return count;
        }

        int daysBetween = 0;
        int lowerYear = Math.Min(whichYear, EpochYear);
        int upperYear = Math.Max(whichYear, EpochYear);

        for (int i = lowerYear; i < upperYear; i++)
        {
            daysBetween += CountWeekdaysInYear(i);
        }

        int day = whichYear > EpochYear
            ? (FirstWeekdayAtEpoch + daysBetween).Modulus(Weekdays.Count)
            : (FirstWeekdayAtEpoch - daysBetween).Modulus(Weekdays.Count);
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
        Year MUDYear = CreateYear(year);
        Month MUDMonth = MUDYear.Months.FirstOrDefault(x => x.Alias == month);
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
        Year year = CreateYear(CurrentDate.Year - age);
        Month month = year.Months.GetWeightedRandom(x => x.Days);
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
        List<string> dateStringSplit = dateString.Split('/').ToList();
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

        int setDay = dayText.GetIntFromOrdinal() ?? 0;

        if (setDay < 1)
        {
            throw new MUDDateException("The day must be a positive integer.");
        }

        if (!int.TryParse(dateStringSplit[2], out int setYear))
        {
            throw new MUDDateException("The year must be a number.");
        }

        // Generate the nominated year
        Year newYear = CreateYear(setYear);

        // Check to see the month exists
        Month month =
            newYear.Months.FirstOrDefault(
                x => x.Alias.Equals(monthText, StringComparison.InvariantCultureIgnoreCase)) ??
            newYear.Months.FirstOrDefault(
                x => x.FullName.Equals(monthText, StringComparison.InvariantCultureIgnoreCase)) ??
            newYear.Months.FirstOrDefault(
                x => x.ShortName.Equals(monthText, StringComparison.InvariantCultureIgnoreCase));

        if (month == null)
        {
            if (!int.TryParse(monthText, out int numMonth))
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
        List<string> dateStringSplit = dateString.Split('/').ToList();
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
            System.Globalization.CultureInfo dtformat = format as System.Globalization.CultureInfo ??
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
        List<string> dateStringSplit = dateString.Split('/').ToList();
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

        int setDay = dayText.GetIntFromOrdinal() ?? 0;

        if (setDay < 1)
        {
            error = "The day must be a positive integer.";
            date = new MudDate(CurrentDate);
            return false;
        }

        if (!int.TryParse(dateStringSplit[2], out int setYear))
        {
            error = "The year must be a number.";
            date = new MudDate(CurrentDate);
            return false;
        }

        // Generate the nominated year
        Year newYear = CreateYear(setYear);

        // Check to see the month exists
        Month month =
            newYear.Months.FirstOrDefault(
                x => x.Alias.Equals(monthText, StringComparison.InvariantCultureIgnoreCase)) ??
            newYear.Months.FirstOrDefault(
                x => x.FullName.Equals(monthText, StringComparison.InvariantCultureIgnoreCase)) ??
            newYear.Months.FirstOrDefault(
                x => x.ShortName.Equals(monthText, StringComparison.InvariantCultureIgnoreCase));

        if (month == null)
        {
            if (!int.TryParse(monthText, out int numMonth))
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

        Dictionary<int, DayName> janDayName = new();
        Dictionary<int, DayName> decDayName = new();
        janDayName.Add(26, new DayName("Australia Day", "Australia Day"));
        decDayName.Add(25, new DayName("Christmas", "Christmas Day"));
        decDayName.Add(26, new DayName("Boxing Day", "Boxing Day"));
        decDayName.Add(31, new DayName("New Years Eve", "New Years Eve"));
        List<IntercalaryDay> febInter = new();
        IntercalaryDay feb29 = new() { InsertNumnewDays = 1 };
        feb29.SpecialDayNames.Add(29, new DayName("Backwards Day", "Backwards Day"));
        feb29.Rule.DivisibleBy = 4;
        feb29.Rule.Offset = 0;
        IntercalaryRule f29e = new(100, 0);
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

        Dictionary<int, DayName> janDayName = new();
        Dictionary<int, DayName> decDayName = new();
        janDayName.Add(26, new DayName("Emperor Day", "Emperor Day"));
        decDayName.Add(25, new DayName("Abhor the Witchmas", "Abhor the Witchmas"));
        List<IntercalaryDay> febInter = new();
        IntercalaryDay feb29 = new() { InsertNumnewDays = 1, Rule = { DivisibleBy = 4, Offset = 0 } };
        IntercalaryRule f29e = new(100, 0);
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

    #region IEditableItem Implementation

    public const string BuildingHelpText = @"You can use the following options with this command:

	#3alias <alias>#0 - changes the calendar alias
	#3shortname <name>#0 - changes the short calendar name
	#3fullname <name>#0 - changes the full calendar name
	#3desc <description>#0 - changes the description
	#3plane <plane>#0 - changes the intended plane alias
	#3clock <clock>#0 - changes the feed clock
	#3date <date>#0 - sets the current date
	#3epoch <year> <weekday>#0 - sets the epoch year and first weekday
	#3short|long|wordy <mask>#0 - changes display masks
	#3era <ancient|modern> <short|long> <text>#0 - changes era text
	#3weekday add|rename|remove ...#0 - edits weekdays
	#3month add|rename|alias|short|days|order|remove|nonweekday|special ...#0 - edits months
	#3intercalary day|month ...#0 - edits intercalary day and month rules
	#3preview [year]#0 - previews a generated year
	#3validate#0 - validates generated calendar data";

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "alias":
                return BuildingCommandAlias(actor, command);
            case "name":
            case "shortname":
                return BuildingCommandShortName(actor, command);
            case "fullname":
            case "full":
                return BuildingCommandFullName(actor, command);
            case "desc":
            case "description":
                return BuildingCommandDescription(actor, command);
            case "plane":
                return BuildingCommandPlane(actor, command);
            case "clock":
            case "feedclock":
                return BuildingCommandClock(actor, command);
            case "date":
            case "current":
                return BuildingCommandDate(actor, command);
            case "epoch":
                return BuildingCommandEpoch(actor, command);
            case "short":
                return BuildingCommandShortMask(actor, command);
            case "long":
                return BuildingCommandLongMask(actor, command);
            case "wordy":
                return BuildingCommandWordyMask(actor, command);
            case "era":
                return BuildingCommandEra(actor, command);
            case "weekday":
            case "weekdays":
                return BuildingCommandWeekday(actor, command);
            case "month":
            case "months":
                return BuildingCommandMonth(actor, command);
            case "intercalary":
            case "intercalaries":
                return BuildingCommandIntercalary(actor, command);
            case "preview":
                return BuildingCommandPreview(actor, command);
            case "validate":
                return BuildingCommandValidate(actor, command);
            default:
                actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
                return false;
        }
    }

    private bool BuildingCommandAlias(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What alias should this calendar have?");
            return false;
        }

        var alias = command.PopSpeech();
        if (Gameworld.Calendars.Any(x => x != this && x.Alias.EqualTo(alias)))
        {
            actor.OutputHandler.Send($"There is already a calendar with the alias {alias.ColourValue()}.");
            return false;
        }

        Alias = alias;
        Changed = true;
        actor.OutputHandler.Send($"This calendar now has the alias {alias.ColourValue()}.");
        return true;
    }

    private bool BuildingCommandShortName(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What short name should this calendar have?");
            return false;
        }

        ShortName = command.SafeRemainingArgument.TitleCase();
        Changed = true;
        actor.OutputHandler.Send($"This calendar's short name is now {ShortName.ColourName()}.");
        return true;
    }

    private bool BuildingCommandFullName(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What full name should this calendar have?");
            return false;
        }

        FullName = command.SafeRemainingArgument.TitleCase();
        Changed = true;
        actor.OutputHandler.Send($"This calendar's full name is now {FullName.ColourName()}.");
        return true;
    }

    private bool BuildingCommandDescription(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What description should this calendar have?");
            return false;
        }

        Description = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send("Description set.");
        return true;
    }

    private bool BuildingCommandPlane(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What plane alias should this calendar use?");
            return false;
        }

        Plane = command.PopSpeech();
        Changed = true;
        actor.OutputHandler.Send($"This calendar now applies to the {Plane.ColourValue()} plane alias.");
        return true;
    }

    private bool BuildingCommandClock(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which clock should feed this calendar?");
            return false;
        }

        var clock = Gameworld.Clocks.GetByIdOrNames(command.SafeRemainingArgument);
        if (clock is null)
        {
            actor.OutputHandler.Send("There is no such clock.");
            return false;
        }

        return ConfirmStructuralChange(actor, $"change feed clock to {clock.Name}", () =>
        {
            FeedClock = clock;
            ClockID = clock.Id;
            Changed = true;
            actor.OutputHandler.Send($"This calendar is now fed by the {clock.Name.ColourName()} clock.");
        });
    }

    private bool BuildingCommandDate(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What date should this calendar be set to?");
            return false;
        }

        if (!TryGetDate(command.SafeRemainingArgument, actor, out var date, out var error))
        {
            actor.OutputHandler.Send(error);
            return false;
        }

        CurrentDate = date;
        CurrentDate.IsPrimaryDate = true;
        Changed = true;
        actor.OutputHandler.Send($"This calendar's current date is now {DisplayDate(CurrentDate, CalendarDisplayMode.Long).ColourValue()}.");
        return true;
    }

    private bool BuildingCommandEpoch(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var year))
        {
            actor.OutputHandler.Send("You must specify an epoch year.");
            return false;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which weekday is the first weekday of the epoch year?");
            return false;
        }

        if (!TryGetWeekdayIndex(command.SafeRemainingArgument, out var weekday))
        {
            actor.OutputHandler.Send("That is not a valid weekday for this calendar.");
            return false;
        }

        return ConfirmStructuralChange(actor, $"change epoch to year {year:N0} and weekday {Weekdays[weekday]}", () =>
        {
            EpochYear = year;
            FirstWeekdayAtEpoch = weekday;
            ClearDateCaches();
            NormaliseCurrentDate();
            Changed = true;
            actor.OutputHandler.Send($"The epoch year is now {year.ToStringN0(actor).ColourValue()} with first weekday {Weekdays[weekday].ColourValue()}.");
        });
    }

    private bool BuildingCommandShortMask(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What short display mask should this calendar use?");
            return false;
        }

        ShortString = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send("Short display mask set.");
        return true;
    }

    private bool BuildingCommandLongMask(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What long display mask should this calendar use?");
            return false;
        }

        LongString = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send("Long display mask set.");
        return true;
    }

    private bool BuildingCommandWordyMask(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What wordy display mask should this calendar use?");
            return false;
        }

        WordyString = command.SafeRemainingArgument;
        Changed = true;
        actor.OutputHandler.Send("Wordy display mask set.");
        return true;
    }

    private bool BuildingCommandEra(ICharacter actor, StringStack command)
    {
        var era = command.PopForSwitch();
        var length = command.PopForSwitch();
        if (string.IsNullOrEmpty(era) || string.IsNullOrEmpty(length) || command.IsFinished)
        {
            actor.OutputHandler.Send("You must specify an era, short/long, and the replacement text.");
            return false;
        }

        switch ((era, length))
        {
            case ("ancient", "short"):
                AncientEraShortString = command.SafeRemainingArgument;
                break;
            case ("ancient", "long"):
                AncientEraLongString = command.SafeRemainingArgument;
                break;
            case ("modern", "short"):
                ModernEraShortString = command.SafeRemainingArgument;
                break;
            case ("modern", "long"):
                ModernEraLongString = command.SafeRemainingArgument;
                break;
            default:
                actor.OutputHandler.Send("The era must be ancient or modern, and the type must be short or long.");
                return false;
        }

        Changed = true;
        actor.OutputHandler.Send("Era text set.");
        return true;
    }

    private bool BuildingCommandWeekday(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "add":
            case "new":
                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("What weekday name do you want to add?");
                    return false;
                }

                var newWeekday = command.SafeRemainingArgument.TitleCase();
                return ConfirmStructuralChange(actor, $"add weekday {newWeekday}", () =>
                {
                    Weekdays.Add(newWeekday);
                    ClearDateCaches();
                    NormaliseCurrentDate();
                    Changed = true;
                    actor.OutputHandler.Send($"You add weekday {Weekdays.Last().ColourValue()}.");
                });
            case "rename":
            case "name":
                if (command.IsFinished || !TryGetWeekdayIndex(command.PopSpeech(), out var index))
                {
                    actor.OutputHandler.Send("Which weekday do you want to rename?");
                    return false;
                }

                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("What new name should that weekday have?");
                    return false;
                }

                var newName = command.SafeRemainingArgument.TitleCase();
                return ConfirmStructuralChange(actor, $"rename weekday #{index + 1:N0} to {newName}", () =>
                {
                    Weekdays[index] = newName;
                    ClearDateCaches();
                    Changed = true;
                    actor.OutputHandler.Send($"Weekday #{(index + 1).ToStringN0(actor)} is now {Weekdays[index].ColourValue()}.");
                });
            case "remove":
            case "delete":
            case "del":
                if (Weekdays.Count <= 1)
                {
                    actor.OutputHandler.Send("A calendar must have at least one weekday.");
                    return false;
                }

                if (command.IsFinished || !TryGetWeekdayIndex(command.SafeRemainingArgument, out index))
                {
                    actor.OutputHandler.Send("Which weekday do you want to remove?");
                    return false;
                }

                var old = Weekdays[index];
                return ConfirmStructuralChange(actor, $"remove weekday {old}", () =>
                {
                    Weekdays.RemoveAt(index);
                    FirstWeekdayAtEpoch = FirstWeekdayAtEpoch.Modulus(Weekdays.Count);
                    ClearDateCaches();
                    NormaliseCurrentDate();
                    Changed = true;
                    actor.OutputHandler.Send($"You remove the weekday {old.ColourValue()}. Existing weekday-based schedules may need review.");
                });
            default:
                actor.OutputHandler.Send("You must specify add, rename or remove.");
                return false;
        }
    }

    private bool BuildingCommandMonth(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "add":
            case "new":
                return BuildingCommandMonthAdd(actor, command);
            case "remove":
            case "delete":
            case "del":
                return BuildingCommandMonthRemove(actor, command);
            case "rename":
            case "name":
                return BuildingCommandMonthRename(actor, command);
            case "alias":
                return BuildingCommandMonthAlias(actor, command);
            case "short":
            case "shortname":
                return BuildingCommandMonthShort(actor, command);
            case "days":
                return BuildingCommandMonthDays(actor, command);
            case "order":
                return BuildingCommandMonthOrder(actor, command);
            case "nonweekday":
            case "nonweekdays":
                return BuildingCommandMonthNonWeekday(actor, command);
            case "special":
            case "specialday":
                return BuildingCommandMonthSpecial(actor, command);
            default:
                actor.OutputHandler.Send("You must specify add, remove, rename, alias, short, days, order, nonweekday or special.");
                return false;
        }
    }

    private bool BuildingCommandMonthAdd(ICharacter actor, StringStack command)
    {
        var shortName = command.PopSpeech();
        var alias = command.PopSpeech();
        if (string.IsNullOrEmpty(shortName) || string.IsNullOrEmpty(alias) || command.IsFinished)
        {
            actor.OutputHandler.Send("Syntax: month add <short> <alias> \"<full name>\" <days> [order]");
            return false;
        }

        var fullName = command.PopSpeech().TitleCase();
        if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var days) || days < 1)
        {
            actor.OutputHandler.Send("You must specify a positive number of days.");
            return false;
        }

        var order = Months.Any() ? Months.Max(x => x.NominalOrder) + 1 : 1;
        if (!command.IsFinished && (!int.TryParse(command.PopSpeech(), out order) || order < 1))
        {
            actor.OutputHandler.Send("The order must be a positive number.");
            return false;
        }

        if (Months.Any(x => x.Alias.EqualTo(alias) || x.ShortName.EqualTo(shortName)))
        {
            actor.OutputHandler.Send("A month already has that alias or short name.");
            return false;
        }

        return ConfirmStructuralChange(actor, $"add month {fullName}", () =>
        {
            Months.Add(new MonthDefinition(shortName, alias, fullName, order, days, new Dictionary<int, DayName>(), []));
            Months.Sort((x, y) => x.NominalOrder.CompareTo(y.NominalOrder));
            ClearDateCaches();
            NormaliseCurrentDate();
            Changed = true;
            actor.OutputHandler.Send($"You add the month {fullName.ColourName()}.");
        });
    }

    private bool BuildingCommandMonthRemove(ICharacter actor, StringStack command)
    {
        if (Months.Count <= 1)
        {
            actor.OutputHandler.Send("A calendar must have at least one month.");
            return false;
        }

        if (command.IsFinished || GetMonth(command.SafeRemainingArgument) is not { } month)
        {
            actor.OutputHandler.Send("Which month do you want to remove?");
            return false;
        }

        return ConfirmStructuralChange(actor, $"remove month {month.FullName}", () =>
        {
            Months.Remove(month);
            ClearDateCaches();
            NormaliseCurrentDate();
            Changed = true;
            actor.OutputHandler.Send($"You remove the month {month.FullName.ColourName()}. Existing stored dates may need review.");
        });
    }

    private bool BuildingCommandMonthRename(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || GetMonth(command.PopSpeech()) is not { } month || command.IsFinished)
        {
            actor.OutputHandler.Send("Syntax: month rename <month> <new full name>");
            return false;
        }

        var fullName = command.SafeRemainingArgument.TitleCase();
        return ConfirmStructuralChange(actor, $"rename month {month.FullName} to {fullName}", () =>
        {
            month.FullName = fullName;
            ClearDateCaches();
            Changed = true;
            actor.OutputHandler.Send($"That month is now called {month.FullName.ColourName()}.");
        });
    }

    private bool BuildingCommandMonthAlias(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || GetMonth(command.PopSpeech()) is not { } month || command.IsFinished)
        {
            actor.OutputHandler.Send("Syntax: month alias <month> <new alias>");
            return false;
        }

        var alias = command.PopSpeech();
        if (Months.Any(x => x != month && x.Alias.EqualTo(alias)))
        {
            actor.OutputHandler.Send("Another month already has that alias.");
            return false;
        }

        return ConfirmStructuralChange(actor, $"change month {month.FullName} alias to {alias}", () =>
        {
            month.Alias = alias;
            ClearDateCaches();
            NormaliseCurrentDate();
            Changed = true;
            actor.OutputHandler.Send($"That month now has alias {alias.ColourValue()}.");
        });
    }

    private bool BuildingCommandMonthShort(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || GetMonth(command.PopSpeech()) is not { } month || command.IsFinished)
        {
            actor.OutputHandler.Send("Syntax: month short <month> <new short name>");
            return false;
        }

        var shortName = command.PopSpeech();
        return ConfirmStructuralChange(actor, $"change month {month.FullName} short name to {shortName}", () =>
        {
            month.ShortName = shortName;
            ClearDateCaches();
            Changed = true;
            actor.OutputHandler.Send($"That month now has short name {month.ShortName.ColourValue()}.");
        });
    }

    private bool BuildingCommandMonthDays(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || GetMonth(command.PopSpeech()) is not { } month ||
            command.IsFinished || !int.TryParse(command.PopSpeech(), out var days) || days < 1)
        {
            actor.OutputHandler.Send("Syntax: month days <month> <positive days>");
            return false;
        }

        return ConfirmStructuralChange(actor, $"set month {month.FullName} to {days:N0} days", () =>
        {
            month.NormalDays = days;
            month.NonWeekdays.RemoveAll(x => x > days);
            foreach (var key in month.SpecialDayNames.Keys.Where(x => x > days).ToList())
            {
                month.SpecialDayNames.Remove(key);
            }

            ClearDateCaches();
            NormaliseCurrentDate();
            Changed = true;
            actor.OutputHandler.Send($"That month now has {days.ToStringN0(actor).ColourValue()} normal days.");
        });
    }

    private bool BuildingCommandMonthOrder(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || GetMonth(command.PopSpeech()) is not { } month ||
            command.IsFinished || !int.TryParse(command.PopSpeech(), out var order))
        {
            actor.OutputHandler.Send("Syntax: month order <month> <order>");
            return false;
        }

        return ConfirmStructuralChange(actor, $"set month {month.FullName} order to {order:N0}", () =>
        {
            month.NominalOrder = order;
            Months.Sort((x, y) => x.NominalOrder.CompareTo(y.NominalOrder));
            ClearDateCaches();
            NormaliseCurrentDate();
            Changed = true;
            actor.OutputHandler.Send($"That month now has nominal order {order.ToStringN0(actor).ColourValue()}.");
        });
    }

    private bool BuildingCommandMonthNonWeekday(ICharacter actor, StringStack command)
    {
        var action = command.PopForSwitch();
        if (command.IsFinished || GetMonth(command.PopSpeech()) is not { } month ||
            command.IsFinished || !int.TryParse(command.PopSpeech(), out var day) || day < 1 || day > month.NormalDays)
        {
            actor.OutputHandler.Send("Syntax: month nonweekday add|remove <month> <day>");
            return false;
        }

        if (!action.EqualTo("add") && !action.EqualTo("remove") && !action.EqualTo("delete"))
        {
            actor.OutputHandler.Send("You must specify add or remove.");
            return false;
        }

        return ConfirmStructuralChange(actor, $"{action} non-weekday day {day:N0} for {month.FullName}", () =>
        {
            if (action.EqualTo("add"))
            {
                if (!month.NonWeekdays.Contains(day))
                {
                    month.NonWeekdays.Add(day);
                }
            }
            else
            {
                month.NonWeekdays.Remove(day);
            }

            ClearDateCaches();
            NormaliseCurrentDate();
            Changed = true;
            actor.OutputHandler.Send($"Day {day.ToStringN0(actor)} of {month.FullName.ColourName()} is {(month.NonWeekdays.Contains(day) ? "now" : "no longer")} a non-weekday.");
        });
    }

    private bool BuildingCommandMonthSpecial(ICharacter actor, StringStack command)
    {
        var action = command.PopForSwitch();
        if (command.IsFinished || GetMonth(command.PopSpeech()) is not { } month ||
            command.IsFinished || !int.TryParse(command.PopSpeech(), out var day) || day < 1 || day > month.NormalDays)
        {
            actor.OutputHandler.Send("Syntax: month special add|remove <month> <day> [<short> <long>]");
            return false;
        }

        if (action.EqualTo("remove") || action.EqualTo("delete"))
        {
            return ConfirmStructuralChange(actor, $"remove special day {day:N0} from {month.FullName}", () =>
            {
                month.SpecialDayNames.Remove(day);
                ClearDateCaches();
                Changed = true;
                actor.OutputHandler.Send($"Day {day.ToStringN0(actor)} of {month.FullName.ColourName()} no longer has a special name.");
            });
        }

        if (!action.EqualTo("add") || command.IsFinished)
        {
            actor.OutputHandler.Send("Syntax: month special add <month> <day> <short> <long>");
            return false;
        }

        var shortName = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What long name should this special day have?");
            return false;
        }

        var longName = command.SafeRemainingArgument;
        return ConfirmStructuralChange(actor, $"add special day {day:N0} to {month.FullName}", () =>
        {
            month.SpecialDayNames[day] = new DayName(shortName, longName);
            ClearDateCaches();
            Changed = true;
            actor.OutputHandler.Send($"Day {day.ToStringN0(actor)} of {month.FullName.ColourName()} is now {shortName.ColourValue()}.");
        });
    }

    private bool BuildingCommandIntercalary(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "day":
            case "days":
                return BuildingCommandIntercalaryDay(actor, command);
            case "month":
            case "months":
                return BuildingCommandIntercalaryMonth(actor, command);
            default:
                actor.OutputHandler.Send("You must specify whether you are editing intercalary days or months.");
                return false;
        }
    }

    private bool BuildingCommandIntercalaryDay(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "add":
            case "new":
                if (command.IsFinished || GetMonth(command.PopSpeech()) is not { } month ||
                    command.IsFinished || !int.TryParse(command.PopSpeech(), out var insertDays) || insertDays < 1 ||
                    command.IsFinished || !int.TryParse(command.PopSpeech(), out var divisor) || divisor < 1)
                {
                    actor.OutputHandler.Send("Syntax: intercalary day add <month> <insertdays> <divisor> [offset]");
                    return false;
                }

                var offset = 0;
                if (!command.IsFinished && !int.TryParse(command.PopSpeech(), out offset))
                {
                    actor.OutputHandler.Send("The offset must be a valid number.");
                    return false;
                }

                return ConfirmStructuralChange(actor, $"add intercalary day rule to {month.FullName}", () =>
                {
                    month.Intercalaries.Add(new IntercalaryDay
                    {
                        InsertNumnewDays = insertDays,
                        Rule = new IntercalaryRule(divisor, offset)
                    });
                    ClearDateCaches();
                    NormaliseCurrentDate();
                    Changed = true;
                    actor.OutputHandler.Send($"You add an intercalary day rule to {month.FullName.ColourName()}.");
                });
            case "remove":
            case "delete":
            case "del":
                if (command.IsFinished || GetMonth(command.PopSpeech()) is not { } removeMonth ||
                    command.IsFinished || !int.TryParse(command.PopSpeech(), out var index) ||
                    index < 1 || index > removeMonth.Intercalaries.Count)
                {
                    actor.OutputHandler.Send("Syntax: intercalary day remove <month> <number>");
                    return false;
                }

                return ConfirmStructuralChange(actor, $"remove intercalary day rule #{index:N0} from {removeMonth.FullName}", () =>
                {
                    removeMonth.Intercalaries.RemoveAt(index - 1);
                    ClearDateCaches();
                    NormaliseCurrentDate();
                    Changed = true;
                    actor.OutputHandler.Send($"You remove intercalary day rule #{index.ToStringN0(actor)} from {removeMonth.FullName.ColourName()}.");
                });
            default:
                actor.OutputHandler.Send("You must specify add or remove.");
                return false;
        }
    }

    private bool BuildingCommandIntercalaryMonth(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "add":
            case "new":
                var shortName = command.PopSpeech();
                var alias = command.PopSpeech();
                if (string.IsNullOrEmpty(shortName) || string.IsNullOrEmpty(alias) || command.IsFinished)
                {
                    actor.OutputHandler.Send("Syntax: intercalary month add <short> <alias> \"<full>\" <days> <position> <divisor> [offset]");
                    return false;
                }

                var fullName = command.PopSpeech().TitleCase();
                if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var days) || days < 1 ||
                    command.IsFinished)
                {
                    actor.OutputHandler.Send("Syntax: intercalary month add <short> <alias> \"<full>\" <days> <position> <divisor> [offset]");
                    return false;
                }

                var position = command.PopSpeech();
                if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var divisor) || divisor < 1)
                {
                    actor.OutputHandler.Send("You must specify a positive divisor.");
                    return false;
                }

                var offset = 0;
                if (!command.IsFinished && !int.TryParse(command.PopSpeech(), out offset))
                {
                    actor.OutputHandler.Send("The offset must be a valid number.");
                    return false;
                }

                return ConfirmStructuralChange(actor, $"add intercalary month {fullName}", () =>
                {
                    Intercalaries.Add(new IntercalaryMonth(new IntercalaryRule(divisor, offset),
                        new MonthDefinition(shortName, alias, fullName, Months.Max(x => x.NominalOrder) + 1, days,
                            new Dictionary<int, DayName>(), []), position));
                    ClearDateCaches();
                    NormaliseCurrentDate();
                    Changed = true;
                    actor.OutputHandler.Send($"You add intercalary month {fullName.ColourName()}.");
                });
            case "remove":
            case "delete":
            case "del":
                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("Which intercalary month do you want to remove?");
                    return false;
                }

                var target = Intercalaries.FirstOrDefault(x =>
                    x.Month.Alias.EqualTo(command.SafeRemainingArgument) ||
                    x.Month.ShortName.EqualTo(command.SafeRemainingArgument) ||
                    x.Month.FullName.EqualTo(command.SafeRemainingArgument));
                if (target is null)
                {
                    actor.OutputHandler.Send("There is no such intercalary month.");
                    return false;
                }

                return ConfirmStructuralChange(actor, $"remove intercalary month {target.Month.FullName}", () =>
                {
                    Intercalaries.Remove(target);
                    ClearDateCaches();
                    NormaliseCurrentDate();
                    Changed = true;
                    actor.OutputHandler.Send($"You remove intercalary month {target.Month.FullName.ColourName()}.");
                });
            default:
                actor.OutputHandler.Send("You must specify add or remove.");
                return false;
        }
    }

    private bool BuildingCommandPreview(ICharacter actor, StringStack command)
    {
        var yearNumber = CurrentDate?.Year ?? EpochYear;
        if (!command.IsFinished && !int.TryParse(command.PopSpeech(), out yearNumber))
        {
            actor.OutputHandler.Send("The year must be a valid number.");
            return false;
        }

        var year = CreateYear(yearNumber);
        var sb = new StringBuilder();
        sb.AppendLine($"Calendar Preview: {FullName} - Year {yearNumber:N0}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
        foreach (var month in year.Months)
        {
            sb.AppendLine($"{month.Alias.ColourValue(),-12} {month.FullName.ColourName(),-30} {month.Days.ToStringN0(actor).ColourValue()} days");
        }

        actor.OutputHandler.Send(sb.ToString());
        return false;
    }

    private bool BuildingCommandValidate(ICharacter actor, StringStack command)
    {
        try
        {
            if (!Weekdays.Any())
            {
                actor.OutputHandler.Send("This calendar is invalid because it has no weekdays.");
                return false;
            }

            if (!Months.Any())
            {
                actor.OutputHandler.Send("This calendar is invalid because it has no months.");
                return false;
            }

            _ = CreateYear(CurrentDate?.Year ?? EpochYear);
            _ = CurrentDate ?? StoredTimeFallbacks.FirstValidDate(this);
            actor.OutputHandler.Send("This calendar generated successfully.");
            return false;
        }
        catch (Exception ex)
        {
            actor.OutputHandler.Send($"This calendar is not valid: {ex.Message.ColourError()}");
            return false;
        }
    }

    private bool RequiresStructuralConfirmation => Gameworld?.Calendars?.Contains(this) == true;

    private bool ConfirmStructuralChange(ICharacter actor, string description, Action action)
    {
        if (!RequiresStructuralConfirmation)
        {
            action();
            return true;
        }

        actor.OutputHandler.Send(
            $"{"Warning: this calendar is loaded in the gameworld. This change can invalidate stored dates, schedules, celestials, economy references, clan dates, or other saved time data. Fallbacks will protect load paths and notify admins, but you should preview and validate the calendar after accepting.".ColourError()}\n\nChange: {description.ColourCommand()}\n{Accept.StandardAcceptPhrasing}");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            DescriptionString = $"Confirm calendar structural change: {description}",
            AcceptAction = text =>
            {
                action();
                actor.OutputHandler.Send($"You accept the calendar structural change: {description.ColourCommand()}.");
            },
            RejectAction = text => actor.OutputHandler.Send("You decide not to make that calendar change."),
            ExpireAction = () => actor.OutputHandler.Send("The calendar structural change confirmation expires.")
        }));
        return false;
    }

    private bool TryGetWeekdayIndex(string text, out int index)
    {
        if (int.TryParse(text, out var value) && value >= 1 && value <= Weekdays.Count)
        {
            index = value - 1;
            return true;
        }

        index = Weekdays.FindIndex(x => x.EqualTo(text));
        return index >= 0;
    }

    private MonthDefinition GetMonth(string text)
    {
        return int.TryParse(text, out var value)
            ? Months.FirstOrDefault(x => x.NominalOrder == value)
            : Months.FirstOrDefault(x =>
                x.Alias.EqualTo(text) ||
                x.ShortName.EqualTo(text) ||
                x.FullName.EqualTo(text));
    }

    public string Show(ICharacter actor)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Calendar #{Id.ToStringN0(actor)} - {ShortName}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
        sb.AppendLine($"Alias: {Alias.ColourValue()}");
        sb.AppendLine($"Full Name: {FullName.ColourName()}");
        sb.AppendLine($"Description: {Description.ColourValue()}");
        sb.AppendLine($"Plane: {Plane.ColourValue()}");
        sb.AppendLine($"Feed Clock: {FeedClock?.Name.ColourName() ?? "None".ColourError()}");
        sb.AppendLine($"Current Date: {(CurrentDate is null ? "None".ColourError() : DisplayDate(CurrentDate, CalendarDisplayMode.Long).ColourValue())}");
        sb.AppendLine($"Epoch: {EpochYear.ToStringN0(actor).ColourValue()} / {(Weekdays.Count > FirstWeekdayAtEpoch ? Weekdays[FirstWeekdayAtEpoch].ColourValue() : "Invalid".ColourError())}");
        sb.AppendLine($"Short Mask: {ShortString.ColourCommand()}");
        sb.AppendLine($"Long Mask: {LongString.ColourCommand()}");
        sb.AppendLine($"Wordy Mask: {WordyString.ColourCommand()}");
        sb.AppendLine($"Ancient Era: {AncientEraShortString.ColourValue()} / {AncientEraLongString.ColourValue()}");
        sb.AppendLine($"Modern Era: {ModernEraShortString.ColourValue()} / {ModernEraLongString.ColourValue()}");
        sb.AppendLine($"Weekdays: {Weekdays.Select((x, i) => $"{(i + 1).ToStringN0(actor)}. {x.ColourValue()}").ListToString()}");
        sb.AppendLine("Months:");
        foreach (var month in Months.OrderBy(x => x.NominalOrder))
        {
            sb.AppendLine($"\t{month.NominalOrder.ToStringN0(actor)}. {month.ShortName.ColourValue()} / {month.Alias.ColourValue()} - {month.FullName.ColourName()} ({month.NormalDays.ToStringN0(actor)} days, {month.Intercalaries.Count.ToStringN0(actor)} intercalary rules)");
        }

        if (Intercalaries.Any())
        {
            sb.AppendLine("Intercalary Months:");
            foreach (var intercalary in Intercalaries)
            {
                sb.AppendLine($"\t{intercalary.Month.Alias.ColourValue()} - {intercalary.Month.FullName.ColourName()} before {intercalary.InsertPosition.ColourValue()} when {intercalary.Rule}");
            }
        }

        return sb.ToString();
    }

    #endregion

    #region IFutureProgVariable implementation

    public IProgVariable GetProperty(string property)
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

    public ProgVariableTypes Type => ProgVariableTypes.Calendar;

    public object GetObject => this;

    private static ProgVariableTypes DotReferenceHandler(string property)
    {
        switch (property.ToLowerInvariant())
        {
            case "id":
                return ProgVariableTypes.Number;
            case "name":
                return ProgVariableTypes.Text;
            case "date":
                return ProgVariableTypes.MudDateTime;
            case "clock":
                return ProgVariableTypes.Clock;
            default:
                return ProgVariableTypes.Error;
        }
    }

    private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
    {
        return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "id", ProgVariableTypes.Number },
            { "name", ProgVariableTypes.Text },
            { "date", ProgVariableTypes.MudDateTime },
            { "clock", ProgVariableTypes.Clock }
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
        ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Calendar, DotReferenceHandler(),
            DotReferenceHelp());
    }

    #endregion
}

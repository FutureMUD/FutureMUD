using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.TimeAndDate.Date
{
    public enum CalendarDisplayMode {
        Short,
        Long,
        Wordy
    }

    public delegate void CalendarEventHandler();
    
    public interface ICalendar : ISaveable, IXmlSavable, IXmlLoadable, IFutureProgVariable, IFrameworkItem
    {
        event CalendarEventHandler DaysUpdated;
        event CalendarEventHandler MonthsUpdated;
        event CalendarEventHandler YearsUpdated;
        string Alias { get; }
        string ShortName { get; }
        string FullName { get; }
        string Description { get; }
        string Plane { get; }
        string ShortString { get; }
        string LongString { get; }
        string WordyString { get; }
        IClock FeedClock { get; set; }
        long ClockID { get; set; }
        string ModernEraShortString { get; set; }
        string ModernEraLongString { get; set; }
        string AncientEraShortString { get; set; }
        string AncientEraLongString { get; set; }
        MudDate CurrentDate { get; }
        MudDateTime CurrentDateTime { get; }
        int EpochYear { get; }
        int FirstWeekdayAtEpoch { get; }
        List<string> Weekdays { get; }
        List<MonthDefinition> Months { get; }
        List<IntercalaryMonth> Intercalaries { get; }
        void UpdateDays();
        void UpdateMonths();
        void UpdateYears();

        /// <summary>
        ///     Generates a new year (An ordered list of Months) accurate for the given calendar year
        /// </summary>
        /// <param name="whichYear">The numerical year to generate</param>
        /// <returns>A list of months generated correctly for that year</returns>
        Year CreateYear(int whichYear);

        /// <summary>
        ///     This function counts the number of weekdays in a year - that is, days that are not specifically excluded from being
        ///     a weekday. It is used internally in the function GetFirstWeekday
        /// </summary>
        /// <param name="whichYear">The year to evaluate</param>
        /// <returns>The number of weekdays in the year</returns>
        int CountWeekdaysInYear(int whichYear);

        /// <summary>
        ///     This function counts the number of days in a year.
        /// </summary>
        /// <param name="whichYear">The year to evaluate</param>
        /// <returns>The number of days in the year</returns>
        int CountDaysInYear(int whichYear);

        /// <summary>
        ///     This function returns the number of days between two years exclusive of the endYear
        /// </summary>
        /// <param name="startYear">The first year to count</param>
        /// <param name="endYear">The last year, which is not counted</param>
        /// <returns>The number of days in the year</returns>
        int CountDaysBetweenYears(int startYear, int endYear);

        /// <summary>
        ///     Returns the index of the first weekday of the year for the specified year. This may not be the first day of the
        ///     year if the first day is excluded from weekdays.
        /// </summary>
        /// <param name="whichYear">Which year to compare</param>
        /// <returns>The index of the first weekday of the year</returns>
        int GetFirstWeekday(int whichYear);

        /// <summary>
        ///     Sets the current position of the calendar to the specified date string. Throws exceptions if it runs into errors
        /// </summary>
        /// <param name="dateString">
        ///     The date string is in format dd-mm-yyyy, where dd is the internal numerical value of the day,
        ///     mm is the string alias of the month, and yyyy is the numerical year. For example, 1-mar-2012 is the 1st day of the
        ///     month of march in the year 2012
        /// </param>
        void SetDate(string dateString);

        MudDate GetDateInYear(int day, string month, int year);
        MudDate GetBirthday(int day, string month, int age);
        MudDate GetRandomBirthday(int age);

        /// <summary>
        ///     Creates a new Date based on a date string. Throws an exception if it runs into an error.
        /// </summary>
        /// <param name="dateString">
        ///     The date string is in format dd-mm-yyyy, where dd is the internal numerical value of the day,
        ///     mm is the string alias of the month, and yyyy is the numerical year. For example, 1-mar-2012 is the 1st day of the
        ///     month of march in the year 2012
        /// </param>
        /// <returns>A new object of class Date containing the date information for the specified date in this calendar</returns>
        MudDate GetDate(string dateString);
        bool TryGetDate(string dateString, out MudDate date, out string error);

        string DisplayDate(CalendarDisplayMode mode);
        string DisplayDate(string mask);
        string DisplayDate(MudDate theDate, CalendarDisplayMode mode);
        string DisplayDate(MudDate theDate, string mask);
        void SetupTestData();
        void SetupTestData2();
    }
}
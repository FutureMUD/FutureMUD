using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MudSharp.TimeAndDate
{
    public class MudDateTime : IProgVariable, IComparable, IComparable<MudDateTime>
    {
        private static readonly Regex PlayerParseRegex = new(@"^(?<date>\d+[/-][a-z]+[/-]\d+) (?:(?<timezone>\w+)\s+){0,1}(?<time>\d+:\d+:\d+(?:\s*\w+)*)$", RegexOptions.IgnoreCase);

        private static readonly Regex AlternatePlayerParseRegex = new(@"(?<date>(?<date1>[a-z0-9]+)[ /-](?<date2>[a-z0-9]+)[ /-](?<date3>[0-9]+)) (?<time>(?<hour>\d+):(?<minute>\d+):*(?<second>\d+)*(?<period>[a-z]+)*\s*(?<timezone>[a-z]+)*)", RegexOptions.IgnoreCase);

        private static readonly Regex ParseRegex =
            new(@"^(?<calendar>\d+)_(?<date>[^_]+)_(?<clock>\d+)_(?<time>(?<timezone>[^ ]+) .+)$");

        public MudDate Date { get; }
        public MudTime Time { get; }
        public IMudTimeZone TimeZone { get; }

        public ICalendar Calendar => Date?.Calendar;
        public IClock Clock => Time?.Clock;
        public IFuturemud Gameworld { get; set; }

        public static string TryParseHelpText(ICharacter actor, MudDate date, MudTime time, IMudTimeZone tz)
        {
            IFormatProvider format = actor as IFormatProvider ?? CultureInfo.InvariantCulture;
            string exampleDate = date == null
                ? "1/jan/1"
                : $"{date.Day.ToString("N0", format)}/{date.Month.Alias}/{date.Year}";
            string exampleTime = time == null
                ? "0:0:0"
                : $"{time.Hours.ToString("N0", format)}:{time.Minutes.ToString("N0", format)}:{time.Seconds.ToString("N0", format)}";
            string timezoneName = tz?.Name ?? "UTC";
            return $@"Valid input is in the form #3<date> <time>#0, where the components are explained below. 

For example, this is one way that you could enter the current date and time: #3{exampleDate} {exampleTime} {timezoneName}#0
You can also enter the special values #3never#0 and #3now#0.

#6Dates#0

You can enter dates in one of several formats:

#3<day>/<month>/<year>#0 or #3<month>/<day>/<year>#0 are both fine if the month is the name or alias of the month, e.g. #312/Jan/2022#0 or #3July-04-1788#0

If you use all numbers, your input will be interpreted using the settings of your account culture - this may mean that it is read as #3day/month/year#0 (e.g. UK/Europe), #3month/day/year#0 (e.g. US) or #3year/month/day#0 (e.g. East Asia).

You can also use #3/#0, #3-#0 or spaces to separate the three parts of your date.

#6Times#0

Times are entered in the following format:

	#3<hours>:<minutes>:[<seconds>][<period>] [<timezone>]#0

For example, #33:15pm#0, #315:15:00#0 and #315:15:00 UTC#0 would all be valid dates.".SubstituteANSIColour();
        }

        public static string TryParseHelpText(ICharacter actor)
        {
            if (actor?.Location == null)
            {
                return TryParseHelpText(actor, null, null, null);
            }

            return TryParseHelpText(actor, actor.Location.Date(null), actor.Location.Time(null),
                actor.Location.TimeZone(null));
        }

        private static string TryParseHelpText(ICharacter actor, ICalendar calendar, IClock clock)
        {
            return actor == null
                ? TryParseHelpText(actor, calendar?.CurrentDate, clock?.CurrentTime, clock?.PrimaryTimezone)
                : TryParseHelpText(actor);
        }

        public static string TryParseHelpText(ICharacter actor, IEconomicZone zone)
        {
            return TryParseHelpText(actor, zone.FinancialPeriodReferenceCalendar.CurrentDate, zone.FinancialPeriodReferenceClock.CurrentTime, zone.FinancialPeriodTimezone);
        }

        /// <summary>
        /// Tries to parse a datetime from player input
        /// </summary>
        /// <param name="text">The text representing the datetime</param>
        /// <param name="calendar">The calendar</param>
        /// <param name="clock">The clock</param>
        /// <param name="actor">The actor for whom you're parsing</param>
        /// <param name="dt">The datetime</param>
        /// <param name="error">An error about what went wrong, if error</param>
        /// <returns>True if successfully parsed</returns>
        public static bool TryParse(string text, ICalendar calendar, IClock clock, ICharacter actor, out MudDateTime dt, out string error)
        {
            dt = null;
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(text))
            {
                error = TryParseHelpText(actor, calendar, clock);
                return false;
            }

            if (text.Equals("never", StringComparison.InvariantCultureIgnoreCase))
            {
                dt = Never;
                return true;
            }

            if ((calendar == null) || (clock == null))
            {
                error = TryParseHelpText(actor, calendar, clock);
                return false;
            }

            if (text.Equals("now", StringComparison.InvariantCultureIgnoreCase))
            {
                dt = calendar.CurrentDateTime;
                return true;
            }

            if (text.Equals("soon", StringComparison.InvariantCultureIgnoreCase))
            {
                dt = calendar.CurrentDateTime + MudTimeSpan.FromSeconds(30);
                return true;
            }

            Match regexMatch = AlternatePlayerParseRegex.Match(text);
            if (!regexMatch.Success)
            {
                error = TryParseHelpText(actor, calendar, clock);
                return false;
            }

            if (!calendar.TryGetDate(regexMatch.Groups["date"].Value, actor, out MudDate date, out error))
            {
                return false;
            }

            int hour = int.Parse(regexMatch.Groups["hour"].Value);
            int minute = int.Parse(regexMatch.Groups["minute"].Value);
            int second = regexMatch.Groups["second"].Length > 0 ? int.Parse(regexMatch.Groups["second"].Value) : 0;
            IMudTimeZone tz = regexMatch.Groups["timezone"].Length > 0
                ? clock.Timezones.GetByIdOrName(regexMatch.Groups["timezone"].Value)
                : clock.PrimaryTimezone;
            if (tz == null)
            {
                error = $"The timezone \"{regexMatch.Groups["timezone"].Value}\" is not valid.";
                return false;
            }

            if (regexMatch.Groups["period"].Length > 0)
            {
                int hourInterval = clock.HourIntervalNames.FindIndex(
                    x => x.Equals(regexMatch.Groups["period"].Value, StringComparison.InvariantCultureIgnoreCase));
                if (hourInterval < 0)
                {
                    error = $"The hour period \"{regexMatch.Groups["period"].Value}\" is not valid.";
                    return false;
                }

                hour +=
                    hourInterval *
                    (clock.HoursPerDay / clock.NumberOfHourIntervals);
            }

            MudTime time = new(second, minute, hour, tz, clock, 0);
            dt = new MudDateTime(date, time, tz);
            return true;
        }

        /// <summary>
        /// Tries to parse a date time from player input. Try not to use this version if you have an actor at this context
        /// </summary>
        /// <param name="text">The text representing the datetime</param>
        /// <param name="calendar">The calendar</param>
        /// <param name="clock">The clock</param>
        /// <param name="dt">The datetime</param>
        /// <returns>True if successful, false if not</returns>
        public static bool TryParse(string text, ICalendar calendar, IClock clock, out MudDateTime dt)
        {

            dt = null;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            if (text.Equals("never", StringComparison.InvariantCultureIgnoreCase))
            {
                dt = Never;
                return true;
            }

            if ((calendar == null) || (clock == null))
            {
                return false;
            }

            if (text.Equals("now", StringComparison.InvariantCultureIgnoreCase))
            {
                dt = calendar.CurrentDateTime;
                return true;
            }

            Match match = PlayerParseRegex.Match(text);
            if (!match.Success)
            {
                return false;
            }
            try
            {
                MudDate date = calendar.GetDate(match.Groups["date"].Value);
                IMudTimeZone timezone = match.Groups["timezone"].Length > 0
                ? clock.Timezones.GetByIdOrName(match.Groups["timezone"].Value)
                : clock.PrimaryTimezone;
                if (timezone == null)
                {
                    return false;
                }
                MudTime time = clock.GetTime($"{timezone.Alias} {match.Groups["time"].Value}");
                dt = new MudDateTime(date, time, timezone);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool TryParse(string text, IFuturemud gameworld, out MudDateTime dt)
        {
            dt = null;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            if (text.Equals("never", StringComparison.InvariantCultureIgnoreCase))
            {
                dt = Never;
                return true;
            }
            if (gameworld == null)
            {
                return false;
            }
            Match match = ParseRegex.Match(text);
            if (!match.Success)
            {
                return false;
            }
            ICalendar calendar = gameworld.Calendars.Get(long.Parse(match.Groups["calendar"].Value));
            if (calendar == null)
            {
                return false;
            }
            IClock clock = gameworld.Clocks.Get(long.Parse(match.Groups["clock"].Value));
            if (clock == null)
            {
                return false;
            }
            try
            {
                MudDate date = calendar.GetDate(match.Groups["date"].Value);
                IMudTimeZone timezone = clock.Timezones.GetByIdOrName(match.Groups["timezone"].Value);
                if (timezone == null)
                {
                    return false;
                }
                MudTime time = clock.GetTime(match.Groups["time"].Value);
                dt = new MudDateTime(date, time, timezone);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public MudDateTime GetByTimeZone(IMudTimeZone timezone)
        {
            if ((timezone == TimeZone) || (Date == null))
            {
                return new MudDateTime(this);
            }

            MudTime newTime = Time.GetTimeByTimezone(timezone);
            MudDate newDate = new(Date);
            if (newTime.DaysOffsetFromDatum != 0)
            {
                newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
            }
            return new MudDateTime(newDate, newTime, timezone);
        }

        public MudDateTime ConvertToOtherCalendar(ICalendar convert)
        {
            if (Date == null)
            {
                return Never;
            }

            return new MudDateTime(Date.ConvertToOtherCalendar(convert), new MudTime(Time), TimeZone);
        }

        public string ToString(CalendarDisplayMode calendarMode, TimeDisplayTypes clockMode)
        {
            return Date == null
                ? "Never"
                : $"{Date.Calendar.DisplayDate(Date, calendarMode)} {Time.Clock.DisplayTime(Time, clockMode)}";
        }

        /// <summary>
        /// An alias for calling ToString with the CalendarDisplayMode.Short and TimeDisplayTypes.Short options
        /// </summary>
        /// <returns>The time and date strings</returns>
        public override string ToString()
        {
            return ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short);
        }

        /// <summary>
        /// Returns a round-trip parseable version of the datetime not primarily intended for user display
        /// </summary>
        /// <returns></returns>
        public string GetDateTimeString()
        {
            return Date == null ? "Never" : $"{Calendar?.Id ?? 0}_{Date.GetDateString()}_{Clock?.Id ?? 0}_{Time.GetTimeString()}";
        }

        #region Constructors

        public MudDateTime(MudDate date, MudTime time, IMudTimeZone timezone)
        {
            Date = date;
            Time = time;
            TimeZone = timezone;
            Gameworld = Clock?.Gameworld;
        }

        public MudDateTime(MudDateTime rhs)
        {
            if (rhs?.Date == null)
            {
                Date = null;
                Time = null;
                TimeZone = null;
                Gameworld = rhs?.Gameworld;
                return;
            }

            Date = new MudDate(rhs.Date);
            Time = rhs.Time == null ? null : new MudTime(rhs.Time);
            TimeZone = rhs?.TimeZone;
            Gameworld = rhs?.Gameworld ?? Clock?.Gameworld;
        }

        public MudDateTime(string text, ICalendar calendar, IClock clock)
        {
            Gameworld = clock?.Gameworld;
            if (text.Equals("Never", StringComparison.InvariantCultureIgnoreCase))
            {
                Date = null;
                Time = null;
                TimeZone = null;
                return;
            }
            else
            {
                string[] splitText = text.Split(' ');
                Date = calendar.GetDate(splitText[0]);
                TimeZone = clock.Timezones.GetByIdOrName(splitText[1]);
                Time = clock.GetTime($"{splitText[1]} {splitText[2]}");
            }
        }

        public MudDateTime(string text, IFuturemud gameworld)
        {
            Gameworld = gameworld;
            if (text.Equals("Never", StringComparison.InvariantCultureIgnoreCase))
            {
                Date = null;
                Time = null;
                TimeZone = null;
            }
            else
            {
                string[] splitText = text.Split('_');
                ICalendar calendar = Gameworld.Calendars.Get(long.Parse(splitText[0]));
                Date = calendar.GetDate(splitText[1]);
                IClock clock = Gameworld.Clocks.Get(long.Parse(splitText[2]));
                string[] timeSplit = splitText[3].Split(' ');
                TimeZone = clock.Timezones.GetByIdOrName(timeSplit[0]);
                Time = clock.GetTime(splitText[3]);
            }
        }

        public static MudDateTime Never => new(default, default, default(IMudTimeZone));

        #endregion

        #region Operator Overloads

        public static bool Equals(MudDateTime dt1, MudDateTime dt2)
        {
            // Null datetime or null date is never. Never is less than all datetimes except itself
            if ((dt1?.Date == null) && (dt2?.Date == null))
            {
                return true;
            }
            if ((dt2?.Date == null) || (dt1?.Date == null))
            {
                return false;
            }

            if (dt1.TimeZone != dt2.TimeZone)
            {
                MudTime newTime = dt2.Time.GetTimeByTimezone(dt1.TimeZone);
                MudDate newDate = new(dt2.Date);
                if (newTime.DaysOffsetFromDatum != 0)
                {
                    newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
                }
                dt2 = new MudDateTime(newDate, newTime, dt1.TimeZone);
            }

            return dt2.Date.Equals(dt1.Date) && dt2.Time.Equals(dt1.Time);
        }

        public bool Equals(MudDateTime dt)
        {
            return Equals(this, dt);
        }

        public override bool Equals(object obj)
        {
            if (obj is MudDateTime objAsDateTime)
            {
                return Equals(this, objAsDateTime);
            }

            return obj is MudDate objAsDate && Date?.Equals(objAsDate) == true;
        }

        public override int GetHashCode()
        {
            return Date?.GetHashCode() + Time?.GetHashCode() + TimeZone?.GetHashCode() ?? 0;
        }

        public static MudDateTime operator +(MudDateTime dt, MudTimeSpan ts)
        {
            if (dt?.Date == null)
            {
                return Never;
            }

            MudTime newTime = new(dt.Time);
            newTime.AddSeconds(ts.SecondComponentOnly);
            newTime.AddMinutes(ts.MinuteComponentOnly);
            newTime.AddHours(ts.HourComponentOnly);

            MudDate newDate = new(dt.Date);
            newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
            newTime.DaysOffsetFromDatum = 0;
            newDate.AdvanceDays(ts.DayComponentOnly);
            newDate.AdvanceDays(newDate.Calendar.Weekdays.Count * ts.Weeks);
            newDate.AdvanceMonths(ts.Months, false, true);
            newDate.AdvanceYears(ts.Years, true);

            return new MudDateTime(newDate, newTime, dt.TimeZone);
        }

        public static MudDateTime operator -(MudDateTime dt, MudTimeSpan ts)
        {
            if (dt?.Date == null)
            {
                return Never;
            }

            MudTime newTime = new(dt.Time);
            newTime.AddSeconds(-1 * ts.SecondComponentOnly);
            newTime.AddMinutes(-1 * ts.MinuteComponentOnly);
            newTime.AddHours(-1 * ts.HourComponentOnly);

            MudDate newDate = new(dt.Date);
            newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
            newTime.DaysOffsetFromDatum = 0;
            newDate.AdvanceDays(-1 * ts.DayComponentOnly);
            newDate.AdvanceDays(-1 * newDate.Calendar.Weekdays.Count * ts.Weeks);
            newDate.AdvanceMonths(-1 * ts.Months, false, true);
            newDate.AdvanceYears(-1 * ts.Years, true);

            return new MudDateTime(newDate, newTime, dt.TimeZone);
        }

        public static TimeSpan operator -(MudDateTime dt1, MudDateTime dt2)
        {
            if (dt1 == null)
            {
                throw new ArgumentNullException(nameof(dt1));
            }

            if (dt2 == null)
            {
                throw new ArgumentNullException(nameof(dt2));
            }

            if (dt1.TimeZone != dt2.TimeZone)
            {
                dt1 = dt1.GetByTimeZone(dt1.Clock.PrimaryTimezone);
                dt2 = dt2.GetByTimeZone(dt2.Clock.PrimaryTimezone);
            }

            return dt1.Date - dt2.Date + (dt1.Time - dt2.Time);
        }

        public static bool operator <(MudDateTime dt1, MudDateTime dt2)
        {
            // Null datetime or null date is never. Never is less than all datetimes except itself
            if ((dt1?.Date == null) && (dt2?.Date == null))
            {
                return false;
            }
            if (dt2?.Date == null)
            {
                return false;
            }
            if (dt1?.Date == null)
            {
                return true;
            }

            if (dt1.TimeZone != dt2.TimeZone)
            {
                dt1 = dt1.GetByTimeZone(dt1.Clock.PrimaryTimezone);
                dt2 = dt2.GetByTimeZone(dt2.Clock.PrimaryTimezone);
            }

            if (dt1.Date.Equals(dt2.Date))
            {
                return dt1.Time < dt2.Time;
            }

            return dt1.Date < dt2.Date;
        }

        public static bool operator <=(MudDateTime dt1, MudDateTime dt2)
        {
            // Null datetime or null date is never. Never is less than all datetimes except itself
            if ((dt1?.Date == null) && (dt2?.Date == null))
            {
                return true;
            }
            if (dt2?.Date == null)
            {
                return false;
            }
            if (dt1?.Date == null)
            {
                return true;
            }

            if (dt1.TimeZone != dt2.TimeZone)
            {
                dt1 = dt1.GetByTimeZone(dt1.Clock.PrimaryTimezone);
                dt2 = dt2.GetByTimeZone(dt2.Clock.PrimaryTimezone);
            }

            if (dt1.Date.Equals(dt2.Date))
            {
                return dt1.Time <= dt2.Time;
            }

            return dt1.Date < dt2.Date;
        }

        public static bool operator >(MudDateTime dt1, MudDateTime dt2)
        {
            // Null datetime or null date is never. Never is less than all datetimes except itself
            if ((dt1?.Date == null) && (dt2?.Date == null))
            {
                return false;
            }
            if (dt2?.Date == null)
            {
                return true;
            }
            if (dt1?.Date == null)
            {
                return false;
            }

            if (dt1.TimeZone != dt2.TimeZone)
            {
                dt1 = dt1.GetByTimeZone(dt1.Clock.PrimaryTimezone);
                dt2 = dt2.GetByTimeZone(dt2.Clock.PrimaryTimezone);
            }

            if (dt1.Date.Equals(dt2.Date))
            {
                return dt1.Time > dt2.Time;
            }

            return dt1.Date > dt2.Date;
        }

        public static bool operator >=(MudDateTime dt1, MudDateTime dt2)
        {
            // Null datetime or null date is never. Never is less than all datetimes except itself
            if ((dt1?.Date == null) && (dt2?.Date == null))
            {
                return true;
            }
            if (dt2?.Date == null)
            {
                return true;
            }
            if (dt1?.Date == null)
            {
                return false;
            }

            if (dt1.TimeZone != dt2.TimeZone)
            {
                dt1 = dt1.GetByTimeZone(dt1.Clock.PrimaryTimezone);
                dt2 = dt2.GetByTimeZone(dt2.Clock.PrimaryTimezone);
            }

            if (dt1.Date.Equals(dt2.Date))
            {
                return dt1.Time >= dt2.Time;
            }

            return dt1.Date > dt2.Date;
        }

        #endregion

        #region IFutureProgVariable implementation

        public IProgVariable GetProperty(string property)
        {
            switch (property.ToLowerInvariant())
            {
                case "second":
                    return new NumberVariable(Time?.Seconds ?? 0);
                case "minute":
                    return new NumberVariable(Time?.Minutes ?? 0);
                case "hour":
                    return new NumberVariable(Time?.Hours ?? 0);
                case "day":
                    return new NumberVariable(Date?.Day ?? 0);
                case "month":
                    return new TextVariable(Date?.Month.Alias ?? "Never");
                case "year":
                    return new NumberVariable(Date?.Year ?? 0);
                case "isnever":
                    return new BooleanVariable(Date == null);
                case "midnight":
                    return Date == null ? Never : new MudDateTime(Date, new MudTime(0, 0, 0, TimeZone, Clock, 0), TimeZone);
                case "calendar":
                    return Calendar;
                case "clock":
                    return Clock;
                case "timezone":
                    return new TextVariable(TimeZone?.Alias ?? "None");
            }

            throw new NotSupportedException($"Unsupported property type {property} in MudDateTime.GetProperty");
        }

        public ProgVariableTypes Type => ProgVariableTypes.MudDateTime;

        public object GetObject => this;

        private static ProgVariableTypes DotReferenceHandler(string property)
        {
            switch (property.ToLowerInvariant())
            {
                case "second":
                    return ProgVariableTypes.Number;
                case "minute":
                    return ProgVariableTypes.Number;
                case "hour":
                    return ProgVariableTypes.Number;
                case "day":
                    return ProgVariableTypes.Number;
                case "month":
                    return ProgVariableTypes.Text;
                case "year":
                    return ProgVariableTypes.Number;
                case "isnever":
                    return ProgVariableTypes.Boolean;
                case "midnight":
                    return ProgVariableTypes.MudDateTime;
                case "calendar":
                    return ProgVariableTypes.Calendar;
                case "clock":
                    return ProgVariableTypes.Clock;
                case "timezone":
                    return ProgVariableTypes.Text;
                default:
                    return ProgVariableTypes.Error;
            }
        }

        private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
        {
            return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"second", ProgVariableTypes.Number},
                {"minute", ProgVariableTypes.Number},
                {"hour", ProgVariableTypes.Number},
                {"day", ProgVariableTypes.Number},
                {"month", ProgVariableTypes.Text},
                {"year", ProgVariableTypes.Number},
                {"isnever", ProgVariableTypes.Boolean},
                {"midnight", ProgVariableTypes.MudDateTime},
                {"calendar", ProgVariableTypes.Calendar},
                {"clock", ProgVariableTypes.Clock},
                {"timezone", ProgVariableTypes.Text},
            };
        }

        private static IReadOnlyDictionary<string, string> DotReferenceHelp()
        {
            return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"second", ""},
                {"minute", ""},
                {"hour", ""},
                {"day", ""},
                {"month", ""},
                {"year", ""},
                {"isnever", ""},
                {"midnight", "Returns the MudDateTime with the time component set to midnight"},
                {"calendar", ""},
                {"clock", ""},
                {"timezone", ""},
            };
        }

        public static void RegisterFutureProgCompiler()
        {
            ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.MudDateTime, DotReferenceHandler(), DotReferenceHelp());
        }

        #endregion

        #region IComparable Implementation


        public int CompareTo(object obj)
        {
            return CompareTo(obj as MudDateTime);
        }

        public int CompareTo([AllowNull] MudDateTime other)
        {
            return CompareTo(this, other);
        }

        public static int CompareTo(MudDateTime left, MudDateTime right)
        {
            if (left?.Date == null)
            {
                if (right?.Date == null)
                {
                    return 0;
                }

                return -1;
            }

            if (right?.Date == null)
            {
                return 1;
            }

            MudDateTime rTz = right.GetByTimeZone(left.TimeZone);
            switch (left.Date.CompareTo(rTz.Date))
            {
                case -1:
                    return -1;
                case 0:
                    return left.Time.CompareTo(rTz.Time);
                case 1:
                    return 1;
            }
            return 0;
        }
        #endregion
    }
}

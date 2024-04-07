using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.RegularExpressions;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MudSharp.TimeAndDate {
    public class MudDateTime : IFutureProgVariable, IComparable, IComparable<MudDateTime> {
        private static readonly Regex PlayerParseRegex = new(@"^(?<date>\d+[/-][a-z]+[/-]\d+) (?:(?<timezone>\w+)\s+){0,1}(?<time>\d+:\d+:\d+(?:\s*\w+)*)$", RegexOptions.IgnoreCase);

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
			return $@"
Valid input is in this format: {"<day>/<month name>/<year> <timezone> <hours>:<minutes>:<seconds>".ColourCommand()}
For example, this is how you would enter the current date and time: {$"{date.Day.ToString("N0", actor)}/{date.Month.Alias}/{date.Year} {tz.Name} {time.Hours.ToString("N0", actor)}:{time.Minutes.ToString("N0", actor)}:{time.Seconds.ToString("N0", actor)}".ColourCommand()}
You can also enter the special values #3never#0 and #3now#0.";
		}

		public static string TryParseHelpText(ICharacter actor)
		{
			return TryParseHelpText(actor, actor.Location.Date(null), actor.Location.Time(null),
				actor.Location.TimeZone(null));
		}

		public static string TryParseHelpText(ICharacter actor, IEconomicZone zone)
		{
			return TryParseHelpText(actor, zone.FinancialPeriodReferenceCalendar.CurrentDate, zone.FinancialPeriodReferenceClock.CurrentTime, zone.FinancialPeriodTimezone);
		}

		public static bool TryParse(string text, ICalendar calendar, IClock clock, out MudDateTime dt) {
			dt = null;
			if (text.Equals("never", StringComparison.InvariantCultureIgnoreCase)) {
				dt = Never;
				return true;
			}
			if (text.Equals("now", StringComparison.InvariantCultureIgnoreCase))
			{
				dt = calendar.CurrentDateTime;
			}
			if (string.IsNullOrEmpty(text) || (calendar == null) || (clock == null)) {
				return false;
			}
			var match = PlayerParseRegex.Match(text);
			if (!match.Success) {
				return false;
			}
			try {
				var date = calendar.GetDate(match.Groups["date"].Value);
				var timezone = match.Groups["timezone"].Length > 0
				? clock.Timezones.GetByName(match.Groups["timezone"].Value)
				: clock.PrimaryTimezone;
				if (timezone == null) {
					return false;
				}
				var time = clock.GetTime(match.Groups["time"].Value);
				dt = new MudDateTime(date, time, timezone);
				return true;
			}
			catch {
				return false;
			}
		}
		public static bool TryParse(string text, IFuturemud gameworld, out MudDateTime dt) {
			dt = null;
			if (text.Equals("never", StringComparison.InvariantCultureIgnoreCase)) {
				dt = Never;
				return true;
			}
			if (string.IsNullOrEmpty(text) || (gameworld == null)) {
				return false;
			}
			var match = ParseRegex.Match(text);
			if (!match.Success) {
				return false;
			}
			var calendar = gameworld.Calendars.Get(long.Parse(match.Groups["calendar"].Value));
			if (calendar == null) {
				return false;
			}
			var clock = gameworld.Clocks.Get(long.Parse(match.Groups["clock"].Value));
			if (clock == null) {
				return false;
			}
			try {
				var date = calendar.GetDate(match.Groups["date"].Value);
				var timezone = clock.Timezones.GetByName(match.Groups["timezone"].Value);
				if (timezone == null) {
					return false;
				}
                var time = clock.GetTime(match.Groups["time"].Value);
                dt = new MudDateTime(date, time, timezone);
                return true;
            }
            catch {
                return false;
            }
        }

        public MudDateTime GetByTimeZone(IMudTimeZone timezone) {
            if ((timezone == TimeZone) || (Date == null)) {
                return this;
            }

            var newTime = Time.GetTimeByTimezone(timezone);
            var newDate = new MudDate(Date);
            if (newTime.DaysOffsetFromDatum != 0) {
                newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
            }
            return new MudDateTime(newDate, newTime, timezone);
        }

        public string ToString(CalendarDisplayMode calendarMode, TimeDisplayTypes clockMode) {
            return Date == null
                ? "Never"
                : $"{Date.Calendar.DisplayDate(Date, calendarMode)} {Time.Clock.DisplayTime(Time, clockMode)}";
        }

        public override string ToString() {
            if (Date == null) {
                return "Never";
            }

            return Date.GetDateString() + " " + Time.GetTimeString();
        }

        public string GetDateTimeString() {
            return Date == null ? "Never" : $"{Calendar?.Id ?? 0}_{Date.GetDateString()}_{Clock?.Id ?? 0}_{Time.GetTimeString()}";
        }

        #region Constructors

        public MudDateTime(MudDate date, MudTime time, IMudTimeZone timezone) {
            Date = date;
            Time = time;
            TimeZone = timezone;
            Gameworld = Clock.Gameworld;
        }

        public MudDateTime(MudDateTime rhs) {
            Date = new MudDate(rhs?.Date);
            Time = new MudTime(rhs?.Time);
            TimeZone = rhs?.TimeZone;
            Gameworld = Clock.Gameworld;
        }

        public MudDateTime(string text, ICalendar calendar, IClock clock) {
            if (text.Equals("Never")) {
                Date = null;
                Time = null;
                TimeZone = null;
            }
            else {
                var splitText = text.Split(' ');
                Date = calendar.GetDate(splitText[0]);
                TimeZone = clock.Timezones.GetByName(splitText[1]);
                Time = clock.GetTime($"{splitText[1]} {splitText[2]}");
            }

            Gameworld = Clock.Gameworld;
        }

        public MudDateTime(string text, IFuturemud gameworld) {
            Gameworld = gameworld;
            if (text.Equals("Never")) {
                Date = null;
                Time = null;
                TimeZone = null;
            }
            else {
                var splitText = text.Split('_');
                var calendar = Gameworld.Calendars.Get(long.Parse(splitText[0]));
                Date = calendar.GetDate(splitText[1]);
                var clock = Gameworld.Clocks.Get(long.Parse(splitText[2]));
                var timeSplit = splitText[3].Split(' ');
                TimeZone = clock.Timezones.GetByName(timeSplit[0]);
                Time = clock.GetTime(splitText[3]);
            }
        }

        public static MudDateTime Never => new(default(MudDate), default(MudTime), default(IMudTimeZone));

        #endregion

        #region Operator Overloads

        public static bool Equals(MudDateTime dt1, MudDateTime dt2) {
            // Null datetime or null date is never. Never is less than all datetimes except itself
            if ((dt1?.Date == null) && (dt2?.Date == null)) {
                return true;
            }
            if ((dt2?.Date == null) || (dt1?.Date == null)) {
                return false;
            }

            if (dt1.TimeZone != dt2.TimeZone) {
                var newTime = dt2.Time.GetTimeByTimezone(dt1.TimeZone);
                var newDate = new MudDate(dt2.Date);
                if (newTime.DaysOffsetFromDatum != 0) {
                    newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
                }
                dt2 = new MudDateTime(newDate, newTime, dt1.TimeZone);
            }

            return dt2.Date.Equals(dt1.Date) && dt2.Time.Equals(dt1.Time);
        }

        public bool Equals(MudDateTime dt) {
            return Equals(this, dt);
        }

        public override bool Equals(object obj) {
            if (obj is MudDateTime objAsDateTime)
            {
                return Equals(this, objAsDateTime);
            }

            return obj is MudDate objAsDate && Date.Equals(objAsDate);
        }

        public override int GetHashCode() {
            return Date?.GetHashCode() + Time?.GetHashCode() + TimeZone?.GetHashCode() ?? 0;
        }

        public static MudDateTime operator +(MudDateTime dt, MudTimeSpan ts) {
            if (dt?.Date == null) {
                return Never;
            }

            var newTime = new MudTime(dt.Time);
            newTime.AddSeconds(ts.Seconds);
            newTime.AddMinutes(ts.Minutes);
            newTime.AddHours(ts.Hours);

            var newDate = new MudDate(dt.Date);
            newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
            newDate.AdvanceDays(ts.DayComponentOnly);
            newDate.AdvanceDays(newDate.Calendar.Weekdays.Count * ts.Weeks);
            newDate.AdvanceMonths(ts.Months, false, true);
            newDate.AdvanceYears(ts.Years, true);

            return new MudDateTime(newDate, newTime, dt.TimeZone);
        }

        public static MudDateTime operator -(MudDateTime dt, MudTimeSpan ts) {
            if (dt?.Date == null) {
                return Never;
            }

            var newTime = new MudTime(dt.Time);
            newTime.AddSeconds(-1*ts.Seconds);
            newTime.AddMinutes(-1*ts.Minutes);
            newTime.AddHours(-1*ts.Hours);

            var newDate = new MudDate(dt.Date);
            newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
            newDate.AdvanceDays(-1 * ts.DayComponentOnly);
            newDate.AdvanceDays(-1 * newDate.Calendar.Weekdays.Count * ts.Weeks);
            newDate.AdvanceMonths(-1 * ts.Months, false, true);
            newDate.AdvanceYears(-1 * ts.Years, true);

            return new MudDateTime(newDate, newTime, dt.TimeZone);
        }

        public static TimeSpan operator -(MudDateTime dt1, MudDateTime dt2) {
            if (dt1 == null) {
                throw new ArgumentNullException(nameof(dt1));
            }

            if (dt2 == null) {
                throw new ArgumentNullException(nameof(dt2));
            }

            if (dt1.TimeZone != dt2.TimeZone) {
                var newTime = dt2.Time.GetTimeByTimezone(dt1.TimeZone);
                MudDate newDate;
                if (newTime.DaysOffsetFromDatum == 0) {
                    newDate = dt2.Date;
                }
                else {
                    newDate = new MudDate(dt2.Date);
                    newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
                }

                dt2 = new MudDateTime(newDate, newTime, dt1.TimeZone);
            }

            return dt1.Date - dt2.Date + (dt1.Time - dt2.Time);
        }

        public static bool operator <(MudDateTime dt1, MudDateTime dt2) {
            // Null datetime or null date is never. Never is less than all datetimes except itself
            if ((dt1?.Date == null) && (dt2?.Date == null)) {
                return false;
            }
            if (dt2?.Date == null) {
                return false;
            }
            if (dt1?.Date == null) {
                return true;
            }

            if (dt1.TimeZone != dt2.TimeZone) {
                var newTime = dt2.Time.GetTimeByTimezone(dt1.TimeZone);
                var newDate = new MudDate(dt2.Date);
                if (newTime.DaysOffsetFromDatum != 0) {
                    newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
                }
                dt2 = new MudDateTime(newDate, newTime, dt1.TimeZone);
            }

            if (dt1.Date.Equals(dt2.Date)) {
                return dt1.Time < dt2.Time;
            }

            return dt1.Date < dt2.Date;
        }

        public static bool operator <=(MudDateTime dt1, MudDateTime dt2) {
            // Null datetime or null date is never. Never is less than all datetimes except itself
            if ((dt1?.Date == null) && (dt2?.Date == null)) {
                return true;
            }
            if (dt2?.Date == null) {
                return false;
            }
            if (dt1?.Date == null) {
                return true;
            }

            if (dt1.TimeZone != dt2.TimeZone) {
                var newTime = dt2.Time.GetTimeByTimezone(dt1.TimeZone);
                var newDate = new MudDate(dt2.Date);
                if (newTime.DaysOffsetFromDatum != 0) {
                    newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
                }
                dt2 = new MudDateTime(newDate, newTime, dt1.TimeZone);
            }

            if (dt1.Date.Equals(dt2.Date)) {
                return dt1.Time <= dt2.Time;
            }

            return dt1.Date < dt2.Date;
        }

        public static bool operator >(MudDateTime dt1, MudDateTime dt2) {
            // Null datetime or null date is never. Never is less than all datetimes except itself
            if ((dt1?.Date == null) && (dt2?.Date == null)) {
                return false;
            }
            if (dt2?.Date == null) {
                return true;
            }
            if (dt1?.Date == null) {
                return false;
            }

            if (dt1.TimeZone != dt2.TimeZone) {
                var newTime = dt2.Time.GetTimeByTimezone(dt1.TimeZone);
                var newDate = new MudDate(dt2.Date);
                if (newTime.DaysOffsetFromDatum != 0) {
                    newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
                }
                dt2 = new MudDateTime(newDate, newTime, dt1.TimeZone);
            }

            if (dt1.Date.Equals(dt2.Date)) {
                return dt1.Time > dt2.Time;
            }

            return dt1.Date > dt2.Date;
        }

        public static bool operator >=(MudDateTime dt1, MudDateTime dt2) {
            // Null datetime or null date is never. Never is less than all datetimes except itself
            if ((dt1?.Date == null) && (dt2?.Date == null)) {
                return true;
            }
            if (dt2?.Date == null) {
                return true;
            }
            if (dt1?.Date == null) {
                return false;
            }

            if (dt1.TimeZone != dt2.TimeZone) {
                var newTime = dt2.Time.GetTimeByTimezone(dt1.TimeZone);
                var newDate = new MudDate(dt2.Date);
                if (newTime.DaysOffsetFromDatum != 0) {
                    newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
                }
                dt2 = new MudDateTime(newDate, newTime, dt1.TimeZone);
            }

            if (dt1.Date.Equals(dt2.Date)) {
                return dt1.Time >= dt2.Time;
            }

            return dt1.Date > dt2.Date;
        }

        #endregion

        #region IFutureProgVariable implementation

        public IFutureProgVariable GetProperty(string property) {
            switch (property.ToLowerInvariant()) {
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
                    return new MudDateTime(Date, new MudTime(0, 0, 0, TimeZone, Clock, false), TimeZone);
                case "calendar":
                    return Calendar;
                case "clock":
                    return Clock;
                case "timezone":
                    return new TextVariable(TimeZone?.Alias ?? "None");
            }

            throw new NotSupportedException($"Unsupported property type {property} in MudDateTime.GetProperty");
        }

        public FutureProgVariableTypes Type => FutureProgVariableTypes.MudDateTime;

        public object GetObject => this;

        private static FutureProgVariableTypes DotReferenceHandler(string property) {
            switch (property.ToLowerInvariant()) {
                case "second":
                    return FutureProgVariableTypes.Number;
                case "minute":
                    return FutureProgVariableTypes.Number;
                case "hour":
                    return FutureProgVariableTypes.Number;
                case "day":
                    return FutureProgVariableTypes.Number;
                case "month":
                    return FutureProgVariableTypes.Text;
                case "year":
                    return FutureProgVariableTypes.Number;
                case "isnever":
                    return FutureProgVariableTypes.Boolean;
                case "midnight":
                    return FutureProgVariableTypes.MudDateTime;
                case "calendar":
                    return FutureProgVariableTypes.Calendar;
                case "clock":
                    return FutureProgVariableTypes.Clock;
                case "timezone":
                    return FutureProgVariableTypes.Text;
                default:
                    return FutureProgVariableTypes.Error;
            }
        }

        private static IReadOnlyDictionary<string,FutureProgVariableTypes> DotReferenceHandler()
        {
            return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"second", FutureProgVariableTypes.Number},
                {"minute", FutureProgVariableTypes.Number},
                {"hour", FutureProgVariableTypes.Number},
                {"day", FutureProgVariableTypes.Number},
                {"month", FutureProgVariableTypes.Text},
                {"year", FutureProgVariableTypes.Number},
                {"isnever", FutureProgVariableTypes.Boolean},
                {"midnight", FutureProgVariableTypes.MudDateTime},
                {"calendar", FutureProgVariableTypes.Calendar},
                {"clock", FutureProgVariableTypes.Clock},
                {"timezone", FutureProgVariableTypes.Text},
            };
        }

        private static IReadOnlyDictionary<string,string> DotReferenceHelp()
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

        public static void RegisterFutureProgCompiler() {
            FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.MudDateTime, DotReferenceHandler(), DotReferenceHelp());
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
	        if (left is null)
	        {
		        if (right is null)
		        {
			        return 0;
		        }

		        return -1;
	        }

	        if (right is null)
	        {
		        return 1;
	        }

	        var rTz = right.GetByTimeZone(left.TimeZone);
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
using MudSharp.Framework;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp.TimeAndDate.Intervals
{
    public class RecurringInterval
    {
        private const int MaxOrdinalMonthSearches = 10000;

        private static readonly Regex LegacyParseRegex =
            new(@"^every\s*(?<amount>[0-9]+)*\s*(?<interval>minutes?|hours?|days?|months?|weekdays?|weeks?|years?)(?:\s+(?<modifier>[+-]?\d+))*\s*$",
                RegexOptions.IgnoreCase);

        private static readonly Regex OrdinalDayParseRegex =
            new(@"^every\s*(?<amount>[0-9]+)*\s*months?\s+on\s+(?:the\s+)?(?:(?<last>last)\s+day|day\s+(?<day>\d+)|(?<ordinal>\d+)(?:st|nd|rd|th)?)\s*$",
                RegexOptions.IgnoreCase);

        private static readonly Regex OrdinalWeekdayParseRegex =
            new(@"^every\s*(?<amount>[0-9]+)*\s*months?\s+on\s+(?:the\s+)?(?<ordinal>\d+)(?:st|nd|rd|th)?(?<orlast>\s+or\s+last)?\s+(?<weekday>.+?)\s*$",
                RegexOptions.IgnoreCase);

        private static readonly Regex LastWeekdayParseRegex =
            new(@"^every\s*(?<amount>[0-9]+)*\s*months?\s+on\s+(?:the\s+)?last\s+(?<weekday>.+?)\s*$",
                RegexOptions.IgnoreCase);

        public IntervalType Type { get; init; }
        public int IntervalAmount { get; init; }
        public int Modifier { get; init; }
        public int SecondaryModifier { get; init; }
        public OrdinalFallbackMode OrdinalFallbackMode { get; init; }

        public override string ToString()
        {
            return Type switch
            {
                IntervalType.OrdinalDayOfMonth => Modifier == -1
                    ? $"every {IntervalAmount:F0} months on last day"
                    : $"every {IntervalAmount:F0} months on day {Modifier:F0}",
                IntervalType.OrdinalWeekdayOfMonth => Modifier == -1
                    ? $"every {IntervalAmount:F0} months on last weekday {SecondaryModifier:F0}"
                    : $"every {IntervalAmount:F0} months on the {Modifier:F0}{(OrdinalFallbackMode == OrdinalFallbackMode.OrLast ? " or last" : "")} weekday {SecondaryModifier:F0}",
                _ => $"every {IntervalAmount:F0} {Type switch { IntervalType.Daily => "days", IntervalType.Hourly => "hours", IntervalType.Minutely => "minutes", IntervalType.Monthly => "months", IntervalType.SpecificWeekday => "weekdays", IntervalType.Weekly => "weeks", IntervalType.Yearly => "years", _ => "days" }} {Modifier:+000}"
            };
        }

        public static RecurringInterval Parse(string text)
        {
            if (!TryParse(text, null, out RecurringInterval interval, out _))
            {
                throw new ArgumentOutOfRangeException(nameof(text));
            }

            return interval;
        }

        public static bool TryParse(string text, out RecurringInterval interval)
        {
            return TryParse(text, null, out interval, out _);
        }

        public static bool TryParse(string text, ICalendar calendar, out RecurringInterval interval)
        {
            return TryParse(text, calendar, out interval, out _);
        }

        public static bool TryParse(string text, ICalendar calendar, out RecurringInterval interval, out string error)
        {
            interval = null;
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(text))
            {
                error = "No interval text was supplied.";
                return false;
            }

            text = text.Trim();
            if (TryParseOrdinalDay(text, out interval, out error))
            {
                return true;
            }

            if (TryParseLastWeekday(text, calendar, out interval, out error))
            {
                return true;
            }

            if (TryParseOrdinalWeekday(text, calendar, out interval, out error))
            {
                return true;
            }

            return TryParseLegacy(text, out interval, out error);
        }

        private static bool TryParseLegacy(string text, out RecurringInterval interval, out string error)
        {
            interval = null;
            error = string.Empty;
            Match match = LegacyParseRegex.Match(text);
            if (!match.Success)
            {
                error = "The interval did not match a known interval grammar.";
                return false;
            }

            int amount = ParseAmount(match);
            if (amount <= 0)
            {
                error = "The interval amount must be greater than zero.";
                return false;
            }

            IntervalType intervalType;
            switch (match.Groups["interval"].Value.ToLowerInvariant())
            {
                case "minute":
                case "minutes":
                    intervalType = IntervalType.Minutely;
                    break;
                case "hour":
                case "hours":
                    intervalType = IntervalType.Hourly;
                    break;
                case "day":
                case "days":
                    intervalType = IntervalType.Daily;
                    break;
                case "week":
                case "weeks":
                    intervalType = IntervalType.Weekly;
                    break;
                case "month":
                case "months":
                    intervalType = IntervalType.Monthly;
                    break;
                case "year":
                case "years":
                    intervalType = IntervalType.Yearly;
                    break;
                case "weekday":
                case "weekdays":
                    intervalType = IntervalType.SpecificWeekday;
                    break;
                default:
                    error = $"Invalid interval type \"{match.Groups["interval"].Value}\".";
                    return false;
            }

            int modifier = 0;
            if (match.Groups["modifier"].Length > 0 && !int.TryParse(match.Groups["modifier"].Value, out modifier))
            {
                error = $"Invalid interval modifier \"{match.Groups["modifier"].Value}\".";
                return false;
            }

            interval = new RecurringInterval { IntervalAmount = amount, Modifier = modifier, Type = intervalType };
            return true;
        }

        private static bool TryParseOrdinalDay(string text, out RecurringInterval interval, out string error)
        {
            interval = null;
            error = string.Empty;
            Match match = OrdinalDayParseRegex.Match(text);
            if (!match.Success)
            {
                return false;
            }

            int amount = ParseAmount(match);
            if (amount <= 0)
            {
                error = "The interval amount must be greater than zero.";
                return false;
            }

            int day;
            if (match.Groups["last"].Success)
            {
                day = -1;
            }
            else
            {
                string value = match.Groups["day"].Success ? match.Groups["day"].Value : match.Groups["ordinal"].Value;
                day = int.Parse(value);
                if (day <= 0)
                {
                    error = "The day of month must be greater than zero.";
                    return false;
                }
            }

            interval = new RecurringInterval
            {
                IntervalAmount = amount,
                Type = IntervalType.OrdinalDayOfMonth,
                Modifier = day
            };
            return true;
        }

        private static bool TryParseLastWeekday(string text, ICalendar calendar, out RecurringInterval interval, out string error)
        {
            interval = null;
            error = string.Empty;
            Match match = LastWeekdayParseRegex.Match(text);
            if (!match.Success)
            {
                return false;
            }

            int amount = ParseAmount(match);
            if (amount <= 0)
            {
                error = "The interval amount must be greater than zero.";
                return false;
            }

            if (!TryParseWeekday(match.Groups["weekday"].Value, calendar, out int weekday, out error))
            {
                return false;
            }

            interval = new RecurringInterval
            {
                IntervalAmount = amount,
                Type = IntervalType.OrdinalWeekdayOfMonth,
                Modifier = -1,
                SecondaryModifier = weekday
            };
            return true;
        }

        private static bool TryParseOrdinalWeekday(string text, ICalendar calendar, out RecurringInterval interval, out string error)
        {
            interval = null;
            error = string.Empty;
            Match match = OrdinalWeekdayParseRegex.Match(text);
            if (!match.Success)
            {
                return false;
            }

            int amount = ParseAmount(match);
            if (amount <= 0)
            {
                error = "The interval amount must be greater than zero.";
                return false;
            }

            int ordinal = int.Parse(match.Groups["ordinal"].Value);
            if (ordinal <= 0)
            {
                error = "The ordinal must be greater than zero.";
                return false;
            }

            if (!TryParseWeekday(match.Groups["weekday"].Value, calendar, out int weekday, out error))
            {
                return false;
            }

            interval = new RecurringInterval
            {
                IntervalAmount = amount,
                Type = IntervalType.OrdinalWeekdayOfMonth,
                Modifier = ordinal,
                SecondaryModifier = weekday,
                OrdinalFallbackMode = match.Groups["orlast"].Success
                    ? OrdinalFallbackMode.OrLast
                    : OrdinalFallbackMode.ExactOnly
            };
            return true;
        }

        private static int ParseAmount(Match match)
        {
            return match.Groups["amount"].Length > 0 ? int.Parse(match.Groups["amount"].Value) : 1;
        }

        private static bool TryParseWeekday(string text, ICalendar calendar, out int weekday, out string error)
        {
            error = string.Empty;
            weekday = -1;
            text = text.Trim();
            if (text.StartsWith("weekday ", StringComparison.InvariantCultureIgnoreCase))
            {
                text = text[8..].Trim();
            }

            if (int.TryParse(text, out weekday))
            {
                if (calendar == null || weekday >= 0 && weekday < calendar.Weekdays.Count)
                {
                    return true;
                }

                error = $"The weekday index {weekday.ToString("N0")} is outside this calendar's weekday range.";
                return false;
            }

            if (calendar == null)
            {
                error = "Weekday names require a calendar-aware interval parser.";
                return false;
            }

            weekday = calendar.Weekdays.FindIndex(x => x.EqualTo(text) || x.Pluralise().EqualTo(text));
            if (weekday >= 0)
            {
                return true;
            }

            error = $"The weekday \"{text}\" does not exist in the {calendar.ShortName} calendar.";
            return false;
        }

        public string Describe(ICalendar whichCalendar)
        {
            switch (Type)
            {
                case IntervalType.Minutely:
                    return IntervalAmount == 1 ? "every minute" : $"every {IntervalAmount} minutes";
                case IntervalType.Hourly:
                    return IntervalAmount == 1 ? "every hour" : $"every {IntervalAmount} hours";
                case IntervalType.Daily:
                    return IntervalAmount == 1 ? "every day" : $"every {IntervalAmount} days";
                case IntervalType.Monthly:
                    return IntervalAmount == 1 ? "every month" : $"every {IntervalAmount} months";
                case IntervalType.SpecificWeekday:
                    return IntervalAmount == 1
                        ? $"every {whichCalendar.Weekdays[Modifier]}"
                        : $"every {IntervalAmount} {whichCalendar.Weekdays[Modifier].Pluralise()}";
                case IntervalType.Weekly:
                    return IntervalAmount == 1 ? "every week" : $"every {IntervalAmount} weeks";
                case IntervalType.Yearly:
                    return IntervalAmount == 1 ? "every year" : $"every {IntervalAmount} years";
                case IntervalType.OrdinalDayOfMonth:
                    return Modifier == -1
                        ? IntervalAmount == 1 ? "every month on the last day" : $"every {IntervalAmount} months on the last day"
                        : IntervalAmount == 1 ? $"every month on the {Modifier.ToOrdinal()}" : $"every {IntervalAmount} months on the {Modifier.ToOrdinal()}";
                case IntervalType.OrdinalWeekdayOfMonth:
                    var weekday = whichCalendar != null && SecondaryModifier >= 0 && SecondaryModifier < whichCalendar.Weekdays.Count
                        ? whichCalendar.Weekdays[SecondaryModifier]
                        : $"weekday {SecondaryModifier}";
                    var ordinalText = Modifier == -1 ? "last" : Modifier.ToOrdinal();
                    var fallbackText = OrdinalFallbackMode == OrdinalFallbackMode.OrLast ? " or last" : string.Empty;
                    return IntervalAmount == 1
                        ? $"every month on the {ordinalText}{fallbackText} {weekday}"
                        : $"every {IntervalAmount} months on the {ordinalText}{fallbackText} {weekday}";
                default:
                    throw new NotSupportedException("RecurringInterval.Describe found an unknown IntervalType.");
            }
        }

        public MudDateTime GetNextAdjacentToCurrent(MudDateTime referenceDateTime)
        {
            return GetNextDateTime(GetLastDateTime(referenceDateTime));
        }

        public MudDateTime GetLastDateTime(MudDateTime referenceDateTime)
        {
            MudDateTime newDate = IsOrdinalMonthly
                ? AlignOrdinalDateTime(referenceDateTime, -1)
                : new MudDateTime(referenceDateTime);
            MudDateTime currentDateTime = CurrentDateTimeFor(referenceDateTime);

            while (newDate > currentDateTime)
            {
                newDate = MoveDateTimeByInterval(newDate, -1);
            }

            return newDate;
        }

        public DateTime GetNextDateTime(DateTime referenceDateTime)
        {
            switch (Type)
            {
                case IntervalType.Daily:
                    return referenceDateTime.AddDays(IntervalAmount);
                case IntervalType.Weekly:
                    return referenceDateTime.AddDays(7 * IntervalAmount);
                case IntervalType.Monthly:
                case IntervalType.OrdinalDayOfMonth:
                case IntervalType.OrdinalWeekdayOfMonth:
                    return referenceDateTime.AddMonths(IntervalAmount);
                case IntervalType.Yearly:
                    return referenceDateTime.AddYears(IntervalAmount);
                case IntervalType.SpecificWeekday:
                    DateTime result = referenceDateTime;
                    for (int i = 0; i < 7; i++)
                    {
                        result = result.AddDays(1);
                        if (result.DayOfWeek == (DayOfWeek)Modifier)
                        {
                            return IntervalAmount > 1 ? result.AddDays(7 * (IntervalAmount - 1)) : result;
                        }
                    }

                    throw new NotSupportedException("There were more than 7 days of the week. A non-UTC date was probably passed in.");
                case IntervalType.Hourly:
                    return referenceDateTime.AddHours(IntervalAmount);
                case IntervalType.Minutely:
                    return referenceDateTime.AddMinutes(IntervalAmount);
            }

            throw new NotSupportedException("The IntervalType was not supported in RecurringInterval.GetNextDateTime");
        }

        public MudDateTime GetNextDateTime(MudDateTime referenceDateTime)
        {
            MudDateTime newDate = IsOrdinalMonthly
                ? AlignOrdinalDateTime(referenceDateTime, 1)
                : new MudDateTime(referenceDateTime);
            MudDateTime currentDateTime = CurrentDateTimeFor(referenceDateTime);

            while (newDate <= currentDateTime)
            {
                newDate = MoveDateTimeByInterval(newDate, 1);
            }
            return newDate;
        }

        public MudDate GetNextDateExclusive(ICalendar whichCalendar, MudDate referenceDate)
        {
            MudDate newDate = IsOrdinalMonthly
                ? AlignOrdinalDate(referenceDate, 1)
                : new MudDate(referenceDate);
            while (newDate <= whichCalendar.CurrentDate)
            {
                newDate = MoveDateByInterval(newDate, 1);
            }

            return newDate;
        }

        public MudDate GetNextDate(ICalendar whichCalendar, MudDate referenceDate)
        {
            MudDate newDate = IsOrdinalMonthly
                ? AlignOrdinalDate(referenceDate, 1)
                : new MudDate(referenceDate);
            while (newDate < whichCalendar.CurrentDate)
            {
                newDate = MoveDateByInterval(newDate, 1);
            }

            return newDate;
        }

        public double DaysPerInterval(ICalendar calendar)
        {
            switch (Type)
            {
                case IntervalType.Daily:
                    return IntervalAmount;
                case IntervalType.Weekly:
                case IntervalType.SpecificWeekday:
                    return calendar.Weekdays.Count * IntervalAmount;
                case IntervalType.Monthly:
                case IntervalType.OrdinalDayOfMonth:
                case IntervalType.OrdinalWeekdayOfMonth:
                    return calendar.CurrentDate.ThisYear.Months.Average(x => x.Days) * IntervalAmount;
                case IntervalType.Yearly:
                    return calendar.CountDaysInYear(calendar.EpochYear) * IntervalAmount;
                case IntervalType.Hourly:
                    return IntervalAmount / (double)calendar.FeedClock.HoursPerDay;
                case IntervalType.Minutely:
                    return IntervalAmount / ((double)calendar.FeedClock.MinutesPerHour * calendar.FeedClock.HoursPerDay);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsOrdinalMonthly => Type is IntervalType.OrdinalDayOfMonth or IntervalType.OrdinalWeekdayOfMonth;

        private static MudDateTime CurrentDateTimeFor(MudDateTime referenceDateTime)
        {
            var currentDateTime = new MudDateTime(referenceDateTime.Calendar.CurrentDate,
                referenceDateTime.Clock.CurrentTime, referenceDateTime.Clock.PrimaryTimezone).GetByTimeZone(
                referenceDateTime.TimeZone);
            currentDateTime.Time.DaysOffsetFromDatum = 0;
            return currentDateTime;
        }

        private MudDateTime AlignOrdinalDateTime(MudDateTime referenceDateTime, int direction)
        {
            var date = AlignOrdinalDate(referenceDateTime.Date, direction);
            return new MudDateTime(date, MudTime.CopyOf(referenceDateTime.Time, true), referenceDateTime.TimeZone);
        }

        private MudDate AlignOrdinalDate(MudDate referenceDate, int direction)
        {
            var candidate = ResolveOrdinalDateInMonth(referenceDate.Calendar, referenceDate.ThisYear, referenceDate.Month);
            if (candidate != null && (direction > 0 ? candidate >= referenceDate : candidate <= referenceDate))
            {
                return candidate;
            }

            return MoveOrdinalDate(referenceDate, direction);
        }

        private MudDateTime MoveDateTimeByInterval(MudDateTime referenceDateTime, int direction)
        {
            var newDate = MoveDateByInterval(referenceDateTime.Date, direction);
            var newTime = MudTime.CopyOf(referenceDateTime.Time);
            if (Type == IntervalType.Minutely)
            {
                newDate = new MudDate(referenceDateTime.Date);
                newTime.DaysOffsetFromDatum = 0;
                newTime.AddMinutes(direction * IntervalAmount);
                ApplyTimeDayOffset(newDate, newTime);
            }
            else if (Type == IntervalType.Hourly)
            {
                newDate = new MudDate(referenceDateTime.Date);
                newTime.DaysOffsetFromDatum = 0;
                newTime.AddHours(direction * IntervalAmount);
                ApplyTimeDayOffset(newDate, newTime);
            }

            return new MudDateTime(newDate, newTime, referenceDateTime.TimeZone);
        }

        private static void ApplyTimeDayOffset(MudDate newDate, MudTime newTime)
        {
            if (newTime.DaysOffsetFromDatum != 0)
            {
                newDate.AdvanceDays(newTime.DaysOffsetFromDatum);
                newTime.DaysOffsetFromDatum = 0;
            }
        }

        private MudDate MoveDateByInterval(MudDate referenceDate, int direction)
        {
            var newDate = new MudDate(referenceDate);
            switch (Type)
            {
                case IntervalType.Minutely:
                case IntervalType.Hourly:
                case IntervalType.Daily:
                    newDate.AdvanceDays(direction * IntervalAmount);
                    break;
                case IntervalType.Monthly:
                    newDate.AdvanceMonths(direction * IntervalAmount, true, true);
                    break;
                case IntervalType.SpecificWeekday:
                    newDate.AdvanceToNextWeekday(Modifier, direction * IntervalAmount);
                    break;
                case IntervalType.Weekly:
                    newDate.AdvanceDays(direction * IntervalAmount * referenceDate.Calendar.Weekdays.Count);
                    break;
                case IntervalType.Yearly:
                    newDate.AdvanceYears(direction * IntervalAmount, true);
                    break;
                case IntervalType.OrdinalDayOfMonth:
                case IntervalType.OrdinalWeekdayOfMonth:
                    return MoveOrdinalDate(referenceDate, direction);
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return newDate;
        }

        private MudDate MoveOrdinalDate(MudDate referenceDate, int direction)
        {
            var cursor = new MudDate(referenceDate);
            for (int i = 0; i < MaxOrdinalMonthSearches; i++)
            {
                cursor.AdvanceMonths(direction * IntervalAmount, true, true);
                var candidate = ResolveOrdinalDateInMonth(cursor.Calendar, cursor.ThisYear, cursor.Month);
                if (candidate != null)
                {
                    return candidate;
                }
            }

            throw new InvalidOperationException($"Could not find a valid {Describe(referenceDate.Calendar)} occurrence within {MaxOrdinalMonthSearches.ToString("N0")} searched months.");
        }

        private MudDate ResolveOrdinalDateInMonth(ICalendar calendar, Year year, Month month)
        {
            return Type switch
            {
                IntervalType.OrdinalDayOfMonth => ResolveOrdinalDayOfMonth(calendar, year, month),
                IntervalType.OrdinalWeekdayOfMonth => ResolveOrdinalWeekdayOfMonth(calendar, year, month),
                _ => throw new InvalidOperationException("ResolveOrdinalDateInMonth called for a non-ordinal interval.")
            };
        }

        private MudDate ResolveOrdinalDayOfMonth(ICalendar calendar, Year year, Month month)
        {
            var day = Modifier == -1 ? month.Days : Math.Min(Modifier, month.Days);
            return new MudDate(calendar, day, year.YearName, month, year, false);
        }

        private MudDate ResolveOrdinalWeekdayOfMonth(ICalendar calendar, Year year, Month month)
        {
            MudDate lastMatch = null;
            var seen = 0;
            for (int day = 1; day <= month.Days; day++)
            {
                var candidate = new MudDate(calendar, day, year.YearName, month, year, false);
                if (candidate.WeekdayIndex != SecondaryModifier)
                {
                    continue;
                }

                lastMatch = candidate;
                seen++;
                if (Modifier > 0 && seen == Modifier)
                {
                    return candidate;
                }
            }

            if (Modifier == -1 || OrdinalFallbackMode == OrdinalFallbackMode.OrLast)
            {
                return lastMatch;
            }

            return null;
        }
    }
}

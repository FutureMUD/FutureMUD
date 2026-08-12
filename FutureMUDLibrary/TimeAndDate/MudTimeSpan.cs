using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TimeSpanParserUtil;

namespace MudSharp.TimeAndDate
{
    public class MudTimeSpan : IComparable, IComparable<MudTimeSpan>, IEquatable<MudTimeSpan>
    {
        public const long MillisecondsPerSecond = 1000;
        private const double SecondsPerMillisecond = 1.0 / MillisecondsPerSecond;

        public const long MillisecondsPerMinute = MillisecondsPerSecond * 60;
        private const double MinutesPerMillisecond = 1.0 / MillisecondsPerMinute;

        public const long MillisecondsPerHour = MillisecondsPerMinute * 60;
        private const double HoursPerMillisecond = 1.0 / MillisecondsPerHour;

        public const long MillisecondsPerDay = MillisecondsPerHour * 24;
        private const double DaysPerMillisecond = 1.0 / MillisecondsPerDay;

        public const long MillisecondsPerWeek = MillisecondsPerDay * 7;
        private const double WeeksPerMillisecond = 1.0 / MillisecondsPerWeek;

        public const long MillisecondsPerMonth = MillisecondsPerDay * 30;
        private const double MonthsPerMillisecond = 1.0 / MillisecondsPerMonth;

        public const long MillisecondsPerYear = MillisecondsPerDay * 365;
        private const double YearsPerMillisecond = 1.0 / MillisecondsPerYear;

        internal const long MaxSeconds = long.MaxValue / (MillisecondsPerSecond * 10000);
        internal const long MinSeconds = long.MinValue / (MillisecondsPerSecond * 10000);
        internal const long MaxMilliSeconds = long.MaxValue / 10000;
        internal const long MinMilliSeconds = long.MinValue / 10000;

        private long _milliseconds;
        public long Milliseconds => _milliseconds +
                    (_weeks * MillisecondsPerWeek) +
                    (_months * MillisecondsPerMonth) +
                    (_years * MillisecondsPerYear)
                    ;

        public int MillisecondComponentOnly => (int)(_milliseconds % MillisecondsPerSecond);

        public int Seconds => (int)(Milliseconds / MillisecondsPerSecond);

        public int SecondComponentOnly => (int)(_milliseconds % MillisecondsPerMinute / MillisecondsPerSecond);

        public int Minutes => (int)(Milliseconds / MillisecondsPerMinute);

        public int MinuteComponentOnly => (int)(_milliseconds % MillisecondsPerHour / MillisecondsPerMinute);

        public int Hours => (int)(Milliseconds / MillisecondsPerHour);

        public int HourComponentOnly => (int)((_milliseconds % MillisecondsPerDay) / MillisecondsPerHour);

        public int Days => (int)(Milliseconds / MillisecondsPerDay);

        public int DayComponentOnly => (int)(_milliseconds / MillisecondsPerDay);

        private int _weeks;
        public int Weeks => _weeks;

        private int _months;
        public int Months => _months;

        private int _years;
        public int Years => _years;

        public long Ticks => Milliseconds * 10000;

        public static readonly MudTimeSpan Zero = new(0);
        public static readonly MudTimeSpan MaxValue = new(long.MaxValue);
        public static readonly MudTimeSpan MinValue = new(long.MinValue);

        public TimeSpan AsTimeSpan()
        {
            return TimeSpan.FromMilliseconds(Milliseconds);
        }

        public MudTimeSpan Inverse()
        {
            return new MudTimeSpan(checked(-_years), checked(-_months), checked(-_weeks), checked(-_milliseconds));
        }

        public static implicit operator TimeSpan(MudTimeSpan mts)
        {
            return mts.AsTimeSpan();
        }

        public static implicit operator MudTimeSpan(TimeSpan ts)
        {
            return new MudTimeSpan(ts.Ticks);
        }

        internal static long TimeToMilliseconds(int hour, int minute, int second)
        {
            // totalSeconds is bounded by 2^31 * 2^12 + 2^31 * 2^8 + 2^31,
            // which is less than 2^44, meaning we won't overflow totalSeconds.
            long totalSeconds = (long)hour * 3600 + (long)minute * 60 + second;
            if (totalSeconds > MaxSeconds || totalSeconds < MinSeconds)
            {
                throw new ArgumentOutOfRangeException();
            }

            return totalSeconds * MillisecondsPerSecond;
        }

        public MudTimeSpan(MudTimeSpan rhs)
        {
            _milliseconds = rhs._milliseconds;
            _weeks = rhs.Weeks;
            _months = rhs.Months;
            _years = rhs.Years;
        }

        public MudTimeSpan(long ticks)
        {
            _milliseconds = ticks / 10000;
        }

        public MudTimeSpan(int hours, int minutes, int seconds)
        {
            _milliseconds = TimeToMilliseconds(hours, minutes, seconds);
        }

        public MudTimeSpan(int days, int hours, int minutes, int seconds)
            : this(0, 0, 0, days, hours, minutes, seconds, 0)
        {
        }

        public MudTimeSpan(int years, int months, int weeks, long days, long hours, long minutes, long seconds, long milliseconds)
        {
            _milliseconds = checked((checked(days * 3600 * 24) + checked(hours * 3600) + checked(minutes * 60) +
                                     seconds) * 1000 + milliseconds);
            _months = months;
            _years = years;
            _weeks = weeks;
            if (Milliseconds > MaxMilliSeconds || Milliseconds < MinMilliSeconds)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public MudTimeSpan(int years, int months, int weeks, double days)
        {
            _milliseconds = (long)(days * MillisecondsPerDay);
            _months = months;
            _years = years;
            _weeks = weeks;
            if (Milliseconds > MaxMilliSeconds || Milliseconds < MinMilliSeconds)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public MudTimeSpan(int years, int months, int weeks, long milliseconds)
        {
            _milliseconds = milliseconds;
            _months = months;
            _years = years;
            _weeks = weeks;
            if (Milliseconds > MaxMilliSeconds || Milliseconds < MinMilliSeconds)
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public MudTimeSpan(int years, int months, int weeks, TimeSpan extra)
        {
            _years = years;
            _months = months;
            _weeks = weeks;
            _milliseconds = (long)extra.TotalMilliseconds;
        }

        public static MudTimeSpan FromSeconds(double seconds)
        {
            return new MudTimeSpan((long)(seconds * MillisecondsPerSecond * 10000));
        }

        public static MudTimeSpan FromMinutes(double minutes)
        {
            return new MudTimeSpan((long)(minutes * MillisecondsPerMinute * 10000));
        }

        public static MudTimeSpan FromHours(double hours)
        {
            return new MudTimeSpan((long)(hours * MillisecondsPerHour * 10000));
        }

        public static MudTimeSpan FromDays(double days)
        {
            return new MudTimeSpan((long)(days * MillisecondsPerDay * 10000));
        }

        public static MudTimeSpan FromWeeks(int weeks, double days = 0.0)
        {
            return new MudTimeSpan(0, 0, weeks, days);
        }

        public static MudTimeSpan FromMonths(int months, double days = 0.0)
        {
            return new MudTimeSpan(0, months, 0, days);
        }

        public static MudTimeSpan FromYears(int years, double days = 0.0)
        {
            return new MudTimeSpan(years, 0, 0, days);
        }

        private static readonly Regex UnitRegex = new(@"\G\s*(?<quantity>[+-]?\d+)\s*(?<unit>milliseconds?|ms|seconds?|secs?|s|months?|mons?|mo|minutes?|mins?|m|hours?|hrs?|h|days?|d|weeks?|w|years?|y)\s*", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        public static bool TryParse(string text, IFormatProvider format, out MudTimeSpan timespan)
        {
			// Persisted round-trip text is intentionally culture-invariant. Retain the format
			// parameter for source compatibility with the historic parser overload.
			_ = format;
            if (string.IsNullOrWhiteSpace(text))
            {
                timespan = Zero;
                return false;
            }

            var trimmedText = text.Trim();
            if (trimmedText.EqualToAny("zero", "none", "nothing"))
            {
                timespan = Zero;
                return true;
            }

            int years = 0;
            int months = 0;
            int weeks = 0;
            long days = 0L;
            long hours = 0L;
            long minutes = 0L;
            long seconds = 0L;
            long milliseconds = 0L;

            var position = 0;
            while (position < trimmedText.Length)
            {
                var match = UnitRegex.Match(trimmedText, position);
                if (!match.Success || match.Index != position)
                {
                    if (position == 0 && TimeSpanParser.TryParse(trimmedText,
                            new TimeSpanParserOptions
                            {
                                FormatProvider = CultureInfo.InvariantCulture,
                                AllowUnitlessZero = true,
                                ColonedDefault = Units.Days
                            }, out TimeSpan parsedSpan))
                    {
                        timespan = new MudTimeSpan(parsedSpan);
                        return true;
                    }

                    timespan = Zero;
                    return false;
                }

                position += match.Length;
                try
                {
                    var quantity = long.Parse(match.Groups["quantity"].Value, CultureInfo.InvariantCulture);
                    switch (match.Groups["unit"].Value.ToLowerInvariant())
                    {
                        case "ms":
                        case "millisecond":
                        case "milliseconds":
                            milliseconds = checked(milliseconds + quantity);
                            continue;
                        case "s":
                        case "second":
                        case "seconds":
                        case "sec":
                        case "secs":
                            seconds = checked(seconds + quantity);
                            continue;
                        case "m":
                        case "min":
                        case "mins":
                        case "minute":
                        case "minutes":
                            minutes = checked(minutes + quantity);
                            continue;
                        case "h":
                        case "hr":
                        case "hrs":
                        case "hour":
                        case "hours":
                            hours = checked(hours + quantity);
                            continue;
                        case "d":
                        case "day":
                        case "days":
                            days = checked(days + quantity);
                            continue;
                        case "w":
                        case "week":
                        case "weeks":
                            weeks = checked(weeks + (int)quantity);
                            continue;
                        case "mo":
                        case "mon":
                        case "mons":
                        case "month":
                        case "months":
                            months = checked(months + (int)quantity);
                            continue;
                        case "y":
                        case "year":
                        case "years":
                            years = checked(years + (int)quantity);
                            continue;
                    }
                }
                catch (OverflowException)
                {
                    timespan = Zero;
                    return false;
                }
            }

            try
            {
                timespan = new MudTimeSpan(years, months, weeks, days, hours, minutes, seconds, milliseconds);
                return true;
            }
            catch (OverflowException)
            {
                timespan = Zero;
                return false;
            }
        }

        public static bool TryParse(string text, out MudTimeSpan timespan)
        {
            return TryParse(text, CultureInfo.InvariantCulture, out timespan);
        }

        public static MudTimeSpan Parse(string text)
        {
            if (!TryParse(text, out MudTimeSpan ts))
            {
                throw new ApplicationException("Error parsing MudTimeSpan in Parse method");
            }

            return ts;
        }

        public string GetRoundTripParseText
        {
            get
            {
                List<string> strings = new();
                if (_years != 0)
                {
                    strings.Add($"{_years.ToString(CultureInfo.InvariantCulture)} years");
                }
                if (_months != 0)
                {
                    strings.Add($"{_months.ToString(CultureInfo.InvariantCulture)} months");
                }
                if (_weeks != 0)
                {
                    strings.Add($"{_weeks.ToString(CultureInfo.InvariantCulture)} weeks");
                }
                if (_milliseconds != 0)
                {
                    strings.Add($"{_milliseconds.ToString(CultureInfo.InvariantCulture)}ms");
                }
                return strings.DefaultIfEmpty("zero").ListToString(separator: " ", conjunction: "");
            }
        }

        public double TotalDays => Milliseconds * DaysPerMillisecond;

        public double TotalHours => Milliseconds * HoursPerMillisecond;

        public double TotalMilliseconds
        {
            get
            {
                if (Milliseconds > MaxMilliSeconds)
                {
                    return MaxMilliSeconds;
                }

                if (Milliseconds < MinMilliSeconds)
                {
                    return MinMilliSeconds;
                }

                return Milliseconds;
            }
        }

        public double TotalMinutes => Milliseconds * MinutesPerMillisecond;

        public double TotalSeconds => Milliseconds * SecondsPerMillisecond;

        public MudTimeSpan Add(MudTimeSpan ts)
        {
            ArgumentNullException.ThrowIfNull(ts);
            return new MudTimeSpan(checked(_years + ts._years), checked(_months + ts._months),
                checked(_weeks + ts._weeks), checked(_milliseconds + ts._milliseconds));
        }

        public MudTimeSpan Subtract(MudTimeSpan ts)
        {
            ArgumentNullException.ThrowIfNull(ts);
            return new MudTimeSpan(checked(_years - ts._years), checked(_months - ts._months),
                checked(_weeks - ts._weeks), checked(_milliseconds - ts._milliseconds));
        }

        // Compares two MudTimeSpan values, returning an integer that indicates their
        // relationship.
        //
        public static int Compare(MudTimeSpan t1, MudTimeSpan t2)
        {
            return t1.CompareTo(t2);
        }

        // Compares two MudTimeSpan values, returning an integer that indicates their
        // relationship.
        //
        public static int Compare(MudTimeSpan t1, TimeSpan t2)
        {
            return t1.Ticks.CompareTo(t2.Ticks);
        }

        // Returns a value less than zero if this  object
        public int CompareTo(MudTimeSpan ts)
        {
            if (ts is null)
            {
                return 1;
            }

            var approximateComparison = Milliseconds.CompareTo(ts.Milliseconds);
            if (approximateComparison != 0)
            {
                return approximateComparison;
            }

            var yearsComparison = _years.CompareTo(ts._years);
            if (yearsComparison != 0) return yearsComparison;
            var monthsComparison = _months.CompareTo(ts._months);
            if (monthsComparison != 0) return monthsComparison;
            var weeksComparison = _weeks.CompareTo(ts._weeks);
            if (weeksComparison != 0) return weeksComparison;
            return _milliseconds.CompareTo(ts._milliseconds);
        }

        public int CompareTo(object value)
        {
            if (value is null)
            {
                return 1;
            }

            if (value is not MudTimeSpan timeSpan)
            {
                throw new ArgumentException(nameof(value));
            }

            return CompareTo(timeSpan);
        }

        public static MudTimeSpan operator -(MudTimeSpan t)
        {
            ArgumentNullException.ThrowIfNull(t);
            return t.Inverse();
        }

        public static MudTimeSpan operator -(MudTimeSpan t1, MudTimeSpan t2)
        {
            return t1.Subtract(t2);
        }

        public static MudTimeSpan operator +(MudTimeSpan t)
        {
            return t;
        }

        public static MudTimeSpan operator +(MudTimeSpan t1, MudTimeSpan t2)
        {
            return t1.Add(t2);
        }

        public static bool operator ==(MudTimeSpan t1, MudTimeSpan t2)
        {
            return Equals(t1, t2);
        }

        public static bool operator !=(MudTimeSpan t1, MudTimeSpan t2)
        {
            return !Equals(t1, t2);
        }

        public static bool operator <(MudTimeSpan t1, MudTimeSpan t2)
        {
            return Compare(t1, t2) < 0;
        }

        public static bool operator <=(MudTimeSpan t1, MudTimeSpan t2)
        {
            return Compare(t1, t2) <= 0;
        }

        public static bool operator >(MudTimeSpan t1, MudTimeSpan t2)
        {
            return Compare(t1, t2) > 0;
        }

        public static bool operator >=(MudTimeSpan t1, MudTimeSpan t2)
        {
            return Compare(t1, t2) >= 0;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_years, _months, _weeks, _milliseconds);
        }

        public bool Equals(MudTimeSpan ts)
        {
            return ts is not null &&
                   _years == ts._years &&
                   _months == ts._months &&
                   _weeks == ts._weeks &&
                   _milliseconds == ts._milliseconds;
        }

        public override bool Equals(object obj)
        {
            return obj is MudTimeSpan ts && Equals(ts);
        }
    }
}

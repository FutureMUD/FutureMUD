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
    public class MudTimeSpan
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
        public long Milliseconds
        {
            get
            {
                return 
                    _milliseconds + 
                    (_weeks * MillisecondsPerWeek) +
                    (_months * MillisecondsPerMonth) +
                    (_years * MillisecondsPerYear)
                    ;
            }
        }

        public int MillisecondComponentOnly
        {
            get
            {
                return (int)_milliseconds;
            }
        }

        public int Seconds
        {
            get
            {
                return (int)(Milliseconds % SecondsPerMillisecond);
            }
        }

        public int SecondComponentOnly
        {
            get
            {
                return (int)(_milliseconds % MillisecondsPerMinute / MillisecondsPerSecond);
            }
        }

        public int Minutes
        {
            get
            {
                return (int)(Milliseconds % MillisecondsPerMinute);
            }
        }

        public int MinuteComponentOnly
        {
            get
            {
                return (int)(_milliseconds % MillisecondsPerHour / MillisecondsPerMinute);
            }
        }

        public int Hours
        {
            get
            {
                return (int)(Milliseconds % MillisecondsPerHour);
            }
        }

        public int HourComponentOnly
        {
            get
            {
                return (int)((_milliseconds % MillisecondsPerDay) / MillisecondsPerHour);
            }
        }

        public int Days
        {
            get
            {
                return (int)(Milliseconds / MillisecondsPerDay);
            }
        }

        public int DayComponentOnly
        {
            get
            {
                return (int)(_milliseconds / MillisecondsPerDay);
            }
        }

        private int _weeks;
        public int Weeks
        {
            get
            {
                return _weeks;
            }
        }

        private int _months;
        public int Months
        {
            get
            {
                return _months;
            }
        }

        private int _years;
        public int Years
        {
            get
            {
                return _years;
            }
        }

        public long Ticks
        {
            get
            {
                return Milliseconds * 10000;
            }
        }

        public static readonly MudTimeSpan Zero = new(0);
        public static readonly MudTimeSpan MaxValue = new(long.MaxValue);
        public static readonly MudTimeSpan MinValue = new(long.MinValue);

        public TimeSpan AsTimeSpan()
        {
            return TimeSpan.FromMilliseconds(Milliseconds);
        }

        public MudTimeSpan Inverse()
        {
            return new MudTimeSpan(_years * -1, _months * -1, _weeks * -1, _milliseconds * -1);
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
                throw new ArgumentOutOfRangeException();
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
            _milliseconds = (days * 3600 * 24 + hours * 3600 + minutes * 60 + seconds) * 1000 + milliseconds;
            _months = months;
            _years = years;
            _weeks = weeks;
            if (Milliseconds > MaxMilliSeconds || Milliseconds < MinMilliSeconds)
                throw new ArgumentOutOfRangeException();
        }

        public MudTimeSpan(int years, int months, int weeks, double days)
        {
            _milliseconds = (long)(days * MillisecondsPerDay);
            _months = months;
            _years = years;
            _weeks = weeks;
            if (Milliseconds > MaxMilliSeconds || Milliseconds < MinMilliSeconds)
                throw new ArgumentOutOfRangeException();
        }

        public MudTimeSpan(int years, int months, int weeks, long milliseconds)
        {
            _milliseconds = milliseconds;
            _months = months;
            _years = years;
            _weeks = weeks;
            if (Milliseconds > MaxMilliSeconds || Milliseconds < MinMilliSeconds)
                throw new ArgumentOutOfRangeException();
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

        private static Regex UnitRegex = new(@"(?<quantity>\d+)\s*(?<unit>millisecond|millisecondssecond|seconds|sec|secs|minute|minutes|hour|hours|hr|hrs|day|days|week|weeks|month|months|mon|mons|year|years|min|mins|s|ms|m|h|d|w|mo|y)", RegexOptions.IgnoreCase);

        public static bool TryParse(string text, IFormatProvider format, out MudTimeSpan timespan)
        {
            if (string.IsNullOrEmpty(text))
            {
                timespan = Zero;
                return false;
            }

            var ss = new StringStack(text);
            if (ss.Peek().EqualToAny("zero", "none", "nothing"))
            {
                timespan = Zero;
                return false;
            }

            var years = 0;
            var months = 0;
            var weeks = 0;
            var days = 0L;
            var hours = 0L;
            var minutes = 0L;
            var seconds = 0L;
            var milliseconds = 0L;

            var subtext = string.Empty;
            while (!ss.IsFinished)
            {
                subtext += ss.Pop();
                if (!int.TryParse(subtext, out _) && TimeSpanParser.TryParse(subtext, new TimeSpanParserOptions { FormatProvider = format, AllowUnitlessZero = true, ColonedDefault = Units.Days}, out var ts))
                {
                    days += ts.Days;
                    hours += ts.Hours;
                    minutes += ts.Minutes;
                    seconds += ts.Seconds;
                    milliseconds += ts.Milliseconds;
                    subtext = string.Empty;
                    continue;
                }

                if (UnitRegex.IsMatch(subtext))
                {
                    var match = UnitRegex.Match(subtext);
                    switch (match.Groups["unit"].Value.ToLowerInvariant())
                    {
                        case "ms":
                        case "millisecond":
                        case "milliseconds":
                            milliseconds += long.Parse(match.Groups["quantity"].Value);
                            subtext = string.Empty;
                            continue;
                        case "s":
                        case "second":
                        case "seconds":
                        case "sec":
                        case "secs":
                            seconds += long.Parse(match.Groups["quantity"].Value);
                            subtext = string.Empty;
                            continue;
                        case "m":
                        case "min":
                        case "mins":
                        case "minute":
                        case "minutes":
                            minutes += long.Parse(match.Groups["quantity"].Value);
                            subtext = string.Empty;
                            continue;
                        case "h":
                        case "hr":
                        case "hrs":
                        case "hour":
                        case "hours":
                            hours += long.Parse(match.Groups["quantity"].Value);
                            subtext = string.Empty;
                            continue;
                        case "d":
                        case "day":
                        case "days":
                            days += long.Parse(match.Groups["quantity"].Value);
                            subtext = string.Empty;
                            continue;
                        case "w":
                        case "week":
                        case "weeks":
                            weeks += int.Parse(match.Groups["quantity"].Value);
                            subtext = string.Empty;
                            continue;
                        case "mo":
                        case "mon":
                        case "mons":
                        case "month":
                        case "months":
                            months += int.Parse(match.Groups["quantity"].Value);
                            subtext = string.Empty;
                            continue;
                        case "y":
                        case "year":
                        case "years":
                            years += int.Parse(match.Groups["quantity"].Value);
                            subtext = string.Empty;
                            continue;
                    }
                }
            }

            if (!string.IsNullOrEmpty(subtext))
            {
                timespan = Zero;
                return false;
            }

            timespan = new MudTimeSpan(years, months, weeks, days, hours, minutes, seconds, milliseconds);
            return true;
        }

        public static bool TryParse(string text, out MudTimeSpan timespan)
        {
            return TryParse(text, CultureInfo.InvariantCulture, out timespan);
        }

        public static MudTimeSpan Parse(string text)
        {
            if (!TryParse(text, out var ts))
            {
                throw new ApplicationException("Error parsing MudTimeSpan in Parse method");
            }

            return ts;
        }

        public string GetRoundTripParseText
        {
            get
            {
                var strings = new List<string>();
                if (_years > 0)
                {
                    strings.Add($"{_years:F0} years");
                }
                if (_months > 0)
                {
                    strings.Add($"{_months:F0} months");
                }
                if (_weeks > 0)
                {
                    strings.Add($"{_weeks:F0} weeks");
                }
                if (_milliseconds > 0)
                {
                    strings.Add($"{_milliseconds:F0}ms");
                }
                return strings.DefaultIfEmpty("zero").ListToString(separator: " ", conjunction: "");
            }
        }

        public double TotalDays
        {
            get { return Milliseconds * DaysPerMillisecond; }
        }

        public double TotalHours
        {
            get { return Milliseconds * HoursPerMillisecond; }
        }

        public double TotalMilliseconds
        {
            get
            {
                if (Milliseconds > MaxMilliSeconds)
                    return MaxMilliSeconds;

                if (Milliseconds < MinMilliSeconds)
                    return MinMilliSeconds;

                return Milliseconds;
            }
        }

        public double TotalMinutes
        {
            get { return Milliseconds * MinutesPerMillisecond; }
        }

        public double TotalSeconds
        {
            get { return Milliseconds * SecondsPerMillisecond; }
        }

        public MudTimeSpan Add(MudTimeSpan ts)
        {
            long result = Ticks + ts.Ticks;
            // Overflow if signs of operands was identical and result's
            // sign was opposite.
            // >> 63 gives the sign bit (either 64 1's or 64 0's).
            if ((Ticks >> 63 == ts.Ticks >> 63) && (Ticks >> 63 != result >> 63))
                throw new OverflowException();
            return new MudTimeSpan(result);
        }

        public MudTimeSpan Subtract(MudTimeSpan ts)
        {
            long result = Ticks - ts.Ticks;
            // Overflow if signs of operands was different and result's
            // sign was opposite from the first argument's sign.
            // >> 63 gives the sign bit (either 64 1's or 64 0's).
            if ((Ticks >> 63 != ts.Ticks >> 63) && (Ticks >> 63 != result >> 63))
                throw new OverflowException();
            return new MudTimeSpan(result);
        }

        // Compares two MudTimeSpan values, returning an integer that indicates their
        // relationship.
        //
        public static int Compare(MudTimeSpan t1, MudTimeSpan t2)
        {
            if (t1.Ticks > t2.Ticks) return 1;
            if (t1.Ticks < t2.Ticks) return -1;
            return 0;
        }

        // Compares two MudTimeSpan values, returning an integer that indicates their
        // relationship.
        //
        public static int Compare(MudTimeSpan t1, TimeSpan t2)
        {
            if (t1.Ticks > t2.Ticks) return 1;
            if (t1.Ticks < t2.Ticks) return -1;
            return 0;
        }

        // Returns a value less than zero if this  object
        public int CompareTo(object value)
        {
            if (value == null) return 1;
            if (value is not MudTimeSpan ts)
            {
                throw new ArgumentException(nameof(value));
            }

            var ms = ts.Milliseconds;
            if (Milliseconds > ms) { return 1; }
            if (Milliseconds < ms) { return -1; }
            return 0;
        }

        public static MudTimeSpan operator -(MudTimeSpan t)
        {
            if (t.Ticks == TimeSpan.MinValue.Ticks)
                throw new OverflowException();
            return new TimeSpan(-t.Ticks);
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
            return t1.Ticks == t2.Ticks;
        }

        public static bool operator !=(MudTimeSpan t1, MudTimeSpan t2)
        {
            return t1.Ticks != t2.Ticks;
        }

        public static bool operator <(MudTimeSpan t1, MudTimeSpan t2)
        {
            return t1.Ticks < t2.Ticks;
        }

        public static bool operator <=(MudTimeSpan t1, MudTimeSpan t2)
        {
            return t1.Ticks <= t2.Ticks;
        }

        public static bool operator >(MudTimeSpan t1, MudTimeSpan t2)
        {
            return t1.Ticks > t2.Ticks;
        }

        public static bool operator >=(MudTimeSpan t1, MudTimeSpan t2)
        {
            return t1.Ticks >= t2.Ticks;
        }

        public override int GetHashCode()
        {
            return Ticks.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is MudTimeSpan ts)
            {
                return ts.Ticks.Equals(Ticks);
            }

            return base.Equals(obj);
        }
    }
}

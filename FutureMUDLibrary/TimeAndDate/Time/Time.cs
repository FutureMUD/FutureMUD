using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using MudSharp.Framework;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp.TimeAndDate.Time
{
    public class MudTime : IComparable, IComparable<MudTime>, IEquatable<MudTime>
    {
        #region Properties

        protected int _seconds;

        public int Seconds
        {
            get => _seconds;
            protected set
            {
                _seconds = value;
                if (IsPrimaryTime)
                {
                    Clock.UpdateSeconds();
                    Clock.Changed = true;
                }
            }
        }

        protected int _minutes;

        public int Minutes
        {
            get => _minutes;
            protected set
            {
                _minutes = value;
                if (IsPrimaryTime)
                {
                    Clock.UpdateMinutes();
                    Clock.Changed = true;
                }
            }
        }

        protected int _hours;

        public int Hours
        {
            get => _hours;
            protected set
            {
                _hours = value;
                if (IsPrimaryTime)
                {
                    Clock.UpdateHours();
                    Clock.Changed = true;
                }
            }
        }

        private void ApplyTimeDelta(long seconds, bool notifySeconds, bool notifyMinutes, bool notifyHours)
        {
            var secondsPerHour = (long)Clock.SecondsPerMinute * Clock.MinutesPerHour;
            var secondsPerDay = secondsPerHour * Clock.HoursPerDay;
            var currentSeconds = ((long)Hours * Clock.MinutesPerHour + Minutes) * Clock.SecondsPerMinute + Seconds;
            var totalSeconds = checked(currentSeconds + seconds);
            var days = FloorDivide(totalSeconds, secondsPerDay);
            var timeOfDay = totalSeconds - days * secondsPerDay;
            var hours = (int)(timeOfDay / secondsPerHour);
            timeOfDay %= secondsPerHour;
            var minutes = (int)(timeOfDay / Clock.SecondsPerMinute);
            var newSeconds = (int)(timeOfDay % Clock.SecondsPerMinute);

            ApplyNormalisedTime(checked((int)days), hours, minutes, newSeconds, notifySeconds, notifyMinutes,
                notifyHours);
        }

        private void ApplyNormalisedTime(int days, int hours, int minutes, int seconds, bool notifySeconds,
            bool notifyMinutes, bool notifyHours)
        {
            var oldHours = _hours;
            var oldMinutes = _minutes;
            var oldSeconds = _seconds;
            _hours = hours;
            _minutes = minutes;
            _seconds = seconds;

            if (!IsPrimaryTime)
            {
                DaysOffsetFromDatum = checked(DaysOffsetFromDatum + days);
                return;
            }

            if (days != 0)
            {
                Clock.AdvanceDays(days);
            }

            var secondsChanged = oldSeconds != seconds;
            var minutesChanged = oldMinutes != minutes;
            var hoursChanged = oldHours != hours;
            if (notifySeconds && (secondsChanged || minutesChanged || hoursChanged))
            {
                Clock.UpdateSeconds();
            }

            if (notifyMinutes && (minutesChanged || hoursChanged))
            {
                Clock.UpdateMinutes();
            }

            if (notifyHours && hoursChanged)
            {
                Clock.UpdateHours();
            }

            if (days != 0 || secondsChanged || minutesChanged || hoursChanged)
            {
                Clock.Changed = true;
            }
        }

        private static long FloorDivide(long value, long divisor)
        {
            var quotient = value / divisor;
            var remainder = value % divisor;
            return remainder < 0 ? quotient - 1 : quotient;
        }

        protected IMudTimeZone _timezone;

        public IMudTimeZone Timezone
        {
            get => _timezone; protected set => _timezone = value;
        }

        protected IClock _clock;

        public IClock Clock
        {
            get => _clock; protected set => _clock = value;
        }

        /// <summary>
        ///     This value is only non-zero in a non-primary time
        /// </summary>
        protected int _daysOffsetFromDatum;

        public int DaysOffsetFromDatum
        {
            get => _daysOffsetFromDatum; set => _daysOffsetFromDatum = value;
        }

        /// <summary>
        ///     This value is true when the time is the "feeder" time for a Clock. In this case, it communicates AdvanceDays to the
        ///     clock.
        /// </summary>
        protected bool _isPrimaryTime;

        public bool IsPrimaryTime
        {
            get => _isPrimaryTime; protected set => _isPrimaryTime = value;
        }

        #endregion

        #region Variables

        #endregion

        #region Constructors

        private static readonly Regex TimeTokenRegex =
            new(@"^(?<hours>\d+):(?<minutes>\d+)(?::(?<seconds>\d+)){0,1}(?<meridian>[a-z]+){0,1}$",
                RegexOptions.IgnoreCase);

        private static void ValidateComponents(int seconds, int minutes, int hours, IMudTimeZone timezone, IClock clock)
        {
            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }

            if (timezone == null)
            {
                throw new ArgumentNullException(nameof(timezone));
            }

            if (timezone.Clock != null && !ReferenceEquals(timezone.Clock, clock) && !clock.Timezones.Contains(timezone))
            {
                throw new ArgumentException("The timezone does not belong to the specified clock.", nameof(timezone));
            }

            if (seconds < 0 || seconds >= clock.SecondsPerMinute)
            {
                throw new ArgumentOutOfRangeException(nameof(seconds));
            }

            if (minutes < 0 || minutes >= clock.MinutesPerHour)
            {
                throw new ArgumentOutOfRangeException(nameof(minutes));
            }

            if (hours < 0 || hours >= clock.HoursPerDay)
            {
                throw new ArgumentOutOfRangeException(nameof(hours));
            }
        }

        public static MudTime CreatePrimaryTime(int seconds, int minutes, int hours, IMudTimeZone timezone, IClock clock)
        {
            ValidateComponents(seconds, minutes, hours, timezone, clock);
            return new MudTime(seconds, minutes, hours, timezone, clock, true);
        }

        public static MudTime FromPrimaryTime(int seconds, int minutes, int hours, IMudTimeZone timezone, IClock clock)
        {
            ValidateComponents(seconds, minutes, hours, timezone, clock);
            return new MudTime(seconds, minutes, hours, timezone, clock, false);
        }

        public static MudTime FromLocalTime(int seconds, int minutes, int hours, IMudTimeZone timezone, IClock clock, int daysOffsetFromDatum = 0)
        {
            ValidateComponents(seconds, minutes, hours, timezone, clock);
            return new MudTime(seconds, minutes, hours, timezone, clock, daysOffsetFromDatum);
        }

        public static MudTime CopyOf(MudTime rhs, bool resetDaysOffsetFromDatum = false)
        {
            if (rhs == null)
            {
                throw new ArgumentNullException(nameof(rhs));
            }

            var copy = new MudTime(rhs);
            if (resetDaysOffsetFromDatum)
            {
                copy._daysOffsetFromDatum = 0;
            }

            return copy;
        }

        public static MudTime ParseLocalTime(string timestring, IClock clock)
        {
            if (!TryParseLocalTime(timestring, clock, out var time, out var error))
            {
                throw new ArgumentException(error, nameof(timestring));
            }

            return time;
        }

        public static bool TryParseLocalTime(string timestring, IClock clock, out MudTime time, out string error)
        {
            time = null;
            error = string.Empty;
            if (clock == null)
            {
                error = "No clock was supplied.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(timestring))
            {
                error = "No time string was supplied.";
                return false;
            }

            var tokens = timestring.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var timeIndex = tokens.FindIndex(x => x.Contains(':'));
            if (timeIndex < 0)
            {
                error = "The time string does not contain a time component.";
                return false;
            }

            var match = TimeTokenRegex.Match(tokens[timeIndex]);
            if (!match.Success)
            {
                error = "The time component was not valid.";
                return false;
            }

            var meridian = match.Groups["meridian"].Success ? match.Groups["meridian"].Value : string.Empty;
            var timezoneText = string.Empty;
            foreach (var token in tokens.Where((_, index) => index != timeIndex))
            {
                if (string.IsNullOrEmpty(meridian) &&
                    clock.HourIntervalNames.Any(x => x.Equals(token, StringComparison.InvariantCultureIgnoreCase)))
                {
                    meridian = token;
                    continue;
                }

                if (!string.IsNullOrEmpty(timezoneText))
                {
                    error = "The time string contained more than one timezone or unknown token.";
                    return false;
                }

                timezoneText = token;
            }

            var timezone = string.IsNullOrEmpty(timezoneText)
                ? clock.PrimaryTimezone
                : clock.Timezones.GetByIdOrName(timezoneText);
            if (timezone == null)
            {
                error = $"The timezone \"{timezoneText}\" is not valid.";
                return false;
            }

            var hours = int.Parse(match.Groups["hours"].Value);
            var minutes = int.Parse(match.Groups["minutes"].Value);
            var seconds = match.Groups["seconds"].Success ? int.Parse(match.Groups["seconds"].Value) : 0;

            if (!string.IsNullOrEmpty(meridian))
            {
                var hourInterval = clock.HourIntervalNames.FindIndex(
                    x => x.Equals(meridian, StringComparison.InvariantCultureIgnoreCase));
                if (hourInterval < 0)
                {
                    error = $"The hour period \"{meridian}\" is not valid.";
                    return false;
                }

                var intervalLength = clock.HoursPerDay / clock.NumberOfHourIntervals;
                if (clock.NoZeroHour && hours == intervalLength)
                {
                    hours = 0;
                }

                hours += hourInterval * intervalLength;
            }

            try
            {
                time = FromLocalTime(seconds, minutes, hours, timezone, clock);
                return true;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                error = $"The {ex.ParamName} component is out of range for this clock.";
                return false;
            }
            catch (ArgumentException ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private MudTime(string timestring, IClock clock)
        {
            string[] split1 = timestring.Split(' ');
            _timezone = clock.Timezones.First(x => x.Name.EqualTo(split1[0]));
            var split = split1[1].Split(':').Select(int.Parse).ToList();
            _seconds = split[2];
            _minutes = split[1];
            _hours = split[0];
            _clock = clock;
            _isPrimaryTime = false;
        }

        private MudTime(int seconds, int minutes, int hours, IMudTimeZone timezone, IClock clock, bool isprimarytime)
        {
            _seconds = seconds;
            _minutes = minutes;
            _hours = hours;
            _timezone = timezone;
            _clock = clock;
            _isPrimaryTime = isprimarytime;

            // If this isn't a primary time, we need to apply timezone criteria
            if (!IsPrimaryTime)
            {
                if (Timezone.OffsetMinutes != 0)
                {
                    AddMinutes(Timezone.OffsetMinutes);
                }

                if (Timezone.OffsetHours != 0)
                {
                    AddHours(Timezone.OffsetHours);
                }
            }
        }

        private MudTime(int seconds, int minutes, int hours, IMudTimeZone timezone, IClock clock, int daysOffset)
        {
            _seconds = seconds;
            _minutes = minutes;
            _hours = hours;
            _timezone = timezone;
            _clock = clock;
            _isPrimaryTime = false;
            _daysOffsetFromDatum = daysOffset;
        }

        /// <summary>
        ///     Copy Constructor
        /// </summary>
        /// <param name="rhs">Time to copy</param>
        private MudTime(MudTime rhs)
        {
            _seconds = rhs.Seconds;
            _minutes = rhs.Minutes;
            _hours = rhs.Hours;
            _timezone = rhs.Timezone;
            _clock = rhs.Clock;
            _daysOffsetFromDatum = rhs.DaysOffsetFromDatum;
            _isPrimaryTime = false;
        }

        #endregion

        #region Methods

        public bool Equals(MudTime compareTime)
        {
            if (compareTime is null || Clock?.Id != compareTime.Clock?.Id)
            {
                return false;
            }

            var left = Timezone == Clock.PrimaryTimezone ? this : GetTimeByTimezone(Clock.PrimaryTimezone);
            var right = compareTime.Timezone == compareTime.Clock.PrimaryTimezone
                ? compareTime
                : compareTime.GetTimeByTimezone(compareTime.Clock.PrimaryTimezone);
            return left.Seconds == right.Seconds &&
                   left.Minutes == right.Minutes &&
                   left.Hours == right.Hours &&
                   left.DaysOffsetFromDatum == right.DaysOffsetFromDatum;
        }

        public override bool Equals(object obj)
        {
            return obj is MudTime time && Equals(time);
        }

        public override int GetHashCode()
        {
            var primary = Timezone == Clock?.PrimaryTimezone ? this : GetTimeByTimezone(Clock.PrimaryTimezone);
            return HashCode.Combine(Clock?.Id ?? 0L, primary.Hours, primary.Minutes, primary.Seconds,
                primary.DaysOffsetFromDatum);
        }

        /// <summary>
        ///     Returns a number of seconds equal to the difference between two times. If they are from two different clocks, it
        ///     uses their
        ///     current times to do the comparison
        /// </summary>
        /// <param name="compareTime">Time to be compared.</param>
        /// <returns>Number of seconds difference between two times. Negative values means compareTime is later than time.</returns>
        public int SecondsDifference(MudTime compareTime)
        {
            if (Clock.Id == compareTime.Clock.Id)
            {
                return
                    Seconds - compareTime.Seconds +
                    (Minutes - compareTime.Minutes) * Clock.SecondsPerMinute +
                    (Hours - compareTime.Hours) * Clock.MinutesPerHour * Clock.SecondsPerMinute +
                    (DaysOffsetFromDatum - compareTime.DaysOffsetFromDatum) * Clock.HoursPerDay * Clock.MinutesPerHour *
                    Clock.SecondsPerMinute;
            }
            return
                SecondsDifference(Clock.CurrentTime) -
                (int)
                (compareTime.SecondsDifference(compareTime.Clock.CurrentTime) *
                 (Clock.InGameSecondsPerRealSecond / compareTime.Clock.InGameSecondsPerRealSecond))
                ;
        }

        public void SetTime(int hours, int minutes, int seconds)
        {
            if (hours < 0 || hours >= Clock.HoursPerDay)
            {
                throw new ArgumentOutOfRangeException(nameof(hours));
            }

            if (minutes < 0 || minutes >= Clock.MinutesPerHour)
            {
                throw new ArgumentOutOfRangeException(nameof(minutes));
            }

            if (seconds < 0 || seconds >= Clock.SecondsPerMinute)
            {
                throw new ArgumentOutOfRangeException(nameof(seconds));
            }

            ApplyNormalisedTime(0, hours, minutes, seconds, true, true, true);
        }

        public void AddSeconds(int seconds)
        {
            if (seconds == 0)
            {
                return;
            }

            ApplyTimeDelta(seconds, true, true, true);
        }

        public void AddMinutes(int minutes)
        {
            if (minutes == 0)
            {
                return;
            }

            ApplyTimeDelta((long)minutes * Clock.SecondsPerMinute, false, true, true);
        }

        public void AddHours(int hours)
        {
            if (hours == 0)
            {
                return;
            }

            ApplyTimeDelta((long)hours * Clock.MinutesPerHour * Clock.SecondsPerMinute, false, false, true);
        }

        protected void AdvanceDays(int days)
        {
            if (IsPrimaryTime)
            {
                Clock.AdvanceDays(days);
            }
            else
            {
                DaysOffsetFromDatum += days;
            }

            if (IsPrimaryTime)
            {
                Clock.Changed = true;
            }
        }

        public MudTime GetTimeByTimezone(IMudTimeZone timezone)
        {
            ArgumentNullException.ThrowIfNull(timezone);
            if (timezone.Clock is not null && !ReferenceEquals(timezone.Clock, Clock) && !Clock.Timezones.Contains(timezone))
            {
                throw new ArgumentException("The timezone does not belong to this clock.", nameof(timezone));
            }

            var minutesPerDay = (long)Clock.HoursPerDay * Clock.MinutesPerHour;
            var sourceMinutes = (long)DaysOffsetFromDatum * minutesPerDay +
                                (long)Hours * Clock.MinutesPerHour + Minutes;
            var targetMinutes = sourceMinutes +
                                (long)(timezone.OffsetHours - Timezone.OffsetHours) * Clock.MinutesPerHour +
                                timezone.OffsetMinutes - Timezone.OffsetMinutes;
            var daysOffset = FloorDivide(targetMinutes, minutesPerDay);
            var timeOfDayMinutes = targetMinutes - daysOffset * minutesPerDay;
            var newHours = (int)(timeOfDayMinutes / Clock.MinutesPerHour);
            var newMinutes = (int)(timeOfDayMinutes % Clock.MinutesPerHour);
            return FromLocalTime(Seconds, newMinutes, newHours, timezone, Clock, checked((int)daysOffset));
        }

        public static bool operator <(MudTime t1, MudTime t2)
        {
            if (t1.DaysOffsetFromDatum != t2.DaysOffsetFromDatum)
            {
                return t1.DaysOffsetFromDatum < t2.DaysOffsetFromDatum;
            }
            if (t1.Hours != t2.Hours)
            {
                return t1.Hours < t2.Hours;
            }
            if (t1.Minutes != t2.Minutes)
            {
                return t1.Minutes < t2.Minutes;
            }
            return (t1.Seconds != t2.Seconds) && (t1.Seconds < t2.Seconds);
        }

        public static bool operator >(MudTime t1, MudTime t2)
        {
            if (t1.DaysOffsetFromDatum != t2.DaysOffsetFromDatum)
            {
                return t1.DaysOffsetFromDatum > t2.DaysOffsetFromDatum;
            }
            if (t1.Hours != t2.Hours)
            {
                return t1.Hours > t2.Hours;
            }
            if (t1.Minutes != t2.Minutes)
            {
                return t1.Minutes > t2.Minutes;
            }
            return (t1.Seconds != t2.Seconds) && (t1.Seconds > t2.Seconds);
        }

        public static bool operator <=(MudTime t1, MudTime t2)
        {
            if (t1.DaysOffsetFromDatum != t2.DaysOffsetFromDatum)
            {
                return t1.DaysOffsetFromDatum < t2.DaysOffsetFromDatum;
            }
            if (t1.Hours != t2.Hours)
            {
                return t1.Hours < t2.Hours;
            }
            if (t1.Minutes != t2.Minutes)
            {
                return t1.Minutes < t2.Minutes;
            }
            return t1.Seconds <= t2.Seconds;
        }

        public static bool operator >=(MudTime t1, MudTime t2)
        {
            if (t1.DaysOffsetFromDatum != t2.DaysOffsetFromDatum)
            {
                return t1.DaysOffsetFromDatum > t2.DaysOffsetFromDatum;
            }
            if (t1.Hours != t2.Hours)
            {
                return t1.Hours > t2.Hours;
            }
            if (t1.Minutes != t2.Minutes)
            {
                return t1.Minutes > t2.Minutes;
            }
            return t1.Seconds >= t2.Seconds;
        }

        public static TimeSpan operator -(MudTime t1, MudTime t2)
        {
            return TimeSpan.FromSeconds(
                (t1.DaysOffsetFromDatum - t2.DaysOffsetFromDatum) * t1.Clock.HoursPerDay * t1.Clock.MinutesPerHour * t1.Clock.SecondsPerMinute +
                (t1.Hours - t2.Hours) * t1.Clock.MinutesPerHour * t1.Clock.SecondsPerMinute +
                (t1.Minutes - t2.Minutes) * t1.Clock.SecondsPerMinute +
                (t1.Seconds - t2.Seconds)
            );
        }

        public static MudTime operator +(MudTime time, TimeSpan ts)
        {
            time = CopyOf(time, true);
            time.AdvanceDays(ts.Days);
            time.AddSeconds(ts.Seconds);
            time.AddMinutes(ts.Minutes);
            time.AddHours(ts.Hours);
            return time;
        }

        public static MudTime operator -(MudTime time, TimeSpan ts)
        {
            time = CopyOf(time, true);
            time.AdvanceDays(-ts.Days);
            time.AddSeconds(-1 * ts.Seconds);
            time.AddMinutes(-1 * ts.Minutes);
            time.AddHours(-1 * ts.Hours);
            return time;
        }

        public double TimeFraction
            =>
            (double)Hours / Clock.HoursPerDay + (double)Minutes / (Clock.MinutesPerHour * Clock.HoursPerDay) +
            (double)Seconds / (Clock.SecondsPerMinute * Clock.MinutesPerHour * Clock.HoursPerDay);

        public string GetTimeString()
        {
            return $"{Timezone.Alias} {Hours}:{Minutes}:{Seconds}";
        }

        public override string ToString()
        {
            return GetTimeString();
        }

        public int CompareTo(MudTime other)
        {
            // First convert to same timezone
            MudTime nt = other.GetTimeByTimezone(Timezone);
            if (nt > this)
            {
                return -1;
            }
            if (nt < this)
            {
                return 1;
            }

            return 0;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (!(obj is MudTime mt))
            {
                return 1;
            }

            return CompareTo(mt);
        }

        public string Display(TimeDisplayTypes type)
        {
            return Clock.DisplayTime(this, type);
        }

        #endregion
    }
}

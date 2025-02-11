using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using MudSharp.Framework;

namespace MudSharp.TimeAndDate.Time {
	public class MudTime : IComparable, IComparable<MudTime> {
		#region Properties

		protected int _seconds;

		public int Seconds
		{
			get { return _seconds; }
			protected set
			{
				_seconds = value;
				if (IsPrimaryTime) {
					Clock.UpdateSeconds();
					Clock.Changed = true;
				}
			}
		}

		protected int _minutes;

		public int Minutes
		{
			get { return _minutes; }
			protected set
			{
				_minutes = value;
				if (IsPrimaryTime) {
					Clock.UpdateMinutes();
					Clock.Changed = true;
				}
			}
		}

		protected int _hours;

		public int Hours
		{
			get { return _hours; }
			protected set
			{
				_hours = value;
				if (IsPrimaryTime) {
					Clock.UpdateHours();
					Clock.Changed = true;
				}
			}
		}

		protected void UpdateTime(int days, int hours, int minutes, int seconds)
		{
			int oldHours = Hours, oldMinutes = Minutes, oldSeconds = Seconds;
			_seconds = seconds;
			if (_seconds == Clock.SecondsPerMinute)
			{
				_seconds = 0;
			}
			_minutes = minutes;
			if (_minutes == Clock.MinutesPerHour)
			{
				_minutes = 0;
			}
			_hours = hours;
			if (_hours == Clock.HoursPerDay)
			{
				_hours = 0;
			}
			if (days != 0)
			{
				if (IsPrimaryTime)
				{
					Clock.AdvanceDays(1);
				}
				else
				{
					DaysOffsetFromDatum += 1;
				}
			}
			if (oldHours != hours || oldMinutes != minutes || oldSeconds != seconds)
			{
				Clock.UpdateSeconds();
				if (oldHours != hours || oldMinutes != minutes)
				{
					Clock.UpdateMinutes();
					if (oldHours != hours)
					{
						Clock.UpdateHours();
					}
				}
			}

			Clock.Changed = true;
		}

		protected IMudTimeZone _timezone;

		public IMudTimeZone Timezone
		{
			get { return _timezone; }
			protected set { _timezone = value; }
		}

		protected IClock _clock;

		public IClock Clock
		{
			get { return _clock; }
			protected set { _clock = value; }
		}

		/// <summary>
		///     This value is only non-zero in a non-primary time
		/// </summary>
		protected int _daysOffsetFromDatum;

		public int DaysOffsetFromDatum
		{
			get { return _daysOffsetFromDatum; }
			set { _daysOffsetFromDatum = value; }
		}

		/// <summary>
		///     This value is true when the time is the "feeder" time for a Clock. In this case, it communicates AdvanceDays to the
		///     clock.
		/// </summary>
		protected bool _isPrimaryTime;

		public bool IsPrimaryTime
		{
			get { return _isPrimaryTime; }
			protected set { _isPrimaryTime = value; }
		}

		#endregion

		#region Variables

		#endregion

		#region Constructors

		public MudTime(string timestring, IClock clock) {
			var split1 = timestring.Split(' ');
			_timezone = clock.Timezones.First(x => x.Name.EqualTo(split1[0]));
			var split = split1[1].Split(':').Select(int.Parse).ToList();
			_seconds = split[2];
			_minutes = split[1];
			_hours = split[0];
			_clock = clock;
			_isPrimaryTime = false;
		}

		public MudTime(int seconds, int minutes, int hours, IMudTimeZone timezone, IClock clock, bool isprimarytime) {
			_seconds = seconds;
			_minutes = minutes;
			_hours = hours;
			_timezone = timezone;
			_clock = clock;
			_isPrimaryTime = isprimarytime;

			// If this isn't a primary time, we need to apply timezone criteria
			if (!IsPrimaryTime) {
				if (Timezone.OffsetMinutes != 0) {
					AddMinutes(Timezone.OffsetMinutes);
				}

				if (Timezone.OffsetHours != 0) {
					AddHours(Timezone.OffsetHours);
				}
			}
		}

		public MudTime(int seconds, int minutes, int hours, IMudTimeZone timezone, IClock clock, int daysOffset) {
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
		public MudTime(MudTime rhs) {
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

		public bool Equals(MudTime compareTime) {
			return
				(Seconds == compareTime.Seconds) &&
				(Minutes == compareTime.Minutes) &&
				(Hours == compareTime.Hours) &&
				(DaysOffsetFromDatum == compareTime.DaysOffsetFromDatum) &&
				(Clock.Id == compareTime.Clock.Id);
		}

		/// <summary>
		///     Returns a number of seconds equal to the difference between two times. If they are from two different clocks, it
		///     uses their
		///     current times to do the comparison
		/// </summary>
		/// <param name="compareTime">Time to be compared.</param>
		/// <returns>Number of seconds difference between two times. Negative values means compareTime is later than time.</returns>
		public int SecondsDifference(MudTime compareTime) {
			if (Clock.Id == compareTime.Clock.Id) {
				return
					Seconds - compareTime.Seconds +
					(Minutes - compareTime.Minutes)*Clock.SecondsPerMinute +
					(Hours - compareTime.Hours)*Clock.MinutesPerHour*Clock.SecondsPerMinute +
					(DaysOffsetFromDatum - compareTime.DaysOffsetFromDatum)*Clock.HoursPerDay*Clock.MinutesPerHour*
					Clock.SecondsPerMinute;
			}
			return
				SecondsDifference(Clock.CurrentTime) -
				(int)
				(compareTime.SecondsDifference(compareTime.Clock.CurrentTime)*
				 (Clock.InGameSecondsPerRealSecond/compareTime.Clock.InGameSecondsPerRealSecond))
				;
		}

		public void SetTime(int hours, int minutes, int seconds)
		{
			Hours = hours;
			Minutes = minutes;
			Seconds = seconds;
		}

		public void AddSeconds(int seconds)
		{
			if (seconds == 0)
			{
				return;
			}

			if (seconds > 0)
			{
				if (seconds < Clock.SecondsPerMinute - Seconds)
				{
					Seconds += seconds;
					return;
				}

				var oldSeconds = Seconds;
				var newSeconds = (seconds + Seconds) % Clock.SecondsPerMinute;
				var minutes = (seconds + oldSeconds) / Clock.SecondsPerMinute;
				if (minutes < Clock.MinutesPerHour - Minutes)
				{
					UpdateTime(0, Hours, Minutes + minutes, newSeconds);
					return;
				}

				var oldMinutes = Minutes;
				var newMinutes = (minutes + Minutes) % Clock.MinutesPerHour;
				var hours = (minutes + oldMinutes) / Clock.MinutesPerHour;
				if (hours < Clock.HoursPerDay - Hours)
				{
					UpdateTime(0, Hours + hours, newMinutes, newSeconds);
					return;
				}

				var oldHours = Hours;
				var newHours = (hours + Hours) % Clock.HoursPerDay;
				var days = (hours + oldHours) / Clock.HoursPerDay;
				UpdateTime(days, newHours, newMinutes, newSeconds);
				return;
			}
			else
			{
				if (Math.Abs(seconds) <= Seconds)
				{
					Seconds += seconds;
					return;
				}

				var newSeconds = Clock.SecondsPerMinute - Math.Abs((seconds + Seconds) % Clock.SecondsPerMinute);
				var minutes = ((seconds + Seconds)/Clock.SecondsPerMinute -
							   ((seconds + Seconds)%Clock.SecondsPerMinute == 0 ? 0 : 1));
				if (Math.Abs(minutes) <= Minutes)
				{
					UpdateTime(0, Hours, Minutes + minutes, newSeconds);
					return;
					
				}
				
				var hours = ((minutes + Minutes)/Clock.MinutesPerHour -
						 ((Minutes + minutes)%Clock.MinutesPerHour == 0 ? 0 : 1));
				var newMinutes = Clock.MinutesPerHour - Math.Abs((minutes + Minutes)%Clock.MinutesPerHour);
				if (Math.Abs(hours) <= Hours) {
					UpdateTime(0, Hours + hours, newMinutes, newSeconds);
					return;
				}
				
				var days = ((hours + Hours)/Clock.HoursPerDay - ((Hours + hours)%Clock.HoursPerDay == 0 ? 0 : 1));
				var newHours = Clock.HoursPerDay - Math.Abs((hours + Hours)%Clock.HoursPerDay);
				UpdateTime(days, newHours, newMinutes, newSeconds);
			}
		}
		
		public void AddMinutes(int minutes) {
			if (minutes == 0) {
				return;
			}

			if (minutes > 0) // Positive Numbers
			{
				if (minutes >= Clock.MinutesPerHour - Minutes) {
					var oldMinutes = Minutes;
					Minutes = (minutes + Minutes) % Clock.MinutesPerHour;
					AddHours((minutes + oldMinutes)/Clock.MinutesPerHour);
				}
				else {
					Minutes += minutes;
				}
			}
			else // Negative Numbers
			{
				if (Math.Abs(minutes) > Minutes) {
					AddHours((minutes + Minutes)/Clock.MinutesPerHour -
							 ((Minutes + minutes)%Clock.MinutesPerHour == 0 ? 0 : 1));
					Minutes = Clock.MinutesPerHour - Math.Abs((minutes + Minutes)%Clock.MinutesPerHour);
					if (Minutes == Clock.MinutesPerHour) {
						Minutes = 0;
					}
				}
				else {
					Minutes += minutes;
				}
			}

			if (IsPrimaryTime) {
				Clock.Changed = true;
			}
		}

		public void AddHours(int hours) {
			if (hours == 0) {
				return;
			}

			if (hours > 0) // Positive Numbers
			{
				if (hours >= Clock.HoursPerDay - Hours) {
					var oldHours = Hours;
					Hours = (hours + Hours) % Clock.HoursPerDay;
					AdvanceDays((hours + oldHours)/Clock.HoursPerDay);
				}
				else {
					Hours += hours;
				}
			}
			else // Negative Numbers
			{
				if (Math.Abs(hours) > Hours) {
					AdvanceDays((hours + Hours)/Clock.HoursPerDay - ((Hours + hours)%Clock.HoursPerDay == 0 ? 0 : 1));
					Hours = Clock.HoursPerDay - Math.Abs((hours + Hours)%Clock.HoursPerDay);
					if (Hours == Clock.HoursPerDay) {
						Hours = 0;
					}
				}
				else {
					Hours += hours;
				}
			}

			if (IsPrimaryTime) {
				Clock.Changed = true;
			}
		}

		protected void AdvanceDays(int days) {
			if (IsPrimaryTime) {
				Clock.AdvanceDays(days);
			}
			else {
				DaysOffsetFromDatum += days;
			}

			if (IsPrimaryTime) {
				Clock.Changed = true;
			}
		}

		public MudTime GetTimeByTimezone(IMudTimeZone timezone) {
			var newMinutes = Minutes + timezone.OffsetMinutes - Timezone.OffsetMinutes;
			// Integer division is towards zero not -infinity, so we need to add -1 if newMinutes is less than zero
			var newHours = Hours - Timezone.OffsetHours + timezone.OffsetHours + newMinutes/Clock.MinutesPerHour -
						   (newMinutes < 0 ? 1 : 0);
			// Integer division is towards zero not -infinity, so we need to add -1 if newHours is less than zero
			var daysOffset = newHours/Clock.HoursPerDay - (newHours < 0 ? 1 : 0);
			// C# % operator is simple remainder not actual modulus. Hence the extension method.
			newMinutes = newMinutes.Modulus(Clock.MinutesPerHour);
			newHours = newHours.Modulus(Clock.HoursPerDay);
			return new MudTime(Seconds, newMinutes, newHours, timezone, Clock,
				daysOffset);
		}

		public static bool operator <(MudTime t1, MudTime t2) {
			if (t1.DaysOffsetFromDatum != t2.DaysOffsetFromDatum)
			{
				return t1.DaysOffsetFromDatum < t2.DaysOffsetFromDatum;
			}
			if (t1.Hours != t2.Hours) {
				return t1.Hours < t2.Hours;
			}
			if (t1.Minutes != t2.Minutes) {
				return t1.Minutes < t2.Minutes;
			}
			return (t1.Seconds != t2.Seconds) && (t1.Seconds < t2.Seconds);
		}

		public static bool operator >(MudTime t1, MudTime t2) {
			if (t1.DaysOffsetFromDatum != t2.DaysOffsetFromDatum)
			{
				return t1.DaysOffsetFromDatum > t2.DaysOffsetFromDatum;
			}
			if (t1.Hours != t2.Hours) {
				return t1.Hours > t2.Hours;
			}
			if (t1.Minutes != t2.Minutes) {
				return t1.Minutes > t2.Minutes;
			}
			return (t1.Seconds != t2.Seconds) && (t1.Seconds > t2.Seconds);
		}

		public static bool operator <=(MudTime t1, MudTime t2) {
			if (t1.DaysOffsetFromDatum != t2.DaysOffsetFromDatum)
			{
				return t1.DaysOffsetFromDatum < t2.DaysOffsetFromDatum;
			}
			if (t1.Hours != t2.Hours) {
				return t1.Hours < t2.Hours;
			}
			if (t1.Minutes != t2.Minutes) {
				return t1.Minutes < t2.Minutes;
			}
			return t1.Seconds <= t2.Seconds;
		}

		public static bool operator >=(MudTime t1, MudTime t2) {
			if (t1.DaysOffsetFromDatum != t2.DaysOffsetFromDatum)
			{
				return t1.DaysOffsetFromDatum > t2.DaysOffsetFromDatum;
			}
			if (t1.Hours != t2.Hours) {
				return t1.Hours > t2.Hours;
			}
			if (t1.Minutes != t2.Minutes) {
				return t1.Minutes > t2.Minutes;
			}
			return t1.Seconds >= t2.Seconds;
		}

		public static TimeSpan operator -(MudTime t1, MudTime t2) {
			return TimeSpan.FromSeconds(
				(t1.Hours - t2.Hours)*t1.Clock.MinutesPerHour*t1.Clock.SecondsPerMinute +
				(t1.Minutes - t2.Minutes)*t1.Clock.SecondsPerMinute +
				(t1.Seconds - t2.Seconds)
			);
		}

		public static MudTime operator +(MudTime time, TimeSpan ts) {
			time = new MudTime(time) {_daysOffsetFromDatum = 0};
			time.AddSeconds(ts.Seconds);
			time.AddMinutes(ts.Minutes);
			time.AddHours(ts.Hours);
			return time;
		}

		public static MudTime operator -(MudTime time, TimeSpan ts) {
			time = new MudTime(time) {_daysOffsetFromDatum = 0};
			time.AddSeconds(-1*ts.Seconds);
			time.AddMinutes(-1*ts.Minutes);
			time.AddHours(-1*ts.Hours);
			return time;
		}

		public double TimeFraction
			=>
			(double) Hours/Clock.HoursPerDay + (double) Minutes/(Clock.MinutesPerHour*Clock.HoursPerDay) +
			(double) Seconds/(Clock.SecondsPerMinute*Clock.MinutesPerHour*Clock.HoursPerDay);

		public string GetTimeString() {
			return $"{Timezone.Alias} {Hours}:{Minutes}:{Seconds}";
		}

		public override string ToString() {
			return GetTimeString();
		}

		public int CompareTo(MudTime other)
		{
			// First convert to same timezone
			var nt = other.GetTimeByTimezone(Timezone);
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
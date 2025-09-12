using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;

namespace MudSharp.TimeAndDate.Time;

public class Clock : SaveableItem, IClock
{
	public override string FrameworkItemType => "Clock";

	public void LoadFromXml(XElement root)
	{
		if (!root.HasElements)
		{
			throw new XmlException("Root without any elements in Clock LoadFromXML.");
		}

		// Alias
		var element = root.Element("Alias");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing alias value in Clock LoadFromXML.");
		}

		// All check
		Alias = element.Value;

		// Description
		element = root.Element("Description");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing description value in Clock LoadFromXML.");
		}

		Description = element.Value;

		// ShortDisplayString
		element = root.Element("ShortDisplayString");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing shortdisplaystring value in Clock LoadFromXML.");
		}

		ShortDisplayString = element.Value;

		// LongDisplayString
		element = root.Element("LongDisplayString");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing longdisplaystring value in Clock LoadFromXML.");
		}

		LongDisplayString = element.Value;

		// SuperDisplayString
		element = root.Element("SuperDisplayString");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing description value in Clock LoadFromXML.");
		}

		SuperDisplayString = element.Value;

		// SecondsPerMinute
		element = root.Element("SecondsPerMinute");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing secondsperminute value in Clock LoadFromXML.");
		}

		try
		{
			SecondsPerMinute = int.Parse(element.Value);
		}
		catch
		{
			throw new XmlException("Value for secondsperminute in Clock LoadFromXML is not a valid Integer");
		}

		// SecondsPerMinute
		element = root.Element("MinutesPerHour");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing minutesperhour value in Clock LoadFromXML.");
		}

		try
		{
			MinutesPerHour = int.Parse(element.Value);
		}
		catch
		{
			throw new XmlException("Value for minutesperhour in Clock LoadFromXML is not a valid Integer");
		}

		// HoursPerDay
		element = root.Element("HoursPerDay");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing hoursperday value in Clock LoadFromXML.");
		}

		try
		{
			HoursPerDay = int.Parse(element.Value);
		}
		catch
		{
			throw new XmlException("Value for hoursperday in Clock LoadFromXML is not a valid Integer");
		}

		// SecondFixedDigits
		element = root.Element("SecondFixedDigits");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing secondfixeddigits value in Clock LoadFromXML.");
		}

		try
		{
			SecondFixedDigits = int.Parse(element.Value);
		}
		catch
		{
			throw new XmlException("Value for secondfixeddigits in Clock LoadFromXML is not a valid Integer");
		}

		// MinuteFixedDigits
		element = root.Element("MinuteFixedDigits");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing minutefixeddigits value in Clock LoadFromXML.");
		}

		try
		{
			MinuteFixedDigits = int.Parse(element.Value);
		}
		catch
		{
			throw new XmlException("Value for minutefixeddigits in Clock LoadFromXML is not a valid Integer");
		}

		// HoursFixedDigits
		element = root.Element("HourFixedDigits");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing hourfixeddigits value in Clock LoadFromXML.");
		}

		try
		{
			HourFixedDigits = int.Parse(element.Value);
		}
		catch
		{
			throw new XmlException("Value for hourfixeddigits in Clock LoadFromXML is not a valid Integer");
		}

		// InGameSecondsPerRealSecond
		element = root.Element("InGameSecondsPerRealSecond");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing ingamesecondsperrealsecond value in Clock LoadFromXML.");
		}

		try
		{
			InGameSecondsPerRealSecond = int.Parse(element.Value);
		}
		catch
		{
			throw new XmlException(
				"Value for ingamesecondsperrealsecond in Clock LoadFromXML is not a valid Integer");
		}

		// NumerOfHourIntervals
		element = root.Element("NumberOfHourIntervals");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing numberofhourintervals value in Clock LoadFromXML.");
		}

		try
		{
			NumberOfHourIntervals = int.Parse(element.Value);
		}
		catch
		{
			throw new XmlException("Value for numberofhourintervals in Clock LoadFromXML is not a valid Integer");
		}

		// NoZeroHour
		element = root.Element("NoZeroHour");
		if (element == null || element.Value.Length == 0)
		{
			throw new XmlException("Missing nozerohour value in Clock LoadFromXML.");
		}

		try
		{
			NoZeroHour = bool.Parse(element.Value);
		}
		catch
		{
			throw new XmlException("Value for nozerohour in Clock LoadFromXML is not a valid Boolean");
		}

		// HourIntervalNames
		element = root.Element("HourIntervalNames");
		if (element?.HasElements != true)
		{
			throw new XmlException("Clock has no HourIntervalNames defined in Clock LoadFromXml.");
		}

		foreach (var subElement in element.Elements("HourIntervalName"))
		{
			if (subElement.Value.Length == 0)
			{
				throw new XmlException("Malformed HourIntervalName in Clock LoadFromXML.");
			}

			HourIntervalNames.Add(subElement.Value);
		}

		// HourIntervalLongNames
		element = root.Element("HourIntervalLongNames");
		if (element?.HasElements != true)
		{
			throw new XmlException("Clock has no HourIntervalLongNames defined in Clock LoadFromXml.");
		}

		foreach (var subElement in element.Elements("HourIntervalLongName"))
		{
			if (subElement.Value.Length == 0)
			{
				throw new XmlException("Malformed HourIntervalLongName in Clock LoadFromXML.");
			}

			HourIntervalLongNames.Add(subElement.Value);
		}

		// Crude Hour Intervals
		element = root.Element("CrudeTimeIntervals");
		if (element?.HasElements != true)
		{
			throw new XmlException("Clock has no Crude Time Intervals defined in Clock LoadFromXml.");
		}

		foreach (var subElement in element.Elements("CrudeTimeInterval"))
		{
			CrudeTimeIntervals.Add(new BoundRange<string>(CrudeTimeIntervals, subElement.Attribute("text").Value,
				Convert.ToDouble(subElement.Attribute("Lower").Value),
				Convert.ToDouble(subElement.Attribute("Upper").Value)));
		}

		CrudeTimeIntervals.Sort();
	}

	public XElement SaveToXml()
	{
		return new XElement("Clock", new XElement("Alias", Alias), new XElement("Description", Description),
			new XElement("ShortDisplayString", ShortDisplayString),
			new XElement("SuperDisplayString", SuperDisplayString),
			new XElement("LongDisplayString", LongDisplayString), new XElement("SecondsPerMinute", SecondsPerMinute),
			new XElement("MinutesPerHour", MinutesPerHour), new XElement("HoursPerDay", HoursPerDay),
			new XElement("InGameSecondsPerRealSecond", InGameSecondsPerRealSecond),
			new XElement("SecondFixedDigits", SecondFixedDigits),
			new XElement("MinuteFixedDigits", MinuteFixedDigits), new XElement("HourFixedDigits", HourFixedDigits),
			new XElement("NoZeroHour", NoZeroHour), new XElement("NumberOfHourIntervals", NumberOfHourIntervals),
			new XElement("HourIntervalNames", new object[]
				{
					from hour in HourIntervalNames
					select new XElement("HourIntervalName", hour)
				}
			), new XElement("HourIntervalLongNames", new object[]
				{
					from hour in HourIntervalLongNames
					select new XElement("HourIntervalLongName", hour)
				}
			), new XElement("CrudeTimeIntervals", new object[]
				{
					from range in CrudeTimeIntervals.Ranges
					select
						new XElement("CrudeTimeInterval", new XAttribute("text", range.Value),
							new XAttribute("Lower", range.LowerLimit), new XAttribute("Upper", range.UpperLimit))
				}
			));
	}

	#region ISaveable Members

	public override void Save()
	{
	using (new FMDB())
	{
		var dbclock = FMDB.Context.Clocks.Find(Id);
		dbclock.Hours = CurrentTime.Hours;
		dbclock.Minutes = CurrentTime.Minutes;
		dbclock.Seconds = CurrentTime.Seconds;
		dbclock.PrimaryTimezoneId = PrimaryTimezone?.Id ?? dbclock.PrimaryTimezoneId;
		dbclock.Definition = SaveToXml().ToString();
	}

	Changed = false;
	}

	#endregion

	#region Events

	public event ClockEventHandler SecondsUpdated;
	public event ClockEventHandler MinutesUpdated;
	public event ClockEventHandler HoursUpdated;
	public event ClockAdvanceDaysEventHandler DaysUpdated;

	#endregion

	#region Event Handlers

	public void UpdateSeconds()
	{
		SecondsUpdated?.Invoke();
	}

	public void UpdateMinutes()
	{
		MinutesUpdated?.Invoke();
	}

	public void UpdateHours()
	{
		HoursUpdated?.Invoke();
	}

	public void AdvanceDays(int days)
	{
		DaysUpdated?.Invoke(days);
	}

	#endregion

	#region Properties

	#region Descriptive Properties

	/// <summary>
	///	This alias is a unique string reference to the Clock
	/// </summary>
	protected string _alias;

	public string Alias
	{
		get => _alias;
		protected set => _alias = value;
	}

	/// <summary>
	///	This is a brief summary description of the clock, e.g. "The UTC Clock for Earth circa 2012"
	/// </summary>
	protected string _description;

	public string Description
	{
		get => _description;
		protected set => _description = value;
	}

	IEnumerable<string> IHaveMultipleNames.Names => [Name, Alias];

	#endregion

	#region Behavioural Properties

	protected int _secondsPerMinute;

	public int SecondsPerMinute
	{
		get => _secondsPerMinute;
		protected set => _secondsPerMinute = value;
	}

	protected int _minutesPerHour;

	public int MinutesPerHour
	{
		get => _minutesPerHour;
		protected set => _minutesPerHour = value;
	}

	protected int _hoursPerDay;

	public int HoursPerDay
	{
		get => _hoursPerDay;
		protected set => _hoursPerDay = value;
	}

	protected double _inGameSecondsPerRealSecond;

	public double InGameSecondsPerRealSecond
	{
		get => _inGameSecondsPerRealSecond;
		protected set => _inGameSecondsPerRealSecond = value;
	}

	#endregion

	#region Display Properties

	/// <summary>
	///	Number of digits to fix the display of hours to, e.g. 2 makes hours appear as 06
	/// </summary>
	protected int _hourFixedDigits;

	public int HourFixedDigits
	{
		get => _hourFixedDigits;
		protected set => _hourFixedDigits = value;
	}

	/// <summary>
	///	Number of digits to fix the display of minutes to, e.g. 2 makes minutes appear 06
	/// </summary>
	protected int _minuteFixedDigits;

	public int MinuteFixedDigits
	{
		get => _minuteFixedDigits;
		protected set => _minuteFixedDigits = value;
	}

	/// <summary>
	///	Number of digits to fix the display of seconds to, e.g. 2 makes seconds appear 06
	/// </summary>
	protected int _secondFixedDigits;

	public int SecondFixedDigits
	{
		get => _secondFixedDigits;
		protected set => _secondFixedDigits = value;
	}


	/// <summary>
	///	The short version of the time display string.
	///	E.g.
	///	$hh:$m$i would display 11:34pm
	/// </summary>
	protected string _shortDisplayString;

	public string ShortDisplayString
	{
		get => _shortDisplayString;
		protected set => _shortDisplayString = value;
	}

	protected string _longDisplayString;

	public string LongDisplayString
	{
		get => _longDisplayString;
		protected set => _longDisplayString = value;
	}

	/// <summary>
	///	Display string for "super" users, e.g. admins and log files
	/// </summary>
	protected string _superDisplayString;

	public string SuperDisplayString
	{
		get => _superDisplayString;
		protected set => _superDisplayString = value;
	}

	/// <summary>
	///	This property describes how many discrete intervals of hours are in a one day period - for instance, in the common
	///	clock, there are 2 - a.m and p.m
	///	The total number of hours must be equally divisible by this number
	/// </summary>
	protected int _numberOfHourIntervals;

	public int NumberOfHourIntervals
	{
		get => _numberOfHourIntervals;
		protected set => _numberOfHourIntervals = value;
	}

	/// <summary>
	///	Display names for intervals of hours, e.g. "a.m.", "p.m."
	/// </summary>
	protected List<string> _hourIntervalNames = new();

	public List<string> HourIntervalNames => _hourIntervalNames;

	/// <summary>
	///	Long names for intervals of hours, e.g. "in the afternoon", "in the morning"
	/// </summary>
	protected List<string> _hourIntervalLongNames = new();

	public List<string> HourIntervalLongNames => _hourIntervalLongNames;

	/// <summary>
	///	Whether or not the clock displays hour 0 as 0 - for instance, in the standard clock hour zero is actually 12am
	/// </summary>
	protected bool _noZeroHour;

	public bool NoZeroHour
	{
		get => _noZeroHour;
		protected set => _noZeroHour = value;
	}

	#endregion

	protected MudTime _currentTime;

	public MudTime CurrentTime
	{
		get => _currentTime;
		protected init => _currentTime = value;
	}

	protected readonly All<IMudTimeZone> _timezones = new();
	public IUneditableAll<IMudTimeZone> Timezones => _timezones;
	public IMudTimeZone PrimaryTimezone { get; protected set; }

	public void AddTimezone(IMudTimeZone timezone)
	{
		_timezones.Add(timezone);
		if (PrimaryTimezone is null)
		{
			PrimaryTimezone = timezone;
		}
	}

	#endregion

	#region Constructors

	public Clock(IFuturemud gameworld, string name, string alias)
	{
		Gameworld = gameworld;
		_name = name;
		Alias = alias;
		Description = name;
		SecondsPerMinute = 60;
		MinutesPerHour = 60;
		HoursPerDay = 24;
		InGameSecondsPerRealSecond = 1.0;
		HourFixedDigits = 2;
		MinuteFixedDigits = 2;
		SecondFixedDigits = 2;
		ShortDisplayString = "$j:$m$i";
		LongDisplayString = "$c $l";
		SuperDisplayString = "$h:$m:$s $t";
		NumberOfHourIntervals = 2;
		HourIntervalNames.AddRange(new[] { "am", "pm" });
		HourIntervalLongNames.AddRange(new[] { "in the morning", "in the afternoon" });
		NoZeroHour = true;
		using (new FMDB())
		{
			var dbclock = new Models.Clock();
			FMDB.Context.Clocks.Add(dbclock);
			dbclock.Definition = string.Empty;
			dbclock.Hours = 0;
			dbclock.Minutes = 0;
			dbclock.Seconds = 0;
			dbclock.PrimaryTimezoneId = 0;
			FMDB.Context.SaveChanges();
			_id = dbclock.Id;
		}

		var timezone = new MudTimeZone(this, 0, 0, $"{Name} Standard Time", alias);
		AddTimezone(timezone);
		PrimaryTimezone = timezone;
		CurrentTime = new MudTime(0, 0, 0, timezone, this, true);
		Save();
	}

	private Clock(Clock rhs, string name, string alias)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		Alias = alias;
		Description = rhs.Description;
		SecondsPerMinute = rhs.SecondsPerMinute;
		MinutesPerHour = rhs.MinutesPerHour;
		HoursPerDay = rhs.HoursPerDay;
		InGameSecondsPerRealSecond = rhs.InGameSecondsPerRealSecond;
		HourFixedDigits = rhs.HourFixedDigits;
		MinuteFixedDigits = rhs.MinuteFixedDigits;
		SecondFixedDigits = rhs.SecondFixedDigits;
		ShortDisplayString = rhs.ShortDisplayString;
		LongDisplayString = rhs.LongDisplayString;
		SuperDisplayString = rhs.SuperDisplayString;
		NumberOfHourIntervals = rhs.NumberOfHourIntervals;
		HourIntervalNames.AddRange(rhs.HourIntervalNames);
		HourIntervalLongNames.AddRange(rhs.HourIntervalLongNames);
		NoZeroHour = rhs.NoZeroHour;
		foreach (var range in rhs.CrudeTimeIntervals.Ranges)
		{
			CrudeTimeIntervals.Add(new BoundRange<string>(CrudeTimeIntervals, range.Value, range.LowerLimit, range.UpperLimit));
		}

		using (new FMDB())
		{
			var dbclock = new Models.Clock();
			FMDB.Context.Clocks.Add(dbclock);
			dbclock.Definition = string.Empty;
			dbclock.Hours = rhs.CurrentTime.Hours;
			dbclock.Minutes = rhs.CurrentTime.Minutes;
			dbclock.Seconds = rhs.CurrentTime.Seconds;
			dbclock.PrimaryTimezoneId = 0;
			FMDB.Context.SaveChanges();
			_id = dbclock.Id;
		}

		foreach (var timezone in rhs.Timezones)
		{
			var tz = new MudTimeZone(this, timezone.OffsetHours, timezone.OffsetMinutes, timezone.Description, timezone.Alias);
			AddTimezone(tz);
			if (timezone == rhs.PrimaryTimezone)
			{
				PrimaryTimezone = tz;
			}
		}

		CurrentTime = new MudTime(rhs.CurrentTime.Seconds, rhs.CurrentTime.Minutes, rhs.CurrentTime.Hours, PrimaryTimezone, this, true);
		Save();
	}

	public Clock(XElement loadfile, MudTimeZone primaryTimeZone = null, int hours = 0, int minutes = 0, int seconds = 0)
	{
		LoadFromXml(loadfile);
		_name = Alias;
		PrimaryTimezone = primaryTimeZone;
		CurrentTime = new MudTime(seconds, minutes, hours, PrimaryTimezone, this, true);
		;
	}

	public Clock(XElement loadfile, IFuturemud gameworld, MudTimeZone primaryTimeZone, int hours = 0, int minutes = 0, int seconds = 0)
	{
		Gameworld = gameworld;
		LoadFromXml(loadfile);
		_name = Alias;
		PrimaryTimezone = primaryTimeZone;
		CurrentTime = new MudTime(seconds, minutes, hours, PrimaryTimezone, this, true);
		;
	}

	public Clock(XElement loadfile, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		LoadFromXml(loadfile);
		_name = Alias;
		PrimaryTimezone = _timezones.FirstOrDefault();
		CurrentTime = new MudTime(0, 0, 0, PrimaryTimezone, this, true);
	}

	public Clock(MudSharp.Models.Clock clock, IFuturemud game)
	{
		_id = clock.Id;
		Gameworld = game;
		LoadFromXml(XElement.Parse(clock.Definition));
		_name = Alias;
		foreach (var timezone in clock.Timezones)
		{
			_timezones.Add(new MudTimeZone(timezone, this, game));
		}

		PrimaryTimezone = _timezones.Get(clock.PrimaryTimezoneId);
		CurrentTime = new MudTime(clock.Seconds, clock.Minutes, clock.Hours, PrimaryTimezone, this, true);
	}

	public IClock Clone(string name, string alias)
	{
		return new Clock(this, name, alias);
	}

	#endregion

	#region Methods

	public static int GetVagueHour(int hour, int minute, int numberofminutes, int maxhours)
	{
		if (minute >= 5 * numberofminutes / 6)
		{
			hour += 1;
			while (hour >= maxhours)
			{
				hour -= maxhours;
			}
			return hour;
		}

		while (hour >= maxhours)
		{
			hour -= maxhours;
		}

		return hour;
	}

	public static string GetVagueTime(int hour, int minute, int numberofminutes, int maxhours)
	{
		while (hour > maxhours)
		{
			hour -= maxhours;
		}

		var nextHour = hour + 1;
		while (nextHour > maxhours)
		{
			nextHour -= maxhours;
		}

		if (hour == 0)
		{
			hour = maxhours;
		}

		if (nextHour == 0)
		{
			nextHour = maxhours;
		}

		if (minute <= numberofminutes / 6)
		{
			return hour.ToWordyNumber() + " o'clock";
		}

		if (minute < 6 * numberofminutes / 16)
		{
			return "a quarter past " + hour.ToWordyNumber();
		}

		if (minute <= 5 * numberofminutes / 8)
		{
			return "half past " + hour.ToWordyNumber();
		}

		if (minute <= numberofminutes - numberofminutes / 6)
		{
			return "a quarter to " + nextHour.ToWordyNumber();
		}

		return nextHour.ToWordyNumber() + " o'clock";
	}

	protected readonly CircularRange<string> CrudeTimeIntervals = new();

	protected string GetCrudeTime(double hour)
	{
		var interval = CrudeTimeIntervals.Get(hour);
		var fraction = CrudeTimeIntervals.RangeFraction(hour);
		return (fraction <= 0.33 ? "early " : fraction >= 0.67 ? "late " : "") + interval;
	}

	public static Regex timeRegex = new(@"\$([a-zA-Z])");

	/// <summary>
	///	Some examples of Display times and their expected outputs:
	///	"$h$m hours" - 1630 hours
	///	"$j:$m$i" - 4:30pm
	///	"$c $L" - half past four in the afternoon
	/// </summary>
	/// <param name="theTime"></param>
	/// <returns></returns>
	public string DisplayTime(MudTime theTime, string timeString)
	{
		return Regex.Replace(timeString, @"\$(?<variable>[\w])", m =>
		{
			switch (m.Groups["variable"].Value)
			{
				case "s":
					// Seconds as digits
					return SecondFixedDigits > 0
						? theTime.Seconds.ToString("D" + SecondFixedDigits)
						: theTime.Seconds.ToString();
				case "S":
					// Seconds as word
					return theTime.Seconds.ToWordyNumber();
				case "m":
					// Minutes as digits
					return MinuteFixedDigits > 0
						? theTime.Minutes.ToString("D" + MinuteFixedDigits)
						: theTime.Minutes.ToString();
				case "M":
					return theTime.Minutes.ToWordyNumber();
					// Minutes as word
				case "h":
					// 24-hour hours as digits
					return HourFixedDigits > 0 ? theTime.Hours.ToString("D" + HourFixedDigits) : theTime.Hours.ToString();
				case "H":
					// 24-hour hours as word
					return theTime.Hours.ToWordyNumber();
				case "j":
					// Period-specific hours as digits
					return HourFixedDigits > 0
						? (theTime.Hours % (HoursPerDay / NumberOfHourIntervals) == 0 && NoZeroHour
							? HoursPerDay / NumberOfHourIntervals
							: theTime.Hours % (HoursPerDay / NumberOfHourIntervals)).ToString()
						: (theTime.Hours % (HoursPerDay / NumberOfHourIntervals) == 0 && NoZeroHour
							? HoursPerDay / NumberOfHourIntervals
							: theTime.Hours % (HoursPerDay / NumberOfHourIntervals)).ToString("D" + HourFixedDigits);
				case "J":
					// Period-specific hours as word
					return (theTime.Hours % (HoursPerDay / NumberOfHourIntervals) == 0 && NoZeroHour
						? HoursPerDay / NumberOfHourIntervals
						: theTime.Hours % (HoursPerDay / NumberOfHourIntervals)).ToWordyNumber();
				case "t":
					// Timezone alias (e.g. UTC)
					return theTime.Timezone.Alias;
				case "T":
					// Timezone description (e.g. Universal Time Clock)
					return theTime.Timezone.Description;
				case "c":
					// Vague time, e.g. Half past twelve
					return GetVagueTime(
						NoZeroHour
							? theTime.Hours % (HoursPerDay / NumberOfHourIntervals) == 0
								? HoursPerDay / NumberOfHourIntervals
								: theTime.Hours % (HoursPerDay / NumberOfHourIntervals)
							: theTime.Hours, theTime.Minutes, MinutesPerHour, HoursPerDay / NumberOfHourIntervals);
				case "C":
					// Crude time, e.g. early afternoon
					return GetCrudeTime(theTime.Hours + (double)theTime.Minutes / MinutesPerHour);
				case "i":
					// Short interval name, e.g. a.m
					return HourIntervalNames[theTime.Hours / (HoursPerDay / NumberOfHourIntervals)];
				case "I":
					// Long interval name, e.g. in the morning
					return HourIntervalLongNames[theTime.Hours / (HoursPerDay / NumberOfHourIntervals)];
				case "l":
					// Vague-specific short interval name, e.g. a.m as if using the vague hour
					var vh = GetVagueHour(theTime.Hours, theTime.Minutes, MinutesPerHour, HoursPerDay);
					return HourIntervalNames[vh / (HoursPerDay / NumberOfHourIntervals)];
				case "L":
					// Vague-specific long interval name, e.g. in the morning as if using the vague hour
					return HourIntervalLongNames[GetVagueHour(theTime.Hours, theTime.Minutes, MinutesPerHour, HoursPerDay) / (HoursPerDay / NumberOfHourIntervals)];

			}
			return m.Value;
		});
	}

	public string DisplayTime(MudTime theTime, TimeDisplayTypes type)
	{
		switch (type)
		{
			case TimeDisplayTypes.Short:
				return DisplayTime(theTime, ShortDisplayString);
			case TimeDisplayTypes.Long:
				return DisplayTime(theTime, LongDisplayString);
			case TimeDisplayTypes.Immortal:
				return DisplayTime(theTime, SuperDisplayString);
			case TimeDisplayTypes.Crude:
				return DisplayTime(theTime, "$C");
			case TimeDisplayTypes.Vague:
				return DisplayTime(theTime, "$c $L");
			default:
				return DisplayTime(theTime, "$C");
		}
	}

	public string DisplayTime(TimeDisplayTypes type)
	{
		return DisplayTime(CurrentTime, type);
	}

	private static readonly Regex TimeStringRegex =
		new(
			@"^(?<timezone>[a-z]+){0,}\s*(?<hours>\d+){1}:(?<minutes>\d+){1}:(?<seconds>\d+){1}\s*(?<meridian>[a-z]+){0,}$",
			RegexOptions.IgnoreCase);

	public MudTime GetTime(string timeString)
	{
		var match = TimeStringRegex.Match(timeString);
		if (!match.Success)
		{
			throw new ArgumentException("The time string supplied did not match the time regex.");
		}

		return new MudTime(int.Parse(match.Groups["seconds"].Value), int.Parse(match.Groups["minutes"].Value),
			match.Groups["meridian"].Success
				? HourIntervalNames.FindIndex(
					x => x.Equals(match.Groups["meridian"].Value, StringComparison.InvariantCultureIgnoreCase)) *
				(HoursPerDay / NumberOfHourIntervals) + int.Parse(match.Groups["hours"].Value)
				: int.Parse(match.Groups["hours"].Value),
			match.Groups["timezone"].Success
				? Timezones.FirstOrDefault(
					x => x.Alias.Equals(match.Groups["timezone"].Value, StringComparison.InvariantCultureIgnoreCase))
				: PrimaryTimezone, this, 0);
	}

	public void SetTime(MudTime time)
	{
		_currentTime = time;
	}

	#endregion

	#region IEditableItem Implementation

	public const string BuildingHelpText = @"You can use the following options with this command:

	#3name <name>#0 - sets the name of the clock
	#3alias <alias>#0 - sets the alias of the clock
	#3desc <description>#0 - sets the description
	#3spm <##>#0 - sets the number of seconds per minute
	#3mph <##>#0 - sets the number of minutes per hour
	#3hpd <##>#0 - sets the number of hours per day
	#3speed <number>#0 - sets the in-game seconds per real second
	#3hourdigits <##>#0 - sets the fixed digits for hours
	#3minutedigits <##>#0 - sets the fixed digits for minutes
	#3seconddigits <##>#0 - sets the fixed digits for seconds
	#3short <string>#0 - sets the short display string
	#3long <string>#0 - sets the long display string
	#3super <string>#0 - sets the superuser display string
	#3intervals <##>#0 - sets the number of hour intervals
	#3intervalname <##> <name>#0 - sets a short name for an hour interval
	#3intervallong <##> <name>#0 - sets a long name for an hour interval
	#3nozero <true|false>#0 - sets whether hour zero is displayed";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
	switch (command.PopForSwitch())
	{
		case "name":
			return BuildingCommandName(actor, command);
		case "alias":
			return BuildingCommandAlias(actor, command);
		case "desc":
		case "description":
			return BuildingCommandDescription(actor, command);
		case "spm":
		case "secondsperminute":
			return BuildingCommandSecondsPerMinute(actor, command);
		case "mph":
		case "minutesperhour":
			return BuildingCommandMinutesPerHour(actor, command);
		case "hpd":
		case "hoursperday":
			return BuildingCommandHoursPerDay(actor, command);
		case "speed":
			return BuildingCommandSpeed(actor, command);
		case "hourdigits":
			return BuildingCommandHourDigits(actor, command);
		case "minutedigits":
			return BuildingCommandMinuteDigits(actor, command);
		case "seconddigits":
			return BuildingCommandSecondDigits(actor, command);
		case "short":
			return BuildingCommandShort(actor, command);
		case "long":
			return BuildingCommandLong(actor, command);
		case "super":
			return BuildingCommandSuper(actor, command);
		case "intervals":
			return BuildingCommandIntervals(actor, command);
		case "intervalname":
			return BuildingCommandIntervalName(actor, command);
		case "intervallong":
		case "intervallongname":
			return BuildingCommandIntervalLongName(actor, command);
		case "nozero":
		case "nozerohour":
			return BuildingCommandNoZeroHour(actor, command);
		default:
			actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
			return false;
	}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
	if (command.IsFinished)
	{
		actor.OutputHandler.Send("What name should this clock have?");
		return false;
	}

	var name = command.SafeRemainingArgument.TitleCase();
	_name = name;
	Changed = true;
	actor.OutputHandler.Send($"This clock is now named {name.ColourName()}.");
	return true;
       }

	private bool BuildingCommandAlias(ICharacter actor, StringStack command)
	{
	if (command.IsFinished)
	{
		actor.OutputHandler.Send("What alias should this clock have?");
		return false;
	}

	var alias = command.PopSpeech();
	if (Gameworld.Clocks.Any(x => x != this && x.Alias.EqualTo(alias)))
	{
		actor.OutputHandler.Send($"There is already a clock with the alias {alias.ColourValue()}.");
		return false;
	}

	Alias = alias;
	Changed = true;
	actor.OutputHandler.Send($"This clock now has the alias {alias.ColourValue()}.");
	return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
	if (command.IsFinished)
	{
		actor.OutputHandler.Send("What description should this clock have?");
		return false;
	}

	Description = command.SafeRemainingArgument;
	Changed = true;
	actor.OutputHandler.Send("Description set.");
	return true;
	}

	private bool BuildingCommandSecondsPerMinute(ICharacter actor, StringStack command)
	{
	if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value <= 0)
	{
		actor.OutputHandler.Send("You must specify a positive number of seconds per minute.");
		return false;
	}

	SecondsPerMinute = value;
	Changed = true;
	actor.OutputHandler.Send($"This clock now has {value.ToString("N0", actor).ColourValue()} seconds per minute.");
	return true;
	}

	private bool BuildingCommandMinutesPerHour(ICharacter actor, StringStack command)
	{
	if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value <= 0)
	{
		actor.OutputHandler.Send("You must specify a positive number of minutes per hour.");
		return false;
	}

	MinutesPerHour = value;
	Changed = true;
	actor.OutputHandler.Send($"This clock now has {value.ToString("N0", actor).ColourValue()} minutes per hour.");
	return true;
	}

	private bool BuildingCommandHoursPerDay(ICharacter actor, StringStack command)
	{
	if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value <= 0)
	{
		actor.OutputHandler.Send("You must specify a positive number of hours per day.");
		return false;
	}

	if (value % NumberOfHourIntervals != 0)
	{
		actor.OutputHandler.Send($"The value must be divisible by the number of hour intervals ({NumberOfHourIntervals}).");
		return false;
	}

	HoursPerDay = value;
	Changed = true;
	actor.OutputHandler.Send($"This clock now has {value.ToString("N0", actor).ColourValue()} hours per day.");
	return true;
	}

	private bool BuildingCommandSpeed(ICharacter actor, StringStack command)
	{
	if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var value) || value <= 0.0)
	{
		actor.OutputHandler.Send("You must specify a positive number of in-game seconds per real second.");
		return false;
	}

	InGameSecondsPerRealSecond = value;
	Changed = true;
	actor.OutputHandler.Send($"This clock now advances {value.ToString("N3", actor).ColourValue()} seconds each real second.");
	return true;
	}

	private bool BuildingCommandHourDigits(ICharacter actor, StringStack command)
	{
	if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value < 0)
	{
		actor.OutputHandler.Send("You must specify a non-negative number of digits.");
		return false;
	}

	HourFixedDigits = value;
	Changed = true;
	actor.OutputHandler.Send($"This clock will now use {value.ToString("N0", actor).ColourValue()} digits for hours.");
	return true;
	}

	private bool BuildingCommandMinuteDigits(ICharacter actor, StringStack command)
	{
	if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value < 0)
	{
		actor.OutputHandler.Send("You must specify a non-negative number of digits.");
		return false;
	}

	MinuteFixedDigits = value;
	Changed = true;
	actor.OutputHandler.Send($"This clock will now use {value.ToString("N0", actor).ColourValue()} digits for minutes.");
	return true;
	}

	private bool BuildingCommandSecondDigits(ICharacter actor, StringStack command)
	{
	if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value < 0)
	{
		actor.OutputHandler.Send("You must specify a non-negative number of digits.");
		return false;
	}

	SecondFixedDigits = value;
	Changed = true;
	actor.OutputHandler.Send($"This clock will now use {value.ToString("N0", actor).ColourValue()} digits for seconds.");
	return true;
	}

	private bool BuildingCommandShort(ICharacter actor, StringStack command)
	{
	if (command.IsFinished)
	{
		actor.OutputHandler.Send("What short display string should this clock use?");
		return false;
	}

	ShortDisplayString = command.SafeRemainingArgument;
	Changed = true;
	actor.OutputHandler.Send("Short display string set.");
	return true;
	}

	private bool BuildingCommandLong(ICharacter actor, StringStack command)
	{
	if (command.IsFinished)
	{
		actor.OutputHandler.Send("What long display string should this clock use?");
		return false;
	}

	LongDisplayString = command.SafeRemainingArgument;
	Changed = true;
	actor.OutputHandler.Send("Long display string set.");
	return true;
	}

	private bool BuildingCommandSuper(ICharacter actor, StringStack command)
	{
	if (command.IsFinished)
	{
		actor.OutputHandler.Send("What superuser display string should this clock use?");
		return false;
	}

	SuperDisplayString = command.SafeRemainingArgument;
	Changed = true;
	actor.OutputHandler.Send("Super display string set.");
	return true;
	}

	private bool BuildingCommandIntervals(ICharacter actor, StringStack command)
	{
	if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value <= 0)
	{
		actor.OutputHandler.Send("You must specify a positive number of intervals.");
		return false;
	}

	if (HoursPerDay % value != 0)
	{
		actor.OutputHandler.Send($"The value must divide evenly into the hours per day ({HoursPerDay}).");
		return false;
	}

	NumberOfHourIntervals = value;
	while (HourIntervalNames.Count < value)
	{
		HourIntervalNames.Add($"Interval {HourIntervalNames.Count + 1}");
	}

	while (HourIntervalNames.Count > value)
	{
		HourIntervalNames.RemoveAt(HourIntervalNames.Count - 1);
	}

	while (HourIntervalLongNames.Count < value)
	{
		HourIntervalLongNames.Add($"Interval {HourIntervalLongNames.Count + 1}");
	}

	while (HourIntervalLongNames.Count > value)
	{
		HourIntervalLongNames.RemoveAt(HourIntervalLongNames.Count - 1);
	}

	Changed = true;
	actor.OutputHandler.Send($"This clock will now have {value.ToString("N0", actor).ColourValue()} hour intervals.");
	return true;
	}

	private bool BuildingCommandIntervalName(ICharacter actor, StringStack command)
	{
	if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var index))
	{
		actor.OutputHandler.Send("Which interval do you want to name?");
		return false;
	}

	if (index < 1 || index > HourIntervalNames.Count)
	{
		actor.OutputHandler.Send("There is no such interval.");
		return false;
	}

	if (command.IsFinished)
	{
		actor.OutputHandler.Send("What name should this interval have?");
		return false;
	}

	var name = command.SafeRemainingArgument;
	HourIntervalNames[index - 1] = name;
	Changed = true;
	actor.OutputHandler.Send($"Interval {index} will now be called {name.ColourValue()}.");
	return true;
	}

	private bool BuildingCommandIntervalLongName(ICharacter actor, StringStack command)
	{
	if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var index))
	{
		actor.OutputHandler.Send("Which interval do you want to name?");
		return false;
	}

	if (index < 1 || index > HourIntervalLongNames.Count)
	{
		actor.OutputHandler.Send("There is no such interval.");
		return false;
	}

	if (command.IsFinished)
	{
		actor.OutputHandler.Send("What long name should this interval have?");
		return false;
	}

	var name = command.SafeRemainingArgument;
	HourIntervalLongNames[index - 1] = name;
	Changed = true;
	actor.OutputHandler.Send($"Interval {index} will now have long name {name.ColourValue()}.");
	return true;
	}

	private bool BuildingCommandNoZeroHour(ICharacter actor, StringStack command)
	{
	var text = command.PopSpeech();
	if (!text.EqualTo("true") && !text.EqualTo("false") && !text.EqualTo("yes") && !text.EqualTo("no"))
	{
		actor.OutputHandler.Send("Do you want this clock to skip zero hour? (true/false)");
		return false;
	}

	NoZeroHour = text.EqualTo("true") || text.EqualTo("yes");
	Changed = true;
	actor.OutputHandler.Send($"This clock will {(NoZeroHour ? "not " : "")}show hour zero.");
	return true;
       }

	public string Show(ICharacter actor)
	{
	var sb = new StringBuilder();
	sb.AppendLine($"Clock #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
	sb.AppendLine($"Alias: {Alias.ColourValue()}");
	sb.AppendLine($"Description: {Description.ColourValue()}");
	sb.AppendLine($"Seconds/Minute: {SecondsPerMinute.ToString("N0", actor).ColourValue()}");
	sb.AppendLine($"Minutes/Hour: {MinutesPerHour.ToString("N0", actor).ColourValue()}");
	sb.AppendLine($"Hours/Day: {HoursPerDay.ToString("N0", actor).ColourValue()}");
	sb.AppendLine($"IG Seconds/Real Second: {InGameSecondsPerRealSecond.ToString("N3", actor).ColourValue()}");
	sb.AppendLine($"Hour Digits: {HourFixedDigits.ToString("N0", actor).ColourValue()}");
	sb.AppendLine($"Minute Digits: {MinuteFixedDigits.ToString("N0", actor).ColourValue()}");
	sb.AppendLine($"Second Digits: {SecondFixedDigits.ToString("N0", actor).ColourValue()}");
	sb.AppendLine($"Short Display: {ShortDisplayString.ColourCommand()}");
	sb.AppendLine($"Long Display: {LongDisplayString.ColourCommand()}");
	sb.AppendLine($"Super Display: {SuperDisplayString.ColourCommand()}");
	sb.AppendLine($"Number of Intervals: {NumberOfHourIntervals.ToString("N0", actor).ColourValue()}");
	sb.AppendLine($"Interval Names: {HourIntervalNames.Select((x, i) => $"[{i + 1}] {x}").ListToCommaSeparatedValues(", ")}");
	sb.AppendLine($"Interval Long Names: {HourIntervalLongNames.Select((x, i) => $"[{i + 1}] {x}").ListToCommaSeparatedValues(", ")}");
	sb.AppendLine($"No Zero Hour: {NoZeroHour.ToColouredString()}");
	sb.AppendLine($"Primary Timezone: {PrimaryTimezone?.Alias.ColourValue() ?? "None"}");
	sb.AppendLine($"Timezones: {Timezones.Select(x => x.Alias.ColourValue()).ListToString()}");
	if (CrudeTimeIntervals.Ranges.Any())
	{
		sb.AppendLine("Crude Time Intervals:");
		var i = 1;
		foreach (var range in CrudeTimeIntervals.Ranges)
		{
			sb.AppendLine($"\t{i++.ToString("N0", actor)}. {range.LowerLimit.ToString("N2", actor)} - {range.UpperLimit.ToString("N2", actor)}: {range.Value.ColourValue()}");
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
			default:
				throw new NotSupportedException($"Unsupported property type {property} in Clock.GetProperty");
		}
	}

	public ProgVariableTypes Type => ProgVariableTypes.Clock;

	public object GetObject => this;

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "" },
			{ "name", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Clock, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

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
			CrudeTimeIntervals.Add(new BoundRange<string>(subElement.Attribute("text").Value,
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
	///     This alias is a unique string reference to the Clock
	/// </summary>
	protected string _alias;

	public string Alias
	{
		get => _alias;
		protected set => _alias = value;
	}

	/// <summary>
	///     This is a brief summary description of the clock, e.g. "The UTC Clock for Earth circa 2012"
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
	///     Number of digits to fix the display of hours to, e.g. 2 makes hours appear as 06
	/// </summary>
	protected int _hourFixedDigits;

	public int HourFixedDigits
	{
		get => _hourFixedDigits;
		protected set => _hourFixedDigits = value;
	}

	/// <summary>
	///     Number of digits to fix the display of minutes to, e.g. 2 makes minutes appear 06
	/// </summary>
	protected int _minuteFixedDigits;

	public int MinuteFixedDigits
	{
		get => _minuteFixedDigits;
		protected set => _minuteFixedDigits = value;
	}

	/// <summary>
	///     Number of digits to fix the display of seconds to, e.g. 2 makes seconds appear 06
	/// </summary>
	protected int _secondFixedDigits;

	public int SecondFixedDigits
	{
		get => _secondFixedDigits;
		protected set => _secondFixedDigits = value;
	}


	/// <summary>
	///     The short version of the time display string.
	///     E.g.
	///     $hh:$m$i would display 11:34pm
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
	///     Display string for "super" users, e.g. admins and log files
	/// </summary>
	protected string _superDisplayString;

	public string SuperDisplayString
	{
		get => _superDisplayString;
		protected set => _superDisplayString = value;
	}

	/// <summary>
	///     This property describes how many discrete intervals of hours are in a one day period - for instance, in the common
	///     clock, there are 2 - a.m and p.m
	///     The total number of hours must be equally divisible by this number
	/// </summary>
	protected int _numberOfHourIntervals;

	public int NumberOfHourIntervals
	{
		get => _numberOfHourIntervals;
		protected set => _numberOfHourIntervals = value;
	}

	/// <summary>
	///     Display names for intervals of hours, e.g. "a.m.", "p.m."
	/// </summary>
	protected List<string> _hourIntervalNames = new();

	public List<string> HourIntervalNames => _hourIntervalNames;

	/// <summary>
	///     Long names for intervals of hours, e.g. "in the afternoon", "in the morning"
	/// </summary>
	protected List<string> _hourIntervalLongNames = new();

	public List<string> HourIntervalLongNames => _hourIntervalLongNames;

	/// <summary>
	///     Whether or not the clock displays hour 0 as 0 - for instance, in the standard clock hour zero is actually 12am
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

	public Clock(XElement loadfile, MudTimeZone primaryTimeZone = null, int hours = 0, int minutes = 0, int seconds = 0)
	{
		LoadFromXml(loadfile);
		_name = Alias;
		PrimaryTimezone = primaryTimeZone;
		CurrentTime = new MudTime(seconds, minutes, hours, PrimaryTimezone, this, true);
		;
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

	#endregion

	#region Methods

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
	///     Some examples of Display times and their expected outputs:
	///     "$h$m hours" - 1630 hours
	///     "$j:$m$i" - 4:30pm
	///     "$c $l" - half past four in the afternoon
	/// </summary>
	/// <param name="theTime"></param>
	/// <param name="brief"></param>
	/// <returns></returns>
	public string DisplayTime(MudTime theTime, string timeString)
	{
		return timeString
		       .Replace("$s",
			       SecondFixedDigits > 0
				       ? theTime.Seconds.ToString("D" + SecondFixedDigits)
				       : theTime.Seconds.ToString()) // seconds
		       .Replace("$S", theTime.Seconds.ToWordyNumber())
		       .Replace("$m",
			       MinuteFixedDigits > 0
				       ? theTime.Minutes.ToString("D" + MinuteFixedDigits)
				       : theTime.Minutes.ToString()) // minutes
		       .Replace("$M", theTime.Minutes.ToWordyNumber())
		       .Replace("$j",
			       HourFixedDigits > 0
				       ? (theTime.Hours % (HoursPerDay / NumberOfHourIntervals) == 0 && NoZeroHour
					       ? HoursPerDay / NumberOfHourIntervals
					       : theTime.Hours % (HoursPerDay / NumberOfHourIntervals)).ToString()
				       : (theTime.Hours % (HoursPerDay / NumberOfHourIntervals) == 0 && NoZeroHour
					       ? HoursPerDay / NumberOfHourIntervals
					       : theTime.Hours % (HoursPerDay / NumberOfHourIntervals)).ToString("D" + HourFixedDigits))
		       // time interval specific hours
		       .Replace("$J",
			       (theTime.Hours % (HoursPerDay / NumberOfHourIntervals) == 0 && NoZeroHour
				       ? HoursPerDay / NumberOfHourIntervals
				       : theTime.Hours % (HoursPerDay / NumberOfHourIntervals)).ToWordyNumber())
		       // time interval specific hours
		       .Replace("$h",
			       HourFixedDigits > 0 ? theTime.Hours.ToString("D" + HourFixedDigits) : theTime.Hours.ToString())
		       // gross hours
		       .Replace("$H", theTime.Hours.ToWordyNumber()) // gross hours
		       .Replace("$T", theTime.Timezone.Description) // timezone description
		       .Replace("$t", theTime.Timezone.Alias) // timezone alias
		       .Replace("$c",
			       GetVagueTime(
				       NoZeroHour
					       ? theTime.Hours % (HoursPerDay / NumberOfHourIntervals) == 0
						       ? HoursPerDay / NumberOfHourIntervals
						       : theTime.Hours % (HoursPerDay / NumberOfHourIntervals)
					       : theTime.Hours, theTime.Minutes, MinutesPerHour, HoursPerDay / NumberOfHourIntervals))
		       // crude time - e.g. four o'clock
		       .Replace("$I", HourIntervalLongNames[theTime.Hours / (HoursPerDay / NumberOfHourIntervals)])
		       // time interval long name e.g. in the afternoon, in the morning
		       .Replace("$i", HourIntervalNames[theTime.Hours / (HoursPerDay / NumberOfHourIntervals)])
			// time inveral name e.g. a.m / p.m
			;
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
				return GetCrudeTime(theTime.Hours + (double)theTime.Minutes / MinutesPerHour);
			case TimeDisplayTypes.Vague:
				return
					GetVagueTime(theTime.Hours, theTime.Minutes, MinutesPerHour, HoursPerDay / NumberOfHourIntervals) +
					" " + HourIntervalLongNames[theTime.Hours / (HoursPerDay / NumberOfHourIntervals)];
			default:
				return GetCrudeTime(theTime.Hours + (double)theTime.Minutes / MinutesPerHour);
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
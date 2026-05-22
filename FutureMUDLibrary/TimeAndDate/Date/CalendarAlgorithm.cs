#nullable enable

using MudSharp.Celestial;
using MudSharp.TimeAndDate.Time;
using System.Xml.Linq;

namespace MudSharp.TimeAndDate.Date;

public enum CalendarAlgorithmType
{
	FixedMonths,
	TabularLunar,
	CalculatedHebrew,
	SolarEquinox,
	AstronomicalLunar,
	EastAsianLunisolar
}

public enum CalendarDayBoundaryType
{
	ClockMidnight,
	FixedClockTime,
	SunsetAtAuthorityLocation,
	SunriseAtAuthorityLocation,
	AstronomicalEvent
}

public interface ICalendarAlgorithm
{
	CalendarAlgorithmType Type { get; }

	string DisplayName { get; }

	string Summary { get; }

	Year CreateYear(ICalendar calendar, int whichYear);

	int CountWeekdaysInYear(ICalendar calendar, int whichYear);

	int CountDaysInYear(ICalendar calendar, int whichYear);

	XElement SaveToXml();
}

public interface IFixedMonthCalendarAlgorithm : ICalendarAlgorithm
{
}

public interface ICalculatedCalendarAlgorithm : ICalendarAlgorithm
{
}

public interface IAstronomicalCalendarAlgorithm : ICalculatedCalendarAlgorithm
{
	GeographicCoordinate? AuthorityLocation { get; }
}

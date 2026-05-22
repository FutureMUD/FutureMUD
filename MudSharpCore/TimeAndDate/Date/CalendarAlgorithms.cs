#nullable enable

using MudSharp.Celestial;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.TimeAndDate.Date;

public static class CalendarAlgorithmFactory
{
	public static ICalendarAlgorithm LoadFromXml(XElement? element, GeographicCoordinate? authorityLocation = null)
	{
		var typeText = element?.Attribute("type")?.Value ?? element?.Element("type")?.Value ?? "fixed-months";
		return typeText.ToLowerInvariant() switch
		{
			"fixed" or "fixed-month" or "fixed-months" => new FixedMonthCalendarAlgorithm(),
			"tabular-lunar" => new TabularLunarCalendarAlgorithm(element),
			"hebrew" or "calculated-hebrew" => new CalculatedHebrewCalendarAlgorithm(element),
			"solar-equinox" => new SolarEquinoxCalendarAlgorithm(element),
			"astronomical-lunar" => new AstronomicalLunarCalendarAlgorithm(element, authorityLocation),
			"east-asian-lunisolar" or "eastasian-lunisolar" => new EastAsianLunisolarCalendarAlgorithm(element, authorityLocation),
			_ => new FixedMonthCalendarAlgorithm()
		};
	}

	public static ICalendarAlgorithm Create(CalendarAlgorithmType type, GeographicCoordinate? authorityLocation = null)
	{
		return type switch
		{
			CalendarAlgorithmType.TabularLunar => new TabularLunarCalendarAlgorithm(null),
			CalendarAlgorithmType.CalculatedHebrew => new CalculatedHebrewCalendarAlgorithm(null),
			CalendarAlgorithmType.SolarEquinox => new SolarEquinoxCalendarAlgorithm(null),
			CalendarAlgorithmType.AstronomicalLunar => new AstronomicalLunarCalendarAlgorithm(null, authorityLocation),
			CalendarAlgorithmType.EastAsianLunisolar => new EastAsianLunisolarCalendarAlgorithm(null, authorityLocation),
			_ => new FixedMonthCalendarAlgorithm()
		};
	}

	public static bool TryParseType(string text, out CalendarAlgorithmType type)
	{
		type = text.ToLowerInvariant() switch
		{
			"fixed" or "fixed-month" or "fixed-months" => CalendarAlgorithmType.FixedMonths,
			"tabular-lunar" or "lunar-tabular" => CalendarAlgorithmType.TabularLunar,
			"hebrew" or "calculated-hebrew" => CalendarAlgorithmType.CalculatedHebrew,
			"solar-equinox" or "persian" => CalendarAlgorithmType.SolarEquinox,
			"astronomical-lunar" or "lunar-astronomical" => CalendarAlgorithmType.AstronomicalLunar,
			"east-asian-lunisolar" or "eastasian-lunisolar" or "lunisolar" => CalendarAlgorithmType.EastAsianLunisolar,
			_ => CalendarAlgorithmType.FixedMonths
		};
		return text.ToLowerInvariant().In("fixed", "fixed-month", "fixed-months", "tabular-lunar", "lunar-tabular",
			"hebrew", "calculated-hebrew", "solar-equinox", "persian", "astronomical-lunar",
			"lunar-astronomical", "east-asian-lunisolar", "eastasian-lunisolar", "lunisolar");
	}

	public static string XmlTypeFor(CalendarAlgorithmType type)
	{
		return type switch
		{
			CalendarAlgorithmType.TabularLunar => "tabular-lunar",
			CalendarAlgorithmType.CalculatedHebrew => "calculated-hebrew",
			CalendarAlgorithmType.SolarEquinox => "solar-equinox",
			CalendarAlgorithmType.AstronomicalLunar => "astronomical-lunar",
			CalendarAlgorithmType.EastAsianLunisolar => "east-asian-lunisolar",
			_ => "fixed-months"
		};
	}
}

public sealed class FixedMonthCalendarAlgorithm : IFixedMonthCalendarAlgorithm
{
	public CalendarAlgorithmType Type => CalendarAlgorithmType.FixedMonths;

	public string DisplayName => "Fixed Months";

	public string Summary => "Legacy fixed month and intercalary rules";

	public Year CreateYear(ICalendar calendar, int whichYear)
	{
		return new Year(CreateMonths(calendar, whichYear), whichYear, calendar);
	}

	public int CountWeekdaysInYear(ICalendar calendar, int whichYear)
	{
		return CreateMonths(calendar, whichYear).Sum(x => x.CountWeekdays());
	}

	public int CountDaysInYear(ICalendar calendar, int whichYear)
	{
		return CreateMonths(calendar, whichYear).Sum(x => x.Days);
	}

	private static List<Month> CreateMonths(ICalendar calendar, int whichYear)
	{
		var months = new List<Month>();
		months.AddRange(calendar.Months.Select(x => new Month(x, whichYear)));
		months.AddRange(calendar.Intercalaries
		                   .Where(x => x.Rule.IsIntercalaryYear(whichYear))
		                   .Select(x => new Month(x.Month, whichYear)));
		return months.OrderBy(x => x.NominalOrder).ToList();
	}

	public XElement SaveToXml()
	{
		return new XElement("algorithm", new XAttribute("type", CalendarAlgorithmFactory.XmlTypeFor(Type)));
	}
}

public sealed class TabularLunarCalendarAlgorithm : ICalculatedCalendarAlgorithm
{
	private static readonly int[] LeapYears = [2, 5, 7, 10, 13, 16, 18, 21, 24, 26, 29];

	public TabularLunarCalendarAlgorithm(XElement? root)
	{
		LeapMonthAlias = root?.Attribute("leapMonth")?.Value ?? root?.Element("leapMonth")?.Value;
		Variant = root?.Attribute("variant")?.Value ?? root?.Element("variant")?.Value ?? "tabular-lunar";
	}

	public CalendarAlgorithmType Type => CalendarAlgorithmType.TabularLunar;

	public string DisplayName => "Tabular Lunar";

	public string Summary => $"Alternating 30/29 day lunar months ({Variant})";

	public string? LeapMonthAlias { get; }

	public string Variant { get; }

	public Year CreateYear(ICalendar calendar, int whichYear)
	{
		return new Year(CreateMonths(calendar, whichYear), whichYear, calendar);
	}

	public int CountWeekdaysInYear(ICalendar calendar, int whichYear)
	{
		return CreateMonths(calendar, whichYear).Sum(x => x.CountWeekdays());
	}

	public int CountDaysInYear(ICalendar calendar, int whichYear)
	{
		return CreateMonths(calendar, whichYear).Sum(x => x.Days);
	}

	private List<Month> CreateMonths(ICalendar calendar, int whichYear)
	{
		var months = calendar.Months
		                     .Select((month, index) => BuildMonth(month, whichYear, index % 2 == 0 ? 30 : 29))
		                     .ToList();
		if (IsLeapYear(whichYear) && months.Count > 0)
		{
			var index = string.IsNullOrWhiteSpace(LeapMonthAlias)
				? months.Count - 1
				: Math.Max(0, months.FindIndex(x => x.Alias.EqualTo(LeapMonthAlias)));
			var source = calendar.Months[Math.Min(index < 0 ? months.Count - 1 : index, calendar.Months.Count - 1)];
			months[index < 0 ? months.Count - 1 : index] = BuildMonth(source, whichYear, months[index < 0 ? months.Count - 1 : index].Days + 1);
		}

		return months;
	}

	public XElement SaveToXml()
	{
		var element = new XElement("algorithm", new XAttribute("type", CalendarAlgorithmFactory.XmlTypeFor(Type)),
			new XAttribute("variant", Variant));
		if (!string.IsNullOrWhiteSpace(LeapMonthAlias))
		{
			element.Add(new XAttribute("leapMonth", LeapMonthAlias));
		}

		return element;
	}

	public static bool IsLeapYear(int year)
	{
		return LeapYears.Contains(CycleYear(year, 30));
	}

	private static int CycleYear(int year, int cycle)
	{
		return ((year - 1).Modulus(cycle)) + 1;
	}

	public static Month BuildMonth(MonthDefinition source, int year, int days, string? alias = null, string? shortName = null,
		string? fullName = null, int? nominalOrder = null)
	{
		var definition = new MonthDefinition(
			shortName ?? source.ShortName,
			alias ?? source.Alias,
			fullName ?? source.FullName,
			nominalOrder ?? source.NominalOrder,
			days,
			source.SpecialDayNames
			      .Where(x => x.Key <= days)
			      .ToDictionary(x => x.Key, x => x.Value),
			[]);
		definition.NonWeekdays.AddRange(source.NonWeekdays.Where(x => x <= days));
		return new Month(definition, year);
	}
}

public sealed class CalculatedHebrewCalendarAlgorithm : ICalculatedCalendarAlgorithm
{
	public CalculatedHebrewCalendarAlgorithm(XElement? root)
	{
	}

	public CalendarAlgorithmType Type => CalendarAlgorithmType.CalculatedHebrew;

	public string DisplayName => "Calculated Hebrew";

	public string Summary => "19-year Metonic cycle with calculated Heshvan/Kislev and Adar I/II handling";

	public Year CreateYear(ICalendar calendar, int whichYear)
	{
		return new Year(CreateMonths(calendar, whichYear), whichYear, calendar);
	}

	public int CountWeekdaysInYear(ICalendar calendar, int whichYear)
	{
		return CreateMonths(calendar, whichYear).Sum(x => x.CountWeekdays());
	}

	public int CountDaysInYear(ICalendar calendar, int whichYear)
	{
		return CreateMonths(calendar, whichYear).Sum(x => x.Days);
	}

	private static List<Month> CreateMonths(ICalendar calendar, int whichYear)
	{
		var leap = IsLeapYear(whichYear);
		var yearLength = HebrewYearLength(whichYear);
		var heshvanLong = yearLength.Modulus(10) == 5;
		var kislevShort = yearLength.Modulus(10) == 3;
		var months = new List<Month>();

		Add("nisan", 30);
		Add("iyyar", 29);
		Add("sivan", 30);
		Add("tammuz", 29);
		Add("av", 30);
		Add("elul", 29);
		Add("tishrei", 30);
		Add("heshvan", heshvanLong ? 30 : 29);
		Add("kislev", kislevShort ? 29 : 30);
		Add("tevet", 29);
		Add("shevat", 30);
		if (leap)
		{
			Add("adar-i", 30);
			Add("adar-ii", 29);
		}
		else
		{
			Add("adar", 29);
		}

		return months;

		void Add(string alias, int days)
		{
			var source = calendar.Months.FirstOrDefault(x => x.Alias.EqualTo(alias)) ??
			             calendar.Months.FirstOrDefault(x => x.Alias.Replace("_", "-").EqualTo(alias)) ??
			             calendar.Months[Math.Min(months.Count, calendar.Months.Count - 1)];
			months.Add(TabularLunarCalendarAlgorithm.BuildMonth(source, whichYear, days, nominalOrder: months.Count + 1));
		}
	}

	public XElement SaveToXml()
	{
		return new XElement("algorithm", new XAttribute("type", CalendarAlgorithmFactory.XmlTypeFor(Type)));
	}

	public static bool IsLeapYear(int year)
	{
		return ((7 * year + 1).Modulus(19)) < 7;
	}

	private static int HebrewYearLength(int year)
	{
		return HebrewCalendarElapsedDays(year + 1) - HebrewCalendarElapsedDays(year);
	}

	private static int HebrewCalendarElapsedDays(int year)
	{
		var previousYear = year - 1;
		var cycle = previousYear / 19;
		var yearInCycle = previousYear.Modulus(19);
		var monthsElapsed = 235 * cycle + 12 * yearInCycle + (7 * yearInCycle + 1) / 19;
		var partsElapsed = 204 + 793 * monthsElapsed;
		var hoursElapsed = 5 + 12 * monthsElapsed + partsElapsed / 1080;
		var day = 1 + 29 * monthsElapsed + hoursElapsed / 24;
		var parts = 1080 * (hoursElapsed.Modulus(24)) + partsElapsed.Modulus(1080);

		if (parts >= 19440 ||
		    day.Modulus(7) == 2 && parts >= 9924 && !IsLeapYear(year) ||
		    day.Modulus(7) == 1 && parts >= 16789 && IsLeapYear(year - 1))
		{
			day++;
		}

		if (day.Modulus(7).In(0, 3, 5))
		{
			day++;
		}

		return day;
	}
}

public sealed class SolarEquinoxCalendarAlgorithm : ICalculatedCalendarAlgorithm
{
	private static readonly int[] LeapYears = [1, 5, 9, 13, 17, 22, 26, 30];

	public SolarEquinoxCalendarAlgorithm(XElement? root)
	{
		TargetLongitude = root?.Attribute("longitude")?.Value.GetDouble() ?? 0.0;
	}

	public CalendarAlgorithmType Type => CalendarAlgorithmType.SolarEquinox;

	public string DisplayName => "Solar Equinox";

	public string Summary => $"Deterministic solar year with leap years in a 33-year cycle at longitude {TargetLongitude.ToString("N3", CultureInfo.InvariantCulture)}";

	public double TargetLongitude { get; }

	public Year CreateYear(ICalendar calendar, int whichYear)
	{
		return new Year(CreateMonths(calendar, whichYear), whichYear, calendar);
	}

	public int CountWeekdaysInYear(ICalendar calendar, int whichYear)
	{
		return CreateMonths(calendar, whichYear).Sum(x => x.CountWeekdays());
	}

	public int CountDaysInYear(ICalendar calendar, int whichYear)
	{
		return CreateMonths(calendar, whichYear).Sum(x => x.Days);
	}

	private static List<Month> CreateMonths(ICalendar calendar, int whichYear)
	{
		var leap = IsLeapYear(whichYear);
		var months = new List<Month>();
		if (calendar.Months.Count == 13)
		{
			for (var i = 0; i < calendar.Months.Count; i++)
			{
				var source = calendar.Months[i];
				months.Add(TabularLunarCalendarAlgorithm.BuildMonth(source, whichYear,
					source.NormalDays + (i == calendar.Months.Count - 1 && leap ? 1 : 0)));
			}

			return months;
		}

		for (var i = 0; i < calendar.Months.Count; i++)
		{
			var days = i < 6 ? 31 : i < 11 ? 30 : leap ? 30 : 29;
			months.Add(TabularLunarCalendarAlgorithm.BuildMonth(calendar.Months[i], whichYear, days));
		}

		return months;
	}

	public XElement SaveToXml()
	{
		return new XElement("algorithm",
			new XAttribute("type", CalendarAlgorithmFactory.XmlTypeFor(Type)),
			new XAttribute("longitude", TargetLongitude.ToString(CultureInfo.InvariantCulture)));
	}

	public static bool IsLeapYear(int year)
	{
		return LeapYears.Contains(((year - 1).Modulus(33)) + 1);
	}
}

public sealed class AstronomicalLunarCalendarAlgorithm : ICalculatedCalendarAlgorithm, IAstronomicalCalendarAlgorithm
{
	private const double SynodicMonth = 29.530588853;

	public AstronomicalLunarCalendarAlgorithm(XElement? root, GeographicCoordinate? authorityLocation)
	{
		Variant = root?.Attribute("variant")?.Value ?? root?.Element("variant")?.Value ?? "deterministic-new-moon";
		AuthorityLocation = authorityLocation;
	}

	public CalendarAlgorithmType Type => CalendarAlgorithmType.AstronomicalLunar;

	public string DisplayName => "Astronomical Lunar";

	public string Summary => $"Mean-lunation deterministic astronomical lunar approximation ({Variant})";

	public string Variant { get; }

	public GeographicCoordinate? AuthorityLocation { get; }

	public Year CreateYear(ICalendar calendar, int whichYear)
	{
		return new Year(CreateMonths(calendar, whichYear), whichYear, calendar);
	}

	public int CountWeekdaysInYear(ICalendar calendar, int whichYear)
	{
		return CreateMonths(calendar, whichYear).Sum(x => x.CountWeekdays());
	}

	public int CountDaysInYear(ICalendar calendar, int whichYear)
	{
		return CreateMonths(calendar, whichYear).Sum(x => x.Days);
	}

	private List<Month> CreateMonths(ICalendar calendar, int whichYear)
	{
		var baseMonths = calendar.Months
		                         .Where(x => !IsLeapMonthAlias(x.Alias))
		                         .Take(12)
		                         .ToList();
		var months = baseMonths
		             .Select((month, index) => TabularLunarCalendarAlgorithm.BuildMonth(month, whichYear,
			             MonthLength((whichYear - calendar.EpochYear) * 12 + index)))
		             .ToList();

		if (Variant.EqualTo("babylonian-regulated") && IsMetonicLeapYear(whichYear))
		{
			var insertAfterUlulu = CycleYear(whichYear, 19) == 17;
			var leapAlias = insertAfterUlulu ? "ululu-ii" : "addaru-ii";
			var source = calendar.Months.FirstOrDefault(x => x.Alias.EqualTo(leapAlias)) ??
			             calendar.Months.FirstOrDefault(x => IsLeapMonthAlias(x.Alias));
			if (source is not null)
			{
				var insertIndex = insertAfterUlulu
					? Math.Max(0, months.FindIndex(x => x.Alias.EqualTo("ululu")) + 1)
					: months.Count;
				months.Insert(insertIndex, TabularLunarCalendarAlgorithm.BuildMonth(source, whichYear,
					MonthLength((whichYear - calendar.EpochYear) * 12 + months.Count), nominalOrder: insertIndex + 1));
			}
		}

		return months;
	}

	public XElement SaveToXml()
	{
		return new XElement("algorithm",
			new XAttribute("type", CalendarAlgorithmFactory.XmlTypeFor(Type)),
			new XAttribute("variant", Variant));
	}

	public static int MonthLength(int lunation)
	{
		return (int)Math.Floor((lunation + 1) * SynodicMonth) - (int)Math.Floor(lunation * SynodicMonth);
	}

	public static bool IsMetonicLeapYear(int year)
	{
		return CycleYear(year, 19).In(3, 6, 8, 11, 14, 17, 19);
	}

	private static int CycleYear(int year, int cycle)
	{
		return ((year - 1).Modulus(cycle)) + 1;
	}

	private static bool IsLeapMonthAlias(string alias)
	{
		return alias.Contains("ii", StringComparison.InvariantCultureIgnoreCase) ||
		       alias.StartsWith("leap-", StringComparison.InvariantCultureIgnoreCase);
	}
}

public sealed class EastAsianLunisolarCalendarAlgorithm : ICalculatedCalendarAlgorithm, IAstronomicalCalendarAlgorithm
{
	public EastAsianLunisolarCalendarAlgorithm(XElement? root, GeographicCoordinate? authorityLocation)
	{
		Variant = root?.Attribute("variant")?.Value ?? root?.Element("variant")?.Value ?? "east-asian-lunisolar";
		AuthorityLocation = authorityLocation;
	}

	public CalendarAlgorithmType Type => CalendarAlgorithmType.EastAsianLunisolar;

	public string DisplayName => "East Asian Lunisolar";

	public string Summary => $"Deterministic lunisolar approximation with Metonic leap-month placement ({Variant})";

	public string Variant { get; }

	public GeographicCoordinate? AuthorityLocation { get; }

	public Year CreateYear(ICalendar calendar, int whichYear)
	{
		return new Year(CreateMonths(calendar, whichYear), whichYear, calendar);
	}

	public int CountWeekdaysInYear(ICalendar calendar, int whichYear)
	{
		return CreateMonths(calendar, whichYear).Sum(x => x.CountWeekdays());
	}

	public int CountDaysInYear(ICalendar calendar, int whichYear)
	{
		return CreateMonths(calendar, whichYear).Sum(x => x.Days);
	}

	private static List<Month> CreateMonths(ICalendar calendar, int whichYear)
	{
		var baseMonths = calendar.Months.Where(x => !x.Alias.StartsWith("leap-", StringComparison.InvariantCultureIgnoreCase))
		                         .Take(12)
		                         .ToList();
		var months = baseMonths
		             .Select((month, index) => TabularLunarCalendarAlgorithm.BuildMonth(month, whichYear,
			             AstronomicalLunarCalendarAlgorithm.MonthLength((whichYear - calendar.EpochYear) * 12 + index)))
		             .ToList();

		if (AstronomicalLunarCalendarAlgorithm.IsMetonicLeapYear(whichYear))
		{
			var leapSource = calendar.Months.FirstOrDefault(x => x.Alias.StartsWith("leap-", StringComparison.InvariantCultureIgnoreCase));
			if (leapSource is not null && months.Count >= 6)
			{
				months.Insert(6, TabularLunarCalendarAlgorithm.BuildMonth(leapSource, whichYear,
					AstronomicalLunarCalendarAlgorithm.MonthLength((whichYear - calendar.EpochYear) * 12 + 6),
					nominalOrder: 6));
			}
		}

		return months;
	}

	public XElement SaveToXml()
	{
		return new XElement("algorithm",
			new XAttribute("type", CalendarAlgorithmFactory.XmlTypeFor(Type)),
			new XAttribute("variant", Variant));
	}
}

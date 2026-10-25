#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class TimeSeeder
{
	// Recovered ordered year labels. NamedYearCycle has no setting-specific combination rules.
	private static readonly string[] ArmageddonYearNames =
	[
		"Jihae's Anger",
		"Drov's Peace",
		"Desert's Vengeance",
		"Ruk's Slumber",
		"Whira's Defiance",
		"Dragon's Reverence",
		"Vivadu's Agitation",
		"King's Anger",
		"Silt's Peace",
		"Suk-Krath's Vengeance",
		"Lirathu's Slumber",
		"Jihae's Defiance",
		"Drov's Reverence",
		"Desert's Agitation",
		"Ruk's Anger",
		"Whira's Peace",
		"Dragon's Vengeance",
		"Vivadu's Slumber",
		"King's Defiance",
		"Silt's Reverence",
		"Suk-Krath's Agitation",
		"Lirathu's Anger",
		"Jihae's Peace",
		"Drov's Vengeance",
		"Desert's Slumber",
		"Ruk's Defiance",
		"Whira's Reverence",
		"Dragon's Agitation",
		"Vivadu's Anger",
		"King's Peace",
		"Silt's Vengeance",
		"Suk-Krath's Slumber",
		"Lirathu's Defiance",
		"Jihae's Reverence",
		"Drov's Agitation",
		"Desert's Anger",
		"Ruk's Peace",
		"Whira's Vengeance",
		"Dragon's Slumber",
		"Vivadu's Defiance",
		"King's Reverence",
		"Silt's Agitation",
		"Suk-Krath's Anger",
		"Lirathu's Peace",
		"Jihae's Vengeance",
		"Drov's Slumber",
		"Desert's Defiance",
		"Ruk's Reverence",
		"Whira's Agitation",
		"Dragon's Anger",
		"Vivadu's Peace",
		"King's Vengeance",
		"Silt's Slumber",
		"Suk-Krath's Defiance",
		"Lirathu's Reverence",
		"Jihae's Agitation",
		"Drov's Anger",
		"Desert's Peace",
		"Ruk's Vengeance",
		"Whira's Slumber",
		"Dragon's Defiance",
		"Vivadu's Reverence",
		"King's Agitation",
		"Silt's Anger",
		"Suk-Krath's Peace",
		"Lirathu's Vengeance",
		"Jihae's Slumber",
		"Drov's Defiance",
		"Desert's Reverence",
		"Ruk's Agitation",
		"Whira's Anger",
		"Dragon's Peace",
		"Vivadu's Vengeance",
		"King's Slumber",
		"Silt's Defiance",
		"Suk-Krath's Reverence",
		"Lirathu's Agitation",
	];

	private static readonly string[] ArmageddonHours =
	[
		"before dawn", "dawn", "early morning", "late morning", "high sun",
		"early afternoon", "late afternoon", "dusk", "late at night"
	];

	private static string SeedArmageddonTime(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> answers)
	{
		context.Database.BeginTransaction();
		try
		{
			var calendar = context.Calendars.AsEnumerable().FirstOrDefault(x => string.Equals(CalendarAlias(x.Definition), "armageddon", StringComparison.OrdinalIgnoreCase));
			var clock = calendar is not null
				? context.Clocks.Find(calendar.FeedClockId) ?? throw new InvalidOperationException("The Armageddon calendar's feed clock is missing.")
				: context.Clocks.AsEnumerable().FirstOrDefault(x => string.Equals(ClockAlias(x.Definition), "Armageddon", StringComparison.OrdinalIgnoreCase));
			if (clock is null)
			{
				clock = EnsureClock(context, new XElement("Clock",
					new XElement("Alias", "Armageddon"), new XElement("Description", "Zalanthan Time"),
					new XElement("ShortDisplayString", "$h:$m:$s"), new XElement("SuperDisplayString", "$h:$m:$s $t"),
					new XElement("LongDisplayString", "$i"), new XElement("SecondsPerMinute", 60),
					new XElement("MinutesPerHour", 60), new XElement("HoursPerDay", 9),
					new XElement("InGameSecondsPerRealSecond", int.Parse(answers["secondsmultiplier"], System.Globalization.CultureInfo.InvariantCulture)),
					new XElement("SecondFixedDigits", 2), new XElement("MinuteFixedDigits", 2),
					new XElement("HourFixedDigits", 1), new XElement("NoZeroHour", false),
					new XElement("NumberOfHourIntervals", 9),
					new XElement("HourIntervalNames", ArmageddonHours.Select(x => new XElement("HourIntervalName", x))),
					new XElement("HourIntervalLongNames", ArmageddonHours.Select(x => new XElement("HourIntervalLongName", x))),
					new XElement("CrudeTimeIntervals", ArmageddonHours.Select((x, i) => new XElement("CrudeTimeInterval",
						new XAttribute("text", x), new XAttribute("Lower", i), new XAttribute("Upper", i + 1))))).ToString());
			}

			context.SaveChanges();
			var timezone = context.Timezones.Where(x => x.ClockId == clock.Id).AsEnumerable()
				.FirstOrDefault(x => x.Name.Equals("ZST", StringComparison.OrdinalIgnoreCase));
			if (timezone is null)
			{
				timezone = new Timezone { Clock = clock, ClockId = clock.Id, Name = "ZST", Description = "Zalanthan Standard Time" };
				context.Timezones.Add(timezone);
			}
			context.SaveChanges();
			if (clock.PrimaryTimezoneId == 0) clock.PrimaryTimezoneId = timezone.Id;
			if (calendar is null)
			{
				var definition = XElement.Parse(BuildCalendarDefinition("armageddon", "Zalanthan Calendar", "Zalanthan Calendar",
					"The calendar of Zalanthas: three 231-day months and a named 77-year King's Age.",
					"$dd/$mo/$yy", "$ww, the $dt day of $mf, in the Year of $yn, Year $yi of $yc $ya",
					"$ww, the $dt day of $mf, in the Year of $yn, Year $yi of $yc $ya",
					"BA", "before the Ages", "A", "of the Ages", 1, 0,
					["Ocandra", "Terrin", "Abid", "Cingel", "Nekrete", "Waleuk", "Yochem", "Huegel", "Dzeda", "Barani", "Detal"],
					[new("descending", "Descending Sun", "Descending Sun", 231), new("low", "Low Sun", "Low Sun", 231),
					 new("ascending", "Ascending Sun", "Ascending Sun", 231)], Algorithm("fixed-months"), DayBoundary("ClockMidnight")));
				definition.Element("plane")!.Value = "zalanthas";
				definition.Add(new XElement("namedyearcycle", new XAttribute("name", "King's Age"), new XAttribute("epochyear", 1),
					ArmageddonYearNames.Select(x => new XElement("year", x))));
				calendar = EnsureCalendar(context, clock, $"01/descending/{answers["startyear"]}", definition.ToString());
				context.SaveChanges();
				SyncShardAndZoneTimeBindings(context, calendar, clock, timezone);
			}

			context.SaveChanges();
			context.Database.CommitTransaction();
			return "Successfully set up the Armageddon clock and calendar; existing definitions and dates were preserved.";
		}
		catch
		{
			context.Database.RollbackTransaction();
			throw;
		}
	}
}

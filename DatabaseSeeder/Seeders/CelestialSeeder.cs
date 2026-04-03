using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using Calendar = MudSharp.Models.Calendar;

namespace DatabaseSeeder.Seeders;

public class CelestialSeeder : IDatabaseSeeder
{
	private const string EarthSunPackage = "EarthSun";
	private const string EarthMoonPackage = "EarthMoonView";
	private const string GasGiantPackage = "GasGiantMoonView";
	private const string SeederPackageElement = "SeederPackage";
	private const string SeederRoleElement = "SeederRole";
	private const string EarthPlanetName = "Earth";
	private const string JupiterPlanetName = "Jupiter";
	private const string JupiterSunName = "Sol";
	private const string GanymedeMoonName = "Ganymede";
	private const double DefaultPeakIllumination = 1.0;
	private const double DefaultFullMoonReferenceDay = 0.0;
	private const double EarthSunDaysPerYear = 365.24;
	private const double EarthSunMeanAnomaly = 6.24006;
	private const double EarthSunAnomalyPerDay = 0.017202;
	private const double EarthSunEclipticLongitude = 1.796595;
	private const double EarthSunObliquity = 0.409093;
	private const double EarthSunOrbitalEccentricity = 0.016713;
	private const double EarthSunSemiMajorAxis = 149597870.7;
	private const double EarthSunApparentAngularRadius = 0.004654793;
	private const double EarthSunDayNumberAtEpoch = 2451545.0;
	private const double EarthSunSiderealTimeAtEpoch = 4.889488;
	private const double EarthSunSiderealTimePerDay = 6.300388;
	private const double EarthSunPeakIllumination = 98000.0;
	private const double EarthSunAlphaScattering = 0.05;
	private const double EarthSunBetaScattering = 0.035;
	private const double EarthSunPlanetaryRadius = 6378.0;
	private const double EarthSunAtmosphericDensityScalingFactor = 6.35;
	private const double EarthMoonDaysPerYear = 29.530588;
	private const double EarthMoonMeanAnomaly = 2.355556;
	private const double EarthMoonAnomalyPerDay = 0.228027;
	private const double EarthMoonArgumentOfPeriapsis = 5.552765;
	private const double EarthMoonLongitudeOfAscendingNode = 2.18244;
	private const double EarthMoonOrbitalInclination = 0.0898;
	private const double EarthMoonOrbitalEccentricity = 0.0549;
	private const double EarthMoonSemiMajorAxis = 384400.0;
	private const double EarthMoonDayNumberAtEpoch = 2451545.0;
	private const double EarthMoonSiderealTimeAtEpoch = 4.889488;
	private const double EarthMoonSiderealTimePerDay = EarthSunSiderealTimePerDay;
	private const double EarthAngularRadiusFromMoon = 0.0165;
	private const double JupiterSunDaysPerYear = 10475.8818867393;
	private const double JupiterSunMeanAnomaly = 0.343270671018783;
	private const double JupiterSunAnomalyPerDay = 0.000599776264672575;
	private const double JupiterSunEclipticLongitude = 0.257060466847075;
	private const double JupiterSunObliquity = 0.0546288055874225;
	private const double JupiterSunOrbitalEccentricity = 0.048775;
	private const double JupiterSunSemiMajorAxis = 778547200.0;
	private const double JupiterSunApparentAngularRadius = 0.000894416;
	private const double JupiterSunDayNumberAtEpoch = 2451545.0;
	private const double JupiterSunSiderealTimeAtEpoch = 4.97331570355784;
	private const double JupiterSunSiderealTimePerDay = 6.28378508344426;
	private const double JupiterSunKepplerC1 = 0.0967569879178372;
	private const double JupiterSunKepplerC2 = 0.0029273119273445;
	private const double JupiterSunKepplerC3 = 0.000122772356038737;
	private const double JupiterSunPeakIllumination = 3619.8;
	private const double JupiterSunPlanetaryRadius = 69911.0;
	private const double GanymedeDaysPerYear = 7.16683557838692;
	private const double GanymedeMeanAnomaly = 0.82944303060139;
	private const double GanymedeAnomalyPerDay = 0.878153082764442;
	private const double GanymedeArgumentOfPeriapsis = 4.87986552021969;
	private const double GanymedeLongitudeOfAscendingNode = 5.96372010319795;
	private const double GanymedeOrbitalInclination = 0.0355727108921961;
	private const double GanymedeOrbitalEccentricity = 0.00158762974782861;
	private const double GanymedeSemiMajorAxis = 1070400.0;
	private const double GanymedeDayNumberAtEpoch = 2451545.0;
	private const double GanymedeSiderealTimeAtEpoch = JupiterSunSiderealTimeAtEpoch;
	private const double GanymedeSiderealTimePerDay = JupiterSunSiderealTimePerDay;
	private const double JupiterAngularRadiusFromGanymede = 0.0653594916439514;

	private static readonly Regex DateValidationRegex = new(@"\d+(-|/|\s)\w+(-|/|\s)\d+", RegexOptions.IgnoreCase);

	public bool SafeToRunMoreThanOnce => true;

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
	[
		("installsun",
			@"Do you want to install an Earth-facing Sun? This option assumes that your world is based on an Earth-like planet orbiting Sol.

Please answer #3yes#F or #3no#F: ",
			(context, answers) => true,
			ValidateYesNo),
		("suncalendar",
			@"Which calendar would you like your Earth-facing sun to be tied to, for the purposes of working out where it should be in the sky?

Please specify a calendar name or ID (if not known, use 1 for ID): ",
			(context, answers) => AnswerIsYes(answers, "installsun"),
			ValidateCalendar),
		("sunname",
			@"What name would you like to give to your Earth-facing sun? For example, for Earth's sun you would enter 'The Sun' or 'Sol'.

Your choice: ",
			(context, answers) => AnswerIsYes(answers, "installsun"),
			(answer, context) => (true, string.Empty)),
		("sunepoch",
			@"You must now enter a valid date for the 'epoch' of your Earth-facing sun.

Generally you'll want this to be whatever date is equivalent to the 1st of January in your calendar, and the same year that your game's current date is.

What epoch date do you want to use?",
			(context, answers) => AnswerIsYes(answers, "installsun"),
			ValidateDate),
		("installmoon",
			@"Do you want to install the Earth's Moon package? This installs the planetary moon plus the matching Earth and Sol moon-view celestials.",
			(context, answers) => AnswerIsYes(answers, "installsun") || HasSingleEarthSunCandidate(context),
			ValidateYesNo),
		("mooncalendar",
			@"Which calendar should the Earth Moon package be tied to?",
			(context, answers) => AnswerIsYes(answers, "installmoon"),
			ValidateCalendar),
		("moonname",
			@"What name would you like to give to your moon?",
			(context, answers) => AnswerIsYes(answers, "installmoon"),
			(answer, context) => (true, string.Empty)),
		("moonepoch",
			@"What epoch date should be used for the moon? This is a date that is known to be a full moon. For Earth, you could use 21/jan/2000.",
			(context, answers) => AnswerIsYes(answers, "installmoon"),
			ValidateDate),
		("installgasgiantmoon",
			@"Do you want to install the Gas Giant Moon package? This installs a Jupiter-facing Sun, Ganymede, Jupiter-from-Ganymede, and Sol-from-Ganymede.",
			(context, answers) => true,
			ValidateYesNo),
		("gasgiantcalendar",
			@"Which calendar should the Gas Giant Moon package be tied to?",
			(context, answers) => AnswerIsYes(answers, "installgasgiantmoon"),
			ValidateCalendar),
		("gasgiantsunepoch",
			@"What epoch date should be used for the Jupiter-facing Sun? This should be the start-of-year epoch for your chosen calendar.",
			(context, answers) => AnswerIsYes(answers, "installgasgiantmoon"),
			ValidateDate),
		("gasgiantmoonepoch",
			@"What epoch date should be used for Ganymede? This should be a date that is known to be a full Ganymede as seen from Jupiter.",
			(context, answers) => AnswerIsYes(answers, "installgasgiantmoon"),
			ValidateDate)
	];

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		context.Database.BeginTransaction();
		try
		{
			Celestial? earthSun = null;
			if (AnswerIsYes(questionAnswers, "installsun"))
			{
				earthSun = EnsureEarthSunPackage(context, questionAnswers);
			}

			if (AnswerIsYes(questionAnswers, "installmoon"))
			{
				EnsureEarthMoonPackage(context, questionAnswers, earthSun);
			}

			if (AnswerIsYes(questionAnswers, "installgasgiantmoon"))
			{
				EnsureGasGiantPackage(context, questionAnswers);
			}

			context.SaveChanges();
			context.Database.CommitTransaction();
			return "Successfully set up celestials.";
		}
		catch
		{
			context.Database.RollbackTransaction();
			throw;
		}
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any())
		{
			return ShouldSeedResult.PrerequisitesNotMet;
		}

		var hasEarthSun = HasEarthSunPackage(context);
		var hasEarthMoon = HasEarthMoonPackage(context);
		var hasGasGiant = HasGasGiantPackage(context);
		if (!hasEarthSun && !hasEarthMoon && !hasGasGiant)
		{
			return ShouldSeedResult.ReadyToInstall;
		}

		return hasEarthSun && hasEarthMoon && hasGasGiant
			? ShouldSeedResult.MayAlreadyBeInstalled
			: ShouldSeedResult.ExtraPackagesAvailable;
	}

	public int SortOrder => 6;
	public string Name => "Celestial Seeder";
	public string Tagline => "Sets up Suns, Moons, etc";
	public string FullDescription => "This seeder sets up stock celestial packages such as Earth-facing suns, planetary moons, and moon-view celestial objects. It is additive and intended to be rerun safely.";

	internal static string? ResolveSunEpochDefault(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers)
	{
		return ResolveEpochDefault(context, answers, "suncalendar", 1);
	}

	internal static string? ResolveMoonEpochDefault(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers)
	{
		return ResolveEpochDefault(context, answers, "mooncalendar", 21);
	}

	internal static string? ResolveGasGiantSunEpochDefault(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers)
	{
		return ResolveEpochDefault(context, answers, "gasgiantcalendar", 1);
	}

	internal static string? ResolveGasGiantMoonEpochDefault(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers)
	{
		return ResolveEpochDefault(context, answers, "gasgiantcalendar", 1);
	}

	internal static ConsoleQuestionDisplay ResolveSunEpochDisplay(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers)
	{
		return BuildEpochDisplay(
			context,
			answers,
			"suncalendar",
			"You must now enter a valid date for the 'epoch' of your Earth-facing sun.",
			"Generally you'll want this to be whatever date is equivalent to the 1st of the first month in your calendar, in the same year that your game's current date uses.",
			"A sensible stock default for this package is the first day of the first month.",
			1);
	}

	internal static ConsoleQuestionDisplay ResolveMoonEpochDisplay(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers)
	{
		return BuildEpochDisplay(
			context,
			answers,
			"mooncalendar",
			"What epoch date should be used for the moon?",
			"This should be a date that is known to be a full moon in the calendar you selected.",
			"The stock Earth moon package uses day 21 of the first month as its known full-moon reference.",
			21);
	}

	internal static ConsoleQuestionDisplay ResolveGasGiantSunEpochDisplay(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers)
	{
		return BuildEpochDisplay(
			context,
			answers,
			"gasgiantcalendar",
			"What epoch date should be used for the Jupiter-facing Sun?",
			"This should be the start-of-year epoch for your chosen calendar.",
			"A sensible stock default for this package is the first day of the first month.",
			1);
	}

	internal static ConsoleQuestionDisplay ResolveGasGiantMoonEpochDisplay(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers)
	{
		return BuildEpochDisplay(
			context,
			answers,
			"gasgiantcalendar",
			"What epoch date should be used for Ganymede?",
			"This is the stock epoch-aligned reference date used by the seeded Jupiter/Ganymede package.",
			"The seeded default uses the first day of the first month so the package lines up with the authored epoch constants.",
			1);
	}

	private static bool AnswerIsYes(IReadOnlyDictionary<string, string> answers, string key)
	{
		return answers.TryGetValue(key, out var value) && value.EqualToAny("yes", "y");
	}

	private static (bool Success, string error) ValidateYesNo(string answer, FuturemudDatabaseContext context)
	{
		return answer.EqualToAny("yes", "y", "no", "n") ? (true, string.Empty) : (false, "Please choose yes or no.");
	}

	private static (bool Success, string error) ValidateDate(string answer, FuturemudDatabaseContext context)
	{
		return DateValidationRegex.IsMatch(answer)
			? (true, string.Empty)
			: (false, "The date you supplied is definitely not a valid date. Please refer to the guidance above.");
	}

	private static string? ResolveEpochDefault(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers,
		string calendarAnswerKey,
		int day)
	{
		return TryGetCalendarDateFormat(context, answers, calendarAnswerKey, day, out var formattedDate)
			? formattedDate
			: null;
	}

	private static ConsoleQuestionDisplay BuildEpochDisplay(
		FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers,
		string calendarAnswerKey,
		string heading,
		string guidance,
		string stockDefaultExplanation,
		int defaultDay)
	{
		if (!TryGetCalendarPromptInfo(context, answers, calendarAnswerKey, out var info))
		{
			return new ConsoleQuestionDisplay(
				$@"{heading}

{guidance}

Please enter the epoch date using the format of the calendar you selected.",
				null);
		}

		var monthExample = FormatCalendarDate(info, 1, info.FirstMonthAlias, "year");
		var moonExample = FormatCalendarDate(info, 21, info.FirstMonthAlias, "year");
		var defaultAnswer = ResolveEpochDefault(context, answers, calendarAnswerKey, defaultDay);

		var exampleText = defaultDay == 21
			? $"For this calendar, a first-month full-moon style example would look like {moonExample}."
			: $"For this calendar, a start-of-year example would look like {monthExample}.";

		return new ConsoleQuestionDisplay(
			$@"{heading}

{guidance}

The selected calendar is {info.CalendarName}.
{exampleText}
{stockDefaultExplanation}",
			defaultAnswer);
	}

	private static bool TryGetCalendarDateFormat(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers,
		string calendarAnswerKey,
		int day,
		out string? formattedDate)
	{
		if (TryGetCalendarPromptInfo(context, answers, calendarAnswerKey, out var info))
		{
			formattedDate = FormatCalendarDate(info, day, info.FirstMonthAlias, info.CurrentYear);
			return true;
		}

		formattedDate = null;
		return false;
	}

	private static bool TryGetCalendarPromptInfo(FuturemudDatabaseContext context,
		IReadOnlyDictionary<string, string> answers,
		string calendarAnswerKey,
		out CalendarPromptInfo info)
	{
		info = null!;
		if (!answers.TryGetValue(calendarAnswerKey, out var calendarAnswer) ||
		    !TryGetCalendar(context, calendarAnswer, out var calendar) ||
		    string.IsNullOrWhiteSpace(calendar?.Definition) ||
		    string.IsNullOrWhiteSpace(calendar.Date))
		{
			return false;
		}

		var definition = XElement.Parse(calendar.Definition);
		var firstMonthAlias = definition.Descendants("month")
			                     .Select(x => x.Element("alias")?.Value)
			                     .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ??
		                     "january";
		var calendarName = definition.Element("fullname")?.Value ??
		                   definition.Element("shortname")?.Value ??
		                   definition.Element("alias")?.Value ??
		                   $"calendar #{calendar.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
		var parts = SplitDateParts(calendar.Date);
		if (parts.Count != 3)
		{
			return false;
		}

		var yearIndex = FindYearIndex(parts);
		if (yearIndex == -1)
		{
			return false;
		}

		info = new CalendarPromptInfo(calendar, calendarName, firstMonthAlias, calendar.Date, parts[yearIndex]);
		return true;
	}

	internal static string FormatCalendarDate(Calendar calendar, int day, string monthAlias, string year)
	{
		if (string.IsNullOrWhiteSpace(calendar.Date))
		{
			return $"{day:00}/{monthAlias}/{year}";
		}

		var parts = SplitDateParts(calendar.Date);
		var separators = SplitDateSeparators(calendar.Date);
		if (parts.Count != 3 || separators.Count != 2)
		{
			return $"{day:00}/{monthAlias}/{year}";
		}

		var yearIndex = FindYearIndex(parts);
		var dayIndex = FindDayIndex(parts, yearIndex);
		var monthIndex = Enumerable.Range(0, 3).First(x => x != yearIndex && x != dayIndex);
		var rendered = new string[3];
		rendered[yearIndex] = year;
		rendered[dayIndex] = day.ToString($"D{Math.Max(2, parts[dayIndex].Length)}", System.Globalization.CultureInfo.InvariantCulture);
		rendered[monthIndex] = monthAlias;
		return $"{rendered[0]}{separators[0]}{rendered[1]}{separators[1]}{rendered[2]}";
	}

	private static string FormatCalendarDate(CalendarPromptInfo info, int day, string monthAlias, string year)
	{
		return FormatCalendarDate(info.Calendar, day, monthAlias, year);
	}

	private static List<string> SplitDateParts(string value)
	{
		return Regex.Split(value, @"[-/\\\s]+")
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.ToList();
	}

	private static List<string> SplitDateSeparators(string value)
	{
		return Regex.Matches(value, @"[-/\\\s]+")
			.Select(x => x.Value)
			.ToList();
	}

	private static int FindYearIndex(IReadOnlyList<string> parts)
	{
		for (var i = parts.Count - 1; i >= 0; i--)
		{
			if (int.TryParse(parts[i], NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out _))
			{
				return i;
			}
		}

		return -1;
	}

	private static int FindDayIndex(IReadOnlyList<string> parts, int yearIndex)
	{
		for (var i = 0; i < parts.Count; i++)
		{
			if (i == yearIndex)
			{
				continue;
			}

			if (int.TryParse(parts[i], NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out _))
			{
				return i;
			}
		}

		return yearIndex == 2 ? 0 : 1;
	}

	private sealed record CalendarPromptInfo(
		Calendar Calendar,
		string CalendarName,
		string FirstMonthAlias,
		string CurrentDate,
		string CurrentYear);

	private static (bool Success, string error) ValidateCalendar(string answer, FuturemudDatabaseContext context)
	{
		if (TryGetCalendar(context, answer, out _))
		{
			return (true, string.Empty);
		}

		return (false,
			$"You must choose a valid calendar by ID or exact name. Valid IDs are: {context.Calendars.Select(x => x.Id.ToString("N0", System.Globalization.CultureInfo.InvariantCulture)).ListToCommaSeparatedValues(", ")}");
	}

	private Celestial EnsureEarthSunPackage(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> answers)
	{
		var existing = GetEarthSunCandidates(context).FirstOrDefault();
		if (existing is not null)
		{
			return existing;
		}

		var calendar = GetCalendar(context, answers["suncalendar"]);
		var calendarId = calendar.Id;
		var sun = CreateCelestial("Sun", calendar.FeedClockId,
			BuildSunDefinition(
				answers["sunname"], calendarId, answers["sunepoch"], EarthSunPackage, "Sun",
				EarthSunDaysPerYear, EarthSunMeanAnomaly, EarthSunAnomalyPerDay, EarthSunEclipticLongitude, EarthSunObliquity,
				EarthSunOrbitalEccentricity, EarthSunSemiMajorAxis, EarthSunApparentAngularRadius,
				EarthSunDayNumberAtEpoch, EarthSunSiderealTimeAtEpoch, EarthSunSiderealTimePerDay,
				0.033419565, 0.000349066, 0.000005235988, 0.0, 0.0, 0.0,
				EarthSunPeakIllumination, EarthSunAlphaScattering, EarthSunBetaScattering,
				EarthSunPlanetaryRadius, EarthSunAtmosphericDensityScalingFactor));
		context.Celestials.Add(sun);
		context.SaveChanges();
		return sun;
	}

	private void EnsureEarthMoonPackage(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> answers, Celestial? createdEarthSun)
	{
		if (HasEarthMoonPackage(context))
		{
			return;
		}

		if (!TryGetEarthSunForMoonPackage(context, createdEarthSun, out var sun, out var error))
		{
			throw new InvalidOperationException(error);
		}

		var calendar = GetCalendar(context, answers["mooncalendar"]);
		var calendarId = calendar.Id;
		var moon = CreateCelestial("PlanetaryMoon", calendar.FeedClockId,
			BuildMoonDefinition(
				answers["moonname"], calendarId, answers["moonepoch"], EarthMoonPackage, "Moon",
				EarthMoonDaysPerYear, EarthMoonMeanAnomaly, EarthMoonAnomalyPerDay, EarthMoonArgumentOfPeriapsis,
				EarthMoonLongitudeOfAscendingNode, EarthMoonOrbitalInclination, EarthMoonOrbitalEccentricity,
				EarthMoonSemiMajorAxis,
				EarthMoonDayNumberAtEpoch, EarthMoonSiderealTimeAtEpoch, EarthMoonSiderealTimePerDay));
		context.Celestials.Add(moon);
		context.SaveChanges();

		context.Celestials.Add(CreateCelestial("PlanetFromMoon", calendar.FeedClockId,
			BuildPlanetFromMoonDefinition(EarthMoonPackage, "Planet", EarthPlanetName, moon.Id, sun!.Id,
				EarthAngularRadiusFromMoon, GetSunApparentAngularRadius(sun))));
		context.Celestials.Add(CreateCelestial("SunFromPlanetaryMoon", calendar.FeedClockId,
			BuildSunFromMoonDefinition(EarthMoonPackage, "SunFromMoon", GetSunName(sun), moon.Id, sun.Id, GetSunIlluminationXml(sun))));
	}

	private void EnsureGasGiantPackage(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> answers)
	{
		if (HasGasGiantPackage(context))
		{
			return;
		}

		var calendar = GetCalendar(context, answers["gasgiantcalendar"]);
		var calendarId = calendar.Id;
		var sun = CreateCelestial("Sun", calendar.FeedClockId,
			BuildSunDefinition(
				JupiterSunName, calendarId, answers["gasgiantsunepoch"], GasGiantPackage, "Sun",
				JupiterSunDaysPerYear, JupiterSunMeanAnomaly, JupiterSunAnomalyPerDay, JupiterSunEclipticLongitude, JupiterSunObliquity,
				JupiterSunOrbitalEccentricity, JupiterSunSemiMajorAxis, JupiterSunApparentAngularRadius,
				JupiterSunDayNumberAtEpoch, JupiterSunSiderealTimeAtEpoch, JupiterSunSiderealTimePerDay,
				JupiterSunKepplerC1, JupiterSunKepplerC2, JupiterSunKepplerC3, 0.0, 0.0, 0.0,
				JupiterSunPeakIllumination, EarthSunAlphaScattering, EarthSunBetaScattering,
				JupiterSunPlanetaryRadius, EarthSunAtmosphericDensityScalingFactor));
		context.Celestials.Add(sun);
		context.SaveChanges();

		var moon = CreateCelestial("PlanetaryMoon", calendar.FeedClockId,
			BuildMoonDefinition(
				GanymedeMoonName, calendarId, answers["gasgiantmoonepoch"], GasGiantPackage, "Moon",
				GanymedeDaysPerYear, GanymedeMeanAnomaly, GanymedeAnomalyPerDay, GanymedeArgumentOfPeriapsis,
				GanymedeLongitudeOfAscendingNode, GanymedeOrbitalInclination, GanymedeOrbitalEccentricity,
				GanymedeSemiMajorAxis,
				GanymedeDayNumberAtEpoch, GanymedeSiderealTimeAtEpoch, GanymedeSiderealTimePerDay));
		context.Celestials.Add(moon);
		context.SaveChanges();

		context.Celestials.Add(CreateCelestial("PlanetFromMoon", calendar.FeedClockId,
			BuildPlanetFromMoonDefinition(GasGiantPackage, "Planet", JupiterPlanetName, moon.Id, sun.Id,
				JupiterAngularRadiusFromGanymede, GetSunApparentAngularRadius(sun))));
		context.Celestials.Add(CreateCelestial("SunFromPlanetaryMoon", calendar.FeedClockId,
			BuildSunFromMoonDefinition(GasGiantPackage, "SunFromMoon", JupiterSunName, moon.Id, sun.Id, GetSunIlluminationXml(sun))));
	}

	private static Celestial CreateCelestial(string type, long feedClockId, string definition)
	{
		return new Celestial
		{
			CelestialType = type,
			CelestialYear = 0,
			LastYearBump = 0,
			Minutes = 0,
			FeedClockId = feedClockId,
			Definition = definition
		};
	}

	private static string BuildSunDefinition(
		string name, long calendarId, string epoch, string packageName, string role,
		double daysPerYear, double meanAnomaly, double anomalyPerDay, double eclipticLongitude, double obliquity,
		double orbitalEccentricity, double semiMajorAxis, double apparentAngularRadius,
		double dayNumberAtEpoch, double siderealTimeAtEpoch, double siderealTimePerDay,
		double c1, double c2, double c3, double c4, double c5, double c6,
		double peakIllumination, double alphaScattering, double betaScattering, double planetaryRadius, double atmosphericDensityScalingFactor)
	{
		return new XElement("SunV2",
			new XElement("Name", name),
			Metadata(packageName, role),
			new XElement("Calendar", calendarId),
			new XElement("Orbital",
				new XElement("CelestialDaysPerYear", Format(daysPerYear)),
				new XElement("MeanAnomalyAngleAtEpoch", Format(meanAnomaly)),
				new XElement("AnomalyChangeAnglePerDay", Format(anomalyPerDay)),
				new XElement("EclipticLongitude", Format(eclipticLongitude)),
				new XElement("EquatorialObliquity", Format(obliquity)),
				new XElement("OrbitalEccentricity", Format(orbitalEccentricity)),
				new XElement("OrbitalSemiMajorAxis", Format(semiMajorAxis)),
				new XElement("ApparentAngularRadius", Format(apparentAngularRadius)),
				new XElement("DayNumberAtEpoch", Format(dayNumberAtEpoch)),
				new XElement("SiderealTimeAtEpoch", Format(siderealTimeAtEpoch)),
				new XElement("SiderealTimePerDay", Format(siderealTimePerDay)),
				new XElement("KepplerC1Approximant", Format(c1)),
				new XElement("KepplerC2Approximant", Format(c2)),
				new XElement("KepplerC3Approximant", Format(c3)),
				new XElement("KepplerC4Approximant", Format(c4)),
				new XElement("KepplerC5Approximant", Format(c5)),
				new XElement("KepplerC6Approximant", Format(c6)),
				new XElement("EpochDate", epoch)),
			new XElement("Illumination",
				new XElement("PeakIllumination", Format(peakIllumination)),
				new XElement("AlphaScatteringConstant", Format(alphaScattering)),
				new XElement("BetaScatteringConstant", Format(betaScattering)),
				new XElement("PlanetaryRadius", Format(planetaryRadius)),
				new XElement("AtmosphericDensityScalingFactor", Format(atmosphericDensityScalingFactor))),
			StandardSunTriggers(),
			StandardSunElevationDescriptions(),
			StandardSunAzimuthDescriptions())
			.ToString(SaveOptions.DisableFormatting);
	}

	private static string BuildMoonDefinition(
		string name, long calendarId, string epoch, string packageName, string role,
		double daysPerYear, double meanAnomaly, double anomalyPerDay, double periapsis, double ascendingNode,
		double inclination, double eccentricity, double semiMajorAxis, double dayNumberAtEpoch, double siderealTimeAtEpoch, double siderealTimePerDay)
	{
		return new XElement("PlanetaryMoon",
			new XElement("Name", name),
			Metadata(packageName, role),
			new XElement("Calendar", calendarId),
			new XElement("Orbital",
				new XElement("CelestialDaysPerYear", Format(daysPerYear)),
				new XElement("MeanAnomalyAngleAtEpoch", Format(meanAnomaly)),
				new XElement("AnomalyChangeAnglePerDay", Format(anomalyPerDay)),
				new XElement("ArgumentOfPeriapsis", Format(periapsis)),
				new XElement("LongitudeOfAscendingNode", Format(ascendingNode)),
				new XElement("OrbitalInclination", Format(inclination)),
				new XElement("OrbitalEccentricity", Format(eccentricity)),
				new XElement("OrbitalSemiMajorAxis", Format(semiMajorAxis)),
				new XElement("DayNumberAtEpoch", Format(dayNumberAtEpoch)),
				new XElement("SiderealTimeAtEpoch", Format(siderealTimeAtEpoch)),
				new XElement("SiderealTimePerDay", Format(siderealTimePerDay)),
				new XElement("EpochDate", epoch)),
			new XElement("Illumination",
				new XElement("PeakIllumination", Format(DefaultPeakIllumination)),
				new XElement("FullMoonReferenceDay", Format(DefaultFullMoonReferenceDay))),
			new XElement("Triggers",
				new XElement("Trigger", new XAttribute("angle", "-0.015184"), new XAttribute("direction", "Ascending"), new XCData("The moon rises above the horizon.")),
				new XElement("Trigger", new XAttribute("angle", "-0.015184"), new XAttribute("direction", "Descending"), new XCData("The moon sets on the horizon."))))
			.ToString(SaveOptions.DisableFormatting);
	}

	private static string BuildPlanetFromMoonDefinition(
		string packageName,
		string role,
		string name,
		long moonId,
		long sunId,
		double angularRadius,
		double sunAngularRadius)
	{
		return new XElement("PlanetFromMoon",
			new XElement("Name", name),
			Metadata(packageName, role),
			new XElement("Moon", moonId),
			new XElement("Sun", sunId),
			new XElement("PeakIllumination", Format(DefaultPeakIllumination)),
			new XElement("AngularRadius", Format(angularRadius)),
			new XElement("SunAngularRadius", Format(sunAngularRadius)))
			.ToString(SaveOptions.DisableFormatting);
	}

	private static string BuildSunFromMoonDefinition(string packageName, string role, string name, long moonId, long sunId, string illuminationXml)
	{
		var root = new XElement("SunFromPlanetaryMoon",
			new XElement("Name", name),
			Metadata(packageName, role),
			new XElement("Moon", moonId),
			new XElement("Sun", sunId));
		var illumination = string.IsNullOrWhiteSpace(illuminationXml) ? null : XElement.Parse(illuminationXml);
		if (illumination is not null)
		{
			root.Add(illumination);
		}

		return root.ToString(SaveOptions.DisableFormatting);
	}

	private static object[] Metadata(string packageName, string role)
	{
		return
		[
			new XElement(SeederPackageElement, packageName),
			new XElement(SeederRoleElement, role)
		];
	}

	private static XElement StandardSunTriggers()
	{
		return new XElement("Triggers",
			new XElement("Trigger", new XAttribute("angle", "-0.015184364492350668"), new XAttribute("direction", "Ascending"), new XCData("The edge of the sun rises over the horizon as dawn breaks.")),
			new XElement("Trigger", new XAttribute("angle", "-0.015184364492350668"), new XAttribute("direction", "Descending"), new XCData("The sun says its goodbyes for the day and sets on the horizon")),
			new XElement("Trigger", new XAttribute("angle", "-0.20943951023931953"), new XAttribute("direction", "Ascending"), new XCData("The first faint traces of light begin to dim the eastern sky as dawn approaches.")),
			new XElement("Trigger", new XAttribute("angle", "-0.20943951023931953"), new XAttribute("direction", "Descending"), new XCData("The last traces of light leave the western sky, and the night begins.")),
			new XElement("Trigger", new XAttribute("angle", "-0.10471975511965977"), new XAttribute("direction", "Ascending"), new XCData("The eastern sky begins to come alive with colour and light as dawn approaches.")),
			new XElement("Trigger", new XAttribute("angle", "-0.10471975511965977"), new XAttribute("direction", "Descending"), new XCData("The glow in the western sky, the last remnants of the day that was, fade away to a dim memory, heralding the evening.")),
			new XElement("Trigger", new XAttribute("angle", "0.052359877559829883"), new XAttribute("direction", "Descending"), new XCData("Shadows lengthen and the western sky turns shades of orange and pink as the sun dips low to the horizon.")));
	}

	private static XElement StandardSunElevationDescriptions()
	{
		return new XElement("ElevationDescriptions",
			Description("-1.5707963267948966192313216916398", "-0.20944", "is gone from the sky, and it is night."),
			Description("-0.20944", "-0.10472", "is only visible as a faint, dim glow just beneath the {0} horizon."),
			Description("-0.10472", "-0.0152", "is only visible as a warm glow just beneath the {0} horizon."),
			Description("-0.0152", "-0.00595", "is partially visible above the {0} horizon."),
			Description("-0.00595", "0.1047", "is very low in the {0} sky, colour awash with oranges and pinks."),
			Description("0.1047", "0.21", "is low in the {0} sky, casting long, dark shadows over the land."),
			Description("0.21", "0.42", "is in the {0} sky, at a middle range of elevation."),
			Description("0.42", "1.20943951023931953", "is high in the {0} sky, casting its rays far and wide."),
			Description("1.20943951023931953", "1.35", "is very nearly directly overhead."),
			Description("1.35", "1.5707963267948966192313216916398", "is directly overhead, banishing shadows from the land."));
	}

	private static XElement StandardSunAzimuthDescriptions()
	{
		return new XElement("AzimuthDescriptions",
			Description("-0.19634954084936647692528676655901", "0.19634954084936647692528676655901", "northern"),
			Description("0.19634954084936647692528676655901", "0.589048622548086", "north-northeastern"),
			Description("0.589048622548086", "0.98174770424681", "northeastern"),
			Description("0.98174770424681", "1.37444678594553", "east-northeastern"),
			Description("1.37444678594553", "1.76714586764426", "eastern"),
			Description("1.76714586764426", "2.15984494934298", "east-southeastern"),
			Description("2.15984494934298", "2.55254403104171", "southeastern"),
			Description("2.55254403104171", "2.94524311274043", "south-southeastern"),
			Description("2.94524311274043", "3.33794219443916", "southern"),
			Description("3.33794219443916", "3.73064127613788", "south-southwestern"),
			Description("3.73064127613788", "4.1233403578366", "southwestern"),
			Description("4.1233403578366", "4.51603943953533", "west-southwestern"),
			Description("4.51603943953533", "4.90873852123405", "western"),
			Description("4.90873852123405", "5.30143760293278", "west-northwestern"),
			Description("5.30143760293278", "5.6941366846315", "northwestern"),
			Description("5.6941366846315", "6.08683576633022", "north-northwestern"));
	}

	private static XElement Description(string lower, string upper, string text)
	{
		return new XElement("Description",
			new XAttribute("lower", lower),
			new XAttribute("upper", upper),
			new XCData(text));
	}

	private bool TryGetEarthSunForMoonPackage(FuturemudDatabaseContext context, Celestial? createdEarthSun, out Celestial? sun, out string error)
	{
		if (createdEarthSun is not null)
		{
			sun = createdEarthSun;
			error = string.Empty;
			return true;
		}

		var candidates = GetEarthSunCandidates(context).Take(2).ToList();
		if (candidates.Count == 1)
		{
			sun = candidates[0];
			error = string.Empty;
			return true;
		}

		sun = null;
		error = candidates.Count == 0
			? "The Earth's Moon package requires a matching Earth-facing Sun. Install the Earth-facing Sun package first or in the same seeder run."
			: "The Earth's Moon package found multiple Earth-facing Suns and cannot determine which one to link.";
		return false;
	}

	private bool HasSingleEarthSunCandidate(FuturemudDatabaseContext context)
	{
		return GetEarthSunCandidates(context).Take(2).Count() == 1;
	}

	private bool HasEarthSunPackage(FuturemudDatabaseContext context)
	{
		return GetEarthSunCandidates(context).Any();
	}

	private bool HasEarthMoonPackage(FuturemudDatabaseContext context)
	{
		var markedTypes = context.Celestials.AsEnumerable()
			.Where(x => GetPackageName(x) == EarthMoonPackage)
			.Select(x => x.CelestialType)
			.ToHashSet();
		return (markedTypes.Contains("PlanetaryMoon") && markedTypes.Contains("PlanetFromMoon") && markedTypes.Contains("SunFromPlanetaryMoon")) ||
		       GetLegacyEarthMoonTriples(context).Any();
	}

	private bool HasGasGiantPackage(FuturemudDatabaseContext context)
	{
		var markedTypes = context.Celestials.AsEnumerable()
			.Where(x => GetPackageName(x) == GasGiantPackage)
			.Select(x => x.CelestialType)
			.ToHashSet();
		return markedTypes.Contains("Sun") && markedTypes.Contains("PlanetaryMoon") &&
		       markedTypes.Contains("PlanetFromMoon") && markedTypes.Contains("SunFromPlanetaryMoon");
	}

	private IEnumerable<Celestial> GetEarthSunCandidates(FuturemudDatabaseContext context)
	{
		return context.Celestials.AsEnumerable().Where(IsEarthSunCelestial);
	}

	private bool IsEarthSunCelestial(Celestial celestial)
	{
		if (!celestial.CelestialType.EqualTo("Sun"))
		{
			return false;
		}

		if (GetPackageName(celestial) == EarthSunPackage)
		{
			return true;
		}

		var root = TryGetDefinitionRoot(celestial);
		return root is not null &&
		       ValuesMatch(root, "Orbital/CelestialDaysPerYear", EarthSunDaysPerYear) &&
		       ValuesMatch(root, "Orbital/MeanAnomalyAngleAtEpoch", EarthSunMeanAnomaly) &&
		       ValuesMatch(root, "Orbital/AnomalyChangeAnglePerDay", EarthSunAnomalyPerDay) &&
		       ValuesMatch(root, "Orbital/EclipticLongitude", EarthSunEclipticLongitude) &&
		       ValuesMatch(root, "Orbital/EquatorialObliquity", EarthSunObliquity) &&
		       ValuesMatch(root, "Orbital/OrbitalEccentricity", EarthSunOrbitalEccentricity) &&
		       ValuesMatch(root, "Orbital/OrbitalSemiMajorAxis", EarthSunSemiMajorAxis) &&
		       ValuesMatch(root, "Orbital/ApparentAngularRadius", EarthSunApparentAngularRadius) &&
		       ValuesMatch(root, "Orbital/DayNumberAtEpoch", EarthSunDayNumberAtEpoch) &&
		       ValuesMatch(root, "Orbital/SiderealTimePerDay", EarthSunSiderealTimePerDay);
	}

	private IEnumerable<(Celestial Moon, Celestial Planet, Celestial SunView)> GetLegacyEarthMoonTriples(FuturemudDatabaseContext context)
	{
		var celestials = context.Celestials.AsEnumerable().ToList();
		var moons = celestials.Where(IsLegacyEarthMoonCelestial).ToList();
		var planets = celestials.Where(x => x.CelestialType == "PlanetFromMoon").ToList();
		var sunViews = celestials.Where(x => x.CelestialType == "SunFromPlanetaryMoon").ToList();
		foreach (var moon in moons)
		{
			var moonId = moon.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
			var planet = planets.FirstOrDefault(x =>
			{
				var root = TryGetDefinitionRoot(x);
				return root is not null && root.Element("Name")?.Value == EarthPlanetName &&
				       root.Element("Moon")?.Value == moonId &&
				       ValuesMatch(root, "AngularRadius", EarthAngularRadiusFromMoon);
			});
			if (planet is null)
			{
				continue;
			}

			var sunId = TryGetDefinitionRoot(planet)?.Element("Sun")?.Value;
			var sunView = sunViews.FirstOrDefault(x =>
			{
				var root = TryGetDefinitionRoot(x);
				return root is not null && root.Element("Moon")?.Value == moonId && root.Element("Sun")?.Value == sunId;
			});
			if (sunView is not null)
			{
				yield return (moon, planet, sunView);
			}
		}
	}

	private bool IsLegacyEarthMoonCelestial(Celestial celestial)
	{
		if (!celestial.CelestialType.EqualTo("PlanetaryMoon"))
		{
			return false;
		}

		var root = TryGetDefinitionRoot(celestial);
		return root is not null &&
		       ValuesMatch(root, "Orbital/CelestialDaysPerYear", EarthMoonDaysPerYear) &&
		       ValuesMatch(root, "Orbital/MeanAnomalyAngleAtEpoch", EarthMoonMeanAnomaly) &&
		       ValuesMatch(root, "Orbital/AnomalyChangeAnglePerDay", EarthMoonAnomalyPerDay) &&
		       ValuesMatch(root, "Orbital/ArgumentOfPeriapsis", EarthMoonArgumentOfPeriapsis) &&
		       ValuesMatch(root, "Orbital/LongitudeOfAscendingNode", EarthMoonLongitudeOfAscendingNode) &&
		       ValuesMatch(root, "Orbital/OrbitalInclination", EarthMoonOrbitalInclination) &&
		       ValuesMatch(root, "Orbital/OrbitalEccentricity", EarthMoonOrbitalEccentricity) &&
		       ValuesMatch(root, "Orbital/OrbitalSemiMajorAxis", EarthMoonSemiMajorAxis) &&
		       ValuesMatch(root, "Orbital/SiderealTimeAtEpoch", EarthMoonSiderealTimeAtEpoch) &&
		       ValuesMatch(root, "Orbital/SiderealTimePerDay", EarthMoonSiderealTimePerDay);
	}

	private static string GetSunIlluminationXml(Celestial sun)
	{
		return TryGetDefinitionRoot(sun)?.Element("Illumination")?.ToString(SaveOptions.DisableFormatting) ?? string.Empty;
	}

	private static string GetSunName(Celestial sun)
	{
		return TryGetDefinitionRoot(sun)?.Element("Name")?.Value ?? "The Sun";
	}

	private static double GetSunApparentAngularRadius(Celestial sun)
	{
		return TryGetDefinitionRoot(sun)?
			       .Element("Orbital")?
			       .Element("ApparentAngularRadius")?
			       .Value.GetDouble()
		       ?? 0.0;
	}

	private static string? GetPackageName(Celestial celestial)
	{
		return TryGetDefinitionRoot(celestial)?.Element(SeederPackageElement)?.Value;
	}

	private static XElement? TryGetDefinitionRoot(Celestial celestial)
	{
		try
		{
			return string.IsNullOrWhiteSpace(celestial.Definition) ? null : XElement.Parse(celestial.Definition);
		}
		catch
		{
			return null;
		}
	}

	private static bool ValuesMatch(XElement root, string path, double expected, double tolerance = 0.000001)
	{
		var value = GetPathValue(root, path);
		return double.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out var actual) &&
		       Math.Abs(actual - expected) <= tolerance;
	}

	private static string? GetPathValue(XElement root, string path)
	{
		var current = root;
		foreach (var part in path.Split('/'))
		{
			current = current.Element(part);
			if (current is null)
			{
				return null;
			}
		}

		return current.Value;
	}

	private static string Format(double value)
	{
		return value.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
	}

	private static Calendar GetCalendar(FuturemudDatabaseContext context, string answer)
	{
		return TryGetCalendar(context, answer, out var calendar)
			? calendar!
			: throw new InvalidOperationException($"Could not find a calendar matching '{answer}'.");
	}

	private static bool TryGetCalendar(FuturemudDatabaseContext context, string answer, out Calendar? calendar)
	{
		if (long.TryParse(answer, out var id))
		{
			calendar = context.Calendars.FirstOrDefault(x => x.Id == id);
			if (calendar is not null)
			{
				return true;
			}
		}

		calendar = context.Calendars.AsEnumerable().FirstOrDefault(x =>
		{
			if (string.IsNullOrWhiteSpace(x.Definition))
			{
				return false;
			}

			try
			{
				var definition = XElement.Parse(x.Definition);
				return definition.Element("alias")?.Value.EqualTo(answer) == true ||
				       definition.Element("shortname")?.Value.EqualTo(answer) == true ||
				       definition.Element("fullname")?.Value.EqualTo(answer) == true;
			}
			catch
			{
				return false;
			}
		});
		return calendar is not null;
	}
}

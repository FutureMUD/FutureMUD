#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

namespace DatabaseSeeder.Seeders;

public partial class TimeSeeder : IDatabaseSeeder
{
    private static readonly string[] StockCalendarAliases =
    [
        "gregorian",
        "julian",
        "middle-earth",
        "tranquility",
        "republicain",
        "mission",
        "seasonal-360",
        "roman",
        "dwarven",
        "shire-reckoning",
        "stewards-reckoning",
        "new-reckoning",
        "orc-reckoning",
        "rivendell",
        "kings-reckoning",
        "islamic-hijri",
        "hebrew",
        "old-persian",
        "babylonian",
        "chinese-minguo",
        "chinese-lunisolar",
        "korean-dangi",
        "korean-lunisolar",
        "japanese-koki",
        "japanese-lunisolar"
    ];

    public IEnumerable<(string Id, string Question,
        Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
        Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
        new List<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
            Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
        {
            ("secondsmultiplier", @"How many in-game seconds do you want to pass per real second?

When making your decision on this setting you should consider a few 'side effects' of the choice you make. For example, if you choose a ratio that is a factor of 24 (so 1, 2, 3, 4, 6, or 8) the in-game time at a given real time of the day will always be the same. This means that if a person always logged in at the same real time each day it would also be the same in-game time.

This could be disadvantageous if that time was always at night for example, and their character couldn't do things that they needed to do in the daytime. There is a little bit of variation because time in FutureMUD does not run while the MUD is not running, but hopefully your downtime is minimal and so this previous advice should be broadly true.

Another consideration is the realism of the scenario. If you use a rate that is especially high, time goes by so fast that it basically has to be ignored in-game. When a single conversation might take 3 in-game days to play out, it does somewhat ruin the immersion. For this reason, I recommend against ratios much higher than 10.

Personally, I recommend either 2 or 5 as a ratio but you are free to choose what you will, and you can also adjust this setting later without issue if you change your mind.

With all that in mind, what whole number of in-game seconds should be added for every 1 real second that passes? ",
                (context, answers) => true,
                (answer, context) =>
                {
                    if (!uint.TryParse(answer, out uint value) || value <= 0) { return (false, "You must supply a valid positive integer."); } return (true, string.Empty);
                }),
            ("mode",
                @"There are several pre-made calendars that you can choose to use. If you are using a calendar that is not one of the ones listed below, I suggest that you use the latin-ancient calendar and modify the generated file as that calendar makes the most use of advanced features for examples.

Broadly speaking, there are eight calendars for you to choose from:

#AGregorian#F - which is the calendar most of the world uses in the modern era
#AJulian#F - which is very similar to the Gregorian calendar but with different leap year rules
#ARoman#F - which was a calendar used in the Roman republic before the Julian reforms
#AMiddle-Earth#F - various calendars described by J.R.R. Tolkien for Middle-Earth
#ATranquility#F: The 13-month, 28 day Tranquility calendar, commencing at the Moon Landing
#ACalendare Republicain#F: The French Republican calendar (including decimal clock) from the French Revolution
#AMission#F: A sci-fi generation ship calendar with 360 day years, 36 day months and 6 day weeks
#ASeasonal 360#F: A simple 360 day fantasy calendar with three months per season and a 6 day week
#AAstronomical and Historical Approximations#F: deterministic Hijri, Hebrew, Old Persian, Babylonian and East Asian calendar packages

The specific available calendars are as follows:

	#Bgregorian-us#F: Gregorian with US-style dates (e.g. month/day/year)
	#Bgregorian-uk#F: Gregorian with UK-style dates (e.g. day/month/year)
	#Bgregorian-us-ce#F: Gregorian with ""CE"" rather than ""AD"" styling and US-style dates (e.g. month/day/year)
	#Bgregorian-uk-ce#F: Gregorian with ""CE"" rather than ""AD"" styling and UK-style dates (e.g. day/month/year)
	#Bjulian#F: Julian (used from 46BC until the 16th Century)
	#Blatin-7day#F: Julian with Latin day and month names, year from Rome's founding, and a 7 day week
	#Blatin-8day#F: Julian with Latin day and month names, year from Rome's founding, and an 8 day week
	#Blatin-ancient#F: The pre-reform Roman calendar with all names in Latin
	#Bmiddle-earth#F: Includes various middle-earth calendars
	#Btranquility#F: The 13-month, 28 day Tranquility calendar, commencing at the Moon Landing
	#Brepublicain#F: The French Republican calendar (including decimal clock) from the French Revolution
	#Bmission#F: A sci-fi generation ship calendar with 360 day years, 36 day months and 6 day weeks
	#Bseasonal-360#F: A simple 360 day fantasy calendar with Early/Mid/Late seasons and First Day through Sixth Day weekdays
	#Bislamic-hijri#F: Deterministic visible-crescent Hijri approximation with sunset day boundaries
	#Bhebrew#F: Calculated Hebrew calendar with deterministic postponement/leap-month rules
	#Bold-persian#F: Old Persian/Zoroastrian-style solar calendar with epagomenal days
	#Bbabylonian#F: Regulated deterministic Babylonian lunisolar approximation
	#Bchinese-minguo#F: Gregorian-derived Chinese Minguo civil calendar
	#Bchinese-lunisolar#F: Deterministic Chinese lunisolar approximation
	#Bkorean-dangi#F: Gregorian-derived Korean Dangi civil calendar
	#Bkorean-lunisolar#F: Deterministic Korean lunisolar approximation
	#Bjapanese-koki#F: Gregorian-derived Japanese Koki civil calendar
	#Bjapanese-lunisolar#F: Deterministic Japanese lunisolar approximation
", (context, answers) => true, (answer, context) =>
                {
                    switch (answer.ToLowerInvariant())
                    {
                        case "gregorian-us":
                        case "gregorian-uk":
                        case "gregorian-us-ce":
                        case "gregorian-uk-ce":
                        case "julian":
                        case "latin-7day":
                        case "latin-8day":
                        case "latin-ancient":
                        case "middle-earth":
                        case "republicain":
                        case "tranquility":
                        case "mission":
                        case "seasonal-360":
                        case "islamic-hijri":
                        case "hebrew":
                        case "old-persian":
                        case "babylonian":
                        case "chinese-minguo":
                        case "chinese-lunisolar":
                        case "korean-dangi":
                        case "korean-modern":
                        case "korean-lunisolar":
                        case "japanese-koki":
                        case "japanese-modern":
                        case "japanese-lunisolar":
                            return (true, string.Empty);
                    }

                    return (false, "That is not a valid selection.");
                }),
            ("startyear", "What starting year do you want to set up for the calendar? ", (context, answers) => true,
                (answer, context) =>
                {
                    if (!uint.TryParse(answer, out uint value) || value <= 0) { return (false, "You must supply a valid positive integer."); } return (true, string.Empty);
                }),
            ("ardaage", @"Which age do you want your calendars set up for? 

	#B1) First Age
	2) Second Age
	3) Third Age
	4) Fourth Age#F

Your answer: ", (context, answers) => answers["mode"].EqualTo("middle-earth"), (answer, context) =>
            {
                if (!uint.TryParse(answer, out uint value) || value <= 0 || value > 4) { return (false, "You must answer between 1 and 4."); } return (true, string.Empty);
            })
        };

    public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
    {
        context.Database.BeginTransaction();
        Clock clock = EnsureClock(
            context,
            $@"<Clock>  <Alias>UTC</Alias>  <Description>Universal Time Clock</Description>  <ShortDisplayString>$j:$m:$s $i</ShortDisplayString>  <SuperDisplayString>$j:$m:$s $i $t</SuperDisplayString>  <LongDisplayString>$c $i</LongDisplayString>  <SecondsPerMinute>60</SecondsPerMinute>  <MinutesPerHour>60</MinutesPerHour>  <HoursPerDay>24</HoursPerDay>  <InGameSecondsPerRealSecond>{int.Parse(questionAnswers["secondsmultiplier"])}</InGameSecondsPerRealSecond>  <SecondFixedDigits>2</SecondFixedDigits>  <MinuteFixedDigits>2</MinuteFixedDigits>  <HourFixedDigits>0</HourFixedDigits>  <NoZeroHour>true</NoZeroHour>  <NumberOfHourIntervals>2</NumberOfHourIntervals>  <HourIntervalNames>    <HourIntervalName>a.m</HourIntervalName>    <HourIntervalName>p.m</HourIntervalName>  </HourIntervalNames>  <HourIntervalLongNames>    <HourIntervalLongName>in the morning</HourIntervalLongName>    <HourIntervalLongName>in the afternoon</HourIntervalLongName>  </HourIntervalLongNames>  <CrudeTimeIntervals>    <CrudeTimeInterval text=""night"" Lower=""-2"" Upper=""4""/>    <CrudeTimeInterval text=""morning"" Lower=""4"" Upper=""12""/>    <CrudeTimeInterval text=""afternoon"" Lower=""12"" Upper=""18""/>    <CrudeTimeInterval text=""evening"" Lower=""18"" Upper=""22""/>  </CrudeTimeIntervals></Clock>");
        Timezone utc = EnsureTimezone(context, clock, "UTC", "Universal Time Clock (UTC)", 0, 0);
        context.SaveChanges();
        clock.PrimaryTimezoneId = utc.Id;
        string primaryCalendarAlias = ResolvePrimaryCalendarAlias(questionAnswers["mode"]);

        switch (questionAnswers["mode"].ToLowerInvariant())
        {
            case "gregorian-us":
                SetupGregorian(context, true, false, clock, questionAnswers);
                break;
            case "gregorian-uk":
                SetupGregorian(context, false, false, clock, questionAnswers);
                break;
            case "gregorian-us-ce":
                SetupGregorian(context, true, true, clock, questionAnswers);
                break;
            case "gregorian-uk-ce":
                SetupGregorian(context, false, true, clock, questionAnswers);
                break;
            case "julian":
                SetupJulian(context, clock, questionAnswers);
                break;
            case "latin-ancient":
                SetupLatinAncient(context, clock, questionAnswers);
                break;
            case "latin-7day":
                SetupLatin(context, clock, questionAnswers, false);
                break;
            case "latin-8day":
                SetupLatin(context, clock, questionAnswers, true);
                break;
            case "middle-earth":
                SetupMiddleEarth(context, clock, questionAnswers);
                break;
            case "tranquility":
                SetupTranquility(context, clock, questionAnswers);
                break;
            case "mission":
                SetupMissionCalendar(context, clock, questionAnswers);
                break;
            case "seasonal-360":
                SetupSeasonal360Calendar(context, clock, questionAnswers);
                break;
            case "islamic-hijri":
                SetupIslamicHijri(context, clock, questionAnswers);
                break;
            case "hebrew":
                SetupHebrew(context, clock, questionAnswers);
                break;
            case "old-persian":
                SetupOldPersian(context, clock, questionAnswers);
                break;
            case "babylonian":
                SetupBabylonian(context, clock, questionAnswers);
                break;
            case "chinese-minguo":
                SetupChineseMinguo(context, clock, questionAnswers);
                break;
            case "chinese-lunisolar":
                SetupChineseLunisolar(context, clock, questionAnswers);
                break;
            case "korean-dangi":
            case "korean-modern":
                SetupKoreanDangi(context, clock, questionAnswers);
                break;
            case "korean-lunisolar":
                SetupKoreanLunisolar(context, clock, questionAnswers);
                break;
            case "japanese-koki":
            case "japanese-modern":
                SetupJapaneseKoki(context, clock, questionAnswers);
                break;
            case "japanese-lunisolar":
                SetupJapaneseLunisolar(context, clock, questionAnswers);
                break;
            case "republicain":
                clock = EnsureClock(
                    context,
                    $@"<Clock>  
	<Alias>Decimal</Alias>
	<Description>Decimal Clock</Description>
	<ShortDisplayString>$jh$mm$ss</ShortDisplayString>
	<SuperDisplayString>$jh$mm$ss $t</SuperDisplayString>
	<LongDisplayString>$c</LongDisplayString>
	<SecondsPerMinute>100</SecondsPerMinute>
	<MinutesPerHour>100</MinutesPerHour>
	<HoursPerDay>10</HoursPerDay>
	<InGameSecondsPerRealSecond>{int.Parse(questionAnswers["secondsmultiplier"])}</InGameSecondsPerRealSecond>
	<SecondFixedDigits>2</SecondFixedDigits>
	<MinuteFixedDigits>2</MinuteFixedDigits>
	<HourFixedDigits>0</HourFixedDigits>
	<NoZeroHour>true</NoZeroHour>	
	<NumberOfHourIntervals>1</NumberOfHourIntervals>
	<HourIntervalNames>
		<HourIntervalName>o'clock</HourIntervalName>
	</HourIntervalNames>
	<HourIntervalLongNames>
		<HourIntervalLongName>of the clock</HourIntervalLongName>
	</HourIntervalLongNames>
	<CrudeTimeIntervals>
		<CrudeTimeInterval text=""night"" Lower=""-1"" Upper=""2""/>
		<CrudeTimeInterval text=""morning"" Lower=""2"" Upper=""5""/>\
		<CrudeTimeInterval text=""afternoon"" Lower=""5"" Upper=""7""/>
		<CrudeTimeInterval text=""evening"" Lower=""7"" Upper=""9""/>
	</CrudeTimeIntervals>
</Clock>");

                utc = EnsureTimezone(context, clock, "PMT", "Paris Mean Time (PMT)", 0, 0);
                EnsureTimezone(context, clock, "UTC", "Universal Time Clock (UTC)", 0, 0);
                context.SaveChanges();
                clock.PrimaryTimezoneId = utc.Id;
                SetupFrenchRepublican(context, clock, questionAnswers);
                break;
            default:
                throw new InvalidOperationException(@"Invalid selection for ""mode"" in TimeSeeder.");
        }

        context.SaveChanges();

        Calendar primaryCalendar = context.Calendars.Local.FirstOrDefault(x =>
                string.Equals(CalendarAlias(x.Definition), primaryCalendarAlias, StringComparison.OrdinalIgnoreCase)) ??
            context.Calendars
                .AsEnumerable()
                .First(x => string.Equals(CalendarAlias(x.Definition), primaryCalendarAlias, StringComparison.OrdinalIgnoreCase));
        SyncShardAndZoneTimeBindings(context, primaryCalendar, clock, utc);

        context.SaveChanges();
        context.Database.CommitTransaction();

        return "Successfully set up clock and calendar.";
    }

    public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
    {
        if (!context.Accounts.Any())
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        bool hasUtcClock = context.Clocks
            .Select(x => x.Definition)
            .AsEnumerable()
            .Any(x => string.Equals(ClockAlias(x), "UTC", StringComparison.OrdinalIgnoreCase));
        bool hasStockCalendar = context.Calendars
            .Select(x => x.Definition)
            .AsEnumerable()
            .Any(x => StockCalendarAliases.Contains(CalendarAlias(x) ?? string.Empty, StringComparer.OrdinalIgnoreCase));

        return SeederRepeatabilityHelper.ClassifyByPresence(
        [
            hasUtcClock,
            context.Timezones.Any(x => x.Name == "UTC"),
            hasStockCalendar
        ]);
    }

    public int SortOrder => 5;
    public string Name => "Time Seeder";
    public string Tagline => "Sets up Calendars and Clocks";

    public string FullDescription =>
        "This seeder will set up a clock, timezones and a calendar. It is necessary to have at least one calendar before you can make any cultures, which is a pre-requisite for having characters in game. If you want to do a custom calendar that is not listed in the options that are presented to you, I recommend you choose one anyway and then modify the XML yourself. It should be fairly straightforward but feel free to hit me up for any help.";

    private static string ResolvePrimaryCalendarAlias(string mode)
    {
        return mode.Trim().ToLowerInvariant() switch
        {
            "middle-earth" => "eldarin-quenya",
            "mission" => "mission",
            "seasonal-360" => "seasonal-360",
            "republicain" => "republicain",
            "tranquility" => "tranquility",
            "islamic-hijri" => "islamic-hijri",
            "hebrew" => "hebrew",
            "old-persian" => "old-persian",
            "babylonian" => "babylonian",
            "chinese-minguo" => "chinese-minguo",
            "chinese-lunisolar" => "chinese-lunisolar",
            "korean-dangi" => "korean-dangi",
            "korean-modern" => "korean-dangi",
            "korean-lunisolar" => "korean-lunisolar",
            "japanese-koki" => "japanese-koki",
            "japanese-modern" => "japanese-koki",
            "japanese-lunisolar" => "japanese-lunisolar",
            "latin-ancient" => "julian",
            "latin-7day" => "julian",
            "latin-8day" => "julian",
            "julian" => "julian",
            _ => "gregorian"
        };
    }

    private static string? ClockAlias(string definition)
    {
        return TryReadElementValue(definition, "Alias");
    }

    private static string? CalendarAlias(string definition)
    {
        return TryReadElementValue(definition, "alias");
    }

    private static string? TryReadElementValue(string definition, string elementName)
    {
        XElement root = XElement.Parse(definition);
        return root.Descendants()
            .FirstOrDefault(x => x.Name.LocalName.Equals(elementName, StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }

    private static Clock EnsureClock(FuturemudDatabaseContext context, string definition)
    {
        string alias = ClockAlias(definition) ?? throw new InvalidOperationException("Clock definitions must declare an alias.");
        Clock clock = context.Clocks.Local.FirstOrDefault(x =>
                        string.Equals(ClockAlias(x.Definition), alias, StringComparison.OrdinalIgnoreCase)) ??
                    context.Clocks
                        .AsEnumerable()
                        .FirstOrDefault(x => string.Equals(ClockAlias(x.Definition), alias, StringComparison.OrdinalIgnoreCase)) ??
                    CreateClock(context);

        clock.Definition = definition;
        return clock;
    }

    private static Timezone EnsureTimezone(FuturemudDatabaseContext context, Clock clock, string name, string description,
        int offsetHours, int offsetMinutes)
    {
        Timezone timezone = SeederRepeatabilityHelper.EnsureEntity(
            context.Timezones,
            x => x.ClockId == clock.Id && string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase),
            x => x.ClockId == clock.Id,
            () =>
            {
                Timezone created = new();
                context.Timezones.Add(created);
                return created;
            });

        timezone.Name = name;
        timezone.Description = description;
        timezone.Clock = clock;
        timezone.ClockId = clock.Id;
        timezone.OffsetHours = offsetHours;
        timezone.OffsetMinutes = offsetMinutes;
        return timezone;
    }

    private static Calendar EnsureCalendar(FuturemudDatabaseContext context, Clock clock, string date, string definition)
    {
        string alias = CalendarAlias(definition) ??
                    throw new InvalidOperationException("Calendar definitions must declare an alias.");
        Calendar calendar = context.Calendars.Local.FirstOrDefault(x =>
                           string.Equals(CalendarAlias(x.Definition), alias, StringComparison.OrdinalIgnoreCase)) ??
                       context.Calendars
                           .AsEnumerable()
                           .FirstOrDefault(x => string.Equals(CalendarAlias(x.Definition), alias, StringComparison.OrdinalIgnoreCase)) ??
                       CreateCalendar(context);

        calendar.FeedClockId = clock.Id;
        calendar.Date = date;
        calendar.Definition = definition;
        return calendar;
    }

    private static Clock CreateClock(FuturemudDatabaseContext context)
    {
        Clock created = new();
        context.Clocks.Add(created);
        return created;
    }

    private static Calendar CreateCalendar(FuturemudDatabaseContext context)
    {
        Calendar created = new();
        context.Calendars.Add(created);
        return created;
    }

    private static void SyncShardAndZoneTimeBindings(FuturemudDatabaseContext context, Calendar calendar, Clock clock,
        Timezone timezone)
    {
        foreach (Shard? shard in context.Shards.Include(x => x.Zones).ToList())
        {
            SeederRepeatabilityHelper.EnsureLink(
                context.ShardsCalendars,
                x => x.ShardId == shard.Id && x.CalendarId == calendar.Id,
                () => new ShardsCalendars { Shard = shard, CalendarId = calendar.Id });

            SeederRepeatabilityHelper.EnsureLink(
                context.ShardsClocks,
                x => x.ShardId == shard.Id && x.ClockId == clock.Id,
                () => new ShardsClocks { Shard = shard, ClockId = clock.Id });

            foreach (Zone? zone in shard.Zones)
            {
                List<ZonesTimezones> zoneTimezones = context.ZonesTimezones
                    .Where(x => x.ZoneId == zone.Id && x.ClockId == clock.Id)
                    .ToList();
                if (zoneTimezones.Any(x => x.TimezoneId == timezone.Id))
                {
                    context.ZonesTimezones.RemoveRange(zoneTimezones.Where(x => x.TimezoneId != timezone.Id));
                    continue;
                }

                context.ZonesTimezones.RemoveRange(zoneTimezones);
                context.ZonesTimezones.Add(new ZonesTimezones
                {
                    Zone = zone,
                    ClockId = clock.Id,
                    TimezoneId = timezone.Id
                });
            }
        }
    }
}

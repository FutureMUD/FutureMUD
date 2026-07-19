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

public partial class TimeSeeder
{
    private readonly record struct MonthSpec(string Alias, string ShortName, string FullName, int Days);

    private static readonly string[] SevenDayWeek =
    [
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday",
        "Sunday"
    ];

    private static string BuildCalendarDefinition(string alias, string shortName, string fullName, string description,
        string shortMask, string longMask, string wordyMask, string ancientEraShort, string ancientEraLong,
        string modernEraShort, string modernEraLong, int epochYear, int firstWeekday, IEnumerable<string> weekdays,
        IEnumerable<MonthSpec> months, XElement algorithm, XElement dayBoundary)
    {
        return new XElement("calendar",
            new XElement("alias", alias),
            new XElement("shortname", shortName),
            new XElement("fullname", fullName),
            new XElement("description", new XCData(description)),
            new XElement("shortstring", shortMask),
            new XElement("longstring", longMask),
            new XElement("wordystring", wordyMask),
            new XElement("plane", "earth"),
            new XElement("feedclock", 0),
            new XElement("epochyear", epochYear),
            new XElement("weekdayatepoch", firstWeekday),
            new XElement("ancienterashortstring", ancientEraShort),
            new XElement("ancienteralongstring", ancientEraLong),
            new XElement("modernerashortstring", modernEraShort),
            new XElement("moderneralongstring", modernEraLong),
            new XElement("weekdays", weekdays.Select(x => new XElement("weekday", x))),
            new XElement("months", months.Select((x, i) => BuildMonthElement(x, i + 1))),
            new XElement("intercalarymonths"),
            algorithm,
            dayBoundary).ToString(SaveOptions.DisableFormatting);
    }

    private static XElement BuildMonthElement(MonthSpec month, int order)
    {
        return new XElement("month",
            new XElement("alias", month.Alias),
            new XElement("shortname", month.ShortName),
            new XElement("fullname", month.FullName),
            new XElement("nominalorder", order),
            new XElement("normaldays", month.Days),
            new XElement("intercalarydays"),
            new XElement("specialdays"),
            new XElement("nonweekdays"));
    }

    private static XElement Algorithm(string type, string? variant = null)
    {
        var element = new XElement("algorithm", new XAttribute("type", type));
        if (!string.IsNullOrWhiteSpace(variant))
        {
            element.Add(new XAttribute("variant", variant));
        }

        return element;
    }

    private static XElement DayBoundary(string type, bool includeAuthority = false)
    {
        var element = new XElement("dayboundary", new XAttribute("type", type));
        if (includeAuthority)
        {
            element.Add(new XElement("authority",
                new XAttribute("latitude", "0"),
                new XAttribute("longitude", "0"),
                new XAttribute("elevation", "0"),
                new XAttribute("radius", "0")));
        }

        return element;
    }

    private static IReadOnlyList<MonthSpec> GregorianMonths(string prefix = "")
    {
        return
        [
            new($"{prefix}january", "Jan", "January", 31),
            new($"{prefix}february", "Feb", "February", 28),
            new($"{prefix}march", "Mar", "March", 31),
            new($"{prefix}april", "Apr", "April", 30),
            new($"{prefix}may", "May", "May", 31),
            new($"{prefix}june", "Jun", "June", 30),
            new($"{prefix}july", "Jul", "July", 31),
            new($"{prefix}august", "Aug", "August", 31),
            new($"{prefix}september", "Sep", "September", 30),
            new($"{prefix}october", "Oct", "October", 31),
            new($"{prefix}november", "Nov", "November", 30),
            new($"{prefix}december", "Dec", "December", 31)
        ];
    }

    private static string BuildGregorianDerivedCalendar(string alias, string shortName, string fullName,
        string description, string startYear, string ancientShort, string ancientLong, string modernShort,
        string modernLong, IReadOnlyList<MonthSpec> months)
    {
        var root = XElement.Parse(BuildCalendarDefinition(alias, shortName, fullName, description, "$dd/$mo/$yy $ee",
            "$nz$ww the $dt of $mf, $yy $EE", "$NZ$ww on this $DT day of the month of $mf, in year $yy $EE",
            ancientShort, ancientLong, modernShort, modernLong, int.Parse(startYear), 0, SevenDayWeek, months,
            Algorithm("fixed-months"), DayBoundary("ClockMidnight")));
        var february = root.Element("months")!.Elements("month").ElementAt(1);
        february.Element("intercalarydays")!.Add(new XElement("intercalary",
            new XElement("insertdays", 1),
            new XElement("specialdays"),
            new XElement("nonweekdays"),
            new XElement("removenonweekdays"),
            new XElement("removespecialdays"),
            new XElement("intercalaryrule",
                new XElement("offset", 0),
                new XElement("divisor", 4),
                new XElement("exceptions",
                    new XElement("intercalaryrule",
                        new XElement("offset", 0),
                        new XElement("divisor", 100),
                        new XElement("exceptions",
                            new XElement("intercalaryrule",
                                new XElement("offset", 0),
                                new XElement("divisor", 400),
                                new XElement("exceptions"),
                                new XElement("ands"),
                                new XElement("ors"))),
                        new XElement("ands"),
                        new XElement("ors"))),
                new XElement("ands"),
                new XElement("ors"))));
        return root.ToString(SaveOptions.DisableFormatting);
    }

    private void SetupIslamicHijri(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        var months = new[]
        {
            new MonthSpec("muharram", "Muh", "Muharram", 30),
            new MonthSpec("safar", "Saf", "Safar", 29),
            new MonthSpec("rabi-awwal", "RAw", "Rabi al-Awwal", 30),
            new MonthSpec("rabi-thani", "RTh", "Rabi al-Thani", 29),
            new MonthSpec("jumada-ula", "JUl", "Jumada al-Ula", 30),
            new MonthSpec("jumada-akhirah", "JAk", "Jumada al-Akhirah", 29),
            new MonthSpec("rajab", "Raj", "Rajab", 30),
            new MonthSpec("shaban", "Sha", "Shaban", 29),
            new MonthSpec("ramadan", "Ram", "Ramadan", 30),
            new MonthSpec("shawwal", "Shw", "Shawwal", 29),
            new MonthSpec("dhu-qadah", "DQa", "Dhu al-Qadah", 30),
            new MonthSpec("dhu-hijjah", "DHi", "Dhu al-Hijjah", 29)
        };
        var definition = BuildCalendarDefinition("islamic-hijri", "Islamic Hijri Calendar",
            "Deterministic Islamic Hijri Calendar",
            "A deterministic Hijri approximation using mean astronomical lunar month starts and visible-crescent-style sunset day boundaries. It is not a manual observation ledger.",
            "$dd/$mm/$yy AH", "$nz$ww the $dt of $mf, $yy AH", "$NZ$ww on the $DT day of $mf, in year $yy AH",
            "BH", "before Hijrah", "AH", "after Hijrah", int.Parse(questionAnswers["startyear"]), 0, SevenDayWeek,
            months, Algorithm("astronomical-lunar", "visible-crescent-approximation"),
            DayBoundary("SunsetAtAuthorityLocation", true));
        EnsureCalendar(context, clock, $"1/muharram/{questionAnswers["startyear"]}", definition);
    }

    private void SetupHebrew(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        var months = new[]
        {
            new MonthSpec("nisan", "Nis", "Nisan", 30),
            new MonthSpec("iyyar", "Iyy", "Iyyar", 29),
            new MonthSpec("sivan", "Siv", "Sivan", 30),
            new MonthSpec("tammuz", "Tam", "Tammuz", 29),
            new MonthSpec("av", "Av", "Av", 30),
            new MonthSpec("elul", "Elu", "Elul", 29),
            new MonthSpec("tishrei", "Tis", "Tishrei", 30),
            new MonthSpec("heshvan", "Hes", "Heshvan", 29),
            new MonthSpec("kislev", "Kis", "Kislev", 30),
            new MonthSpec("tevet", "Tev", "Tevet", 29),
            new MonthSpec("shevat", "She", "Shevat", 30),
            new MonthSpec("adar", "Ada", "Adar", 29),
            new MonthSpec("adar-i", "Ad1", "Adar I", 30),
            new MonthSpec("adar-ii", "Ad2", "Adar II", 29)
        };
        var definition = BuildCalendarDefinition("hebrew", "Calculated Hebrew Calendar",
            "Calculated Hebrew Calendar",
            "A deterministic calculated Hebrew calendar with a 19-year Metonic cycle, postponement rules and Adar I/II leap-month handling.",
            "$dd/$mm/$yy AM", "$nz$ww the $dt of $mf, $yy AM", "$NZ$ww on the $DT day of $mf, in year $yy AM",
            "BM", "before creation", "AM", "anno mundi", int.Parse(questionAnswers["startyear"]), 0, SevenDayWeek,
            months, Algorithm("calculated-hebrew"), DayBoundary("SunsetAtAuthorityLocation", true));
        EnsureCalendar(context, clock, $"1/nisan/{questionAnswers["startyear"]}", definition);
    }

    private void SetupOldPersian(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        var months = new[]
        {
            new MonthSpec("farvardin", "Far", "Farvardin", 30),
            new MonthSpec("ardwahisht", "Ard", "Ardwahisht", 30),
            new MonthSpec("hordad", "Hor", "Hordad", 30),
            new MonthSpec("tir", "Tir", "Tir", 30),
            new MonthSpec("amurdad", "Amu", "Amurdad", 30),
            new MonthSpec("shahrewar", "Sha", "Shahrewar", 30),
            new MonthSpec("mihr", "Mih", "Mihr", 30),
            new MonthSpec("aban", "Aba", "Aban", 30),
            new MonthSpec("adar", "Ada", "Adar", 30),
            new MonthSpec("dae", "Dae", "Dae", 30),
            new MonthSpec("wahman", "Wah", "Wahman", 30),
            new MonthSpec("spendarmad", "Spe", "Spendarmad", 30),
            new MonthSpec("gatha", "Gat", "Gatha Days", 5)
        };
        var definition = BuildCalendarDefinition("old-persian", "Old Persian Solar Calendar",
            "Old Persian/Zoroastrian-Style Solar Calendar",
            "A deterministic Old Persian/Zoroastrian-style solar approximation using twelve 30-day months plus epagomenal Gatha days. Leap epagomenal days follow a deterministic solar-equinox cycle.",
            "$dd/$mm/$yy", "$nz$ww the $dt of $mf, $yy", "$NZ$ww on the $DT day of $mf, in year $yy",
            "BE", "before era", "AE", "after era", int.Parse(questionAnswers["startyear"]), 0, SevenDayWeek,
            months, Algorithm("solar-equinox", "old-persian-epagomenal"), DayBoundary("ClockMidnight"));
        EnsureCalendar(context, clock, $"1/farvardin/{questionAnswers["startyear"]}", definition);
    }

    private void SetupBabylonian(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        var months = new[]
        {
            new MonthSpec("nisannu", "Nis", "Nisannu", 30),
            new MonthSpec("aiaru", "Aia", "Aiaru", 29),
            new MonthSpec("simanu", "Sim", "Simanu", 30),
            new MonthSpec("duzu", "Duz", "Duzu", 29),
            new MonthSpec("abu", "Abu", "Abu", 30),
            new MonthSpec("ululu", "Ulu", "Ululu", 29),
            new MonthSpec("tashritu", "Tas", "Tashritu", 30),
            new MonthSpec("arahsamnu", "Ara", "Arahsamnu", 29),
            new MonthSpec("kislimu", "Kis", "Kislimu", 30),
            new MonthSpec("tebetu", "Teb", "Tebetu", 29),
            new MonthSpec("shabatu", "Sha", "Shabatu", 30),
            new MonthSpec("addaru", "Add", "Addaru", 29),
            new MonthSpec("addaru-ii", "Ad2", "Addaru II", 29),
            new MonthSpec("ululu-ii", "Ul2", "Ululu II", 29)
        };
        var definition = BuildCalendarDefinition("babylonian", "Babylonian Lunisolar Calendar",
            "Deterministic Babylonian Lunisolar Calendar",
            "A regulated deterministic Babylonian lunisolar approximation with Addaru II and Ululu II leap months in a 19-year cycle. It is not a historical observation ledger.",
            "$dd/$mm/$yy", "$nz$ww the $dt of $mf, $yy", "$NZ$ww on the $DT day of $mf, in year $yy",
            "BE", "before era", "AE", "after era", int.Parse(questionAnswers["startyear"]), 0, SevenDayWeek,
            months, Algorithm("astronomical-lunar", "babylonian-regulated"),
            DayBoundary("SunsetAtAuthorityLocation", true));
        EnsureCalendar(context, clock, $"1/nisannu/{questionAnswers["startyear"]}", definition);
    }

    private void SetupChineseMinguo(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        var months = new[]
        {
            new MonthSpec("yi-yue", "Yi", "Yi Yue", 31),
            new MonthSpec("er-yue", "Er", "Er Yue", 28),
            new MonthSpec("san-yue", "San", "San Yue", 31),
            new MonthSpec("si-yue", "Si", "Si Yue", 30),
            new MonthSpec("wu-yue", "Wu", "Wu Yue", 31),
            new MonthSpec("liu-yue", "Liu", "Liu Yue", 30),
            new MonthSpec("qi-yue", "Qi", "Qi Yue", 31),
            new MonthSpec("ba-yue", "Ba", "Ba Yue", 31),
            new MonthSpec("jiu-yue", "Jiu", "Jiu Yue", 30),
            new MonthSpec("shi-yue", "Shi", "Shi Yue", 31),
            new MonthSpec("shiyi-yue", "Shy", "Shiyi Yue", 30),
            new MonthSpec("shier-yue", "She", "Shier Yue", 31)
        };
        var definition = BuildGregorianDerivedCalendar("chinese-minguo", "Chinese Minguo Calendar",
            "Chinese Minguo Civil Calendar",
            "A Gregorian-derived Chinese Minguo civil calendar package with Latin1-safe month names.",
            questionAnswers["startyear"], "BM", "before Minguo", "Minguo", "Minguo era", months);
        EnsureCalendar(context, clock, $"1/yi-yue/{questionAnswers["startyear"]}", definition);
    }

    private void SetupKoreanDangi(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        var months = new[]
        {
            new MonthSpec("ilwol", "Il", "Ilwol", 31),
            new MonthSpec("iwol", "Iw", "Iwol", 28),
            new MonthSpec("samwol", "Sam", "Samwol", 31),
            new MonthSpec("sawol", "Sa", "Sawol", 30),
            new MonthSpec("owol", "O", "Owol", 31),
            new MonthSpec("yuwol", "Yu", "Yuwol", 30),
            new MonthSpec("chirwol", "Chi", "Chirwol", 31),
            new MonthSpec("parwol", "Par", "Parwol", 31),
            new MonthSpec("guwol", "Gu", "Guwol", 30),
            new MonthSpec("siwol", "Si", "Siwol", 31),
            new MonthSpec("sibirwol", "SbI", "Sibirwol", 30),
            new MonthSpec("sibiwol", "Sb", "Sibiwol", 31)
        };
        var definition = BuildGregorianDerivedCalendar("korean-dangi", "Korean Dangi Calendar",
            "Korean Dangi Civil Calendar",
            "A Gregorian-derived Korean Dangi civil calendar package with Latin1-safe month names.",
            questionAnswers["startyear"], "BD", "before Dangi", "Dangi", "Dangi era", months);
        EnsureCalendar(context, clock, $"1/ilwol/{questionAnswers["startyear"]}", definition);
    }

    private void SetupJapaneseKoki(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        var months = new[]
        {
            new MonthSpec("ichigatsu", "Ich", "Ichigatsu", 31),
            new MonthSpec("nigatsu", "Ni", "Nigatsu", 28),
            new MonthSpec("sangatsu", "San", "Sangatsu", 31),
            new MonthSpec("shigatsu", "Shi", "Shigatsu", 30),
            new MonthSpec("gogatsu", "Go", "Gogatsu", 31),
            new MonthSpec("rokugatsu", "Rok", "Rokugatsu", 30),
            new MonthSpec("shichigatsu", "Shc", "Shichigatsu", 31),
            new MonthSpec("hachigatsu", "Hac", "Hachigatsu", 31),
            new MonthSpec("kugatsu", "Ku", "Kugatsu", 30),
            new MonthSpec("jugatsu", "Ju", "Jugatsu", 31),
            new MonthSpec("juichigatsu", "Jui", "Juichigatsu", 30),
            new MonthSpec("junigatsu", "Jun", "Junigatsu", 31)
        };
        var definition = BuildGregorianDerivedCalendar("japanese-koki", "Japanese Koki Calendar",
            "Japanese Koki Civil Calendar",
            "A Gregorian-derived Japanese Koki civil calendar package with Latin1-safe month names.",
            questionAnswers["startyear"], "BK", "before Koki", "Koki", "Koki era", months);
        EnsureCalendar(context, clock, $"1/ichigatsu/{questionAnswers["startyear"]}", definition);
    }

    private void SetupChineseLunisolar(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        SetupEastAsianLunisolar(context, clock, questionAnswers, "chinese-lunisolar", "Chinese Lunisolar Calendar",
            "Deterministic Chinese Lunisolar Calendar", "chinese-lunisolar",
            [
                new MonthSpec("zhengyue", "Zhe", "Zhengyue", 30),
                new MonthSpec("eryue", "Er", "Eryue", 29),
                new MonthSpec("sanyue", "San", "Sanyue", 30),
                new MonthSpec("siyue", "Si", "Siyue", 29),
                new MonthSpec("wuyue", "Wu", "Wuyue", 30),
                new MonthSpec("liuyue", "Liu", "Liuyue", 29),
                new MonthSpec("qiyue", "Qi", "Qiyue", 30),
                new MonthSpec("bayue", "Ba", "Bayue", 29),
                new MonthSpec("jiuyue", "Jiu", "Jiuyue", 30),
                new MonthSpec("shiyue", "Shi", "Shiyue", 29),
                new MonthSpec("dongyue", "Don", "Dongyue", 30),
                new MonthSpec("layue", "La", "Layue", 29),
                new MonthSpec("leap-liuyue", "Run", "Run Liuyue", 29)
            ]);
    }

    private void SetupKoreanLunisolar(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        SetupEastAsianLunisolar(context, clock, questionAnswers, "korean-lunisolar", "Korean Lunisolar Calendar",
            "Deterministic Korean Lunisolar Calendar", "korean-lunisolar",
            [
                new MonthSpec("jeongwol", "Jeo", "Jeongwol", 30),
                new MonthSpec("iwol", "Iw", "Iwol", 29),
                new MonthSpec("samwol", "Sam", "Samwol", 30),
                new MonthSpec("sawol", "Sa", "Sawol", 29),
                new MonthSpec("owol", "O", "Owol", 30),
                new MonthSpec("yuwol", "Yu", "Yuwol", 29),
                new MonthSpec("chirwol", "Chi", "Chirwol", 30),
                new MonthSpec("parwol", "Par", "Parwol", 29),
                new MonthSpec("guwol", "Gu", "Guwol", 30),
                new MonthSpec("siwol", "Si", "Siwol", 29),
                new MonthSpec("dongjiwol", "Don", "Dongjiwol", 30),
                new MonthSpec("seotdal", "Seo", "Seotdal", 29),
                new MonthSpec("leap-yuwol", "Yun", "Yun Yuwol", 29)
            ]);
    }

    private void SetupJapaneseLunisolar(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        SetupEastAsianLunisolar(context, clock, questionAnswers, "japanese-lunisolar", "Japanese Lunisolar Calendar",
            "Deterministic Japanese Lunisolar Calendar", "japanese-lunisolar",
            [
                new MonthSpec("mutsuki", "Mut", "Mutsuki", 30),
                new MonthSpec("kisaragi", "Kis", "Kisaragi", 29),
                new MonthSpec("yayoi", "Yay", "Yayoi", 30),
                new MonthSpec("uzuki", "Uzu", "Uzuki", 29),
                new MonthSpec("satsuki", "Sat", "Satsuki", 30),
                new MonthSpec("minazuki", "Min", "Minazuki", 29),
                new MonthSpec("fumizuki", "Fum", "Fumizuki", 30),
                new MonthSpec("hazuki", "Haz", "Hazuki", 29),
                new MonthSpec("nagatsuki", "Nag", "Nagatsuki", 30),
                new MonthSpec("kannazuki", "Kan", "Kannazuki", 29),
                new MonthSpec("shimotsuki", "Shi", "Shimotsuki", 30),
                new MonthSpec("shiwasu", "Shw", "Shiwasu", 29),
                new MonthSpec("leap-minazuki", "Uru", "Uru Minazuki", 29)
            ]);
    }

    private void SetupEastAsianLunisolar(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers, string alias, string shortName, string fullName, string variant,
        IReadOnlyList<MonthSpec> months)
    {
        var definition = BuildCalendarDefinition(alias, shortName, fullName,
            "A deterministic East Asian lunisolar approximation using mean astronomical new moons, principal solar-term style leap-month placement and Latin1-safe names.",
            "$dd/$mm/$yy", "$nz$ww the $dt of $mf, $yy", "$NZ$ww on the $DT day of $mf, in year $yy",
            "BE", "before era", "AE", "after era", int.Parse(questionAnswers["startyear"]), 0, SevenDayWeek,
            months, Algorithm("east-asian-lunisolar", variant), DayBoundary("ClockMidnight"));
        EnsureCalendar(context, clock, $"1/{months[0].Alias}/{questionAnswers["startyear"]}", definition);
    }

    private void SetupMissionCalendar(FuturemudDatabaseContext context, Clock clock, IReadOnlyDictionary<string, string> questionAnswers)
    {
        Calendar calendar = new()
        {
            FeedClockId = clock.Id,
            Date = $"01/ignis/{questionAnswers["startyear"]}",
            Definition = @"<calendar>
  <alias>mission</alias>
  <shortname>The Mission Calendar</shortname>
  <fullname>The Mission Calendar</fullname>
  <description><![CDATA[The Mission Calendar is the standard timekeeping system used aboard the generation ship and by its descendants. It begins at the moment of launch, known as Mission Year 0, Day 0, which serves as the epoch for all recorded history and official documentation. Designed to be entirely independent of any planetary cycle, the calendar is a rational, closed system intended to maintain social cohesion, regulate work and rest, and structure the crew’s multigenerational journey.

The Mission Calendar divides each year into ten months of thirty-six days, for a total of 360 days per year. Each month is named for a concept or element central to shipboard life, such as Ignis, Ventus, and Umbra. Months are further divided into sixty six-day weeks, with five days devoted to work and one reserved for rest, reflection, or recreation. This regular rhythm is supported by a controlled, ship-wide 24-hour light cycle to maintain circadian health and consistent scheduling.

Several cultural and operational observances are tied to the Mission Calendar, the most significant of which is Founders’ Day, celebrated on the final day of each year to commemorate the ship’s departure. Other traditions include periodic Quiet Cycles, when nonessential systems are powered down for maintenance and contemplation, and Crew Reassignment Day, a midyear event marking the rotation of shifts, apprenticeships, and training programs. Together, these features make the Mission Calendar both a practical tool and a symbolic reminder of the crew’s shared purpose.]]></description>
  <shortstring>$yy.$mo.$dd</shortstring>
  <longstring>$nz$ww the $dt of $mf, year $yy $EE</longstring>
  <wordystring>$nz$ww the $dt of $mf, year $yy $EE</wordystring>
  <plane>earth</plane>
  <feedclock>0</feedclock>
  <epochyear>0</epochyear>
  <weekdayatepoch>1</weekdayatepoch>
  <ancienterashortstring>b.m</ancienterashortstring>
  <ancienteralongstring>before the mission</ancienteralongstring>
  <modernerashortstring>y.m</modernerashortstring>
  <moderneralongstring>of the mission</moderneralongstring>
  <weekdays>
	<weekday>Axis</weekday>
	<weekday>Core</weekday>
	<weekday>Drift</weekday>
	<weekday>Flux</weekday>
	<weekday>Pulse</weekday>
	<weekday>Rest</weekday>
  </weekdays>
  <months>
	<month>
	  <alias>ignis</alias>
	  <shortname>ign</shortname>
	  <fullname>Ignis</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>36</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>ventus</alias>
	  <shortname>ven</shortname>
	  <fullname>Ventus</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>36</normaldays>
	  <intercalarydays/>
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>aqua</alias>
	  <shortname>aqu</shortname>
	  <fullname>Aqua</fullname>
	  <nominalorder>3</nominalorder>
	  <normaldays>36</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>terra</alias>
	  <shortname>ter</shortname>
	  <fullname>Terra</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>36</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>lux</alias>
	  <shortname>lux</shortname>
	  <fullname>Lux</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>36</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""18"" short=""Reassignment Day"" long=""Reassignment Day"" weekday=""true"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>umbra</alias>
	  <shortname>umb</shortname>
	  <fullname>Umbra</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>36</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>ferrum</alias>
	  <shortname>fer</shortname>
	  <fullname>Ferrum</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>36</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>silica</alias>
	  <shortname>sil</shortname>
	  <fullname>Silica</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>36</normaldays>
	  <specialdays />
	  <intercalarydays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>Vita</alias>
	  <shortname>vit</shortname>
	  <fullname>Vita</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>36</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>nexus</alias>
	  <shortname>nex</shortname>
	  <fullname>Nexus</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>36</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""36"" short=""Founder's Day"" long=""Founder's Day"" weekday=""true"" />
	  </specialdays>
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths/>
</calendar>"
        };
        calendar = EnsureCalendar(context, clock, calendar.Date, calendar.Definition);
        context.SaveChanges();
    }

    private void SetupSeasonal360Calendar(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        Calendar calendar = new()
        {
            FeedClockId = clock.Id,
            Date = $"01/early-winter/{questionAnswers["startyear"]}",
            Definition = @"<calendar>
  <alias>seasonal-360</alias>
  <shortname>Seasonal 360 Calendar</shortname>
  <fullname>The Seasonal 360 Calendar</fullname>
  <description><![CDATA[A simple 360 day calendar for regular fantasy settings. The year is divided into twelve even months of thirty days, with three months each for winter, spring, summer and autumn. Weeks are six days long and simply counted from First Day through Sixth Day, making it straightforward for builders to customise further.]]></description>
  <shortstring>$dd/$mo/$yy</shortstring>
  <longstring>$nz$ww the $dt of $mf, year $yy</longstring>
  <wordystring>$NZ$ww on the $DT day of $mf, in year $yy</wordystring>
  <plane>earth</plane>
  <feedclock>0</feedclock>
  <epochyear>0</epochyear>
  <weekdayatepoch>1</weekdayatepoch>
  <ancienterashortstring>BR</ancienterashortstring>
  <ancienteralongstring>before reckoning</ancienteralongstring>
  <modernerashortstring>AR</modernerashortstring>
  <moderneralongstring>after reckoning</moderneralongstring>
  <weekdays>
	<weekday>First Day</weekday>
	<weekday>Second Day</weekday>
	<weekday>Third Day</weekday>
	<weekday>Fourth Day</weekday>
	<weekday>Fifth Day</weekday>
	<weekday>Sixth Day</weekday>
  </weekdays>
  <months>
	<month>
	  <alias>early-winter</alias>
	  <shortname>ewi</shortname>
	  <fullname>Early Winter</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>mid-winter</alias>
	  <shortname>mwi</shortname>
	  <fullname>Mid Winter</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>late-winter</alias>
	  <shortname>lwi</shortname>
	  <fullname>Late Winter</fullname>
	  <nominalorder>3</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>early-spring</alias>
	  <shortname>esp</shortname>
	  <fullname>Early Spring</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>mid-spring</alias>
	  <shortname>msp</shortname>
	  <fullname>Mid Spring</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>late-spring</alias>
	  <shortname>lsp</shortname>
	  <fullname>Late Spring</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>early-summer</alias>
	  <shortname>esu</shortname>
	  <fullname>Early Summer</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>mid-summer</alias>
	  <shortname>msu</shortname>
	  <fullname>Mid Summer</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>late-summer</alias>
	  <shortname>lsu</shortname>
	  <fullname>Late Summer</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>early-autumn</alias>
	  <shortname>eau</shortname>
	  <fullname>Early Autumn</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>mid-autumn</alias>
	  <shortname>mau</shortname>
	  <fullname>Mid Autumn</fullname>
	  <nominalorder>11</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>late-autumn</alias>
	  <shortname>lau</shortname>
	  <fullname>Late Autumn</fullname>
	  <nominalorder>12</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths />
</calendar>"
        };
        calendar = EnsureCalendar(context, clock, calendar.Date, calendar.Definition);
        context.SaveChanges();
    }

    private void SetupFrenchRepublican(FuturemudDatabaseContext context, Clock clock, IReadOnlyDictionary<string, string> questionAnswers)
    {
        Calendar calendar = new()
        {
            FeedClockId = clock.Id,
            Date = $"01/vendemiaire/{questionAnswers["startyear"]}",
            Definition = @"<calendar>
  <alias>republicain</alias>
  <shortname>The Calendrier Républicain</shortname>
  <fullname>The Calendrier Républicain</fullname>
  <description><![CDATA[The Calendrier Républicain was a calendar created and implemented during the French Revolution and used by the French government for about 12 years from late 1793 to 1805, and for 18 days by the Paris Commune in 1871.

The calendar consisted of twelve 30-day months, each divided into three 10-day cycles similar to weeks, plus five or six intercalary days at the end to fill out the balance of a solar year.

It was designed in part to remove all religious and royalist influences from the calendar, and it was part of a larger attempt at dechristianisation and decimalisation in France (which also included decimal time of day, decimalisation of currency, and metrication)]]></description>
  <shortstring>$dd/$mo/$yy</shortstring>
  <longstring>$nz$ww the $dt of $mf, an $yy $EE</longstring>
  <wordystring>$nz$ww the $dt of $mf, an $yy $EE</wordystring>
  <plane>earth</plane>
  <feedclock>0</feedclock>
  <epochyear>1</epochyear>
  <weekdayatepoch>1</weekdayatepoch>
  <ancienterashortstring>a.l.R</ancienterashortstring>
  <ancienteralongstring>avant la République</ancienteralongstring>
  <modernerashortstring>d.l.R</modernerashortstring>
  <moderneralongstring>de la République</moderneralongstring>
  <weekdays>
	<weekday>Primidi</weekday>
	<weekday>Duodi</weekday>
	<weekday>Tridi</weekday>
	<weekday>Quartidi</weekday>
	<weekday>Quintidi</weekday>
	<weekday>Sextidi</weekday>
	<weekday>Septidi</weekday>
	<weekday>Octidi</weekday>
	<weekday>Nonidi</weekday>
	<weekday>Décadi</weekday>
  </weekdays>
  <months>
	<month>
	  <alias>vendemiaire</alias>
	  <shortname>vend</shortname>
	  <fullname>Vendémiaire</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>brumaire</alias>
	  <shortname>bru</shortname>
	  <fullname>Brumaire</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays/>
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>frimaire</alias>
	  <shortname>fri</shortname>
	  <fullname>Frimaire</fullname>
	  <nominalorder>3</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>nivose</alias>
	  <shortname>niv</shortname>
	  <fullname>Nivôse</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>pluviose</alias>
	  <shortname>plu</shortname>
	  <fullname>Pluviôse</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>ventose</alias>
	  <shortname>vent</shortname>
	  <fullname>Ventôse</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>germinal</alias>
	  <shortname>ger</shortname>
	  <fullname>Germinal</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>floreal</alias>
	  <shortname>flo</shortname>
	  <fullname>Floréal</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>30</normaldays>
	  <specialdays />
	  <intercalarydays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>prairial</alias>
	  <shortname>pra</shortname>
	  <fullname>Prairial</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>messidor</alias>
	  <shortname>mes</shortname>
	  <fullname>Messidor</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>thermidor</alias>
	  <shortname>the</shortname>
	  <fullname>Thermidor</fullname>
	  <nominalorder>11</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>fructidor</alias>
	  <shortname>fru</shortname>
	  <fullname>Fructidor</fullname>
	  <nominalorder>12</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths>
	<intercalarymonth>
	  <position>13</position>
	  <month>
		<alias>complementaires</alias>
		<shortname>comp</shortname>
		<fullname>Les Jours Complémentaires</fullname>
		<nominalorder>13</nominalorder>
		<normaldays>5</normaldays>
		<intercalarydays>
		<intercalaryday>
		  <insertdays>1</insertdays>
		  <nonweekdays />
		  <removenonweekdays />
		  <specialdays/>
		  <removespecialdays />
		  <intercalaryrule>
			<offset>0</offset>
			<divisor>4</divisor>
			<exceptions/>
			<ands />
			<ors />
		  </intercalaryrule>
		</intercalaryday>
	  </intercalarydays>
		<specialdays>
		  <specialday day=""1"" short=""La Fête de la Vertu"" long=""La Fête de la Vertu"" weekday=""true"" />
		  <specialday day=""2"" short=""La Fête du Génie"" long=""La Fête du Génie"" weekday=""true"" />
		  <specialday day=""3"" short=""La Fête du Travail"" long=""La Fête du Travail"" weekday=""true"" />
		  <specialday day=""4"" short=""La Fête de l'Opinion"" long=""La Fête de l'Opinion"" weekday=""true"" />
		  <specialday day=""5"" short=""La Fête des Récompense"" long=""La Fête des Récompense"" weekday=""true"" />
		  <specialday day=""6"" short=""La Fête de la Révolution"" long=""La Fête de la Révolution"" weekday=""true"" />
		</specialdays>
		<nonweekdays>
		  <nonweekday>1</nonweekday>
		</nonweekdays>
	  </month>
	  <intercalaryrule>
		<offset>0</offset>
		<divisor>1</divisor>
		<exceptions/>
		<ands />
		<ors />
	  </intercalaryrule>
	</intercalarymonth>
  </intercalarymonths>
</calendar>"
        };
        calendar = EnsureCalendar(context, clock, calendar.Date, calendar.Definition);
        context.SaveChanges();
    }

    private void SetupTranquility(FuturemudDatabaseContext context, Clock clock, IReadOnlyDictionary<string, string> questionAnswers)
    {
        Calendar calendar = new()
        {
            FeedClockId = clock.Id,
            Date = $"01/archimedes/{questionAnswers["startyear"]}",
            Definition = @"<calendar>
  <alias>tranquility</alias>
  <shortname>The Tranquility Calendar</shortname>
  <fullname>The Tranquility Calendar</fullname>
  <description><![CDATA[The Tranquility Calendar is a calendar that has its roots in science. Its center date in history is called Moon Landing Day. The actual center point in time is the exact moment the word tranquility is mentioned in this somewhat famous quote - Houston, Tranquility Base Here. The Eagle Has Landed. Moon Landing Day has 20 hours, 18 minutes, and 1.2 seconds Before Tranquility and 3 hours, 41 minutes, 58.8 seconds After Tranquility.

The Tranquility Calendar is logically structured so that days of the week do not vary from month to month, and there are no changes in calendar structure based on year (e.g., there is no change from Julian to Gregorian calendars in the past).

The Tranquility Calendar originally appeared in the July 1989 (Mendel 19 A.T.) issue of Omni Magazine in an article written by Jeff Siggins]]></description>
  <shortstring>$dd/$mo/$yy</shortstring>
  <longstring>$nz$ww the $dt of $mf, year $yy $EE</longstring>
  <wordystring>$nz$ww the $dt of $mf, year $yy $EE</wordystring>
  <plane>earth</plane>
  <feedclock>0</feedclock>
  <epochyear>1</epochyear>
  <weekdayatepoch>5</weekdayatepoch>
  <ancienterashortstring>B.T</ancienterashortstring>
  <ancienteralongstring>before Tranquility</ancienteralongstring>
  <modernerashortstring>A.T</modernerashortstring>
  <moderneralongstring>after Tranquility</moderneralongstring>
  <weekdays>
	<weekday>Friday</weekday>
	<weekday>Saturday</weekday>
	<weekday>Sunday</weekday>
	<weekday>Monday</weekday>
	<weekday>Tuesday</weekday>
	<weekday>Wednesday</weekday>
	<weekday>Thursday</weekday>
  </weekdays>
  <months>
	<month>
	  <alias>archimedes</alias>
	  <shortname>arc</shortname>
	  <fullname>Archimedes</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>brahe</alias>
	  <shortname>bra</shortname>
	  <fullname>Brahe</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays/>
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>copernicus</alias>
	  <shortname>cop</shortname>
	  <fullname>Copernicus</fullname>
	  <nominalorder>3</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>darwin</alias>
	  <shortname>dar</shortname>
	  <fullname>Darwin</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>einstein</alias>
	  <shortname>ein</shortname>
	  <fullname>Einstein</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>faraday</alias>
	  <shortname>far</shortname>
	  <fullname>Faraday</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>galileo</alias>
	  <shortname>gal</shortname>
	  <fullname>Galileo</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>hippocrates</alias>
	  <shortname>hip</shortname>
	  <fullname>Hippocrates</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>28</normaldays>
	  <specialdays />
	  <intercalarydays>
		<intercalary>
		  <insertdays>1</insertdays>
		  <specialdays>
			  <specialday day=""29"" short=""Aldrin Day"" long=""Aldrin Day"" weekday=""false""/>
		  </specialdays>
		  <nonweekdays>
			  <nonweekday>29</nonweekday>
			</nonweekdays>
			<removenonweekdays />
			<removespecialdays />
			<intercalaryrule>
			  <offset>-31</offset>
			  <divisor>4</divisor>
			  <exceptions>
				<intercalaryrule>
				  <offset>-31</offset>
				  <divisor>100</divisor>
				  <exceptions>
					<intercalaryrule>
					  <offset>-31</offset>
					  <divisor>400</divisor>
					  <exceptions />
					  <ands />
					  <ors />
					</intercalaryrule>
				  </exceptions>
				  <ands />
				  <ors />
				</intercalaryrule>
			  </exceptions>
			  <ands />
			  <ors />
			</intercalaryrule>
		</intercalary>
	  </intercalarydays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>imhotep</alias>
	  <shortname>imh</shortname>
	  <fullname>Imhotep</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>jung</alias>
	  <shortname>jun</shortname>
	  <fullname>Jung</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>kepler</alias>
	  <shortname>kep</shortname>
	  <fullname>Kepler</fullname>
	  <nominalorder>11</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>lavoisier</alias>
	  <shortname>lav</shortname>
	  <fullname>Lavoisier</fullname>
	  <nominalorder>12</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>mendel</alias>
	  <shortname>men</shortname>
	  <fullname>Mendel</fullname>
	  <nominalorder>13</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays>
	  </intercalarydays>
	  <specialdays />
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths>
	<intercalarymonth>
	  <position>14</position>
	  <month>
		<alias>tranquility</alias>
		<shortname>tra</shortname>
		<fullname>Tranquility</fullname>
		<nominalorder>14</nominalorder>
		<normaldays>1</normaldays>
		<intercalarydays/>
		<specialdays>
		  <specialday day=""1"" short=""Armstrong Day"" long=""Armstrong Day"" weekday=""false"" />
		</specialdays>
		<nonweekdays>
		  <nonweekday>1</nonweekday>
		</nonweekdays>
	  </month>
	  <intercalaryrule>
		<offset>0</offset>
		<divisor>1</divisor>
		<exceptions/>
		<ands />
		<ors />
	  </intercalaryrule>
	</intercalarymonth>
  </intercalarymonths>
</calendar>"
        };
        calendar = EnsureCalendar(context, clock, calendar.Date, calendar.Definition);
        context.SaveChanges();
    }

    private void SetupMiddleEarth(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        string modernShort, modernLong, ancientShort, ancientLong;
        switch (questionAnswers["ardaage"].ToLowerInvariant())
        {
            case "1":
                modernShort = "F.A.";
                modernLong = "First Age";
                ancientShort = "Y.T.";
                ancientLong = "The Years of the Trees";
                break;
            case "2":
                modernShort = "S.A.";
                modernLong = "Second Age";
                ancientShort = "bef.";
                ancientLong = "Before the Second Age";
                break;
            case "3":
                modernShort = "T.A.";
                modernLong = "Third Age";
                ancientShort = "bef.";
                ancientLong = "Before the Third Age";
                break;
            case "4":
                modernShort = "Fo.A.";
                modernLong = "Fourth Age";
                ancientShort = "bef.";
                ancientLong = "Before the Fourth Age";
                break;
            default:
                throw new ApplicationException("Invalid option chosen in SetupMiddleEarth()");
        }

        // Eldarin Quenya
        Calendar calendar = new()
        {
            FeedClockId = clock.Id,
            Date = $"01/yestare/{questionAnswers["startyear"]}",
            Definition = @$"<calendar>
  <alias>eldarin-quenya</alias>
  <shortname>Eldarin Calendar (Quenya)</shortname>
  <fullname>The Eldarin Calendar, in Quenya</fullname>
  <description><![CDATA[The Eldarin Calendar of the Quenya elves, with Quenya month and holiday names]]></description>
  <shortstring>$dd/$mo/$yy</shortstring>
  <longstring>$nz$ww the $dt of $mf, $ee $yy</longstring>
  <wordystring>$NZ$ww on the $DT day of the month of $mf, in the $YO year of the $EE</wordystring>
  <plane>arda</plane>
  <feedclock>0</feedclock>
  <epochyear>0</epochyear>
  <weekdayatepoch>1</weekdayatepoch>
  <ancienterashortstring>{ancientShort}</ancienterashortstring>
  <ancienteralongstring>{ancientLong}</ancienteralongstring>
  <modernerashortstring>{modernShort}</modernerashortstring>
  <moderneralongstring>{modernLong}</moderneralongstring>
  <weekdays>
	<weekday>Elenya</weekday>
	<weekday>Anarya</weekday>
	<weekday>Isilya</weekday>
	<weekday>Aldúya</weekday>
	<weekday>Menelya</weekday>
	<weekday>Valanya</weekday>
  </weekdays>
  <months>
	<month>
	  <alias>yestare</alias>
	  <shortname>yes</shortname>
	  <fullname>Yestarë</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>tuile</alias>
	  <shortname>tui</shortname>
	  <fullname>Tuilë</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>54</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>laire</alias>
	  <shortname>lai</shortname>
	  <fullname>Lairë</fullname>
	  <nominalorder>3</nominalorder>
	  <normaldays>72</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>yav</alias>
	  <shortname>yav</shortname>
	  <fullname>Yávië</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>54</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>enderi</alias>
	  <shortname>end</shortname>
	  <fullname>Enderi</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>3</normaldays>
	  <intercalarydays>
		<intercalary>
		  <insertdays>3</insertdays>
		  <specialdays>
		   </specialdays>
		  <nonweekdays/>
			 <removenonweekdays />
			 <removespecialdays />
			 <intercalaryrule>
			   <offset>0</offset>
			   <divisor>12</divisor>
			   <exceptions>
				 <intercalaryrule>
				   <offset>0</offset>
				   <divisor>144</divisor>
				   <exceptions>
				   </exceptions>
				   <ands />
				   <ors />
				 </intercalaryrule>
			   </exceptions>
			   <ands />
			   <ors />
			 </intercalaryrule>
		</intercalary>
	  </intercalarydays>
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>quelle</alias>
	  <shortname>que</shortname>
	  <fullname>Quellë</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>54</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>hrive</alias>
	  <shortname>hri</shortname>
	  <fullname>Hrívë</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>72</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>coire</alias>
	  <shortname>coi</shortname>
	  <fullname>Coirë</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>54</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>mettare</alias>
	  <shortname>met</shortname>
	  <fullname>Mettarë</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths>
  </intercalarymonths>
</calendar>"
        };
        calendar = EnsureCalendar(context, clock, calendar.Date, calendar.Definition);
        context.SaveChanges();

        // Eldarin Sindarin
        calendar = new Calendar
        {
            FeedClockId = clock.Id,
            Date = $"01/yestare/{questionAnswers["startyear"]}",
            Definition = @$"<calendar>
  <alias>eldarin-sindarin</alias>
  <shortname>Eldarin Calendar (Sindarin)</shortname>
  <fullname>The Eldarin Calendar, in Sindarin</fullname>
  <description><![CDATA[The Eldarin Calendar of the Sindarin elves, with Sindarin month and holiday names]]></description>
  <shortstring>$dd/$mo/$yy</shortstring>
  <longstring>$nz$ww the $dt of $mf, $ee $yy</longstring>
  <wordystring>$NZ$ww on the $DT day of the month of $mf, in the $YO year of the $EE</wordystring>
  <plane>arda</plane>
  <feedclock>0</feedclock>
  <epochyear>0</epochyear>
  <weekdayatepoch>1</weekdayatepoch>
  <ancienterashortstring>{ancientShort}</ancienterashortstring>
  <ancienteralongstring>{ancientLong}</ancienteralongstring>
  <modernerashortstring>{modernShort}</modernerashortstring>
  <moderneralongstring>{modernLong}</moderneralongstring>
  <weekdays>
	<weekday>Orgilion</weekday>
	<weekday>Oranor</weekday>
	<weekday>Orithil</weekday>
	<weekday>Orgaladhad</weekday>
	<weekday>Ormenel</weekday>
	<weekday>Orbelain</weekday>
  </weekdays>
  <months>
	<month>
	  <alias>yestare</alias>
	  <shortname>yes</shortname>
	  <fullname>Yestarë</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>ethuil</alias>
	  <shortname>eth</shortname>
	  <fullname>Ethuil</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>54</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>laer</alias>
	  <shortname>lae</shortname>
	  <fullname>Laer</fullname>
	  <nominalorder>3</nominalorder>
	  <normaldays>72</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>iavas</alias>
	  <shortname>iav</shortname>
	  <fullname>Iavas</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>54</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>enedhin</alias>
	  <shortname>ene</shortname>
	  <fullname>Enedhin</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>3</normaldays>
	  <intercalarydays>
		<intercalary>
		  <insertdays>3</insertdays>
		  <specialdays>
		   </specialdays>
		  <nonweekdays/>
			 <removenonweekdays />
			 <removespecialdays />
			 <intercalaryrule>
			   <offset>0</offset>
			   <divisor>12</divisor>
			   <exceptions>
				 <intercalaryrule>
				   <offset>0</offset>
				   <divisor>144</divisor>
				   <exceptions>
				   </exceptions>
				   <ands />
				   <ors />
				 </intercalaryrule>
			   </exceptions>
			   <ands />
			   <ors />
			 </intercalaryrule>
		</intercalary>
	  </intercalarydays>
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>firith</alias>
	  <shortname>fir</shortname>
	  <fullname>Firith</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>54</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>rhiw</alias>
	  <shortname>rhi</shortname>
	  <fullname>Rhîw</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>72</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>echuir</alias>
	  <shortname>ech</shortname>
	  <fullname>Echuir</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>54</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>penninor</alias>
	  <shortname>pen</shortname>
	  <fullname>Penninor</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths>
  </intercalarymonths>
</calendar>"
        };
        calendar = EnsureCalendar(context, clock, calendar.Date, calendar.Definition);
        context.SaveChanges();

        // King's Reckoning Quenya
        calendar = new Calendar
        {
            FeedClockId = clock.Id,
            Date = $"01/yestare/{questionAnswers["startyear"]}",
            Definition = @$"<calendar>
  <alias>kings-reckoning-quenya</alias>
  <shortname>King's Reckoning Calendar (Quenya)</shortname>
  <fullname>The King's Reckoning Calendar, in Quenya</fullname>
  <description><![CDATA[The King's Reckoning Calendar of the 2nd Age Numenoreans, with Quenya month and holiday names]]></description>
  <shortstring>$dd/$mo/$yy</shortstring>
  <longstring>$nz$ww the $dt of $mf, $ee $yy</longstring>
  <wordystring>$NZ$ww on the $DT day of the month of $mf, in the $YO year of the $EE</wordystring>
  <plane>arda</plane>
  <feedclock>0</feedclock>
  <epochyear>0</epochyear>
  <weekdayatepoch>1</weekdayatepoch>
  <ancienterashortstring>{ancientShort}</ancienterashortstring>
  <ancienteralongstring>{ancientLong}</ancienteralongstring>
  <modernerashortstring>{modernShort}</modernerashortstring>
  <moderneralongstring>{modernLong}</moderneralongstring>
  <weekdays>
	<weekday>Elenya</weekday>
	<weekday>Anarya</weekday>
	<weekday>Isilya</weekday>
	<weekday>Aldëa</weekday>
	<weekday>Menelya</weekday>
	<weekday>Valanya</weekday>
	<weekday>Eärenya</weekday>
  </weekdays>
  <months>
	<month>
	  <alias>yestare</alias>
	  <shortname>yes</shortname>
	  <fullname>Yestarë</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>narvinye</alias>
	  <shortname>nar</shortname>
	  <fullname>Narvinyë</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>nenime</alias>
	  <shortname>nen</shortname>
	  <fullname>Nénimë</fullname>
	  <nominalorder>3</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>sulime</alias>
	  <shortname>sul</shortname>
	  <fullname>Súlimë</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>viresse</alias>
	  <shortname>vir</shortname>
	  <fullname>Víressë</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays>
	  </intercalarydays>
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>lotesse</alias>
	  <shortname>lot</shortname>
	  <fullname>Lótessë</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>narie</alias>
	  <shortname>nar</shortname>
	  <fullname>Nárië</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>Loende</alias>
	  <shortname>loe</shortname>
	  <fullname>Loëndë</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays>
	  <intercalary>
		  <insertdays>1</insertdays>
		  <specialdays>
		   </specialdays>
			<nonweekdays>
			 </nonweekdays>
			 <removenonweekdays />
			 <removespecialdays />
			 <intercalaryrule>
			   <offset>0</offset>
			   <divisor>4</divisor>
			   <exceptions>
				 <intercalaryrule>
				   <offset>0</offset>
				   <divisor>100</divisor>
				   <exceptions>
				   </exceptions>
				   <ands />
				   <ors />
				 </intercalaryrule>
			   </exceptions>
			   <ands />
			   <ors />
			 </intercalaryrule>
		</intercalary>
	  </intercalarydays>
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>cermie</alias>
	  <shortname>cer</shortname>
	  <fullname>Cermië</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>urime</alias>
	  <shortname>uri</shortname>
	  <fullname>Urimë</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>yavannie</alias>
	  <shortname>yav</shortname>
	  <fullname>Yavannië</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>narquelie</alias>
	  <shortname>nar</shortname>
	  <fullname>Narquelië</fullname>
	  <nominalorder>11</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>hisime</alias>
	  <shortname>his</shortname>
	  <fullname>Hísimë</fullname>
	  <nominalorder>12</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>ringare</alias>
	  <shortname>rin</shortname>
	  <fullname>Ringarë</fullname>
	  <nominalorder>13</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>mettare</alias>
	  <shortname>met</shortname>
	  <fullname>Mettarë</fullname>
	  <nominalorder>14</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths>
  </intercalarymonths>
</calendar>"
        };
        calendar = EnsureCalendar(context, clock, calendar.Date, calendar.Definition);
        context.SaveChanges();

        // King's Reckoning Sindarin
        calendar = new Calendar
        {
            FeedClockId = clock.Id,
            Date = $"01/yestare/{questionAnswers["startyear"]}",
            Definition = @$"<calendar>
  <alias>kings-reckoning-sindarin</alias>
  <shortname>King's Reckoning Calendar (Sindarin)</shortname>
  <fullname>The King's Reckoning Calendar, in Sindarin</fullname>
  <description><![CDATA[The King's Reckoning Calendar of the 2nd Age Numenoreans, with Sindarin month and holiday names]]></description>
  <shortstring>$dd/$mo/$yy</shortstring>
  <longstring>$nz$ww the $dt of $mf, $ee $yy</longstring>
  <wordystring>$NZ$ww on the $DT day of the month of $mf, in the $YO year of the $EE</wordystring>
  <plane>arda</plane>
  <feedclock>0</feedclock>
  <epochyear>0</epochyear>
  <weekdayatepoch>1</weekdayatepoch>
  <ancienterashortstring>{ancientShort}</ancienterashortstring>
  <ancienteralongstring>{ancientLong}</ancienteralongstring>
  <modernerashortstring>{modernShort}</modernerashortstring>
  <moderneralongstring>{modernLong}</moderneralongstring>
  <weekdays>
	<weekday>Orgilion</weekday>
	<weekday>Oranor</weekday>
	<weekday>Orithil</weekday>
	<weekday>Orgaladh</weekday>
	<weekday>Ormenel</weekday>
	<weekday>Orbelain</weekday>
	<weekday>Oraearon</weekday>
  </weekdays>
  <months>
	<month>
	  <alias>yestare</alias>
	  <shortname>yes</shortname>
	  <fullname>Yestarë</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>narwain</alias>
	  <shortname>nar</shortname>
	  <fullname>Narwain</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>ninui</alias>
	  <shortname>nin</shortname>
	  <fullname>Nínui</fullname>
	  <nominalorder>3</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>gwaeron</alias>
	  <shortname>gwa</shortname>
	  <fullname>Gwaeron</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>gwirith</alias>
	  <shortname>gwi</shortname>
	  <fullname>Gwirith</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays>
	  </intercalarydays>
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>lothron</alias>
	  <shortname>lot</shortname>
	  <fullname>Lothron</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>norui</alias>
	  <shortname>nor</shortname>
	  <fullname>Nórui</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>enedhin</alias>
	  <shortname>ene</shortname>
	  <fullname>Enedhin</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays>
	  <intercalary>
		  <insertdays>1</insertdays>
		  <specialdays>
		   </specialdays>
			<nonweekdays>
			 </nonweekdays>
			 <removenonweekdays />
			 <removespecialdays />
			 <intercalaryrule>
			   <offset>0</offset>
			   <divisor>4</divisor>
			   <exceptions>
				 <intercalaryrule>
				   <offset>0</offset>
				   <divisor>100</divisor>
				   <exceptions>
				   </exceptions>
				   <ands />
				   <ors />
				 </intercalaryrule>
			   </exceptions>
			   <ands />
			   <ors />
			 </intercalaryrule>
		</intercalary>
	  </intercalarydays>
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>cerveth</alias>
	  <shortname>cer</shortname>
	  <fullname>Cerveth</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>urui</alias>
	  <shortname>uru</shortname>
	  <fullname>Urui</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>ivanneth</alias>
	  <shortname>iva</shortname>
	  <fullname>Ivanneth</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>narbeleth</alias>
	  <shortname>nar</shortname>
	  <fullname>Narbeleth</fullname>
	  <nominalorder>11</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>hithui</alias>
	  <shortname>hit</shortname>
	  <fullname>Hithui</fullname>
	  <nominalorder>12</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>Girithron</alias>
	  <shortname>gir</shortname>
	  <fullname>Girithron</fullname>
	  <nominalorder>13</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>penninor</alias>
	  <shortname>pen</shortname>
	  <fullname>Penninor</fullname>
	  <nominalorder>14</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths>
  </intercalarymonths>
</calendar>"
        };
        calendar = EnsureCalendar(context, clock, calendar.Date, calendar.Definition);
        context.SaveChanges();

        // Shire Reckoning
        calendar = new Calendar
        {
            FeedClockId = clock.Id,
            Date = $"01/firstyul/{questionAnswers["startyear"]}",
            Definition = @$"<calendar>
  <alias>shire-reckoning</alias>
  <shortname>Shire Reckoning Calendar</shortname>
  <fullname>The Shire Reckoning Calendar</fullname>
  <description><![CDATA[The Shire Reckoning Calendar of the inhabitants of the Shire and Bree]]></description>
  <shortstring>$dd/$mo/$yy</shortstring>
  <longstring>$nz$ww the $dt of $mf, $ee $yy</longstring>
  <wordystring>$NZ$ww on the $DT day of the month of $mf, in the $YO year of the $EE</wordystring>
  <plane>arda</plane>
  <feedclock>0</feedclock>
  <epochyear>0</epochyear>
  <weekdayatepoch>1</weekdayatepoch>
  <ancienterashortstring>{ancientShort}</ancienterashortstring>
  <ancienteralongstring>{ancientLong}</ancienteralongstring>
  <modernerashortstring>{modernShort}</modernerashortstring>
  <moderneralongstring>{modernLong}</moderneralongstring>
  <weekdays>
	<weekday>Sterday</weekday>
	<weekday>Sunday</weekday>
	<weekday>Monday</weekday>
	<weekday>Trewsday</weekday>
	<weekday>Hensday</weekday>
	<weekday>Mersday</weekday>
	<weekday>Highday</weekday>
  </weekdays>
  <months>
	<month>
	  <alias>firstyul</alias>
	  <shortname>yul1</shortname>
	  <fullname>First Yule</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>afteryule</alias>
	  <shortname>ayu</shortname>
	  <fullname>Afteryule</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>solmath</alias>
	  <shortname>sol</shortname>
	  <fullname>Solmath</fullname>
	  <nominalorder>3</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>rethe</alias>
	  <shortname>ret</shortname>
	  <fullname>Rethe</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>astron</alias>
	  <shortname>ast</shortname>
	  <fullname>Astron</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays>
	  </intercalarydays>
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>thrimidge</alias>
	  <shortname>thr</shortname>
	  <fullname>Thrimidge</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>forelithe</alias>
	  <shortname>for</shortname>
	  <fullname>Forelithe</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>lithe</alias>
	  <shortname>lit</shortname>
	  <fullname>Lithe</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>3</normaldays>
	  <intercalarydays>
	  <intercalary>
		  <insertdays>1</insertdays>
		  <specialdays>
			<specialday day=""3"" short=""Overlithe"" long=""Overlithe"" />
		   </specialdays>
			<nonweekdays>
			 </nonweekdays>
			 <removenonweekdays />
			 <removespecialdays />
			 <intercalaryrule>
			   <offset>0</offset>
			   <divisor>4</divisor>
			   <exceptions>
				 <intercalaryrule>
				   <offset>0</offset>
				   <divisor>100</divisor>
				   <exceptions>
				   </exceptions>
				   <ands />
				   <ors />
				 </intercalaryrule>
			   </exceptions>
			   <ands />
			   <ors />
			 </intercalaryrule>
		</intercalary>
	  </intercalarydays>
	  <specialdays>
		<specialday day=""2"" short=""Midyear's Day"" long=""Midyear's Day"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>afterlithe</alias>
	  <shortname>aft</shortname>
	  <fullname>Afterlithe</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>wedmath</alias>
	  <shortname>wed</shortname>
	  <fullname>Wedmath</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>halimath</alias>
	  <shortname>hal</shortname>
	  <fullname>Halimath</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>winterfilth</alias>
	  <shortname>win</shortname>
	  <fullname>Winterfilth</fullname>
	  <nominalorder>11</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>blotmath</alias>
	  <shortname>blo</shortname>
	  <fullname>Blotmath</fullname>
	  <nominalorder>12</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>foreyule</alias>
	  <shortname>fry</shortname>
	  <fullname>Foreyule</fullname>
	  <nominalorder>13</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>lastyule</alias>
	  <shortname>yul2</shortname>
	  <fullname>Last Yule</fullname>
	  <nominalorder>14</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths>
  </intercalarymonths>
</calendar>"
        };
        calendar = EnsureCalendar(context, clock, calendar.Date, calendar.Definition);
        context.SaveChanges();

        // Steward's Reckoning Quenya
        calendar = new Calendar
        {
            FeedClockId = clock.Id,
            Date = $"01/yestare/{questionAnswers["startyear"]}",
            Definition = @$"<calendar>
  <alias>stewards-reckoning-quenya</alias>
  <shortname>Steward's Reckoning Calendar (Quenya)</shortname>
  <fullname>The Steward's Reckoning Calendar, in Quenya</fullname>
  <description><![CDATA[The Steward's Reckoning Calendar of the 3rd Age Gondorians, with Quenya month and holiday names]]></description>
  <shortstring>$dd/$mo/$yy</shortstring>
  <longstring>$nz$ww the $dt of $mf, $ee $yy</longstring>
  <wordystring>$NZ$ww on the $DT day of the month of $mf, in the $YO year of the $EE</wordystring>
  <plane>arda</plane>
  <feedclock>0</feedclock>
  <epochyear>0</epochyear>
  <weekdayatepoch>1</weekdayatepoch>
  <ancienterashortstring>{ancientShort}</ancienterashortstring>
  <ancienteralongstring>{ancientLong}</ancienteralongstring>
  <modernerashortstring>{modernShort}</modernerashortstring>
  <moderneralongstring>{modernLong}</moderneralongstring>
  <weekdays>
	<weekday>Elenya</weekday>
	<weekday>Anarya</weekday>
	<weekday>Isilya</weekday>
	<weekday>Aldëa</weekday>
	<weekday>Menelya</weekday>
	<weekday>Valanya</weekday>
	<weekday>Eärenya</weekday>
  </weekdays>
  <months>
	<month>
	  <alias>yestare</alias>
	  <shortname>yes</shortname>
	  <fullname>Yestarë</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>narvinye</alias>
	  <shortname>nar</shortname>
	  <fullname>Narvinyë</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>nenime</alias>
	  <shortname>nen</shortname>
	  <fullname>Nénimë</fullname>
	  <nominalorder>3</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>sulime</alias>
	  <shortname>sul</shortname>
	  <fullname>Súlimë</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>tuilere</alias>
	  <shortname>tul</shortname>
	  <fullname>Tuilérë</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Spring Day"" long=""Spring Day"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>viresse</alias>
	  <shortname>vir</shortname>
	  <fullname>Víressë</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays>
	  </intercalarydays>
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>lotesse</alias>
	  <shortname>lot</shortname>
	  <fullname>Lótessë</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>narie</alias>
	  <shortname>nar</shortname>
	  <fullname>Nárië</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>Loende</alias>
	  <shortname>loe</shortname>
	  <fullname>Loëndë</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays>
	  <intercalary>
		  <insertdays>1</insertdays>
		  <specialdays>
		   </specialdays>
			<nonweekdays>
			 </nonweekdays>
			 <removenonweekdays />
			 <removespecialdays />
			 <intercalaryrule>
			   <offset>0</offset>
			   <divisor>4</divisor>
			   <exceptions>
				 <intercalaryrule>
				   <offset>0</offset>
				   <divisor>100</divisor>
				   <exceptions>
				   </exceptions>
				   <ands />
				   <ors />
				 </intercalaryrule>
			   </exceptions>
			   <ands />
			   <ors />
			 </intercalaryrule>
		</intercalary>
	  </intercalarydays>
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>cermie</alias>
	  <shortname>cer</shortname>
	  <fullname>Cermië</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>urime</alias>
	  <shortname>uri</shortname>
	  <fullname>Urimë</fullname>
	  <nominalorder>11</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>yavannie</alias>
	  <shortname>yav</shortname>
	  <fullname>Yavannië</fullname>
	  <nominalorder>12</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>yaviere</alias>
	  <shortname>yve</shortname>
	  <fullname>Yáviérë</fullname>
	  <nominalorder>13</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Harvest Day"" long=""Harvest Day"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>narquelie</alias>
	  <shortname>nar</shortname>
	  <fullname>Narquelië</fullname>
	  <nominalorder>14</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>hisime</alias>
	  <shortname>his</shortname>
	  <fullname>Hísimë</fullname>
	  <nominalorder>15</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>ringare</alias>
	  <shortname>rin</shortname>
	  <fullname>Ringarë</fullname>
	  <nominalorder>16</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>mettare</alias>
	  <shortname>met</shortname>
	  <fullname>Mettarë</fullname>
	  <nominalorder>17</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths>
  </intercalarymonths>
</calendar>"
        };
        calendar = EnsureCalendar(context, clock, calendar.Date, calendar.Definition);
        context.SaveChanges();

        // Steward's Reckoning Sindarin
        calendar = new Calendar
        {
            FeedClockId = clock.Id,
            Date = $"01/yestare/{questionAnswers["startyear"]}",
            Definition = @$"<calendar>
  <alias>stewards-reckoning-sindarin</alias>
  <shortname>Stewards's Reckoning Calendar (Sindarin)</shortname>
  <fullname>The Stewards's Reckoning Calendar, in Sindarin</fullname>
  <description><![CDATA[The Stewards's Reckoning Calendar of the 3rd Age Gondorians, with Sindarin month and holiday names]]></description>
  <shortstring>$dd/$mo/$yy</shortstring>
  <longstring>$nz$ww the $dt of $mf, $ee $yy</longstring>
  <wordystring>$NZ$ww on the $DT day of the month of $mf, in the $YO year of the $EE</wordystring>
  <plane>arda</plane>
  <feedclock>0</feedclock>
  <epochyear>0</epochyear>
  <weekdayatepoch>1</weekdayatepoch>
  <ancienterashortstring>{ancientShort}</ancienterashortstring>
  <ancienteralongstring>{ancientLong}</ancienteralongstring>
  <modernerashortstring>{modernShort}</modernerashortstring>
  <moderneralongstring>{modernLong}</moderneralongstring>
  <weekdays>
	<weekday>Orgilion</weekday>
	<weekday>Oranor</weekday>
	<weekday>Orithil</weekday>
	<weekday>Orgaladh</weekday>
	<weekday>Ormenel</weekday>
	<weekday>Orbelain</weekday>
	<weekday>Oraearon</weekday>
  </weekdays>
  <months>
	<month>
	  <alias>yestare</alias>
	  <shortname>yes</shortname>
	  <fullname>Yestarë</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>narwain</alias>
	  <shortname>nar</shortname>
	  <fullname>Narwain</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>ninui</alias>
	  <shortname>nin</shortname>
	  <fullname>Nínui</fullname>
	  <nominalorder>3</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>gwaeron</alias>
	  <shortname>gwa</shortname>
	  <fullname>Gwaeron</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>tuilere</alias>
	  <shortname>tui</shortname>
	  <fullname>Tuilérë</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Spring Day"" long=""Spring Day"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>gwirith</alias>
	  <shortname>gwi</shortname>
	  <fullname>Gwirith</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays>
	  </intercalarydays>
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>lothron</alias>
	  <shortname>lot</shortname>
	  <fullname>Lothron</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>norui</alias>
	  <shortname>nor</shortname>
	  <fullname>Nórui</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>enedhin</alias>
	  <shortname>ene</shortname>
	  <fullname>Enedhin</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays>
	  <intercalary>
		  <insertdays>1</insertdays>
		  <specialdays>
		   </specialdays>
			<nonweekdays>
			 </nonweekdays>
			 <removenonweekdays />
			 <removespecialdays />
			 <intercalaryrule>
			   <offset>0</offset>
			   <divisor>4</divisor>
			   <exceptions>
				 <intercalaryrule>
				   <offset>0</offset>
				   <divisor>100</divisor>
				   <exceptions>
				   </exceptions>
				   <ands />
				   <ors />
				 </intercalaryrule>
			   </exceptions>
			   <ands />
			   <ors />
			 </intercalaryrule>
		</intercalary>
	  </intercalarydays>
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>cerveth</alias>
	  <shortname>cer</shortname>
	  <fullname>Cerveth</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>urui</alias>
	  <shortname>uru</shortname>
	  <fullname>Urui</fullname>
	  <nominalorder>11</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>ivanneth</alias>
	  <shortname>iva</shortname>
	  <fullname>Ivanneth</fullname>
	  <nominalorder>12</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>yaviere</alias>
	  <shortname>yav</shortname>
	  <fullname>Yáviérë</fullname>
	  <nominalorder>13</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Harvest Day"" long=""Harvest Day"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>narbeleth</alias>
	  <shortname>nar</shortname>
	  <fullname>Narbeleth</fullname>
	  <nominalorder>14</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>hithui</alias>
	  <shortname>hit</shortname>
	  <fullname>Hithui</fullname>
	  <nominalorder>15</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>Girithron</alias>
	  <shortname>gir</shortname>
	  <fullname>Girithron</fullname>
	  <nominalorder>16</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>penninor</alias>
	  <shortname>pen</shortname>
	  <fullname>Penninor</fullname>
	  <nominalorder>17</nominalorder>
	  <normaldays>1</normaldays>
	  <intercalarydays />
	  <specialdays>
	  </specialdays>
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths>
  </intercalarymonths>
</calendar>"
        };
        calendar = EnsureCalendar(context, clock, calendar.Date, calendar.Definition);
        context.SaveChanges();
    }

    private void SetupLatinAncient(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        Calendar calendar = new()
        {
            FeedClockId = clock.Id,
            Date = $"01/ianuarius/{questionAnswers["startyear"]}",
            Definition = @"<calendar>
  <alias>julian</alias>
  <shortname>Julian Calendar</shortname>
  <fullname>The Julian Calendar, in Latin</fullname>
  <description><![CDATA[The calendar created by Julius Caesar with original Latin terminology.]]></description>
  <shortstring>$dd/$mo/$yy</shortstring>
  <longstring>$nz$ww the $dt of $mf, $yy AUC</longstring>
  <wordystring>$NZ$ww on the $DT day of the month of $mf, in the $YO year since the founding of Rome</wordystring>
  <plane>earth</plane>
  <feedclock>0</feedclock>
  <epochyear>750</epochyear>
  <weekdayatepoch>4</weekdayatepoch>
  <ancienterashortstring>PUC</ancienterashortstring>
  <ancienteralongstring>year before the founding of Rome</ancienteralongstring>
  <modernerashortstring>AUC</modernerashortstring>
  <moderneralongstring>year since the founding of Rome</moderneralongstring>
  <weekdays>
	<weekday>Nundinarum A</weekday>
	<weekday>Nundinarum B</weekday>
	<weekday>Nundinarum C</weekday>
	<weekday>Nundinarum D</weekday>
	<weekday>Nundinarum E</weekday>
	<weekday>Nundinarum F</weekday>
	<weekday>Nundinarum G</weekday>
	<weekday>Nundinarum H</weekday>
  </weekdays>
  <months>
	<month>
	  <alias>ianuarius</alias>
	  <shortname>ian</shortname>
	  <fullname>Ianuarius</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>29</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Ianuarius"" />
		<specialday day=""5"" short=""Nones"" long=""Nones of Ianuarius"" />
		<specialday day=""13"" short=""Ides"" long=""Ides of Ianuarius"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>februarius</alias>
	  <shortname>feb</shortname>
	  <fullname>Februarius</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>23</normaldays>
	  <intercalarydays>
		<intercalaryday>
		  <insertdays>5</insertdays>
		  <nonweekdays />
		  <removenonweekdays />
		  <specialdays/>
		  <removespecialdays />
		  <intercalaryrule>
			<offset>0</offset>
			<divisor>1</divisor>
			<exceptions>
				<intercalaryrule>
				  <offset>0</offset>
				  <divisor>2</divisor>
				  <exceptions>
					  <intercalaryrule>
					  <offset>0</offset>
					  <divisor>24</divisor>
					  <exceptions>
					  </exceptions>
					  <ands />
					  <ors />
					</intercalaryrule>
				  </exceptions>
				  <ands />
				  <ors />
				</intercalaryrule>
			</exceptions>
			<ands />
			<ors />
		  </intercalaryrule>
		</intercalaryday>
	  </intercalarydays>
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Februarius"" />
		<specialday day=""5"" short=""Nones"" long=""Nones of Februarius"" />
		<specialday day=""13"" short=""Ides"" long=""Ides of Februarius"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>martius</alias>
	  <shortname>mar</shortname>
	  <fullname>Martius</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Martius"" />
		<specialday day=""7"" short=""Nones"" long=""Nones of Martius"" />
		<specialday day=""15"" short=""Ides"" long=""Ides of Martius"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>aprilis</alias>
	  <shortname>apr</shortname>
	  <fullname>Aprilis</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>29</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Aprilis"" />
		<specialday day=""7"" short=""Nones"" long=""Nones of Aprilis"" />
		<specialday day=""15"" short=""Ides"" long=""Ides of Aprilis"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>maius</alias>
	  <shortname>mai</shortname>
	  <fullname>Maius</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Maius"" />
		<specialday day=""7"" short=""Nones"" long=""Nones of Maius"" />
		<specialday day=""15"" short=""Ides"" long=""Ides of Maius"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>iunius</alias>
	  <shortname>iun</shortname>
	  <fullname>Iunius</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>29</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Iunius"" />
		<specialday day=""5"" short=""Nones"" long=""Nones of Iunius"" />
		<specialday day=""13"" short=""Ides"" long=""Ides of Iunius"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>quintilis</alias>
	  <shortname>qui</shortname>
	  <fullname>Quintilis</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Quintilis"" />
		<specialday day=""7"" short=""Nones"" long=""Nones of Quintilis"" />
		<specialday day=""15"" short=""Ides"" long=""Ides of Quintilis"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>sextilis</alias>
	  <shortname>sex</shortname>
	  <fullname>Sextilis</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>29</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Sextilis"" />
		<specialday day=""5"" short=""Nones"" long=""Nones of Sextilis"" />
		<specialday day=""13"" short=""Ides"" long=""Ides of Sextilis"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>september</alias>
	  <shortname>sep</shortname>
	  <fullname>September</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>29</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of September"" />
		<specialday day=""5"" short=""Nones"" long=""Nones of September"" />
		<specialday day=""13"" short=""Ides"" long=""Ides of September"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>october</alias>
	  <shortname>oct</shortname>
	  <fullname>October</fullname>
	  <nominalorder>11</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of October"" />
		<specialday day=""7"" short=""Nones"" long=""Nones of October"" />
		<specialday day=""15"" short=""Ides"" long=""Ides of October"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>november</alias>
	  <shortname>nov</shortname>
	  <fullname>November</fullname>
	  <nominalorder>12</nominalorder>
	  <normaldays>29</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of November"" />
		<specialday day=""5"" short=""Nones"" long=""Nones of November"" />
		<specialday day=""13"" short=""Ides"" long=""Ides of November"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>december</alias>
	  <shortname>dec</shortname>
	  <fullname>December</fullname>
	  <nominalorder>13</nominalorder>
	  <normaldays>29</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of December"" />
		<specialday day=""5"" short=""Nones"" long=""Nones of December"" />
		<specialday day=""13"" short=""Ides"" long=""Ides of December"" />
	  </specialdays>
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths>
	<intercalarymonth>
		<position>martus</position>
		<intercalaryrule>
		  <offset>0</offset>
		  <divisor>2</divisor>
		  <exceptions>
			  <intercalaryrule>
			  <offset>0</offset>
			  <divisor>24</divisor>
			  <exceptions>
			  </exceptions>
			  <ands />
			  <ors />
			</intercalaryrule>
		  </exceptions>
		  <ands />
		  <ors />
		</intercalaryrule>
		<month>
		  <alias>mercedonius</alias>
		  <shortname>mer</shortname>
		  <fullname>Mercedonius</fullname>
		  <nominalorder>3</nominalorder>
		  <normaldays>27</normaldays>
		  <intercalarydays>
			<intercalary>
			  <insertdays>1</insertdays>
			  <nonweekdays />
			  <removenonweekdays />
			  <specialdays/>
			  <removespecialdays />
				<intercalaryrule>
				  <offset>0</offset>
				  <divisor>4</divisor>
				  <exceptions>
				  </exceptions>
				  <ands />
				  <ors />
				 </intercalaryrule>
			   </intercalary>
		  </intercalarydays>
		  <specialdays>
			<specialday day=""1"" short=""Kalends"" long=""Kalends of Mercedonius"" />
			<specialday day=""5"" short=""Nones"" long=""Nones of Mercedonius"" />
			<specialday day=""13"" short=""Ides"" long=""Ides of Mercedonius"" />
		  </specialdays>
		  <nonweekdays />
		</month>
	</intercalarymonth>
  </intercalarymonths>
</calendar>"
        };
        calendar = EnsureCalendar(context, clock, calendar.Date, calendar.Definition);
        context.SaveChanges();
    }

    private void SetupLatin(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers, bool useNundinae)
    {
        Calendar calendar = new()
        {
            FeedClockId = clock.Id,
            Date = $"01/ianuarius/{questionAnswers["startyear"]}",
            Definition = @$"<calendar>
  <alias>julian</alias>
  <shortname>Julian Calendar</shortname>
  <fullname>The Julian Calendar, in Latin</fullname>
  <description><![CDATA[The calendar created by Julius Caesar with original Latin terminology.]]></description>
  <shortstring>$dd/$mo/$yy</shortstring>
  <longstring>$nz$ww the $dt of $mf, $yy AUC</longstring>
  <wordystring>$NZ$ww on the $DT day of the month of $mf, in the $YO year since the founding of Rome</wordystring>
  <plane>earth</plane>
  <feedclock>0</feedclock>
  <epochyear>750</epochyear>
  <weekdayatepoch>4</weekdayatepoch>
  <ancienterashortstring>PUC</ancienterashortstring>
  <ancienteralongstring>year before the founding of Rome</ancienteralongstring>
  <modernerashortstring>AUC</modernerashortstring>
  <moderneralongstring>year since the founding of Rome</moderneralongstring>
  <weekdays>
	{(useNundinae ? "<weekday>Nundinarum A</weekday>\n    <weekday>Nundinarum B</weekday>\n    <weekday>Nundinarum C</weekday>\n    <weekday>Nundinarum D</weekday>\n    <weekday>Nundinarum E</weekday>\n    <weekday>Nundinarum F</weekday>\n    <weekday>Nundinarum G</weekday>\n    <weekday>Nundinarum H</weekday>" : "<weekday>Dies Saturni</weekday>\n    <weekday> Dies Solis</weekday>\n    <weekday>Dies Lunae</weekday>\n    <weekday>Dies Martis</weekday>\n    <weekday>Dies Mercurii</weekday>\n    <weekday>Dies Jovis</weekday>\n    <weekday>Dies Veneris</weekday>")}
  </weekdays>
  <months>
	<month>
	  <alias>ianuarius</alias>
	  <shortname>ian</shortname>
	  <fullname>Ianuarius</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Ianuarius"" />
		<specialday day=""5"" short=""Nones"" long=""Nones of Ianuarius"" />
		<specialday day=""13"" short=""Ides"" long=""Ides of Ianuarius"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>februarius</alias>
	  <shortname>feb</shortname>
	  <fullname>Februarius</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays>
		<intercalaryday>
		  <insertdays>1</insertdays>
		  <nonweekdays />
		  <removenonweekdays />
		  <specialdays/>
		  <removespecialdays />
		  <intercalaryrule>
			<offset>0</offset>
			<divisor>4</divisor>
			<exceptions />
			<ands />
			<ors />
		  </intercalaryrule>
		</intercalaryday>
	  </intercalarydays>
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Februarius"" />
		<specialday day=""5"" short=""Nones"" long=""Nones of Februarius"" />
		<specialday day=""13"" short=""Ides"" long=""Ides of Februarius"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>martius</alias>
	  <shortname>mar</shortname>
	  <fullname>Martius</fullname>
	  <nominalorder>3</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Martius"" />
		<specialday day=""7"" short=""Nones"" long=""Nones of Martius"" />
		<specialday day=""15"" short=""Ides"" long=""Ides of Martius"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>aprilis</alias>
	  <shortname>apr</shortname>
	  <fullname>Aprilis</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Aprilis"" />
		<specialday day=""7"" short=""Nones"" long=""Nones of Aprilis"" />
		<specialday day=""15"" short=""Ides"" long=""Ides of Aprilis"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>maius</alias>
	  <shortname>mai</shortname>
	  <fullname>Maius</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Maius"" />
		<specialday day=""7"" short=""Nones"" long=""Nones of Maius"" />
		<specialday day=""15"" short=""Ides"" long=""Ides of Maius"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>iunius</alias>
	  <shortname>iun</shortname>
	  <fullname>Iunius</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Iunius"" />
		<specialday day=""5"" short=""Nones"" long=""Nones of Iunius"" />
		<specialday day=""13"" short=""Ides"" long=""Ides of Iunius"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>iulius</alias>
	  <shortname>iul</shortname>
	  <fullname>Iulius</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Iulius"" />
		<specialday day=""7"" short=""Nones"" long=""Nones of Iulius"" />
		<specialday day=""15"" short=""Ides"" long=""Ides of Iulius"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>augustus</alias>
	  <shortname>aug</shortname>
	  <fullname>Augustus</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of Augustus"" />
		<specialday day=""5"" short=""Nones"" long=""Nones of Augustus"" />
		<specialday day=""13"" short=""Ides"" long=""Ides of Augustus"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>september</alias>
	  <shortname>sep</shortname>
	  <fullname>September</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of September"" />
		<specialday day=""5"" short=""Nones"" long=""Nones of September"" />
		<specialday day=""13"" short=""Ides"" long=""Ides of September"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>october</alias>
	  <shortname>oct</shortname>
	  <fullname>October</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of October"" />
		<specialday day=""7"" short=""Nones"" long=""Nones of October"" />
		<specialday day=""15"" short=""Ides"" long=""Ides of October"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>november</alias>
	  <shortname>nov</shortname>
	  <fullname>November</fullname>
	  <nominalorder>11</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of November"" />
		<specialday day=""5"" short=""Nones"" long=""Nones of November"" />
		<specialday day=""13"" short=""Ides"" long=""Ides of November"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>december</alias>
	  <shortname>dec</shortname>
	  <fullname>December</fullname>
	  <nominalorder>12</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""1"" short=""Kalends"" long=""Kalends of December"" />
		<specialday day=""5"" short=""Nones"" long=""Nones of December"" />
		<specialday day=""13"" short=""Ides"" long=""Ides of December"" />
	  </specialdays>
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths />
</calendar>"
        };
        calendar = EnsureCalendar(context, clock, calendar.Date, calendar.Definition);
        context.SaveChanges();
    }

    private void SetupJulian(FuturemudDatabaseContext context, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        Calendar calendar = new()
        {
            FeedClockId = clock.Id,
            Date = $"01/january/{questionAnswers["startyear"]}",
            Definition = @"<calendar>
  <alias>julian</alias>
  <shortname>Julian Calendar</shortname>
  <fullname>The Julian Calendar, in English</fullname>
  <description><![CDATA[The calendar created by Julius Caesar and in use until replaced by the Gregorian. English Names version.]]></description>
  <shortstring>$dd/$mo/$yy</shortstring>
  <longstring>$nz$ww the $dt of $mf, $yy A.D</longstring>
  <wordystring>$NZ$ww on the $DT day of the month of $mf, in the $YO year of our Lord</wordystring>
  <plane>earth</plane>
  <feedclock>0</feedclock>
  <epochyear>2010</epochyear>
  <weekdayatepoch>4</weekdayatepoch>
  <ancienterashortstring>BC</ancienterashortstring>
  <ancienteralongstring>before Christ</ancienteralongstring>
  <modernerashortstring>AD</modernerashortstring>
  <moderneralongstring>year of our Lord</moderneralongstring>
  <weekdays>
	<weekday>Monday</weekday>
	<weekday>Tuesday</weekday>
	<weekday>Wednesday</weekday>
	<weekday>Thursday</weekday>
	<weekday>Friday</weekday>
	<weekday>Saturday</weekday>
	<weekday>Sunday</weekday>
  </weekdays>
  <months>
	<month>
	  <alias>january</alias>
	  <shortname>jan</shortname>
	  <fullname>January</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
	   <specialday day=""1"" short=""New Years Day"" long=""New Years Day"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>february</alias>
	  <shortname>feb</shortname>
	  <fullname>February</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays>
		<intercalaryday>
		  <insertdays>1</insertdays>
		  <nonweekdays />
		  <removenonweekdays />
		  <specialdays/>
		  <removespecialdays />
		  <intercalaryrule>
			<offset>0</offset>
			<divisor>4</divisor>
			<exceptions />
			<ands />
			<ors />
		  </intercalaryrule>
		</intercalaryday>
	  </intercalarydays>
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>march</alias>
	  <shortname>mar</shortname>
	  <fullname>March</fullname>
	  <nominalorder>3</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>april</alias>
	  <shortname>apr</shortname>
	  <fullname>April</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>may</alias>
	  <shortname>may</shortname>
	  <fullname>May</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>june</alias>
	  <shortname>jun</shortname>
	  <fullname>June</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>july</alias>
	  <shortname>jul</shortname>
	  <fullname>July</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>august</alias>
	  <shortname>aug</shortname>
	  <fullname>August</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>september</alias>
	  <shortname>sep</shortname>
	  <fullname>September</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>october</alias>
	  <shortname>oct</shortname>
	  <fullname>October</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>november</alias>
	  <shortname>nov</shortname>
	  <fullname>November</fullname>
	  <nominalorder>11</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>december</alias>
	  <shortname>dec</shortname>
	  <fullname>December</fullname>
	  <nominalorder>12</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""31"" short=""New Years Eve"" long=""New Years Eve"" />
	  </specialdays>
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths />
</calendar>"
        };
        calendar = EnsureCalendar(context, clock, calendar.Date, calendar.Definition);
        context.SaveChanges();
    }

    private void SetupGregorian(FuturemudDatabaseContext context, bool useImperial, bool useCE, Clock clock,
        IReadOnlyDictionary<string, string> questionAnswers)
    {
        Calendar calendar = new()
        {
            FeedClockId = clock.Id,
            Date = useImperial
                ? $"january/01/{questionAnswers["startyear"]}"
                : $"01/january/{questionAnswers["startyear"]}",
            Definition = @$"<calendar>
  <alias>gregorian</alias>
  <shortname>Gregorian Calendar (EN-{(useImperial ? "US" : "UK")})</shortname>
  <fullname>The Gregorian Calendar, in English with {(useImperial ? "British" : "American")} Date Display</fullname>
  <description><![CDATA[The calendar created by pope Gregory to replace the Julian calendar. English edition.]]></description>
  <shortstring>{(useImperial ? "$mo/$dd/$yy" : "$dd/$mo/$yy")}</shortstring>
  <longstring>$nz$ww {(useImperial ? "$mf $dt" : "the $dt of $mf")}, $yy {(useCE ? "C.E" : "A.D")}</longstring>
  <wordystring>$NZ$ww on the $DT day of the month of $mf, in the $YO year of {(useCE ? "the Common Era" : "our Lord")}</wordystring>
  <plane>earth</plane>
  <feedclock>0</feedclock>
  <epochyear>2010</epochyear>
  <weekdayatepoch>4</weekdayatepoch>
  <ancienterashortstring>{(useCE ? "BCE" : "BC")}</ancienterashortstring>
  <ancienteralongstring>{(useCE ? "before the Common Era" : "before Christ")}</ancienteralongstring>
  <modernerashortstring>{(useCE ? "CE" : "AD")}</modernerashortstring>
  <moderneralongstring>{(useCE ? "year of the Common Era" : "year of our Lord")}</moderneralongstring>
  <weekdays>
	<weekday>Monday</weekday>
	<weekday>Tuesday</weekday>
	<weekday>Wednesday</weekday>
	<weekday>Thursday</weekday>
	<weekday>Friday</weekday>
	<weekday>Saturday</weekday>
	<weekday>Sunday</weekday>
  </weekdays>
  <months>
	<month>
	  <alias>january</alias>
	  <shortname>jan</shortname>
	  <fullname>January</fullname>
	  <nominalorder>1</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
	   <specialday day=""1"" short=""New Years Day"" long=""New Years Day"" />
	  </specialdays>
	  <nonweekdays />
	</month>
	<month>
	  <alias>february</alias>
	  <shortname>feb</shortname>
	  <fullname>February</fullname>
	  <nominalorder>2</nominalorder>
	  <normaldays>28</normaldays>
	  <intercalarydays>
		<intercalaryday>
		  <insertdays>1</insertdays>
		  <nonweekdays />
		  <removenonweekdays />
		  <specialdays/>
		  <removespecialdays />
		  <intercalaryrule>
			<offset>0</offset>
			<divisor>4</divisor>
			<exceptions>
			  <intercalaryrule>
				<offset>0</offset>
				<divisor>100</divisor>
				<exceptions>
				  <intercalaryrule>
					<offset>0</offset>
					<divisor>400</divisor>
					<exceptions />
					<ands />
					<ors />
				  </intercalaryrule>
				</exceptions>
				<ands />
				<ors />
			  </intercalaryrule>
			</exceptions>
			<ands />
			<ors />
		  </intercalaryrule>
		</intercalaryday>
	  </intercalarydays>
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>march</alias>
	  <shortname>mar</shortname>
	  <fullname>March</fullname>
	  <nominalorder>3</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>april</alias>
	  <shortname>apr</shortname>
	  <fullname>April</fullname>
	  <nominalorder>4</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>may</alias>
	  <shortname>may</shortname>
	  <fullname>May</fullname>
	  <nominalorder>5</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>june</alias>
	  <shortname>jun</shortname>
	  <fullname>June</fullname>
	  <nominalorder>6</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>july</alias>
	  <shortname>jul</shortname>
	  <fullname>July</fullname>
	  <nominalorder>7</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>august</alias>
	  <shortname>aug</shortname>
	  <fullname>August</fullname>
	  <nominalorder>8</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>september</alias>
	  <shortname>sep</shortname>
	  <fullname>September</fullname>
	  <nominalorder>9</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>october</alias>
	  <shortname>oct</shortname>
	  <fullname>October</fullname>
	  <nominalorder>10</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>november</alias>
	  <shortname>nov</shortname>
	  <fullname>November</fullname>
	  <nominalorder>11</nominalorder>
	  <normaldays>30</normaldays>
	  <intercalarydays />
	  <specialdays />
	  <nonweekdays />
	</month>
	<month>
	  <alias>december</alias>
	  <shortname>dec</shortname>
	  <fullname>December</fullname>
	  <nominalorder>12</nominalorder>
	  <normaldays>31</normaldays>
	  <intercalarydays />
	  <specialdays>
		<specialday day=""25"" short=""Christmas"" long=""Christmas Day"" />
		<specialday day=""26"" short=""Boxing Day"" long=""Boxing Day"" />
		<specialday day=""31"" short=""New Years Eve"" long=""New Years Eve"" />
	  </specialdays>
	  <nonweekdays />
	</month>
  </months>
  <intercalarymonths />
</calendar>"
        };
        calendar = EnsureCalendar(context, clock, calendar.Date, calendar.Definition);
        context.SaveChanges();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public class TimeSeeder : IDatabaseSeeder
{
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
					if (!uint.TryParse(answer, out var value) || value <= 0)
						return (false, "You must supply a valid positive integer.");
					return (true, string.Empty);
				}),
			("mode",
				@"There are several pre-made calendars that you can choose to use. If you are using a calendar that is not one of the ones listed below, I suggest that you use the latin-ancient calendar and modify the generated file as that calendar makes the most use of advanced features for examples.

Broadly speaking, there are three calendars for you to choose from:

#AGregorian#F - which is the calendar most of the world uses in the modern era
#AJulian#F - which is very similar to the Gregorian calendar but with different leap year rules
#ARoman#F - which was a calendar used in the Roman republic before the Julian reforms
#AMiddle-Earth#F - various calendars described by J.R.R. Tolkien for Middle-Earth

The available calendars are as follows:

    #Bgregorian-us#F: Gregorian with US-style dates (e.g. month/day/year)
    #Bgregorian-uk#F: Gregorian with UK-style dates (e.g. day/month/year)
    #Bgregorian-us-ce#F: Gregorian with ""CE"" rather than ""AD"" styling and US-style dates (e.g. month/day/year)
    #Bgregorian-uk-ce#F: Gregorian with ""CE"" rather than ""AD"" styling and UK-style dates (e.g. day/month/year)
    #Bjulian#F: Julian (used from 46BC until the 16th Century)
    #Blatin-7day#F: Julian with Latin day and month names, year from Rome's founding, and a 7 day week
    #Blatin-8day#F: Julian with Latin day and month names, year from Rome's founding, and an 8 day week
    #Blatin-ancient#F: The pre-reform Roman calendar with all names in Latin
    #Bmiddle-earth#F: Includes various middle-earth calendars
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
							return (true, string.Empty);
					}

					return (false, "That is not a valid selection.");
				}),
			("startyear", "What starting year do you want to set up for the calendar? ", (context, answers) => true,
				(answer, context) =>
				{
					if (!uint.TryParse(answer, out var value) || value <= 0)
						return (false, "You must supply a valid positive integer.");
					return (true, string.Empty);
				}),
			("ardaage", @"Which age do you want your calendars set up for? 

    #B1) First Age
    2) Second Age
    3) Third Age
    4) Fourth Age#F

Your answer: ", (context, answers) => answers["mode"].EqualTo("middle-earth"), (answer, context) =>
			{
				if (!uint.TryParse(answer, out var value) || value <= 0 || value > 4)
					return (false, "You must answer between 1 and 4.");
				return (true, string.Empty);
			})
		};

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		context.Database.BeginTransaction();
		var clock = new Clock
		{
			Definition =
				@$"<Clock>  <Alias>UTC</Alias>  <Description>Universal Time Clock</Description>  <ShortDisplayString>$j:$m:$s $i</ShortDisplayString>  <SuperDisplayString>$j:$m:$s $i $t</SuperDisplayString>  <LongDisplayString>$c $i</LongDisplayString>  <SecondsPerMinute>60</SecondsPerMinute>  <MinutesPerHour>60</MinutesPerHour>  <HoursPerDay>24</HoursPerDay>  <InGameSecondsPerRealSecond>{int.Parse(questionAnswers["secondsmultiplier"])}</InGameSecondsPerRealSecond>  <SecondFixedDigits>2</SecondFixedDigits>  <MinuteFixedDigits>2</MinuteFixedDigits>  <HourFixedDigits>0</HourFixedDigits>  <NoZeroHour>true</NoZeroHour>  <NumberOfHourIntervals>2</NumberOfHourIntervals>  <HourIntervalNames>    <HourIntervalName>a.m</HourIntervalName>    <HourIntervalName>p.m</HourIntervalName>  </HourIntervalNames>  <HourIntervalLongNames>    <HourIntervalLongName>in the morning</HourIntervalLongName>    <HourIntervalLongName>in the afternoon</HourIntervalLongName>  </HourIntervalLongNames>  <CrudeTimeIntervals>    <CrudeTimeInterval text=""night"" Lower=""-2"" Upper=""4""/>    <CrudeTimeInterval text=""morning"" Lower=""4"" Upper=""12""/>    <CrudeTimeInterval text=""afternoon"" Lower=""12"" Upper=""18""/>    <CrudeTimeInterval text=""evening"" Lower=""18"" Upper=""22""/>  </CrudeTimeIntervals></Clock>",
			Seconds = 0,
			Minutes = 0,
			Hours = 0
		};
		context.Clocks.Add(clock);
		context.SaveChanges();

		var utc = new Timezone
		{
			Name = "UTC",
			Description = "Universal Time Clock (UTC)",
			Clock = clock,
			OffsetHours = 0,
			OffsetMinutes = 0
		};
		context.Timezones.Add(utc);
		context.SaveChanges();
		clock.PrimaryTimezoneId = utc.Id;

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
			default:
				throw new InvalidOperationException(@"Invalid selection for ""mode"" in TimeSeeder.");
		}

		context.SaveChanges();

		foreach (var shard in context.Shards.Include(x => x.Zones).ToList())
		{
			if (!context.ShardsCalendars.Any(x => x.ShardId == shard.Id))
				context.ShardsCalendars.Add(new ShardsCalendars
					{ Shard = shard, CalendarId = context.Calendars.First().Id });

			if (!context.ShardsClocks.Any(x => x.ShardId == shard.Id))
			{
				context.ShardsClocks.Add(new ShardsClocks { Shard = shard, ClockId = context.Clocks.First().Id });
				foreach (var zone in shard.Zones)
					if (!context.ZonesTimezones.Any(x => x.ZoneId == zone.Id))
						context.ZonesTimezones.Add(new ZonesTimezones
						{
							Zone = zone, ClockId = context.Clocks.First().Id,
							TimezoneId = context.Timezones.First(x => x.ClockId == context.Clocks.First().Id).Id
						});
			}
		}

		context.SaveChanges();
		context.Database.CommitTransaction();

		return "Successfully set up clock and calendar.";
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any()) return ShouldSeedResult.PrerequisitesNotMet;

		if (context.Clocks.Any()) return ShouldSeedResult.MayAlreadyBeInstalled;

		return ShouldSeedResult.ReadyToInstall;
	}

	public int SortOrder => 5;
	public string Name => "Time Seeder";
	public string Tagline => "Sets up Calendars and Clocks";

	public string FullDescription =>
		"This seeder will set up a clock, timezones and a calendar. It is necessary to have at least one calendar before you can make any cultures, which is a pre-requisite for having characters in game. If you want to do a custom calendar that is not listed in the options that are presented to you, I recommend you choose one anyway and then modify the XML yourself. It should be fairly straightforward but feel free to hit me up for any help.";

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
		var calendar = new Calendar
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
		context.Calendars.Add(calendar);
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
		context.Calendars.Add(calendar);
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
		context.Calendars.Add(calendar);
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
		context.Calendars.Add(calendar);
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
		context.Calendars.Add(calendar);
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
		context.Calendars.Add(calendar);
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
		context.Calendars.Add(calendar);
		context.SaveChanges();
	}

	private void SetupLatinAncient(FuturemudDatabaseContext context, Clock clock,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		var calendar = new Calendar
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
    <weekday>Nûndinârum A</weekday>
    <weekday>Nûndinârum B</weekday>
    <weekday>Nûndinârum C</weekday>
    <weekday>Nûndinârum D</weekday>
    <weekday>Nûndinârum E</weekday>
    <weekday>Nûndinârum F</weekday>
    <weekday>Nûndinârum G</weekday>
    <weekday>Nûndinârum H</weekday>
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
		context.Calendars.Add(calendar);
		context.SaveChanges();
	}

	private void SetupLatin(FuturemudDatabaseContext context, Clock clock,
		IReadOnlyDictionary<string, string> questionAnswers, bool useNundinae)
	{
		var calendar = new Calendar
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
    {(useNundinae ? "<weekday>Nûndinârum A</weekday>\n    <weekday>Nûndinârum B</weekday>\n    <weekday>Nûndinârum C</weekday>\n    <weekday>Nûndinârum D</weekday>\n    <weekday>Nûndinârum E</weekday>\n    <weekday>Nûndinârum F</weekday>\n    <weekday>Nûndinârum G</weekday>\n    <weekday>Nûndinârum H</weekday>" : "<weekday>Dies Saturni</weekday>\n    <weekday> Dies Solis</weekday>\n    <weekday>Dies Lunae</weekday>\n    <weekday>Dies Martis</weekday>\n    <weekday>Dies Mercurii</weekday>\n    <weekday>Dies Jovis</weekday>\n    <weekday>Dies Veneris</weekday>")}
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
		context.Calendars.Add(calendar);
		context.SaveChanges();
	}

	private void SetupJulian(FuturemudDatabaseContext context, Clock clock,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		var calendar = new Calendar
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
		context.Calendars.Add(calendar);
		context.SaveChanges();
	}

	private void SetupGregorian(FuturemudDatabaseContext context, bool useImperial, bool useCE, Clock clock,
		IReadOnlyDictionary<string, string> questionAnswers)
	{
		var calendar = new Calendar
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
		context.Calendars.Add(calendar);
		context.SaveChanges();
	}
}
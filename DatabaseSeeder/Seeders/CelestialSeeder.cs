using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public class CelestialSeeder : IDatabaseSeeder
{
	private static readonly Regex DateValidationRegex = new(@"\d+(-|/|\s)\w+(-|/|\s)\d+", RegexOptions.IgnoreCase);

	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
		new List<(string Id, string Question, Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool>
			Filter, Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
		{
			("installsun",
				@"Do you want to install a Sun? This option assumes that your world is based on a planet orbiting around a single sun. The specific sun installed will basically be Earth's sun Sol.

Please answer #3yes#F or #3no#F: ", (context, answers) => true,
				(answer, context) =>
				{
					if (!answer.EqualToAny("yes", "y", "no", "n")) return (false, "Please choose yes or no.");

					return (true, string.Empty);
				}),
			("suncalendar",
				@"Which calendar would you like your sun to be tied to, for the purposes of working out where it should be in the sky?

Please specify a calendar name or ID (if not known, use 1 for ID): ",
				(context, answers) => answers["installsun"].EqualToAny("y", "yes"), (answer, context) =>
				{
					if (long.TryParse(answer, out var id))
						if (context.Calendars.Any(x => x.Id == id))
							return (true, string.Empty);

					return (false,
						$"You must choose from calendars with IDs in the following list: {context.Calendars.Select(x => x.Id.ToString("N0")).ListToCommaSeparatedValues(", ")}");
				}),
			("sunname",
				@"What name would you like to give to your sun? For example, for the Earth's sun you would enter 'The Sun' or 'Sol'.

Your choice: ", (context, answers) => answers["installsun"].EqualToAny("y", "yes"),
				(answer, context) => { return (true, string.Empty); }),
			("sunepoch", @"You must now enter a valid date for the 'epoch' of your celestial. 

Generally you'll want this to be whatever date is equivalent to the 1st of January in your calendar (so the start of the year about 10 days after the summer solstice), and the same year that your game's current date is.

For example, if you were using the Gregorian calendar your epoch date might be #301-jan-2000#f
If you chose the Latin calendars, you might want to do #301-ianuarius-500#f instead.
If you chose the Middle-Earth calendars, you almost certainly want to use #301-yestare-YEAR#f.

What epoch date do you want to use?", (context, answers) => answers["installsun"].EqualToAny("y", "yes"),
                                (answer, context) =>
                                {
                                        if (!DateValidationRegex.IsMatch(answer))
                                                return (false,
                                                        "The date you supplied is definitely not a valid date. Please refer to the guidance above.");
                                        return (true, string.Empty);
                                }),
                        ("installmoon",
                                @"Do you want to install Earth's Moon as a planetary moon?",
                                (context, answers) => true,
                                (answer, context) =>
                                {
                                        if (!answer.EqualToAny("yes", "y", "no", "n")) return (false, "Please choose yes or no.");
                                        return (true, string.Empty);
                                }),
                        ("mooncalendar",
                                @"Which calendar should the moon be tied to?",
                                (context, answers) => answers["installmoon"].EqualToAny("y", "yes"),
                                (answer, context) =>
                                {
                                        if (long.TryParse(answer, out var id))
                                                if (context.Calendars.Any(x => x.Id == id))
                                                        return (true, string.Empty);
                                        return (false,
                                                $"You must choose from calendars with IDs in the following list: {context.Calendars.Select(x => x.Id.ToString("N0")).ListToCommaSeparatedValues(", ")}");
                                }),
                        ("moonname",
                                @"What name would you like to give to your moon?",
                                (context, answers) => answers["installmoon"].EqualToAny("y", "yes"),
                                (answer, context) => (true, string.Empty)),
                        ("moonepoch",
                                @"What epoch date should be used for the moon?",
                                (context, answers) => answers["installmoon"].EqualToAny("y", "yes"),
                                (answer, context) =>
                                {
                                        if (!DateValidationRegex.IsMatch(answer))
                                                return (false, "The date you supplied is definitely not a valid date. Please refer to the guidance above.");
                                        return (true, string.Empty);
                                })
                };

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
                context.Database.BeginTransaction();
                if (questionAnswers["installsun"].EqualToAny("yes", "y")) SetupSun(context, questionAnswers);
                if (questionAnswers["installmoon"].EqualToAny("yes", "y")) SetupMoon(context, questionAnswers);
                context.SaveChanges();
		context.Database.CommitTransaction();

		return "Successfully set up celestials.";
	}

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
	{
		if (!context.Accounts.Any()) return ShouldSeedResult.PrerequisitesNotMet;

		if (context.Celestials.Any()) return ShouldSeedResult.MayAlreadyBeInstalled;

		return ShouldSeedResult.ReadyToInstall;
	}

	public int SortOrder => 6;
	public string Name => "Celestial Seeder";
	public string Tagline => "Sets up Suns, Moons, etc";

	public string FullDescription =>
		"This seeder will set up celestial objects such as suns, moons, parent planets etc. Options will be expanded as they are added to the engine. You will probably want to go into the XML for the generated objects and edit some parameters.";

	private void SetupSun(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		var sunName = questionAnswers["sunname"];
		var sunCalendarId = long.Parse(questionAnswers["suncalendar"]);
		var epoch = questionAnswers["sunepoch"];
		var calendar = context.Calendars.First(x => x.Id == sunCalendarId);

		context.Celestials.Add(new Celestial
		{
			CelestialType = "Sun",
			CelestialYear = 0,
			LastYearBump = 0,
			Minutes = 0,
			FeedClockId = calendar.FeedClockId,
			Definition = @$"<SunV2>
    <Name>{sunName}</Name>
     <Calendar>{sunCalendarId}</Calendar>
     <Orbital>
         <CelestialDaysPerYear>365.24</CelestialDaysPerYear>
         <MeanAnomalyAngleAtEpoch>6.24006</MeanAnomalyAngleAtEpoch>
         <AnomalyChangeAnglePerDay>0.017202</AnomalyChangeAnglePerDay>
         <EclipticLongitude>1.796595</EclipticLongitude>
         <EquatorialObliquity>0.409093</EquatorialObliquity>
         <DayNumberAtEpoch>2451545</DayNumberAtEpoch>
         <SiderealTimeAtEpoch>4.889488</SiderealTimeAtEpoch>
         <SiderealTimePerDay>6.300388</SiderealTimePerDay>
         <KepplerC1Approximant>0.033419565</KepplerC1Approximant>
         <KepplerC2Approximant>0.000349066</KepplerC2Approximant>
         <KepplerC3Approximant>0.000005235988</KepplerC3Approximant>
         <KepplerC4Approximant>0</KepplerC4Approximant>
         <KepplerC5Approximant>0</KepplerC5Approximant>
         <KepplerC6Approximant>0</KepplerC6Approximant>
         <EpochDate>{epoch}</EpochDate>
     </Orbital>
 	<Illumination> 
 		<PeakIllumination>98000</PeakIllumination> 
 		<AlphaScatteringConstant>0.05</AlphaScatteringConstant>
 		<BetaScatteringConstant>0.035</BetaScatteringConstant> 
 		<PlanetaryRadius>6378</PlanetaryRadius>
 		<AtmosphericDensityScalingFactor>6.35</AtmosphericDensityScalingFactor>
 	</Illumination>
    <Triggers>
      <Trigger angle=""-0.015184364492350668"" direction=""Ascending"" ><![CDATA[The edge of the sun rises over the horizon as dawn breaks.]]></Trigger>
      <Trigger angle=""-0.015184364492350668"" direction=""Descending"" ><![CDATA[The sun says its goodbyes for the day and sets on the horizon]]></Trigger>
      <Trigger angle=""-0.20943951023931953"" direction=""Ascending"" ><![CDATA[The first faint traces of light begin to dim the eastern sky as dawn approaches.]]></Trigger>
      <Trigger angle=""-0.20943951023931953"" direction=""Descending"" ><![CDATA[The last traces of light leave the western sky, and the night begins.]]></Trigger>
      <Trigger angle=""-0.10471975511965977"" direction=""Ascending"" ><![CDATA[The eastern sky begins to come alive with colour and light as dawn approaches.]]></Trigger>
      <Trigger angle=""-0.10471975511965977"" direction=""Descending"" ><![CDATA[The glow in the western sky, the last remnants of the day that was, fade away to a dim memory, heralding the evening.]]></Trigger>
      <Trigger angle=""0.052359877559829883"" direction=""Descending"" ><![CDATA[Shadows lengthen and the western sky turns shades of orange and pink as the sun dips low to the horizon.]]></Trigger>
    </Triggers>
    <ElevationDescriptions>
      <Description lower=""-1.5707963267948966192313216916398"" upper=""-0.20944""><![CDATA[is gone from the sky, and it is night.]]></Description>
      <Description lower=""-0.20944"" upper=""-0.10472""><![CDATA[is only visible as a faint, dim glow just beneath the {{0}} horizon.]]></Description>
      <Description lower=""-0.10472"" upper=""-0.0152""><![CDATA[is only visible as a warm glow just beneath the {{0}} horizon.]]></Description>
      <Description lower=""-0.0152"" upper=""-0.00595""><![CDATA[is partially visible above the {{0}} horizon.]]></Description>
      <Description lower=""-0.00595"" upper=""0.1047""><![CDATA[is very low in the {{0}} sky, colour awash with oranges and pinks.]]></Description>
      <Description lower=""0.1047"" upper=""0.21""><![CDATA[is low in the {{0}} sky, casting long, dark shadows over the land.]]></Description>
      <Description lower=""0.21"" upper=""0.42""><![CDATA[is in the {{0}} sky, at a middle range of elevation.]]></Description>
      <Description lower=""0.42"" upper=""1.20943951023931953""><![CDATA[is high in the {{0}} sky, casting its rays far and wide.]]></Description>
      <Description lower=""1.20943951023931953"" upper=""1.35""><![CDATA[is very nearly directly overhead.]]></Description>
      <Description lower=""1.35"" upper=""1.5707963267948966192313216916398""><![CDATA[is directly overhead, banishing shadows from the land.]]></Description>
    </ElevationDescriptions>
    <AzimuthDescriptions>
      <Description lower=""-0.19634954084936647692528676655901"" upper=""0.19634954084936647692528676655901""><![CDATA[northern]]></Description>
      <Description lower=""0.19634954084936647692528676655901"" upper=""0.589048622548086""><![CDATA[north-northeastern]]></Description>
      <Description lower=""0.589048622548086"" upper=""0.98174770424681""><![CDATA[northeastern]]></Description>
      <Description lower=""0.98174770424681"" upper=""1.37444678594553""><![CDATA[east-northeastern]]></Description>
      <Description lower=""1.37444678594553"" upper=""1.76714586764426""><![CDATA[eastern]]></Description>
      <Description lower=""1.76714586764426"" upper=""2.15984494934298""><![CDATA[east-southeastern]]></Description>
      <Description lower=""2.15984494934298"" upper=""2.55254403104171""><![CDATA[southeastern]]></Description>
      <Description lower=""2.55254403104171"" upper=""2.94524311274043""><![CDATA[south-southeastern]]></Description>
      <Description lower=""2.94524311274043"" upper=""3.33794219443916""><![CDATA[southern]]></Description>
      <Description lower=""3.33794219443916"" upper=""3.73064127613788""><![CDATA[south-southwestern]]></Description>
      <Description lower=""3.73064127613788"" upper=""4.1233403578366""><![CDATA[southwestern]]></Description>
      <Description lower=""4.1233403578366"" upper=""4.51603943953533""><![CDATA[west-southwestern]]></Description>
      <Description lower=""4.51603943953533"" upper=""4.90873852123405""><![CDATA[western]]></Description>
      <Description lower=""4.90873852123405"" upper=""5.30143760293278""><![CDATA[west-northwestern]]></Description>
      <Description lower=""5.30143760293278"" upper=""5.6941366846315""><![CDATA[northwestern]]></Description>
      <Description lower=""5.6941366846315"" upper=""6.08683576633022""><![CDATA[north-northwestern]]></Description>
    </AzimuthDescriptions>
  </SunV2>"
		});
	}
}
        private void SetupMoon(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
        {
                var moonName = questionAnswers["moonname"];
                var moonCalendarId = long.Parse(questionAnswers["mooncalendar"]);
                var epoch = questionAnswers["moonepoch"];
                var calendar = context.Calendars.First(x => x.Id == moonCalendarId);

                context.Celestials.Add(new Celestial
                {
                        CelestialType = "PlanetaryMoon",
                        CelestialYear = 0,
                        LastYearBump = 0,
                        Minutes = 0,
                        FeedClockId = calendar.FeedClockId,
                        Definition = @$"<PlanetaryMoon>
    <Name>{moonName}</Name>
    <Calendar>{moonCalendarId}</Calendar>
    <Orbital>
        <CelestialDaysPerYear>29.530588</CelestialDaysPerYear>
        <MeanAnomalyAngleAtEpoch>5.55665</MeanAnomalyAngleAtEpoch>
        <AnomalyChangeAnglePerDay>0.229971</AnomalyChangeAnglePerDay>
        <ArgumentOfPeriapsis>5.1985</ArgumentOfPeriapsis>
        <LongitudeOfAscendingNode>2.18244</LongitudeOfAscendingNode>
        <OrbitalInclination>0.0898</OrbitalInclination>
        <OrbitalEccentricity>0.0549</OrbitalEccentricity>
        <DayNumberAtEpoch>2451545</DayNumberAtEpoch>
        <SiderealTimeAtEpoch>4.889488</SiderealTimeAtEpoch>
        <SiderealTimePerDay>6.300388</SiderealTimePerDay>
        <EpochDate>{epoch}</EpochDate>
    </Orbital>
    <Illumination>
        <PeakIllumination>1.0</PeakIllumination>
        <FullMoonReferenceDay>0</FullMoonReferenceDay>
    </Illumination>
</PlanetaryMoon>"
                });
        }

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp_Unit_Tests
{
	[TestClass]
	public class CelestialTests
	{
		private static NewSun _newSun;		
		private static IFuturemud _gameworld;
		private static MudSharp.TimeAndDate.Date.Calendar _testCalendar;
		private static MudSharp.TimeAndDate.Time.Clock _testClock;

		[ClassInitialize]
		public static void MyClassInitialize(TestContext testContext)
		{
			var saveManagerMock = new Mock<ISaveManager>();
			saveManagerMock.Setup(x => x.Add(It.IsAny<ISaveable>()));

			var clocks = new All<IClock>();

			var calendars = new All<ICalendar>();

			var mock = new Mock<IFuturemud>();
			mock.SetupGet(t => t.Clocks).Returns(clocks);
			mock.SetupGet(t => t.Calendars).Returns(calendars);
			mock.SetupGet(t => t.SaveManager).Returns(saveManagerMock.Object);
			_gameworld = mock.Object;

			_testCalendar = new MudSharp.TimeAndDate.Date.Calendar(XElement.Parse(
											 @"<calendar><alias>labmud</alias><shortname>The LabMUD Calendar</shortname><fullname>The LabMUD Calendar</fullname><description><![CDATA[The calendar used by test subjects in the Lab, and by which all dates of importance to test subjects are communicated.]]></description><shortstring>$dd/$mo/$yy $ee</shortstring><longstring>$nz$ww the $dt of $mf, year $yy $EE</longstring><wordystring>$nz$ww the $dt of $mf, year $yy $EE</wordystring><plane>earth</plane><feedclock>0</feedclock><epochyear>1</epochyear><weekdayatepoch>5</weekdayatepoch><ancienterashortstring>B.T</ancienterashortstring><ancienteralongstring>before Tranquility</ancienteralongstring><modernerashortstring>A.T</modernerashortstring><moderneralongstring>after Tranquility</moderneralongstring><weekdays><weekday>Work Day</weekday><weekday>Sports Day</weekday><weekday>Science Day</weekday><weekday>Hump Day</weekday><weekday>Garbage Day</weekday><weekday>Laundry Day</weekday><weekday>Lazy Day</weekday></weekdays><months><month><alias>archimedes</alias><shortname>arc</shortname><fullname>Archimedes</fullname><nominalorder>1</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays></specialdays><nonweekdays /></month><month><alias>brahe</alias><shortname>bra</shortname><fullname>Brahe</fullname><nominalorder>2</nominalorder><normaldays>28</normaldays><intercalarydays/><specialdays /><nonweekdays /></month><month><alias>copernicus</alias><shortname>cop</shortname><fullname>Copernicus</fullname><nominalorder>3</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>darwin</alias><shortname>dar</shortname><fullname>Darwin</fullname><nominalorder>4</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>einstein</alias><shortname>ein</shortname><fullname>Einstein</fullname><nominalorder>5</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>faraday</alias><shortname>far</shortname><fullname>Faraday</fullname><nominalorder>6</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>galileo</alias><shortname>gal</shortname><fullname>Galileo</fullname><nominalorder>7</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>hippocrates</alias><shortname>hip</shortname><fullname>Hippocrates</fullname><nominalorder>8</nominalorder><normaldays>28</normaldays><specialdays /><intercalarydays><intercalary><insertdays>1</insertdays><specialdays><specialday day=""29"" short=""Aldrin Day"" long=""Aldrin Day"" /></specialdays><nonweekdays><nonweekday>29</nonweekday></nonweekdays><removenonweekdays /><removespecialdays /><intercalaryrule><offset>-31</offset><divisor>4</divisor><exceptions><intercalaryrule><offset>-31</offset><divisor>100</divisor><exceptions><intercalaryrule><offset>-31</offset><divisor>400</divisor><exceptions /><ands /><ors /></intercalaryrule></exceptions><ands /><ors /></intercalaryrule></exceptions><ands /><ors /></intercalaryrule></intercalary></intercalarydays><nonweekdays /></month><month><alias>imhotep</alias><shortname>imh</shortname><fullname>Imhotep</fullname><nominalorder>9</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>jung</alias><shortname>jun</shortname><fullname>Jung</fullname><nominalorder>10</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>kepler</alias><shortname>kep</shortname><fullname>Kepler</fullname><nominalorder>11</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>lavoisier</alias><shortname>lav</shortname><fullname>Lavoisier</fullname><nominalorder>12</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month><month><alias>mendel</alias><shortname>men</shortname><fullname>Mendel</fullname><nominalorder>13</nominalorder><normaldays>28</normaldays><intercalarydays></intercalarydays><specialdays /><nonweekdays /></month></months><intercalarymonths><intercalarymonth><position>14</position><month><alias>tranquility</alias><shortname>tra</shortname><fullname>Tranquility</fullname><nominalorder>0</nominalorder><normaldays>1</normaldays><intercalarydays/><specialdays><specialday day=""1"" short=""Armstrong Day"" long=""Armstrong Day"" /></specialdays><nonweekdays><nonweekday>1</nonweekday></nonweekdays></month><intercalaryrule><offset>0</offset><divisor>1</divisor><exceptions/><ands /><ors /></intercalaryrule></intercalarymonth></intercalarymonths></calendar>")
			, _gameworld)
			{
				Id = 1
			};
			_testCalendar.SetDate("3/jun/35");

			_testClock = new MudSharp.TimeAndDate.Time.Clock(XElement.Parse(@"<Clock>  <Alias>UTC</Alias>  <Description>Universal Time Clock</Description>  <ShortDisplayString>$j:$m:$s $i</ShortDisplayString>  <SuperDisplayString>$j:$m:$s $i $t</SuperDisplayString>  <LongDisplayString>$c $i</LongDisplayString>  <SecondsPerMinute>60</SecondsPerMinute>  <MinutesPerHour>60</MinutesPerHour>  <HoursPerDay>24</HoursPerDay>  <InGameSecondsPerRealSecond>2</InGameSecondsPerRealSecond>  <SecondFixedDigits>2</SecondFixedDigits>  <MinuteFixedDigits>2</MinuteFixedDigits>  <HourFixedDigits>0</HourFixedDigits>  <NoZeroHour>true</NoZeroHour>  <NumberOfHourIntervals>2</NumberOfHourIntervals>  <HourIntervalNames>    <HourIntervalName>a.m</HourIntervalName>    <HourIntervalName>p.m</HourIntervalName>  </HourIntervalNames>  <HourIntervalLongNames>    <HourIntervalLongName>in the morning</HourIntervalLongName>    <HourIntervalLongName>in the afternoon</HourIntervalLongName>  </HourIntervalLongNames>  <CrudeTimeIntervals>    <CrudeTimeInterval text=""night"" Lower=""-2"" Upper=""4""/>    <CrudeTimeInterval text=""morning"" Lower=""4"" Upper=""12""/>    <CrudeTimeInterval text=""afternoon"" Lower=""12"" Upper=""18""/>    <CrudeTimeInterval text=""evening"" Lower=""18"" Upper=""22""/>  </CrudeTimeIntervals></Clock>"), 
				_gameworld,
				new MudTimeZone(1, 0, 0, "UTC+0", "utc"), 
				12,
				0,
				0
				) { Id = 1 };

			_testCalendar.FeedClock = _testClock;

			

			clocks.Add(_testClock);
			calendars.Add(_testCalendar);

			_newSun = new NewSun(new Celestial
			{
				Id = 1,
				CelestialYear = 0,
				LastYearBump = 0,
				FeedClockId = 1,
				Minutes = 0,
				Seasons = new List<Season>(),
				WeatherControllers = new List<WeatherController>(),
				Definition = @"<Sun>
   <Name>The Sun</Name>
	<Calendar>1</Calendar>
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
		<EpochDate>25-far-31</EpochDate>
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
	 <Description lower=""-0.20944"" upper=""-0.10472""><![CDATA[is only visible as a faint, dim glow just beneath the {0} horizon.]]></Description>
	 <Description lower=""-0.10472"" upper=""-0.0152""><![CDATA[is only visible as a warm glow just beneath the {0} horizon.]]></Description>
	 <Description lower=""-0.0152"" upper=""-0.00595""><![CDATA[is partially visible above the {0} horizon.]]></Description>
	 <Description lower=""-0.00595"" upper=""0.1047""><![CDATA[is very low in the {0} sky, colour awash with oranges and pinks.]]></Description>
	 <Description lower=""0.1047"" upper=""0.21""><![CDATA[is low in the {0} sky, casting long, dark shadows over the land.]]></Description>
	 <Description lower=""0.21"" upper=""0.42""><![CDATA[is in the {0} sky, at a middle range of elevation.]]></Description>
	 <Description lower=""0.42"" upper=""1.20943951023931953""><![CDATA[is high in the {0} sky, casting its rays far and wide.]]></Description>
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
 </Sun>"
			},
				_gameworld
			);
		}

		[TestMethod]
		public void TestOrbitalMathematics()
		{
			Assert.AreEqual(2453097, _newSun.CurrentDayNumber, 0.005, "Day Number");
			Assert.AreEqual(1.521591, _newSun.MeanAnomaly(_newSun.CurrentDayNumber), 0.005);
			Assert.AreEqual(1.555004855, _newSun.TrueAnomaly(_newSun.CurrentDayNumber), 0.005);
			var geography = new GeographicCoordinate(0.907571, 0.08727, 0.0, 0.0);
			var currentInfo = _newSun.CurrentPosition(geography);
			Assert.AreEqual(0.744438, currentInfo.LastAscensionAngle, 0.005, "Ascension Angle");
			Assert.AreEqual(0.089199, currentInfo.LastAzimuthAngle, 0.005, "Azimuth");

			geography = new GeographicCoordinate(0, 0, 0.0, 0.0);
			_testClock.CurrentTime.SetTime(18, 59, 17);
			var dn1 = _newSun.CurrentDayNumber;
			var ha1 = _newSun.HourAngle(dn1, geography);
			var st1 = _newSun.SiderealTime(dn1, geography);
			var ra1 = _newSun.RightAscension(dn1);
			var aa1 = _newSun.Altitude(dn1, geography);

			_testClock.CurrentTime.SetTime(19, 0, 0);
			var dn2 = _newSun.CurrentDayNumber;
			var ha2 = _newSun.HourAngle(dn2, geography);
			var st2 = _newSun.SiderealTime(dn2, geography);
			var ra2 = _newSun.RightAscension(dn2);
			var aa2 = _newSun.Altitude(dn2, geography);

			_testClock.CurrentTime.SetTime(19, 1, 0);
			var dn3 = _newSun.CurrentDayNumber;
			var ha3 = _newSun.HourAngle(dn3, geography);
			var st3 = _newSun.SiderealTime(dn3, geography);
			var ra3 = _newSun.RightAscension(dn3);
			var aa3 = _newSun.Altitude(dn3, geography);

			_testClock.CurrentTime.SetTime(19, 2, 0);
			var dn4 = _newSun.CurrentDayNumber;
			var ha4 = _newSun.HourAngle(dn4, geography);
			var st4 = _newSun.SiderealTime(dn4, geography);
			var ra4 = _newSun.RightAscension(dn4);
			var aa4 = _newSun.Altitude(dn4, geography);

			_testClock.CurrentTime.SetTime(19, 3, 0);
			var dn5 = _newSun.CurrentDayNumber;
			var ha5 = _newSun.HourAngle(dn5, geography);
			var st5 = _newSun.SiderealTime(dn5, geography);
			var ra5 = _newSun.RightAscension(dn5);
			var aa5 = _newSun.Altitude(dn5, geography);

			_testClock.CurrentTime.SetTime(19, 4, 0);
			var dn6 = _newSun.CurrentDayNumber;
			var ha6 = _newSun.HourAngle(dn6, geography);
			var st6 = _newSun.SiderealTime(dn6, geography);
			var ra6 = _newSun.RightAscension(dn6);
			var aa6 = _newSun.Altitude(dn6, geography);
		}
	}
}

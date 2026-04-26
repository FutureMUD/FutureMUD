using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Listeners;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DbEconomicZone = MudSharp.Models.EconomicZone;
using RuntimeCalendar = MudSharp.TimeAndDate.Date.Calendar;
using RuntimeClock = MudSharp.TimeAndDate.Time.Clock;
using RuntimeEconomicZone = MudSharp.Economy.EconomicZone;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class EconomicZoneTests
{
	[TestMethod]
	public void BuildingCommandCalendar_AcceptRebuildsFinancialPeriodsWithoutThrowing()
	{
		var originalAcceptPhrasing = SetStandardAcceptPhrasingForTests();
		try
		{
			var saveManager = new Mock<ISaveManager>();
			var gameworld = CreateGameworld(saveManager, out var oldCalendar, out var newCalendar, out var listeners);
			var economicZone = CreateEconomicZone(gameworld.Object, oldCalendar);
			var originalPeriod = economicZone.CurrentFinancialPeriod;
			var actor = CreateActor(gameworld.Object, out var outputMessages, out var capturedEffects);

			var result = economicZone.BuildingCommandCalendar(actor.Object, new StringStack("fiscal"));

			Assert.IsTrue(result);
			var capturedEffect = capturedEffects.SingleOrDefault();
			Assert.IsNotNull(capturedEffect);
			StringAssert.Contains(outputMessages.First(), "Warning: Changing the calendar will delete all the existing financial periods");

			Assert.IsInstanceOfType(capturedEffect, typeof(Accept));
			((Accept)capturedEffect!).Proposal.Accept();

			Assert.AreSame(newCalendar, economicZone.FinancialPeriodReferenceCalendar);
			Assert.AreNotSame(originalPeriod, economicZone.CurrentFinancialPeriod);
			Assert.AreEqual(1, economicZone.FinancialPeriods.Count());
			Assert.AreSame(economicZone.CurrentFinancialPeriod, economicZone.FinancialPeriods.Single());
			Assert.AreSame(newCalendar, economicZone.CurrentFinancialPeriod.FinancialPeriodStartMUD.Calendar);
			StringAssert.Contains(outputMessages.Last(), "now use");
			StringAssert.Contains(outputMessages.Last(), newCalendar.Name);
			Assert.IsTrue(listeners.Count >= 2);
			saveManager.Verify(x => x.Abort(It.Is<ISaveable>(item => ReferenceEquals(item, originalPeriod))), Times.Once);
		}
		finally
		{
			RestoreStandardAcceptPhrasing(originalAcceptPhrasing);
		}
	}

	private static string? SetStandardAcceptPhrasingForTests()
	{
		var field = typeof(Accept)
			.GetField("_standardAcceptPhrasing", BindingFlags.Static | BindingFlags.NonPublic)!;
		var original = (string?)field.GetValue(null);
		field.SetValue(null, "Type accept to accept, or decline to decline.");
		return original;
	}

	private static void RestoreStandardAcceptPhrasing(string? original)
	{
		typeof(Accept)
			.GetField("_standardAcceptPhrasing", BindingFlags.Static | BindingFlags.NonPublic)!
			.SetValue(null, original);
	}

	private static Mock<ICharacter> CreateActor(IFuturemud gameworld, out List<string> outputMessages, out List<IEffect> capturedEffects)
	{
		var messages = new List<string>();
		outputMessages = messages;
		var effects = new List<IEffect>();
		capturedEffects = effects;

		var output = new Mock<IOutputHandler>();
		output
			.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<string, bool, bool>((text, _, _) => messages.Add(text))
			.Returns(true);

		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld);
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor
			.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((newEffect, _) => effects.Add(newEffect));

		return actor;
	}

	private static Mock<IFuturemud> CreateGameworld(
		Mock<ISaveManager> saveManager,
		out ICalendar oldCalendar,
		out ICalendar newCalendar,
		out List<ITemporalListener> listeners)
	{
		var clocks = new All<IClock>();
		var calendars = new All<ICalendar>();
		var currencies = new All<ICurrency>();
		var zones = new All<IZone>();
		var shops = new All<IShop>();
		var cells = new All<ICell>();
		var capturedListeners = new List<ITemporalListener>();
		listeners = capturedListeners;

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Clocks).Returns(clocks);
		gameworld.SetupGet(x => x.Calendars).Returns(calendars);
		gameworld.SetupGet(x => x.Currencies).Returns(currencies);
		gameworld.SetupGet(x => x.Zones).Returns(zones);
		gameworld.SetupGet(x => x.Shops).Returns(shops);
		gameworld.SetupGet(x => x.Cells).Returns(cells);
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld
			.Setup(x => x.Add(It.IsAny<ITemporalListener>()))
			.Callback<ITemporalListener>(listener => capturedListeners.Add(listener));

		var clock = CreateClock(gameworld.Object);
		clocks.Add(clock);

		oldCalendar = CreateCalendar(gameworld.Object, clock, 1L, "civic", "Civic Calendar");
		newCalendar = CreateCalendar(gameworld.Object, clock, 2L, "fiscal", "Fiscal Calendar");
		calendars.Add(oldCalendar);
		calendars.Add(newCalendar);

		var currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.Id).Returns(1L);
		currency.SetupGet(x => x.Name).Returns("Test Dollars");
		currency.SetupGet(x => x.FrameworkItemType).Returns("Currency");
		currencies.Add(currency.Object);

		var zone = new Mock<IZone>();
		zone.SetupGet(x => x.Id).Returns(1L);
		zone.SetupGet(x => x.Name).Returns("Test Zone");
		zone.SetupGet(x => x.FrameworkItemType).Returns("Zone");
		zones.Add(zone.Object);

		return gameworld;
	}

	private static RuntimeEconomicZone CreateEconomicZone(IFuturemud gameworld, ICalendar oldCalendar)
	{
		return new RuntimeEconomicZone(new DbEconomicZone
		{
			Id = 1L,
			Name = "Test Economic Zone",
			CurrencyId = 1L,
			ZoneForTimePurposesId = 1L,
			ReferenceCalendarId = oldCalendar.Id,
			ReferenceClockId = oldCalendar.FeedClock.Id,
			ReferenceTime = "utc 0:0:0",
			IntervalType = (int)IntervalType.Yearly,
			IntervalAmount = 1,
			IntervalModifier = 0,
			PreviousFinancialPeriodsToKeep = 50,
			EstatesEnabled = true,
			PermitTaxableLosses = false
		}, gameworld);
	}

	private static IClock CreateClock(IFuturemud gameworld)
	{
		var clock = new RuntimeClock(
			XElement.Parse(
			@"<Clock>
	<Alias>UTC</Alias>
	<Description>Universal Time Clock</Description>
	<ShortDisplayString>$j:$m:$s $i</ShortDisplayString>
	<SuperDisplayString>$j:$m:$s $i $t</SuperDisplayString>
	<LongDisplayString>$c $i</LongDisplayString>
	<SecondsPerMinute>60</SecondsPerMinute>
	<MinutesPerHour>60</MinutesPerHour>
	<HoursPerDay>24</HoursPerDay>
	<InGameSecondsPerRealSecond>2</InGameSecondsPerRealSecond>
	<SecondFixedDigits>2</SecondFixedDigits>
	<MinuteFixedDigits>2</MinuteFixedDigits>
	<HourFixedDigits>0</HourFixedDigits>
	<NoZeroHour>true</NoZeroHour>
	<NumberOfHourIntervals>2</NumberOfHourIntervals>
	<HourIntervalNames>
		<HourIntervalName>a.m</HourIntervalName>
		<HourIntervalName>p.m</HourIntervalName>
	</HourIntervalNames>
	<HourIntervalLongNames>
		<HourIntervalLongName>in the morning</HourIntervalLongName>
		<HourIntervalLongName>in the afternoon</HourIntervalLongName>
	</HourIntervalLongNames>
	<CrudeTimeIntervals>
		<CrudeTimeInterval text=""night"" Lower=""-2"" Upper=""4"" />
		<CrudeTimeInterval text=""morning"" Lower=""4"" Upper=""12"" />
		<CrudeTimeInterval text=""afternoon"" Lower=""12"" Upper=""18"" />
		<CrudeTimeInterval text=""evening"" Lower=""18"" Upper=""22"" />
	</CrudeTimeIntervals>
</Clock>"),
			gameworld,
			new MudTimeZone(1, 0, 0, "utc", "utc"),
			0,
			0,
			0)
		{
			Id = 1L
		};
		clock.AddTimezone(clock.PrimaryTimezone);
		return clock;
	}

	private static ICalendar CreateCalendar(IFuturemud gameworld, IClock clock, long id, string alias, string name)
	{
		var calendar = new RuntimeCalendar(
			XElement.Parse(
				$@"<calendar>
	<alias>{alias}</alias>
	<shortname>{name}</shortname>
	<fullname>{name}</fullname>
	<description>Test calendar</description>
	<shortstring>$dd/$mo/$yy</shortstring>
	<longstring>$dd $mo $yy</longstring>
	<wordystring>$dd $mo $yy</wordystring>
	<plane>earth</plane>
	<feedclock>{clock.Id}</feedclock>
	<epochyear>2000</epochyear>
	<weekdayatepoch>5</weekdayatepoch>
	<ancienterashortstring>BC</ancienterashortstring>
	<ancienteralongstring>before common era</ancienteralongstring>
	<modernerashortstring>AD</modernerashortstring>
	<moderneralongstring>common era</moderneralongstring>
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
		<month><alias>january</alias><shortname>jan</shortname><fullname>January</fullname><nominalorder>1</nominalorder><normaldays>31</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>february</alias><shortname>feb</shortname><fullname>February</fullname><nominalorder>2</nominalorder><normaldays>28</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>march</alias><shortname>mar</shortname><fullname>March</fullname><nominalorder>3</nominalorder><normaldays>31</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>april</alias><shortname>apr</shortname><fullname>April</fullname><nominalorder>4</nominalorder><normaldays>30</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>may</alias><shortname>may</shortname><fullname>May</fullname><nominalorder>5</nominalorder><normaldays>31</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>june</alias><shortname>jun</shortname><fullname>June</fullname><nominalorder>6</nominalorder><normaldays>30</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>july</alias><shortname>jul</shortname><fullname>July</fullname><nominalorder>7</nominalorder><normaldays>31</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>august</alias><shortname>aug</shortname><fullname>August</fullname><nominalorder>8</nominalorder><normaldays>31</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>september</alias><shortname>sep</shortname><fullname>September</fullname><nominalorder>9</nominalorder><normaldays>30</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>october</alias><shortname>oct</shortname><fullname>October</fullname><nominalorder>10</nominalorder><normaldays>31</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>november</alias><shortname>nov</shortname><fullname>November</fullname><nominalorder>11</nominalorder><normaldays>30</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
		<month><alias>december</alias><shortname>dec</shortname><fullname>December</fullname><nominalorder>12</nominalorder><normaldays>31</normaldays><intercalarydays /><specialdays /><nonweekdays /></month>
	</months>
	<intercalarymonths />
</calendar>"),
			gameworld)
		{
			Id = id
		};
		calendar.FeedClock = clock;
		calendar.SetDate("1/january/2000");
		return calendar;
	}
}

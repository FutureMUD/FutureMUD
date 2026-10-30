#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.RPG.Law;
using MudSharp.RPG.Law.PunishmentStrategies;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AutomaticSentencingTests
{
	[TestMethod]
	public void AutomaticConviction_ElapsedRemandRecordsFineAndReleasesPrisoner()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var calendars = new All<ICalendar>(); var clocks = new All<IClock>();
		world.SetupGet(x => x.Calendars).Returns(calendars); world.SetupGet(x => x.Clocks).Returns(clocks);
		world.SetupGet(x => x.Rooms).Returns(new All<IRoom>());
		world.SetupGet(x => x.FutureProgs).Returns(new All<MudSharp.FutureProg.IFutureProg>());
		var clock = new Clock(XElement.Parse(@"<Clock><Alias>UTC</Alias><Description>Test</Description><ShortDisplayString>$j:$m:$s</ShortDisplayString><SuperDisplayString>$j:$m:$s</SuperDisplayString><LongDisplayString>$j:$m:$s</LongDisplayString><SecondsPerMinute>60</SecondsPerMinute><MinutesPerHour>60</MinutesPerHour><HoursPerDay>24</HoursPerDay><InGameSecondsPerRealSecond>1</InGameSecondsPerRealSecond><SecondFixedDigits>2</SecondFixedDigits><MinuteFixedDigits>2</MinuteFixedDigits><HourFixedDigits>0</HourFixedDigits><NoZeroHour>false</NoZeroHour><NumberOfHourIntervals>1</NumberOfHourIntervals><HourIntervalNames><HourIntervalName>day</HourIntervalName></HourIntervalNames><HourIntervalLongNames><HourIntervalLongName>day</HourIntervalLongName></HourIntervalLongNames><CrudeTimeIntervals><CrudeTimeInterval text='day' Lower='0' Upper='24'/></CrudeTimeIntervals></Clock>"), world.Object) { Id = 1 };
		var timezone = new MudTimeZone(1, 0, 0, "Test", "UTC", clock); clock.AddTimezone(timezone);
		clock.SetTime(MudTime.CreatePrimaryTime(0, 0, 0, timezone, clock)); clocks.Add(clock);
		var calendar = new Calendar(1, world.Object); calendar.SetNoSave(true); calendar.SetupTestData();
		calendar.FeedClock = clock; calendar.SetDate("1/jan/2026"); calendars.Add(calendar);
		var now = new MudDateTime(calendar.CurrentDate, clock.CurrentTime, timezone);
		var authority = new LegalAuthority(new MudSharp.Models.LegalAuthority { Id = 1, Name = "Guard",
			AutomaticallyConvict = true, AutomaticConvictionTime = 300 }, world.Object);
		var holding = new Mock<IRoom>(); var release = new Mock<IRoom>(); var storage = new Mock<IRoom>();
		authority.PrisonReleaseLocation = release.Object; authority.PrisonerBelongingsStorageLocation = storage.Object;
		((List<IRoom>)typeof(LegalAuthority).GetField("_roomLocations", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(authority)!).Add(holding.Object);
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Id).Returns(100);
		actor.SetupGet(x => x.Location).Returns(holding.Object); actor.SetupGet(x => x.Body).Returns(new Mock<IBody>().Object);
		var awaiting = new AwaitingSentencing(actor.Object, authority, now);
		actor.Setup(x => x.EffectsOfType<AwaitingSentencing>(It.IsAny<Predicate<AwaitingSentencing>>())).Returns([awaiting]);
		var fine = new PunishmentStrategyFine(world.Object, XElement.Parse("<Strategy type='fine'><DefaultFineAmount>25</DefaultFineAmount><MaximumFineAmount>25</MaximumFineAmount></Strategy>"), authority);
		var law = new Mock<ILaw>(); law.SetupGet(x => x.PunishmentStrategy).Returns(fine);
		var crime = new Mock<ICrime>(); crime.SetupGet(x => x.CriminalId).Returns(100);
		crime.SetupGet(x => x.Criminal).Returns(actor.Object); crime.SetupGet(x => x.Law).Returns(law.Object);
		crime.Setup(x => x.EligableForAutomaticConviction()).Returns(true);
		crime.Setup(x => x.Convict(null, It.IsAny<PunishmentResult>(), It.IsAny<string>()))
			.Callback<ICharacter, PunishmentResult, string>((_, result, _) => authority.ConvictCrime(actor.Object, crime.Object, result));
		authority.AccuseCrime(crime.Object);
		var heartbeat = typeof(LegalAuthority).GetMethod("SentencingHeartbeat", BindingFlags.Instance | BindingFlags.NonPublic)!;
		heartbeat.Invoke(authority, [now + MudTimeSpan.FromSeconds(299)]);
		release.Verify(x => x.Enter(actor.Object, null, false, RoomLayer.GroundLevel), Times.Never);
		heartbeat.Invoke(authority, [now + MudTimeSpan.FromSeconds(301)]);
		Assert.AreEqual(25m, authority.FinesOwed(actor.Object).Fine);
		release.Verify(x => x.Enter(actor.Object, null, false, RoomLayer.GroundLevel), Times.Once);
		actor.Verify(x => x.RemoveAllEffects<AwaitingSentencing>(It.IsAny<Predicate<AwaitingSentencing>>(), false), Times.Once);
	}

	[TestMethod]
	public void Fine_ZeroSeverityUsesStandardAndOneUsesMaximum()
	{
		var world = new Mock<IFuturemud>(); var authority = new Mock<ILegalAuthority>();
		world.SetupGet(x => x.FutureProgs).Returns(new All<MudSharp.FutureProg.IFutureProg>());
		var fine = new PunishmentStrategyFine(world.Object, XElement.Parse(
			"<Strategy type='fine'><DefaultFineAmount>10</DefaultFineAmount><MaximumFineAmount>50</MaximumFineAmount></Strategy>"), authority.Object);
		Assert.AreEqual(10m, fine.GetResult(null!, null!).Fine);
		Assert.AreEqual(30m, fine.GetResult(null!, null!, 0.5).Fine);
		Assert.AreEqual(50m, fine.GetResult(null!, null!, 1).Fine);
		foreach (var amount in new[] { 10m, 25m, 50m })
		{
			fine.FineAmount = fine.MaximumFineAmount = amount;
			Assert.AreEqual(amount, fine.GetResult(null!, null!).Fine);
		}
	}

	[TestMethod]
	public void AutomaticSentence_FineOnlyReleasesAndReturnsBelongings_ExecutionRemainsHeld()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		world.SetupGet(x => x.Rooms).Returns(new All<IRoom>());
		world.SetupGet(x => x.FutureProgs).Returns(new All<MudSharp.FutureProg.IFutureProg>());
		var authority = new LegalAuthority(new MudSharp.Models.LegalAuthority { Id = 1, Name = "Guard" }, world.Object);
		var holding = new Mock<IRoom>(); var release = new Mock<IRoom>(); var storage = new Mock<IRoom>();
		authority.PrisonReleaseLocation = release.Object; authority.PrisonerBelongingsStorageLocation = storage.Object;
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Location).Returns(holding.Object);
		var body = new Mock<IBody>(); actor.SetupGet(x => x.Body).Returns(body.Object);
		var bundle = new Mock<IGameItem>(); bundle.Setup(x => x.AffectedBy<PrisonerBelongings>(actor.Object)).Returns(true);
		storage.SetupGet(x => x.GameItems).Returns([bundle.Object]);
		body.Setup(x => x.CanGet(bundle.Object, 0, It.IsAny<ItemCanGetIgnore>())).Returns(true);
		authority.CompleteAutomaticSentencing(actor.Object);
		holding.Verify(x => x.Leave(actor.Object), Times.Once);
		release.Verify(x => x.Enter(actor.Object, null, false, RoomLayer.GroundLevel), Times.Once);
		body.Verify(x => x.Get(bundle.Object, 0, null, true, It.IsAny<ItemCanGetIgnore>()), Times.Once);
		actor.Setup(x => x.EffectsOfType<AwaitingExecution>(It.IsAny<Predicate<AwaitingExecution>>()))
			.Returns([new AwaitingExecution(actor.Object, authority, MudDateTime.Never)]);
		authority.CompleteAutomaticSentencing(actor.Object);
		release.Verify(x => x.Enter(actor.Object, null, false, RoomLayer.GroundLevel), Times.Once);
	}
}

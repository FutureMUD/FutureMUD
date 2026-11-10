using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.Economy;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using Appointment = MudSharp.Community.Appointment;
using Clan = MudSharp.Community.Clan;
using Election = MudSharp.Community.Election;

#nullable enable

namespace MudSharp_Unit_Tests.Community;

[TestClass]
[DoNotParallelize]
public class ClanElectionGovernanceTests
{
	[TestMethod]
	public void Confirmation_RequiresExactActiveOfficeIncludingOtherClans()
	{
		using var f = new Fixture();
		Assert.IsTrue(ClanCommandUtilities.CanConfirmElection(f.Actor.Object, f.Office));
		f.AuthorityMembership.SetupGet(x => x.IsArchivedMembership).Returns(true);
		Assert.IsFalse(ClanCommandUtilities.CanConfirmElection(f.Actor.Object, f.Office));
		f.AuthorityMembership.SetupGet(x => x.IsArchivedMembership).Returns(false);
		f.AuthorityMembership.Object.Appointments.Clear();
		Assert.IsFalse(ClanCommandUtilities.CanConfirmElection(f.Actor.Object, f.Office));
		f.Actor.Setup(x => x.IsAdministrator(PermissionLevel.Admin)).Returns(true);
		Assert.IsTrue(ClanCommandUtilities.CanConfirmElection(f.Actor.Object, f.Office));
	}

	[TestMethod]
	public void VotingEnd_WholeSlateWaitsAndConfirmationInstallsBothWithoutRecurringElection()
	{
		using var f = new Fixture();
		f.Office.MaximumSimultaneousHolders = 2;
		var winner2 = f.Member(3, "Second");
		var election = f.Ballot();
		election.Nominate(winner2.Object);
		election.Vote(winner2.Object, winner2.Object, 1);
		election.MoveTo(ElectionStage.Voting);
		Assert.IsFalse(election.CheckElectionStage());
		Assert.AreEqual(ElectionStage.AwaitingConfirmation, election.ElectionStage);
		Assert.AreEqual(2, election.Victors.Count());
		Assert.IsEmpty(f.Winner.Object.Appointments);
		Assert.IsEmpty(winner2.Object.Appointments);
		Assert.IsFalse(election.CheckElectionStage());
		Assert.IsFalse(election.Confirm(f.Player.Object));
		Assert.IsFalse(election.Veto(f.Player.Object));
		Assert.IsTrue(election.Confirm(f.Actor.Object));
		Assert.IsTrue(election.IsFinalised);
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		Assert.Contains(f.Office, winner2.Object.Appointments);
		Assert.AreEqual(1, f.Office.Elections.Count());
		Assert.IsFalse(election.Confirm(f.Actor.Object));
		Assert.IsFalse(election.Veto(f.Actor.Object));
	}

	[TestMethod]
	public void ElectionOverview_ExternalAuthoritySeesOnlyGovernedOfficeAndCannotParticipate()
	{
		using var f = new Fixture();
		f.Actor.SetupGet(x => x.Account).Returns(Mock.Of<IAccount>(x => x.LineFormatLength == 120));
		var hidden = Mock.Of<IAppointment>(x => x.Id == 5 && x.Name == "SecretOffice" && x.IsAppointedByElection);
		f.Clan.Appointments.Add(hidden);
		var display = f.Clan.DescribeElections(f.Actor.Object).RawText();
		StringAssert.Contains(display, "Priest");
		Assert.IsFalse(display.Contains("SecretOffice"));
		StringAssert.Contains(display, "are not eligible to nominate");
		StringAssert.Contains(display, "cannot vote");
	}

	[TestMethod]
	public void ExpiredBallot_AuthorityClanNotYetLoaded_StillRequiresConfirmation()
	{
		using var f = new Fixture();
		f.World.SetupGet(x => x.Clans).Returns(new All<IClan> { f.Clan });
		Assert.IsNull(f.Office.ElectionConfirmationAppointment);
		Assert.IsTrue(f.Office.RequiresElectionConfirmation);
		var election = f.Ballot();
		election.MoveTo(ElectionStage.Voting);
		Assert.IsFalse(election.CheckElectionStage());
		Assert.AreEqual(ElectionStage.AwaitingConfirmation, election.ElectionStage);
		Assert.IsEmpty(f.Winner.Object.Appointments);
	}

	[TestMethod]
	public void PendingAndApprovedResults_RoundTripWithoutInstallingEarlyOrReopeningVoting()
	{
		using var f = new Fixture();
		var election = f.Ballot(f.Now + TimeSpan.FromDays(2));
		election.Save();
		f.Database.Context.SaveChanges();
		var stored = f.Database.Context.Elections.Find(election.Id)!;
		var reloaded = new Election(stored, f.Office);
		Assert.AreEqual(ElectionStage.AwaitingConfirmation, reloaded.ElectionStage);
		Assert.IsFalse(reloaded.CheckElectionStage());
		Assert.IsEmpty(f.Winner.Object.Appointments);
		Assert.IsTrue(reloaded.Confirm(f.Actor.Object));
		Assert.AreEqual(ElectionStage.Preinstallation, reloaded.ElectionStage);
		Assert.IsFalse(reloaded.CheckElectionStage());
		reloaded.Save();
		f.Database.Context.SaveChanges();
		Assert.IsTrue(stored.IsConfirmed);
		var approved = new Election(stored, f.Office);
		Assert.IsTrue(approved.IsConfirmed);
		Assert.AreEqual(ElectionStage.Preinstallation, approved.ElectionStage);
		Assert.IsFalse(approved.CheckElectionStage());
		Assert.IsEmpty(f.Winner.Object.Appointments);
	}

	[TestMethod]
	public void Veto_PreservesVotesButNoVictorsAndStartsReplacementVacancyElection()
	{
		using var f = new Fixture();
		var election = f.Ballot();
		Assert.IsTrue(election.Veto(f.Actor.Object));
		Assert.AreEqual(ElectionStage.Vetoed, election.ElectionStage);
		Assert.IsTrue(election.IsFinalised);
		Assert.IsEmpty(election.Victors);
		Assert.HasCount(1, election.Votes.ToList());
		Assert.IsEmpty(f.Winner.Object.Appointments);
		Assert.HasCount(1, f.Office.Elections.Where(x => !x.IsFinalised && x.IsByElection).ToList());
		election.Save();
		f.Database.Context.SaveChanges();
		var loaded = new Election(f.Database.Context.Elections.Find(election.Id)!, f.Office);
		Assert.AreEqual(ElectionStage.Vetoed, loaded.ElectionStage);
		Assert.IsEmpty(loaded.Victors);
	}

	[TestMethod]
	public void Resign_DefaultAllowsAndOpensOneByElectionWhileKeepingMembershipAndOtherHolders()
	{
		using var f = new Fixture();
		f.Office.MaximumSimultaneousHolders = 2;
		var other = f.Member(3, "Other");
		other.Object.Appointments.Add(f.Office);
		f.Winner.Object.Appointments.Add(f.Office);
		f.Office.LoadElections([]);
		Assert.IsEmpty(f.Office.Elections);
		var pending = f.Ballot();
		f.Player.SetupGet(x => x.ClanMemberships).Returns([f.Winner.Object]);
		f.Clan.Resign(f.Player.Object, new StringStack("Priest"));
		Assert.IsEmpty(f.Winner.Object.Appointments);
		Assert.IsFalse(f.Winner.Object.IsArchivedMembership);
		Assert.Contains(f.Office, other.Object.Appointments);
		Assert.IsEmpty(pending.Victors);
		f.Office.CheckForByElections();
		Assert.HasCount(1, f.Office.Elections.Where(x => !x.IsFinalised).ToList());
	}

	[TestMethod]
	public void ApprovedPrimary_WinningIncumbentResigns_ReplacementNeedsFreshConfirmationAfterReload()
	{
		using var f = new Fixture();
		f.Office.ElectionTerm = TimeSpan.FromDays(365);
		f.Winner.Object.Appointments.Add(f.Office);
		var runnerUp = f.Member(3, "RunnerUp");
		var election = f.Ballot(f.Now + TimeSpan.FromDays(2), isByElection: false);
		election.Nominate(runnerUp.Object);
		election.Vote(f.Winner.Object, f.Winner.Object, 2);
		election.Vote(runnerUp.Object, runnerUp.Object, 1);
		Assert.IsTrue(election.Confirm(f.Actor.Object));
		f.Player.SetupGet(x => x.ClanMemberships).Returns([f.Winner.Object]);
		f.Clan.Resign(f.Player.Object, new StringStack("Priest"));
		Assert.AreEqual(ElectionStage.AwaitingConfirmation, election.ElectionStage);
		Assert.IsFalse(election.IsConfirmed);
		Assert.AreSame(runnerUp.Object, election.Victors.Single());
		Assert.IsNull(typeof(Election).GetField("_activeListener", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(election));
		election.Save();
		f.Database.Context.SaveChanges();
		var loaded = new Election(f.Database.Context.Elections.Find(election.Id)!, f.Office);
		loaded.ResultsInEffectDate = f.Now - TimeSpan.FromDays(1);
		Assert.IsFalse(loaded.CheckElectionStage());
		Assert.IsEmpty(runnerUp.Object.Appointments);
		Assert.IsTrue(loaded.Confirm(f.Actor.Object));
		Assert.Contains(f.Office, runnerUp.Object.Appointments);
	}

	[TestMethod]
	public void IndefiniteOffice_ExternalReservationsLeaveNoElectableVacancies()
	{
		using var f = new Fixture();
		var control = new Mock<IExternalClanControl>();
		control.SetupGet(x => x.VassalClan).Returns(f.Clan);
		control.SetupGet(x => x.ControlledAppointment).Returns(f.Office);
		control.SetupGet(x => x.NumberOfAppointments).Returns(1);
		control.SetupGet(x => x.Appointees).Returns([]);
		f.Clan.ExternalControls.Add(control.Object);
		f.Clan.LoadElections();
		Assert.IsEmpty(f.Office.Elections);
	}

	[TestMethod]
	public void Builder_IndefiniteTenureCancelsScheduledPrimaryAndRetainsOpenByElection()
	{
		using var f = new Fixture();
		f.Office.ElectionTerm = TimeSpan.FromDays(365);
		var byElection = f.Ballot();
		var primary = new Election(f.Office, false, 1, f.Now + TimeSpan.FromDays(365));
		f.Office.AddElection(primary);
		typeof(MudSharp.Commands.Modules.ClanModule)
			.GetMethod("ClanEditAppointmentElections", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [f.Actor.Object, new StringStack("term none"), f.Clan, f.Office]);
		Assert.AreEqual(TimeSpan.Zero, f.Office.ElectionTerm);
		Assert.IsFalse(f.Office.Elections.Contains(primary));
		Assert.Contains(byElection, f.Office.Elections.ToList());
	}

	[TestMethod]
	public void Resign_DisabledOfficeRemainsHeld()
	{
		using var f = new Fixture();
		f.Office.CanResign = false;
		f.Winner.Object.Appointments.Add(f.Office);
		f.Player.SetupGet(x => x.ClanMemberships).Returns([f.Winner.Object]);
		f.Clan.Resign(f.Player.Object, new StringStack("Priest"));
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		Assert.IsEmpty(f.Office.Elections);
	}

	[TestMethod]
	public void DirectAppointment_OnlyEnabledAuthorityCanAppointAndDismissElectedOffice()
	{
		using var f = new Fixture();
		f.Clan.Appoint(f.Actor.Object, new StringStack("Winner Priest"));
		Assert.IsEmpty(f.Winner.Object.Appointments);
		f.Office.CanDirectlyAppoint = true;
		f.Clan.Appoint(f.Actor.Object, new StringStack("Winner Priest"));
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		f.Office.CanDirectlyAppoint = false;
		f.Clan.Dismiss(f.Actor.Object, new StringStack("Winner Priest"));
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		f.Office.CanDirectlyAppoint = true;
		f.Clan.Dismiss(f.Actor.Object, new StringStack("Winner Priest"));
		Assert.IsEmpty(f.Winner.Object.Appointments);
		Assert.HasCount(1, f.Office.Elections.Where(x => !x.IsFinalised).ToList());
	}

	[TestMethod]
	public void DiscretionaryDismissal_EmploymentGrantedOfficeCannotBeRemoved()
	{
		using var f = new Fixture();
		f.Office.CanDirectlyAppoint = true;
		f.Winner.Object.Appointments.Add(f.Office);
		f.Player.SetupGet(x => x.Id).Returns(f.Winner.Object.MemberId);
		f.Winner.SetupGet(x => x.MemberCharacter).Returns(f.Player.Object);
		var listing = Mock.Of<IJobListing>(x => x.ClanMembership == f.Clan && x.ClanAppointment == f.Office);
		var job = Mock.Of<IActiveJob>(x => x.Id == 1 && x.Character == f.Player.Object && x.Listing == listing && x.Name == "Cleric");
		f.World.SetupGet(x => x.ActiveJobs).Returns(new All<IActiveJob> { job });
		f.Clan.Dismiss(f.Actor.Object, new StringStack("Winner Priest"));
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		f.Player.SetupGet(x => x.ClanMemberships).Returns([f.Winner.Object]);
		f.Clan.Resign(f.Player.Object, new StringStack("Priest"));
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		Assert.IsEmpty(f.Office.Elections);
	}

	[TestMethod]
	public void GovernanceSettings_SaveReloadRetainsExternalAuthorityAndDefaults()
	{
		using var f = new Fixture();
		Assert.IsTrue(f.Office.CanResign);
		f.Office.CanResign = false;
		f.Office.CanDirectlyAppoint = true;
		f.Office.Save();
		var stored = f.Database.Context.Appointments.Find(f.Office.Id)!;
		var loaded = new Appointment(stored, f.Clan);
		Assert.IsFalse(loaded.CanResign);
		Assert.IsTrue(loaded.CanDirectlyAppoint);
		Assert.AreSame(f.Authority.Object, loaded.ElectionConfirmationAppointment);
		Assert.AreEqual(TimeSpan.Zero, loaded.ElectionTerm);
	}

	private sealed class TestElection(MudSharp.Models.Election row, IAppointment office) : Election(row, office)
	{
		public void MoveTo(ElectionStage stage) => ElectionStage = stage;
	}

	private sealed class Fixture : IDisposable
	{
		public InMemoryDatabaseTestScope Database { get; } = new();
		public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<ICharacter> Player { get; } = new();
		public Mock<IAppointment> Authority { get; } = new();
		public Mock<IClanMembership> AuthorityMembership { get; } = new();
		public Mock<IClanMembership> Winner { get; }
		public Clan Clan { get; }
		public Appointment Office { get; }
		public Clock Clock { get; }
		public MudDateTime Now { get; }
		private long _id = 20;

		public Fixture()
		{
			World.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			World.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
			World.SetupGet(x => x.ActiveJobs).Returns(new All<IActiveJob>());
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Player.SetupGet(x => x.Gameworld).Returns(World.Object);
			Player.SetupGet(x => x.ClanMemberships).Returns([]);
			World.SetupGet(x => x.Actors).Returns(new All<ICharacter>());
			World.SetupGet(x => x.CachedActors).Returns(new All<ICharacter>());
			Clock = new Clock(XElement.Parse(@"<Clock><Alias>UTC</Alias><Description>Test</Description><ShortDisplayString>$j:$m:$s</ShortDisplayString><SuperDisplayString>$j:$m:$s</SuperDisplayString><LongDisplayString>$j:$m:$s</LongDisplayString><SecondsPerMinute>60</SecondsPerMinute><MinutesPerHour>60</MinutesPerHour><HoursPerDay>24</HoursPerDay><InGameSecondsPerRealSecond>1</InGameSecondsPerRealSecond><SecondFixedDigits>2</SecondFixedDigits><MinuteFixedDigits>2</MinuteFixedDigits><HourFixedDigits>0</HourFixedDigits><NoZeroHour>false</NoZeroHour><NumberOfHourIntervals>1</NumberOfHourIntervals><HourIntervalNames><HourIntervalName>day</HourIntervalName></HourIntervalNames><HourIntervalLongNames><HourIntervalLongName>day</HourIntervalLongName></HourIntervalLongNames><CrudeTimeIntervals><CrudeTimeInterval text='day' Lower='0' Upper='24'/></CrudeTimeIntervals></Clock>"), World.Object) { Id = 1 };
			var timezone = new MudTimeZone(1, 0, 0, "Test", "UTC", Clock);
			Clock.AddTimezone(timezone);
			Clock.SetTime(MudTime.CreatePrimaryTime(0, 0, 0, timezone, Clock));
			var calendar = new Calendar(1, World.Object);
			calendar.SetNoSave(true);
			calendar.SetupTestData();
			calendar.FeedClock = Clock;
			calendar.SetDate("1/jan/2026");
			World.SetupGet(x => x.Calendars).Returns(new All<ICalendar> { calendar });
			World.SetupGet(x => x.Clocks).Returns(new All<IClock> { Clock });
			Now = calendar.CurrentDateTime;
			Clan = (Clan)Activator.CreateInstance(typeof(Clan), BindingFlags.Instance | BindingFlags.NonPublic,
				null, [new MudSharp.Models.Clan { Id = 1, Name = "Parish", FullName = "Parish", Alias = "parish", CalendarId = 1 }, World.Object], null)!;
			var authorityClan = new Mock<IClan>();
			authorityClan.SetupGet(x => x.Id).Returns(2);
			authorityClan.SetupGet(x => x.FullName).Returns("Diocese");
			authorityClan.SetupGet(x => x.Appointments).Returns([Authority.Object]);
			Authority.SetupGet(x => x.Id).Returns(9);
			Authority.SetupGet(x => x.Name).Returns("Bishop");
			Authority.SetupGet(x => x.Clan).Returns(authorityClan.Object);
			World.SetupGet(x => x.Clans).Returns(new All<IClan> { Clan, authorityClan.Object });
			var officeRow = new MudSharp.Models.Appointment { Id = 1, ClanId = 1, Name = "Priest", IsAppointedByElection = true,
				MaximumSimultaneousHolders = 1, NominationPeriodMinutes = 1440, VotingPeriodMinutes = 1440 };
			Database.Context.Appointments.Add(officeRow);
			Database.Context.SaveChanges();
			Office = new Appointment(officeRow, Clan) { ElectionConfirmationAppointment = Authority.Object };
			Office.TitlesAndProgs.Add(Tuple.Create<IFutureProg, string>(null!, "Priest"));
			Clan.Appointments.Add(Office);
			Winner = Member(2, "Winner");
			AuthorityMembership.SetupGet(x => x.Clan).Returns(authorityClan.Object);
			AuthorityMembership.SetupGet(x => x.Appointments).Returns([Authority.Object]);
			Actor.SetupGet(x => x.ClanMemberships).Returns([AuthorityMembership.Object]);
			Actor.SetupGet(x => x.PersonalName).Returns(Mock.Of<IPersonalName>(x => x.GetName(NameStyle.FullName) == "Bishop"));
			Actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			Player.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		}

		public Mock<IClanMembership> Member(long id, string name)
		{
			var member = new Mock<IClanMembership>();
			member.SetupGet(x => x.MemberId).Returns(id);
			member.SetupGet(x => x.Name).Returns(name);
			member.SetupGet(x => x.Clan).Returns(Clan);
			member.SetupGet(x => x.Appointments).Returns([]);
			member.SetupGet(x => x.PersonalName).Returns(Mock.Of<IPersonalName>(x => x.GetName(NameStyle.FullName) == name));
			member.SetupProperty(x => x.Changed);
			Clan.Memberships.Add(member.Object);
			return member;
		}

		public TestElection Ballot(MudDateTime? result = null, bool isByElection = true)
		{
			var row = new MudSharp.Models.Election { Id = _id++, AppointmentId = Office.Id, IsByElection = isByElection,
				ElectionStage = (int)ElectionStage.AwaitingConfirmation, NumberOfAppointments = Office.MaximumSimultaneousHolders,
				NominationStartDate = (Now - TimeSpan.FromDays(3)).GetDateTimeString(),
				VotingStartDate = (Now - TimeSpan.FromDays(2)).GetDateTimeString(),
				VotingEndDate = (Now - TimeSpan.FromDays(1)).GetDateTimeString(),
				ResultsInEffectDate = (result ?? Now).GetDateTimeString() };
			Database.Context.Elections.Add(row);
			Database.Context.SaveChanges();
			var election = new TestElection(row, Office);
			election.Nominate(Winner.Object);
			election.Vote(Winner.Object, Winner.Object, 1);
			Office.AddElection(election);
			return election;
		}

		public void Dispose() => Database.Dispose();
	}
}

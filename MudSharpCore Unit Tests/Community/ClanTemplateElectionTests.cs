using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character.Name;
using MudSharp.Commands.Modules;
using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate.Intervals;
using Appointment = MudSharp.Community.Appointment;
using Election = MudSharp.Community.Election;
using Rank = MudSharp.Community.Rank;

#nullable enable

namespace MudSharp_Unit_Tests.Community;

public partial class ClanElectionGovernanceTests
{
	[TestMethod]
	[DataRow(0)]
	[DataRow(365)]
	public void Template_LoadAndVacancyChecks_DoNotCreateOrRegisterElections(int termDays)
	{
		using var f = new Fixture();
		f.Clan.IsTemplate = true;
		f.Office.ElectionTerm = TimeSpan.FromDays(termDays);
		// Even legacy rows with unusable dates must never be instantiated for templates.
		f.Office.LoadElections([new MudSharp.Models.Election { Id = 99 }]);
		f.Clan.LoadElections();
		f.Office.CheckForByElections();
		var election = new Election(f.Office, false, 1, f.Now + TimeSpan.FromDays(365));
		f.Office.AddElection(election);
		Assert.IsEmpty(f.Office.Elections);
		Assert.IsFalse(f.Database.Context.Elections.Any());
		f.World.Verify(x => x.Add(It.IsAny<IElection>()), Times.Never);
		Assert.IsNull(ActiveListener(election));
	}

	[TestMethod]
	public void Template_EditorEnablesAndChangesTenure_OnlyStoresConfiguration()
	{
		using var f = new Fixture();
		f.Clan.IsTemplate = true;
		f.Office.IsAppointedByElection = false;
		EditElection(f, "enable");
		Assert.IsTrue(f.Office.IsAppointedByElection);
		Assert.AreEqual(TimeSpan.FromDays(365), f.Office.ElectionTerm);
		EditElection(f, "term 30");
		Assert.AreEqual(TimeSpan.FromDays(30), f.Office.ElectionTerm);
		EditElection(f, "term none");
		Assert.AreEqual(TimeSpan.Zero, f.Office.ElectionTerm);
		Assert.IsEmpty(f.Office.Elections);
		Assert.IsFalse(f.Database.Context.Elections.Any());
		f.Office.Save();
		var loaded = new Appointment(f.Database.Context.Appointments.Find(f.Office.Id)!, f.Clan);
		Assert.IsTrue(loaded.IsAppointedByElection);
		Assert.AreEqual(TimeSpan.FromDays(7), loaded.VotingPeriod);
		Assert.AreEqual(TimeSpan.Zero, loaded.ElectionTerm);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void TemplateToggle_SuspendsAndResumesListenerWithoutDuplicatingBallot(bool startsImmediately)
	{
		using var f = new Fixture();
		f.Office.ElectionTerm = TimeSpan.FromDays(365);
		var election = new Election(f.Office, false, 1, startsImmediately ? null : f.Now + TimeSpan.FromDays(365));
		f.Office.AddElection(election);
		Assert.IsNotNull(ActiveListener(election));
		f.Actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(true);
		void Toggle() => typeof(ClanModule).GetMethod("ClanEditTemplate", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [f.Actor.Object, new StringStack(""), f.Clan]);
		Toggle();
		Assert.IsTrue(f.Clan.IsTemplate);
		Assert.IsNull(ActiveListener(election));
		Assert.IsFalse(election.CheckElectionStage());
		Toggle();
		Assert.IsFalse(f.Clan.IsTemplate);
		Assert.IsNotNull(ActiveListener(election));
		f.Clan.LoadElections();
		Assert.HasCount(1, f.Office.Elections.ToList());
		Assert.AreSame(election, f.Office.Elections.Single());
		Assert.AreEqual(1, f.Database.Context.Elections.Count());
	}

	[TestMethod]
	public void Template_PendingBallotDoesNotBlockConfigurationOrReportSuccessfulParticipation()
	{
		using var f = new Fixture();
		var election = f.Ballot(isByElection: false);
		f.Clan.IsTemplate = true;
		EditElection(f, "confirmation none");
		Assert.IsNull(f.Office.ElectionConfirmationAppointment);
		EditElection(f, "term 30");
		Assert.AreEqual(TimeSpan.FromDays(30), f.Office.ElectionTerm);
		Assert.AreEqual(1, f.Database.Context.Elections.Count());
		Assert.AreEqual(ElectionStage.AwaitingConfirmation, election.ElectionStage);
		election.MoveTo(ElectionStage.Nomination);
		f.Clan.Nominate(f.Player.Object, new StringStack("Priest"));
		election.MoveTo(ElectionStage.Voting);
		f.Clan.Vote(f.Player.Object, new StringStack("Priest Winner"));
		f.Clan.WithdrawNomination(f.Player.Object, new StringStack("Priest"));
		Mock.Get(f.Player.Object.OutputHandler).Verify(x => x.Send("Template clans do not run elections.", It.IsAny<bool>(), It.IsAny<bool>()), Times.Exactly(3));
	}

	[TestMethod]
	public void Template_LegacyBallotCannotProgressParticipateConfirmOrVeto()
	{
		using var f = new Fixture();
		var election = f.Ballot();
		f.Clan.IsTemplate = true;
		Assert.IsFalse(election.Confirm(f.Actor.Object));
		Assert.IsFalse(election.Veto(f.Actor.Object));
		election.WithdrawNomination(f.Winner.Object);
		var other = f.Member(3, "Other");
		election.Nominate(other.Object);
		election.Vote(other.Object, f.Winner.Object, 5);
		election.MoveTo(ElectionStage.Voting);
		Assert.IsFalse(election.CheckElectionStage());
		Assert.AreEqual(ElectionStage.Voting, election.ElectionStage);
		Assert.IsFalse(election.IsFinalised);
		Assert.HasCount(1, election.Nominees.ToList());
		Assert.HasCount(1, election.Votes.ToList());
		Assert.IsEmpty(f.Winner.Object.Appointments);
		f.World.Verify(x => x.SystemMessage(It.IsAny<string>(), It.IsAny<Func<MudSharp.Character.ICharacter, bool>>()), Times.Never);
	}

	[TestMethod]
	public void Template_RecallDoesNotCreateBallot()
	{
		using var f = new Fixture();
		f.Clan.IsTemplate = true;
		f.Office.CanBeRecalled = true;
		f.Office.CanInitiateRecallProg = Mock.Of<IFutureProg>(x => x.ExecuteBool(f.Player.Object) == true);
		f.Office.RecallVotesProg = Mock.Of<IFutureProg>();
		f.Winner.Object.Appointments.Add(f.Office);
		f.Player.SetupGet(x => x.ClanMemberships).Returns([f.Winner.Object]);
		f.Clan.Recall(f.Player.Object, new StringStack("Winner Priest"));
		Assert.IsEmpty(f.Office.Elections);
		Assert.IsFalse(f.Database.Context.Elections.Any());
	}

	[TestMethod]
	[DataRow(365, false)]
	[DataRow(365, true)]
	[DataRow(0, false)]
	[DataRow(0, true)]
	public void CloneTemplate_InitializesFreshElectionsUsingCopiedRulesAndFoundingHolders(int termDays, bool founderHoldsOffice)
	{
		using var f = new Fixture();
		f.Clan.IsTemplate = true;
		f.Clan.Description = "Template";
		f.Clan.PayInterval = new RecurringInterval { Type = IntervalType.Daily, IntervalAmount = 7 };
		f.Database.Context.Clans.Add(new MudSharp.Models.Clan
		{
			Id = f.Clan.Id, Name = "Parish", Alias = "parish",
			PayIntervalReferenceDate = f.Now.Date.GetDateString(),
			PayIntervalReferenceTime = f.Now.Time.GetTimeString()
		});
		var rankRow = new MudSharp.Models.Rank { Id = 1, Name = "Member", RankPath = "General", ClanId = f.Clan.Id };
		f.Clan.Ranks.Add(new Rank(rankRow, f.Clan));
		f.Office.ElectionTerm = TimeSpan.FromDays(termDays);
		f.Office.ElectionLeadTime = TimeSpan.FromDays(3);
		f.Office.MaximumConsecutiveTerms = 2;
		f.Office.MaximumTotalTerms = 5;
		f.Office.IsSecretBallot = true;
		f.Office.CanBeRecalled = true;
		f.Office.RecallThreshold = 0.6;
		f.Office.Privileges = founderHoldsOffice ? ClanPrivilegeType.All : ClanPrivilegeType.None;
		var authorityRow = new MudSharp.Models.Appointment { Id = 2, Name = "Bishop", ClanId = f.Clan.Id };
		var authority = new Appointment(authorityRow, f.Clan);
		f.Clan.Appointments.Add(authority);
		f.Office.ElectionConfirmationAppointment = authority;
		f.Database.Context.SaveChanges();
		f.Actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(true);
		f.Actor.SetupGet(x => x.Id).Returns(10);
		f.Actor.SetupGet(x => x.Culture.PrimaryCalendar).Returns(f.Clan.Calendar);
		var name = new Mock<IPersonalName>();
		name.Setup(x => x.SaveToXml()).Returns(XElement.Parse("<Name culture='1'><Element usage='BirthName'>Founder</Element></Name>"));
		f.Actor.SetupGet(x => x.CurrentName).Returns(name.Object);
		f.World.SetupGet(x => x.NameCultures).Returns(new All<INameCulture>());
		var clans = new All<IClan> { f.Clan };
		f.World.SetupGet(x => x.Clans).Returns(clans);
		Clan? clone = null;
		f.World.Setup(x => x.Add(It.IsAny<IClan>())).Callback<IClan>(x =>
		{
			clone = (Clan)x;
			clans.Add(x);
		});
		try
		{
			typeof(ClanModule).GetMethod("ClanCreateTemplate", BindingFlags.NonPublic | BindingFlags.Static)!
				.Invoke(null, [f.Actor.Object, new StringStack("parish newparish New Parish")]);
		}
		catch (TargetInvocationException ex) when (ex.InnerException is not null)
		{
			ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
		}
		Assert.IsNotNull(clone);
		Assert.IsFalse(clone.IsTemplate);
		var office = clone.Appointments.Single(x => x.Name == "Priest");
		Assert.AreEqual(f.Office.ElectionTerm, office.ElectionTerm);
		Assert.AreEqual(f.Office.NominationPeriod, office.NominationPeriod);
		Assert.AreEqual(f.Office.VotingPeriod, office.VotingPeriod);
		Assert.AreEqual(f.Office.ElectionLeadTime, office.ElectionLeadTime);
		Assert.AreEqual(2, office.MaximumConsecutiveTerms);
		Assert.AreEqual(5, office.MaximumTotalTerms);
		Assert.IsTrue(office.IsSecretBallot);
		Assert.IsTrue(office.CanBeRecalled);
		Assert.AreEqual(0.6, office.RecallThreshold);
		Assert.AreSame(clone.Appointments.Single(x => x.Name == "Bishop"), office.ElectionConfirmationAppointment);
		if (termDays == 0 && founderHoldsOffice)
		{
			Assert.IsEmpty(office.Elections);
		}
		else
		{
			var election = office.Elections.Single();
			Assert.AreSame(office, election.Appointment);
			Assert.IsNotNull(ActiveListener((Election)election));
			Assert.AreEqual(termDays == 0, election.IsByElection);
			Assert.AreEqual(founderHoldsOffice ? ElectionStage.Preelection : ElectionStage.Nomination, election.ElectionStage);
			Assert.AreEqual(founderHoldsOffice ? f.Now + TimeSpan.FromDays(termDays) :
				f.Now + office.NominationPeriod + office.VotingPeriod + (election.IsByElection ? TimeSpan.Zero : office.ElectionLeadTime),
				election.ResultsInEffectDate);
			Assert.IsEmpty(election.Nominees);
			Assert.IsEmpty(election.Votes);
		}
		Assert.IsEmpty(f.Office.Elections);
		Assert.IsTrue(f.Database.Context.Elections.All(x => x.AppointmentId == office.Id));
	}

	private static object? ActiveListener(Election election) => typeof(Election)
		.GetField("_activeListener", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(election);

	private static void EditElection(Fixture f, string text) => typeof(ClanModule)
		.GetMethod("ClanEditAppointmentElections", BindingFlags.NonPublic | BindingFlags.Static)!
		.Invoke(null, [f.Actor.Object, new StringStack(text), f.Clan, f.Office]);
}

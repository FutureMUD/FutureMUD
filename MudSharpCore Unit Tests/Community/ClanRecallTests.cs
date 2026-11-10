using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Community;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.FutureProg;
using Election = MudSharp.Community.Election;
using Appointment = MudSharp.Community.Appointment;

#nullable enable

namespace MudSharp_Unit_Tests.Community;

public partial class ClanElectionGovernanceTests
{
	[TestMethod]
	public void ParentDismiss_OptInGrantsOnlyExactActiveParentRemovalAuthority()
	{
		using var f = new Fixture();
		var parent = Mock.Of<IAppointment>(x => x.Id == 8 && x.Clan == f.Clan);
		f.Office.ParentPosition = parent;
		var member = f.Member(3, "Parent");
		member.Object.Appointments.Add(parent);
		f.Actor.SetupGet(x => x.ClanMemberships).Returns([member.Object]);
		f.Winner.Object.Appointments.Add(f.Office);
		f.Clan.Dismiss(f.Actor.Object, new StringStack("Winner Priest"));
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		f.Office.CanParentDismiss = true;
		member.SetupGet(x => x.IsArchivedMembership).Returns(true);
		f.Clan.Dismiss(f.Actor.Object, new StringStack("Winner Priest"));
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		member.SetupGet(x => x.IsArchivedMembership).Returns(false);
		member.Object.Appointments.Clear();
		Assert.IsFalse(ClanCommandUtilities.CanParentDismiss(member.Object, f.Office));
		member.Object.Appointments.Add(parent);
		f.Clan.Dismiss(f.Actor.Object, new StringStack("Winner Priest"));
		Assert.IsEmpty(f.Winner.Object.Appointments);
		Assert.IsFalse(f.Office.CanDirectlyAppoint);
		Assert.HasCount(1, f.Office.Elections.Where(x => !x.IsFinalised && x.IsByElection).ToList());
		f.Clan.Appoint(f.Actor.Object, new StringStack("Winner Priest"));
		Assert.IsEmpty(f.Winner.Object.Appointments);
	}

	[TestMethod]
	public void Recall_InitiationUsesSeparateEligibilityAndRejectsDuplicateOrArchivedMember()
	{
		using var f = new Fixture();
		f.Office.CanBeRecalled = true;
		f.Winner.Object.Appointments.Add(f.Office);
		var eligible = new Mock<IFutureProg>();
		eligible.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(true);
		f.Office.CanNominateProg = eligible.Object;
		f.Office.NumberOfVotesProg = eligible.Object;
		f.Player.SetupGet(x => x.ClanMemberships).Returns([f.Winner.Object]);
		f.Clan.Recall(f.Player.Object, new StringStack("Winner Priest"));
		Assert.IsEmpty(f.Office.Elections);
		f.Office.CanInitiateRecallProg = eligible.Object;
		f.Office.RecallVotesProg = eligible.Object;
		f.Winner.SetupGet(x => x.IsArchivedMembership).Returns(true);
		f.Clan.Recall(f.Player.Object, new StringStack("Winner Priest"));
		Assert.IsEmpty(f.Office.Elections);
		f.Winner.SetupGet(x => x.IsArchivedMembership).Returns(false);
		f.Clan.Recall(f.Player.Object, new StringStack("Winner Priest"));
		f.Clan.Recall(f.Player.Object, new StringStack("Winner Priest"));
		Assert.HasCount(1, f.Office.Elections.ToList());
		Assert.AreEqual(ElectionStage.Voting, f.Office.Elections.Single().ElectionStage);
		Assert.AreSame(f.Winner.Object, f.Office.Elections.Single().RecallTarget);
	}

	[TestMethod]
	public void RecallVote_SeparateWeightReplacementAndMembershipAreEnforced()
	{
		using var f = new Fixture();
		var recall = StartRecall(f);
		f.World.SetupGet(x => x.Elections).Returns(new All<IElection> { recall });
		var prog = new Mock<IFutureProg>();
		prog.Setup(x => x.ExecuteInt(It.IsAny<object[]>())).Returns(3);
		f.Office.NumberOfVotesProg = prog.Object;
		f.Player.SetupGet(x => x.ClanMemberships).Returns([f.Winner.Object]);
		f.Clan.Vote(f.Player.Object, new StringStack($"{recall.Id} yes"));
		Assert.IsEmpty(recall.Votes);
		f.Office.RecallVotesProg = prog.Object;
		f.Clan.Vote(f.Actor.Object, new StringStack($"{recall.Id} yes"));
		Assert.IsEmpty(recall.Votes); // Confirmation authority grants no participation.
		f.Clan.Vote(f.Player.Object, new StringStack($"{recall.Id} yes"));
		Assert.AreEqual(3, recall.Votes.Single().Votes);
		f.Clan.Vote(f.Player.Object, new StringStack($"{recall.Id} no"));
		Assert.AreEqual(-3, recall.Votes.Single().Votes);
		f.Clan.Vote(f.Player.Object, new StringStack($"{recall.Id} nonsense"));
		Assert.AreEqual(-3, recall.Votes.Single().Votes);
		f.Winner.SetupGet(x => x.IsArchivedMembership).Returns(true);
		f.Clan.Vote(f.Player.Object, new StringStack($"{recall.Id} yes"));
		Assert.AreEqual(-3, recall.Votes.Single().Votes);
		Assert.AreEqual(ElectionStage.Cancelled, recall.ElectionStage);
	}

	[TestMethod]
	[DataRow(0, 0, 0.5, false)]
	[DataRow(1, 1, 0.5, false)]
	[DataRow(2, 1, 0.5, true)]
	[DataRow(3, 1, 0.75, false)]
	[DataRow(4, 1, 0.75, true)]
	public void Recall_ThresholdUsesStrictWeightedShareAndDoesNotInstallAnyone(int yes, int no, double threshold, bool passes)
	{
		using var f = new Fixture();
		f.Office.RecallThreshold = threshold;
		f.Office.ElectionConfirmationAppointment = null!;
		f.Office.MaximumSimultaneousHolders = 2;
		var other = f.Member(3, "Other");
		other.Object.Appointments.Add(f.Office);
		var recall = StartRecall(f);
		if (yes > 0) recall.Vote(f.Winner.Object, f.Winner.Object, yes);
		if (no > 0) recall.Vote(other.Object, f.Winner.Object, -no);
		CloseRecall(f, recall);
		Assert.AreEqual(passes, recall.RecallPassed);
		Assert.AreEqual(!passes, f.Winner.Object.Appointments.Contains(f.Office));
		Assert.Contains(f.Office, other.Object.Appointments);
		Assert.IsEmpty(recall.Victors);
		Assert.AreEqual(passes ? 1 : 0, f.Office.Elections.Count(x => !x.IsFinalised && x.IsByElection));
		Assert.IsNull(ClanCommandUtilities.GetPrimaryOpenElection(f.Office));
	}

	[TestMethod]
	public void Recall_SuccessWaitsForConfirmationAndSurvivesReloadWithOriginalThreshold()
	{
		using var f = new Fixture();
		var recall = StartRecall(f);
		recall.Vote(f.Winner.Object, f.Winner.Object, 2);
		var other = f.Member(3, "Other");
		recall.Vote(other.Object, f.Winner.Object, -1);
		CloseRecall(f, recall);
		Assert.AreEqual(ElectionStage.AwaitingConfirmation, recall.ElectionStage);
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		recall.Save();
		var stored = f.Database.Context.Elections.Find(recall.Id)!;
		stored.VotingEndDate = recall.VotingEndDate.GetDateTimeString();
		stored.ResultsInEffectDate = recall.ResultsInEffectDate.GetDateTimeString();
		f.Database.Context.SaveChanges();
		var loaded = new Election(f.Database.Context.Elections.Find(recall.Id)!, f.Office);
		f.Office.RemoveElection(recall);
		f.Office.AddElection(loaded);
		Assert.IsTrue(loaded.IsRecall);
		Assert.AreSame(f.Winner.Object, loaded.RecallTarget);
		Assert.IsFalse(loaded.CheckElectionStage());
		Assert.IsFalse(loaded.Confirm(f.Player.Object));
		Assert.IsTrue(loaded.Confirm(f.Actor.Object));
		Assert.IsTrue(loaded.IsFinalised);
		Assert.IsEmpty(f.Winner.Object.Appointments);
		Assert.AreEqual(-1, loaded.Votes.Single(x => x.Voter == other.Object).Votes);
		f.Office.RecallThreshold = 0.99;
		Assert.IsTrue(loaded.RecallPassed);
	}

	[TestMethod]
	public void Recall_VetoPreservesHolderVotesAndRegularCycleWithoutAnotherRecall()
	{
		using var f = new Fixture();
		f.Office.ElectionTerm = TimeSpan.FromDays(365);
		var primary = new Election(f.Office, false, 1, f.Now + TimeSpan.FromDays(365));
		f.Office.AddElection(primary);
		var recall = StartRecall(f);
		recall.Vote(f.Winner.Object, f.Winner.Object, 1);
		CloseRecall(f, recall);
		Assert.IsTrue(recall.Veto(f.Actor.Object));
		Assert.AreEqual(ElectionStage.Vetoed, recall.ElectionStage);
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		Assert.HasCount(1, recall.Votes.ToList());
		Assert.AreSame(primary, ClanCommandUtilities.GetPrimaryOpenElection(f.Office));
		Assert.HasCount(2, f.Office.Elections.ToList());
	}

	[TestMethod]
	public void Recall_DismissalCancelsPendingRecallEvenIfHolderIsReappointed()
	{
		using var f = new Fixture();
		var recall = StartRecall(f);
		recall.Vote(f.Winner.Object, f.Winner.Object, 1);
		CloseRecall(f, recall);
		f.Clan.DismissAppointment(f.Winner.Object, f.Office);
		f.Winner.Object.Appointments.Add(f.Office);
		Assert.AreEqual(ElectionStage.Cancelled, recall.ElectionStage);
		Assert.IsFalse(recall.Confirm(f.Actor.Object));
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
	}

	[TestMethod]
	public void Recall_EmploymentGrantedOfficeCannotBeRecalledOrRemovedByPendingBallot()
	{
		using var f = new Fixture();
		var recall = StartRecall(f);
		recall.Vote(f.Winner.Object, f.Winner.Object, 1);
		CloseRecall(f, recall);
		var listing = Mock.Of<IJobListing>(x => x.ClanMembership == f.Clan && x.ClanAppointment == f.Office);
		f.Player.SetupGet(x => x.Id).Returns(f.Winner.Object.MemberId);
		var job = Mock.Of<IActiveJob>(x => x.Id == 1 && x.Character == f.Player.Object && x.Listing == listing);
		f.World.SetupGet(x => x.ActiveJobs).Returns(new All<IActiveJob> { job });
		Assert.IsTrue(recall.Confirm(f.Actor.Object));
		Assert.AreEqual(ElectionStage.Cancelled, recall.ElectionStage);
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		f.Office.CanBeRecalled = true;
		f.Office.CanInitiateRecallProg = Mock.Of<IFutureProg>(x => x.ExecuteBool(It.IsAny<object[]>()) == true);
		f.Office.RecallVotesProg = f.Office.CanInitiateRecallProg;
		f.Player.SetupGet(x => x.ClanMemberships).Returns([f.Winner.Object]);
		f.Clan.Recall(f.Player.Object, new StringStack("Winner Priest"));
		Assert.HasCount(1, f.Office.Elections.ToList());
	}

	[TestMethod]
	public void Recall_ClosedSecretBallotCannotBeChangedAndOnlyPublishesTotals()
	{
		using var f = new Fixture();
		f.Office.IsSecretBallot = true;
		var recall = StartRecall(f);
		recall.Vote(f.Winner.Object, f.Winner.Object, 2);
		Assert.IsFalse(recall.Show(f.Player.Object).Contains("Remove:"));
		CloseRecall(f, recall);
		recall.Vote(f.Winner.Object, f.Winner.Object, -2);
		Assert.AreEqual(2, recall.Votes.Single().Votes);
		StringAssert.Contains(recall.Show(f.Player.Object), "Remove:");
		Assert.IsFalse(recall.Show(f.Player.Object).Contains(": 2 to remove"));
	}

	[TestMethod]
	public void RecallBuilder_ThresholdValidationAndOpenBallotPolicyProtection()
	{
		using var f = new Fixture();
		EditRecall(f, "recall threshold 75");
		Assert.AreEqual(0.75, f.Office.RecallThreshold);
		foreach (var invalid in new[] { "-1", "100", "NaN", "Infinity", "nonsense" })
			EditRecall(f, $"recall threshold {invalid}");
		Assert.AreEqual(0.75, f.Office.RecallThreshold);
		f.Office.CanBeRecalled = true;
		StartRecall(f);
		EditRecall(f, "recall threshold 20");
		EditRecall(f, "recall none");
		Assert.AreEqual(0.75, f.Office.RecallThreshold);
		Assert.IsTrue(f.Office.CanBeRecalled);
		EditRecall(f, "none");
		Assert.IsFalse(f.Office.CanBeRecalled);
		Assert.IsEmpty(f.Office.Elections);
	}

	[TestMethod]
	public void RecallPolicy_SaveReloadPreservesOptInsThresholdAndSeparateProgs()
	{
		using var f = new Fixture();
		Assert.IsFalse(f.Office.CanParentDismiss);
		Assert.IsFalse(f.Office.CanBeRecalled);
		Assert.AreEqual(0.5, f.Office.RecallThreshold);
		var initiate = Mock.Of<IFutureProg>(x => x.Id == 80);
		var votes = Mock.Of<IFutureProg>(x => x.Id == 81);
		f.World.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg> { initiate, votes });
		f.Office.CanParentDismiss = f.Office.CanBeRecalled = true;
		f.Office.RecallThreshold = 0.75;
		f.Office.CanInitiateRecallProg = initiate;
		f.Office.RecallVotesProg = votes;
		f.Office.Save();
		var loaded = new Appointment(f.Database.Context.Appointments.Find(f.Office.Id)!, f.Clan);
		Assert.IsTrue(loaded.CanParentDismiss);
		Assert.IsTrue(loaded.CanBeRecalled);
		Assert.AreEqual(0.75, loaded.RecallThreshold);
		Assert.AreSame(initiate, loaded.CanInitiateRecallProg);
		Assert.AreSame(votes, loaded.RecallVotesProg);
	}

	private static Election StartRecall(Fixture f)
	{
		if (!f.Winner.Object.Appointments.Contains(f.Office)) f.Winner.Object.Appointments.Add(f.Office);
		var recall = new Election(f.Office, false, 1, null, f.Winner.Object);
		f.Office.AddElection(recall);
		return recall;
	}

	private static void CloseRecall(Fixture f, Election recall)
	{
		recall.VotingEndDate = recall.ResultsInEffectDate = f.Now - TimeSpan.FromDays(1);
		recall.CheckElectionStage();
	}

	private static void EditRecall(Fixture f, string command) => typeof(MudSharp.Commands.Modules.ClanModule)
		.GetMethod("ClanEditAppointmentElections", BindingFlags.Static | BindingFlags.NonPublic)!
		.Invoke(null, [f.Actor.Object, new StringStack(command), f.Clan, f.Office]);
}

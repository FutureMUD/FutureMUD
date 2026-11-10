using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.TimeAndDate.Listeners;
using Appointment = MudSharp.Community.Appointment;
using Election = MudSharp.Community.Election;

#nullable enable

namespace MudSharp_Unit_Tests.Community;

public partial class ClanElectionGovernanceTests
{
	[TestMethod]
	[DataRow(true, 0, false)]
	[DataRow(false, 0, false)]
	[DataRow(true, 365, false)]
	[DataRow(false, 365, false)]
	[DataRow(true, 0, true)]
	public void Election_InstallationPromotesOnlyBelowMinimumRank(bool byElection, int termDays, bool higherRank)
	{
		using var f = new Fixture();
		var minimum = Mock.Of<IRank>(x => x.Id == 10 && x.RankNumber == 2);
		var previous = Mock.Of<IRank>(x => x.Id == 11 && x.RankNumber == (higherRank ? 3 : 1));
		f.Winner.SetupProperty(x => x.Rank, previous);
		f.Winner.Setup(x => x.SetRank(It.IsAny<IRank>())).Callback<IRank>(rank => f.Winner.Object.Rank = rank);
		f.Office.MinimumRankToHold = minimum;
		f.Office.ElectionTerm = TimeSpan.FromDays(termDays);
		var election = f.Ballot(isByElection: byElection);
		Assert.AreSame(previous, f.Winner.Object.Rank);
		Assert.IsTrue(election.Confirm(f.Actor.Object));
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		Assert.AreSame(higherRank ? previous : minimum, f.Winner.Object.Rank);
		Assert.IsTrue(f.Winner.Object.Changed);
	}

	[TestMethod]
	public void Dismiss_RootHolderCannotBypassResignationRulesButAdministratorCanRemove()
	{
		using var f = new Fixture();
		f.Office.IsAppointedByElection = false;
		f.Office.CanResign = false;
		f.Winner.Object.Appointments.Add(f.Office);
		f.Winner.SetupGet(x => x.NetPrivileges).Returns(ClanPrivilegeType.CanDismiss);
		f.Player.SetupGet(x => x.Id).Returns(f.Winner.Object.MemberId);
		f.Player.SetupGet(x => x.ClanMemberships).Returns([f.Winner.Object]);
		f.Clan.Dismiss(f.Player.Object, new StringStack("Winner Priest"));
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		f.Clan.Resign(f.Player.Object, new StringStack("Priest"));
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		f.Office.CanResign = true;
		f.Clan.Resign(f.Player.Object, new StringStack("Priest"));
		Assert.IsEmpty(f.Winner.Object.Appointments);
		f.Winner.Object.Appointments.Add(f.Office);
		f.Office.CanResign = false;
		f.Player.Setup(x => x.IsAdministrator(PermissionLevel.Admin)).Returns(true);
		f.Clan.Dismiss(f.Player.Object, new StringStack("Winner Priest"));
		Assert.IsEmpty(f.Winner.Object.Appointments);
	}

	[TestMethod]
	public void Dismiss_ExternallyAppointedRootRequiresLiegeDismissalAndKeepsOtherLocalHoldersDismissible()
	{
		using var f = new Fixture();
		f.Office.IsAppointedByElection = false;
		f.Winner.Object.Appointments.Add(f.Office);
		var local = f.Member(4, "Local");
		local.Object.Appointments.Add(f.Office);
		var officer = f.Member(5, "Officer");
		officer.SetupGet(x => x.NetPrivileges).Returns(ClanPrivilegeType.CanDismiss);
		f.Player.SetupGet(x => x.Id).Returns(5);
		f.Player.SetupGet(x => x.ClanMemberships).Returns([officer.Object]);
		AddRecallControl(f);
		f.Clan.Dismiss(f.Player.Object, new StringStack("Winner Priest"));
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		f.Clan.Dismiss(f.Player.Object, new StringStack("Local Priest"));
		Assert.IsEmpty(local.Object.Appointments);
		f.Actor.SetupGet(x => x.ClanMemberships).Returns([LiegeMembership(f)]);
		f.Clan.DismissExternal(f.Actor.Object, new StringStack("Winner Priest"));
		Assert.IsEmpty(f.Winner.Object.Appointments);
		Assert.IsEmpty(f.Clan.ExternalControls.Single().Appointees);
	}

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void Recall_BothAuthoritiesMustApproveAndPartialApprovalSurvivesReload(bool liegeFirst)
	{
		using var f = new Fixture();
		AddRecallControl(f);
		var recall = StartRecall(f);
		recall.Vote(f.Winner.Object, f.Winner.Object, 1);
		CloseRecall(f, recall);
		Assert.AreEqual(ElectionStage.AwaitingConfirmation, recall.ElectionStage);
		Assert.IsFalse(recall.Confirm(f.Player.Object));
		var localAuthority = new Mock<MudSharp.Character.ICharacter>();
		localAuthority.SetupGet(x => x.ClanMemberships).Returns([f.AuthorityMembership.Object]);
		localAuthority.SetupGet(x => x.PersonalName).Returns(f.Actor.Object.PersonalName);
		// The liege is independent of the configured confirmation office.
		f.Actor.SetupGet(x => x.ClanMemberships).Returns([LiegeMembership(f)]);
		Assert.IsTrue(recall.Confirm(liegeFirst ? f.Actor.Object : localAuthority.Object));
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		Assert.AreEqual(ElectionStage.AwaitingConfirmation, recall.ElectionStage);
		recall.Save();
		var pendingRow = f.Database.Context.Elections.Find(recall.Id)!;
		pendingRow.VotingEndDate = recall.VotingEndDate.GetDateTimeString();
		pendingRow.ResultsInEffectDate = recall.ResultsInEffectDate.GetDateTimeString();
		f.Database.Context.SaveChanges();
		f.Database.Context.ChangeTracker.Clear();
		var stored = f.Database.Context.Elections.Include(x => x.ElectionNominees).Include(x => x.ElectionVotes).Single(x => x.Id == recall.Id);
		var loaded = new Election(stored, f.Office);
		f.Office.RemoveElection(recall);
		f.Office.AddElection(loaded);
		Assert.AreEqual(ElectionStage.AwaitingConfirmation, loaded.ElectionStage);
		Assert.IsTrue(loaded.Confirm(liegeFirst ? localAuthority.Object : f.Actor.Object));
		Assert.IsTrue(loaded.IsFinalised);
		Assert.IsTrue(ClanCommandUtilities.CanConfirmElection(f.Actor.Object, loaded));
		Assert.IsFalse(ClanCommandUtilities.CanConfirmElection(f.Actor.Object, f.Office));
		Assert.IsEmpty(f.Winner.Object.Appointments);
		Assert.IsEmpty(f.Clan.ExternalControls.Single().Appointees);
	}

	[TestMethod]
	public void Recall_ExternalControlAloneRequiresApprovalAndAllowsLiegeVeto()
	{
		using var f = new Fixture();
		f.Office.ElectionConfirmationAppointment = null!;
		AddRecallControl(f);
		f.Actor.SetupGet(x => x.ClanMemberships).Returns([LiegeMembership(f)]);
		var recall = StartRecall(f);
		recall.Vote(f.Winner.Object, f.Winner.Object, 1);
		CloseRecall(f, recall);
		Assert.AreEqual(ElectionStage.AwaitingConfirmation, recall.ElectionStage);
		Assert.IsFalse(recall.IsFinalised);
		Assert.IsTrue(recall.Veto(f.Actor.Object));
		Assert.AreEqual(ElectionStage.Vetoed, recall.ElectionStage);
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
	}

	[TestMethod]
	public void Recall_ArchivedLiegeCannotApproveAndControllingOfficeChangesInvalidateApproval()
	{
		using var f = new Fixture();
		var control = AddRecallControl(f);
		var membership = Mock.Get(LiegeMembership(f));
		f.Actor.SetupGet(x => x.ClanMemberships).Returns([membership.Object]);
		var recall = StartRecall(f);
		recall.Vote(f.Winner.Object, f.Winner.Object, 1);
		CloseRecall(f, recall);
		membership.SetupGet(x => x.IsArchivedMembership).Returns(true);
		Assert.IsFalse(recall.Confirm(f.Actor.Object));
		membership.SetupGet(x => x.IsArchivedMembership).Returns(false);
		Assert.IsTrue(recall.Confirm(f.Actor.Object));
		control.ControllingAppointment = Mock.Of<IAppointment>(x => x.Id == 40);
		f.Actor.SetupGet(x => x.ClanMemberships).Returns([f.AuthorityMembership.Object]);
		Assert.IsTrue(recall.Confirm(f.Actor.Object));
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		Assert.AreEqual(ElectionStage.AwaitingConfirmation, recall.ElectionStage);
	}

	[TestMethod]
	public void DeleteAppointment_RealLifecycleDeletesOfficeElectionsListenersAndBothExternalReferences()
	{
		using var f = new Fixture();
		var control = AddRecallControl(f);
		var election = f.Ballot();
		var listener = new Mock<ITemporalListener>();
		typeof(Election).GetField("_activeListener", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(election, listener.Object);
		f.Winner.Object.Appointments.Add(f.Office);
		var childRow = new MudSharp.Models.Appointment { Id = 5, ClanId = 1, Name = "Child", ParentAppointmentId = f.Office.Id };
		f.Database.Context.Appointments.Add(childRow);
		f.Database.Context.SaveChanges();
		var child = new Appointment(childRow, f.Clan) { ParentPosition = f.Office };
		f.Clan.Appointments.Add(child);
		var budgetRow = new MudSharp.Models.ClanBudget { Id = 5, ClanId = 1, AppointmentId = 1, Name = "Budget",
			CurrentPeriodStart = f.Now.GetDateTimeString(), CurrentPeriodEnd = f.Now.GetDateTimeString() };
		f.Database.Context.ClanBudgets.Add(budgetRow);
		f.Database.Context.SaveChanges();
		f.Clan.Budgets.Add(new ClanBudget(budgetRow, f.Clan));
		var membershipRow = new MudSharp.Models.ClanMembership { ClanId = 1, CharacterId = 70, RankId = 1, JoinDate = "1/jan/2026", PersonalName = "Test" };
		membershipRow.ClanMembershipsAppointments.Add(new MudSharp.Models.ClanMembershipsAppointments { AppointmentId = 1 });
		f.Database.Context.ClanMemberships.Add(membershipRow);
		f.Database.Context.SaveChanges();
		var membership = new ClanMembership(f.World.Object) { Clan = f.Clan, MemberId = 70,
			Rank = Mock.Of<IRank>(x => x.Id == 1), JoinDate = f.Clan.Calendar.CurrentDate };
		membership.Appointments.Add(f.Office);
		f.Clan.Memberships.Add(membership);
		Mock.Get(f.World.Object.SaveManager).Setup(x => x.Flush()).Callback(() =>
		{
			if (membership.Changed) membership.Save();
			if (child.Changed) child.Save();
		});
		var roleMembership = Mock.Of<MudSharp.CharacterCreation.Roles.IRoleClanMembership>(x => x.Clan == f.Clan &&
			x.Appointments == new System.Collections.Generic.List<IAppointment> { f.Office });
		var role = new Mock<MudSharp.CharacterCreation.Roles.IChargenRole>();
		role.SetupGet(x => x.Id).Returns(4);
		role.SetupProperty(x => x.Changed);
		role.SetupGet(x => x.ClanMemberships).Returns([roleMembership]);
		f.World.SetupGet(x => x.Roles).Returns(new All<MudSharp.CharacterCreation.Roles.IChargenRole> { role.Object });
		f.Database.Context.ChargenRolesClanMembershipsAppointments.Add(new MudSharp.Models.ChargenRolesClanMembershipsAppointments { ChargenRoleId = 4, ClanId = 1, AppointmentId = 1 });
		f.Database.Context.SaveChanges();
		f.Clan.DeleteAppointment(f.Office);
		child.Save();
		f.Database.Context.SaveChanges();
		Assert.IsFalse(f.Clan.Appointments.Contains(f.Office));
		Assert.IsEmpty(f.Winner.Object.Appointments);
		Assert.IsNull(child.ParentPosition);
		Assert.IsEmpty(f.Office.Elections);
		listener.Verify(x => x.CancelListener(), Times.Once);
		f.World.Verify(x => x.Destroy(election), Times.Once);
		Assert.IsEmpty(f.Clan.ExternalControls);
		Assert.IsEmpty(control.LiegeClan.ExternalControls);
		Assert.IsEmpty(f.Clan.Budgets);
		Assert.IsEmpty(membership.Appointments);
		Assert.IsEmpty(roleMembership.Appointments);
		Assert.IsTrue(role.Object.Changed);
		f.Database.Context.ChangeTracker.Clear();
		Assert.IsNull(f.Database.Context.Appointments.Find(f.Office.Id));
		Assert.IsNull(f.Database.Context.Elections.Find(election.Id));
		Assert.IsEmpty(f.Database.Context.ExternalClanControls.ToList());
		Assert.IsEmpty(f.Database.Context.ClanBudgets.ToList());
		Assert.IsEmpty(f.Database.Context.ClanMembershipsAppointments.ToList());
		Assert.IsEmpty(f.Database.Context.ChargenRolesClanMembershipsAppointments.ToList());
		Assert.IsNull(f.Database.Context.Appointments.Find(child.Id)!.ParentAppointmentId);
	}

	[TestMethod]
	public void Recall_ControlChangedAwayAndBackRevokesApprovalAndPersistsRevocation()
	{
		using var f = new Fixture();
		var control = AddRecallControl(f);
		var office = Mock.Of<IAppointment>(x => x.Id == 40 && x.Name == "Steward" && x.Clan == control.LiegeClan);
		Mock.Get(control.LiegeClan).SetupGet(x => x.Appointments).Returns([office]);
		f.Actor.SetupGet(x => x.ClanMemberships).Returns([LiegeMembership(f)]);
		var recall = StartRecall(f);
		recall.Vote(f.Winner.Object, f.Winner.Object, 1);
		CloseRecall(f, recall);
		Assert.IsTrue(recall.Confirm(f.Actor.Object));
		recall.Save();
		Assert.IsFalse(recall.Changed);
		f.Clan.SetControllingAppointment(f.Actor.Object, new StringStack("Priest Steward"));
		f.Clan.SetControllingAppointment(f.Actor.Object, new StringStack("Priest none"));
		Assert.IsTrue(recall.Changed);
		recall.Save();
		var row = f.Database.Context.Elections.Find(recall.Id)!;
		row.VotingEndDate = recall.VotingEndDate.GetDateTimeString();
		row.ResultsInEffectDate = recall.ResultsInEffectDate.GetDateTimeString();
		f.Database.Context.SaveChanges();
		var loaded = new Election(row, f.Office);
		f.Office.RemoveElection(recall);
		f.Office.AddElection(loaded);
		f.Actor.SetupGet(x => x.ClanMemberships).Returns([f.AuthorityMembership.Object]);
		Assert.IsTrue(loaded.Confirm(f.Actor.Object));
		Assert.AreEqual(ElectionStage.AwaitingConfirmation, loaded.ElectionStage);
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		f.Actor.SetupGet(x => x.ClanMemberships).Returns([LiegeMembership(f)]);
		Assert.IsTrue(loaded.Confirm(f.Actor.Object));
		Assert.IsTrue(loaded.IsFinalised);
	}

	[TestMethod]
	public void Recall_ControllingOfficeAndEveryLiegeMustApproveWithoutOrdinaryElectionAuthority()
	{
		using var f = new Fixture();
		f.Office.ElectionConfirmationAppointment = null!;
		var first = AddRecallControl(f);
		var scoped = Mock.Of<IAppointment>(x => x.Id == 30 && x.Clan == first.LiegeClan);
		Mock.Get(first.LiegeClan).SetupGet(x => x.Appointments).Returns([scoped]);
		first.ControllingAppointment = scoped;
		var firstMember = Mock.Of<IClanMembership>(x => x.Clan == first.LiegeClan &&
			x.Appointments == new System.Collections.Generic.List<IAppointment> { scoped });
		f.Actor.SetupGet(x => x.ClanMemberships).Returns([firstMember]);
		var second = new Mock<IExternalClanControl>();
		second.SetupGet(x => x.VassalClan).Returns(f.Clan);
		second.SetupGet(x => x.ControlledAppointment).Returns(f.Office);
		second.SetupGet(x => x.Appointees).Returns([f.Winner.Object]);
		second.SetupGet(x => x.LiegeClan).Returns(Mock.Of<IClan>(x => x.Id == 4));
		f.Clan.ExternalControls.Add(second.Object);
		var recall = StartRecall(f);
		recall.Vote(f.Winner.Object, f.Winner.Object, 1);
		CloseRecall(f, recall);
		Assert.IsTrue(recall.Confirm(f.Actor.Object));
		Assert.IsFalse(recall.IsFinalised);
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		Assert.IsFalse(ClanCommandUtilities.CanConfirmElection(f.Actor.Object, f.Office));
		f.Actor.SetupGet(x => x.ClanMemberships).Returns([Mock.Of<IClanMembership>(x =>
			x.Clan == second.Object.LiegeClan && x.NetPrivileges == ClanPrivilegeType.CanManageClanVassals)]);
		Assert.IsTrue(recall.Confirm(f.Actor.Object));
		Assert.IsTrue(recall.IsFinalised);
		Assert.IsEmpty(f.Winner.Object.Appointments);
		Assert.AreEqual(2, recall.RecallConfirmationAuthorities.Count());
	}

	[TestMethod]
	public void DeleteAppointment_ControllingOfficeRemovesRelationshipFromBothClans()
	{
		using var f = new Fixture();
		var vassal = new Mock<IClan>();
		vassal.SetupGet(x => x.Id).Returns(3);
		vassal.SetupGet(x => x.ExternalControls).Returns([]);
		var controlled = Mock.Of<IAppointment>(x => x.Id == 30 && x.Clan == vassal.Object);
		vassal.SetupGet(x => x.Appointments).Returns([controlled]);
		f.World.SetupGet(x => x.Clans).Returns(new All<IClan> { f.Clan, f.Authority.Object.Clan, vassal.Object });
		var row = new MudSharp.Models.ExternalClanControl { VassalClanId = 3, LiegeClanId = 1, ControlledAppointmentId = 30,
			ControllingAppointmentId = 1, NumberOfAppointments = 1 };
		f.Database.Context.ExternalClanControls.Add(row);
		f.Database.Context.SaveChanges();
		_ = new ExternalClanControl(row, f.World.Object);
		f.Clan.DeleteAppointment(f.Office);
		Assert.IsEmpty(f.Clan.ExternalControls);
		Assert.IsEmpty(vassal.Object.ExternalControls);
		Assert.Contains(controlled, vassal.Object.Appointments);
		f.Database.Context.ChangeTracker.Clear();
		Assert.IsEmpty(f.Database.Context.ExternalClanControls.ToList());
		Assert.IsNull(f.Database.Context.Appointments.Find(f.Office.Id));
	}

	[TestMethod]
	public void DeleteAppointment_JobListingReferenceRejectsBeforeCleanup()
	{
		using var f = new Fixture();
		f.Winner.Object.Appointments.Add(f.Office);
		f.Database.Context.JobListings.Add(new MudSharp.Models.JobListing { Id = 5, AppointmentId = f.Office.Id,
			Name = "Office job", Description = "Test", Definition = "<Definition/>", JobListingType = "Test", PosterType = "Character", MoneyPaidIn = "<Money/>" });
		f.Database.Context.SaveChanges();
		Assert.ThrowsExactly<InvalidOperationException>(() => f.Clan.DeleteAppointment(f.Office));
		Assert.Contains(f.Office, f.Clan.Appointments);
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		Assert.IsNotNull(f.Database.Context.Appointments.Find(f.Office.Id));
	}

	[TestMethod]
	public void Election_PromotionRepairsPaygradeAndPersistsRankOnlyWhenInstalled()
	{
		using var f = new Fixture();
		var oldPay = Mock.Of<IPaygrade>(x => x.Id == 1);
		var newPay = Mock.Of<IPaygrade>(x => x.Id == 2 && x.PayAmount == 10);
		var previous = Mock.Of<IRank>(x => x.Id == 1 && x.RankNumber == 1);
		var minimum = Mock.Of<IRank>(x => x.Id == 2 && x.RankNumber == 2 &&
			x.Paygrades == new System.Collections.Generic.List<IPaygrade> { newPay });
		var member = new ClanMembership(f.World.Object) { Clan = f.Clan, MemberId = 2, Rank = previous,
			Paygrade = oldPay, PersonalName = f.Winner.Object.PersonalName, JoinDate = f.Clan.Calendar.CurrentDate };
		f.Clan.Memberships.Remove(f.Winner.Object);
		f.Clan.Memberships.Add(member);
		f.Database.Context.ClanMemberships.Add(new MudSharp.Models.ClanMembership { ClanId = 1, CharacterId = 2,
			RankId = 1, PaygradeId = 1, PersonalName = "Test", JoinDate = "1/jan/2026" });
		f.Database.Context.SaveChanges();
		f.Office.MinimumRankToHold = minimum;
		var election = f.Ballot();
		election.WithdrawNomination(f.Winner.Object);
		election.Nominate(member);
		election.Vote(member, member, 1);
		Assert.AreSame(previous, member.Rank);
		Assert.IsTrue(election.Confirm(f.Actor.Object));
		Assert.AreSame(minimum, member.Rank);
		Assert.AreSame(newPay, member.Paygrade);
		member.Save();
		f.Database.Context.ChangeTracker.Clear();
		var saved = f.Database.Context.ClanMemberships.Find(1L, 2L)!;
		Assert.AreEqual(2L, saved.RankId);
		Assert.AreEqual(2L, saved.PaygradeId);
	}

	[TestMethod]
	public void DeleteAppointment_ConfirmationReferenceRejectsBeforeAnyCleanup()
	{
		using var f = new Fixture();
		f.Winner.Object.Appointments.Add(f.Office);
		var dependent = Mock.Of<IAppointment>(x => x.Id == 60 && x.ElectionConfirmationAppointment == f.Office);
		f.Clan.Appointments.Add(dependent);
		Assert.ThrowsExactly<InvalidOperationException>(() => f.Clan.DeleteAppointment(f.Office));
		Assert.Contains(f.Office, f.Winner.Object.Appointments);
		Assert.Contains(f.Office, f.Clan.Appointments);
		Assert.IsNotNull(f.Database.Context.Appointments.Find(f.Office.Id));
	}

	private static IClanMembership LiegeMembership(Fixture f) => Mock.Of<IClanMembership>(x =>
		x.Clan == f.Clan.ExternalControls.Single().LiegeClan && x.NetPrivileges == ClanPrivilegeType.CanManageClanVassals &&
		x.Appointments == new System.Collections.Generic.List<IAppointment>());

	private static ExternalClanControl AddRecallControl(Fixture f)
	{
		var liege = new Mock<IClan>();
		liege.SetupGet(x => x.Id).Returns(3);
		liege.SetupGet(x => x.FullName).Returns("Liege");
		liege.SetupGet(x => x.ExternalControls).Returns([]);
		liege.SetupGet(x => x.Appointments).Returns([]);
		f.World.SetupGet(x => x.Clans).Returns(new All<IClan> { f.Clan, f.Authority.Object.Clan, liege.Object });
		var row = new MudSharp.Models.ExternalClanControl { VassalClanId = 1, LiegeClanId = 3, ControlledAppointmentId = 1, NumberOfAppointments = 1 };
		f.Database.Context.ExternalClanControls.Add(row);
		f.Database.Context.SaveChanges();
		var control = new ExternalClanControl(row, f.World.Object);
		control.Appointees.Add(f.Winner.Object);
		return control;
	}
}

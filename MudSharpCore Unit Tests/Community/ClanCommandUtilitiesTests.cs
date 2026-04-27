using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Community;
using MudSharp.TimeAndDate;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests.Community;

[TestClass]
public class ClanCommandUtilitiesTests
{
	[TestMethod]
	public void HoldsOrControlsAppointment_WhenMembershipHoldsParentAppointment_ReturnsTrueForChildAppointment()
	{
		Mock<IAppointment> parent = new();
		parent.SetupGet(x => x.ParentPosition).Returns((IAppointment)null);

		Mock<IAppointment> child = new();
		child.SetupGet(x => x.ParentPosition).Returns(parent.Object);

		Mock<IClanMembership> membership = new();
		membership.SetupGet(x => x.Appointments).Returns(new List<IAppointment> { parent.Object });

		Assert.IsTrue(ClanCommandUtilities.HoldsOrControlsAppointment(membership.Object, child.Object));
	}

	[TestMethod]
	public void HasReachedTotalTermLimit_WhenVictoriesMatchLimit_ReturnsTrue()
	{
		List<IElection> elections =
		[
			CreateElectionMock(isFinalised: true, isByElection: false, victorIds: [7]).Object,
			CreateElectionMock(isFinalised: true, isByElection: false, victorIds: [7]).Object
		];

		Assert.IsTrue(ClanCommandUtilities.HasReachedTotalTermLimit(elections, 7, 2));
	}

	[TestMethod]
	public void HasReachedConsecutiveTermLimit_IgnoresByElections_WhenLatestRegularTermsAreWins()
	{
		List<IElection> elections =
		[
			CreateElectionMock(isFinalised: true, isByElection: false, victorIds: [7]).Object,
			CreateElectionMock(isFinalised: true, isByElection: true, victorIds: [99]).Object,
			CreateElectionMock(isFinalised: true, isByElection: false, victorIds: [7]).Object
		];

		Assert.IsTrue(ClanCommandUtilities.HasReachedConsecutiveTermLimit(elections, 7, 2));
	}

	[TestMethod]
	public void HasFreePosition_WhenInternalHoldersAndUnusedExternalReservationsFillCapacity_ReturnsFalse()
	{
		Mock<IClan> clan = new();

		Mock<IAppointment> appointment = new();
		appointment.SetupGet(x => x.Clan).Returns(clan.Object);
		appointment.SetupGet(x => x.MaximumSimultaneousHolders).Returns(2);

		List<IClanMembership> memberships =
		[
			CreateMembershipMock(1, false, appointment.Object).Object
		];

		List<IExternalClanControl> externalControls =
		[
			CreateExternalControlMock(clan.Object, appointment.Object, numberOfAppointments: 1, appointeeCount: 0).Object
		];

		Assert.IsFalse(ClanCommandUtilities.HasFreePosition(appointment.Object, memberships, externalControls));
	}

	[TestMethod]
	public void HasFreePosition_IgnoresArchivedMembersOtherAppointmentsAndFilledReservations()
	{
		Mock<IClan> clan = new();

		Mock<IAppointment> appointment = new();
		appointment.SetupGet(x => x.Clan).Returns(clan.Object);
		appointment.SetupGet(x => x.MaximumSimultaneousHolders).Returns(3);

		Mock<IAppointment> otherAppointment = new();
		otherAppointment.SetupGet(x => x.Clan).Returns(clan.Object);

		List<IClanMembership> memberships =
		[
			CreateMembershipMock(1, false, appointment.Object).Object,
			CreateMembershipMock(2, true, appointment.Object).Object
		];

		List<IExternalClanControl> externalControls =
		[
			CreateExternalControlMock(clan.Object, appointment.Object, numberOfAppointments: 1, appointeeCount: 1).Object,
			CreateExternalControlMock(clan.Object, otherAppointment.Object, numberOfAppointments: 5, appointeeCount: 0).Object
		];

		Assert.IsTrue(ClanCommandUtilities.HasFreePosition(appointment.Object, memberships, externalControls));
	}

	[TestMethod]
	public void GetUncoveredAppointmentVacancies_SubtractsOpenByElectionSeats()
	{
		Mock<IClan> clan = new();

		Mock<IAppointment> appointment = new();
		appointment.SetupGet(x => x.Clan).Returns(clan.Object);
		appointment.SetupGet(x => x.MaximumSimultaneousHolders).Returns(3);

		Mock<IElection> openByElection = CreateElectionMock(isFinalised: false, isByElection: true,
			numberOfAppointments: 1);
		appointment.SetupGet(x => x.Elections).Returns([openByElection.Object]);

		List<IClanMembership> memberships =
		[
			CreateMembershipMock(1, false, appointment.Object).Object
		];

		Assert.AreEqual(1,
			ClanCommandUtilities.GetUncoveredAppointmentVacancies(appointment.Object, memberships, []));
	}

	[TestMethod]
	public void GetUncoveredAppointmentVacancies_ReturnsZeroWhenExistingByElectionCoversVacancy()
	{
		Mock<IClan> clan = new();

		Mock<IAppointment> appointment = new();
		appointment.SetupGet(x => x.Clan).Returns(clan.Object);
		appointment.SetupGet(x => x.MaximumSimultaneousHolders).Returns(2);

		Mock<IElection> openByElection = CreateElectionMock(isFinalised: false, isByElection: true,
			numberOfAppointments: 1);
		appointment.SetupGet(x => x.Elections).Returns([openByElection.Object]);

		List<IClanMembership> memberships =
		[
			CreateMembershipMock(1, false, appointment.Object).Object
		];

		Assert.AreEqual(0,
			ClanCommandUtilities.GetUncoveredAppointmentVacancies(appointment.Object, memberships, []));
	}

	[TestMethod]
	public void ElectionNeedsContestedVote_UsesElectionSeatCount()
	{
		var nominees = new[]
		{
			CreateMembershipMock(1, false).Object,
			CreateMembershipMock(2, false).Object
		};
		Mock<IElection> election = CreateElectionMock(isFinalised: false, isByElection: true,
			numberOfAppointments: 2, nominees: nominees);

		Assert.IsFalse(ClanCommandUtilities.ElectionNeedsContestedVote(election.Object));
	}

	[TestMethod]
	public void OpenElectionHelpers_ReturnExpectedOpenElections()
	{
		Mock<IAppointment> appointment = new();
		Mock<IElection> primaryOpen = CreateElectionMock(isFinalised: false, isByElection: false);
		Mock<IElection> byElectionOpen = CreateElectionMock(isFinalised: false, isByElection: true);
		Mock<IElection> finalisedPrimary = CreateElectionMock(isFinalised: true, isByElection: false);

		appointment.SetupGet(x => x.Elections).Returns(
			[
				primaryOpen.Object,
				byElectionOpen.Object,
				finalisedPrimary.Object
			]);

		Assert.AreSame(primaryOpen.Object, ClanCommandUtilities.GetPrimaryOpenElection(appointment.Object));
		Assert.AreSame(byElectionOpen.Object, ClanCommandUtilities.GetFirstOpenByElection(appointment.Object));
		Assert.AreSame(primaryOpen.Object, ClanCommandUtilities.GetNextOpenElection(appointment.Object));
		CollectionAssert.AreEqual(new[] { byElectionOpen.Object },
			ClanCommandUtilities.GetOpenByElections(appointment.Object).ToArray());
	}

	private static Mock<IClanMembership> CreateMembershipMock(long memberId, bool isArchivedMembership,
		params IAppointment[] appointments)
	{
		Mock<IClanMembership> membership = new();
		membership.SetupGet(x => x.MemberId).Returns(memberId);
		membership.SetupGet(x => x.IsArchivedMembership).Returns(isArchivedMembership);
		membership.SetupGet(x => x.Appointments).Returns(appointments.ToList());
		return membership;
	}

	private static Mock<IExternalClanControl> CreateExternalControlMock(IClan vassalClan, IAppointment appointment,
		int numberOfAppointments, int appointeeCount)
	{
		Mock<IExternalClanControl> control = new();
		control.SetupGet(x => x.VassalClan).Returns(vassalClan);
		control.SetupGet(x => x.ControlledAppointment).Returns(appointment);
		control.SetupGet(x => x.NumberOfAppointments).Returns(numberOfAppointments);
		control.SetupGet(x => x.Appointees).Returns(
			Enumerable.Range(0, appointeeCount)
			          .Select(x => CreateMembershipMock(x + 100, false).Object)
			          .ToList());
		return control;
	}

	private static Mock<IElection> CreateElectionMock(bool isFinalised, bool isByElection, params long[] victorIds)
	{
		return CreateElectionMock(isFinalised, isByElection, 0, [], victorIds);
	}

	private static Mock<IElection> CreateElectionMock(bool isFinalised, bool isByElection, int numberOfAppointments,
		IEnumerable<IClanMembership> nominees = null, params long[] victorIds)
	{
		Mock<IElection> election = new();
		election.SetupGet(x => x.IsFinalised).Returns(isFinalised);
		election.SetupGet(x => x.IsByElection).Returns(isByElection);
		election.SetupGet(x => x.NumberOfAppointments).Returns(numberOfAppointments);
		election.SetupGet(x => x.Nominees).Returns(nominees ?? []);
		election.SetupGet(x => x.ResultsInEffectDate).Returns((MudDateTime)null);
		election.SetupGet(x => x.Victors).Returns(victorIds.Select(x => CreateMembershipMock(x, false).Object).ToList());
		return election;
	}
}

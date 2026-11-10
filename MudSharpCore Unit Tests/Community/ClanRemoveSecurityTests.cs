#nullable enable

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Community;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.Community;

[TestClass]
public class ClanRemoveSecurityTests
{
	[TestMethod]
	[DataRow("rank")]
	[DataRow("paygrade")]
	[DataRow("appointment")]
	public void ClanRemove_StaleSelectionWithoutCurrentAuthority_DeniesBeforeLookup(string kind)
	{
		foreach (var state in new[] { "missing", "archived", "revoked" })
		{
			var (actor, clan) = Fixture(state, false);
			Invoke(actor.Object, kind);
			clan.VerifyGet(x => x.Ranks, Times.Never);
			clan.VerifyGet(x => x.Paygrades, Times.Never);
			clan.VerifyGet(x => x.Appointments, Times.Never);
			clan.Verify(x => x.DeleteRank(It.IsAny<IRank>()), Times.Never);
			clan.Verify(x => x.DeletePaygrade(It.IsAny<IPaygrade>()), Times.Never);
			clan.Verify(x => x.DeleteAppointment(It.IsAny<IAppointment>()), Times.Never);
		}
	}

	[TestMethod]
	[DataRow("rank", false)]
	[DataRow("paygrade", false)]
	[DataRow("appointment", false)]
	[DataRow("rank", true)]
	[DataRow("paygrade", true)]
	[DataRow("appointment", true)]
	public void ClanRemove_CurrentMemberPrivilegeOrAdmin_PreservesRemoval(string kind, bool administrator)
	{
		var (actor, clan) = Fixture(administrator ? "missing" : "active", administrator);
		Invoke(actor.Object, kind);
		clan.Verify(x => x.DeleteRank(It.IsAny<IRank>()), Times.Exactly(kind == "rank" ? 1 : 0));
		clan.Verify(x => x.DeletePaygrade(It.IsAny<IPaygrade>()), Times.Exactly(kind == "paygrade" ? 1 : 0));
		clan.Verify(x => x.DeleteAppointment(It.IsAny<IAppointment>()), Times.Exactly(kind == "appointment" ? 1 : 0));
	}

	[TestMethod]
	public void ClanRemove_ReferencedConfirmationOffice_RejectsDeletion()
	{
		var (actor, clan) = Fixture("active", false);
		var dependent = Mock.Of<IAppointment>(x => x.ElectionConfirmationAppointment == clan.Object.Appointments[0]);
		var other = Mock.Of<IClan>(x => x.Id == 2 && x.Appointments == new System.Collections.Generic.List<IAppointment> { dependent });
		Mock.Get(actor.Object.Gameworld).SetupGet(x => x.Clans).Returns(new All<IClan> { clan.Object, other });
		Invoke(actor.Object, "appointment");
		clan.Verify(x => x.DeleteAppointment(It.IsAny<IAppointment>()), Times.Never);
	}

	[TestMethod]
	public void ClanRemove_JobListingReference_RejectsDeletion()
	{
		var (actor, clan) = Fixture("active", false);
		var listing = Mock.Of<MudSharp.Economy.IJobListing>(x => x.Id == 2 && x.ClanAppointment == clan.Object.Appointments[0]);
		Mock.Get(actor.Object.Gameworld).SetupGet(x => x.JobListings).Returns(new All<MudSharp.Economy.IJobListing> { listing });
		Invoke(actor.Object, "appointment");
		clan.Verify(x => x.DeleteAppointment(It.IsAny<IAppointment>()), Times.Never);
	}

	private static void Invoke(ICharacter actor, string kind) =>
		typeof(ClanModule).GetMethod("ClanRemove", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [actor, new StringStack($"{kind} 13")]);

	private static (Mock<ICharacter> Actor, Mock<IClan> Clan) Fixture(string membershipState, bool administrator)
	{
		var clan = new Mock<IClan>();
		clan.SetupGet(x => x.Id).Returns(1);
		clan.SetupGet(x => x.FullName).Returns("Test Clan");
		clan.SetupGet(x => x.Ranks).Returns([Mock.Of<IRank>(x => x.Id == 13 && x.Name == "Target"), Mock.Of<IRank>(x => x.Id == 14)]);
		clan.SetupGet(x => x.Paygrades).Returns([Mock.Of<IPaygrade>(x => x.Id == 13 && x.Name == "Target")]);
		clan.SetupGet(x => x.Appointments).Returns([Mock.Of<IAppointment>(x => x.Id == 13 && x.Name == "Target")]);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(Mock.Of<IFuturemud>(x => x.Clans == new All<IClan> { clan.Object } && x.JobListings == new All<MudSharp.Economy.IJobListing>()));
		actor.Setup(x => x.IsAdministrator(PermissionLevel.Admin)).Returns(administrator);
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		actor.Setup(x => x.CombinedEffectsOfType<BuilderEditingEffect<IClan>>())
			.Returns([new BuilderEditingEffect<IClan>(actor.Object) { EditingItem = clan.Object }]);
		var membership = new Mock<IClanMembership>();
		membership.SetupGet(x => x.Clan).Returns(clan.Object);
		membership.SetupGet(x => x.IsArchivedMembership).Returns(membershipState == "archived");
		membership.SetupGet(x => x.NetPrivileges).Returns(membershipState == "revoked" ? ClanPrivilegeType.None :
			ClanPrivilegeType.CanCreateRanks | ClanPrivilegeType.CanCreatePaygrades | ClanPrivilegeType.CanCreateAppointments);
		actor.SetupGet(x => x.ClanMemberships).Returns(membershipState == "missing" ? [] : [membership.Object]);
		return (actor, clan);
	}
}

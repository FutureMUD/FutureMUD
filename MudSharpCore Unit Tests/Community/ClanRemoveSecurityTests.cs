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
	[DataTestMethod]
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

	[DataTestMethod]
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

	private static void Invoke(ICharacter actor, string kind) =>
		typeof(ClanModule).GetMethod("ClanRemove", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [actor, new StringStack($"{kind} 13")]);

	private static (Mock<ICharacter> Actor, Mock<IClan> Clan) Fixture(string membershipState, bool administrator)
	{
		var clan = new Mock<IClan>();
		clan.SetupGet(x => x.FullName).Returns("Test Clan");
		clan.SetupGet(x => x.Ranks).Returns([Mock.Of<IRank>(x => x.Id == 13 && x.Name == "Target"), Mock.Of<IRank>(x => x.Id == 14)]);
		clan.SetupGet(x => x.Paygrades).Returns([Mock.Of<IPaygrade>(x => x.Id == 13 && x.Name == "Target")]);
		clan.SetupGet(x => x.Appointments).Returns([Mock.Of<IAppointment>(x => x.Id == 13 && x.Name == "Target")]);
		var actor = new Mock<ICharacter>();
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

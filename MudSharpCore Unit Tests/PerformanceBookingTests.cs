#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
using MudSharp.RPG.Performances;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PerformanceBookingTests
{
	[TestMethod]
	public void ScheduledClanRunRechecksTheOrganisersCurrentAuthority()
	{
		var organiser = new Mock<ICharacter>();
		var world = new Mock<IFuturemud>();
		var clan = new Mock<IClan>();
		clan.SetupGet(x => x.Id).Returns(15);
		clan.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Clan);
		var authorised = true;
		clan.Setup(x => x.HasAuthority(organiser.Object,EmploymentAuthority.AssignTasks)).Returns(() => authorised);
		var clans = new All<IClan>();
		clans.Add(clan.Object);
		world.SetupGet(x => x.Clans).Returns(clans);
		world.Setup(x => x.TryGetCharacter(10,true)).Returns(organiser.Object);
		var service = new PerformanceService(world.Object);
		service.Works.Add(new PerformanceWork {Id = 8});
		var plan = new PerformancePlan { WorkId = 8, BookedBy = 10, HostType = EmploymentHostType.Clan, HostId = 15 };

		Assert.IsNull(service.AuthorityProblem(plan));
		authorised = false;
		StringAssert.Contains(service.AuthorityProblem(plan),"no longer has authority");
	}

	[TestMethod]
	public void StaffBookingLosesItsOverrideWhenAccountAuthorityIsRevoked()
	{
		var organiser = new Mock<ICharacter>();
		var account = new Mock<IAccount>();
		var authority = new Mock<IAuthority>();
		authority.SetupGet(x => x.Level).Returns(PermissionLevel.HighAdmin);
		account.SetupGet(x => x.Authority).Returns(authority.Object);
		organiser.SetupGet(x => x.Account).Returns(account.Object);
		organiser.SetupGet(x => x.Id).Returns(22);
		organiser.SetupGet(x => x.PermissionLevel).Returns(PermissionLevel.HighAdmin);
		var world = new Mock<IFuturemud>();
		world.Setup(x => x.TryGetCharacter(22,true)).Returns(organiser.Object);
		var plan = new PerformancePlan { BookedAsStaff = true, BookedBy = 22, Owner = 10 };
		var service = new PerformanceService(world.Object);

		Assert.IsNull(service.AuthorityProblem(plan));
		authority.SetupGet(x => x.Level).Returns(PermissionLevel.Player);
		Assert.IsNotNull(service.AuthorityProblem(plan));
	}
}

#nullable enable

using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Community;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.Community;

[TestClass]
[DoNotParallelize]
public class AppointmentBuilderTests
{
	[TestMethod]
	public void ParentCommand_RejectsSelfDescendantAndCorruptChainsButAllowsReparentingAndClear()
	{
		var root = Office("Root");
		var child = Office("Child", root.Object);
		var leaf = Office("Leaf", child.Object);
		var other = Office("Other");
		var clan = new Mock<IClan>();
		clan.SetupGet(x => x.FullName).Returns("Test Clan");
		clan.SetupGet(x => x.Appointments).Returns([root.Object, child.Object, leaf.Object, other.Object]);
		var actor = Mock.Of<ICharacter>(x => x.OutputHandler == Mock.Of<IOutputHandler>());
		void Parent(IAppointment appointment, string target) => typeof(ClanModule)
			.GetMethod("ClanEditAppointmentParent", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [actor, new StringStack(target), clan.Object, appointment]);

		Parent(root.Object, "Root");
		Parent(root.Object, "Leaf");
		Assert.IsNull(root.Object.ParentPosition);
		Assert.IsFalse(root.Object.Changed);
		Parent(child.Object, "Other");
		Assert.AreSame(other.Object, child.Object.ParentPosition);
		Parent(child.Object, "clear");
		Assert.IsNull(child.Object.ParentPosition);
		child.Object.ParentPosition = leaf.Object; // Simulate a corrupt graph loaded from an older world.
		Parent(other.Object, "Child");
		Assert.IsNull(other.Object.ParentPosition);
	}

	[TestMethod]
	public void Save_AppointmentAuthoritySetAndClear_SurvivesReload()
	{
		using var database = new InMemoryDatabaseTestScope();
		var row = new MudSharp.Models.Appointment { Id = 1, Name = "Deputy", ClanId = 1 };
		database.Context.Appointments.Add(row);
		database.Context.SaveChanges();
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		var rank = Mock.Of<IRank>(x => x.Id == 2 && x.Name == "Senior");
		var parent = Office("Leader");
		parent.SetupGet(x => x.Id).Returns(3);
		var clan = new Mock<IClan>();
		clan.SetupGet(x => x.Gameworld).Returns(world.Object);
		clan.SetupGet(x => x.Ranks).Returns([rank]);
		clan.SetupGet(x => x.Appointments).Returns([parent.Object]);
		var appointment = new Appointment(row, clan.Object)
		{
			ParentPosition = parent.Object, MinimumRankToAppoint = rank, Changed = true
		};
		appointment.Save();
		database.Context.ChangeTracker.Clear();
		var stored = database.Context.Appointments.AsNoTracking().Single();
		var reloaded = new Appointment(stored, clan.Object);
		reloaded.FinaliseLoad(stored);
		Assert.AreSame(parent.Object, reloaded.ParentPosition);
		Assert.AreSame(rank, reloaded.MinimumRankToAppoint);
		var leader = new Mock<IClanMembership>();
		leader.SetupGet(x => x.Appointments).Returns([parent.Object]);
		Assert.IsTrue(ClanCommandUtilities.HoldsOrControlsAppointment(leader.Object, reloaded));
		reloaded.ParentPosition = null;
		reloaded.MinimumRankToAppoint = null;
		reloaded.Changed = true;
		reloaded.Save();
		database.Context.ChangeTracker.Clear();
		stored = database.Context.Appointments.AsNoTracking().Single();
		reloaded = new Appointment(stored, clan.Object);
		reloaded.FinaliseLoad(stored);
		Assert.IsNull(reloaded.ParentPosition);
		Assert.IsNull(reloaded.MinimumRankToAppoint);
		Assert.IsFalse(ClanCommandUtilities.HoldsOrControlsAppointment(leader.Object, reloaded));
	}

	private static Mock<IAppointment> Office(string name, IAppointment? parent = null)
	{
		var office = new Mock<IAppointment>();
		office.SetupGet(x => x.Name).Returns(name);
		office.SetupProperty(x => x.ParentPosition, parent!);
		office.SetupProperty(x => x.Changed);
		return office;
	}
}

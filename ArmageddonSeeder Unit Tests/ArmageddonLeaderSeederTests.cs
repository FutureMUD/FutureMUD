#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonLeaderSeederTests
{
	[TestMethod]
	public void Leaders_AreUnfilledOfficesInOnlyTheTwoApprovedClans()
	{
		using var db = SeedClans();
		var leaders = db.Appointments.Include(x => x.AppointmentsTitles).OrderBy(x => x.ClanId).ToArray();
		Assert.AreEqual(2, leaders.Length);
		CollectionAssert.AreEquivalent(new[] { "armclan28", "armclan41" }, leaders.Select(x => db.Clans.Single(c => c.Id == x.ClanId).Alias).ToArray());
		foreach (var leader in leaders)
		{
			Assert.AreEqual("Leader", leader.Name);
			Assert.AreEqual("leader", leader.AppointmentsTitles.Single().Title);
			Assert.AreEqual(0, leader.MaximumSimultaneousHolders, "The source permits multiple independently appointed leaders.");
			Assert.AreEqual(0L, leader.Privileges);
			Assert.IsNull(leader.MinimumRankId);
			Assert.IsNull(leader.MinimumRankToAppointId);
			Assert.IsNull(leader.ParentAppointmentId);
			Assert.IsNull(leader.PaygradeId);
			Assert.IsFalse(leader.IsAppointedByElection);
		}
		Assert.AreEqual(0, db.ClanMemberships.Count());
		Assert.AreEqual(0, db.ClanMembershipsAppointments.Count());
		Assert.IsTrue(db.Ranks.All(x => x.Privileges == 0));
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		foreach (var definition in db.FutureProgs.ToArray()) compiler.Compile(definition.Id);
	}

	[TestMethod]
	public void LeaderPermission_RequiresTheCorrectOfficeRegardlessOfRankAndFailsClosedWhenMissing()
	{
		using var db = SeedClans();
		MudSharp.FutureProg.FutureProg.Initialise();
		foreach (var sourceId in new[] { 28, 41 })
		{
			var seededClan = db.Clans.Single(x => x.Alias == "armclan" + sourceId);
			var seededLeader = db.Appointments.Single(x => x.ClanId == seededClan.Id);
			var clan = Item<IClan>(seededClan.Id, ProgVariableTypes.Clan);
			var leader = Item<IAppointment>(seededLeader.Id, ProgVariableTypes.ClanAppointment);
			var otherClan = Item<IClan>(seededClan.Id + 1000, ProgVariableTypes.Clan);
			var otherLeader = Item<IAppointment>(seededLeader.Id + 1000, ProgVariableTypes.ClanAppointment);
			var appointments = new List<IAppointment> { leader.Object };
			clan.SetupGet(x => x.Appointments).Returns(() => appointments);
			var clans = new All<IClan>(); clans.Add(clan.Object);
			var world = new Mock<IFuturemud>(); world.SetupGet(x => x.Clans).Returns(clans);
			var actor = Item<ICharacter>(10, ProgVariableTypes.Character);
			var memberships = new List<IClanMembership>();
			actor.SetupGet(x => x.ClanMemberships).Returns(memberships);
			var membership = new Mock<IClanMembership>();
			membership.SetupProperty(x => x.Clan, clan.Object);
			membership.SetupProperty(x => x.Rank, Mock.Of<IRank>(x => x.RankNumber == 100));
			membership.SetupProperty(x => x.Appointments, new List<IAppointment>());
			var definition = db.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.FunctionName == $"ArmageddonWorldfileClan{sourceId}Leader");
			var prog = new MudSharp.FutureProg.FutureProg(world.Object, definition.FunctionName,
				ProgVariableTypes.FromStorageString(definition.ReturnTypeDefinition),
				definition.FutureProgsParameters.OrderBy(x => x.ParameterIndex).Select(x => Tuple.Create(ProgVariableTypes.FromStorageString(x.ParameterTypeDefinition), x.ParameterName)),
				definition.FunctionText);
			Assert.IsTrue(prog.Compile(), prog.CompileError);
			Assert.IsFalse(prog.ExecuteBool(actor.Object), "Nonmembers cannot command the guards.");
			memberships.Add(membership.Object);
			Assert.IsFalse(prog.ExecuteBool(actor.Object), "High rank alone does not confer leadership.");
			membership.Object.Rank = Mock.Of<IRank>(x => x.RankNumber == 0);
			membership.Object.Appointments.Add(leader.Object);
			Assert.IsTrue(prog.ExecuteBool(actor.Object), "An appointed low-ranking member is a leader.");
			membership.Object.Clan = otherClan.Object;
			Assert.IsFalse(prog.ExecuteBool(actor.Object), "The office cannot authorise another clan's membership.");
			membership.Object.Clan = clan.Object;
			membership.Object.Appointments = [otherLeader.Object];
			Assert.IsFalse(prog.ExecuteBool(actor.Object), "Another clan's Leader office is insufficient.");
			Assert.IsFalse(prog.ExecuteBool(new object?[] { null }));
			membership.Object.Appointments = [leader.Object];
			appointments.Clear();
			Assert.IsFalse(prog.ExecuteBool(actor.Object), "A removed office fails closed.");
			world.SetupGet(x => x.Clans).Returns(new All<IClan>());
			Assert.IsFalse(prog.ExecuteBool(actor.Object), "A removed clan fails closed.");
		}
	}

	private static Mock<T> Item<T>(long id, ProgVariableTypes type) where T : class, IFrameworkItem, IProgVariable
	{
		var mock = new Mock<T>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.As<IProgVariable>().SetupGet(x => x.Type).Returns(type);
		mock.As<IProgVariable>().SetupGet(x => x.GetObject).Returns(mock.Object);
		return mock;
	}

	private static FuturemudDatabaseContext SeedClans()
	{
		var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		db.Materials.Add(new Material { Id = 40, Name = "Obsidian", Type = 0 });
		db.Clocks.Add(new Clock { Id = 40, PrimaryTimezoneId = 40 });
		db.Timezones.Add(new Timezone { Id = 40, ClockId = 40, Name = "local" });
		db.SaveChanges();
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var source = new ArmageddonWorldfilePackage { Clans = archive.Clans.Where(x => x.Index is 12 or 28 or 41).ToArray() };
		var writer = new ArmageddonWorldfileWriter(db, source);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallClansAndCurrency", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		db.ChangeTracker.Clear();
		return db;
	}
}

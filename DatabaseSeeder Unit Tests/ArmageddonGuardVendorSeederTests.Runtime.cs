#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Body;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using NativeProg = MudSharp.FutureProg.FutureProg;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonGuardVendorSeederTests
{
	[TestMethod]
	public void MantisVendor_UsesSourceClanExceptionAndRefusesKeywordDuplicatesBeforeNativeTransfer()
	{
		using var db = Fixture(out var writer, out _);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallMantisVendors", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		var (_, progs, clans) = Runtime(db);
		var actor = Actor(); var vendor = Actor();
		var race = Item<IRace>(4000, ProgVariableTypes.Race);
		actor.Setup(x => x.GetProperty("race")).Returns(race.Object);
		actor.Setup(x => x.TargetActor("vendor", PerceiveIgnoreFlags.None)).Returns(vendor.Object);
		actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		var memberships = new List<IClanMembership>(); actor.SetupGet(x => x.ClanMemberships).Returns(memberships);
		var room = Item<IRoom>(1, ProgVariableTypes.Location);
		room.Setup(x => x.GetProperty("items")).Returns(new CollectionVariable(new List<IGameItem>(), ProgVariableTypes.Item));
		actor.Setup(x => x.GetProperty("location")).Returns(room.Object);
		var inventory = new List<IGameItem>();
		actor.Setup(x => x.GetProperty("inventory")).Returns(() => new CollectionVariable(inventory, ProgVariableTypes.Item));
		var stock = Item<IGameItem>(30, ProgVariableTypes.Item);
		stock.Setup(x => x.HasKeyword("cloak", actor.Object, true, false)).Returns(true);
		var source = new Mock<IBody>(); var destination = new Mock<IBody>();
		source.SetupGet(x => x.HeldItems).Returns(new[] { stock.Object });
		vendor.SetupGet(x => x.Body).Returns(source.Object); actor.SetupGet(x => x.Body).Returns(destination.Object);
		var prog = progs.GetByName("ArmageddonWorldfileMantisVendor");
		bool Ask() => prog.ExecuteBool(actor.Object, vendor.Object, "ask", new List<string> { "vendor", "cloak" });
		var barred = new Mock<IClanMembership>(); barred.SetupGet(x => x.Clan).Returns(clans[1007]); memberships.Add(barred.Object);
		Assert.IsFalse(Ask());
		source.Verify(x => x.GiveWithoutMerge(It.IsAny<IGameItem>(), It.IsAny<IBody>(), It.IsAny<IEmote>()), Times.Never);
		var exception = new Mock<IClanMembership>(); exception.SetupGet(x => x.Clan).Returns(clans[1037]); memberships.Add(exception.Object);
		// A different prototype with the same requested keyword still prevents a duplicate.
		var duplicate = Item<IGameItem>(31, ProgVariableTypes.Item);
		duplicate.Setup(x => x.HasKeyword("cloak", actor.Object, true, false)).Returns(true); inventory.Add(duplicate.Object);
		Assert.IsTrue(Ask());
		source.Verify(x => x.GiveWithoutMerge(It.IsAny<IGameItem>(), It.IsAny<IBody>(), It.IsAny<IEmote>()), Times.Never);
		inventory.Clear();
		Assert.IsTrue(Ask());
		source.Verify(x => x.GiveWithoutMerge(stock.Object, destination.Object, null), Times.Once);
		vendor.Setup(x => x.GetProperty("isawake")).Returns(new BooleanVariable(false));
		Assert.IsFalse(Ask());
		source.Verify(x => x.GiveWithoutMerge(stock.Object, destination.Object, null), Times.Once);
	}

	[TestMethod]
	public void LandGuard_EngagesOnlyAtItsProtectedHomeAndNeverConsumesTheEvent()
	{
		using var db = Fixture(out var writer, out _);
		InstallGuards(writer, db);
		var (world, progs, _) = Runtime(db);
		var actor = Actor(); var guard = Actor();
		var home = Item<IRoom>(106592, ProgVariableTypes.Location);
		var elsewhere = Item<IRoom>(2, ProgVariableTypes.Location);
		var rooms = new All<IRoom>(); rooms.Add(home.Object); world.SetupGet(x => x.Rooms).Returns(rooms);
		actor.Setup(x => x.GetProperty("location")).Returns(home.Object);
		guard.Setup(x => x.GetProperty("location")).Returns(home.Object);
		guard.SetupGet(x => x.Location).Returns(home.Object);
		home.Setup(x => x.LayerCharacters(It.IsAny<RoomLayer>())).Returns(new[] { actor.Object });
		actor.Setup(x => x.GetKeywordsFor(guard.Object)).Returns(new[] { "traveller" });
		actor.Setup(x => x.HasKeywords(It.IsAny<IEnumerable<string>>(), guard.Object, It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		guard.Setup(x => x.CanSee(actor.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		guard.Setup(x => x.CanSee(actor.Object)).Returns(true);
		var prog = progs.GetByName("ArmageddonWorldfileGuardGathergyp_gate_guard");
		bool Notify() => prog.ExecuteBool(actor.Object, guard.Object, home.Object, null, "method", 5, "operation");
		Assert.IsFalse(Notify());
		var commands = guard.Invocations.Where(x => x.Method.Name == "ExecuteCommand").Select(x => x.Arguments[0]?.ToString()).ToArray();
		Assert.AreEqual(3, commands.Length); Assert.AreEqual("shout Defiler!", commands[0]); Assert.AreEqual("shout Defiler!", commands[1]);
		Assert.AreEqual("kill 1.traveller", commands[2]);
		guard.Setup(x => x.GetProperty("location")).Returns(elsewhere.Object);
		Assert.IsFalse(Notify());
		Assert.AreEqual(3, guard.Invocations.Count(x => x.Method.Name == "ExecuteCommand"));
	}

	[TestMethod]
	public void KohmarAsk_DeclinesWrongAddresseeAndNonleadersAndExecutesOnlyTheRequestedNativeAction()
	{
		using var db = Fixture(out var writer, out _);
		InstallGuards(writer, db);
		var (world, progs, clans) = Runtime(db);
		var actor = Actor(); var guard = Actor(); var other = Actor();
		actor.Setup(x => x.TargetActor("guard", PerceiveIgnoreFlags.None)).Returns(guard.Object);
		actor.Setup(x => x.TargetActor("other", PerceiveIgnoreFlags.None)).Returns(other.Object);
		var memberships = new List<IClanMembership>(); actor.SetupGet(x => x.ClanMemberships).Returns(memberships);
		var membership = new Mock<IClanMembership>(); membership.SetupGet(x => x.Clan).Returns(clans[1028]);
		membership.SetupGet(x => x.Appointments).Returns(clans[1028].Appointments);
		var ask = progs.GetByName("ArmageddonWorldfileGuardAskkohmar_body_guards28");
		Assert.IsFalse(ask.ExecuteBool(actor.Object, guard.Object, "ask", new List<string> { "guard", "follow", "self" }));
		memberships.Add(membership.Object);
		Assert.IsFalse(ask.ExecuteBool(actor.Object, guard.Object, "ask", new List<string> { "other", "follow", "self" }));
		Assert.IsTrue(ask.ExecuteBool(actor.Object, guard.Object, "ask", new List<string> { "guard", "follow", "self" }));
		Assert.IsTrue(ask.ExecuteBool(actor.Object, guard.Object, "ask", new List<string> { "guard", "guard", "self" }));
		Assert.IsTrue(ask.ExecuteBool(actor.Object, guard.Object, "ask", new List<string> { "guard", "follow" }));
		var commands = guard.Invocations.Where(x => x.Method.Name == "ExecuteCommand").Select(x => x.Arguments[0]?.ToString()).ToArray();
		CollectionAssert.AreEqual(new[] { "follow self", "bodyguard off", "say Follow who?" }, commands);
	}

	[TestMethod]
	public void NobleAdmission_PreservesEffectiveClan39BranchAndRequiresPlayerOfficeOrActualFaleRank()
	{
		using var db = Fixture(out var writer, out _);
		InstallGuards(writer, db);
		var (world, progs, clans) = Runtime(db);
		var traveller = Actor(); var guard = Actor();
		guard.Setup(x => x.CanSee(traveller.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		var room = Item<IRoom>(150881, ProgVariableTypes.Location);
		var rooms = new All<IRoom>(); rooms.Add(room.Object); world.SetupGet(x => x.Rooms).Returns(rooms);
		var exit = new Mock<IRoomExit>();
		exit.SetupGet(x => x.GetObject).Returns(exit.Object); exit.SetupGet(x => x.Type).Returns(ProgVariableTypes.Exit);
		exit.Setup(x => x.GetProperty("direction")).Returns(new TextVariable("South"));
		exit.Setup(x => x.GetProperty("origin")).Returns(room.Object);
		var memberships = new List<IClanMembership>(); traveller.SetupGet(x => x.ClanMemberships).Returns(memberships);
		var membership = new Mock<IClanMembership>();
		membership.SetupProperty(x => x.Clan, clans[1039]);
		membership.SetupProperty(x => x.Appointments, new List<IAppointment>());
		membership.SetupProperty(x => x.Rank, Mock.Of<IRank>(x => x.RankNumber == 0));
		memberships.Add(membership.Object);
		var permission = progs.GetByName("ArmageddonWorldfileNobleGatePermission");
		bool Allowed() => permission.ExecuteBool(guard.Object, traveller.Object, exit.Object);
		Assert.IsTrue(Allowed(), "The effective source clan-39 branch has no rank requirement.");
		membership.Object.Clan = clans[1041]; Assert.IsFalse(Allowed());
		membership.Object.Appointments = clans[1041].Appointments; Assert.IsTrue(Allowed());
		traveller.Setup(x => x.GetProperty("npctemplateid")).Returns(new NumberVariable(123)); Assert.IsFalse(Allowed());
		traveller.Setup(x => x.GetProperty("npctemplateid")).Returns(new NumberVariable(0));
		membership.Object.Clan = clans[1026]; membership.Object.Appointments = [];
		membership.Object.Rank = Mock.Of<IRank>(x => x.RankNumber == 1); Assert.IsFalse(Allowed());
		membership.Object.Rank = Mock.Of<IRank>(x => x.RankNumber == 2); Assert.IsTrue(Allowed());
		exit.Setup(x => x.GetProperty("direction")).Returns(new TextVariable("North")); Assert.IsFalse(Allowed());
	}

	private static void InstallGuards(ArmageddonWorldfileWriter writer, FuturemudDatabaseContext db)
	{
		foreach (var name in new[] { "InstallClanLeaderAppointments", "InstallGuardSpecials" })
			typeof(ArmageddonWorldfileWriter).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
	}

	private static (Mock<IFuturemud> World, All<IFutureProg> Progs, Dictionary<long, IClan> Clans) Runtime(FuturemudDatabaseContext db,
		Action<Mock<IVariableRegister>>? configureRegisters = null)
	{
		NativeProg.Initialise();
		var world = new Mock<IFuturemud>(); var progs = new All<IFutureProg>(); world.SetupGet(x => x.FutureProgs).Returns(progs);
		var registers = new Mock<IVariableRegister>();
		registers.Setup(x => x.GetType(ProgVariableTypes.Character, "fixture_cash")).Returns(ProgVariableTypes.Number);
		world.SetupGet(x => x.VariableRegister).Returns(registers.Object);
		configureRegisters?.Invoke(registers);
		var clans = new Dictionary<long, IClan>(); var registry = new All<IClan>(); world.SetupGet(x => x.Clans).Returns(registry);
		foreach (var record in db.Clans)
		{
			var clan = Item<IClan>(record.Id, ProgVariableTypes.Clan);
			clan.SetupGet(x => x.Appointments).Returns(db.Appointments.Where(x => x.ClanId == record.Id).AsEnumerable()
				.Select(x => Item<IAppointment>(x.Id, ProgVariableTypes.ClanAppointment).Object).ToList());
			clan.SetupGet(x => x.Ranks).Returns(db.Ranks.Where(x => x.ClanId == record.Id).AsEnumerable().Select(x =>
			{
				var rank = Item<IRank>(x.Id, ProgVariableTypes.ClanRank); rank.SetupGet(y => y.RankNumber).Returns(x.RankNumber); return rank.Object;
			}).ToList());
			clans.Add(record.Id, clan.Object); registry.Add(clan.Object);
		}
		var constructor = typeof(NativeProg).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
			[typeof(MudSharp.Models.FutureProg), typeof(IFuturemud)], null)!;
		foreach (var definition in db.FutureProgs.Include(x => x.FutureProgsParameters))
			progs.Add((IFutureProg)constructor.Invoke([definition, world.Object]));
		foreach (var prog in progs) Assert.IsTrue(prog.Compile(), prog.FunctionName + ": " + prog.CompileError);
		return (world, progs, clans);
	}

	private static Mock<ICharacter> Actor()
	{
		var actor = Item<ICharacter>(1, ProgVariableTypes.Character);
		actor.Setup(x => x.Equals(It.IsAny<IPerceivable>())).Returns((IPerceivable other) => ReferenceEquals(other, actor.Object));
		actor.Setup(x => x.Equals(It.IsAny<ICharacter>())).Returns((ICharacter other) => ReferenceEquals(other, actor.Object));
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		actor.Setup(x => x.GetProperty("isawake")).Returns(new BooleanVariable(true));
		actor.Setup(x => x.GetProperty("incombat")).Returns(new BooleanVariable(false));
		actor.Setup(x => x.GetProperty("npctemplateid")).Returns(new NumberVariable(0));
		actor.Setup(x => x.GetProperty("riders")).Returns(new CollectionVariable(new List<ICharacter>(), ProgVariableTypes.Character));
		return actor;
	}

	private static Mock<T> Item<T>(long id, ProgVariableTypes type) where T : class, IFrameworkItem, IProgVariable
	{
		var item = new Mock<T>(); item.SetupGet(x => x.Id).Returns(id);
		item.As<IProgVariable>().SetupGet(x => x.GetObject).Returns(item.Object);
		item.As<IProgVariable>().SetupGet(x => x.Type).Returns(type);
		item.As<IProgVariable>().Setup(x => x.GetProperty("id")).Returns(new NumberVariable(id));
		return item;
	}
}

#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClanPrivilegeType = MudSharp.Community.ClanPrivilegeType;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonServiceSeederTests
{
	private static T Field<T>(ArmageddonWorldfileWriter writer, string name) =>
		(T)typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
	private static void Set(ArmageddonWorldfileWriter writer, string name, object value) =>
		typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(writer, value);

	[TestMethod]
	public void Services_UseFixedNativeBranchesZeroBalanceClanAccountsAndUpfrontStableFees()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load();
		var banker = archive.Npcs.Single(x => x.Vnum == 20112);
		var stablehand = archive.Npcs.Single(x => x.Vnum == 64027);
		var source = new ArmageddonWorldfilePackage { Npcs = [banker, stablehand], Resets =
			[new(new("fixture.cmd", 1), 0, "M", [0, banker.Vnum, 1, 10], [], []),
			 new(new("fixture.cmd", 2), 0, "M", [0, stablehand.Vnum, 1, 11], [], [])] };
		var writer = new ArmageddonWorldfileWriter(db, source);
		var currency = new Currency { Id = 10, Name = "Obsidian" }; db.Currencies.Add(currency); Set(writer, "_currency", currency);
		var clock = new Clock { Id = 10, PrimaryTimezoneId = 10 }; db.Clocks.Add(clock);
		db.Timezones.Add(new Timezone { Id = 10, ClockId = 10, Name = "local" });
		db.Calendars.Single().Date = "1/Jan/2000";
		var economic = new EconomicZone { Id = 10, Name = "Trade", CurrencyId = 10 }; db.EconomicZones.Add(economic);
		Field<Dictionary<int, EconomicZone>>(writer, "_economicZones").Add(0, economic);
		foreach (var vnum in new[] { 10, 11 })
		{
			var room = new Room { Id = vnum, UniqueName = $"Vnum{vnum}", EffectData = "<Effects/>" };
			room.RoomOverlays.Add(new RoomOverlay { Id = vnum, RoomId = vnum, RoomName = "Source service room" });
			db.Rooms.Add(room); Field<Dictionary<int, Room>>(writer, "_rooms").Add(vnum, room);
		}
		foreach (var sourceId in new[] { 4, 12 })
		{
			var clan = new Clan { Id = sourceId, Name = $"Clan {sourceId}", FullName = $"Clan {sourceId}" }; db.Clans.Add(clan);
			Field<Dictionary<int, Clan>>(writer, "_clans").Add(sourceId, clan);
			var rank = new Rank { Id = sourceId, ClanId = sourceId, Name = "Member", Privileges = 0 }; db.Ranks.Add(rank);
			Field<Dictionary<(int Clan, int Rank), Rank>>(writer, "_ranks").Add((sourceId, 0), rank);
		}
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallNativeServices", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		Assert.AreEqual(1, db.BankBranches.Count()); Assert.AreEqual(10L, db.BankBranches.Single().RoomId);
		Assert.AreEqual(2, db.BankAccounts.Count());
		Assert.IsTrue(db.BankAccounts.All(x => x.CurrentBalance == 0 && x.AccountNumber > 0 && x.AuthorisedBankPaymentItems == ""));
		Assert.IsTrue(db.BankAccounts.All(x => x.AccountOwnerClanId.HasValue));
		Assert.AreEqual(2, db.Appointments.Count());
		Assert.IsTrue(db.Appointments.All(x => x.Privileges == (long)ClanPrivilegeType.CanManageBankAccounts));
		Assert.AreEqual(0, db.ClanMembershipsAppointments.Count());
		Assert.IsTrue(db.Ranks.All(x => (x.Privileges & (long)ClanPrivilegeType.CanDepositBankAccounts) != 0 &&
			(x.Privileges & (long)ClanPrivilegeType.CanManageBankAccounts) == 0));
		var stable = db.Stables.Single(); Assert.AreEqual(11L, stable.RoomId);
		Assert.AreEqual(20m, stable.LodgeFee); Assert.AreEqual(0m, stable.DailyFee);
		Assert.IsNotNull(stable.LodgeFeeProgId);
		StringAssert.Contains(db.FutureProgs.Single(x => x.Id == stable.LodgeFeeProgId).FunctionText, "isclanmember");
	}
}

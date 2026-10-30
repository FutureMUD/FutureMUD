#nullable enable

using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Reflection;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonTradingSeederTests
{
	private static T Field<T>(ArmageddonWorldfileWriter writer, string name) =>
		(T)typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;

	[TestMethod]
	public void SparseClanCatalogue_CreatesNumericRanksRequiredByGateAndArenaProgs()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		db.Materials.Add(new Material { Id = 40, Name = "Obsidian", Type = 0 });
		db.Clocks.Add(new Clock { Id = 40, PrimaryTimezoneId = 40 });
		db.Timezones.Add(new Timezone { Id = 40, ClockId = 40, Name = "local" }); db.SaveChanges();
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var source = new ArmageddonWorldfilePackage { Clans = archive.Clans.Where(x => x.Index is 4 or 12 or 23 or 26).ToArray() };
		Assert.IsTrue(source.Clans.All(x => x.Ranks.Length == 1), "The real sparse source catalogue must exercise this regression.");
		var writer = new ArmageddonWorldfileWriter(db, source);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallClansAndCurrency", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null); db.SaveChanges();
		foreach (var (clan, required) in new[] { (4, 4), (12, 6), (23, 3), (26, 2) })
		{
			var ranks = Field<Dictionary<(int, int), Rank>>(writer, "_ranks");
			Assert.AreEqual(required, ranks[(clan, required)].RankNumber);
			Assert.AreEqual("Rank " + required, ranks[(clan, required)].Name);
			Assert.AreEqual("a member", ranks[(clan, 0)].Name);
		}
		Assert.AreEqual(0, db.ClanMemberships.Count(), "Missing catalogue ranks do not author any memberships/promotions.");
	}

	[TestMethod]
	public void ApprovedKohmarHome_MapsOnlyTheTwoSealedRoomResetsWithoutChangingArchive()
	{
		var archive = ArmageddonWorldfileTestData.Load();
		var guards = archive.Resets.Where(x => x.Opcode == "M" && x.Arguments[1] is 46704 or 46705).ToArray();
		Assert.AreEqual(2, guards.Length);
		foreach (var source in guards)
		{
			var mapped = ArmageddonWorldfileWriter.ResolvePopulationReset(source);
			Assert.AreEqual(2011, source.Arguments[3]); Assert.AreEqual(20040, mapped.Arguments[3]);
			Assert.AreSame(source.Source, mapped.Source); Assert.AreEqual(source.Zone, mapped.Zone);
		}
		foreach (var source in archive.Resets.Where(x => !guards.Contains(x)))
			Assert.AreSame(source, ArmageddonWorldfileWriter.ResolvePopulationReset(source));
	}

	[TestMethod]
	public void NpcReactions_CompileAttachedPulseCommandAndInductionProgsOnly()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var vnums = new[] { 21017, 70111, 6030, 6058, 64132, 64133, 64134, 50420, 50440, 39005, 50007 };
		var source = new ArmageddonWorldfilePackage { Npcs = archive.Npcs.Where(x => vnums.Contains(x.Vnum)).ToArray() };
		var writer = new ArmageddonWorldfileWriter(db, source);
		var currency = new Currency { Id = 40, Name = "Obsidian" }; db.Currencies.Add(currency);
		typeof(ArmageddonWorldfileWriter).GetField("_currency", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(writer, currency);
		db.Races.Add(new Race { Id = 40, Name = "Halfling" });
		db.Languages.Add(new Language { Id = 40, Name = "Kentu" }); db.Accents.Add(new Accent { Id = 40, LanguageId = 40, Name = "Kentu" });
		foreach (var id in new[] { 9, 12, 24 }) Field<Dictionary<int, Clan>>(writer, "_clans").Add(id, new Clan { Id = id });
		Field<Dictionary<(int, int), Rank>>(writer, "_ranks").Add((9, 0), new Rank { Id = 9009, ClanId = 9, RankNumber = 0 });
		Field<Dictionary<int, GameItemProto>>(writer, "_items").Add(70223, new GameItemProto { Id = 70223 });
		foreach (var npc in source.Npcs)
		{
			var template = new NpcTemplate { Id = npc.Vnum, RevisionNumber = 0, Definition = "<Definition><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>" };
			db.NpcTemplates.Add(template); Field<Dictionary<int, NpcTemplate>>(writer, "_npcs").Add(npc.Vnum, template);
		}
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallNpcReactions", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null); db.SaveChanges();
		Assert.AreEqual(8, db.Hooks.Count());
		Assert.AreEqual(4, db.Hooks.Count(x => x.Type == "FutureProgHook"));
		StringAssert.Contains(db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileHoneyProduction").FunctionText, "random(1, 150)");
		Assert.IsFalse(db.FutureProgs.Any(x => x.FunctionName.Contains("InjuryHorn")), "Unattached source pulse/direction branches must remain inactive.");
		var induction = db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfilePtarkenInduction");
		StringAssert.Contains(induction.FunctionText, "claninvite"); StringAssert.Contains(induction.FunctionText, "\"Ptarken\"");
		Assert.AreEqual("<var></var>", db.VariableDefaults.Single(x => x.Property == "armw_hometown").DefaultValue);
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		foreach (var prog in db.FutureProgs.ToArray()) compiler.Compile(prog.Id);
	}

	[TestMethod]
	public void Trading_CompilesActualCommandProgsAndPreservesAssociationsAndEcology()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var vnums = new[] { 50420, 64132, 64133, 64134, 39004, 54204 };
		var gift = archive.Items.First(x => x.Vnum / 1000 == 39);
		var source = new ArmageddonWorldfilePackage { Npcs = archive.Npcs.Where(x => vnums.Contains(x.Vnum)).ToArray(), Items = [gift] };
		var writer = new ArmageddonWorldfileWriter(db, source);
		var currency = new Currency { Id = 40, Name = "Obsidian" }; db.Currencies.Add(currency);
		typeof(ArmageddonWorldfileWriter).GetField("_currency", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(writer, currency);
		db.Races.Add(new Race { Id = 40, Name = "Halfling" });
		Field<Dictionary<int, Clan>>(writer, "_clans").Add(9, new Clan { Id = 9 });
		Field<Dictionary<int, GameItemProto>>(writer, "_items").Add(gift.Vnum, new GameItemProto { Id = 120000 });
		Field<Dictionary<string, GameItemProto>>(writer, "_resetItemVariants").Add(gift.Vnum + ":fixture", new GameItemProto { Id = 120001 });
		foreach (var npc in source.Npcs)
		{
			var template = new NpcTemplate { Id = npc.Vnum, RevisionNumber = 0, Definition = "<Definition><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>" };
			db.NpcTemplates.Add(template); Field<Dictionary<int, NpcTemplate>>(writer, "_npcs").Add(npc.Vnum, template);
		}
		db.VariableDefinitions.AddRange(ArmageddonGatheringRegisters.Definitions); db.SaveChanges();
		foreach (var method in new[] { "InstallGamblingSpecials", "InstallTraderSpecials" })
			typeof(ArmageddonWorldfileWriter).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		Assert.AreEqual(9, db.Hooks.Count());
		Assert.IsTrue(db.Hooks.All(x => x.Definition.Contains("<UseProgResultForHandling>true</UseProgResultForHandling>")));
		Assert.AreEqual(0, db.Shops.Count(), "Barter is implemented by progs, independently of permanent shops.");
		var forest = db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileForestGift");
		StringAssert.Contains(forest.FunctionText, "120000|120001");
		var trader = db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileTraderhalfling_trader");
		StringAssert.Contains(trader.FunctionText, "exchangeitems(@actor, @offered, @trader, @requested)");
		StringAssert.Contains(trader.FunctionText, "min(100, getregister");
		Assert.IsFalse(db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileTradersewer_trader").FunctionText.Contains("setregister"));
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		foreach (var prog in db.FutureProgs.ToArray()) compiler.Compile(prog.Id);
		var pebble = compiler.Compile(db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfilePebbleOutcome").Id);
		var counts = new Dictionary<decimal, int>();
		for (var player = 1; player <= 10; player++)
		for (var dealer = 3; dealer <= 10; dealer++)
		{
			var result = pebble.ExecuteDecimal(new object[] { player, dealer });
			counts[result] = counts.GetValueOrDefault(result) + 1;
		}
		Assert.AreEqual(28, counts[1]); Assert.AreEqual(8, counts[0]); Assert.AreEqual(44, counts[-1]);
		var wheel = compiler.Compile(db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileWheelOutcome").Id);
		counts.Clear();
		for (var colour = 1; colour <= 2; colour++)
		for (var image = 1; image <= 30; image++)
		for (var wheelColour = 1; wheelColour <= 2; wheelColour++)
		for (var wheelImage = 1; wheelImage <= 30; wheelImage++)
		{
			var result = wheel.ExecuteDecimal(new object[] { colour, image, wheelColour, wheelImage });
			counts[result] = counts.GetValueOrDefault(result) + 1;
		}
		Assert.AreEqual(60, counts[3]); Assert.AreEqual(60, counts[2]); Assert.AreEqual(1740, counts[0]); Assert.AreEqual(1740, counts[-1]);
		var dice = compiler.Compile(db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileDiceOutcome").Id);
		for (var point = 0; point <= 12; point++)
		for (var roll = 2; roll <= 12; roll++)
		{
			var expected = point == 0 ? (roll is 7 or 11 ? 1 : 0) : roll is 7 or 11 ? -1 : roll == point ? 1 : 0;
			Assert.AreEqual((decimal)expected, dice.ExecuteDecimal(new object[] { point, roll }), $"point {point}, roll {roll}");
		}
		var wheelInput = compiler.Compile(db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileGamegypsy_game_three").Id);
		var runtimeProgs = new All<IFutureProg>(); runtimeProgs.Add(wheelInput);
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.FutureProgs).Returns(runtimeProgs);
		var hook = new HookOnInput(db.Hooks.Single(x => x.Definition.Contains("<FutureProg>" + wheelInput.Id + "</FutureProg>")), world.Object);
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.GetObject).Returns(actor.Object);
		actor.Setup(x => x.GetProperty("isawake")).Returns(new BooleanVariable(true));
		var dealerActor = new Mock<ICharacter>(); dealerActor.SetupGet(x => x.GetObject).Returns(dealerActor.Object);
		foreach (var amount in new[] { decimal.MinValue, decimal.MaxValue })
		{
			dealerActor.Invocations.Clear();
			Assert.IsTrue(hook.Function(EventType.CommandInput, [actor.Object, dealerActor.Object, "bet", new StringStack(amount.ToString(CultureInfo.InvariantCulture))]),
				"Extreme numeric input must be consumed by an ordinary refusal, before currency lookup or bankroll multiplication.");
			var expected = amount < 0 ? "say You must bet at least 20 coins." : "say The maximum bet I will cover is 500.";
			Assert.IsTrue(dealerActor.Invocations.Any(x => x.Method.Name == "ExecuteCommand" && x.Arguments.Contains(expected)));
		}
	}
}

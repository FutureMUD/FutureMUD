#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonGuardVendorSeederTests
{
	[TestMethod]
	public void VendorServices_OnlyRetainedWaterSellersUseNativeThreeBitFiftyMillilitreRefillQuotation()
	{
		using var db = VendorServicesFixture(out var writer, out var source);
		InstallVendorServices(writer, db);

		var water = db.Liquids.Single(x => x.Name == "water");
		var refillRecord = db.SeederManagedRecords.Single(x => x.StableKey == "npc-service.water-refills");
		var provenance = JsonDocument.Parse(refillRecord.SeedBaseline!).RootElement;
		CollectionAssert.AreEquivalent(new[] { 37106, 37739 }, provenance.GetProperty("Sources").EnumerateArray().Select(x => x.GetInt32()).ToArray());
		Assert.AreEqual(water.Id, provenance.GetProperty("Liquid").GetInt64());
		Assert.AreEqual(0.05m, provenance.GetProperty("UnitVolume").GetDecimal());
		Assert.AreEqual(3, provenance.GetProperty("BitsPerUnit").GetInt32());
		StringAssert.Contains(provenance.GetProperty("Disposition").GetString()!, "round up to a whole bit");
		Assert.IsFalse(source.Npcs.Any(x => x.Vnum == 48008 && x.Programs.Contains("C fill az_waterseller")),
			"The unplaced Silla water seller must not activate.");

		var quote = db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileWaterSellerfill");
		StringAssert.Contains(quote.FunctionText, $"liquidrefillcost(@vessel, {water.Id}, 0.05, 3)");
		StringAssert.Contains(quote.FunctionText, $"purchaseliquidrefill(@actor, @seller, @vessel, tocurrency(9001), {water.Id}, 0.05, 3)");
		Assert.IsTrue(quote.FunctionText.IndexOf("liquidrefillcost", StringComparison.Ordinal) < quote.FunctionText.IndexOf("purchaseliquidrefill", StringComparison.Ordinal),
			"The native quote validates the exact refill before the matching native purchase/debit call.");
		var hooks = db.Hooks.Where(x => x.Name == "Worldfile Water Refills fill").ToArray();
		Assert.AreEqual(1, hooks.Length);
		var attached = TemplatesWithHook(db, hooks[0].Id);
		CollectionAssert.AreEquivalent(new[] { 37106L, 37739L }, attached);
		CompileGeneratedProgs(db);
	}

	[TestMethod]
	public void VendorServices_MountSalesKeepOnlyTheEighteenAssignedSourcePricesAndUseNativeMountPermission()
	{
		using var db = VendorServicesFixture(out var writer, out var source);
		InstallVendorServices(writer, db);

		var expected = new Dictionary<int, (int Vnum, int Price)[]>
		{
			[3004] = [(3001, 437), (3002, 472), (3003, 450), (20062, 517)],
			[20054] = [(50451, 375), (20062, 450), (48009, 750), (20056, 400), (3002, 650)],
			[37735] = [(50451, 500), (20062, 650), (37780, 875)],
			[50413] = [(3001, 350), (20062, 425), (48002, 500), (20056, 520), (3002, 520), (50451, 520)]
		};
		var records = db.SeederManagedRecords.AsEnumerable().Where(x => x.StableKey.StartsWith("npc-service.mount-sales.", StringComparison.Ordinal) &&
			int.TryParse(x.StableKey[("npc-service.mount-sales.".Length)..], out _)).ToDictionary(
			x => int.Parse(x.StableKey[("npc-service.mount-sales.".Length)..]));
		Assert.AreEqual(4, records.Count);
		var total = 0;
		foreach (var (seller, catalogue) in expected)
		{
			var json = JsonDocument.Parse(records[seller].SeedBaseline!).RootElement.GetProperty("Catalogue");
			var actual = json.EnumerateArray().Select(x => (Vnum: x.GetProperty("Vnum").GetInt32(), Price: x.GetProperty("Price").GetInt32())).ToArray();
			CollectionAssert.AreEqual(catalogue, actual, $"Seller {seller} catalogue and source prices");
			total += actual.Length;
			var list = db.FutureProgs.Single(x => x.FunctionName == $"ArmageddonWorldfileMountSeller{seller}list");
			Assert.AreEqual(catalogue.Length + 1, list.FunctionText.Split("send(@actor", StringSplitOptions.None).Length - 1,
				$"Seller {seller} emits one heading and only its assigned stock rows.");
			foreach (var (entry, index) in catalogue.Select((item, index) => (item, index)))
			{
				var animal = source.Npcs.Single(x => x.Vnum == entry.Vnum);
				StringAssert.Contains(list.FunctionText, $"{index + 1}) {animal.ShortDescription} — {entry.Price} bits");
			}
			var buy = db.FutureProgs.Single(x => x.FunctionName == $"ArmageddonWorldfileMountSeller{seller}buy");
			StringAssert.Contains(buy.FunctionText, "if (canmount(@actor, @animal))");
			StringAssert.Contains(buy.FunctionText, "mount(@actor, @animal)");
			Assert.IsTrue(buy.FunctionText.IndexOf("canmount(@actor, @animal)", StringComparison.Ordinal) < buy.FunctionText.IndexOf("mount(@actor, @animal)", StringComparison.Ordinal));
		}
		Assert.AreEqual(18, total);
		Assert.IsFalse(expected[50413].Any(x => x.Vnum == 50413), "The uninitialised seventh seller slot is not invented as stock.");

		var inert = db.SeederManagedRecords.Single(x => x.StableKey == "npc-service.mount-sales.inert.48008");
		Assert.AreEqual("SourceInertService", inert.EntityType);
		Assert.IsFalse(db.Hooks.Any(x => x.Name.Contains("48008", StringComparison.Ordinal)));
		Assert.IsFalse(db.FutureProgs.Any(x => x.FunctionName.StartsWith("ArmageddonWorldfileMountSeller48008", StringComparison.Ordinal)));
		CompileGeneratedProgs(db);
	}

	private static FuturemudDatabaseContext VendorServicesFixture(out ArmageddonWorldfileWriter writer, out ArmageddonWorldfilePackage source)
	{
		var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var included = new HashSet<int> { 37106, 37739, 48008, 3004, 20054, 37735, 50413,
			3001, 3002, 3003, 20062, 50451, 48009, 20056, 37780, 48002 };
		source = new ArmageddonWorldfilePackage
		{
			Npcs = archive.Npcs.Where(x => included.Contains(x.Vnum)).ToArray(),
			Resets = archive.Resets.Where(x => x.Opcode == "M" && x.ReferenceIssues.Length == 0 && x.Arguments.Length > 3 && included.Contains(x.Arguments[1])).ToArray()
		};
		writer = new ArmageddonWorldfileWriter(db, source);
		foreach (var npc in source.Npcs)
		{
			var template = new NpcTemplate { Id = npc.Vnum, RevisionNumber = 0, Definition = "<Definition><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>" };
			db.NpcTemplates.Add(template);
			Field<Dictionary<int, NpcTemplate>>(writer, "_npcs").Add(npc.Vnum, template);
			Field<Dictionary<int, long[]>>(writer, "_npcFamilyIds").Add(npc.Vnum, [template.Id]);
		}
		typeof(ArmageddonWorldfileWriter).GetField("_currency", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(writer, new Currency { Id = 9001 });
		db.SaveChanges();
		return db;
	}

	private static void InstallVendorServices(ArmageddonWorldfileWriter writer, FuturemudDatabaseContext db)
	{
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallVendorServices", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
	}

	private static long[] TemplatesWithHook(FuturemudDatabaseContext db, long hookId) => db.NpcTemplates.AsEnumerable()
		.Where(x => XElement.Parse(x.Definition).Element("TemplateLoadAdditions")!.Element("Hooks")!.Elements("Hook")
			.Any(h => (long)h.Attribute("id")! == hookId))
		.Select(x => x.Id).ToArray();

	private static void CompileGeneratedProgs(FuturemudDatabaseContext db)
	{
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		foreach (var prog in db.FutureProgs.ToArray()) compiler.Compile(prog.Id);
	}
}

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using MudSharp.Database;
using MudSharp.Framework;
using GameItemProto = MudSharp.Models.GameItemProto;
using MudSharp.GameItems;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Models;
using Moq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfileCustomClothingTests
{
	[TestMethod]
	public void CustomClothing_CompilesNativeCommandProgramsAndRetainsExactSourceCatalogueAndPrepaidReceiptPolicy()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var npc = archive.Npcs.Single(x => x.Vnum == 50429);
		var reset = archive.Resets.Single(x => x.Opcode == "M" && x.ReferenceIssues.Length == 0 && x.Arguments.Length > 3 &&
			x.Arguments[1] == 50429 && x.Arguments[3] == 50419);
		var room = archive.Rooms.Single(x => x.Vnum == 50419);
		var garments = archive.Items.Where(x => x.Vnum is >= 13700 and <= 13843).OrderBy(x => x.Vnum).ToArray();
		var ticket = archive.Items.Single(x => x.Vnum == 1598);
		Assert.AreEqual(144, garments.Length);
		Assert.IsTrue(garments.All(x => x.ItemType == 11), "Every source output is an ITEM_WORN prototype.");
		var source = new ArmageddonWorldfilePackage { Npcs = [npc], Resets = [reset], Rooms = [room], Items = [.. garments, ticket] };
		var writer = new ArmageddonWorldfileWriter(db, source);

		var zone = new Zone { Id = 9900, Name = "Custom Clothing Fixture" };
		db.Zones.Add(zone);
		Field<Dictionary<int, Zone>>(writer, "_zones").Add(room.Zone, zone);
		var sourceRoom = new Room { Id = 9901, ZoneId = zone.Id, UniqueName = "Vnum50419", EffectData = "<Effects/>" };
		db.Rooms.Add(sourceRoom);
		Field<Dictionary<int, Room>>(writer, "_rooms").Add(room.Vnum, sourceRoom);
		db.RoomOverlayPackages.Add(new RoomOverlayPackage { Id = 9902, Name = "Custom Clothing Fixture", RevisionNumber = 0 });
		Set(writer, "_overlays", db.RoomOverlayPackages.Local.Single());
		db.Terrains.Add(new Terrain { Id = 9903, Name = "Urban Street" });
		var template = new NpcTemplate { Id = 9904, RevisionNumber = 0, Definition = "<Definition><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>" };
		db.NpcTemplates.Add(template);
		Field<Dictionary<int, NpcTemplate>>(writer, "_npcs").Add(50429, template);
		Field<Dictionary<int, long[]>>(writer, "_npcFamilyIds").Add(50429, [template.Id]);
		var nextId = 10000L;
		foreach (var item in source.Items)
		{
			var proto = new GameItemProto { Id = nextId++, RevisionNumber = 0, Name = item.ShortDescription };
			db.GameItemProtos.Add(proto);
			Field<Dictionary<int, GameItemProto>>(writer, "_items").Add(item.Vnum, proto);
		}
		var currency = new Currency { Id = 9905, Name = "Obsidian" };
		db.Currencies.Add(currency);
		Set(writer, "_currency", currency);
		var sizingProg = new MudSharp.Models.FutureProg
		{
			Id = 9906, FunctionName = "ArmageddonWorldfileScalarWearableSize", FunctionText = "return 8",
			ReturnTypeDefinition = ProgVariableTypes.Number.ToStorageString(), AcceptsAnyParameters = false,
			StaticType = 0, Category = "Fixture", Subcategory = "Fixture", Public = false,
			FutureProgsParameters = { new() { ParameterIndex = 0, ParameterName = "character", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() } }
		};
		db.FutureProgs.Add(sizingProg);
		db.SaveChanges();

		typeof(ArmageddonWorldfileWriter).GetMethod("InstallCustomClothing", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);

		var generated = db.FutureProgs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfileCustomClothing", StringComparison.Ordinal)).ToArray();
		Assert.AreEqual(5, generated.Length);
		var buy = generated.Single(x => x.FunctionName.EndsWith("Buy", StringComparison.Ordinal)).FunctionText;
		var quote = generated.Single(x => x.FunctionName.EndsWith("Quote", StringComparison.Ordinal)).FunctionText;
		var collect = generated.Single(x => x.FunctionName.EndsWith("Collection", StringComparison.Ordinal)).FunctionText;
		StringAssert.Contains(buy, "max(1, @basePrice) + @fit * @materialPrice");
		StringAssert.Contains(buy, "@hours * 600");
		StringAssert.Contains(buy, $"loaditem({Field<Dictionary<int, GameItemProto>>(writer, "_items")[13843].Id})");
		StringAssert.Contains(buy, $"loaditem({Field<Dictionary<int, GameItemProto>>(writer, "_items")[13700].Id})");
		StringAssert.Contains(buy, "setwearablefitscalar(@garment, @fit)");
		StringAssert.Contains(buy, "purchaseitemdelivery(@actor, @seller, @chit, @custodyRoom");
		StringAssert.Contains(buy, "put(@chit, @custodyRoom)");
		StringAssert.Contains(buy, "silentget(@actor, @chit)");
		Assert.IsTrue(buy.IndexOf("put(@chit, @custodyRoom)", StringComparison.Ordinal) < buy.IndexOf("purchaseitemdelivery(", StringComparison.Ordinal));
		StringAssert.Contains(quote, "max(1, @basePrice) + @fit * @materialPrice");
		StringAssert.Contains(quote, "@args.Count(word, true) != 5");
		StringAssert.Contains(collect, "@ticketLink != @chit.id");
		StringAssert.Contains(collect, "@garment.proto != @expectedProto");
		StringAssert.Contains(collect, "wearablefitscalar(@garment) != @fit");
		StringAssert.Contains(collect, "if (@due > now())");
		StringAssert.Contains(collect, ", 0)");
		Assert.IsFalse(collect.Contains(", 0, @fitText)", StringComparison.Ordinal), "Prepaid collection must work with sizing disabled.");
		StringAssert.Contains(collect, "targetitem(@actor, @actor.inventory, getindex(@args, 0))");
		var record = System.Text.Json.JsonDocument.Parse(db.SeederManagedRecords.Single(x => x.StableKey == "npc-service.custom-clothing").SeedBaseline!);
		CollectionAssert.AreEqual(Enumerable.Range(13700, 144).ToArray(), record.RootElement.GetProperty("OutputVnums").EnumerateArray().Select(x => x.GetInt32()).ToArray());

		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		foreach (var prog in generated) compiler.Compile(prog.Id);
		var receipt = compiler.Compile(generated.Single(x => x.FunctionName.EndsWith("SaveReceipt", StringComparison.Ordinal)).Id);
		var garment = RuntimeItem(7123);
		var chit = RuntimeItem(8123);
		var due = DateTime.UtcNow.AddHours(2);
		Assert.IsTrue(receipt.ExecuteBool(garment.Object, chit.Object, 13843m, 8m, due, 1234m));
		var registers = ((MudSharp.FutureProg.FutureProg)receipt).Gameworld.VariableRegister;
		Assert.AreEqual(8123m, registers.GetValue(garment.Object, "armw_customclothing_ticket").GetObject);
		Assert.AreEqual(7123m, registers.GetValue(chit.Object, "armw_customclothing_garment").GetObject);
		Assert.AreEqual(13843m, registers.GetValue(chit.Object, "armw_customclothing_vnum").GetObject);
		Assert.AreEqual(8m, registers.GetValue(chit.Object, "armw_customclothing_fit").GetObject);
		Assert.AreEqual(due, registers.GetValue(chit.Object, "armw_customclothing_due").GetObject);
		Assert.AreEqual(1234m, registers.GetValue(chit.Object, "armw_customclothing_price").GetObject);
	}

	private static Mock<IGameItem> RuntimeItem(long id)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Type).Returns(ProgVariableTypes.Item);
		item.SetupGet(x => x.GetObject).Returns(() => item.Object);
		item.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(id));
		return item;
	}

	private static T Field<T>(ArmageddonWorldfileWriter writer, string name) =>
		(T)typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;

	private static void Set(ArmageddonWorldfileWriter writer, string name, object value) =>
		typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(writer, value);
}

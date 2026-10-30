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
using MudSharp.FutureProg;
using MudSharp.Models;
using Moq;
using MudSharp.FutureProg.Variables;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfileTailoringTests
{
	[TestMethod]
	public void GeneratedTailoringOrderQuoteAndCollectionProgramsCompileForAllEightTailors()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var vnums = new[] { 6157, 20038, 21013, 28305, 37746, 50454, 64045, 70105 };
		var tailors = archive.Npcs.Where(x => vnums.Contains(x.Vnum)).ToArray();
		var resets = archive.Resets.Where(x => x.Opcode == "M" && x.ReferenceIssues.Length == 0 && x.Arguments.Length > 3 && vnums.Contains(x.Arguments[1])).ToArray();
		var roomVnums = resets.Select(x => x.Arguments[3]).Distinct().ToHashSet();
		var ticket = archive.Items.Single(x => x.Vnum == 1598);
		var wearable = archive.Items.First(x => x.ItemType == 11 && (x.Flags[1] & (1 << 6)) == 0);
		var armour = archive.Items.First(x => x.ItemType == 9 && (x.Flags[1] & (1 << 6)) == 0);
		var source = new ArmageddonWorldfilePackage
		{
			Npcs = tailors,
			Items = [ticket, wearable, armour],
			Resets = resets,
			Rooms = archive.Rooms.Where(x => roomVnums.Contains(x.Vnum)).ToArray()
		};
		Assert.AreEqual(8, tailors.Length);
		Assert.IsTrue(source.Resets.Length > 0);
		var writer = new ArmageddonWorldfileWriter(db, source);

		var templates = Field<Dictionary<int, NpcTemplate>>(writer, "_npcs");
		var families = Field<Dictionary<int, long[]>>(writer, "_npcFamilyIds");
		var nextTemplateId = (db.NpcTemplates.Max(x => (long?)x.Id) ?? 0) + 1;
		foreach (var npc in tailors)
		{
			var template = new NpcTemplate { Id = nextTemplateId++, RevisionNumber = 0, Definition = "<Definition><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>" };
			db.NpcTemplates.Add(template);
			templates.Add(npc.Vnum, template);
			families.Add(npc.Vnum, [template.Id]);
		}
		var prototypes = Field<Dictionary<int, GameItemProto>>(writer, "_items");
		var effective = Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems");
		var nextItemId = (db.GameItemProtos.Max(x => (long?)x.Id) ?? 0) + 1;
		foreach (var item in source.Items)
		{
			var proto = new GameItemProto { Id = nextItemId++, RevisionNumber = 0 };
			db.GameItemProtos.Add(proto);
			prototypes.Add(item.Vnum, proto);
			effective.Add(proto.Id, (item, item.Values));
		}

		var currency = new Currency { Id = 9001, Name = "Obsidian" };
		db.Currencies.Add(currency);
		Set(writer, "_currency", currency);
		Set(writer, "_overlays", new RoomOverlayPackage { Id = 8000, Name = "Armageddon Fixture", RevisionNumber = 0 });
		db.RoomOverlayPackages.Add((RoomOverlayPackage)typeof(ArmageddonWorldfileWriter).GetField("_overlays", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!);
		db.Terrains.Add(new Terrain { Id = 8001, Name = "Urban Street" });
		var zones = Field<Dictionary<int, Zone>>(writer, "_zones");
		var rooms = Field<Dictionary<int, Room>>(writer, "_rooms");
		var nextRoomId = (db.Rooms.Max(x => (long?)x.Id) ?? 0) + 1;
		foreach (var sourceRoom in source.Rooms)
		{
			if (!zones.TryGetValue(sourceRoom.Zone, out var zone))
			{
				zone = new Zone { Id = 9000 + sourceRoom.Zone, Name = "Tailoring fixture zone " + sourceRoom.Zone };
				db.Zones.Add(zone);
				zones.Add(sourceRoom.Zone, zone);
			}
			var room = new Room { Id = nextRoomId++, ZoneId = zone.Id, UniqueName = "TailoringFixture" + sourceRoom.Vnum, EffectData = "<Effects/>" };
			db.Rooms.Add(room);
			rooms.Add(sourceRoom.Vnum, room);
		}

		var sizingProg = new MudSharp.Models.FutureProg
		{
			Id = (db.FutureProgs.Max(x => (long?)x.Id) ?? 0) + 1, FunctionName = "ArmageddonWorldfileScalarWearableSize", FunctionText = "return 10",
			ReturnTypeDefinition = ProgVariableTypes.Number.ToStorageString(), AcceptsAnyParameters = false,
			StaticType = 0, Category = "Fixture", Subcategory = "Fixture", Public = false,
			FutureProgsParameters = { new() { ParameterIndex = 0, ParameterName = "character", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() } }
		};
		db.FutureProgs.Add(sizingProg);
		Set(writer, "_wearableScalarSizeProgId", sizingProg.Id);
		db.SaveChanges();

		typeof(ArmageddonWorldfileWriter).GetMethod("InstallTailoring", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		var generated = db.FutureProgs.Where(x => x.FunctionName.StartsWith("ArmageddonWorldfileTailor", StringComparison.Ordinal)).ToArray();
		Assert.AreEqual(5, generated.Length);
		StringAssert.Contains(generated.Single(x => x.FunctionName.EndsWith("Order", StringComparison.Ordinal)).FunctionText, "wearablefitscalar(@item)");
		StringAssert.Contains(generated.Single(x => x.FunctionName.EndsWith("Collection", StringComparison.Ordinal)).FunctionText, "purchaseitemdelivery(");
		foreach (var prog in generated) compiler.Compile(prog.Id);
		var deficit = compiler.Compile(generated.Single(x => x.FunctionName.EndsWith("ConditionDeficit", StringComparison.Ordinal)).Id);
		Assert.AreEqual(25m, deficit.Execute(100, 0.75));
		Assert.AreEqual(0m, deficit.Execute(100, 1.0));
		Assert.AreEqual(100m, deficit.Execute(100, 0.0));
		Assert.AreEqual(0m, deficit.Execute(0, 0.0));
		var matching = compiler.Compile(generated.Single(x => x.FunctionName.EndsWith("TicketMatches", StringComparison.Ordinal)).Id);
		var garment = RuntimeItem(150);
		var original = RuntimeItem(200);
		var duplicate = RuntimeItem(201);
		Assert.IsTrue(((MudSharp.FutureProg.FutureProg)matching).Gameworld.VariableRegister.SetValue(garment, "armw_tailor_receipt", new NumberVariable(200m)));
		Assert.AreEqual(true, matching.Execute(garment, original));
		Assert.AreEqual(false, matching.Execute(garment, duplicate));
	}

	private static MudSharp.GameItems.IGameItem RuntimeItem(long id)
	{
		var item = new Mock<MudSharp.GameItems.IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Type).Returns(ProgVariableTypes.Item);
		item.SetupGet(x => x.GetObject).Returns(item.Object);
		item.Setup(x => x.GetProperty("id")).Returns(new NumberVariable((decimal)id));
		return item.Object;
	}

	private static T Field<T>(ArmageddonWorldfileWriter writer, string name) =>
		(T)typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;

	private static void Set(ArmageddonWorldfileWriter writer, string name, object value) =>
		typeof(ArmageddonWorldfileWriter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(writer, value);
}

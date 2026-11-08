#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Models;
using GameItemProto = MudSharp.Models.GameItemProto;
using RuntimeRoom = MudSharp.Construction.IRoom;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfileSavedPopulationTests
{
	[TestMethod]
	public void SavedRooms_RemovedContentsAndNewInterior_AreNotConfusedOnLaterBoots()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var item = new ArmWorldItem(new("fixture", 1), 401, -1, "fixture", "a fixture", "", "", [15, 0, 0, 0, 0], [0, 0, 0, 0, 0, 0], [], [], [], [], []);
		var reset = new ArmWorldReset(new("fixture", 1), 3, "O", [0, 401, 1, 201, 0, 0, 0, 0, 0, 0, 0], [], []);
		var source = new ArmageddonWorldfilePackage { Items = [item], Zones = [new(new("fixture", 1), 3, 3, "Fixture", [3999, 15, 0, 1, 0, 1, 0], [])] };
		var writer = new ArmageddonWorldfileWriter(db, source);
		Field<Dictionary<int, Room>>(writer, "_rooms").Add(201, new Room { Id = 9201 });
		Field<Dictionary<int, GameItemProto>>(writer, "_items").Add(401, new GameItemProto { Id = 9401 });
		var build = typeof(ArmageddonWorldfileWriter).GetMethod("BuildZoneResetProgram", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var code = (string)build.Invoke(writer, [3, new[] { reset }, new Dictionary<(int, int), NpcTemplate>(), true])!;
		var prog = new MudSharp.Models.FutureProg { Id = 99002, FunctionName = "SavedPopulationFixture", Category = "Fixture", Subcategory = "Fixture",
			ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(), FunctionText = code };
		db.FutureProgs.Add(prog);
		db.SaveChanges();
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		var compiled = compiler.Compile(prog.Id);
		var world = (IFuturemud)typeof(OfflineProgCompilation).GetField("_world", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(compiler)!;
		var template = Room(9201, world);
		template.Setup(x => x.EffectsOfType<RoomTemplateInstance>(It.IsAny<Predicate<RoomTemplateInstance>>()))
			.Returns([new RoomTemplateInstance(template.Object, 9201, true)]);
		world.Add(template.Object);
		var prototype = new Mock<IGameItemProto>();
		prototype.SetupGet(x => x.Id).Returns(9401);
		prototype.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		prototype.SetupGet(x => x.Components).Returns([]);
		var nextItem = 9600L;
		prototype.Setup(x => x.CreateNew(null, null, 1, "")).Returns(() => new[] { ExistingItem(9401, nextItem++) });
		world.Add(prototype.Object);
		var contents = new List<IGameItem>();
		var first = Room(9202, world, contents);
		first.Setup(x => x.EffectsOfType<RoomTemplateInstance>(It.IsAny<Predicate<RoomTemplateInstance>>()))
			.Returns([new RoomTemplateInstance(first.Object, 9201, false)]);
		world.Add(first.Object);
		Assert.AreEqual(true, compiled.Execute());
		Assert.AreEqual(1, contents.Count, "First initialisation must really create its source object.");
		Assert.AreEqual(true, world.VariableRegister.GetValue(first.Object, ArmageddonWorldfileWriter.SavedPopulationRegister(3)).GetObject);
		contents.Clear(); // The original chest was taken or destroyed.
		Assert.AreEqual(true, compiled.Execute(), "A later boot must not call the missing item factory to refill this room.");
		Assert.AreEqual(0, contents.Count);

		var secondContents = new List<IGameItem>();
		var second = Room(9203, world, secondContents);
		second.Setup(x => x.EffectsOfType<RoomTemplateInstance>(It.IsAny<Predicate<RoomTemplateInstance>>()))
			.Returns([new RoomTemplateInstance(second.Object, 9201, false)]);
		world.Add(second.Object);
		Assert.AreEqual(true, compiled.Execute());
		Assert.AreEqual(true, world.VariableRegister.GetValue(second.Object, ArmageddonWorldfileWriter.SavedPopulationRegister(3)).GetObject,
			"A new interior must initialise independently while the older empty interior stays untouched.");
		Assert.AreEqual(0, contents.Count);
		Assert.AreEqual(1, secondContents.Count);
		prototype.Verify(x => x.CreateNew(null, null, 1, ""), Times.Exactly(2));
	}

	[TestMethod]
	public void SavedPopulation_CompletedRoomDoesNotRecreateAbsentNpcOrNestedItems()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var item = new ArmWorldItem(new("fixture", 1), 401, -1, "fixture", "a fixture", "", "", [15, 0, 0, 0, 0], [0, 0, 0, 0, 0, 0], [], [], [], [], []);
		var npc = new ArmWorldNpc(new("fixture", 1), 501, -1, "resident", "a resident", "", "", "", [], [], [], [], 0, [], [], [], []);
		var resets = new[]
		{
			new ArmWorldReset(new("fixture", 1), 3, "M", [0, 501, 1, 201], [], []),
			new ArmWorldReset(new("fixture", 2), 3, "O", [0, 401, 1, 201, 0, 0, 0, 0, 0, 0, 0], [], []),
			new ArmWorldReset(new("fixture", 3), 3, "P", [1, 401, 1, 401, 0, 0, 0, 0, 0, 0, 0], [], [])
		};
		var zones = new[] { 2, 3 }.Select(z => new ArmWorldZone(new("fixture", 1), z, z, "Fixture", [3999, 15, 0, 1, 0, 1, 0], [])).ToArray();
		var writer = new ArmageddonWorldfileWriter(db, new ArmageddonWorldfilePackage { Items = [item], Npcs = [npc], Zones = zones });
		Field<Dictionary<int, Room>>(writer, "_rooms").Add(201, new Room { Id = 9201 });
		Field<Dictionary<int, GameItemProto>>(writer, "_items").Add(401, new GameItemProto { Id = 9401 });
		Field<Dictionary<int, long[]>>(writer, "_npcFamilyIds").Add(501, [9501]);
		var variants = new Dictionary<(int, int), NpcTemplate> { [(3, 1)] = new() { Id = 9501 } };
		var build = typeof(ArmageddonWorldfileWriter).GetMethod("BuildZoneResetProgram", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var code = (string)build.Invoke(writer, [3, resets, variants, true])!;
		var ordinary = (string)build.Invoke(writer, [2, resets, variants, true])!;
		Assert.IsFalse(ordinary.Contains("initialrooms", StringComparison.Ordinal), "Ordinary mode-zero zones must retain their boot behaviour.");
		var prog = new MudSharp.Models.FutureProg { Id = 99003, FunctionName = "SavedNpcAndContentsFixture", Category = "Fixture", Subcategory = "Fixture",
			ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(), FunctionText = code };
		db.FutureProgs.Add(prog);
		db.SaveChanges();
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		var compiled = compiler.Compile(prog.Id);
		var world = (IFuturemud)typeof(OfflineProgCompilation).GetField("_world", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(compiler)!;
		var room = Room(9201, world);
		world.Add(room.Object);
		world.VariableRegister.SetValue(room.Object, ArmageddonWorldfileWriter.SavedPopulationRegister(3), new BooleanVariable(true));
		Assert.AreEqual(true, compiled.Execute(), "Neither factory exists: success proves an empty initialised room does not respawn residents or containers.");
	}

	[TestMethod]
	public void SavedZoneClassification_UsesSourceAutosaveAssignmentsRatherThanModeZero()
	{
		ArmageddonWorldfileTestData.RequireAvailable();
		var source = ArmageddonWorldfileTestData.Load().ForInstallation();
		var saved = source.Zones.Where(x => ArmageddonWorldfileWriter.IsSourceSavedPopulationZone(x.Index)).ToArray();
		CollectionAssert.AreEqual(new[] { 3, 13, 33, 73 }, saved.Select(x => x.Index).ToArray());
		Assert.IsTrue(saved.All(x => x.Fields[2] == 0));
		Assert.IsTrue(source.Resets.Where(x => ArmageddonWorldfileWriter.IsSourceSavedPopulationZone(x.Zone) &&
			(x.Opcode is "M" or "O")).All(x => x.Arguments[0] == 0), "Cross-root source conditionals require a separate mapping review.");
		Assert.IsTrue(source.Zones.Any(x => x.Fields[2] == 0 && !ArmageddonWorldfileWriter.IsSourceSavedPopulationZone(x.Index)));
		Assert.IsFalse(ArmageddonWorldfileWriter.IsSourceSavedPopulationZone(50), "Allanak is not a source auto-save zone merely because it uses mode zero.");
	}

	private static Mock<RuntimeRoom> Room(long id, IFuturemud world, List<IGameItem>? items = null)
	{
		items ??= [];
		var room = new Mock<RuntimeRoom>();
		room.SetupGet(x => x.Id).Returns(id);
		room.SetupGet(x => x.Gameworld).Returns(world);
		room.SetupGet(x => x.Type).Returns(ProgVariableTypes.Location);
		room.SetupGet(x => x.GetObject).Returns(room.Object);
		room.Setup(x => x.GetProperty("id")).Returns(new NumberVariable(id));
		room.Setup(x => x.GetProperty("items")).Returns(() => new CollectionVariable(items, ProgVariableTypes.Item));
		room.Setup(x => x.Insert(It.IsAny<IGameItem>(), false)).Callback<IGameItem, bool>((item, _) => items.Add(item));
		return room;
	}

	private static IGameItem ExistingItem(long prototype, long id = 9601)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Type).Returns(ProgVariableTypes.Item);
		item.SetupGet(x => x.GetObject).Returns(item.Object);
		item.Setup(x => x.GetProperty("proto")).Returns(new NumberVariable(prototype));
		return item.Object;
	}

	private static T Field<T>(object target, string name) =>
		(T)target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(target)!;
}

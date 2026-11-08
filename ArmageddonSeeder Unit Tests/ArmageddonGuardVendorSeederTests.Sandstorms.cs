#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonGuardVendorSeederTests
{
	[TestMethod]
	public void Sandstorms_CompileActualRoutesAndRestoreSavedPositionsWithoutRestartingThem()
	{
		using var db = SandstormFixture(out var writer);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallSandstorms", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		var positions = db.VariableDefaults.ToDictionary(x => x.Property, x => decimal.Parse(XElement.Parse(x.DefaultValue).Value));
		var (world, progs, _) = Runtime(db, register =>
		{
			register.Setup(x => x.GetType(ProgVariableTypes.Location, It.IsAny<string>())).Returns(ProgVariableTypes.Number);
			register.Setup(x => x.GetValue(It.IsAny<IProgVariable>(), It.IsAny<string>())).Returns((IProgVariable _, string key) => new NumberVariable(positions[key]));
			register.Setup(x => x.SetValue(It.IsAny<IProgVariable>(), It.IsAny<string>(), It.IsAny<IProgVariable>()))
				.Callback<IProgVariable, string, IProgVariable>((_, key, value) => positions[key] = (decimal)value.GetObject).Returns(true);
		});
		var rooms = new All<IRoom>();
		var mocks = new Dictionary<int, Mock<IRoom>>();
		var effects = new Dictionary<long, List<RoomWeatherEffect>>();
		foreach (var source in Field<ArmageddonWorldfilePackage>(writer, "_source").Rooms)
		{
			var room = Item<IRoom>(100000 + source.Vnum, ProgVariableTypes.Location);
			var list = new List<RoomWeatherEffect>(); effects.Add(room.Object.Id, list);
			room.SetupGet(x => x.Gameworld).Returns(world.Object);
			room.Setup(x => x.OutdoorsType(null)).Returns(RoomOutdoorsType.Outdoors);
			room.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			room.Setup(x => x.EffectsOfType<RoomWeatherEffect>(null)).Returns(list);
			room.Setup(x => x.AddEffect(It.IsAny<MudSharp.Effects.IEffect>())).Callback<MudSharp.Effects.IEffect>(e => list.Add((RoomWeatherEffect)e));
			room.Setup(x => x.RemoveEffect(It.IsAny<MudSharp.Effects.IEffect>(), true)).Callback<MudSharp.Effects.IEffect, bool>((e, _) => list.Remove((RoomWeatherEffect)e));
			rooms.Add(room.Object); mocks.Add(source.Vnum, room);
		}
		world.SetupGet(x => x.Rooms).Returns(rooms);
		var weather = Mock.Of<IWeatherEvent>(x => x.Id == db.WeatherEvents.Single().Id);
		world.SetupGet(x => x.WeatherEvents).Returns(new All<IWeatherEvent> { weather });
		var restore = progs.GetByName("ArmageddonWorldfileSandstormRestore");
		Assert.IsTrue(restore.ExecuteBool());
		Assert.AreEqual(3, effects.Values.Sum(x => x.Count));
		Assert.IsTrue(restore.ExecuteBool()); Assert.AreEqual(3, effects.Values.Sum(x => x.Count));
		var next = progs.GetByName("ArmageddonWorldfileSandstormNext");
		var sourceRoom = Field<ArmageddonWorldfilePackage>(writer, "_source").Rooms.Single(x => x.Vnum == 48167);
		foreach (var turn in new[] { false, true })
		{
			var destination = sourceRoom.Exits.Single(x => x.Direction == (turn ? 0 : 1)).Destination;
			Assert.AreSame(mocks[destination].Object, next.Execute(mocks[48167].Object, turn));
		}
		var dust = Mock.Of<MudSharp.Form.Material.ISolid>(x => x.Id == db.Materials.Single(m => m.Name == "dust").Id);
		world.SetupGet(x => x.Materials).Returns(new All<MudSharp.Form.Material.ISolid> { dust });
		var deposit = Item<IGameItem>(9876, ProgVariableTypes.Item);
		var prototype = new Mock<IGameItemProto>();
		prototype.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		prototype.Setup(x => x.CreateNew(null, null, 1, "")).Returns(new[] { deposit.Object });
		var prototypes = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		prototypes.Setup(x => x.Get(48009)).Returns(prototype.Object); world.SetupGet(x => x.ItemProtos).Returns(prototypes.Object);
		var npc = Actor(); npc.Setup(x => x.GetProperty("isawake")).Returns(new BooleanVariable(true));
		npc.Setup(x => x.GetProperty("incombat")).Returns(new BooleanVariable(false));
		using (Constants.PushRandom(new SandstormRolls(2, 1, 1, 2)))
			Assert.IsTrue(progs.GetByName("ArmageddonWorldfileSandstormController").ExecuteBool(npc.Object));
		mocks[48167].Verify(x => x.Insert(deposit.Object, false), Times.Once);
		Assert.AreEqual(mocks[sourceRoom.Exits.Single(x => x.Direction == 1).Destination].Object.Id, positions["armw_sandstorm_0"]);
		Assert.AreEqual(0, effects[mocks[48167].Object.Id].Count);
		Assert.AreEqual(1, effects[(long)positions["armw_sandstorm_0"]].Count);
		positions["armw_sandstorm_0"] = mocks[48165].Object.Id;
		foreach (var list in effects.Values) list.Clear();
		Assert.IsTrue(restore.ExecuteBool());
		Assert.AreEqual(1, effects[mocks[48165].Object.Id].Count);
		Assert.AreEqual(0, effects[mocks[48167].Object.Id].Count);
		Assert.AreEqual(900, db.GameItemProtos.Single().MorphTimeSeconds);
		Assert.IsNull(db.GameItemProtos.Single().MorphGameItemProtoId);
		Assert.IsTrue(db.Materials.Any(x => x.Name == "dust" && x.ResidueSdesc == "(dusty)"));
		var settings = WeatherHazardSettings.FromXml(XElement.Parse(db.WeatherEvents.Single().AdditionalInfo).Element("Hazards"));
		Assert.AreEqual(0L, settings.ResidueMaterialId, "The local controller supplies residue independently of regional weather ticks.");
		Assert.IsFalse(db.Hooks.Any(x => x.Type == "InputHook"), "Debug-coordinate talk is not installed.");
	}

	private sealed class SandstormRolls(params int[] values) : Random
	{
		private readonly Queue<int> _values = new(values);
		public override int Next(int minValue, int maxValue)
		{
			var value = _values.Dequeue();
			Assert.IsTrue(value >= minValue && value < maxValue);
			return value;
		}
	}

	private static FuturemudDatabaseContext SandstormFixture(out ArmageddonWorldfileWriter writer)
	{
		var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var npc = archive.Npcs.Single(x => x.Vnum == 48102);
		var source = new ArmageddonWorldfilePackage { Npcs = [npc], Rooms = archive.Rooms.Where(x => x.Zone == 48).ToArray(), Items = archive.Items.Where(x => x.Vnum == 48009).ToArray() };
		writer = new ArmageddonWorldfileWriter(db, source);
		db.Materials.Add(new Db.Material { Id = 999, Name = "sand", MaterialDescription = "sand", ResidueSdesc = "(sandy)", ResidueDesc = "It is covered in sand.", ResidueColour = "yellow", SolventVolumeRatio = 2 });
		foreach (var room in source.Rooms)
		{
			var native = new Db.Room { Id = 100000 + room.Vnum, UniqueName = "Vnum" + room.Vnum, EffectData = "<Effects/>" };
			db.Rooms.Add(native); Field<Dictionary<int, Db.Room>>(writer, "_rooms").Add(room.Vnum, native);
		}
		var item = new Db.GameItemProto { Id = 48009, Name = "deposit" }; db.GameItemProtos.Add(item);
		Field<Dictionary<int, Db.GameItemProto>>(writer, "_items").Add(48009, item);
		Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Add(item.Id, (source.Items.Single(), source.Items.Single().Values));
		var cash = new Db.FutureProg { Id = 248102, FunctionName = "ArmageddonWorldfileNpcCash48102", FunctionText = "return true", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString() };
		cash.FutureProgsParameters.Add(new Db.FutureProgsParameter { ParameterIndex = 0, ParameterName = "npc", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		db.FutureProgs.Add(cash); Field<List<Db.FutureProg>>(writer, "_progs").Add(cash);
		var template = new Db.NpcTemplate { Id = 48102, Definition = "<Definition><OnLoadProg>248102</OnLoadProg><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>" };
		db.NpcTemplates.Add(template); Field<Dictionary<int, Db.NpcTemplate>>(writer, "_npcs").Add(48102, template);
		Field<Dictionary<int, long[]>>(writer, "_npcFamilyIds").Add(48102, [48102]);
		db.SaveChanges(); return db;
	}
}

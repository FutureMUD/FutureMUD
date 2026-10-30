#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonGuardVendorSeederTests
{
	[TestMethod]
	[DataRow(56101, 6)]
	[DataRow(56105, 1)]
	public void Plants_SourceInitialPhaseAndSixPulseRecurrence_CreateFruitInsideThePlant(int vnum, int firstPulse)
	{
		using var db = PlantFixture(out var writer, vnum); InstallPlants(writer, db);
		var f = new PlantRuntime(db, writer, vnum);
		for (var i = 1; i < firstPulse; i++) { f.Tick(); Assert.AreEqual(0, f.Created.Count); }
		f.Tick(); Assert.AreEqual(1, f.Created.Count); Assert.AreEqual(0m, f.Counter);
		Assert.AreSame(f.Created.Single().Object, f.Contents.Single());
		for (var i = 0; i < 5; i++) f.Tick();
		Assert.AreEqual(1, f.Created.Count);
		f.Tick(); Assert.AreEqual(2, f.Created.Count); Assert.AreEqual(2, f.Contents.Count);
		f.Container.Verify(x => x.CanPut(It.IsAny<IGameItem>()), Times.Never, "Natural growth uses the source budget, not player insertion rules.");
		Assert.AreEqual(vnum == 56105 ? 100m : 0m, db.GameItemProtos.Single(x => x.Id == vnum).CostInBaseCurrency);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Plants_StrictCapacityAndEffectiveResetOverride_PreserveOrDiscardOnlyTheCandidate(bool variant)
	{
		using var db = PlantFixture(out var writer, 56105); InstallPlants(writer, db);
		var f = new PlantRuntime(db, writer, variant ? 156105 : 56105);
		f.Contents.Add(f.WeightedItem(99, 0.02 * f.Kilogram).Object);
		f.Tick();
		Assert.AreEqual(1, f.Created.Count); Assert.AreEqual(0m, f.Counter);
		Assert.AreEqual(variant ? 2 : 1, f.Contents.Count);
		f.Created.Single().Verify(x => x.Delete(), variant ? Times.Never() : Times.Once());
	}

	[TestMethod]
	public void Plants_CarriedInstance_AdvancesPhaseWithoutCreatingFruit()
	{
		using var db = PlantFixture(out var writer, 56105); InstallPlants(writer, db);
		var f = new PlantRuntime(db, writer, 56105, ground: false);
		f.Tick(); Assert.AreEqual(0m, f.Counter); Assert.AreEqual(0, f.Created.Count);
	}

	[TestMethod]
	[DataRow(45405)]
	[DataRow(65940)]
	public void Plants_UnmappedGreelzaAndUnplacedKalan_RemainInert(int vnum)
	{
		using var db = PlantFixture(out var writer, vnum); InstallPlants(writer, db);
		Assert.AreEqual(0, db.ProgSchedules.Count());
		Assert.IsFalse(db.VariableDefinitions.Any(x => x.Property == "armw_plant_pulses"));
	}

	[TestMethod]
	public void Plants_FullRetainedSelection_CompilesSeventeenMappingsAndExcludesBrokenOrUnplacedDefinitions()
	{
		int[] plants = [25015, 56101, 56103, 56105, 56108, 56110, 56113, 56115, 56117, 56120, 56122, 56124, 56128, 56131, 56133, 56136, 56138, 45405, 65940];
		using var db = PlantFixture(out var writer, plants); InstallPlants(writer, db);
		Assert.AreEqual(17, Field<Dictionary<string, int>>(writer, "_counts")["fruiting plant definitions"]);
		Assert.AreEqual(34, db.GameItemProtosOnLoadProgs.Count());
		using var compiler = new OfflineProgCompilation(db.FutureProgs.ToArray(), db.VariableDefinitions.ToArray());
		foreach (var prog in db.FutureProgs) compiler.Compile(prog.Id);
	}

	private static void InstallPlants(ArmageddonWorldfileWriter writer, FuturemudDatabaseContext db)
	{
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallPlantGrowth", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
	}

	private static FuturemudDatabaseContext PlantFixture(out ArmageddonWorldfileWriter writer, params int[] vnums)
	{
		var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var fruitVnums = vnums.Where(x => x != 45405).Select(x => x switch { 25015 => 25309, 65940 => 50306, _ => x + 1 });
		var source = new ArmageddonWorldfilePackage { Items = archive.Items.Where(x => vnums.Concat(fruitVnums).Contains(x.Vnum)).ToArray(),
			Resets = archive.Resets.Where(x => x.Opcode == "O" && vnums.Contains(x.Arguments[1])).ToArray() };
		writer = new ArmageddonWorldfileWriter(db, source);
		foreach (var item in source.Items)
		{
			var proto = new Db.GameItemProto { Id = item.Vnum, Name = item.Name, CostInBaseCurrency = item.Physical[1] };
			db.GameItemProtos.Add(proto); Field<Dictionary<int, Db.GameItemProto>>(writer, "_items").Add(item.Vnum, proto);
			Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Add(proto.Id, (item, item.Values));
			if (!vnums.Contains(item.Vnum)) continue;
			var variant = new Db.GameItemProto { Id = item.Vnum + 100000, Name = item.Name };
			db.GameItemProtos.Add(variant); var values = item.Values.ToArray(); values[0]++;
			Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Add(variant.Id, (item, values));
		}
		db.Clocks.Add(new Db.Clock { Id = 20, PrimaryTimezoneId = 20, Definition = "<Clock><InGameSecondsPerRealSecond>6</InGameSecondsPerRealSecond><SecondsPerMinute>60</SecondsPerMinute></Clock>" });
		db.Timezones.Add(new Db.Timezone { Id = 20, ClockId = 20, Name = "AST" });
		db.Calendars.Single().FeedClockId = 20; db.Calendars.Single().Date = "1/1/1";
		db.SaveChanges(); return db;
	}

	private sealed class PlantRuntime
	{
		public List<IGameItem> Contents { get; } = [];
		public List<Mock<IGameItem>> Created { get; } = [];
		public Mock<IContainer> Container { get; } = new();
		public double Kilogram { get; }
		public decimal Counter => _counters.GetValueOrDefault(_parent.Object.Id, -1m);
		private readonly Dictionary<long, decimal> _counters = new();
		private readonly Mock<IGameItem> _parent;
		private readonly IFutureProg _pulse;
		public PlantRuntime(FuturemudDatabaseContext db, ArmageddonWorldfileWriter writer, long prototype, bool ground = true)
		{
			Kilogram = Field<double>(writer, "_kilograms");
			var (world, progs, _) = Runtime(db, registers =>
			{
				registers.Setup(x => x.GetType(ProgVariableTypes.Item, "armw_plant_pulses")).Returns(ProgVariableTypes.Number);
				registers.Setup(x => x.GetValue(It.IsAny<IProgVariable>(), "armw_plant_pulses"))
					.Returns((IProgVariable item, string _) => new NumberVariable(_counters.GetValueOrDefault(((IFrameworkItem)item.GetObject).Id, -1m)));
				registers.Setup(x => x.SetValue(It.IsAny<IProgVariable>(), "armw_plant_pulses", It.IsAny<IProgVariable>()))
					.Callback<IProgVariable, string, IProgVariable>((item, _, value) => _counters[((IFrameworkItem)item.GetObject).Id] = (decimal)value.GetObject).Returns(true);
			});
			var profile = Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems")[prototype];
			var fruit = profile.Source.Vnum == 25015 ? 25309 : profile.Source.Vnum + 1;
			var fruitSource = Field<ArmageddonWorldfilePackage>(writer, "_source").Items.Single(x => x.Vnum == fruit);
			_parent = Item<IGameItem>(700000 + prototype, ProgVariableTypes.Item);
			_parent.Setup(x => x.GetProperty("proto")).Returns(new NumberVariable(prototype));
			var room = Item<IRoom>(1, ProgVariableTypes.Location);
			// Native item locations resolve through holders/containers too; only direct membership is ground placement.
			_parent.Setup(x => x.GetProperty("location")).Returns(room.Object);
			room.Setup(x => x.GetProperty("items")).Returns(new CollectionVariable(ground ? new[] { _parent.Object } : Array.Empty<IGameItem>(), ProgVariableTypes.Item));
			_parent.Setup(x => x.GetProperty("contents")).Returns(() => new CollectionVariable(Contents, ProgVariableTypes.Item));
			_parent.Setup(x => x.GetProperty("weight")).Returns(() => new NumberVariable(profile.Source.Physical[0] / 100.0 * Kilogram + Contents.Sum(x => x.Weight)));
			_parent.Setup(x => x.GetItemType<IContainer>()).Returns(Container.Object);
			Container.Setup(x => x.CanPut(It.IsAny<IGameItem>())).Returns(false);
			Container.Setup(x => x.Put(null, It.IsAny<IGameItem>(), true)).Callback<ICharacter?, IGameItem, bool>((_, item, _) => Contents.Add(item));
			world.SetupGet(x => x.Items).Returns(new All<IGameItem> { _parent.Object });
			var fruitProto = new Mock<IGameItemProto>(); fruitProto.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
			fruitProto.Setup(x => x.CreateNew(null, null, 1, "")).Returns(() =>
			{
				var item = WeightedItem(1000 + Created.Count, fruitSource.Physical[0] / 100.0 * Kilogram); Created.Add(item); return new[] { item.Object };
			});
			var prototypes = new Mock<IUneditableRevisableAll<IGameItemProto>>(); prototypes.Setup(x => x.Get(fruit)).Returns(fruitProto.Object);
			world.SetupGet(x => x.ItemProtos).Returns(prototypes.Object);
			var init = db.GameItemProtos.Single(x => x.Id == prototype).GameItemProtosOnLoadProgs.Single();
			Assert.IsTrue(progs.Get(init.FutureProgId).ExecuteBool(_parent.Object));
			_pulse = progs.GetByName("ArmageddonWorldfilePlantPulse");
		}
		public void Tick() => Assert.IsTrue(_pulse.ExecuteBool());
		public Mock<IGameItem> WeightedItem(long id, double weight)
		{
			var item = Item<IGameItem>(id, ProgVariableTypes.Item);
			item.SetupGet(x => x.Weight).Returns(weight); item.Setup(x => x.GetProperty("weight")).Returns(new NumberVariable(weight)); return item;
		}
	}
}

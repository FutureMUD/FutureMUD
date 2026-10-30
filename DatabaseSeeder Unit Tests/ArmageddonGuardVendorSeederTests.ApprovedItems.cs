#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonGuardVendorSeederTests
{
	[TestMethod]
	public void Tables_OnlyPlacedSourceLayoutsBecomeNativeFurnitureAndAllResetProgramsCompile()
	{
		using var db = ApprovedItemFixture(out var writer);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallTables", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(writer, null);
		var records = db.SeederManagedRecords.Local.Where(x => x.StableKey.StartsWith("table-layout.")).ToArray();
		var rooms = records.Select(x => int.Parse(x.StableKey.Split('.')[1])).Distinct().ToArray();
		Assert.AreEqual(31, rooms.Length);
		Assert.IsNull(writer.TryBuildTableReset(20, "destination900", 900, "command"), "No seating is invented where the source has no layout.");
		Assert.IsNull(writer.TryBuildTableReset(3067, "destination900", 900, "command"), "An unplaced layout must not activate.");
		foreach (var room in rooms)
		{
			var prog = new Db.FutureProg { Id = 900000 + room, FunctionName = "FixtureTableReset" + room,
				ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(), FunctionText =
				"var lastobj as item\nvar command as boolean\n" + writer.TryBuildTableReset(room, "destination900", 900, "command") + "\nreturn @command" };
			prog.FutureProgsParameters.Add(new Db.FutureProgsParameter { ParameterIndex = 0, ParameterName = "destination900", ParameterTypeDefinition = ProgVariableTypes.Location.ToStorageString() });
			db.FutureProgs.Add(prog);
		}
		db.SaveChanges();
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		foreach (var prog in db.FutureProgs.ToArray()) compiler.Compile(prog.Id);
		foreach (var record in records)
		{
			var proto = db.GameItemProtos.Single(x => x.Id == record.LogicalId);
			StringAssert.StartsWith(proto.UniqueName, "Vnum20610Room");
			Assert.AreEqual(2, proto.GameItemProtosGameItemComponentProtos.Count);
		}
		Assert.AreEqual(0, db.Hooks.Count(), "Furniture uses native speech volumes without custom conversation hooks.");
	}

	[TestMethod]
	public void ManaPendant_ClampsDonationBeforeAffordabilityAndNeverWastesFullCharges()
	{
		using var db = ApprovedItemFixture(out var writer);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallRubyPendant", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(writer, null);
		db.SaveChanges();
		decimal charge = 0;
		var (world, progs, _) = Runtime(db, registers =>
		{
			registers.Setup(x => x.GetType(ProgVariableTypes.Item, "armw_pendant_mana")).Returns(ProgVariableTypes.Number);
			registers.Setup(x => x.GetValue(It.IsAny<IProgVariable>(), "armw_pendant_mana")).Returns(() => new NumberVariable(charge));
			registers.Setup(x => x.SetValue(It.IsAny<IProgVariable>(), "armw_pendant_mana", It.IsAny<IProgVariable>()))
				.Callback<IProgVariable, string, IProgVariable>((_, _, value) => charge = (decimal)value.GetObject).Returns(true);
		});
		var actor = Actor(); var pendant = Item<IGameItem>(50001, ProgVariableTypes.Item);
		actor.Setup(x => x.GetProperty("wornitems")).Returns(new CollectionVariable(new[] { pendant.Object }, ProgVariableTypes.Item));
		actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		pendant.Setup(x => x.HasKeyword("pendant", actor.Object, true, false)).Returns(true);
		pendant.Setup(x => x.HasKeywords(It.IsAny<IEnumerable<string>>(), actor.Object, It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		pendant.Setup(x => x.GetKeywordsFor(actor.Object)).Returns(new[] { "pendant" });
		var mana = new Mock<IMagicResource>(); mana.SetupGet(x => x.Id).Returns(881);
		mana.Setup(x => x.ResourceCap(actor.Object)).Returns(100.0);
		world.SetupGet(x => x.MagicResources).Returns(new All<IMagicResource> { mana.Object });
		var balances = new Dictionary<IMagicResource, double> { [mana.Object] = 30.0 };
		actor.SetupGet(x => x.MagicResourceAmounts).Returns(balances);
		actor.Setup(x => x.AddResource(mana.Object, It.IsAny<double>())).Callback<IMagicResource, double>((resource, delta) => balances[resource] += delta);
		double stamina = 20;
		var body = new Mock<IBody>(); actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.HealthStrategy).Returns(Mock.Of<IHealthStrategy>());
		body.SetupGet(x => x.MaximumStamina).Returns(100.0); body.SetupGet(x => x.CurrentStamina).Returns(() => stamina);
		body.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns((double cost) => cost <= stamina);
		body.Setup(x => x.SpendStamina(It.IsAny<double>())).Callback<double>(cost => stamina -= cost);
		var prog = progs.GetByName("ArmageddonWorldfileRubyPendant");
		Assert.IsTrue(prog.ExecuteBool(actor.Object, pendant.Object, "donate", new List<string> { "100", "pendant" }));
		Assert.AreEqual(10.0, balances[mana.Object]); Assert.AreEqual(0.0, stamina); Assert.AreEqual(4m, charge);
		charge = 75; stamina = 20;
		Assert.IsTrue(prog.ExecuteBool(actor.Object, pendant.Object, "donate", new List<string> { "5", "pendant" }));
		Assert.AreEqual(10.0, balances[mana.Object]); Assert.AreEqual(20.0, stamina); Assert.AreEqual(75m, charge);
		balances[mana.Object] = 98;
		Assert.IsTrue(prog.ExecuteBool(actor.Object, pendant.Object, "gather", new List<string> { "10", "pendant" }));
		Assert.AreEqual(100.0, balances[mana.Object]); Assert.AreEqual(73m, charge);
	}

	[TestMethod]
	public void ApprovedItems_GeneratedProgsCompileAndGemsPreserveOnlyAttachedCallbacks()
	{
		using var db = ApprovedItemFixture(out var writer);
		foreach (var method in new[] { "InstallBindingItems", "InstallRubyPendant", "InstallItemGames", "InstallJailNeeds", "InstallBoards", "InstallLightning" })
			typeof(ArmageddonWorldfileWriter).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(writer, null);
		db.SaveChanges();
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray(), db.VariableDefinitions.ToArray());
		foreach (var prog in db.FutureProgs.ToArray()) compiler.Compile(prog.Id);
		foreach (var vnum in new[] { 50606, 50607 })
		{
			var proto = Field<Dictionary<int, Db.GameItemProto>>(writer, "_items")[vnum];
			var component = proto.GameItemProtosGameItemComponentProtos.Select(link => db.GameItemComponentProtos.Single(x => x.Id == link.GameItemComponentProtoId)).Single(x => x.Type == "ConditionalBinding");
			var policy = ConditionalMagicItemPolicy.Load(XElement.Parse(component.Definition));
			Assert.AreEqual(0, policy.Errors().Count);
			Assert.IsTrue(policy.PreventRemoval); Assert.IsTrue(policy.InterceptHostileSpells);
			Assert.AreEqual(0.0, policy.ActivationChance);
			Assert.AreEqual(0L, policy.IncompatibleWearerProgId);
			Assert.AreEqual(new ItemConsequence(), policy.Periodic);
			Assert.AreEqual(new ItemConsequence(), policy.IncompatibleWear);
			Assert.AreEqual(new ItemConsequence(15, 20, 30, 10, true), policy.Removal);
			Assert.AreEqual(new ItemConsequence(20, 40, 40, 10, true), policy.Activation);
			CollectionAssert.AreEqual(new long[] { 771 }, policy.InterceptSpellIds);
		}
		Assert.IsTrue(db.Hooks.Any(x => x.Name == "Worldfile ruby pendant gather"));
		Assert.IsTrue(db.Hooks.Any(x => x.Name == "Worldfile ruby pendant donate"));
		Assert.IsTrue(db.Hooks.Any(x => x.Name == "Worldfile dartboard"));
		Assert.IsTrue(db.Hooks.Any(x => x.Name == "Worldfile dice pair roll"));
		Assert.AreEqual(5, db.Hooks.Count(x => x.Name.StartsWith("Worldfile Jail Needs")));
		Assert.AreEqual(16, db.Boards.Count()); Assert.AreEqual(0, db.BoardPosts.Count());
		Assert.AreEqual(6, db.ProgSchedules.Single(x => x.Name == "Armageddon worldfile storm cloud pulse").IntervalModifier);
		Assert.AreEqual(60, db.ProgSchedules.Single(x => x.Name == "Armageddon worldfile storm weather update").IntervalModifier);
	}

	private static FuturemudDatabaseContext ApprovedItemFixture(out ArmageddonWorldfileWriter writer)
	{
		var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var ids = new[] { 950, 2817, 2818, 50056, 50099, 50102, 50600, 50601, 50604, 50606, 50607, 50609, 69010, 13602, 70224, 37980, 20610 }.ToHashSet();
		ids.UnionWith(archive.Items.Where(x => x.Programs.Any(p => p.EndsWith(" board", StringComparison.Ordinal))).Select(x => x.Vnum));
		ids.UnionWith(archive.Items.Where(x => x.ItemType == 24).Select(x => x.Vnum));
		var resets = archive.Resets.Where(x => x.ReferenceIssues.Length == 0 && x.Arguments.Length > 1 && ids.Contains(x.Arguments[1]) && x.Opcode is "O" or "G" or "E" or "P").ToArray();
		var source = new ArmageddonWorldfilePackage { Items = archive.Items.Where(x => ids.Contains(x.Vnum)).ToArray(), Resets = resets };
		writer = new ArmageddonWorldfileWriter(db, source);
		foreach (var item in source.Items)
		{
			var proto = new Db.GameItemProto { Id = 100000 + item.Vnum, Name = item.ShortDescription };
			db.GameItemProtos.Add(proto); Field<Dictionary<int, Db.GameItemProto>>(writer, "_items").Add(item.Vnum, proto);
			Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems").Add(proto.Id, (item, item.Values));
		}
		var clan = new Db.Clan { Id = 9012, Name = "Allanak militia" }; db.Clans.Add(clan);
		Field<Dictionary<int, Db.Clan>>(writer, "_clans").Add(12, clan);
		var zone = new Db.Zone { Id = 9050, Name = "Allanak" }; db.Zones.Add(zone);
		Field<Dictionary<int, Db.Zone>>(writer, "_zones").Add(50, zone);
		foreach (var vnum in resets.Where(x => x.Opcode == "O").Select(x => x.Arguments[3]).Append(48000).Distinct())
		{
			var room = new Db.Room { Id = 200000 + vnum, UniqueName = "Vnum" + vnum }; db.Rooms.Add(room);
			Field<Dictionary<int, Db.Room>>(writer, "_rooms").Add(vnum, room);
		}
		void Owned(string key, string type, long id) => db.SeederManagedRecords.Add(new Db.SeederManagedRecord
			{ Seeder = "Fixture", StableKey = key, EntityType = type, LogicalId = id });
		Owned("arm.class.southern_templar", nameof(Db.ChargenRole), 880);
		Owned(ArmageddonProgressionInstaller.ManaResourceKey, nameof(Db.MagicResource), 881);
		Owned("arm.skill.throw", nameof(Db.TraitDefinition), 882);
		db.TraitDefinitions.Add(new Db.TraitDefinition { Id = 883, Type = 1, Name = "Agility" });
		var cap = 890;
		foreach (var name in new[] { "earth", "fire", "water", "wind", "shadow", "lightning", "void", "defiler" })
			Owned("arm.capability." + name, nameof(Db.MagicCapability), cap++);
		Owned("arm.templar.southern.gem.component", nameof(Db.GameItemComponentProto), 900);
		db.GameItemComponentProtos.Add(new Db.GameItemComponentProto { Id = 900, Type = "ConditionalBinding", Definition =
			new ConditionalMagicItemPolicy { Kind = ConditionalMagicItemKind.ConditionalBinding, EligibilityProgId = 1, ControllerProgId = 1,
				InterceptSpellIds = [771], Periodic = new(10, 25, 25, 10, true), IncompatibleWearerProgId = 2,
				IncompatibleWear = new(HealthFraction: 1, DestroyItem: true) }.Save().ToString() });
		db.Clocks.Add(new Db.Clock { Id = 20, PrimaryTimezoneId = 20, Definition = "<Clock><InGameSecondsPerRealSecond>12</InGameSecondsPerRealSecond><SecondsPerMinute>60</SecondsPerMinute></Clock>" });
		db.Timezones.Add(new Db.Timezone { Id = 20, ClockId = 20, Name = "AST" });
		db.Calendars.Single().FeedClockId = 20; db.Calendars.Single().Date = "1/1/1";
		db.SaveChanges(); return db;
	}
}

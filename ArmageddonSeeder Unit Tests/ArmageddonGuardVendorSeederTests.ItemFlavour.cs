#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.TimeAndDate.Intervals;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonGuardVendorSeederTests
{
	[TestMethod]
	[DataRow(6.0, 3)]
	[DataRow(1.0, 1)]
	public void Vines_ExecuteAttachedGroundItemFlavourAndPreserveNativeScheduleMapping(double clockRate, int minutes)
	{
		using var db = VineFixture(out var writer, clockRate);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallItemFlavour", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		var schedule = db.ProgSchedules.Single();
		Assert.AreEqual((int)IntervalType.Minutely, schedule.IntervalType);
		Assert.AreEqual(minutes, schedule.IntervalModifier);
		var (world, progs, _) = Runtime(db);
		var room = Item<IRoom>(70520, ProgVariableTypes.Location);
		var output = new Mock<IOutputHandler>(); var observer = Actor(); observer.SetupGet(x => x.OutputHandler).Returns(output.Object);
		room.SetupGet(x => x.Characters).Returns(new[] { observer.Object }); room.SetupGet(x => x.Gameworld).Returns(world.Object);
		var items = new All<IGameItem>();
		var groundItems = new List<IGameItem>();
		room.Setup(x => x.GetProperty("items")).Returns(() => new CollectionVariable(groundItems, ProgVariableTypes.Item));
		foreach (var (id, proto, ground) in new[] { (1L, 70220, true), (2L, 170220, true), (3L, 70220, false), (4L, 99999, true) })
		{
			var item = Item<IGameItem>(id, ProgVariableTypes.Item);
			item.Setup(x => x.GetProperty("proto")).Returns(new NumberVariable(proto));
			item.Setup(x => x.GetProperty("location")).Returns(room.Object);
			if (ground) groundItems.Add(item.Object);
			items.Add(item.Object);
		}
		world.SetupGet(x => x.Items).Returns(items);
		var pulse = progs.Get(schedule.FutureProgId);
		using (Constants.PushRandom(new VineRolls(1, 2))) Assert.IsTrue(pulse.ExecuteBool());
		Assert.AreEqual(2, output.Invocations.Count(x => x.Method.Name == "Send"));
		Assert.IsTrue(output.Invocations.Any(x => x.Arguments.OfType<EmoteOutput>().Any(a => a.DefaultEmote.RawText == "Something rustles against the ground.")));
		Assert.IsTrue(output.Invocations.Any(x => x.Arguments.OfType<EmoteOutput>().Any(a => a.DefaultEmote.RawText == "You notice movement out of the corner of your eye.")));
		output.Invocations.Clear();
		using (Constants.PushRandom(new VineRolls(3, 25))) Assert.IsTrue(pulse.ExecuteBool());
		Assert.AreEqual(0, output.Invocations.Count(x => x.Method.Name == "Send"));
	}

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void Vines_UnplacedDefinitionOrMissingAttachment_DoesNotInventAPulse(bool removeAttachment)
	{
		using var db = VineFixture(out var writer, 6, resetPlaced: removeAttachment);
		var source = Field<ArmageddonWorldfilePackage>(writer, "_source");
		if (removeAttachment) Array.Fill(source.Items, source.Items[0] with { Programs = [] });
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallItemFlavour", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		Assert.AreEqual(0, db.ProgSchedules.Local.Count);
		Assert.AreEqual(0, db.FutureProgs.Local.Count(x => x.FunctionName == "ArmageddonWorldfileVinesPulse"));
	}

	private static FuturemudDatabaseContext VineFixture(out ArmageddonWorldfileWriter writer, double rate, bool resetPlaced = true)
	{
		var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var plant = archive.Items.Single(x => x.Vnum == 70220);
		var source = new ArmageddonWorldfilePackage { Items = [plant], Resets = resetPlaced ? archive.Resets.Where(x => x.Opcode == "O" && x.Arguments[1] == 70220).ToArray() : [] };
		Assert.IsTrue(!resetPlaced || source.Resets.Length > 0 && source.Resets.All(x => x.ReferenceIssues.Length == 0));
		writer = new ArmageddonWorldfileWriter(db, source);
		var prototype = new Db.GameItemProto { Id = 70220, Name = "tubular vines" };
		var variant = new Db.GameItemProto { Id = 170220, Name = "tubular vines reset variant" };
		db.GameItemProtos.AddRange(prototype, variant);
		Field<Dictionary<int, Db.GameItemProto>>(writer, "_items").Add(70220, prototype);
		Field<Dictionary<string, Db.GameItemProto>>(writer, "_resetItemVariants").Add("70220:fixture", variant);
		var culture = System.Globalization.CultureInfo.InvariantCulture;
		db.Clocks.Add(new Db.Clock { Id = 20, PrimaryTimezoneId = 20, Definition = $"<Clock><InGameSecondsPerRealSecond>{rate.ToString(culture)}</InGameSecondsPerRealSecond><SecondsPerMinute>60</SecondsPerMinute></Clock>" });
		db.Timezones.Add(new Db.Timezone { Id = 20, ClockId = 20, Name = "AST" });
		db.Calendars.Single().FeedClockId = 20; db.Calendars.Single().Date = "1/1/1";
		db.SaveChanges(); return db;
	}

	private sealed class VineRolls(params int[] values) : Random
	{
		private readonly Queue<int> _values = new(values);
		public override int Next(int minValue, int maxValue)
		{
			var value = _values.Dequeue(); Assert.IsTrue(value >= minValue && value < maxValue); return value;
		}
	}
}

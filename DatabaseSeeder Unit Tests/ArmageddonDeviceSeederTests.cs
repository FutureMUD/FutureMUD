#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.GameItems;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonDeviceSeederTests
{
	[TestMethod]
	public void PlacedDevices_SeedFourReservoirsWithSourceCostsAndLeaveSixInert()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var package = ArmageddonWorldfileTestData.Load().ForInstallation();
		var placed = package.Resets.Where(x => x.Opcode is "O" or "G" or "E" or "P")
			.Where(x => x.Arguments.Length > 1).Select(x => x.Arguments[1]).ToHashSet();
		var devices = package.Items.Where(x => x.ItemType is 3 or 4 && placed.Contains(x.Vnum)).ToArray();
		Assert.AreEqual(10, devices.Length);
		var writer = new ArmageddonWorldfileWriter(db, new ArmageddonWorldfilePackage { Items = devices, Resets = package.Resets });
		var keys = new[] { "arm.spell.ember_lance", "arm.spell.cinder_rain", "arm.spell.raise_servitor", "arm.spell.banish_traveller" };
		for (var i = 0; i < keys.Length; i++)
			db.SeederManagedRecords.Add(new Db.SeederManagedRecord { Seeder = "Fixture", StableKey = keys[i],
				EntityType = nameof(Db.MagicSpell), LogicalId = 980001 + i });
		db.SeederManagedRecords.Add(new Db.SeederManagedRecord { Seeder = "Fixture", StableKey = ArmageddonProgressionInstaller.ManaResourceKey,
			EntityType = nameof(Db.MagicResource), LogicalId = 980010 });
		var effective = (Dictionary<long, (ArmWorldItem Source, int[] Values)>)typeof(ArmageddonWorldfileWriter)
			.GetField("_effectiveItems", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(writer)!;
		foreach (var device in devices)
		{
			var proto = new Db.GameItemProto { Id = 990000 + device.Vnum, RevisionNumber = 0, Name = device.Name };
			db.GameItemProtos.Add(proto);
			effective.Add(proto.Id, (device, device.Values));
			db.GameItems.Add(new Db.GameItem { Id = proto.Id + 100000, GameItemProtoId = proto.Id, GameItemProtoRevision = 0 });
		}
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallPlacedMagicDevices", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		var expected = new Dictionary<int, int[]>
		{
			[25109] = [50, 60, 70, 80, 100, 120, 160], [65509] = [50, 60, 70, 80, 100, 120, 160],
			[25606] = [200, 200, 200, 200, 200, 200, 250], [25914] = [80, 100, 120, 160, 250]
		};
		Assert.AreEqual(4, db.GameItemComponents.Local.Count);
		foreach (var device in devices)
		{
			var instance = db.GameItemComponents.Local.SingleOrDefault(x => x.GameItemId == 1090000 + device.Vnum);
			if (!expected.TryGetValue(device.Vnum, out var costs)) { Assert.IsNull(instance); continue; }
			Assert.IsNotNull(instance);
			var component = db.GameItemComponentProtos.Local.Single(x => x.Id == instance.GameItemComponentProtoId);
			var root = XElement.Parse(component.Definition);
			var reservoir = root.Element("Reservoir")!;
			Assert.AreEqual((int)MagicDeviceEligibility.Anyone, (int)root.Element("Eligibility")!);
			Assert.AreEqual((int)MagicDeviceMode.EnergyReservoir, (int)reservoir.Attribute("mode")!);
			Assert.AreEqual(device.Values[0], (int)reservoir.Attribute("maximum")!);
			Assert.AreEqual(device.Values[2], (int)reservoir.Attribute("initial")!);
			Assert.IsTrue((bool)reservoir.Attribute("worn")!);
			Assert.AreEqual(device.Vnum == 25109, (bool)reservoir.Attribute("openSky")!);
			CollectionAssert.AreEqual(costs, reservoir.Elements("GradeCost").Select(x => (int)x.Attribute("cost")!).ToArray());
			Assert.IsTrue(reservoir.Elements("GradeCost").All(x => (int)x.Attribute("grade")! <= 7));
			Assert.IsNotNull(XElement.Parse(instance.Definition).Attribute("checksum"));
		}
	}
}

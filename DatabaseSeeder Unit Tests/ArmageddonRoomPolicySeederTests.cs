#nullable enable

using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoomRestrictions = MudSharp.Construction.RoomRestrictions;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonRoomPolicySeederTests
{
	[TestMethod]
	public void RoomImport_PreservesPolicyAndObservationDirectionAndCreatesNativeVerticalFall()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		db.Zones.Add(new Zone { Id = 1, Name = "Baseline", ShardId = 1 });
		db.Terrains.AddRange(new Terrain { Id = 1, Name = "Hallway" },
			new Terrain { Id = 2, Name = "Gas Giant Atmosphere" }, new Terrain { Id = 3, Name = "Sandy Desert" });
		db.SaveChanges();
		var location = new ArmWorldSource("fixture.wld", 1);
		var source = new ArmageddonWorldfilePackage
		{
			Zones = [new ArmWorldZone(location, 0, 0, "Imported zone", [], [])],
			Rooms =
			[
				new(location, 0, "Base", 10, 0, 1 << 7, 0,
					[new ArmWorldExit(4, 11, "", 0, -1, "")], [], [], 0, ["C south storms_edge"], "Base."),
				new(location, 0, "Air", 11, 0, (1 << 11) | (1 << 14) | (1 << 16), 0,
					[new ArmWorldExit(5, 10, "", 0, -1, "")], [], [], 0, [], "Air."),
				new(location, 0, "Observer", 12, 0, 1 << 8, 0, [], [new("wall", "An old wall.")],
					[JsonSerializer.SerializeToElement(new { Type = "w", Room = 11, Text = "Above:" })], 0, [], "Observer.")
			]
		};
		var writer = new ArmageddonWorldfileWriter(db, source);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallRooms", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.ChangeTracker.Clear();
		var lower = db.Rooms.Single(x => x.UniqueName == "Vnum10");
		var upper = db.Rooms.Single(x => x.UniqueName == "Vnum11");
		var observer = db.Rooms.Single(x => x.UniqueName == "Vnum12");
		var passage = db.Exits.Single();
		Assert.AreEqual(lower.Id, passage.FallRoom);
		Assert.IsFalse(passage.IsClimbExit, "The no-climb upper room requires flight rather than offering a climb.");
		var links = XElement.Parse(upper.EffectData).Elements("Effect").Where(x => (string?)x.Element("Type") == "RoomObservation").ToArray();
		Assert.AreEqual(1, links.Length);
		Assert.AreEqual(observer.Id, (long)links[0].Element("Effect")!.Element("ObserverRoomId")!);
		Assert.AreEqual("Above:", (string?)links[0].Element("Effect")!.Element("Prefix"));
		Assert.IsFalse(observer.EffectData.Contains("RoomObservation"));
		Assert.IsTrue(observer.EffectData.Contains("KeywordDescriptions"));
		Assert.AreEqual(1, (int)XElement.Parse(observer.EffectData).Elements("Effect")
			.Single(x => (string?)x.Element("Type") == "RoomPolicy").Element("Effect")!.Element("MaximumOccupants")!);
		Assert.IsTrue(lower.EffectData.Contains("RoomExitPolicy"));
	}

	[TestMethod]
	public void SelectedCorpus_QualifiesAllVisualLinksAndExplicitLethalAndMagicFlags()
	{
		var source = ArmageddonWorldfileTestData.Load().ForInstallation();
		var links = source.Rooms.SelectMany(x => x.ObservationLinks).ToArray();
		Assert.AreEqual(237, links.Length);
		Assert.IsTrue(links.All(x => x.GetProperty("Type").GetString() == "w"));
		var rooms = source.Rooms.Select(x => x.Vnum).ToHashSet();
		Assert.IsTrue(links.All(x => rooms.Contains(x.GetProperty("Room").GetInt32())));
		Assert.AreEqual(17, source.Rooms.Count(x => (ArmageddonWorldfileWriter.RestrictionsFor(x.Flags) & RoomRestrictions.Lethal) != 0));
		Assert.AreEqual(290, source.Rooms.Count(x => (ArmageddonWorldfileWriter.RestrictionsFor(x.Flags) & RoomRestrictions.Magic) != 0));
		Assert.AreEqual(1264, source.Rooms.Count(x => (ArmageddonWorldfileWriter.RestrictionsFor(x.Flags) & RoomRestrictions.NpcWandering) != 0));
	}
}

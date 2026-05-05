#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Construction;
using MudSharp.GameItems;
using RPI_Engine_Worldfile_Converter;

namespace RPI_Engine_Worldfile_Converter_Tests;

[TestClass]
public class RpiRoomConversionTests
{
	[TestMethod]
	public void RoomParser_ReadsFixtureCorpus_AndCapturesSidecarData()
	{
		var parser = new RpiRoomWorldfileParser();
		var corpus = parser.ParseDirectory(GetRoomFixtureDirectory());

		Assert.AreEqual(13, corpus.Rooms.Count);
		Assert.AreEqual(0, corpus.Failures.Count);

		var hiddenRoom = corpus.Rooms.Single(x => x.Vnum == 1002);
		var hiddenExit = hiddenRoom.Exits.Single(x => x.Direction == RpiRoomDirection.North);
		Assert.AreEqual(RpiRoomExitSectionType.Hidden, hiddenExit.SectionType);
		Assert.AreEqual(RpiRoomDoorType.Door, hiddenExit.DoorType);
		Assert.AreEqual(1, hiddenRoom.Secrets.Count);
		Assert.AreEqual(45, hiddenRoom.Secrets[0].Difficulty);

		var trappedRoom = corpus.Rooms.Single(x => x.Vnum == 1003);
		Assert.IsTrue(trappedRoom.Exits.Single(x => x.Direction == RpiRoomDirection.East).IsTrapped);

		var xeroxRoom = corpus.Rooms.Single(x => x.Vnum == 1006);
		Assert.AreEqual(1007, xeroxRoom.XeroxSourceVnum);

		var sidecarRoom = corpus.Rooms.Single(x => x.Vnum == 1007);
		Assert.AreEqual(1, sidecarRoom.ExtraDescriptions.Count);
		Assert.AreEqual(1, sidecarRoom.WrittenDescriptions.Count);
		Assert.AreEqual(1, sidecarRoom.RoomProgs.Count);
		Assert.AreEqual(12, sidecarRoom.Weather!.WeatherDescriptions.Count);
		Assert.AreEqual(6, sidecarRoom.Weather.AlasDescriptions.Count);
		Assert.AreEqual(12, sidecarRoom.Capacity);
		Assert.IsTrue(sidecarRoom.RoomFlags.HasFlag(RpiRoomFlags.SafeQuit));
		Assert.IsTrue(sidecarRoom.RoomFlags.HasFlag(RpiRoomFlags.Indoors));
	}

	[TestMethod]
	public void RoomTransformer_MapsFixtureCorpus_ToExpectedZonesTerrainsAndExitSemantics()
	{
		var parser = new RpiRoomWorldfileParser();
		var corpus = parser.ParseDirectory(GetRoomFixtureDirectory());
		var transformer = new FutureMudRoomTransformer();
		var conversion = transformer.Convert(corpus.Rooms);
		var rooms = conversion.Rooms.ToDictionary(x => x.Vnum);

		Assert.AreEqual(2, conversion.Zones.Count);

		var minasMorgul = conversion.Zones.Single(x => x.GroupKey == "minas-morgul");
		CollectionAssert.AreEquivalent(new[] { 5, 6 }, minasMorgul.SourceZones.ToArray());
		Assert.AreEqual("Minas Morgul", minasMorgul.ZoneName);

		Assert.AreEqual("Cobblestone Road", rooms[1001].TerrainName);
		Assert.AreEqual("Temperate Coniferous Forest", rooms[1003].TerrainName);
		Assert.AreEqual("Swamp Forest", rooms[1004].TerrainName);

		Assert.IsTrue(rooms[1006].XeroxResolved);
		Assert.AreEqual(rooms[1007].RawDescription, rooms[1006].EffectiveDescription);
		Assert.IsNotNull(rooms[1006].EffectiveWeather);

		Assert.AreEqual((int)CellOutdoorsType.IndoorsClimateExposed, rooms[1007].OutdoorsTypeValue);
		Assert.IsTrue(rooms[1007].SafeQuit);

		var hiddenExit = conversion.Exits.Single(x => x.RoomVnum1 == 1002 && x.RoomVnum2 == 1004);
		Assert.IsTrue(hiddenExit.AcceptsDoor);
		Assert.IsTrue(hiddenExit.Side1.Hidden);
		Assert.IsFalse(hiddenExit.Side2.Hidden);
		Assert.AreEqual(45, hiddenExit.Side1.SearchDifficulty);
		Assert.AreEqual((int)SizeCategory.Large, hiddenExit.DoorSize);

		var trapdoorExit = conversion.Exits.Single(x => x.RoomVnum1 == 1003 && x.RoomVnum2 == 1008);
		Assert.IsTrue(trapdoorExit.Side1.Trapped);
		Assert.AreEqual((int)SizeCategory.Small, trapdoorExit.DoorSize);

		var doubleDoorExit = conversion.Exits.Single(x => x.RoomVnum1 == 1008 && x.RoomVnum2 == 1009);
		Assert.AreEqual((int)SizeCategory.VeryLarge, doubleDoorExit.DoorSize);

		var gateExit = conversion.Exits.Single(x => x.RoomVnum1 == 1009 && x.RoomVnum2 == 1010);
		Assert.AreEqual((int)SizeCategory.Enormous, gateExit.DoorSize);

		var verticalExit = conversion.Exits.Single(x => x.RoomVnum1 == 1004 && x.RoomVnum2 == 1005);
		Assert.IsTrue(verticalExit.IsClimbExit);
		Assert.AreEqual(1005, verticalExit.FallFromRoomVnum);
		Assert.AreEqual(1004, verticalExit.FallToRoomVnum);
	}

	[TestMethod]
	public void RoomTransformer_AssignsDeterministicCoordinates_AndRecordsLayoutConflicts()
	{
		var parser = new RpiRoomWorldfileParser();
		var corpus = parser.ParseDirectory(GetRoomFixtureDirectory());
		var transformer = new FutureMudRoomTransformer();
		var conversion = transformer.Convert(corpus.Rooms);
		var rooms = conversion.Rooms.ToDictionary(x => x.Vnum);

		Assert.AreEqual(new RoomCoordinate(0, 0, 0), rooms[1000].Coordinates);
		Assert.AreEqual(new RoomCoordinate(0, 1, 0), rooms[1001].Coordinates);
		Assert.AreEqual(new RoomCoordinate(1, 0, 0), rooms[1002].Coordinates);
		Assert.AreEqual(new RoomCoordinate(1, 1, 0), rooms[1003].Coordinates);
		Assert.AreEqual(new RoomCoordinate(1, 2, 0), rooms[1004].Coordinates);
		Assert.AreEqual(new RoomCoordinate(1, 2, 1), rooms[1005].Coordinates);

		Assert.IsTrue(rooms[1004].Warnings.Any(x => x.Code == "layout-conflict"));
	}

	[TestMethod]
	public void RpiRoomDirections_MapLegacyDirectionValues_ToFutureMudCardinalValues()
	{
		Assert.AreEqual(CardinalDirection.North, RpiRoomDirection.North.ToFutureMudDirection());
		Assert.AreEqual(CardinalDirection.East, RpiRoomDirection.East.ToFutureMudDirection());
		Assert.AreEqual(CardinalDirection.South, RpiRoomDirection.South.ToFutureMudDirection());
		Assert.AreEqual(CardinalDirection.West, RpiRoomDirection.West.ToFutureMudDirection());
		Assert.AreEqual(CardinalDirection.Up, RpiRoomDirection.Up.ToFutureMudDirection());
		Assert.AreEqual(CardinalDirection.Down, RpiRoomDirection.Down.ToFutureMudDirection());
		Assert.AreEqual(CardinalDirection.NorthEast, RpiRoomDirection.NorthEast.ToFutureMudDirection());
		Assert.AreEqual(CardinalDirection.NorthWest, RpiRoomDirection.NorthWest.ToFutureMudDirection());
		Assert.AreEqual(CardinalDirection.SouthEast, RpiRoomDirection.SouthEast.ToFutureMudDirection());
		Assert.AreEqual(CardinalDirection.SouthWest, RpiRoomDirection.SouthWest.ToFutureMudDirection());
		Assert.AreEqual(CardinalDirection.Unknown, RpiRoomDirection.Outside.ToFutureMudDirection());
		Assert.AreEqual(CardinalDirection.Unknown, RpiRoomDirection.Inside.ToFutureMudDirection());

		Assert.IsTrue(RpiRoomDirections.TryParse("10", out var southEast));
		Assert.AreEqual(RpiRoomDirection.SouthEast, southEast);
	}

	[TestMethod]
	public void RoomTransformer_DoesNotShrinkNormalDoorBecauseRoomDescriptionMentionsTrapdoor()
	{
		var tavern = new RpiRoomRecord
		{
			Vnum = 99010,
			SourceFile = "rooms.99",
			Zone = 99,
			Name = "The Strange Brew Tavern",
			Description = "A broad tavern with a normal wooden door to the north.",
			RawFlags = 0,
			RoomFlags = RpiRoomFlags.None,
			RawSectorType = (int)RpiRoomSectorType.Inside,
			SectorType = RpiRoomSectorType.Inside,
			Deity = 0,
			Exits =
			[
				new RpiRoomExitRecord(
					RpiRoomDirection.North,
					RpiRoomExitSectionType.Normal,
					string.Empty,
					"door",
					RpiRoomDoorType.Door,
					-1,
					0,
					99011)
			],
		};
		var kitchen = new RpiRoomRecord
		{
			Vnum = 99011,
			SourceFile = "rooms.99",
			Zone = 99,
			Name = "A Cluttered Brewery and Kitchen",
			Description = "A carpet in one corner covers a trapdoor, while a normal door leads south.",
			RawFlags = 0,
			RoomFlags = RpiRoomFlags.None,
			RawSectorType = (int)RpiRoomSectorType.Inside,
			SectorType = RpiRoomSectorType.Inside,
			Deity = 0,
			Exits =
			[
				new RpiRoomExitRecord(
					RpiRoomDirection.South,
					RpiRoomExitSectionType.Normal,
					string.Empty,
					"door",
					RpiRoomDoorType.Door,
					-1,
					0,
					99010)
			],
		};

		var transformer = new FutureMudRoomTransformer();
		var convertedExit = transformer.Convert([tavern, kitchen]).Exits.Single();

		Assert.AreEqual((int)SizeCategory.Large, convertedExit.DoorSize);
		Assert.AreEqual((int)SizeCategory.Large, convertedExit.MaximumSizeToEnter);
		Assert.AreEqual((int)SizeCategory.Large, convertedExit.MaximumSizeToEnterUpright);
	}

	[TestMethod]
	public void RoomIdPlanner_PreservesPositiveLegacyVnums_ForRoomAndCellIds()
	{
		var rooms = new[]
		{
			CreateConvertedRoom(66896),
			CreateConvertedRoom(66897),
		};

		var plan = FutureMudRoomIdPlanner.Plan(rooms, new HashSet<long>(), new HashSet<long>());

		Assert.AreEqual(0, plan.Issues.Count);
		Assert.AreEqual(66896L, plan.Reservations[66896].RoomId);
		Assert.AreEqual(66896L, plan.Reservations[66896].CellId);
		Assert.AreEqual(66897L, plan.Reservations[66897].RoomId);
		Assert.AreEqual(66897L, plan.Reservations[66897].CellId);
	}

	[TestMethod]
	public void RoomIdPlanner_AssignsHighFallbackIds_ForZeroVnumsAndCollisions()
	{
		var rooms = new[]
		{
			CreateConvertedRoom(0),
			CreateConvertedRoom(1000),
			CreateConvertedRoom(1001),
		};

		var plan = FutureMudRoomIdPlanner.Plan(
			rooms,
			new HashSet<long> { 1000 },
			new HashSet<long> { 1001 });

		Assert.AreEqual(1002L, plan.Reservations[0].RoomId);
		Assert.AreEqual(1002L, plan.Reservations[0].CellId);
		Assert.AreEqual(1003L, plan.Reservations[1000].RoomId);
		Assert.AreEqual(1000L, plan.Reservations[1000].CellId);
		Assert.AreEqual(1001L, plan.Reservations[1001].RoomId);
		Assert.AreEqual(1003L, plan.Reservations[1001].CellId);
		Assert.IsTrue(plan.Issues.Any(x => x.SourceKey == "rooms.0#0" && x.Message.Contains("cannot be used")));
		Assert.IsTrue(plan.Issues.Any(x => x.SourceKey == "rooms.1000#1000" && x.Message.Contains("already exists")));
		Assert.IsTrue(plan.Issues.Any(x => x.SourceKey == "rooms.1001#1001" && x.Message.Contains("already exists")));
	}

	[TestMethod]
	public void RoomTransformer_WarnsForLongDescriptions_AndImporterTruncatesForPersistence()
	{
		var longDescription = new string('a', FutureMudRoomImportLimits.CellDescriptionMaxLength + 1);
		var room = new RpiRoomRecord
		{
			Vnum = 99000,
			SourceFile = "rooms.99",
			Zone = 99,
			Name = "Long Description Test",
			Description = longDescription,
			RawFlags = 0,
			RoomFlags = RpiRoomFlags.None,
			RawSectorType = (int)RpiRoomSectorType.Inside,
			SectorType = RpiRoomSectorType.Inside,
			Deity = 0,
		};

		var transformer = new FutureMudRoomTransformer();
		var converted = transformer.Convert([room]).Rooms.Single();

		Assert.AreEqual(longDescription.Length, converted.EffectiveDescription.Length);
		Assert.IsTrue(converted.Warnings.Any(x => x.Code == "cell-description-truncated"));

		var truncated = FutureMudRoomImportLimits.TruncateCellDescription(converted.EffectiveDescription);

		Assert.AreEqual(FutureMudRoomImportLimits.CellDescriptionMaxLength, truncated.Length);
		Assert.AreEqual(longDescription[..FutureMudRoomImportLimits.CellDescriptionMaxLength], truncated);
	}

	[TestMethod]
	public void RoomTransformer_WarnsForLongExitText_AndImporterTruncatesEveryExitTextField()
	{
		var longExitDescription = new string('d', FutureMudRoomImportLimits.ExitTextMaxLength + 1);
		var longKeyword = new string('k', FutureMudRoomImportLimits.ExitTextMaxLength + 1);
		var sourceRoom = new RpiRoomRecord
		{
			Vnum = 99000,
			SourceFile = "rooms.99",
			Zone = 99,
			Name = "Long Exit Source",
			Description = "A room with a suspiciously long exit.",
			RawFlags = 0,
			RoomFlags = RpiRoomFlags.None,
			RawSectorType = (int)RpiRoomSectorType.Inside,
			SectorType = RpiRoomSectorType.Inside,
			Deity = 0,
			Exits =
			[
				new RpiRoomExitRecord(
					RpiRoomDirection.North,
					RpiRoomExitSectionType.Normal,
					longExitDescription,
					longKeyword,
					RpiRoomDoorType.None,
					-1,
					0,
					99001)
			],
		};
		var destinationRoom = new RpiRoomRecord
		{
			Vnum = 99001,
			SourceFile = "rooms.99",
			Zone = 99,
			Name = "Long Exit Destination",
			Description = "A destination room.",
			RawFlags = 0,
			RoomFlags = RpiRoomFlags.None,
			RawSectorType = (int)RpiRoomSectorType.Inside,
			SectorType = RpiRoomSectorType.Inside,
			Deity = 0,
		};

		var transformer = new FutureMudRoomTransformer();
		var convertedExit = transformer.Convert([sourceRoom, destinationRoom]).Exits.Single();

		Assert.AreEqual(longExitDescription.Length, convertedExit.Side1.Description.Length);
		Assert.AreEqual(longKeyword.Length, convertedExit.Side1.Keywords.Length);
		Assert.AreEqual(longKeyword.Length, convertedExit.Side1.PrimaryKeyword!.Length);
		Assert.IsTrue(convertedExit.Warnings.Any(x => x.Code == "exit-description-truncated"));
		Assert.IsTrue(convertedExit.Warnings.Any(x => x.Code == "exit-keywords-truncated"));
		Assert.IsTrue(convertedExit.Warnings.Any(x => x.Code == "exit-primary-keyword-truncated"));

		var truncatedDescription = FutureMudRoomImportLimits.TruncateExitText(convertedExit.Side1.Description);
		var truncatedKeywords = FutureMudRoomImportLimits.TruncateExitText(convertedExit.Side1.Keywords);
		var truncatedPrimaryKeyword = FutureMudRoomImportLimits.TruncateExitText(convertedExit.Side1.PrimaryKeyword);

		Assert.AreEqual(FutureMudRoomImportLimits.ExitTextMaxLength, truncatedDescription.Length);
		Assert.AreEqual(FutureMudRoomImportLimits.ExitTextMaxLength, truncatedKeywords.Length);
		Assert.AreEqual(FutureMudRoomImportLimits.ExitTextMaxLength, truncatedPrimaryKeyword.Length);
		Assert.AreEqual(longExitDescription[..FutureMudRoomImportLimits.ExitTextMaxLength], truncatedDescription);
		Assert.AreEqual(longKeyword[..FutureMudRoomImportLimits.ExitTextMaxLength], truncatedKeywords);
		Assert.AreEqual(longKeyword[..FutureMudRoomImportLimits.ExitTextMaxLength], truncatedPrimaryKeyword);
	}

	[TestMethod]
	public void RoomTransformer_WarnsForSelfLoopExits()
	{
		var room = new RpiRoomRecord
		{
			Vnum = 99000,
			SourceFile = "rooms.99",
			Zone = 99,
			Name = "Self Loop Room",
			Description = "A room whose exits point back into itself.",
			RawFlags = 0,
			RoomFlags = RpiRoomFlags.None,
			RawSectorType = (int)RpiRoomSectorType.Inside,
			SectorType = RpiRoomSectorType.Inside,
			Deity = 0,
			Exits =
			[
				new RpiRoomExitRecord(
					RpiRoomDirection.North,
					RpiRoomExitSectionType.Normal,
					"around",
					"loop",
					RpiRoomDoorType.None,
					-1,
					0,
					99000),
				new RpiRoomExitRecord(
					RpiRoomDirection.South,
					RpiRoomExitSectionType.Normal,
					"around",
					"loop",
					RpiRoomDoorType.None,
					-1,
					0,
					99000)
			],
		};

		var transformer = new FutureMudRoomTransformer();
		var convertedExit = transformer.Convert([room]).Exits.Single();

		Assert.AreEqual(99000, convertedExit.RoomVnum1);
		Assert.AreEqual(99000, convertedExit.RoomVnum2);
		Assert.IsTrue(convertedExit.Side2.Visible);
		Assert.IsTrue(convertedExit.Warnings.Any(x => x.Code == "self-loop-exit"));
	}

	private static ConvertedRoomDefinition CreateConvertedRoom(int vnum)
	{
		return new ConvertedRoomDefinition
		{
			Vnum = vnum,
			SourceFile = $"rooms.{vnum}",
			SourceZone = vnum / 1000,
			SourceKey = $"rooms.{vnum}#{vnum}",
			ZoneGroupKey = "test-zone",
			ZoneName = "Test Zone",
			OverlayPackageName = "RPI Import Rooms - Test Zone (#1r0)",
			Name = $"Room {vnum}",
			RawDescription = "A test room.",
			EffectiveDescription = "A test room.",
			TerrainName = "Hall",
			OutdoorsTypeName = "Indoors",
			OutdoorsTypeValue = (int)CellOutdoorsType.Indoors,
			SafeQuit = false,
			Coordinates = new RoomCoordinate(vnum, 0, 0),
			RawFlags = 0,
			RoomFlagNames = Array.Empty<string>(),
			SectorType = RpiRoomSectorType.Inside,
			Deity = 0,
		};
	}

	private static string GetRoomFixtureDirectory()
	{
		var candidates = new[]
		{
			Path.Combine(AppContext.BaseDirectory, "Fixtures", "Rooms"),
			Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Fixtures", "Rooms")),
			Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "RPI Engine Worldfile Converter Tests", "Fixtures", "Rooms"))
		};

		return candidates.First(x => File.Exists(Path.Combine(x, "rooms.1")));
	}
}

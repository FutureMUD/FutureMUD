using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Construction;
using MudSharp.Construction.ImportExport;

namespace MudSharpCore_Unit_Tests.Construction.ImportExport;

[TestClass]
public class SpatialAreaPackageSerializerTests
{
	[TestMethod]
	public void SerializeDeserialize_ValidPackage_RoundTripsWithIntegrity()
	{
		var package = CreateValidPackage();

		var json = SpatialAreaPackageSerializer.Serialize(package);
		var result = SpatialAreaPackageSerializer.Deserialize(json);

		Assert.IsTrue(result.Success);
		Assert.IsNotNull(result.Package);
		Assert.AreEqual("Test Zone", result.Package.Zone.Name);
		Assert.AreEqual(2, result.Package.Cells.Count);
		Assert.AreEqual(1, result.Package.Exits.Count);
		Assert.AreEqual(64, result.Package.IntegritySha256.Length);
	}

	[TestMethod]
	public void Deserialize_TamperedPayload_RejectsIntegrity()
	{
		var json = SpatialAreaPackageSerializer.Serialize(CreateValidPackage())
			.Replace("Second room", "Tampered room", StringComparison.Ordinal);

		var result = SpatialAreaPackageSerializer.Deserialize(json);

		Assert.IsFalse(result.Success);
		Assert.IsNull(result.Package);
		Assert.IsTrue(result.Diagnostics.Any(x => x.Code == "integrity-failed"));
	}

	[TestMethod]
	public void Validate_OrphanedExitReference_ReportsActionableDiagnostics()
	{
		var package = CreateValidPackage();
		package.Cells[0].Overlay.ExitKeys.Add("exit-missing");
		package.Exits[0].Cell2Key = "cell-missing";

		var diagnostics = SpatialAreaPackageSerializer.Validate(package);

		Assert.IsTrue(diagnostics.Any(x => x.Code == "orphan-overlay-exit"));
		Assert.IsTrue(diagnostics.Any(x => x.Code == "orphan-exit"));
	}

	[TestMethod]
	public void Validate_DuplicateKeys_RejectsAmbiguousIdRemapping()
	{
		var package = CreateValidPackage();
		package.Cells[1].Key = package.Cells[0].Key;

		var diagnostics = SpatialAreaPackageSerializer.Validate(package);

		Assert.IsTrue(diagnostics.Any(x => x.Code == "duplicate-cell-key"));
	}

	[TestMethod]
	public void TryResolvePackagePath_TraversalName_IsRejected()
	{
		var root = Path.Combine(Path.GetTempPath(), "futuremud-spatial-package-tests");

		var success = SpatialAreaTransferService.TryResolvePackagePath(
			root,
			"..\\outside",
			out _,
			out var error);

		Assert.IsFalse(success);
		StringAssert.Contains(error, "no path");
	}

	[TestMethod]
	public void TryResolvePackagePath_SimpleName_AddsExpectedSuffix()
	{
		var root = Path.Combine(Path.GetTempPath(), "futuremud-spatial-package-tests");

		var success = SpatialAreaTransferService.TryResolvePackagePath(
			root,
			"safe-zone",
			out var path,
			out _);

		Assert.IsTrue(success);
		Assert.AreEqual("safe-zone.fmsa.json", Path.GetFileName(path));
		Assert.IsTrue(path.StartsWith(Path.GetFullPath(root), StringComparison.OrdinalIgnoreCase));
	}

	[TestMethod]
	public void Deserialize_Version1PackageWithEmptyRoom_WarnsAndRemainsImportable()
	{
		var package = CreateValidPackage();
		package.Rooms.Add(new SpatialRoomDefinition
		{
			Key = "room-00003",
			SourceId = 102,
			X = 9
		});

		var json = SpatialAreaPackageSerializer.Serialize(package);
		var result = SpatialAreaPackageSerializer.Deserialize(json);

		Assert.IsTrue(result.Success);
		Assert.IsNotNull(result.Package);
		Assert.IsFalse(json.Contains("\"Zones\"", StringComparison.Ordinal));
		Assert.IsTrue(result.Diagnostics.Any(x => x.Code == "empty-room-skipped"));
	}

	[TestMethod]
	public void Validate_Version2MultiZonePackage_PreservesCrossZoneExit()
	{
		var package = CreateValidVersion2Package();
		var secondZone = new SpatialZoneDefinition
		{
			Key = "zone-00002",
			SourceId = 11,
			Name = "Second Zone",
			DefaultCellKey = "cell-00002",
			TimeZones = package.Zone.TimeZones
				.Select(x => new SpatialTimeZoneDefinition
				{
					ClockAlias = x.ClockAlias,
					TimeZoneAlias = x.TimeZoneAlias,
					TimeZoneDescription = x.TimeZoneDescription
				})
				.ToList()
		};
		package.Zones.Add(secondZone);
		package.SourceZones.Add(new SpatialAreaPackageSource
		{
			ZoneName = secondZone.Name,
			ZoneId = secondZone.SourceId,
			ShardName = package.Source.ShardName,
			ShardId = package.Source.ShardId,
			OverlayPackageName = package.Source.OverlayPackageName,
			OverlayPackageId = package.Source.OverlayPackageId,
			OverlayPackageRevision = package.Source.OverlayPackageRevision
		});
		package.Rooms[1].ZoneKey = secondZone.Key;
		package.Omissions.Add(new SpatialPackageOmission
		{
			Code = "boundary-exit",
			Message = "A boundary exit was deliberately omitted."
		});

		var json = SpatialAreaPackageSerializer.Serialize(package);
		var result = SpatialAreaPackageSerializer.Deserialize(json);

		Assert.IsTrue(result.Success,
			string.Join(Environment.NewLine, result.Diagnostics.Select(x => $"{x.Code}: {x.Message}")));
		Assert.IsNotNull(result.Package);
		Assert.AreEqual(2, result.Package.Zones.Count);
		Assert.AreEqual("boundary-exit", result.Package.Omissions.Single().Code);
		Assert.AreEqual("cell-00001", result.Package.Exits[0].Cell1Key);
		Assert.AreEqual("cell-00002", result.Package.Exits[0].Cell2Key);
	}

	[TestMethod]
	public void SerializeDeserialize_Version2RouteCell_RoundTripsGeometryAndAnchor()
	{
		var package = CreateValidVersion2Package();
		package.Cells[0].RouteCell = new SpatialRouteCellDefinition
		{
			LengthMetres = 500.0,
			DefaultPositionMetres = 125.0,
			PositiveDirectionName = "eastbound",
			NegativeDirectionName = "westbound",
			MetresPerRoomEquivalent = 100.0,
			TopologyVersion = 3,
			Landmarks =
			[
				new SpatialRouteLandmarkDefinition
				{
					SourceId = 400,
					Name = "Old Oak",
					Keywords = "oak tree",
					Description = "An old oak stands beside the road.",
					PositionMetres = 200.0
				}
			],
			ExitAnchors =
			[
				new SpatialRouteExitAnchorDefinition
				{
					ExitKey = "exit-00001",
					MinimumPositionMetres = 100.0,
					MaximumPositionMetres = 150.0,
					ArrivalPositionMetres = 125.0
				}
			]
		};

		var json = SpatialAreaPackageSerializer.Serialize(package);
		var result = SpatialAreaPackageSerializer.Deserialize(json);

		Assert.IsTrue(result.Success,
			string.Join(Environment.NewLine, result.Diagnostics.Select(x => $"{x.Code}: {x.Message}")));
		Assert.IsNotNull(result.Package);
		Assert.AreEqual(500.0, result.Package.Cells[0].RouteCell?.LengthMetres);
		Assert.AreEqual("exit-00001", result.Package.Cells[0].RouteCell?.ExitAnchors.Single().ExitKey);
	}

	private static SpatialAreaPackage CreateValidPackage()
	{
		return new SpatialAreaPackage
		{
			Version = 1,
			CreatedUtc = new DateTime(2026, 7, 24, 0, 0, 0, DateTimeKind.Utc),
			Source = new SpatialAreaPackageSource
			{
				ZoneName = "Test Zone",
				ZoneId = 10,
				ShardName = "Test Shard",
				ShardId = 2,
				OverlayPackageName = "Test Overlay",
				OverlayPackageId = 4,
				OverlayPackageRevision = 1
			},
			Zone = new SpatialZoneDefinition
			{
				Name = "Test Zone",
				DefaultCellKey = "cell-00001",
				TimeZones =
				[
					new SpatialTimeZoneDefinition
					{
						ClockAlias = "test-clock",
						TimeZoneAlias = "utc",
						TimeZoneDescription = "Universal"
					}
				]
			},
			Rooms =
			[
				new SpatialRoomDefinition
				{
					Key = "room-00001",
					SourceId = 100
				},
				new SpatialRoomDefinition
				{
					Key = "room-00002",
					SourceId = 101,
					X = 1
				}
			],
			Cells =
			[
				new SpatialCellDefinition
				{
					Key = "cell-00001",
					SourceId = 200,
					RoomKey = "room-00001",
					Overlay = new SpatialCellOverlayDefinition
					{
						CellName = "First Room",
						CellDescription = "The first room.",
						Terrain = new SpatialNamedReference { SourceId = 1, Name = "Default" },
						AmbientLightFactor = 1.0,
						ExitKeys = ["exit-00001"]
					}
				},
				new SpatialCellDefinition
				{
					Key = "cell-00002",
					SourceId = 201,
					RoomKey = "room-00002",
					Overlay = new SpatialCellOverlayDefinition
					{
						CellName = "Second Room",
						CellDescription = "Second room",
						Terrain = new SpatialNamedReference { SourceId = 1, Name = "Default" },
						AmbientLightFactor = 1.0,
						ExitKeys = ["exit-00001"]
					}
				}
			],
			Exits =
			[
				new SpatialExitDefinition
				{
					Key = "exit-00001",
					SourceId = 300,
					Cell1Key = "cell-00001",
					Cell2Key = "cell-00002",
					Side1 = new SpatialExitSideDefinition { Direction = (int)CardinalDirection.East },
					Side2 = new SpatialExitSideDefinition { Direction = (int)CardinalDirection.West },
					TimeMultiplier = 1.0,
					MaximumSizeToEnter = 12,
					MaximumSizeToEnterUpright = 12
				}
			]
		};
	}

	private static SpatialAreaPackage CreateValidVersion2Package()
	{
		var package = CreateValidPackage();
		package.Version = 2;
		package.Zone.Key = "zone-00001";
		package.Zone.SourceId = package.Source.ZoneId;
		package.Zones = [package.Zone];
		package.SourceZones = [package.Source];
		foreach (var room in package.Rooms)
		{
			room.ZoneKey = package.Zone.Key;
		}

		return package;
	}
}

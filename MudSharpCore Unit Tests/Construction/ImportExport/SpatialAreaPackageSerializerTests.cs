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

	private static SpatialAreaPackage CreateValidPackage()
	{
		return new SpatialAreaPackage
		{
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
}

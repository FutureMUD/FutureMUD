#nullable enable

using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Construction.ImportExport;
using Legacy = MudSharp.Construction.ImportExport.Legacy;

namespace MudSharpCore_Unit_Tests.Construction.ImportExport;

[TestClass]
public class SpatialAreaPackageLegacyTests
{
	private static string Fixture(int version) => File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
		"Construction", "ImportExport", "Fixtures", $"legacy-v{version}.json"));

	[DataTestMethod]
	[DataRow(1, "12d3e8316ac244409021aaba2ee0eeb170026420f502ea8847eccb935bd686eb")]
	[DataRow(2, "bff02a1deec8d987a339f5481dffc8d5ff79a79ca5993576eecf30162ec0f082")]
	[DataRow(3, "7443b506d3cc331bae679142eb05833827f38fab376bf94d67265ca761429293")]
	public void FrozenPriorBinaryPackage_VerifiesBeforeNormalising(int version, string checksum)
	{
		var result = SpatialAreaPackageSerializer.Deserialize(Fixture(version));
		Assert.IsTrue(result.Success, string.Join("; ", result.Diagnostics.Select(x => x.Message)));
		Assert.AreEqual(version, result.SourceVersion);
		Assert.AreEqual(checksum, result.SourceIntegritySha256);
		Assert.AreEqual(4, result.Package!.Version);
		Assert.AreEqual(200L, result.Package.Cells[0].SourceId, "Retain Cell ID, never Room ID.");
		Assert.AreEqual(17, result.Package.Cells[0].X);
		Assert.AreEqual(result.Package.Cells[0].X, result.Package.Cells[1].X, "Duplicate coordinates survive.");
		if (version >= 2) Assert.AreEqual(7L, result.Package.Cells[0].RouteCell!.TopologyVersion);
		if (version == 3)
		{
			Assert.AreEqual(2, result.Package.Areas.Count);
			Assert.IsTrue(result.Package.Areas.All(x => x.CellKeys.Contains("c1")));
		}
		var currentJson = SpatialAreaPackageSerializer.Serialize(result.Package);
		Assert.IsFalse(currentJson.Contains("\"Rooms\"", StringComparison.Ordinal));
		Assert.IsFalse(currentJson.Contains("\"RoomKey\"", StringComparison.Ordinal));
		Assert.IsTrue(SpatialAreaPackageSerializer.Deserialize(currentJson).Success);
	}

	[DataTestMethod]
	[DataRow(1)] [DataRow(2)] [DataRow(3)]
	public void FrozenPriorBinaryPackage_TamperingFailsOriginalChecksum(int version)
	{
		var result = SpatialAreaPackageSerializer.Deserialize(Fixture(version).Replace("Legacy", "Tampered", StringComparison.Ordinal));
		Assert.IsFalse(result.Success);
		Assert.IsTrue(result.Diagnostics.Any(x => x.Code == "integrity-failed"));
	}

	[TestMethod]
	public void LegacyAmbiguityAndReferencedEmptyRoom_RefuseBeforeImport()
	{
		var legacy = Legacy.SpatialAreaPackageSerializer.Deserialize(Fixture(3)).Package!;
		legacy.Cells[1].RoomKey = legacy.Cells[0].RoomKey;
		var result = SpatialAreaPackageSerializer.Deserialize(Legacy.SpatialAreaPackageSerializer.Serialize(legacy));
		Assert.IsFalse(result.Success);
		Assert.IsTrue(result.Diagnostics.Any(x => x.Code == "multi-cell-room"));
		Assert.IsTrue(result.Diagnostics.Any(x => x.Code == "referenced-empty-room"));
	}

	[TestMethod]
	public void LegacyUnreferencedEmptyRoom_WarnsAndPreservesSoleCells()
	{
		var legacy = Legacy.SpatialAreaPackageSerializer.Deserialize(Fixture(1)).Package!;
		legacy.Rooms.Add(new Legacy.SpatialRoomDefinition { Key = "empty", SourceId = 999, X = 99 });
		var result = SpatialAreaPackageSerializer.Deserialize(Legacy.SpatialAreaPackageSerializer.Serialize(legacy));
		Assert.IsTrue(result.Success);
		Assert.AreEqual(2, result.Package!.Cells.Count);
		Assert.IsTrue(result.Diagnostics.Any(x => x.Code == "empty-room-skipped"));
	}
}

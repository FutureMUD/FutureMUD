#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Construction.ImportExport;

namespace MudSharpCore_Unit_Tests.Construction.ImportExport;

[TestClass]
public class SpatialAreaPackageChecksumTests
{
	private const string Description = "First line\r\nsecond line\nthird line";

	private static JsonNode Package(int version)
	{
		if (version <= 3)
		{
			var node = JsonNode.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
				"Construction", "ImportExport", "Fixtures", $"legacy-v{version}.json")))!;
			node["Cells"]![0]!["Overlay"]!["CellDescription"] = Description;
			return node;
		}
		var current = SpatialAreaPackageSerializerTests.CreateValidPackage();
		current.Rooms[0].Overlay.RoomDescription = Description;
		return JsonNode.Parse(version == 4
			? SpatialAreaPackageSerializerTests.CreateVersion4Archive(current).Archive
			: SpatialAreaPackageSerializer.Serialize(current))!;
	}

	private static string SignedArchive(int version, bool unixChecksum)
	{
		var node = Package(version);
		node["IntegritySha256"] = string.Empty;
		var canonicalOptions = new JsonSerializerOptions { WriteIndented = true, NewLine = unixChecksum ? "\n" : "\r\n" };
		var checksum = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(node.ToJsonString(canonicalOptions))));
		node["IntegritySha256"] = checksum;
		// Transfer formatting is deliberately the opposite of the writer's signed canonical form.
		return node.ToJsonString(new JsonSerializerOptions { WriteIndented = true, NewLine = unixChecksum ? "\r\n" : "\n" });
	}

	[DataTestMethod]
	[DataRow(1, false)] [DataRow(1, true)]
	[DataRow(2, false)] [DataRow(2, true)]
	[DataRow(3, false)] [DataRow(3, true)]
	[DataRow(4, false)] [DataRow(4, true)]
	[DataRow(5, false)] [DataRow(5, true)]
	public void Deserialize_HistoricalWindowsAndUnixChecksums_VerifyBeforeConversion(int version, bool unixChecksum)
	{
		var archive = SignedArchive(version, unixChecksum);
		var checksum = JsonNode.Parse(archive)!["IntegritySha256"]!.GetValue<string>();
		var result = SpatialAreaPackageSerializer.Deserialize(archive);
		Assert.IsTrue(result.Success, string.Join("; ", result.Diagnostics.Select(x => x.Message)));
		Assert.AreEqual(version, result.SourceVersion);
		Assert.AreEqual(checksum, version < 5 ? result.SourceIntegritySha256 : result.Package!.IntegritySha256);
		Assert.AreEqual(5, result.Package!.Version);
		Assert.AreEqual(Description, result.Package.Rooms[0].Overlay.RoomDescription);
	}

	[DataTestMethod]
	[DataRow(1, false)] [DataRow(1, true)]
	[DataRow(2, false)] [DataRow(2, true)]
	[DataRow(3, false)] [DataRow(3, true)]
	[DataRow(4, false)] [DataRow(4, true)]
	[DataRow(5, false)] [DataRow(5, true)]
	public void Deserialize_PayloadNewlineChangeWithOriginalChecksum_RefusesBeforeConversion(int version, bool unixChecksum)
	{
		var node = JsonNode.Parse(SignedArchive(version, unixChecksum))!;
		node[version == 5 ? "Rooms" : "Cells"]![0]!["Overlay"]![version == 5 ? "RoomDescription" : "CellDescription"] =
			Description.Replace("\r\n", "\n", StringComparison.Ordinal);
		var result = SpatialAreaPackageSerializer.Deserialize(node.ToJsonString());
		Assert.IsFalse(result.Success);
		Assert.IsNull(result.Package);
		Assert.IsTrue(result.Diagnostics.Any(x => x.Code == "integrity-failed"));
	}

	[TestMethod]
	public void Serialize_CurrentPackage_UsesStableHistoricalWindowsFormatting()
	{
		var json = SpatialAreaPackageSerializer.Serialize(SpatialAreaPackageSerializerTests.CreateValidPackage());
		StringAssert.Contains(json, "\r\n");
		Assert.IsFalse(json.Replace("\r\n", string.Empty, StringComparison.Ordinal).Contains('\n'));
	}
}

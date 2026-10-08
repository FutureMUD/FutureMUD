using System.Text.Json;
using System.Text.Json.Nodes;

#nullable enable

namespace MudSharp.Construction.ImportExport;

public static partial class SpatialAreaPackageSerializer
{
	private static readonly IReadOnlyDictionary<string, string> Version4PropertyNames =
		new Dictionary<string, string>(StringComparer.Ordinal)
		{
			["Cells"] = "Rooms",
			["DefaultCellKey"] = "DefaultRoomKey",
			["CellKeys"] = "RoomKeys",
			["CellName"] = "RoomName",
			["CellDescription"] = "RoomDescription",
			["RouteCell"] = "RouteRoom",
			["Cell1Key"] = "Room1Key",
			["Cell2Key"] = "Room2Key",
			["FallCellKey"] = "FallRoomKey"
		};

	private static SpatialAreaPackageReadResult ReadLegacyV4(string json)
	{
		var source = LegacyV4.SpatialAreaPackageSerializer.Deserialize(json);
		if (!source.Success) return new SpatialAreaPackageReadResult { SourceVersion = 4, Diagnostics = source.Diagnostics };
		var package = NormalizeDirectCellPackage(source.Package!);
		var diagnostics = Validate(package);
		return new SpatialAreaPackageReadResult
		{
			SourceVersion = 4,
			SourceIntegritySha256 = source.Package!.IntegritySha256,
			Package = diagnostics.Any(x => x.Severity == SpatialAreaTransferDiagnosticSeverity.Error) ? null : package,
			Diagnostics = diagnostics
		};
	}

	private static SpatialAreaPackage NormalizeDirectCellPackage(LegacyV4.SpatialAreaPackage source)
	{
		// The frozen reader has already checked the original canonical checksum.
		// Rename property labels, never identifier values or user-authored strings.
		var node = JsonSerializer.SerializeToNode(source, Options)!;
		RenameVersion4Properties(node);
		node["Version"] = SpatialAreaPackage.CurrentVersion;
		var package = node.Deserialize<SpatialAreaPackage>(Options)!;
		Serialize(package);
		return package;
	}

	private static void RenameVersion4Properties(JsonNode? node)
	{
		if (node is JsonObject properties)
		{
			foreach (var property in properties.ToArray())
			{
				RenameVersion4Properties(property.Value);
				if (!Version4PropertyNames.TryGetValue(property.Key, out var replacement)) continue;
				properties.Remove(property.Key);
				properties.Add(replacement, property.Value);
			}
		}
		else if (node is JsonArray values)
		{
			foreach (var value in values) RenameVersion4Properties(value);
		}
	}
}

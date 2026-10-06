using System.Text.Json;

#nullable enable

namespace MudSharp.Construction.ImportExport;

public static partial class SpatialAreaPackageSerializer
{
	private static T CopyWireValue<T>(object value) where T : class =>
		JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value), Options)!;

	private static SpatialAreaPackageReadResult ReadLegacy(string json)
	{
		try
		{
			// Verify the original canonical checksum before normalization changes any shape.
			var read = Legacy.SpatialAreaPackageSerializer.Deserialize(json);
			if (!read.Success)
				return new SpatialAreaPackageReadResult { Diagnostics = read.Diagnostics };
			var source = read.Package!;
			var diagnostics = read.Diagnostics.ToList();
			var children = source.Cells.GroupBy(x => x.RoomKey, StringComparer.Ordinal)
				.ToDictionary(x => x.Key, x => x.ToList(), StringComparer.Ordinal);
			foreach (var group in children.Where(x => x.Value.Count != 1))
				diagnostics.Add(Error("multi-cell-room",
					$"Legacy room '{group.Key}' has multiple cells ({string.Join(", ", group.Value.Select(x => x.Key))}); explicit disposition is required."));
			if (source.Areas is null || source.Areas.Any(x => x is null))
				diagnostics.Add(Error("missing-areas", "Legacy area data is missing or contains a null entry."));
			else
			{
				foreach (var area in source.Areas)
				{
					if (area.RoomKeys is null || area.RoomKeys.Count == 0)
						diagnostics.Add(Error("invalid-area-rooms", $"Legacy area '{area.Key}' requires room membership."));
					else if (area.RoomKeys.Any(x => x is null || !children.ContainsKey(x)))
						diagnostics.Add(Error("referenced-empty-room", $"Legacy area '{area.Key}' references an empty or missing room; explicit disposition is required."));
				}
			}
			if (diagnostics.Any(x => x.Severity == SpatialAreaTransferDiagnosticSeverity.Error))
				return new SpatialAreaPackageReadResult { Diagnostics = diagnostics };

			var rooms = source.Rooms.ToDictionary(x => x.Key, StringComparer.Ordinal);
			var legacyZones = Legacy.SpatialAreaPackageSerializer.GetZoneDefinitions(source);
			var package = new SpatialAreaPackage
			{
				CreatedUtc = source.CreatedUtc,
				Source = CopyWireValue<SpatialAreaPackageSource>(source.Source),
				SourceZones = source.Version == 1
					? [CopyWireValue<SpatialAreaPackageSource>(source.Source)]
					: CopyWireValue<List<SpatialAreaPackageSource>>(source.SourceZones),
				Zones = legacyZones.Select((zone, index) =>
				{
					var result = CopyWireValue<SpatialZoneDefinition>(zone);
					result.Key = Legacy.SpatialAreaPackageSerializer.ZoneKey(source, zone, index);
					return result;
				}).ToList(),
				Cells = source.Cells.Select(cell =>
				{
					var room = rooms[cell.RoomKey];
					return new SpatialCellDefinition
					{
						Key = cell.Key,
						SourceId = cell.SourceId,
						ZoneKey = Legacy.SpatialAreaPackageSerializer.RoomZoneKey(source, room),
						X = room.X, Y = room.Y, Z = room.Z,
						Overlay = CopyWireValue<SpatialCellOverlayDefinition>(cell.Overlay),
						ForagableProfile = cell.ForagableProfile is null ? null : CopyWireValue<SpatialNamedReference>(cell.ForagableProfile),
						Tags = CopyWireValue<List<SpatialNamedReference>>(cell.Tags),
						RangedCovers = CopyWireValue<List<SpatialNamedReference>>(cell.RangedCovers),
						MagicResources = CopyWireValue<List<SpatialMagicResourceDefinition>>(cell.MagicResources),
						RouteCell = cell.RouteCell is null ? null : CopyWireValue<SpatialRouteCellDefinition>(cell.RouteCell)
					};
				}).ToList(),
				Exits = CopyWireValue<List<SpatialExitDefinition>>(source.Exits),
				Areas = source.Areas!.Select(area => new SpatialAreaDefinition
				{
					Key = area.Key, SourceId = area.SourceId, Name = area.Name,
					WeatherController = area.WeatherController is null ? null : CopyWireValue<SpatialNamedReference>(area.WeatherController),
					CellKeys = area.RoomKeys.Select(x => children[x].Single().Key).ToList()
				}).ToList(),
				Omissions = CopyWireValue<List<SpatialPackageOmission>>(source.Omissions)
			};
			diagnostics.AddRange(Validate(package));
			if (diagnostics.Any(x => x.Severity == SpatialAreaTransferDiagnosticSeverity.Error))
				return new SpatialAreaPackageReadResult { Diagnostics = diagnostics };
			Serialize(package);
			return new SpatialAreaPackageReadResult
			{
				Package = package, Diagnostics = diagnostics,
				SourceVersion = source.Version, SourceIntegritySha256 = source.IntegritySha256
			};
		}
		catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or NullReferenceException or JsonException)
		{
			// Invalid legacy keys/references must produce a preflight refusal, never a write-time crash.
			return new SpatialAreaPackageReadResult { Diagnostics = [Error("invalid-legacy-references", ex.Message)] };
		}
	}
}

#nullable enable

using System.Data;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;

namespace MudSharp.Construction.ImportExport;

public sealed partial class SpatialAreaTransferService
{
	private SpatialAreaTransferResult ImportVersion2(
		ICharacter actor,
		IShard targetShard,
		ImportPreflight preflight)
	{
		var package = preflight.Package;
		var gameworld = actor.Gameworld;
		var dbZones = new Dictionary<string, Models.Zone>(StringComparer.Ordinal);
		var dbRooms = new Dictionary<string, Models.Room>(StringComparer.Ordinal);
		var dbCells = new Dictionary<string, Models.Cell>(StringComparer.Ordinal);
		var databaseCommitted = false;
		try
		{
			using (new FMDB())
			using (var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable))
			{
				foreach (var targetName in preflight.ZoneNames.Values)
				{
					if (FMDB.Context.Zones.Any(x => x.Name == targetName))
					{
						return Failure(
							$"A zone named '{targetName}' was created after validation. Nothing was imported.",
							preflight.Diagnostics,
							"zone-name-collision");
					}
				}

				foreach (var (zone, index) in preflight.Zones.Select((value, index) => (value, index)))
				{
					var zoneKey = SpatialAreaPackageSerializer.ZoneKey(package, zone, index);
					var dbZone = new Models.Zone
					{
						Name = preflight.ZoneNames[zoneKey],
						ShardId = targetShard.Id,
						Latitude = zone.LatitudeRadians,
						Longitude = zone.LongitudeRadians,
						Elevation = zone.ElevationMetres,
						AmbientLightPollution = zone.AmbientLightPollution,
						ForagableProfileId = zone.ForagableProfile is null
							? null
							: preflight.ForagableProfiles[zone.ForagableProfile.Name].Id,
						WeatherControllerId = preflight.WeatherControllers[zoneKey]?.Id
					};
					foreach (var resolvedTimeZone in preflight.TimeZonesByZone[zoneKey].Values)
					{
						dbZone.ZonesTimezones.Add(new ZonesTimezones
						{
							Zone = dbZone,
							ClockId = resolvedTimeZone.Clock.Id,
							TimezoneId = resolvedTimeZone.TimeZone.Id
						});
					}

					dbZones.Add(zoneKey, dbZone);
					FMDB.Context.Zones.Add(dbZone);
				}

				var importedRoomKeys = package.Cells
					.Select(x => x.RoomKey)
					.ToHashSet(StringComparer.Ordinal);
				foreach (var room in package.Rooms.Where(x => importedRoomKeys.Contains(x.Key)))
				{
					var zoneKey = SpatialAreaPackageSerializer.RoomZoneKey(package, room);
					var dbRoom = new Models.Room
					{
						Zone = dbZones[zoneKey],
						X = room.X,
						Y = room.Y,
						Z = room.Z
					};
					dbRooms.Add(room.Key, dbRoom);
					FMDB.Context.Rooms.Add(dbRoom);
				}

				FMDB.Context.SaveChanges();

				foreach (var cell in package.Cells)
				{
					var dbCell = new Models.Cell
					{
						Room = dbRooms[cell.RoomKey],
						Temporary = false,
						EffectData = "<Effects/>",
						ForagableProfileId = cell.ForagableProfile is null
							? null
							: preflight.ForagableProfiles[cell.ForagableProfile.Name].Id
					};
					dbCells.Add(cell.Key, dbCell);
					FMDB.Context.Cells.Add(dbCell);
				}

				FMDB.Context.SaveChanges();

				var dbOverlays = new Dictionary<string, Models.CellOverlay>(StringComparer.Ordinal);
				foreach (var cell in package.Cells)
				{
					var overlay = cell.Overlay;
					var dbOverlay = new Models.CellOverlay
					{
						Cell = dbCells[cell.Key],
						Name = preflight.OverlayPackage.Name,
						CellName = overlay.CellName,
						CellDescription = overlay.CellDescription,
						CellOverlayPackageId = preflight.OverlayPackage.Id,
						CellOverlayPackageRevisionNumber = preflight.OverlayPackage.RevisionNumber,
						TerrainId = preflight.Terrains[overlay.Terrain.Name].Id,
						HearingProfileId = overlay.HearingProfile is null
							? null
							: preflight.HearingProfiles[overlay.HearingProfile.Name].Id,
						OutdoorsType = overlay.OutdoorsType,
						AmbientLightFactor = overlay.AmbientLightFactor,
						AddedLight = overlay.AddedLight,
						AtmosphereId = overlay.Atmosphere is null
							? null
							: preflight.Fluids[FluidKey(overlay.Atmosphere)].Id,
						AtmosphereType = overlay.Atmosphere?.Kind,
						SafeQuit = overlay.SafeQuit
					};
					dbCells[cell.Key].CellOverlays.Add(dbOverlay);
					dbOverlays.Add(cell.Key, dbOverlay);
					FMDB.Context.CellOverlays.Add(dbOverlay);
				}

				FMDB.Context.SaveChanges();

				var dbRouteCells = new Dictionary<string, Models.RouteCell>(StringComparer.Ordinal);
				foreach (var cell in package.Cells)
				{
					var dbCell = dbCells[cell.Key];
					dbCell.CurrentOverlay = dbOverlays[cell.Key];
					foreach (var tag in cell.Tags)
					{
						dbCell.CellsTags.Add(new CellsTags
						{
							Cell = dbCell,
							TagId = preflight.Tags[tag.Name].Id
						});
					}

					foreach (var cover in cell.RangedCovers)
					{
						dbCell.CellsRangedCovers.Add(new CellsRangedCovers
						{
							Cell = dbCell,
							RangedCoverId = preflight.RangedCovers[cover.Name].Id
						});
					}

					foreach (var resource in cell.MagicResources)
					{
						dbCell.CellsMagicResources.Add(new CellMagicResource
						{
							Cell = dbCell,
							MagicResourceId = preflight.MagicResources[resource.Resource.Name].Id,
							Amount = resource.Amount
						});
					}

					if (cell.RouteCell is null)
					{
						continue;
					}

					var route = cell.RouteCell;
					var dbRoute = new Models.RouteCell
					{
						Cell = dbCell,
						CellId = dbCell.Id,
						LengthMetres = (decimal)route.LengthMetres,
						DefaultPositionMetres = (decimal)route.DefaultPositionMetres,
						PositiveDirectionName = route.PositiveDirectionName,
						NegativeDirectionName = route.NegativeDirectionName,
						MetresPerRoomEquivalent = (decimal)route.MetresPerRoomEquivalent,
						TopologyVersion = route.TopologyVersion
					};
					foreach (var landmark in route.Landmarks)
					{
						dbRoute.Landmarks.Add(new Models.RouteCellLandmark
						{
							RouteCell = dbRoute,
							Name = landmark.Name,
							Keywords = landmark.Keywords,
							Description = landmark.Description,
							PositionMetres = (decimal)landmark.PositionMetres,
							DisplayOrder = landmark.DisplayOrder
						});
					}

					dbCell.RouteCell = dbRoute;
					dbRouteCells.Add(cell.Key, dbRoute);
					FMDB.Context.RouteCells.Add(dbRoute);
				}

				var dbExits = new Dictionary<string, Models.Exit>(StringComparer.Ordinal);
				foreach (var exit in package.Exits)
				{
					var dbExit = new Models.Exit
					{
						CellId1 = dbCells[exit.Cell1Key].Id,
						CellId2 = dbCells[exit.Cell2Key].Id,
						Direction1 = exit.Side1.Direction,
						Direction2 = exit.Side2.Direction,
						TimeMultiplier = exit.TimeMultiplier,
						AcceptsDoor = exit.AcceptsDoor,
						DoorSize = exit.AcceptsDoor ? exit.DoorSize : null,
						MaximumSizeToEnter = exit.MaximumSizeToEnter,
						MaximumSizeToEnterUpright = exit.MaximumSizeToEnterUpright,
						FallCell = exit.FallCellKey is null ? null : dbCells[exit.FallCellKey].Id,
						IsClimbExit = exit.IsClimbExit,
						ClimbDifficulty = exit.ClimbDifficulty,
						BlockedLayers = string.Join(",", exit.BlockedLayers),
						Keywords1 = exit.Side1.Keywords,
						Keywords2 = exit.Side2.Keywords,
						InboundDescription1 = exit.Side1.InboundDescription,
						InboundDescription2 = exit.Side2.InboundDescription,
						OutboundDescription1 = exit.Side1.OutboundDescription,
						OutboundDescription2 = exit.Side2.OutboundDescription,
						InboundTarget1 = exit.Side1.InboundTarget,
						InboundTarget2 = exit.Side2.InboundTarget,
						OutboundTarget1 = exit.Side1.OutboundTarget,
						OutboundTarget2 = exit.Side2.OutboundTarget,
						Verb1 = exit.Side1.Verb,
						Verb2 = exit.Side2.Verb,
						PrimaryKeyword1 = exit.Side1.PrimaryKeyword,
						PrimaryKeyword2 = exit.Side2.PrimaryKeyword
					};
					dbExits.Add(exit.Key, dbExit);
					FMDB.Context.Exits.Add(dbExit);
				}

				FMDB.Context.SaveChanges();

				foreach (var cell in package.Cells)
				{
					foreach (var exitKey in cell.Overlay.ExitKeys)
					{
						dbOverlays[cell.Key].CellOverlaysExits.Add(new CellOverlayExit
						{
							CellOverlay = dbOverlays[cell.Key],
							Exit = dbExits[exitKey]
						});
					}

					if (cell.RouteCell is null)
					{
						continue;
					}

					var dbRoute = dbRouteCells[cell.Key];
					foreach (var anchor in cell.RouteCell.ExitAnchors)
					{
						dbRoute.ExitAnchors.Add(new Models.RouteExitAnchor
						{
							RouteCell = dbRoute,
							Exit = dbExits[anchor.ExitKey],
							ExitId = dbExits[anchor.ExitKey].Id,
							RouteCellId = dbRoute.CellId,
							MinimumPositionMetres = (decimal)anchor.MinimumPositionMetres,
							MaximumPositionMetres = (decimal)anchor.MaximumPositionMetres,
							ArrivalPositionMetres = (decimal)anchor.ArrivalPositionMetres
						});
					}
				}

				foreach (var (zone, index) in preflight.Zones.Select((value, index) => (value, index)))
				{
					var zoneKey = SpatialAreaPackageSerializer.ZoneKey(package, zone, index);
					dbZones[zoneKey].DefaultCell = dbCells[zone.DefaultCellKey];
				}

				FMDB.Context.SaveChanges();
				transaction.Commit();
				databaseCommitted = true;
			}

			var runtimeZones = new Dictionary<string, Zone>(StringComparer.Ordinal);
			foreach (var (zone, index) in preflight.Zones.Select((value, index) => (value, index)))
			{
				var zoneKey = SpatialAreaPackageSerializer.ZoneKey(package, zone, index);
				var runtimeZone = new Zone(dbZones[zoneKey], gameworld);
				runtimeZones.Add(zoneKey, runtimeZone);
				gameworld.Add(runtimeZone);
			}

			foreach (var (zone, index) in preflight.Zones.Select((value, index) => (value, index)))
			{
				var zoneKey = SpatialAreaPackageSerializer.ZoneKey(package, zone, index);
				var runtimeZone = runtimeZones[zoneKey];
				var defaultCell = package.Cells.First(x => x.Key == zone.DefaultCellKey);
				var roomDefinitions = package.Rooms
					.Where(x => dbRooms.ContainsKey(x.Key) &&
					            SpatialAreaPackageSerializer.RoomZoneKey(package, x) == zoneKey)
					.OrderByDescending(x => x.Key == defaultCell.RoomKey)
					.ThenBy(x => x.Key);
				foreach (var roomDefinition in roomDefinitions)
				{
					var newRoom = new Room(dbRooms[roomDefinition.Key], runtimeZone);
					gameworld.Add(newRoom);
					foreach (var cellDefinition in package.Cells
						         .Where(x => x.RoomKey == roomDefinition.Key)
						         .OrderByDescending(x => x.Key == zone.DefaultCellKey)
						         .ThenBy(x => x.Key))
					{
						var newCell = new Cell(dbCells[cellDefinition.Key], newRoom);
						gameworld.Add(newCell);
					}
				}

			}

			foreach (var runtimeZone in runtimeZones.Values)
			{
				runtimeZone.PostLoadSetup();
			}

			var importedZoneIds = runtimeZones.Values.Select(x => x.Id).ToList();
			return new SpatialAreaTransferResult
			{
				Success = true,
				Summary = $"Imported {runtimeZones.Count:N0} new zone(s): " +
				          $"{runtimeZones.Values.Select(x => $"{x.Name} (#{x.Id:N0})").ListToString()}. " +
				          "Existing spatial content was not modified.",
				PackagePath = preflight.PackagePath,
				ImportedZoneId = importedZoneIds[0],
				ImportedZoneIds = importedZoneIds,
				ZoneCount = runtimeZones.Count,
				Diagnostics = preflight.Diagnostics,
				RoomCount = PackagedRoomCount(package),
				CellCount = package.Cells.Count,
				ExitCount = package.Exits.Count,
				OmittedItems = PackageOmissions(preflight)
			};
		}
		catch (Exception ex)
		{
			if (databaseCommitted && dbZones.Count > 0)
			{
				var committedDiagnostics = preflight.Diagnostics.ToList();
				var zoneIds = dbZones.Values.Select(x => x.Id).ToList();
				committedDiagnostics.Add(Error("runtime-load-failed",
					$"The database import committed as zone(s) {zoneIds.Select(x => $"#{x:N0}").ListToString()}, " +
					$"but the live server could not register all content: {ex.Message}"));
				return new SpatialAreaTransferResult
				{
					Summary = "The new zones were persisted but are not fully available in memory. Restart the server " +
					          "before retrying or editing them; do not re-import the package.",
					PackagePath = preflight.PackagePath,
					ImportedZoneId = zoneIds[0],
					ImportedZoneIds = zoneIds,
					ZoneCount = zoneIds.Count,
					Diagnostics = committedDiagnostics,
					RoomCount = PackagedRoomCount(package),
					CellCount = package.Cells.Count,
					ExitCount = package.Exits.Count,
					OmittedItems = PackageOmissions(preflight)
				};
			}

			return Failure(
				$"Import failed before commit: {ex.Message}. The database transaction was rolled back.",
				preflight.Diagnostics,
				"import-failed");
		}
	}

	private static int PackagedRoomCount(SpatialAreaPackage package)
	{
		return package.Cells
			.Select(x => x.RoomKey)
			.Distinct(StringComparer.Ordinal)
			.Count();
	}

	private static IReadOnlyList<string> PackageOmissions(ImportPreflight preflight)
	{
		return (preflight.Package.Omissions ?? [])
			.Select(x => x.Message)
			.Concat(preflight.Diagnostics
				.Where(x => x.Code == "empty-room-skipped")
				.Select(x => x.Message))
			.Distinct(StringComparer.InvariantCultureIgnoreCase)
			.ToList();
	}
}

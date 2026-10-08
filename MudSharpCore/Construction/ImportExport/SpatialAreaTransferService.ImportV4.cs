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
	private SpatialAreaTransferResult ImportVersion4(
		ICharacter actor,
		IShard targetShard,
		ImportPreflight preflight)
	{
		var package = preflight.Package;
		var gameworld = actor.Gameworld;
		var dbZones = new Dictionary<string, Models.Zone>(StringComparer.Ordinal);
		var dbAreas = new Dictionary<string, Models.Areas>(StringComparer.Ordinal);
		var dbRooms = new Dictionary<string, Models.Room>(StringComparer.Ordinal);
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

				foreach (var room in package.Rooms)
				{
					var dbRoom = new Models.Room
					{
						Zone = dbZones[room.ZoneKey],
						X = room.X, Y = room.Y, Z = room.Z,
						Temporary = false,
						EffectData = "<Effects/>",
						ForagableProfileId = room.ForagableProfile is null
							? null
							: preflight.ForagableProfiles[room.ForagableProfile.Name].Id
					};
					dbRooms.Add(room.Key, dbRoom);
					FMDB.Context.Rooms.Add(dbRoom);
				}

				FMDB.Context.SaveChanges();

				foreach (var area in package.Areas)
				{
					var dbArea = new Models.Areas
					{
						Name = area.Name,
						WeatherControllerId = preflight.AreaWeatherControllers[area.Key]?.Id
					};
					foreach (var cellKey in area.RoomKeys)
					{
						dbArea.AreasRooms.Add(new AreasRooms
						{
							Area = dbArea,
							Room = dbRooms[cellKey]
						});
					}

					dbAreas.Add(area.Key, dbArea);
					FMDB.Context.Areas.Add(dbArea);
				}

				FMDB.Context.SaveChanges();

				var dbOverlays = new Dictionary<string, Models.RoomOverlay>(StringComparer.Ordinal);
				foreach (var room in package.Rooms)
				{
					var overlay = room.Overlay;
					var dbOverlay = new Models.RoomOverlay
					{
						Room = dbRooms[room.Key],
						Name = preflight.OverlayPackage.Name,
						RoomName = overlay.RoomName,
						RoomDescription = overlay.RoomDescription,
						RoomOverlayPackageId = preflight.OverlayPackage.Id,
						RoomOverlayPackageRevisionNumber = preflight.OverlayPackage.RevisionNumber,
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
					dbRooms[room.Key].RoomOverlays.Add(dbOverlay);
					dbOverlays.Add(room.Key, dbOverlay);
					FMDB.Context.RoomOverlays.Add(dbOverlay);
				}

				FMDB.Context.SaveChanges();

				var dbRouteRooms = new Dictionary<string, Models.RouteRoom>(StringComparer.Ordinal);
				foreach (var room in package.Rooms)
				{
					var dbRoom = dbRooms[room.Key];
					dbRoom.CurrentOverlay = dbOverlays[room.Key];
					foreach (var tag in room.Tags)
					{
						dbRoom.RoomsTags.Add(new RoomsTags
						{
							Room = dbRoom,
							TagId = preflight.Tags[tag.Name].Id
						});
					}

					foreach (var cover in room.RangedCovers)
					{
						dbRoom.RoomsRangedCovers.Add(new RoomsRangedCovers
						{
							Room = dbRoom,
							RangedCoverId = preflight.RangedCovers[cover.Name].Id
						});
					}

					foreach (var resource in room.MagicResources)
					{
						dbRoom.RoomsMagicResources.Add(new RoomMagicResource
						{
							Room = dbRoom,
							MagicResourceId = preflight.MagicResources[resource.Resource.Name].Id,
							Amount = resource.Amount
						});
					}

					if (room.RouteRoom is null)
					{
						continue;
					}

					var route = room.RouteRoom;
					var dbRoute = new Models.RouteRoom
					{
						Room = dbRoom,
						RoomId = dbRoom.Id,
						LengthMetres = (decimal)route.LengthMetres,
						DefaultPositionMetres = (decimal)route.DefaultPositionMetres,
						PositiveDirectionName = route.PositiveDirectionName,
						NegativeDirectionName = route.NegativeDirectionName,
						MetresPerRoomEquivalent = (decimal)route.MetresPerRoomEquivalent,
						TopologyVersion = route.TopologyVersion
					};
					foreach (var landmark in route.Landmarks)
					{
						dbRoute.Landmarks.Add(new Models.RouteRoomLandmark
						{
							RouteRoom = dbRoute,
							Name = landmark.Name,
							Keywords = landmark.Keywords,
							Description = landmark.Description,
							PositionMetres = (decimal)landmark.PositionMetres,
							DisplayOrder = landmark.DisplayOrder
						});
					}

					dbRoom.RouteRoom = dbRoute;
					dbRouteRooms.Add(room.Key, dbRoute);
					FMDB.Context.RouteRooms.Add(dbRoute);
				}

				var dbExits = new Dictionary<string, Models.Exit>(StringComparer.Ordinal);
				foreach (var exit in package.Exits)
				{
					var dbExit = new Models.Exit
					{
						RoomId1 = dbRooms[exit.Room1Key].Id,
						RoomId2 = dbRooms[exit.Room2Key].Id,
						Direction1 = exit.Side1.Direction,
						Direction2 = exit.Side2.Direction,
						TimeMultiplier = exit.TimeMultiplier,
						AcceptsDoor = exit.AcceptsDoor,
						DoorSize = exit.AcceptsDoor ? exit.DoorSize : null,
						MaximumSizeToEnter = exit.MaximumSizeToEnter,
						MaximumSizeToEnterUpright = exit.MaximumSizeToEnterUpright,
						FallRoom = exit.FallRoomKey is null ? null : dbRooms[exit.FallRoomKey].Id,
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

				foreach (var room in package.Rooms)
				{
					foreach (var exitKey in room.Overlay.ExitKeys)
					{
						dbOverlays[room.Key].RoomOverlaysExits.Add(new RoomOverlayExit
						{
							RoomOverlay = dbOverlays[room.Key],
							Exit = dbExits[exitKey]
						});
					}

					if (room.RouteRoom is null)
					{
						continue;
					}

					var dbRoute = dbRouteRooms[room.Key];
					foreach (var anchor in room.RouteRoom.ExitAnchors)
					{
						dbRoute.ExitAnchors.Add(new Models.RouteExitAnchor
						{
							RouteRoom = dbRoute,
							Exit = dbExits[anchor.ExitKey],
							ExitId = dbExits[anchor.ExitKey].Id,
							RouteRoomId = dbRoute.RoomId,
							MinimumPositionMetres = (decimal)anchor.MinimumPositionMetres,
							MaximumPositionMetres = (decimal)anchor.MaximumPositionMetres,
							ArrivalPositionMetres = (decimal)anchor.ArrivalPositionMetres
						});
					}
				}

				foreach (var (zone, index) in preflight.Zones.Select((value, index) => (value, index)))
				{
					var zoneKey = SpatialAreaPackageSerializer.ZoneKey(package, zone, index);
					dbZones[zoneKey].DefaultRoom = dbRooms[zone.DefaultRoomKey];
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
				foreach (var definition in package.Rooms.Where(x => x.ZoneKey == zoneKey)
					         .OrderByDescending(x => x.Key == zone.DefaultRoomKey).ThenBy(x => x.Key))
					gameworld.Add(new Room(dbRooms[definition.Key], runtimeZone));
			}

			foreach (var area in package.Areas)
			{
				var runtimeArea = new Area(dbAreas[area.Key], gameworld);
				gameworld.Add(runtimeArea);
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
				RoomCount = package.Rooms.Count,
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
					RoomCount = package.Rooms.Count,
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

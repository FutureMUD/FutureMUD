#nullable enable

using System.IO;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Framework;

namespace MudSharp.Construction.ImportExport;

public sealed partial class SpatialAreaTransferService
{
	public SpatialAreaTransferResult ExportZones(
		IReadOnlyCollection<IZone> zones,
		string packageFileName)
	{
		var diagnostics = new List<SpatialAreaTransferDiagnostic>();
		var omissions = new List<SpatialPackageOmission>();
		var selectedZones = zones
			.DistinctBy(x => x.Id)
			.OrderBy(x => x.Id)
			.ToList();
		if (selectedZones.Count == 0)
		{
			return Failure("You must select at least one zone to export.", diagnostics, "no-zones");
		}

		if (!TryResolvePackagePath(PackageDirectory, packageFileName, out var packagePath, out var pathError))
		{
			return Failure(pathError, diagnostics, "invalid-package-name");
		}

		if (File.Exists(packagePath))
		{
			return Failure(
				$"A package named '{Path.GetFileName(packagePath)}' already exists. Export never overwrites an existing package.",
				diagnostics,
				"package-exists");
		}

		var allRooms = selectedZones.SelectMany(x => x.Rooms).DistinctBy(x => x.Id).OrderBy(x => x.Id).ToList();
		var temporaryRooms = allRooms.Where(x => x.Temporary).ToList();
		foreach (var temporaryRoom in temporaryRooms)
		{
			omissions.Add(new SpatialPackageOmission
			{
				Code = "temporary-cell",
				Message = $"Temporary cell #{temporaryRoom.Id:N0} ({temporaryRoom.Name}) was skipped because dwelling and other temporary state is not portable."
			});
		}

		var rooms = allRooms.Where(x => !x.Temporary).ToList();
		if (rooms.Count == 0)
			return Failure("The selected zones do not contain any exportable cells.", diagnostics, "empty-zone");
		if (rooms.Count > SpatialAreaPackageSerializer.MaximumRooms)
			return Failure("The selected zones exceed the package safety limits.", diagnostics, "zone-too-large");

		foreach (var zone in selectedZones.Where(x => x.DefaultRoom is null || !rooms.Any(room => room.Id == x.DefaultRoom.Id)))
		{
			diagnostics.Add(Error("unexportable-default-cell",
				$"Zone '{zone.Name}' has default cell #{zone.DefaultRoom?.Id:N0}, which cannot be exported."));
		}

		diagnostics.AddRange(ValidateExportableRooms(rooms));
		if (diagnostics.Any(x => x.Severity == SpatialAreaTransferDiagnosticSeverity.Error))
		{
			return new SpatialAreaTransferResult
			{
				Summary = "The zones were not exported because they contain unsupported spatial state.",
				Diagnostics = diagnostics,
				ZoneCount = selectedZones.Count,
				RoomCount = rooms.Count,
				OmittedItems = omissions.Select(x => x.Message).ToList()
			};
		}

		var zoneKeys = selectedZones
			.Select((zone, index) => (zone.Id, Key: $"zone-{index + 1:D5}"))
			.ToDictionary(x => x.Id, x => x.Key);
		var cellKeys = rooms
			.Select((room, index) => (room.Id, Key: $"cell-{index + 1:D5}"))
			.ToDictionary(x => x.Id, x => x.Key);
		var cellIds = cellKeys.Keys.ToHashSet();
		var areas = rooms
			.SelectMany(x => x.OwningAreas)
			.DistinctBy(x => x.Id)
			.OrderBy(x => x.Id)
			.Where(area => area.Rooms.All(room => cellKeys.ContainsKey(room.Id)))
			.ToList();
		foreach (var area in rooms
			         .SelectMany(x => x.OwningAreas)
			         .DistinctBy(x => x.Id)
			         .Except(areas))
		{
			omissions.Add(new SpatialPackageOmission
			{
				Code = "partial-area-membership",
				Message = $"Area group '{area.Name}' was skipped because it also contains rooms outside the package."
			});
		}

		var allSeenExits = rooms
			.SelectMany(room => room.Gameworld.ExitManager.GetExitsFor(room, room.CurrentOverlay))
			.Select(x => x.Exit)
			.DistinctBy(x => x.Id)
			.OrderBy(x => x.Id)
			.ToList();
		var missingExitIds = rooms
			.SelectMany(x => x.CurrentOverlay.ExitIDs)
			.Distinct()
			.Except(allSeenExits.Select(x => x.Id))
			.ToList();
		foreach (var missingExitId in missingExitIds)
		{
			diagnostics.Add(Error("missing-source-exit",
				$"An active overlay references exit #{missingExitId:N0}, but that exit could not be loaded from the source database."));
		}

		if (missingExitIds.Count > 0)
		{
			return new SpatialAreaTransferResult
			{
				Summary = "The zones were not exported because their active topology contains invalid exit references.",
				Diagnostics = diagnostics,
				ZoneCount = selectedZones.Count,
				RoomCount = rooms.Count,
				OmittedItems = omissions.Select(x => x.Message).ToList()
			};
		}

		var internalExits = allSeenExits
			.Where(x => x.Rooms.All(room => cellIds.Contains(room.Id)))
			.ToList();
		var boundaryExits = allSeenExits
			.Where(x => x.Rooms.Any(room => !cellIds.Contains(room.Id)))
			.ToList();
		if (internalExits.Count > SpatialAreaPackageSerializer.MaximumExits)
		{
			return Failure("The selected zones exceed the package exit safety limit.", diagnostics, "zone-too-large");
		}

		foreach (var exit in boundaryExits)
		{
			foreach (var origin in exit.Rooms.Where(x => cellIds.Contains(x.Id)))
			{
				var destination = exit.Rooms.First(x => x.Id != origin.Id);
				var side = exit.RoomExitFor(origin);
				var name = side is INonCardinalRoomExit nonCardinal
					? nonCardinal.Verb
					: side.OutboundDirection.DescribeEnum().ToLowerInvariant();
				var reason = destination.Temporary
					? "the destination cell is temporary."
					: $"destination zone '{destination.Zone.Name}' was not selected.";
				var message = $"Exit \"{name}\" from cell #{origin.Id:N0} ({origin.Name}) to cell " +
				              $"#{destination.Id:N0} ({destination.Name}) was skipped because {reason}";
				omissions.Add(new SpatialPackageOmission { Code = "boundary-exit", Message = message });
			}
		}

		foreach (var exit in internalExits)
		{
			if (exit.Door is not null)
			{
				omissions.Add(new SpatialPackageOmission
				{
					Code = "installed-door",
					Message = $"Installed door item on exit #{exit.Id:N0} was skipped. The imported exit will retain its door capability but have no door item."
				});
			}

			if (exit.FallRoom is not null && !cellIds.Contains(exit.FallRoom.Id))
			{
				diagnostics.Add(Error("external-fall-cell",
					$"Exit #{exit.Id:N0} falls to cell #{exit.FallRoom.Id:N0}, which is outside the package."));
			}
		}

		if (diagnostics.Any(x => x.Severity == SpatialAreaTransferDiagnosticSeverity.Error))
		{
			return new SpatialAreaTransferResult
			{
				Summary = "The zones were not exported because one or more exits require unsupported dependencies.",
				Diagnostics = diagnostics,
				ZoneCount = selectedZones.Count,
				RoomCount = rooms.Count,
				ExitCount = internalExits.Count,
				OmittedItems = omissions.Select(x => x.Message).ToList()
			};
		}

		AddNonSpatialOmissions(rooms, omissions);
		var exitKeys = internalExits
			.Select((exit, index) => (exit.Id, Key: $"exit-{index + 1:D5}"))
			.ToDictionary(x => x.Id, x => x.Key);
		var package = BuildPackageVersion4(
			selectedZones,
			rooms,
			internalExits,
			areas,
			zoneKeys,
			cellKeys,
			exitKeys,
			omissions,
			diagnostics);
		var json = SpatialAreaPackageSerializer.Serialize(package);
		if (Encoding.UTF8.GetByteCount(json) > SpatialAreaPackageSerializer.MaximumPackageBytes)
		{
			return Failure("The serialized package exceeds the 16 MiB safety limit.", diagnostics, "package-too-large");
		}

		try
		{
			Directory.CreateDirectory(PackageDirectory);
			using var stream = new FileStream(packagePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
			using var writer = new StreamWriter(stream, new UTF8Encoding(false));
			writer.Write(json);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			return Failure($"The package could not be written: {ex.Message}", diagnostics, "package-write-failed");
		}

		return new SpatialAreaTransferResult
		{
			Success = true,
			Summary = $"Exported {selectedZones.Count:N0} zone(s) as package version {SpatialAreaPackage.CurrentVersion:N0}.",
			PackagePath = packagePath,
			Diagnostics = diagnostics,
			ZoneCount = selectedZones.Count,
			RoomCount = rooms.Count,
			ExitCount = internalExits.Count,
			OmittedItems = omissions.Select(x => x.Message).ToList()
		};
	}

	private static SpatialAreaPackage BuildPackageVersion4(
		IReadOnlyList<IZone> zones,
		IReadOnlyList<IRoom> rooms,
		IReadOnlyList<IExit> exits,
		IReadOnlyList<IArea> areas,
		IReadOnlyDictionary<long, string> zoneKeys,
		IReadOnlyDictionary<long, string> cellKeys,
		IReadOnlyDictionary<long, string> exitKeys,
		IReadOnlyList<SpatialPackageOmission> omissions,
		ICollection<SpatialAreaTransferDiagnostic> diagnostics)
	{
		var overlayPackages = rooms
			.Select(x => x.CurrentOverlay.Package)
			.Distinct()
			.ToList();
		if (overlayPackages.Count > 1)
		{
			diagnostics.Add(Warning("mixed-overlays",
				$"The selection uses {overlayPackages.Count:N0} different active overlay packages. Each cell's active overlay data will be imported into the selected target package."));
		}

		var sources = zones
			.Select(zone => new SpatialAreaPackageSource
			{
				ZoneName = zone.Name,
				ZoneId = zone.Id,
				ShardName = zone.Shard.Name,
				ShardId = zone.Shard.Id,
				OverlayPackageName = overlayPackages.Count == 1 ? overlayPackages[0].Name : "Mixed Active Overlays",
				OverlayPackageId = overlayPackages.Count == 1 ? overlayPackages[0].Id : 0,
				OverlayPackageRevision = overlayPackages.Count == 1 ? overlayPackages[0].RevisionNumber : 0
			})
			.ToList();
		var zoneDefinitions = zones
			.Select(zone => new SpatialZoneDefinition
			{
				Key = zoneKeys[zone.Id],
				SourceId = zone.Id,
				Name = zone.Name,
				LatitudeRadians = zone.Geography.Latitude,
				LongitudeRadians = zone.Geography.Longitude,
				ElevationMetres = zone.Geography.Elevation,
				AmbientLightPollution = zone.AmbientLightPollution,
				ForagableProfile = Reference(zone.ForagableProfile),
				WeatherController = Reference(zone.WeatherController),
				DefaultRoomKey = cellKeys[zone.DefaultRoom.Id],
				TimeZones = zone.GetEditableZone.TimeZones
					.OrderBy(x => x.Key.Alias)
					.Select(x => new SpatialTimeZoneDefinition
					{
						ClockAlias = x.Key.Alias,
						TimeZoneAlias = x.Value.Alias,
						TimeZoneDescription = x.Value.Description
					})
					.ToList()
			})
			.ToList();

		var explicitForagableProfiles = new Dictionary<long, long?>();
		var routeRooms = new Dictionary<long, Models.RouteRoom>();
		using (new FMDB())
		{
			foreach (var dbRoom in FMDB.Context.Rooms
				         .Where(x => cellKeys.Keys.Contains(x.Id))
				         .Select(x => new { x.Id, x.ForagableProfileId }))
			{
				explicitForagableProfiles[dbRoom.Id] = dbRoom.ForagableProfileId;
			}

			routeRooms = FMDB.Context.RouteRooms
				.AsNoTracking()
				.Where(x => cellKeys.Keys.Contains(x.RoomId))
				.Include(x => x.Landmarks)
				.Include(x => x.ExitAnchors)
				.ToDictionary(x => x.RoomId);
		}

		var package = new SpatialAreaPackage
		{
			Version = SpatialAreaPackage.CurrentVersion,
			CreatedUtc = DateTime.UtcNow,
			Source = sources[0],
			SourceZones = sources,
			Zones = zoneDefinitions,
			Areas = areas
				.Select(area => new SpatialAreaDefinition
				{
					Key = $"area-{area.Id:D5}",
					SourceId = area.Id,
					Name = area.Name,
					WeatherController = Reference(area.WeatherController),
					RoomKeys = area.Rooms
						.OrderBy(x => x.Id)
						.Select(x => cellKeys[x.Id])
						.ToList()
				})
				.ToList(),
			Omissions = omissions.ToList()
		};

		package.Rooms = rooms
			.OrderBy(x => x.Id)
			.Select(room =>
			{
				var overlay = room.CurrentOverlay;
				var explicitForagable = explicitForagableProfiles.GetValueOrDefault(room.Id);
				return new SpatialRoomDefinition
				{
					Key = cellKeys[room.Id],
					SourceId = room.Id,
					ZoneKey = zoneKeys[room.OwningZone.Id],
					X = room.StoredCoordinates.X, Y = room.StoredCoordinates.Y, Z = room.StoredCoordinates.Z,
					ForagableProfile = explicitForagable.HasValue
						? Reference(room.Gameworld.ForagableProfiles.Get(explicitForagable.Value))
						: null,
					Tags = room.Tags.OrderBy(x => x.Name).Select(x => Reference(x)!).ToList(),
					RangedCovers = room.LocalCover.OrderBy(x => x.Name).Select(x => Reference(x)!).ToList(),
					MagicResources = room.MagicResourceAmounts
						.OrderBy(x => x.Key.Name)
						.Select(x => new SpatialMagicResourceDefinition
						{
							Resource = Reference(x.Key)!,
							Amount = x.Value
						})
						.ToList(),
					RouteRoom = routeRooms.TryGetValue(room.Id, out var route)
						? BuildRouteRoomDefinition(route, exitKeys, overlay.ExitIDs)
						: null,
					Overlay = new SpatialRoomOverlayDefinition
					{
						RoomName = overlay.RoomName,
						RoomDescription = overlay.RoomDescription,
						Terrain = Reference(overlay.Terrain)!,
						HearingProfile = Reference(overlay.HearingProfile),
						Atmosphere = FluidReference(overlay.Atmosphere),
						OutdoorsType = (int)overlay.OutdoorsType,
						AmbientLightFactor = overlay.AmbientLightFactor,
						AddedLight = overlay.AddedLight,
						SafeQuit = overlay.SafeQuit,
						ExitKeys = overlay.ExitIDs
							.Where(exitKeys.ContainsKey)
							.Select(x => exitKeys[x])
							.Order()
							.ToList()
					}
				};
			})
			.ToList();

		package.Exits = exits
			.Select(exit =>
			{
				var endpoints = exit.Rooms.ToList();
				return new SpatialExitDefinition
				{
					Key = exitKeys[exit.Id],
					SourceId = exit.Id,
					Room1Key = cellKeys[endpoints[0].Id],
					Room2Key = cellKeys[endpoints[1].Id],
					Side1 = BuildExitSide(exit.RoomExitFor(endpoints[0])),
					Side2 = BuildExitSide(exit.RoomExitFor(endpoints[1])),
					TimeMultiplier = exit.TimeMultiplier,
					AcceptsDoor = exit.AcceptsDoor,
					DoorSize = (int)exit.DoorSize,
					MaximumSizeToEnter = (int)exit.MaximumSizeToEnter,
					MaximumSizeToEnterUpright = (int)exit.MaximumSizeToEnterUpright,
					IsClimbExit = exit.IsClimbExit,
					ClimbDifficulty = (int)exit.ClimbDifficulty,
					FallRoomKey = exit.FallRoom is null ? null : cellKeys[exit.FallRoom.Id],
					BlockedLayers = exit.BlockedLayers.Select(x => (int)x).Order().ToList()
				};
			})
			.ToList();
		return package;
	}

	private static SpatialRouteRoomDefinition BuildRouteRoomDefinition(
		Models.RouteRoom route,
		IReadOnlyDictionary<long, string> exitKeys,
		IEnumerable<long> activeExitIds)
	{
		var activeExitIdSet = activeExitIds.ToHashSet();
		return new SpatialRouteRoomDefinition
		{
			LengthMetres = (double)route.LengthMetres,
			DefaultPositionMetres = (double)route.DefaultPositionMetres,
			PositiveDirectionName = route.PositiveDirectionName,
			NegativeDirectionName = route.NegativeDirectionName,
			MetresPerRoomEquivalent = (double)route.MetresPerRoomEquivalent,
			TopologyVersion = route.TopologyVersion,
			Landmarks = route.Landmarks
				.OrderBy(x => x.DisplayOrder)
				.ThenBy(x => x.PositionMetres)
				.ThenBy(x => x.Id)
				.Select(x => new SpatialRouteLandmarkDefinition
				{
					SourceId = x.Id,
					Name = x.Name,
					Keywords = x.Keywords,
					Description = x.Description,
					PositionMetres = (double)x.PositionMetres,
					DisplayOrder = x.DisplayOrder
				})
				.ToList(),
			ExitAnchors = route.ExitAnchors
				.Where(x => exitKeys.ContainsKey(x.ExitId) && activeExitIdSet.Contains(x.ExitId))
				.OrderBy(x => x.ExitId)
				.Select(x => new SpatialRouteExitAnchorDefinition
				{
					ExitKey = exitKeys[x.ExitId],
					MinimumPositionMetres = (double)x.MinimumPositionMetres,
					MaximumPositionMetres = (double)x.MaximumPositionMetres,
					ArrivalPositionMetres = (double)x.ArrivalPositionMetres
				})
				.ToList()
		};
	}

	private static void AddNonSpatialOmissions(
		IReadOnlyCollection<IRoom> rooms,
		ICollection<SpatialPackageOmission> omissions)
	{
		foreach (var room in rooms)
		{
			var characterCount = room.Characters.Count();
			var itemCount = room.GameItems.Count();
			if (characterCount > 0 || itemCount > 0)
			{
				omissions.Add(new SpatialPackageOmission
				{
					Code = "live-contents",
					Message = $"Cell #{room.Id:N0} ({room.Name}) contains {characterCount:N0} character(s) and " +
					          $"{itemCount:N0} item(s); live contents are not included."
				});
			}

			var hookCount = room.Hooks.Count();
			if (hookCount > 0)
			{
				omissions.Add(new SpatialPackageOmission
				{
					Code = "cell-hooks",
					Message = $"Cell #{room.Id:N0} ({room.Name}) has {hookCount:N0} installed hook(s), which are not included."
				});
			}
		}
	}
}

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

		var allRooms = selectedZones
			.SelectMany(x => x.Rooms)
			.DistinctBy(x => x.Id)
			.OrderBy(x => x.Id)
			.ToList();
		var temporaryCells = allRooms
			.SelectMany(x => x.Cells)
			.Where(x => x.Temporary)
			.DistinctBy(x => x.Id)
			.OrderBy(x => x.Id)
			.ToList();
		foreach (var temporaryCell in temporaryCells)
		{
			omissions.Add(new SpatialPackageOmission
			{
				Code = "temporary-cell",
				Message = $"Temporary cell #{temporaryCell.Id:N0} ({temporaryCell.Name}) was skipped because dwelling and other temporary state is not portable."
			});
		}

		var rooms = allRooms
			.Where(x => x.Cells.Any(cell => !cell.Temporary))
			.ToList();
		foreach (var emptyRoom in allRooms.Except(rooms))
		{
			if (emptyRoom.Cells.Any())
			{
				continue;
			}

			var omission = $"Room #{emptyRoom.Id:N0} at ({emptyRoom.X:N0}, {emptyRoom.Y:N0}, {emptyRoom.Z:N0}) " +
			               $"in zone '{emptyRoom.Zone.Name}' was skipped because it contains no cells.";
			omissions.Add(new SpatialPackageOmission { Code = "empty-room", Message = omission });
		}

		var cells = rooms
			.SelectMany(x => x.Cells)
			.Where(x => !x.Temporary)
			.DistinctBy(x => x.Id)
			.OrderBy(x => x.Id)
			.ToList();
		if (rooms.Count == 0 || cells.Count == 0)
		{
			return Failure("The selected zones do not contain any exportable rooms and cells.", diagnostics, "empty-zone");
		}

		if (rooms.Count > SpatialAreaPackageSerializer.MaximumRooms ||
		    cells.Count > SpatialAreaPackageSerializer.MaximumCells)
		{
			return Failure("The selected zones exceed the package safety limits.", diagnostics, "zone-too-large");
		}

		foreach (var zone in selectedZones.Where(x => !cells.Any(cell => cell.Id == x.DefaultCell.Id)))
		{
			diagnostics.Add(Error("unexportable-default-cell",
				$"Zone '{zone.Name}' has default cell #{zone.DefaultCell.Id:N0}, which cannot be exported."));
		}

		diagnostics.AddRange(ValidateExportableCells(cells));
		if (diagnostics.Any(x => x.Severity == SpatialAreaTransferDiagnosticSeverity.Error))
		{
			return new SpatialAreaTransferResult
			{
				Summary = "The zones were not exported because they contain unsupported spatial state.",
				Diagnostics = diagnostics,
				ZoneCount = selectedZones.Count,
				RoomCount = rooms.Count,
				CellCount = cells.Count,
				OmittedItems = omissions.Select(x => x.Message).ToList()
			};
		}

		var zoneKeys = selectedZones
			.Select((zone, index) => (zone.Id, Key: $"zone-{index + 1:D5}"))
			.ToDictionary(x => x.Id, x => x.Key);
		var roomKeys = rooms
			.Select((room, index) => (room.Id, Key: $"room-{index + 1:D5}"))
			.ToDictionary(x => x.Id, x => x.Key);
		var cellKeys = cells
			.Select((cell, index) => (cell.Id, Key: $"cell-{index + 1:D5}"))
			.ToDictionary(x => x.Id, x => x.Key);
		var cellIds = cellKeys.Keys.ToHashSet();
		var areas = cells
			.SelectMany(x => x.Areas)
			.DistinctBy(x => x.Id)
			.OrderBy(x => x.Id)
			.Where(area => area.Rooms.All(room => roomKeys.ContainsKey(room.Id)))
			.ToList();
		foreach (var area in cells
			         .SelectMany(x => x.Areas)
			         .DistinctBy(x => x.Id)
			         .Except(areas))
		{
			omissions.Add(new SpatialPackageOmission
			{
				Code = "partial-area-membership",
				Message = $"Area group '{area.Name}' was skipped because it also contains rooms outside the package."
			});
		}

		var allSeenExits = cells
			.SelectMany(cell => cell.Gameworld.ExitManager.GetExitsFor(cell, cell.CurrentOverlay))
			.Select(x => x.Exit)
			.DistinctBy(x => x.Id)
			.OrderBy(x => x.Id)
			.ToList();
		var missingExitIds = cells
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
				CellCount = cells.Count,
				OmittedItems = omissions.Select(x => x.Message).ToList()
			};
		}

		var internalExits = allSeenExits
			.Where(x => x.Cells.All(cell => cellIds.Contains(cell.Id)))
			.ToList();
		var boundaryExits = allSeenExits
			.Where(x => x.Cells.Any(cell => !cellIds.Contains(cell.Id)))
			.ToList();
		if (internalExits.Count > SpatialAreaPackageSerializer.MaximumExits)
		{
			return Failure("The selected zones exceed the package exit safety limit.", diagnostics, "zone-too-large");
		}

		foreach (var exit in boundaryExits)
		{
			foreach (var origin in exit.Cells.Where(x => cellIds.Contains(x.Id)))
			{
				var destination = exit.Cells.First(x => x.Id != origin.Id);
				var side = exit.CellExitFor(origin);
				var name = side is INonCardinalCellExit nonCardinal
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

			if (exit.FallCell is not null && !cellIds.Contains(exit.FallCell.Id))
			{
				diagnostics.Add(Error("external-fall-cell",
					$"Exit #{exit.Id:N0} falls to cell #{exit.FallCell.Id:N0}, which is outside the package."));
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
				CellCount = cells.Count,
				ExitCount = internalExits.Count,
				OmittedItems = omissions.Select(x => x.Message).ToList()
			};
		}

		AddNonSpatialOmissions(cells, omissions);
		var exitKeys = internalExits
			.Select((exit, index) => (exit.Id, Key: $"exit-{index + 1:D5}"))
			.ToDictionary(x => x.Id, x => x.Key);
		var package = BuildPackageVersion2(
			selectedZones,
			rooms,
			cells,
			internalExits,
			areas,
			zoneKeys,
			roomKeys,
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
			CellCount = cells.Count,
			ExitCount = internalExits.Count,
			OmittedItems = omissions.Select(x => x.Message).ToList()
		};
	}

	private static SpatialAreaPackage BuildPackageVersion2(
		IReadOnlyList<IZone> zones,
		IReadOnlyList<IRoom> rooms,
		IReadOnlyList<ICell> cells,
		IReadOnlyList<IExit> exits,
		IReadOnlyList<IArea> areas,
		IReadOnlyDictionary<long, string> zoneKeys,
		IReadOnlyDictionary<long, string> roomKeys,
		IReadOnlyDictionary<long, string> cellKeys,
		IReadOnlyDictionary<long, string> exitKeys,
		IReadOnlyList<SpatialPackageOmission> omissions,
		ICollection<SpatialAreaTransferDiagnostic> diagnostics)
	{
		var overlayPackages = cells
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
				DefaultCellKey = cellKeys[zone.DefaultCell.Id],
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
		var routeCells = new Dictionary<long, Models.RouteCell>();
		using (new FMDB())
		{
			foreach (var dbCell in FMDB.Context.Cells
				         .Where(x => cellKeys.Keys.Contains(x.Id))
				         .Select(x => new { x.Id, x.ForagableProfileId }))
			{
				explicitForagableProfiles[dbCell.Id] = dbCell.ForagableProfileId;
			}

			routeCells = FMDB.Context.RouteCells
				.AsNoTracking()
				.Where(x => cellKeys.Keys.Contains(x.CellId))
				.Include(x => x.Landmarks)
				.Include(x => x.ExitAnchors)
				.ToDictionary(x => x.CellId);
		}

		var package = new SpatialAreaPackage
		{
			Version = SpatialAreaPackage.CurrentVersion,
			CreatedUtc = DateTime.UtcNow,
			Source = sources[0],
			Zone = zoneDefinitions[0],
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
						.Select(x => roomKeys[x.Id])
						.ToList()
				})
				.ToList(),
			Omissions = omissions.ToList(),
			Rooms = rooms
				.OrderBy(x => x.Id)
				.Select(x => new SpatialRoomDefinition
				{
					Key = roomKeys[x.Id],
					SourceId = x.Id,
					ZoneKey = zoneKeys[x.Zone.Id],
					X = x.X,
					Y = x.Y,
					Z = x.Z
				})
				.ToList()
		};

		package.Cells = cells
			.OrderBy(x => x.Id)
			.Select(cell =>
			{
				var overlay = cell.CurrentOverlay;
				var explicitForagable = explicitForagableProfiles.GetValueOrDefault(cell.Id);
				return new SpatialCellDefinition
				{
					Key = cellKeys[cell.Id],
					SourceId = cell.Id,
					RoomKey = roomKeys[cell.Room.Id],
					ForagableProfile = explicitForagable.HasValue
						? Reference(cell.Gameworld.ForagableProfiles.Get(explicitForagable.Value))
						: null,
					Tags = cell.Tags.OrderBy(x => x.Name).Select(x => Reference(x)!).ToList(),
					RangedCovers = cell.LocalCover.OrderBy(x => x.Name).Select(x => Reference(x)!).ToList(),
					MagicResources = cell.MagicResourceAmounts
						.OrderBy(x => x.Key.Name)
						.Select(x => new SpatialMagicResourceDefinition
						{
							Resource = Reference(x.Key)!,
							Amount = x.Value
						})
						.ToList(),
					RouteCell = routeCells.TryGetValue(cell.Id, out var route)
						? BuildRouteCellDefinition(route, exitKeys, overlay.ExitIDs)
						: null,
					Overlay = new SpatialCellOverlayDefinition
					{
						CellName = overlay.CellName,
						CellDescription = overlay.CellDescription,
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
				var endpoints = exit.Cells.ToList();
				return new SpatialExitDefinition
				{
					Key = exitKeys[exit.Id],
					SourceId = exit.Id,
					Cell1Key = cellKeys[endpoints[0].Id],
					Cell2Key = cellKeys[endpoints[1].Id],
					Side1 = BuildExitSide(exit.CellExitFor(endpoints[0])),
					Side2 = BuildExitSide(exit.CellExitFor(endpoints[1])),
					TimeMultiplier = exit.TimeMultiplier,
					AcceptsDoor = exit.AcceptsDoor,
					DoorSize = (int)exit.DoorSize,
					MaximumSizeToEnter = (int)exit.MaximumSizeToEnter,
					MaximumSizeToEnterUpright = (int)exit.MaximumSizeToEnterUpright,
					IsClimbExit = exit.IsClimbExit,
					ClimbDifficulty = (int)exit.ClimbDifficulty,
					FallCellKey = exit.FallCell is null ? null : cellKeys[exit.FallCell.Id],
					BlockedLayers = exit.BlockedLayers.Select(x => (int)x).Order().ToList()
				};
			})
			.ToList();
		return package;
	}

	private static SpatialRouteCellDefinition BuildRouteCellDefinition(
		Models.RouteCell route,
		IReadOnlyDictionary<long, string> exitKeys,
		IEnumerable<long> activeExitIds)
	{
		var activeExitIdSet = activeExitIds.ToHashSet();
		return new SpatialRouteCellDefinition
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
		IReadOnlyCollection<ICell> cells,
		ICollection<SpatialPackageOmission> omissions)
	{
		foreach (var cell in cells)
		{
			var characterCount = cell.Characters.Count();
			var itemCount = cell.GameItems.Count();
			if (characterCount > 0 || itemCount > 0)
			{
				omissions.Add(new SpatialPackageOmission
				{
					Code = "live-contents",
					Message = $"Cell #{cell.Id:N0} ({cell.Name}) contains {characterCount:N0} character(s) and " +
					          $"{itemCount:N0} item(s); live contents are not included."
				});
			}

			var hookCount = cell.Hooks.Count();
			if (hookCount > 0)
			{
				omissions.Add(new SpatialPackageOmission
				{
					Code = "cell-hooks",
					Message = $"Cell #{cell.Id:N0} ({cell.Name}) has {hookCount:N0} installed hook(s), which are not included."
				});
			}
		}
	}
}

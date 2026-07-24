#nullable enable

using System.Data;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Combat;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate.Time;
using MudSharp.Work.Foraging;

namespace MudSharp.Construction.ImportExport;

public sealed partial class SpatialAreaTransferService : ISpatialAreaTransferService
{
	private sealed class ImportPreflight
	{
		public required SpatialAreaPackage Package { get; init; }
		public required string PackagePath { get; init; }
		public required string ZoneName { get; init; }
		public required IReadOnlyList<SpatialZoneDefinition> Zones { get; init; }
		public required IReadOnlyDictionary<string, string> ZoneNames { get; init; }
		public required ICellOverlayPackage OverlayPackage { get; init; }
		public required IReadOnlyDictionary<string, ITerrain> Terrains { get; init; }
		public required IReadOnlyDictionary<string, IHearingProfile> HearingProfiles { get; init; }
		public required IReadOnlyDictionary<string, IFluid> Fluids { get; init; }
		public required IReadOnlyDictionary<string, IForagableProfile> ForagableProfiles { get; init; }
		public required IReadOnlyDictionary<string, ITag> Tags { get; init; }
		public required IReadOnlyDictionary<string, IRangedCover> RangedCovers { get; init; }
		public required IReadOnlyDictionary<string, IMagicResource> MagicResources { get; init; }
		public required IReadOnlyDictionary<string, (IClock Clock, IMudTimeZone TimeZone)> TimeZones { get; init; }
		public required IReadOnlyDictionary<string,
			IReadOnlyDictionary<string, (IClock Clock, IMudTimeZone TimeZone)>> TimeZonesByZone { get; init; }
		public required IReadOnlyDictionary<string, IWeatherController?> WeatherControllers { get; init; }
		public IWeatherController? WeatherController { get; init; }
		public List<SpatialAreaTransferDiagnostic> Diagnostics { get; } = [];
	}

	public static SpatialAreaTransferService Instance { get; } = new();

	private SpatialAreaTransferService()
	{
	}

	public string PackageDirectory => Path.Combine(Directory.GetCurrentDirectory(), "Spatial Packages");

	public SpatialAreaTransferResult ExportZone(IZone zone, string packageFileName)
	{
		return ExportZones([zone], packageFileName);
	}

	public SpatialAreaTransferResult ValidateImport(
		ICharacter actor,
		IShard targetShard,
		string packageFileName,
		string? zoneNameOverride = null)
	{
		var preflight = PreflightImport(
			actor,
			targetShard,
			packageFileName,
			zoneNameOverride,
			out var failure);
		if (preflight is null)
		{
			return failure!;
		}

		var zoneNames = preflight.ZoneNames.Values.ToList();
		return new SpatialAreaTransferResult
		{
			Success = true,
			Summary =
				$"Package validation succeeded. Import will create {zoneNames.Count:N0} new zone(s): {zoneNames.ListToString()}. Existing rooms and zones will not be modified.",
			PackagePath = preflight.PackagePath,
			Diagnostics = preflight.Diagnostics,
			ZoneCount = zoneNames.Count,
			RoomCount = PackagedRoomCount(preflight.Package),
			CellCount = preflight.Package.Cells.Count,
			ExitCount = preflight.Package.Exits.Count,
			OmittedItems = PackageOmissions(preflight)
		};
	}

	public SpatialAreaTransferResult ImportZone(
		ICharacter actor,
		IShard targetShard,
		string packageFileName,
		string? zoneNameOverride = null)
	{
		var preflight = PreflightImport(
			actor,
			targetShard,
			packageFileName,
			zoneNameOverride,
			out var failure);
		if (preflight is null)
		{
			return failure!;
		}

		if (preflight.Package.Version >= 2)
		{
			return ImportVersion2(actor, targetShard, preflight);
		}

		var package = preflight.Package;
		var gameworld = actor.Gameworld;
		Models.Zone? dbZone = null;
		var dbRooms = new Dictionary<string, Models.Room>(StringComparer.Ordinal);
		var dbCells = new Dictionary<string, Models.Cell>(StringComparer.Ordinal);
		var databaseCommitted = false;
		try
		{
			using (new FMDB())
			using (var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.Serializable))
			{
				if (FMDB.Context.Zones.Any(x => x.Name == preflight.ZoneName))
				{
					return Failure(
						$"A zone named '{preflight.ZoneName}' was created after validation. Nothing was imported.",
						preflight.Diagnostics,
						"zone-name-collision");
				}

				dbZone = new Models.Zone
				{
					Name = preflight.ZoneName,
					ShardId = targetShard.Id,
					Latitude = package.Zone.LatitudeRadians,
					Longitude = package.Zone.LongitudeRadians,
					Elevation = package.Zone.ElevationMetres,
					AmbientLightPollution = package.Zone.AmbientLightPollution,
					ForagableProfileId = package.Zone.ForagableProfile is null
						? null
						: preflight.ForagableProfiles[package.Zone.ForagableProfile.Name].Id,
					WeatherControllerId = preflight.WeatherController?.Id
				};
				FMDB.Context.Zones.Add(dbZone);

				foreach (var resolvedTimeZone in preflight.TimeZones.Values)
				{
					dbZone.ZonesTimezones.Add(new ZonesTimezones
					{
						Zone = dbZone,
						ClockId = resolvedTimeZone.Clock.Id,
						TimezoneId = resolvedTimeZone.TimeZone.Id
					});
				}

				var importedRoomKeys = package.Cells
					.Select(x => x.RoomKey)
					.ToHashSet(StringComparer.Ordinal);
				foreach (var room in package.Rooms.Where(x => importedRoomKeys.Contains(x.Key)))
				{
					var dbRoom = new Models.Room
					{
						Zone = dbZone,
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

				foreach (var cell in package.Cells)
				{
					dbCells[cell.Key].CurrentOverlay = dbOverlays[cell.Key];
					foreach (var tag in cell.Tags)
					{
						dbCells[cell.Key].CellsTags.Add(new CellsTags
						{
							Cell = dbCells[cell.Key],
							TagId = preflight.Tags[tag.Name].Id
						});
					}

					foreach (var cover in cell.RangedCovers)
					{
						dbCells[cell.Key].CellsRangedCovers.Add(new CellsRangedCovers
						{
							Cell = dbCells[cell.Key],
							RangedCoverId = preflight.RangedCovers[cover.Name].Id
						});
					}

					foreach (var resource in cell.MagicResources)
					{
						dbCells[cell.Key].CellsMagicResources.Add(new CellMagicResource
						{
							Cell = dbCells[cell.Key],
							MagicResourceId = preflight.MagicResources[resource.Resource.Name].Id,
							Amount = resource.Amount
						});
					}
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
				}

				dbZone.DefaultCell = dbCells[package.Zone.DefaultCellKey];
				FMDB.Context.SaveChanges();
				transaction.Commit();
				databaseCommitted = true;
			}

			var newZone = new Zone(dbZone, gameworld);
			gameworld.Add(newZone);
			foreach (var roomDefinition in package.Rooms.Where(x => dbRooms.ContainsKey(x.Key)))
			{
				var newRoom = new Room(dbRooms[roomDefinition.Key], newZone);
				gameworld.Add(newRoom);
				foreach (var cellDefinition in package.Cells.Where(x => x.RoomKey == roomDefinition.Key))
				{
					var newCell = new Cell(dbCells[cellDefinition.Key], newRoom);
					gameworld.Add(newCell);
				}
			}

			newZone.PostLoadSetup();

			return new SpatialAreaTransferResult
			{
				Success = true,
				Summary =
					$"Imported package as new zone '{preflight.ZoneName}' (#{newZone.Id:N0}). Existing spatial content was not modified.",
				PackagePath = preflight.PackagePath,
				ImportedZoneId = newZone.Id,
				ImportedZoneIds = [newZone.Id],
				ZoneCount = 1,
				Diagnostics = preflight.Diagnostics,
				RoomCount = PackagedRoomCount(package),
				CellCount = package.Cells.Count,
				ExitCount = package.Exits.Count,
				OmittedItems = PackageOmissions(preflight)
			};
		}
		catch (Exception ex)
		{
			if (databaseCommitted && dbZone is not null)
			{
				var committedDiagnostics = preflight.Diagnostics.ToList();
				committedDiagnostics.Add(Error("runtime-load-failed",
					$"The database import committed as zone #{dbZone.Id:N0}, but the live server could not register it: {ex.Message}"));
				return new SpatialAreaTransferResult
				{
					Summary =
						$"The new zone was persisted as #{dbZone.Id:N0}, but is not fully available in memory. Restart the server before retrying or editing it; do not re-import the package.",
					PackagePath = preflight.PackagePath,
					ImportedZoneId = dbZone.Id,
					ImportedZoneIds = [dbZone.Id],
					ZoneCount = 1,
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

	private ImportPreflight? PreflightImport(
		ICharacter actor,
		IShard targetShard,
		string packageFileName,
		string? zoneNameOverride,
		out SpatialAreaTransferResult? failure)
	{
		failure = null;
		var diagnostics = new List<SpatialAreaTransferDiagnostic>();
		if (actor.CurrentOverlayPackage is null ||
		    actor.CurrentOverlayPackage.Status != RevisionStatus.UnderDesign)
		{
			failure = Failure(
				"You must be editing an under-design cell overlay package before validating or importing a spatial package.",
				diagnostics,
				"overlay-package-required");
			return null;
		}

		if (!TryResolvePackagePath(PackageDirectory, packageFileName, out var packagePath, out var pathError))
		{
			failure = Failure(pathError, diagnostics, "invalid-package-name");
			return null;
		}

		if (!File.Exists(packagePath))
		{
			failure = Failure(
				$"There is no spatial package named '{Path.GetFileName(packagePath)}' in '{PackageDirectory}'.",
				diagnostics,
				"package-not-found");
			return null;
		}

		string json;
		try
		{
			var fileInfo = new FileInfo(packagePath);
			if (fileInfo.Length > SpatialAreaPackageSerializer.MaximumPackageBytes)
			{
				failure = Failure("The package exceeds the 16 MiB safety limit.", diagnostics,
					"package-too-large");
				return null;
			}

			json = File.ReadAllText(packagePath, Encoding.UTF8);
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
		{
			failure = Failure($"The package could not be read: {ex.Message}", diagnostics,
				"package-read-failed");
			return null;
		}

		var readResult = SpatialAreaPackageSerializer.Deserialize(json);
		diagnostics.AddRange(readResult.Diagnostics);
		if (!readResult.Success || readResult.Package is null)
		{
			failure = new SpatialAreaTransferResult
			{
				Summary = "Package validation failed.",
				PackagePath = packagePath,
				Diagnostics = diagnostics
			};
			return null;
		}

		var package = readResult.Package;
		var zones = SpatialAreaPackageSerializer.GetZoneDefinitions(package);
		if (!string.IsNullOrWhiteSpace(zoneNameOverride) && zones.Count != 1)
		{
			diagnostics.Add(Error("multi-zone-name-override",
				"A zone-name override can only be used with a single-zone package."));
		}

		var zoneNames = new Dictionary<string, string>(StringComparer.Ordinal);
		foreach (var (zone, index) in zones.Select((value, index) => (value, index)))
		{
			var key = SpatialAreaPackageSerializer.ZoneKey(package, zone, index);
			var name = zones.Count == 1 && !string.IsNullOrWhiteSpace(zoneNameOverride)
				? zoneNameOverride.Trim().TitleCase()
				: zone.Name;
			zoneNames[key] = name;
			if (string.IsNullOrWhiteSpace(name))
			{
				diagnostics.Add(Error("invalid-zone-name", $"Imported zone '{key}' must have a non-empty name."));
			}

			if (actor.Gameworld.Zones.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
			{
				diagnostics.Add(Error("zone-name-collision",
					$"A zone named '{name}' already exists. Imports never merge into or overwrite an existing zone."));
			}

			ValidateZoneNumbers(zone, diagnostics);
		}

		var duplicateTargetName = zoneNames.Values
			.GroupBy(x => x, StringComparer.InvariantCultureIgnoreCase)
			.FirstOrDefault(x => x.Count() > 1);
		if (duplicateTargetName is not null)
		{
			diagnostics.Add(Error("duplicate-target-zone-name",
				$"More than one packaged zone would be imported as '{duplicateTargetName.Key}'."));
		}

		ValidateEnums(package, diagnostics);

		var terrains = ResolveReferences(
			package.Cells.Select(x => x.Overlay.Terrain),
			actor.Gameworld.Terrains,
			"terrain",
			diagnostics);
		var hearingProfiles = ResolveReferences(
			package.Cells
				.Select(x => x.Overlay.HearingProfile)
				.Where(x => x is not null)
				.Select(x => x!),
			actor.Gameworld.HearingProfiles,
			"hearing-profile",
			diagnostics);
		var foragableProfiles = ResolveReferences(
			package.Cells.Select(x => x.ForagableProfile)
				.Concat(zones.Select(x => x.ForagableProfile))
				.Where(x => x is not null)
				.Select(x => x!),
			actor.Gameworld.ForagableProfiles,
			"foragable-profile",
			diagnostics);
		var tags = ResolveReferences(
			package.Cells.SelectMany(x => x.Tags),
			actor.Gameworld.Tags,
			"tag",
			diagnostics);
		var covers = ResolveReferences(
			package.Cells.SelectMany(x => x.RangedCovers),
			actor.Gameworld.RangedCovers,
			"ranged-cover",
			diagnostics);
		var magicResources = ResolveReferences(
			package.Cells.SelectMany(x => x.MagicResources).Select(x => x.Resource),
			actor.Gameworld.MagicResources,
			"magic-resource",
			diagnostics);

		var fluids = new Dictionary<string, IFluid>(StringComparer.InvariantCultureIgnoreCase);
		foreach (var fluidReference in package.Cells
			         .Select(x => x.Overlay.Atmosphere)
			         .Where(x => x is not null)
			         .Select(x => x!)
			         .DistinctBy(FluidKey))
		{
			IFluid? fluid = fluidReference.Kind.ToLowerInvariant() switch
			{
				"liquid" => actor.Gameworld.Liquids.GetByName(fluidReference.Name),
				"gas" => actor.Gameworld.Gases.GetByName(fluidReference.Name),
				_ => null
			};
			if (fluid is null)
			{
				diagnostics.Add(Error("missing-fluid",
					$"Required {fluidReference.Kind} atmosphere '{fluidReference.Name}' does not exist in the target installation."));
				continue;
			}

			fluids[FluidKey(fluidReference)] = fluid;
		}

		var weatherControllers = new Dictionary<string, IWeatherController?>(StringComparer.Ordinal);
		foreach (var (zone, index) in zones.Select((value, index) => (value, index)))
		{
			var key = SpatialAreaPackageSerializer.ZoneKey(package, zone, index);
			IWeatherController? weatherController = null;
			if (zone.WeatherController is not null)
			{
				weatherController = actor.Gameworld.WeatherControllers
					.FirstOrDefault(x => x.Name.Equals(
						zone.WeatherController.Name,
						StringComparison.InvariantCultureIgnoreCase));
				if (weatherController is null)
				{
					diagnostics.Add(Error("missing-weather-controller",
						$"Required weather controller '{zone.WeatherController.Name}' for zone '{zoneNames[key]}' does not exist in the target installation."));
				}
			}

			weatherControllers[key] = weatherController;
		}

		var timeZonesByZone = zones
			.Select((zone, index) =>
			{
				var key = SpatialAreaPackageSerializer.ZoneKey(package, zone, index);
				return (key, value: (IReadOnlyDictionary<string, (IClock Clock, IMudTimeZone TimeZone)>)
					ResolveTimeZones(zone, targetShard, diagnostics));
			})
			.ToDictionary(x => x.key, x => x.value, StringComparer.Ordinal);
		var firstZoneKey = SpatialAreaPackageSerializer.ZoneKey(package, zones[0], 0);
		var timeZones = timeZonesByZone[firstZoneKey];
		var firstWeatherController = weatherControllers[firstZoneKey];
		if (diagnostics.Any(x => x.Severity == SpatialAreaTransferDiagnosticSeverity.Error))
		{
			failure = new SpatialAreaTransferResult
			{
				Summary = "Package validation failed. Nothing was imported.",
				PackagePath = packagePath,
				Diagnostics = diagnostics,
				ZoneCount = zones.Count,
				RoomCount = PackagedRoomCount(package),
				CellCount = package.Cells.Count,
				ExitCount = package.Exits.Count,
				OmittedItems = package.Omissions?.Select(x => x.Message).ToList() ?? []
			};
			return null;
		}

		var preflight = new ImportPreflight
		{
			Package = package,
			PackagePath = packagePath,
			ZoneName = zoneNames[firstZoneKey],
			Zones = zones,
			ZoneNames = zoneNames,
			OverlayPackage = actor.CurrentOverlayPackage,
			Terrains = terrains,
			HearingProfiles = hearingProfiles,
			Fluids = fluids,
			ForagableProfiles = foragableProfiles,
			Tags = tags,
			RangedCovers = covers,
			MagicResources = magicResources,
			TimeZones = timeZones,
			TimeZonesByZone = timeZonesByZone,
			WeatherController = firstWeatherController,
			WeatherControllers = weatherControllers
		};
		preflight.Diagnostics.AddRange(diagnostics);
		return preflight;
	}

	private static SpatialExitSideDefinition BuildExitSide(ICellExit side)
	{
		if (side is not INonCardinalCellExit nonCardinal)
		{
			return new SpatialExitSideDefinition { Direction = (int)side.OutboundDirection };
		}

		return new SpatialExitSideDefinition
		{
			Direction = (int)side.OutboundDirection,
			Verb = nonCardinal.Verb,
			PrimaryKeyword = nonCardinal.PrimaryKeyword,
			Keywords = string.Join(" ", nonCardinal.Keywords),
			InboundDescription = nonCardinal.InboundDescription,
			InboundTarget = nonCardinal.InboundTarget,
			OutboundDescription = nonCardinal.OutboundDescription,
			OutboundTarget = nonCardinal.OutboundTarget
		};
	}

	private static IReadOnlyList<SpatialAreaTransferDiagnostic> ValidateExportableCells(
		IReadOnlyCollection<ICell> cells)
	{
		var diagnostics = new List<SpatialAreaTransferDiagnostic>();
		using (new FMDB())
		{
			var ids = cells.Select(x => x.Id).ToList();
			var persistedCells = FMDB.Context.Cells
				.Where(x => ids.Contains(x.Id))
				.ToDictionary(x => x.Id);
			foreach (var cell in cells)
			{
				if (cell.Temporary)
				{
					diagnostics.Add(Error("temporary-cell",
						$"Cell #{cell.Id:N0} is temporary and cannot be faithfully imported."));
				}

				if (cell is Cell concreteCell &&
				    (concreteCell.HostedVehicleId.HasValue || concreteCell.HostedVehicleCompartmentId.HasValue))
				{
					diagnostics.Add(Error("hosted-vehicle-cell",
						$"Cell #{cell.Id:N0} is a hosted vehicle interior and cannot be detached from its vehicle."));
				}

				if (cell.AgricultureField is not null)
				{
					diagnostics.Add(Error("agriculture-field",
						$"Cell #{cell.Id:N0} has an agriculture field, which spatial packages do not carry."));
				}

				if (persistedCells.TryGetValue(cell.Id, out var dbCell))
				{
					if (HasPersistedEffects(dbCell.EffectData))
					{
						diagnostics.Add(Error("persisted-cell-effects",
							$"Cell #{cell.Id:N0} has persisted effects. Spatial packages refuse to discard effect state."));
					}

					if (HasSurfaceLiquid(dbCell.SurfaceLiquidData))
					{
						diagnostics.Add(Error("surface-liquid",
							$"Cell #{cell.Id:N0} has persistent surface-liquid state, which spatial packages do not carry."));
					}
				}
			}
		}

		var areaCount = cells
			.SelectMany(x => x.Areas)
			.Distinct()
			.Count();
		if (areaCount > 0)
		{
			diagnostics.Add(Warning("area-membership-omitted",
				$"Membership in {areaCount:N0} cross-cutting area group(s) is not imported by package version 1."));
		}

		var characterCount = cells.Sum(x => x.Characters.Count());
		var itemCount = cells.Sum(x => x.GameItems.Count());
		if (characterCount > 0 || itemCount > 0)
		{
			diagnostics.Add(Warning("contents-omitted",
				$"The source contains {characterCount:N0} character(s) and {itemCount:N0} item(s). Spatial packages never move live contents."));
		}

		var hookCount = cells.Sum(x => x.Hooks.Count());
		if (hookCount > 0)
		{
			diagnostics.Add(Warning("hooks-omitted",
				$"{hookCount:N0} installed cell hook reference(s) are not spatial topology and are not included in package version 1."));
		}

		return diagnostics;
	}

	private static bool HasPersistedEffects(string? effectData)
	{
		if (string.IsNullOrWhiteSpace(effectData))
		{
			return false;
		}

		try
		{
			return XElement.Parse(effectData).Elements().Any();
		}
		catch
		{
			return true;
		}
	}

	private static bool HasSurfaceLiquid(string? surfaceLiquidData)
	{
		if (string.IsNullOrWhiteSpace(surfaceLiquidData))
		{
			return false;
		}

		try
		{
			var element = XElement.Parse(surfaceLiquidData);
			return element.HasElements || !string.IsNullOrWhiteSpace(element.Value);
		}
		catch
		{
			return true;
		}
	}

	private static Dictionary<string, T> ResolveReferences<T>(
		IEnumerable<SpatialNamedReference> references,
		IEnumerable<T> candidates,
		string kind,
		ICollection<SpatialAreaTransferDiagnostic> diagnostics)
		where T : class, IFrameworkItem
	{
		var result = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
		foreach (var reference in references
			         .Where(x => x is not null)
			         .DistinctBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase))
		{
			var item = candidates.FirstOrDefault(x =>
				x.Name.Equals(reference.Name, StringComparison.InvariantCultureIgnoreCase));
			if (item is null)
			{
				diagnostics.Add(Error($"missing-{kind}",
					$"Required {kind} '{reference.Name}' does not exist in the target installation."));
				continue;
			}

			result[reference.Name] = item;
		}

		return result;
	}

	private static Dictionary<string, (IClock Clock, IMudTimeZone TimeZone)> ResolveTimeZones(
		SpatialZoneDefinition zone,
		IShard targetShard,
		ICollection<SpatialAreaTransferDiagnostic> diagnostics)
	{
		var result =
			new Dictionary<string, (IClock Clock, IMudTimeZone TimeZone)>(
				StringComparer.InvariantCultureIgnoreCase);
		foreach (var sourceTimeZone in zone.TimeZones)
		{
			var clock = targetShard.Clocks.FirstOrDefault(x =>
				x.Alias.Equals(sourceTimeZone.ClockAlias, StringComparison.InvariantCultureIgnoreCase));
			if (clock is null)
			{
				diagnostics.Add(Error("missing-clock",
					$"Target shard '{targetShard.Name}' does not have required clock alias '{sourceTimeZone.ClockAlias}'."));
				continue;
			}

			var timeZone = clock.Timezones.FirstOrDefault(x =>
				x.Alias.Equals(sourceTimeZone.TimeZoneAlias, StringComparison.InvariantCultureIgnoreCase) ||
				x.Description.Equals(sourceTimeZone.TimeZoneDescription, StringComparison.InvariantCultureIgnoreCase));
			if (timeZone is null)
			{
				diagnostics.Add(Error("missing-timezone",
					$"Clock '{clock.Alias}' does not have required timezone '{sourceTimeZone.TimeZoneAlias}'."));
				continue;
			}

			result[clock.Alias] = (clock, timeZone);
		}

		foreach (var clock in targetShard.Clocks.Where(x => !result.ContainsKey(x.Alias)))
		{
			result[clock.Alias] = (clock, clock.PrimaryTimezone);
			diagnostics.Add(Warning("target-clock-defaulted",
				$"Target-only clock '{clock.Alias}' will use its primary timezone."));
		}

		return result;
	}

	private static void ValidateZoneNumbers(
		SpatialZoneDefinition zone,
		ICollection<SpatialAreaTransferDiagnostic> diagnostics)
	{
		if (!double.IsFinite(zone.LatitudeRadians) ||
		    zone.LatitudeRadians is < -Math.PI / 2.0 or > Math.PI / 2.0)
		{
			diagnostics.Add(Error("invalid-latitude", "Zone latitude is outside the valid range."));
		}

		if (!double.IsFinite(zone.LongitudeRadians) ||
		    zone.LongitudeRadians is < -Math.PI or > Math.PI)
		{
			diagnostics.Add(Error("invalid-longitude", "Zone longitude is outside the valid range."));
		}

		if (!double.IsFinite(zone.ElevationMetres))
		{
			diagnostics.Add(Error("invalid-elevation", "Zone elevation is not finite."));
		}

		if (!double.IsFinite(zone.AmbientLightPollution) || zone.AmbientLightPollution < 0.0)
		{
			diagnostics.Add(Error("invalid-ambient-light", "Zone ambient light must be finite and non-negative."));
		}
	}

	private static void ValidateEnums(
		SpatialAreaPackage package,
		ICollection<SpatialAreaTransferDiagnostic> diagnostics)
	{
		foreach (var cell in package.Cells)
		{
			if (!Enum.IsDefined((CellOutdoorsType)cell.Overlay.OutdoorsType))
			{
				diagnostics.Add(Error("invalid-outdoors-type",
					$"Cell '{cell.Key}' has an unknown outdoors type."));
			}

			foreach (var resource in cell.MagicResources.Where(x =>
				         !double.IsFinite(x.Amount) || x.Amount < 0.0))
			{
				diagnostics.Add(Error("invalid-magic-resource",
					$"Cell '{cell.Key}' has an invalid amount for magic resource '{resource.Resource.Name}'."));
			}
		}

		foreach (var exit in package.Exits)
		{
			if (!Enum.IsDefined((CardinalDirection)exit.Side1.Direction) ||
			    !Enum.IsDefined((CardinalDirection)exit.Side2.Direction) ||
			    (exit.AcceptsDoor && !Enum.IsDefined((SizeCategory)exit.DoorSize)) ||
			    !Enum.IsDefined((SizeCategory)exit.MaximumSizeToEnter) ||
			    !Enum.IsDefined((SizeCategory)exit.MaximumSizeToEnterUpright) ||
			    !Enum.IsDefined((Difficulty)exit.ClimbDifficulty) ||
			    exit.BlockedLayers.Any(x => !Enum.IsDefined((RoomLayer)x)))
			{
				diagnostics.Add(Error("invalid-exit-enum",
					$"Exit '{exit.Key}' contains an enum value unknown to this server."));
			}

			var side1NonCardinal = !string.IsNullOrWhiteSpace(exit.Side1.Verb);
			var side2NonCardinal = !string.IsNullOrWhiteSpace(exit.Side2.Verb);
			if (side1NonCardinal != ((CardinalDirection)exit.Side1.Direction == CardinalDirection.Unknown) ||
			    side2NonCardinal != ((CardinalDirection)exit.Side2.Direction == CardinalDirection.Unknown))
			{
				diagnostics.Add(Error("invalid-exit-kind",
					$"Exit '{exit.Key}' has inconsistent cardinal direction and non-cardinal verb data."));
			}
		}
	}

	public static bool TryResolvePackagePath(
		string packageDirectory,
		string packageFileName,
		out string packagePath,
		out string error)
	{
		packagePath = string.Empty;
		error = string.Empty;
		var fileName = packageFileName.Trim();
		if (string.IsNullOrWhiteSpace(fileName))
		{
			error = "You must specify a package file name.";
			return false;
		}

		if (!fileName.EndsWith(".fmsa.json", StringComparison.InvariantCultureIgnoreCase))
		{
			fileName += ".fmsa.json";
		}

		if (!Path.GetFileName(fileName).Equals(fileName, StringComparison.Ordinal) ||
		    fileName.Any(x => !(char.IsLetterOrDigit(x) || x is '-' or '_' or '.')))
		{
			error = "Package names may contain only letters, digits, periods, hyphens, and underscores, with no path.";
			return false;
		}

		var root = Path.GetFullPath(packageDirectory)
			.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
		var candidate = Path.GetFullPath(Path.Combine(root, fileName));
		if (!candidate.StartsWith(root, StringComparison.OrdinalIgnoreCase))
		{
			error = "The package path must remain inside the server's Spatial Packages directory.";
			return false;
		}

		packagePath = candidate;
		return true;
	}

	private static string FluidKey(SpatialFluidReference reference)
	{
		return $"{reference.Kind}:{reference.Name}";
	}

	private static SpatialNamedReference? Reference(IFrameworkItem? item)
	{
		return item is null ? null : new SpatialNamedReference { SourceId = item.Id, Name = item.Name };
	}

	private static SpatialFluidReference? FluidReference(IFluid? fluid)
	{
		return fluid is null
			? null
			: new SpatialFluidReference
			{
				SourceId = fluid.Id,
				Name = fluid.Name,
				Kind = fluid is ILiquid ? "liquid" : "gas"
			};
	}

	private static SpatialAreaTransferResult Failure(
		string message,
		IEnumerable<SpatialAreaTransferDiagnostic> diagnostics,
		string code)
	{
		var allDiagnostics = diagnostics.ToList();
		allDiagnostics.Add(Error(code, message));
		return new SpatialAreaTransferResult
		{
			Summary = message,
			Diagnostics = allDiagnostics
		};
	}

	private static SpatialAreaTransferDiagnostic Error(string code, string message)
	{
		return new SpatialAreaTransferDiagnostic(SpatialAreaTransferDiagnosticSeverity.Error, code, message);
	}

	private static SpatialAreaTransferDiagnostic Warning(string code, string message)
	{
		return new SpatialAreaTransferDiagnostic(SpatialAreaTransferDiagnosticSeverity.Warning, code, message);
	}
}

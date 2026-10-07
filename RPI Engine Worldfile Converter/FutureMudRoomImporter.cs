#nullable enable

using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace RPI_Engine_Worldfile_Converter;

public sealed record FutureMudRoomValidationIssue(string SourceKey, string Severity, string Message);

public sealed record FutureMudTerrainReference(long Id, string Name, long? AtmosphereId, string AtmosphereType);

public sealed record FutureMudClockTimezoneReference(long ClockId, long TimezoneId, string ClockDescription);

public sealed record FutureMudZoneTemplateReference(
	long Id,
	string Name,
	long ShardId,
	double Latitude,
	double Longitude,
	double Elevation,
	double AmbientLightPollution,
	long? WeatherControllerId,
	IReadOnlyList<FutureMudClockTimezoneReference> Timezones);

public sealed record FutureMudRoomImportDefaults(
	long ShardId,
	double Latitude,
	double Longitude,
	double Elevation,
	double AmbientLightPollution,
	long? WeatherControllerId,
	IReadOnlyList<FutureMudClockTimezoneReference> Timezones,
	string Description);

public sealed record RoomApplyAuditZoneEntry(
	string GroupKey,
	string ZoneName,
	string OverlayPackageName,
	string Action,
	long? ZoneId,
	long? PackageId);

public sealed record RoomApplyAuditRoomEntry(
	string SourceKey,
	int Vnum,
	string ZoneGroupKey,
	string Action,
	long? ZoneId,
	long? LegacyRoomId,
	long? RoomId,
	long? OverlayId);

public sealed record RoomApplyAuditExitEntry(
	string ExitKey,
	int RoomVnum1,
	int RoomVnum2,
	string Action,
	long? ExitId);

public sealed record RoomApplyAuditReport(
	DateTime GeneratedUtc,
	bool Execute,
	string DefaultsDescription,
	IReadOnlyList<RoomApplyAuditZoneEntry> Zones,
	IReadOnlyList<RoomApplyAuditRoomEntry> Rooms,
	IReadOnlyList<RoomApplyAuditExitEntry> Exits);

public sealed record FutureMudRoomImportResult(
	int InsertedZoneCount,
	int InsertedRoomCount,
	int InsertedExitCount,
	int SkippedExistingZoneCount,
	IReadOnlyList<FutureMudRoomValidationIssue> Issues,
	RoomApplyAuditReport Audit);

public sealed record FutureMudRoomIdReservation(int Vnum, long RoomId);

public sealed record FutureMudRoomIdReservationPlan(
	IReadOnlyDictionary<int, FutureMudRoomIdReservation> Reservations,
	IReadOnlyList<FutureMudRoomValidationIssue> Issues);

public static class FutureMudRoomImportLimits
{
	public const int RoomDescriptionMaxLength = 4000;
	public const int ExitTextMaxLength = 255;

	public static string TruncateRoomDescription(string description)
	{
		return TruncateText(description, RoomDescriptionMaxLength);
	}

	public static string TruncateExitText(string? text)
	{
		return TruncateText(text ?? string.Empty, ExitTextMaxLength);
	}

	private static string TruncateText(string text, int maxLength)
	{
		return text.Length <= maxLength
			? text
			: text[..maxLength];
	}
}

public static class FutureMudRoomIdPlanner
{
	public static FutureMudRoomIdReservationPlan Plan(
		IEnumerable<ConvertedRoomDefinition> rooms,
		IReadOnlySet<long> existingRoomIds)
	{
		var roomList = rooms
			.OrderBy(x => x.Vnum)
			.ToList();
		Dictionary<int, FutureMudRoomIdReservation> reservations = [];
		List<FutureMudRoomValidationIssue> issues = [];
		HashSet<long> reservedRoomIds = [];
		var nextRoomFallbackId = DetermineFirstFallbackId(roomList, existingRoomIds);

		foreach (var room in roomList)
		{
			var cellId = ReserveId(
				room,
				"Room",
				existingRoomIds,
				reservedRoomIds,
				ref nextRoomFallbackId,
				issues);

			reservations[room.Vnum] = new FutureMudRoomIdReservation(room.Vnum, cellId);
		}

		return new FutureMudRoomIdReservationPlan(reservations, issues);
	}

	private static long DetermineFirstFallbackId(
		IReadOnlyList<ConvertedRoomDefinition> rooms,
		IReadOnlySet<long> existingIds)
	{
		var highestExistingId = existingIds.Count == 0
			? 0L
			: existingIds.Max();
		var highestLegacyId = rooms
			.Select(x => x.LegacyPersistenceId ?? 0L)
			.DefaultIfEmpty(0L)
			.Max();

		return Math.Max(highestExistingId, highestLegacyId) + 1;
	}

	private static long ReserveId(
		ConvertedRoomDefinition room,
		string entityName,
		IReadOnlySet<long> existingIds,
		ISet<long> reservedIds,
		ref long nextFallbackId,
		ICollection<FutureMudRoomValidationIssue> issues)
	{
		if (room.LegacyPersistenceId is { } legacyId)
		{
			if (!existingIds.Contains(legacyId) && reservedIds.Add(legacyId))
			{
				return legacyId;
			}

			var fallbackId = ReserveFallbackId(existingIds, reservedIds, ref nextFallbackId);
			var reason = existingIds.Contains(legacyId)
				? "already exists in the target database"
				: "was already reserved by another converted room";
			issues.Add(new FutureMudRoomValidationIssue(
				room.SourceKey,
				"warning",
				$"Legacy vnum {room.Vnum} could not be used as the FutureMUD {entityName} id because id {legacyId} {reason}; assigned fallback id {fallbackId}."));
			return fallbackId;
		}

		var generatedFallbackId = ReserveFallbackId(existingIds, reservedIds, ref nextFallbackId);
		issues.Add(new FutureMudRoomValidationIssue(
			room.SourceKey,
			"warning",
			$"Legacy vnum {room.Vnum} cannot be used as the FutureMUD {entityName} id; assigned fallback id {generatedFallbackId}."));
		return generatedFallbackId;
	}

	private static long ReserveFallbackId(
		IReadOnlySet<long> existingIds,
		ISet<long> reservedIds,
		ref long nextFallbackId)
	{
		while (existingIds.Contains(nextFallbackId) || reservedIds.Contains(nextFallbackId))
		{
			nextFallbackId++;
		}

		var fallbackId = nextFallbackId;
		reservedIds.Add(fallbackId);
		nextFallbackId++;
		return fallbackId;
	}
}

public sealed class FutureMudRoomBaselineCatalog
{
	public required long BuilderAccountId { get; init; }
	public required Dictionary<string, FutureMudTerrainReference> Terrains { get; init; }
	public required Dictionary<string, FutureMudZoneTemplateReference> ZoneTemplates { get; init; }
	public required IReadOnlyList<long> ShardIds { get; init; }
	public required IReadOnlyList<FutureMudClockTimezoneReference> PrimaryTimezones { get; init; }
	public required IReadOnlyList<long> ClockIds { get; init; }

	public static FutureMudRoomBaselineCatalog Load(FuturemudDatabaseContext context)
	{
		var builderAccountId = context.Accounts
			.AsNoTracking()
			.Select(x => x.Id)
			.OrderBy(x => x)
			.FirstOrDefault();
		if (builderAccountId == 0)
		{
			throw new InvalidOperationException("The target database does not contain any accounts to attribute imported room packages to.");
		}

		var terrains = context.Terrains
			.AsNoTracking()
			.ToDictionary(
				x => x.Name,
				x => new FutureMudTerrainReference(x.Id, x.Name, x.AtmosphereId, x.AtmosphereType ?? string.Empty),
				StringComparer.OrdinalIgnoreCase);

		var zones = context.Zones
			.AsNoTracking()
			.Include(x => x.ZonesTimezones)
			.ToList();
		var zoneTemplates = zones.ToDictionary(
			x => x.Name,
			x => new FutureMudZoneTemplateReference(
				x.Id,
				x.Name,
				x.ShardId,
				x.Latitude,
				x.Longitude,
				x.Elevation,
				x.AmbientLightPollution,
				x.WeatherControllerId,
				x.ZonesTimezones
					.OrderBy(y => y.ClockId)
					.Select(y => new FutureMudClockTimezoneReference(y.ClockId, y.TimezoneId, $"Clock {y.ClockId}"))
					.ToList()),
			StringComparer.OrdinalIgnoreCase);

		var primaryTimezones = context.Timezones
			.AsNoTracking()
			.Include(x => x.Clock)
			.Where(x => x.Clock.PrimaryTimezoneId == x.Id)
			.OrderBy(x => x.ClockId)
			.Select(x => new FutureMudClockTimezoneReference(x.ClockId, x.Id, x.Clock.Definition))
			.ToList();

		return new FutureMudRoomBaselineCatalog
		{
			BuilderAccountId = builderAccountId,
			Terrains = terrains,
			ZoneTemplates = zoneTemplates,
			ShardIds = context.Shards
				.AsNoTracking()
				.Select(x => x.Id)
				.OrderBy(x => x)
				.ToList(),
			PrimaryTimezones = primaryTimezones,
			ClockIds = context.Clocks
				.AsNoTracking()
				.Select(x => x.Id)
				.OrderBy(x => x)
				.ToList(),
		};
	}

	public bool TryResolveImportDefaults(string? zoneTemplateName, out FutureMudRoomImportDefaults? defaults, out string? error)
	{
		defaults = null;
		error = null;

		if (!string.IsNullOrWhiteSpace(zoneTemplateName))
		{
			if (!ZoneTemplates.TryGetValue(zoneTemplateName, out var template))
			{
				error = $"Could not find zone template '{zoneTemplateName}'.";
				return false;
			}

			defaults = new FutureMudRoomImportDefaults(
				template.ShardId,
				template.Latitude,
				template.Longitude,
				template.Elevation,
				template.AmbientLightPollution,
				template.WeatherControllerId,
				template.Timezones,
				$"Inherited shard, geography, weather controller, ambient light, and timezones from zone template '{template.Name}'.");
			return true;
		}

		if (ShardIds.Count != 1)
		{
			error = "Automatic zone defaults require exactly one shard in the target database; use --zone-template to choose an existing seeded zone.";
			return false;
		}

		if (ClockIds.Count == 0 || PrimaryTimezones.Count != ClockIds.Count)
		{
			error = "Automatic zone defaults require a primary timezone for every clock; use --zone-template to inherit seeded timezone data.";
			return false;
		}

		defaults = new FutureMudRoomImportDefaults(
			ShardIds[0],
			0.0,
			0.0,
			0.0,
			0.0,
			null,
			PrimaryTimezones,
			"Auto-selected the single available shard and all primary timezones because --zone-template was not supplied.");
		return true;
	}
}

public static class FutureMudRoomValidation
{
	public static IReadOnlyList<FutureMudRoomValidationIssue> Validate(
		FutureMudRoomBaselineCatalog catalog,
		RoomConversionResult conversion,
		string? zoneTemplateName)
	{
		List<FutureMudRoomValidationIssue> issues = [];
		HashSet<string> zoneNames = [];
		HashSet<string> packageNames = [];
		HashSet<int> roomVnums = [];

		foreach (var zone in conversion.Zones)
		{
			if (!zoneNames.Add(zone.ZoneName))
			{
				issues.Add(new FutureMudRoomValidationIssue(
					zone.GroupKey,
					"error",
					$"Duplicate converted zone name '{zone.ZoneName}'."));
			}

			if (!packageNames.Add(zone.OverlayPackageName))
			{
				issues.Add(new FutureMudRoomValidationIssue(
					zone.GroupKey,
					"error",
					$"Duplicate overlay package name '{zone.OverlayPackageName}'."));
			}
		}

		foreach (var room in conversion.Rooms)
		{
			if (!roomVnums.Add(room.Vnum))
			{
				issues.Add(new FutureMudRoomValidationIssue(room.SourceKey, "error", $"Duplicate room vnum '{room.Vnum}'."));
			}

			if (!catalog.Terrains.ContainsKey(room.TerrainName))
			{
				issues.Add(new FutureMudRoomValidationIssue(
					room.SourceKey,
					"error",
					$"Missing terrain '{room.TerrainName}'."));
			}
		}

		if (!catalog.TryResolveImportDefaults(zoneTemplateName, out _, out var error))
		{
			issues.Add(new FutureMudRoomValidationIssue("baseline", "error", error ?? "Could not resolve room import defaults."));
		}

		return issues;
	}
}

public sealed class FutureMudRoomImporter
{
	private sealed class RoomDbState
	{
		public required ConvertedRoomDefinition Room { get; init; }
		public required MudSharp.Models.Room DbRoom { get; init; }
		public required MudSharp.Models.RoomOverlay DbOverlay { get; init; }
	}

	private readonly FuturemudDatabaseContext _context;
	private readonly FutureMudRoomBaselineCatalog _catalog;
	private readonly string? _zoneTemplateName;
	private readonly DateTime _now = DateTime.UtcNow;
	private long _nextRoomOverlayPackageId;

	public FutureMudRoomImporter(
		FuturemudDatabaseContext context,
		FutureMudRoomBaselineCatalog catalog,
		string? zoneTemplateName)
	{
		_context = context;
		_catalog = catalog;
		_zoneTemplateName = zoneTemplateName;
		_nextRoomOverlayPackageId = (context.RoomOverlayPackages
			.AsNoTracking()
			.Max(x => (long?)x.Id) ?? 0L) + 1L;
	}

	public IReadOnlyList<FutureMudRoomValidationIssue> Validate(RoomConversionResult conversion)
	{
		return FutureMudRoomValidation.Validate(_catalog, conversion, _zoneTemplateName);
	}

	public FutureMudRoomImportResult Apply(RoomConversionResult conversion, bool execute)
	{
		var issues = Validate(conversion).ToList();
		var fatalIssues = issues.Where(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase)).ToList();
		if (fatalIssues.Count > 0 || !_catalog.TryResolveImportDefaults(_zoneTemplateName, out var defaults, out _))
		{
			return new FutureMudRoomImportResult(
				0,
				0,
				0,
				0,
				issues,
				new RoomApplyAuditReport(DateTime.UtcNow, execute, "Validation failed.", [], [], []));
		}

		var packageMarkers = LoadExistingPackageMarkers();
		var existingPackageNames = _context.RoomOverlayPackages
			.AsNoTracking()
			.Select(x => x.Name)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var existingZoneNames = _context.Zones
			.AsNoTracking()
			.Select(x => x.Name)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var orderedZones = conversion.Zones
			.OrderBy(x => x.ZoneName, StringComparer.OrdinalIgnoreCase)
			.ToList();
		var orderedExits = conversion.Exits
			.OrderBy(x => x.ExitKey, StringComparer.OrdinalIgnoreCase)
			.ToList();
		var roomZoneGroupByVnum = conversion.Rooms.ToDictionary(x => x.Vnum, x => x.ZoneGroupKey);
		Dictionary<string, string> zoneActionByGroupKey = new(StringComparer.OrdinalIgnoreCase);
		var skipReasons = conversion.Zones.ToDictionary(
			x => x.GroupKey,
			x => GetSkipReason(x, CreatePackageMarker(x), packageMarkers, existingPackageNames, existingZoneNames),
			StringComparer.OrdinalIgnoreCase);
		var roomsToCreate = conversion.Zones
			.Where(x => skipReasons[x.GroupKey] is null)
			.SelectMany(x => x.Rooms)
			.ToList();
		var idPlan = FutureMudRoomIdPlanner.Plan(
			roomsToCreate,
			_context.Rooms.AsNoTracking().Select(x => x.Id).ToHashSet());
		issues.AddRange(idPlan.Issues);

		List<RoomApplyAuditZoneEntry> zoneAudit = [];
		List<RoomApplyAuditRoomEntry> roomAudit = [];
		List<RoomApplyAuditExitEntry> exitAudit = [];

		var insertedZoneCount = 0;
		var insertedRoomCount = 0;
		var insertedExitCount = 0;
		var skippedExistingZoneCount = 0;

		Dictionary<int, RoomDbState> createdRoomStates = [];
		using var transaction = execute ? _context.Database.BeginTransaction() : null;

		foreach (var zone in orderedZones)
		{
			var marker = CreatePackageMarker(zone);
			var skipReason = skipReasons[zone.GroupKey];
			if (skipReason is not null)
			{
				zoneActionByGroupKey[zone.GroupKey] = "skipped-existing";
				issues.Add(new FutureMudRoomValidationIssue(zone.GroupKey, "warning", skipReason));
				zoneAudit.Add(new RoomApplyAuditZoneEntry(zone.GroupKey, zone.ZoneName, zone.OverlayPackageName, "skipped-existing", null, null));
				foreach (var room in zone.Rooms)
				{
					roomAudit.Add(new RoomApplyAuditRoomEntry(room.SourceKey, room.Vnum, room.ZoneGroupKey, "skipped-existing", null, null, null, null));
				}

				skippedExistingZoneCount++;
				continue;
			}

			if (!execute)
			{
				zoneActionByGroupKey[zone.GroupKey] = "would-create";
				zoneAudit.Add(new RoomApplyAuditZoneEntry(zone.GroupKey, zone.ZoneName, zone.OverlayPackageName, "would-create", null, null));
				foreach (var room in zone.Rooms)
				{
					var reservation = idPlan.Reservations[room.Vnum];
					roomAudit.Add(new RoomApplyAuditRoomEntry(
						room.SourceKey,
						room.Vnum,
						room.ZoneGroupKey,
						"would-create",
						null,
						null,
						reservation.RoomId,
						null));
				}

				continue;
			}

			zoneActionByGroupKey[zone.GroupKey] = "created";
			var dbZone = new Zone
			{
				Name = zone.ZoneName,
				ShardId = defaults!.ShardId,
				Latitude = defaults.Latitude,
				Longitude = defaults.Longitude,
				Elevation = defaults.Elevation,
				AmbientLightPollution = defaults.AmbientLightPollution,
				WeatherControllerId = defaults.WeatherControllerId,
			};
			_context.Zones.Add(dbZone);

			foreach (var timezone in defaults.Timezones)
			{
				dbZone.ZonesTimezones.Add(new ZonesTimezones
				{
					Zone = dbZone,
					ClockId = timezone.ClockId,
					TimezoneId = timezone.TimezoneId,
				});
			}

			var editableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = _catalog.BuilderAccountId,
				BuilderDate = _now,
				BuilderComment = BuildPackageBuilderComment(zone),
				ReviewerAccountId = _catalog.BuilderAccountId,
				ReviewerDate = _now,
				ReviewerComment = "Imported by the RPI Engine Worldfile Converter.",
			};
			var package = new RoomOverlayPackage
			{
				Id = _nextRoomOverlayPackageId++,
				Name = zone.OverlayPackageName,
				RevisionNumber = 0,
				EditableItem = editableItem,
			};
			_context.RoomOverlayPackages.Add(package);
			_context.SaveChanges();

			zoneAudit.Add(new RoomApplyAuditZoneEntry(zone.GroupKey, zone.ZoneName, zone.OverlayPackageName, "created", dbZone.Id, package.Id));
			insertedZoneCount++;

			var zoneRooms = new List<(ConvertedRoomDefinition room, MudSharp.Models.Room dbRoom)>();
			foreach (var room in zone.Rooms.OrderBy(x => x.Vnum))
			{
				var reservation = idPlan.Reservations[room.Vnum];
				var dbRoom = new MudSharp.Models.Room
				{
					Id = reservation.RoomId, ZoneId = dbZone.Id,
					X = room.Coordinates.X, Y = room.Coordinates.Y, Z = room.Coordinates.Z,
					Temporary = room.RoomFlagNames.Contains(nameof(RpiRoomFlags.Temporary), StringComparer.OrdinalIgnoreCase),
					EffectData = "<Effects/>"
				};
				zoneRooms.Add((room, dbRoom));
			}

			_context.Rooms.AddRange(zoneRooms.Select(x => x.dbRoom));
			_context.SaveChanges();

			var firstRoomId = zoneRooms.Select(x => x.dbRoom.Id).OrderBy(x => x).FirstOrDefault();
			dbZone.DefaultRoomId = firstRoomId == 0 ? null : firstRoomId;

			var zoneOverlays = new List<(ConvertedRoomDefinition room, MudSharp.Models.Room dbRoom, MudSharp.Models.RoomOverlay dbOverlay)>();
			foreach (var (room, dbRoom) in zoneRooms)
			{
				var terrain = _catalog.Terrains[room.TerrainName];
				var dbOverlay = new MudSharp.Models.RoomOverlay
				{
					Name = package.Name,
					RoomName = room.Name,
					RoomDescription = FutureMudRoomImportLimits.TruncateRoomDescription(room.EffectiveDescription),
					RoomOverlayPackageId = package.Id,
					RoomOverlayPackageRevisionNumber = package.RevisionNumber,
					RoomId = dbRoom.Id,
					TerrainId = terrain.Id,
					OutdoorsType = room.OutdoorsTypeValue,
					AmbientLightFactor = 1.0,
					AddedLight = 0.0,
					AtmosphereId = terrain.AtmosphereId,
					AtmosphereType = terrain.AtmosphereType,
					SafeQuit = room.SafeQuit,
				};
				zoneOverlays.Add((room, dbRoom, dbOverlay));
			}

			_context.RoomOverlays.AddRange(zoneOverlays.Select(x => x.dbOverlay));
			_context.SaveChanges();

			foreach (var (room, dbRoom, dbOverlay) in zoneOverlays)
			{
				dbRoom.CurrentOverlayId = dbOverlay.Id;
				createdRoomStates[room.Vnum] = new RoomDbState
				{
					Room = room,
					DbRoom = dbRoom,
					DbOverlay = dbOverlay,
				};

				roomAudit.Add(new RoomApplyAuditRoomEntry(
					room.SourceKey,
					room.Vnum,
					room.ZoneGroupKey,
					"created",
					dbZone.Id,
					null,
					dbRoom.Id,
					dbOverlay.Id));
				insertedRoomCount++;
			}

			_context.SaveChanges();
			packageMarkers.Add(marker);
			existingPackageNames.Add(zone.OverlayPackageName);
			existingZoneNames.Add(zone.ZoneName);
		}

		if (!execute)
		{
			foreach (var exit in orderedExits)
			{
				var action = roomZoneGroupByVnum.TryGetValue(exit.RoomVnum1, out var zoneGroupKey) &&
				             zoneActionByGroupKey.TryGetValue(zoneGroupKey, out var zoneAction) &&
				             zoneAction == "skipped-existing"
					? "skipped-existing"
					: "would-create";
				exitAudit.Add(new RoomApplyAuditExitEntry(exit.ExitKey, exit.RoomVnum1, exit.RoomVnum2, action, null));
			}

			return new FutureMudRoomImportResult(
				0,
				0,
				0,
				skippedExistingZoneCount,
				issues,
				new RoomApplyAuditReport(DateTime.UtcNow, false, defaults!.Description, zoneAudit, roomAudit, exitAudit));
		}

		var hiddenExitIdsByRoom = new Dictionary<long, List<long>>();
		var exitModels = new List<(ConvertedRoomExitDefinition definition, Exit dbExit)>();
		var linkedOverlayExits = new HashSet<(long RoomOverlayId, long ExitId)>();
		foreach (var exit in orderedExits)
		{
			if (!createdRoomStates.TryGetValue(exit.RoomVnum1, out var room1) ||
			    !createdRoomStates.TryGetValue(exit.RoomVnum2, out var room2))
			{
				exitAudit.Add(new RoomApplyAuditExitEntry(exit.ExitKey, exit.RoomVnum1, exit.RoomVnum2, "skipped-missing-room", null));
				continue;
			}

			var dbExit = new Exit
			{
				RoomId1 = room1.DbRoom.Id,
				RoomId2 = room2.DbRoom.Id,
				Direction1 = (int)exit.Side1.Direction.ToFutureMudDirection(),
				Direction2 = (int)exit.Side2.Direction.ToFutureMudDirection(),
				Keywords1 = FutureMudRoomImportLimits.TruncateExitText(exit.Side1.Keywords),
				Keywords2 = FutureMudRoomImportLimits.TruncateExitText(exit.Side2.Keywords),
				PrimaryKeyword1 = FutureMudRoomImportLimits.TruncateExitText(exit.Side1.PrimaryKeyword),
				PrimaryKeyword2 = FutureMudRoomImportLimits.TruncateExitText(exit.Side2.PrimaryKeyword),
				InboundDescription1 = FutureMudRoomImportLimits.TruncateExitText(exit.Side1.Description),
				InboundDescription2 = FutureMudRoomImportLimits.TruncateExitText(exit.Side2.Description),
				OutboundDescription1 = FutureMudRoomImportLimits.TruncateExitText(exit.Side1.Description),
				OutboundDescription2 = FutureMudRoomImportLimits.TruncateExitText(exit.Side2.Description),
				InboundTarget1 = FutureMudRoomImportLimits.TruncateExitText(exit.Side1.Description),
				InboundTarget2 = FutureMudRoomImportLimits.TruncateExitText(exit.Side2.Description),
				OutboundTarget1 = FutureMudRoomImportLimits.TruncateExitText(exit.Side1.Description),
				OutboundTarget2 = FutureMudRoomImportLimits.TruncateExitText(exit.Side2.Description),
				Verb1 = string.Empty,
				Verb2 = string.Empty,
				DoorId = null,
				AcceptsDoor = exit.AcceptsDoor,
				DoorSize = exit.DoorSize,
				MaximumSizeToEnter = exit.MaximumSizeToEnter,
				MaximumSizeToEnterUpright = exit.MaximumSizeToEnterUpright,
				FallRoom = exit.FallToRoomVnum is not null && createdRoomStates.TryGetValue(exit.FallToRoomVnum.Value, out var fallRoom)
					? fallRoom.DbRoom.Id
					: null,
				IsClimbExit = exit.IsClimbExit,
				ClimbDifficulty = exit.ClimbDifficulty,
				TimeMultiplier = 1.0,
				BlockedLayers = string.Empty,
			};
			exitModels.Add((exit, dbExit));
		}

		_context.Exits.AddRange(exitModels.Select(x => x.dbExit));
		_context.SaveChanges();

		foreach (var (definition, dbExit) in exitModels)
		{
			var room1 = createdRoomStates[definition.RoomVnum1];
			var room2 = createdRoomStates[definition.RoomVnum2];

			AddRoomOverlayExitLink(linkedOverlayExits, room1.DbOverlay.Id, dbExit.Id);

			if (definition.Side2.Visible)
			{
				AddRoomOverlayExitLink(linkedOverlayExits, room2.DbOverlay.Id, dbExit.Id);
			}

			if (definition.Side1.Hidden)
			{
				AddHiddenExit(hiddenExitIdsByRoom, room1.DbRoom.Id, dbExit.Id);
			}

			if (definition.Side2.Hidden)
			{
				AddHiddenExit(hiddenExitIdsByRoom, room2.DbRoom.Id, dbExit.Id);
			}

			exitAudit.Add(new RoomApplyAuditExitEntry(definition.ExitKey, definition.RoomVnum1, definition.RoomVnum2, "created", dbExit.Id));
			insertedExitCount++;
		}

		foreach (var roomState in createdRoomStates.Values)
		{
			roomState.DbRoom.EffectData = BuildEffectData(hiddenExitIdsByRoom.TryGetValue(roomState.DbRoom.Id, out var exitIds)
				? exitIds
				: Array.Empty<long>());
		}

		_context.SaveChanges();
		transaction?.Commit();

		return new FutureMudRoomImportResult(
			insertedZoneCount,
			insertedRoomCount,
			insertedExitCount,
			skippedExistingZoneCount,
			issues,
			new RoomApplyAuditReport(DateTime.UtcNow, true, defaults!.Description, zoneAudit, roomAudit, exitAudit));
	}

	private void AddRoomOverlayExitLink(
		ISet<(long RoomOverlayId, long ExitId)> linkedOverlayExits,
		long cellOverlayId,
		long exitId)
	{
		if (!linkedOverlayExits.Add((cellOverlayId, exitId)))
		{
			return;
		}

		_context.RoomOverlaysExits.Add(new RoomOverlayExit
		{
			RoomOverlayId = cellOverlayId,
			ExitId = exitId,
		});
	}

	private static string? GetSkipReason(
		ConvertedZoneDefinition zone,
		string marker,
		IReadOnlySet<string> packageMarkers,
		IReadOnlySet<string> existingPackageNames,
		IReadOnlySet<string> existingZoneNames)
	{
		if (packageMarkers.Contains(marker))
		{
			return $"Skipped zone group '{zone.GroupKey}' because its import package marker already exists.";
		}

		if (existingPackageNames.Contains(zone.OverlayPackageName))
		{
			return $"Skipped zone group '{zone.GroupKey}' because overlay package '{zone.OverlayPackageName}' already exists.";
		}

		if (existingZoneNames.Contains(zone.ZoneName))
		{
			return $"Skipped zone group '{zone.GroupKey}' because zone '{zone.ZoneName}' already exists.";
		}

		return null;
	}

	private HashSet<string> LoadExistingPackageMarkers()
	{
		return _context.RoomOverlayPackages
			.AsNoTracking()
			.Where(x => x.EditableItem != null && x.EditableItem.BuilderComment != null && x.EditableItem.BuilderComment.StartsWith("RPIROOMPACKAGE|"))
			.Select(x => x.EditableItem!.BuilderComment)
			.AsEnumerable()
			.Select(x => x.Split('\n', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty)
			.Where(x => x.StartsWith("RPIROOMPACKAGE|", StringComparison.Ordinal))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static string CreatePackageMarker(ConvertedZoneDefinition zone)
	{
		return $"RPIROOMPACKAGE|{zone.GroupKey}|{string.Join(",", zone.SourceZones)}";
	}

	private static string BuildPackageBuilderComment(ConvertedZoneDefinition zone)
	{
		return string.Join('\n',
		[
			CreatePackageMarker(zone),
			$"ZoneName={zone.ZoneName}",
			$"OverlayPackage={zone.OverlayPackageName}",
			$"SourceZones={string.Join(",", zone.SourceZones)}",
			$"Evidence={zone.Evidence}",
		]);
	}

	private static void AddHiddenExit(IDictionary<long, List<long>> hiddenExitIdsByRoom, long cellId, long exitId)
	{
		if (!hiddenExitIdsByRoom.TryGetValue(cellId, out var exitIds))
		{
			exitIds = [];
			hiddenExitIdsByRoom[cellId] = exitIds;
		}

		exitIds.Add(exitId);
	}

	private static string BuildEffectData(IEnumerable<long> hiddenExitIds)
	{
		var effectElements = hiddenExitIds
			.Distinct()
			.OrderBy(x => x)
			.Select(exitId => new XElement("Effect",
				new XElement("ApplicabilityProg", 0),
				new XElement("Type", "ExitHidden"),
				new XElement("Original", 0),
				new XElement("Remaining", 0),
				new XElement("Effect",
					new XElement("Exit", exitId))))
			.ToList();

		return new XElement("Effects", effectElements).ToString(SaveOptions.DisableFormatting);
	}
}

#nullable enable

using System.Globalization;
using System.Text.RegularExpressions;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;

namespace RPI_Engine_Worldfile_Converter;

public sealed class FutureMudRoomTransformer
{
	private static readonly Regex BaseRoomRegex = new(
		@"^(?:base\s*room|baseroom)\s+for\s+(?<name>.+)$",
		RegexOptions.Compiled | RegexOptions.IgnoreCase);

	private static readonly IReadOnlyList<RpiZoneGroupRule> ZoneGroupingRules =
	[
		new(
			"minas-morgul",
			"Minas Morgul",
			[5, 6],
			"Base-room evidence groups zone 5 (Minas Morgul) with zone 6 (Morgul Humans) into one Minas Morgul overlay pass."),
	];

	private sealed class ZoneState
	{
		public required string GroupKey { get; init; }
		public required string ZoneName { get; init; }
		public required string OverlayPackageName { get; init; }
		public required IReadOnlyList<int> SourceZones { get; init; }
		public required string Evidence { get; init; }
		public required List<RoomState> Rooms { get; init; }
		public required List<RoomConversionWarning> Warnings { get; init; }
	}

	private sealed class RoomState
	{
		public required RpiRoomRecord Source { get; init; }
		public required ZoneState Zone { get; init; }
		public required string EffectiveDescription { get; set; }
		public required RpiRoomWeatherRecord? EffectiveWeather { get; set; }
		public required bool XeroxResolved { get; set; }
		public required string TerrainName { get; set; }
		public required int OutdoorsTypeValue { get; set; }
		public required string OutdoorsTypeName { get; set; }
		public required bool SafeQuit { get; set; }
		public required List<RoomConversionWarning> Warnings { get; init; }
		public RoomCoordinate Coordinates { get; set; } = new(0, 0, 0);
		public ConvertedRoomDefinition ToDefinition()
		{
			return new ConvertedRoomDefinition
			{
				Vnum = Source.Vnum,
				SourceFile = Source.SourceFile,
				SourceZone = Source.Zone,
				SourceKey = Source.SourceKey,
				ZoneGroupKey = Zone.GroupKey,
				ZoneName = Zone.ZoneName,
				OverlayPackageName = Zone.OverlayPackageName,
				Name = Source.Name,
				RawDescription = Source.Description,
				EffectiveDescription = EffectiveDescription,
				TerrainName = TerrainName,
				OutdoorsTypeName = OutdoorsTypeName,
				OutdoorsTypeValue = OutdoorsTypeValue,
				SafeQuit = SafeQuit,
				Coordinates = Coordinates,
				RawFlags = Source.RawFlags,
				RoomFlagNames = DescribeFlags(Source.RoomFlags),
				SectorType = Source.SectorType,
				Deity = Source.Deity,
				XeroxSourceVnum = Source.XeroxSourceVnum,
				XeroxResolved = XeroxResolved,
				Capacity = Source.Capacity,
				EffectiveWeather = EffectiveWeather,
				ExtraDescriptions = Source.ExtraDescriptions,
				WrittenDescriptions = Source.WrittenDescriptions,
				RoomProgs = Source.RoomProgs,
				Secrets = Source.Secrets,
				Warnings = Warnings
					.Concat(Source.ParseWarnings.Select(x => new RoomConversionWarning(x.Code, x.Message)))
					.ToList()
			};
		}
	}

	public RoomConversionResult Convert(IEnumerable<RpiRoomRecord> rooms)
	{
		var orderedRooms = rooms
			.OrderBy(x => x.Zone)
			.ThenBy(x => x.Vnum)
			.ToList();
		var roomByVnum = orderedRooms
			.GroupBy(x => x.Vnum)
			.ToDictionary(x => x.Key, x => x.First());
		var zones = BuildZones(orderedRooms);
		var roomStates = BuildRoomStates(zones, roomByVnum);
		ResolveXerox(roomStates, roomByVnum);
		MapTerrainAndOutdoors(roomStates, roomByVnum);
		AssignCoordinates(zones, roomByVnum);
		var exits = BuildExits(roomStates, roomByVnum);

		var convertedRooms = roomStates
			.OrderBy(x => x.Source.Zone)
			.ThenBy(x => x.Source.Vnum)
			.Select(x => x.ToDefinition())
			.ToList();
		var convertedZones = zones
			.OrderBy(x => x.ZoneName, StringComparer.OrdinalIgnoreCase)
			.Select(zone => new ConvertedZoneDefinition(
				zone.GroupKey,
				zone.ZoneName,
				zone.OverlayPackageName,
				zone.SourceZones,
				zone.Evidence,
				zone.Rooms
					.OrderBy(x => x.Source.Vnum)
					.Select(x => x.ToDefinition())
					.ToList(),
				zone.Warnings.ToList()))
			.ToList();

		return new RoomConversionResult(convertedZones, convertedRooms, exits);
	}

	private static List<ZoneState> BuildZones(IReadOnlyList<RpiRoomRecord> rooms)
	{
		var groupedRooms = new Dictionary<string, List<RpiRoomRecord>>(StringComparer.OrdinalIgnoreCase);
		var groupedZoneNumbers = new Dictionary<string, SortedSet<int>>(StringComparer.OrdinalIgnoreCase);
		var groupedWarnings = new Dictionary<string, List<RoomConversionWarning>>(StringComparer.OrdinalIgnoreCase);

		foreach (var room in rooms)
		{
			var rule = ZoneGroupingRules.FirstOrDefault(x => x.SourceZones.Contains(room.Zone));
			var groupKey = rule?.GroupKey ?? $"zone-{room.Zone:D2}";
			if (!groupedRooms.TryGetValue(groupKey, out var roomList))
			{
				roomList = [];
				groupedRooms[groupKey] = roomList;
				groupedZoneNumbers[groupKey] = [];
				groupedWarnings[groupKey] = [];
			}

			roomList.Add(room);
			groupedZoneNumbers[groupKey].Add(room.Zone);
		}

		List<ZoneState> zones = [];
		foreach (var grouping in groupedRooms.OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
		{
			var rule = ZoneGroupingRules.FirstOrDefault(x => x.GroupKey.Equals(grouping.Key, StringComparison.OrdinalIgnoreCase));
			var sourceZones = groupedZoneNumbers[grouping.Key].OrderBy(x => x).ToList();
			var warnings = groupedWarnings[grouping.Key];
			var zoneName = rule?.DisplayNameOverride ??
			               DetermineZoneName(grouping.Value, sourceZones, warnings);
			var evidence = rule?.Evidence ??
			               $"Derived from base-room evidence in rooms.{sourceZones[0].ToString(CultureInfo.InvariantCulture)}.";

			zones.Add(new ZoneState
			{
				GroupKey = grouping.Key,
				ZoneName = zoneName,
				OverlayPackageName = $"RPI Import Rooms - {zoneName}",
				SourceZones = sourceZones,
				Evidence = evidence,
				Rooms = [],
				Warnings = warnings,
			});
		}

		return zones;
	}

	private static List<RoomState> BuildRoomStates(
		IEnumerable<ZoneState> zones,
		IReadOnlyDictionary<int, RpiRoomRecord> roomByVnum)
	{
		var zoneBySourceZone = zones
			.SelectMany(zone => zone.SourceZones.Select(sourceZone => (zone, sourceZone)))
			.ToDictionary(x => x.sourceZone, x => x.zone);

		List<RoomState> states = [];
		foreach (var room in roomByVnum.Values.OrderBy(x => x.Zone).ThenBy(x => x.Vnum))
		{
			var zone = zoneBySourceZone[room.Zone];
			var state = new RoomState
			{
				Source = room,
				Zone = zone,
				EffectiveDescription = room.Description,
				EffectiveWeather = room.Weather,
				XeroxResolved = room.XeroxSourceVnum is null,
				TerrainName = "Hall",
				OutdoorsTypeValue = (int)CellOutdoorsType.Outdoors,
				OutdoorsTypeName = CellOutdoorsType.Outdoors.Describe(),
				SafeQuit = room.RoomFlags.HasFlag(RpiRoomFlags.SafeQuit),
				Warnings = [],
			};
			zone.Rooms.Add(state);
			states.Add(state);
		}

		return states;
	}

	private static void ResolveXerox(IReadOnlyList<RoomState> roomStates, IReadOnlyDictionary<int, RpiRoomRecord> roomByVnum)
	{
		foreach (var state in roomStates)
		{
			if (state.Source.XeroxSourceVnum is not { } xeroxVnum)
			{
				continue;
			}

			if (!roomByVnum.TryGetValue(xeroxVnum, out var xeroxRoom))
			{
				state.XeroxResolved = false;
				state.Warnings.Add(new RoomConversionWarning(
					"missing-xerox-room",
					$"Could not resolve xerox source room #{xeroxVnum}."));
				continue;
			}

			state.EffectiveDescription = xeroxRoom.Description;
			state.EffectiveWeather = xeroxRoom.Weather;
			state.XeroxResolved = true;
		}
	}

	private static void MapTerrainAndOutdoors(IReadOnlyList<RoomState> roomStates, IReadOnlyDictionary<int, RpiRoomRecord> roomByVnum)
	{
		foreach (var state in roomStates)
		{
			state.TerrainName = DetermineTerrain(state.Source, state.EffectiveDescription, roomByVnum);
			state.OutdoorsTypeValue = DetermineOutdoorsTypeValue(state.Source, state.TerrainName, state.EffectiveDescription);
			state.OutdoorsTypeName = ((CellOutdoorsType)state.OutdoorsTypeValue).Describe();
			state.SafeQuit = state.Source.RoomFlags.HasFlag(RpiRoomFlags.SafeQuit);
		}
	}

	private static void AssignCoordinates(IReadOnlyList<ZoneState> zones, IReadOnlyDictionary<int, RpiRoomRecord> roomByVnum)
	{
		foreach (var zone in zones)
		{
			var coords = CalculateZoneCoordinates(zone, roomByVnum);
			foreach (var room in zone.Rooms)
			{
				room.Coordinates = coords[room.Source.Vnum];
			}
		}
	}

	private static List<ConvertedRoomExitDefinition> BuildExits(
		IReadOnlyList<RoomState> roomStates,
		IReadOnlyDictionary<int, RpiRoomRecord> roomByVnum)
	{
		var stateByVnum = roomStates.ToDictionary(x => x.Source.Vnum);
		HashSet<string> processed = [];
		List<ConvertedRoomExitDefinition> exits = [];

		foreach (var state in roomStates.OrderBy(x => x.Source.Zone).ThenBy(x => x.Source.Vnum))
		{
			var room = state.Source;
			foreach (var exit in room.Exits
				         .OrderBy(x => x.Direction.SortOrder())
				         .ThenBy(x => x.DestinationVnum))
			{
				var sideKey = $"{room.Vnum}:{(int)exit.Direction}";
				if (!processed.Add(sideKey))
				{
					continue;
				}

				if (!roomByVnum.TryGetValue(exit.DestinationVnum, out var destinationRoom))
				{
					state.Warnings.Add(new RoomConversionWarning(
						"missing-exit-destination",
						$"Exit {exit.Direction.Describe()} points to missing room #{exit.DestinationVnum}."));
					continue;
				}

				var reverseExit = destinationRoom.Exits.FirstOrDefault(x =>
					x.Direction == exit.Direction.Opposite() &&
					x.DestinationVnum == room.Vnum);
				if (reverseExit is not null)
				{
					processed.Add($"{destinationRoom.Vnum}:{(int)reverseExit.Direction}");
				}

				var side1 = BuildExitSide(room, exit);
				var side2 = reverseExit is not null
					? BuildExitSide(destinationRoom, reverseExit)
					: BuildStubSide(destinationRoom.Vnum, exit.Direction.Opposite());
				var warnings = new List<RoomConversionWarning>();

				var acceptsDoor = side1.DoorType != RpiRoomDoorType.None || side2.DoorType != RpiRoomDoorType.None;
				var doorSize = acceptsDoor
					? (int?)InferDoorSize(room, destinationRoom, side1, side2)
					: null;
				var maximumSizeToEnter = acceptsDoor ? doorSize!.Value : (int)SizeCategory.Titanic;
				var maximumSizeToEnterUpright = acceptsDoor
					? InferMaximumUprightSize(doorSize!.Value, room, destinationRoom, side1, side2)
					: (int)SizeCategory.Titanic;

				var isVertical = side1.Direction is RpiRoomDirection.Up or RpiRoomDirection.Down;
				var isClimbExit = isVertical && (room.RoomFlags.HasFlag(RpiRoomFlags.Climb) ||
				                                 destinationRoom.RoomFlags.HasFlag(RpiRoomFlags.Climb));
				var fallMetadata = DetermineFallMetadata(room, destinationRoom, side1, side2);

				if (side1.SearchDifficulty is not null && !side1.Hidden)
				{
					warnings.Add(new RoomConversionWarning(
						"secret-search-without-hidden-exit",
						$"Room #{room.Vnum} has Q data for {side1.Direction.Describe()} without a hidden exit marker."));
				}

				if (reverseExit is null)
				{
					warnings.Add(new RoomConversionWarning(
						"one-sided-exit",
						$"Room #{room.Vnum} has a one-sided {side1.Direction.Describe()} exit to #{destinationRoom.Vnum}."));
				}

				exits.Add(new ConvertedRoomExitDefinition(
					BuildExitKey(room.Vnum, destinationRoom.Vnum, side1.Direction, side2.Direction),
					room.Vnum,
					destinationRoom.Vnum,
					side1,
					side2,
					acceptsDoor,
					doorSize,
					maximumSizeToEnter,
					maximumSizeToEnterUpright,
					isClimbExit,
					(int)Difficulty.Normal,
					fallMetadata.fallFrom,
					fallMetadata.fallTo,
					warnings));
			}
		}

		AppendUnresolvedSecretWarnings(roomStates);

		return exits
			.OrderBy(x => x.RoomVnum1)
			.ThenBy(x => x.RoomVnum2)
			.ThenBy(x => x.Side1.Direction.SortOrder())
			.ToList();
	}

	private static void AppendUnresolvedSecretWarnings(IReadOnlyList<RoomState> roomStates)
	{
		foreach (var state in roomStates)
		{
			var exitDirections = state.Source.Exits.Select(x => x.Direction).ToHashSet();
			foreach (var secret in state.Source.Secrets)
			{
				if (exitDirections.Contains(secret.Direction))
				{
					continue;
				}

				state.Warnings.Add(new RoomConversionWarning(
					"orphan-secret",
					$"Secret/search metadata exists for {secret.Direction.Describe()} without a matching exit."));
			}
		}
	}

	private static ConvertedRoomExitSideDefinition BuildExitSide(RpiRoomRecord room, RpiRoomExitRecord exit)
	{
		var secret = room.Secrets.FirstOrDefault(x => x.Direction == exit.Direction);
		return new ConvertedRoomExitSideDefinition(
			room.Vnum,
			exit.Direction,
			true,
			NormaliseKeywords(exit.Keyword),
			DeterminePrimaryKeyword(exit.Keyword),
			exit.GeneralDescription.Trim(),
			exit.IsHidden,
			exit.IsTrapped,
			secret?.Difficulty,
			secret?.SearchText,
			exit.KeyVnum,
			exit.PickPenalty,
			exit.IsPickProof,
			exit.DoorType);
	}

	private static ConvertedRoomExitSideDefinition BuildStubSide(int roomVnum, RpiRoomDirection direction)
	{
		return new ConvertedRoomExitSideDefinition(
			roomVnum,
			direction,
			false,
			string.Empty,
			null,
			string.Empty,
			false,
			false,
			null,
			null,
			-1,
			0,
			false,
			RpiRoomDoorType.None);
	}

	private static (int? fallFrom, int? fallTo) DetermineFallMetadata(
		RpiRoomRecord room,
		RpiRoomRecord destinationRoom,
		ConvertedRoomExitSideDefinition side1,
		ConvertedRoomExitSideDefinition side2)
	{
		if (side1.Direction == RpiRoomDirection.Down && room.RoomFlags.HasFlag(RpiRoomFlags.Fall))
		{
			return (room.Vnum, destinationRoom.Vnum);
		}

		if (side2.Direction == RpiRoomDirection.Down && destinationRoom.RoomFlags.HasFlag(RpiRoomFlags.Fall))
		{
			return (destinationRoom.Vnum, room.Vnum);
		}

		return (null, null);
	}

	private static IReadOnlyDictionary<int, RoomCoordinate> CalculateZoneCoordinates(
		ZoneState zone,
		IReadOnlyDictionary<int, RpiRoomRecord> roomByVnum)
	{
		var roomStates = zone.Rooms.ToDictionary(x => x.Source.Vnum);
		Dictionary<int, RoomCoordinate> coordinates = [];
		HashSet<string> occupied = [];
		var remaining = roomStates.Keys.ToHashSet();
		var componentIndex = 0;

		while (remaining.Count > 0)
		{
			var seedVnum = ChooseSeedVnum(zone, remaining);
			var intendedSeed = componentIndex == 0
				? new RoomCoordinate(0, 0, 0)
				: new RoomCoordinate(
					coordinates.Count == 0 ? 0 : coordinates.Values.Max(x => x.X) + 10,
					0,
					0);
			var seedCoordinate = FindNearestFreeCoordinate(intendedSeed, occupied);
			coordinates[seedVnum] = seedCoordinate;
			occupied.Add(CoordinateKey(seedCoordinate));
			remaining.Remove(seedVnum);

			Queue<int> queue = new();
			queue.Enqueue(seedVnum);

			while (queue.Count > 0)
			{
				var currentVnum = queue.Dequeue();
				var currentCoordinate = coordinates[currentVnum];
				var currentRoom = roomByVnum[currentVnum];
				foreach (var exit in currentRoom.Exits
					         .Where(x => roomStates.ContainsKey(x.DestinationVnum))
					         .OrderBy(x => x.Direction.SortOrder())
					         .ThenBy(x => x.DestinationVnum))
				{
					if (coordinates.ContainsKey(exit.DestinationVnum))
					{
						continue;
					}

					var delta = exit.Direction.Delta();
					var intended = new RoomCoordinate(
						currentCoordinate.X + delta.X,
						currentCoordinate.Y + delta.Y,
						currentCoordinate.Z + delta.Z);
					var assigned = FindNearestFreeCoordinate(intended, occupied);
					if (assigned != intended)
					{
						roomStates[exit.DestinationVnum].Warnings.Add(new RoomConversionWarning(
							"layout-conflict",
							$"Coordinate {FormatCoordinate(intended)} was occupied; placed room at {FormatCoordinate(assigned)} instead."));
					}

					coordinates[exit.DestinationVnum] = assigned;
					occupied.Add(CoordinateKey(assigned));
					remaining.Remove(exit.DestinationVnum);
					queue.Enqueue(exit.DestinationVnum);
				}
			}

			componentIndex++;
		}

		return coordinates;
	}

	private static int ChooseSeedVnum(ZoneState zone, IReadOnlySet<int> remaining)
	{
		foreach (var sourceZone in zone.SourceZones.OrderBy(x => x))
		{
			var baseVnum = sourceZone * 1000;
			if (remaining.Contains(baseVnum))
			{
				return baseVnum;
			}
		}

		return remaining.Min();
	}

	private static RoomCoordinate FindNearestFreeCoordinate(RoomCoordinate intended, IReadOnlySet<string> occupied)
	{
		if (!occupied.Contains(CoordinateKey(intended)))
		{
			return intended;
		}

		for (var distance = 1; distance < 2048; distance++)
		{
			foreach (var candidate in EnumerateCandidates(intended, distance))
			{
				if (!occupied.Contains(CoordinateKey(candidate)))
				{
					return candidate;
				}
			}
		}

		return intended;
	}

	private static IEnumerable<RoomCoordinate> EnumerateCandidates(RoomCoordinate intended, int distance)
	{
		for (var x = -distance; x <= distance; x++)
		{
			var y = distance - Math.Abs(x);
			if (y == 0)
			{
				yield return new RoomCoordinate(intended.X + x, intended.Y, intended.Z);
				continue;
			}

			yield return new RoomCoordinate(intended.X + x, intended.Y - y, intended.Z);
			yield return new RoomCoordinate(intended.X + x, intended.Y + y, intended.Z);
		}
	}

	private static string CoordinateKey(RoomCoordinate coordinate)
	{
		return $"{coordinate.X},{coordinate.Y},{coordinate.Z}";
	}

	private static string FormatCoordinate(RoomCoordinate coordinate)
	{
		return $"({coordinate.X}, {coordinate.Y}, {coordinate.Z})";
	}

	private static string DetermineZoneName(
		IReadOnlyList<RpiRoomRecord> rooms,
		IReadOnlyList<int> sourceZones,
		ICollection<RoomConversionWarning> warnings)
	{
		foreach (var zone in sourceZones.OrderBy(x => x))
		{
			var baseRoom = rooms
				.Where(x => x.Zone == zone)
				.OrderBy(x => x.Vnum == zone * 1000 ? 0 : 1)
				.ThenBy(x => x.Vnum)
				.FirstOrDefault();
			if (baseRoom is null)
			{
				continue;
			}

			var cleaned = CleanBaseRoomName(baseRoom.Name);
			if (!string.IsNullOrWhiteSpace(cleaned))
			{
				return cleaned;
			}
		}

		var fallback = $"Zone {sourceZones[0].ToString("D2", CultureInfo.InvariantCulture)}";
		warnings.Add(new RoomConversionWarning(
			"zone-name-fallback",
			$"Fell back to '{fallback}' because no base-room zone name could be derived."));
		return fallback;
	}

	private static string CleanBaseRoomName(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			return string.Empty;
		}

		var match = BaseRoomRegex.Match(name.Trim());
		if (match.Success)
		{
			return match.Groups["name"].Value.Trim();
		}

		return name.Trim();
	}

	private static string DetermineTerrain(
		RpiRoomRecord room,
		string effectiveDescription,
		IReadOnlyDictionary<int, RpiRoomRecord> roomByVnum)
	{
		var text = $"{room.Name} {effectiveDescription}";
		var neighbours = room.Exits
			.Select(x => roomByVnum.TryGetValue(x.DestinationVnum, out var destination) ? destination : null)
			.Where(x => x is not null)
			.Cast<RpiRoomRecord>()
			.ToList();

		return room.SectorType switch
		{
			RpiRoomSectorType.Inside => "Hall",
			RpiRoomSectorType.City => "Urban Street",
			RpiRoomSectorType.Road => ContainsAny(text, "cobble", "cobblestone", "stone", "paved", "causeway", "flagstone")
				? "Cobblestone Road"
				: "Compacted Dirt Road",
			RpiRoomSectorType.Trail => "Trail",
			RpiRoomSectorType.Field => "Field",
			RpiRoomSectorType.Woods => "Broadleaf Forest",
			RpiRoomSectorType.Forest => ContainsAny(text, "pine", "fir", "spruce", "cedar", "yew", "needle")
				? "Temperate Coniferous Forest"
				: "Broadleaf Forest",
			RpiRoomSectorType.Hills => "Hills",
			RpiRoomSectorType.Mountain => "Mountainside",
			RpiRoomSectorType.Swamp => ContainsAny(text, "tree", "trees", "cypress", "mangrove", "forest")
				? "Swamp Forest"
				: "Temperate Freshwater Swamp",
			RpiRoomSectorType.Dock => DetermineDockTerrain(text, neighbours),
			RpiRoomSectorType.CrowsNest => "Rooftop",
			RpiRoomSectorType.Pasture => "Pasture",
			RpiRoomSectorType.Heath => "Heath",
			RpiRoomSectorType.Pit => ContainsAny(text, "cave", "cavern", "catacomb", "crypt", "tunnel", "mine")
				? "Cave"
				: "Dungeon",
			RpiRoomSectorType.LeanTo => "Cave Entrance",
			RpiRoomSectorType.Lake => "Lake",
			RpiRoomSectorType.River => "River",
			RpiRoomSectorType.Ocean => "Ocean",
			RpiRoomSectorType.Reef => "Reef",
			RpiRoomSectorType.Underwater => DetermineUnderwaterTerrain(text, neighbours),
			_ => "Hall",
		};
	}

	private static string DetermineDockTerrain(string text, IReadOnlyCollection<RpiRoomRecord> neighbours)
	{
		if (ContainsAny(text, "ocean", "sea", "surf", "harbor", "harbour", "coast", "shore"))
		{
			return "Ocean Surf";
		}

		if (ContainsAny(text, "lake", "lakeshore", "shoreline") ||
		    neighbours.Any(x => x.SectorType == RpiRoomSectorType.Lake))
		{
			return "Lake Shore";
		}

		return "Riverbank";
	}

	private static string DetermineUnderwaterTerrain(string text, IReadOnlyCollection<RpiRoomRecord> neighbours)
	{
		if (ContainsAny(text, "ocean", "sea", "surf", "reef", "tide"))
		{
			return "Deep Ocean";
		}

		if (ContainsAny(text, "river", "current", "torrent", "rapids", "stream") ||
		    neighbours.Any(x => x.SectorType == RpiRoomSectorType.River))
		{
			return "Deep River";
		}

		if (ContainsAny(text, "pipe", "sewer", "tunnel", "cavern", "crypt", "underground", "cesspool"))
		{
			return "Underground Water";
		}

		if (neighbours.Any(x => x.SectorType == RpiRoomSectorType.Ocean || x.SectorType == RpiRoomSectorType.Reef))
		{
			return "Deep Ocean";
		}

		if (neighbours.Any(x => x.SectorType == RpiRoomSectorType.Lake))
		{
			return "Deep Lake";
		}

		return "Deep Lake";
	}

	private static int DetermineOutdoorsTypeValue(RpiRoomRecord room, string terrainName, string effectiveDescription)
	{
		var defaultType = terrainName switch
		{
			"Hall" or "Dungeon" or "Cave" => CellOutdoorsType.Indoors,
			"Rooftop" or "Gatehouse" or "Battlement" or "Cave Entrance" => CellOutdoorsType.IndoorsClimateExposed,
			_ => CellOutdoorsType.Outdoors,
		};

		if (room.RoomFlags.HasFlag(RpiRoomFlags.Indoors))
		{
			var text = $"{room.Name} {effectiveDescription}";
			return (int)(ContainsAny(text, "rooftop", "gatehouse", "battlement", "open-walled", "open walled", "balcony")
				? CellOutdoorsType.IndoorsClimateExposed
				: CellOutdoorsType.Indoors);
		}

		return (int)defaultType;
	}

	private static IReadOnlyList<string> DescribeFlags(RpiRoomFlags flags)
	{
		return Enum.GetValues<RpiRoomFlags>()
			.Where(x => x != RpiRoomFlags.None && flags.HasFlag(x))
			.Select(x => x.ToString())
			.ToList();
	}

	private static string NormaliseKeywords(string? keyword)
	{
		return string.Join(
			' ',
			(keyword ?? string.Empty)
				.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
				.Distinct(StringComparer.OrdinalIgnoreCase));
	}

	private static string? DeterminePrimaryKeyword(string? keyword)
	{
		return (keyword ?? string.Empty)
			.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.FirstOrDefault();
	}

	private static int InferDoorSize(
		RpiRoomRecord sourceRoom,
		RpiRoomRecord destinationRoom,
		ConvertedRoomExitSideDefinition side1,
		ConvertedRoomExitSideDefinition side2)
	{
		var text = string.Join(
			' ',
			new[]
			{
				sourceRoom.Name,
				sourceRoom.Description,
				destinationRoom.Name,
				destinationRoom.Description,
				side1.Keywords,
				side1.Description,
				side2.Keywords,
				side2.Description,
			});

		if (ContainsAny(text, "great gate", "massive gate", "fortress gate"))
		{
			return (int)SizeCategory.Enormous;
		}

		if (ContainsAny(text, "double doors", "carriage doors"))
		{
			return (int)SizeCategory.VeryLarge;
		}

		if (ContainsAny(text, "trapdoor", "hatch"))
		{
			return (int)SizeCategory.Small;
		}

		if (ContainsAny(text, "gate", "portcullis"))
		{
			return (int)SizeCategory.Huge;
		}

		return (int)SizeCategory.Large;
	}

	private static int InferMaximumUprightSize(
		int doorSize,
		RpiRoomRecord sourceRoom,
		RpiRoomRecord destinationRoom,
		ConvertedRoomExitSideDefinition side1,
		ConvertedRoomExitSideDefinition side2)
	{
		var text = string.Join(
			' ',
			new[]
			{
				sourceRoom.Name,
				sourceRoom.Description,
				destinationRoom.Name,
				destinationRoom.Description,
				side1.Keywords,
				side1.Description,
				side2.Keywords,
				side2.Description,
			});

		if (ContainsAny(text, "trapdoor", "hatch"))
		{
			return (int)SizeCategory.Small;
		}

		if (ContainsAny(text, "crawl", "crouch", "low", "tight", "narrow", "squeeze"))
		{
			return Math.Min(doorSize, (int)SizeCategory.Normal);
		}

		return doorSize;
	}

	private static string BuildExitKey(
		int roomVnum1,
		int roomVnum2,
		RpiRoomDirection direction1,
		RpiRoomDirection direction2)
	{
		return $"{roomVnum1}:{direction1.Describe().ToLowerInvariant()}->{roomVnum2}:{direction2.Describe().ToLowerInvariant()}";
	}

	private static bool ContainsAny(string? text, params string[] needles)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		return needles.Any(needle => text.Contains(needle, StringComparison.OrdinalIgnoreCase));
	}
}

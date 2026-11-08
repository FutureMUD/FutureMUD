#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private static readonly string[] SectorTerrains =
	[
		"Hallway", "Urban Street", "Field", "Sandy Desert", "Hills", "Mountainside", "Deep Lake",
		"Gas Giant Atmosphere", "Astral Void", "Woodland", "Tropical Dry Forest", "Salt Flat", "Thorn Scrub",
		"Ruins", "Cavern", "Dirt Road", "Shallow Lake", "Cavern", "Sewer", "Field", "Grasslands",
		"Lava Field", "Deep Lake", "Cavern", "Shadow Realm", "Gas Giant Atmosphere", "Astral Expanse"
	];
	private static readonly string[] RequiredBodies =
	[
		"Toed Quadruped", "Ungulate", "Avian", "Vermiform", "Serpentine", "Cephalopod",
		"Insectoid", "Winged Insectoid", "Beetle", "Centipede", "Arachnid", "Scorpion", "Reptilian"
	];
	private readonly Dictionary<(int Room, int Direction), Exit> _exitMap = new();
	private readonly List<(Exit Exit, ArmWorldRoom Room, ArmWorldExit Side, ArmWorldExit? Reverse)> _doors = [];
	private readonly Dictionary<(int Room, int Direction), int> _doorResetStates = new();
	private static readonly CardinalDirection[] Directions =
		[CardinalDirection.North, CardinalDirection.East, CardinalDirection.South, CardinalDirection.West,
		 CardinalDirection.Up, CardinalDirection.Down];
	private static readonly int[] ReverseDirections = [2, 3, 0, 1, 5, 4];

	internal static ArmWorldExtra[] RoomInspectionDescriptions(ArmWorldRoom source)
	{
		// Keep authored room details first when a source keyword is ambiguous. Directional
		// aliases also let players inspect the passage without selecting its installed door.
		string[] directionKeywords = ["north northward", "east eastward", "south southward",
			"west westward", "up upward", "down downward"];
		return source.ExtraDescriptions.Concat(source.Exits
			.Where(exit => exit.Direction is >= 0 and <= 5 && !string.IsNullOrWhiteSpace(exit.Description))
			.Select(exit => new ArmWorldExtra($"{directionKeywords[exit.Direction]} {exit.Keyword}".TrimEnd(),
				exit.Description.TrimEnd())))
			.ToArray();
	}

	private void InstallRooms()
	{
		foreach (var command in _source.Resets.Where(x => x.Opcode == "D" && x.ReferenceIssues.Length == 0))
			_doorResetStates[(command.Arguments[1], command.Arguments[2])] = command.Arguments[3];
		var templateZone = _db.Zones.OrderBy(x => x.Id).First();
		if (ArmageddonWorldfileLighting.AttachSky(_db, templateZone.ShardId))
			Adapt("world sky", 0, "Suk-Krath, Jihae and Lirathu attached to the imported world's shard using their authored Armageddon clock and calendar.");
		else
			Adapt("world sky", 0, "Armageddon celestial package is absent; configure a sky before relying on natural daylight.");
		var timezones = _db.ZonesTimezones.AsNoTracking().Where(x => x.ZoneId == templateZone.Id).ToArray();
		foreach (var source in _source.Zones)
		{
			var zone = new Zone
			{
				Id = Next<Zone>(), Name = source.Name.Trim(), ShardId = templateZone.ShardId,
				Latitude = templateZone.Latitude, Longitude = templateZone.Longitude, Elevation = templateZone.Elevation,
				WeatherControllerId = templateZone.WeatherControllerId
			};
			_db.Zones.Add(zone);
			_zones.Add(source.Index, zone);
			foreach (var timezone in timezones)
				_db.ZonesTimezones.Add(new() { ZoneId = zone.Id, ClockId = timezone.ClockId, TimezoneId = timezone.TimezoneId });
			Record("zone." + source.Index, nameof(Zone), zone.Id, null, source);
		}
		_overlays = new RoomOverlayPackage
		{
			Id = Next<RoomOverlayPackage>(), RevisionNumber = 0, Name = "Armageddon Worldfile", EditableItem = Approval()
		};
		_db.RoomOverlayPackages.Add(_overlays);
		var terrains = _db.Terrains.AsNoTracking().ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		var nativeAir = (Terrain)_db.Entry(terrains["Gas Giant Atmosphere"]).CurrentValues.ToObject();
		nativeAir.Id = Next<Terrain>();
		nativeAir.Name = "Armageddon Air";
		nativeAir.DefaultTerrain = false;
		nativeAir.AtmosphereId = terrains["Sandy Desert"].AtmosphereId;
		nativeAir.AtmosphereType = terrains["Sandy Desert"].AtmosphereType;
		_db.Terrains.Add(nativeAir);
		Terrain? nativeSilt = null;
		if (_source.Rooms.Any(x => x.Sector == 6))
		{
			nativeSilt = (Terrain)_db.Entry(terrains["Sandy Desert"]).CurrentValues.ToObject();
			nativeSilt.Id = Next<Terrain>();
			nativeSilt.Name = "Armageddon Silt";
			nativeSilt.DefaultTerrain = false;
			nativeSilt.TerrainBehaviourMode = "silt Titanic opaque norun";
			_db.Terrains.Add(nativeSilt);
			Record("terrain.silt", nameof(Terrain), nativeSilt.Id, null,
				new { SourceSector = 6, Disposition = "Native opaque silt immersion and vehicle-silt environment; Titanic immersion is a configured categorical depth approximation." });
		}
		foreach (var source in _source.Rooms)
		{
			if (source.Sector < 0 || source.Sector >= SectorTerrains.Length)
				throw new InvalidOperationException($"{source.Source}: unknown sector {source.Sector}.");
			var terrain = source.Sector == 6 ? nativeSilt! : source.Sector is 7 or 25 ? nativeAir : terrains[SectorTerrains[source.Sector]];
			var room = new Room
			{
				Id = Next<Room>(), UniqueName = Key(source.Vnum), ZoneId = _zones[source.Zone].Id,
				EffectData = "<Effects/>", X = 0, Y = 0, Z = 0
			};
			var flags = unchecked((uint)source.Flags);
			var inspectionDescriptions = RoomInspectionDescriptions(source);
			if (inspectionDescriptions.Length > 0)
				room.EffectData = new XElement("Effects", new XElement("Effect", new XElement("ApplicabilityProg", 0),
					new XElement("Type", "KeywordDescriptions"), new XElement("Original", 0), new XElement("Remaining", 0),
					new XElement("Effect", KeywordDescriptionsXml(inspectionDescriptions)))).ToString();
			var restrictions = RestrictionsFor(source.Flags);
			if (restrictions != RoomRestrictions.None || (flags & (1u << 8)) != 0)
				AddRoomEffect(room, "RoomPolicy", new XElement("Effect",
					new XElement("Restrictions", (int)restrictions),
					new XElement("MaximumOccupants", (flags & (1u << 8)) != 0 ? 1 : 0)));
			InstallRoomSpecials(source, room);
			var overlay = new RoomOverlay
			{
				Id = Next<RoomOverlay>(), Name = Key(source.Vnum), Room = room, RoomId = room.Id,
				RoomName = source.Name, RoomDescription = source.Description.TrimEnd(),
				RoomOverlayPackageId = _overlays.Id, RoomOverlayPackageRevisionNumber = 0,
				TerrainId = terrain.Id, AtmosphereId = terrain.AtmosphereId, AtmosphereType = terrain.AtmosphereType,
				OutdoorsType = (flags & (1u << 3)) != 0 ? (int)RoomOutdoorsType.Indoors : terrain.DefaultRoomOutdoorsType,
				AmbientLightFactor = (flags & 1u) != 0 ? 0.0 : 1.0,
				AddedLight = ArmageddonWorldfileLighting.AddedLight(source.Zone, source.Sector, source.Flags),
				// RFL_SAFE gates quitting (other.c), not combat. Do not infer Peaceful from its name.
				SafeQuit = (flags & (1u << 4)) != 0
			};
			_db.Rooms.Add(room);
			_db.RoomOverlays.Add(overlay);
			_rooms.Add(source.Vnum, room);
			Record("room." + Key(source.Vnum), nameof(Room), room.Id, null, source);
		}
		Flush();
		InstallObservationLinks();
		foreach (var room in _rooms.Values)
			room.CurrentOverlayId = room.RoomOverlays.Single().Id;
		foreach (var pair in _zones)
			pair.Value.DefaultRoomId = _rooms.Values.Where(x => x.ZoneId == pair.Value.Id).Select(x => (long?)x.Id).FirstOrDefault();
		var sourceRooms = _source.Rooms.ToDictionary(x => x.Vnum);
		foreach (var source in _source.Rooms)
		foreach (var side in source.Exits)
		{
			if (_exitMap.ContainsKey((source.Vnum, side.Direction))) continue;
			if (side.Direction is < 0 or > 5 || !_rooms.TryGetValue(side.Destination, out var destination))
				throw new InvalidOperationException($"Unqualified exit in {source.Source}.");
			var reverse = side.Destination == source.Vnum ? null : sourceRooms[side.Destination].Exits.FirstOrDefault(x =>
				x.Direction == ReverseDirections[side.Direction] && x.Destination == source.Vnum &&
				x.Key == side.Key && x.Flags == side.Flags);
			var exit = new Exit
			{
				Id = Next<Exit>(), RoomId1 = _rooms[source.Vnum].Id, RoomId2 = destination.Id,
				Direction1 = (int)Directions[side.Direction], Direction2 = (int)Directions[ReverseDirections[side.Direction]],
				Keywords1 = side.Keyword, Keywords2 = reverse?.Keyword ?? "", PrimaryKeyword1 = side.Keyword.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "",
				PrimaryKeyword2 = reverse?.Keyword.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "",
				AcceptsDoor = side.Flags != 0 || HasDoorReset(source.Vnum, side.Direction),
				DoorSize = (int)SizeCategory.Titanic, MaximumSizeToEnter = (int)SizeCategory.Titanic,
				MaximumSizeToEnterUpright = (int)SizeCategory.Titanic, TimeMultiplier = 1.0, BlockedLayers = "",
				InboundDescription1 = "", InboundDescription2 = "", OutboundDescription1 = "", OutboundDescription2 = "",
				InboundTarget1 = "", InboundTarget2 = "", OutboundTarget1 = "", OutboundTarget2 = "", Verb1 = "", Verb2 = ""
			};
			var originFalling = (unchecked((uint)source.Flags) & (1u << 11)) != 0;
			var destinationSource = sourceRooms[side.Destination];
			var destinationFalling = (unchecked((uint)destinationSource.Flags) & (1u << 11)) != 0;
			if (side.Direction == 4 && destinationFalling || side.Direction == 5 && originFalling)
			{
				var upper = side.Direction == 4 ? destinationSource : source;
				exit.FallRoom = side.Direction == 4 ? _rooms[source.Vnum].Id : destination.Id;
				exit.IsClimbExit = (unchecked((uint)upper.Flags) & (1u << 14)) == 0;
				exit.ClimbDifficulty = (int)Difficulty.Normal;
				_counts["native climb/fall passages"] = _counts.GetValueOrDefault("native climb/fall passages") + 1;
			}
			_db.Exits.Add(exit);
			_db.RoomOverlaysExits.Add(new() { RoomOverlayId = _rooms[source.Vnum].RoomOverlays.Single().Id, ExitId = exit.Id });
			_exitMap.Add((source.Vnum, side.Direction), exit);
			if (reverse is not null && !_exitMap.ContainsKey((side.Destination, reverse.Direction)))
			{
				_db.RoomOverlaysExits.Add(new() { RoomOverlayId = destination.RoomOverlays.Single().Id, ExitId = exit.Id });
				_exitMap.Add((side.Destination, reverse.Direction), exit);
			}
			if (exit.AcceptsDoor) _doors.Add((exit, source, side, reverse));
		}
		_counts["rooms"] = _rooms.Count;
		_counts["directed exits"] = _exitMap.Count;
		InstallMapCoordinates();
		Adapt("environment", 0, "Source weather, unknown coordinates and non-equivalent room policies remain in provenance; native terrain/environment bindings are explicit approximations.");
		Adapt("sector", 6, "Silt uses native opaque silt immersion and the native silt vehicle environment; categorical depth is Titanic, with source physiology differences retained for review.");
		Flush();
	}

	internal static RoomRestrictions RestrictionsFor(int sourceFlags)
	{
		var flags = unchecked((uint)sourceFlags);
		return ((flags & (1u << 2)) != 0 ? RoomRestrictions.NpcWandering : 0) |
		       ((flags & (1u << 1)) != 0 ? RoomRestrictions.Lethal : 0) |
		       ((flags & (1u << 7)) != 0 ? RoomRestrictions.Magic : 0) |
		       ((flags & (1u << 14)) != 0 ? RoomRestrictions.Climbing : 0) |
		       ((flags & (1u << 15)) != 0 ? RoomRestrictions.Mounts : 0) |
		       ((flags & (1u << 16)) != 0 ? RoomRestrictions.Vehicles : 0) |
		       ((flags & (1u << 17)) != 0 ? RoomRestrictions.Flight : 0);
	}

	private void ConfigureTunnelCosts()
	{
		static string NormaliseRaceName(string name) => name.Split(" (", StringSplitOptions.None)[0]
			.Replace("-", "").Replace("_", "").Replace(" ", "").ToLowerInvariant();
		var giantIds = _db.Races.AsEnumerable().Where(x => NormaliseRaceName(x.Name) == "halfgiant")
			.Select(x => x.Id).Distinct().ToArray();
		var predicate = giantIds.Length == 0 ? "false" : string.Join(" or ", giantIds.Select(id => $"@character.race.id == {id}"));
		var prog = Prog("ArmageddonWorldfileTunnelOccupancy", $"if (isincorporeal(@character))\n\treturn 0\nend if\nif ({predicate})\n\treturn 2\nend if\nreturn 1",
			MudSharp.FutureProg.ProgVariableTypes.Number, ("character", MudSharp.FutureProg.ProgVariableTypes.Character));
		foreach (var source in _source.Rooms.Where(x => (unchecked((uint)x.Flags) & (1u << 8)) != 0))
		{
			var room = _rooms[source.Vnum];
			var effects = XElement.Parse(room.EffectData);
			effects.Elements("Effect").Single(x => x.Element("Type")?.Value == "RoomPolicy")
				.Element("Effect")!.SetElementValue("OccupancyCostProg", prog.Id);
			room.EffectData = effects.ToString();
		}
	}

	private static void AddRoomEffect(Room room, string type, XElement definition)
	{
		var effects = XElement.Parse(room.EffectData);
		effects.Add(new XElement("Effect", new XElement("ApplicabilityProg", 0), new XElement("Type", type),
			new XElement("Original", 0), new XElement("Remaining", 0), definition));
		room.EffectData = effects.ToString();
	}

	private void InstallObservationLinks()
	{
		var count = 0;
		foreach (var observer in _source.Rooms)
		foreach (var link in observer.ObservationLinks)
		{
			var target = link.GetProperty("Room").GetInt32();
			if (link.GetProperty("Type").GetString() != "w" || !_rooms.TryGetValue(target, out var watched))
			{
				Adapt("observation link", observer.Vnum, $"Unsupported or unavailable observation target {target}; source link retained.");
				continue;
			}
			AddRoomEffect(watched, "RoomObservation", new XElement("Effect",
				new XElement("ObserverRoomId", _rooms[observer.Vnum].Id),
				new XElement("Prefix", link.GetProperty("Text").GetString() ?? "")));
			count++;
		}
		_counts["visual observation links"] = count;
	}

	private void InstallRoomSpecials(ArmWorldRoom source, Room room)
	{
		foreach (var program in source.Programs)
		{
			if (program == "C south storms_edge")
				AddRoomEffect(room, "RoomExitPolicy", ExitPolicyXml(CardinalDirection.South,
					"The wind is too strong to allow further travel south.", false));
			else if (program == "C up tor_upper_stopper")
				AddRoomEffect(room, "RoomExitPolicy", ExitPolicyXml(CardinalDirection.Up,
					"You try to go up but fall.", true));
			else continue;
			_counts["active room special associations"] = _counts.GetValueOrDefault("active room special associations") + 1;
			Record($"room-special.{source.Vnum}.{program.Split(' ').Last()}", nameof(Room), room.Id, null,
				new { Source = source.Source, Program = program, Disposition = "Native direction-specific room exit policy; source Tor levitation uses native flight. Access is checked during path planning and execution." });
		}
	}

	private static XElement ExitPolicyXml(CardinalDirection direction, string message, bool flight) => new("Effect",
		new XElement("Direction", (int)direction), new XElement("Message", message),
		new XElement("AccessProg", 0), new XElement("RequiresFlight", flight));

	private bool HasDoorReset(int vnum, int direction) => _doorResetStates.ContainsKey((vnum, direction));

	private void InstallMapCoordinates()
	{
		var adjacency = _source.Rooms.ToDictionary(x => x.Vnum, _ => new List<(int Room, int X, int Y, int Z)>());
		var steps = new (int X, int Y, int Z)[] { (0, 1, 0), (1, 0, 0), (0, -1, 0), (-1, 0, 0), (0, 0, 1), (0, 0, -1) };
		foreach (var source in _source.Rooms)
		foreach (var exit in source.Exits)
		{
			var step = steps[exit.Direction];
			adjacency[source.Vnum].Add((exit.Destination, step.X, step.Y, step.Z));
			adjacency[exit.Destination].Add((source.Vnum, -step.X, -step.Y, -step.Z));
		}
		var visited = new HashSet<int>();
		var component = 0;
		var conflicts = 0;
		foreach (var start in _rooms.Keys.OrderBy(x => x))
		{
			if (!visited.Add(start)) continue;
			_rooms[start].X = component++ * 10000;
			var pending = new Queue<int>();
			pending.Enqueue(start);
			while (pending.TryDequeue(out var current))
			{
				var origin = _rooms[current];
				foreach (var (target, x, y, z) in adjacency[current])
				{
					var room = _rooms[target];
					if (visited.Add(target))
					{
						(room.X, room.Y, room.Z) = (origin.X + x, origin.Y + y, origin.Z + z);
						pending.Enqueue(target);
					}
					else if ((room.X, room.Y, room.Z) != (origin.X + x, origin.Y + y, origin.Z + z)) conflicts++;
				}
			}
		}
		Adapt("map coordinates", 0, $"Deterministic breadth-first layout has {component} disconnected components and {conflicts} directed coordinate contradictions. Exit topology remains authoritative; coordinates are reconstructed, not recovered geography.");
	}
}

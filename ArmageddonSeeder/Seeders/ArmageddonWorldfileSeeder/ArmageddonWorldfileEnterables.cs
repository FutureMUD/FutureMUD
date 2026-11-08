#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.Vehicles;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	// Reviewed transport definitions. Wrecks, portals and props sharing ITEM_WAGON are deliberately absent.
	internal static readonly HashSet<int> VehicleVnums =
	[
		800, 903, 904, 905, 1601, 1606, 2045, 2148, 2552, 3029, 3062, 3150, 7501,
		13401, 13402, 13403, 13404, 13405, 13407, 13408, 13409, 15205, 17012, 19035,
		19537, 19555, 21003, 23400, 30049, 30555, 37950, 43049, 43051, 50016, 50082,
		50400, 54004, 56900, 64703, 64800, 64802, 64804, 66003, 67201, 69095, 72836
	];
	private readonly Dictionary<long, long> _vehicleByItemProto = new();
	private readonly HashSet<int> _interiorTemplates = new();
	private readonly Dictionary<int, int[]?> _interiorGraphs = new();
	private HashSet<int>? _sharedInteriorRoots;

	internal static HashSet<int> FindSharedInteriorRoots(ArmageddonWorldfilePackage source)
	{
		source = source.ForInstallation();
		var items = source.Items.ToDictionary(x => x.Vnum);
		var roomIds = source.Rooms.Select(x => x.Vnum).ToHashSet();
		var keepers = source.Shops.Select(x => x.Keeper).ToHashSet();
		var fixedServices = source.Npcs.Where(x => HasSpecial(x, "banker") || HasSpecial(x, "animal_to_store") || HasSpecial(x, "luirs_stablehand"))
			.Select(x => x.Vnum).ToHashSet();
		var shopRooms = source.Resets.Where(x => x.Opcode == "M" && (keepers.Contains(x.Arguments[1]) || fixedServices.Contains(x.Arguments[1])))
			.Select(x => x.Arguments[3]).ToHashSet();
		var entries = source.Items.Where(x => x.ItemType == 23).Select(x => (Item: x, Values: x.Values))
			.Concat(source.Resets.Where(x => x.ReferenceIssues.Length == 0 && x.Opcode is "O" or "G" or "E" or "P")
				.Where(x => items.TryGetValue(x.Arguments[1], out var item) && item.ItemType == 23)
				.Select(x => (Item: items[x.Arguments[1]], Values: ResetValues(x))));
		var graphs = new Dictionary<int, int[]?>();
		var candidates = new List<(int Root, int[] Rooms)>();
		var shared = new HashSet<int>();
		foreach (var (item, values) in entries)
		{
			if (values.Length != 6 || values[0] <= 0 || !roomIds.Contains(values[0]) || (values[3] & 64) != 0) continue;
			if (!graphs.TryGetValue(values[0], out var graph)) graphs[values[0]] = graph = FindInteriorGraph(source, values[0]);
			if (graph is null || values[2] > 0 && !graph.Contains(values[2]) ||
			    !VehicleVnums.Contains(item.Vnum) && graph.Any(shopRooms.Contains)) shared.Add(values[0]);
			else candidates.Add((values[0], graph));
		}
		bool changed;
		do
		{
			changed = false;
			foreach (var (root, graph) in candidates)
				if (graph.Any(shared.Contains)) changed |= shared.Add(root);
		} while (changed);
		return shared;
	}

	/// <summary>Only disconnected, bounded graphs are safe to instantiate automatically.</summary>
	internal static int[]? FindInteriorGraph(ArmageddonWorldfilePackage source, int entrance)
	{
		if (entrance <= 0) return null;
		var rooms = source.Rooms.ToDictionary(x => x.Vnum);
		if (!rooms.ContainsKey(entrance)) return null;
		var seen = new HashSet<int>();
		var pending = new Queue<int>();
		pending.Enqueue(entrance);
		while (pending.TryDequeue(out var next))
		{
			if (!seen.Add(next)) continue;
			if (seen.Count > 100) return null;
			foreach (var side in rooms[next].Exits)
			{
				if (!rooms.ContainsKey(side.Destination)) return null;
				pending.Enqueue(side.Destination);
			}
		}
		if (source.Rooms.Any(x => !seen.Contains(x.Vnum) && x.Exits.Any(e => seen.Contains(e.Destination)))) return null;
		// An arena/structure may be reset inside the very rooms it references. Cloning such
		// a graph would suppress its exterior placement or recursively manufacture interiors.
		var enterables = source.Items.Where(x => x.ItemType == 23).Select(x => x.Vnum).ToHashSet();
		if (source.Resets.Any(x => x.Opcode == "O" && x.ReferenceIssues.Length == 0 && x.Arguments.Length >= 10 &&
			enterables.Contains(x.Arguments[1]) && seen.Contains(x.Arguments[3]) && seen.Contains(x.Arguments[4]))) return null;
		return seen.Order().ToArray();
	}

	private void AddEnterable(GameItemProto item, ArmWorldItem source, int[] values)
	{
		_sharedInteriorRoots ??= FindSharedInteriorRoots(_source);
		if (values[0] <= 0 || !_rooms.ContainsKey(values[0]))
		{
			Adapt("enterable entrance", source.Vnum, $"Prototype {item.UniqueName} has no usable positive entrance ({values[0]}); no invented interior was added.");
			return;
		}
		if ((values[3] & (1 << 6)) != 0)
		{
			Adapt("enterable no-enter", source.Vnum, "Source explicitly prohibits entry; no native entrance was enabled.");
			return;
		}
		if (!_interiorGraphs.TryGetValue(values[0], out var graph))
			_interiorGraphs[values[0]] = graph = FindInteriorGraph(_source, values[0]);
		if (_sharedInteriorRoots.Contains(values[0])) graph = null;
		if (graph is not null && values[2] > 0 && !graph.Contains(values[2]))
		{
			Adapt("enterable control room", source.Vnum, $"Front room {values[2]} is outside the bounded entrance graph; a shared entrance preserves the source geography pending review.");
			graph = null;
		}
		if (graph is not null && !VehicleVnums.Contains(source.Vnum))
		{
			var keepers = _source.Shops.Select(x => x.Keeper).ToHashSet();
			if (_source.Resets.Any(x => x.Opcode == "M" && keepers.Contains(x.Arguments[1]) && graph.Contains(x.Arguments[3])))
				graph = null; // Permanent shops retain their one shared trading location; service roots were classified above.
		}
		if (graph is not null)
		{
			foreach (var vnum in graph) _interiorTemplates.Add(vnum);
			if (VehicleVnums.Contains(source.Vnum))
			{
				CreateVehicle(item, source, values, graph);
				return;
			}
		}
		var keyword = source.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "entrance";
		Attach(item, Component($"dwelling.{item.Id}", "Dwelling", new XElement("Definition",
			new XElement("TemplateEntryCell", _rooms[values[0]].Id), new XElement("DoorProto", 0),
			new XElement("DoorSize", (int)SizeCategory.Titanic), new XElement("EntranceKeyword", keyword),
			new XElement("EntranceDescription", source.ShortDescription), new XElement("SharedInterior", graph is null),
			new XElement("TemplateRooms", (graph ?? []).Select(v => new XElement("Room", _rooms[v].Id))))));
		Record($"enterable.{item.Id}", "ArmageddonEnterable", item.Id, item.RevisionNumber,
			new { SourceVnum = source.Vnum, Entrance = values[0], Rooms = graph, Disposition = graph is null ? "Shared entrance to existing geography" : "Instanced native dwelling" });
		if (graph is null && VehicleVnums.Contains(source.Vnum))
			Adapt("vehicle boundary", source.Vnum, "Interior connects to a broader world graph; retained as a shared entrance pending explicit vehicle boundaries, rather than cloning world geography.");
		_counts[graph is null ? "shared entrance prototypes" : "dwelling prototypes"] =
			_counts.GetValueOrDefault(graph is null ? "shared entrance prototypes" : "dwelling prototypes") + 1;
	}

	private void CreateVehicle(GameItemProto exterior, ArmWorldItem source, int[] values, int[] graph)
	{
		var vehicle = new VehicleProto
		{
			Id = Next<VehicleProto>(), RevisionNumber = 0, EditableItem = Approval(), Name = exterior.UniqueName!,
			Description = source.FullDescription, VehicleScale = (int)VehicleScale.RoomScale,
			ExteriorItemProtoId = exterior.Id, ExteriorItemProtoRevision = exterior.RevisionNumber
		};
		_db.VehicleProtos.Add(vehicle);
		var compartments = new Dictionary<int, VehicleCompartmentProto>();
		foreach (var vnum in graph)
		{
			var overlay = _rooms[vnum].RoomOverlays.Single();
			var compartment = new VehicleCompartmentProto
			{
				Id = Next<VehicleCompartmentProto>(), VehicleProtoId = vehicle.Id, VehicleProtoRevision = 0,
				Name = overlay.RoomName, Description = overlay.RoomDescription, DisplayOrder = compartments.Count,
				InteriorTerrainId = overlay.TerrainId, InteriorOutdoorsType = overlay.OutdoorsType
			};
			_db.VehicleCompartmentProtos.Add(compartment);
			compartments.Add(vnum, compartment);
		}
		var driverRoom = values[2] > 0 ? values[2] : values[0];
		var driver = new VehicleOccupantSlotProto
		{
			Id = Next<VehicleOccupantSlotProto>(), VehicleProtoId = vehicle.Id, VehicleProtoRevision = 0,
			VehicleCompartmentProtoId = compartments[driverRoom].Id, Name = "driver", SlotType = (int)VehicleOccupantSlotType.Driver,
			Capacity = 1, RequiredForMovement = true
		};
		_db.VehicleOccupantSlotProtos.Add(driver);
		_db.VehicleControlStationProtos.Add(new VehicleControlStationProto
		{
			Id = Next<VehicleControlStationProto>(), VehicleProtoId = vehicle.Id, VehicleProtoRevision = 0,
			VehicleOccupantSlotProtoId = driver.Id, Name = "controls", IsPrimary = true
		});
		var access = new VehicleAccessPointProto
		{
			Id = Next<VehicleAccessPointProto>(), VehicleProtoId = vehicle.Id, VehicleProtoRevision = 0,
			VehicleCompartmentProtoId = compartments[values[0]].Id, Name = "entrance", Description = source.ShortDescription,
			AccessPointType = (int)VehicleAccessPointType.Ramp, StartsOpen = true
		};
		_db.VehicleAccessPointProtos.Add(access);
		var projection = new GameItemProto
		{
			Id = Next<GameItemProto>(), UniqueName = exterior.UniqueName + "_Entrance", Name = "entrance",
			Keywords = "entrance", ShortDescription = "the entrance", LongDescription = "An entrance is here.",
			FullDescription = "The entrance to the vehicle.", MaterialId = exterior.MaterialId, Size = exterior.Size,
			Weight = 0, IsHiddenFromPlayers = true, ReadOnly = true, EditableItem = Approval(),
			BaseItemQuality = exterior.BaseItemQuality, PermitPlayerSkins = false
		};
		_db.GameItemProtos.Add(projection);
		Attach(projection, Component($"vehicleaccess.{exterior.Id}", "Vehicle Access Point", new XElement("Definition",
			new XElement("VehiclePrototypeId", vehicle.Id), new XElement("AccessPointPrototypeId", access.Id))));
		access.ProjectionItemProtoId = projection.Id;
		access.ProjectionItemProtoRevision = 0;
		var linkedPairs = new HashSet<(int, int)>();
		foreach (var vnum in graph)
		foreach (var side in _source.Rooms.Single(x => x.Vnum == vnum).Exits)
		{
			if (side.Destination == vnum || !linkedPairs.Add((Math.Min(vnum, side.Destination), Math.Max(vnum, side.Destination)))) continue;
			_db.VehicleCompartmentLinkProtos.Add(new VehicleCompartmentLinkProto
			{
				Id = Next<VehicleCompartmentLinkProto>(), VehicleProtoId = vehicle.Id, VehicleProtoRevision = 0,
				SourceVehicleCompartmentProtoId = compartments[vnum].Id, DestinationVehicleCompartmentProtoId = compartments[side.Destination].Id,
				OutboundDirection = $"room{side.Destination}", InboundDirection = $"room{vnum}",
				OutboundDescription = "towards", InboundDescription = "from"
			});
		}
		// Preserve cardinal exit topology from the actual templates at runtime, not lossy non-cardinal compartment links.
		Attach(exterior, Component($"vehicle.{exterior.Id}", "Vehicle Exterior", new XElement("Definition",
			new XElement("VehiclePrototypeId", vehicle.Id), new XElement("InteriorRoomTemplates", compartments.Select(x =>
				new XElement("Room", new XAttribute("compartment", x.Value.Id), new XAttribute("template", _rooms[x.Key].Id)))))));
		foreach (var environment in new[] { VehicleMovementEnvironment.Land, VehicleMovementEnvironment.Silt })
		{
			var allowed = (values[3] & (environment == VehicleMovementEnvironment.Silt ? 2 : 1)) != 0;
			if (!allowed && environment == VehicleMovementEnvironment.Silt) continue;
			var movement = new VehicleMovementProfileProto
			{
				Id = Next<VehicleMovementProfileProto>(), VehicleProtoId = vehicle.Id, VehicleProtoRevision = 0,
				Name = environment == VehicleMovementEnvironment.Silt ? "silt" : "land", MovementType = (int)VehicleMovementProfileType.RoomExit,
				MovementEnvironment = (int)environment, IsDefault = environment == VehicleMovementEnvironment.Silt ? (values[3] & 1) == 0 : (values[3] & 1) != 0 || (values[3] & 2) == 0,
				RequiredInstalledRole = "", RequiresTowLinksClosed = true
			};
			_db.VehicleMovementProfileProtos.Add(movement);
			_db.VehiclePropulsionProfileProtos.Add(new VehiclePropulsionProfileProto
			{
				Id = Next<VehiclePropulsionProfileProto>(), VehicleMovementProfileProtoId = movement.Id,
				PropulsionType = (int)(allowed ? VehiclePropulsionType.ExternallyPulled : VehiclePropulsionType.None),
				IsDefault = true, BaseMoveTimeMilliseconds = 10000, SpeedMultiplierExpression = "1", StaminaCostExpression = "1"
			});
		}
		_db.VehicleTowPointProtos.Add(new VehicleTowPointProto
		{
			Id = Next<VehicleTowPointProto>(), VehicleProtoId = vehicle.Id, VehicleProtoRevision = 0, Name = "harness",
			Description = "The harness for drawing the vehicle.", TowType = "pull", CanBeTowed = true,
			MaximumTowedWeight = 100000 * _kilograms, CharacterPullMultiplier = 1
		});
		_vehicleByItemProto.Add(exterior.Id, vehicle.Id);
		Record($"vehicle.{exterior.UniqueName}", nameof(VehicleProto), vehicle.Id, 0,
			new { SourceVnum = source.Vnum, Values = values, Rooms = graph, NativeDriverRoom = driverRoom,
				Defaults = "10 second base move; native hitch/pull readiness; 100000 kg tow rating. Source mount cap and damage fields retained for review." });
		_counts["vehicle prototypes"] = _counts.GetValueOrDefault("vehicle prototypes") + 1;
	}

	private void FinalizeInteriorTemplates()
	{
		foreach (var vnum in _interiorTemplates)
		{
			var room = _rooms[vnum];
			var zone = _source.Rooms.Single(x => x.Vnum == vnum).Zone;
			var prog = _progs.SingleOrDefault(x => x.FunctionName == $"ArmageddonWorldfileZone{zone}Boot");
			var effects = XElement.Parse(room.EffectData);
			effects.Add(new XElement("Effect", new XElement("ApplicabilityProg", 0), new XElement("Type", "RoomTemplateInstance"),
				new XElement("Original", 0), new XElement("Remaining", 0), new XElement("Effect",
					new XElement("TemplateRoomId", room.Id), new XElement("IsTemplate", true), new XElement("OnLoadProg", prog?.Id ?? 0))));
			room.EffectData = effects.ToString();
		}
		_counts["interior template rooms"] = _interiorTemplates.Count;
	}
}

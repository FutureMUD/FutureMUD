using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Resources;
using MudSharp.NPC.Templates;
using MudSharp.TimeAndDate.Listeners;
using MudSharp.Vehicles;

namespace FutureMUD.RoomSpatialAcceptance;

internal static class FullBootScenario
{
	private static readonly JsonSerializerOptions Json = new() { WriteIndented = true };
	private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
	private static string Env(string key) => Environment.GetEnvironmentVariable(key) ?? throw new InvalidOperationException("Missing owned scenario input.");
	private static IZone Owner(IRoom room) =>
#if LEGACY
		cell.Room.Zone;
#else
		room.OwningZone;
#endif
	public static void Run(IFuturemud world)
	{
		var phase = Env("FUTUREMUD_CELL_FULLBOOT_PHASE");
		var descriptor = Env("FUTUREMUD_CELL_FULLBOOT_DESCRIPTOR");
		var assertions = new List<string>();
		var status = "FAIL";
		string? failure = null;
		try
		{
			Require(Thread.CurrentThread.Name == "Main Game Thread", "Scenario must execute on the normal game thread.");
			assertions.Add("Normal Main -> LoadFromDatabase -> final Cell.PostLoadTasks/CompleteMagicLoad -> ready -> game-thread scheduler");
			if (phase == "prepare") Prepare(world, descriptor, assertions);
			else if (phase == "restored") AssertRestoredLegacy(world, descriptor, assertions);
			else if (phase == "legacy-read")
			{
				var ids = JsonSerializer.Deserialize<Dictionary<string, long>>(File.ReadAllText(descriptor))!;
				var outside = world.Rooms.Get(8101);
				var parent = outside.EffectsOfType<MagicSpellParent>().Single(); var ward = outside.EffectsOfType<SpellRoomWardEffect>().Single();
				Require(parent.Spell?.Id == ids["Spell"] && ward.School?.Id == ids["School"], "Historical normal boot failed to bind persisted cell ward source/school before any spatial migration.");
				assertions.Add("Unchanged historical normal cold boot binds persisted ward source/school");
			}
#if !LEGACY
			else
			{
				var ids = JsonSerializer.Deserialize<Dictionary<string, long>>(File.ReadAllText(descriptor))!;
				if (phase == "operate") { AssertLoaded(world, ids, false, assertions); Operate(world, ids, assertions); File.WriteAllText(descriptor, JsonSerializer.Serialize(ids, Json)); }
				else if (phase == "cold") AssertLoaded(world, ids, true, assertions);
				else throw new InvalidOperationException("Unknown full boot phase.");
			}
#endif
			world.SaveManager.Flush();
			world.LogManager.FlushLog();
			assertions.Add("Actual SaveManager/LogManager flush before normal HaltGameLoop shutdown");
			status = "PASS";
		}
		catch (Exception ex) { failure = ex.ToString(); }
		finally
		{
			File.WriteAllText(Env("FUTUREMUD_CELL_FULLBOOT_RECEIPT"), JsonSerializer.Serialize(new { Status = status, Phase = phase, Assertions = assertions, Failure = failure, GameThreadId = Environment.CurrentManagedThreadId, NormalFinalizationObserved = true }, Json));
			Console.WriteLine($"CellFullBoot-scenario phase={phase} status={status} assertions={assertions.Count}");
			world.HaltGameLoop();
		}
	}

	private static void AssertRestoredLegacy(IFuturemud world, string descriptor, List<string> assertions)
	{
		var ids = JsonSerializer.Deserialize<Dictionary<string, long>>(File.ReadAllText(descriptor))!;
		var outside = (Room)world.Rooms.Get(8101); var resource = world.MagicResources.Get(ids["Resource"]);
		Require(outside.UniqueName == "acceptance_cell_1" && outside.MagicResourceAmounts[resource] == 37.25, "Restored historical binary/files/database did not bind exact cell/resource identity.");
		using (var yield = JsonDocument.Parse(File.ReadAllText(descriptor + ".yield"))) Require(Math.Abs(outside.GetForagableYield(yield.RootElement.GetProperty("Key").GetString()!) - yield.RootElement.GetProperty("Amount").GetDouble()) < 0.000001, "Restored historical normal Cell.PostLoadTasks lost yield.");
		foreach (var key in new[] { "Actor", "DwellingActor", "VehicleActor" })
		{
			var npc = world.NPCs.SingleOrDefault(x => x.Id == ids[key]);
			Require(npc is not null, "Restored historical normal boot did not register the authored NPC: " + key);
			Require(npc.Body.Id == ids[key + "Body"] && npc.InstanceId == ids[key + "Instance"] && npc.Location.Id == (key == "Actor" ? 8102 : key == "DwellingActor" ? 8104 : ids["Interior0"]), "Restored historical native identity/custody location failed.");
		}
		Require(world.Vehicles.Get(ids["Vehicle"]).Compartments.Count() == 2 && world.Items.Get(ids["Held"]).GetItemType<IHoldable>()!.HeldBy.Id == ids["ActorBody"] && world.Items.Get(ids["Contained"]).ContainedIn.Id == ids["Container"], "Restored native hosted/body/container graph failed.");
		assertions.Add("Matching full database/binary/private-file backup restored; actual historical normal entrypoint cold-loads NPC/body/instance/custody/hosted/resource/yield graph");
	}

	private static void Prepare(IFuturemud world, string descriptor, List<string> assertions)
	{
		var ids = new Dictionary<string, long>();
		ICharacter admin;
		using (new FMDB()) admin = world.TryGetCharacter(FMDB.Context.Characters.Single(x => x.IsAdminAvatar).Id, true);
		Require(admin is not null && admin.Body is not null, "Legitimate seeded Admin did not materialize.");
		var template = new SimpleNPCTemplate(world, admin.Account, admin.GetCharacterTemplate(), "Qualification Guardian"); world.Add(template); template.Changed = true;
		ICharacter Npc(string key, IRoom room)
		{
			var npc = template.CreateNewCharacter(new SpatialLocation(room, RoomLayer.GroundLevel, null)); world.Add(npc, true); room.Login(npc);
			ids[key] = npc.Id; ids[key + "Body"] = npc.Body.Id; ids[key + "Instance"] = npc.InstanceId;
			return npc;
		}
		var plain = world.ItemProtos.Single(x => x.UniqueName == "medieval_industry_stock_book_board_pair");
		IGameItem Item(string key, IRoom room)
		{
			var item = plain.CreateNew(admin); world.Add(item); room.Insert(item, true); world.SaveManager.Flush(); ids[key] = item.Id; return item;
		}
		var outside = world.Rooms.Get(8101); var doomed = world.Rooms.Get(8102); var inner = world.Rooms.Get(8104);
		ids["OriginalZone"] = Owner(outside).Id; ids["Shard"] = Owner(outside).Shard.Id;
		var actor = Npc("Actor", doomed); Npc("DwellingActor", inner);
		Item("Loose", doomed); var held = Item("Held", doomed); actor.Body.Get(held, silent: true);
		Require(actor.Body.HeldItems.Contains(held), "Native held custody setup failed.");
		var containerProto = world.ItemProtos.Where(x => x.Status == MudSharp.Framework.Revision.RevisionStatus.Current)
			.First(x => x.Components.Any(c => c.TypeDescription == "Container"));
		var container = containerProto.CreateNew(admin); world.Add(container); doomed.Insert(container, true);
		var child = plain.CreateNew(admin); world.Add(child); container.GetItemType<IContainer>()!.Put(actor, child, false);
		world.SaveManager.Flush(); ids["Container"] = container.Id; ids["Contained"] = child.Id;
		Require(child.ContainedIn == container, "Native container custody setup failed.");
		Item("DwellingLoose", inner);
		var vehicle = (Vehicle)VehicleFactory.CreateVehicle(world.VehiclePrototypes.Get(900000, 0), outside, RoomLayer.GroundLevel);
		ids["Vehicle"] = vehicle.Id; ids["Exterior"] = vehicle.ExteriorItem.Id;
		var interiors = vehicle.Compartments.Select(x => x.InteriorRoom).ToArray();
		Require(interiors.Length == 2 && interiors.All(x => x is not null) && vehicle.Dockings.Any(), "Native hosted interiors/open docking fixture failed.");
		for (var i = 0; i < interiors.Length; i++) ids["Interior" + i] = interiors[i].Id;
		Npc("VehicleActor", interiors[0]); Item("VehicleLoose", interiors[0]);
		if (Environment.GetEnvironmentVariable("FUTUREMUD_CELL_FULLBOOT_WARD_PROBE") == "1")
		{
			var school = new MagicSchool(world, "Qualification School", "qualify", "qualified", Telnet.Cyan); world.Add(school);
			var spell = new MagicSpell("Qualification Retained Ward", school); world.Add(spell);
			var parent = new MagicSpellParent(outside, spell, actor);
			var ward = new SpellRoomWardEffect(outside, parent, school, MagicInterdictionMode.Fail, MagicInterdictionCoverage.Incoming, false, null);
			parent.AddSpellEffect(ward); outside.AddEffect(ward); outside.AddEffect(parent);
			ids["Spell"] = spell.Id; ids["School"] = school.Id;
			File.WriteAllText(descriptor + ".ward", parent.Identity.ToString());
		}
		var resource = new SimpleMagicResource(world, "Qualification Cell Reserve", "QCR"); world.Add(resource); ids["Resource"] = resource.Id;
		var profile = world.ForagableProfiles.First(x => x.Status == MudSharp.Framework.Revision.RevisionStatus.Current && x.MaximumYieldPoints.Any(y => y.Value > 0));
		var yield = profile.MaximumYieldPoints.First(x => x.Value > 0); outside.ForagableProfile = profile;
		((Room)outside).ConsumeYield(yield.Key, yield.Value * 0.25);
		File.WriteAllText(descriptor + ".yield", JsonSerializer.Serialize(new { Key = yield.Key, Amount = ((Room)outside).GetForagableYield(yield.Key), Profile = profile.Id }));
		world.SaveManager.Flush();
		// The resource is stored after the legacy world saves, so the next normal boot
		// must resolve this formerly unknown balance in CompleteMagicLoad.
		using (new FMDB())
		{
			FMDB.Context.RoomsMagicResources.Add(new MudSharp.Models.RoomMagicResource { RoomId = 8101, MagicResourceId = resource.Id, Amount = 37.25 });
			FMDB.Context.SaveChanges();
		}
		File.WriteAllText(descriptor, JsonSerializer.Serialize(ids, Json));
		assertions.Add("Genuine legacy normal world: three native NPC/body/primary-instance graphs, loose/held/container custody, persisted valid dwelling");
		assertions.Add("Genuine legacy factory vehicle: two hosted interiors, transient connector, open docking, native loaded occupant/item");
		assertions.Add("Persisted nonregenerating exact cell reserve and nondefault foraging yield");
	}

#if !LEGACY
	private static void AssertLoaded(IFuturemud world, Dictionary<string, long> ids, bool cold, List<string> assertions)
	{
		var outside = (Room)world.Rooms.Get(8101); Require(outside is not null && outside.UniqueName == "acceptance_cell_1", "Stable outside cell identity/identifier lost.");
		Require(outside.OwningZone.Id == ids["OriginalZone"] && outside.StoredCoordinates == (17, -2, 4), "Stored outside ownership/coordinates lost.");
		var resource = world.MagicResources.Get(ids["Resource"]);
		Require(outside.MagicResources.Count(x => x.Id == resource.Id) == 1 && outside.MagicResourceAmounts[resource] == 37.25, "CompleteMagicLoad balance lost or replayed.");
		using (var yield = JsonDocument.Parse(File.ReadAllText(Env("FUTUREMUD_CELL_FULLBOOT_DESCRIPTOR") + ".yield")))
			Require(outside.ForagableProfile.Id == yield.RootElement.GetProperty("Profile").GetInt64() && Math.Abs(outside.GetForagableYield(yield.RootElement.GetProperty("Key").GetString()!) - yield.RootElement.GetProperty("Amount").GetDouble()) < 0.000001, "Normal Cell.PostLoadTasks did not restore the authored nondefault foraging yield.");
		Require(!outside.EffectsOfType<MagicSpellParent>().Any(), "Unrequested magic effect appeared in the bounded reserve-state fixture.");
		var destination = cold ? 8101L : 8102L;
		foreach (var key in new[] { "Actor", "DwellingActor", "VehicleActor" })
		{
			var actor = world.NPCs.SingleOrDefault(x => x.Id == ids[key]);
			Require(actor is not null, "Normal boot did not register the authored NPC: " + key);
			var expected = cold ? 8101L : key == "Actor" ? 8102L : key == "DwellingActor" ? 8104L : ids["Interior0"];
			Require(actor.Body.Id == ids[key + "Body"] && actor.InstanceId == ids[key + "Instance"] && actor.Location.Id == expected && actor.Location.Characters.Count(x => x.Id == actor.Id) == 1, "Native loaded character/instance/body/cell identity lost: " + key);
		}
		var mainActor = world.NPCs.Single(x => x.Id == ids["Actor"]);
		Require(mainActor.Body.HeldItems.Single(x => x.Id == ids["Held"]).Id == ids["Held"], "Held item identity/custody lost.");
		var bag = world.Items.Get(ids["Container"]); var child = world.Items.Get(ids["Contained"]);
		Require(bag.Location.Id == destination && child.ContainedIn == bag && bag.GetItemType<IContainer>()!.Contents.Count(x => x.Id == child.Id) == 1, "Container custody identity lost or duplicated.");
		Require(world.Items.Get(ids["Loose"]).Location.Id == destination, "Loose item identity/cell lost.");
		Require(world.Items.Get(ids["VehicleLoose"]).Location.Id == (cold ? 8101 : ids["Interior0"]) && world.Items.Get(ids["DwellingLoose"]).Location.Id == (cold ? 8101 : 8104), "Vehicle/dwelling loose-item identity or placement lost across boot.");
		using (new FMDB())
		{
			Require(FMDB.Context.Npcs.Count(x => x.TemplateId == world.NpcTemplates.Single(x => x.Name == "Qualification Guardian").Id) == 3, "Normal boot replay created or deleted NPCs.");
			Require(FMDB.Context.BodiesGameItems.Count(x => x.GameItemId == ids["Held"]) == 1 && FMDB.Context.BodiesGameItems.Single(x => x.GameItemId == ids["Held"]).BodyId == ids["ActorBody"] && !FMDB.Context.BodiesGameItems.Any(x => x.GameItemId == ids["Contained"]) && FMDB.Context.RoomsGameItems.All(x => x.GameItemId != ids["Held"] && x.GameItemId != ids["Contained"]), "Persisted inventory custody has duplicate or missing owners.");
			Require(FMDB.Context.GameItems.Single(x => x.Id == child.Id).ContainerId == bag.Id, "Persisted contained owner lost.");
		}
		if (!cold)
		{
			Require(world.Rooms.Get(8102).StoredCoordinates == (17, -2, 4) && world.Zones.Get(ids["OriginalZone"]).DefaultRoom.Id == 8102, "Duplicate coordinates/explicit default lost on full load.");
			Require(world.Areas.Get(9000).Rooms.Select(x => x.Id).Order().SequenceEqual(new[] { 8101L, 8102L }) && world.Areas.Get(9001).Rooms.Single().Id == 8102, "Overlapping legacy Area memberships lost.");
			var vehicle = world.Vehicles.Get(ids["Vehicle"]);
			Require(vehicle.Compartments.Select(x => x.InteriorRoom.Id).Order().SequenceEqual(new[] { ids["Interior0"], ids["Interior1"] }.Order()) && vehicle.ExteriorItem.Id == ids["Exterior"] && vehicle.Dockings.Any(), "Existing hosted identities or docking did not load.");
			Require(vehicle.Compartments.All(x => ((Room)x.InteriorRoom).HostedVehicleId == vehicle.Id && x.InteriorRoom.OwningZone.Id == ids["OriginalZone"]), "Hosted intrinsic ownership lost.");
		}
		else
		{
			Require(world.Rooms.Get(ids["Clone"]).OwningZone.Id == 9000 && world.Rooms.Get(ids["Clone"]).StoredCoordinates == (31, -7, 2) && world.Rooms.Get(ids["Clone"]).UniqueName == "acceptance_clone", "Native create/clone/rezone/save did not cold load.");
			Require(world.Areas.Get(ids["CreatedArea"]).Rooms.Single().Id == ids["Clone"], "Saved new Area membership did not cold load.");
			Require(world.Areas.Get(9000).Rooms.Single().Id == 8101 && !world.Areas.Get(9001).Rooms.Any() && !world.Areas.Get(9002).Rooms.Any(), "Deleted cell Area cleanup did not persist.");
			Require(world.Zones.Get(ids["OriginalZone"]).DefaultRoom.Id == ids["ExpectedDefault"] && world.Zones.Get(9000).DefaultRoom.Id == ids["Clone"], "Selected/fallback defaults did not persist.");
			foreach (var id in new[] { 8102L, 8103L, 8104L, ids["Created"], ids["Interior0"] , ids["Interior1"] }) Require(world.Rooms.Get(id) is null && !world.Zones.Any(x => x.Rooms.Any(c => c.Id == id)) && !world.Shards.Any(x => x.Rooms.Any(c => c.Id == id)), "Deleted cell reappeared in a registry.");
			Require(world.Vehicles.Get(ids["Vehicle"]) is null && world.Items.Get(900000) is null && world.Items.Get(ids["Exterior"]).GetItemType<IVehicleExterior>()!.Vehicle is null, "Retired vehicle/dwelling recreated or exterior link survived.");
			using (new FMDB()) Require(!FMDB.Context.Exits.Any(x => x.Id >= 9000 && x.Id <= 9002) && !FMDB.Context.Vehicles.Any(x => x.Id == ids["Vehicle"]) && !FMDB.Context.VehicleCompartments.Any(x => x.VehicleId == ids["Vehicle"]) && !FMDB.Context.VehicleDockings.Any(x => x.VehicleId == ids["Vehicle"]), "Native deleted exit/vehicle/interior/docking state survived.");
			using (new FMDB()) Require(FMDB.Context.GameItems.LongCount() == ids["ItemCount"] && FMDB.Context.Characters.LongCount() == ids["CharacterCount"] && FMDB.Context.Bodies.LongCount() == ids["BodyCount"] && FMDB.Context.Rooms.LongCount() == ids["CellCount"], "Cold boot replay created/deleted physical entities.");
		}
		assertions.Add(cold ? "Cold normal boot: exact saved identities, defaults, Areas, loose/body/container custody, hosted retirement/dwelling deletion, reserve/yield unchanged; no entity replay" : "First current normal boot: unequal identities, duplicate XYZ, explicit default, overlapping Areas, existing custody, hosted cells and reserve/yield fully materialized");
	}

	private static void Operate(IFuturemud world, Dictionary<string, long> ids, List<string> assertions)
	{
		var outside = (Room)world.Rooms.Get(8101); var doomed = (Room)world.Rooms.Get(8102); var originalZone = outside.OwningZone;
		var created = new Room(outside.CurrentOverlay.Package, originalZone); ids["Created"] = created.Id;
		var clone = new Room(outside.CurrentOverlay.Package, originalZone, outside); ids["Clone"] = clone.Id;
		Require(clone.UniqueName is null, "Clone duplicated a global stable identifier.");
		Require(clone.TrySetUniqueName("acceptance_clone", out var error), error); clone.SetCoordinates(31, -7, 2);
		var actor = world.NPCs.Single(x => x.Id == ids["Actor"]); var loose = world.Items.Get(ids["Loose"]);
		doomed.Leave(actor); clone.Enter(actor); doomed.Extract(loose); clone.Insert(loose, true);
		clone.SetNewZone(world.Zones.Get(9000));
		Require(!originalZone.Characters.Contains(actor) && !originalZone.GameItems.Contains(loose) && world.Zones.Get(9000).Characters.Count(x => x == actor) == 1 && world.Zones.Get(9000).GameItems.Count(x => x == loose) == 1 && originalZone.Shard.Characters.Count(x => x == actor) == 1 && originalZone.Shard.GameItems.Count(x => x == loose) == 1, "Occupied rezone did not reconcile actual intrinsic aggregate membership exactly once.");
		clone.Leave(actor); doomed.Enter(actor); clone.Extract(loose); doomed.Insert(loose, true);
		var area = new Area(clone, "Qualification newly created area"); area.Add(created); area.Remove(created); ids["CreatedArea"] = area.Id;
		world.SaveManager.Flush();
		Require(world.Zones.Get(9000).DefaultRoom == clone && world.Zones.Get(9000).Rooms.Count(x => x == clone) == 1 && !originalZone.Rooms.Contains(clone), "Create/clone/rezone/default registry transition failed.");
		created.Destroy(outside);
		var temporal = ListenerFactory.CreateTimeOffsetListener(world.Clocks.First(), 0, 0, 1, 1, _ => throw new InvalidOperationException("Deleted listener fired."), [doomed]);
		Require(temporal.PertainsTo(doomed) && world.Listeners.Contains(temporal), "Real temporal listener fixture did not register.");
		doomed.Destroy(outside);
		Require(!world.Listeners.Contains(temporal), "Real deletion left a temporal listener registered.");
		var payload = temporal.GetType().GetProperty("Payload"); Require(payload is not null && payload.GetValue(temporal) is null, "Canceled temporal listener retained its callback.");
		Require(world.NPCs.Single(x => x.Id == ids["Actor"]).Location == outside && outside.GameItems.Any(x => x.Id == ids["Loose"]) && outside.GameItems.Any(x => x.Id == ids["Container"]), "Loaded occupants/items did not evacuate on direct Cell deletion.");
		ids["ExpectedDefault"] = originalZone.DefaultRoom.Id;
		Require(originalZone.DefaultRoom.Id != doomed.Id && originalZone.Rooms.Contains(originalZone.DefaultRoom), "Deletion retained a dead default.");
		assertions.Add("Actual create/clone/identifier/rezone/coordinates/Area/save/default; direct deletion evacuates loaded native character/item/custody and cancels real listener");
		var vehicle = (Vehicle)world.Vehicles.Get(ids["Vehicle"]);
		var interiorIds = vehicle.Compartments.Select(x => x.InteriorRoom.Id).ToArray();
		var transient = world.ExitManager.TransientExits.Where(x => x.Rooms.Any(c => interiorIds.Contains(c.Id))).ToArray();
		Require(transient.Length > 0 && !vehicle.CanRetire(out var blocked) && blocked.Contains("occupants"), "Occupied vehicle retirement did not refuse.");
		Require(world.Vehicles.Get(vehicle.Id) == vehicle && interiorIds.All(x => world.Rooms.Get(x) is not null), "Refused retirement mutated vehicle identities.");
		foreach (var room in vehicle.Compartments.Select(x => x.InteriorRoom))
		{
			foreach (var occupant in room.Characters.ToArray()) { room.Leave(occupant); outside.Enter(occupant); }
			foreach (var item in room.GameItems.ToArray()) { room.Extract(item); outside.Insert(item, true); }
		}
		Require(vehicle.Retire(out var reason), reason);
		Require(world.Items.Get(ids["VehicleLoose"]).Location == outside, "Retirement lost the exact evacuated loose item.");
		Require(interiorIds.All(x => world.Rooms.Get(x) is null) && world.Vehicles.Get(vehicle.Id) is null && vehicle.ExteriorItem.GetItemType<IVehicleExterior>()!.Vehicle is null, "Native vehicle retirement left hosted cells/vehicle/link.");
		Require(world.Rooms.All(x => world.ExitManager.GetAllExits(x).All(e => !transient.Contains(e.Exit))), "Retirement left transient connector/docking exits registered on surviving overlays.");
		Require(world.ExitManager.TransientExits.All(x => !transient.Contains(x)), "Retirement left a transient exit in the actual exit manager registry.");
		world.Items.Get(900000).Delete();
		Require(world.Rooms.Get(8103) is null && world.Rooms.Get(8104) is null && world.NPCs.Single(x => x.Id == ids["DwellingActor"]).Location == outside && world.Items.Get(ids["DwellingLoose"]).Location == outside, "Valid persisted dwelling did not delete its cells or evacuate loaded contents.");
		world.SaveManager.Flush();
		foreach (var id in new[] { 8102L, 8103L, 8104L, ids["Created"], ids["Interior0"], ids["Interior1"] }) Require(!world.Zones.Any(x => x.Rooms.Any(c => c.Id == id)) && !world.Shards.Any(x => x.Rooms.Any(c => c.Id == id)), "Deletion left zone/shard membership.");
		using (new FMDB()) Require(!FMDB.Context.Rooms.Any(x => x.Id == 8102 || x.Id == 8103 || x.Id == 8104 || x.HostedVehicleId == ids["Vehicle"]) && !FMDB.Context.AreasRooms.Any(x => x.RoomId == 8102 || x.RoomId == 8103 || x.RoomId == 8104), "Final deletion ownership/Area database cleanup failed.");
		using (new FMDB()) { ids["ItemCount"] = FMDB.Context.GameItems.LongCount(); ids["CharacterCount"] = FMDB.Context.Characters.LongCount(); ids["BodyCount"] = FMDB.Context.Bodies.LongCount(); ids["CellCount"] = FMDB.Context.Rooms.LongCount(); }
		assertions.Add("Native room-scale occupied refusal then evacuation/retirement, transient/open docking/hosted registry cleanup; valid persisted dwelling parent deletion with loaded native contents; exact persistent deletion joins");
	}
#endif
}

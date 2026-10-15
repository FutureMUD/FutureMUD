#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.FutureProg;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PrepareS4FamilyFixture(TestDatabase database, string path)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var settings = ArmageddonInstallation.Read(db)!;
		var ids = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package && !x.Retired && x.LogicalId > 0)
			.ToDictionary(x => x.StableKey, x => x.LogicalId!.Value);
		var input = JsonNode.Parse(File.ReadAllText(path))!.AsObject();
		var actor = db.Characters.Single(x => x.Id == input["character"]!.GetValue<long>());
		var home = db.Rooms.Include(x => x.CurrentOverlay).Single(x => x.Id == actor.Location);
		var terrain = new Db.Terrain();
		db.Entry(terrain).CurrentValues.SetValues(db.Terrains.AsNoTracking().Single(x => x.Id == home.CurrentOverlay.TerrainId));
		terrain.Id = 0; terrain.Name = "S4 family trackable ground"; terrain.DefaultTerrain = false;
		// Native track difficulty uses ability/intensity. This surface selects Automatic for
		// positive route processing; stock seeker skills, templates and RNG remain unchanged.
		terrain.CanHaveTracks = true; terrain.TrackIntensityMultiplierVisual = 0.1; terrain.TrackIntensityMultiplierOlfactory = 1;
		terrain.MovementRate = 1; db.Add(terrain); db.SaveChanges();
		Db.Room Room(string label, int offset)
		{
			var room = new Db.Room { ZoneId = home.ZoneId, X = home.X + offset, Y = home.Y, Z = home.Z, Temporary = false, EffectData = "<Effects/>" };
			db.Add(room); db.SaveChanges();
			var overlay = new Db.RoomOverlay(); db.Entry(overlay).CurrentValues.SetValues(home.CurrentOverlay);
			overlay.Id = 0; overlay.RoomId = room.Id; overlay.Name = label; overlay.RoomName = label; overlay.AddedLight = 1000;
			overlay.RoomDescription = "An ordinary permanent room in an isolated S4 family fixture.";
			overlay.TerrainId = terrain.Id;
			db.Add(overlay); db.SaveChanges(); room.CurrentOverlayId = overlay.Id; db.SaveChanges(); return room;
		}
		var origin = Room("S4 family origin", 100); var destination = Room("S4 family destination", 101);
		var exit = new Db.Exit
		{
			RoomId1 = origin.Id, RoomId2 = destination.Id,
			Direction1 = (int)CardinalDirection.North, Direction2 = (int)CardinalDirection.South,
			TimeMultiplier = 1.0, AcceptsDoor = false,
			MaximumSizeToEnter = (int)SizeCategory.Titanic,
			MaximumSizeToEnterUpright = (int)SizeCategory.Titanic,
			BlockedLayers = string.Empty
		};
		db.Add(exit); db.SaveChanges();
		db.Add(new Db.RoomOverlayExit { RoomOverlayId = origin.CurrentOverlayId!.Value, ExitId = exit.Id });
		db.Add(new Db.RoomOverlayExit { RoomOverlayId = destination.CurrentOverlayId!.Value, ExitId = exit.Id }); db.SaveChanges();
		var diagnostic = new Db.FutureProg { FunctionName = "s4FamilyHeldItems", FunctionText = "return @creature.helditems",
			ReturnTypeDefinition = (ProgVariableTypes.Collection | ProgVariableTypes.Item).ToStorageString(), FunctionComment = "Read-only isolated native custody diagnostic", Category = "Harness", Subcategory = "S4 Family" };
		diagnostic.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "creature", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		db.Add(diagnostic); db.SaveChanges();
		var routes = new List<object>();
		foreach (var (key, resource) in new[] { ("arm.spell.seeking_wind", "wind"), ("arm.spell.wind_courier", "wind"), ("arm.spell.haunting_shape", "shadow"), ("arm.spell.call_outsider", "void"), ("arm.spell.borrow_the_dead", "void") })
		{
			var spell = db.MagicSpells.Single(x => x.Id == ids[key]);
			var route = PreparedStockCampaignRoute(db, settings, key, spell, ids["arm.resource." + resource], ids[ArmageddonProgressionInstaller.SourceResourceKey], 1);
			var school = db.MagicSchools.Single(x => x.Id == spell.MagicSchoolId);
			routes.Add(new { key, spell = spell.Id, name = spell.Name, capability = route.Capability, merit = route.Merit, resource = ids["arm.resource." + resource], invoked = school.SchoolVerb.ToLowerInvariant() });
		}
		var source = Path.Combine(CatalogueRepository, "tests", "ArmageddonPreparedSeederNativeHarness", "Program.S4FamilyFixture.cs");
		input["family_acceptance"] = JsonSerializer.SerializeToNode(new { cases = Environment.GetEnvironmentVariable("FUTUREMUD_S4_FAMILY_CASES") ?? "all", origin = origin.Id, destination = destination.Id, exit = exit.Id, trackable_terrain = terrain.Id, can_get_prog = diagnostic.Id, routes,
			quarry_template = ids["arm.npc.earth_mount.g1"], recipient_template = ids["arm.npc.clay_sentinel.g1"], parcel_prototype = ids["arm.item.component_blank"],
			seek_template = ids["arm.npc.seeking_wind.g1"], courier_template = ids["arm.npc.wind_courier.g1"], haunt_template = ids["arm.npc.haunting_shape.g1"], outsider_template = ids["arm.npc.outsider.g1"],
			source = new { path = source, sha256 = BaseHash(source) } });
		File.WriteAllText(path, input.ToJsonString());
		Console.WriteLine("ARMS4-family-input=prepared five isolated grade-one routes two ordinary rooms no package gate changes");
	}
}

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
	private static void PrepareReleaseRegionalFixture(MudSharp.Database.FuturemudDatabaseContext db, JsonObject input,
		Db.Character actor, IReadOnlyDictionary<string, long> claims, double capacity)
	{
		Require(ReleaseStockSmoke && ReleaseRegionalSmoke && capacity >= 105, "Regional smoke requires original Release stock and ordinary capacity105.");
		var home = db.Rooms.Include(x => x.CurrentOverlay).Single(x => x.Id == actor.Location);
		var terrainId = claims[ArmageddonRegionalLandInstaller.TerrainKey("fertile")];
		Db.Room MakeRoom(string name, int offset)
		{
			var room = new Db.Room { ZoneId = home.ZoneId, X = home.X + offset, Y = home.Y, Z = home.Z, Temporary = false, EffectData = "<Effects/>" };
			db.Add(room); db.SaveChanges();
			var overlay = new Db.RoomOverlay(); db.Entry(overlay).CurrentValues.SetValues(home.CurrentOverlay);
			overlay.Id = 0; overlay.RoomId = room.Id; overlay.Name = name; overlay.RoomName = name; overlay.AddedLight = 1000;
			overlay.RoomDescription = "An isolated regional gathering acceptance room."; overlay.TerrainId = terrainId;
			db.Add(overlay); db.SaveChanges(); room.CurrentOverlayId = overlay.Id; db.SaveChanges(); return room;
		}
		var a = MakeRoom("Guest Lounge regional source", 500);
		var b = MakeRoom("Regional local fallback", 501);
		a.EnvironmentalMagicBindingMode = 1;
		a.EnvironmentalMagicProfileId = claims[ArmageddonProgressionInstaller.LandProfileKey];
		var area = new Db.Areas { Name = "Release shared life acceptance" };
		area.AreasRooms.Add(new() { RoomId = a.Id }); area.AreasRooms.Add(new() { RoomId = b.Id }); db.Add(area);
		var exit = new Db.Exit { RoomId1 = a.Id, RoomId2 = b.Id, Direction1 = (int)CardinalDirection.North, Direction2 = (int)CardinalDirection.South,
			TimeMultiplier = 1, AcceptsDoor = false, MaximumSizeToEnter = (int)SizeCategory.Titanic, MaximumSizeToEnterUpright = (int)SizeCategory.Titanic, BlockedLayers = "" };
		db.Add(exit); db.SaveChanges();
		db.Add(new Db.RoomOverlayExit { RoomOverlayId = a.CurrentOverlayId!.Value, ExitId = exit.Id });
		db.Add(new Db.RoomOverlayExit { RoomOverlayId = b.CurrentOverlayId!.Value, ExitId = exit.Id });
		actor.Location = a.Id;
		var expressions = new[] { "healthcapacity(@actor, \"health\")", "healthcapacity(@actor, \"stun\")", "healthcapacity(@actor, \"stamina\")",
			"healthremaining(@actor, \"health\")", "healthremaining(@actor, \"stun\")", "healthremaining(@actor, \"stamina\")",
			"gettrait(@actor, totrait(" + claims[ArmageddonProgressionInstaller.GatheringTraitKey] + "))", "getregister(@actor, \"armageddon_ecology\")" };
		var probe = new Db.FutureProg { FunctionName = "ReleaseRegionalBodyProbe", ReturnTypeDefinition = ProgVariableTypes.Text.ToStorageString(),
			FunctionText = "return \"GATHERVALUES \" + " + string.Join(" + \"|\" + ", expressions.Select(x => "totext(" + x + ")")) + " + \" ENDGATHER\"",
			Category = "Testing", Subcategory = "Regional", FunctionComment = "Pure native body observation", AcceptsAnyParameters = false, StaticType = 0 };
		probe.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "actor", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		db.Add(probe); db.SaveChanges();
		using (var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray(), db.VariableDefinitions.ToArray())) compiler.Compile(probe.Id);
		var schema = input["sql_schema"]!.AsArray();
		void Add<T>(string alias, params string[] columns) where T : class => schema.Add(JsonSerializer.SerializeToNode(PreparedStockSqlTable<T>(db, alias, columns)));
		Add<Db.RegionalGatheringProfile>("RegionalGatheringProfiles", "Id", "Name", "Revision", "Definition");
		Add<Db.MagicGenerator>("MagicGenerators", "Id", "Definition");
		Add<Db.RegionalGatheringPool>("RegionalGatheringPools", "OwnerKind", "OwnerId", "ProfileId", "Priority", "Revision", "Stock", "Capacity", "Initialised", "Inherit");
		Add<Db.Areas>("Areas", "Id", "Name"); Add<Db.AreasRooms>("AreasRooms", "AreaId", "RoomId");
		Add<Db.RoomsForagableYield>("RoomsForagableYields", "RoomId", "ForagableType", "Yield");
		Add<Db.VariableValue>("VariableValues", "ReferenceTypeDefinition", "ReferenceId", "ReferenceProperty", "ValueTypeDefinition", "ValueDefinition");
		Add<Db.Wound>("Wounds", "Id", "BodyId", "OriginalDamage", "CurrentDamage", "CurrentPain", "CurrentStun", "WoundType");
		Add<Db.RoomEnvironmentalState>("RoomEnvironmentalStates", "RoomId", "Revision", "ScarDamage", "RecentPressure");
		Add<Db.MagicGatheringOperation>("MagicGatheringOperations", "ActorId", "BodyId", "DamageCost", "PainCost", "StunCost", "SourceDebit", "SourceDebited", "NotificationCompleted", "LandDetailJson", "EcologicalChildId", "EcologicalApplied");
		Add<Db.MagicGatheringParticipant>("MagicGatheringParticipants", "OperationId", "RoomId", "SourceKey");
		input["release_originals"]!["RegionalGatheringProfiles"] = JsonSerializer.SerializeToNode(db.RegionalGatheringProfiles.AsNoTracking().AsEnumerable().Select(x => new { id = x.Id, definition = x.Definition }).ToArray());
		input["release_originals"]!["MagicGenerators"] = JsonSerializer.SerializeToNode(db.MagicGenerators.AsNoTracking().AsEnumerable().Select(x => new { id = x.Id, definition = x.Definition }).ToArray());
		input["regional_land"] = JsonSerializer.SerializeToNode(new { actor = actor.Id, body = actor.BodyId, account = "S5Learner", capacity,
			capability = claims["arm.capability.sorcerer"], resource = claims["arm.resource.sorcerer"], trait = claims[ArmageddonProgressionInstaller.GatheringTraitKey],
			area = area.Id, a = a.Id, b = b.Id, profile = claims[ArmageddonRegionalLandInstaller.MixedProfileKey], probe = probe.Id,
			invoked = db.MagicSchools.Single(x => x.Id == db.MagicCapabilities.Single(c => c.Id == claims["arm.capability.sorcerer"]).MagicSchoolId).SchoolVerb,
			limits = "Original Release Sorcerer and wildland; ordinary copied Player body, explicit permanent merit, no magic knowledge/skill/reserve injection. A has an explicit legacy profile without matching forage; B uses approved fertile local profile. Area activated through native builder command; first/cold only, no exceptional fault injection or hour/ash wait." });
	}
}

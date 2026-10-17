#nullable enable

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Magic;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PrepareRemainingStockFixture(TestDatabase database)
	{
		var path = Environment.GetEnvironmentVariable("FUTUREMUD_S4_ATTUNEMENT_STOCK_INPUT")!;
		var input = JsonNode.Parse(File.ReadAllText(path))!.AsObject();
		using var db = NewIndependentContext(database.ConnectionString);
		var settings = ArmageddonInstallation.Read(db)!;
		var ids = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package && !x.Retired && x.LogicalId > 0)
			.ToDictionary(x => x.StableKey, x => x.LogicalId!.Value, StringComparer.Ordinal);
		var actor = db.Characters.Single(x => x.Id == input["character"]!.GetValue<long>());
		var home = db.Rooms.Include(x => x.CurrentOverlay).Single(x => x.Id == actor.Location);
		var overlay = home.CurrentOverlay;
		var room = new Db.Room { ZoneId = home.ZoneId, X = db.Rooms.Where(x => x.ZoneId == home.ZoneId).Max(x => x.X) + 1,
			Y = home.Y, Z = home.Z, EffectData = "<Effects />", Temporary = false };
		db.Add(room); db.SaveChanges();
		var destination = new Db.RoomOverlay { RoomId = room.Id, Name = "S4 remaining stock mapped destination", RoomName = "S4 mapped outdoor destination",
			RoomDescription = "An ordinary permanent room in a disposable native qualification world.",
			RoomOverlayPackageId = overlay.RoomOverlayPackageId, RoomOverlayPackageRevisionNumber = overlay.RoomOverlayPackageRevisionNumber,
			TerrainId = overlay.TerrainId, OutdoorsType = (int)RoomOutdoorsType.Outdoors, HearingProfileId = overlay.HearingProfileId,
			AmbientLightFactor = overlay.AmbientLightFactor, AddedLight = overlay.AddedLight, AtmosphereId = overlay.AtmosphereId,
			AtmosphereType = overlay.AtmosphereType, SafeQuit = true };
		db.Add(destination); db.SaveChanges(); room.CurrentOverlayId = destination.Id;
		room.RoomsTags.Add(new() { RoomId = room.Id, TagId = ids[ArmageddonStockSupportInstaller.TravelWildPoolTagKey] }); db.SaveChanges();
		var routes = new List<object>();
		// Normal enrolment always acquires controlled grade one. All-grade behavior is covered
		// by the owning managed cases; this lane samples ordinary installed-stock acquisition.
		foreach (var (key, grade, resource) in new[] { (ArmageddonReturnTetherSpellContent.Key, 1, "earth"),
			(ArmageddonWeightedTravelSpellContent.WaystepKey, 1, "wind"), (ArmageddonStormLocusSpellContent.Key, 1, "lightning"),
			(ArmageddonMindSilenceSpellContent.Key, 1, "void"), (ArmageddonCompellingRegardSpellContent.Key, 1, "void"),
			(ArmageddonOpenThresholdSpellContent.Key, 1, "void"), ("arm.spell.arcane_mark", 1, "void") })
		{
			var spell = db.MagicSpells.Single(x => x.Id == ids[key]);
			var route = PreparedStockCampaignRoute(db, settings, key, spell, ids["arm.resource." + resource], ids[ArmageddonProgressionInstaller.SourceResourceKey], 1);
			var capability = db.MagicCapabilities.Single(x => x.Id == route.Capability); var definition = XElement.Parse(capability.Definition);
			definition.Element("Casting")!.Element("Admission")!.SetAttributeValue("min", grade); capability.Definition = definition.ToString(); db.SaveChanges();
			var school = db.MagicSchools.Single(x => x.Id == spell.MagicSchoolId);
			routes.Add(new { key, spell = spell.Id, name = spell.Name, grade, capability = route.Capability, merit = route.Merit,
				resource = ids["arm.resource." + resource], invoked = school.SchoolVerb.ToLowerInvariant() });
		}
		var source = Path.Combine(CatalogueRepository, "tests", "ArmageddonPreparedSeederNativeHarness", "Program.RemainingStockFixture.cs");
		input["remaining_stock"] = JsonSerializer.SerializeToNode(new { home = home.Id, destination = room.Id, routes,
			target_template = ids["arm.npc.earth_mount.g1"], source = new { path = source, sha256 = BaseHash(source) } });
		File.WriteAllText(path, input.ToJsonString());
		Console.WriteLine("ARMS4-remaining-input=prepared seven explicit disposable routes permanent mapped room no package admissions");
	}
}

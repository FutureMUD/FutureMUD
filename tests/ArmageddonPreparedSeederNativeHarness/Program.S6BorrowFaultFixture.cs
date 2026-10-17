#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static bool S6BorrowFault => Environment.GetEnvironmentVariable("FUTUREMUD_S6_MEASUREMENT_SCOPE") == "borrow-retirement";

	private static void PrepareS6BorrowFaultFixture(MudSharp.Database.FuturemudDatabaseContext db, JsonObject input,
		Db.Character template, IReadOnlyDictionary<string, long> ids)
	{
		PrepareS6UncertaintyFixture(db, input, template, ids);
		var subject = input["s6_uncertainty"]!.AsObject();
		var actor = subject["actor"]!.GetValue<long>();
		var spell = db.MagicSpells.Single(x => x.Id == ids["arm.spell.borrow_the_dead"]);
		var schoolKey = ids.Single(x => x.Key.StartsWith("arm.school.", StringComparison.Ordinal) && x.Value == spell.MagicSchoolId).Key;
		var resource = ids[schoolKey.Replace("arm.school.", "arm.resource.", StringComparison.Ordinal)];
		var settings = ArmageddonInstallation.Read(db)!;
		var routes = new JsonArray();
		foreach (var alias in new[] { "borrow", "alternate" })
		{
			var route = PreparedStockCampaignRoute(db, settings, "arm.spell.borrow_the_dead", spell, resource,
				ids[ArmageddonProgressionInstaller.SourceResourceKey], 1);
			db.PerceiverMerits.Add(new() { CharacterId = actor, MeritId = route.Merit });
			routes.Add(JsonSerializer.SerializeToNode(new { alias, spell = spell.Id, name = spell.Name, capability = route.Capability,
				merit = route.Merit, resource, invoked = db.MagicSchools.Single(x => x.Id == spell.MagicSchoolId).SchoolVerb }));
		}
		db.SaveChanges();
		input["s6_borrow_fault"] = new JsonObject {
			["actor"] = actor, ["body"] = subject["body"]!.DeepClone(), ["account"] = subject["account"]!.DeepClone(),
			["room"] = template.Location, ["routes"] = routes, ["npc_template"] = ids["arm.npc.clay_sentinel.g1"],
			["item_prototype"] = ids["arm.item.component_blank"],
			["limits"] = "Exact installed Borrow the Dead possession adapter; copied-body ordinary Player with separate high-opening admissions, native finite paid funding. One exact original corpse/body/foreign item. Actual secondary-instance DELETE provider fault; no fabricated animation journal or refund. MyISAM harness-only trigger-hit observer persists the rejected DELETE attempt. Native hold/recovery/cold must be measured; absence is a failure, not inferred from other adapters."
		};
	}
}

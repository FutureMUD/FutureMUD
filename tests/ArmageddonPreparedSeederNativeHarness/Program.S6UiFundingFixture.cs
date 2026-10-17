#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Framework;
using MudSharp.FutureProg;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static bool S6UiFunding => Environment.GetEnvironmentVariable("FUTUREMUD_S6_MEASUREMENT_SCOPE") == "ui-funding";

	private static void PrepareS6UiFundingFixture(MudSharp.Database.FuturemudDatabaseContext db, JsonObject input,
		IReadOnlyDictionary<string, long> claims)
	{
		var settings = ArmageddonInstallation.Read(db)!;
		Require(settings.Mode == ArmageddonProgressionMode.Simple && S6Classes && !S6Readiness,
			"UI/funding is a separate Simple/classes-on measurement, without N24/readiness execution.");
		PrepareS6ChargenFixture(db, input, claims);
		PrepareS6ChargenFixture(db, input, claims, "arm.capability.earth", "S6EarthApplicant", "s6_earth_chargen");
		var gathering = input["s6_gathering"]!.AsObject();
		var enabledGathering = XElement.Parse(db.MagicCapabilities.Single(x => x.Id == gathering["capability"]!.GetValue<long>()).Definition).Element("Gathering")!;
		var sorcerer = db.MagicCapabilities.Single(x => x.Id == input["s6_chargen"]!["capability"]!.GetValue<long>());
		var definition = XElement.Parse(sorcerer.Definition);
		definition.Element("Gathering")!.ReplaceWith(new XElement(enabledGathering));
		sorcerer.Definition = definition.ToString();
		var probe = new Db.FutureProg {
			FunctionName = "S6UiCapacityAttributeProbe", ReturnTypeDefinition = ProgVariableTypes.Text.ToStorageString(),
			FunctionText = $"return \"CAPACITYINPUT \" + totext(gettrait(@actor, totrait({settings.CapacityAttribute}))) + \" ENDCAPACITY\"",
			Category = "Testing", Subcategory = "S6", FunctionComment = "Pure effective native capacity attribute observation",
			AcceptsAnyParameters = false, StaticType = 0
		};
		probe.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "actor", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		db.FutureProgs.Add(probe);
		db.SaveChanges();
		using (var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray(), db.VariableDefinitions.ToArray()))
			compiler.Compile(probe.Id);
		input["sql_schema"]!.AsArray().Add(JsonSerializer.SerializeToNode(PreparedStockSqlTable<Db.Trait>(db, "Traits", ["BodyId", "TraitDefinitionId", "Value"])));
		var progression = new ArmageddonStockProgression(settings.Mode);
		var practiceKey = "arm.spell.sense_enchantment";
		Require(progression.Bindings.Single(x => x.CapabilityKey == "arm.capability.sorcerer" && x.ContentKey == practiceKey).Starting,
			"Ordinary practice must use an actually authored Sorcerer starting root.");
		var practice = db.MagicSpells.Single(x => x.Id == claims[practiceKey]);
		var practicePolicy = XElement.Parse(practice.Definition).Element("ControlledPower")!.Element("Practice")!;
		Require((bool?)practicePolicy.Attribute("enabled") == true && (double?)practicePolicy.Attribute("seconds") == 30 &&
			(double?)practicePolicy.Attribute("energy") == 1, "Ordinary Sorcerer root must retain its stock thirty-second full-cost practice.");
		input["s6_ui_funding"] = JsonSerializer.SerializeToNode(new {
			capacity_probe = probe.Id, sorcerer_practice = practice.Id, sorcerer_practice_name = practice.Name,
			limits = "Two empty Player accounts complete actual Simple/Class UI. No copied applicant bodies, forced attributes, resource credit, deterministic gain, N24 or timed ash wait. Private Class/capability/permission copies retain all stock policies except independent availability. Original gates remain false; ordinary affordability may legitimately refuse."
		});
	}
}

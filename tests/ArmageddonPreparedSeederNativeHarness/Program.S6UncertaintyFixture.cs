#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PrepareS6UncertaintyFixture(MudSharp.Database.FuturemudDatabaseContext db, JsonObject input, Db.Character template, IReadOnlyDictionary<string, long> ids)
	{
		var settings = ArmageddonInstallation.Read(db)!;
		var account = new Db.Account(); db.Entry(account).CurrentValues.SetValues(db.Accounts.AsNoTracking().Single(x => x.Id == template.AccountId));
		account.Id = 0; account.Name = "N25Witness"; account.Email = string.Empty; account.LastLoginTime = null;
		db.Accounts.Add(account); db.SaveChanges();
		var body = new Db.Body(); db.Entry(body).CurrentValues.SetValues(db.Bodies.AsNoTracking().Single(x => x.Id == template.BodyId));
		body.Id = 0; body.ShortDescription = "an uncertainty subject"; body.FullDescription = "An uncertainty subject is here.";
		body.EffectData = "<Effects />"; db.Bodies.Add(body); db.SaveChanges();
		foreach (var t in db.Traits.AsNoTracking().Where(x => x.BodyId == template.BodyId).ToArray())
			db.Traits.Add(new() { BodyId = body.Id, TraitDefinitionId = t.TraitDefinitionId, Value = t.Value, AdditionalValue = t.AdditionalValue });
		var actor = new Db.Character(); db.Entry(actor).CurrentValues.SetValues(template);
		actor.Id = 0; actor.BodyId = body.Id; actor.AccountId = account.Id; actor.Name = "N25Witness";
		actor.NameInfo = actor.NameInfo.Replace("S5Learner", "N25Witness", StringComparison.Ordinal);
		db.Characters.Add(actor); db.SaveChanges();
		db.CharacterBodies.Add(new() { CharacterId = actor.Id, BodyId = body.Id, Alias = "base", TransformationEcho = string.Empty, AllowVoluntarySwitch = true });
		foreach (var t in db.CharacterTraits.AsNoTracking().Where(x => x.CharacterId == template.Id).ToArray())
			db.CharacterTraits.Add(new() { CharacterId = actor.Id, TraitDefinitionId = t.TraitDefinitionId, Value = t.Value, AdditionalValue = t.AdditionalValue });
		foreach (var language in db.CharactersLanguages.AsNoTracking().Where(x => x.CharacterId == template.Id).ToArray())
			db.CharactersLanguages.Add(new() { CharacterId = actor.Id, LanguageId = language.LanguageId });
		foreach (var accent in db.CharactersAccents.AsNoTracking().Where(x => x.CharacterId == template.Id).ToArray())
			db.CharactersAccents.Add(new() { CharacterId = actor.Id, AccentId = accent.AccentId, Familiarity = accent.Familiarity });
		var routes = new List<object>();
		foreach (var (key, alias, resourceKey) in new[] {
			("arm.spell.earthen_ward", "debit", "arm.resource.earth"),
			("arm.spell.earthen_ward", "alternate", "arm.resource.earth"),
			("arm.spell.unbodied_journey", "effect", "arm.resource.void"),
			("arm.spell.read_attunement", "unrelated", "arm.resource.sorcerer") })
		{
			var spell = db.MagicSpells.Single(x => x.Id == ids[key]);
			var resource = ids[resourceKey];
			var route = PreparedStockCampaignRoute(db, settings, key, spell, resource, ids[ArmageddonProgressionInstaller.SourceResourceKey], 1);
			db.PerceiverMerits.Add(new() { CharacterId = actor.Id, MeritId = route.Merit });
			routes.Add(new { key, alias, spell = spell.Id, name = spell.Name, capability = route.Capability, merit = route.Merit, resource,
				invoked = db.MagicSchools.Single(x => x.Id == spell.MagicSchoolId).SchoolVerb.ToLowerInvariant() });
		}
		db.SaveChanges();
		Require(!db.CharacterAcquiredSpells.Any(x => x.CharacterId == actor.Id) && !db.CharactersMagicResources.Any(x => x.CharacterId == actor.Id), "N25 subject inherited acquisition or funding.");
		var schema = input["sql_schema"]!.AsArray();
		schema.Add(JsonSerializer.SerializeToNode(PreparedStockSqlTable<Db.MagicCastingOperation>(db, "MagicCastingOperations", ["Id", "CharacterId", "ActorId", "BodyId", "MagicCapabilityId", "MagicSpellId", "TraitDefinitionId", "ReserveId", "Stage", "Definition", "Diagnostic"])));
		input["s6_uncertainty"] = JsonSerializer.SerializeToNode(new { actor = actor.Id, body = body.Id, account = account.Name, routes,
			limits = "Copied-body ordinary Player; separate explicit high-opening qualification routes invoke unchanged native stock spells. Funding uses finite paid native gathering, not injected balances. Actual MySQL SIGNAL after debit and successful projection effect; actual secondary-body retry and reboot retirement. Natural affordability/mastery statistics remain in their existing separate qualification scopes." });
		Console.WriteLine("ARMS6-N25-input=separate ordinary Player, zero funding/acquisition, native paid routes and actual provider fault predicates");
	}
}

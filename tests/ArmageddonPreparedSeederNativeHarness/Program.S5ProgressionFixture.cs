#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Accounts;
using MudSharp.Framework;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PrepareS5ProgressionFixture(TestDatabase database, string path)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var settings = ArmageddonInstallation.Read(db)!;
		var stock = new ArmageddonStockProgression(settings.Mode);
		var claims = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package && !x.Retired && x.LogicalId > 0)
			.ToDictionary(x => x.StableKey, x => x.LogicalId!.Value);
		var spec = stock.Capabilities.Single(x => x.Key == "arm.capability.earth");
		var traits = stock.Bindings.Select(x => x.TraitKey).Distinct().ToDictionary(x => x, x => claims[x]);
		var spells = spec.SpellKeys.ToDictionary(x => x, x => claims[x]);
		var reserve = claims[spec.ResourceKey];
		var casting = stock.Casting(spec, traits, reserve, spells, claims[ArmageddonProgressionInstaller.SourceResourceKey]);
		if (S6Integration)
		{
			casting = new XElement(XElement.Parse(db.MagicCapabilities.Single(x => x.Id == claims[spec.Key]).Definition).Element("Casting")!);
			Require((bool?)casting.Attribute("enabled") == false && casting.Elements("Admission").Count() == spec.SpellKeys.Count,
				"S6 must use the complete persisted stock policy, not reconstruct a missing graph.");
			casting.SetAttributeValue("enabled", true); // Only the separate disposable qualification copy.
		}
		casting.SetAttributeValue("identity", Guid.NewGuid());
		var generator = db.MagicGenerators.Single(x => x.Id == claims["arm.generator.earth"]);
		var regen = XElement.Parse(generator.Definition);
		Require((double?)regen.Element("AmountPerMinute") == 2 && (double?)regen.Element("RestMultiplier") == 2 &&
			(bool?)regen.Element("ConsciousOnly") == true, "Locked native Earth regeneration changed.");
		var capability = new Db.MagicCapability { Name = "S5 full stock Earth qualification", CapabilityModel = "skilllevel", PowerLevel = 1,
			MagicSchoolId = db.MagicCapabilities.Single(x => x.Id == claims[spec.Key]).MagicSchoolId,
			Definition = new XElement("Definition", new XElement("ConcentrationTrait", (long)casting.Attribute("trait")!),
				new XElement("ConcentrationCapabilityExpression", "1"), new XElement("ConcentrationDifficultyExpression", "5"),
				new XElement("Regenerators", new XElement("Regenerator", generator.Id)), casting).ToString() };
		db.MagicCapabilities.Add(capability); db.SaveChanges();
		var merit = new Db.Merit { Name = "S5 full stock Earth qualification caster", Type = "Magic Capability", MeritScope = 2, MeritType = 0,
			Definition = new XElement("Definition", new XElement("ChargenAvailableProg", settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg]),
				new XElement("ApplicabilityProg", settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]), new XElement("ChargenBlurb", "Isolated S5 qualification"),
				new XElement("DescriptionText", "@ have|has Earth magic."), new XElement("Capabilities", new XElement("Capability", capability.Id))).ToString() };
		db.Merits.Add(merit); db.SaveChanges();
		var input = JsonNode.Parse(File.ReadAllText(path))!.AsObject();
		var template = db.Characters.AsNoTracking().Single(x => x.Id == input["character"]!.GetValue<long>());
		var templateBody = db.Bodies.AsNoTracking().Single(x => x.Id == template.BodyId);
		var templateAccount = db.Accounts.AsNoTracking().Single(x => x.Id == template.AccountId);
		// Only a new disposable actor is prepared. No acquisition, energy, controlled grades or opportunity history is copied.
		var account = new Db.Account(); db.Entry(account).CurrentValues.SetValues(templateAccount);
		account.Id = 0; account.Name = "S5Learner"; account.Email = ""; account.LastLoginTime = null;
		account.AuthorityGroupId = db.AuthorityGroups.Single(x => x.AuthorityLevel == (int)PermissionLevel.Player).Id;
		db.Accounts.Add(account); db.SaveChanges();
		var body = new Db.Body(); db.Entry(body).CurrentValues.SetValues(templateBody);
		body.Id = 0; body.ShortDescription = "a stock learning participant"; body.FullDescription = "A stock learning participant is here.";
		body.ShortDescriptionPatternId = null; body.FullDescriptionPatternId = null; body.EffectData = "<Effects />";
		db.Bodies.Add(body); db.SaveChanges();
		foreach (var t in db.Traits.AsNoTracking().Where(x => x.BodyId == templateBody.Id).ToArray())
			db.Traits.Add(new() { BodyId = body.Id, TraitDefinitionId = t.TraitDefinitionId, Value = t.Value, AdditionalValue = t.AdditionalValue });
		var actor = new Db.Character(); db.Entry(actor).CurrentValues.SetValues(template);
		actor.Id = 0; actor.AccountId = account.Id; actor.BodyId = body.Id; actor.Name = "S5Learner";
		actor.NameInfo = actor.NameInfo.Replace("Admin", "S5Learner", StringComparison.Ordinal);
		actor.IsAdminAvatar = false; actor.EffectData = "<Effects />"; actor.LastLoginTime = null; actor.LastLogoutTime = null;
		actor.NeedsModel = "Active"; actor.CurrentCombatSettingId = null;
		actor.FoodSatiatedHours = 16; actor.DrinkSatiatedHours = 8; actor.WaterLitres = 3;
		db.Characters.Add(actor); db.SaveChanges();
		db.CharacterBodies.Add(new() { CharacterId = actor.Id, BodyId = body.Id, Alias = "base", TransformationEcho = "", AllowVoluntarySwitch = true });
		var ownedTraits = traits.Values.Distinct().ToList();
		foreach (var t in db.CharacterTraits.AsNoTracking().Where(x => x.CharacterId == template.Id && !ownedTraits.Contains(x.TraitDefinitionId)).ToArray())
			db.CharacterTraits.Add(new() { CharacterId = actor.Id, TraitDefinitionId = t.TraitDefinitionId, Value = t.Value, AdditionalValue = t.AdditionalValue });
		foreach (var language in db.CharactersLanguages.AsNoTracking().Where(x => x.CharacterId == template.Id).ToArray())
			db.CharactersLanguages.Add(new() { CharacterId = actor.Id, LanguageId = language.LanguageId });
		foreach (var accent in db.CharactersAccents.AsNoTracking().Where(x => x.CharacterId == template.Id).ToArray())
			db.CharactersAccents.Add(new() { CharacterId = actor.Id, AccentId = accent.AccentId, Familiarity = accent.Familiarity });
		db.PerceiverMerits.Add(new() { CharacterId = actor.Id, MeritId = merit.Id });
		var room = db.Rooms.Include(x => x.CurrentOverlay).Single(x => x.Id == actor.Location); room.CurrentOverlay.AddedLight = 1000;
		db.SaveChanges();
		Require(!db.CharacterAcquiredSpells.Any(x => x.CharacterId == actor.Id) && !db.CharactersMagicResources.Any(x => x.CharacterId == actor.Id),
			"S5 ordinary learner must start with no learned spells or energy.");
		var capacityRaw = db.Traits.Single(x => x.BodyId == body.Id && x.TraitDefinitionId == settings.CapacityAttribute).Value;
		var capacity = Math.Max(2 * capacityRaw, Math.Min(12 * capacityRaw, 100 + 3 * (capacityRaw - 13)));
		Require(capacity >= 50, "Prepared native attribute capacity cannot fund the first stock practice.");
		var root = stock.Bindings.Single(x => x.CapabilityKey == spec.Key && x.ContentKey == "arm.spell.earthen_ward");
		var child = stock.Bindings.Single(x => x.CapabilityKey == spec.Key && x.ContentKey == "arm.spell.stone_skin");
		var schema = input["sql_schema"]!.AsArray();
		schema.Add(JsonSerializer.SerializeToNode(PreparedStockSqlTable<Db.CharacterMagicSkillOpportunity>(db, "CharacterMagicSkillOpportunities",
			[nameof(Db.CharacterMagicSkillOpportunity.CharacterId), nameof(Db.CharacterMagicSkillOpportunity.TraitDefinitionId), nameof(Db.CharacterMagicSkillOpportunity.NextOpportunityUtc), nameof(Db.CharacterMagicSkillOpportunity.StateVersion)])));
		var practice = XElement.Parse(db.MagicSpells.Single(x => x.Id == spells[root.ContentKey]).Definition).Element("ControlledPower")!;
		Require((double?)practice.Element("Practice")?.Attribute("seconds") == 30 && (double?)practice.Element("Practice")?.Attribute("energy") == 1,
			"Stock practice duration/cost multiplier changed.");
		var improver = db.Improvers.Single(x => x.Id == db.TraitDefinitions.Single(x => x.Id == traits[root.TraitKey]).ImproverId);
		Require((double?)XElement.Parse(improver.Definition).Attribute("Chance") == 0.1 && (string?)XElement.Parse(improver.Definition).Attribute("Expression") == "0.5",
			"Stock improver differs from locked native chance/gain.");
		input["s5_progression"] = JsonSerializer.SerializeToNode(new { mode = settings.Mode.ToString(), actor = actor.Id, body = body.Id, account = account.Name,
			capability = capability.Id, merit = merit.Id, resource = reserve, source_resource = claims[ArmageddonProgressionInstaller.SourceResourceKey],
			trait = traits[root.TraitKey], root = spells[root.ContentKey], child = spells[child.ContentKey], opening = root.Opening,
			threshold = settings.Mode == ArmageddonProgressionMode.Classic ? child.ParentThreshold : child.MainThreshold,
			roots = stock.Bindings.Where(x => x.CapabilityKey == spec.Key && x.Starting && !x.Support).Select(x => spells[x.ContentKey]).ToArray(),
			all_spells = spells.Values.ToArray(), skill_ids = ownedTraits,
			starting_skills = stock.Bindings.Where(x => x.CapabilityKey == spec.Key && x.Starting).GroupBy(x => traits[x.TraitKey])
				.ToDictionary(x => x.Key, x => x.Max(y => y.Opening)),
			resource_name = db.MagicResources.Single(x => x.Id == reserve).Name, improver = improver.Definition, generator = generator.Definition,
			use_gate = claims[ArmageddonStockFocusDeviceInstaller.UseGateKey], craft_gate = claims[ArmageddonStockFocusDeviceInstaller.CraftGateKey],
			capacity_attribute = settings.CapacityAttribute, capacity_raw = capacityRaw, capacity, invoked = "armearth", rest_funding_timeout_seconds = 900,
			limits = "Actual full stock Earth graph on a separate enabled qualification capability; original package gates stay false. New Player account/body; no initial mana, knowledge, skill or grade injection. Native stochastic gain may be zero; full branching uses separate deterministic shared-contract coverage." });
		if (S6Integration) PrepareS6GatheringFixture(db, input, actor, claims);
		if (S6UiFunding) PrepareS6UiFundingFixture(db, input, claims);
		if (S6ProgressionFaults) PrepareS6ProgressionFaultFixture(db, input, actor, claims);
		if (S6BorrowFault) PrepareS6BorrowFaultFixture(db, input, actor, claims);
		if (S6Readiness)
		{
			PrepareS6ChargenFixture(db, input, claims);
			PrepareS6UncertaintyFixture(db, input, actor, claims);
		}
		File.WriteAllText(path, input.ToJsonString());
		Console.WriteLine("ARMS5-progression-input=prepared full stock " + settings.Mode + " Earth graph; ordinary Player; zero resource/acquisition; locked native improver/generator");
	}
}

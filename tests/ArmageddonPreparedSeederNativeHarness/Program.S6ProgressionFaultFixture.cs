#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static bool S6ProgressionFaults => Environment.GetEnvironmentVariable("FUTUREMUD_S6_MEASUREMENT_SCOPE") == "progression-faults";

	private static void PrepareS6ProgressionFaultFixture(MudSharp.Database.FuturemudDatabaseContext db, JsonObject input,
		Db.Character template, IReadOnlyDictionary<string, long> ids)
	{
		PrepareS6UncertaintyFixture(db, input, template, ids);
		var cfg = input["s6_uncertainty"]!.AsObject();
		var routes = cfg["routes"]!.AsArray();
		foreach (var alias in new[] { "debit", "effect" })
		{
			var route = routes.Single(x => x!["alias"]!.GetValue<string>() == alias)!;
			var source = db.MagicSpells.AsNoTracking().Single(x => x.Id == route["spell"]!.GetValue<long>());
			var originalTrait = db.TraitDefinitions.AsNoTracking().Single(x => x.Id == source.CastingTraitDefinitionId);
			Require(originalTrait.OwnerScope == 1, "Progression fault skills must remain canonical-character owned.");
			var improver = new Db.Improver();
			db.Entry(improver).CurrentValues.SetValues(db.Improvers.AsNoTracking().Single(x => x.Id == originalTrait.ImproverId));
			improver.Id = 0;
			improver.Name = $"S6 controlled {alias} positive gain";
			improver.Definition = "<Definition Chance='1' Expression='0.5' ImproveOnFail='true' ImproveOnSuccess='true' DifficultyThresholdInterval='0' NoGainSecondsDiceExpression='0' ImprovementProg='0' />";
			db.Improvers.Add(improver);
			db.SaveChanges();
			var trait = new Db.TraitDefinition();
			db.Entry(trait).CurrentValues.SetValues(originalTrait);
			trait.Id = 0;
			trait.Name = $"S6 controlled {alias} progression skill";
			trait.Alias = $"s6{alias}progress";
			trait.ImproverId = improver.Id;
			db.TraitDefinitions.Add(trait);
			db.SaveChanges();
			var spell = new Db.MagicSpell();
			db.Entry(spell).CurrentValues.SetValues(source);
			spell.Id = 0;
			spell.Name = $"S6 controlled {alias} {source.Name}";
			spell.CastingTraitDefinitionId = trait.Id;
			spell.MinimumSuccessThreshold = (int)Outcome.MajorFail;
			var definition = XElement.Parse(spell.Definition);
			if (alias == "effect") definition.Element("ControlledPower")!.SetAttributeValue("masteryChance", 1);
			spell.Definition = definition.ToString();
			db.MagicSpells.Add(spell);
			db.SaveChanges();
			foreach (var selected in routes.Where(x => x!["alias"]!.GetValue<string>() == alias || alias == "debit" && x!["alias"]!.GetValue<string>() == "alternate"))
			{
				var cap = db.MagicCapabilities.Single(x => x.Id == selected!["capability"]!.GetValue<long>());
				var policy = XElement.Parse(cap.Definition);
				policy.Element("ConcentrationTrait")!.Value = trait.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
				policy.Element("Casting")!.SetAttributeValue("trait", trait.Id);
				var admission = policy.Element("Casting")!.Element("Admission")!;
				admission.SetAttributeValue("spell", spell.Id);
				admission.SetAttributeValue("opening", 60);
				cap.Definition = policy.ToString();
				selected!["spell"] = spell.Id;
				selected["name"] = spell.Name;
				selected["trait"] = trait.Id;
				selected["original_spell"] = source.Id;
			}
		}
		db.SaveChanges();
		cfg["limits"] = "Controlled provider mechanism copies only: separate copied character-owned skills/improvers and spells, positive native 0.5 gain, mastery chance1 and permissive manifestation threshold. Stock prices, canonical definitions and original gates unchanged. Copied-body Player capacity136 funds125 through two finite paid native draws; not ordinary balance evidence. Actual MySQL errors after flushed raw gain and persisted grade; quarantine writes allowed.";
		input["sql_schema"]!.AsArray().Add(JsonSerializer.SerializeToNode(PreparedStockSqlTable<Db.CharacterAcquiredSpell>(db, "CharacterAcquiredSpells", ["CharacterId", "MagicSpellId", "ControlledGrade", "NextMasteryUtc", "StateVersion"])));
		input["s6_progression_faults"] = cfg.DeepClone();
	}
}

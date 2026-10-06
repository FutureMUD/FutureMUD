extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonWaterSeeInstaller
{
	internal static IReadOnlyList<string> ValidateMappings(FuturemudDatabaseContext db, ArmageddonWaterSeeBindings? bindings)
	{
		var errors = new List<string>();
		if (bindings?.WaterLiquids is null || bindings.DivinationRankTags is null)
			return ["Water/See requires explicit water liquids, Silt/Shadow terrains and five ordered Divination rank tags."];
		var water = bindings.WaterLiquids;
		if (water.Count is < 1 or > 128 || water.Any(x => x <= 0) || water.Distinct().Count() != water.Count ||
			water.Any(id => !db.Liquids.AsNoTracking().Any(x => x.Id == id)))
			errors.Add("Water Breathing requires one to 128 distinct existing native liquid IDs; no inferred Water/CountsAs mapping.");
		if (bindings.SiltTerrain <= 0 || bindings.ShadowTerrain <= 0 ||
			!db.Terrains.AsNoTracking().Any(x => x.Id == bindings.SiltTerrain) ||
			!db.Terrains.AsNoTracking().Any(x => x.Id == bindings.ShadowTerrain))
			errors.Add("See the Unbodied requires explicit existing Silt and Shadow terrain IDs. The same terrain may serve both; Silt refuses first.");
		var ranks = bindings.DivinationRankTags;
		var tags = db.Tags.AsNoTracking().ToDictionary(x => x.Id);
		if (ranks.Count != 5 || ranks.Any(x => x <= 0 || !tags.ContainsKey(x)) || ranks.Distinct().Count() != 5)
			errors.Add("Select exactly five distinct existing Divination tag IDs in rank 0 through rank 4 order.");
		else
		{
			bool Ancestors(long id, out HashSet<long> ancestors)
			{
				ancestors = new();
				long? current = id;
				while (current is { } tag)
				{
					if (!ancestors.Add(tag) || !tags.TryGetValue(tag, out var row)) return false;
					current = row.ParentId;
				}
				return true;
			}
			for (var rank = 0; rank < 5; rank++)
			{
				if (!Ancestors(ranks[rank], out var ancestors))
					errors.Add($"Divination rank {rank} has a missing ancestor or cycle; repair the selected native hierarchy explicitly.");
				else if (rank > 0 && !ancestors.Contains(ranks[rank - 1]))
					errors.Add($"Divination rank {rank} must descend from rank {rank - 1}; direct parenthood is not required.");
			}
		}
		return errors;
	}

	private static void Preflight(FuturemudDatabaseContext db, ArmageddonWaterSeeInstallPlan plan, List<Contribution> contributions,
		Dictionary<string, SeederManagedRecord> records, List<string> errors)
	{
		foreach (var contribution in contributions)
		{
			if (records.TryGetValue(contribution.Key, out var record))
			{
				if (record.Retired || record.EntityType != contribution.Type.Name || record.RevisionNumber is not null ||
					record.ManifestVersion != Version || Find(db, contribution, record) is null)
					errors.Add($"{contribution.Key}: owned row missing/retired/invalid; restore or explicitly rebind, no resurrection.");
				if (db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
					errors.Add($"{contribution.Key}: competing retained ownership claim.");
			}
			else if (db.SeederManagedRecords.Any(x => x.Seeder == Package && x.StableKey == contribution.Key))
				errors.Add($"{contribution.Key}: cross-module ownership key; no adoption.");
			else if (contribution.Type == typeof(MagicSpell) && (db.MagicSpells.Any(x => x.MagicSchoolId == plan.School && x.Name.ToLower() == contribution.Name.ToLower()) ||
				db.MagicSpells.AsNoTracking().Select(x => x.Definition).AsEnumerable().Any(x => StockIdentity(x) == contribution.Key)))
				errors.Add($"{contribution.Key}: unowned spell name/stock-identity collision.");
		}
		if (records.Count > 0 && !records.Keys.Order().SequenceEqual(contributions.Select(x => x.Key).Order()))
			errors.Add("Incomplete or unknown retained Water/See keys require explicit ownership reconciliation.");
		errors.AddRange(ValidateMappings(db, plan.Bindings));
		if (new[] { plan.School, plan.Resource, plan.AlwaysFalseProg, plan.WaterBreathingSkill, plan.SeeTheUnbodiedSkill }.Any(x => x <= 0) ||
			!db.MagicSchools.Any(x => x.Id == plan.School) ||
			!db.MagicResources.Any(x => x.Id == plan.Resource && (x.MagicResourceType & (int)MagicResourceType.PlayerResource) != 0))
			errors.Add("Select the existing native school and player-capable source resource.");
		foreach (var (key, skill) in new[] { (WaterBreathingKey, plan.WaterBreathingSkill), (SeeTheUnbodiedKey, plan.SeeTheUnbodiedSkill) })
		{
			var owner = db.SeederManagedRecords.AsNoTracking().SingleOrDefault(x => x.Seeder == Package &&
				x.Module == ArmageddonTraditionInstaller.Module && x.StableKey == key + ".skill");
			if (!db.TraitDefinitions.Any(x => x.Id == skill && x.Type == 0 && x.OwnerScope == 1) ||
				owner is null || owner.EntityType != nameof(TraitDefinition) || owner.LogicalId != skill || owner.Retired ||
				owner.RevisionNumber is not null || owner.ManifestVersion != ArmageddonTraditionInstaller.Version ||
				db.SeederManagedRecords.Any(x => x.Id != owner.Id && x.EntityType == nameof(TraitDefinition) && x.LogicalId == skill))
				errors.Add($"{key}: use its existing owned character-source skill from tradition bootstrap.");
		}
		ValidatePrerequisiteDefinitions(db, plan.Resource, errors);
		var progs = db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray();
		using var compiler = new OfflineProgCompilation(progs);
		if (progs.SingleOrDefault(x => x.Id == plan.AlwaysFalseProg) is not { } no || no.FunctionText.Trim() != "return false" ||
			compiler.Compile(no.Id) is var compiled && (compiled.ReturnType != ProgVariableTypes.Boolean || !compiled.MatchesParameters([])))
			errors.Add("Select the ordinary compiled no-argument always-false support prog.");
		if (errors.Count != 0) return;
		foreach (var content in Content(plan))
		{
			var generated = content.EligibilityRow(0)!; generated.Id = -1;
			using var generatedCompiler = new OfflineProgCompilation(progs.Append(generated));
			var predicate = generatedCompiler.Compile(generated.Id);
			if (predicate.ReturnType != ProgVariableTypes.Boolean || !predicate.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Character]))
				errors.Add($"{content.Key}: eligibility must compile as native boolean(character, character).");
		}
	}

	private static void ValidatePrerequisiteDefinitions(FuturemudDatabaseContext db, long resource, List<string> errors)
	{
		foreach (var (key, module) in new[]
		{
			(ArmageddonReviewedUtilityContent.SenseEnchantmentKey, ArmageddonMagicInstaller.Module),
			(ArmageddonReviewedUtilityContent.UnravelEnchantmentKey, ArmageddonMagicInstaller.Module),
			(ArmageddonReviewedUtilityContent.DrawWaterKey, ArmageddonMagicInstaller.Module),
			(ArmageddonReviewedProvisionContent.DrawWineKey, ArmageddonProvisionInstaller.Module),
			(ArmageddonReviewedPierceContent.Key, ArmageddonPierceInstaller.Module)
		})
		{
			var record = db.SeederManagedRecords.AsNoTracking().SingleOrDefault(x => x.Seeder == Package && x.Module == module && x.StableKey == key);
			var spell = record?.LogicalId is { } id ? db.MagicSpells.AsNoTracking().SingleOrDefault(x => x.Id == id) : null;
			if (record is null || record.Retired || record.EntityType != nameof(MagicSpell) || record.RevisionNumber is not null ||
				record.ManifestVersion != Version || spell is null ||
				db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
			{ errors.Add($"{key}: complete owned prerequisite content is required; no name-based adoption or free acquisition."); continue; }
			try
			{
				var xml = XElement.Parse(spell.Definition);
				if ((string?)xml.Element("StockIdentity") != key || xml.Element("ControlledPower") is not { } profile ||
					(int?)profile.Attribute("schema") != 1 || !profile.Elements("Grade").Select(x => (int?)x.Attribute("number")).SequenceEqual(Enumerable.Range(1, 7).Select(x => (int?)x)) ||
					xml.Element("Costs")?.Elements("Cost").Any(x => (long?)x.Attribute("resource") == resource) != true || xml.Element("Effects")?.Elements().Any() != true)
					errors.Add($"{key}: retain a complete native reviewed prerequisite definition with seven grades and source cost.");
			}
			catch (Exception error) when (error is System.Xml.XmlException or FormatException or InvalidOperationException)
			{ errors.Add($"{key}: prerequisite definition is unreadable; preserved without repair."); }
		}
	}

	private static string? StockIdentity(string xml)
	{
		try { return (string?)XElement.Parse(xml).Element("StockIdentity"); }
		catch { return null; }
	}
}

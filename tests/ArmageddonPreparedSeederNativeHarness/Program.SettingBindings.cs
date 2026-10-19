// Installed-row audit for the frozen Armageddon binding inventories.
// Suggested placement: Program.CoreFoundations.BindingAudit.cs (same partial GNHProgram).
// Invoke after combined Setting install and after its rerun, before boot.
// Inventory files must be copied as exact, hash-pinned test evidence, not read from a personal path.
#nullable enable
using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using RuntimeExpression = MudSharp.Body.Traits.TraitExpression;
using CultureInfo = System.Globalization.CultureInfo;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void RunSettingBindingAudit(TestDatabase database, string phase)
	{
		IReadOnlyList<InventoryRow> Read(string file, string expected, Func<IReadOnlyDictionary<string,string>,string> key)
		{
			var path=Path.Combine(CatalogueRepository,"DatabaseSeeder","Assets","Manifests","Armageddon",file);
			Require(BaseHash(path)==expected,"Frozen binding inventory digest changed: "+file);
			var lines=File.ReadAllLines(path); var headings=lines[0].Split('\t');
			return lines.Skip(1).Where(x=>!string.IsNullOrWhiteSpace(x)).Select(line=> {
				var cells=line.Split('\t'); Require(cells.Length==headings.Length,"Malformed binding row: "+file);
				var fields=headings.Select((name,index)=>(name,index)).ToDictionary(x=>x.name,x=>cells[x.index]);
				return new InventoryRow(key(fields),fields);
			}).ToArray();
		}
		using var db=NewIndependentContext(database.ConnectionString);
		var evidence=AuditInstalledSettingBindings(db,
			Read("MUNDANE_CHECK_BINDINGS_20261018.tsv","0C2ED56A750F82A6480D3F1CFE79F964D33F561ED7FA6082BEB22A2542132540",x=>x["seeder_path"]+":"+x["line"]+":"+x["check_type"]),
			Read("MUNDANE_STATIC_DEPENDENCIES_20261018.tsv","E754DB141EC7645D3C85C1F3667DEB3E509423DCE8296CCA8DE6C4CA70F2A478",x=>x["config_key"]),
			Read("MUNDANE_DYNAMIC_BINDING_OBLIGATIONS_20261018.tsv","5D27CAF8D2442785810A31C017D6A014B39E47D073CA27DD49B8E64069345615",x=>x["canonical_key"]+"|"+x["route"]),ReviewedStaticProofs(),ReviewedDynamicProofs());
		var current=AuditCurrentStaticExtras(db);
		var output=Path.GetDirectoryName(Environment.GetEnvironmentVariable("FUTUREMUD_BASE_WORLD_RECEIPT"))!;
		File.WriteAllText(Path.Combine(output,"setting-bindings-"+phase+".json"),JsonSerializer.Serialize(new {Historical=evidence,Current=current},BaseJsonOptions));
		Console.WriteLine("SETTING-bindings="+phase+" PASS 291cases/236types 55statics 71dynamic current-Pilot/MountControl explicit-optional-absences");
	}

	private sealed record InventoryRow(string Identity, IReadOnlyDictionary<string, string> Fields);
	private sealed record BindingProof(string Disposition, string DecisionEvidence,
		Func<FuturemudDatabaseContext, IReadOnlyList<string>> Inspect);
	private sealed record BindingEvidence(string Identity, string Disposition, string DecisionEvidence,
		IReadOnlyList<string> Rows);

	// checkCases: exactly 291 rows; Identity = seeder_path + ':' + line + ':' + check_type.
	// staticInventory: exactly 55 rows; Identity = config_key.
	// dynamicInventory: exactly 71 rows; Identity = canonical_key + '|' + route.
	// Reject duplicates, missing/extra proof keys and unknown dispositions. Do not invent a fallback proof.
	// Approved input SHA256:
	// checks  0C2ED56A750F82A6480D3F1CFE79F964D33F561ED7FA6082BEB22A2542132540
	// statics E754DB141EC7645D3C85C1F3667DEB3E509423DCE8296CCA8DE6C4CA70F2A478
	// dynamic 5D27CAF8D2442785810A31C017D6A014B39E47D073CA27DD49B8E64069345615
	private static object AuditInstalledSettingBindings(FuturemudDatabaseContext db,
		IReadOnlyList<InventoryRow> checkCases, IReadOnlyList<InventoryRow> staticInventory,
		IReadOnlyList<InventoryRow> dynamicInventory,
		IReadOnlyDictionary<string, BindingProof> staticProofs,
		IReadOnlyDictionary<string, BindingProof> dynamicProofs)
	{
		Require(checkCases.Count == 291 && checkCases.Select(x => x.Identity).Distinct().Count() == 291,
			"Pinned check-case inventory differs.");
		var types = checkCases.Select(x => Enum.Parse<CheckType>(x.Fields["check_type"], false)).Distinct().ToArray();
		Require(types.Length == 236, "Pinned check-type inventory differs.");
		var checks = db.Checks.AsNoTracking().ToDictionary(x => x.Type);
		var expressions = db.TraitExpressions.AsNoTracking().ToDictionary(x => x.Id);
		var templates = db.CheckTemplates.AsNoTracking().ToDictionary(x => x.Id);
		var traits = db.TraitDefinitions.AsNoTracking().ToDictionary(x => x.Id);
		var parameters = db.TraitExpressionParameters.AsNoTracking().ToArray().ToLookup(x => x.TraitExpressionId);
		var checkEvidence = new List<object>();
		foreach (var type in types)
		{
			if (TryAuditAbsentTrapCheck(db, type))
			{
				checkEvidence.Add(new { Type = type.ToString(), Disposition = "disabled-no-consumer", Evidence = "Optional TrapSeeder not installed; no templates or seven adapter check rows" });
				continue;
			}
			// Deliberately no world.GetCheck: a missing row must never pass via Default Fallback Check.
			Require(checks.ContainsKey((int)type), "Missing installed check row: " + type);
			var check = checks[(int)type];
			Require(expressions.ContainsKey(check.TraitExpressionId), "Dangling check expression: " + type);
			Require(templates.ContainsKey(check.CheckTemplateId), "Dangling check template: " + type);
			var expression = expressions[check.TraitExpressionId];
			var tableParameters = parameters[expression.Id].ToArray();
			Require(tableParameters.Select(x => x.Parameter).Distinct(StringComparer.Ordinal).Count() == tableParameters.Length,
				"Duplicate persisted expression parameter: " + type);
			var embedded = RuntimeExpression.TraitFormulaRegex.Matches(expression.Expression).Cast<System.Text.RegularExpressions.Match>()
				.Select(x => (Name: x.Groups["name"].Value,
					Id: long.Parse(x.Groups["reference"].Value, CultureInfo.InvariantCulture))).ToArray();
			foreach (var reference in tableParameters.Select(x => x.TraitDefinitionId).Concat(embedded.Select(x => x.Id)))
				Require(traits.ContainsKey(reference), $"Dangling trait {reference} in {type}/{expression.Id}.");
			// Runtime inline references override table parameters. Report/fail stale conflicting persisted IDs
			// in this fresh-world qualification rather than concealing them through the runtime override.
			foreach (var value in embedded)
			{
				var persisted = tableParameters.SingleOrDefault(x => x.Parameter == value.Name);
				Require(persisted is null || persisted.TraitDefinitionId == value.Id,
					$"Stale persisted parameter {type}/{value.Name}: table={persisted?.TraitDefinitionId}, inline={value.Id}.");
			}
			checkEvidence.Add(new { Type = type.ToString(), check.TraitExpressionId, check.CheckTemplateId,
				expression.Expression, Parameters = tableParameters.Select(x => new { x.Parameter, x.TraitDefinitionId, x.CanImprove, x.CanBranch }),
				Embedded = embedded.Select(x => new { x.Name, x.Id }) });
		}
		// Also resolve references of all actual installed checks, including newer checks absent from the
		// historical inventory (e.g. MountControlCheck). Do not replace the 236 historical coverage count.
		foreach (var check in checks.Values)
		{
			Require(expressions.ContainsKey(check.TraitExpressionId) && templates.ContainsKey(check.CheckTemplateId),
				"Unresolved extra installed check: " + check.Type);
			foreach (var param in parameters[check.TraitExpressionId])
				Require(traits.ContainsKey(param.TraitDefinitionId), "Dangling extra-check parameter: " + check.Type);
			foreach (System.Text.RegularExpressions.Match match in RuntimeExpression.TraitFormulaRegex.Matches(expressions[check.TraitExpressionId].Expression))
				Require(traits.ContainsKey(long.Parse(match.Groups["reference"].Value, CultureInfo.InvariantCulture)),
					"Dangling extra-check inline reference: " + check.Type);
		}
		return new { CheckCases = checkCases.Count, CheckTypes = types.Length, InstalledCheckRows = checks.Count,
			Checks = checkEvidence, Statics = AuditDispositions(db, staticInventory, staticProofs, 55),
			Dynamic = AuditDispositions(db, dynamicInventory, dynamicProofs, 71), CurrentExtras = AuditCurrentStaticExtras(db) };
	}

	private static BindingEvidence[] AuditDispositions(FuturemudDatabaseContext db,
		IReadOnlyList<InventoryRow> inventory, IReadOnlyDictionary<string, BindingProof> proofs, int count)
	{
		var ids = inventory.Select(x => x.Identity).ToHashSet(StringComparer.Ordinal);
		Require(inventory.Count == count && ids.Count == count && ids.SetEquals(proofs.Keys),
			"Missing, duplicate or extra binding disposition: expected " + count);
		var allowed = new[] { "bound", "engine-scaffold-ungranted", "disabled-no-consumer", "approved-content-deferred" };
		return inventory.Select(row =>
		{
			var proof = proofs[row.Identity];
			Require(allowed.Contains(proof.Disposition) && !string.IsNullOrWhiteSpace(proof.DecisionEvidence),
				"Unreviewed disposition: " + row.Identity);
			var observed = proof.Inspect(db); // Must assert concrete actual DB rows/gates; throws on mismatch.
			Require(observed.Count > 0, "A disposition returned no row/gate evidence: " + row.Identity);
			return new BindingEvidence(row.Identity, proof.Disposition, proof.DecisionEvidence, observed);
		}).ToArray();
	}

	// Explicit proof factory for fixed trait-ID statics. Validate old inventory plus current appended
	// VehiclePilotTraitId/Enabled separately: neither appears in the historical 55-key inventory.
	private static BindingProof FixedTraitStatic(string config, string canonical, string decision) =>
		new("bound", decision, db =>
		{
			var owned = DatabaseSeeder.Seeders.ArmageddonSkillPackage.Resolve(db);
			var row = db.StaticConfigurations.AsNoTracking().Single(x => x.SettingName == config);
			Require(long.TryParse(row.Definition, NumberStyles.None, CultureInfo.InvariantCulture, out var id) && id == owned[canonical].Id,
				"Wrong canonical static binding: " + config);
			return new[] { $"StaticConfigurations[{config}]={id};{canonical}" };
		});

	private static BindingProof WeaponFamily(string canonical, string typeName, string decision) =>
		new("bound", decision, db =>
		{
			var skills = DatabaseSeeder.Seeders.ArmageddonSkillPackage.Resolve(db);
			var type = db.WeaponTypes.AsNoTracking().Single(x => x.Name == typeName);
			Require(type.AttackTraitId == skills[canonical].Id && type.ParryTraitId == skills["arm.skill.parry"].Id,
				"Wrong weapon family/independent Parry: " + typeName);
			var attacks = db.WeaponAttacks.AsNoTracking().Where(x => x.WeaponTypeId == type.Id).ToArray();
			Require(attacks.Length > 0, "Named-only weapon type has no attack consumer: " + typeName);
			foreach (var attack in attacks)
			{
				foreach (var expression in new[] { attack.DamageExpressionId, attack.StunExpressionId, attack.PainExpressionId })
					Require(db.TraitExpressions.Any(x => x.Id == expression), "Dangling weapon damage expression: " + attack.Name);
				Require(db.CombatMessagesWeaponAttacks.Any(x => x.WeaponAttackId == attack.Id), "Attack lacks combat-message linkage: " + attack.Name);
			}
			var components = db.GameItemComponentProtos.AsNoTracking().Where(x => x.Type == "MeleeWeapon").ToArray()
				.Where(x => (long?)XElement.Parse(x.Definition).Element("WeaponType") == type.Id).ToArray();
			Require(components.Length > 0, "Weapon lacks actual native component consumer: " + typeName);
			return new[] { $"WeaponTypes[{type.Id}]={canonical};Parry={type.ParryTraitId}",
				$"WeaponAttacks={string.Join(',', attacks.Select(x => x.Id))}",
				$"MeleeWeaponComponents={string.Join(',', components.Select(x => x.Id + ":" + x.RevisionNumber))}" };
		});
}

// Reviewed proof-registry requirements (not auto-filled by readiness labels):
// - Razor: WeaponFamily("arm.skill.razor_weapons", "Razor Weapon", dynamic inventory row 8).
// - Trident: WeaponFamily("arm.skill.trident_weapons", "Trident", dynamic inventory row 10).
// - Explicitly list all ten melee weapon families; also audit bow FireTrait/AimTrait and throwing route.
// - Check source-key candidates are a historical mapping inventory, not an authoritative expected formula:
//   have a separate reviewed per-type exact formula/trait/policy assertion for Flee, Ride, Hunt, Charge,
//   Rescue, DirectionSense, BlindFighting, Guarding, Analyze. Do not merely prove that any trait exists.
// - Static references need typed proof: trait ID, trait-expression ID, decorator, improvement model,
//   FutureProg ID (0 only if consumer explicitly supports it), enum/number/string or feature gate.
// - DefaultLift/DragWeightExpression and StreetwiseSkillExpression contain formulas, not numeric IDs:
//   inspect both inline IDs and their owning expression dependency rows where applicable.
// - Dynamic approved crafting/psychic deferrals require row-specific decision evidence AND queries proving
//   which enabled rows are absent; do not mark a currently enabled recipe/power as deferred.
// - 'armor_repair|craft recipe trait' and 'armor_repair|repair trait' are separate obligations.
// - Armor Repair recipe remains absent independently; its repair trait route now checks the authored native RepairKit binding.
// - Deferred psychic rows must distinguish original source grant from any similarly named magic spell.
// - Record observed IDs/values in the receipt. Comparing installed facts after rerun can detect drift;
//   count equality alone cannot certify canonical semantic bindings.
// - Current-new bindings beyond the historic counts belong in AdditionalChecks/Statics/Dynamic evidence.
// - Save evidence with the harness receipt writer after the read-only audit; this draft itself writes no DB.

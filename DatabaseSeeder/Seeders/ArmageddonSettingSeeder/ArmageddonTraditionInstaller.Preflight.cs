extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Models;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;
using GatheringPolicy = EngineCompiler::MudSharp.Magic.Gathering.MagicGatheringPolicy;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonTraditionInstaller
{
	private static void Preflight(FuturemudDatabaseContext db, ArmageddonTraditionInstallPlan plan,
		List<Definition> definitions, Dictionary<string, SeederManagedRecord> records, List<string> errors)
	{
		foreach (var definition in definitions)
		{
			if (records.TryGetValue(definition.Key, out var record))
			{
				if (record.Retired || record.EntityType != definition.Type.Name || record.RevisionNumber is not null || Find(db, definition, record) is null)
					errors.Add($"{definition.Key}: owned identity missing, retired or invalid. Restore or explicitly rebind; no resurrection.");
				if (db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
					errors.Add($"{definition.Key}: competing retained ownership claim.");
			}
			else if (db.SeederManagedRecords.Any(x => x.Seeder == Package && x.StableKey == definition.Key) || NameCollision(db, definition))
				errors.Add($"{definition.Key}: unowned name or cross-module key collision; no automatic adoption.");
		}
		if (records.Keys.Except(definitions.Select(x => x.Key)).Any()) errors.Add("Unknown retained module keys require explicit version reconciliation.");
		if (new[] { plan.School, plan.SourceResource, plan.ReserveResource, plan.Decorator, plan.AlwaysFalseProg, plan.AlwaysTrueProg, plan.GatheringTemplate }.Any(x => x <= 0) ||
			!db.MagicSchools.Any(x => x.Id == plan.School) ||
			!db.MagicResources.Any(x => x.Id == plan.SourceResource && (x.MagicResourceType & (int)MagicResourceType.PlayerResource) != 0) ||
			!db.MagicResources.Any(x => x.Id == plan.ReserveResource && (x.MagicResourceType & (int)MagicResourceType.PlayerResource) != 0) ||
			!db.TraitDecorators.Any(x => x.Id == plan.Decorator && (x.Type == "SimpleNumeric" || x.Type == "Range" || x.Type == "CurrentMax" || x.Type == "Percentage")))
			errors.Add("Select existing native school, source/reserve mana and trait decorator IDs.");
		var progs = db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToList();
		using var compiler = new OfflineProgCompilation(progs);
		foreach (var (id, text) in new[] { (plan.AlwaysFalseProg, "return false"), (plan.AlwaysTrueProg, "return true") })
		{
			if (progs.SingleOrDefault(x => x.Id == id) is not { } row || !row.FunctionText.Trim().Equals(text, StringComparison.OrdinalIgnoreCase))
			{ errors.Add("Select ordinary always-false/always-true support progs explicitly."); continue; }
			var compiled = compiler.Compile(id);
			if (compiled.ReturnType != ProgVariableTypes.Boolean || !compiled.MatchesParameters([])) errors.Add("Boolean support progs must take no arguments.");
		}
		var source = SourceRows.ToDictionary(x => x.Key);
		if (plan.ImplementedSpells.Keys.Any(x => !source.TryGetValue(x, out var row) || row.Kind != "spell") ||
			plan.ImplementedSpells.Values.Distinct().Count() != plan.ImplementedSpells.Count)
			errors.Add("Implemented bindings require unique native spell IDs and exact selected 82 source keys; no roster union.");
		foreach (var (key, id) in plan.ImplementedSpells)
		{
			var spell = db.MagicSpells.AsNoTracking().SingleOrDefault(x => x.Id == id);
			if (spell is null || id <= 0) { errors.Add($"{key}: native spell not found."); continue; }
			var xml = XElement.Parse(spell.Definition); var profile = xml.Element("ControlledPower");
			if ((string?)xml.Element("StockIdentity") != key || profile is null || (int?)profile.Attribute("schema") != 1 ||
				!profile.Elements("Grade").Select(x => (int)x.Attribute("number")!).SequenceEqual(Enumerable.Range(1, 7)) ||
				profile.Elements("Grade").Any(x => (double?)x.Attribute("skill") is not { } skill || !double.IsFinite(skill) || skill < 0 || skill > 100) ||
				xml.Element("Costs")?.Elements("Cost").Any(x => (long?)x.Attribute("resource") == plan.SourceResource) != true ||
				xml.Element("Effects")?.Elements().Any() != true)
				errors.Add($"{key}: select a complete reviewed stock definition with seven percentage gates, real effects and designated source cost.");
		}
		if (!plan.SupportSkills.ContainsKey("arm.support.gather")) errors.Add("Bind Gather explicitly to a real native gathering/check skill; it is the concentration and enrolled gathering support.");
		if (plan.SupportSkills.Keys.Any(x => !source.TryGetValue(x, out var row) || row.Kind != "support") ||
			plan.SupportSkills.Values.Distinct().Count() != plan.SupportSkills.Count)
			errors.Add("Support mappings require distinct native counterpart skills and exact source support keys.");
		foreach (var (key, id) in plan.SupportSkills)
		{
			var trait = db.TraitDefinitions.AsNoTracking().SingleOrDefault(x => x.Id == id);
			if (trait is null || id <= 0 || trait.Type != 0 || trait.OwnerScope != 1 || trait.ImproverId is null ||
			!db.Improvers.Any(x => x.Id == trait.ImproverId && (x.Type == "classic" || x.Type == "branching")) || trait.ExpressionId is null ||
				!db.TraitExpressions.Any(x => x.Id == trait.ExpressionId))
				errors.Add($"{key}: select a character-owned native skill with an existing cap and use improver; no dummy support.");
			if (key == "arm.support.vloran" && !db.Languages.Any(x => x.LinkedTraitId == id)) errors.Add("Vloran must map to an existing native language skill.");
			if (key == "arm.support.component_crafting" && !db.Crafts.Include(x => x.EditableItem).Any(x => x.CheckTraitId == id && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current))
				errors.Add("Component Crafting needs an existing approved native craft using the selected skill; no invented spell-crafting system.");
		}
		var template = db.MagicCapabilities.AsNoTracking().SingleOrDefault(x => x.Id == plan.GatheringTemplate);
		if (template?.CapabilityModel != "skilllevel") { errors.Add("Select a validated native skilllevel gathering template."); return; }
		var root = XElement.Parse(template.Definition); var gathering = root.Element("Gathering");
		if (gathering is null || (int?)gathering.Attribute("version") is not (1 or 2) || root.Element("ConcentrationCapabilityExpression") is null || root.Element("ConcentrationDifficultyExpression") is null)
		{ errors.Add("Gathering template requires versioned methods and native concentration expressions."); return; }
		var methods = gathering.Elements("Method").ToArray();
		if (methods.Length == 0 || methods.Length > 64 || methods.Select(x => (string?)x.Attribute("key")).Distinct().Count() != methods.Length ||
			methods.Select(x => (string?)x.Attribute("alias")).Distinct(StringComparer.OrdinalIgnoreCase).Count() != methods.Length)
			errors.Add("Gathering template has no methods or duplicate method identities.");
		foreach (var method in methods)
		{
			if (!Enum.TryParse<MagicGatheringMethodKind>((string?)method.Attribute("kind"), out var kind) || !Enum.IsDefined(kind) ||
				!Guid.TryParse((string?)method.Attribute("key"), out var key) || key == Guid.Empty ||
				(long?)method.Attribute("destination") != plan.ReserveResource ||
				(double?)method.Attribute("duration") is not { } duration || !double.IsFinite(duration) || duration <= 0 ||
				string.IsNullOrWhiteSpace((string?)method.Attribute("alias")) || string.IsNullOrWhiteSpace((string?)method.Attribute("name")) ||
				(double?)method.Attribute("min") is not { } minimum || !double.IsFinite(minimum) || minimum <= 0 ||
				(double?)method.Attribute("max") is not { } maximum || !double.IsFinite(maximum) || maximum < minimum)
				errors.Add("Gathering template has invalid method identity, kind, destination or duration; select a runtime-validated template.");
			foreach (var attribute in new[] { "ratio", "stamina", "minimumStamina", "damage", "pain", "stun" })
				if ((double?)method.Attribute(attribute) is { } value && (!double.IsFinite(value) || value < 0 || attribute == "ratio" && value == 0))
					errors.Add($"Gathering method has invalid {attribute}.");
			if (kind == MagicGatheringMethodKind.Gentle && ((long?)method.Attribute("source") is not { } resource || resource <= 0 ||
				!db.MagicResources.Any(x => x.Id == resource && (x.MagicResourceType & (int)MagicResourceType.LocationResource) != 0)))
				errors.Add("Gentle method requires an explicit existing native source resource.");
			if (kind != MagicGatheringMethodKind.Gentle && (long?)method.Attribute("source") is > 0)
				errors.Add("Self/Land methods must not contain a Gentle source resource.");
			if (kind == MagicGatheringMethodKind.Land && (method.Element("Land") is not { } land || !land.Elements("Source").Any()))
				errors.Add("Land method requires its real native source policy; no placeholder land method.");
			if (kind != MagicGatheringMethodKind.Land && method.Element("Land") is not null) errors.Add("Only Land methods may contain a Land subtree.");
			var healthPrice = new[] { "damage", "pain", "stun" }.Any(x => (double?)method.Attribute(x) is > 0) ||
				new[] { "damageProg", "painProg", "stunProg" }.Any(x => (long?)method.Attribute(x) is > 0);
			if (healthPrice && (!Enum.TryParse<WoundSeverity>((string?)method.Attribute("maximumHealthSeverity"), out var severity) || !Enum.IsDefined(severity) || severity == WoundSeverity.None))
				errors.Add("Health-priced gathering requires its native maximum wound severity.");
			if (kind == MagicGatheringMethodKind.Self && !healthPrice && (double?)method.Attribute("stamina") is not > 0 && (long?)method.Attribute("staminaProg") is not > 0)
				errors.Add("Self gathering requires a genuine bodily price; no free mana template.");
			foreach (var (attribute, signature) in new[] { ("permission", "permission"), ("durationProg", "numeric"), ("staminaProg", "numeric"),
				("damageProg", "numeric"), ("painProg", "numeric"), ("stunProg", "numeric"), ("onGathered", "onsuccess") })
			{
				if ((long?)method.Attribute(attribute) is not { } id || id == 0) continue;
				if (progs.All(x => x.Id != id) || !GatheringPolicy.ValidSignature(compiler.Compile(id), signature))
					errors.Add($"Gathering {attribute} prog missing or incompatible with native {signature} contract.");
			}
		}
		if (plan.AllowedMethods is { } allowed && (!allowed.Keys.Order().SequenceEqual(Variants.Order()) ||
			allowed.Values.Any(x => x.Count == 0 || x.Distinct().Count() != x.Count || x.Any(y => !Enum.IsDefined(y)))))
		{ errors.Add("Explicit method selections must cover all three variants with distinct valid native methods."); return; }
		foreach (var variant in Variants)
			foreach (var kind in Methods(plan, variant))
				if (!methods.Any(x => (string?)x.Attribute("kind") == kind.ToString())) errors.Add($"{variant}: selected gathering kind {kind} absent from template. Supply a real method or explicitly select the intended subset.");
		if (errors.Count == 0) ValidateNativeGathering(db, template, compiler, progs, errors);
	}
}

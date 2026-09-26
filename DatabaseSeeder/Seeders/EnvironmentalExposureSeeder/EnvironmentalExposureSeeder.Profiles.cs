using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Form.Material;
using MudSharp.Health;
using MudSharp.Models;

#nullable enable

namespace DatabaseSeeder.Seeders;

public partial class EnvironmentalExposureSeeder
{
	public sealed record HazardProfile(string Name, bool Gas, bool Fantasy, string Category, double? MinimumTemperature = null, bool Persistent = false);
	public static IReadOnlyList<HazardProfile> Profiles { get; } = new HazardProfile[]
	{
		new("lava", false, false, "heat", Persistent: true), new("scalding water", false, false, "heat", 70),
		new("dilute cleaning acid", false, false, "chemical"), new("hydrochloric acid", false, false, "chemical"),
		new("sulfuric acid", false, false, "chemical"), new("hydrofluoric acid", false, false, "chemical"),
		new("sodium hydroxide solution", false, false, "chemical"),
		new("Chlorine", true, false, "chemical"), new("Ammonia", true, false, "chemical"),
		new("Sulfur Dioxide", true, false, "chemical"), new("Steam", true, false, "heat", 70),
		new("hot volcanic fumes", true, false, "heat", 70),
		new("infernal lava", false, true, "heat", Persistent: true), new("alchemical universal solvent", false, true, "chemical"),
		new("glass-eating ooze acid", false, true, "chemical"), new("spectral miasma", true, true, "spectral"),
		new("sacred water", false, true, "holy")
	};

	public sealed record FamilyResponse(double Damage, double ConsumptionLitres, bool Exclusion);
	/// <summary>Authored reference-second rates. The audit distinguishes these from qualitative source evidence.</summary>
	public static FamilyResponse Response(HazardProfile profile, ExposureMaterialFamily family, ExposureRoute route = ExposureRoute.GasContact)
	{
		var tissue = family is ExposureMaterialFamily.Tissue;
		var soft = family is ExposureMaterialFamily.Tissue or ExposureMaterialFamily.Leather or ExposureMaterialFamily.AnimalFibre or ExposureMaterialFamily.PlantFibre or ExposureMaterialFamily.WoodPaper or ExposureMaterialFamily.Food;
		var metal = family is ExposureMaterialFamily.Ferrous or ExposureMaterialFamily.ReactiveMetal or ExposureMaterialFamily.CopperAlloy;
		double damage = profile.Name switch
		{
			"lava" => tissue ? 12 : soft || family is ExposureMaterialFamily.Elastomer or ExposureMaterialFamily.OtherPolymer or ExposureMaterialFamily.ResistantPolymer or ExposureMaterialFamily.FuelWax ? 4 : 0.3,
			"infernal lava" => tissue ? 18 : 6,
			"scalding water" or "Steam" or "hot volcanic fumes" => tissue ? 2 : soft ? 0.1 : 0,
			"dilute cleaning acid" => tissue ? 0.05 : family == ExposureMaterialFamily.Carbonate ? 0.15 : metal ? 0.02 : 0,
			"hydrochloric acid" => tissue ? 2 : family is ExposureMaterialFamily.Carbonate or ExposureMaterialFamily.BoneHorn or ExposureMaterialFamily.ReactiveMetal ? 1 : family == ExposureMaterialFamily.Ferrous ? 0.5 : soft ? 0.2 : 0,
			"sulfuric acid" => tissue ? 3 : soft ? 1 : metal || family is ExposureMaterialFamily.Carbonate or ExposureMaterialFamily.BoneHorn ? 0.5 : family is ExposureMaterialFamily.OtherPolymer or ExposureMaterialFamily.Elastomer ? 0.25 : 0,
			"hydrofluoric acid" => tissue ? 4 : family is ExposureMaterialFamily.GlassCeramic or ExposureMaterialFamily.SilicateStone or ExposureMaterialFamily.Carbonate or ExposureMaterialFamily.BoneHorn ? 1.5 : metal ? 0.3 : 0,
			"sodium hydroxide solution" => tissue ? 2 : family is ExposureMaterialFamily.AnimalFibre or ExposureMaterialFamily.Leather or ExposureMaterialFamily.ReactiveMetal ? 1 : family is ExposureMaterialFamily.OtherPolymer ? 0.15 : 0,
			"Chlorine" => tissue ? 0.6 : metal ? 0.03 : family is ExposureMaterialFamily.PlantFibre or ExposureMaterialFamily.Elastomer ? 0.02 : 0,
			"Ammonia" => tissue ? 0.2 : family == ExposureMaterialFamily.CopperAlloy ? 0.02 : 0,
			"Sulfur Dioxide" => tissue ? 0.15 : 0,
			"alchemical universal solvent" => family is ExposureMaterialFamily.NobleMetal or ExposureMaterialFamily.ResistantPolymer ? 0 : 4,
			"glass-eating ooze acid" => family == ExposureMaterialFamily.GlassCeramic ? 5 : 0,
			"spectral miasma" => tissue ? 1.5 : 0,
			"sacred water" => family == ExposureMaterialFamily.Accursed ? 5 : 0,
			_ => 0
		};
		// Respiratory tissue has its own authored rates. Existing organ armour still applies;
		// external irritation rates are below its flat dissipation floor in stock anatomies.
		if (profile.Gas && tissue && route == ExposureRoute.Inhalation)
			damage = profile.Name switch
			{
				"Chlorine" => 18,
				"Ammonia" => 9,
				"Sulfur Dioxide" => 6,
				"Steam" or "hot volcanic fumes" => 18,
				"spectral miasma" => 21,
				_ => damage
			};
		// Unknown chemistry remains unassigned. Explicit fantasy rules may intentionally target their own authored family only.
		if (family == ExposureMaterialFamily.Uncertain) damage = 0;
		var consuming = !profile.Gas && profile.Category is "chemical" or "holy" && damage > 0;
		return new(damage, consuming ? family == ExposureMaterialFamily.ReactiveMetal ? 0.002 : 0.0005 : 0, damage == 0);
	}

	private void InstallProfiles(bool fantasy)
	{
		var tags = _context.Tags.ToDictionary(x => x.Id);
		var materials = _context.Materials.Include(x => x.MaterialsTags).OrderBy(x => x.Id).ToList();
		foreach (var profile in Profiles.Where(x => x.Fantasy == fantasy))
		{
			var liquid = profile.Gas ? null : Liquid(profile.Name, fantasy ? "fantasy" : "natural");
			var gas = profile.Gas ? Gas(profile.Name, fantasy ? "fantasy" : "natural") : null;
			if (liquid is null && gas is null) continue;
			var rules = new List<XElement>();
			foreach (var material in materials)
			{
				var family = ExposureCatalogue.Classify(material.Name, ExposureCatalogue.TagPaths(material.MaterialsTags.Select(x => x.TagId), tags));
				var response = Response(profile, family);
				if (family == ExposureMaterialFamily.Uncertain) continue;
				var routes = profile.Gas ? ExposureRoute.GasContact | ExposureRoute.Inhalation : ExposureRoute.LiquidContact;
				var rule = new XElement("Reaction", new XAttribute("Version", 2), new XAttribute("Id", StableId(profile.Name + ":" + material.Id)),
					new XAttribute("Name", profile.Name + " on " + material.Name), new XAttribute("Routes", (int)routes),
					new XAttribute("Material", material.Id), new XAttribute("Channel", profile.Category == "heat" ? "thermal" : profile.Category),
					new XAttribute("Category", profile.Category), new XAttribute("Priority", 0), new XAttribute("NoReaction", response.Exclusion),
					new XAttribute("DamageType", (int)(profile.Category == "heat" ? DamageType.Burning : DamageType.Chemical)),
					new XAttribute("DamageRate", response.Damage), new XAttribute("PainRate", response.Damage), new XAttribute("StunRate", 0),
					new XAttribute("Consumption", response.ConsumptionLitres > 0 ? 1 : 0), new XAttribute("ConsumptionRate", response.ConsumptionLitres / _litresPerBase),
					profile.MinimumTemperature is { } min ? new XAttribute("MinimumTemperature", min) : null, new XElement("Tags"));
				rules.Add(rule);
				if (profile.Gas && family == ExposureMaterialFamily.Tissue)
				{
					rule.SetAttributeValue("Routes", (int)ExposureRoute.GasContact);
					var inhaled = new XElement(rule);
					var respiratory = Response(profile, family, ExposureRoute.Inhalation);
					inhaled.SetAttributeValue("Id", StableId(profile.Name + ":" + material.Id + ":inhalation"));
					inhaled.SetAttributeValue("Name", profile.Name + " inhaled by " + material.Name);
					inhaled.SetAttributeValue("Routes", (int)ExposureRoute.Inhalation);
					inhaled.SetAttributeValue("DamageRate", respiratory.Damage);
					inhaled.SetAttributeValue("PainRate", respiratory.Damage);
					rules.Add(inhaled);
				}
				if (profile.Name == "sacred water" && family == ExposureMaterialFamily.Accursed)
				{
					var accursed = tags.Values.SingleOrDefault(x => x.Name == "Exposure Accursed");
					if (accursed is not null) rule.Element("Tags")!.Add(new XElement("Tag", accursed.Id));
				}
			}
			if (liquid is not null) ReconcileRules("Liquid", liquid.Id, profile.Name, liquid.SurfaceReactionInfo, rules, profile.Persistent, xml => liquid.SurfaceReactionInfo = xml);
			else ReconcileRules("Gas", gas!.Id, profile.Name, gas.SurfaceReactionInfo, rules, false, xml => gas.SurfaceReactionInfo = xml);
		}
	}

	private void ReconcileRules(string type, long id, string key, string? currentXml, IEnumerable<XElement> desiredRules, bool persistent, Action<string> apply)
	{
		XElement root;
		try { root = string.IsNullOrWhiteSpace(currentXml) ? new XElement("Reactions") : XElement.Parse(currentXml); }
		catch (System.Xml.XmlException) { _notes.Add($"Preserved malformed {type} reaction XML: {key}. Repair it before rerunning."); return; }
		var desired = desiredRules.ToDictionary(x => "rule:" + x.Attribute("Id")!.Value, x => x.ToString(SaveOptions.DisableFormatting));
		if (type == "Liquid") desired["persistence"] = persistent ? "UntilRemoved" : "OrdinaryDrying";
		var record = Record(type + "Reactions", "profile:" + key, id, "reactions");
		var previous = record.SeedBaseline is null ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline)!;
		if (type == "Gas")
		{
			// An edited combined rule still owns its inhalation route. Do not introduce a
			// competing stock rule or silently override the builder when splitting defaults.
			foreach (var existing in root.Elements("Reaction"))
			{
				var existingKey = "rule:" + (string?)existing.Attribute("Id");
				if (!desired.TryGetValue(existingKey, out var nextXml) ||
					((ExposureRoute)((int?)existing.Attribute("Routes") ?? 0) & ExposureRoute.Inhalation) == 0 ||
					(ExposureRoute)((int?)XElement.Parse(nextXml).Attribute("Routes") ?? 0) != ExposureRoute.GasContact ||
					previous.TryGetValue(existingKey, out var baseline) && baseline == existing.ToString(SaveOptions.DisableFormatting)) continue;
				var respiratoryKey = "rule:" + StableId(key + ":" + (long?)existing.Attribute("Material") + ":inhalation");
				if (!desired.Remove(respiratoryKey)) continue;
				_notes.Add($"Preserved customised combined gas rule on {key}; deferred its separate inhalation rule for material {(long?)existing.Attribute("Material")}. Resolve the route split explicitly in the builder.");
				var separate = root.Elements("Reaction").FirstOrDefault(x => "rule:" + (string?)x.Attribute("Id") == respiratoryKey);
				if (separate is not null && (!previous.TryGetValue(respiratoryKey, out var oldSeparate) || oldSeparate != separate.ToString(SaveOptions.DisableFormatting)))
					_notes.Add($"Conflicting customised inhalation rules on {key}, material {(long?)existing.Attribute("Material")}: both builder definitions are preserved. Overlapping equal-priority rules remain inactive until the builder resolves their routes or priorities.");
			}
		}
		var ownedKeys = desired.Keys.Union(previous.Keys).ToHashSet();
		var current = new Dictionary<string, string>();
		foreach (var rule in root.Elements("Reaction"))
		{
			var ruleKey = "rule:" + (string?)rule.Attribute("Id");
			if (!ownedKeys.Contains(ruleKey)) continue;
			if (!current.TryAdd(ruleKey, rule.ToString(SaveOptions.DisableFormatting))) { _notes.Add($"Duplicate reaction identity on {key}; preserved for builder repair."); return; }
		}
		if (type == "Liquid" && root.Attribute("Persistence") is { } persistence) current["persistence"] = persistence.Value;
		record.SeedBaseline ??= "{}"; // Missing newly-owned keys are additive; existing same-ID definitions are protected.
		var merged = SeederManagedRecordReconciler.Reconcile(record, current, desired, false, _notes);
		foreach (var rule in root.Elements("Reaction").Where(x => ownedKeys.Contains("rule:" + (string?)x.Attribute("Id"))).ToArray()) rule.Remove();
		foreach (var pair in merged.Where(x => x.Key.StartsWith("rule:", StringComparison.Ordinal)).OrderBy(x => x.Key)) root.Add(XElement.Parse(pair.Value));
		if (merged.TryGetValue("persistence", out var retained)) root.SetAttributeValue("Persistence", retained);
		apply(root.ToString());
	}

	private void InstallMaterialResponses()
	{
		var tags = _context.Tags.ToDictionary(x => x.Id);
		foreach (var material in _context.Materials.Include(x => x.MaterialsTags).ToList())
		{
			if (material.Name.StartsWith("exposure ", StringComparison.OrdinalIgnoreCase)) continue;
			var family = ExposureCatalogue.Classify(material.Name, ExposureCatalogue.TagPaths(material.MaterialsTags.Select(x => x.TagId), tags));
			if (family == ExposureMaterialFamily.Uncertain) continue;
			var permeable = family is ExposureMaterialFamily.Tissue or ExposureMaterialFamily.AnimalFibre or ExposureMaterialFamily.PlantFibre or ExposureMaterialFamily.Food;
			var desired = new MaterialExposureProperties
			{
				LiquidTransmission = permeable ? 1 : family is ExposureMaterialFamily.Leather or ExposureMaterialFamily.WoodPaper ? 0.2 : 0,
				GasTransmission = permeable ? 1 : family is ExposureMaterialFamily.Leather or ExposureMaterialFamily.WoodPaper ? 0.5 : 0,
				ThermalTransmission = family is ExposureMaterialFamily.AnimalFibre or ExposureMaterialFamily.PlantFibre ? 0.7 : 1,
				SoakPerSecond = permeable ? 0.05 : 0.005, ThermalSlope = family == ExposureMaterialFamily.Tissue ? 0.2 : 0.02,
				ThermalCap = family == ExposureMaterialFamily.Tissue ? 12 : 4
			};
			Overlay("MaterialExposure", material.Id, "material:" + material.Id, material.ExposureInfo, desired.Save(), string.IsNullOrWhiteSpace(material.ExposureInfo), xml => material.ExposureInfo = xml);
			if (material.HeatDamagePoint == 412.0389 && material.Name is "fatty flesh" or "flesh" or "muscly flesh" or "bony flesh" or "dense bony flesh" or "viscera" or "compact bone" or "spongy bone")
				material.HeatDamagePoint = family == ExposureMaterialFamily.Tissue ? 55 : 120;
		}
	}
}

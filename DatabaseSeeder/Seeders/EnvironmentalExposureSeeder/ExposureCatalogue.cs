using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

#nullable enable

namespace DatabaseSeeder.Seeders;

public enum ExposureMaterialFamily
{
	Tissue, Leather, AnimalFibre, PlantFibre, WoodPaper, BoneHorn, Carbonate, SilicateStone, GlassCeramic,
	Ferrous, ReactiveMetal, CopperAlloy, NobleMetal, ResistantPolymer, OtherPolymer, Elastomer, FuelWax, Food, Accursed, Uncertain
}

/// <summary>Qualitative compatibility groups, not a chemical or medical model. Unrecognised materials stay uncertain.</summary>
public static class ExposureCatalogue
{
	public static ExposureMaterialFamily Classify(string name, IEnumerable<string> tags)
	{
		var n = name.ToLowerInvariant();
		var paths = tags.Select(x => x.ToLowerInvariant()).ToArray();
		bool Tag(string text) => paths.Any(x => x.Contains(text, StringComparison.Ordinal));
		if (n == "exposure accursed bone") return ExposureMaterialFamily.Accursed;
		if (n == "exposure porous acid-resistant cloth") return ExposureMaterialFamily.ResistantPolymer;
		if (n == "exposure sealed susceptible rubber") return ExposureMaterialFamily.Elastomer;
		if (n == "exposure compatible storage polymer") return ExposureMaterialFamily.ResistantPolymer;
		if (n is "fatty flesh" or "flesh" or "muscly flesh" or "bony flesh" or "dense bony flesh" or "viscera" || Tag("animal skin") && !n.Contains("hide")) return ExposureMaterialFamily.Tissue;
		if (Tag("leather") || n.EndsWith(" hide") || n == "leather") return ExposureMaterialFamily.Leather;
		if (Tag("animal fiber") || Tag("hair") || n is "silk" or "silk gauze" or "wool" or "camelid wool") return ExposureMaterialFamily.AnimalFibre;
		if (n.Contains("bone") && !n.Contains("china") || Tag("horn")) return ExposureMaterialFamily.BoneHorn;
		if (Tag("shell") || n is "limestone" or "marble" or "chalk" or "calcite" or "dolomite" or "travertine" or "aragonite") return ExposureMaterialFamily.Carbonate;
		if (Tag("glass") || Tag("ceramic") || n is "glass" or "stoneware") return ExposureMaterialFamily.GlassCeramic;
		if (Tag("wood") || Tag("paper") || n is "paper" or "papyrus" or "parchment") return ExposureMaterialFamily.WoodPaper;
		if (Tag("natural fiber fabric") || Tag("fiber crop") || n is "cotton fibre" or "hemp fibre" or "flax fibre") return ExposureMaterialFamily.PlantFibre;
		if (Tag("manufactured metal") || n is "iron" or "steel" or "gold" or "silver" or "copper" or "aluminium" or "zinc" or "platinum")
		{
			if (n.Contains("gold") || n.Contains("silver") || n.Contains("platinum") || n is "palladium") return ExposureMaterialFamily.NobleMetal;
			if (n.Contains("steel") || n.Contains("iron")) return ExposureMaterialFamily.Ferrous;
			if (n.Contains("copper") || n.Contains("brass") || n.Contains("bronze")) return ExposureMaterialFamily.CopperAlloy;
			if (n is "aluminium" or "aluminum" or "zinc" or "magnesium" or "tin" or "lead") return ExposureMaterialFamily.ReactiveMetal;
			return ExposureMaterialFamily.Uncertain; // Neither an arbitrary alloy nor its era proves compatibility.
		}
		if (n is "ptfe" or "polypropylene" or "high-density polyethylene" or "low-density polyethylene") return ExposureMaterialFamily.ResistantPolymer;
		if (n.Contains("rubber")) return ExposureMaterialFamily.Elastomer;
		if (Tag("plastic") || Tag("synthetic fiber")) return ExposureMaterialFamily.OtherPolymer;
		if (Tag("wax") || n is "beeswax" or "paraffin wax" or "tallow" or "bitumen") return ExposureMaterialFamily.FuelWax;
		if (n is "basalt" or "granite" or "gabbro" or "quartz" or "quartzite" or "obsidian" or "slate" or "sandstone") return ExposureMaterialFamily.SilicateStone;
		if (Tag("food") || Tag("crop") || Tag("vegetation")) return ExposureMaterialFamily.Food;
		return ExposureMaterialFamily.Uncertain;
	}

	public static IReadOnlyList<string> TagPaths(IEnumerable<long> ids, IReadOnlyDictionary<long, Tag> tags)
	{
		string Path(long id, HashSet<long> visited)
		{
			if (!visited.Add(id) || !tags.TryGetValue(id, out var tag)) return $"missing-or-cyclic-tag:{id}";
			return tag.ParentId is { } parent ? Path(parent, visited) + " / " + tag.Name : tag.Name;
		}
		return ids.Select(id => Path(id, new())).Order(StringComparer.Ordinal).ToArray();
	}
}

public sealed record ExposureAuditRow(string Kind, long Id, string Name, string StableIdentifier, string Family,
	string Disposition, string Reason, IReadOnlyList<string> Tags, IReadOnlyList<string> Aliases, double? HeatThresholdCelsius,
	string? Transmission, string? Reactions, IReadOnlyList<string> Owners, int AnatomyReferences, long? DrugId,
	IReadOnlyList<string> SubstanceBindings);

public static class ExposureCatalogueAudit
{
	public static IReadOnlyList<ExposureAuditRow> Capture(FuturemudDatabaseContext context)
	{
		var tags = context.Tags.ToDictionary(x => x.Id);
		var ownership = context.SeederManagedRecords.ToList();
		var substances = context.MagicalSubstances.ToList();
		var bodyMaterials = context.BodypartProtos.AsEnumerable().GroupBy(x => x.DefaultMaterialId).ToDictionary(x => x.Key, x => x.Count());
		var races = context.Races.ToList();
		var breathing = context.RacesBreathableGases.AsEnumerable().GroupBy(x => x.GasId).ToDictionary(x => x.Key, x => x.Count());
		IReadOnlyList<string> Bindings(string kind, long id) => substances.Where(x =>
		{
			try { return XElement.Parse(x.Definition).Elements("Binding").Any(b => (int?)b.Attribute("carrier") == (kind == "liquid" ? 0 : 1) && (long?)b.Attribute("id") == id); }
			catch (System.Xml.XmlException) { return false; }
		}).Select(x => $"{x.Id}:{x.Name}").ToArray();
		IReadOnlyList<string> Owners(string type, long id) => ownership.Where(x => x.EntityType == type && x.LogicalId == id).Select(x => $"{x.Seeder}:{x.StableKey}").ToArray();
		var result = new List<ExposureAuditRow>();
		foreach (var material in context.Materials.Include(x => x.MaterialsTags).Include(x => x.MaterialAliases).ToList())
		{
			var paths = ExposureCatalogue.TagPaths(material.MaterialsTags.Select(x => x.TagId), tags);
			var family = ExposureCatalogue.Classify(material.Name, paths);
			result.Add(new("material", material.Id, material.Name, $"material:{material.Id}", family.ToString(),
				family == ExposureMaterialFamily.Uncertain ? "unsupported/uncertain" : string.IsNullOrWhiteSpace(material.ExposureInfo) ? "context-specific" : "covered by a family rule",
				family == ExposureMaterialFamily.Uncertain ? "No reviewed compatibility group; no new chemical rule inferred from Organic, metal ore, era or a fantasy name." : "Exact material rules derive from the documented qualitative family; authored game rates are not laboratory limits.",
				paths, material.MaterialAliases.Select(x => x.Alias).ToArray(), material.HeatDamagePoint, material.ExposureInfo, null,
				Owners("Material", material.Id), bodyMaterials.GetValueOrDefault(material.Id), null, Array.Empty<string>()));
		}
		foreach (var liquid in context.Liquids.Include(x => x.LiquidsTags).ToList())
		{
			var hazardous = HasRules(liquid.SurfaceReactionInfo);
			result.Add(new("liquid", liquid.Id, liquid.Name, $"liquid:{liquid.Id}", "fluid", hazardous ? "hazardous interaction assigned" : "intentionally non-hazardous",
				hazardous ? "Inspect reaction XML for routes, exact targets, exclusions, consumption and preserved v1 payload." : "No new contact injury. Culinary acids, brines, beverages, fuels and drug carriers retain their independent food/drug/fire behaviour.",
				ExposureCatalogue.TagPaths(liquid.LiquidsTags.Select(x => x.TagId), tags), Array.Empty<string>(), null, null, liquid.SurfaceReactionInfo,
				Owners("Liquid", liquid.Id), races.Count(x => x.BloodLiquidId == liquid.Id || x.SweatLiquidId == liquid.Id), liquid.DrugId, Bindings("liquid", liquid.Id)));
		}
		foreach (var gas in context.Gases.Include(x => x.GasesTags).ToList())
		{
			var hazardous = HasRules(gas.SurfaceReactionInfo);
			result.Add(new("gas", gas.Id, gas.Name, $"gas:{gas.Id}", "fluid", hazardous ? "hazardous interaction assigned" : "context-specific",
				hazardous ? "Contact and inhalation are independent of oxygen compatibility and existing drug delivery." : "No new contact injury assigned; breathability, asphyxiation and drug effects remain independent. A drug binding is not evidence of corrosiveness.",
				ExposureCatalogue.TagPaths(gas.GasesTags.Select(x => x.TagId), tags), Array.Empty<string>(), null, null, gas.SurfaceReactionInfo,
				Owners("Gas", gas.Id), breathing.GetValueOrDefault(gas.Id), gas.DrugId, Bindings("gas", gas.Id)));
		}
		return result;
	}
	private static bool HasRules(string? xml)
	{
		try { return !string.IsNullOrWhiteSpace(xml) && XElement.Parse(xml).Elements("Reaction").Any(); }
		catch (System.Xml.XmlException) { return true; } // Malformed definitions require review, never a safe-default claim.
	}
}

#nullable enable

using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class CoreDataSeeder
{
	private sealed record StockGasDefinition(
		string Name,
		string Description,
		double Density,
		double BoilingPoint,
		string DisplayColour,
		string SmellText,
		string VagueSmellText,
		double SmellIntensity,
		double OxidationFactor,
		string? CountAsName,
		ItemQuality? CountAsQuality,
		bool Organic,
		IReadOnlyCollection<string> Tags)
	{
		public double ThermalConductivity { get; init; } = 0.0257;
		public double ElectricalConductivity { get; init; } = 0.000005;
		public double SpecificHeatCapacity { get; init; } = 1.005;
		public double Viscosity { get; init; } = 15.0;
	}

	private static readonly (string Name, string? Parent)[] StockGasTagDefinitions =
	[
		("Gases", "Materials"),
		("Breathable Atmospheres", "Gases"),
		("Industrial Gases", "Gases"),
		("Elemental Gases", "Gases"),
		("Fuel Gases", "Industrial Gases"),
		("Medical Gases", "Industrial Gases"),
		("Noble Gases", "Elemental Gases"),
		("Refrigerant Gases", "Industrial Gases")
	];

	private static readonly IReadOnlyCollection<StockGasDefinition> StockGasDefinitions = BuildStockGasDefinitions();

	internal static IReadOnlyCollection<string> StockGasNamesForTesting =>
		StockGasDefinitions
			.Select(x => x.Name)
			.ToArray();

	internal static IReadOnlyCollection<string> StockBreathableAtmosphereGasNamesForTesting =>
		StockGasDefinitions
			.Where(x => x.Tags.Contains("Breathable Atmospheres", StringComparer.OrdinalIgnoreCase))
			.Select(x => x.Name)
			.ToArray();

	internal static IReadOnlyCollection<string> StockEconomicGasNamesForTesting =>
		StockGasDefinitions
			.Where(x => !x.Tags.Contains("Breathable Atmospheres", StringComparer.OrdinalIgnoreCase))
			.Select(x => x.Name)
			.ToArray();

	private static IReadOnlyCollection<StockGasDefinition> BuildStockGasDefinitions()
	{
		static StockGasDefinition Gas(
			string name,
			string description,
			double density,
			double boilingPoint,
			string colour = "cyan",
			string smell = "It has no smell",
			string? vagueSmell = null,
			double smellIntensity = 0.0,
			double oxidationFactor = 0.0,
			string? countAs = null,
			ItemQuality? countAsQuality = null,
			bool organic = false,
			params string[] tags)
		{
			return new StockGasDefinition(
				name,
				description,
				density,
				boilingPoint,
				colour,
				smell,
				vagueSmell ?? smell,
				smellIntensity,
				oxidationFactor,
				countAs,
				countAsQuality,
				organic,
				tags);
		}

		static StockGasDefinition Breathable(
			string name,
			string description,
			ItemQuality? countAsQuality,
			double density = 0.001205,
			double oxidationFactor = 1.0,
			string smell = "It has no smell",
			string? vagueSmell = null,
			double smellIntensity = 0.0)
		{
			return Gas(
				name,
				description,
				density,
				-200.0,
				"blue",
				smell,
				vagueSmell,
				smellIntensity,
				oxidationFactor,
				name.Equals("Breathable Atmosphere", StringComparison.OrdinalIgnoreCase) ? null : "Breathable Atmosphere",
				countAsQuality,
				tags: ["Breathable Atmospheres"]);
		}

		return
		[
			Breathable("Breathable Atmosphere", "Breathable Air", null),
			Breathable("Fresh Breathable Atmosphere", "clean, fresh breathable air", ItemQuality.Legendary,
				smell: "It smells clean and fresh", vagueSmell: "It smells fresh", smellIntensity: 0.5),
			Breathable("Filtered Breathable Atmosphere", "filtered breathable air", ItemQuality.Legendary),
			Breathable("Pressurized Breathable Atmosphere", "pressurized breathable air", ItemQuality.Legendary,
				density: 0.0015),
			Breathable("Humid Breathable Atmosphere", "humid breathable air", ItemQuality.Heroic,
				density: 0.00124, smell: "It smells faintly damp", vagueSmell: "It smells damp", smellIntensity: 0.5),
			Breathable("Hot Humid Breathable Atmosphere", "hot, humid breathable air", ItemQuality.Excellent,
				density: 0.00112, smell: "It smells warm and damp", vagueSmell: "It smells damp", smellIntensity: 0.7),
			Breathable("Cold Dry Breathable Atmosphere", "cold, dry breathable air", ItemQuality.Excellent,
				density: 0.00135),
			Breathable("Stale Breathable Atmosphere", "stale breathable air", ItemQuality.Great,
				smell: "It smells stale", vagueSmell: "It smells stale", smellIntensity: 1.5),
			Breathable("Polluted Breathable Atmosphere", "polluted but breathable air", ItemQuality.Good,
				smell: "It smells dirty and chemical", vagueSmell: "It smells polluted", smellIntensity: 2.5),
			Breathable("Smoke-Tainted Breathable Atmosphere", "smoke-tainted breathable air", ItemQuality.Substandard,
				smell: "It smells of smoke", vagueSmell: "It smells smoky", smellIntensity: 3.0),
			Breathable("Sulfurous Breathable Atmosphere", "sulfurous but breathable air", ItemQuality.Poor,
				smell: "It has a sharp sulfurous tang", vagueSmell: "It smells sulfurous", smellIntensity: 3.5),
			Breathable("High Altitude Breathable Atmosphere", "thin high-altitude breathable air", ItemQuality.Great,
				density: 0.0009),
			Breathable("Thin Breathable Atmosphere", "thin breathable air", ItemQuality.Good,
				density: 0.00065),
			Breathable("Very Thin Breathable Atmosphere", "very thin breathable air", ItemQuality.Standard,
				density: 0.0004),
			Breathable("Oxygen-Enriched Breathable Atmosphere", "oxygen-enriched breathable air", ItemQuality.Legendary,
				density: 0.00132, oxidationFactor: 1.4),
			Breathable("High-Oxygen Breathable Atmosphere", "high-oxygen breathable air", ItemQuality.Legendary,
				density: 0.00145, oxidationFactor: 2.0),

			Gas("Hydrogen", "hydrogen gas", 0.0000899, -252.9, "white", tags: ["Industrial Gases", "Elemental Gases", "Fuel Gases"]),
			Gas("Helium", "helium gas", 0.0001785, -268.9, "cyan", tags: ["Industrial Gases", "Elemental Gases", "Noble Gases"]),
			Gas("Nitrogen", "nitrogen gas", 0.001251, -195.8, "cyan", tags: ["Industrial Gases", "Elemental Gases"]),
			Gas("Oxygen", "oxygen gas", 0.001429, -183.0, "blue", oxidationFactor: 4.76, tags: ["Industrial Gases", "Elemental Gases", "Medical Gases"]),
			Gas("Fluorine", "fluorine gas", 0.001696, -188.1, "yellow", smell: "It has a sharp chemical smell", vagueSmell: "It smells sharply chemical", smellIntensity: 4.0, oxidationFactor: 3.0, tags: ["Industrial Gases", "Elemental Gases"]),
			Gas("Chlorine", "chlorine gas", 0.003214, -34.0, "green", smell: "It has a sharp bleach-like smell", vagueSmell: "It smells like bleach", smellIntensity: 5.0, oxidationFactor: 0.8, tags: ["Industrial Gases", "Elemental Gases"]),
			Gas("Neon", "neon gas", 0.000900, -246.1, "magenta", tags: ["Industrial Gases", "Elemental Gases", "Noble Gases"]),
			Gas("Argon", "argon gas", 0.001784, -185.8, "cyan", tags: ["Industrial Gases", "Elemental Gases", "Noble Gases"]),
			Gas("Krypton", "krypton gas", 0.003749, -153.4, "cyan", tags: ["Industrial Gases", "Elemental Gases", "Noble Gases"]),
			Gas("Xenon", "xenon gas", 0.005894, -108.1, "cyan", tags: ["Industrial Gases", "Elemental Gases", "Noble Gases"]),
			Gas("Ozone", "ozone gas", 0.002144, -112.0, "blue", smell: "It has a sharp clean smell", vagueSmell: "It smells sharp and clean", smellIntensity: 2.0, oxidationFactor: 2.0, tags: ["Industrial Gases"]),

			Gas("Carbon Dioxide", "carbon dioxide gas", 0.001977, -78.5, "white", tags: ["Industrial Gases"]),
			Gas("Carbon Monoxide", "carbon monoxide gas", 0.001145, -191.5, "white", tags: ["Industrial Gases", "Fuel Gases"]),
			Gas("Methane", "methane gas", 0.000657, -161.5, "yellow", organic: true, tags: ["Industrial Gases", "Fuel Gases"]),
			Gas("Natural Gas", "natural gas", 0.00075, -161.5, "yellow", smell: "It has a faint sulfurous odorant smell", vagueSmell: "It smells faintly sulfurous", smellIntensity: 2.0, countAs: "Methane", countAsQuality: ItemQuality.Good, organic: true, tags: ["Industrial Gases", "Fuel Gases"]),
			Gas("Ethane", "ethane gas", 0.001356, -88.6, "yellow", organic: true, tags: ["Industrial Gases", "Fuel Gases"]),
			Gas("Propane", "propane gas", 0.001882, -42.1, "yellow", smell: "It has a faint fuel odorant smell", vagueSmell: "It smells faintly of fuel", smellIntensity: 2.0, organic: true, tags: ["Industrial Gases", "Fuel Gases"]),
			Gas("Butane", "butane gas", 0.002489, -0.5, "yellow", smell: "It has a faint fuel odorant smell", vagueSmell: "It smells faintly of fuel", smellIntensity: 2.0, organic: true, tags: ["Industrial Gases", "Fuel Gases"]),
			Gas("Isobutane", "isobutane gas", 0.00251, -11.7, "yellow", countAs: "Butane", countAsQuality: ItemQuality.Great, organic: true, tags: ["Industrial Gases", "Fuel Gases", "Refrigerant Gases"]),
			Gas("Liquefied Petroleum Gas", "liquefied petroleum gas vapour", 0.0021, -42.0, "yellow", smell: "It has a fuel odorant smell", vagueSmell: "It smells of fuel", smellIntensity: 2.5, countAs: "Propane", countAsQuality: ItemQuality.Good, organic: true, tags: ["Industrial Gases", "Fuel Gases"]),
			Gas("Acetylene", "acetylene gas", 0.001097, -84.0, "yellow", smell: "It has a faint garlic-like smell", vagueSmell: "It smells faintly garlicky", smellIntensity: 2.0, organic: true, tags: ["Industrial Gases", "Fuel Gases"]),
			Gas("Ethylene", "ethylene gas", 0.001178, -103.7, "yellow", organic: true, tags: ["Industrial Gases", "Fuel Gases"]),
			Gas("Propylene", "propylene gas", 0.00181, -47.6, "yellow", organic: true, tags: ["Industrial Gases", "Fuel Gases"]),
			Gas("Dimethyl Ether", "dimethyl ether gas", 0.001918, -24.8, "yellow", smell: "It has a faint ether-like smell", vagueSmell: "It smells faintly ether-like", smellIntensity: 1.5, organic: true, tags: ["Industrial Gases", "Fuel Gases", "Refrigerant Gases"]),

			Gas("Ammonia", "ammonia gas", 0.000769, -33.3, "green", smell: "It has a sharp ammonia smell", vagueSmell: "It smells sharply alkaline", smellIntensity: 5.0, tags: ["Industrial Gases"]),
			Gas("Nitrous Oxide", "nitrous oxide gas", 0.001978, -88.5, "blue", oxidationFactor: 1.4, tags: ["Industrial Gases", "Medical Gases"]),
			Gas("Nitric Oxide", "nitric oxide gas", 0.00134, -151.8, "green", oxidationFactor: 0.5, tags: ["Industrial Gases"]),
			Gas("Nitrogen Dioxide", "nitrogen dioxide gas", 0.00205, 21.2, "red", smell: "It has a sharp acrid smell", vagueSmell: "It smells acrid", smellIntensity: 4.0, oxidationFactor: 0.7, tags: ["Industrial Gases"]),
			Gas("Sulfur Dioxide", "sulfur dioxide gas", 0.002927, -10.0, "green", smell: "It smells like burnt matches", vagueSmell: "It smells sharply sulfurous", smellIntensity: 4.0, tags: ["Industrial Gases"]),
			Gas("Hydrogen Sulfide", "hydrogen sulfide gas", 0.001539, -60.3, "green", smell: "It smells like rotten eggs", vagueSmell: "It smells rotten", smellIntensity: 8.0, tags: ["Industrial Gases"]),
			Gas("Hydrogen Chloride", "hydrogen chloride gas", 0.00149, -85.1, "green", smell: "It has a sharp acidic smell", vagueSmell: "It smells acidic", smellIntensity: 4.0, tags: ["Industrial Gases"]),
			Gas("Hydrogen Fluoride", "hydrogen fluoride gas", 0.000922, 19.5, "green", smell: "It has a sharp acidic smell", vagueSmell: "It smells acidic", smellIntensity: 4.0, tags: ["Industrial Gases"]),
			Gas("Hydrogen Cyanide", "hydrogen cyanide gas", 0.000687, 25.6, "green", smell: "It has a faint bitter almond smell", vagueSmell: "It smells faintly bitter", smellIntensity: 1.5, organic: true, tags: ["Industrial Gases"]),
			Gas("Sulfur Hexafluoride", "sulfur hexafluoride gas", 0.00617, -63.8, "cyan", tags: ["Industrial Gases"]),
			Gas("Silane", "silane gas", 0.00144, -112.0, "yellow", organic: false, tags: ["Industrial Gases"]),
			Gas("Phosphine", "phosphine gas", 0.00153, -87.7, "green", smell: "It has a garlic-like smell", vagueSmell: "It smells garlicky", smellIntensity: 3.0, tags: ["Industrial Gases"]),
			Gas("Chlorine Dioxide", "chlorine dioxide gas", 0.00309, 11.0, "green", smell: "It has a sharp chlorine smell", vagueSmell: "It smells like chlorine", smellIntensity: 5.0, oxidationFactor: 1.0, tags: ["Industrial Gases"]),
			Gas("Phosgene", "phosgene gas", 0.00425, 8.2, "green", smell: "It smells faintly of musty hay", vagueSmell: "It smells musty", smellIntensity: 2.0, organic: true, tags: ["Industrial Gases"]),

			Gas("Steam", "water vapour", 0.000598, 100.0, "white", tags: ["Industrial Gases"]),
			Gas("Methyl Chloride", "methyl chloride gas", 0.00222, -24.2, "green", smell: "It has a faint sweet smell", vagueSmell: "It smells faintly sweet", smellIntensity: 1.5, organic: true, tags: ["Industrial Gases", "Refrigerant Gases"]),
			Gas("Methyl Bromide", "methyl bromide gas", 0.00397, 3.6, "green", smell: "It has a faint sweet smell", vagueSmell: "It smells faintly sweet", smellIntensity: 1.5, organic: true, tags: ["Industrial Gases"]),
			Gas("Refrigerant R-134a", "R-134a refrigerant gas", 0.00425, -26.3, "cyan", organic: true, tags: ["Industrial Gases", "Refrigerant Gases"]),
			Gas("Difluoromethane", "difluoromethane gas", 0.00228, -51.7, "cyan", organic: true, tags: ["Industrial Gases", "Refrigerant Gases"]),
			Gas("Chlorodifluoromethane", "chlorodifluoromethane gas", 0.00366, -40.8, "cyan", organic: true, tags: ["Industrial Gases", "Refrigerant Gases"]),
			Gas("Tetrafluoromethane", "tetrafluoromethane gas", 0.00372, -128.0, "cyan", organic: true, tags: ["Industrial Gases", "Refrigerant Gases"]),
			Gas("Vinyl Chloride", "vinyl chloride gas", 0.00267, -13.4, "yellow", smell: "It has a faint sweet smell", vagueSmell: "It smells faintly sweet", smellIntensity: 1.5, organic: true, tags: ["Industrial Gases", "Fuel Gases"]),
			Gas("Formaldehyde", "formaldehyde gas", 0.000815, -19.0, "green", smell: "It has a sharp resinous smell", vagueSmell: "It smells sharply resinous", smellIntensity: 4.0, organic: true, tags: ["Industrial Gases"]),
			Gas("Landfill Gas", "landfill gas", 0.0010, -160.0, "yellow", smell: "It smells rotten and sulfurous", vagueSmell: "It smells rotten", smellIntensity: 5.0, countAs: "Methane", countAsQuality: ItemQuality.Substandard, organic: true, tags: ["Industrial Gases", "Fuel Gases"]),
			Gas("Biogas", "biogas", 0.0011, -160.0, "yellow", smell: "It smells earthy and sulfurous", vagueSmell: "It smells earthy", smellIntensity: 3.0, countAs: "Methane", countAsQuality: ItemQuality.Standard, organic: true, tags: ["Industrial Gases", "Fuel Gases"]),
			Gas("Synthesis Gas", "synthesis gas", 0.0010, -190.0, "yellow", countAs: "Hydrogen", countAsQuality: ItemQuality.Substandard, tags: ["Industrial Gases", "Fuel Gases"])
		];
	}

	internal static IReadOnlyDictionary<string, Gas> EnsureStockGases(FuturemudDatabaseContext context)
	{
		var tags = EnsureStockGasTags(context);
		var gases = context.Gases
			.AsEnumerable()
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
		var nextGasId = context.Gases
			.Select(x => x.Id)
			.AsEnumerable()
			.DefaultIfEmpty(0L)
			.Max() + 1L;

		foreach (var definition in StockGasDefinitions)
		{
			if (!gases.TryGetValue(definition.Name, out var gas))
			{
				gas = new Gas
				{
					Id = definition.Name.Equals("Breathable Atmosphere", StringComparison.OrdinalIgnoreCase) &&
					     !context.Gases.Any(x => x.Id == 1L)
						? 1L
						: nextGasId++
				};
				if (gas.Id >= nextGasId)
				{
					nextGasId = gas.Id + 1L;
				}

				context.Gases.Add(gas);
				gases[definition.Name] = gas;
			}

			gas.Name = definition.Name;
			gas.Description = definition.Description;
			gas.Density = definition.Density;
			gas.ThermalConductivity = definition.ThermalConductivity;
			gas.ElectricalConductivity = definition.ElectricalConductivity;
			gas.Organic = definition.Organic;
			gas.SpecificHeatCapacity = definition.SpecificHeatCapacity;
			gas.BoilingPoint = definition.BoilingPoint;
			gas.DisplayColour = definition.DisplayColour;
			gas.Viscosity = definition.Viscosity;
			gas.SmellIntensity = definition.SmellIntensity;
			gas.SmellText = definition.SmellText;
			gas.VagueSmellText = definition.VagueSmellText;
			gas.OxidationFactor = definition.OxidationFactor;
			gas.DrugId = null;
			gas.DrugGramsPerUnitVolume = 0.0;
			gas.PrecipitateId = null;
			gas.CountsAsQuality = definition.CountAsQuality is null ? null : (int)definition.CountAsQuality.Value;
		}

		foreach (var definition in StockGasDefinitions.Where(x => !string.IsNullOrWhiteSpace(x.CountAsName)))
		{
			gases[definition.Name].CountAsId = gases[definition.CountAsName!].Id;
		}

		foreach (var definition in StockGasDefinitions.Where(x => string.IsNullOrWhiteSpace(x.CountAsName)))
		{
			gases[definition.Name].CountAsId = null;
		}

		context.SaveChanges();

		foreach (var definition in StockGasDefinitions)
		{
			var gas = gases[definition.Name];
			var existingTagIds = context.GasesTags
				.Where(x => x.GasId == gas.Id)
				.Select(x => x.TagId)
				.ToHashSet();

			foreach (var tag in definition.Tags.Select(x => tags[x]).Where(x => x is not null))
			{
				if (existingTagIds.Contains(tag!.Id))
				{
					continue;
				}

				context.GasesTags.Add(new GasesTags
				{
					Gas = gas,
					GasId = gas.Id,
					Tag = tag,
					TagId = tag.Id
				});
			}
		}

		context.SaveChanges();
		return gases;
	}

	internal static Gas EnsureBreathableAtmosphere(FuturemudDatabaseContext context)
	{
		return EnsureStockGases(context)["Breathable Atmosphere"];
	}

	private static Dictionary<string, Tag?> EnsureStockGasTags(FuturemudDatabaseContext context)
	{
		var tags = context.Tags
			.AsEnumerable()
			.ToDictionary(x => x.Name, x => (Tag?)x, StringComparer.OrdinalIgnoreCase);

		foreach (var (name, parentName) in StockGasTagDefinitions)
		{
			if (tags.ContainsKey(name))
			{
				continue;
			}

			var tag = new Tag
			{
				Name = name,
				Parent = parentName is null || !tags.TryGetValue(parentName, out var parent) ? null : parent
			};
			context.Tags.Add(tag);
			tags[name] = tag;
		}

		context.SaveChanges();
		return tags;
	}
}

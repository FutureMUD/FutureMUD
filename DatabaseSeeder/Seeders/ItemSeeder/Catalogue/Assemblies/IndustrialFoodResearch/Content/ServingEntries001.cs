#nullable enable

namespace DatabaseSeeder.Seeders;

public static partial class IndustrialFoodResearchStock
{
	public static IndustrialisedFoodServingEntryRow ServingEntries0001() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 2,
			ServingReference: "industrial_food_serving_factory_breakfast",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_staples_001",
			Quantity: 300,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0002() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 3,
			ServingReference: "industrial_food_serving_factory_breakfast",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_dairy_egg_001",
			Quantity: 120,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0003() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 4,
			ServingReference: "industrial_food_serving_factory_breakfast",
			Order: 2,
			Kind: "liquid",
			StableReference: "industrialised_food_stimulant_001",
			Quantity: 300,
			Unit: "ml"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0004() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 5,
			ServingReference: "industrial_food_serving_mill_lunch",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_composite_001",
			Quantity: 450,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0005() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 6,
			ServingReference: "industrial_food_serving_mill_lunch",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_baking_001",
			Quantity: 120,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0006() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 7,
			ServingReference: "industrial_food_serving_mill_lunch",
			Order: 1,
			Kind: "liquid",
			StableReference: "industrialised_food_non_alcoholic_001",
			Quantity: 400,
			Unit: "ml"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0007() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 8,
			ServingReference: "industrial_food_serving_railway_refreshment",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_composite_002",
			Quantity: 320,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0008() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 9,
			ServingReference: "industrial_food_serving_railway_refreshment",
			Order: 2,
			Kind: "item",
			StableReference: "industrialised_food_dessert_001",
			Quantity: 90,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0009() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 10,
			ServingReference: "industrial_food_serving_railway_refreshment",
			Order: 2,
			Kind: "liquid",
			StableReference: "industrialised_food_stimulant_002",
			Quantity: 250,
			Unit: "ml"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0010() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 11,
			ServingReference: "industrial_food_serving_hospital_soft",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_composite_003",
			Quantity: 300,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0011() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 12,
			ServingReference: "industrial_food_serving_hospital_soft",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_infant_dietetic_001",
			Quantity: 180,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0012() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 13,
			ServingReference: "industrial_food_serving_hospital_soft",
			Order: 1,
			Kind: "liquid",
			StableReference: "industrialised_food_non_alcoholic_002",
			Quantity: 250,
			Unit: "ml"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0013() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 14,
			ServingReference: "industrial_food_serving_school_midday",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_composite_004",
			Quantity: 350,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0014() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 15,
			ServingReference: "industrial_food_serving_school_midday",
			Order: 2,
			Kind: "item",
			StableReference: "industrialised_food_fruit_nut_001",
			Quantity: 120,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0015() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 16,
			ServingReference: "industrial_food_serving_school_midday",
			Order: 1,
			Kind: "liquid",
			StableReference: "industrialised_food_non_alcoholic_003",
			Quantity: 250,
			Unit: "ml"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0016() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 17,
			ServingReference: "industrial_food_serving_prison_standard",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_staples_002",
			Quantity: 300,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0017() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 18,
			ServingReference: "industrial_food_serving_prison_standard",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_plant_001",
			Quantity: 250,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0018() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 19,
			ServingReference: "industrial_food_serving_prison_standard",
			Order: 1,
			Kind: "liquid",
			StableReference: "industrialised_food_non_alcoholic_004",
			Quantity: 350,
			Unit: "ml"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0019() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 20,
			ServingReference: "industrial_food_serving_naval_watch",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_ration_001",
			Quantity: 250,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0020() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 21,
			ServingReference: "industrial_food_serving_naval_watch",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_preserve_condiment_001",
			Quantity: 80,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0021() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 22,
			ServingReference: "industrial_food_serving_naval_watch",
			Order: 1,
			Kind: "liquid",
			StableReference: "industrialised_food_stimulant_003",
			Quantity: 300,
			Unit: "ml"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0022() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 23,
			ServingReference: "industrial_food_serving_mine_shift",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_composite_005",
			Quantity: 500,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0023() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 24,
			ServingReference: "industrial_food_serving_mine_shift",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_baking_002",
			Quantity: 150,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0024() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 25,
			ServingReference: "industrial_food_serving_mine_shift",
			Order: 1,
			Kind: "liquid",
			StableReference: "industrialised_food_non_alcoholic_005",
			Quantity: 500,
			Unit: "ml"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0025() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 26,
			ServingReference: "industrial_food_serving_relief_basic",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_ration_002",
			Quantity: 300,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0026() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 27,
			ServingReference: "industrial_food_serving_relief_basic",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_plant_002",
			Quantity: 180,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0027() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 28,
			ServingReference: "industrial_food_serving_relief_basic",
			Order: 1,
			Kind: "liquid",
			StableReference: "industrialised_food_non_alcoholic_006",
			Quantity: 500,
			Unit: "ml"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0028() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 29,
			ServingReference: "industrial_food_serving_religious_fast",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_plant_003",
			Quantity: 350,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0029() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 30,
			ServingReference: "industrial_food_serving_religious_fast",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_baking_003",
			Quantity: 120,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0030() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 31,
			ServingReference: "industrial_food_serving_religious_fast",
			Order: 2,
			Kind: "item",
			StableReference: "industrialised_food_fruit_nut_002",
			Quantity: 100,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0031() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 32,
			ServingReference: "industrial_food_serving_religious_fast",
			Order: 1,
			Kind: "liquid",
			StableReference: "industrialised_food_non_alcoholic_007",
			Quantity: 300,
			Unit: "ml"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0032() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 33,
			ServingReference: "industrial_food_serving_urban_cafe",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_baking_004",
			Quantity: 100,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0033() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 34,
			ServingReference: "industrial_food_serving_urban_cafe",
			Order: 1,
			Kind: "liquid",
			StableReference: "industrialised_food_stimulant_004",
			Quantity: 250,
			Unit: "ml"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0034() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 35,
			ServingReference: "industrial_food_serving_railway_worker",
			Order: 1,
			Kind: "item",
			StableReference: "industrial_food_ration_001",
			Quantity: 300,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0035() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 36,
			ServingReference: "industrial_food_serving_railway_worker",
			Order: 1,
			Kind: "item",
			StableReference: "industrialised_food_preserve_condiment_002",
			Quantity: 50,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0036() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 37,
			ServingReference: "industrial_food_serving_railway_worker",
			Order: 1,
			Kind: "liquid",
			StableReference: "industrialised_food_non_alcoholic_008",
			Quantity: 500,
			Unit: "ml"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0037() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 38,
			ServingReference: "modern_food_serving_airline_tray",
			Order: 1,
			Kind: "item",
			StableReference: "modern_food_composite_001",
			Quantity: 280,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0038() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 39,
			ServingReference: "modern_food_serving_airline_tray",
			Order: 1,
			Kind: "item",
			StableReference: "modern_food_plant_001",
			Quantity: 100,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0039() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 40,
			ServingReference: "modern_food_serving_airline_tray",
			Order: 2,
			Kind: "item",
			StableReference: "modern_food_dessert_001",
			Quantity: 70,
			Unit: "g"
		);

	public static IndustrialisedFoodServingEntryRow ServingEntries0040() =>
		new IndustrialisedFoodServingEntryRow(
			Source: "serving-entries.tsv",
			Line: 41,
			ServingReference: "modern_food_serving_airline_tray",
			Order: 1,
			Kind: "liquid",
			StableReference: "modern_food_non_alcoholic_001",
			Quantity: 180,
			Unit: "ml"
		);
}

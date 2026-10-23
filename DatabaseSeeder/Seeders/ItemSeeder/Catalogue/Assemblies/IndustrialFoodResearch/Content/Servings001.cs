#nullable enable

namespace DatabaseSeeder.Seeders;

public static partial class IndustrialFoodResearchStock
{
	public static IndustrialisedFoodServingRow Servings0001() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 2,
			StableReference: "industrial_food_serving_factory_breakfast",
			Admissions: [
				"industrial"
			],
			Context: "workplace",
			Name: "factory-shift breakfast",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0002() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 3,
			StableReference: "industrial_food_serving_mill_lunch",
			Admissions: [
				"industrial"
			],
			Context: "workplace",
			Name: "mill-worker midday meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0003() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 4,
			StableReference: "industrial_food_serving_railway_refreshment",
			Admissions: [
				"industrial"
			],
			Context: "transport",
			Name: "railway refreshment service",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0004() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 5,
			StableReference: "industrial_food_serving_hospital_soft",
			Admissions: [
				"industrial",
				"modern",
				"nuclear",
				"information"
			],
			Context: "hospital",
			Name: "hospital soft meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0005() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 6,
			StableReference: "industrial_food_serving_school_midday",
			Admissions: [
				"industrial",
				"modern",
				"nuclear",
				"information"
			],
			Context: "school",
			Name: "school midday meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0006() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 7,
			StableReference: "industrial_food_serving_prison_standard",
			Admissions: [
				"industrial",
				"modern",
				"nuclear",
				"information"
			],
			Context: "prison",
			Name: "standard prison meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0007() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 8,
			StableReference: "industrial_food_serving_naval_watch",
			Admissions: [
				"industrial",
				"modern",
				"nuclear",
				"information"
			],
			Context: "maritime",
			Name: "naval watch meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0008() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 9,
			StableReference: "industrial_food_serving_mine_shift",
			Admissions: [
				"industrial",
				"modern",
				"nuclear",
				"information"
			],
			Context: "workplace",
			Name: "mine-shift meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0009() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 10,
			StableReference: "industrial_food_serving_relief_basic",
			Admissions: [
				"industrial",
				"modern",
				"nuclear",
				"information"
			],
			Context: "emergency",
			Name: "basic relief meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0010() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 11,
			StableReference: "industrial_food_serving_religious_fast",
			Admissions: [
				"industrial",
				"modern",
				"nuclear",
				"information"
			],
			Context: "religious institution",
			Name: "fasting-day communal meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0011() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 12,
			StableReference: "industrial_food_serving_urban_cafe",
			Admissions: [
				"industrial",
				"modern",
				"nuclear",
				"information"
			],
			Context: "hospitality",
			Name: "urban cafe service",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0012() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 13,
			StableReference: "industrial_food_serving_railway_worker",
			Admissions: [
				"industrial"
			],
			Context: "transport",
			Name: "railway worker packed meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0013() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 14,
			StableReference: "modern_food_serving_airline_tray",
			Admissions: [
				"modern",
				"nuclear",
				"information"
			],
			Context: "aviation",
			Name: "early airline tray meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0014() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 15,
			StableReference: "modern_food_serving_office_lunch",
			Admissions: [
				"modern",
				"nuclear",
				"information"
			],
			Context: "workplace",
			Name: "office canteen lunch",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0015() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 16,
			StableReference: "modern_food_serving_drive_in",
			Admissions: [
				"modern",
				"nuclear",
				"information"
			],
			Context: "hospitality",
			Name: "roadside quick-service meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0016() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 17,
			StableReference: "modern_food_serving_canteen_vegetarian",
			Admissions: [
				"modern",
				"nuclear",
				"information"
			],
			Context: "institution",
			Name: "vegetarian canteen meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0017() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 18,
			StableReference: "modern_food_serving_migrant_household",
			Admissions: [
				"modern",
				"nuclear",
				"information"
			],
			Context: "household",
			Name: "migrant household supper",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0018() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 19,
			StableReference: "nuclear_food_serving_emergency_shelter",
			Admissions: [
				"nuclear",
				"information"
			],
			Context: "emergency",
			Name: "emergency shelter meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0019() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 20,
			StableReference: "nuclear_food_serving_factory_night",
			Admissions: [
				"nuclear",
				"information"
			],
			Context: "workplace",
			Name: "night-shift factory meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0020() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 21,
			StableReference: "nuclear_food_serving_frozen_dinner",
			Admissions: [
				"nuclear",
				"information"
			],
			Context: "household",
			Name: "convenience dinner service",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0021() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 22,
			StableReference: "information_food_serving_office_delivery",
			Admissions: [
				"information"
			],
			Context: "workplace",
			Name: "office delivery meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0022() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 23,
			StableReference: "information_food_serving_care_texture",
			Admissions: [
				"information"
			],
			Context: "care institution",
			Name: "texture-modified care meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0023() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 24,
			StableReference: "information_food_serving_field_response",
			Admissions: [
				"information"
			],
			Context: "emergency",
			Name: "field-response meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0024() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 25,
			StableReference: "information_food_serving_plant_cafe",
			Admissions: [
				"information"
			],
			Context: "hospitality",
			Name: "plant-focused cafe meal",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0025() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 26,
			StableReference: "industrialised_food_serving_livestock_feed",
			Admissions: [
				"industrial",
				"modern",
				"nuclear",
				"information"
			],
			Context: "animal feeding",
			Name: "working-horse feed issue",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);

	public static IndustrialisedFoodServingRow Servings0026() =>
		new IndustrialisedFoodServingRow(
			Source: "servings.tsv",
			Line: 27,
			StableReference: "modern_food_serving_pet_feed",
			Admissions: [
				"modern",
				"nuclear",
				"information"
			],
			Context: "animal feeding",
			Name: "household pet feeding",
			ReviewState: IndustrialisedFoodReviewState.DependencyReviewed
		);
}
